using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("MapTileLayer", typeof(MapTileLayer))]
	[XmlElementClass("MapPolygonLayer", typeof(MapPolygonLayer))]
	[XmlElementClass("MapPointLayer", typeof(MapPointLayer))]
	[XmlElementClass("MapLineLayer", typeof(MapLineLayer))]
	internal abstract class MapLayer : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<MapLayer, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				VisibilityMode,
				MinimumZoom,
				MaximumZoom,
				Transparency
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
				return base.PropertyStore.GetObject<string>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapVisibilityModes), MapVisibilityModes.Visible)]
		public ReportExpression<MapVisibilityModes> VisibilityMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapVisibilityModes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "50")]
		public ReportExpression<double> MinimumZoom
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "200")]
		public ReportExpression<double> MaximumZoom
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "0")]
		public ReportExpression<double> Transparency
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public MapLayer()
		{
		}

		internal MapLayer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			VisibilityMode = MapVisibilityModes.Visible;
			MinimumZoom = 50.0;
			MaximumZoom = 200.0;
			Transparency = 0.0;
		}
	}
}
