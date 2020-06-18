namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLineRules : ReportObject
	{
		internal class Definition : DefinitionStore<MapLineRules, Definition.Properties>
		{
			internal enum Properties
			{
				MapSizeRule,
				MapColorRule,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapSizeRule MapSizeRule
		{
			get
			{
				return (MapSizeRule)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public MapColorRule MapColorRule
		{
			get
			{
				return (MapColorRule)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapLineRules()
		{
		}

		internal MapLineRules(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
