using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface ISortDataHolder : IStorable, IPersistable
	{
		void NextRow();

		void Traverse(ProcessingStages operation, ITraversalContext traversalContext);
	}
}
