using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IReportInstanceContainer
	{
		IReference<ReportInstance> ReportInstance
		{
			get;
		}

		IReference<ReportInstance> SetReportInstance(ReportInstance reportInstance, OnDemandMetadata odpMetadata);
	}
}
