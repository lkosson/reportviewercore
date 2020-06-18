using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapElementViewInstance : MapViewInstance
	{
		private MapElementView m_defObject;

		private string m_layerName;

		public string LayerName
		{
			get
			{
				if (m_layerName == null)
				{
					m_layerName = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView)m_defObject.MapViewDef).EvaluateLayerName(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_layerName;
			}
		}

		internal MapElementViewInstance(MapElementView defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_layerName = null;
		}
	}
}
