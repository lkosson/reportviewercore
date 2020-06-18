using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.SqlServer.Types;
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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			if (!m_mapMapper.CanAddSpatialElement)
			{
				return;
			}
			object value = m_dataSetInstance.Row[spatialFieldIndex].Value;
			ISpatialElement spatialElement = null;
			if (value is SqlGeography)
			{
				spatialElement = m_spatialElementManager.AddGeography((SqlGeography)(object)(SqlGeography)value, m_mapVectorLayer.Name);
			}
			else
			{
				if (!(value is SqlGeometry))
				{
					throw new RenderingObjectModelException(RPRes.rsMapInvalidSpatialFieldType(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name));
				}
				spatialElement = m_spatialElementManager.AddGeometry((SqlGeometry)(object)(SqlGeometry)value, m_mapVectorLayer.Name);
			}
			if (spatialElement != null)
			{
				ProcessNonSpatialFields(spatialElement, nonSpatialFieldInfos);
				SpatialElementInfo spatialElementInfo = new SpatialElementInfo();
				spatialElementInfo.CoreSpatialElement = spatialElement;
				OnSpatialElementAdded(spatialElementInfo);
			}
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
