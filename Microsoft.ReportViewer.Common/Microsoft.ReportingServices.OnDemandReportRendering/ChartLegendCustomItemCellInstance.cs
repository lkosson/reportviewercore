namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCellInstance : BaseInstance
	{
		private ChartLegendCustomItemCell m_chartLegendCustomItemCellDef;

		private StyleInstance m_style;

		private ChartCellType? m_cellType;

		private string m_text;

		private int? m_cellSpan;

		private string m_toolTip;

		private int? m_imageWidth;

		private int? m_imageHeight;

		private int? m_symbolHeight;

		private int? m_symbolWidth;

		private ChartCellAlignment? m_alignment;

		private int? m_topMargin;

		private int? m_bottomMargin;

		private int? m_leftMargin;

		private int? m_rightMargin;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartLegendCustomItemCellDef, m_chartLegendCustomItemCellDef.ChartDef, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartCellType CellType
		{
			get
			{
				if (!m_cellType.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_cellType = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateCellType(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_cellType.Value;
			}
		}

		public string Text
		{
			get
			{
				if (m_text == null && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_text = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateText(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_text;
			}
		}

		public int CellSpan
		{
			get
			{
				if (!m_cellSpan.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_cellSpan = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateCellSpan(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_cellSpan.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_toolTip = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateToolTip(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public int ImageWidth
		{
			get
			{
				if (!m_imageWidth.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_imageWidth = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateImageWidth(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_imageWidth.Value;
			}
		}

		public int ImageHeight
		{
			get
			{
				if (!m_imageHeight.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_imageHeight = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateImageHeight(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_imageHeight.Value;
			}
		}

		public int SymbolHeight
		{
			get
			{
				if (!m_symbolHeight.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_symbolHeight = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateSymbolHeight(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_symbolHeight.Value;
			}
		}

		public int SymbolWidth
		{
			get
			{
				if (!m_symbolWidth.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_symbolWidth = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateSymbolWidth(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_symbolWidth.Value;
			}
		}

		public ChartCellAlignment Alignment
		{
			get
			{
				if (!m_alignment.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_alignment = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateAlignment(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_alignment.Value;
			}
		}

		public int TopMargin
		{
			get
			{
				if (!m_topMargin.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_topMargin = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateTopMargin(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_topMargin.Value;
			}
		}

		public int BottomMargin
		{
			get
			{
				if (!m_bottomMargin.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_bottomMargin = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateBottomMargin(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_bottomMargin.Value;
			}
		}

		public int LeftMargin
		{
			get
			{
				if (!m_leftMargin.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_leftMargin = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateLeftMargin(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_leftMargin.Value;
			}
		}

		public int RightMargin
		{
			get
			{
				if (!m_rightMargin.HasValue && !m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					m_rightMargin = m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateRightMargin(ReportScopeInstance, m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_rightMargin.Value;
			}
		}

		internal ChartLegendCustomItemCellInstance(ChartLegendCustomItemCell chartLegendCustomItemCellDef)
			: base(chartLegendCustomItemCellDef.ChartDef)
		{
			m_chartLegendCustomItemCellDef = chartLegendCustomItemCellDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_cellType = null;
			m_text = null;
			m_cellSpan = null;
			m_toolTip = null;
			m_imageWidth = null;
			m_imageHeight = null;
			m_symbolHeight = null;
			m_symbolWidth = null;
			m_alignment = null;
			m_topMargin = null;
			m_bottomMargin = null;
			m_leftMargin = null;
			m_rightMargin = null;
		}
	}
}
