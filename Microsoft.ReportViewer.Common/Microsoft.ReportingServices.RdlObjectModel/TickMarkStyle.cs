namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TickMarkStyle : ReportObject
	{
		internal class Definition : DefinitionStore<TickMarkStyle, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				DistanceFromScale,
				Placement,
				EnableGradient,
				GradientDensity,
				TickMarkImage,
				Length,
				Width,
				Shape,
				Hidden,
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

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> DistanceFromScale
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

		[ReportExpressionDefaultValue(typeof(Placements), Placements.Inside)]
		public ReportExpression<Placements> Placement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<Placements>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> EnableGradient
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ValidValues(0.0, 100.0)]
		[ReportExpressionDefaultValue(typeof(double), 30.0)]
		public ReportExpression<double> GradientDensity
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

		public TickMarkImage TickMarkImage
		{
			get
			{
				return (TickMarkImage)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public ReportExpression<double> Length
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

		public ReportExpression<double> Width
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MarkerStyles), MarkerStyles.Rectangle)]
		public ReportExpression<MarkerStyles> Shape
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MarkerStyles>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public TickMarkStyle()
		{
		}

		internal TickMarkStyle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Placement = Placements.Inside;
			Shape = MarkerStyles.Rectangle;
			GradientDensity = 30.0;
		}
	}
}
