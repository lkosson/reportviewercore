namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class ParameterImpl : Parameter
	{
		private object[] m_value;

		private string[] m_labels;

		private bool m_isMultiValue;

		public override object Value
		{
			get
			{
				if (m_value == null)
				{
					return null;
				}
				if (!m_isMultiValue)
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
				if (!m_isMultiValue)
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

		internal ParameterImpl(object[] value, string[] labels, bool isMultiValue)
		{
			m_value = value;
			m_labels = labels;
			m_isMultiValue = isMultiValue;
		}
	}
}
