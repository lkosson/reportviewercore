using System.Diagnostics;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal interface IRSTraceInternalWithDynamicLevel : IRSTraceInternal
	{
		void SetTraceLevel(TraceLevel traceLevel);
	}
}
