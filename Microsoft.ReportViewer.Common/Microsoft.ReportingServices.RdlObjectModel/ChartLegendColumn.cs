using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartLegendColumn : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartLegendColumn, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				ColumnType,
				Value,
				Style,
				ActionInfo,
				ToolTip,
				ToolTipLocID,
				MinimumWidth,
				MaximumWidth,
				SeriesSymbolWidth,
				SeriesSymbolHeight,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ChartLegendColumnTypes ColumnType
		{
			get
			{
				return (ChartLegendColumnTypes)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Value
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

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
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

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> MinimumWidth
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

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> MaximumWidth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 200)]
		public ReportExpression<int> SeriesSymbolWidth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 70)]
		public ReportExpression<int> SeriesSymbolHeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public ChartLegendColumn()
		{
		}

		internal ChartLegendColumn(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			SeriesSymbolWidth = 200;
			SeriesSymbolHeight = 70;
		}
	}
}
