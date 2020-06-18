using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal abstract class UniqueNameValidator : NameValidator
	{
		internal UniqueNameValidator()
			: base(caseInsensitiveComparison: false)
		{
		}

		internal abstract bool Validate(Severity severity, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext);
	}
}
