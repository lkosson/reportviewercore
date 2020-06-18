namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartNoMoveDirectionsInstance : BaseInstance
	{
		private ChartNoMoveDirections m_chartNoMoveDirectionsDef;

		private bool? m_up;

		private bool? m_down;

		private bool? m_left;

		private bool? m_right;

		private bool? m_upLeft;

		private bool? m_upRight;

		private bool? m_downLeft;

		private bool? m_downRight;

		public bool Up
		{
			get
			{
				if (!m_up.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_up = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateUp(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_up.Value;
			}
		}

		public bool Down
		{
			get
			{
				if (!m_down.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_down = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateDown(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_down.Value;
			}
		}

		public bool Left
		{
			get
			{
				if (!m_left.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_left = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateLeft(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_left.Value;
			}
		}

		public bool Right
		{
			get
			{
				if (!m_right.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_right = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateRight(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_right.Value;
			}
		}

		public bool UpLeft
		{
			get
			{
				if (!m_upLeft.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_upLeft = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateUpLeft(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_upLeft.Value;
			}
		}

		public bool UpRight
		{
			get
			{
				if (!m_upRight.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_upRight = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateUpRight(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_upRight.Value;
			}
		}

		public bool DownLeft
		{
			get
			{
				if (!m_downLeft.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_downLeft = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateDownLeft(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_downLeft.Value;
			}
		}

		public bool DownRight
		{
			get
			{
				if (!m_downRight.HasValue && !m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					m_downRight = m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateDownRight(ReportScopeInstance, m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_downRight.Value;
			}
		}

		internal ChartNoMoveDirectionsInstance(ChartNoMoveDirections chartNoMoveDirectionsDef)
			: base(chartNoMoveDirectionsDef.ReportScope)
		{
			m_chartNoMoveDirectionsDef = chartNoMoveDirectionsDef;
		}

		protected override void ResetInstanceCache()
		{
			m_up = null;
			m_down = null;
			m_left = null;
			m_right = null;
			m_upLeft = null;
			m_upRight = null;
			m_downLeft = null;
			m_downRight = null;
		}
	}
}
