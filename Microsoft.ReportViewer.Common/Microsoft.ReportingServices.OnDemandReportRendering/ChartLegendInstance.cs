namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendInstance : BaseInstance
	{
		private ChartLegend m_legendDef;

		private StyleInstance m_style;

		private bool? m_hidden;

		private ChartLegendPositions? m_position;

		private ChartLegendLayouts? m_layout;

		private bool? m_dockOutsideChartArea;

		private bool? m_autoFitTextDisabled;

		private ReportSize m_minFontSize;

		private ChartSeparators? m_headerSeparator;

		private ReportColor m_headerSeparatorColor;

		private ChartSeparators? m_columnSeparator;

		private ReportColor m_columnSeparatorColor;

		private int? m_columnSpacing;

		private bool? m_interlacedRows;

		private ReportColor m_interlacedRowsColor;

		private bool? m_equallySpacedItems;

		private ChartAutoBool? m_reversed;

		private int? m_maxAutoSize;

		private int? m_textWrapThreshold;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_legendDef, m_legendDef.ChartDef, m_legendDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_hidden = m_legendDef.ChartLegendDef.EvaluateHidden(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public ChartLegendPositions Position
		{
			get
			{
				if (!m_position.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_position = m_legendDef.ChartLegendDef.EvaluatePosition(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		public ChartLegendLayouts Layout
		{
			get
			{
				if (!m_layout.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_layout = m_legendDef.ChartLegendDef.EvaluateLayout(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_layout.Value;
			}
		}

		public bool DockOutsideChartArea
		{
			get
			{
				if (!m_dockOutsideChartArea.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_dockOutsideChartArea = m_legendDef.ChartLegendDef.EvaluateDockOutsideChartArea(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_dockOutsideChartArea.Value;
			}
		}

		public bool AutoFitTextDisabled
		{
			get
			{
				if (!m_autoFitTextDisabled.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_autoFitTextDisabled = m_legendDef.ChartLegendDef.EvaluateAutoFitTextDisabled(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_autoFitTextDisabled.Value;
			}
		}

		public ReportSize MinFontSize
		{
			get
			{
				if (m_minFontSize == null && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_minFontSize = new ReportSize(m_legendDef.ChartLegendDef.EvaluateMinFontSize(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_minFontSize;
			}
		}

		public ChartSeparators HeaderSeparator
		{
			get
			{
				if (!m_headerSeparator.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_headerSeparator = m_legendDef.ChartLegendDef.EvaluateHeaderSeparator(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_headerSeparator.Value;
			}
		}

		public ReportColor HeaderSeparatorColor
		{
			get
			{
				if (m_headerSeparatorColor == null && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_headerSeparatorColor = new ReportColor(m_legendDef.ChartLegendDef.EvaluateHeaderSeparatorColor(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_headerSeparatorColor;
			}
		}

		public ChartSeparators ColumnSeparator
		{
			get
			{
				if (!m_columnSeparator.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_columnSeparator = m_legendDef.ChartLegendDef.EvaluateColumnSeparator(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_columnSeparator.Value;
			}
		}

		public ReportColor ColumnSeparatorColor
		{
			get
			{
				if (m_columnSeparatorColor == null && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_columnSeparatorColor = new ReportColor(m_legendDef.ChartLegendDef.EvaluateColumnSeparatorColor(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_columnSeparatorColor;
			}
		}

		public int ColumnSpacing
		{
			get
			{
				if (!m_columnSpacing.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_columnSpacing = m_legendDef.ChartLegendDef.EvaluateColumnSpacing(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_columnSpacing.Value;
			}
		}

		public bool InterlacedRows
		{
			get
			{
				if (!m_interlacedRows.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_interlacedRows = m_legendDef.ChartLegendDef.EvaluateInterlacedRows(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_interlacedRows.Value;
			}
		}

		public ReportColor InterlacedRowsColor
		{
			get
			{
				if (m_interlacedRowsColor == null && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_interlacedRowsColor = new ReportColor(m_legendDef.ChartLegendDef.EvaluateInterlacedRowsColor(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_interlacedRowsColor;
			}
		}

		public bool EquallySpacedItems
		{
			get
			{
				if (!m_equallySpacedItems.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_equallySpacedItems = m_legendDef.ChartLegendDef.EvaluateEquallySpacedItems(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_equallySpacedItems.Value;
			}
		}

		public ChartAutoBool Reversed
		{
			get
			{
				if (!m_reversed.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_reversed = m_legendDef.ChartLegendDef.EvaluateReversed(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_reversed.Value;
			}
		}

		public int MaxAutoSize
		{
			get
			{
				if (!m_maxAutoSize.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_maxAutoSize = m_legendDef.ChartLegendDef.EvaluateMaxAutoSize(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_maxAutoSize.Value;
			}
		}

		public int TextWrapThreshold
		{
			get
			{
				if (!m_textWrapThreshold.HasValue && !m_legendDef.ChartDef.IsOldSnapshot)
				{
					m_textWrapThreshold = m_legendDef.ChartLegendDef.EvaluateTextWrapThreshold(ReportScopeInstance, m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_textWrapThreshold.Value;
			}
		}

		internal ChartLegendInstance(ChartLegend legendDef)
			: base(legendDef.ChartDef)
		{
			m_legendDef = legendDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_hidden = null;
			m_position = null;
			m_layout = null;
			m_dockOutsideChartArea = null;
			m_autoFitTextDisabled = null;
			m_minFontSize = null;
			m_headerSeparator = null;
			m_headerSeparatorColor = null;
			m_columnSeparator = null;
			m_columnSeparatorColor = null;
			m_columnSpacing = null;
			m_interlacedRows = null;
			m_interlacedRowsColor = null;
			m_equallySpacedItems = null;
			m_reversed = null;
			m_maxAutoSize = null;
			m_textWrapThreshold = null;
		}
	}
}
