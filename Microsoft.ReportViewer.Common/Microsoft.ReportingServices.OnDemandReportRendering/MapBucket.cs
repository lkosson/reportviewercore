using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBucket : MapObjectCollectionItem
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket m_defObject;

		private ReportVariantProperty m_startValue;

		private ReportVariantProperty m_endValue;

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

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket MapBucketDef => m_defObject;

		public MapBucketInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapBucketInstance(this);
				}
				return (MapBucketInstance)m_instance;
			}
		}

		internal MapBucket(Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
