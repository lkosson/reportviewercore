namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class PointerImage : BaseGaugeImage
	{
		internal new class Definition : DefinitionStore<PointerImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
				HueColor,
				Transparency,
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

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> OffsetX
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

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> OffsetY
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public PointerImage()
		{
		}

		internal PointerImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
