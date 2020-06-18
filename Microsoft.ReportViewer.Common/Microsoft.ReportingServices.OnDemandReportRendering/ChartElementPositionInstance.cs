namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartElementPositionInstance : BaseInstance
	{
		private ChartElementPosition m_defObject;

		private double? m_top;

		private double? m_left;

		private double? m_height;

		private double? m_width;

		public double Top
		{
			get
			{
				if (!m_top.HasValue)
				{
					m_top = m_defObject.ChartElementPositionDef.EvaluateTop(ReportScopeInstance, m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return m_top.Value;
			}
		}

		public double Left
		{
			get
			{
				if (!m_left.HasValue)
				{
					m_left = m_defObject.ChartElementPositionDef.EvaluateLeft(ReportScopeInstance, m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return m_left.Value;
			}
		}

		public double Height
		{
			get
			{
				if (!m_height.HasValue)
				{
					m_height = m_defObject.ChartElementPositionDef.EvaluateHeight(ReportScopeInstance, m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return m_height.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!m_width.HasValue)
				{
					m_width = m_defObject.ChartElementPositionDef.EvaluateWidth(ReportScopeInstance, m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		internal ChartElementPositionInstance(ChartElementPosition defObject)
			: base(defObject.ChartDef)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_top = null;
			m_left = null;
			m_height = null;
			m_width = null;
		}
	}
}
