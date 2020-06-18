using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataRegionInstance : MapSpatialDataInstance
	{
		private MapSpatialDataRegion m_defObject;

		private object m_vectorData;

		public object VectorData
		{
			get
			{
				if (m_vectorData == null)
				{
					m_vectorData = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion)m_defObject.MapSpatialDataDef).EvaluateVectorData(m_defObject.ReportScope.ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return m_vectorData;
			}
		}

		internal MapSpatialDataRegionInstance(MapSpatialDataRegion defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_vectorData = null;
		}
	}
}
