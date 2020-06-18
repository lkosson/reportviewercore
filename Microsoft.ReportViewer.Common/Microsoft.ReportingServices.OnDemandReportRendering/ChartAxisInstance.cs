namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisInstance : BaseInstance
	{
		private ChartAxis m_axisDef;

		private StyleInstance m_style;

		private object m_min;

		private object m_max;

		private object m_crossAt;

		private ChartAutoBool? m_visible;

		private ChartAutoBool? m_margin;

		private double? m_interval;

		private ChartIntervalType? m_intervalType;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalOffsetType;

		private double? m_labelInterval;

		private ChartIntervalType? m_labelIntervalType;

		private double? m_labelIntervalOffset;

		private ChartIntervalType? m_labelIntervalOffsetType;

		private bool? m_variableAutoInterval;

		private bool? m_marksAlwaysAtPlotEdge;

		private bool? m_reverse;

		private ChartAxisLocation? m_location;

		private bool? m_interlaced;

		private ReportColor m_interlacedColor;

		private bool? m_logScale;

		private double? m_logBase;

		private bool? m_hideLabels;

		private double? m_angle;

		private bool? m_preventFontShrink;

		private bool? m_preventFontGrow;

		private bool? m_preventLabelOffset;

		private bool? m_preventWordWrap;

		private ChartAxisLabelRotation? m_allowLabelRotation;

		private bool? m_includeZero;

		private bool? m_labelsAutoFitDisabled;

		private ReportSize m_minFontSize;

		private ReportSize m_maxFontSize;

		private bool? m_offsetLabels;

		private bool? m_hideEndLabels;

		private ChartAxisArrow? m_arrows;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_axisDef, m_axisDef.ChartDef, m_axisDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public object CrossAt
		{
			get
			{
				if (m_crossAt == null)
				{
					if (m_axisDef.ChartDef.IsOldSnapshot)
					{
						m_crossAt = m_axisDef.RenderAxisInstance.CrossAtValue;
					}
					else
					{
						m_crossAt = m_axisDef.AxisDef.EvaluateCrossAt(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_crossAt;
			}
		}

		public object Minimum
		{
			get
			{
				if (m_min == null)
				{
					if (m_axisDef.ChartDef.IsOldSnapshot)
					{
						m_min = m_axisDef.RenderAxisInstance.MinValue;
					}
					else
					{
						m_min = m_axisDef.AxisDef.EvaluateMin(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_min;
			}
		}

		public object Maximum
		{
			get
			{
				if (m_max == null)
				{
					if (m_axisDef.ChartDef.IsOldSnapshot)
					{
						m_max = m_axisDef.RenderAxisInstance.MaxValue;
					}
					else
					{
						m_max = m_axisDef.AxisDef.EvaluateMax(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_max;
			}
		}

		public ChartAutoBool Visible
		{
			get
			{
				if (!m_visible.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					string val = m_axisDef.AxisDef.EvaluateVisible(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
					m_visible = EnumTranslator.TranslateChartAutoBool(val, m_axisDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return m_visible.Value;
			}
		}

		public ChartAutoBool Margin
		{
			get
			{
				if (!m_margin.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					string val = m_axisDef.AxisDef.EvaluateMargin(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
					m_margin = EnumTranslator.TranslateChartAutoBool(val, m_axisDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return m_margin.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_interval = m_axisDef.AxisDef.EvaluateInterval(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!m_intervalType.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_intervalType = m_axisDef.AxisDef.EvaluateIntervalType(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalType.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffset = m_axisDef.AxisDef.EvaluateIntervalOffset(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!m_intervalOffsetType.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffsetType = m_axisDef.AxisDef.EvaluateIntervalOffsetType(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffsetType.Value;
			}
		}

		public double LabelInterval
		{
			get
			{
				if (!m_labelInterval.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_labelInterval = m_axisDef.AxisDef.EvaluateLabelInterval(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_labelInterval.Value;
			}
		}

		public ChartIntervalType LabelIntervalType
		{
			get
			{
				if (!m_labelIntervalType.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_labelIntervalType = m_axisDef.AxisDef.EvaluateLabelIntervalType(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_labelIntervalType.Value;
			}
		}

		public double LabelIntervalOffset
		{
			get
			{
				if (!m_labelIntervalOffset.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_labelIntervalOffset = m_axisDef.AxisDef.EvaluateLabelIntervalOffset(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_labelIntervalOffset.Value;
			}
		}

		public ChartIntervalType LabelIntervalOffsetType
		{
			get
			{
				if (!m_labelIntervalOffsetType.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_labelIntervalOffsetType = m_axisDef.AxisDef.EvaluateLabelIntervalOffsetType(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_labelIntervalOffsetType.Value;
			}
		}

		public bool VariableAutoInterval
		{
			get
			{
				if (!m_variableAutoInterval.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_variableAutoInterval = m_axisDef.AxisDef.EvaluateVariableAutoInterval(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_variableAutoInterval.Value;
			}
		}

		public bool MarksAlwaysAtPlotEdge
		{
			get
			{
				if (!m_marksAlwaysAtPlotEdge.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_marksAlwaysAtPlotEdge = m_axisDef.AxisDef.EvaluateMarksAlwaysAtPlotEdge(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_marksAlwaysAtPlotEdge.Value;
			}
		}

		public bool Reverse
		{
			get
			{
				if (!m_reverse.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_reverse = m_axisDef.AxisDef.EvaluateReverse(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_reverse.Value;
			}
		}

		public ChartAxisLocation Location
		{
			get
			{
				if (!m_location.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_location = m_axisDef.AxisDef.EvaluateLocation(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_location.Value;
			}
		}

		public bool Interlaced
		{
			get
			{
				if (!m_interlaced.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_interlaced = m_axisDef.AxisDef.EvaluateInterlaced(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_interlaced.Value;
			}
		}

		public ReportColor InterlacedColor
		{
			get
			{
				if (m_interlacedColor == null && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_interlacedColor = new ReportColor(m_axisDef.AxisDef.EvaluateInterlacedColor(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_interlacedColor;
			}
		}

		public bool LogScale
		{
			get
			{
				if (!m_logScale.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_logScale = m_axisDef.AxisDef.EvaluateLogScale(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_logScale.Value;
			}
		}

		public double LogBase
		{
			get
			{
				if (!m_logBase.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_logBase = m_axisDef.AxisDef.EvaluateLogBase(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_logBase.Value;
			}
		}

		public bool HideLabels
		{
			get
			{
				if (!m_hideLabels.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_hideLabels = m_axisDef.AxisDef.EvaluateHideLabels(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hideLabels.Value;
			}
		}

		public double Angle
		{
			get
			{
				if (!m_angle.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_angle = m_axisDef.AxisDef.EvaluateAngle(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_angle.Value;
			}
		}

		public ChartAxisArrow Arrows
		{
			get
			{
				if (!m_arrows.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_arrows = m_axisDef.AxisDef.EvaluateArrows(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_arrows.Value;
			}
		}

		public bool PreventFontShrink
		{
			get
			{
				if (!m_preventFontShrink.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_preventFontShrink = m_axisDef.AxisDef.EvaluatePreventFontShrink(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_preventFontShrink.Value;
			}
		}

		public bool PreventFontGrow
		{
			get
			{
				if (!m_preventFontGrow.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_preventFontGrow = m_axisDef.AxisDef.EvaluatePreventFontGrow(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_preventFontGrow.Value;
			}
		}

		public bool PreventLabelOffset
		{
			get
			{
				if (!m_preventLabelOffset.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_preventLabelOffset = m_axisDef.AxisDef.EvaluatePreventLabelOffset(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_preventLabelOffset.Value;
			}
		}

		public bool PreventWordWrap
		{
			get
			{
				if (!m_preventWordWrap.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_preventWordWrap = m_axisDef.AxisDef.EvaluatePreventWordWrap(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_preventWordWrap.Value;
			}
		}

		public ChartAxisLabelRotation AllowLabelRotation
		{
			get
			{
				if (!m_allowLabelRotation.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_allowLabelRotation = m_axisDef.AxisDef.EvaluateAllowLabelRotation(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_allowLabelRotation.Value;
			}
		}

		public bool IncludeZero
		{
			get
			{
				if (!m_includeZero.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_includeZero = m_axisDef.AxisDef.EvaluateIncludeZero(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_includeZero.Value;
			}
		}

		public bool LabelsAutoFitDisabled
		{
			get
			{
				if (!m_labelsAutoFitDisabled.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_labelsAutoFitDisabled = m_axisDef.AxisDef.EvaluateLabelsAutoFitDisabled(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_labelsAutoFitDisabled.Value;
			}
		}

		public ReportSize MinFontSize
		{
			get
			{
				if (m_minFontSize == null && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_minFontSize = new ReportSize(m_axisDef.AxisDef.EvaluateMinFontSize(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_minFontSize;
			}
		}

		public ReportSize MaxFontSize
		{
			get
			{
				if (m_maxFontSize == null && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_maxFontSize = new ReportSize(m_axisDef.AxisDef.EvaluateMaxFontSize(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_maxFontSize;
			}
		}

		public bool OffsetLabels
		{
			get
			{
				if (!m_offsetLabels.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_offsetLabels = m_axisDef.AxisDef.EvaluateOffsetLabels(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_offsetLabels.Value;
			}
		}

		public bool HideEndLabels
		{
			get
			{
				if (!m_hideEndLabels.HasValue && !m_axisDef.ChartDef.IsOldSnapshot)
				{
					m_hideEndLabels = m_axisDef.AxisDef.EvaluateHideEndLabels(ReportScopeInstance, m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hideEndLabels.Value;
			}
		}

		internal ChartAxisInstance(ChartAxis axisDef)
			: base(axisDef.ChartDef)
		{
			m_axisDef = axisDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_min = null;
			m_max = null;
			m_crossAt = null;
			m_visible = null;
			m_margin = null;
			m_interval = null;
			m_intervalType = null;
			m_intervalOffset = null;
			m_intervalOffsetType = null;
			m_labelInterval = null;
			m_labelIntervalType = null;
			m_labelIntervalOffset = null;
			m_labelIntervalOffsetType = null;
			m_variableAutoInterval = null;
			m_marksAlwaysAtPlotEdge = null;
			m_reverse = null;
			m_location = null;
			m_interlaced = null;
			m_interlacedColor = null;
			m_logScale = null;
			m_logBase = null;
			m_hideLabels = null;
			m_angle = null;
			m_arrows = null;
			m_preventFontShrink = null;
			m_preventFontGrow = null;
			m_preventLabelOffset = null;
			m_preventWordWrap = null;
			m_allowLabelRotation = null;
			m_includeZero = null;
			m_labelsAutoFitDisabled = null;
			m_minFontSize = null;
			m_maxFontSize = null;
			m_offsetLabels = null;
			m_hideEndLabels = null;
		}
	}
}
