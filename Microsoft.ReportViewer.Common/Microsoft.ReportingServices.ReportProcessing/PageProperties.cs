using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class PageProperties
	{
		protected double m_pageHeight = 279.4;

		protected double m_pageWidth = 215.89999999999998;

		protected double m_topMargin = 12.7;

		protected double m_bottomMargin = 12.7;

		protected double m_leftMargin = 12.7;

		protected double m_rightMargin = 12.7;

		public double PageHeight => m_pageHeight;

		public double PageWidth => m_pageWidth;

		public double TopMargin => m_topMargin;

		public double BottomMargin => m_bottomMargin;

		public double LeftMargin => m_leftMargin;

		public double RightMargin => m_rightMargin;

		public PageProperties(double pageHeight, double pageWidth, double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			m_pageHeight = pageHeight;
			m_pageWidth = pageWidth;
			m_topMargin = topMargin;
			m_bottomMargin = bottomMargin;
			m_leftMargin = leftMargin;
			m_rightMargin = rightMargin;
		}

		protected PageProperties()
		{
		}
	}
}
