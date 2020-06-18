using Microsoft.ReportingServices.Rendering.Utilities;
using System;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class CellBorderColor
	{
		private byte[] m_borderColorsTop;

		private byte[] m_borderColorsLeft;

		private byte[] m_borderColorsBottom;

		private byte[] m_borderColorsRight;

		internal int SprmSize => (3 + m_borderColorsTop.Length) * 4;

		internal CellBorderColor(int numColumns)
		{
			m_borderColorsTop = new byte[4 * numColumns];
			m_borderColorsLeft = new byte[4 * numColumns];
			m_borderColorsBottom = new byte[4 * numColumns];
			m_borderColorsRight = new byte[4 * numColumns];
			InitColors(m_borderColorsTop);
			InitColors(m_borderColorsLeft);
			InitColors(m_borderColorsBottom);
			InitColors(m_borderColorsRight);
		}

		private void InitColors(byte[] borderColors)
		{
			for (int i = 3; i < borderColors.Length; i += 4)
			{
				borderColors[i] = byte.MaxValue;
			}
		}

		internal void Reset()
		{
			Array.Clear(m_borderColorsTop, 0, m_borderColorsTop.Length);
			Array.Clear(m_borderColorsLeft, 0, m_borderColorsLeft.Length);
			Array.Clear(m_borderColorsBottom, 0, m_borderColorsBottom.Length);
			Array.Clear(m_borderColorsRight, 0, m_borderColorsRight.Length);
			InitColors(m_borderColorsTop);
			InitColors(m_borderColorsLeft);
			InitColors(m_borderColorsBottom);
			InitColors(m_borderColorsRight);
		}

		internal void SetColor(TableData.Positions position, int cellIndex, int ico24)
		{
			int offset = cellIndex * 4;
			switch (position)
			{
			case TableData.Positions.Bottom:
				LittleEndian.PutInt(m_borderColorsBottom, offset, ico24);
				break;
			case TableData.Positions.Left:
				LittleEndian.PutInt(m_borderColorsLeft, offset, ico24);
				break;
			case TableData.Positions.Right:
				LittleEndian.PutInt(m_borderColorsRight, offset, ico24);
				break;
			case TableData.Positions.Top:
				LittleEndian.PutInt(m_borderColorsTop, offset, ico24);
				break;
			}
		}

		internal byte[] ToByteArray()
		{
			int num = 0;
			byte[] array = new byte[SprmSize];
			num += Word97Writer.AddSprm(array, num, 54810, 0, m_borderColorsTop);
			num += Word97Writer.AddSprm(array, num, 54811, 0, m_borderColorsLeft);
			num += Word97Writer.AddSprm(array, num, 54812, 0, m_borderColorsBottom);
			num += Word97Writer.AddSprm(array, num, 54813, 0, m_borderColorsRight);
			return array;
		}
	}
}
