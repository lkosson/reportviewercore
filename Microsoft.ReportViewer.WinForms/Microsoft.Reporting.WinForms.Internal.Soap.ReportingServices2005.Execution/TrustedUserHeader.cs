using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[Serializable]
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", IsNullable = false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class TrustedUserHeader : SoapHeader
	{
		private string userNameField;

		private byte[] userTokenField;

		private XmlAttribute[] anyAttrField;

		public string UserName
		{
			get
			{
				return userNameField;
			}
			set
			{
				userNameField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] UserToken
		{
			get
			{
				return userTokenField;
			}
			set
			{
				userTokenField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return anyAttrField;
			}
			set
			{
				anyAttrField = value;
			}
		}
	}
}
