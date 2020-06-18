using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapAppearanceRule
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule m_defObject;

		protected MapAppearanceRuleInstance m_instance;

		private ReportVariantProperty m_dataValue;

		private ReportEnumProperty<MapRuleDistributionType> m_distributionType;

		private ReportIntProperty m_bucketCount;

		private ReportVariantProperty m_startValue;

		private ReportVariantProperty m_endValue;

		private MapBucketCollection m_mapBuckets;

		private ReportStringProperty m_legendText;

		public string DataElementName => m_defObject.DataElementName;

		public DataElementOutputTypes DataElementOutput => m_defObject.DataElementOutput;

		public ReportVariantProperty DataValue
		{
			get
			{
				if (m_dataValue == null && m_defObject.DataValue != null)
				{
					m_dataValue = new ReportVariantProperty(m_defObject.DataValue);
				}
				return m_dataValue;
			}
		}

		public ReportEnumProperty<MapRuleDistributionType> DistributionType
		{
			get
			{
				if (m_distributionType == null && m_defObject.DistributionType != null)
				{
					m_distributionType = new ReportEnumProperty<MapRuleDistributionType>(m_defObject.DistributionType.IsExpression, m_defObject.DistributionType.OriginalText, EnumTranslator.TranslateMapRuleDistributionType(m_defObject.DistributionType.StringValue, null));
				}
				return m_distributionType;
			}
		}

		public ReportIntProperty BucketCount
		{
			get
			{
				if (m_bucketCount == null && m_defObject.BucketCount != null)
				{
					m_bucketCount = new ReportIntProperty(m_defObject.BucketCount.IsExpression, m_defObject.BucketCount.OriginalText, m_defObject.BucketCount.IntValue, 0);
				}
				return m_bucketCount;
			}
		}

		public ReportVariantProperty StartValue
		{
			get
			{
				if (m_startValue == null && m_defObject.StartValue != null)
				{
					m_startValue = new ReportVariantProperty(m_defObject.StartValue);
				}
				return m_startValue;
			}
		}

		public ReportVariantProperty EndValue
		{
			get
			{
				if (m_endValue == null && m_defObject.EndValue != null)
				{
					m_endValue = new ReportVariantProperty(m_defObject.EndValue);
				}
				return m_endValue;
			}
		}

		public MapBucketCollection MapBuckets
		{
			get
			{
				if (m_mapBuckets == null && m_defObject.MapBuckets != null)
				{
					m_mapBuckets = new MapBucketCollection(this, m_map);
				}
				return m_mapBuckets;
			}
		}

		public string LegendName => m_defObject.LegendName;

		public ReportStringProperty LegendText
		{
			get
			{
				if (m_legendText == null && m_defObject.LegendText != null)
				{
					m_legendText = new ReportStringProperty(m_defObject.LegendText);
				}
				return m_legendText;
			}
		}

		internal IReportScope ReportScope => m_mapVectorLayer.ReportScope;

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule MapAppearanceRuleDef => m_defObject;

		public MapAppearanceRuleInstance Instance => GetInstance();

		internal MapAppearanceRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			m_defObject = defObject;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal abstract MapAppearanceRuleInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapBuckets != null)
			{
				m_mapBuckets.SetNewContext();
			}
		}
	}
}
