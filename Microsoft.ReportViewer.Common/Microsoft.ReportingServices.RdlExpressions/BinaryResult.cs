using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal struct BinaryResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal byte[] Value;
	}
}
