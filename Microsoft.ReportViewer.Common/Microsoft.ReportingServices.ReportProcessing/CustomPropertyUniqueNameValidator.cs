namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class CustomPropertyUniqueNameValidator : NameValidator
	{
		internal CustomPropertyUniqueNameValidator()
		{
		}

		internal bool Validate(Severity severity, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext)
		{
			bool result = true;
			if (propertyNameValue == null || !IsUnique(propertyNameValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCustomPropertyName, severity, objectType, objectName, propertyNameValue);
				result = false;
			}
			return result;
		}
	}
}
