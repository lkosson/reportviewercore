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
	public class ReportMargins
	{
		private double topField;

		private double bottomField;

		private double leftField;

		private double rightField;

		public double Top
		{
			get
			{
				return topField;
			}
			set
			{
				topField = value;
			}
		}

		public double Bottom
		{
			get
			{
				return bottomField;
			}
			set
			{
				bottomField = value;
			}
		}

		public double Left
		{
			get
			{
				return leftField;
			}
			set
			{
				leftField = value;
			}
		}

		public double Right
		{
			get
			{
				return rightField;
			}
			set
			{
				rightField = value;
			}
		}
	}
}
