using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class RTSelectionHighlight
	{
		private TextBoxContext m_selectionStart;

		private TextBoxContext m_selectionEnd;

		private bool m_allowColorInversion = true;

		private Color m_color = SystemColors.Highlight;

		internal TextBoxContext SelectionStart
		{
			get
			{
				return m_selectionStart;
			}
			set
			{
				m_selectionStart = value;
			}
		}

		internal TextBoxContext SelectionEnd
		{
			get
			{
				return m_selectionEnd;
			}
			set
			{
				m_selectionEnd = value;
			}
		}

		internal Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		internal bool AllowColorInversion
		{
			get
			{
				return m_allowColorInversion;
			}
			set
			{
				m_allowColorInversion = value;
			}
		}

		internal RTSelectionHighlight(TextBoxContext Start, TextBoxContext End, Color Color)
		{
			m_selectionStart = Start;
			m_selectionEnd = End;
			m_color = Color;
		}

		internal RTSelectionHighlight(TextBoxContext Start, TextBoxContext End)
		{
			m_selectionStart = Start;
			m_selectionEnd = End;
		}

		public override string ToString()
		{
			return "Start: " + m_selectionStart.ToString() + "\r\nEnd:  " + m_selectionEnd.ToString();
		}
	}
}
