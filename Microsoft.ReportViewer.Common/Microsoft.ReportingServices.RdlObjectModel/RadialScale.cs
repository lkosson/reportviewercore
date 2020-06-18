namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class RadialScale : GaugeScale
	{
		internal new class Definition : DefinitionStore<RadialScale, Definition.Properties>
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
				Radius,
				StartAngle,
				SweepAngle,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ValidValues(0.0, double.MaxValue)]
		[ReportExpressionDefaultValue(typeof(double), 37.0)]
		public ReportExpression<double> Radius
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

		[ValidValues(0.0, 360.0)]
		[ReportExpressionDefaultValue(typeof(double), 20.0)]
		public ReportExpression<double> StartAngle
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

		[ValidValues(0.0, 360.0)]
		[ReportExpressionDefaultValue(typeof(double), 320.0)]
		public ReportExpression<double> SweepAngle
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

		public RadialScale()
		{
		}

		internal RadialScale(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Radius = 37.0;
			StartAngle = 20.0;
			SweepAngle = 320.0;
		}
	}
}
