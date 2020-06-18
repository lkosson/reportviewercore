namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapBorderSkin : ReportObject
	{
		internal class Definition : DefinitionStore<MapBorderSkin, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				MapBorderSkinType,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapBorderSkinTypes), MapBorderSkinTypes.None)]
		public ReportExpression<MapBorderSkinTypes> MapBorderSkinType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapBorderSkinTypes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapBorderSkin()
		{
		}

		internal MapBorderSkin(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapBorderSkinType = MapBorderSkinTypes.None;
		}
	}
}
