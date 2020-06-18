namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class PinLabel : ReportObject
	{
		internal class Definition : DefinitionStore<PinLabel, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Text,
				AllowUpsideDown,
				DistanceFromScale,
				FontAngle,
				Placement,
				RotateLabel,
				UseFontPercent,
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
		public ReportExpression Text
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> AllowUpsideDown
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 2.0)]
		public ReportExpression<double> DistanceFromScale
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
		public ReportExpression<double> FontAngle
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

		[ReportExpressionDefaultValue(typeof(Placements), Placements.Inside)]
		public ReportExpression<Placements> Placement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<Placements>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> RotateLabel
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UseFontPercent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public PinLabel()
		{
		}

		internal PinLabel(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Placement = Placements.Inside;
			DistanceFromScale = 2.0;
		}
	}
}
