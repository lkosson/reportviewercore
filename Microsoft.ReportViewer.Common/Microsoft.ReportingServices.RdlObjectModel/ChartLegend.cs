using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartLegend : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartLegend, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Hidden,
				Style,
				Position,
				Layout,
				Docking,
				DockToChartArea,
				DockOutsideChartArea,
				ChartElementPosition,
				ChartLegendTitle,
				AutoFitTextDisabled,
				MinFontSize,
				ChartLegendColumns,
				HeaderSeparator,
				HeaderSeparatorColor,
				ColumnSeparator,
				ColumnSeparatorColor,
				ColumnSpacing,
				InterlacedRows,
				InterlacedRowsColor,
				EquallySpacedItems,
				Reversed,
				MaxAutoSize,
				TextWrapThreshold,
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
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

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartPositions), ChartPositions.RightTop)]
		public ReportExpression<ChartPositions> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartPositions>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartLegendLayouts), ChartLegendLayouts.AutoTable)]
		public ReportExpression<ChartLegendLayouts> Layout
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartLegendLayouts>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[DefaultValue("")]
		public string DockToChartArea
		{
			get
			{
				return (string)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> DockOutsideChartArea
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				return (ChartElementPosition)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public ChartLegendTitle ChartLegendTitle
		{
			get
			{
				return (ChartLegendTitle)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> AutoFitTextDisabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "7pt")]
		public ReportExpression<ReportSize> MinFontSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartLegendColumn>))]
		public IList<ChartLegendColumn> ChartLegendColumns
		{
			get
			{
				return (IList<ChartLegendColumn>)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartHeaderSeparatorTypes), ChartHeaderSeparatorTypes.None)]
		public ReportExpression<ChartHeaderSeparatorTypes> HeaderSeparator
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartHeaderSeparatorTypes>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> HeaderSeparatorColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartColumnSeparatorTypes), ChartColumnSeparatorTypes.None)]
		public ReportExpression<ChartColumnSeparatorTypes> ColumnSeparator
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartColumnSeparatorTypes>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> ColumnSeparatorColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 50)]
		public ReportExpression<int> ColumnSpacing
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> InterlacedRows
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> InterlacedRowsColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> EquallySpacedItems
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartLegendReversedTypes), ChartLegendReversedTypes.Auto)]
		public ReportExpression<ChartLegendReversedTypes> Reversed
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartLegendReversedTypes>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 50)]
		public ReportExpression<int> MaxAutoSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 25)]
		public ReportExpression<int> TextWrapThreshold
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		public ChartLegend()
		{
		}

		internal ChartLegend(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartLegendColumns = new RdlCollection<ChartLegendColumn>();
			Position = ChartPositions.RightTop;
			Layout = ChartLegendLayouts.AutoTable;
			ColumnSpacing = 50;
			MaxAutoSize = 50;
			TextWrapThreshold = 25;
		}
	}
}
