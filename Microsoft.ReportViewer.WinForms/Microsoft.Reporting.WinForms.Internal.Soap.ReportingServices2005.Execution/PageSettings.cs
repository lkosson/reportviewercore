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
	public class PageSettings
	{
		private ReportPaperSize paperSizeField;

		private ReportMargins marginsField;

		public ReportPaperSize PaperSize
		{
			get
			{
				return paperSizeField;
			}
			set
			{
				paperSizeField = value;
			}
		}

		public ReportMargins Margins
		{
			get
			{
				return marginsField;
			}
			set
			{
				marginsField = value;
			}
		}
	}
}
