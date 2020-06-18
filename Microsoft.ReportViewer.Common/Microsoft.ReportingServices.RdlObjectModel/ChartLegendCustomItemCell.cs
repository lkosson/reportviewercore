using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartLegendCustomItemCell : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartLegendCustomItemCell, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				CellType,
				Text,
				CellSpan,
				Style,
				ActionInfo,
				ToolTip,
				ToolTipLocID,
				ImageHeight,
				ImageWidth,
				SymbolHeight,
				SymbolWidth,
				Alignment,
				TopMargin,
				BottomMargin,
				LeftMargin,
				RightMargin,
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

		public ChartLegendItemCellTypes CellType
		{
			get
			{
				return (ChartLegendItemCellTypes)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Text
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

		[DefaultValue(1)]
		[ValidValues(0, int.MaxValue)]
		public int CellSpan
		{
			get
			{
				return base.PropertyStore.GetInteger(3);
			}
			set
			{
				((IntProperty)DefinitionStore<ChartLegendCustomItemCell, Definition.Properties>.GetProperty(3)).Validate(this, value);
				base.PropertyStore.SetInteger(3, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
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

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> ImageHeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> ImageWidth
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

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> SymbolHeight
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

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> SymbolWidth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[DefaultValue(ChartLegendItemAlignmentTypes.Center)]
		public ChartLegendItemAlignmentTypes Alignment
		{
			get
			{
				return (ChartLegendItemAlignmentTypes)base.PropertyStore.GetInteger(12);
			}
			set
			{
				base.PropertyStore.SetInteger(12, (int)value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> TopMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> BottomMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> LeftMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> RightMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public ChartLegendCustomItemCell()
		{
		}

		internal ChartLegendCustomItemCell(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
