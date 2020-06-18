using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class TextBoxState
	{
		[Flags]
		private enum TextBoxInternalState : byte
		{
			Direction = 0x1,
			DefaultTextAlign = 0x2,
			WritingMode = 0x4,
			VerticalAlign = 0x18,
			VerticalAlignBottom = 0x8,
			VerticalAlignMiddle = 0x10,
			SpanPages = 0x20,
			ResetHorizontalState = 0x40,
			VerticalMode = 0x80
		}

		private TextBoxInternalState m_state;

		public byte State
		{
			get
			{
				return (byte)m_state;
			}
			set
			{
				m_state = (TextBoxInternalState)value;
			}
		}

		public RPLFormat.Directions Direction
		{
			get
			{
				if ((int)(m_state & TextBoxInternalState.Direction) > 0)
				{
					return RPLFormat.Directions.RTL;
				}
				return RPLFormat.Directions.LTR;
			}
			set
			{
				if (value == RPLFormat.Directions.RTL)
				{
					m_state |= TextBoxInternalState.Direction;
				}
				else
				{
					m_state &= ~TextBoxInternalState.Direction;
				}
			}
		}

		public RPLFormat.TextAlignments DefaultTextAlign
		{
			get
			{
				if ((int)(m_state & TextBoxInternalState.DefaultTextAlign) > 0)
				{
					return RPLFormat.TextAlignments.Right;
				}
				return RPLFormat.TextAlignments.Left;
			}
			set
			{
				if (value == RPLFormat.TextAlignments.Right)
				{
					m_state |= TextBoxInternalState.DefaultTextAlign;
				}
				else
				{
					m_state &= ~TextBoxInternalState.DefaultTextAlign;
				}
			}
		}

		public RPLFormat.WritingModes WritingMode
		{
			get
			{
				if ((int)(m_state & TextBoxInternalState.WritingMode) > 0)
				{
					if ((int)(m_state & TextBoxInternalState.VerticalMode) > 0)
					{
						return RPLFormat.WritingModes.Rotate270;
					}
					return RPLFormat.WritingModes.Vertical;
				}
				return RPLFormat.WritingModes.Horizontal;
			}
			set
			{
				if (value == RPLFormat.WritingModes.Vertical || value == RPLFormat.WritingModes.Rotate270)
				{
					m_state |= TextBoxInternalState.WritingMode;
					if (value == RPLFormat.WritingModes.Rotate270)
					{
						m_state |= TextBoxInternalState.VerticalMode;
					}
					else
					{
						m_state &= ~TextBoxInternalState.VerticalMode;
					}
				}
				else
				{
					m_state &= ~TextBoxInternalState.WritingMode;
				}
			}
		}

		public bool VerticalText
		{
			get
			{
				if (WritingMode != RPLFormat.WritingModes.Vertical)
				{
					return WritingMode == RPLFormat.WritingModes.Rotate270;
				}
				return true;
			}
		}

		public bool HorizontalText => WritingMode == RPLFormat.WritingModes.Horizontal;

		public RPLFormat.VerticalAlignments VerticalAlignment
		{
			get
			{
				if ((int)(m_state & TextBoxInternalState.VerticalAlign) > 0)
				{
					if ((int)(m_state & TextBoxInternalState.VerticalAlignBottom) > 0)
					{
						return RPLFormat.VerticalAlignments.Bottom;
					}
					return RPLFormat.VerticalAlignments.Middle;
				}
				return RPLFormat.VerticalAlignments.Top;
			}
			set
			{
				m_state &= ~TextBoxInternalState.VerticalAlign;
				switch (value)
				{
				case RPLFormat.VerticalAlignments.Bottom:
					m_state |= TextBoxInternalState.VerticalAlignBottom;
					break;
				case RPLFormat.VerticalAlignments.Middle:
					m_state |= TextBoxInternalState.VerticalAlignMiddle;
					break;
				}
			}
		}

		public bool SpanPages
		{
			get
			{
				return (int)(m_state & TextBoxInternalState.SpanPages) > 0;
			}
			set
			{
				if (value)
				{
					m_state |= TextBoxInternalState.SpanPages;
				}
				else
				{
					m_state &= ~TextBoxInternalState.SpanPages;
				}
			}
		}

		public bool ResetHorizontalState
		{
			get
			{
				return (int)(m_state & TextBoxInternalState.ResetHorizontalState) > 0;
			}
			set
			{
				if (value)
				{
					m_state |= TextBoxInternalState.ResetHorizontalState;
				}
				else
				{
					m_state &= ~TextBoxInternalState.ResetHorizontalState;
				}
			}
		}
	}
}
