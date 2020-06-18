namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapPolygonRules : ReportObject
	{
		internal class Definition : DefinitionStore<MapPolygonRules, Definition.Properties>
		{
			internal enum Properties
			{
				MapColorRule,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapColorRule MapColorRule
		{
			get
			{
				return (MapColorRule)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public MapPolygonRules()
		{
		}

		internal MapPolygonRules(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
