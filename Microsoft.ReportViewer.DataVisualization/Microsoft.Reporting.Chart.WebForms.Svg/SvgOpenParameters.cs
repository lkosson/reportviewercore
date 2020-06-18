namespace Microsoft.Reporting.Chart.WebForms.Svg
{
	internal struct SvgOpenParameters
	{
		private bool m_toolTipsEnabled;

		private bool m_resizable;

		private bool m_preserveAspectRatio;

		public bool ToolTipsEnabled
		{
			get
			{
				return m_toolTipsEnabled;
			}
			set
			{
				m_toolTipsEnabled = value;
			}
		}

		public bool Resizable
		{
			get
			{
				return m_resizable;
			}
			set
			{
				m_resizable = value;
			}
		}

		public bool PreserveAspectRatio
		{
			get
			{
				return m_preserveAspectRatio;
			}
			set
			{
				m_preserveAspectRatio = value;
			}
		}

		public SvgOpenParameters(bool toolTipsEnabled, bool resizable, bool preserveAspectRatio)
		{
			m_toolTipsEnabled = toolTipsEnabled;
			m_resizable = resizable;
			m_preserveAspectRatio = preserveAspectRatio;
		}

		public override int GetHashCode()
		{
			return m_preserveAspectRatio.GetHashCode() ^ m_resizable.GetHashCode() ^ m_toolTipsEnabled.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SvgOpenParameters))
			{
				return false;
			}
			return Equals((SvgOpenParameters)obj);
		}

		public bool Equals(SvgOpenParameters other)
		{
			if (m_preserveAspectRatio != other.m_preserveAspectRatio)
			{
				return false;
			}
			if (m_resizable != other.m_resizable)
			{
				return false;
			}
			if (m_toolTipsEnabled != other.m_toolTipsEnabled)
			{
				return false;
			}
			return true;
		}

		public static bool operator ==(SvgOpenParameters value1, SvgOpenParameters value2)
		{
			return value2.Equals(value2);
		}

		public static bool operator !=(SvgOpenParameters value1, SvgOpenParameters value2)
		{
			return !value2.Equals(value2);
		}
	}
}
