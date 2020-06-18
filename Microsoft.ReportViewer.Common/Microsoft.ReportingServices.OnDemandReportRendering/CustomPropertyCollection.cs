using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomPropertyCollection : ReportElementCollectionBase<CustomProperty>
	{
		private List<CustomProperty> m_list;

		private Dictionary<string, CustomProperty> m_lookupTable;

		private ReportElement m_reportElementOwner;

		public CustomProperty this[string name]
		{
			get
			{
				if (name != null && m_lookupTable != null)
				{
					CustomProperty value = null;
					if (m_lookupTable.TryGetValue(name, out value))
					{
						return value;
					}
				}
				return null;
			}
		}

		public override CustomProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_list[index];
			}
		}

		public override int Count => m_list.Count;

		internal CustomPropertyCollection()
		{
			m_list = new List<CustomProperty>();
		}

		internal CustomPropertyCollection(IReportScopeInstance romInstance, RenderingContext renderingContext, ReportElement reportElementOwner, ICustomPropertiesHolder customPropertiesHolder, ObjectType objectType, string objectName)
		{
			m_reportElementOwner = reportElementOwner;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList customProperties = customPropertiesHolder.CustomProperties;
			if (customProperties == null)
			{
				m_list = new List<CustomProperty>();
				return;
			}
			bool flag = InstancePathItem.IsValidContext(customPropertiesHolder.InstancePath.InstancePath);
			int count = customProperties.Count;
			m_list = new List<CustomProperty>(count);
			for (int i = 0; i < count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataValue dataValue = customProperties[i];
				string name = null;
				object value = null;
				TypeCode valueTypeCode = TypeCode.Empty;
				if (flag)
				{
					dataValue.EvaluateNameAndValue(m_reportElementOwner, romInstance, customPropertiesHolder.InstancePath, renderingContext.OdpContext, objectType, objectName, out name, out value, out valueTypeCode);
				}
				CustomProperty customProperty = new CustomProperty(m_reportElementOwner, renderingContext, dataValue.Name, dataValue.Value, name, value, valueTypeCode);
				m_list.Add(customProperty);
				if (flag)
				{
					AddPropToLookupTable(name, customProperty);
				}
			}
		}

		internal CustomPropertyCollection(RenderingContext renderingContext, Microsoft.ReportingServices.ReportRendering.CustomPropertyCollection collection)
		{
			if (collection == null)
			{
				m_list = new List<CustomProperty>();
				return;
			}
			int count = collection.Count;
			m_list = new List<CustomProperty>(count);
			for (int i = 0; i < count; i++)
			{
				collection.GetNameValueExpressions(i, out Microsoft.ReportingServices.ReportProcessing.ExpressionInfo nameExpression, out Microsoft.ReportingServices.ReportProcessing.ExpressionInfo valueExpression, out string name, out object value);
				CustomProperty customProperty = new CustomProperty(renderingContext, nameExpression, valueExpression, name, value, TypeCode.Empty);
				m_list.Add(customProperty);
				AddPropToLookupTable(name, customProperty);
			}
		}

		internal CustomProperty Add(RenderingContext renderingContext, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo nameExpr, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo valueExpr)
		{
			CustomProperty customProperty = new CustomProperty(m_reportElementOwner, renderingContext, nameExpr, valueExpr, null, null, TypeCode.Empty);
			Global.Tracer.Assert(customProperty.Instance != null, "prop.Instance != null");
			m_list.Add(customProperty);
			return customProperty;
		}

		internal void UpdateCustomProperties(Microsoft.ReportingServices.ReportRendering.CustomPropertyCollection collection)
		{
			int count = m_list.Count;
			for (int i = 0; i < count; i++)
			{
				string name = null;
				object value = null;
				collection?.GetNameValue(i, out name, out value);
				m_list[i].Update(name, value, TypeCode.Empty);
			}
		}

		internal void UpdateCustomProperties(IReportScopeInstance romInstance, ICustomPropertiesHolder customPropertiesHolder, OnDemandProcessingContext context, ObjectType objectType, string objectName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList customProperties = customPropertiesHolder.CustomProperties;
			int count = m_list.Count;
			bool flag = false;
			if (m_lookupTable == null)
			{
				flag = true;
			}
			for (int i = 0; i < count; i++)
			{
				string name = null;
				object value = null;
				TypeCode valueTypeCode = TypeCode.Empty;
				if (customProperties != null && i < customProperties.Count)
				{
					customProperties[i].EvaluateNameAndValue(m_reportElementOwner, romInstance, customPropertiesHolder.InstancePath, context, objectType, objectName, out name, out value, out valueTypeCode);
				}
				m_list[i].Update(name, value, valueTypeCode);
				if (flag)
				{
					AddPropToLookupTable(name, m_list[i]);
				}
			}
		}

		private void AddPropToLookupTable(string name, CustomProperty property)
		{
			if (m_lookupTable == null)
			{
				m_lookupTable = new Dictionary<string, CustomProperty>(m_list.Count);
			}
			if (name != null && !m_lookupTable.ContainsKey(name))
			{
				m_lookupTable.Add(name, property);
			}
		}

		internal void ConstructCustomPropertyDefinitions(Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList dataValueDefs)
		{
			Global.Tracer.Assert(dataValueDefs != null && m_list.Count == dataValueDefs.Count, "m_list.Count == dataValueDefs.Count");
			for (int i = 0; i < m_list.Count; i++)
			{
				CustomProperty customProperty = m_list[i];
				customProperty.ConstructCustomPropertyDefinition(dataValueDefs[i]);
				if (customProperty.Instance != null && customProperty.Instance.Name != null)
				{
					AddPropToLookupTable(customProperty.Instance.Name, customProperty);
				}
			}
		}

		internal void GetDynamicValues(out List<string> customPropertyNames, out List<object> customPropertyValues)
		{
			customPropertyNames = new List<string>(m_list.Count);
			customPropertyValues = new List<object>(m_list.Count);
			bool flag = false;
			for (int i = 0; i < m_list.Count; i++)
			{
				CustomProperty customProperty = m_list[i];
				string item = null;
				if (customProperty.Name.IsExpression)
				{
					flag = true;
					item = customProperty.Instance.Name;
				}
				object item2 = null;
				if (customProperty.Value.IsExpression)
				{
					flag = true;
					item2 = customProperty.Instance.Value;
				}
				customPropertyNames.Add(item);
				customPropertyValues.Add(item2);
			}
			if (!flag)
			{
				customPropertyNames = null;
				customPropertyValues = null;
			}
		}

		internal void SetDynamicValues(List<string> customPropertyNames, List<object> customPropertyValues)
		{
			if (customPropertyNames == null && customPropertyValues == null)
			{
				return;
			}
			Global.Tracer.Assert(customPropertyNames != null && customPropertyValues != null && customPropertyNames.Count == customPropertyValues.Count && m_list.Count == customPropertyNames.Count, "Chck customPropertyNames and customPropertyValues consistency");
			for (int i = 0; i < m_list.Count; i++)
			{
				CustomProperty customProperty = m_list[i];
				if (customProperty.Name.IsExpression)
				{
					customProperty.Instance.Name = customPropertyNames[i];
				}
				if (customProperty.Value.IsExpression)
				{
					customProperty.Instance.Value = customPropertyValues[i];
				}
			}
		}
	}
}
