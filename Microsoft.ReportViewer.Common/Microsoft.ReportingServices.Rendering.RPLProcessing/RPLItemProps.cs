namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLItemProps : RPLElementProps
	{
		private string m_label;

		private string m_bookmark;

		private string m_tooltip;

		public string ToolTip
		{
			get
			{
				return m_tooltip;
			}
			set
			{
				m_tooltip = value;
			}
		}

		public string Bookmark
		{
			get
			{
				return m_bookmark;
			}
			set
			{
				m_bookmark = value;
			}
		}

		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal RPLItemProps()
		{
		}
	}
}
