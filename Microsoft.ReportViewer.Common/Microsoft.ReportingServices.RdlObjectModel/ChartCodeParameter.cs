using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartCodeParameter : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartCodeParameter, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Value,
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

		public ChartCodeParameter()
		{
		}

		internal ChartCodeParameter(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
