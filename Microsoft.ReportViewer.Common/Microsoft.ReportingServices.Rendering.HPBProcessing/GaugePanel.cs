using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class GaugePanel : DynamicImage, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		protected override byte ElementToken => 14;

		protected override bool SpecialBorderHandling => false;

		internal GaugePanel()
		{
		}

		internal GaugePanel(Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel source, PageContext pageContext)
			: base(source)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel gaugePanel = (Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel)m_source;
			m_pageBreakProperties = PageBreakProperties.Create(gaugePanel.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				m_pageName = gaugePanel.Instance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && gaugePanel.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLGaugePanel();
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
			return ObjectType.GaugePanel;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.GaugePanel, ObjectType.DynamicImage, memberInfoList);
			}
			return m_declaration;
		}
	}
}
