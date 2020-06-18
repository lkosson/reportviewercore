using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCell : ChartObjectCollectionItem<ChartLegendCustomItemCellInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell m_chartLegendCustomItemCellDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportEnumProperty<ChartCellType> m_cellType;

		private ReportStringProperty m_text;

		private ReportIntProperty m_cellSpan;

		private ReportStringProperty m_toolTip;

		private ReportIntProperty m_imageWidth;

		private ReportIntProperty m_imageHeight;

		private ReportIntProperty m_symbolHeight;

		private ReportIntProperty m_symbolWidth;

		private ReportEnumProperty<ChartCellAlignment> m_alignment;

		private ReportIntProperty m_topMargin;

		private ReportIntProperty m_bottomMargin;

		private ReportIntProperty m_leftMargin;

		private ReportIntProperty m_rightMargin;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartLegendCustomItemCellDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_chart.ChartDef.UniqueName + "x" + m_chartLegendCustomItemCellDef.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, m_chart, m_chartLegendCustomItemCellDef.Action, m_chart.ChartDef, m_chart, ObjectType.Chart, m_chartLegendCustomItemCellDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportEnumProperty<ChartCellType> CellType
		{
			get
			{
				if (m_cellType == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.CellType != null)
				{
					m_cellType = new ReportEnumProperty<ChartCellType>(m_chartLegendCustomItemCellDef.CellType.IsExpression, m_chartLegendCustomItemCellDef.CellType.OriginalText, EnumTranslator.TranslateChartCellType(m_chartLegendCustomItemCellDef.CellType.StringValue, null));
				}
				return m_cellType;
			}
		}

		public ReportStringProperty Text
		{
			get
			{
				if (m_text == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.Text != null)
				{
					m_text = new ReportStringProperty(m_chartLegendCustomItemCellDef.Text);
				}
				return m_text;
			}
		}

		public ReportIntProperty CellSpan
		{
			get
			{
				if (m_cellSpan == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.CellSpan != null)
				{
					m_cellSpan = new ReportIntProperty(m_chartLegendCustomItemCellDef.CellSpan.IsExpression, m_chartLegendCustomItemCellDef.CellSpan.OriginalText, m_chartLegendCustomItemCellDef.CellSpan.IntValue, 1);
				}
				return m_cellSpan;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartLegendCustomItemCellDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ReportIntProperty ImageWidth
		{
			get
			{
				if (m_imageWidth == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.ImageWidth != null)
				{
					m_imageWidth = new ReportIntProperty(m_chartLegendCustomItemCellDef.ImageWidth.IsExpression, m_chartLegendCustomItemCellDef.ImageWidth.OriginalText, m_chartLegendCustomItemCellDef.ImageWidth.IntValue, 0);
				}
				return m_imageWidth;
			}
		}

		public ReportIntProperty ImageHeight
		{
			get
			{
				if (m_imageHeight == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.ImageHeight != null)
				{
					m_imageHeight = new ReportIntProperty(m_chartLegendCustomItemCellDef.ImageHeight.IsExpression, m_chartLegendCustomItemCellDef.ImageHeight.OriginalText, m_chartLegendCustomItemCellDef.ImageHeight.IntValue, 0);
				}
				return m_imageHeight;
			}
		}

		public ReportIntProperty SymbolHeight
		{
			get
			{
				if (m_symbolHeight == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.SymbolHeight != null)
				{
					m_symbolHeight = new ReportIntProperty(m_chartLegendCustomItemCellDef.SymbolHeight.IsExpression, m_chartLegendCustomItemCellDef.SymbolHeight.OriginalText, m_chartLegendCustomItemCellDef.SymbolHeight.IntValue, 0);
				}
				return m_symbolHeight;
			}
		}

		public ReportIntProperty SymbolWidth
		{
			get
			{
				if (m_symbolWidth == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.SymbolWidth != null)
				{
					m_symbolWidth = new ReportIntProperty(m_chartLegendCustomItemCellDef.SymbolWidth.IsExpression, m_chartLegendCustomItemCellDef.SymbolWidth.OriginalText, m_chartLegendCustomItemCellDef.SymbolWidth.IntValue, 0);
				}
				return m_symbolWidth;
			}
		}

		public ReportEnumProperty<ChartCellAlignment> Alignment
		{
			get
			{
				if (m_alignment == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.Alignment != null)
				{
					m_alignment = new ReportEnumProperty<ChartCellAlignment>(m_chartLegendCustomItemCellDef.Alignment.IsExpression, m_chartLegendCustomItemCellDef.Alignment.OriginalText, EnumTranslator.TranslateChartCellAlignment(m_chartLegendCustomItemCellDef.Alignment.StringValue, null));
				}
				return m_alignment;
			}
		}

		public ReportIntProperty TopMargin
		{
			get
			{
				if (m_topMargin == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.TopMargin != null)
				{
					m_topMargin = new ReportIntProperty(m_chartLegendCustomItemCellDef.TopMargin.IsExpression, m_chartLegendCustomItemCellDef.TopMargin.OriginalText, m_chartLegendCustomItemCellDef.TopMargin.IntValue, 0);
				}
				return m_topMargin;
			}
		}

		public ReportIntProperty BottomMargin
		{
			get
			{
				if (m_bottomMargin == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.BottomMargin != null)
				{
					m_bottomMargin = new ReportIntProperty(m_chartLegendCustomItemCellDef.BottomMargin.IsExpression, m_chartLegendCustomItemCellDef.BottomMargin.OriginalText, m_chartLegendCustomItemCellDef.BottomMargin.IntValue, 0);
				}
				return m_bottomMargin;
			}
		}

		public ReportIntProperty LeftMargin
		{
			get
			{
				if (m_leftMargin == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.LeftMargin != null)
				{
					m_leftMargin = new ReportIntProperty(m_chartLegendCustomItemCellDef.LeftMargin.IsExpression, m_chartLegendCustomItemCellDef.LeftMargin.OriginalText, m_chartLegendCustomItemCellDef.LeftMargin.IntValue, 0);
				}
				return m_leftMargin;
			}
		}

		public ReportIntProperty RightMargin
		{
			get
			{
				if (m_rightMargin == null && !m_chart.IsOldSnapshot && m_chartLegendCustomItemCellDef.RightMargin != null)
				{
					m_rightMargin = new ReportIntProperty(m_chartLegendCustomItemCellDef.RightMargin.IsExpression, m_chartLegendCustomItemCellDef.RightMargin.OriginalText, m_chartLegendCustomItemCellDef.RightMargin.IntValue, 0);
				}
				return m_rightMargin;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell ChartLegendCustomItemCellDef => m_chartLegendCustomItemCellDef;

		public ChartLegendCustomItemCellInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartLegendCustomItemCellInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartLegendCustomItemCell(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCellDef, Chart chart)
		{
			m_chartLegendCustomItemCellDef = chartLegendCustomItemCellDef;
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
		}
	}
}
