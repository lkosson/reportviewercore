using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class FieldImpl : Field
	{
		private object m_value;

		private bool m_isAggregationField;

		private bool m_aggregationFieldChecked;

		private DataFieldStatus m_fieldStatus;

		private string m_exceptionMessage;

		private Hashtable m_properties;

		private Microsoft.ReportingServices.ReportProcessing.Field m_fieldDef;

		private bool m_usedInExpression;

		public override object this[string key]
		{
			get
			{
				if (key == null)
				{
					return null;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "Value", ignoreCase: true) == 0)
				{
					return Value;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "IsMissing", ignoreCase: true) == 0)
				{
					return IsMissing;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "LevelNumber", ignoreCase: true) == 0)
				{
					return LevelNumber;
				}
				return GetProperty(key);
			}
		}

		public override object Value
		{
			get
			{
				if (m_fieldStatus == DataFieldStatus.None)
				{
					if (m_value is CalculatedFieldWrapper)
					{
						return ((CalculatedFieldWrapper)m_value).Value;
					}
					return m_value;
				}
				throw new ReportProcessingException_FieldError(m_fieldStatus, m_exceptionMessage);
			}
		}

		public override bool IsMissing => DataFieldStatus.IsMissing == m_fieldStatus;

		public override string UniqueName => GetProperty("UniqueName") as string;

		public override string BackgroundColor => GetProperty("BackgroundColor") as string;

		public override string Color => GetProperty("Color") as string;

		public override string FontFamily => GetProperty("FontFamily") as string;

		public override string FontSize => GetProperty("FontSize") as string;

		public override string FontWeight => GetProperty("FontWeight") as string;

		public override string FontStyle => GetProperty("FontStyle") as string;

		public override string TextDecoration => GetProperty("TextDecoration") as string;

		public override string FormattedValue => GetProperty("FormattedValue") as string;

		public override object Key => GetProperty("Key");

		public override int LevelNumber
		{
			get
			{
				object property = GetProperty("LevelNumber");
				if (property == null)
				{
					return 0;
				}
				if (property is int)
				{
					return (int)property;
				}
				bool valid;
				int result = DataTypeUtility.ConvertToInt32(DataAggregate.GetTypeCode(property), property, out valid);
				if (valid)
				{
					return result;
				}
				return 0;
			}
		}

		public override string ParentUniqueName => GetProperty("ParentUniqueName") as string;

		internal DataFieldStatus FieldStatus => m_fieldStatus;

		internal string ExceptionMessage => m_exceptionMessage;

		internal bool IsAggregationField => m_isAggregationField;

		internal bool AggregationFieldChecked
		{
			get
			{
				return m_aggregationFieldChecked;
			}
			set
			{
				m_aggregationFieldChecked = value;
			}
		}

		internal Hashtable Properties => m_properties;

		internal Microsoft.ReportingServices.ReportProcessing.Field FieldDef => m_fieldDef;

		internal bool UsedInExpression
		{
			get
			{
				return m_usedInExpression;
			}
			set
			{
				m_usedInExpression = value;
			}
		}

		internal FieldImpl(object value, bool isAggregationField, Microsoft.ReportingServices.ReportProcessing.Field fieldDef)
		{
			m_value = value;
			m_isAggregationField = isAggregationField;
			m_aggregationFieldChecked = false;
			m_fieldStatus = DataFieldStatus.None;
			m_fieldDef = fieldDef;
			m_usedInExpression = false;
		}

		internal FieldImpl(DataFieldStatus status, string exceptionMessage, Microsoft.ReportingServices.ReportProcessing.Field fieldDef)
		{
			m_value = null;
			m_isAggregationField = false;
			m_aggregationFieldChecked = false;
			Global.Tracer.Assert(status != DataFieldStatus.None, "(DataFieldStatus.None != status)");
			m_fieldStatus = status;
			m_exceptionMessage = exceptionMessage;
			m_fieldDef = fieldDef;
			m_usedInExpression = false;
		}

		internal void SetValue(object value)
		{
			m_value = value;
		}

		internal void SetProperty(string propertyName, object propertyValue)
		{
			if (m_properties == null)
			{
				m_properties = new Hashtable();
			}
			m_properties[propertyName] = propertyValue;
		}

		private object GetProperty(string propertyName)
		{
			if (m_properties == null)
			{
				return null;
			}
			return m_properties[propertyName];
		}
	}
}
