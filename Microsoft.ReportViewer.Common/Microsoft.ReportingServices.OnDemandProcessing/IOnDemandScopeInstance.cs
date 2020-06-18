using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal interface IOnDemandScopeInstance : IStorable, IPersistable
	{
		bool IsNoRows
		{
			get;
		}

		bool IsMostRecentlyCreatedScopeInstance
		{
			get;
		}

		bool HasUnProcessedServerAggregate
		{
			get;
		}

		void SetupEnvironment();

		IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(DataRegion rifDataRegion);

		IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope);
	}
}
