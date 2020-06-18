using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataPointValuesInstance : BaseInstance
	{
		private ChartDataPointValues m_chartDataPointValuesDef;

		private object m_x;

		private object m_y;

		private object m_size;

		private object m_high;

		private object m_low;

		private object m_start;

		private object m_end;

		private object m_mean;

		private object m_median;

		private object m_highlightX;

		private object m_highlightY;

		private object m_highlightSize;

		private string m_formatX;

		private string m_formatY;

		private string m_formatSize;

		private string m_currencyLanguageX;

		private string m_currencyLanguageY;

		private string m_currencyLanguageSize;

		private List<string> m_fieldsUsedInValues;

		private bool m_fieldsUsedInValuesEvaluated;

		public object X
		{
			get
			{
				if (m_x == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("X");
						if (dataValue != null)
						{
							m_x = dataValue.Instance.Value;
						}
					}
					else
					{
						m_x = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateX(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_x;
			}
		}

		public object Y
		{
			get
			{
				if (m_y == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("Y");
						if (dataValue != null)
						{
							m_y = dataValue.Instance.Value;
						}
					}
					else
					{
						m_y = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateY(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_y;
			}
		}

		public object Size
		{
			get
			{
				if (m_size == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("Size");
						if (dataValue != null)
						{
							m_size = dataValue.Instance.Value;
						}
					}
					else
					{
						m_size = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateSize(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_size;
			}
		}

		public object High
		{
			get
			{
				if (m_high == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("High");
						if (dataValue != null)
						{
							m_high = dataValue.Instance.Value;
						}
					}
					else
					{
						m_high = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHigh(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_high;
			}
		}

		public object Low
		{
			get
			{
				if (m_low == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("Low");
						if (dataValue != null)
						{
							m_low = dataValue.Instance.Value;
						}
					}
					else
					{
						m_low = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateLow(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_low;
			}
		}

		public object Start
		{
			get
			{
				if (m_start == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("Open");
						if (dataValue != null)
						{
							m_start = dataValue.Instance.Value;
						}
					}
					else
					{
						m_start = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateStart(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_start;
			}
		}

		public object End
		{
			get
			{
				if (m_end == null)
				{
					if (m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = m_chartDataPointValuesDef.GetDataValue("Close");
						if (dataValue != null)
						{
							m_end = dataValue.Instance.Value;
						}
					}
					else
					{
						m_end = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateEnd(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return m_end;
			}
		}

		public object Mean
		{
			get
			{
				if (m_mean == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_mean = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateMean(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_mean;
			}
		}

		public object Median
		{
			get
			{
				if (m_median == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_median = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateMedian(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_median;
			}
		}

		public object HighlightX
		{
			get
			{
				if (m_highlightX == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_highlightX = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHighlightX(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_highlightX;
			}
		}

		public object HighlightY
		{
			get
			{
				if (m_highlightY == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_highlightY = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHighlightY(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_highlightY;
			}
		}

		public object HighlightSize
		{
			get
			{
				if (m_highlightSize == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_highlightSize = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHighlightSize(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_highlightSize;
			}
		}

		public string FormatX
		{
			get
			{
				if (m_formatX == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_formatX = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateFormatX(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_formatX;
			}
		}

		public string FormatY
		{
			get
			{
				if (m_formatY == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_formatY = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateFormatY(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_formatY;
			}
		}

		public string FormatSize
		{
			get
			{
				if (m_formatSize == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_formatSize = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateFormatSize(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_formatSize;
			}
		}

		public string CurrencyLanguageX
		{
			get
			{
				if (m_currencyLanguageX == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_currencyLanguageX = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateCurrencyLanguageX(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_currencyLanguageX;
			}
		}

		public string CurrencyLanguageY
		{
			get
			{
				if (m_currencyLanguageY == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_currencyLanguageY = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateCurrencyLanguageY(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_currencyLanguageY;
			}
		}

		public string CurrencyLanguageSize
		{
			get
			{
				if (m_currencyLanguageSize == null && !m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					m_currencyLanguageSize = m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateCurrencyLanguageSize(ReportScopeInstance, m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_currencyLanguageSize;
			}
		}

		internal ChartDataPointValuesInstance(ChartDataPointValues chartDataPointValuesDef)
			: base(chartDataPointValuesDef.ChartDataPoint)
		{
			m_chartDataPointValuesDef = chartDataPointValuesDef;
		}

		protected override void ResetInstanceCache()
		{
			m_x = null;
			m_y = null;
			m_size = null;
			m_high = null;
			m_low = null;
			m_start = null;
			m_end = null;
			m_mean = null;
			m_median = null;
			m_highlightX = null;
			m_highlightY = null;
			m_highlightSize = null;
			m_formatX = null;
			m_formatY = null;
			m_formatSize = null;
			m_currencyLanguageX = null;
			m_currencyLanguageY = null;
			m_currencyLanguageSize = null;
			m_fieldsUsedInValuesEvaluated = false;
			m_fieldsUsedInValues = null;
		}

		internal List<string> GetFieldsUsedInValues()
		{
			if (!m_fieldsUsedInValuesEvaluated)
			{
				m_fieldsUsedInValuesEvaluated = true;
				Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPointDef = m_chartDataPointValuesDef.ChartDataPoint.DataPointDef;
				if (dataPointDef.Action != null && dataPointDef.Action.TrackFieldsUsedInValueExpression)
				{
					m_fieldsUsedInValues = new List<string>();
					ObjectModelImpl reportObjectModel = m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext.ReportObjectModel;
					reportObjectModel.ResetFieldsUsedInExpression();
					ReportVariantProperty x = m_chartDataPointValuesDef.X;
					if (x != null && x.IsExpression)
					{
						_ = X;
					}
					x = m_chartDataPointValuesDef.Y;
					if (x != null && x.IsExpression)
					{
						_ = Y;
					}
					x = m_chartDataPointValuesDef.Size;
					if (x != null && x.IsExpression)
					{
						_ = Size;
					}
					x = m_chartDataPointValuesDef.High;
					if (x != null && x.IsExpression)
					{
						_ = High;
					}
					x = m_chartDataPointValuesDef.Low;
					if (x != null && x.IsExpression)
					{
						_ = Low;
					}
					x = m_chartDataPointValuesDef.Start;
					if (x != null && x.IsExpression)
					{
						_ = Start;
					}
					x = m_chartDataPointValuesDef.End;
					if (x != null && x.IsExpression)
					{
						_ = End;
					}
					x = m_chartDataPointValuesDef.Mean;
					if (x != null && x.IsExpression)
					{
						_ = Mean;
					}
					x = m_chartDataPointValuesDef.Median;
					if (x != null && x.IsExpression)
					{
						_ = Median;
					}
					reportObjectModel.AddFieldsUsedInExpression(m_fieldsUsedInValues);
				}
			}
			return m_fieldsUsedInValues;
		}
	}
}
