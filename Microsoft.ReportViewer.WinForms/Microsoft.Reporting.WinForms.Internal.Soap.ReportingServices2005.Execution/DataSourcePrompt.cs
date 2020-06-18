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
	public class DataSourcePrompt
	{
		private string nameField;

		private string dataSourceIDField;

		private string promptField;

		public string Name
		{
			get
			{
				return nameField;
			}
			set
			{
				nameField = value;
			}
		}

		public string DataSourceID
		{
			get
			{
				return dataSourceIDField;
			}
			set
			{
				dataSourceIDField = value;
			}
		}

		public string Prompt
		{
			get
			{
				return promptField;
			}
			set
			{
				promptField = value;
			}
		}
	}
}
