namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartGridLines : ReportObject
	{
		internal class Definition : DefinitionStore<ChartGridLines, Definition.Properties>
		{
			internal enum Properties
			{
				Hidden,
				Style,
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

		[ReportExpressionDefaultValue(typeof(ChartGridLinesEnabledTypes), ChartGridLinesEnabledTypes.Auto)]
		public ReportExpression<ChartGridLinesEnabledTypes> Enabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartGridLinesEnabledTypes>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Interval
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

		[ReportExpressionDefaultValue(typeof(ChartIntervalTypes), ChartIntervalTypes.Default)]
		public ReportExpression<ChartIntervalTypes> IntervalType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalTypes>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> IntervalOffset
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

		[ReportExpressionDefaultValue(typeof(ChartIntervalOffsetTypes), ChartIntervalOffsetTypes.Default)]
		public ReportExpression<ChartIntervalOffsetTypes> IntervalOffsetType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalOffsetTypes>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public ChartGridLines()
		{
		}

		internal ChartGridLines(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
