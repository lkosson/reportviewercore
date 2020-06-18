using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IRunningValueHolder
	{
		DataScopeInfo DataScopeInfo
		{
			get;
		}

		List<RunningValueInfo> GetRunningValueList();

		void ClearIfEmpty();
	}
}
