namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartAxisScaleBreak : ReportObject
	{
		internal class Definition : DefinitionStore<ChartAxisScaleBreak, Definition.Properties>
		{
			internal enum Properties
			{
				Enabled,
				BreakLineType,
				CollapsibleSpaceThreshold,
				MaxNumberOfBreaks,
				Spacing,
				IncludeZero,
				Style,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Enabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartBreakLineTypes), ChartBreakLineTypes.Ragged)]
		public ReportExpression<ChartBreakLineTypes> BreakLineType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartBreakLineTypes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 25)]
		public ReportExpression<int> CollapsibleSpaceThreshold
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 2)]
		public ReportExpression<int> MaxNumberOfBreaks
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 1.5)]
		public ReportExpression<double> Spacing
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

		[ReportExpressionDefaultValue(typeof(ChartIncludeZeroTypes), ChartIncludeZeroTypes.Auto)]
		public ReportExpression<ChartIncludeZeroTypes> IncludeZero
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIncludeZeroTypes>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public ChartAxisScaleBreak()
		{
		}

		internal ChartAxisScaleBreak(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Spacing = 1.5;
			CollapsibleSpaceThreshold = 25;
			MaxNumberOfBreaks = 2;
		}
	}
}
