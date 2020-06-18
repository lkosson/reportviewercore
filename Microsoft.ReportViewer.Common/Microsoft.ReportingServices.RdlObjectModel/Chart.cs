using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Chart : DataRegion
	{
		internal new class Definition : DefinitionStore<Chart, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				KeepTogether,
				NoRowsMessage,
				DataSetName,
				PageBreak,
				PageName,
				Filters,
				SortExpressions,
				ChartCategoryHierarchy,
				ChartSeriesHierarchy,
				ChartData,
				ChartAreas,
				ChartLegends,
				ChartTitles,
				Palette,
				ChartCustomPaletteColors,
				PaletteHatchBehavior,
				DynamicHeight,
				DynamicWidth,
				ChartBorderSkin,
				ChartNoDataMessage,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ChartCategoryHierarchy ChartCategoryHierarchy
		{
			get
			{
				return (ChartCategoryHierarchy)base.PropertyStore.GetObject(25);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(25, value);
			}
		}

		public ChartSeriesHierarchy ChartSeriesHierarchy
		{
			get
			{
				return (ChartSeriesHierarchy)base.PropertyStore.GetObject(26);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(26, value);
			}
		}

		public ChartData ChartData
		{
			get
			{
				return (ChartData)base.PropertyStore.GetObject(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartArea>))]
		public IList<ChartArea> ChartAreas
		{
			get
			{
				return (IList<ChartArea>)base.PropertyStore.GetObject(28);
			}
			set
			{
				base.PropertyStore.SetObject(28, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartLegend>))]
		public IList<ChartLegend> ChartLegends
		{
			get
			{
				return (IList<ChartLegend>)base.PropertyStore.GetObject(29);
			}
			set
			{
				base.PropertyStore.SetObject(29, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartTitle>))]
		public IList<ChartTitle> ChartTitles
		{
			get
			{
				return (IList<ChartTitle>)base.PropertyStore.GetObject(30);
			}
			set
			{
				base.PropertyStore.SetObject(30, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartPalettes), ChartPalettes.Default)]
		public ReportExpression<ChartPalettes> Palette
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartPalettes>>(31);
			}
			set
			{
				base.PropertyStore.SetObject(31, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportExpression<ReportColor>>))]
		[XmlArrayItem("ChartCustomPaletteColor", typeof(ReportExpression<ReportColor>))]
		public IList<ReportExpression<ReportColor>> ChartCustomPaletteColors
		{
			get
			{
				return (IList<ReportExpression<ReportColor>>)base.PropertyStore.GetObject(32);
			}
			set
			{
				base.PropertyStore.SetObject(32, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartPaletteHatchBehaviorTypes), ChartPaletteHatchBehaviorTypes.Default)]
		public ReportExpression<ChartPaletteHatchBehaviorTypes> PaletteHatchBehavior
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartPaletteHatchBehaviorTypes>>(33);
			}
			set
			{
				base.PropertyStore.SetObject(33, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> DynamicHeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(34);
			}
			set
			{
				base.PropertyStore.SetObject(34, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> DynamicWidth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(35);
			}
			set
			{
				base.PropertyStore.SetObject(35, value);
			}
		}

		public ChartBorderSkin ChartBorderSkin
		{
			get
			{
				return (ChartBorderSkin)base.PropertyStore.GetObject(36);
			}
			set
			{
				base.PropertyStore.SetObject(36, value);
			}
		}

		public ChartTitle ChartNoDataMessage
		{
			get
			{
				return (ChartTitle)base.PropertyStore.GetObject(37);
			}
			set
			{
				base.PropertyStore.SetObject(37, value);
			}
		}

		public Chart()
		{
		}

		internal Chart(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartCategoryHierarchy = new ChartCategoryHierarchy();
			ChartSeriesHierarchy = new ChartSeriesHierarchy();
			ChartAreas = new RdlCollection<ChartArea>();
			ChartLegends = new RdlCollection<ChartLegend>();
			ChartTitles = new RdlCollection<ChartTitle>();
			ChartCustomPaletteColors = new RdlCollection<ReportExpression<ReportColor>>();
		}
	}
}
