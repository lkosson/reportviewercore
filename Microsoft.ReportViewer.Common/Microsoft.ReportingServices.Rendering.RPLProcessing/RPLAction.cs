namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLAction
	{
		private string m_label;

		private string m_hyperlink;

		private string m_bookmarkLink;

		private string m_drillthroughId;

		private string m_drillthroughUrl;

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

		public string Hyperlink
		{
			get
			{
				return m_hyperlink;
			}
			set
			{
				m_hyperlink = value;
			}
		}

		public string BookmarkLink
		{
			get
			{
				return m_bookmarkLink;
			}
			set
			{
				m_bookmarkLink = value;
			}
		}

		public string DrillthroughId
		{
			get
			{
				return m_drillthroughId;
			}
			set
			{
				m_drillthroughId = value;
			}
		}

		public string DrillthroughUrl
		{
			get
			{
				return m_drillthroughUrl;
			}
			set
			{
				m_drillthroughUrl = value;
			}
		}

		internal RPLAction()
		{
		}

		internal RPLAction(string label)
		{
			m_label = label;
		}
	}
}
