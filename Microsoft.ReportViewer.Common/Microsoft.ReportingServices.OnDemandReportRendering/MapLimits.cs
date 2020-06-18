using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLimits
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits m_defObject;

		private MapLimitsInstance m_instance;

		private ReportDoubleProperty m_minimumX;

		private ReportDoubleProperty m_minimumY;

		private ReportDoubleProperty m_maximumX;

		private ReportDoubleProperty m_maximumY;

		private ReportBoolProperty m_limitToData;

		public ReportDoubleProperty MinimumX
		{
			get
			{
				if (m_minimumX == null && m_defObject.MinimumX != null)
				{
					m_minimumX = new ReportDoubleProperty(m_defObject.MinimumX);
				}
				return m_minimumX;
			}
		}

		public ReportDoubleProperty MinimumY
		{
			get
			{
				if (m_minimumY == null && m_defObject.MinimumY != null)
				{
					m_minimumY = new ReportDoubleProperty(m_defObject.MinimumY);
				}
				return m_minimumY;
			}
		}

		public ReportDoubleProperty MaximumX
		{
			get
			{
				if (m_maximumX == null && m_defObject.MaximumX != null)
				{
					m_maximumX = new ReportDoubleProperty(m_defObject.MaximumX);
				}
				return m_maximumX;
			}
		}

		public ReportDoubleProperty MaximumY
		{
			get
			{
				if (m_maximumY == null && m_defObject.MaximumY != null)
				{
					m_maximumY = new ReportDoubleProperty(m_defObject.MaximumY);
				}
				return m_maximumY;
			}
		}

		public ReportBoolProperty LimitToData
		{
			get
			{
				if (m_limitToData == null && m_defObject.LimitToData != null)
				{
					m_limitToData = new ReportBoolProperty(m_defObject.LimitToData);
				}
				return m_limitToData;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits MapLimitsDef => m_defObject;

		public MapLimitsInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapLimitsInstance(this);
				}
				return m_instance;
			}
		}

		internal MapLimits(Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
