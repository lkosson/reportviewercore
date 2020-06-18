using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class FieldInstance
	{
		private RecordField m_recordField;

		private FieldInfo m_fieldInfo;

		private ExtendedPropertyCollection m_extendedProperties;

		public object Value
		{
			get
			{
				if (IsMissingRecord)
				{
					return null;
				}
				return m_recordField.FieldValue;
			}
		}

		public bool IsAggregationField
		{
			get
			{
				if (IsMissingRecord)
				{
					return false;
				}
				return m_recordField.IsAggregationField;
			}
		}

		public bool IsOverflow
		{
			get
			{
				if (IsMissingRecord)
				{
					return false;
				}
				return m_recordField.IsOverflow;
			}
		}

		public bool IsUnSupportedDataType
		{
			get
			{
				if (IsMissingRecord)
				{
					return false;
				}
				return m_recordField.IsUnSupportedDataType;
			}
		}

		public bool IsError
		{
			get
			{
				if (IsMissingRecord)
				{
					return true;
				}
				return m_recordField.IsError;
			}
		}

		public ExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				if (m_extendedProperties == null && m_fieldInfo != null)
				{
					m_extendedProperties = new ExtendedPropertyCollection(m_recordField, m_fieldInfo.PropertyNames);
				}
				return m_extendedProperties;
			}
		}

		private bool IsMissingRecord => m_recordField == null;

		internal FieldInstance(FieldInfo fieldInfo, RecordField recordField)
		{
			m_fieldInfo = fieldInfo;
			m_recordField = recordField;
		}

		internal void UpdateRecordField(RecordField field)
		{
			m_recordField = field;
			if (m_extendedProperties != null)
			{
				m_extendedProperties.UpdateRecordField(m_recordField);
			}
		}
	}
}
