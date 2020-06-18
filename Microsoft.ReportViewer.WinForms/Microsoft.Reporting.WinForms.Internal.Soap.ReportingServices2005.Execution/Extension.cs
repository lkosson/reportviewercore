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
	public class Extension
	{
		private ExtensionTypeEnum extensionTypeField;

		private string nameField;

		private string localizedNameField;

		private bool visibleField;

		private bool isModelGenerationSupportedField;

		public ExtensionTypeEnum ExtensionType
		{
			get
			{
				return extensionTypeField;
			}
			set
			{
				extensionTypeField = value;
			}
		}

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

		public string LocalizedName
		{
			get
			{
				return localizedNameField;
			}
			set
			{
				localizedNameField = value;
			}
		}

		public bool Visible
		{
			get
			{
				return visibleField;
			}
			set
			{
				visibleField = value;
			}
		}

		public bool IsModelGenerationSupported
		{
			get
			{
				return isModelGenerationSupportedField;
			}
			set
			{
				isModelGenerationSupportedField = value;
			}
		}
	}
}
