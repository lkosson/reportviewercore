namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartAlignType : ReportObject
	{
		internal class Definition : DefinitionStore<ChartAlignType, Definition.Properties>
		{
			internal enum Properties
			{
				AxesView,
				Cursor,
				Position,
				InnerPlotPosition
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> AxesView
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Cursor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> InnerPlotPosition
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ChartAlignType()
		{
		}

		internal ChartAlignType(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
