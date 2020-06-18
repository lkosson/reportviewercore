namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLimits : ReportObject
	{
		internal class Definition : DefinitionStore<MapLimits, Definition.Properties>
		{
			internal enum Properties
			{
				MinimumX,
				MinimumY,
				MaximumX,
				MaximumY,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression<double> MinimumX
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression<double> MinimumY
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ReportExpression<double> MaximumX
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

		public ReportExpression<double> MaximumY
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

		public MapLimits()
		{
		}

		internal MapLimits(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
