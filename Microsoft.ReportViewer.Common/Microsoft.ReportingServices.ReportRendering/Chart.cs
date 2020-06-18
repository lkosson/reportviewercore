using Microsoft.ReportingServices.ReportProcessing;
using System.IO;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Chart : DataRegion
	{
		public enum ImageType
		{
			PNG,
			EMF
		}

		private ChartDataPointCollection m_datapoints;

		private ChartMemberCollection m_categories;

		private ChartMemberCollection m_series;

		private int m_categoryGroupingLevels;

		private ImageMapAreasCollection[] m_imageMapAreaCollection;

		private float m_scaleX = -1f;

		private float m_scaleY = -1f;

		public ChartDataPointCollection DataPointCollection
		{
			get
			{
				ChartDataPointCollection chartDataPointCollection = m_datapoints;
				if (m_datapoints == null)
				{
					int num = 0;
					int num2 = 0;
					if (base.ReportItemInstance != null && 0 < ((ChartInstance)base.ReportItemInstance).DataPoints.Count)
					{
						num = DataPointSeriesCount;
						num2 = DataPointCategoryCount;
					}
					else
					{
						num = ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticSeriesCount;
						num2 = ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticCategoryCount;
					}
					chartDataPointCollection = new ChartDataPointCollection(this, num, num2);
					if (base.RenderingContext.CacheState)
					{
						m_datapoints = chartDataPointCollection;
					}
				}
				return chartDataPointCollection;
			}
		}

		public ChartMemberCollection CategoryMemberCollection
		{
			get
			{
				ChartMemberCollection chartMemberCollection = m_categories;
				if (m_categories == null)
				{
					chartMemberCollection = new ChartMemberCollection(this, null, ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).Columns, (base.ReportItemInstance == null) ? null : ((ChartInstance)base.ReportItemInstance).ColumnInstances);
					if (base.RenderingContext.CacheState)
					{
						m_categories = chartMemberCollection;
					}
				}
				return chartMemberCollection;
			}
		}

		public ChartMemberCollection SeriesMemberCollection
		{
			get
			{
				ChartMemberCollection chartMemberCollection = m_series;
				if (m_series == null)
				{
					chartMemberCollection = new ChartMemberCollection(this, null, ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).Rows, (base.ReportItemInstance == null) ? null : ((ChartInstance)base.ReportItemInstance).RowInstances);
					if (base.RenderingContext.CacheState)
					{
						m_series = chartMemberCollection;
					}
				}
				return chartMemberCollection;
			}
		}

		public int DataPointCategoryCount
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((ChartInstance)base.ReportItemInstance).DataPointCategoryCount;
			}
		}

		public int DataPointSeriesCount
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((ChartInstance)base.ReportItemInstance).DataPointSeriesCount;
			}
		}

		public int CategoriesCount => ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticCategoryCount;

		public int SeriesCount => ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticSeriesCount;

		public override bool NoRows
		{
			get
			{
				if (base.ReportItemInstance == null || ((ChartInstance)base.ReportItemInstance).DataPointSeriesCount == 0 || ((ChartInstance)base.ReportItemInstance).DataPointCategoryCount == 0)
				{
					return true;
				}
				return false;
			}
		}

		internal ChartDataPointInstancesList DataPoints => ((ChartInstance)base.ReportItemInstance).DataPoints;

		internal int CategoryGroupingLevels
		{
			get
			{
				if (m_categoryGroupingLevels == 0)
				{
					m_categoryGroupingLevels = 1;
					ChartHeading chartHeading = ((Microsoft.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).Columns;
					Global.Tracer.Assert(chartHeading != null);
					while (chartHeading.SubHeading != null)
					{
						chartHeading = chartHeading.SubHeading;
						m_categoryGroupingLevels++;
					}
				}
				return m_categoryGroupingLevels;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((ChartInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		internal ImageMapAreasCollection[] DataPointMapAreas
		{
			get
			{
				if (m_imageMapAreaCollection == null)
				{
					RenderChartImageMap();
				}
				return m_imageMapAreaCollection;
			}
		}

		internal float ScaleX
		{
			get
			{
				return m_scaleX;
			}
			set
			{
				m_scaleX = value;
			}
		}

		internal float ScaleY
		{
			get
			{
				return m_scaleY;
			}
			set
			{
				m_scaleY = value;
			}
		}

		internal Chart(int intUniqueName, Microsoft.ReportingServices.ReportProcessing.Chart reportItemDef, ChartInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public void SetDpi(int xDpi, int yDpi)
		{
			if (xDpi > 0 && xDpi != 96)
			{
				m_scaleX = (float)xDpi / 96f;
			}
			if (yDpi > 0 && yDpi != 96)
			{
				m_scaleY = (float)yDpi / 96f;
			}
		}

		public MemoryStream GetImage(out bool hasImageMap)
		{
			return GetImage(ImageType.PNG, out hasImageMap);
		}

		public MemoryStream GetImage()
		{
			bool hasImageMap;
			return GetImage(ImageType.PNG, out hasImageMap);
		}

		public MemoryStream GetImage(ImageType type)
		{
			bool hasImageMap;
			return GetImage(type, out hasImageMap);
		}

		public MemoryStream GetImage(ImageType type, out bool hasImageMap)
		{
			hasImageMap = false;
			return null;
		}

		private bool RenderChartImageMap()
		{
			return m_imageMapAreaCollection != null;
		}
	}
}
