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
	public class ParameterFieldReference : ParameterValueOrFieldReference
	{
		private string parameterNameField;

		private string fieldAliasField;

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

		public string FieldAlias
		{
			get
			{
				return fieldAliasField;
			}
			set
			{
				fieldAliasField = value;
			}
		}
	}
}
