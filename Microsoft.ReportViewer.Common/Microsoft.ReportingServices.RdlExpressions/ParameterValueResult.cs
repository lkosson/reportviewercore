using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal struct ParameterValueResult
	{
		internal bool ErrorOccurred;

		internal object Value;

		internal DataType Type;
	}
}
