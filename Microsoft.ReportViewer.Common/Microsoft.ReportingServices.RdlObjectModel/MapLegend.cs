using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLegend : MapDockableSubItem, INamedObject
	{
		internal new class Definition : DefinitionStore<MapLegend, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				MapLocation,
				MapSize,
				LeftMargin,
				RightMargin,
				TopMargin,
				BottomMargin,
				ZIndex,
				ActionInfo,
				MapPosition,
				DockOutsideViewport,
				Hidden,
				ToolTip,
				Name,
				Layout,
				MapLegendTitle,
				AutoFitTextDisabled,
				MinFontSize,
				InterlacedRows,
				InterlacedRowsColor,
				EquallySpacedItems,
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
				return base.PropertyStore.GetObject<string>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapLegendLayouts), MapLegendLayouts.AutoTable)]
		public ReportExpression<MapLegendLayouts> Layout
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapLegendLayouts>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public MapLegendTitle MapLegendTitle
		{
			get
			{
				return (MapLegendTitle)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public ReportExpression<bool> AutoFitTextDisabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "7pt")]
		public ReportExpression<ReportSize> MinFontSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

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

		[ReportExpressionDefaultValue(typeof(ReportColor), "LightGray")]
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

		[ReportExpressionDefaultValue(typeof(int), "25")]
		public ReportExpression<int> TextWrapThreshold
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		public MapLegend()
		{
		}

		internal MapLegend(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Layout = MapLegendLayouts.AutoTable;
			MinFontSize = new ReportExpression<ReportSize>("7pt", CultureInfo.InvariantCulture);
			InterlacedRowsColor = new ReportExpression<ReportColor>("LightGray", CultureInfo.InvariantCulture);
			TextWrapThreshold = 25;
		}
	}
}
