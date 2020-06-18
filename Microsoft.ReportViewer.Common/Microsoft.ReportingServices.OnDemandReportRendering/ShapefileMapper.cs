using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class ShapefileMapper : SpatialDataMapper
	{
		private MapShapefile m_shapefile;

		private bool m_shapefileMatchingLayer;

		internal ShapefileMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, MapControl coreMap, MapMapper mapMapper)
			: base(vectorLayerMapper, spatialElementsDictionary, coreMap, mapMapper)
		{
			m_shapefile = (MapShapefile)m_mapVectorLayer.MapSpatialData;
		}

		internal void SubscribeToAddedEvent()
		{
			m_coreMap.ElementAdded += CoreMap_ElementAdded;
		}

		internal void UnsubscribeToAddedEvent()
		{
			m_coreMap.ElementAdded -= CoreMap_ElementAdded;
		}

		private void CoreMap_ElementAdded(object sender, ElementEventArgs e)
		{
			if (!(e.MapElement is ISpatialElement))
			{
				return;
			}
			if (!m_shapefileMatchingLayer)
			{
				if (!m_vectorLayerMapper.IsValidSpatialElement((ISpatialElement)e.MapElement))
				{
					throw new RenderingObjectModelException(RPRes.rsMapShapefileTypeMismatch(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name, m_shapefile.Instance.Source));
				}
				m_shapefileMatchingLayer = true;
			}
			SpatialElementInfo spatialElementInfo = new SpatialElementInfo();
			spatialElementInfo.CoreSpatialElement = (ISpatialElement)e.MapElement;
			OnSpatialElementAdded(spatialElementInfo);
		}

		internal override void Populate()
		{
			SubscribeToAddedEvent();
			GetFieldNameMapping(out string[] dbfNames, out string[] uniqueNames);
			Stream stream = m_shapefile.Instance.Stream;
			if (stream != null)
			{
				m_coreMap.mapCore.MaxSpatialElementCount = m_mapMapper.RemainingSpatialElementCount;
				m_coreMap.mapCore.MaxSpatialPointCount = m_mapMapper.RemainingTotalPointCount;
				switch (m_coreMap.mapCore.LoadFromShapeFileStreams(stream, m_shapefile.Instance.DBFStream, dbfNames, uniqueNames, m_mapVectorLayer.Name, m_mapVectorLayer.Name))
				{
				case SpatialLoadResult.MaxSpatialElementCountReached:
					m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumSpatialElementCountReached(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name);
					break;
				case SpatialLoadResult.MaxSpatialPointCountReached:
					m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumTotalPointCountReached(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name);
					break;
				}
				UnsubscribeToAddedEvent();
				return;
			}
			throw new RenderingObjectModelException(RPRes.rsMapCannotLoadShapefile(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_shapefile.Instance.Source));
		}

		private void GetFieldNameMapping(out string[] dbfNames, out string[] uniqueNames)
		{
			MapFieldNameCollection mapFieldNames = m_shapefile.MapFieldNames;
			if (mapFieldNames == null)
			{
				dbfNames = null;
				uniqueNames = null;
				return;
			}
			dbfNames = new string[mapFieldNames.Count];
			uniqueNames = new string[mapFieldNames.Count];
			for (int i = 0; i < mapFieldNames.Count; i++)
			{
				string fieldName = GetFieldName(mapFieldNames[i]);
				dbfNames[i] = fieldName;
				uniqueNames[i] = SpatialDataMapper.GetUniqueFieldName(m_mapVectorLayer.Name, fieldName);
			}
		}
	}
}
