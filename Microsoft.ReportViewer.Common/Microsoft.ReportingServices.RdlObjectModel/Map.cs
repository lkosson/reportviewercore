using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Map : ReportItem
	{
		internal new class Definition : DefinitionStore<Map, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				MapViewport,
				MapDataRegions,
				MapLayers,
				MapLegends,
				MapTitles,
				MapDistanceScale,
				MapColorScale,
				MapBorderSkin,
				PageBreak,
				AntiAliasing,
				TextAntiAliasingQuality,
				ShadowIntensity,
				MaximumSpatialElementCount,
				MaximumTotalPointCount,
				TileLanguage,
				PropertyCount,
				PageName
			}

			private Definition()
			{
			}
		}

		public MapViewport MapViewport
		{
			get
			{
				return (MapViewport)base.PropertyStore.GetObject(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapDataRegion>))]
		public IList<MapDataRegion> MapDataRegions
		{
			get
			{
				return (IList<MapDataRegion>)base.PropertyStore.GetObject(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapLayer>))]
		public IList<MapLayer> MapLayers
		{
			get
			{
				return (IList<MapLayer>)base.PropertyStore.GetObject(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapLegend>))]
		public IList<MapLegend> MapLegends
		{
			get
			{
				return (IList<MapLegend>)base.PropertyStore.GetObject(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapTitle>))]
		public IList<MapTitle> MapTitles
		{
			get
			{
				return (IList<MapTitle>)base.PropertyStore.GetObject(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		public MapDistanceScale MapDistanceScale
		{
			get
			{
				return (MapDistanceScale)base.PropertyStore.GetObject(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		public MapColorScale MapColorScale
		{
			get
			{
				return (MapColorScale)base.PropertyStore.GetObject(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
			}
		}

		public MapBorderSkin MapBorderSkin
		{
			get
			{
				return (MapBorderSkin)base.PropertyStore.GetObject(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				return (PageBreak)base.PropertyStore.GetObject(26);
			}
			set
			{
				base.PropertyStore.SetObject(26, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression PageName
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(34);
			}
			set
			{
				base.PropertyStore.SetObject(34, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapAntiAliasings), MapAntiAliasings.All)]
		public ReportExpression<MapAntiAliasings> AntiAliasing
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapAntiAliasings>>(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapTextAntiAliasingQualities), MapTextAntiAliasingQualities.High)]
		public ReportExpression<MapTextAntiAliasingQualities> TextAntiAliasingQuality
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapTextAntiAliasingQualities>>(28);
			}
			set
			{
				base.PropertyStore.SetObject(28, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "25")]
		public ReportExpression<double> ShadowIntensity
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(29);
			}
			set
			{
				base.PropertyStore.SetObject(29, value);
			}
		}

		[DefaultValue(20000)]
		public int MaximumSpatialElementCount
		{
			get
			{
				return base.PropertyStore.GetInteger(30);
			}
			set
			{
				base.PropertyStore.SetInteger(30, value);
			}
		}

		[DefaultValue(1000000)]
		public int MaximumTotalPointCount
		{
			get
			{
				return base.PropertyStore.GetInteger(31);
			}
			set
			{
				base.PropertyStore.SetInteger(31, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression TileLanguage
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(32);
			}
			set
			{
				base.PropertyStore.SetObject(32, value);
			}
		}

		public Map()
		{
		}

		internal Map(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapViewport = new MapViewport();
			MapLayers = new RdlCollection<MapLayer>();
			MapLegends = new RdlCollection<MapLegend>();
			MapTitles = new RdlCollection<MapTitle>();
			AntiAliasing = MapAntiAliasings.All;
			TextAntiAliasingQuality = MapTextAntiAliasingQualities.High;
			ShadowIntensity = 25.0;
			MaximumSpatialElementCount = 20000;
			MaximumTotalPointCount = 1000000;
		}
	}
}
