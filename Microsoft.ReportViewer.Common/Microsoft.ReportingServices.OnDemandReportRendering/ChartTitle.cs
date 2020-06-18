using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTitle : ChartObjectCollectionItem<ChartTitleInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle m_chartTitleDef;

		private Style m_style;

		private ReportStringProperty m_caption;

		private Microsoft.ReportingServices.ReportProcessing.ChartTitle m_renderChartTitleDef;

		private Microsoft.ReportingServices.ReportProcessing.ChartTitleInstance m_renderChartTitleInstance;

		private ReportEnumProperty<ChartTitlePositions> m_position;

		private ActionInfo m_actionInfo;

		private ReportBoolProperty m_hidden;

		private ReportIntProperty m_dockOffset;

		private ReportBoolProperty m_dockOutsideChartArea;

		private ReportStringProperty m_toolTip;

		private ReportEnumProperty<TextOrientations> m_textOrientation;

		private ChartElementPosition m_chartElementPosition;

		public string Name
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return "Default";
				}
				return m_chartTitleDef.TitleName;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (m_caption == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_renderChartTitleDef.Caption != null)
						{
							m_caption = new ReportStringProperty(m_renderChartTitleDef.Caption);
						}
					}
					else if (m_chartTitleDef.Caption != null)
					{
						m_caption = new ReportStringProperty(m_chartTitleDef.Caption);
					}
				}
				return m_caption;
			}
		}

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_style = new Style(m_renderChartTitleDef.StyleClass, m_renderChartTitleInstance.StyleAttributeValues, m_chart.RenderingContext);
					}
					else if (m_chartTitleDef != null)
					{
						m_style = new Style(m_chart, m_chart, m_chartTitleDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ReportEnumProperty<ChartTitlePositions> Position
		{
			get
			{
				if (m_position == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						ChartTitlePositions value = ChartTitlePositions.TopCenter;
						Microsoft.ReportingServices.ReportProcessing.ChartTitle.Positions position = m_renderChartTitleDef.Position;
						if ((uint)position <= 2u)
						{
							value = ChartTitlePositions.TopCenter;
						}
						m_position = new ReportEnumProperty<ChartTitlePositions>(value);
					}
					else if (m_chartTitleDef.Position != null)
					{
						m_position = new ReportEnumProperty<ChartTitlePositions>(m_chartTitleDef.Position.IsExpression, m_chartTitleDef.Position.OriginalText, EnumTranslator.TranslateChartTitlePosition(m_chartTitleDef.Position.StringValue, null));
					}
				}
				return m_position;
			}
		}

		public string UniqueName => m_chart.ChartDef.UniqueName + "xChartTitle_" + Name;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartTitleDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, m_chart, m_chartTitleDef.Action, m_chart.ChartDef, m_chart, ObjectType.Chart, m_chartTitleDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && !m_chart.IsOldSnapshot && m_chartTitleDef.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_chartTitleDef.Hidden);
				}
				return m_hidden;
			}
		}

		public string DockToChartArea
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return null;
				}
				return m_chartTitleDef.DockToChartArea;
			}
		}

		public ReportIntProperty DockOffset
		{
			get
			{
				if (m_dockOffset == null && !m_chart.IsOldSnapshot && m_chartTitleDef.DockOffset != null)
				{
					m_dockOffset = new ReportIntProperty(m_chartTitleDef.DockOffset.IsExpression, m_chartTitleDef.DockOffset.OriginalText, m_chartTitleDef.DockOffset.IntValue, 0);
				}
				return m_dockOffset;
			}
		}

		public ReportBoolProperty DockOutsideChartArea
		{
			get
			{
				if (m_dockOutsideChartArea == null && !m_chart.IsOldSnapshot && m_chartTitleDef.DockOutsideChartArea != null)
				{
					m_dockOutsideChartArea = new ReportBoolProperty(m_chartTitleDef.DockOutsideChartArea);
				}
				return m_dockOutsideChartArea;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartTitleDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartTitleDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ReportEnumProperty<TextOrientations> TextOrientation
		{
			get
			{
				if (m_textOrientation == null && !m_chart.IsOldSnapshot && m_chartTitleDef.TextOrientation != null)
				{
					m_textOrientation = new ReportEnumProperty<TextOrientations>(m_chartTitleDef.TextOrientation.IsExpression, m_chartTitleDef.TextOrientation.OriginalText, EnumTranslator.TranslateTextOrientations(m_chartTitleDef.TextOrientation.StringValue, null));
				}
				return m_textOrientation;
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				if (m_chartElementPosition == null && !m_chart.IsOldSnapshot && m_chartTitleDef.ChartElementPosition != null)
				{
					m_chartElementPosition = new ChartElementPosition(m_chartTitleDef.ChartElementPosition, m_chart);
				}
				return m_chartElementPosition;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle ChartTitleDef => m_chartTitleDef;

		internal Microsoft.ReportingServices.ReportProcessing.ChartTitleInstance RenderChartTitleInstance => m_renderChartTitleInstance;

		public ChartTitleInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartTitleInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartTitle(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle chartTitleDef, Chart chart)
		{
			m_chart = chart;
			m_chartTitleDef = chartTitleDef;
		}

		internal ChartTitle(Microsoft.ReportingServices.ReportProcessing.ChartTitle renderChartTitleDef, Microsoft.ReportingServices.ReportProcessing.ChartTitleInstance renderChartTitleInstance, Chart chart)
		{
			m_chart = chart;
			m_renderChartTitleDef = renderChartTitleDef;
			m_renderChartTitleInstance = renderChartTitleInstance;
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
			if (m_chartElementPosition != null)
			{
				m_chartElementPosition.SetNewContext();
			}
		}
	}
}
