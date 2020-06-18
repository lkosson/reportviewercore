using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class OnDemandReportParameterDataSetCache : ReportParameterDataSetCache
	{
		internal OnDemandReportParameterDataSetCache(ProcessReportParameters aParamProcessor, ParameterInfo aParameter, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef aParamDef, bool aProcessValidValues, bool aProcessDefaultValues)
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
