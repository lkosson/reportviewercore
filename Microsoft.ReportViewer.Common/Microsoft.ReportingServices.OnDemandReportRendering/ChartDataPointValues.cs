using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataPointValues
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointValues m_chartDataPointValuesDef;

		private Chart m_chart;

		private ChartDataPointValuesInstance m_instance;

		private ReportVariantProperty m_x;

		private ReportVariantProperty m_y;

		private ReportVariantProperty m_size;

		private ReportVariantProperty m_high;

		private ReportVariantProperty m_low;

		private ReportVariantProperty m_start;

		private ReportVariantProperty m_end;

		private ReportVariantProperty m_mean;

		private ReportVariantProperty m_median;

		private ReportVariantProperty m_highlightX;

		private ReportVariantProperty m_highlightY;

		private ReportVariantProperty m_highlightSize;

		private ReportStringProperty m_formatX;

		private ReportStringProperty m_formatY;

		private ReportStringProperty m_formatSize;

		private ReportStringProperty m_currencyLanguageX;

		private ReportStringProperty m_currencyLanguageY;

		private ReportStringProperty m_currencyLanguageSize;

		private ChartDataPoint m_dataPoint;

		public ReportVariantProperty X
		{
			get
			{
				if (m_x == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("X");
						if (dataValue != null)
						{
							m_x = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.X != null)
					{
						m_x = new ReportVariantProperty(m_chartDataPointValuesDef.X);
					}
				}
				return m_x;
			}
		}

		public ReportVariantProperty Y
		{
			get
			{
				if (m_y == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("Y");
						if (dataValue != null)
						{
							m_y = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.Y != null)
					{
						m_y = new ReportVariantProperty(m_chartDataPointValuesDef.Y);
					}
				}
				return m_y;
			}
		}

		public ReportVariantProperty Size
		{
			get
			{
				if (m_size == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("Size");
						if (dataValue != null)
						{
							m_size = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.Size != null)
					{
						m_size = new ReportVariantProperty(m_chartDataPointValuesDef.Size);
					}
				}
				return m_size;
			}
		}

		public ReportVariantProperty High
		{
			get
			{
				if (m_high == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("High");
						if (dataValue != null)
						{
							m_high = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.High != null)
					{
						m_high = new ReportVariantProperty(m_chartDataPointValuesDef.High);
					}
				}
				return m_high;
			}
		}

		public ReportVariantProperty Low
		{
			get
			{
				if (m_low == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("Low");
						if (dataValue != null)
						{
							m_low = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.Low != null)
					{
						m_low = new ReportVariantProperty(m_chartDataPointValuesDef.Low);
					}
				}
				return m_low;
			}
		}

		public ReportVariantProperty Start
		{
			get
			{
				if (m_start == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("Open");
						if (dataValue != null)
						{
							m_start = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.Start != null)
					{
						m_start = new ReportVariantProperty(m_chartDataPointValuesDef.Start);
					}
				}
				return m_start;
			}
		}

		public ReportVariantProperty End
		{
			get
			{
				if (m_end == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						DataValue dataValue = GetDataValue("Close");
						if (dataValue != null)
						{
							m_end = dataValue.Value;
						}
					}
					else if (m_chartDataPointValuesDef.End != null)
					{
						m_end = new ReportVariantProperty(m_chartDataPointValuesDef.End);
					}
				}
				return m_end;
			}
		}

		public ReportVariantProperty Mean
		{
			get
			{
				if (m_mean == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.Mean != null)
				{
					m_mean = new ReportVariantProperty(m_chartDataPointValuesDef.Mean);
				}
				return m_mean;
			}
		}

		public ReportVariantProperty Median
		{
			get
			{
				if (m_median == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.Median != null)
				{
					m_median = new ReportVariantProperty(m_chartDataPointValuesDef.Median);
				}
				return m_median;
			}
		}

		public ReportVariantProperty HighlightX
		{
			get
			{
				if (m_highlightX == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.HighlightX != null)
				{
					m_highlightX = new ReportVariantProperty(m_chartDataPointValuesDef.HighlightX);
				}
				return m_highlightX;
			}
		}

		public ReportVariantProperty HighlightY
		{
			get
			{
				if (m_highlightY == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.HighlightY != null)
				{
					m_highlightY = new ReportVariantProperty(m_chartDataPointValuesDef.HighlightY);
				}
				return m_highlightY;
			}
		}

		public ReportVariantProperty HighlightSize
		{
			get
			{
				if (m_highlightSize == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.HighlightSize != null)
				{
					m_highlightSize = new ReportVariantProperty(m_chartDataPointValuesDef.HighlightSize);
				}
				return m_highlightSize;
			}
		}

		public ReportStringProperty FormatX
		{
			get
			{
				if (m_formatX == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.FormatX != null)
				{
					m_formatX = new ReportStringProperty(m_chartDataPointValuesDef.FormatX);
				}
				return m_formatX;
			}
		}

		public ReportStringProperty FormatY
		{
			get
			{
				if (m_formatY == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.FormatY != null)
				{
					m_formatY = new ReportStringProperty(m_chartDataPointValuesDef.FormatY);
				}
				return m_formatY;
			}
		}

		public ReportStringProperty FormatSize
		{
			get
			{
				if (m_formatSize == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.FormatSize != null)
				{
					m_formatSize = new ReportStringProperty(m_chartDataPointValuesDef.FormatSize);
				}
				return m_formatSize;
			}
		}

		public ReportStringProperty CurrencyLanguageX
		{
			get
			{
				if (m_currencyLanguageX == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.CurrencyLanguageX != null)
				{
					m_currencyLanguageX = new ReportStringProperty(m_chartDataPointValuesDef.CurrencyLanguageX);
				}
				return m_currencyLanguageX;
			}
		}

		public ReportStringProperty CurrencyLanguageY
		{
			get
			{
				if (m_currencyLanguageY == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.CurrencyLanguageY != null)
				{
					m_currencyLanguageY = new ReportStringProperty(m_chartDataPointValuesDef.CurrencyLanguageY);
				}
				return m_currencyLanguageY;
			}
		}

		public ReportStringProperty CurrencyLanguageSize
		{
			get
			{
				if (m_currencyLanguageSize == null && !m_chart.IsOldSnapshot && m_chartDataPointValuesDef.CurrencyLanguageSize != null)
				{
					m_currencyLanguageSize = new ReportStringProperty(m_chartDataPointValuesDef.CurrencyLanguageSize);
				}
				return m_currencyLanguageSize;
			}
		}

		internal Chart ChartDef => m_chart;

		internal ChartDataPoint ChartDataPoint => m_dataPoint;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointValues ChartDataPointValuesDef => m_chartDataPointValuesDef;

		public ChartDataPointValuesInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartDataPointValuesInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartDataPointValues(ChartDataPoint dataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointValues chartDataPointValuesDef, Chart chart)
		{
			m_dataPoint = dataPoint;
			m_chartDataPointValuesDef = chartDataPointValuesDef;
			m_chart = chart;
		}

		internal ChartDataPointValues(ChartDataPoint dataPoint, Chart chart)
		{
			m_dataPoint = dataPoint;
			m_chart = chart;
		}

		internal DataValue GetDataValue(string propertyName)
		{
			DataValueCollection dataValues = ((ShimChartDataPoint)m_dataPoint).DataValues;
			try
			{
				return dataValues[propertyName];
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				return null;
			}
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
