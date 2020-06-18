using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IAggregateHolder
	{
		DataScopeInfo DataScopeInfo
		{
			get;
		}

		List<DataAggregateInfo> GetAggregateList();

		List<DataAggregateInfo> GetPostSortAggregateList();

		void ClearIfEmpty();
	}
}
