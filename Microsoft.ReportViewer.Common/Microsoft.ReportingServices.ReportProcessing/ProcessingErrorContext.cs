using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Collections;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ProcessingErrorContext : ErrorContext
	{
		private Hashtable m_itemsRegistered;

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			return Register(code, severity, objectType, objectName, propertyName, null, arguments);
		}

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, ProcessingMessageList innerMessages, params string[] arguments)
		{
			try
			{
				Monitor.Enter(this);
				if (severity == Severity.Error)
				{
					m_hasError = true;
				}
				if (RegisterItem(severity, code, objectType, objectName))
				{
					if (m_messages == null)
					{
						m_messages = new ProcessingMessageList();
					}
					ProcessingMessage processingMessage = ErrorContext.CreateProcessingMessage(code, severity, objectType, objectName, propertyName, innerMessages, arguments);
					m_messages.Add(processingMessage);
					return processingMessage;
				}
				return null;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		internal override void Register(RSException rsException, ObjectType objectType)
		{
			try
			{
				Monitor.Enter(this);
				base.Register(rsException, objectType);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private bool RegisterItem(Severity severity, ProcessingErrorCode code, ObjectType objectType, string objectName)
		{
			if (m_itemsRegistered == null)
			{
				m_itemsRegistered = new Hashtable();
			}
			if (ObjectType.DataSet == objectType && (ProcessingErrorCode.rsErrorReadingDataSetField == code || ProcessingErrorCode.rsDataSetFieldTypeNotSupported == code || ProcessingErrorCode.rsMissingFieldInDataSet == code || ProcessingErrorCode.rsErrorReadingFieldProperty == code))
			{
				return true;
			}
			bool result = false;
			int num = (int)code;
			string text = num.ToString(CultureInfo.InvariantCulture);
			if (objectType == ObjectType.Report || ObjectType.PageHeader == objectType || ObjectType.PageFooter == objectType)
			{
				string key = text + objectType;
				if (!m_itemsRegistered.ContainsKey(key))
				{
					result = true;
					m_itemsRegistered.Add(key, null);
				}
			}
			else
			{
				Hashtable hashtable = (Hashtable)m_itemsRegistered[objectType];
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					m_itemsRegistered[objectType] = hashtable;
				}
				Global.Tracer.Assert(objectName != null, "(null != objectName)");
				string key2 = severity.ToString() + text + objectName;
				if (!hashtable.ContainsKey(key2))
				{
					result = true;
					hashtable.Add(key2, null);
				}
			}
			return result;
		}

		internal void Combine(ProcessingMessageList messages)
		{
			if (messages == null)
			{
				return;
			}
			for (int i = 0; i < messages.Count; i++)
			{
				ProcessingMessage processingMessage = messages[i];
				if (processingMessage.Severity == Severity.Error)
				{
					m_hasError = true;
				}
				if (RegisterItem(processingMessage.Severity, processingMessage.Code, processingMessage.ObjectType, processingMessage.ObjectName))
				{
					if (m_messages == null)
					{
						m_messages = new ProcessingMessageList();
					}
					m_messages.Add(processingMessage);
				}
			}
		}
	}
}
