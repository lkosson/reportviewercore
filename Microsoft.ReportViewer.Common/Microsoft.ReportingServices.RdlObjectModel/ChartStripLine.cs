namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartStripLine : ReportObject
	{
		internal class Definition : DefinitionStore<ChartStripLine, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Title,
				TextOrientation,
				ActionInfo,
				ToolTip,
				ToolTipLocID,
				Interval,
				IntervalType,
				IntervalOffset,
				IntervalOffsetType,
				StripWidth,
				StripWidthType,
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

		[ReportExpressionDefaultValue]
		public ReportExpression Title
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextOrientations), TextOrientations.Auto)]
		public ReportExpression<TextOrientations> TextOrientation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextOrientations>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Interval
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

		[ReportExpressionDefaultValue(typeof(ChartIntervalTypes), ChartIntervalTypes.Auto)]
		public ReportExpression<ChartIntervalTypes> IntervalType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalTypes>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> IntervalOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartIntervalOffsetTypes), ChartIntervalOffsetTypes.Auto)]
		public ReportExpression<ChartIntervalOffsetTypes> IntervalOffsetType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalOffsetTypes>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> StripWidth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartStripWidthTypes), ChartStripWidthTypes.Auto)]
		public ReportExpression<ChartStripWidthTypes> StripWidthType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartStripWidthTypes>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public ChartStripLine()
		{
		}

		internal ChartStripLine(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			IntervalType = ChartIntervalTypes.Auto;
			IntervalOffsetType = ChartIntervalOffsetTypes.Auto;
		}
	}
}
