namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartLegendColumnHeader : ReportObject
	{
		internal class Definition : DefinitionStore<ChartLegendColumnHeader, Definition.Properties>
		{
			internal enum Properties
			{
				Value,
				Style,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Value
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

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ChartLegendColumnHeader()
		{
		}

		internal ChartLegendColumnHeader(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
