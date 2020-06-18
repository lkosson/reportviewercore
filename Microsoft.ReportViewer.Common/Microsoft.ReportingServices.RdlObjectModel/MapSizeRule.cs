namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapSizeRule : MapAppearanceRule
	{
		internal new class Definition : DefinitionStore<MapSizeRule, Definition.Properties>
		{
			internal enum Properties
			{
				DataValue,
				DistributionType,
				BucketCount,
				StartValue,
				EndValue,
				MapBuckets,
				LegendName,
				LegendText,
				DataElementName,
				DataElementOutput,
				StartSize,
				EndSize,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression<ReportSize> StartSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public ReportExpression<ReportSize> EndSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapSizeRule()
		{
		}

		internal MapSizeRule(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
