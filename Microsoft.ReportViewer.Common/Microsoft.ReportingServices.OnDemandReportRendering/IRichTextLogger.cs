using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IRichTextLogger
	{
		RSTrace Tracer
		{
			get;
		}

		void RegisterOutOfRangeSizeWarning(string propertyName, string value, string minVal, string maxVal);

		void RegisterInvalidValueWarning(string propertyName, string value, int charPosition);

		void RegisterInvalidColorWarning(string propertyName, string value, int charPosition);

		void RegisterInvalidSizeWarning(string propertyName, string value, int charPosition);
	}
}
