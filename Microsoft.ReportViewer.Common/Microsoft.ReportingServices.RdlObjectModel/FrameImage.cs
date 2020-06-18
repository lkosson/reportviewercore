namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class FrameImage : BaseGaugeImage
	{
		internal new class Definition : DefinitionStore<FrameImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
				HueColor,
				Transparency,
				ClipImage,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> HueColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Transparency
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> ClipImage
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

		public FrameImage()
		{
		}

		internal FrameImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
