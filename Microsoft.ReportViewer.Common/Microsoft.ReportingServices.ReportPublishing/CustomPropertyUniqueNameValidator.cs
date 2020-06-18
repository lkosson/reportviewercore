using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class CustomPropertyUniqueNameValidator : DynamicImageOrCustomUniqueNameValidator
	{
		internal CustomPropertyUniqueNameValidator()
		{
		}

		internal override bool Validate(Severity severity, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext)
		{
			Global.Tracer.Assert(condition: false);
			return Validate(severity, "", objectType, objectName, propertyNameValue, errorContext);
		}

		internal override bool Validate(Severity severity, string propertyName, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext)
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
