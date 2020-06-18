using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class CellPadding
	{
		private CellSpacingStruct m_topPadding;

		private CellSpacingStruct m_leftPadding;

		private CellSpacingStruct m_bottomPadding;

		private CellSpacingStruct m_rightPadding;

		private List<byte[]> m_sprms;

		private int m_runningSize;

		private int m_numColumns;

		private int m_adjustmentTop;

		private int m_adjustmentBottom;

		internal int HeightAdjustment => m_adjustmentBottom + m_adjustmentTop;

		internal int SprmSize => m_runningSize;

		internal CellPadding(int numColumns)
		{
			m_sprms = new List<byte[]>();
			m_runningSize = 0;
			m_adjustmentTop = 0;
			m_adjustmentBottom = 0;
			m_numColumns = numColumns;
		}

		internal void SetPaddingTop(int cellIndex, int twips)
		{
			if (m_topPadding == null)
			{
				m_topPadding = new CellSpacingStruct(CellSpacingStruct.Location.Top);
				m_topPadding.ItcFirst = cellIndex;
				m_topPadding.Width = twips;
				m_adjustmentTop = Math.Max(twips, m_adjustmentTop);
			}
			else if (m_topPadding.Width != twips)
			{
				m_topPadding.ItcLim = cellIndex;
				Commit(m_topPadding);
				m_topPadding.ItcFirst = cellIndex;
				m_topPadding.Width = twips;
				m_adjustmentTop = Math.Max(twips, m_adjustmentTop);
			}
		}

		internal void SetPaddingLeft(int cellIndex, int twips)
		{
			if (m_leftPadding == null)
			{
				m_leftPadding = new CellSpacingStruct(CellSpacingStruct.Location.Left);
				m_leftPadding.ItcFirst = cellIndex;
				m_leftPadding.Width = twips;
			}
			else if (m_leftPadding.Width != twips)
			{
				m_leftPadding.ItcLim = cellIndex;
				Commit(m_leftPadding);
				m_leftPadding.ItcFirst = cellIndex;
				m_leftPadding.Width = twips;
			}
		}

		internal void SetPaddingBottom(int cellIndex, int twips)
		{
			if (m_bottomPadding == null)
			{
				m_bottomPadding = new CellSpacingStruct(CellSpacingStruct.Location.Bottom);
				m_bottomPadding.ItcFirst = cellIndex;
				m_bottomPadding.Width = twips;
				m_adjustmentBottom = Math.Max(twips, m_adjustmentBottom);
			}
			else if (m_bottomPadding.Width != twips)
			{
				m_bottomPadding.ItcLim = cellIndex;
				Commit(m_bottomPadding);
				m_bottomPadding.ItcFirst = cellIndex;
				m_bottomPadding.Width = twips;
				m_adjustmentBottom = Math.Max(twips, m_adjustmentBottom);
			}
		}

		internal void SetPaddingRight(int cellIndex, int twips)
		{
			if (m_rightPadding == null)
			{
				m_rightPadding = new CellSpacingStruct(CellSpacingStruct.Location.Right);
				m_rightPadding.ItcFirst = cellIndex;
				m_rightPadding.Width = twips;
			}
			else if (m_rightPadding.Width != twips)
			{
				m_rightPadding.ItcLim = cellIndex;
				Commit(m_rightPadding);
				m_rightPadding.ItcFirst = cellIndex;
				m_rightPadding.Width = twips;
			}
		}

		internal byte[] ToByteArray()
		{
			byte[] array = new byte[m_runningSize];
			int num = 0;
			for (int i = 0; i < m_sprms.Count; i++)
			{
				byte[] array2 = m_sprms[i];
				Array.Copy(array2, 0, array, num, array2.Length);
				num += array2.Length;
			}
			return array;
		}

		private void Commit(CellSpacingStruct spacing)
		{
			byte[] array = spacing.ToByteArray();
			m_sprms.Add(array);
			m_runningSize += array.Length;
		}

		internal void Finish()
		{
			if (m_topPadding != null)
			{
				m_topPadding.ItcLim = m_numColumns;
				Commit(m_topPadding);
			}
			if (m_leftPadding != null)
			{
				m_leftPadding.ItcLim = m_numColumns;
				Commit(m_leftPadding);
			}
			if (m_bottomPadding != null)
			{
				m_bottomPadding.ItcLim = m_numColumns;
				Commit(m_bottomPadding);
			}
			if (m_rightPadding != null)
			{
				m_rightPadding.ItcLim = m_numColumns;
				Commit(m_rightPadding);
			}
		}

		internal void Reset()
		{
			m_rightPadding = null;
			m_leftPadding = null;
			m_topPadding = null;
			m_bottomPadding = null;
			m_sprms = new List<byte[]>();
			m_runningSize = 0;
			m_adjustmentTop = 0;
			m_adjustmentBottom = 0;
		}
	}
}
