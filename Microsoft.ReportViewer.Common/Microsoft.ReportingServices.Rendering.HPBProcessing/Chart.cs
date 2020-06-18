using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Chart : DynamicImage, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		private double m_dynamicWidth;

		private double m_dynamicHeight;

		public override int Size => base.Size + 16;

		protected override byte ElementToken => 11;

		protected override bool SpecialBorderHandling => ((Microsoft.ReportingServices.OnDemandReportRendering.Chart)m_source).SpecialBorderHandling;

		internal Chart()
		{
		}

		internal Chart(Microsoft.ReportingServices.OnDemandReportRendering.Chart source, PageContext pageContext)
			: base(source)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Chart chart = (Microsoft.ReportingServices.OnDemandReportRendering.Chart)m_source;
			ChartInstance chartInstance = (ChartInstance)m_source.Instance;
			m_pageBreakProperties = PageBreakProperties.Create(chart.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				m_pageName = chartInstance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && chart.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
			m_dynamicWidth = chartInstance.DynamicWidth.ToMillimeters();
			m_dynamicHeight = chartInstance.DynamicHeight.ToMillimeters();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DynamicWidth:
					writer.Write(m_dynamicWidth);
					break;
				case MemberName.DynamicHeight:
					writer.Write(m_dynamicHeight);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DynamicWidth:
					m_dynamicWidth = reader.ReadDouble();
					break;
				case MemberName.DynamicHeight:
					m_dynamicHeight = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Chart;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DynamicWidth, Token.Double));
				list.Add(new MemberInfo(MemberName.DynamicHeight, Token.Double));
				return new Declaration(ObjectType.Chart, ObjectType.DynamicImage, list);
			}
			return m_declaration;
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLChart();
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			m_itemPageSizes.AdjustWidthTo(m_dynamicWidth);
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			m_itemPageSizes.AdjustHeightTo(m_dynamicHeight);
		}
	}
}
