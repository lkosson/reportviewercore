using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapPolygonLayer : MapVectorLayer
	{
		internal new class Definition : DefinitionStore<MapPolygonLayer, Definition.Properties>
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
				MapPolygonTemplate,
				MapPolygonRules,
				MapCenterPointTemplate,
				MapCenterPointRules,
				MapPolygons,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				return (MapPolygonTemplate)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapPolygonRules MapPolygonRules
		{
			get
			{
				return (MapPolygonRules)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				return (MapPointTemplate)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public MapPointRules MapCenterPointRules
		{
			get
			{
				return (MapPointRules)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapPolygon>))]
		public IList<MapPolygon> MapPolygons
		{
			get
			{
				return (IList<MapPolygon>)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public MapPolygonLayer()
		{
		}

		internal MapPolygonLayer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapPolygons = new RdlCollection<MapPolygon>();
		}
	}
}
