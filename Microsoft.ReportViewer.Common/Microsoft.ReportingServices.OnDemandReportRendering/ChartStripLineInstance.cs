using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartStripLineInstance : BaseInstance
	{
		private ChartStripLine m_chartStripLineDef;

		private StyleInstance m_style;

		private string m_title;

		private int? m_titleAngle;

		private TextOrientations? m_textOrientation;

		private string m_toolTip;

		private double? m_interval;

		private ChartIntervalType? m_intervalType;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalOffsetType;

		private double? m_stripWidth;

		private ChartIntervalType? m_stripWidthType;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartStripLineDef, m_chartStripLineDef.ChartDef, m_chartStripLineDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public string Title
		{
			get
			{
				if (m_title == null && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_title = m_chartStripLineDef.ChartStripLineDef.EvaluateTitle(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_title;
			}
		}

		[Obsolete("Use TextOrientation instead.")]
		public int TitleAngle
		{
			get
			{
				if (!m_titleAngle.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_titleAngle = m_chartStripLineDef.ChartStripLineDef.EvaluateTitleAngle(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_titleAngle.Value;
			}
		}

		public TextOrientations TextOrientation
		{
			get
			{
				if (!m_textOrientation.HasValue)
				{
					m_textOrientation = m_chartStripLineDef.ChartStripLineDef.EvaluateTextOrientation(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_textOrientation.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_toolTip = m_chartStripLineDef.ChartStripLineDef.EvaluateToolTip(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_interval = m_chartStripLineDef.ChartStripLineDef.EvaluateInterval(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!m_intervalType.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_intervalType = m_chartStripLineDef.ChartStripLineDef.EvaluateIntervalType(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalType.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffset = m_chartStripLineDef.ChartStripLineDef.EvaluateIntervalOffset(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!m_intervalOffsetType.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffsetType = m_chartStripLineDef.ChartStripLineDef.EvaluateIntervalOffsetType(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffsetType.Value;
			}
		}

		public double StripWidth
		{
			get
			{
				if (!m_stripWidth.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_stripWidth = m_chartStripLineDef.ChartStripLineDef.EvaluateStripWidth(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_stripWidth.Value;
			}
		}

		public ChartIntervalType StripWidthType
		{
			get
			{
				if (!m_stripWidthType.HasValue && !m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					m_stripWidthType = m_chartStripLineDef.ChartStripLineDef.EvaluateStripWidthType(ReportScopeInstance, m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_stripWidthType.Value;
			}
		}

		internal ChartStripLineInstance(ChartStripLine chartStripLineDef)
			: base(chartStripLineDef.ChartDef)
		{
			m_chartStripLineDef = chartStripLineDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_title = null;
			m_textOrientation = null;
			m_titleAngle = null;
			m_toolTip = null;
			m_interval = null;
			m_intervalType = null;
			m_intervalOffset = null;
			m_intervalOffsetType = null;
			m_stripWidth = null;
			m_stripWidthType = null;
		}
	}
}
