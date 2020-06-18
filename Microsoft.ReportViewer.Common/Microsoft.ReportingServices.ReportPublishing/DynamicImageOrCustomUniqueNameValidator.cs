using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal abstract class DynamicImageOrCustomUniqueNameValidator : UniqueNameValidator
	{
		internal abstract bool Validate(Severity severity, string propertyName, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext);
	}
}
