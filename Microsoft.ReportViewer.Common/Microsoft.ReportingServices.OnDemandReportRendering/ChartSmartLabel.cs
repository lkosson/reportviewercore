using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartSmartLabel
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel m_chartSmartLabelDef;

		private ChartSmartLabelInstance m_instance;

		private ReportEnumProperty<ChartAllowOutsideChartArea> m_allowOutSidePlotArea;

		private ReportColorProperty m_calloutBackColor;

		private ReportEnumProperty<ChartCalloutLineAnchor> m_calloutLineAnchor;

		private ReportColorProperty m_calloutLineColor;

		private ReportEnumProperty<ChartCalloutLineStyle> m_calloutLineStyle;

		private ReportSizeProperty m_calloutLineWidth;

		private ReportEnumProperty<ChartCalloutStyle> m_calloutStyle;

		private ReportBoolProperty m_showOverlapped;

		private ReportBoolProperty m_markerOverlapping;

		private ReportSizeProperty m_maxMovingDistance;

		private ReportSizeProperty m_minMovingDistance;

		private ChartNoMoveDirections m_noMoveDirections;

		private ReportBoolProperty m_disabled;

		private InternalChartSeries m_chartSeries;

		public ReportEnumProperty<ChartAllowOutsideChartArea> AllowOutSidePlotArea
		{
			get
			{
				if (m_allowOutSidePlotArea == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_allowOutSidePlotArea = new ReportEnumProperty<ChartAllowOutsideChartArea>(ChartAllowOutsideChartArea.False);
					}
					else if (m_chartSmartLabelDef.AllowOutSidePlotArea != null)
					{
						m_allowOutSidePlotArea = new ReportEnumProperty<ChartAllowOutsideChartArea>(m_chartSmartLabelDef.AllowOutSidePlotArea.IsExpression, m_chartSmartLabelDef.AllowOutSidePlotArea.OriginalText, EnumTranslator.TranslateChartAllowOutsideChartArea(m_chartSmartLabelDef.AllowOutSidePlotArea.StringValue, null));
					}
				}
				return m_allowOutSidePlotArea;
			}
		}

		public ReportColorProperty CalloutBackColor
		{
			get
			{
				if (m_calloutBackColor == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.CalloutBackColor != null)
				{
					ExpressionInfo calloutBackColor = m_chartSmartLabelDef.CalloutBackColor;
					m_calloutBackColor = new ReportColorProperty(calloutBackColor.IsExpression, calloutBackColor.OriginalText, calloutBackColor.IsExpression ? null : new ReportColor(calloutBackColor.StringValue.Trim(), allowTransparency: true), calloutBackColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
				}
				return m_calloutBackColor;
			}
		}

		public ReportEnumProperty<ChartCalloutLineAnchor> CalloutLineAnchor
		{
			get
			{
				if (m_calloutLineAnchor == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.CalloutLineAnchor != null)
				{
					m_calloutLineAnchor = new ReportEnumProperty<ChartCalloutLineAnchor>(m_chartSmartLabelDef.CalloutLineAnchor.IsExpression, m_chartSmartLabelDef.CalloutLineAnchor.OriginalText, EnumTranslator.TranslateChartCalloutLineAnchor(m_chartSmartLabelDef.CalloutLineAnchor.StringValue, null));
				}
				return m_calloutLineAnchor;
			}
		}

		public ReportColorProperty CalloutLineColor
		{
			get
			{
				if (m_calloutLineColor == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.CalloutLineColor != null)
				{
					ExpressionInfo calloutLineColor = m_chartSmartLabelDef.CalloutLineColor;
					m_calloutLineColor = new ReportColorProperty(calloutLineColor.IsExpression, calloutLineColor.OriginalText, calloutLineColor.IsExpression ? null : new ReportColor(calloutLineColor.StringValue.Trim(), allowTransparency: true), calloutLineColor.IsExpression ? new ReportColor("", Color.Black, parsed: true) : null);
				}
				return m_calloutLineColor;
			}
		}

		public ReportEnumProperty<ChartCalloutLineStyle> CalloutLineStyle
		{
			get
			{
				if (m_calloutLineStyle == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.CalloutLineStyle != null)
				{
					m_calloutLineStyle = new ReportEnumProperty<ChartCalloutLineStyle>(m_chartSmartLabelDef.CalloutLineStyle.IsExpression, m_chartSmartLabelDef.CalloutLineStyle.OriginalText, EnumTranslator.TranslateChartCalloutLineStyle(m_chartSmartLabelDef.CalloutLineStyle.StringValue, null));
				}
				return m_calloutLineStyle;
			}
		}

		public ReportSizeProperty CalloutLineWidth
		{
			get
			{
				if (m_calloutLineWidth == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.CalloutLineWidth != null)
				{
					m_calloutLineWidth = new ReportSizeProperty(m_chartSmartLabelDef.CalloutLineWidth);
				}
				return m_calloutLineWidth;
			}
		}

		public ReportEnumProperty<ChartCalloutStyle> CalloutStyle
		{
			get
			{
				if (m_calloutStyle == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.CalloutStyle != null)
				{
					m_calloutStyle = new ReportEnumProperty<ChartCalloutStyle>(m_chartSmartLabelDef.CalloutStyle.IsExpression, m_chartSmartLabelDef.CalloutStyle.OriginalText, EnumTranslator.TranslateChartCalloutStyle(m_chartSmartLabelDef.CalloutStyle.StringValue, null));
				}
				return m_calloutStyle;
			}
		}

		public ReportBoolProperty ShowOverlapped
		{
			get
			{
				if (m_showOverlapped == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.ShowOverlapped != null)
				{
					m_showOverlapped = new ReportBoolProperty(m_chartSmartLabelDef.ShowOverlapped);
				}
				return m_showOverlapped;
			}
		}

		public ReportBoolProperty MarkerOverlapping
		{
			get
			{
				if (m_markerOverlapping == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.MarkerOverlapping != null)
				{
					m_markerOverlapping = new ReportBoolProperty(m_chartSmartLabelDef.MarkerOverlapping);
				}
				return m_markerOverlapping;
			}
		}

		public ReportSizeProperty MaxMovingDistance
		{
			get
			{
				if (m_maxMovingDistance == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.MaxMovingDistance != null)
				{
					m_maxMovingDistance = new ReportSizeProperty(m_chartSmartLabelDef.MaxMovingDistance);
				}
				return m_maxMovingDistance;
			}
		}

		public ReportSizeProperty MinMovingDistance
		{
			get
			{
				if (m_minMovingDistance == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.MinMovingDistance != null)
				{
					m_minMovingDistance = new ReportSizeProperty(m_chartSmartLabelDef.MinMovingDistance);
				}
				return m_minMovingDistance;
			}
		}

		public ChartNoMoveDirections NoMoveDirections
		{
			get
			{
				if (m_noMoveDirections == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.NoMoveDirections != null)
				{
					m_noMoveDirections = new ChartNoMoveDirections(m_chartSeries, m_chartSmartLabelDef.NoMoveDirections, m_chart);
				}
				return m_noMoveDirections;
			}
		}

		public ReportBoolProperty Disabled
		{
			get
			{
				if (m_disabled == null && !m_chart.IsOldSnapshot && m_chartSmartLabelDef.Disabled != null)
				{
					m_disabled = new ReportBoolProperty(m_chartSmartLabelDef.Disabled);
				}
				return m_disabled;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (m_chartSeries != null)
				{
					return m_chartSeries.ReportScope;
				}
				return m_chart;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel ChartSmartLabelDef => m_chartSmartLabelDef;

		public ChartSmartLabelInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartSmartLabelInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartSmartLabel(InternalChartSeries chartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabelDef, Chart chart)
		{
			m_chartSeries = chartSeries;
			m_chartSmartLabelDef = chartSmartLabelDef;
			m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_noMoveDirections != null)
			{
				m_noMoveDirections.SetNewContext();
			}
		}
	}
}
