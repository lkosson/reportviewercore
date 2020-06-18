using Microsoft.ReportingServices.Rendering.SPBProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class InteractivityPaginationModuleFactory
	{
		internal static IInteractivityPaginationModule CreateInteractivityPaginationModule()
		{
			return new SPBInteractivityProcessing();
		}
	}
}
