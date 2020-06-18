using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Field
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		public string Name => m_fieldDef.Name;

		public string DataField => m_fieldDef.DataField;

		internal Field(Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			m_fieldDef = fieldDef;
		}
	}
}
