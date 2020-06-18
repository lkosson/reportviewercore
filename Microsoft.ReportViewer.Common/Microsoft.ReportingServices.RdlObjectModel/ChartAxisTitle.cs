namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartAxisTitle : ReportObject
	{
		internal class Definition : DefinitionStore<ChartAxisTitle, Definition.Properties>
		{
			internal enum Properties
			{
				Caption,
				CaptionLocID,
				Position,
				Style,
				TextOrientation,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression Caption
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartAxisTitlePositions), ChartAxisTitlePositions.Center)]
		public ReportExpression<ChartAxisTitlePositions> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartAxisTitlePositions>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextOrientations), TextOrientations.Auto)]
		public ReportExpression<TextOrientations> TextOrientation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextOrientations>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public ChartAxisTitle()
		{
		}

		internal ChartAxisTitle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
