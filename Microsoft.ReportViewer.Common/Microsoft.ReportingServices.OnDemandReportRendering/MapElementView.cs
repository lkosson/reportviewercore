using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapElementView : MapView
	{
		private ReportStringProperty m_layerName;

		private MapBindingFieldPairCollection m_mapBindingFieldPairs;

		public ReportStringProperty LayerName
		{
			get
			{
				if (m_layerName == null && MapElementViewDef.LayerName != null)
				{
					m_layerName = new ReportStringProperty(MapElementViewDef.LayerName);
				}
				return m_layerName;
			}
		}

		public MapBindingFieldPairCollection MapBindingFieldPairs
		{
			get
			{
				if (m_mapBindingFieldPairs == null && MapElementViewDef.MapBindingFieldPairs != null)
				{
					m_mapBindingFieldPairs = new MapBindingFieldPairCollection(MapElementViewDef.MapBindingFieldPairs, m_map);
				}
				return m_mapBindingFieldPairs;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView MapElementViewDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView)base.MapViewDef;

		public new MapElementViewInstance Instance => (MapElementViewInstance)GetInstance();

		internal MapElementView(Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView defObject, Map map)
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
				m_instance = new MapElementViewInstance(this);
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
			if (m_mapBindingFieldPairs != null)
			{
				m_mapBindingFieldPairs.SetNewContext();
			}
		}
	}
}
