using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IShowHideSender
	{
		void ProcessSender(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext context, int uniqueName);
	}
}
