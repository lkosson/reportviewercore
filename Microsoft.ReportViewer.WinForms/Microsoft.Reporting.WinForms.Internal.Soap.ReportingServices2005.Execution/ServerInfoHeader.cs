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
	public class ServerInfoHeader : SoapHeader
	{
		private string reportServerVersionNumberField;

		private string reportServerEditionField;

		private string reportServerVersionField;

		private string reportServerDateTimeField;

		private XmlAttribute[] anyAttrField;

		public string ReportServerVersionNumber
		{
			get
			{
				return reportServerVersionNumberField;
			}
			set
			{
				reportServerVersionNumberField = value;
			}
		}

		public string ReportServerEdition
		{
			get
			{
				return reportServerEditionField;
			}
			set
			{
				reportServerEditionField = value;
			}
		}

		public string ReportServerVersion
		{
			get
			{
				return reportServerVersionField;
			}
			set
			{
				reportServerVersionField = value;
			}
		}

		public string ReportServerDateTime
		{
			get
			{
				return reportServerDateTimeField;
			}
			set
			{
				reportServerDateTimeField = value;
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
