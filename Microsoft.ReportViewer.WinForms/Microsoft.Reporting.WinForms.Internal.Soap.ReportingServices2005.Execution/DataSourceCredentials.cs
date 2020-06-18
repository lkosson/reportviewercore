using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[Serializable]
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class DataSourceCredentials
	{
		private string dataSourceNameField;

		private string userNameField;

		private string passwordField;

		public string DataSourceName
		{
			get
			{
				return dataSourceNameField;
			}
			set
			{
				dataSourceNameField = value;
			}
		}

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

		public string Password
		{
			get
			{
				return passwordField;
			}
			set
			{
				passwordField = value;
			}
		}
	}
}
