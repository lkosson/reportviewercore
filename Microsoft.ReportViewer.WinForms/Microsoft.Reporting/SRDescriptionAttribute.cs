using System;
using System.ComponentModel;

namespace Microsoft.Reporting
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = false)]
	internal sealed class SRDescriptionAttribute : DescriptionAttribute
	{
		private string m_key;

		private bool m_initialized;

		public override string Description
		{
			get
			{
				if (!m_initialized)
				{
					base.DescriptionValue = CommonStrings.Keys.GetString(m_key);
					m_initialized = true;
				}
				return base.Description;
			}
		}

		public SRDescriptionAttribute(string key)
		{
			m_key = key;
			m_initialized = false;
		}
	}
}
