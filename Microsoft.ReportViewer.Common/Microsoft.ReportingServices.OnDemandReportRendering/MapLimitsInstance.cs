namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLimitsInstance : BaseInstance
	{
		private MapLimits m_defObject;

		private double? m_minimumX;

		private double? m_minimumY;

		private double? m_maximumX;

		private double? m_maximumY;

		private bool? m_limitToData;

		public double MinimumX
		{
			get
			{
				if (!m_minimumX.HasValue)
				{
					m_minimumX = m_defObject.MapLimitsDef.EvaluateMinimumX(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_minimumX.Value;
			}
		}

		public double MinimumY
		{
			get
			{
				if (!m_minimumY.HasValue)
				{
					m_minimumY = m_defObject.MapLimitsDef.EvaluateMinimumY(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_minimumY.Value;
			}
		}

		public double MaximumX
		{
			get
			{
				if (!m_maximumX.HasValue)
				{
					m_maximumX = m_defObject.MapLimitsDef.EvaluateMaximumX(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_maximumX.Value;
			}
		}

		public double MaximumY
		{
			get
			{
				if (!m_maximumY.HasValue)
				{
					m_maximumY = m_defObject.MapLimitsDef.EvaluateMaximumY(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_maximumY.Value;
			}
		}

		public bool LimitToData
		{
			get
			{
				if (!m_limitToData.HasValue)
				{
					m_limitToData = m_defObject.MapLimitsDef.EvaluateLimitToData(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_limitToData.Value;
			}
		}

		internal MapLimitsInstance(MapLimits defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_minimumX = null;
			m_minimumY = null;
			m_maximumX = null;
			m_maximumY = null;
			m_limitToData = null;
		}
	}
}
