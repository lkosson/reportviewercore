using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class CaretInfo
	{
		private Point m_position = Point.Empty;

		private bool m_isFirstLine;

		private bool m_isLastLine;

		private int m_ascent;

		private int m_descent;

		private int m_height;

		private int m_lineHeight;

		private int m_lineYOffset;

		internal Point Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal bool IsFirstLine
		{
			get
			{
				return m_isFirstLine;
			}
			set
			{
				m_isFirstLine = value;
			}
		}

		internal bool IsLastLine
		{
			get
			{
				return m_isLastLine;
			}
			set
			{
				m_isLastLine = value;
			}
		}

		internal int Ascent
		{
			get
			{
				return m_ascent;
			}
			set
			{
				m_ascent = value;
			}
		}

		internal int Descent
		{
			get
			{
				return m_descent;
			}
			set
			{
				m_descent = value;
			}
		}

		internal int LineHeight
		{
			get
			{
				return m_lineHeight;
			}
			set
			{
				m_lineHeight = value;
			}
		}

		internal int LineYOffset
		{
			get
			{
				return m_lineYOffset;
			}
			set
			{
				m_lineYOffset = value;
			}
		}

		internal int Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal CaretInfo()
		{
		}
	}
}
