namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartMarker : ReportObject
	{
		internal class Definition : DefinitionStore<ChartMarker, Definition.Properties>
		{
			internal enum Properties
			{
				Type,
				Size,
				Style,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartMarkerTypes), ChartMarkerTypes.None)]
		public ReportExpression<ChartMarkerTypes> Type
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartMarkerTypes>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "3.75pt")]
		public ReportExpression<ReportSize> Size
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public EmptyColorStyle Style
		{
			get
			{
				return (EmptyColorStyle)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ChartMarker()
		{
		}

		internal ChartMarker(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
