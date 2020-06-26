namespace Microsoft.Reporting.NETCore
{
	internal sealed class ComboBoxItem
	{
		private ValidValue m_validValue;

		private readonly string m_displayString;

		public string Value => m_validValue.Value;

		public ComboBoxItem(ValidValue validValue, string locNullText)
		{
			m_validValue = validValue;
			m_displayString = m_validValue.Label;
			if (m_displayString == null)
			{
				m_displayString = Value;
			}
			if (m_displayString == null)
			{
				m_displayString = "(" + locNullText + ")";
			}
		}

		public override string ToString()
		{
			return m_displayString;
		}
	}
}
