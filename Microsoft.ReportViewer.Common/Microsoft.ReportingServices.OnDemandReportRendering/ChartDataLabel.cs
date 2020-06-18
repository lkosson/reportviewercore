using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataLabel : IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel m_chartDataLabelDef;

		private Chart m_chart;

		private ChartDataLabelInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_value;

		private ChartDataPoint m_dataPoint;

		private InternalChartSeries m_chartSeries;

		private ReportBoolProperty m_visible;

		private ReportEnumProperty<ChartDataLabelPositions> m_position;

		private ReportIntProperty m_rotation;

		private ActionInfo m_actionInfo;

		private ReportBoolProperty m_useValueAsLabel;

		private ReportStringProperty m_toolTip;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_style = new Style(m_dataPoint.RenderDataPointDef.DataLabel.StyleClass, m_dataPoint.RenderItem.InstanceInfo.DataLabelStyleAttributeValues, m_chart.RenderingContext);
					}
					else if (m_chartDataLabelDef.StyleClass != null)
					{
						m_style = new Style(m_chart, ReportScope, m_chartDataLabelDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public string UniqueName => InstancePath.UniqueName + "xDataLabel";

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartDataLabelDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, ReportScope, m_chartDataLabelDef.Action, InstancePath, m_chart, ObjectType.Chart, m_chartDataLabelDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportStringProperty Label
		{
			get
			{
				if (m_value == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_value = new ReportStringProperty(m_dataPoint.RenderDataPointDef.DataLabel.Value);
					}
					else if (m_chartDataLabelDef.Label != null)
					{
						m_value = new ReportStringProperty(m_chartDataLabelDef.Label);
					}
				}
				return m_value;
			}
		}

		public ReportBoolProperty UseValueAsLabel
		{
			get
			{
				if (m_useValueAsLabel == null && !m_chart.IsOldSnapshot && m_chartDataLabelDef.UseValueAsLabel != null)
				{
					m_useValueAsLabel = new ReportBoolProperty(m_chartDataLabelDef.UseValueAsLabel);
				}
				return m_useValueAsLabel;
			}
		}

		public ReportBoolProperty Visible
		{
			get
			{
				if (m_visible == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_visible = new ReportBoolProperty(m_dataPoint.RenderDataPointDef.DataLabel.Visible);
					}
					else if (m_chartDataLabelDef.Visible != null)
					{
						m_visible = new ReportBoolProperty(m_chartDataLabelDef.Visible);
					}
				}
				return m_visible;
			}
		}

		public ReportIntProperty Rotation
		{
			get
			{
				if (m_rotation == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_rotation = new ReportIntProperty(m_dataPoint.RenderDataPointDef.DataLabel.Rotation);
					}
					else if (m_chartDataLabelDef.Rotation != null)
					{
						m_rotation = new ReportIntProperty(m_chartDataLabelDef.Rotation);
					}
				}
				return m_rotation;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartDataLabelDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartDataLabelDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ReportEnumProperty<ChartDataLabelPositions> Position
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					ChartDataLabelPositions value = ChartDataLabelPositions.Auto;
					switch (m_dataPoint.RenderDataPointDef.DataLabel.Position)
					{
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Auto:
						value = ChartDataLabelPositions.Auto;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Bottom:
						value = ChartDataLabelPositions.Bottom;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.BottomLeft:
						value = ChartDataLabelPositions.BottomLeft;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.BottomRight:
						value = ChartDataLabelPositions.BottomRight;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Center:
						value = ChartDataLabelPositions.Center;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Left:
						value = ChartDataLabelPositions.Left;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Right:
						value = ChartDataLabelPositions.Right;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Top:
						value = ChartDataLabelPositions.Top;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.TopLeft:
						value = ChartDataLabelPositions.TopLeft;
						break;
					case Microsoft.ReportingServices.ReportProcessing.ChartDataLabel.Positions.TopRight:
						value = ChartDataLabelPositions.TopRight;
						break;
					}
					m_position = new ReportEnumProperty<ChartDataLabelPositions>(value);
				}
				else if (m_chartDataLabelDef.Position != null)
				{
					m_position = new ReportEnumProperty<ChartDataLabelPositions>(m_chartDataLabelDef.Position.IsExpression, m_chartDataLabelDef.Position.OriginalText, EnumTranslator.TranslateChartDataLabelPosition(m_chartDataLabelDef.Position.StringValue, null));
				}
				return m_position;
			}
		}

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

		private IInstancePath InstancePath
		{
			get
			{
				if (m_dataPoint != null)
				{
					return m_dataPoint.DataPointDef;
				}
				if (m_chartSeries != null)
				{
					return m_chartSeries.ChartSeriesDef;
				}
				return m_chart.ChartDef;
			}
		}

		internal Chart ChartDef => m_chart;

		internal ChartDataPoint ChartDataPoint => m_dataPoint;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel ChartDataLabelDef => m_chartDataLabelDef;

		public ChartDataLabelInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartDataLabelInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartDataLabel(ChartDataPoint dataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabelDef, Chart chart)
		{
			m_dataPoint = dataPoint;
			m_chartDataLabelDef = chartDataLabelDef;
			m_chart = chart;
		}

		internal ChartDataLabel(InternalChartSeries series, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabelDef, Chart chart)
		{
			m_chartSeries = series;
			m_chartDataLabelDef = chartDataLabelDef;
			m_chart = chart;
		}

		internal ChartDataLabel(ChartDataPoint dataPoint, Chart chart)
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
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
		}
	}
}
