using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class SelectiveRendering
	{
		private sealed class ItemContext
		{
			internal RPLWriter RPLWriter
			{
				get;
				private set;
			}

			internal PageContext PageContext
			{
				get;
				private set;
			}

			internal Microsoft.ReportingServices.OnDemandReportRendering.Report Report
			{
				get;
				private set;
			}

			internal Microsoft.ReportingServices.OnDemandReportRendering.ReportSection ReportSection
			{
				get;
				private set;
			}

			internal ItemContext(RPLWriter rplWriter, PageContext pageContext, Microsoft.ReportingServices.OnDemandReportRendering.Report report, Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection)
			{
				RPLWriter = rplWriter;
				PageContext = pageContext;
				Report = report;
				ReportSection = reportSection;
			}
		}

		private abstract class ReportToRplWriterBase
		{
			protected readonly PageItem m_pageItem;

			private readonly ItemContext m_itemContext;

			protected PageContext PageContext => m_itemContext.PageContext;

			protected RPLWriter RplWriter => m_itemContext.RPLWriter;

			protected Microsoft.ReportingServices.OnDemandReportRendering.Report Report => m_itemContext.Report;

			protected Microsoft.ReportingServices.OnDemandReportRendering.ReportSection ReportSection => m_itemContext.ReportSection;

			protected float Width => (float)m_pageItem.ItemPageSizes.Width;

			protected float Height => (float)m_pageItem.ItemPageSizes.Height;

			protected ReportToRplWriterBase(PageItem pageItem, ItemContext itemContext)
			{
				m_pageItem = pageItem;
				m_itemContext = itemContext;
			}
		}

		private sealed class ReportToRplStreamWriter : ReportToRplWriterBase
		{
			private BinaryWriter m_spbifWriter;

			private ReportToRplStreamWriter(PageItem item, ItemContext itemContext)
				: base(item, itemContext)
			{
				m_spbifWriter = base.RplWriter.BinaryWriter;
			}

			internal static void Write(PageItem item, ItemContext itemContext)
			{
				new ReportToRplStreamWriter(item, itemContext).WriteImpl();
			}

			private void WriteImpl()
			{
				BinaryWriter binaryWriter = base.RplWriter.BinaryWriter;
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)2);
				WritePropertyToRplStream(15, base.Report.Name);
				WritePropertyToRplStream(9, base.Report.Description);
				WritePropertyToRplStream(13, base.Report.Author);
				WritePropertyToRplStream(11, Microsoft.ReportingServices.Rendering.HPBProcessing.Report.GetReportLanguage(base.Report));
				if (base.Report.AutoRefresh > 0)
				{
					WritePropertyToRplStream(14, base.Report.AutoRefresh);
				}
				binaryWriter.Write((byte)12);
				binaryWriter.Write(base.Report.ExecutionTime.ToBinary());
				if (base.Report.Location != null)
				{
					WritePropertyToRplStream(10, base.Report.Location.ToString());
				}
				binaryWriter.Write(byte.MaxValue);
				long value = WritePage();
				long position2 = baseStream.Position;
				binaryWriter.Write((byte)18);
				binaryWriter.Write(position);
				binaryWriter.Write(1);
				binaryWriter.Write(value);
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position2);
				binaryWriter.Write(byte.MaxValue);
			}

			private long WritePage()
			{
				Stream baseStream = m_spbifWriter.BaseStream;
				long position = baseStream.Position;
				m_spbifWriter.Write((byte)19);
				m_spbifWriter.Write((byte)3);
				WritePropertyToRplStream(17, base.Width);
				WritePropertyToRplStream(16, base.Height);
				m_spbifWriter.Write((byte)6);
				m_spbifWriter.Write((byte)0);
				m_spbifWriter.Write((byte)5);
				m_spbifWriter.Write((byte)0);
				m_spbifWriter.Write(byte.MaxValue);
				m_spbifWriter.Write(byte.MaxValue);
				long grandParentEndOffset = WriteReportSection();
				long position2 = baseStream.Position;
				WriteSingleMeasurement(position, grandParentEndOffset, base.Width, base.Height);
				long position3 = baseStream.Position;
				m_spbifWriter.Write((byte)254);
				m_spbifWriter.Write(position2);
				m_spbifWriter.Write(byte.MaxValue);
				return position3;
			}

			private long WriteReportSection()
			{
				Stream baseStream = m_spbifWriter.BaseStream;
				long position = baseStream.Position;
				m_spbifWriter.Write((byte)21);
				m_spbifWriter.Write((byte)22);
				WritePropertyToRplStream(0, base.ReportSection.ID);
				WritePropertyToRplStream(1, 1);
				m_spbifWriter.Write(byte.MaxValue);
				long grandParentEndOffset = WriteSingleColumn();
				long position2 = baseStream.Position;
				WriteSingleMeasurement(position, grandParentEndOffset, base.Width, base.Height);
				long position3 = baseStream.Position;
				m_spbifWriter.Write((byte)254);
				m_spbifWriter.Write(position2);
				m_spbifWriter.Write(byte.MaxValue);
				return position3;
			}

			private long WriteSingleColumn()
			{
				Stream baseStream = m_spbifWriter.BaseStream;
				long position = baseStream.Position;
				m_spbifWriter.Write((byte)20);
				long grandParentEndOffset = WriteReportBody();
				long position2 = baseStream.Position;
				WriteSingleMeasurement(position, grandParentEndOffset, base.Width, base.Height);
				long position3 = baseStream.Position;
				m_spbifWriter.Write((byte)254);
				m_spbifWriter.Write(position2);
				m_spbifWriter.Write(byte.MaxValue);
				return position3;
			}

			private long WriteReportBody()
			{
				long position = m_spbifWriter.BaseStream.Position;
				m_spbifWriter.Write((byte)6);
				m_spbifWriter.Write((byte)15);
				m_spbifWriter.Write((byte)0);
				WritePropertyToRplStream(1, base.ReportSection.Body.ID);
				WritePropertyToRplStream(0, base.ReportSection.Body.InstanceUniqueName);
				m_spbifWriter.Write(byte.MaxValue);
				m_spbifWriter.Write(byte.MaxValue);
				m_pageItem.WriteStartItemToStream(base.RplWriter, base.PageContext);
				long position2 = m_spbifWriter.BaseStream.Position;
				m_spbifWriter.Write((byte)16);
				m_spbifWriter.Write(position);
				m_spbifWriter.Write(1);
				m_pageItem.WritePageItemSizes(m_spbifWriter);
				long position3 = m_spbifWriter.BaseStream.Position;
				m_spbifWriter.Write((byte)254);
				m_spbifWriter.Write(position2);
				m_spbifWriter.Write(byte.MaxValue);
				return position3;
			}

			private void WritePropertyToRplStream(byte itemNameToken, string value)
			{
				if (value != null)
				{
					m_spbifWriter.Write(itemNameToken);
					m_spbifWriter.Write(value);
				}
			}

			private void WritePropertyToRplStream(byte itemNameToken, float value)
			{
				m_spbifWriter.Write(itemNameToken);
				m_spbifWriter.Write(value);
			}

			private void WritePropertyToRplStream(byte itemNameToken, int value)
			{
				m_spbifWriter.Write(itemNameToken);
				m_spbifWriter.Write(value);
			}

			private void WriteSingleMeasurement(long parentStartOffset, long grandParentEndOffset, float width, float height)
			{
				m_spbifWriter.Write((byte)16);
				m_spbifWriter.Write(parentStartOffset);
				m_spbifWriter.Write(1);
				m_spbifWriter.Write(0f);
				m_spbifWriter.Write(0f);
				m_spbifWriter.Write(width);
				m_spbifWriter.Write(height);
				m_spbifWriter.Write(0);
				m_spbifWriter.Write((byte)0);
				m_spbifWriter.Write(grandParentEndOffset);
			}
		}

		private sealed class ReportToRplOmWriter : ReportToRplWriterBase
		{
			private ReportToRplOmWriter(PageItem item, ItemContext itemContext)
				: base(item, itemContext)
			{
			}

			internal static void Write(PageItem item, ItemContext itemContext)
			{
				new ReportToRplOmWriter(item, itemContext).WriteImpl();
			}

			private void WriteImpl()
			{
				Version rPLVersion = new Version(10, 6, 0);
				RPLReport rPLReport = new RPLReport();
				rPLReport.ReportName = base.Report.Name;
				rPLReport.Description = base.Report.Description;
				rPLReport.Author = base.Report.Author;
				rPLReport.AutoRefresh = base.Report.AutoRefresh;
				rPLReport.ExecutionTime = base.Report.ExecutionTime;
				rPLReport.Location = base.Report.Location.ToString();
				rPLReport.Language = Microsoft.ReportingServices.Rendering.HPBProcessing.Report.GetReportLanguage(base.Report);
				rPLReport.RPLVersion = rPLVersion;
				rPLReport.RPLPaginatedPages = new RPLPageContent[1];
				base.RplWriter.Report = rPLReport;
				RPLReportSection rPLReportSection = new RPLReportSection(1);
				rPLReportSection.ID = base.ReportSection.ID;
				rPLReportSection.ColumnCount = 1;
				RPLBody rPLBody = new RPLBody();
				RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
				rPLItemMeasurement.Left = 0f;
				rPLItemMeasurement.Top = 0f;
				rPLItemMeasurement.Width = base.Width;
				rPLItemMeasurement.Height = base.Height;
				rPLItemMeasurement.ZIndex = 0;
				rPLItemMeasurement.State = 0;
				rPLItemMeasurement.Element = rPLBody;
				rPLReportSection.Columns[0] = rPLItemMeasurement;
				rPLReportSection.BodyArea = new RPLMeasurement();
				rPLReportSection.BodyArea.Top = 0f;
				rPLReportSection.BodyArea.Height = base.Height;
				m_pageItem.WriteStartItemToStream(base.RplWriter, base.PageContext);
				RPLItemMeasurement[] array2 = rPLBody.Children = new RPLItemMeasurement[1];
				array2[0] = m_pageItem.WritePageItemSizes();
				RPLPageLayout rPLPageLayout = new RPLPageLayout();
				rPLPageLayout.PageHeight = base.Height;
				rPLPageLayout.PageWidth = base.Width;
				rPLPageLayout.Style = new RPLElementStyle(null, null);
				RPLPageContent rPLPageContent = new RPLPageContent(1, rPLPageLayout);
				RPLMeasurement rPLMeasurement = new RPLMeasurement();
				rPLMeasurement.Left = 0f;
				rPLMeasurement.Top = 0f;
				rPLMeasurement.Width = base.Width;
				rPLMeasurement.Height = base.Height;
				rPLPageContent.ReportSectionSizes[0] = rPLMeasurement;
				rPLPageContent.AddReportSection(rPLReportSection);
				base.RplWriter.Report.RPLPaginatedPages[0] = rPLPageContent;
			}
		}

		private const char ReportItemPathSeparator = '/';

		private Microsoft.ReportingServices.OnDemandReportRendering.Report m_report;

		private PageContext m_pageContext;

		private PaginationSettings m_paginationSettings;

		internal bool Done
		{
			get;
			private set;
		}

		internal SelectiveRendering(Microsoft.ReportingServices.OnDemandReportRendering.Report report, PageContext pageContext, PaginationSettings paginationSettings)
		{
			m_report = report;
			m_pageContext = pageContext;
			m_paginationSettings = paginationSettings;
		}

		internal void RenderReportItem(RPLWriter rplWriter, string reportItemName)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection = null;
			ReportItem obj = FindReportItem(m_report, SplitReportItemPath(reportItemName), out reportSection) ?? throw new SelectiveRenderingCannotFindReportItemException(reportItemName);
			CustomReportItem criOwner = obj.CriOwner;
			if (criOwner != null)
			{
				criOwner.DynamicWidth = ReportSize.FromMillimeters(m_paginationSettings.PhysicalPageWidth);
				criOwner.DynamicHeight = ReportSize.FromMillimeters(m_paginationSettings.PhysicalPageHeight);
			}
			PageItem pageItem = PageItem.Create(obj, tablixCellParent: false, m_pageContext);
			pageItem.ItemPageSizes.Top = 0.0;
			pageItem.ItemPageSizes.Left = 0.0;
			pageItem.ItemPageSizes.Width = m_paginationSettings.PhysicalPageWidth;
			pageItem.ItemPageSizes.Height = m_paginationSettings.PhysicalPageHeight;
			ItemContext itemContext = new ItemContext(rplWriter, m_pageContext, m_report, reportSection);
			if (rplWriter.BinaryWriter != null)
			{
				ReportToRplStreamWriter.Write(pageItem, itemContext);
			}
			else
			{
				ReportToRplOmWriter.Write(pageItem, itemContext);
			}
			Done = true;
		}

		private static IEnumerable<string> SplitReportItemPath(string reportItemPath)
		{
			if (reportItemPath == null)
			{
				return Enumerable.Empty<string>();
			}
			return reportItemPath.Split('/');
		}

		private static ReportItem FindReportItem(Microsoft.ReportingServices.OnDemandReportRendering.Report report, IEnumerable<string> reportItemPathSteps, out Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection)
		{
			reportSection = null;
			int num = reportItemPathSteps.Count();
			if (num == 0)
			{
				return null;
			}
			bool flag = num > 1;
			string text = reportItemPathSteps.FirstOrDefault();
			ReportItem reportItem = null;
			foreach (Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection2 in report.ReportSections)
			{
				foreach (ReportItem item in reportSection2.Body.ReportItemCollection)
				{
					if (flag)
					{
						Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = item as Microsoft.ReportingServices.OnDemandReportRendering.SubReport;
						if (subReport != null && subReport.Report != null && string.CompareOrdinal(item.Name, text) == 0)
						{
							reportItem = FindReportItem(subReport.Report, reportItemPathSteps.Skip(1), out reportSection);
						}
					}
					else
					{
						Microsoft.ReportingServices.OnDemandReportRendering.Rectangle rectangle = item as Microsoft.ReportingServices.OnDemandReportRendering.Rectangle;
						if (rectangle != null)
						{
							reportItem = FindReportItem(rectangle, text);
						}
						else if (string.CompareOrdinal(item.Name, text) == 0)
						{
							reportItem = item;
						}
					}
					if (reportItem != null)
					{
						reportSection = reportSection2;
						return reportItem;
					}
				}
			}
			return null;
		}

		private static ReportItem FindReportItem(Microsoft.ReportingServices.OnDemandReportRendering.Rectangle container, string reportItemName)
		{
			foreach (ReportItem item in container.ReportItemCollection)
			{
				if (string.CompareOrdinal(item.Name, reportItemName) == 0)
				{
					return item;
				}
			}
			return null;
		}
	}
}
