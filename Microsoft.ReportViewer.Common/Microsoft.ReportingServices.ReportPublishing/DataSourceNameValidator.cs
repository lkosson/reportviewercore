using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class DataSourceNameValidator : NameValidator
	{
		internal DataSourceNameValidator()
			: base(caseInsensitiveComparison: false)
		{
		}

		internal bool Validate(ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(objectName) || objectName.Length > 256)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDataSourceNameLength, Severity.Error, objectType, objectName, "Name", "256");
				result = false;
			}
			if (!IsUnique(objectName))
			{
				errorContext.Register(ProcessingErrorCode.rsDuplicateDataSourceName, Severity.Error, objectType, objectName, "Name");
				result = false;
			}
			if (!NameValidator.IsCLSCompliant(objectName))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDataSourceNameNotCLSCompliant, Severity.Error, objectType, objectName, "Name");
				result = false;
			}
			return result;
		}
	}
}
