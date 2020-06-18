namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapDataBoundView : MapView
	{
		internal new class Definition : DefinitionStore<MapDataBoundView, Definition.Properties>
		{
			internal enum Properties
			{
				Zoom,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapDataBoundView()
		{
		}

		internal MapDataBoundView(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
