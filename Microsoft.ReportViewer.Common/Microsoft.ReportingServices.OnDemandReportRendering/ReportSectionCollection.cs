using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSectionCollection : ReportElementCollectionBase<ReportSection>
	{
		private ReportSection[] m_sections;

		public override ReportSection this[int index]
		{
			get
			{
				if (0 > index || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_sections[index];
			}
		}

		public override int Count => m_sections.Length;

		internal ReportSectionCollection(Report reportDef)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection> reportSections = reportDef.ReportDef.ReportSections;
			m_sections = new ReportSection[reportSections.Count];
			for (int i = 0; i < m_sections.Length; i++)
			{
				m_sections[i] = new ReportSection(reportDef, reportSections[i], i);
			}
		}

		internal ReportSectionCollection(Report reportDef, Microsoft.ReportingServices.ReportRendering.Report renderReport)
		{
			m_sections = new ReportSection[1];
			m_sections[0] = new ReportSection(reportDef, renderReport, 0);
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < m_sections.Length; i++)
			{
				m_sections[i].SetNewContext();
			}
		}
	}
}
