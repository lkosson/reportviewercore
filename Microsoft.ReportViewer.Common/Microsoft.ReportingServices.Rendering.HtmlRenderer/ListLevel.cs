using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ListLevel
	{
		private static byte[] m_closeUL;

		private static byte[] m_closeOL;

		private static byte[] m_olArabic;

		private static byte[] m_olRoman;

		private static byte[] m_olAlpha;

		private static byte[] m_ulCircle;

		private static byte[] m_ulDisc;

		private static byte[] m_ulSquare;

		private static byte[] m_classNoVerticalMargin;

		private static byte[] m_noVerticalMarginClassName;

		private static byte[] m_closeBracket;

		private int m_listLevel;

		private RPLFormat.ListStyles m_style = RPLFormat.ListStyles.Bulleted;

		private IHtmlReportWriter m_renderer;

		public int Level
		{
			get
			{
				return m_listLevel;
			}
			set
			{
				m_listLevel = value;
			}
		}

		public RPLFormat.ListStyles Style
		{
			get
			{
				return m_style;
			}
			set
			{
				m_style = value;
			}
		}

		static ListLevel()
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			m_closeUL = uTF8Encoding.GetBytes("</ul>");
			m_closeOL = uTF8Encoding.GetBytes("</ol>");
			m_olArabic = uTF8Encoding.GetBytes("<ol");
			m_olRoman = uTF8Encoding.GetBytes("<ol type=\"i\"");
			m_olAlpha = uTF8Encoding.GetBytes("<ol type=\"a\"");
			m_ulDisc = uTF8Encoding.GetBytes("<ul type=\"disc\"");
			m_ulSquare = uTF8Encoding.GetBytes("<ul type=\"square\"");
			m_ulCircle = uTF8Encoding.GetBytes("<ul type=\"circle\"");
			m_classNoVerticalMargin = uTF8Encoding.GetBytes(" class=\"r16\"");
			m_noVerticalMarginClassName = uTF8Encoding.GetBytes("r16");
			m_closeBracket = uTF8Encoding.GetBytes(">");
		}

		internal ListLevel(IHtmlReportWriter renderer, int listLevel, RPLFormat.ListStyles style)
		{
			m_renderer = renderer;
			m_listLevel = listLevel;
			m_style = style;
		}

		internal void Open(bool writeNoVerticalMarginClass)
		{
			byte[] bytes = m_olArabic;
			switch (m_style)
			{
			case RPLFormat.ListStyles.Numbered:
				switch (m_listLevel % 3)
				{
				case 2:
					bytes = m_olRoman;
					break;
				case 0:
					bytes = m_olAlpha;
					break;
				}
				break;
			case RPLFormat.ListStyles.Bulleted:
				switch (m_listLevel % 3)
				{
				case 1:
					bytes = m_ulDisc;
					break;
				case 2:
					bytes = m_ulCircle;
					break;
				case 0:
					bytes = m_ulSquare;
					break;
				}
				break;
			}
			m_renderer.WriteStream(bytes);
			if (m_listLevel == 1 && writeNoVerticalMarginClass)
			{
				m_renderer.WriteClassName(m_noVerticalMarginClassName, m_classNoVerticalMargin);
			}
			m_renderer.WriteStream(m_closeBracket);
		}

		internal void Close()
		{
			byte[] bytes = m_closeOL;
			if (m_style == RPLFormat.ListStyles.Bulleted)
			{
				bytes = m_closeUL;
			}
			m_renderer.WriteStream(bytes);
		}
	}
}
