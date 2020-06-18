namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class IndicatorImage : BaseGaugeImage
	{
		internal new class Definition : DefinitionStore<IndicatorImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
				HueColor,
				Transparency,
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

		public IndicatorImage()
		{
		}

		internal IndicatorImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
