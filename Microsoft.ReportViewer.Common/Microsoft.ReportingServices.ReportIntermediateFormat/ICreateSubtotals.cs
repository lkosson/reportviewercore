using Microsoft.ReportingServices.ReportPublishing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface ICreateSubtotals
	{
		void CreateAutomaticSubtotals(AutomaticSubtotalContext context);
	}
}
