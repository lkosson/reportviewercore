namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class CapImage : BaseGaugeImage
	{
		internal new class Definition : DefinitionStore<CapImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
				HueColor,
				OffsetX,
				OffsetY,
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

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> OffsetX
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> OffsetY
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public CapImage()
		{
		}

		internal CapImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
