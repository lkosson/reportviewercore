namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLItemPropsDef : RPLElementPropsDef
	{
		private string m_name;

		private string m_tooltip;

		private string m_bookmark;

		private string m_label;

		private string m_toggleItem;

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

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

		public string ToggleItem
		{
			get
			{
				return m_toggleItem;
			}
			set
			{
				m_toggleItem = value;
			}
		}

		internal RPLItemPropsDef()
		{
		}
	}
}
