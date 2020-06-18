namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartDataPointValues : ReportObject
	{
		internal class Definition : DefinitionStore<ChartDataPointValues, Definition.Properties>
		{
			internal enum Properties
			{
				X,
				Y,
				Size,
				High,
				Low,
				Start,
				End,
				Mean,
				Median,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression X
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

		[ReportExpressionDefaultValue]
		public ReportExpression Y
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Size
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression High
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Low
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Start
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression End
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Mean
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Median
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public ChartDataPointValues()
		{
		}

		internal ChartDataPointValues(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
