using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class TablixFixedHeaderStorage
	{
		private string m_bodyId;

		private string m_htmlId;

		private string m_lastRowGroupCol = "";

		private int m_firstRowGroupColIndex = 1;

		private List<string> m_rowHeaders;

		private List<string> m_columnHeaders;

		private List<string> m_cornerHeaders;

		internal string BodyID
		{
			get
			{
				return m_bodyId;
			}
			set
			{
				m_bodyId = value;
			}
		}

		internal string HtmlId
		{
			get
			{
				return m_htmlId;
			}
			set
			{
				m_htmlId = value;
			}
		}

		public List<string> RowHeaders
		{
			get
			{
				return m_rowHeaders;
			}
			set
			{
				m_rowHeaders = value;
			}
		}

		public bool HasEmptyCol
		{
			get
			{
				return m_firstRowGroupColIndex == 2;
			}
			set
			{
				if (value)
				{
					m_firstRowGroupColIndex = 2;
				}
				else
				{
					m_firstRowGroupColIndex = 1;
				}
			}
		}

		public string FirstRowGroupCol
		{
			get
			{
				if (m_rowHeaders == null)
				{
					return "";
				}
				return m_rowHeaders[m_firstRowGroupColIndex];
			}
		}

		public string LastRowGroupCol
		{
			get
			{
				return m_lastRowGroupCol;
			}
			set
			{
				m_lastRowGroupCol = value;
			}
		}

		public string LastColGroupRow
		{
			get
			{
				if (m_columnHeaders == null)
				{
					return "";
				}
				return m_columnHeaders[m_columnHeaders.Count - 1];
			}
		}

		public List<string> ColumnHeaders
		{
			get
			{
				return m_columnHeaders;
			}
			set
			{
				m_columnHeaders = value;
			}
		}

		public List<string> CornerHeaders
		{
			get
			{
				return m_cornerHeaders;
			}
			set
			{
				m_cornerHeaders = value;
			}
		}
	}
}
