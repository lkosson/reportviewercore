using Microsoft.ReportingServices.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal class RSException : Exception
	{
		internal sealed class AdditionalMessage
		{
			internal string Code
			{
				get;
				private set;
			}

			internal string Severity
			{
				get;
				private set;
			}

			internal string Message
			{
				get;
				private set;
			}

			internal string ObjectType
			{
				get;
				private set;
			}

			internal string ObjectName
			{
				get;
				private set;
			}

			internal string PropertyName
			{
				get;
				private set;
			}

			internal string[] AffectedItems
			{
				get;
				private set;
			}

			internal AdditionalMessage(string code, string severity, string message, string objectType = null, string objectName = null, string propertyName = null, string[] affectedItems = null)
			{
				Code = code;
				Severity = severity;
				Message = message;
				ObjectType = objectType;
				ObjectName = objectName;
				PropertyName = propertyName;
				AffectedItems = affectedItems;
			}
		}

		private const int ExceptionMessageLimit = 3000;

		private string m_toString;

		private string m_ActorUri = "";

		private ErrorCode m_ErrorCode = ErrorCode.rsInternalError;

		private string m_ProductName = "change this";

		private string m_ProductVersion = "1.0";

		private int m_ProductLocaleID = Localization.CatalogCulture.LCID;

		private int m_CountryLocaleID = Localization.ClientPrimaryCulture.LCID;

		private OperatingSystem m_OS;

		private string m_AdditionalTraceMessage;

		private RSTrace m_tracer;

		private TraceLevel m_traceLevel = TraceLevel.Error;

		private object[] m_exceptionData;

		public override string Message => TrimExtraLength(base.Message);

		public string ExceptionLevelHelpLink => CreateHelpLink(typeof(ErrorStrings).FullName, Code.ToString());

		public bool SkipTopLevelMessage
		{
			get
			{
				RSException ex = base.InnerException as RSException;
				if (ex != null)
				{
					return ex.Code == Code;
				}
				return false;
			}
		}

		public ErrorCode Code => m_ErrorCode;

		internal List<AdditionalMessage> AdditionalMessages => GetAdditionalMessages();

		protected virtual bool TraceFullException => true;

		public static string ErrorNotVisibleOnRemoteBrowsers => ErrorStrings.rsErrorNotVisibleToRemoteBrowsers;

		public string AdditionalTraceMessage => m_AdditionalTraceMessage;

		public object[] ExceptionData => m_exceptionData;

		internal string ProductName => m_ProductName;

		internal string ProductVersion => m_ProductVersion;

		internal int ProductLocaleID => m_ProductLocaleID;

		internal string OperatingSystem => m_OS.ToString();

		internal int CountryLocaleID => m_CountryLocaleID;

		public static event EventHandler<RSExceptionCreatedEventArgs> ExceptionCreated;

		internal static bool IsClientLocal()
		{
			return true;
		}

		public override string ToString()
		{
			if (m_toString == null)
			{
				m_toString = TrimExtraLength(base.ToString());
				if (IsClientLocal())
				{
					m_toString = string.Concat(GetType(), ": ", Message);
					for (Exception innerException = base.InnerException; innerException != null; innerException = innerException.InnerException)
					{
						m_toString = string.Concat(m_toString, " ---> ", innerException.GetType(), ": ", innerException.Message);
					}
				}
				m_toString = TrimExtraLength(m_toString);
			}
			return m_toString;
		}

		public RSException(ErrorCode errorCode, string localizedMessage, Exception innerException, RSTrace tracer, string additionalTraceMessage, params object[] exceptionData)
			: this(errorCode, localizedMessage, innerException, tracer, additionalTraceMessage, TraceLevel.Error, exceptionData)
		{
		}

		public RSException(ErrorCode errorCode, string localizedMessage, Exception innerException, RSTrace tracer, string additionalTraceMessage, TraceLevel traceLevel, params object[] exceptionData)
			: base(localizedMessage, innerException)
		{
			m_ErrorCode = errorCode;
			m_ProductLocaleID = CultureInfo.CurrentCulture.LCID;
			m_CountryLocaleID = CultureInfo.InstalledUICulture.LCID;
			m_OS = Microsoft.ReportingServices.Diagnostics.Utilities.OperatingSystem.OsIndependent;
			m_AdditionalTraceMessage = additionalTraceMessage;
			m_tracer = tracer;
			m_traceLevel = traceLevel;
			m_exceptionData = exceptionData;
			Trace();
			OnExceptionCreated();
		}

		public RSException(RSException inner)
			: base(inner.Message, inner)
		{
			m_ErrorCode = inner.m_ErrorCode;
			m_ActorUri = inner.m_ActorUri;
			m_ProductName = inner.m_ProductName;
			m_ProductVersion = inner.m_ProductVersion;
			m_ProductLocaleID = inner.m_ProductLocaleID;
			m_CountryLocaleID = inner.m_CountryLocaleID;
			m_OS = inner.m_OS;
			m_AdditionalTraceMessage = inner.m_AdditionalTraceMessage;
		}

		public void Trace()
		{
			if (m_tracer != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (TraceFullException)
				{
					stringBuilder.AppendFormat("Throwing {0}: {1}, {2};", GetType().FullName, m_AdditionalTraceMessage, base.ToString());
				}
				else
				{
					stringBuilder.AppendFormat("Throwing {0}: {1}, {2};", GetType().FullName, m_AdditionalTraceMessage, base.Message);
				}
				m_tracer.TraceException(m_traceLevel, (3000 > stringBuilder.Length) ? stringBuilder.ToString() : stringBuilder.ToString(0, 3000));
			}
		}

		public void SetExceptionProperties(string actorUri, string productName, string productVersion)
		{
			m_ActorUri = actorUri;
			m_ProductName = productName;
			m_ProductVersion = productVersion;
		}

		private XmlNode DetailsAsXml(bool enableRemoteErrors, out StringBuilder errorMsgBuilder)
		{
			string text = SoapUtil.RemoveInvalidXmlChars(Message);
			errorMsgBuilder = new StringBuilder();
			errorMsgBuilder.Append(text);
			return ToXml(new XmlDocument(), enableRemoteErrors, text, errorMsgBuilder);
		}

		public string DetailsAsXmlString(bool enableRemoteErrors)
		{
			StringBuilder errorMsgBuilder;
			return DetailsAsXml(enableRemoteErrors, out errorMsgBuilder).OuterXml;
		}

		protected RSException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected XmlNode ToXml(XmlDocument doc, bool enableRemoteErrors, string detailedMsg, StringBuilder errorMsgBuilder)
		{
			XmlNode xmlNode = SoapUtil.CreateExceptionDetailsNode(doc, Code.ToString(), detailedMsg, ExceptionLevelHelpLink, m_ProductName, m_ProductVersion, m_ProductLocaleID, m_OS.ToString(), m_CountryLocaleID);
			AddMoreInformation(doc, xmlNode, enableRemoteErrors, errorMsgBuilder);
			AddWarningsInternal(doc, xmlNode);
			return xmlNode;
		}

		protected virtual XmlNode AddMoreInformationForThis(XmlDocument doc, XmlNode parent, StringBuilder errorMsgBuilder)
		{
			return AddMoreInformationForException(doc, parent, this, errorMsgBuilder);
		}

		internal void AddWarningsInternal(XmlDocument doc, XmlNode parent)
		{
			XmlNode xmlNode = SoapUtil.CreateWarningNode(doc);
			parent.AppendChild(xmlNode);
			AddWarnings(doc, xmlNode);
		}

		protected virtual void AddWarnings(XmlDocument doc, XmlNode parent)
		{
			(base.InnerException as RSException)?.AddWarnings(doc, parent);
		}

		protected virtual List<AdditionalMessage> GetAdditionalMessages()
		{
			return null;
		}

		protected static XmlNode CreateMoreInfoNode(string source, XmlDocument doc, XmlNode parent)
		{
			XmlNode xmlNode = SoapUtil.CreateMoreInfoNode(doc);
			XmlNode xmlNode2 = SoapUtil.CreateMoreInfoSourceNode(doc);
			xmlNode2.InnerText = source;
			xmlNode.AppendChild(xmlNode2);
			parent?.AppendChild(xmlNode);
			return xmlNode;
		}

		protected static string AddMessageToMoreInfoNode(XmlDocument doc, XmlNode moreInfoNode, string errCode, string message, string helpLink)
		{
			XmlNode xmlNode = SoapUtil.CreateMoreInfoMessageNode(doc);
			string result = xmlNode.InnerText = SoapUtil.RemoveInvalidXmlChars(message);
			if (errCode != null)
			{
				XmlAttribute xmlAttribute = SoapUtil.CreateErrorCodeAttr(doc);
				xmlAttribute.Value = errCode;
				xmlNode.Attributes.Append(xmlAttribute);
				XmlAttribute xmlAttribute2 = SoapUtil.CreateHelpLinkTagAttr(doc);
				xmlAttribute2.Value = helpLink;
				xmlNode.Attributes.Append(xmlAttribute2);
			}
			moreInfoNode.AppendChild(xmlNode);
			return result;
		}

		protected static void AddWarningNode(XmlDocument doc, XmlNode parent, string code, string severity, string objectName, string objectType, string message)
		{
			XmlNode xmlNode = SoapUtil.CreateWarningNode(doc);
			XmlNode xmlNode2 = SoapUtil.CreateWarningCodeNode(doc);
			xmlNode2.InnerText = code;
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = SoapUtil.CreateWarningSeverityNode(doc);
			xmlNode3.InnerText = severity;
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode4 = SoapUtil.CreateWarningObjectNameNode(doc);
			xmlNode4.InnerText = objectName;
			xmlNode.AppendChild(xmlNode4);
			XmlNode xmlNode5 = SoapUtil.CreateWarningObjectTypeNode(doc);
			xmlNode5.InnerText = objectType;
			xmlNode.AppendChild(xmlNode5);
			XmlNode xmlNode6 = SoapUtil.CreateWarningMessageNode(doc);
			xmlNode6.InnerText = SoapUtil.RemoveInvalidXmlChars(message);
			xmlNode.AppendChild(xmlNode6);
			parent.AppendChild(xmlNode);
		}

		protected string CreateHelpLink(string messageSource, string id)
		{
			return string.Format(CultureInfo.CurrentCulture, "https://go.microsoft.com/fwlink/?LinkId=20476&EvtSrc={0}&EvtID={1}&ProdName=Microsoft%20SQL%20Server%20Reporting%20Services&ProdVer={2}", messageSource, id, m_ProductVersion);
		}

		internal void AddMoreInformation(XmlDocument doc, XmlNode parent, bool enableRemoteErrors, StringBuilder errorMsgBuilder)
		{
			Exception ex = this;
			XmlNode parent2 = parent;
			if (SkipTopLevelMessage)
			{
				ex = base.InnerException;
			}
			bool flag = enableRemoteErrors || IsClientLocal();
			while (true)
			{
				if (ex == null)
				{
					return;
				}
				RSException ex2 = ex as RSException;
				if (ex2 != null)
				{
					parent2 = ex2.AddMoreInformationForThis(doc, parent2, errorMsgBuilder);
					if (!flag && ex2 is SharePointException && ex2.InnerException != null)
					{
						parent2 = AddMoreInformationForException(doc, parent2, ex2.InnerException, errorMsgBuilder);
					}
				}
				else
				{
					if (!flag)
					{
						break;
					}
					parent2 = AddMoreInformationForException(doc, parent2, ex, errorMsgBuilder);
				}
				ex = ex.InnerException;
			}
			Exception e = new Exception(ErrorNotVisibleOnRemoteBrowsers);
			parent2 = AddMoreInformationForException(doc, parent2, e, errorMsgBuilder);
		}

		private XmlNode AddMoreInformationForException(XmlDocument doc, XmlNode parent, Exception e, StringBuilder errorMsgBuilder)
		{
			XmlNode xmlNode = CreateMoreInfoNode(e.Source, doc, parent);
			if (xmlNode != null)
			{
				string text = null;
				RSException ex = e as RSException;
				if (ex != null)
				{
					text = ex.Code.ToString();
				}
				string helpLink = CreateHelpLink(typeof(ErrorStrings).FullName, text);
				string filteredMsg = AddMessageToMoreInfoNode(doc, xmlNode, text, e.Message, helpLink);
				BuildExceptionMessage(e, filteredMsg, errorMsgBuilder);
			}
			return xmlNode;
		}

		private static void BuildExceptionMessage(Exception e, string filteredMsg, StringBuilder errorMsgBuilder)
		{
			if (e != null)
			{
				errorMsgBuilder.Append(" ---> " + e.GetType().FullName);
				if (!string.IsNullOrEmpty(filteredMsg))
				{
					errorMsgBuilder.Append(": " + filteredMsg);
				}
			}
		}

		internal bool ContainsErrorCode(ErrorCode code)
		{
			for (RSException ex = this; ex != null; ex = (ex.InnerException as RSException))
			{
				if (code == ex.Code)
				{
					return true;
				}
			}
			return false;
		}

		private static string TrimExtraLength(string input)
		{
			return input.Substring(0, Math.Min(3000, input.Length));
		}

		private void OnExceptionCreated()
		{
			EventHandler<RSExceptionCreatedEventArgs> exceptionCreated = RSException.ExceptionCreated;
			if (exceptionCreated != null)
			{
				RSExceptionCreatedEventArgs e = new RSExceptionCreatedEventArgs(this);
				exceptionCreated(this, e);
			}
		}
	}
}
