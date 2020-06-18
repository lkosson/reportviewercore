namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class PublishingErrorContext : ErrorContext
	{
		private const int MaxNumberOfMessages = 100;

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			return Register(code, severity, objectType, objectName, propertyName, null, arguments);
		}

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, ProcessingMessageList innerMessages, params string[] arguments)
		{
			if (m_suspendErrors)
			{
				return null;
			}
			if (m_messages == null)
			{
				m_messages = new ProcessingMessageList();
			}
			ProcessingMessage processingMessage = null;
			if (m_messages.Count < 100 || (severity == Severity.Error && !m_hasError))
			{
				processingMessage = ErrorContext.CreateProcessingMessage(code, severity, objectType, objectName, propertyName, innerMessages, arguments);
				m_messages.Add(processingMessage);
			}
			if (severity == Severity.Error)
			{
				m_hasError = true;
			}
			return processingMessage;
		}
	}
}
