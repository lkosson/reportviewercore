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
	public class ReportPaperSize
	{
		private double heightField;

		private double widthField;

		public double Height
		{
			get
			{
				return heightField;
			}
			set
			{
				heightField = value;
			}
		}

		public double Width
		{
			get
			{
				return widthField;
			}
			set
			{
				widthField = value;
			}
		}
	}
}
