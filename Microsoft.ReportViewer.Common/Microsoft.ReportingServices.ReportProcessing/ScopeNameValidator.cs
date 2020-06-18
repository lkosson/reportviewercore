namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ScopeNameValidator : NameValidator
	{
		internal bool Validate(bool isGrouping, string scopeName, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			return Validate(isGrouping, scopeName, objectType, objectName, errorContext, enforceCLSCompliance: true);
		}

		internal bool Validate(bool isGrouping, string scopeName, ObjectType objectType, string objectName, ErrorContext errorContext, bool enforceCLSCompliance)
		{
			bool result = true;
			if (!NameValidator.IsCLSCompliant(scopeName) && enforceCLSCompliance)
			{
				if (isGrouping)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidGroupingNameNotCLSCompliant, Severity.Error, objectType, objectName, "Name", scopeName);
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, Severity.Error, objectType, objectName, "Name");
				}
				result = false;
			}
			if (!IsUnique(scopeName))
			{
				errorContext.Register(ProcessingErrorCode.rsDuplicateScopeName, Severity.Error, objectType, objectName, "Name", scopeName);
				result = false;
			}
			return result;
		}
	}
}
