using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.ReportRendering
{
	[Serializable]
	internal abstract class Page
	{
		private PageSectionInstance m_pageHeaderInstance;

		private PageSectionInstance m_pageFooterInstance;

		[NonSerialized]
		private PageSection m_pageHeader;

		[NonSerialized]
		private PageSection m_pageFooter;

		internal PageSection PageSectionHeader
		{
			get
			{
				return m_pageHeader;
			}
			set
			{
				m_pageHeader = value;
			}
		}

		internal PageSection PageSectionFooter
		{
			get
			{
				return m_pageFooter;
			}
			set
			{
				m_pageFooter = value;
			}
		}

		internal PageSectionInstance HeaderInstance => m_pageHeaderInstance;

		internal PageSectionInstance FooterInstance => m_pageFooterInstance;

		public PageSection Header
		{
			get
			{
				return m_pageHeader;
			}
			set
			{
				m_pageHeader = value;
				if (value != null)
				{
					m_pageHeaderInstance = (PageSectionInstance)value.ReportItemInstance;
				}
			}
		}

		public PageSection Footer
		{
			get
			{
				return m_pageFooter;
			}
			set
			{
				m_pageFooter = value;
				if (value != null)
				{
					m_pageFooterInstance = (PageSectionInstance)value.ReportItemInstance;
				}
			}
		}

		protected Page(PageSection pageHeader, PageSection pageFooter)
		{
			Header = pageHeader;
			Footer = pageFooter;
		}
	}
}
