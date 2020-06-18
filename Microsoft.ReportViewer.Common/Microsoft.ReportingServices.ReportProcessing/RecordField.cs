using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordField
	{
		private object m_fieldValue;

		private bool m_isAggregationField;

		private VariantList m_fieldPropertyValues;

		[NonSerialized]
		private DataFieldStatus m_fieldStatus;

		[NonSerialized]
		private Hashtable m_properties;

		internal object FieldValue
		{
			get
			{
				return m_fieldValue;
			}
			set
			{
				m_fieldValue = value;
			}
		}

		internal bool IsAggregationField
		{
			get
			{
				return m_isAggregationField;
			}
			set
			{
				m_isAggregationField = value;
			}
		}

		internal VariantList FieldPropertyValues
		{
			get
			{
				return m_fieldPropertyValues;
			}
			set
			{
				m_fieldPropertyValues = value;
			}
		}

		internal bool IsOverflow => DataFieldStatus.Overflow == m_fieldStatus;

		internal bool IsUnSupportedDataType => DataFieldStatus.UnSupportedDataType == m_fieldStatus;

		internal bool IsError => DataFieldStatus.IsError == m_fieldStatus;

		internal DataFieldStatus FieldStatus
		{
			get
			{
				return m_fieldStatus;
			}
			set
			{
				Global.Tracer.Assert(m_fieldStatus == DataFieldStatus.None);
				m_fieldStatus = value;
			}
		}

		internal RecordField(FieldImpl field)
		{
			m_fieldStatus = field.FieldStatus;
			m_properties = field.Properties;
			if (m_fieldStatus == DataFieldStatus.None)
			{
				m_fieldValue = field.Value;
				m_isAggregationField = field.IsAggregationField;
			}
		}

		internal RecordField()
		{
		}

		internal void SetProperty(string propertyName, object propertyValue)
		{
			if (m_properties == null)
			{
				m_properties = new Hashtable();
			}
			m_properties[propertyName] = propertyValue;
		}

		internal object GetProperty(string propertyName)
		{
			Global.Tracer.Assert(m_properties != null);
			return m_properties[propertyName];
		}

		internal void PopulateFieldPropertyValues(StringList propertyNames)
		{
			if (propertyNames == null)
			{
				return;
			}
			int count = propertyNames.Count;
			m_fieldPropertyValues = new VariantList(count);
			for (int i = 0; i < count; i++)
			{
				object value = null;
				if (m_properties != null)
				{
					value = m_properties[propertyNames[i]];
				}
				m_fieldPropertyValues.Add(value);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.FieldValue, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.IsAggregateField, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FieldPropertyValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
