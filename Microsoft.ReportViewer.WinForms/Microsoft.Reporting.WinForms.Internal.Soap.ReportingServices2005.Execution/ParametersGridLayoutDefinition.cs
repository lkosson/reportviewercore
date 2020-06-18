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
	public class ParametersGridLayoutDefinition
	{
		private int numberOfColumnsField;

		private int numberOfRowsField;

		private ParametersGridCellDefinition[] cellDefinitionsField;

		public int NumberOfColumns
		{
			get
			{
				return numberOfColumnsField;
			}
			set
			{
				numberOfColumnsField = value;
			}
		}

		public int NumberOfRows
		{
			get
			{
				return numberOfRowsField;
			}
			set
			{
				numberOfRowsField = value;
			}
		}

		public ParametersGridCellDefinition[] CellDefinitions
		{
			get
			{
				return cellDefinitionsField;
			}
			set
			{
				cellDefinitionsField = value;
			}
		}
	}
}
