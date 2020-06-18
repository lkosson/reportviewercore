namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapSize : ReportObject
	{
		internal class Definition : DefinitionStore<MapSize, Definition.Properties>
		{
			internal enum Properties
			{
				Width,
				Height,
				Unit,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression<double> Width
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

		public ReportExpression<double> Height
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

		[ReportExpressionDefaultValue(typeof(MapUnits), MapUnits.Percentage)]
		public ReportExpression<MapUnits> Unit
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapUnits>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapSize()
		{
		}

		internal MapSize(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Unit = MapUnits.Percentage;
		}
	}
}
