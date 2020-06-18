using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomView : MapView
	{
		private ReportDoubleProperty m_centerX;

		private ReportDoubleProperty m_centerY;

		public ReportDoubleProperty CenterX
		{
			get
			{
				if (m_centerX == null && MapCustomViewDef.CenterX != null)
				{
					m_centerX = new ReportDoubleProperty(MapCustomViewDef.CenterX);
				}
				return m_centerX;
			}
		}

		public ReportDoubleProperty CenterY
		{
			get
			{
				if (m_centerY == null && MapCustomViewDef.CenterY != null)
				{
					m_centerY = new ReportDoubleProperty(MapCustomViewDef.CenterY);
				}
				return m_centerY;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView MapCustomViewDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView)base.MapViewDef;

		public new MapCustomViewInstance Instance => (MapCustomViewInstance)GetInstance();

		internal MapCustomView(Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapViewInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapCustomViewInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
