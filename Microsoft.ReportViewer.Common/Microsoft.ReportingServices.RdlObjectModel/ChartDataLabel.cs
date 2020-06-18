namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartDataLabel : ReportObject
	{
		internal class Definition : DefinitionStore<ChartDataLabel, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Label,
				LabelLocID,
				UseValueAsLabel,
				Visible,
				Position,
				Rotation,
				ToolTip,
				ActionInfo,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Label
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UseValueAsLabel
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Visible
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartDataLabelPositions), ChartDataLabelPositions.Auto)]
		public ReportExpression<ChartDataLabelPositions> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartDataLabelPositions>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> Rotation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
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

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public ChartDataLabel()
		{
		}

		internal ChartDataLabel(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
