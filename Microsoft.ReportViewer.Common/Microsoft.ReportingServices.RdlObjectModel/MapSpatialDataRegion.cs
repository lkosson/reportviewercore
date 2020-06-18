namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapSpatialDataRegion : MapSpatialData
	{
		internal class Definition : DefinitionStore<MapSpatialDataRegion, Definition.Properties>
		{
			internal enum Properties
			{
				VectorData,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression VectorData
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public MapSpatialDataRegion()
		{
		}

		internal MapSpatialDataRegion(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
