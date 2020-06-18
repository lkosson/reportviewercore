using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Chart : DataRegion
	{
		private int m_memberCellDefinitionIndex;

		private ChartHierarchy m_categories;

		private ChartHierarchy m_series;

		private ChartData m_chartData;

		private ReportSizeProperty m_dynamicHeight;

		private ReportSizeProperty m_dynamicWidth;

		private ChartAreaCollection m_chartAreas;

		private ChartTitleCollection m_titles;

		private ChartLegendCollection m_legends;

		private ChartBorderSkin m_borderSkin;

		private ChartCustomPaletteColorCollection m_customPaletteColors;

		private ReportEnumProperty<ChartPalette> m_palette;

		private ReportEnumProperty<PaletteHatchBehavior> m_paletteHatchBehavior;

		private ChartTitle m_noDataMessage;

		public bool DataValueSequenceRendering
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return true;
				}
				return ChartDef.DataValueSequenceRendering;
			}
		}

		public ChartHierarchy CategoryHierarchy
		{
			get
			{
				if (m_categories == null)
				{
					m_categories = new ChartHierarchy(this, isColumn: true);
				}
				return m_categories;
			}
		}

		public ChartHierarchy SeriesHierarchy
		{
			get
			{
				if (m_series == null)
				{
					m_series = new ChartHierarchy(this, isColumn: false);
				}
				return m_series;
			}
		}

		public ChartData ChartData
		{
			get
			{
				if (m_chartData == null)
				{
					m_chartData = new ChartData(this);
				}
				return m_chartData;
			}
		}

		public int Categories
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderChart.CategoriesCount;
				}
				return ChartDef.CategoryCount;
			}
		}

		public int Series
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderChart.SeriesCount;
				}
				return ChartDef.SeriesCount;
			}
		}

		public ChartTitleCollection Titles
		{
			get
			{
				if (m_titles == null)
				{
					if (m_isOldSnapshot)
					{
						if (RenderChartDef.Title != null)
						{
							m_titles = new ChartTitleCollection(this);
						}
					}
					else if (ChartDef.Titles != null)
					{
						m_titles = new ChartTitleCollection(this);
					}
				}
				return m_titles;
			}
		}

		public ChartCustomPaletteColorCollection CustomPaletteColors
		{
			get
			{
				if (m_customPaletteColors == null && !m_isOldSnapshot && ChartDef.CustomPaletteColors != null)
				{
					m_customPaletteColors = new ChartCustomPaletteColorCollection(this);
				}
				return m_customPaletteColors;
			}
		}

		public ChartBorderSkin BorderSkin
		{
			get
			{
				if (m_borderSkin == null && !m_isOldSnapshot && ChartDef.BorderSkin != null)
				{
					m_borderSkin = new ChartBorderSkin(ChartDef.BorderSkin, this);
				}
				return m_borderSkin;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Chart ChartDef => m_reportItemDef as Microsoft.ReportingServices.ReportIntermediateFormat.Chart;

		internal override bool HasDataCells
		{
			get
			{
				if (m_chartData != null)
				{
					return m_chartData.HasSeriesCollection;
				}
				return false;
			}
		}

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				if (m_chartData != null)
				{
					return m_chartData.SeriesCollection;
				}
				return null;
			}
		}

		public ChartAreaCollection ChartAreas
		{
			get
			{
				if (m_chartAreas == null)
				{
					if (m_isOldSnapshot)
					{
						m_chartAreas = new ChartAreaCollection(this);
					}
					else if (ChartDef.ChartAreas != null)
					{
						m_chartAreas = new ChartAreaCollection(this);
					}
				}
				return m_chartAreas;
			}
		}

		public ChartLegendCollection Legends
		{
			get
			{
				if (m_legends == null)
				{
					if (m_isOldSnapshot)
					{
						if (RenderChartDef.Legend != null)
						{
							m_legends = new ChartLegendCollection(this);
						}
					}
					else if (ChartDef.Legends != null)
					{
						m_legends = new ChartLegendCollection(this);
					}
				}
				return m_legends;
			}
		}

		public ReportEnumProperty<ChartPalette> Palette
		{
			get
			{
				if (m_palette == null)
				{
					if (m_isOldSnapshot)
					{
						m_palette = new ReportEnumProperty<ChartPalette>(isExpression: false, null, (ChartPalette)RenderChartDef.Palette);
					}
					else if (ChartDef.Palette != null)
					{
						m_palette = new ReportEnumProperty<ChartPalette>(ChartDef.Palette.IsExpression, ChartDef.Palette.OriginalText, EnumTranslator.TranslateChartPalette(ChartDef.Palette.StringValue, null));
					}
				}
				return m_palette;
			}
		}

		public ReportEnumProperty<PaletteHatchBehavior> PaletteHatchBehavior
		{
			get
			{
				if (m_paletteHatchBehavior == null)
				{
					if (m_isOldSnapshot)
					{
						if (Microsoft.ReportingServices.ReportProcessing.Chart.ChartPalette.GrayScale == RenderChartDef.Palette)
						{
							m_paletteHatchBehavior = new ReportEnumProperty<PaletteHatchBehavior>(Microsoft.ReportingServices.OnDemandReportRendering.PaletteHatchBehavior.Always);
						}
					}
					else if (ChartDef.PaletteHatchBehavior != null)
					{
						m_paletteHatchBehavior = new ReportEnumProperty<PaletteHatchBehavior>(ChartDef.PaletteHatchBehavior.IsExpression, ChartDef.PaletteHatchBehavior.OriginalText, EnumTranslator.TranslatePaletteHatchBehavior(ChartDef.PaletteHatchBehavior.StringValue, null));
					}
				}
				return m_paletteHatchBehavior;
			}
		}

		public ReportSizeProperty DynamicHeight
		{
			get
			{
				if (m_dynamicHeight == null)
				{
					if (m_isOldSnapshot)
					{
						m_dynamicHeight = new ReportSizeProperty(isExpression: false, m_renderReportItem.ReportItemDef.Height, new ReportSize(m_renderReportItem.ReportItemDef.Height));
					}
					else if (ChartDef.DynamicHeight != null)
					{
						m_dynamicHeight = new ReportSizeProperty(ChartDef.DynamicHeight);
					}
					else
					{
						m_dynamicHeight = new ReportSizeProperty(isExpression: false, m_reportItemDef.Height, new ReportSize(m_reportItemDef.Height));
					}
				}
				return m_dynamicHeight;
			}
		}

		public ReportSizeProperty DynamicWidth
		{
			get
			{
				if (m_dynamicWidth == null)
				{
					if (m_isOldSnapshot)
					{
						m_dynamicWidth = new ReportSizeProperty(isExpression: false, m_renderReportItem.ReportItemDef.Width, new ReportSize(m_renderReportItem.ReportItemDef.Width));
					}
					else if (ChartDef.DynamicWidth != null)
					{
						m_dynamicWidth = new ReportSizeProperty(ChartDef.DynamicWidth);
					}
					else
					{
						m_dynamicWidth = new ReportSizeProperty(isExpression: false, m_reportItemDef.Width, new ReportSize(m_reportItemDef.Width));
					}
				}
				return m_dynamicWidth;
			}
		}

		public ChartTitle NoDataMessage
		{
			get
			{
				if (m_noDataMessage == null && !base.IsOldSnapshot && ChartDef.NoDataMessage != null)
				{
					m_noDataMessage = new ChartTitle(ChartDef.NoDataMessage, this);
				}
				return m_noDataMessage;
			}
		}

		public bool SpecialBorderHandling
		{
			get
			{
				ChartBorderSkin borderSkin = BorderSkin;
				if (borderSkin == null)
				{
					return false;
				}
				ReportEnumProperty<ChartBorderSkinType> borderSkinType = borderSkin.BorderSkinType;
				if (borderSkinType == null)
				{
					return false;
				}
				ChartBorderSkinType chartBorderSkinType = borderSkinType.IsExpression ? borderSkin.Instance.BorderSkinType : borderSkinType.Value;
				return chartBorderSkinType != ChartBorderSkinType.None;
			}
		}

		internal ChartInstanceInfo ChartInstanceInfo => (ChartInstanceInfo)RenderReportItem.InstanceInfo;

		internal Microsoft.ReportingServices.ReportProcessing.Chart RenderChartDef => (Microsoft.ReportingServices.ReportProcessing.Chart)RenderReportItem.ReportItemDef;

		internal Microsoft.ReportingServices.ReportRendering.Chart RenderChart
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (Microsoft.ReportingServices.ReportRendering.Chart)m_renderReportItem;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		internal Chart(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Chart reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Chart(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.Chart renderChart, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderChart, renderingContext)
		{
			m_snapshotDataRegionType = Type.Chart;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new ChartInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (m_categories != null)
			{
				m_categories.ResetContext();
			}
			if (m_series != null)
			{
				m_series.ResetContext();
			}
			if (m_chartAreas != null)
			{
				m_chartAreas.SetNewContext();
			}
			if (m_titles != null)
			{
				m_titles.SetNewContext();
			}
			if (m_customPaletteColors != null)
			{
				m_customPaletteColors.SetNewContext();
			}
			if (m_legends != null)
			{
				m_legends.SetNewContext();
			}
			if (m_borderSkin != null)
			{
				m_borderSkin.SetNewContext();
			}
			if (m_noDataMessage != null)
			{
				m_noDataMessage.SetNewContext();
			}
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (renderReportItem != null)
			{
				m_categories = null;
				m_series = null;
				m_memberCellDefinitionIndex = 0;
			}
			else
			{
				if (m_categories != null)
				{
					m_categories.ResetContext();
				}
				if (m_series != null)
				{
					m_series.ResetContext();
				}
			}
			m_chartAreas = null;
			m_titles = null;
			m_customPaletteColors = null;
			m_legends = null;
			m_borderSkin = null;
		}

		internal int GetCurrentMemberCellDefinitionIndex()
		{
			return m_memberCellDefinitionIndex;
		}

		internal int GetAndIncrementMemberCellDefinitionIndex()
		{
			return m_memberCellDefinitionIndex++;
		}

		internal void ResetMemberCellDefinitionIndex(int startIndex)
		{
			m_memberCellDefinitionIndex = startIndex;
		}

		internal ChartMember GetChartMember(ChartSeries chartSeries)
		{
			return GetChartMember(SeriesHierarchy.MemberCollection, GetSeriesIndex(chartSeries));
		}

		private int GetSeriesIndex(ChartSeries series)
		{
			ChartSeriesCollection seriesCollection = ChartData.SeriesCollection;
			for (int i = 0; i < seriesCollection.Count; i++)
			{
				if (seriesCollection[i] == series)
				{
					return i;
				}
			}
			return -1;
		}

		internal ChartMember GetChartMember(ChartMemberCollection chartMemberCollection, int memberCellIndex)
		{
			foreach (ChartMember item in chartMemberCollection)
			{
				if (item == null)
				{
					continue;
				}
				if (item.Children == null)
				{
					if (item.MemberCellIndex == memberCellIndex)
					{
						return item;
					}
					continue;
				}
				ChartMember chartMember = GetChartMember(item.Children, memberCellIndex);
				if (chartMember != null)
				{
					return chartMember;
				}
			}
			return null;
		}

		internal List<ChartDerivedSeries> GetChildrenDerivedSeries(string chartSeriesName)
		{
			ChartDerivedSeriesCollection derivedSeriesCollection = ChartData.DerivedSeriesCollection;
			if (derivedSeriesCollection == null)
			{
				return null;
			}
			List<ChartDerivedSeries> list = null;
			foreach (ChartDerivedSeries item in derivedSeriesCollection)
			{
				if (item != null && Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(item.SourceChartSeriesName, chartSeriesName, ignoreCase: false) == 0)
				{
					if (list == null)
					{
						list = new List<ChartDerivedSeries>();
					}
					list.Add(item);
				}
			}
			return list;
		}
	}
}
