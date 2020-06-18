namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class GaugeLabel : GaugePanelItem
	{
		internal new class Definition : DefinitionStore<GaugeLabel, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Style,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Hidden,
				ToolTip,
				ActionInfo,
				ParentItem,
				Text,
				Angle,
				ResizeMode,
				TextShadowOffset,
				UseFontPercent,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Text
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Angle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ResizeModes), ResizeModes.AutoFit)]
		public ReportExpression<ResizeModes> ResizeMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ResizeModes>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> TextShadowOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UseFontPercent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public GaugeLabel()
		{
		}

		internal GaugeLabel(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
