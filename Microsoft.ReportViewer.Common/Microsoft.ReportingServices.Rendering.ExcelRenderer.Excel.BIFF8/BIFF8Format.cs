namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal struct BIFF8Format
	{
		private int m_ifmt;

		private string m_string;

		internal int Index
		{
			get
			{
				return m_ifmt;
			}
			set
			{
				m_ifmt = value;
			}
		}

		internal string String
		{
			get
			{
				return m_string;
			}
			set
			{
				m_string = value;
			}
		}

		internal BIFF8Format(string builtInFormat, int ifmt)
		{
			m_ifmt = ifmt;
			m_string = builtInFormat;
		}
	}
}
