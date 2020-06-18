using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumn : ChartObjectCollectionItem<ChartLegendColumnInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn m_chartLegendColumnDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportEnumProperty<ChartColumnType> m_columnType;

		private ReportStringProperty m_value;

		private ReportStringProperty m_toolTip;

		private ReportSizeProperty m_minimumWidth;

		private ReportSizeProperty m_maximumWidth;

		private ReportIntProperty m_seriesSymbolWidth;

		private ReportIntProperty m_seriesSymbolHeight;

		private ChartLegendColumnHeader m_header;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartLegendColumnDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_chart.ChartDef.UniqueName + "x" + m_chartLegendColumnDef.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, m_chart, m_chartLegendColumnDef.Action, m_chart.ChartDef, m_chart, ObjectType.Chart, m_chartLegendColumnDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportEnumProperty<ChartColumnType> ColumnType
		{
			get
			{
				if (m_columnType == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.ColumnType != null)
				{
					m_columnType = new ReportEnumProperty<ChartColumnType>(m_chartLegendColumnDef.ColumnType.IsExpression, m_chartLegendColumnDef.ColumnType.OriginalText, EnumTranslator.TranslateChartColumnType(m_chartLegendColumnDef.ColumnType.StringValue, null));
				}
				return m_columnType;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (m_value == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.Value != null)
				{
					m_value = new ReportStringProperty(m_chartLegendColumnDef.Value);
				}
				return m_value;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartLegendColumnDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ReportSizeProperty MinimumWidth
		{
			get
			{
				if (m_minimumWidth == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.MinimumWidth != null)
				{
					m_minimumWidth = new ReportSizeProperty(m_chartLegendColumnDef.MinimumWidth);
				}
				return m_minimumWidth;
			}
		}

		public ReportSizeProperty MaximumWidth
		{
			get
			{
				if (m_maximumWidth == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.MaximumWidth != null)
				{
					m_maximumWidth = new ReportSizeProperty(m_chartLegendColumnDef.MaximumWidth);
				}
				return m_maximumWidth;
			}
		}

		public ReportIntProperty SeriesSymbolWidth
		{
			get
			{
				if (m_seriesSymbolWidth == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.SeriesSymbolWidth != null)
				{
					m_seriesSymbolWidth = new ReportIntProperty(m_chartLegendColumnDef.SeriesSymbolWidth.IsExpression, m_chartLegendColumnDef.SeriesSymbolWidth.OriginalText, m_chartLegendColumnDef.SeriesSymbolWidth.IntValue, 200);
				}
				return m_seriesSymbolWidth;
			}
		}

		public ReportIntProperty SeriesSymbolHeight
		{
			get
			{
				if (m_seriesSymbolHeight == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.SeriesSymbolHeight != null)
				{
					m_seriesSymbolHeight = new ReportIntProperty(m_chartLegendColumnDef.SeriesSymbolHeight.IsExpression, m_chartLegendColumnDef.SeriesSymbolHeight.OriginalText, m_chartLegendColumnDef.SeriesSymbolHeight.IntValue, 70);
				}
				return m_seriesSymbolHeight;
			}
		}

		public ChartLegendColumnHeader Header
		{
			get
			{
				if (m_header == null && !m_chart.IsOldSnapshot && m_chartLegendColumnDef.Header != null)
				{
					m_header = new ChartLegendColumnHeader(m_chartLegendColumnDef.Header, m_chart);
				}
				return m_header;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn ChartLegendColumnDef => m_chartLegendColumnDef;

		public ChartLegendColumnInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartLegendColumnInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartLegendColumn(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumnDef, Chart chart)
		{
			m_chartLegendColumnDef = chartLegendColumnDef;
			m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_header != null)
			{
				m_header.SetNewContext();
			}
		}
	}
}
