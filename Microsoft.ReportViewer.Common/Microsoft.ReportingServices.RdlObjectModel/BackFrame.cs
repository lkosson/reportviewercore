namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class BackFrame : ReportObject
	{
		internal class Definition : DefinitionStore<BackFrame, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				FrameStyle,
				FrameShape,
				FrameWidth,
				GlassEffect,
				FrameBackground,
				FrameImage,
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

		[ReportExpressionDefaultValue(typeof(FrameStyles), FrameStyles.None)]
		public ReportExpression<FrameStyles> FrameStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<FrameStyles>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(FrameShapes), FrameShapes.Default)]
		public ReportExpression<FrameShapes> FrameShape
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<FrameShapes>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ValidValues(0.0, 50.0)]
		[ReportExpressionDefaultValue(typeof(double), 8.0)]
		public ReportExpression<double> FrameWidth
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

		[ReportExpressionDefaultValue(typeof(GlassEffects), GlassEffects.None)]
		public ReportExpression<GlassEffects> GlassEffect
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<GlassEffects>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public FrameBackground FrameBackground
		{
			get
			{
				return (FrameBackground)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public FrameImage FrameImage
		{
			get
			{
				return (FrameImage)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public BackFrame()
		{
		}

		internal BackFrame(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			FrameWidth = 8.0;
		}
	}
}
