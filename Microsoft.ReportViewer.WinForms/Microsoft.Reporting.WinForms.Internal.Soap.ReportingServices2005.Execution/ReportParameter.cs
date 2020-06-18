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
	public class ReportParameter
	{
		private string nameField;

		private ParameterTypeEnum typeField;

		private bool typeFieldSpecified;

		private bool nullableField;

		private bool nullableFieldSpecified;

		private bool allowBlankField;

		private bool allowBlankFieldSpecified;

		private bool multiValueField;

		private bool multiValueFieldSpecified;

		private bool queryParameterField;

		private bool queryParameterFieldSpecified;

		private string promptField;

		private bool promptUserField;

		private bool promptUserFieldSpecified;

		private string[] dependenciesField;

		private bool validValuesQueryBasedField;

		private bool validValuesQueryBasedFieldSpecified;

		private ValidValue[] validValuesField;

		private bool defaultValuesQueryBasedField;

		private bool defaultValuesQueryBasedFieldSpecified;

		private string[] defaultValuesField;

		private ParameterStateEnum stateField;

		private bool stateFieldSpecified;

		private string errorMessageField;

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

		public ParameterTypeEnum Type
		{
			get
			{
				return typeField;
			}
			set
			{
				typeField = value;
			}
		}

		[XmlIgnore]
		public bool TypeSpecified
		{
			get
			{
				return typeFieldSpecified;
			}
			set
			{
				typeFieldSpecified = value;
			}
		}

		public bool Nullable
		{
			get
			{
				return nullableField;
			}
			set
			{
				nullableField = value;
			}
		}

		[XmlIgnore]
		public bool NullableSpecified
		{
			get
			{
				return nullableFieldSpecified;
			}
			set
			{
				nullableFieldSpecified = value;
			}
		}

		public bool AllowBlank
		{
			get
			{
				return allowBlankField;
			}
			set
			{
				allowBlankField = value;
			}
		}

		[XmlIgnore]
		public bool AllowBlankSpecified
		{
			get
			{
				return allowBlankFieldSpecified;
			}
			set
			{
				allowBlankFieldSpecified = value;
			}
		}

		public bool MultiValue
		{
			get
			{
				return multiValueField;
			}
			set
			{
				multiValueField = value;
			}
		}

		[XmlIgnore]
		public bool MultiValueSpecified
		{
			get
			{
				return multiValueFieldSpecified;
			}
			set
			{
				multiValueFieldSpecified = value;
			}
		}

		public bool QueryParameter
		{
			get
			{
				return queryParameterField;
			}
			set
			{
				queryParameterField = value;
			}
		}

		[XmlIgnore]
		public bool QueryParameterSpecified
		{
			get
			{
				return queryParameterFieldSpecified;
			}
			set
			{
				queryParameterFieldSpecified = value;
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

		public bool PromptUser
		{
			get
			{
				return promptUserField;
			}
			set
			{
				promptUserField = value;
			}
		}

		[XmlIgnore]
		public bool PromptUserSpecified
		{
			get
			{
				return promptUserFieldSpecified;
			}
			set
			{
				promptUserFieldSpecified = value;
			}
		}

		[XmlArrayItem("Dependency")]
		public string[] Dependencies
		{
			get
			{
				return dependenciesField;
			}
			set
			{
				dependenciesField = value;
			}
		}

		public bool ValidValuesQueryBased
		{
			get
			{
				return validValuesQueryBasedField;
			}
			set
			{
				validValuesQueryBasedField = value;
			}
		}

		[XmlIgnore]
		public bool ValidValuesQueryBasedSpecified
		{
			get
			{
				return validValuesQueryBasedFieldSpecified;
			}
			set
			{
				validValuesQueryBasedFieldSpecified = value;
			}
		}

		public ValidValue[] ValidValues
		{
			get
			{
				return validValuesField;
			}
			set
			{
				validValuesField = value;
			}
		}

		public bool DefaultValuesQueryBased
		{
			get
			{
				return defaultValuesQueryBasedField;
			}
			set
			{
				defaultValuesQueryBasedField = value;
			}
		}

		[XmlIgnore]
		public bool DefaultValuesQueryBasedSpecified
		{
			get
			{
				return defaultValuesQueryBasedFieldSpecified;
			}
			set
			{
				defaultValuesQueryBasedFieldSpecified = value;
			}
		}

		[XmlArrayItem("Value")]
		public string[] DefaultValues
		{
			get
			{
				return defaultValuesField;
			}
			set
			{
				defaultValuesField = value;
			}
		}

		public ParameterStateEnum State
		{
			get
			{
				return stateField;
			}
			set
			{
				stateField = value;
			}
		}

		[XmlIgnore]
		public bool StateSpecified
		{
			get
			{
				return stateFieldSpecified;
			}
			set
			{
				stateFieldSpecified = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return errorMessageField;
			}
			set
			{
				errorMessageField = value;
			}
		}
	}
}
