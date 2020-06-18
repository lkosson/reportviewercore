using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataSetInstance : MapSpatialDataInstance
	{
		private MapSpatialDataSet m_defObject;

		private string m_dataSetName;

		private string m_spatialField;

		public string DataSetName
		{
			get
			{
				if (m_dataSetName == null)
				{
					m_dataSetName = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)m_defObject.MapSpatialDataDef).EvaluateDataSetName(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_dataSetName;
			}
		}

		public string SpatialField
		{
			get
			{
				if (m_spatialField == null)
				{
					m_spatialField = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)m_defObject.MapSpatialDataDef).EvaluateSpatialField(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_spatialField;
			}
		}

		internal MapSpatialDataSetInstance(MapSpatialDataSet defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_dataSetName = null;
			m_spatialField = null;
		}
	}
}
