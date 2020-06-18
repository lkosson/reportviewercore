using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Map : DynamicImage, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		protected override byte ElementToken => 21;

		protected override bool SpecialBorderHandling => ((Microsoft.ReportingServices.OnDemandReportRendering.Map)m_source).SpecialBorderHandling;

		internal Map()
		{
		}

		internal Map(Microsoft.ReportingServices.OnDemandReportRendering.Map source, PageContext pageContext)
			: base(source)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Map map = (Microsoft.ReportingServices.OnDemandReportRendering.Map)m_source;
			m_pageBreakProperties = PageBreakProperties.Create(map.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				m_pageName = map.Instance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && map.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLMap();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Map;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.Map, ObjectType.DynamicImage, memberInfoList);
			}
			return m_declaration;
		}
	}
}
