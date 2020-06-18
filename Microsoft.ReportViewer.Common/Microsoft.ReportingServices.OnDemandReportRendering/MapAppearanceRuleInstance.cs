namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapAppearanceRuleInstance : BaseInstance
	{
		private MapAppearanceRule m_defObject;

		private object m_dataValue;

		private MapRuleDistributionType? m_distributionType;

		private int? m_bucketCount;

		private object m_startValue;

		private object m_endValue;

		private string m_legendText;

		private bool m_legendTextEvaluated;

		public object DataValue
		{
			get
			{
				if (m_dataValue == null)
				{
					m_dataValue = m_defObject.MapAppearanceRuleDef.EvaluateDataValue(m_defObject.ReportScope.ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return m_dataValue;
			}
		}

		public MapRuleDistributionType DistributionType
		{
			get
			{
				if (!m_distributionType.HasValue)
				{
					m_distributionType = m_defObject.MapAppearanceRuleDef.EvaluateDistributionType(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_distributionType.Value;
			}
		}

		public int BucketCount
		{
			get
			{
				if (!m_bucketCount.HasValue)
				{
					m_bucketCount = m_defObject.MapAppearanceRuleDef.EvaluateBucketCount(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_bucketCount.Value;
			}
		}

		public object StartValue
		{
			get
			{
				if (m_startValue == null)
				{
					m_startValue = m_defObject.MapAppearanceRuleDef.EvaluateStartValue(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
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
					m_endValue = m_defObject.MapAppearanceRuleDef.EvaluateEndValue(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return m_endValue;
			}
		}

		public string LegendText
		{
			get
			{
				if (!m_legendTextEvaluated)
				{
					m_legendText = m_defObject.MapAppearanceRuleDef.EvaluateLegendText(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_legendTextEvaluated = true;
				}
				return m_legendText;
			}
		}

		internal MapAppearanceRuleInstance(MapAppearanceRule defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_dataValue = null;
			m_distributionType = null;
			m_bucketCount = null;
			m_startValue = null;
			m_endValue = null;
			m_legendText = null;
			m_legendTextEvaluated = false;
		}
	}
}
