using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartMarker : IROMStyleDefinitionContainer
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker m_markerDef;

		private Chart m_chart;

		private ChartMarkerInstance m_instance;

		private Style m_style;

		private ReportSizeProperty m_size;

		private ReportEnumProperty<ChartMarkerTypes> m_type;

		private ChartDataPoint m_dataPoint;

		private InternalChartSeries m_chartSeries;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_style = new Style(m_dataPoint.RenderDataPointDef.MarkerStyleClass, m_dataPoint.RenderItem.InstanceInfo.MarkerStyleAttributeValues, m_chart.RenderingContext);
					}
					else if (m_markerDef.StyleClass != null)
					{
						m_style = new Style(m_chart, ReportScope, m_markerDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ReportEnumProperty<ChartMarkerTypes> Type
		{
			get
			{
				if (m_type == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						ChartMarkerTypes value = ChartMarkerTypes.None;
						switch (m_dataPoint.RenderDataPointDef.MarkerType)
						{
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Auto:
							value = ChartMarkerTypes.Auto;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Circle:
							value = ChartMarkerTypes.Circle;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Cross:
							value = ChartMarkerTypes.Cross;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Diamond:
							value = ChartMarkerTypes.Diamond;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.None:
							value = ChartMarkerTypes.None;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Square:
							value = ChartMarkerTypes.Square;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Triangle:
							value = ChartMarkerTypes.Triangle;
							break;
						}
						m_type = new ReportEnumProperty<ChartMarkerTypes>(value);
					}
					else if (m_markerDef.Type != null)
					{
						m_type = new ReportEnumProperty<ChartMarkerTypes>(m_markerDef.Type.IsExpression, m_markerDef.Type.OriginalText, EnumTranslator.TranslateChartMarkerType(m_markerDef.Type.StringValue, null));
					}
				}
				return m_type;
			}
		}

		public ReportSizeProperty Size
		{
			get
			{
				if (m_size == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						string text = (m_dataPoint.RenderDataPointDef.MarkerSize == null) ? "5pt" : m_dataPoint.RenderDataPointDef.MarkerSize;
						m_size = new ReportSizeProperty(isExpression: false, text, new ReportSize(text));
					}
					else if (m_markerDef.Size != null)
					{
						m_size = new ReportSizeProperty(m_markerDef.Size, new ReportSize("5pt"));
					}
				}
				return m_size;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker MarkerDef => m_markerDef;

		internal IReportScope ReportScope
		{
			get
			{
				if (m_dataPoint != null)
				{
					return m_dataPoint;
				}
				if (m_chartSeries != null)
				{
					return m_chartSeries.ReportScope;
				}
				return m_chart;
			}
		}

		public ChartMarkerInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartMarkerInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartMarker(ChartDataPoint dataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker markerDef, Chart chart)
			: this(markerDef, chart)
		{
			m_dataPoint = dataPoint;
		}

		internal ChartMarker(InternalChartSeries chartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker markerDef, Chart chart)
			: this(markerDef, chart)
		{
			m_chartSeries = chartSeries;
		}

		internal ChartMarker(Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker markerDef, Chart chart)
		{
			m_markerDef = markerDef;
			m_chart = chart;
		}

		internal ChartMarker(ChartDataPoint dataPoint, Chart chart)
		{
			m_dataPoint = dataPoint;
			m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
