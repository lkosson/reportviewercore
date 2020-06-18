namespace Microsoft.ReportingServices.ReportProcessing
{
	internal struct VariantResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal string ExceptionMessage;

		internal object Value;

		internal VariantResult(bool errorOccurred, object v)
		{
			ErrorOccurred = errorOccurred;
			Value = v;
			FieldStatus = DataFieldStatus.None;
			ExceptionMessage = null;
		}
	}
}
