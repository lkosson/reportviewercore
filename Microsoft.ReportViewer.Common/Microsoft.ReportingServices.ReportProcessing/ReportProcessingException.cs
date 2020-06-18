using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ReportProcessingException : ReportProcessingExceptionBase
	{
		private ProcessingMessageList m_processingMessages;

		private bool m_useMessageListForExceptionMessage;

		private const string ProcessingMessagesName = "ProcessingMessages";

		private const string UseMessageListForExeptionMessageName = "UseMessageListForExeptionMessage";

		public ProcessingMessageList ProcessingMessages
		{
			[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
			get
			{
				return m_processingMessages;
			}
		}

		public override string Message
		{
			get
			{
				if (m_useMessageListForExceptionMessage && m_processingMessages != null)
				{
					foreach (ProcessingMessage processingMessage in m_processingMessages)
					{
						if (processingMessage.Severity == Severity.Error)
						{
							return processingMessage.Message;
						}
					}
				}
				return base.Message;
			}
		}

		internal ReportProcessingException(ProcessingMessageList processingMessages)
			: this(processingMessages, null)
		{
		}

		internal ReportProcessingException(ProcessingMessageList processingMessages, Exception innerException)
			: base(ErrorCode.rsProcessingError, RPRes.Keys.GetString(ErrorCode.rsProcessingError.ToString()), innerException, Global.Tracer, null)
		{
			m_useMessageListForExceptionMessage = true;
			m_processingMessages = processingMessages;
		}

		internal ReportProcessingException(Exception innerException, ProcessingMessageList processingMessages)
			: base(ErrorCode.rsInternalError, RPRes.Keys.GetString(ErrorCode.rsUnexpectedError.ToString()), innerException, Global.Tracer, null)
		{
			m_processingMessages = processingMessages;
		}

		internal ReportProcessingException(ErrorCode code, Exception innerException, params object[] arguments)
			: base(code, string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(code.ToString()), arguments), innerException, Global.Tracer, null)
		{
		}

		internal ReportProcessingException(ErrorCode code)
			: base(code, RPRes.Keys.GetString(code.ToString()), null, Global.Tracer, null)
		{
		}

		internal ReportProcessingException(ErrorCode code, params object[] arguments)
			: base(code, string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(code.ToString()), arguments), null, Global.Tracer, null)
		{
		}

		internal ReportProcessingException(string errMessage, ErrorCode code)
			: base(code, errMessage, null, Global.Tracer, null)
		{
		}

		internal ReportProcessingException(string message, ErrorCode code, Exception innerException)
			: base(code, message, innerException, Global.Tracer, null)
		{
		}

		protected ReportProcessingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			m_processingMessages = (ProcessingMessageList)info.GetValue("ProcessingMessages", typeof(ProcessingMessageList));
			m_useMessageListForExceptionMessage = info.GetBoolean("UseMessageListForExeptionMessage");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ProcessingMessages", m_processingMessages);
			info.AddValue("UseMessageListForExeptionMessage", m_useMessageListForExceptionMessage);
		}

		protected override XmlNode AddMoreInformationForThis(XmlDocument doc, XmlNode parent, StringBuilder errorMsgBuilder)
		{
			if (m_processingMessages == null)
			{
				return base.AddMoreInformationForThis(doc, parent, errorMsgBuilder);
			}
			bool flag = false;
			foreach (ProcessingMessage processingMessage2 in m_processingMessages)
			{
				if (processingMessage2.Severity == Severity.Error)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return base.AddMoreInformationForThis(doc, parent, errorMsgBuilder);
			}
			XmlNode xmlNode = RSException.CreateMoreInfoNode(Source, doc, parent);
			foreach (ProcessingMessage processingMessage3 in m_processingMessages)
			{
				if (processingMessage3.Severity == Severity.Error && xmlNode != null)
				{
					string text = CodeFromMessage(processingMessage3);
					string helpLink = CreateHelpLink(typeof(RPRes).FullName, text);
					RSException.AddMessageToMoreInfoNode(doc, xmlNode, text, processingMessage3.Message, helpLink);
				}
			}
			return xmlNode;
		}

		protected override void AddWarnings(XmlDocument doc, XmlNode parent)
		{
			if (m_processingMessages == null)
			{
				return;
			}
			foreach (ProcessingMessage processingMessage in m_processingMessages)
			{
				if (processingMessage.Severity == Severity.Warning)
				{
					string code = CodeFromMessage(processingMessage);
					RSException.AddWarningNode(doc, parent, code, processingMessage.Severity.ToString(), processingMessage.ObjectName, processingMessage.ObjectType.ToString(), processingMessage.Message);
				}
			}
		}

		protected override List<AdditionalMessage> GetAdditionalMessages()
		{
			if (m_processingMessages == null)
			{
				return null;
			}
			List<AdditionalMessage> list = new List<AdditionalMessage>(m_processingMessages.Count);
			foreach (ProcessingMessage processingMessage in m_processingMessages)
			{
				list.Add(new AdditionalMessage(CodeFromMessage(processingMessage), processingMessage.Severity.ToString(), processingMessage.Message, processingMessage.ObjectType.ToString(), processingMessage.ObjectName, processingMessage.PropertyName));
			}
			return list;
		}

		private static string CodeFromMessage(ProcessingMessage message)
		{
			if (message.Code == ProcessingErrorCode.rsNone)
			{
				return message.CommonCode.ToString();
			}
			return message.Code.ToString();
		}
	}
}
