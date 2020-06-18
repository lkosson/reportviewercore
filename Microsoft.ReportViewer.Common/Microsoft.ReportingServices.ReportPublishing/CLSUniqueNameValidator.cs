using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class CLSUniqueNameValidator : NameValidator
	{
		private ProcessingErrorCode m_errorCodeNotCLS;

		private ProcessingErrorCode m_errorCodeNotUnique;

		private ProcessingErrorCode m_errorCodeNameLength;

		private ProcessingErrorCode m_errorCodeCaseInsensitiveDuplicate;

		internal CLSUniqueNameValidator(ProcessingErrorCode errorCodeNotCLS, ProcessingErrorCode errorCodeNotUnique, ProcessingErrorCode errorCodeNameLength)
			: base(caseInsensitiveComparison: false)
		{
			m_errorCodeNotCLS = errorCodeNotCLS;
			m_errorCodeNotUnique = errorCodeNotUnique;
			m_errorCodeNameLength = errorCodeNameLength;
			m_errorCodeCaseInsensitiveDuplicate = ProcessingErrorCode.rsNone;
		}

		internal CLSUniqueNameValidator(ProcessingErrorCode errorCodeNotCLS, ProcessingErrorCode errorCodeNotUnique, ProcessingErrorCode errorCodeNameLength, ProcessingErrorCode errorCodeCaseInsensitiveDuplicate)
			: base(caseInsensitiveComparison: true)
		{
			m_errorCodeNotCLS = errorCodeNotCLS;
			m_errorCodeNotUnique = errorCodeNotUnique;
			m_errorCodeNameLength = errorCodeNameLength;
			m_errorCodeCaseInsensitiveDuplicate = errorCodeCaseInsensitiveDuplicate;
		}

		internal bool Validate(ObjectType objectType, string name, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				errorContext.Register(m_errorCodeNameLength, Severity.Error, objectType, name, "Name", "256");
				result = false;
			}
			if (!NameValidator.IsCLSCompliant(name))
			{
				errorContext.Register(m_errorCodeNotCLS, Severity.Error, objectType, name, "Name");
				result = false;
			}
			if (!IsUnique(name))
			{
				errorContext.Register(m_errorCodeNotUnique, Severity.Error, objectType, name, "Name");
				result = false;
			}
			if (IsCaseInsensitiveDuplicate(name))
			{
				errorContext.Register(m_errorCodeCaseInsensitiveDuplicate, Severity.Warning, objectType, name, "Name");
			}
			return result;
		}

		internal bool Validate(string name, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				errorContext.Register(m_errorCodeNameLength, Severity.Error, objectType, objectName, "Name", "256");
				result = false;
			}
			if (!NameValidator.IsCLSCompliant(name))
			{
				errorContext.Register(m_errorCodeNotCLS, Severity.Error, objectType, objectName, "Name", name);
				result = false;
			}
			if (!IsUnique(name))
			{
				errorContext.Register(m_errorCodeNotUnique, Severity.Error, objectType, objectName, "Name", name);
				result = false;
			}
			return result;
		}

		internal bool Validate(string name, string dataField, string dataSetName, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				errorContext.Register(m_errorCodeNameLength, Severity.Error, ObjectType.Field, dataField, "Name", name, dataSetName, "256");
				result = false;
			}
			if (!NameValidator.IsCLSCompliant(name))
			{
				errorContext.Register(m_errorCodeNotCLS, Severity.Error, ObjectType.Field, dataField, "Name", name, dataSetName);
				result = false;
			}
			if (!IsUnique(name))
			{
				errorContext.Register(m_errorCodeNotUnique, Severity.Error, ObjectType.Field, dataField, "Name", name, dataSetName);
				result = false;
			}
			return result;
		}
	}
}
