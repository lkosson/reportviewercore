namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapPolygon : MapSpatialElement
	{
		internal new class Definition : DefinitionStore<MapPolygon, Definition.Properties>
		{
			internal enum Properties
			{
				VectorData,
				MapFields,
				UseCustomPolygonTemplate,
				MapPolygonTemplate,
				UseCustomCenterPointTemplate,
				MapCenterPointTemplate,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UseCustomPolygonTemplate
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				return (MapPolygonTemplate)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UseCustomCenterPointTemplate
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				return (MapPointTemplate)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public MapPolygon()
		{
		}

		internal MapPolygon(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
