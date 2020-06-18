namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TopImage : BaseGaugeImage
	{
		internal new class Definition : DefinitionStore<TopImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
				HueColor,
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

		public TopImage()
		{
		}

		internal TopImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
