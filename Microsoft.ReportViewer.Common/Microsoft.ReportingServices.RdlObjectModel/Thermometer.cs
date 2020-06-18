namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Thermometer : ReportObject
	{
		internal class Definition : DefinitionStore<Thermometer, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				BulbOffset,
				BulbSize,
				ThermometerStyle,
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

		[ReportExpressionDefaultValue(typeof(double), 5.0)]
		public ReportExpression<double> BulbOffset
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

		[ReportExpressionDefaultValue(typeof(double), 50.0)]
		public ReportExpression<double> BulbSize
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

		[ReportExpressionDefaultValue(typeof(ThermometerStyles), ThermometerStyles.Standard)]
		public ReportExpression<ThermometerStyles> ThermometerStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ThermometerStyles>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public Thermometer()
		{
		}

		internal Thermometer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			BulbOffset = 5.0;
			BulbSize = 50.0;
		}
	}
}
