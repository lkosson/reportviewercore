using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapField : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<MapField, Definition.Properties>
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

		[XmlElement("Name")]
		public string Name
		{
			get
			{
				return base.PropertyStore.GetObject<string>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public string Value
		{
			get
			{
				return base.PropertyStore.GetObject<string>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapField()
		{
		}

		internal MapField(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
