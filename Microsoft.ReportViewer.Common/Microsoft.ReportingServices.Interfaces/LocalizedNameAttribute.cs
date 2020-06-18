using System;

namespace Microsoft.ReportingServices.Interfaces
{
	[AttributeUsage(AttributeTargets.All)]
	public class LocalizedNameAttribute : Attribute
	{
		private string m_name;

		private bool m_localized;

		public string Name
		{
			get
			{
				if (!m_localized)
				{
					m_localized = true;
					string localizedString = GetLocalizedString(m_name);
					if (localizedString != null)
					{
						m_name = localizedString;
					}
				}
				return m_name;
			}
		}

		public LocalizedNameAttribute()
		{
		}

		public LocalizedNameAttribute(string name)
		{
			m_name = name;
			m_localized = false;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is LocalizedNameAttribute)
			{
				return Name.Equals(((LocalizedNameAttribute)obj).Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		protected virtual string GetLocalizedString(string value)
		{
			return value;
		}
	}
}
