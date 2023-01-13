using Microsoft.Reporting.NETCore.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.SqlServer.ReportingServices;
using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.ServiceModel;
using System.Xml;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public class ReportServerException : ReportViewerException
	{
		private const string SoapErrorNamespace = "http://www.microsoft.com/sql/reportingservices";

		private string m_errorCode = "";

		public string ErrorCode => m_errorCode;

		protected ReportServerException(string message, string errorCode, Exception innerException)
			: base(message, innerException)
		{
			if (errorCode != null)
			{
				m_errorCode = errorCode;
			}
		}

		protected ReportServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			info.AddValue("ReportServerErrorCode", m_errorCode);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			m_errorCode = (info.GetValue("ReportServerErrorCode", typeof(string)) as string);
		}

		internal static ReportServerException FromException(Exception e)
		{
			ReportServerException ex = e as ReportServerException;
			if (ex != null)
			{
				return ex;
			}
			FaultException ex2 = e as FaultException;
			if (ex2 != null)
			{
				var fault = ex2.CreateMessageFault();
				var detail = fault.GetReaderAtDetailContents();
				var detailDocument = new XmlDocument();
				var detailNode = detailDocument.ReadNode(detail);
				ReportServerException ex3 = FromMoreInformationNode(GetNestedMoreInformationNode(detailNode));
				if (ex3 != null)
				{
					return ex3;
				}
			}
			else
			{
				if (e is RSExecutionConnection.MissingEndpointException)
				{
					return new ReportServerException(e.Message, null, new MissingEndpointException(e.Message, e.InnerException));
				}
				if (e is RSExecutionConnection.SoapVersionMismatchException)
				{
					return new ReportServerException(e.Message, null, new SoapVersionMismatchException(e.Message, e.InnerException));
				}
			}
			return new ReportServerException(e.Message, null, e);
		}

		internal static ReportServerException FromMoreInformationNode(XmlNode moreInfoNode)
		{
			if (moreInfoNode == null)
			{
				return null;
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(moreInfoNode.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
			string text = null;
			string text2 = null;
			XmlNode xmlNode = moreInfoNode.SelectSingleNode("rs:Message", xmlNamespaceManager);
			if (xmlNode != null)
			{
				string innerText = xmlNode.InnerText;
				XmlNode namedItem = xmlNode.Attributes.GetNamedItem("ErrorCode", "http://www.microsoft.com/sql/reportingservices");
				if (namedItem != null)
				{
					text2 = namedItem.Value;
				}
				text = ((!string.IsNullOrEmpty(text2)) ? SoapExceptionStrings.RSSoapMessageFormat(innerText, text2) : innerText);
			}
			ReportServerException ex = FromMoreInformationNode(GetNestedMoreInformationNode(moreInfoNode));
			if (string.IsNullOrEmpty(text))
			{
				return ex;
			}
			return new ReportServerException(text, text2, ex);
		}

		private static XmlNode GetNestedMoreInformationNode(XmlNode node)
		{
			if (node == null)
			{
				return null;
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(node.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
			return node.SelectSingleNode("rs:MoreInformation", xmlNamespaceManager);
		}
	}
}
