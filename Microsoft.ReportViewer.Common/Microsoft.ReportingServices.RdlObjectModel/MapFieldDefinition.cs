using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapFieldDefinition : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<MapFieldDefinition, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				DataType,
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

		public MapDataTypes DataType
		{
			get
			{
				return (MapDataTypes)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		public MapFieldDefinition()
		{
		}

		internal MapFieldDefinition(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
