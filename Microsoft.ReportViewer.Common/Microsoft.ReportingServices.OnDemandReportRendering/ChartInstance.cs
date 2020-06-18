using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartInstance : DynamicImageInstance, IDynamicImageInstance
	{
		private ReportSize m_dynamicHeight;

		private ReportSize m_dynamicWidth;

		private ChartPalette? m_palette;

		private PaletteHatchBehavior? m_paletteHatchBehavior;

		protected override int WidthInPixels => MappingHelper.ToIntPixels(DynamicWidth, m_dpiX);

		protected override int HeightInPixels => MappingHelper.ToIntPixels(DynamicHeight, m_dpiX);

		public ReportSize DynamicHeight
		{
			get
			{
				if (m_dynamicHeight == null)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_dynamicHeight = new ReportSize(m_reportElementDef.RenderReportItem.Height);
					}
					else
					{
						string text = ((Microsoft.ReportingServices.ReportIntermediateFormat.Chart)m_reportElementDef.ReportItemDef).EvaluateDynamicHeight(this, m_reportElementDef.RenderingContext.OdpContext);
						if (!string.IsNullOrEmpty(text))
						{
							m_dynamicHeight = new ReportSize(text);
						}
						else
						{
							m_dynamicHeight = ((ReportItem)m_reportElementDef).Height;
						}
					}
				}
				return m_dynamicHeight;
			}
		}

		public ReportSize DynamicWidth
		{
			get
			{
				if (m_dynamicWidth == null)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_dynamicWidth = new ReportSize(m_reportElementDef.RenderReportItem.Width);
					}
					else
					{
						string text = ((Microsoft.ReportingServices.ReportIntermediateFormat.Chart)m_reportElementDef.ReportItemDef).EvaluateDynamicWidth(this, m_reportElementDef.RenderingContext.OdpContext);
						if (!string.IsNullOrEmpty(text))
						{
							m_dynamicWidth = new ReportSize(text);
						}
						else
						{
							m_dynamicWidth = ((ReportItem)m_reportElementDef).Width;
						}
					}
				}
				return m_dynamicWidth;
			}
		}

		public ChartPalette Palette
		{
			get
			{
				if (!m_palette.HasValue)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_palette = ((Chart)m_reportElementDef).Palette.Value;
					}
					else
					{
						m_palette = ((Chart)m_reportElementDef).ChartDef.EvaluatePalette(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
					}
				}
				return m_palette.Value;
			}
		}

		public PaletteHatchBehavior PaletteHatchBehavior
		{
			get
			{
				if (!m_paletteHatchBehavior.HasValue)
				{
					m_paletteHatchBehavior = ((Chart)m_reportElementDef).ChartDef.EvaluatePaletteHatchBehavior(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
				}
				return m_paletteHatchBehavior.Value;
			}
		}

		internal ChartInstance(Chart reportItemDef)
			: base(reportItemDef)
		{
		}

		public override void SetDpi(int xDpi, int yDpi)
		{
			if (m_reportElementDef.IsOldSnapshot)
			{
				((Microsoft.ReportingServices.ReportRendering.Chart)m_reportElementDef.RenderReportItem).SetDpi(xDpi, yDpi);
			}
			else
			{
				base.SetDpi(xDpi, yDpi);
			}
		}

		protected override Stream GetImage(ImageType type, out bool hasImageMap)
		{
			if (m_reportElementDef.IsOldSnapshot)
			{
				return ((Microsoft.ReportingServices.ReportRendering.Chart)m_reportElementDef.RenderReportItem).GetImage((Microsoft.ReportingServices.ReportRendering.Chart.ImageType)type, out hasImageMap);
			}
			return base.GetImage(type, out hasImageMap);
		}

		public override Stream GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			actionImageMaps = null;
			Stream stream = null;
			bool hasImageMap = false;
			if (m_reportElementDef.IsOldSnapshot)
			{
				Microsoft.ReportingServices.ReportRendering.Chart chart = (Microsoft.ReportingServices.ReportRendering.Chart)m_reportElementDef.RenderReportItem;
				stream = chart.GetImage((Microsoft.ReportingServices.ReportRendering.Chart.ImageType)type, out hasImageMap);
				if (hasImageMap)
				{
					int dataPointSeriesCount = chart.DataPointSeriesCount;
					int dataPointCategoryCount = chart.DataPointCategoryCount;
					actionImageMaps = new ActionInfoWithDynamicImageMapCollection();
					for (int i = 0; i < dataPointSeriesCount; i++)
					{
						for (int j = 0; j < dataPointCategoryCount; j++)
						{
							Microsoft.ReportingServices.ReportRendering.ChartDataPoint chartDataPoint = chart.DataPointCollection[i, j];
							Microsoft.ReportingServices.ReportRendering.ActionInfo actionInfo = chartDataPoint.ActionInfo;
							if (actionInfo != null)
							{
								actionImageMaps.InternalList.Add(new ActionInfoWithDynamicImageMap(m_reportElementDef.RenderingContext, actionInfo, chartDataPoint.MapAreas));
							}
						}
					}
				}
			}
			else
			{
				stream = base.GetImage(type, out actionImageMaps);
			}
			return stream;
		}

		public Stream GetCoreXml()
		{
			if (m_reportElementDef.IsOldSnapshot)
			{
				return null;
			}
			using (IChartMapper chartMapper = ChartMapperFactory.CreateChartMapperInstance((Chart)m_reportElementDef, GetDefaultFontFamily()))
			{
				chartMapper.DpiX = m_dpiX;
				chartMapper.DpiY = m_dpiY;
				chartMapper.WidthOverride = m_widthOverride;
				chartMapper.HeightOverride = m_heightOverride;
				chartMapper.RenderChart();
				return chartMapper.GetCoreXml();
			}
		}

		protected override void GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image)
		{
			using (IChartMapper chartMapper = ChartMapperFactory.CreateChartMapperInstance((Chart)m_reportElementDef, GetDefaultFontFamily()))
			{
				chartMapper.DpiX = m_dpiX;
				chartMapper.DpiY = m_dpiY;
				chartMapper.WidthOverride = m_widthOverride;
				chartMapper.HeightOverride = m_heightOverride;
				chartMapper.RenderChart();
				image = chartMapper.GetImage(type);
				actionImageMaps = chartMapper.GetImageMaps();
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_dynamicHeight = null;
			m_dynamicWidth = null;
		}
	}
}
