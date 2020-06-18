using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class DynamicImageObjectUniqueNameValidator : DynamicImageOrCustomUniqueNameValidator
	{
		internal DynamicImageObjectUniqueNameValidator()
		{
		}

		internal void Clear()
		{
			m_dictionary.Clear();
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
				errorContext.Register(ProcessingErrorCode.rsInvalidObjectNameNotUnique, severity, objectType, objectName, propertyName, propertyNameValue);
				result = false;
			}
			if (propertyNameValue != null && !NameValidator.IsCLSCompliant(propertyNameValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidObjectNameNotCLSCompliant, severity, objectType, objectName, propertyName, propertyNameValue);
				result = false;
			}
			return result;
		}
	}
}
