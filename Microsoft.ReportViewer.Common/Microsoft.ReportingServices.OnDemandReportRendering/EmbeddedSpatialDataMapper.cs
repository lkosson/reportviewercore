using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class EmbeddedSpatialDataMapper : SpatialDataMapper
	{
		private ISpatialElementCollection m_embeddedCollection;

		private CoreSpatialElementManager m_spatialElementManager;

		internal EmbeddedSpatialDataMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, ISpatialElementCollection embeddedCollection, CoreSpatialElementManager spatialElementManager, MapControl coreMap, MapMapper mapMapper)
			: base(vectorLayerMapper, spatialElementsDictionary, coreMap, mapMapper)
		{
			m_spatialElementManager = spatialElementManager;
			m_embeddedCollection = embeddedCollection;
		}

		internal override void Populate()
		{
			AddFieldDefinitions();
			AddSpatialElements();
		}

		private void AddFieldDefinitions()
		{
			MapFieldDefinitionCollection mapFieldDefinitions = m_mapVectorLayer.MapFieldDefinitions;
			if (mapFieldDefinitions == null)
			{
				return;
			}
			foreach (MapFieldDefinition item in mapFieldDefinitions)
			{
				m_spatialElementManager.AddFieldDefinition(GetUniqueFieldName(item.Name), GetFieldType(item.DataType));
			}
		}

		private void AddSpatialElements()
		{
			for (int i = 0; i < m_embeddedCollection.Count; i++)
			{
				AddSpatialElement(m_embeddedCollection.GetItem(i));
			}
		}

		private void AddSpatialElement(MapSpatialElement embeddedElement)
		{
		}

		private void ProcessNonSpatialFields(MapSpatialElement embeddedElement, ISpatialElement spatialElement)
		{
			MapFieldCollection mapFields = embeddedElement.MapFields;
			if (mapFields == null)
			{
				return;
			}
			MapFieldDefinitionCollection mapFieldDefinitions = m_mapVectorLayer.MapFieldDefinitions;
			if (mapFieldDefinitions == null)
			{
				return;
			}
			foreach (MapField item in mapFields)
			{
				MapFieldDefinition fieldDefinition = mapFieldDefinitions.GetFieldDefinition(item.Name);
				if (fieldDefinition == null)
				{
					throw new RenderingObjectModelException(RPRes.rsMapInvalidFieldName(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name, item.Name));
				}
				m_spatialElementManager.AddFieldValue(spatialElement, GetUniqueFieldName(item.Name), GetFieldValue(item.Value, fieldDefinition.DataType));
			}
		}

		private Type GetFieldType(MapDataType dataType)
		{
			switch (dataType)
			{
			case MapDataType.Boolean:
				return typeof(bool);
			case MapDataType.DateTime:
				return typeof(DateTime);
			case MapDataType.Float:
				return typeof(double);
			case MapDataType.Integer:
				return typeof(int);
			case MapDataType.Decimal:
				return typeof(decimal);
			default:
				return typeof(string);
			}
		}

		private object GetFieldValue(string stringValue, MapDataType dataType)
		{
			try
			{
				switch (dataType)
				{
				case MapDataType.Boolean:
					return Convert.ToBoolean(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.DateTime:
					return Convert.ToDateTime(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.Float:
					return Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.Integer:
					return Convert.ToInt32(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.Decimal:
					return Convert.ToDecimal(stringValue, CultureInfo.InvariantCulture);
				default:
					return stringValue;
				}
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}
	}
}
