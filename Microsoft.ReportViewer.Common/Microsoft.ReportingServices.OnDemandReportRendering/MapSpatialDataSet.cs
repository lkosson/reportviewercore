using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataSet : MapSpatialData
	{
		private ReportStringProperty m_dataSetName;

		private ReportStringProperty m_spatialField;

		private MapFieldNameCollection m_mapFieldNames;

		public ReportStringProperty DataSetName
		{
			get
			{
				if (m_dataSetName == null && MapSpatialDataSetDef.DataSetName != null)
				{
					m_dataSetName = new ReportStringProperty(MapSpatialDataSetDef.DataSetName);
				}
				return m_dataSetName;
			}
		}

		public ReportStringProperty SpatialField
		{
			get
			{
				if (m_spatialField == null && MapSpatialDataSetDef.SpatialField != null)
				{
					m_spatialField = new ReportStringProperty(MapSpatialDataSetDef.SpatialField);
				}
				return m_spatialField;
			}
		}

		public MapFieldNameCollection MapFieldNames
		{
			get
			{
				if (m_mapFieldNames == null && MapSpatialDataSetDef.MapFieldNames != null)
				{
					m_mapFieldNames = new MapFieldNameCollection(MapSpatialDataSetDef.MapFieldNames, m_map);
				}
				return m_mapFieldNames;
			}
		}

		internal DataSet DataSet
		{
			get
			{
				string text = "";
				DataSet dataSet = null;
				IDefinitionPath parentDefinitionPath = base.MapDef.ParentDefinitionPath;
				while (parentDefinitionPath.ParentDefinitionPath != null && !(parentDefinitionPath is Report))
				{
					parentDefinitionPath = parentDefinitionPath.ParentDefinitionPath;
				}
				if (parentDefinitionPath is Report)
				{
					text = EvaluateDataSetName();
					dataSet = ((Report)parentDefinitionPath).DataSets[text];
				}
				if (dataSet == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidDataSetName, base.MapDef.MapDef.ObjectType, base.MapDef.Name.MarkAsPrivate(), "DataSetName", text.MarkAsPrivate());
				}
				return dataSet;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet MapSpatialDataSetDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)base.MapSpatialDataDef;

		public new MapSpatialDataSetInstance Instance => (MapSpatialDataSetInstance)GetInstance();

		internal MapSpatialDataSet(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		private string EvaluateDataSetName()
		{
			ReportStringProperty dataSetName = DataSetName;
			if (!dataSetName.IsExpression)
			{
				return dataSetName.Value;
			}
			return Instance.DataSetName;
		}

		internal override MapSpatialDataInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapSpatialDataSetInstance(this);
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
			if (m_mapFieldNames != null)
			{
				m_mapFieldNames.SetNewContext();
			}
		}
	}
}
