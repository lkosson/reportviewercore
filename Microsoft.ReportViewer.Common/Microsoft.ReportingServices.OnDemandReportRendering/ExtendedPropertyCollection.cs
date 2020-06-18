using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ExtendedPropertyCollection : ReportElementCollectionBase<ExtendedProperty>
	{
		private ExtendedProperty[] m_extendedProperties;

		private List<string> m_extendedPropertyNames;

		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordField m_recordField;

		public override int Count
		{
			get
			{
				if (m_extendedPropertyNames == null)
				{
					return 0;
				}
				return m_extendedPropertyNames.Count;
			}
		}

		public override ExtendedProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_extendedProperties == null)
				{
					m_extendedProperties = new ExtendedProperty[Count];
				}
				if (m_extendedProperties[index] == null)
				{
					m_extendedProperties[index] = new ExtendedProperty(m_extendedPropertyNames[index], GetFieldPropertyValue(index));
				}
				return m_extendedProperties[index];
			}
		}

		public ExtendedProperty this[string name]
		{
			get
			{
				if (m_extendedPropertyNames != null)
				{
					return this[m_extendedPropertyNames.IndexOf(name)];
				}
				return null;
			}
		}

		internal ExtendedPropertyCollection(Microsoft.ReportingServices.ReportIntermediateFormat.RecordField field, List<string> extendedPropertyNames)
		{
			m_recordField = field;
			m_extendedPropertyNames = extendedPropertyNames;
		}

		internal void UpdateRecordField(Microsoft.ReportingServices.ReportIntermediateFormat.RecordField field)
		{
			m_recordField = field;
			if (m_extendedProperties != null)
			{
				for (int i = 0; i < m_extendedProperties.Length; i++)
				{
					m_extendedProperties[i]?.UpdateValue(GetFieldPropertyValue(i));
				}
			}
		}

		private object GetFieldPropertyValue(int index)
		{
			if (m_recordField == null)
			{
				return null;
			}
			return m_recordField.FieldPropertyValues[index];
		}
	}
}
