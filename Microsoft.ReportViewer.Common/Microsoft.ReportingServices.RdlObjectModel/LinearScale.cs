namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class LinearScale : GaugeScale
	{
		internal new class Definition : DefinitionStore<LinearScale, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				GaugePointers,
				ScaleRanges,
				Style,
				CustomLabels,
				Interval,
				IntervalOffset,
				Logarithmic,
				LogarithmicBase,
				MaximumValue,
				MinimumValue,
				Multiplier,
				Reversed,
				GaugeMajorTickMarks,
				GaugeMinorTickMarks,
				MaximumPin,
				MinimumPin,
				ScaleLabels,
				TickMarksOnTop,
				ToolTip,
				ActionInfo,
				Hidden,
				Width,
				StartMargin,
				EndMargin,
				Position,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 8.0)]
		public ReportExpression<double> StartMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 8.0)]
		public ReportExpression<double> EndMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 50.0)]
		public ReportExpression<double> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		public LinearScale()
		{
		}

		internal LinearScale(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			StartMargin = 8.0;
			EndMargin = 8.0;
			Position = 50.0;
		}
	}
}
