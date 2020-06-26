using System;
using System.ComponentModel;

namespace Microsoft.Reporting
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false)]
	internal sealed class SRCategoryAttribute : CategoryAttribute
	{
		private string m_value;

		private string m_key;

		public SRCategoryAttribute(string key)
		{
			m_key = key;
		}

		protected override string GetLocalizedString(string value)
		{
			if (m_value == null)
			{
				m_value = CommonStrings.Keys.GetString(m_key);
			}
			return m_value;
		}
	}
}
