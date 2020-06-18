using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLineLayer : MapVectorLayer
	{
		internal new class Definition : DefinitionStore<MapLineLayer, Definition.Properties>
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
				MapLineTemplate,
				MapLineRules,
				MapLines,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapLineTemplate MapLineTemplate
		{
			get
			{
				return (MapLineTemplate)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapLineRules MapLineRules
		{
			get
			{
				return (MapLineRules)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapLine>))]
		public IList<MapLine> MapLines
		{
			get
			{
				return (IList<MapLine>)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public MapLineLayer()
		{
		}

		internal MapLineLayer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapLines = new RdlCollection<MapLine>();
		}
	}
}
