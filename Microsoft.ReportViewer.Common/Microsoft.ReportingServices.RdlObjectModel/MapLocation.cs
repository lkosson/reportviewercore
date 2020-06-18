namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLocation : ReportObject
	{
		internal class Definition : DefinitionStore<MapLocation, Definition.Properties>
		{
			internal enum Properties
			{
				Left,
				Top,
				Unit,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "0")]
		public ReportExpression<double> Left
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

		[ReportExpressionDefaultValue(typeof(double), "0")]
		public ReportExpression<double> Top
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

		public MapLocation()
		{
		}

		internal MapLocation(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Left = 0.0;
			Top = 0.0;
			Unit = MapUnits.Percentage;
		}
	}
}
