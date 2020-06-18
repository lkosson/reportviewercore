using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFReportIntersectionScope : IRIFReportDataScope, IRIFReportScope, IInstancePath, IRIFDataScope
	{
		IRIFReportDataScope ParentRowReportScope
		{
			get;
		}

		IRIFReportDataScope ParentColumnReportScope
		{
			get;
		}

		bool IsColumnOuterGrouping
		{
			get;
		}

		void BindToStreamingScopeInstance(IReference<IOnDemandMemberInstance> parentRowScopeInstance, IReference<IOnDemandMemberInstance> parentColumnScopeInstance);
	}
}
