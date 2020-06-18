namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class HyperlinkInfo : AreaInfo
	{
		private string m_url;

		private string m_label;

		internal string URL
		{
			get
			{
				return m_url;
			}
			set
			{
				m_url = value;
			}
		}

		internal string Label => m_label;

		internal virtual bool IsBookmark => false;

		internal HyperlinkInfo(string url, string label, int firstRow, int lastRow, int firstCol, int lastCol)
			: base(firstRow, lastRow, firstCol, lastCol)
		{
			m_url = url;
			if (label == null)
			{
				label = string.Empty;
			}
			m_label = label;
		}

		public override string ToString()
		{
			return "URL: " + URL + ", Bookmark: " + IsBookmark;
		}
	}
}
