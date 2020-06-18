using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class MapVectorLayer : MapLayer
	{
		internal new class Definition : DefinitionStore<MapVectorLayer, Definition.Properties>
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
				DataElementOutput
			}

			private Definition()
			{
			}
		}

		public string MapDataRegionName
		{
			get
			{
				return base.PropertyStore.GetObject<string>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapBindingFieldPair>))]
		public IList<MapBindingFieldPair> MapBindingFieldPairs
		{
			get
			{
				return (IList<MapBindingFieldPair>)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapFieldDefinition>))]
		public IList<MapFieldDefinition> MapFieldDefinitions
		{
			get
			{
				return (IList<MapFieldDefinition>)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public MapSpatialData MapSpatialData
		{
			get
			{
				return (MapSpatialData)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues("MapDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(10);
			}
			set
			{
				((EnumProperty)DefinitionStore<MapVectorLayer, Definition.Properties>.GetProperty(10)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(10, (int)value);
			}
		}

		public MapVectorLayer()
		{
		}

		internal MapVectorLayer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapBindingFieldPairs = new RdlCollection<MapBindingFieldPair>();
			MapFieldDefinitions = new RdlCollection<MapFieldDefinition>();
			DataElementOutput = DataElementOutputTypes.Output;
		}
	}
}
