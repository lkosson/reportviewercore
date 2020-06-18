namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartTickMarks : ReportObject
	{
		internal class Definition : DefinitionStore<ChartTickMarks, Definition.Properties>
		{
			internal enum Properties
			{
				Enabled,
				Type,
				Style,
				Length,
				Interval,
				IntervalType,
				IntervalOffset,
				IntervalOffsetType,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartTickMarksEnabledTypes), ChartTickMarksEnabledTypes.Auto)]
		public ReportExpression<ChartTickMarksEnabledTypes> Enabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartTickMarksEnabledTypes>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartTickMarkTypes), ChartTickMarkTypes.Outside)]
		public ReportExpression<ChartTickMarkTypes> Type
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartTickMarkTypes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 1.0)]
		public ReportExpression<double> Length
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

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Interval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartIntervalTypes), ChartIntervalTypes.Default)]
		public ReportExpression<ChartIntervalTypes> IntervalType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalTypes>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> IntervalOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartIntervalOffsetTypes), ChartIntervalOffsetTypes.Default)]
		public ReportExpression<ChartIntervalOffsetTypes> IntervalOffsetType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalOffsetTypes>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public ChartTickMarks()
		{
		}

		internal ChartTickMarks(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Length = 1.0;
		}
	}
}
