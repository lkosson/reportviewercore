namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBucketInstance : BaseInstance
	{
		private MapBucket m_defObject;

		private object m_startValue;

		private object m_endValue;

		public object StartValue
		{
			get
			{
				if (m_startValue == null)
				{
					m_startValue = m_defObject.MapBucketDef.EvaluateStartValue(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return m_startValue;
			}
		}

		public object EndValue
		{
			get
			{
				if (m_endValue == null)
				{
					m_endValue = m_defObject.MapBucketDef.EvaluateEndValue(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return m_endValue;
			}
		}

		internal MapBucketInstance(MapBucket defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_startValue = null;
			m_endValue = null;
		}
	}
}
