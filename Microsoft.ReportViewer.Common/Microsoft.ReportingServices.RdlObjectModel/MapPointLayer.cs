using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapPointLayer : MapVectorLayer
	{
		internal new class Definition : DefinitionStore<MapPointLayer, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				VisibilityMode,
				MinimumZoom,
				MaximumZoom,
				Transparency,
				MapDataRegionName,
				MapBindingFieldPairs,
				MapFieldDefinitions,
				MapSpatialData,
				DataElementName,
				DataElementOutput,
				MapPointTemplate,
				MapPointRules,
				MapPoints,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapPointTemplate MapPointTemplate
		{
			get
			{
				return (MapPointTemplate)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapPointRules MapPointRules
		{
			get
			{
				return (MapPointRules)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapPoint>))]
		public IList<MapPoint> MapPoints
		{
			get
			{
				return (IList<MapPoint>)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public MapPointLayer()
		{
		}

		internal MapPointLayer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapPoints = new RdlCollection<MapPoint>();
		}
	}
}
