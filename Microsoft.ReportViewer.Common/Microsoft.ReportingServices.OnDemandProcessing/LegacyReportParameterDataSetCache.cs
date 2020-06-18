using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class LegacyReportParameterDataSetCache : ReportParameterDataSetCache
	{
		internal LegacyReportParameterDataSetCache(ProcessReportParameters aParamProcessor, ParameterInfo aParameter, ParameterDef aParamDef, bool aProcessValidValues, bool aProcessDefaultValues)
			: base(aParamProcessor, aParameter, aParamDef, aProcessValidValues, aProcessDefaultValues)
		{
		}

		internal override object GetFieldValue(object aRow, int col)
		{
			FieldImpl[] array = (FieldImpl[])aRow;
			if (array[col].IsMissing)
			{
				return null;
			}
			return array[col].Value;
		}
	}
}
