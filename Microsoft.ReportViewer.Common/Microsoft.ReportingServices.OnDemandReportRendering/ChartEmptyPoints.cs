using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartEmptyPoints : IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private InternalChartSeries m_chartSeries;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints m_chartEmptyPointsDef;

		private ChartEmptyPointsInstance m_instance;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ChartMarker m_marker;

		private ChartDataLabel m_dataLabel;

		private ReportVariantProperty m_axisLabel;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ReportStringProperty m_toolTip;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartEmptyPointsDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chartSeries.ReportScope, m_chartEmptyPointsDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_chartSeries.ChartSeriesDef.UniqueName + "xEmptyPoint";

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartEmptyPointsDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, m_chartSeries.ReportScope, m_chartEmptyPointsDef.Action, m_chartSeries.ChartSeriesDef, m_chart, ObjectType.Chart, m_chartEmptyPointsDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ChartMarker Marker
		{
			get
			{
				if (m_marker == null && !m_chart.IsOldSnapshot && m_chartEmptyPointsDef.Marker != null)
				{
					m_marker = new ChartMarker(m_chartSeries, m_chartEmptyPointsDef.Marker, m_chart);
				}
				return m_marker;
			}
		}

		public ChartDataLabel DataLabel
		{
			get
			{
				if (m_dataLabel == null && !m_chart.IsOldSnapshot && m_chartEmptyPointsDef.DataLabel != null)
				{
					m_dataLabel = new ChartDataLabel(m_chartSeries, m_chartEmptyPointsDef.DataLabel, m_chart);
				}
				return m_dataLabel;
			}
		}

		public ReportVariantProperty AxisLabel
		{
			get
			{
				if (m_axisLabel == null && !m_chart.IsOldSnapshot && m_chartEmptyPointsDef.AxisLabel != null)
				{
					m_axisLabel = new ReportVariantProperty(m_chartEmptyPointsDef.AxisLabel);
				}
				return m_axisLabel;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					_ = m_chart.IsOldSnapshot;
					if (m_chartEmptyPointsDef.ToolTip != null)
					{
						m_toolTip = new ReportStringProperty(m_chartEmptyPointsDef.ToolTip);
					}
				}
				return m_toolTip;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return null;
				}
				if (m_customProperties == null)
				{
					m_customPropertiesReady = true;
					m_customProperties = new CustomPropertyCollection(m_chart.ReportScope.ReportScopeInstance, m_chart.RenderingContext, null, m_chartEmptyPointsDef, ObjectType.Chart, m_chart.Name);
				}
				else if (!m_customPropertiesReady)
				{
					m_customPropertiesReady = true;
					m_customProperties.UpdateCustomProperties(m_chartSeries.ReportScope.ReportScopeInstance, m_chartEmptyPointsDef, m_chart.RenderingContext.OdpContext, ObjectType.Chart, m_chart.Name);
				}
				return m_customProperties;
			}
		}

		internal IReportScope ReportScope => m_chartSeries.ReportScope;

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints ChartEmptyPointsDef => m_chartEmptyPointsDef;

		public ChartEmptyPointsInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartEmptyPointsInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartEmptyPoints(InternalChartSeries chartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPointsDef, Chart chart)
		{
			m_chartEmptyPointsDef = chartEmptyPointsDef;
			m_chart = chart;
			m_chartSeries = chartSeries;
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
			if (m_marker != null)
			{
				m_marker.SetNewContext();
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.SetNewContext();
			}
			m_customPropertiesReady = false;
		}
	}
}
