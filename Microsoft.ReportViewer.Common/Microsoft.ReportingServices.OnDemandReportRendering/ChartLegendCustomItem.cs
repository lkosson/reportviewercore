using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItem : ChartObjectCollectionItem<ChartLegendCustomItemInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem m_chartLegendCustomItemDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ChartMarker m_marker;

		private ReportEnumProperty<ChartSeparators> m_separator;

		private ReportColorProperty m_separatorColor;

		private ReportStringProperty m_toolTip;

		private ChartLegendCustomItemCellCollection m_chartLegendCustomItemCells;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartLegendCustomItemDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_chart.ChartDef.UniqueName + "x" + m_chartLegendCustomItemDef.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, m_chart, m_chartLegendCustomItemDef.Action, m_chart.ChartDef, m_chart, ObjectType.Chart, m_chartLegendCustomItemDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ChartMarker Marker
		{
			get
			{
				if (m_marker == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemDef.Marker != null)
				{
					m_marker = new ChartMarker(m_chartLegendCustomItemDef.Marker, m_chart);
				}
				return m_marker;
			}
		}

		public ReportEnumProperty<ChartSeparators> Separator
		{
			get
			{
				if (m_separator == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemDef.Separator != null)
				{
					m_separator = new ReportEnumProperty<ChartSeparators>(m_chartLegendCustomItemDef.Separator.IsExpression, m_chartLegendCustomItemDef.Separator.OriginalText, EnumTranslator.TranslateChartSeparator(m_chartLegendCustomItemDef.Separator.StringValue, null));
				}
				return m_separator;
			}
		}

		public ReportColorProperty SeparatorColor
		{
			get
			{
				if (m_separatorColor == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemDef.SeparatorColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo separatorColor = m_chartLegendCustomItemDef.SeparatorColor;
					m_separatorColor = new ReportColorProperty(separatorColor.IsExpression, separatorColor.OriginalText, separatorColor.IsExpression ? null : new ReportColor(separatorColor.StringValue.Trim(), allowTransparency: true), separatorColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
				}
				return m_separatorColor;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartLegendCustomItemDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ChartLegendCustomItemCellCollection LegendCustomItemCells
		{
			get
			{
				if (m_chartLegendCustomItemCells == null && !m_chart.IsOldSnapshot && ChartLegendCustomItemDef.LegendCustomItemCells != null)
				{
					m_chartLegendCustomItemCells = new ChartLegendCustomItemCellCollection(this, m_chart);
				}
				return m_chartLegendCustomItemCells;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem ChartLegendCustomItemDef => m_chartLegendCustomItemDef;

		public ChartLegendCustomItemInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartLegendCustomItemInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartLegendCustomItem(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItemDef, Chart chart)
		{
			m_chartLegendCustomItemDef = chartLegendCustomItemDef;
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
			if (m_marker != null)
			{
				m_marker.SetNewContext();
			}
			if (m_chartLegendCustomItemCells != null)
			{
				m_chartLegendCustomItemCells.SetNewContext();
			}
		}
	}
}
