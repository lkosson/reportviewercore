namespace Microsoft.ReportingServices.ReportProcessing
{
	internal struct DataAggregateObjResult
	{
		internal bool ErrorOccurred;

		internal object Value;

		internal bool HasCode;

		internal ProcessingErrorCode Code;

		internal Severity Severity;

		internal string[] Arguments;

		internal DataFieldStatus FieldStatus;
	}
}
