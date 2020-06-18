using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class VariableNameValidator : NameValidator
	{
		internal VariableNameValidator()
			: base(caseInsensitiveComparison: false)
		{
		}

		internal bool Validate(string name, ObjectType objectType, string objectName, ErrorContext errorContext, bool isGrouping, string groupingName)
		{
			bool result = true;
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				if (isGrouping)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidGroupingVariableNameLength, Severity.Error, objectType, objectName, "Variable", name, groupingName, "256");
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidVariableNameLength, Severity.Error, objectType, objectName, "Variable", name, "256");
				}
				result = false;
			}
			if (!NameValidator.IsCLSCompliant(name))
			{
				if (isGrouping)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidGroupingVariableNameNotCLSCompliant, Severity.Error, objectType, objectName, "Variable", name, groupingName);
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidVariableNameNotCLSCompliant, Severity.Error, objectType, objectName, "Variable", name);
				}
				result = false;
			}
			if (!IsUnique(name))
			{
				if (isGrouping)
				{
					errorContext.Register(ProcessingErrorCode.rsDuplicateGroupingVariableName, Severity.Error, objectType, objectName, "Variable", name, groupingName);
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsDuplicateVariableName, Severity.Error, objectType, objectName, "Variable", name);
				}
				result = false;
			}
			return result;
		}
	}
}
