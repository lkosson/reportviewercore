using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class SubReport : PageItemContainer, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override byte RPLFormatType => 12;

		internal SubReport()
		{
		}

		internal SubReport(Microsoft.ReportingServices.OnDemandReportRendering.SubReport source)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
			base.KeepTogetherVertical = (base.UnresolvedKTV = source.KeepTogether);
			base.KeepTogetherHorizontal = (base.UnresolvedKTH = source.KeepTogether);
		}

		protected override void CreateChildren(PageContext pageContext)
		{
			ReportSectionCollection reportSections = (m_source as Microsoft.ReportingServices.OnDemandReportRendering.SubReport).Report.ReportSections;
			int count = reportSections.Count;
			m_children = new PageItem[count];
			m_indexesLeftToRight = new int[count];
			m_rightPadding = 0.0;
			m_bottomPadding = 0.0;
			double num = 0.0;
			for (int i = 0; i < count; i++)
			{
				ReportBody reportBody = new ReportBody(reportSections[i].Body, reportSections[i].Width);
				reportBody.CacheNonSharedProperties(pageContext);
				reportBody.ItemPageSizes.Top = num;
				m_indexesLeftToRight[i] = i;
				num += reportBody.ItemPageSizes.Height;
				if (i > 0)
				{
					List<int> list = new List<int>(1);
					list.Add(i - 1);
					reportBody.PageItemsAbove = list;
				}
				m_children[i] = reportBody;
			}
		}

		protected override void DetermineContentHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors, bool resolveState, bool resolveItem)
		{
			base.DetermineContentHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, ancestors, anyAncestorHasKT, hasUnpinnedAncestors, resolveState, resolveItem);
			if (m_children.Length <= 1)
			{
				return;
			}
			for (int i = 0; i < m_children.Length; i++)
			{
				ReportBody reportBody = m_children[i] as ReportBody;
				if (reportBody != null)
				{
					if (!reportBody.OnThisVerticalPage)
					{
						break;
					}
					if (reportBody.ItemPageSizes.Width < m_itemPageSizes.Width)
					{
						reportBody.ItemPageSizes.Width = m_itemPageSizes.Width;
					}
				}
			}
		}

		internal override void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (rplWriter != null && ((Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source).OmitBorderOnPageBreak)
			{
				OmitBorderOnPageBreak(pageLeft, pageTop, pageRight, pageBottom);
			}
		}

		internal override RPLElement CreateRPLElement()
		{
			return new RPLSubReport();
		}

		internal override RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext)
		{
			return new RPLSubReport(props as RPLItemProps);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)12);
					WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				}
				else if (m_rplElement == null)
				{
					m_rplElement = new RPLSubReport();
					WriteElementProps(m_rplElement.ElementProps, pageContext);
				}
				else
				{
					RPLItemProps rplElementProps = m_rplElement.ElementProps as RPLItemProps;
					m_rplElement = new RPLSubReport(rplElementProps);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source;
			if (subReport.ReportName != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(subReport.ReportName);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source;
			if (subReport.ReportName != null)
			{
				((RPLSubReportPropsDef)sharedProps).ReportName = subReport.ReportName;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = ((Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source).Report;
			if (report != null)
			{
				string reportLanguage = Report.GetReportLanguage(report);
				if (reportLanguage != null)
				{
					spbifWriter.Write((byte)11);
					spbifWriter.Write(reportLanguage);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = ((Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source).Report;
			if (report != null)
			{
				string reportLanguage = Report.GetReportLanguage(report);
				if (reportLanguage != null)
				{
					((RPLSubReportProps)nonSharedProps).Language = reportLanguage;
				}
			}
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			bool inSubReport = pageContext.Common.InSubReport;
			pageContext.Common.InSubReport = true;
			bool result = base.AddToPage(rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState);
			pageContext.Common.InSubReport = inSubReport;
			return result;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SubReport;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.SubReport, ObjectType.PageItemContainer, memberInfoList);
			}
			return m_declaration;
		}
	}
}
