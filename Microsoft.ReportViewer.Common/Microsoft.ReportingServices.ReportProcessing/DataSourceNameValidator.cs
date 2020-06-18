namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class DataSourceNameValidator : NameValidator
	{
		internal bool Validate(ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (!IsUnique(objectName))
			{
				errorContext.Register(ProcessingErrorCode.rsDuplicateDataSourceName, Severity.Error, objectType, objectName, "Name");
				result = false;
			}
			return result;
		}
	}
}
