using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapTile : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<MapTile, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				TileData,
				MIMEType,
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

		public string TileData
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

		public string MIMEType
		{
			get
			{
				return base.PropertyStore.GetObject<string>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapTile()
		{
		}

		internal MapTile(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
