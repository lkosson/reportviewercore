using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ProcessingMessage : IPersistable
	{
		private ProcessingErrorCode m_code;

		private Severity m_severity;

		private ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private string m_message;

		private ProcessingMessageList m_processingMessages;

		private ErrorCode m_commonCode;

		[NonSerialized]
		private static readonly Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = GetNewDeclaration();

		public ProcessingErrorCode Code
		{
			get
			{
				return m_code;
			}
			set
			{
				m_code = value;
			}
		}

		public ErrorCode CommonCode
		{
			get
			{
				return m_commonCode;
			}
			set
			{
				m_commonCode = value;
			}
		}

		public Severity Severity
		{
			get
			{
				return m_severity;
			}
			set
			{
				m_severity = value;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				return m_objectType;
			}
			set
			{
				m_objectType = value;
			}
		}

		public string ObjectName
		{
			get
			{
				return m_objectName;
			}
			set
			{
				m_objectName = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return m_propertyName;
			}
			set
			{
				m_propertyName = value;
			}
		}

		public string Message
		{
			get
			{
				return m_message;
			}
			set
			{
				m_message = value;
			}
		}

		public ProcessingMessageList ProcessingMessages
		{
			get
			{
				return m_processingMessages;
			}
			set
			{
				m_processingMessages = value;
			}
		}

		internal ProcessingMessage(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, string message, ProcessingMessageList innerMessages)
		{
			m_code = code;
			m_commonCode = ErrorCode.rsProcessingError;
			m_severity = severity;
			m_objectType = objectType;
			m_objectName = objectName;
			m_propertyName = propertyName;
			m_message = message;
			m_processingMessages = innerMessages;
		}

		internal ProcessingMessage(ErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, string message, ProcessingMessageList innerMessages)
		{
			m_code = ProcessingErrorCode.rsNone;
			m_commonCode = code;
			m_severity = severity;
			m_objectType = objectType;
			m_objectName = objectName;
			m_propertyName = propertyName;
			m_message = message;
			m_processingMessages = innerMessages;
		}

		internal ProcessingMessage()
		{
		}

		public string FormatMessage()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} ({1}.{2}) : {3} [{4}]", (m_severity == Severity.Warning) ? "Warning" : "Error", m_objectName, m_propertyName, m_message, m_code);
		}

		internal static Microsoft.ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Code, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Severity, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.ObjectType, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.ObjectName, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.PropertyName, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Message, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.ProcessingMessages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ProcessingMessageList));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.CommonCode, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			return new Microsoft.ReportingServices.ReportProcessing.Persistence.Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			return new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		void IPersistable.Serialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code:
					writer.WriteEnum((int)m_code);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity:
					writer.WriteEnum((int)m_severity);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType:
					writer.WriteEnum((int)m_objectType);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName:
					writer.Write(m_objectName);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName:
					writer.Write(m_propertyName);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message:
					writer.Write(m_message);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages:
					writer.Write(m_processingMessages);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode:
					writer.WriteEnum((int)m_commonCode);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code:
					m_code = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity:
					m_severity = (Severity)reader.ReadEnum();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType:
					m_objectType = (ObjectType)reader.ReadEnum();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName:
					m_objectName = reader.ReadString();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName:
					m_propertyName = reader.ReadString();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message:
					m_message = reader.ReadString();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages:
					m_processingMessages = reader.ReadListOfRIFObjects<ProcessingMessageList>();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode:
					m_commonCode = (ErrorCode)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage;
		}
	}
}
