using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ParameterImpl : Parameter
	{
		private object[] m_value;

		private string[] m_labels;

		private bool m_isMultiValue;

		private string m_prompt;

		private int m_hash;

		private bool m_isUserSupplied;

		private bool m_isDataSetQueryParameter;

		public override object Value
		{
			get
			{
				if (m_value == null)
				{
					return null;
				}
				if (!m_isMultiValue || (m_isDataSetQueryParameter && m_value.Length == 1))
				{
					return m_value[0];
				}
				return m_value;
			}
		}

		public override object Label
		{
			get
			{
				if (m_labels == null || m_labels.Length == 0)
				{
					return null;
				}
				if (!m_isMultiValue || (m_isDataSetQueryParameter && m_labels.Length == 1))
				{
					return m_labels[0];
				}
				return m_labels;
			}
		}

		public override int Count
		{
			get
			{
				if (m_value == null)
				{
					return 0;
				}
				return m_value.Length;
			}
		}

		public override bool IsMultiValue => m_isMultiValue;

		internal bool IsUserSupplied => m_isUserSupplied;

		internal string Prompt => m_prompt;

		internal ParameterImpl()
		{
		}

		internal ParameterImpl(ParameterInfo parameterInfo)
		{
			m_value = parameterInfo.Values;
			m_labels = parameterInfo.Labels;
			m_isMultiValue = parameterInfo.MultiValue;
			m_prompt = parameterInfo.Prompt;
			m_isUserSupplied = parameterInfo.IsUserSupplied;
			if (parameterInfo.ParameterObjectType == ObjectType.QueryParameter)
			{
				m_isDataSetQueryParameter = true;
			}
		}

		internal void SetIsMultiValue(bool isMultiValue)
		{
			m_isMultiValue = isMultiValue;
		}

		internal void SetIsUserSupplied(bool isUserSupplied)
		{
			m_isUserSupplied = isUserSupplied;
		}

		internal void SetValues(object[] values)
		{
			m_value = values;
		}

		internal object[] GetValues()
		{
			return m_value;
		}

		internal void SetLabels(string[] labels)
		{
			m_labels = labels;
		}

		internal string[] GetLabels()
		{
			return m_labels;
		}

		internal void SetPrompt(string prompt)
		{
			m_prompt = prompt;
		}

		internal bool ValuesAreEqual(ParameterImpl obj)
		{
			if (!m_isUserSupplied)
			{
				return true;
			}
			int count = Count;
			if (obj == null || count != obj.Count)
			{
				return false;
			}
			object[] values = obj.GetValues();
			for (int i = 0; i < count; i++)
			{
				if (!object.Equals(m_value[i], values[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal int GetValuesHashCode()
		{
			if (!m_isUserSupplied)
			{
				return 0;
			}
			if (m_hash == 0)
			{
				int count = Count;
				m_hash = (0x1A03 | (count + 1 << 16));
				for (int i = 0; i < count; i++)
				{
					if (m_value[i] != null)
					{
						m_hash ^= m_value[i].GetHashCode();
					}
				}
			}
			return m_hash;
		}
	}
}
