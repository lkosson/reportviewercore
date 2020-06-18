namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapColorScaleTitle : ReportObject
	{
		internal class Definition : DefinitionStore<MapColorScaleTitle, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Caption,
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

		public ReportExpression Caption
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapColorScaleTitle()
		{
		}

		internal MapColorScaleTitle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
