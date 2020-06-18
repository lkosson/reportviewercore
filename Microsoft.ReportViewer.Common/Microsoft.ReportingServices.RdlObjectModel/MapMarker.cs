namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapMarker : ReportObject
	{
		internal class Definition : DefinitionStore<MapMarker, Definition.Properties>
		{
			internal enum Properties
			{
				MapMarkerStyle,
				MapMarkerImage,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(MapMarkerStyles), MapMarkerStyles.None)]
		public ReportExpression<MapMarkerStyles> MapMarkerStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapMarkerStyles>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public MapMarkerImage MapMarkerImage
		{
			get
			{
				return (MapMarkerImage)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapMarker()
		{
		}

		internal MapMarker(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapMarkerStyle = MapMarkerStyles.None;
		}
	}
}
