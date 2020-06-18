using Microsoft.Reporting.Map.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class SpatialDataMapper
	{
		protected MapMapper m_mapMapper;

		protected MapVectorLayer m_mapVectorLayer;

		protected VectorLayerMapper m_vectorLayerMapper;

		protected MapControl m_coreMap;

		private List<Type> m_keyTypes;

		private Dictionary<SpatialElementKey, SpatialElementInfoGroup> m_spatialElementsDictionary;

		internal List<Type> KeyTypes => m_keyTypes;

		internal SpatialDataMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, MapControl coreMap, MapMapper mapMapper)
		{
			m_vectorLayerMapper = vectorLayerMapper;
			m_mapVectorLayer = m_vectorLayerMapper.m_mapVectorLayer;
			m_spatialElementsDictionary = spatialElementsDictionary;
			m_coreMap = coreMap;
			m_mapMapper = mapMapper;
		}

		internal abstract void Populate();

		protected void OnSpatialElementAdded(SpatialElementInfo spatialElementInfo)
		{
			m_vectorLayerMapper.OnSpatialElementAdded(spatialElementInfo.CoreSpatialElement);
			m_mapMapper.OnSpatialElementAdded(spatialElementInfo);
			SpatialElementKey spatialElementKey = CreateCoreSpatialElementKey(spatialElementInfo.CoreSpatialElement);
			if (m_mapVectorLayer.MapDataRegion != null && m_keyTypes == null && spatialElementKey.KeyValues != null)
			{
				RegisterKeyTypes(spatialElementKey);
			}
			SpatialElementInfoGroup spatialElementInfoGroup;
			if (!m_spatialElementsDictionary.ContainsKey(spatialElementKey))
			{
				spatialElementInfoGroup = new SpatialElementInfoGroup();
				m_spatialElementsDictionary.Add(spatialElementKey, spatialElementInfoGroup);
			}
			else
			{
				spatialElementInfoGroup = m_spatialElementsDictionary[spatialElementKey];
			}
			spatialElementInfoGroup.Elements.Add(spatialElementInfo);
		}

		private void RegisterKeyTypes(SpatialElementKey key)
		{
			m_keyTypes = new List<Type>();
			foreach (object keyValue in key.KeyValues)
			{
				if (keyValue == null)
				{
					m_keyTypes.Add(null);
				}
				else
				{
					m_keyTypes.Add(keyValue.GetType());
				}
			}
		}

		private SpatialElementKey CreateCoreSpatialElementKey(ISpatialElement coreSpatialElement)
		{
			return CreateCoreSpatialElementKey(coreSpatialElement, m_mapVectorLayer.MapBindingFieldPairs, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name);
		}

		internal static SpatialElementKey CreateCoreSpatialElementKey(ISpatialElement coreSpatialElement, MapBindingFieldPairCollection mapBindingFieldPairs, string mapName, string layerName)
		{
			if (mapBindingFieldPairs == null)
			{
				return new SpatialElementKey(null);
			}
			List<object> list = new List<object>();
			for (int i = 0; i < mapBindingFieldPairs.Count; i++)
			{
				list.Add(GetBindingFieldValue(coreSpatialElement, mapBindingFieldPairs[i], mapName, layerName));
			}
			return new SpatialElementKey(list);
		}

		private static object GetBindingFieldValue(ISpatialElement coreSpatialElement, MapBindingFieldPair bindingFieldPair, string mapName, string layerName)
		{
			string bindingFieldName = GetBindingFieldName(bindingFieldPair);
			if (bindingFieldName == null)
			{
				return null;
			}
			return coreSpatialElement[GetUniqueFieldName(layerName, bindingFieldName)];
		}

		protected string GetUniqueFieldName(string fieldName)
		{
			return GetUniqueFieldName(m_mapVectorLayer.Name, fieldName);
		}

		internal static string GetBindingFieldName(MapBindingFieldPair bindingFieldPair)
		{
			ReportStringProperty fieldName = bindingFieldPair.FieldName;
			if (!fieldName.IsExpression)
			{
				return fieldName.Value;
			}
			return bindingFieldPair.Instance.FieldName;
		}

		internal static string GetUniqueFieldName(string layerName, string fieldName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", layerName, fieldName);
		}

		protected string GetFieldName(MapFieldName fieldName)
		{
			_ = fieldName.Name;
			if (!fieldName.Name.IsExpression)
			{
				return fieldName.Name.Value;
			}
			return fieldName.Instance.Name;
		}
	}
}
