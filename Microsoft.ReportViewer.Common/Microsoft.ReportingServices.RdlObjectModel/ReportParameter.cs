using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ReportParameter : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ReportParameter, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				DataType,
				Nullable,
				DefaultValue,
				AllowBlank,
				Prompt,
				PromptLocID,
				Hidden,
				ValidValues,
				MultiValue,
				UsedInQuery
			}

			private Definition()
			{
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public DataTypes DataType
		{
			get
			{
				return (DataTypes)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		[DefaultValue(false)]
		public bool Nullable
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		public DefaultValue DefaultValue
		{
			get
			{
				return (DefaultValue)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[DefaultValue(false)]
		public bool AllowBlank
		{
			get
			{
				return base.PropertyStore.GetBoolean(4);
			}
			set
			{
				base.PropertyStore.SetBoolean(4, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Prompt
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[DefaultValue(false)]
		public bool Hidden
		{
			get
			{
				return base.PropertyStore.GetBoolean(7);
			}
			set
			{
				base.PropertyStore.SetBoolean(7, value);
			}
		}

		public ValidValues ValidValues
		{
			get
			{
				return (ValidValues)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[DefaultValue(false)]
		public bool MultiValue
		{
			get
			{
				return base.PropertyStore.GetBoolean(9);
			}
			set
			{
				base.PropertyStore.SetBoolean(9, value);
			}
		}

		[DefaultValue(UsedInQueryTypes.Auto)]
		public UsedInQueryTypes UsedInQuery
		{
			get
			{
				return (UsedInQueryTypes)base.PropertyStore.GetInteger(10);
			}
			set
			{
				base.PropertyStore.SetInteger(10, (int)value);
			}
		}

		public ReportParameter()
		{
		}

		internal ReportParameter(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
