using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class SpatialDataSetMapper : SpatialDataMapper
	{
		private class FieldInfo
		{
			public string UniqueName;

			public int Index;

			public bool DefinitionAdded;
		}

		private CoreSpatialElementManager m_spatialElementManager;

		private MapSpatialDataSet m_spatialDataSet;

		private DataSet m_dataSet;

		private DataSetInstance m_dataSetInstance;

		internal SpatialDataSetMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, CoreSpatialElementManager spatialElementManager, MapControl coreMap, MapMapper mapMapper)
			: base(vectorLayerMapper, spatialElementsDictionary, coreMap, mapMapper)
		{
			m_spatialElementManager = spatialElementManager;
			m_spatialDataSet = (MapSpatialDataSet)m_mapVectorLayer.MapSpatialData;
			m_dataSet = m_spatialDataSet.DataSet;
			m_dataSetInstance = m_dataSet.Instance;
		}

		internal override void Populate()
		{
			int spatialFieldIndex = GetSpatialFieldIndex();
			FieldInfo[] nonSpatialFieldInfos = GetNonSpatialFieldInfos();
			m_dataSetInstance.ResetContext();
			while (m_dataSetInstance.MoveNext())
			{
				ProcessRow(spatialFieldIndex, nonSpatialFieldInfos);
			}
			EnsureFieldDefinitionsCreated(nonSpatialFieldInfos);
			m_dataSetInstance.Close();
		}

		private void EnsureFieldDefinitionsCreated(FieldInfo[] nonSpatialFieldInfos)
		{
			if (nonSpatialFieldInfos == null)
			{
				return;
			}
			foreach (FieldInfo fieldInfo in nonSpatialFieldInfos)
			{
				if (!fieldInfo.DefinitionAdded)
				{
					m_spatialElementManager.AddFieldDefinition(fieldInfo.UniqueName, typeof(string));
				}
			}
		}

		private void ProcessRow(int spatialFieldIndex, FieldInfo[] nonSpatialFieldInfos)
		{
		}

		private void ProcessNonSpatialFields(ISpatialElement spatialElement, FieldInfo[] nonSpatialFieldInfos)
		{
			if (nonSpatialFieldInfos == null)
			{
				return;
			}
			foreach (FieldInfo fieldInfo in nonSpatialFieldInfos)
			{
				if (!fieldInfo.DefinitionAdded)
				{
					fieldInfo.DefinitionAdded = AddFieldDefinition(fieldInfo.UniqueName, m_dataSetInstance.Row[fieldInfo.Index].Value);
				}
				m_spatialElementManager.AddFieldValue(spatialElement, fieldInfo.UniqueName, m_dataSetInstance.Row[fieldInfo.Index].Value);
			}
		}

		private bool AddFieldDefinition(string fieldUniqueName, object value)
		{
			if (value != null)
			{
				m_spatialElementManager.AddFieldDefinition(fieldUniqueName, CoreSpatialElementManager.GetFieldType(value));
				return true;
			}
			return false;
		}

		private FieldInfo[] GetNonSpatialFieldInfos()
		{
			MapFieldNameCollection mapFieldNames = m_spatialDataSet.MapFieldNames;
			if (mapFieldNames == null)
			{
				return null;
			}
			FieldInfo[] array = new FieldInfo[mapFieldNames.Count];
			for (int i = 0; i < mapFieldNames.Count; i++)
			{
				FieldInfo fieldInfo = new FieldInfo();
				string fieldName = GetFieldName(mapFieldNames[i]);
				fieldInfo.UniqueName = GetUniqueFieldName(fieldName);
				fieldInfo.Index = GetFieldIndex(fieldName);
				fieldInfo.DefinitionAdded = false;
				array[i] = fieldInfo;
			}
			return array;
		}

		private int GetSpatialFieldIndex()
		{
			return GetFieldIndex(GetSpatialFieldName());
		}

		private int GetFieldIndex(string fieldName)
		{
			int fieldIndex = m_dataSet.NonCalculatedFields.GetFieldIndex(fieldName);
			if (fieldIndex == -1)
			{
				throw new RenderingObjectModelException(RPRes.rsMapInvalidFieldName(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name, fieldName));
			}
			return fieldIndex;
		}

		private string GetSpatialFieldName()
		{
			ReportStringProperty spatialField = m_spatialDataSet.SpatialField;
			if (!spatialField.IsExpression)
			{
				return spatialField.Value;
			}
			return m_spatialDataSet.Instance.SpatialField;
		}
	}
}
