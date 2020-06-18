using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartItemInLegend : IROMActionOwner
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend m_chartItemInLegendDef;

		private ChartItemInLegendInstance m_instance;

		private ActionInfo m_actionInfo;

		private ReportStringProperty m_legendText;

		private ChartDataPoint m_dataPoint;

		private InternalChartSeries m_chartSeries;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		public string UniqueName => InstancePath.UniqueName + "xInLegend";

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartItemInLegendDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, ReportScope, m_chartItemInLegendDef.Action, InstancePath, m_chart, ObjectType.Chart, m_chart.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportStringProperty LegendText
		{
			get
			{
				if (m_legendText == null && !m_chart.IsOldSnapshot && m_chartItemInLegendDef.LegendText != null)
				{
					m_legendText = new ReportStringProperty(m_chartItemInLegendDef.LegendText);
				}
				return m_legendText;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartItemInLegendDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartItemInLegendDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && m_chartItemInLegendDef.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_chartItemInLegendDef.Hidden);
				}
				return m_hidden;
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

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend ChartItemInLegendDef => m_chartItemInLegendDef;

		public ChartItemInLegendInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartItemInLegendInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartItemInLegend(InternalChartSeries chartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegendDef, Chart chart)
		{
			m_chartSeries = chartSeries;
			m_chartItemInLegendDef = chartItemInLegendDef;
			m_chart = chart;
		}

		internal ChartItemInLegend(InternalChartDataPoint chartDataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegendDef, Chart chart)
		{
			m_dataPoint = chartDataPoint;
			m_chartItemInLegendDef = chartItemInLegendDef;
			m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
		}
	}
}
