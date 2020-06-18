namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLPageLayout
	{
		private string m_pageName;

		private float m_pageHeight;

		private float m_pageWidth;

		private float m_marginTop;

		private float m_marginBottom;

		private float m_marginLeft;

		private float m_marginRight;

		private RPLElementStyle m_pageStyle;

		public string PageName
		{
			get
			{
				return m_pageName;
			}
			set
			{
				m_pageName = value;
			}
		}

		public float PageWidth
		{
			get
			{
				return m_pageWidth;
			}
			set
			{
				m_pageWidth = value;
			}
		}

		public float PageHeight
		{
			get
			{
				return m_pageHeight;
			}
			set
			{
				m_pageHeight = value;
			}
		}

		public float MarginTop
		{
			get
			{
				return m_marginTop;
			}
			set
			{
				m_marginTop = value;
			}
		}

		public float MarginBottom
		{
			get
			{
				return m_marginBottom;
			}
			set
			{
				m_marginBottom = value;
			}
		}

		public float MarginLeft
		{
			get
			{
				return m_marginLeft;
			}
			set
			{
				m_marginLeft = value;
			}
		}

		public float MarginRight
		{
			get
			{
				return m_marginRight;
			}
			set
			{
				m_marginRight = value;
			}
		}

		public RPLElementStyle Style
		{
			get
			{
				return m_pageStyle;
			}
			set
			{
				m_pageStyle = value;
			}
		}

		internal RPLPageLayout()
		{
		}
	}
}
