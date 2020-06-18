using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartFormulaParameter : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartFormulaParameter, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Value,
				Source,
				PropertyCount
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

		[ReportExpressionDefaultValue]
		public ReportExpression Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValue("")]
		public string Source
		{
			get
			{
				return (string)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ChartFormulaParameter()
		{
		}

		internal ChartFormulaParameter(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
