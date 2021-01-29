using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	[Serializable]
	internal sealed class PageTableLayout
	{
		private const float LINETHRESHOLD = 0.01f;

		private int m_nrCols;

		private int m_nrRows;

		private PageTableCellList m_tableGrid;

		private bool m_bandTable;

		[NonSerialized]
		private int m_firstVisibleRow;

		[NonSerialized]
		private int m_firstVisibleColumn;

		[NonSerialized]
		private bool m_needExtraRow;

		public const double RoundDelta = 0.0001;

		internal bool BandTable
		{
			get
			{
				return m_bandTable;
			}
			set
			{
				m_bandTable = value;
			}
		}

		internal int NrCols
		{
			get
			{
				return m_nrCols;
			}
			set
			{
				m_nrCols = value;
			}
		}

		internal int NrRows
		{
			get
			{
				return m_nrRows;
			}
			set
			{
				m_nrRows = value;
			}
		}

		internal PageTableLayout(int nrCols, int nrRows)
		{
			m_nrCols = nrCols;
			m_nrRows = nrRows;
			m_tableGrid = new PageTableCellList();
		}

		internal void AddCell(float x, float y, float dx, float dy)
		{
			m_tableGrid.Add(new PageTableCell(x, y, dx, dy));
		}

		internal PageTableCell GetCell(int index)
		{
			if (index < 0 || index > m_nrCols * m_nrRows - 1)
			{
				return null;
			}
			return m_tableGrid[index];
		}

		internal PageTableCell GetCell(int row, int col)
		{
			if (col >= m_nrCols)
			{
				return null;
			}
			return GetCell(row * m_nrCols + col);
		}

		public static void GenerateTableLayout(RPLItemMeasurement[] repItemCollection, float ownerWidth, float ownerHeight, float delta, out PageTableLayout tableLayout)
		{
			GenerateTableLayout(repItemCollection, ownerWidth, ownerHeight, delta, out tableLayout, expandLayout: false, consumeContainerWhiteSpace: false);
		}

		public static void GenerateTableLayout(RPLItemMeasurement[] repItemCollection, float ownerWidth, float ownerHeight, float delta, out PageTableLayout tableLayout, bool expandLayout, bool consumeContainerWhiteSpace)
		{
			List<float> list = new List<float>();
			List<float> list2 = new List<float>();
			int num = 0;
			int num2 = 0;
			tableLayout = null;
			if (repItemCollection == null)
			{
				return;
			}
			FillXArray(repItemCollection, ownerWidth, list);
			FillYArray(repItemCollection, ownerHeight, list2, delta);
			num = list.Count - 1;
			num2 = list2.Count - 1;
			if (num <= 0 || num2 <= 0)
			{
				return;
			}
			tableLayout = new PageTableLayout(num, num2);
			for (int i = 0; i <= num2 - 1; i++)
			{
				for (int j = 0; j <= num - 1; j++)
				{
					tableLayout.AddCell(list[j], list2[i], list[j + 1] - list[j], list2[i + 1] - list2[i]);
				}
			}
			tableLayout.AttachReportItems(repItemCollection, delta, consumeContainerWhiteSpace);
			num = tableLayout.NrCols;
			num2 = tableLayout.NrRows;
			if (num <= 0 || num2 <= 0)
			{
				tableLayout = null;
			}
			else if (num == 1 && num2 == repItemCollection.Length)
			{
				int k;
				for (k = 0; k < num2 && tableLayout.GetCell(k).InUse; k++)
				{
				}
				tableLayout.BandTable = ((k >= num2) ? true : false);
			}
		}

		private void RemoveLastRow()
		{
			m_tableGrid.RemoveRange((m_nrRows - 1) * m_nrCols, m_nrCols);
			m_nrRows--;
		}

		private void RemoveLastCol()
		{
			for (int num = m_nrRows; num > 0; num--)
			{
				int num2 = num * m_nrCols - 1;
				if (num < m_nrRows)
				{
					for (int i = 1; i < m_nrCols; i++)
					{
						PageTableCell cell = GetCell(num2 + i);
						if (cell != null && (cell.Eaten || cell.InUse))
						{
							cell.UsedCell -= cell.UsedCell / m_nrCols;
						}
					}
				}
				m_tableGrid.RemoveAt(num2);
			}
			m_nrCols--;
		}

		internal bool NeedExtraRow()
		{
			int num = m_firstVisibleRow * m_nrCols;
			PageTableCell pageTableCell = null;
			for (int i = m_firstVisibleColumn; i < m_nrCols; i += pageTableCell.ColSpan)
			{
				pageTableCell = m_tableGrid[i + num];
				if (pageTableCell.ColSpan != 1 && (pageTableCell.InUse || pageTableCell.RowSpan > 1 || pageTableCell.HasBorder))
				{
					m_needExtraRow = true;
					break;
				}
			}
			if (m_needExtraRow)
			{
				for (int j = 0; j < m_nrCols; j++)
				{
					if (!m_tableGrid[j].Eaten && !m_tableGrid[j].InUse && m_tableGrid[j].BorderLeft == null)
					{
						pageTableCell = m_tableGrid[j];
						for (; j + 1 < m_nrCols && !m_tableGrid[j + 1].Eaten && !m_tableGrid[j + 1].InUse && m_tableGrid[j + 1].BorderTop == m_tableGrid[j + 1].BorderTop && m_tableGrid[j + 1].BorderBottom == m_tableGrid[j + 1].BorderBottom && m_tableGrid[j + 1].BorderLeft == null; j++)
						{
							pageTableCell.ColSpan++;
							m_tableGrid[j + 1].Eaten = true;
						}
					}
				}
			}
			return m_needExtraRow;
		}

		internal static bool SkipReportItem(RPLItemMeasurement measurement)
		{
			if (measurement.Height == 0f)
			{
				if (measurement.Width == 0f)
				{
					return true;
				}
				if (!(measurement.Element is RPLLine))
				{
					return true;
				}
			}
			else if (measurement.Width == 0f && !(measurement.Element is RPLLine))
			{
				return true;
			}
			return false;
		}

		private static void FillXArray(RPLItemMeasurement[] reportItemCol, float parentWidth, List<float> leftPosition)
		{
			int num = 0;
			int num2 = 0;
			RoundedFloat roundedFloat = new RoundedFloat(0f);
			RPLItemMeasurement rPLItemMeasurement = null;
			int num3 = 0;
			if (reportItemCol == null || leftPosition == null)
			{
				return;
			}
			leftPosition.Add(roundedFloat.Value);
			while (num3 < reportItemCol.Length)
			{
				rPLItemMeasurement = reportItemCol[num3];
				if (SkipReportItem(rPLItemMeasurement))
				{
					num3++;
					continue;
				}
				RoundedFloat x = new RoundedFloat(rPLItemMeasurement.Left);
				while (num > 0 && x < leftPosition[num])
				{
					num--;
				}
				if (num < 0 || x != leftPosition[num])
				{
					leftPosition.Insert(num + 1, rPLItemMeasurement.Left);
					num2++;
				}
				num = num2;
				roundedFloat.Value = ReportItemRightValue(rPLItemMeasurement);
				while (num > 0 && roundedFloat < leftPosition[num])
				{
					num--;
				}
				if (num < 0 || roundedFloat != leftPosition[num])
				{
					leftPosition.Insert(num + 1, roundedFloat.Value);
					num2++;
				}
				num = num2;
				num3++;
			}
			RoundedFloat x2 = new RoundedFloat(parentWidth);
			if (x2 > leftPosition[num] || 1 == leftPosition.Count)
			{
				leftPosition.Insert(num + 1, parentWidth);
			}
		}

		private static void FillYArray(RPLItemMeasurement[] repItemColl, float parentHeight, List<float> topPosition, float delta)
		{
			int num = 0;
			int num2 = 0;
			RoundedFloat roundedFloat = new RoundedFloat(0f);
			int num3 = 0;
			RPLItemMeasurement rPLItemMeasurement = null;
			if (repItemColl == null || topPosition == null)
			{
				return;
			}
			topPosition.Add(roundedFloat.Value);
			while (num3 < repItemColl.Length)
			{
				rPLItemMeasurement = repItemColl[num3];
				if (SkipReportItem(rPLItemMeasurement))
				{
					num3++;
					continue;
				}
				RoundedFloat roundedFloat2 = new RoundedFloat(rPLItemMeasurement.Top - delta);
				while (num > 0 && roundedFloat2 < topPosition[num])
				{
					num--;
				}
				if (num < 0 || roundedFloat2 != topPosition[num])
				{
					topPosition.Insert(num + 1, roundedFloat2.Value - delta);
					num2++;
				}
				num = num2;
				roundedFloat.Value = ReportItemBottomValue(rPLItemMeasurement) - delta;
				while (num > 0 && roundedFloat < topPosition[num])
				{
					num--;
				}
				if (num < 0 || roundedFloat != topPosition[num])
				{
					topPosition.Insert(num + 1, roundedFloat.Value);
					num2++;
				}
				num = num2;
				num3++;
			}
			RoundedFloat x = new RoundedFloat(parentHeight - delta);
			if (x > topPosition[num] || 1 == topPosition.Count)
			{
				topPosition.Insert(num + 1, parentHeight - delta);
			}
		}

		private static float ReportItemRightValue(RPLMeasurement currReportItem)
		{
			return currReportItem.Left + currReportItem.Width;
		}

		private static float ReportItemBottomValue(RPLMeasurement currReportItem)
		{
			return currReportItem.Top + currReportItem.Height;
		}

		internal bool AreSpansInColOne()
		{
			bool flag = false;
			for (int i = 0; i < m_nrRows; i++)
			{
				int index = i * m_nrCols;
				if (!m_tableGrid[index].Eaten && !m_tableGrid[index].InUse)
				{
					if (flag)
					{
						return true;
					}
					flag = true;
				}
				else
				{
					if (m_tableGrid[index].InUse && m_tableGrid[index].RowSpan > 1)
					{
						return true;
					}
					flag = false;
				}
			}
			return false;
		}

		private void AttachVerticalBorder(int xCellFound, int yCellFound, RPLMeasurement measurement, RPLLine currReportItem, bool leftBorder)
		{
			int num = yCellFound;
			PageTableCell pageTableCell = m_tableGrid[xCellFound + num * m_nrCols];
			double num2 = pageTableCell.DYValue.Value;
			if (leftBorder)
			{
				pageTableCell.BorderLeft = currReportItem;
			}
			else
			{
				pageTableCell.BorderRight = currReportItem;
			}
			while ((double)measurement.Height - num2 > 0.0001)
			{
				num++;
				if (num < m_nrRows)
				{
					pageTableCell = m_tableGrid[xCellFound + num * m_nrCols];
					num2 += (double)pageTableCell.DYValue.Value;
					if (leftBorder)
					{
						pageTableCell.BorderLeft = currReportItem;
					}
					else
					{
						pageTableCell.BorderRight = currReportItem;
					}
					continue;
				}
				break;
			}
		}

		private void AttachHorizontalBorder(int xCellFound, int yCellFound, RPLMeasurement measurement, RPLLine currReportItem, bool topBorder)
		{
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			PageTableCell pageTableCell = null;
			int num4 = 1;
			num2 = xCellFound;
			num3 = yCellFound * m_nrCols;
			pageTableCell = m_tableGrid[num2 + num3];
			num = pageTableCell.DXValue.Value;
			if (topBorder)
			{
				pageTableCell.BorderTop = currReportItem;
			}
			else
			{
				pageTableCell.BorderBottom = currReportItem;
			}
			while ((double)measurement.Width - num > 0.0001)
			{
				num2++;
				if (num2 < m_nrCols)
				{
					pageTableCell = m_tableGrid[num2 + num3];
					num += (double)pageTableCell.DXValue.Value;
					num4++;
					if (topBorder)
					{
						pageTableCell.BorderTop = currReportItem;
					}
					else
					{
						pageTableCell.BorderBottom = currReportItem;
					}
					continue;
				}
				break;
			}
		}

		private bool FindRItemCell(RPLMeasurement currReportItem, ref int xCellFound, ref int yCellFound, double delta)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			while (!flag && num < m_nrRows)
			{
				if (m_tableGrid[m_nrCols * num].YValue == (float)((double)currReportItem.Top - delta))
				{
					for (num2 = 0; num2 <= m_nrCols - 1; num2++)
					{
						if (m_tableGrid[num2 + m_nrCols * num].XValue == currReportItem.Left)
						{
							flag = true;
							xCellFound = num2;
							yCellFound = num;
							break;
						}
					}
				}
				num++;
			}
			return flag;
		}

		private void AttachRItem(int xCellFound, int yCellFound, int colSpan, int rowSpan, RPLItemMeasurement measurement)
		{
			int num = 0;
			int num2 = 0;
			int index = xCellFound + yCellFound * m_nrCols;
			for (num = xCellFound; num <= xCellFound + colSpan - 1; num++)
			{
				for (num2 = yCellFound; num2 <= yCellFound + rowSpan - 1; num2++)
				{
					if (num != xCellFound || num2 != yCellFound)
					{
						m_tableGrid[num + m_nrCols * num2].MarkCellEaten(index);
					}
				}
			}
			m_tableGrid[index].MarkCellUsed(measurement, colSpan, rowSpan, index);
		}

		private void ComputeColRowSpan(RPLMeasurement reportItem, int xCellFound, int yCellFound, ref int colSpans, ref int rowSpans)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			int num4 = 0;
			int num5 = yCellFound * m_nrCols;
			int index = xCellFound + num5;
			num = m_tableGrid[index].DXValue.Value;
			num3 = xCellFound;
			colSpans = 1;
			for (; (double)(reportItem.Width - num) > 0.0001; num += m_tableGrid[num3 + num5].DXValue.Value)
			{
				num3++;
				if (num3 >= m_nrCols)
				{
					break;
				}
				colSpans++;
			}
			num2 = m_tableGrid[index].DYValue.Value;
			num4 = yCellFound;
			rowSpans = 1;
			for (; (double)(reportItem.Height - num2) > 0.0001; num2 += m_tableGrid[xCellFound + m_nrCols * num4].DYValue.Value)
			{
				num4++;
				if (num4 >= m_nrRows)
				{
					break;
				}
				rowSpans++;
			}
			for (int i = yCellFound; i < yCellFound + rowSpans; i++)
			{
				for (int j = xCellFound; j < xCellFound + colSpans; j++)
				{
					PageTableCell cell = GetCell(i, j);
					cell.FirstHorzMerge = (j == xCellFound && colSpans > 1);
					cell.FirstVertMerge = (i == yCellFound && rowSpans > 1);
					cell.HorzMerge = (j > xCellFound);
					cell.VertMerge = (i > yCellFound);
				}
			}
		}

		private void FillAndFindOverlap(RPLItemMeasurement[] repItemCollection, double delta)
		{
			bool flag = false;
			int num = -1;
			int num2 = -1;
			int colSpans = 0;
			int rowSpans = 0;
			int num3 = 0;
			PageTableCell pageTableCell = null;
			int num4 = 0;
			RPLItemMeasurement rPLItemMeasurement = null;
			RPLElement rPLElement = null;
			while (num4 < repItemCollection.Length)
			{
				num = -1;
				num2 = -1;
				flag = false;
				rPLItemMeasurement = repItemCollection[num4];
				if (SkipReportItem(rPLItemMeasurement))
				{
					num4++;
					continue;
				}
				rPLElement = rPLItemMeasurement.Element;
				flag = FindRItemCell(rPLItemMeasurement, ref num, ref num2, delta);
				if (!flag && !(rPLElement is RPLLine))
				{
					num4++;
					continue;
				}
				RPLLine rPLLine = rPLElement as RPLLine;
				if (rPLLine != null)
				{
					RPLLinePropsDef rPLLinePropsDef = rPLLine.ElementPropsDef as RPLLinePropsDef;
					float width = rPLItemMeasurement.Width;
					if ((width >= 0f && width < 0.01f) || (width < 0f && width > -0.01f))
					{
						if (!flag)
						{
							int num5 = 0;
							bool flag2 = true;
							while (flag2 && num5 < m_nrRows)
							{
								if (m_tableGrid[num5 * m_nrCols].YValue == (float)((double)rPLItemMeasurement.Top - delta))
								{
									num2 = num5;
									flag2 = false;
								}
								num5++;
							}
							num = m_nrCols - 1;
							if (!flag2)
							{
								AttachVerticalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, leftBorder: false);
							}
						}
						else
						{
							AttachVerticalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, leftBorder: true);
						}
						num4++;
						continue;
					}
					width = rPLItemMeasurement.Height;
					if ((width >= 0f && width < 0.01f) || (width < 0f && width > -0.01f))
					{
						if (!flag)
						{
							int num6 = 0;
							bool flag3 = true;
							while (flag3 && num6 < m_nrCols)
							{
								if (m_tableGrid[num6].XValue == rPLItemMeasurement.Left)
								{
									num = num6;
									flag3 = false;
								}
								num6++;
							}
							num2 = m_nrRows - 1;
							AttachHorizontalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, topBorder: false);
						}
						else
						{
							AttachHorizontalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, topBorder: true);
						}
						num4++;
						continue;
					}
				}
				num3 = num + m_nrCols * num2;
				pageTableCell = m_tableGrid[num3];
				if ((pageTableCell.InUse || pageTableCell.Eaten) && rPLElement is RPLLine && rPLItemMeasurement.Width != 0f && rPLItemMeasurement.Height != 0f)
				{
					num4++;
					continue;
				}
				ComputeColRowSpan(rPLItemMeasurement, num, num2, ref colSpans, ref rowSpans);
				AttachRItem(num, num2, colSpans, rowSpans, rPLItemMeasurement);
				num4++;
			}
		}

		internal bool GetBool(object b)
		{
			if (b != null)
			{
				return (bool)b;
			}
			return false;
		}

		internal void EmptyRowsCells()
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			int num3 = 0;
			PageTableCell pageTableCell = null;
			int num4 = 0;
			int num5 = 0;
			List<int> list = new List<int>();
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (num = 0; num < m_nrRows; num++)
			{
				flag = false;
				int num9 = m_nrCols * num4;
				if (m_tableGrid[num3].DYValue < 0.2f)
				{
					for (num2 = 0; num2 < m_nrCols; num2++)
					{
						if (flag)
						{
							break;
						}
						pageTableCell = m_tableGrid[num3 + num2];
						flag = (pageTableCell.InUse || pageTableCell.BorderBottom != null || pageTableCell.BorderTop != null);
					}
					if (!flag)
					{
						num4++;
						num5 = num3;
						num2 = 0;
						while (num2 < m_nrCols)
						{
							if (m_tableGrid[num3 + num2].Eaten)
							{
								int usedCell = m_tableGrid[num3 + num2].UsedCell;
								m_tableGrid[usedCell].RowSpan--;
								num2 += m_tableGrid[usedCell].ColSpan;
							}
							else
							{
								num2++;
							}
						}
						m_tableGrid.RemoveRange(num3, m_nrCols);
						m_nrRows--;
						num--;
						num3 -= m_nrCols;
					}
				}
				else
				{
					flag = true;
				}
				if (flag && num4 > 0)
				{
					for (num2 = 0; num2 < m_nrCols; num2++)
					{
						pageTableCell = m_tableGrid[num3 + num2];
						if (!pageTableCell.InUse || pageTableCell.UsedCell < num5)
						{
							continue;
						}
						num6 = pageTableCell.RowSpan;
						num7 = pageTableCell.ColSpan;
						int num10 = 0;
						for (int i = 0; i < num6; i++)
						{
							num10 = num3 + num2 + i * m_nrCols;
							for (int j = 0; j < num7; j++)
							{
								m_tableGrid[num10 + j].UsedCell -= num9;
							}
						}
					}
				}
				num3 += m_nrCols;
			}
			if (m_nrRows > 0)
			{
				for (num2 = 0; num2 < m_nrCols; num2++)
				{
					flag = false;
					if (!(m_tableGrid[num2].DXValue < 0.2f))
					{
						continue;
					}
					num = 0;
					num3 = 0;
					while (num < m_nrRows && !flag)
					{
						pageTableCell = m_tableGrid[num3 + num2];
						flag = (pageTableCell.InUse || pageTableCell.BorderLeft != null || pageTableCell.BorderRight != null);
						num++;
						num3 += m_nrCols;
					}
					if (flag)
					{
						continue;
					}
					list.Add(num2);
					num = 0;
					while (num < m_nrRows)
					{
						if (m_tableGrid[num * m_nrCols + num2].Eaten)
						{
							int usedCell2 = m_tableGrid[num * m_nrCols + num2].UsedCell;
							m_tableGrid[usedCell2].ColSpan--;
							num += m_tableGrid[usedCell2].RowSpan;
						}
						else
						{
							num++;
						}
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				for (int l = 0; l < m_nrRows; l++)
				{
					num8 = l * m_nrCols + list[k] - l - k;
					m_tableGrid.RemoveAt(num8);
					for (int m = 0; m < m_nrCols - 1; m++)
					{
						if (num8 + m >= m_tableGrid.Count)
						{
							continue;
						}
						PageTableCell cell = GetCell(num8 + m);
						if (cell != null && (cell.Eaten || cell.InUse))
						{
							int usedCell3 = cell.UsedCell;
							cell.UsedCell -= usedCell3 / m_nrCols;
							if (list[k] < usedCell3 % m_nrCols)
							{
								cell.UsedCell--;
							}
						}
					}
				}
				m_nrCols--;
			}
		}

		internal void AttachReportItems(RPLItemMeasurement[] reportItemCol, double delta, bool consumeContainerWhiteSpace)
		{
			FillAndFindOverlap(reportItemCol, delta);
			EmptyRowsCells();
			if (!consumeContainerWhiteSpace)
			{
				return;
			}
			float lastRowHeight = 0f;
			float lastColWidth = 0f;
			if (m_nrRows > 1)
			{
				while (!HasEmptyBottomRows(ref lastRowHeight))
				{
					RemoveConsumeContainerWhiteSpaceRows(lastRowHeight);
				}
			}
			if (m_nrCols > 1)
			{
				while (!HasEmptyRightColumns(ref lastColWidth))
				{
					RemoveConsumeContainerWhiteSpaceColumns(lastColWidth);
				}
			}
		}

		private bool HasEmptyBottomRows(ref float lastRowHeight)
		{
			bool result = false;
			int row = m_nrRows - 1;
			lastRowHeight = 0f;
			for (int i = 0; i < m_nrCols; i++)
			{
				PageTableCell cell = GetCell(row, i);
				if (!cell.Eaten || m_tableGrid[cell.UsedCell].InUse)
				{
					if (cell.InUse || cell.HasBorder || cell.Eaten)
					{
						result = true;
						break;
					}
					lastRowHeight = Math.Max(cell.DYValue.Value, lastRowHeight);
				}
			}
			return result;
		}

		private bool HasEmptyRightColumns(ref float lastColWidth)
		{
			bool result = false;
			int col = m_nrCols - 1;
			lastColWidth = 0f;
			for (int i = 0; i < m_nrRows; i++)
			{
				PageTableCell cell = GetCell(i, col);
				if (!cell.Eaten || m_tableGrid[cell.UsedCell].InUse)
				{
					if (cell.InUse || cell.HasBorder || cell.Eaten)
					{
						result = true;
						break;
					}
					lastColWidth = Math.Max(cell.DXValue.Value, lastColWidth);
				}
			}
			return result;
		}

		private void RemoveConsumeContainerWhiteSpaceRows(float lastRowHeight)
		{
			int num = m_nrRows - 2;
			for (int i = 0; i < m_nrCols; i++)
			{
				PageTableCell cell = GetCell(num, i);
				int num2 = 1;
				while (cell == null)
				{
					cell = GetCell(num - num2, i);
					if (cell == null)
					{
						num2++;
						if (num2 > num)
						{
							break;
						}
					}
				}
				if (cell != null)
				{
					cell.ConsumedByEmptyWhiteSpace = true;
					cell.DYValue.Value += lastRowHeight;
					cell.KeepBottomBorder = true;
				}
			}
			RemoveLastRow();
		}

		private void RemoveConsumeContainerWhiteSpaceColumns(float lastColWidth)
		{
			int num = m_nrCols - 2;
			for (int i = 0; i < m_nrRows; i++)
			{
				PageTableCell cell = GetCell(i, num);
				int num2 = 1;
				while (cell == null)
				{
					cell = GetCell(i, num - num2);
					if (cell == null)
					{
						num2++;
						if (num2 > num)
						{
							break;
						}
					}
				}
				if (cell != null)
				{
					cell.ConsumedByEmptyWhiteSpace = true;
					cell.DXValue.Value += lastColWidth;
					cell.KeepRightBorder = true;
				}
			}
			RemoveLastCol();
		}

		internal bool EmptyRow(RPLMeasurement[] repItemColl, bool ignoreLines, int rowIndex, bool renderHeight, ref int skipHeight)
		{
			int i = m_firstVisibleColumn;
			bool result = true;
			PageTableCell pageTableCell = null;
			for (; i < NrCols; i++)
			{
				pageTableCell = GetCell(i + rowIndex);
				if (!pageTableCell.InUse)
				{
					continue;
				}
				RPLMeasurement measurement = pageTableCell.Measurement;
				if (measurement != null || !ignoreLines)
				{
					result = false;
					if (!renderHeight && skipHeight < pageTableCell.RowSpan)
					{
						skipHeight = pageTableCell.RowSpan;
					}
					break;
				}
			}
			return result;
		}
	}
}
