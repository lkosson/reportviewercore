using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[Serializable]
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum ParameterTypeEnum
	{
		Boolean,
		DateTime,
		Integer,
		Float,
		String
	}
}
