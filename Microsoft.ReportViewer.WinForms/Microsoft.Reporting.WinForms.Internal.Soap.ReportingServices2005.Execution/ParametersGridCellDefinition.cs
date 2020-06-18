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
	public class ParametersGridCellDefinition
	{
		private int rowIndexField;

		private int columnsIndexField;

		private string parameterNameField;

		public int RowIndex
		{
			get
			{
				return rowIndexField;
			}
			set
			{
				rowIndexField = value;
			}
		}

		public int ColumnsIndex
		{
			get
			{
				return columnsIndexField;
			}
			set
			{
				columnsIndexField = value;
			}
		}

		public string ParameterName
		{
			get
			{
				return parameterNameField;
			}
			set
			{
				parameterNameField = value;
			}
		}
	}
}
