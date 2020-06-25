using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class CoreSpatialElementManager
	{
		protected MapControl m_coreMap;

		protected MapVectorLayer m_mapVectorLayer;

		internal abstract Microsoft.Reporting.Map.WebForms.FieldCollection FieldDefinitions
		{
			get;
		}

		protected abstract NamedCollection SpatialElements
		{
			get;
		}

		internal CoreSpatialElementManager(MapControl coreMap, MapVectorLayer mapVectorLayer)
		{
			m_coreMap = coreMap;
			m_mapVectorLayer = mapVectorLayer;
		}

		internal static Type GetFieldType(object value)
		{
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
				return typeof(int);
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Decimal:
				return typeof(decimal);
			case TypeCode.Single:
			case TypeCode.Double:
				return typeof(double);
			case TypeCode.DateTime:
				return typeof(DateTime);
			case TypeCode.Boolean:
				return typeof(bool);
			default:
				return typeof(string);
			}
		}

		internal void AddFieldDefinition(string fieldName, Type type)
		{
			Microsoft.Reporting.Map.WebForms.Field field = new Microsoft.Reporting.Map.WebForms.Field();
			field.Name = fieldName;
			field.Type = type;
			FieldDefinitions.Add(field);
		}

		internal void AddFieldValue(ISpatialElement spatialElement, string fieldName, object value)
		{
			try
			{
				spatialElement[fieldName] = value;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				spatialElement[fieldName] = value.ToString();
				m_mapVectorLayer.MapDef.RenderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsMapUnsupportedValueFieldType, Severity.Warning, m_mapVectorLayer.MapDef.MapDef.ObjectType, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name, fieldName);
			}
		}

		internal string AddRuleField(object dataValue)
		{
			string text = GenerateUniqueFieldName();
			AddFieldDefinition(text, GetFieldType(dataValue));
			return text;
		}

		private string GenerateUniqueFieldName()
		{
			int num = FieldDefinitions.Count;
			string text;
			while (true)
			{
				text = num.ToString(CultureInfo.InvariantCulture);
				if (FieldDefinitions.GetByName(text) == null)
				{
					break;
				}
				num++;
			}
			return text;
		}

		internal int GetSpatialElementCount()
		{
			int num = 0;
			foreach (ISpatialElement spatialElement in SpatialElements)
			{
				if (!(spatialElement.Layer != m_mapVectorLayer.Name))
				{
					num++;
				}
			}
			return num;
		}

		internal int GetDistinctValuesCount(string fieldName)
		{
			NamedCollection spatialElements = SpatialElements;
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			foreach (ISpatialElement item in spatialElements)
			{
				if (!(item.Layer != m_mapVectorLayer.Name))
				{
					object obj = item[fieldName];
					if (obj != null && !dictionary.ContainsKey(obj))
					{
						dictionary.Add(obj, null);
					}
				}
			}
			return dictionary.Count;
		}

		internal abstract void AddSpatialElement(ISpatialElement spatialElement);

		internal abstract void RemoveSpatialElement(ISpatialElement spatialElement);

		internal abstract ISpatialElement CreateSpatialElement();
	}
}
