using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Tablix : PageItem, IStorable, IPersistable
	{
		internal enum TablixRegion : byte
		{
			Unknown,
			Corner,
			ColumnHeader,
			RowHeader,
			Data
		}

		internal class CreateItemsContext
		{
			[Flags]
			private enum CreateItemState : byte
			{
				AdvanceRow = 0x1,
				PartialDetailRow = 0x2,
				GroupPageBreaks = 0x4,
				ContentFullyCreated = 0x8
			}

			private double m_spaceToFill;

			private double m_detailRowsHeight;

			private CreateItemState m_state = CreateItemState.AdvanceRow;

			internal double DetailRowsHeight => m_detailRowsHeight;

			internal bool PartialDetailRow
			{
				get
				{
					return (int)(m_state & CreateItemState.PartialDetailRow) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= CreateItemState.PartialDetailRow;
					}
					else
					{
						m_state &= ~CreateItemState.PartialDetailRow;
					}
				}
			}

			internal bool ContentFullyCreated
			{
				get
				{
					return (int)(m_state & CreateItemState.ContentFullyCreated) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= CreateItemState.ContentFullyCreated;
					}
					else
					{
						m_state &= ~CreateItemState.ContentFullyCreated;
					}
				}
			}

			internal bool GroupPageBreaks
			{
				get
				{
					return (int)(m_state & CreateItemState.GroupPageBreaks) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= CreateItemState.GroupPageBreaks;
					}
					else
					{
						m_state &= ~CreateItemState.GroupPageBreaks;
					}
				}
			}

			internal bool AdvanceRow
			{
				get
				{
					if (PartialDetailRow || GroupPageBreaks)
					{
						return false;
					}
					return (int)(m_state & CreateItemState.AdvanceRow) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= CreateItemState.AdvanceRow;
					}
					else
					{
						m_state &= ~CreateItemState.AdvanceRow;
					}
				}
			}

			internal bool InnerSpanPages
			{
				get
				{
					if (PartialDetailRow || GroupPageBreaks)
					{
						return true;
					}
					return false;
				}
			}

			internal double SpaceToFill => m_spaceToFill - m_detailRowsHeight;

			internal CreateItemsContext(double spaceToFill)
			{
				m_spaceToFill = spaceToFill;
				ContentFullyCreated = true;
			}

			internal void UpdateInfo(CreateItemsContext childCreateItems)
			{
				m_detailRowsHeight += childCreateItems.DetailRowsHeight;
				if (!childCreateItems.AdvanceRow)
				{
					AdvanceRow = false;
				}
				if (childCreateItems.PartialDetailRow)
				{
					PartialDetailRow = true;
				}
				if (!childCreateItems.ContentFullyCreated)
				{
					ContentFullyCreated = false;
				}
				if (childCreateItems.GroupPageBreaks)
				{
					GroupPageBreaks = true;
				}
			}

			internal void UpdateInfo(RowInfo rowInfo, double delta)
			{
				m_detailRowsHeight += Math.Max(0.0, rowInfo.NormalizeRowHeight + delta);
				if (rowInfo.SpanPagesRow)
				{
					PartialDetailRow = true;
				}
				if (!rowInfo.ContentFullyCreated)
				{
					ContentFullyCreated = false;
					PartialDetailRow = true;
				}
				if (m_spaceToFill - m_detailRowsHeight <= 0.01)
				{
					AdvanceRow = false;
				}
			}
		}

		internal class MergeDetailRows
		{
			[Flags]
			internal enum DetailRowState : byte
			{
				Merge = 0x1,
				Insert = 0x2,
				Skip = 0x4
			}

			private int m_destDetailRows;

			private List<DetailRowState> m_detailRowState;

			internal int Span => m_destDetailRows;

			internal List<DetailRowState> DetailRowsState => m_detailRowState;

			internal MergeDetailRows(int destDetailRows)
			{
				m_destDetailRows = destDetailRows;
				m_detailRowState = new List<DetailRowState>(destDetailRows);
			}

			internal void AddRowState(DetailRowState state)
			{
				m_detailRowState.Add(state);
			}
		}

		internal class LevelInfo
		{
			[Flags]
			private enum LevelInfoState : byte
			{
				PartialLevel = 0x1,
				HiddenLevel = 0x2,
				IgnoreTotals = 0x4
			}

			private int m_spanForParent;

			private double m_sizeForParent;

			private List<PageStructMemberCell> m_memberCells;

			private int m_partialItemIndex = -1;

			private double m_sourceSize;

			private int m_sourceIndex = -1;

			private LevelInfoState m_state = LevelInfoState.HiddenLevel;

			private CreateItemsContext m_createItemsContext;

			internal int SpanForParent
			{
				get
				{
					return m_spanForParent;
				}
				set
				{
					m_spanForParent = value;
				}
			}

			internal double SizeForParent
			{
				get
				{
					return m_sizeForParent;
				}
				set
				{
					m_sizeForParent = value;
				}
			}

			internal double SourceSize
			{
				get
				{
					return m_sourceSize;
				}
				set
				{
					m_sourceSize = value;
				}
			}

			internal List<PageStructMemberCell> MemberCells
			{
				get
				{
					return m_memberCells;
				}
				set
				{
					m_memberCells = value;
				}
			}

			internal bool OmittedList
			{
				get
				{
					if (m_memberCells == null)
					{
						return true;
					}
					for (int i = 0; i < m_memberCells.Count; i++)
					{
						if (!m_memberCells[i].OmittedList)
						{
							return false;
						}
					}
					return true;
				}
			}

			internal bool HiddenLevel
			{
				get
				{
					if (m_memberCells == null)
					{
						return false;
					}
					return (int)(m_state & LevelInfoState.HiddenLevel) > 0;
				}
			}

			internal bool PartialLevel => (int)(m_state & LevelInfoState.PartialLevel) > 0;

			internal bool IgnoreTotals
			{
				get
				{
					return (int)(m_state & LevelInfoState.IgnoreTotals) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= LevelInfoState.IgnoreTotals;
					}
					else
					{
						m_state &= ~LevelInfoState.IgnoreTotals;
					}
				}
			}

			internal bool PartialStruct
			{
				get
				{
					if (PartialLevel)
					{
						return true;
					}
					if (!m_createItemsContext.ContentFullyCreated)
					{
						return true;
					}
					return false;
				}
			}

			internal CreateItemsContext CreateItems => m_createItemsContext;

			internal LevelInfo(double spaceToFill)
			{
				m_createItemsContext = new CreateItemsContext(spaceToFill);
			}

			internal LevelInfo(List<PageStructMemberCell> memberCells, int partialItemIndex, int sourceIndex, CreateItemsContext createItems)
			{
				m_memberCells = memberCells;
				m_sourceIndex = sourceIndex;
				m_partialItemIndex = partialItemIndex;
				if (m_partialItemIndex >= 0)
				{
					m_state |= LevelInfoState.PartialLevel;
				}
				m_createItemsContext = createItems;
			}

			internal MergeDetailRows AddMemberCell(TablixMember member, int sourceIndex, PageMemberCell memberCell, int defMemberInfo, TablixRegion region, double[] sourceSizes, LevelInfo childLevelInfo, PageContext pageContext, Tablix tablix)
			{
				PageStructMemberCell pageStructMemberCell = null;
				if (sourceIndex > m_sourceIndex)
				{
					m_sourceIndex = sourceIndex;
					if (member.IsStatic)
					{
						pageStructMemberCell = new PageStructStaticMemberCell(sourceIndex, childLevelInfo.PartialStruct, createItem: false, member, defMemberInfo);
						if (childLevelInfo.PartialLevel)
						{
							m_state |= LevelInfoState.PartialLevel;
						}
					}
					else if (childLevelInfo.PartialStruct)
					{
						pageStructMemberCell = new PageStructDynamicMemberCell(sourceIndex, partialItem: true, createItem: false, member, defMemberInfo);
						m_state |= LevelInfoState.PartialLevel;
					}
					else
					{
						pageStructMemberCell = new PageStructDynamicMemberCell(sourceIndex, partialItem: true, createItem: true, member, defMemberInfo);
						m_state |= LevelInfoState.PartialLevel;
					}
					if (m_memberCells == null)
					{
						m_memberCells = new List<PageStructMemberCell>();
					}
					m_memberCells.Add(pageStructMemberCell);
					if (!memberCell.Hidden)
					{
						m_sourceSize += sourceSizes[member.MemberCellIndex];
					}
				}
				else if (m_partialItemIndex >= 0)
				{
					pageStructMemberCell = m_memberCells[m_partialItemIndex];
					if (childLevelInfo.PartialStruct)
					{
						pageStructMemberCell.CreateItem = false;
					}
				}
				else
				{
					pageStructMemberCell = m_memberCells[m_memberCells.Count - 1];
					if (member.IsStatic)
					{
						pageStructMemberCell.CreateItem = false;
						pageStructMemberCell.PartialItem = childLevelInfo.PartialStruct;
						if (childLevelInfo.PartialLevel)
						{
							m_state |= LevelInfoState.PartialLevel;
						}
					}
					else
					{
						pageStructMemberCell.PartialItem = true;
						m_state |= LevelInfoState.PartialLevel;
						if (childLevelInfo.PartialStruct)
						{
							pageStructMemberCell.CreateItem = false;
						}
						else
						{
							pageStructMemberCell.CreateItem = true;
						}
					}
				}
				if (!memberCell.Hidden)
				{
					m_state &= ~LevelInfoState.HiddenLevel;
				}
				if (!childLevelInfo.CreateItems.AdvanceRow)
				{
					if (memberCell.KeepTogether)
					{
						tablix.TraceInvalidatedKeepTogetherMember(pageContext, pageStructMemberCell);
					}
					memberCell.KeepTogether = false;
					if (childLevelInfo.CreateItems.InnerSpanPages)
					{
						memberCell.SpanPages = true;
						pageStructMemberCell.SpanPages = true;
					}
				}
				else
				{
					memberCell.KeepTogether = member.KeepTogether;
				}
				MergeDetailRows result = pageStructMemberCell.AddPageMemberCell(memberCell, region, pageContext);
				m_createItemsContext.UpdateInfo(childLevelInfo.CreateItems);
				return result;
			}

			internal void AddPartialStructMemberCell(TablixMember member, int sourceIndex, int defMemberIndex, bool createItem)
			{
				m_sourceIndex = sourceIndex;
				PageStructMemberCell pageStructMemberCell = null;
				pageStructMemberCell = ((!member.IsStatic) ? ((PageStructMemberCell)new PageStructDynamicMemberCell(sourceIndex, partialItem: true, createItem, member, defMemberIndex)) : ((PageStructMemberCell)new PageStructStaticMemberCell(sourceIndex, partialItem: true, createItem, member, defMemberIndex)));
				if (m_memberCells == null)
				{
					m_memberCells = new List<PageStructMemberCell>();
				}
				m_memberCells.Add(pageStructMemberCell);
				m_state |= LevelInfoState.PartialLevel;
			}

			internal void FinishPartialStructMember()
			{
				if (PartialLevel)
				{
					PageStructMemberCell pageStructMemberCell = null;
					m_state &= ~LevelInfoState.PartialLevel;
					if (m_partialItemIndex >= 0)
					{
						pageStructMemberCell = m_memberCells[m_partialItemIndex];
						m_partialItemIndex = -1;
					}
					else
					{
						pageStructMemberCell = m_memberCells[m_memberCells.Count - 1];
					}
					pageStructMemberCell.PartialItem = false;
				}
			}

			internal void SetKeepWith(int count, bool keepWith, bool repeatWith)
			{
				int num = m_memberCells.Count - 1;
				while (count > 0)
				{
					(m_memberCells[num] as PageStructStaticMemberCell).SetKeepWith(keepWith, repeatWith);
					num--;
					count--;
				}
			}
		}

		internal class HeaderFooterRow
		{
			[Flags]
			private enum HeaderFooterRowState : byte
			{
				OnPage = 0x1,
				OnPageRepeat = 0x2,
				RepeatWith = 0x4,
				KeepWith = 0x8
			}

			private int m_rowIndex;

			private int m_rowSpan;

			private double m_height;

			private HeaderFooterRowState m_state;

			private PageStructMemberCell m_structMemberCell;

			internal double Height => m_height;

			internal bool RepeatWith
			{
				get
				{
					return (int)(m_state & HeaderFooterRowState.RepeatWith) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= HeaderFooterRowState.RepeatWith;
					}
					else
					{
						m_state &= ~HeaderFooterRowState.RepeatWith;
					}
				}
			}

			internal bool KeepWith
			{
				get
				{
					return (int)(m_state & HeaderFooterRowState.KeepWith) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= HeaderFooterRowState.KeepWith;
					}
					else
					{
						m_state &= ~HeaderFooterRowState.KeepWith;
					}
				}
			}

			internal bool OnPage
			{
				get
				{
					return (int)(m_state & HeaderFooterRowState.OnPage) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= HeaderFooterRowState.OnPage;
					}
					else
					{
						m_state &= ~HeaderFooterRowState.OnPage;
					}
				}
			}

			internal bool OnPageRepeat
			{
				get
				{
					return (int)(m_state & HeaderFooterRowState.OnPageRepeat) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= HeaderFooterRowState.OnPageRepeat;
					}
					else
					{
						m_state &= ~HeaderFooterRowState.OnPageRepeat;
					}
				}
			}

			internal int RowIndex => m_rowIndex;

			internal int RowSpan => m_rowSpan;

			internal PageStructMemberCell StructMemberCell => m_structMemberCell;

			internal HeaderFooterRow(int rowIndex, int rowSpan, double height, bool onPage, bool repeatWith, bool keepWith, PageStructMemberCell structMemberCell)
			{
				m_rowIndex = rowIndex;
				m_rowSpan = rowSpan;
				m_height = height;
				OnPage = onPage;
				RepeatWith = repeatWith;
				KeepWith = keepWith;
				m_structMemberCell = structMemberCell;
			}

			internal HeaderFooterRow(HeaderFooterRow copy)
			{
				m_rowIndex = copy.RowIndex;
				m_rowSpan = copy.RowSpan;
				m_height = copy.Height;
				OnPage = copy.OnPage;
				OnPageRepeat = copy.OnPageRepeat;
				RepeatWith = copy.RepeatWith;
				KeepWith = copy.KeepWith;
				m_structMemberCell = copy.StructMemberCell;
			}

			internal int BringDetailRowOnPage(ScalableList<RowInfo> detailRows)
			{
				if (!OnPage || !OnPageRepeat)
				{
					return 0;
				}
				RowInfo item = null;
				for (int i = 0; i < m_rowSpan; i++)
				{
					using (detailRows.GetAndPin(m_rowIndex + i, out item))
					{
						item.PageVerticalState = RowInfo.VerticalState.Repeat;
					}
				}
				return 1;
			}
		}

		internal class HeaderFooterRows
		{
			private double m_totalHeight;

			private List<HeaderFooterRow> m_headerFooterRows;

			internal double Height => m_totalHeight;

			internal int Count
			{
				get
				{
					if (m_headerFooterRows == null)
					{
						return 0;
					}
					return m_headerFooterRows.Count;
				}
			}

			internal List<HeaderFooterRow> HFRows => m_headerFooterRows;

			internal int CountNotOnPage
			{
				get
				{
					if (m_headerFooterRows == null)
					{
						return 0;
					}
					int num = 0;
					int num2 = m_headerFooterRows.Count - 1;
					while (num2 >= 0 && !m_headerFooterRows[num2].OnPage)
					{
						num++;
						num2--;
					}
					return num;
				}
			}

			internal double RepeatHeight
			{
				get
				{
					if (m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = m_headerFooterRows[num2];
						if (headerFooterRow.RepeatWith)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal double RepeatNotOnPageHeight
			{
				get
				{
					if (m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = m_headerFooterRows[num2];
						if (!headerFooterRow.OnPage && headerFooterRow.RepeatWith)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal double KeepWithAndRepeatHeight
			{
				get
				{
					if (m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					bool flag = true;
					for (int num2 = m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = m_headerFooterRows[num2];
						if (flag)
						{
							if (headerFooterRow.KeepWith)
							{
								num += headerFooterRow.Height;
							}
							else
							{
								flag = false;
							}
						}
						if (!flag && headerFooterRow.RepeatWith)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal double KeepWithNoRepeatNoPageHeight
			{
				get
				{
					if (m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = m_headerFooterRows[num2];
						if (!headerFooterRow.KeepWith)
						{
							break;
						}
						if (!headerFooterRow.RepeatWith && !headerFooterRow.OnPage)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal HeaderFooterRows()
			{
			}

			internal HeaderFooterRows(HeaderFooterRows copy)
			{
				if (copy == null)
				{
					return;
				}
				m_totalHeight = copy.Height;
				if (copy.Count > 0)
				{
					m_headerFooterRows = new List<HeaderFooterRow>(copy.Count);
					for (int i = 0; i < copy.Count; i++)
					{
						m_headerFooterRows.Add(new HeaderFooterRow(copy.HFRows[i]));
					}
				}
			}

			internal HeaderFooterRows(int count, HeaderFooterRows prevRows, HeaderFooterRows levelRows)
			{
				m_headerFooterRows = new List<HeaderFooterRow>(count);
				Add(prevRows);
				Add(levelRows);
			}

			internal void AddClone(HeaderFooterRows rows)
			{
				if (rows == null)
				{
					return;
				}
				m_totalHeight += rows.Height;
				if (rows.Count > 0)
				{
					if (m_headerFooterRows == null)
					{
						m_headerFooterRows = new List<HeaderFooterRow>(rows.Count);
					}
					for (int i = 0; i < rows.Count; i++)
					{
						m_headerFooterRows.Add(new HeaderFooterRow(rows.HFRows[i]));
					}
				}
			}

			internal void Add(HeaderFooterRows rows)
			{
				if (rows == null)
				{
					return;
				}
				m_totalHeight += rows.Height;
				if (rows.Count > 0)
				{
					if (m_headerFooterRows == null)
					{
						m_headerFooterRows = new List<HeaderFooterRow>(rows.Count);
					}
					for (int i = 0; i < rows.Count; i++)
					{
						m_headerFooterRows.Add(rows.HFRows[i]);
					}
				}
			}

			internal void SetRowsKeepWith(bool keepWith)
			{
				if (m_headerFooterRows != null)
				{
					for (int i = 0; i < m_headerFooterRows.Count; i++)
					{
						m_headerFooterRows[i].KeepWith = keepWith;
					}
				}
			}

			internal void SetRowsOnPage(bool onPage)
			{
				if (m_headerFooterRows != null)
				{
					for (int i = 0; i < m_headerFooterRows.Count; i++)
					{
						m_headerFooterRows[i].OnPage = onPage;
					}
				}
			}

			internal void AddHeaderRow(int rowIndex, int rowSpan, double height, bool onPage, bool repeatWith, bool keepWith, PageStructMemberCell structMemberCell)
			{
				if (m_headerFooterRows == null)
				{
					m_headerFooterRows = new List<HeaderFooterRow>();
				}
				m_headerFooterRows.Add(new HeaderFooterRow(rowIndex, rowSpan, height, onPage, repeatWith, keepWith, structMemberCell));
				m_totalHeight += height;
			}

			internal void AddFooterRow(int rowIndex, int rowSpan, double height, bool onPage, bool repeatWith, bool keepWith, PageStructMemberCell structMemberCell)
			{
				if (m_headerFooterRows == null)
				{
					m_headerFooterRows = new List<HeaderFooterRow>();
				}
				m_headerFooterRows.Insert(0, new HeaderFooterRow(rowIndex, rowSpan, height, onPage, repeatWith, keepWith, structMemberCell));
				m_totalHeight += height;
			}

			internal int AddKeepWithAndRepeat(ref double delta, ref double addHeight, PageContext pageContext, Tablix tablix)
			{
				if (m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				HeaderFooterRow headerFooterRow = null;
				bool flag = true;
				for (int num2 = m_headerFooterRows.Count - 1; num2 >= 0; num2--)
				{
					headerFooterRow = m_headerFooterRows[num2];
					if (headerFooterRow.OnPage)
					{
						continue;
					}
					if (flag)
					{
						if (!headerFooterRow.KeepWith)
						{
							flag = false;
							if (!headerFooterRow.RepeatWith)
							{
								continue;
							}
						}
					}
					else if (!headerFooterRow.RepeatWith)
					{
						continue;
					}
					if (headerFooterRow.Height <= delta)
					{
						delta -= headerFooterRow.Height;
						addHeight += headerFooterRow.Height;
						num++;
						headerFooterRow.OnPage = true;
						if (!flag)
						{
							headerFooterRow.OnPageRepeat = true;
							tablix.TraceRepeatOnNewPage(pageContext, headerFooterRow);
						}
					}
					else
					{
						delta = 0.0;
						if (!pageContext.Common.DiagnosticsEnabled || flag)
						{
							return num;
						}
						tablix.TraceInvalidatedRepeatOnNewPageSize(pageContext, headerFooterRow);
					}
				}
				return num;
			}

			internal int AddToPageRepeat(ref double delta, ref double addHeight, PageContext pageContext, Tablix tablix)
			{
				if (m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				HeaderFooterRow headerFooterRow = null;
				for (int num2 = m_headerFooterRows.Count - 1; num2 >= 0; num2--)
				{
					headerFooterRow = m_headerFooterRows[num2];
					if (headerFooterRow.RepeatWith && !headerFooterRow.OnPage)
					{
						if (headerFooterRow.Height <= delta)
						{
							delta -= headerFooterRow.Height;
							addHeight += headerFooterRow.Height;
							num++;
							headerFooterRow.OnPage = true;
							headerFooterRow.OnPageRepeat = true;
							tablix.TraceRepeatOnNewPage(pageContext, headerFooterRow);
						}
						else
						{
							delta = 0.0;
							if (!pageContext.Common.DiagnosticsEnabled)
							{
								return num;
							}
							tablix.TraceInvalidatedRepeatOnNewPageSize(pageContext, headerFooterRow);
						}
					}
				}
				return num;
			}

			internal void CheckInvalidatedPageRepeat(PageContext pageContext, Tablix tablix)
			{
				if (m_headerFooterRows == null)
				{
					return;
				}
				HeaderFooterRow headerFooterRow = null;
				for (int num = m_headerFooterRows.Count - 1; num >= 0; num--)
				{
					headerFooterRow = m_headerFooterRows[num];
					if (headerFooterRow.RepeatWith && !headerFooterRow.OnPage)
					{
						tablix.TraceInvalidatedRepeatOnNewPageSplitMember(pageContext, headerFooterRow);
					}
				}
			}

			internal int MoveToNextPage(ref double delta)
			{
				if (m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				HeaderFooterRow headerFooterRow = null;
				int num2 = m_headerFooterRows.Count - 1;
				while (num2 >= 0)
				{
					headerFooterRow = m_headerFooterRows[num2];
					if (!headerFooterRow.OnPage || headerFooterRow.OnPageRepeat)
					{
						break;
					}
					if (headerFooterRow.Height <= delta)
					{
						delta -= headerFooterRow.Height;
						num++;
						headerFooterRow.OnPage = false;
						headerFooterRow.OnPageRepeat = false;
						num2--;
						continue;
					}
					delta = 0.0;
					return num;
				}
				return num;
			}

			internal int BringDetailRowOnPage(ScalableList<RowInfo> detailRows)
			{
				if (m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				for (int i = 0; i < m_headerFooterRows.Count; i++)
				{
					num += m_headerFooterRows[i].BringDetailRowOnPage(detailRows);
				}
				return num;
			}
		}

		private class ColumnSpan
		{
			public int Start;

			public int Span;

			public double SpanSize;

			public ColumnSpan(int start, int span, double spanSize)
			{
				Start = start;
				Span = span;
				SpanSize = spanSize;
			}

			public int CalculateEmptyColumnns(ScalableList<ColumnInfo> columnInfoList)
			{
				int num = 0;
				for (int i = Start; i < Start + Span; i++)
				{
					ColumnInfo columnInfo = columnInfoList[i];
					if (columnInfo == null || columnInfo.Empty)
					{
						num++;
					}
				}
				return num;
			}
		}

		[Flags]
		private enum TablixState : short
		{
			NoRows = 0x1,
			IsLTR = 0x2,
			RepeatColumnHeaders = 0x4,
			RepeatedColumnHeaders = 0x8,
			AddToPageColumnHeaders = 0x10,
			SplitColumnHeaders = 0x20,
			RepeatRowHeaders = 0x40,
			AddToPageRowHeaders = 0x80,
			SplitRowHeaders = 0x100,
			ColumnHeadersCreated = 0x200,
			RowHeadersCreated = 0x400
		}

		internal class PageMemberCell : IStorable, IPersistable
		{
			[Flags]
			private enum MemberState : byte
			{
				Clear = 0x0,
				Hidden = 0x1,
				SpanPages = 0x2,
				ContentOnPage = 0x4,
				IgnoreTotals = 0x8,
				KeepTogether = 0x10
			}

			private PageItem m_memberItem;

			private List<PageStructMemberCell> m_children;

			internal double m_sourceWidth;

			internal int m_rowSpan;

			internal int m_colSpan;

			internal int m_currRowSpan;

			internal int m_currColSpan;

			internal double m_startPos;

			internal double m_size;

			internal string m_label;

			internal string m_uniqueName;

			private MemberState m_memberState;

			private string m_pageName;

			private static Declaration m_declaration = GetDeclaration();

			internal List<PageStructMemberCell> Children
			{
				get
				{
					return m_children;
				}
				set
				{
					m_children = value;
				}
			}

			internal bool SpanPages
			{
				get
				{
					return (int)(m_memberState & MemberState.SpanPages) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.SpanPages;
					}
					else
					{
						m_memberState &= ~MemberState.SpanPages;
					}
				}
			}

			internal bool HasInnerPageName
			{
				get
				{
					if (m_children == null || m_children.Count == 0)
					{
						return false;
					}
					for (int i = 0; i < m_children.Count; i++)
					{
						if (m_children[i].HasInnerPageName)
						{
							return true;
						}
					}
					return false;
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(m_memberState & MemberState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.Hidden;
					}
					else
					{
						m_memberState &= ~MemberState.Hidden;
					}
				}
			}

			internal bool ContentOnPage
			{
				get
				{
					return (int)(m_memberState & MemberState.ContentOnPage) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.ContentOnPage;
					}
					else
					{
						m_memberState &= ~MemberState.ContentOnPage;
					}
				}
			}

			internal bool IgnoreTotals
			{
				get
				{
					return (int)(m_memberState & MemberState.IgnoreTotals) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.IgnoreTotals;
					}
					else
					{
						m_memberState &= ~MemberState.IgnoreTotals;
					}
				}
			}

			internal bool HasOmittedChildren
			{
				get
				{
					if (m_children == null || m_children.Count == 0)
					{
						return false;
					}
					for (int i = 0; i < m_children.Count; i++)
					{
						if (m_children[i].HasOmittedChildren)
						{
							return true;
						}
					}
					return false;
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(m_memberState & MemberState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.KeepTogether;
					}
					else
					{
						m_memberState &= ~MemberState.KeepTogether;
					}
				}
			}

			internal double StartPos
			{
				get
				{
					return m_startPos;
				}
				set
				{
					m_startPos = value;
				}
			}

			internal double SizeValue
			{
				get
				{
					return m_size;
				}
				set
				{
					m_size = value;
				}
			}

			internal double EndPos => m_startPos + m_size;

			internal double ContentBottom => m_startPos + ContentHeight;

			internal double ContentRight => m_startPos + ContentWidth;

			internal int ColSpan
			{
				get
				{
					return m_colSpan;
				}
				set
				{
					m_colSpan = value;
				}
			}

			internal int RowSpan
			{
				get
				{
					return m_rowSpan;
				}
				set
				{
					m_rowSpan = value;
				}
			}

			internal int CurrRowSpan
			{
				get
				{
					return m_currRowSpan;
				}
				set
				{
					m_currRowSpan = value;
				}
			}

			internal int CurrColSpan
			{
				get
				{
					return m_currColSpan;
				}
				set
				{
					m_currColSpan = value;
				}
			}

			internal double ContentHeight
			{
				get
				{
					if (m_memberItem == null || Hidden)
					{
						return 0.0;
					}
					return m_memberItem.ItemPageSizes.Bottom;
				}
			}

			internal double ContentWidth
			{
				get
				{
					if (m_memberItem == null || Hidden)
					{
						return 0.0;
					}
					return m_memberItem.ItemPageSizes.Right;
				}
			}

			internal PageItem MemberItem
			{
				get
				{
					return m_memberItem;
				}
				set
				{
					m_memberItem = value;
				}
			}

			internal string UniqueName => m_uniqueName;

			internal string Label => m_label;

			internal string PageName
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

			public int Size => 41 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_memberItem) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_children) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_label) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_uniqueName) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageName);

			internal PageMemberCell()
			{
			}

			internal PageMemberCell(PageItem item, int rowSpan, int colSpan, double sourceWidth, Group group)
			{
				m_memberItem = item;
				m_sourceWidth = sourceWidth;
				m_rowSpan = rowSpan;
				m_currRowSpan = rowSpan;
				m_colSpan = colSpan;
				m_currColSpan = colSpan;
				if (group == null)
				{
					return;
				}
				GroupInstance instance = group.Instance;
				if (group.DocumentMapLabel != null)
				{
					if (group.DocumentMapLabel.IsExpression)
					{
						m_label = instance.DocumentMapLabel;
					}
					else
					{
						m_label = group.DocumentMapLabel.Value;
					}
					if (m_label != null)
					{
						m_uniqueName = instance.UniqueName;
					}
				}
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.MemberItem:
						writer.Write(m_memberItem);
						break;
					case MemberName.Children:
						writer.Write(m_children);
						break;
					case MemberName.Width:
						writer.Write(m_sourceWidth);
						break;
					case MemberName.RowSpan:
						writer.Write(m_rowSpan);
						break;
					case MemberName.ColSpan:
						writer.Write(m_colSpan);
						break;
					case MemberName.CurrRowSpan:
						writer.Write(m_currRowSpan);
						break;
					case MemberName.CurrColSpan:
						writer.Write(m_currColSpan);
						break;
					case MemberName.StartPos:
						writer.Write(m_startPos);
						break;
					case MemberName.Size:
						writer.Write(m_size);
						break;
					case MemberName.Label:
						writer.Write(m_label);
						break;
					case MemberName.UniqueName:
						writer.Write(m_uniqueName);
						break;
					case MemberName.MemberState:
						writer.Write((byte)m_memberState);
						break;
					case MemberName.PageName:
						writer.Write(m_pageName);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.MemberItem:
						m_memberItem = (PageItem)reader.ReadRIFObject();
						break;
					case MemberName.Children:
						m_children = reader.ReadGenericListOfRIFObjects<PageStructMemberCell>();
						break;
					case MemberName.Width:
						m_sourceWidth = reader.ReadDouble();
						break;
					case MemberName.RowSpan:
						m_rowSpan = reader.ReadInt32();
						break;
					case MemberName.ColSpan:
						m_colSpan = reader.ReadInt32();
						break;
					case MemberName.CurrRowSpan:
						m_currRowSpan = reader.ReadInt32();
						break;
					case MemberName.CurrColSpan:
						m_currColSpan = reader.ReadInt32();
						break;
					case MemberName.StartPos:
						m_startPos = reader.ReadDouble();
						break;
					case MemberName.Size:
						m_size = reader.ReadDouble();
						break;
					case MemberName.Label:
						m_label = reader.ReadString();
						break;
					case MemberName.UniqueName:
						m_uniqueName = reader.ReadString();
						break;
					case MemberName.MemberState:
						m_memberState = (MemberState)reader.ReadByte();
						break;
					case MemberName.PageName:
						m_pageName = reader.ReadString();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.PageMemberCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberItem, ObjectType.PageItem));
					list.Add(new MemberInfo(MemberName.Children, ObjectType.RIFObjectList, ObjectType.PageStructMemberCell));
					list.Add(new MemberInfo(MemberName.Width, Token.Double));
					list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.CurrRowSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.CurrColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.StartPos, Token.Double));
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.Label, Token.String));
					list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
					list.Add(new MemberInfo(MemberName.MemberState, Token.Byte));
					list.Add(new MemberInfo(MemberName.PageName, Token.String));
					return new Declaration(ObjectType.PageMemberCell, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal bool RHOnVerticalPage(ScalableList<RowInfo> detailRows, int rowIndex)
			{
				RowInfo rowInfo = detailRows[rowIndex];
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Below)
				{
					return false;
				}
				if (m_rowSpan > 1)
				{
					rowInfo = detailRows[rowIndex + m_rowSpan - 1];
				}
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
				{
					return false;
				}
				return true;
			}

			internal bool RHColsOnHorizontalPage(List<SizeInfo> rowHeadersWidths, int colIndex, out int colSpan)
			{
				colSpan = 0;
				if (m_colSpan > 0)
				{
					for (int i = 0; i < m_colSpan; i++)
					{
						if (rowHeadersWidths[colIndex + i].State == SizeInfo.PageState.Normal)
						{
							colSpan++;
						}
					}
					return colSpan > 0;
				}
				return true;
			}

			internal bool CHRowsOnVerticalPage(List<SizeInfo> colHeadersHeights, int rowIndex, out int rowSpan)
			{
				rowSpan = 0;
				if (m_rowSpan > 0)
				{
					for (int i = 0; i < m_rowSpan; i++)
					{
						if (colHeadersHeights[rowIndex + i].State == SizeInfo.PageState.Normal)
						{
							rowSpan++;
						}
					}
					return rowSpan > 0;
				}
				return true;
			}

			private bool CHOnVerticalPage(List<SizeInfo> colHeadersHeigths, int rowIndex)
			{
				if (colHeadersHeigths == null)
				{
					return true;
				}
				if (m_rowSpan == 0)
				{
					rowIndex--;
					if (rowIndex < 0)
					{
						rowIndex = 0;
					}
					if (colHeadersHeigths[rowIndex].State == SizeInfo.PageState.Normal)
					{
						return true;
					}
				}
				else
				{
					for (int i = 0; i < m_rowSpan; i++)
					{
						if (colHeadersHeigths[rowIndex + i].State == SizeInfo.PageState.Normal)
						{
							return true;
						}
					}
				}
				return false;
			}

			internal bool CHOnHorizontalPage(ScalableList<ColumnInfo> columnInfo, int colIndex)
			{
				ColumnInfo columnInfo2 = columnInfo[colIndex];
				if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					return false;
				}
				if (m_colSpan > 1)
				{
					columnInfo2 = columnInfo[colIndex + m_colSpan - 1];
				}
				if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					return false;
				}
				return true;
			}

			internal void ResolveRHVertical(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, PageContext pageContext)
			{
				int num = rowIndex + m_rowSpan - 1;
				while (rowIndex < num && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
				}
				m_size = detailRows[num].Bottom - detailRows[rowIndex].Top;
				if (m_memberItem == null)
				{
					return;
				}
				if (m_memberItem.KTVIsUnresolved || m_memberItem.NeedResolve)
				{
					double topInParentSystem = Math.Max(0.0, startInTablix - m_startPos);
					double bottomInParentSystem = endInTablix - m_startPos;
					if (!pageContext.IgnorePageBreaks)
					{
						new PageContext(pageContext)
						{
							IgnorePageBreaks = true,
							IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell
						};
					}
					m_memberItem.ResolveVertical(pageContext, topInParentSystem, bottomInParentSystem, null, resolveItem: true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
				}
				double num2 = m_memberItem.ItemPageSizes.Bottom - m_size;
				if (!(num2 > 0.0))
				{
					return;
				}
				m_size += num2;
				RowInfo item = null;
				using (detailRows.GetAndPin(num, out item))
				{
					item.Height += num2;
				}
				num++;
				if (num < detailRows.Count)
				{
					using (detailRows.GetAndPin(num, out item))
					{
						item.PageVerticalState = RowInfo.VerticalState.Unknown;
					}
				}
			}

			internal void ResolveCHHorizontal(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix, PageContext pageContext)
			{
				int num = colIndex + m_colSpan - 1;
				while (colIndex < num && columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					colIndex++;
				}
				m_size = columnInfo[num].Right - columnInfo[colIndex].Left;
				if (m_memberItem == null)
				{
					return;
				}
				if (m_memberItem.KTHIsUnresolved || m_memberItem.NeedResolve)
				{
					double leftInParentSystem = Math.Max(0.0, startInTablix - m_startPos);
					double rightInParentSystem = endInTablix - m_startPos;
					if (!pageContext.IgnorePageBreaks)
					{
						new PageContext(pageContext)
						{
							IgnorePageBreaks = true,
							IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell
						};
					}
					m_memberItem.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, resolveItem: true);
				}
				double num2 = m_memberItem.ItemPageSizes.Right - m_size;
				if (!(num2 > 0.0))
				{
					return;
				}
				m_size += num2;
				ColumnInfo item = null;
				using (columnInfo.GetAndPin(num, out item))
				{
					item.SizeValue += num2;
				}
				num++;
				if (num < columnInfo.Count)
				{
					using (columnInfo.GetAndPin(num, out item))
					{
						item.PageHorizontalState = ColumnInfo.HorizontalState.Unknown;
					}
				}
			}

			internal void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (Hidden)
				{
					return;
				}
				if (rowIndex <= targetRowIndex && rowIndex + m_rowSpan > targetRowIndex)
				{
					if (m_memberItem != null && (m_memberItem.KTVIsUnresolved || m_memberItem.NeedResolve))
					{
						double startPos = rowHeights[rowIndex].StartPos;
						double num = 0.0;
						for (int i = rowIndex; i < targetRowIndex; i++)
						{
							num += rowHeights[i].SizeValue;
						}
						double topInParentSystem = Math.Max(0.0, startInTablix - startPos);
						double bottomInParentSystem = endInTablix - startPos;
						PageContext pageContext2 = pageContext;
						if (!pageContext.IgnorePageBreaks)
						{
							pageContext2 = new PageContext(pageContext);
							pageContext2.IgnorePageBreaks = true;
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						m_memberItem.ResolveVertical(pageContext2, topInParentSystem, bottomInParentSystem, null, resolveItem: true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
						num = m_memberItem.ItemPageSizes.Bottom - num;
						if (num > 0.0)
						{
							SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + m_rowSpan - targetRowIndex, num);
						}
					}
				}
				else if (m_children != null)
				{
					for (int j = 0; j < m_children.Count; j++)
					{
						m_children[j].AlignCHToPageVertical(rowIndex + m_rowSpan, targetRowIndex, startInTablix, endInTablix, rowHeights, pageContext);
					}
				}
			}

			internal void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (!RHOnVerticalPage(detailRows, rowIndex))
				{
					return;
				}
				if (colIndex <= targetColIndex && colIndex + m_colSpan > targetColIndex)
				{
					if (m_memberItem == null || (!m_memberItem.KTHIsUnresolved && !m_memberItem.NeedResolve))
					{
						return;
					}
					double num = 0.0;
					double num2 = 0.0;
					if (isLTR)
					{
						num = colWidths[colIndex].StartPos;
						for (int i = colIndex; i < targetColIndex; i++)
						{
							num2 += colWidths[i].SizeValue;
						}
					}
					else
					{
						num = colWidths[colIndex + m_colSpan - 1].StartPos;
						for (int num3 = colIndex + m_colSpan - 1; num3 > targetColIndex; num3--)
						{
							num2 += colWidths[num3].SizeValue;
						}
					}
					double leftInParentSystem = Math.Max(0.0, startInTablix - num);
					double rightInParentSystem = endInTablix - num;
					m_memberItem.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, resolveItem: true);
					num2 = m_memberItem.ItemPageSizes.Right - num2;
					if (num2 > 0.0)
					{
						if (isLTR)
						{
							SizeInfo.UpdateSize(colWidths[targetColIndex], colIndex + m_colSpan - targetColIndex, num2);
						}
						else
						{
							SizeInfo.UpdateSize(colWidths[targetColIndex], targetColIndex - colIndex + 1, num2);
						}
					}
				}
				else if (m_children != null)
				{
					for (int j = 0; j < m_children.Count; j++)
					{
						m_children[j].AlignRHToPageHorizontal(detailRows, rowIndex, colIndex + m_colSpan, targetColIndex, startInTablix, endInTablix, colWidths, isLTR, pageContext);
						rowIndex += m_children[j].Span;
					}
				}
			}

			internal void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext)
			{
				if (Hidden || !RHOnVerticalPage(detailRows, rowIndex))
				{
					return;
				}
				if (m_memberItem != null)
				{
					bool anyAncestorHasKT = false;
					PageContext pageContext2 = new PageContext(pageContext);
					m_memberItem.CalculateHorizontal(pageContext2, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true, m_sourceWidth);
					double size = Math.Max(m_sourceWidth, m_memberItem.ItemPageSizes.Width);
					UpdateSizes(colIndex, m_colSpan, size, ref colWidths);
				}
				else
				{
					UpdateSizes(colIndex, m_colSpan, m_sourceWidth, ref colWidths);
				}
				if (m_children != null)
				{
					colIndex += m_colSpan;
					for (int i = 0; i < m_children.Count; i++)
					{
						m_children[i].CalculateRHHorizontal(detailRows, rowIndex, colIndex, ref colWidths, pageContext);
						rowIndex += m_children[i].Span;
					}
				}
			}

			internal int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (!RHOnVerticalPage(detailRows, rowIndex))
				{
					return 0;
				}
				if (m_label != null)
				{
					hasLabels = true;
				}
				if (m_memberItem != null)
				{
					double pageTop2 = Math.Max(0.0, pageTop - m_startPos);
					double pageBottom2 = pageBottom - m_startPos;
					double num = 0.0;
					double num2 = 0.0;
					double num3 = 0.0;
					if (!Hidden)
					{
						num = ((!isLTR) ? colWidths[colIndex + m_colSpan - 1].StartPos : colWidths[colIndex].StartPos);
					}
					else if (colWidths != null)
					{
						if (!isLTR)
						{
							num = ((colIndex != colWidths.Count - 1) ? colWidths[colIndex].EndPos : colWidths[colIndex].StartPos);
						}
						else
						{
							colIndex--;
							num = ((colIndex < 0) ? colWidths[0].StartPos : colWidths[colIndex].EndPos);
						}
					}
					num3 = Math.Max(0.0, pageLeft - num);
					num2 = pageRight - num;
					double height = m_memberItem.ItemPageSizes.Height;
					m_memberItem.ItemPageSizes.AdjustHeightTo(m_size + Math.Abs(m_memberItem.ItemPageSizes.Top));
					ContentOnPage = m_memberItem.AddToPage(rplWriter, pageContext, num3, pageTop2, num2, pageBottom2, repeatState);
					if (!IsSpanning(m_startPos, EndPos, pageTop, pageBottom))
					{
						RowInfo rowInfo = detailRows[rowIndex];
						m_memberItem.ItemPageSizes.AdjustHeightTo(rowInfo.RepeatOnPage ? height : 0.0);
					}
					if (ContentOnPage)
					{
						ContentOnPage = m_memberItem.ContentOnPage;
					}
				}
				int num4 = 0;
				if (m_children == null)
				{
					if (Hidden)
					{
						return num4;
					}
					for (int i = 0; i < m_rowSpan; i++)
					{
						if (detailRows[rowIndex + i].PageVerticalState == RowInfo.VerticalState.Normal)
						{
							num4++;
						}
					}
					return num4;
				}
				if (!Hidden)
				{
					colIndex += m_colSpan;
				}
				for (int j = 0; j < m_children.Count; j++)
				{
					num4 += m_children[j].AddToPageRHContent(colWidths, detailRows, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
					rowIndex += m_children[j].Span;
				}
				return num4;
			}

			internal void CalculateCHHorizontal(List<SizeInfo> rowHeigths, int rowIndex, int colIndex, ScalableList<ColumnInfo> columnInfo, PageContext pageContext, bool onPage)
			{
				if (Hidden)
				{
					UpdateHidden(colIndex, m_colSpan, columnInfo);
					return;
				}
				if (m_children != null)
				{
					int num = colIndex;
					for (int i = 0; i < m_children.Count; i++)
					{
						m_children[i].CalculateCHHorizontal(rowHeigths, rowIndex + m_rowSpan, num, columnInfo, pageContext, onPage);
						num += m_children[i].Span;
					}
				}
				if (onPage && CHOnVerticalPage(rowHeigths, rowIndex))
				{
					if (m_memberItem != null)
					{
						bool anyAncestorHasKT = false;
						bool unresolved = m_memberItem.KTHIsUnresolved || m_memberItem.NeedResolve;
						PageContext pageContext2 = new PageContext(pageContext);
						m_memberItem.CalculateHorizontal(pageContext2, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true, m_sourceWidth);
						UpdateSizes(colIndex, m_colSpan, m_memberItem.ItemPageSizes.Width, unresolved, keepTogether: false, split: true, columnInfo);
					}
					else
					{
						UpdateSizes(colIndex, m_colSpan, m_sourceWidth, unresolved: false, keepTogether: false, split: true, columnInfo);
					}
				}
			}

			internal int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				int num = 0;
				int num2 = rowIndex;
				if (!Hidden)
				{
					num2 += m_rowSpan;
				}
				if (m_label != null)
				{
					hasLabels = true;
				}
				if (m_children != null)
				{
					int num3 = colIndex;
					for (int i = 0; i < m_children.Count; i++)
					{
						num += m_children[i].AddToPageCHContent(rowHeights, columnInfo, num2, num3, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
						num3 += m_children[i].Span;
					}
				}
				else if (!Hidden)
				{
					for (int j = 0; j < m_colSpan; j++)
					{
						if (columnInfo[colIndex + j].PageHorizontalState == ColumnInfo.HorizontalState.Normal)
						{
							num++;
						}
					}
				}
				if (m_memberItem != null)
				{
					RepeatState repeatState2 = repeatState;
					if (num != m_colSpan && columnInfo[colIndex + m_colSpan - 1].PageHorizontalState != ColumnInfo.HorizontalState.Normal)
					{
						repeatState2 |= RepeatState.Horizontal;
					}
					if (Hidden)
					{
						double num4 = 0.0;
						rowIndex--;
						if (rowHeights != null)
						{
							if (rowIndex < 0)
							{
								rowIndex = 0;
								num4 = rowHeights[0].StartPos;
							}
							else
							{
								num4 = rowHeights[rowIndex].EndPos;
							}
						}
						if (rowHeights == null || rowHeights[rowIndex].State == SizeInfo.PageState.Normal)
						{
							double pageTop2 = Math.Max(0.0, pageTop - num4);
							double pageBottom2 = pageBottom - num4;
							double pageLeft2 = Math.Max(0.0, pageLeft - m_startPos);
							double pageRight2 = pageRight - m_startPos;
							if (rowHeights != null)
							{
								m_memberItem.ItemPageSizes.AdjustHeightTo(rowHeights[rowIndex + m_rowSpan - 1].EndPos - num4);
							}
							ContentOnPage = m_memberItem.AddToPage(rplWriter, pageContext, pageLeft2, pageTop2, pageRight2, pageBottom2, repeatState2);
						}
					}
					else
					{
						if (!CHOnVerticalPage(rowHeights, rowIndex))
						{
							return num;
						}
						double startPos = rowHeights[rowIndex].StartPos;
						double pageTop3 = Math.Max(0.0, pageTop - startPos);
						double pageBottom3 = pageBottom - startPos;
						double pageLeft3 = Math.Max(0.0, pageLeft - m_startPos);
						double pageRight3 = pageRight - m_startPos;
						m_memberItem.ItemPageSizes.AdjustHeightTo(rowHeights[rowIndex + m_rowSpan - 1].EndPos - startPos);
						ContentOnPage = m_memberItem.AddToPage(rplWriter, pageContext, pageLeft3, pageTop3, pageRight3, pageBottom3, repeatState2);
					}
					if (ContentOnPage)
					{
						ContentOnPage = m_memberItem.ContentOnPage;
					}
				}
				return num;
			}

			internal double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight)
			{
				double num = 0.0;
				int num2 = rowIndex;
				if (m_children != null)
				{
					for (int i = 0; i < m_children.Count; i++)
					{
						num += m_children[i].NormalizeDetailRowHeight(detailRows, num2, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
						num2 += m_children[i].Span;
					}
				}
				else
				{
					RowInfo item = null;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					for (int j = 0; j < m_rowSpan; j++)
					{
						using (detailRows.GetAndPin(num2, out item))
						{
							if (item.PageVerticalState == RowInfo.VerticalState.Above)
							{
								num2++;
								continue;
							}
							if (item.PageVerticalState == RowInfo.VerticalState.Repeat)
							{
								item.Top = startInTablix;
								if (lastRowOnPage >= 0)
								{
									item.Top = detailRows[lastRowOnPage].Bottom;
								}
								lastRowOnPage = num2;
								item.PageVerticalState = RowInfo.VerticalState.Normal;
								item.RepeatOnPage = true;
								num += detailRows[num2].Height;
							}
							else if (item.PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
							{
								item.Top = endInTablix;
								item.PageVerticalState = RowInfo.VerticalState.Below;
								lastRowBelow = num2;
							}
							else if (item.PageVerticalState == RowInfo.VerticalState.Below)
							{
								item.PageVerticalState = RowInfo.VerticalState.Normal;
								if (lastRowOnPage >= 0)
								{
									item.Top = detailRows[lastRowOnPage].Bottom;
								}
								lastRowOnPage = num2;
								num += detailRows[num2].Height;
							}
							else if (lastRowBelow >= 0)
							{
								item.Top = detailRows[lastRowBelow].Bottom;
								lastRowBelow = num2;
								item.PageVerticalState = RowInfo.VerticalState.Below;
							}
							else
							{
								if (item.PageVerticalState == RowInfo.VerticalState.Unknown)
								{
									item.Top = startInTablix;
									item.PageVerticalState = RowInfo.VerticalState.Normal;
								}
								if (lastRowOnPage >= 0)
								{
									item.Top = detailRows[lastRowOnPage].Bottom;
								}
								if (update)
								{
									roundedDouble.Value = item.Top;
									if (roundedDouble >= endInTablix)
									{
										lastRowBelow = num2;
										item.PageVerticalState = RowInfo.VerticalState.Below;
									}
									else
									{
										lastRowOnPage = num2;
									}
								}
								else
								{
									lastRowOnPage = num2;
								}
								num += detailRows[num2].Height;
							}
							goto IL_01f8;
						}
						IL_01f8:
						num2++;
					}
				}
				while (rowIndex < num2 && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
				}
				if (rowIndex == num2)
				{
					return 0.0;
				}
				if (rowIndex < detailRows.Count)
				{
					m_startPos = detailRows[rowIndex].Top;
				}
				if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
				{
					return 0.0;
				}
				if (m_memberItem != null && !Hidden)
				{
					double num3 = m_memberItem.ItemPageSizes.Bottom - num;
					if (num3 > 0.0)
					{
						addedHeight += num3;
						num += num3;
						rowIndex += m_currRowSpan - 1;
						RowInfo item2 = null;
						using (detailRows.GetAndPin(rowIndex, out item2))
						{
							item2.Height += num3;
						}
					}
				}
				m_size = num;
				return num;
			}

			private static bool IsSpanning(double top, double bottom, double startTop, double endBottom)
			{
				RoundedDouble x = new RoundedDouble(top);
				RoundedDouble x2 = new RoundedDouble(bottom);
				if (!(x < startTop) || !(x2 > startTop))
				{
					if (x >= startTop)
					{
						return x2 > endBottom;
					}
					return false;
				}
				return true;
			}

			internal void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix)
			{
				int rowEndIndex = rowIndex;
				RowInfo rowInfo = detailRows[rowEndIndex];
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Below)
				{
					return;
				}
				rowInfo = detailRows[rowEndIndex + m_rowSpan - 1];
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
				{
					m_currRowSpan = m_rowSpan;
					return;
				}
				m_pageName = null;
				bool parentKeepRepeatWith2 = parentKeepRepeatWith;
				bool flag = false;
				if (m_memberItem != null && IsSpanning(m_startPos, EndPos, startInTablix, endInTablix))
				{
					parentKeepRepeatWith2 = false;
					flag = true;
				}
				if (m_children != null)
				{
					PageStructMemberCell pageStructMemberCell = null;
					for (int i = 0; i < m_children.Count; i++)
					{
						pageStructMemberCell = m_children[i];
						pageStructMemberCell.UpdateRows(detailRows, rowEndIndex, parentKeepRepeatWith2, keepRow, delete, startInTablix, endInTablix);
						if (delete && pageStructMemberCell.Span == 0 && !pageStructMemberCell.PartialItem)
						{
							m_children.RemoveAt(i);
							i--;
							i -= RemoveHeadersAbove(m_children, i, detailRows, ref rowEndIndex);
						}
						rowEndIndex += pageStructMemberCell.Span;
					}
					if (m_children.Count == 0)
					{
						m_children = null;
					}
					m_rowSpan = (m_currRowSpan = rowEndIndex - rowIndex);
				}
				else
				{
					rowEndIndex += m_rowSpan;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					for (int j = 0; j < m_rowSpan; j++)
					{
						rowInfo = detailRows[rowIndex + j];
						if (rowInfo.SpanPagesRow)
						{
							break;
						}
						roundedDouble.Value = rowInfo.Bottom;
						if (!(roundedDouble <= endInTablix))
						{
							break;
						}
						if (parentKeepRepeatWith && keepRow > 0)
						{
							if (keepRow == 1)
							{
								if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
								{
									continue;
								}
								roundedDouble.Value = rowInfo.Top;
								if (roundedDouble >= startInTablix)
								{
									using (detailRows.GetAndPin(rowIndex + j, out rowInfo))
									{
										rowInfo.PageVerticalState = RowInfo.VerticalState.Above;
									}
									continue;
								}
							}
							else if (rowInfo.RepeatOnPage)
							{
								using (detailRows.GetAndPin(rowIndex + j, out rowInfo))
								{
									rowInfo.PageVerticalState = RowInfo.VerticalState.Unknown;
									rowInfo.RepeatOnPage = false;
								}
								continue;
							}
						}
						if (delete)
						{
							rowInfo.DisposeDetailCells();
							detailRows.RemoveAt(rowIndex + j);
							m_rowSpan--;
							j--;
							rowEndIndex--;
						}
					}
					m_currRowSpan = m_rowSpan;
				}
				if (m_rowSpan <= 0)
				{
					return;
				}
				while (rowIndex < rowEndIndex && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
					m_currRowSpan--;
				}
				if (m_currRowSpan > 0)
				{
					if (flag)
					{
						double num = detailRows[rowIndex].Top - m_startPos;
						if (num > 0.0)
						{
							m_memberItem.ItemPageSizes.Top -= num;
							m_size -= num;
						}
						m_startPos = detailRows[rowIndex].Top;
					}
					else
					{
						m_startPos = detailRows[rowIndex].Top;
						m_size = detailRows[rowIndex + m_currRowSpan - 1].Bottom - m_startPos;
					}
				}
				else
				{
					m_currRowSpan = m_rowSpan;
				}
			}

			internal void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix)
			{
				int num = colIndex;
				bool flag = false;
				if (m_memberItem != null && IsSpanning(m_startPos, EndPos, startInTablix, endInTablix))
				{
					flag = true;
				}
				if (m_children != null)
				{
					PageStructMemberCell pageStructMemberCell = null;
					for (int i = 0; i < m_children.Count; i++)
					{
						pageStructMemberCell = m_children[i];
						pageStructMemberCell.UpdateColumns(columnInfo, num, startInTablix, endInTablix);
						num += pageStructMemberCell.Span;
					}
				}
				else
				{
					num += m_colSpan;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					ColumnInfo columnInfo2 = null;
					while (colIndex < num)
					{
						columnInfo2 = columnInfo[colIndex];
						if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
						{
							colIndex++;
							continue;
						}
						roundedDouble.Value = columnInfo2.Right;
						if (!(roundedDouble <= endInTablix))
						{
							break;
						}
						using (columnInfo.GetAndPin(colIndex, out columnInfo2))
						{
							columnInfo2.PageHorizontalState = ColumnInfo.HorizontalState.AtLeft;
						}
						colIndex++;
					}
				}
				if (m_colSpan <= 0)
				{
					return;
				}
				while (colIndex < num && columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					colIndex++;
				}
				m_currColSpan = num - colIndex;
				if (m_currColSpan > 0)
				{
					ColumnInfo columnInfo3 = columnInfo[colIndex];
					if (flag)
					{
						double num2 = columnInfo3.Left - m_startPos;
						if (num2 > 0.0)
						{
							m_memberItem.ItemPageSizes.Left -= num2;
							m_size -= num2;
						}
						m_startPos = columnInfo3.Left;
					}
					else
					{
						m_startPos = columnInfo3.Left;
						if (m_currColSpan == 1)
						{
							m_size = columnInfo3.Right - m_startPos;
						}
						else
						{
							m_size = columnInfo[colIndex + m_currColSpan - 1].Right - m_startPos;
						}
					}
				}
				else
				{
					m_currColSpan = m_colSpan;
				}
			}

			internal double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColAtRight, bool update, ref double addedWidth)
			{
				double num = 0.0;
				int num2 = colIndex;
				if (m_children != null)
				{
					for (int i = 0; i < m_children.Count; i++)
					{
						num += m_children[i].NormalizeDetailColWidth(detailCols, num2, startInTablix, endInTablix, ref lastColOnPage, ref lastColAtRight, update, ref addedWidth);
						num2 += m_children[i].Span;
					}
				}
				else
				{
					ColumnInfo item = null;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					for (int j = 0; j < m_colSpan; j++)
					{
						using (detailCols.GetAndPin(num2, out item))
						{
							if (item.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
							{
								num2++;
								continue;
							}
							if (item.PageHorizontalState == ColumnInfo.HorizontalState.LeftOfNextPage)
							{
								item.Left = endInTablix;
								item.PageHorizontalState = ColumnInfo.HorizontalState.AtRight;
								lastColAtRight = num2;
							}
							else if (item.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								item.PageHorizontalState = ColumnInfo.HorizontalState.Normal;
								if (lastColOnPage >= 0)
								{
									item.Left = detailCols[lastColOnPage].Right;
								}
								lastColOnPage = num2;
								num += item.SizeValue;
							}
							else if (lastColAtRight >= 0)
							{
								item.Left = detailCols[lastColAtRight].Right;
								lastColAtRight = num2;
								item.PageHorizontalState = ColumnInfo.HorizontalState.AtRight;
							}
							else
							{
								if (item.PageHorizontalState == ColumnInfo.HorizontalState.Unknown)
								{
									item.Left = startInTablix;
									item.PageHorizontalState = ColumnInfo.HorizontalState.Normal;
								}
								if (lastColOnPage >= 0)
								{
									item.Left = detailCols[lastColOnPage].Right;
								}
								if (update)
								{
									roundedDouble.Value = item.Left;
									if (roundedDouble >= endInTablix)
									{
										lastColAtRight = num2;
										item.PageHorizontalState = ColumnInfo.HorizontalState.AtRight;
									}
									else
									{
										lastColOnPage = num2;
									}
								}
								else
								{
									lastColOnPage = num2;
								}
								num += item.SizeValue;
							}
							goto IL_0199;
						}
						IL_0199:
						num2++;
					}
				}
				while (colIndex < num2 && detailCols[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					colIndex++;
				}
				if (colIndex == num2)
				{
					return 0.0;
				}
				if (colIndex < detailCols.Count)
				{
					m_startPos = detailCols[colIndex].Left;
				}
				if (detailCols[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					return 0.0;
				}
				if (m_memberItem != null && !Hidden)
				{
					double num3 = m_memberItem.ItemPageSizes.Right - num;
					if (num3 > 0.0)
					{
						addedWidth += num3;
						num += num3;
						colIndex += m_currColSpan - 1;
						ColumnInfo item2 = null;
						using (detailCols.GetAndPin(colIndex, out item2))
						{
							item2.SizeValue += num3;
						}
					}
				}
				m_size = num;
				return num;
			}

			internal void Reverse(PageContext pageContext)
			{
				if (m_children != null)
				{
					m_children.Reverse();
					for (int i = 0; i < m_children.Count; i++)
					{
						m_children[i].Reverse(pageContext);
					}
				}
			}

			internal bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (Hidden)
				{
					return false;
				}
				bool flag = false;
				if (rowIndex <= targetRowIndex && rowIndex + m_rowSpan > targetRowIndex && m_memberItem != null)
				{
					double topInParentSystem = Math.Max(0.0, startInTablix - rowHeights[rowIndex].StartPos);
					flag = m_memberItem.ResolveDuplicates(pageContext, topInParentSystem, null, recalculate: false);
					if (flag)
					{
						_ = rowHeights[rowIndex].StartPos;
						double num = 0.0;
						for (int i = rowIndex; i < targetRowIndex; i++)
						{
							num += rowHeights[i].SizeValue;
						}
						num = m_memberItem.ItemPageSizes.Bottom - num;
						if (num > 0.0)
						{
							SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + m_rowSpan - targetRowIndex, num);
						}
					}
				}
				if (m_children != null)
				{
					for (int j = 0; j < m_children.Count; j++)
					{
						flag |= m_children[j].ResolveCHDuplicates(rowIndex + m_rowSpan, targetRowIndex, startInTablix, rowHeights, pageContext);
					}
				}
				return flag;
			}

			internal bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext)
			{
				int num = rowIndex;
				int num2 = rowIndex + m_rowSpan - 1;
				while (rowIndex < num2 && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
				}
				double startPos = m_startPos;
				if (rowIndex < detailRows.Count)
				{
					m_startPos = detailRows[rowIndex].Top;
				}
				bool flag = false;
				if (m_memberItem != null && !Hidden)
				{
					double topInParentSystem = Math.Max(0.0, startInTablix - startPos);
					flag = m_memberItem.ResolveDuplicates(pageContext, topInParentSystem, null, recalculate: false);
				}
				if (m_children != null)
				{
					for (int i = 0; i < m_children.Count; i++)
					{
						flag |= m_children[i].ResolveRHDuplicates(num, startInTablix, detailRows, pageContext);
						num += m_children[i].Span;
					}
				}
				return flag;
			}
		}

		internal abstract class PageStructMemberCell : IStorable, IPersistable
		{
			[Flags]
			private enum MemberState : byte
			{
				Clear = 0x0,
				PartialItem = 0x1,
				CreateItem = 0x2,
				HasOmittedChildren = 0x4,
				NotOmittedList = 0x8,
				SpanPages = 0x10,
				HasInstances = 0x20,
				HasInnerPageName = 0x40
			}

			private int m_sourceIndex;

			private MemberState m_memberState;

			protected int m_span;

			private int m_memberDefIndex = -1;

			private TablixMember m_tablixMember;

			private static Declaration m_declaration = GetDeclaration();

			internal int SourceIndex => m_sourceIndex;

			internal bool PartialItem
			{
				get
				{
					return (int)(m_memberState & MemberState.PartialItem) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.PartialItem;
					}
					else
					{
						m_memberState &= ~MemberState.PartialItem;
					}
				}
			}

			internal bool CreateItem
			{
				get
				{
					return (int)(m_memberState & MemberState.CreateItem) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.CreateItem;
					}
					else
					{
						m_memberState &= ~MemberState.CreateItem;
					}
				}
			}

			internal bool OmittedList
			{
				get
				{
					return false;
				}
				set
				{
					if (value)
					{
						m_memberState &= ~MemberState.NotOmittedList;
					}
					else
					{
						m_memberState |= MemberState.NotOmittedList;
					}
				}
			}

			internal bool HasOmittedChildren
			{
				get
				{
					return (int)(m_memberState & MemberState.HasOmittedChildren) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.HasOmittedChildren;
					}
					else
					{
						m_memberState &= ~MemberState.HasOmittedChildren;
					}
				}
			}

			internal bool SpanPages
			{
				get
				{
					return (int)(m_memberState & MemberState.SpanPages) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.SpanPages;
					}
					else
					{
						m_memberState &= ~MemberState.SpanPages;
					}
				}
			}

			internal bool HasInnerPageName
			{
				get
				{
					return (int)(m_memberState & MemberState.HasInnerPageName) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.HasInnerPageName;
					}
					else
					{
						m_memberState &= ~MemberState.HasInnerPageName;
					}
				}
			}

			internal bool HasInstances
			{
				get
				{
					return (int)(m_memberState & MemberState.HasInstances) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.HasInstances;
					}
					else
					{
						m_memberState &= ~MemberState.HasInstances;
					}
				}
			}

			internal virtual bool StaticTree
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			internal int Span
			{
				get
				{
					return m_span;
				}
				set
				{
					m_span = value;
				}
			}

			internal int MemberDefIndex => m_memberDefIndex;

			internal byte State => (byte)m_memberState;

			internal abstract bool Hidden
			{
				get;
			}

			internal abstract double StartPos
			{
				get;
			}

			internal abstract double SizeValue
			{
				get;
			}

			internal abstract double EndPos
			{
				get;
			}

			internal abstract int PartialRowSpan
			{
				get;
			}

			internal TablixMember TablixMember => m_tablixMember;

			public virtual int Size => Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 1 + 12;

			internal PageStructMemberCell()
			{
			}

			internal PageStructMemberCell(int sourceIndex, bool partialItem, bool createItem, TablixMember member, int memberDefIndex)
			{
				m_sourceIndex = sourceIndex;
				if (partialItem)
				{
					m_memberState |= MemberState.PartialItem;
				}
				if (createItem)
				{
					m_memberState |= MemberState.CreateItem;
				}
				m_memberDefIndex = memberDefIndex;
				m_tablixMember = member;
			}

			internal PageStructMemberCell(PageStructMemberCell copy, int span)
			{
				m_sourceIndex = copy.SourceIndex;
				m_memberDefIndex = copy.MemberDefIndex;
				m_memberState = (MemberState)copy.State;
				m_span = span;
				m_tablixMember = copy.TablixMember;
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write((byte)m_memberState);
						break;
					case MemberName.SourceIndex:
						writer.Write(m_sourceIndex);
						break;
					case MemberName.DefIndex:
						writer.Write(m_memberDefIndex);
						break;
					case MemberName.Span:
						writer.Write(m_span);
						break;
					case MemberName.TablixMember:
					{
						int value = scalabilityCache.StoreStaticReference(m_tablixMember);
						writer.Write(value);
						break;
					}
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.State:
						m_memberState = (MemberState)reader.ReadByte();
						break;
					case MemberName.SourceIndex:
						m_sourceIndex = reader.ReadInt32();
						break;
					case MemberName.DefIndex:
						m_memberDefIndex = reader.ReadInt32();
						break;
					case MemberName.Span:
						m_span = reader.ReadInt32();
						break;
					case MemberName.TablixMember:
					{
						int id = reader.ReadInt32();
						m_tablixMember = (TablixMember)scalabilityCache.FetchStaticReference(id);
						break;
					}
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.PageStructMemberCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.SourceIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.DefIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.Span, Token.Int32));
					list.Add(new MemberInfo(MemberName.TablixMember, Token.Int32));
					return new Declaration(ObjectType.PageStructMemberCell, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal abstract IDisposable PartialMemberInstance(out PageMemberCell memberCell);

			internal abstract void RemoveLastMemberCell();

			internal abstract void DisposeInstances();

			internal abstract void MergePageStructMembers(PageStructMemberCell mergeStructMember, TablixRegion region, MergeDetailRows detailRowsState);

			internal abstract bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext);

			internal abstract bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext);

			internal abstract MergeDetailRows AddPageMemberCell(PageMemberCell memberCell, TablixRegion region, PageContext pageContext);

			internal abstract void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext);

			internal abstract void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext);

			internal abstract void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext);

			internal abstract void CalculateCHHorizontal(List<SizeInfo> rowHeights, int rowIndex, int colIndex, ScalableList<ColumnInfo> columnInfo, PageContext pageContext, bool onPage);

			internal abstract int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels);

			internal abstract int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels);

			internal abstract double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight);

			internal abstract double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColBelow, bool update, ref double addedWidth);

			internal abstract void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix);

			internal abstract void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix);

			internal abstract void Reverse(PageContext pageContext);

			internal virtual PageStructDynamicMemberCell Split(int colsBeforeRowHeaders, PageContext pageContext)
			{
				return null;
			}

			internal void MergePageMembers(PageMemberCell destPageMember, PageMemberCell srcPageMember, TablixRegion region, MergeDetailRows detailRowsState)
			{
				if (destPageMember == null || srcPageMember == null)
				{
					return;
				}
				if (destPageMember.MemberItem == null)
				{
					destPageMember.MemberItem = srcPageMember.MemberItem;
				}
				else
				{
					((HiddenPageItem)destPageMember.MemberItem).AddToCollection((HiddenPageItem)srcPageMember.MemberItem);
				}
				if (destPageMember.Children == null)
				{
					detailRowsState.AddRowState(MergeDetailRows.DetailRowState.Merge);
					return;
				}
				PageStructMemberCell pageStructMemberCell = null;
				PageStructMemberCell pageStructMemberCell2 = null;
				int num = -1;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				while (num4 < destPageMember.Children.Count || num3 < srcPageMember.Children.Count)
				{
					num = -1;
					if (num4 < destPageMember.Children.Count)
					{
						pageStructMemberCell = destPageMember.Children[num4];
						num = pageStructMemberCell.m_sourceIndex;
					}
					num2 = num + 1;
					if (num3 < srcPageMember.Children.Count)
					{
						pageStructMemberCell2 = srcPageMember.Children[num3];
						num2 = pageStructMemberCell2.m_sourceIndex;
					}
					if (num == num2)
					{
						int span = pageStructMemberCell.Span;
						pageStructMemberCell.MergePageStructMembers(pageStructMemberCell2, region, detailRowsState);
						span = pageStructMemberCell.Span - span;
						if (region == TablixRegion.RowHeader)
						{
							destPageMember.RowSpan += span;
						}
						else
						{
							destPageMember.ColSpan += span;
						}
						Span += span;
						num4++;
						num3++;
					}
					else if (num < 0 || num > num2)
					{
						destPageMember.Children.Insert(num4, pageStructMemberCell2);
						if (region == TablixRegion.RowHeader)
						{
							destPageMember.RowSpan += pageStructMemberCell2.Span;
						}
						else
						{
							destPageMember.ColSpan += pageStructMemberCell2.Span;
						}
						Span += pageStructMemberCell2.Span;
						num4++;
						num3++;
						for (int i = 0; i < pageStructMemberCell2.Span; i++)
						{
							detailRowsState.AddRowState(MergeDetailRows.DetailRowState.Insert);
						}
					}
					else
					{
						num4++;
						for (int j = 0; j < pageStructMemberCell.Span; j++)
						{
							detailRowsState.AddRowState(MergeDetailRows.DetailRowState.Skip);
						}
					}
				}
			}
		}

		internal class PageStructStaticMemberCell : PageStructMemberCell
		{
			[Flags]
			private enum MemberState : byte
			{
				Clear = 0x0,
				Header = 0x1,
				Footer = 0x2,
				KeepWith = 0x4,
				RepeatWith = 0x8,
				StaticTree = 0x10
			}

			private PageMemberCell m_memberInstance;

			private MemberState m_memberState;

			private static Declaration m_declaration = GetDeclaration();

			internal PageMemberCell MemberInstance => m_memberInstance;

			internal override int PartialRowSpan => 0;

			internal bool Header => (int)(m_memberState & MemberState.Header) > 0;

			internal bool Footer => (int)(m_memberState & MemberState.Footer) > 0;

			internal bool KeepWith
			{
				get
				{
					return (int)(m_memberState & MemberState.KeepWith) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.KeepWith;
					}
					else
					{
						m_memberState &= ~MemberState.KeepWith;
					}
				}
			}

			internal bool RepeatWith => (int)(m_memberState & MemberState.RepeatWith) > 0;

			internal override bool StaticTree
			{
				get
				{
					return (int)(m_memberState & MemberState.StaticTree) > 0;
				}
				set
				{
					if (value)
					{
						m_memberState |= MemberState.StaticTree;
					}
					else
					{
						m_memberState &= ~MemberState.StaticTree;
					}
				}
			}

			internal override bool Hidden
			{
				get
				{
					if (m_memberInstance == null)
					{
						return false;
					}
					return m_memberInstance.Hidden;
				}
			}

			internal override double StartPos => m_memberInstance.StartPos;

			internal override double SizeValue => m_memberInstance.SizeValue;

			internal override double EndPos => m_memberInstance.EndPos;

			public override int Size => base.Size + 1 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_memberInstance);

			internal PageStructStaticMemberCell()
			{
			}

			internal PageStructStaticMemberCell(int sourceIndex, bool partialItem, bool createItem, TablixMember member, int memberDefIndex)
				: base(sourceIndex, partialItem, createItem, member, memberDefIndex)
			{
				if (member.KeepWithGroup == KeepWithGroup.After)
				{
					m_memberState = MemberState.Header;
				}
				else if (member.KeepWithGroup == KeepWithGroup.Before)
				{
					m_memberState = MemberState.Footer;
				}
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.MemberState:
						writer.Write((byte)m_memberState);
						break;
					case MemberName.MemberInstance:
						writer.Write(m_memberInstance);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.MemberState:
						m_memberState = (MemberState)reader.ReadByte();
						break;
					case MemberName.MemberInstance:
						m_memberInstance = (PageMemberCell)reader.ReadRIFObject();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageStructStaticMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberState, Token.Byte));
					list.Add(new MemberInfo(MemberName.MemberInstance, ObjectType.PageMemberCell));
					return new Declaration(ObjectType.PageStructStaticMemberCell, ObjectType.PageStructMemberCell, list);
				}
				return m_declaration;
			}

			internal void SetKeepWith(bool keepWith, bool repeatWith)
			{
				if (keepWith)
				{
					m_memberState |= MemberState.KeepWith;
				}
				if (repeatWith)
				{
					m_memberState |= MemberState.RepeatWith;
				}
			}

			internal override IDisposable PartialMemberInstance(out PageMemberCell memberCell)
			{
				memberCell = m_memberInstance;
				return null;
			}

			internal override void RemoveLastMemberCell()
			{
				m_memberInstance = null;
			}

			internal override void DisposeInstances()
			{
				m_memberInstance = null;
			}

			internal override void MergePageStructMembers(PageStructMemberCell mergeStructMember, TablixRegion region, MergeDetailRows detailRowsState)
			{
				PageStructStaticMemberCell pageStructStaticMemberCell = mergeStructMember as PageStructStaticMemberCell;
				if (pageStructStaticMemberCell != null)
				{
					MergePageMembers(m_memberInstance, pageStructStaticMemberCell.MemberInstance, region, detailRowsState);
				}
			}

			internal override MergeDetailRows AddPageMemberCell(PageMemberCell memberCell, TablixRegion region, PageContext pageContext)
			{
				m_memberInstance = memberCell;
				base.HasInstances = true;
				int num = 0;
				if (region == TablixRegion.ColumnHeader)
				{
					m_span = memberCell.ColSpan;
					num = memberCell.RowSpan;
				}
				else
				{
					m_span = memberCell.RowSpan;
					num = memberCell.ColSpan;
				}
				if (memberCell.Hidden)
				{
					base.OmittedList = false;
				}
				else
				{
					if (num == 0)
					{
						base.HasOmittedChildren = true;
					}
					if (num > 0 || StaticTree || memberCell.Label != null || memberCell.Children != null)
					{
						base.OmittedList = false;
					}
					if (memberCell.HasInnerPageName)
					{
						base.HasInnerPageName = true;
					}
				}
				return null;
			}

			internal override void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (m_memberInstance != null)
				{
					m_memberInstance.AlignCHToPageVertical(rowIndex, targetRowIndex, startInTablix, endInTablix, rowHeights, pageContext);
				}
			}

			internal override double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight)
			{
				if (m_memberInstance == null)
				{
					return 0.0;
				}
				return m_memberInstance.NormalizeDetailRowHeight(detailRows, rowIndex, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
			}

			internal override void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix)
			{
				if (m_memberInstance != null)
				{
					if (parentKeepRepeatWith && keepRow == 0 && RepeatWith)
					{
						keepRow = (Header ? 1 : 2);
					}
					m_memberInstance.UpdateRows(detailRows, rowIndex, parentKeepRepeatWith, keepRow, delete, startInTablix, endInTablix);
					m_span = m_memberInstance.RowSpan;
					if (!m_memberInstance.HasInnerPageName)
					{
						base.HasInnerPageName = false;
					}
				}
			}

			internal override void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix)
			{
				if (m_memberInstance != null && columnInfo[colIndex].PageHorizontalState != ColumnInfo.HorizontalState.AtRight && columnInfo[colIndex + m_memberInstance.ColSpan - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
				{
					m_memberInstance.UpdateColumns(columnInfo, colIndex, startInTablix, endInTablix);
				}
			}

			internal override void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (m_memberInstance != null)
				{
					m_memberInstance.AlignRHToPageHorizontal(detailRows, rowIndex, colIndex, targetColIndex, startInTablix, endInTablix, colWidths, isLTR, pageContext);
				}
			}

			internal override void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext)
			{
				if (m_memberInstance != null)
				{
					m_memberInstance.CalculateRHHorizontal(detailRows, rowIndex, colIndex, ref colWidths, pageContext);
				}
			}

			internal override int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (m_memberInstance == null)
				{
					return 0;
				}
				return m_memberInstance.AddToPageRHContent(colWidths, detailRows, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
			}

			internal override void CalculateCHHorizontal(List<SizeInfo> rowHeights, int rowIndex, int colIndex, ScalableList<ColumnInfo> columnInfo, PageContext pageContext, bool onPage)
			{
				if (m_memberInstance != null)
				{
					m_memberInstance.CalculateCHHorizontal(rowHeights, rowIndex, colIndex, columnInfo, pageContext, onPage);
				}
			}

			internal override int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (m_memberInstance == null)
				{
					return 0;
				}
				if (columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					return 0;
				}
				if (columnInfo[colIndex + m_memberInstance.ColSpan - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					return 0;
				}
				return m_memberInstance.AddToPageCHContent(rowHeights, columnInfo, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
			}

			internal override double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColBelow, bool update, ref double addedWidth)
			{
				if (m_memberInstance == null)
				{
					return 0.0;
				}
				if (detailCols[colIndex + m_memberInstance.ColSpan - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					return 0.0;
				}
				return m_memberInstance.NormalizeDetailColWidth(detailCols, colIndex, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update, ref addedWidth);
			}

			internal override void Reverse(PageContext pageContext)
			{
				if (m_memberInstance != null)
				{
					m_memberInstance.Reverse(pageContext);
				}
			}

			internal override bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (m_memberInstance == null)
				{
					return false;
				}
				return m_memberInstance.ResolveCHDuplicates(rowIndex, targetRowIndex, startInTablix, rowHeights, pageContext);
			}

			internal override bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext)
			{
				if (m_memberInstance == null)
				{
					return false;
				}
				return m_memberInstance.ResolveRHDuplicates(rowIndex, startInTablix, detailRows, pageContext);
			}
		}

		internal class PageStructDynamicMemberCell : PageStructMemberCell
		{
			private ScalableList<PageMemberCell> m_memberInstances;

			private static Declaration m_declaration = GetDeclaration();

			internal ScalableList<PageMemberCell> MemberInstances => m_memberInstances;

			internal override bool Hidden
			{
				get
				{
					if (m_memberInstances == null || m_memberInstances.Count == 0)
					{
						return false;
					}
					for (int i = 0; i < m_memberInstances.Count; i++)
					{
						if (!m_memberInstances[i].Hidden)
						{
							return false;
						}
					}
					return true;
				}
			}

			internal override double StartPos => m_memberInstances[0].StartPos;

			internal override double SizeValue => EndPos - StartPos;

			internal override double EndPos => m_memberInstances[m_memberInstances.Count - 1].EndPos;

			internal override int PartialRowSpan
			{
				get
				{
					if (m_memberInstances == null || m_memberInstances.Count == 0)
					{
						return 0;
					}
					int num = 0;
					for (int i = 0; i < m_memberInstances.Count - 1; i++)
					{
						num += m_memberInstances[i].RowSpan;
					}
					return num;
				}
			}

			public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_memberInstances);

			internal PageStructDynamicMemberCell()
			{
			}

			internal PageStructDynamicMemberCell(int sourceIndex, bool partialItem, bool createItem, TablixMember member, int memberDefIndex)
				: base(sourceIndex, partialItem, createItem, member, memberDefIndex)
			{
			}

			internal PageStructDynamicMemberCell(PageStructDynamicMemberCell copy, ScalableList<PageMemberCell> memberInstances, int span)
				: base(copy, span)
			{
				m_memberInstances = memberInstances;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.MemberInstances)
					{
						writer.Write(m_memberInstances);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.MemberInstances)
					{
						m_memberInstances = reader.ReadRIFObject<ScalableList<PageMemberCell>>();
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageStructDynamicMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberInstances, ObjectType.ScalableList, ObjectType.PageMemberCell));
					return new Declaration(ObjectType.PageStructDynamicMemberCell, ObjectType.PageStructMemberCell, list);
				}
				return m_declaration;
			}

			internal override IDisposable PartialMemberInstance(out PageMemberCell memberCell)
			{
				memberCell = null;
				if (m_memberInstances == null || m_memberInstances.Count == 0)
				{
					return null;
				}
				return m_memberInstances.GetAndPin(m_memberInstances.Count - 1, out memberCell);
			}

			internal override void RemoveLastMemberCell()
			{
				if (m_memberInstances != null && m_memberInstances.Count != 0)
				{
					m_memberInstances.RemoveAt(m_memberInstances.Count - 1);
				}
			}

			internal override void DisposeInstances()
			{
				if (m_memberInstances != null)
				{
					m_memberInstances.Dispose();
					m_memberInstances = null;
				}
			}

			internal override void MergePageStructMembers(PageStructMemberCell mergeStructMember, TablixRegion region, MergeDetailRows detailRowsState)
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = mergeStructMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell != null)
				{
					PageMemberCell item = null;
					IDisposable andPin = m_memberInstances.GetAndPin(0, out item);
					MergePageMembers(item, pageStructDynamicMemberCell.MemberInstances[0], region, detailRowsState);
					andPin.Dispose();
				}
			}

			internal override MergeDetailRows AddPageMemberCell(PageMemberCell memberCell, TablixRegion region, PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					m_memberInstances = new ScalableList<PageMemberCell>(0, pageContext.ScalabilityCache);
					base.HasInstances = true;
				}
				if (memberCell.Hidden)
				{
					PageMemberCell item = null;
					if (m_memberInstances.Count > 0)
					{
						using (m_memberInstances.GetAndPin(m_memberInstances.Count - 1, out item))
						{
							if (item != null && item.Hidden)
							{
								MergeDetailRows mergeDetailRows = null;
								mergeDetailRows = ((region != TablixRegion.RowHeader) ? new MergeDetailRows(item.ColSpan) : new MergeDetailRows(item.RowSpan));
								MergePageMembers(item, memberCell, region, mergeDetailRows);
								return mergeDetailRows;
							}
						}
					}
				}
				m_memberInstances.Add(memberCell);
				int num = 0;
				if (region == TablixRegion.ColumnHeader)
				{
					m_span += memberCell.ColSpan;
					num = memberCell.RowSpan;
				}
				else
				{
					m_span += memberCell.RowSpan;
					num = memberCell.ColSpan;
				}
				if (memberCell.Hidden)
				{
					base.OmittedList = false;
				}
				else
				{
					if (num == 0)
					{
						base.HasOmittedChildren = true;
					}
					if (num > 0 || memberCell.Label != null || memberCell.Children != null)
					{
						base.OmittedList = false;
					}
					if (memberCell.HasInnerPageName || memberCell.PageName != null)
					{
						base.HasInnerPageName = true;
					}
				}
				return null;
			}

			internal override void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					return;
				}
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						item.AlignCHToPageVertical(rowIndex, targetRowIndex, startInTablix, endInTablix, rowHeights, pageContext);
					}
				}
			}

			internal override double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight)
			{
				if (m_memberInstances == null)
				{
					return 0.0;
				}
				double num = 0.0;
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						num += item.NormalizeDetailRowHeight(detailRows, rowIndex, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
						rowIndex += item.RowSpan;
					}
				}
				return num;
			}

			internal override void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix)
			{
				if (m_memberInstances == null || m_memberInstances.Count == 0)
				{
					return;
				}
				m_span = 0;
				int num = -1;
				int num2 = -1;
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						item.UpdateRows(detailRows, rowIndex, parentKeepRepeatWith, 0, delete, startInTablix, endInTablix);
					}
					item = m_memberInstances[i];
					if (item.RowSpan == 0)
					{
						if (num < 0)
						{
							num = i;
						}
						num2 = i;
					}
					else if (num >= 0)
					{
						int num3 = num2 - num + 1;
						m_memberInstances.RemoveRange(num, num3);
						i -= num3;
						num = (num2 = -1);
					}
					rowIndex += item.RowSpan;
					m_span += item.RowSpan;
				}
				if (num >= 0)
				{
					if (base.PartialItem && !base.CreateItem)
					{
						num2--;
					}
					if (num2 >= num)
					{
						int count = num2 - num + 1;
						m_memberInstances.RemoveRange(num, count);
					}
				}
				if (!base.HasInnerPageName)
				{
					return;
				}
				for (int j = 0; j < m_memberInstances.Count; j++)
				{
					item = m_memberInstances[j];
					if (item.PageName != null || item.HasInnerPageName)
					{
						return;
					}
				}
				base.HasInnerPageName = false;
			}

			internal override void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					return;
				}
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
					{
						break;
					}
					using (m_memberInstances.GetAndPin(i, out item))
					{
						item.AlignRHToPageHorizontal(detailRows, rowIndex, colIndex, targetColIndex, startInTablix, endInTablix, colWidths, isLTR, pageContext);
						rowIndex += item.RowSpan;
					}
				}
			}

			internal override void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					return;
				}
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
					{
						break;
					}
					using (m_memberInstances.GetAndPin(i, out item))
					{
						item.CalculateRHHorizontal(detailRows, rowIndex, colIndex, ref colWidths, pageContext);
						rowIndex += item.RowSpan;
					}
				}
			}

			internal override int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (m_memberInstances == null)
				{
					return 0;
				}
				int num = 0;
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
					{
						break;
					}
					using (m_memberInstances.GetAndPin(i, out item))
					{
						num += item.AddToPageRHContent(colWidths, detailRows, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
						rowIndex += item.RowSpan;
					}
				}
				return num;
			}

			internal override void CalculateCHHorizontal(List<SizeInfo> rowHeights, int rowIndex, int colIndex, ScalableList<ColumnInfo> colWidths, PageContext pageContext, bool onPage)
			{
				if (m_memberInstances == null)
				{
					return;
				}
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						item.CalculateCHHorizontal(rowHeights, rowIndex, colIndex, colWidths, pageContext, onPage);
						colIndex += item.ColSpan;
					}
				}
			}

			internal override int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (m_memberInstances == null)
				{
					return 0;
				}
				int num = 0;
				int num2 = colIndex;
				int num3 = 0;
				RTLTextBoxes delayedTB = null;
				rplWriter?.EnterDelayedTBLevel(isLTR, ref delayedTB);
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						num3 = num2;
						num2 += item.ColSpan;
						if (columnInfo[num3].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							break;
						}
						if (columnInfo[num2 - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
						{
							continue;
						}
						num += item.AddToPageCHContent(rowHeights, columnInfo, rowIndex, num3, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
						goto IL_009c;
					}
					IL_009c:
					rplWriter?.RegisterCellTextBoxes(isLTR, delayedTB);
				}
				rplWriter?.LeaveDelayedTBLevel(isLTR, delayedTB, pageContext);
				return num;
			}

			internal override void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix)
			{
				if (m_memberInstances == null)
				{
					return;
				}
				int num = colIndex;
				int num2 = 0;
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						num2 = num;
						num += item.ColSpan;
						if (columnInfo[num2].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							return;
						}
						if (columnInfo[num - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
						{
							item.UpdateColumns(columnInfo, num2, startInTablix, endInTablix);
						}
					}
				}
			}

			internal override double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColBelow, bool update, ref double addedWidth)
			{
				if (m_memberInstances == null)
				{
					return 0.0;
				}
				double num = 0.0;
				int num2 = colIndex;
				int num3 = 0;
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					num3 = num2;
					num2 += m_memberInstances[i].ColSpan;
					if (detailCols[num2 - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						using (m_memberInstances.GetAndPin(i, out item))
						{
							num += item.NormalizeDetailColWidth(detailCols, num3, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update, ref addedWidth);
						}
					}
				}
				return num;
			}

			internal override void Reverse(PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					return;
				}
				ScalableList<PageMemberCell> scalableList = new ScalableList<PageMemberCell>(0, pageContext.ScalabilityCache);
				for (int num = m_memberInstances.Count - 1; num >= 0; num--)
				{
					scalableList.Add(m_memberInstances[num]);
				}
				m_memberInstances = scalableList;
				PageMemberCell item = null;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						item.Reverse(pageContext);
					}
				}
			}

			internal override PageStructDynamicMemberCell Split(int colsBeforeRowHeaders, PageContext pageContext)
			{
				if (colsBeforeRowHeaders == 0 || m_memberInstances == null)
				{
					return null;
				}
				ScalableList<PageMemberCell> scalableList = new ScalableList<PageMemberCell>(0, pageContext.ScalabilityCache);
				int num = 0;
				int num2 = 0;
				PageMemberCell pageMemberCell = null;
				while (num2 < colsBeforeRowHeaders)
				{
					pageMemberCell = m_memberInstances[num];
					num2 += pageMemberCell.ColSpan;
					scalableList.Add(pageMemberCell);
					num++;
				}
				if (num2 == m_span)
				{
					return null;
				}
				m_span -= num2;
				m_memberInstances.RemoveRange(0, num);
				return new PageStructDynamicMemberCell(this, scalableList, num2);
			}

			internal override bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					return false;
				}
				PageMemberCell item = null;
				bool flag = false;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						flag |= item.ResolveCHDuplicates(rowIndex, targetRowIndex, startInTablix, rowHeights, pageContext);
					}
				}
				return flag;
			}

			internal override bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext)
			{
				if (m_memberInstances == null)
				{
					return false;
				}
				PageMemberCell item = null;
				bool flag = false;
				for (int i = 0; i < m_memberInstances.Count; i++)
				{
					using (m_memberInstances.GetAndPin(i, out item))
					{
						flag |= item.ResolveRHDuplicates(rowIndex, startInTablix, detailRows, pageContext);
						rowIndex += item.RowSpan;
					}
				}
				return flag;
			}
		}

		internal abstract class PageTalixCell : IStorable, IPersistable
		{
			internal PageItem m_cellItem;

			internal int m_colSpan;

			private static Declaration m_declaration = GetDeclaration();

			internal int ColSpan
			{
				get
				{
					return m_colSpan;
				}
				set
				{
					m_colSpan = value;
				}
			}

			internal PageItem CellItem
			{
				get
				{
					return m_cellItem;
				}
				set
				{
					m_cellItem = value;
				}
			}

			public virtual int Size => 4 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_cellItem);

			internal PageTalixCell()
			{
			}

			internal PageTalixCell(PageItem item, int colSpan)
			{
				m_cellItem = item;
				m_colSpan = colSpan;
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						writer.Write(m_colSpan);
						break;
					case MemberName.CellItem:
						writer.Write(m_cellItem);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						m_colSpan = reader.ReadInt32();
						break;
					case MemberName.CellItem:
						m_cellItem = (PageItem)reader.ReadRIFObject();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.PageTablixCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.CellItem, ObjectType.PageItem));
					return new Declaration(ObjectType.PageTablixCell, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal class PageDetailCell : PageTalixCell
		{
			[Flags]
			private enum PageDetailCellState : byte
			{
				Clear = 0x0,
				Hidden = 0x1,
				Release = 0x2,
				ContentOnPage = 0x4,
				KeepTogether = 0x8
			}

			internal double m_sourceWidth;

			private PageDetailCellState m_state;

			private static Declaration m_declaration = GetDeclaration();

			internal double SourceWidth
			{
				get
				{
					return m_sourceWidth;
				}
				set
				{
					m_sourceWidth = value;
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(m_state & PageDetailCellState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= PageDetailCellState.Hidden;
					}
					else
					{
						m_state &= ~PageDetailCellState.Hidden;
					}
				}
			}

			internal bool Release
			{
				get
				{
					return (int)(m_state & PageDetailCellState.Release) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= PageDetailCellState.Release;
					}
					else
					{
						m_state &= ~PageDetailCellState.Release;
					}
				}
			}

			internal bool ContentOnPage
			{
				get
				{
					return (int)(m_state & PageDetailCellState.ContentOnPage) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= PageDetailCellState.ContentOnPage;
					}
					else
					{
						m_state &= ~PageDetailCellState.ContentOnPage;
					}
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(m_state & PageDetailCellState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= PageDetailCellState.KeepTogether;
					}
					else
					{
						m_state &= ~PageDetailCellState.KeepTogether;
					}
				}
			}

			public override int Size => base.Size + 1 + 8;

			internal PageDetailCell()
			{
			}

			internal PageDetailCell(PageItem item, double sourceWidth)
				: base(item, 1)
			{
				m_sourceWidth = sourceWidth;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write((byte)m_state);
						break;
					case MemberName.Width:
						writer.Write(m_sourceWidth);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.State:
						m_state = (PageDetailCellState)reader.ReadByte();
						break;
					case MemberName.Width:
						m_sourceWidth = reader.ReadDouble();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageDetailCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.Width, Token.Double));
					return new Declaration(ObjectType.PageDetailCell, ObjectType.PageTablixCell, list);
				}
				return m_declaration;
			}
		}

		internal class PageCornerCell : PageTalixCell
		{
			internal double m_sourceWidth;

			internal int m_rowSpan;

			internal bool m_contentOnPage;

			private static Declaration m_declaration = GetDeclaration();

			internal int RowSpan => m_rowSpan;

			internal bool ContentOnPage => m_contentOnPage;

			public override int Size => base.Size + 1 + 8 + 4;

			internal PageCornerCell()
			{
			}

			internal PageCornerCell(PageItem item, int rowSpan, int colSpan, double sourceWidth, double sourceHeight)
				: base(item, colSpan)
			{
				m_sourceWidth = sourceWidth;
				m_rowSpan = rowSpan;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ContentOnPage:
						writer.Write(m_contentOnPage);
						break;
					case MemberName.RowSpan:
						writer.Write(m_rowSpan);
						break;
					case MemberName.Width:
						writer.Write(m_sourceWidth);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ContentOnPage:
						m_contentOnPage = reader.ReadBoolean();
						break;
					case MemberName.RowSpan:
						m_rowSpan = reader.ReadInt32();
						break;
					case MemberName.Width:
						m_sourceWidth = reader.ReadDouble();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageCornerCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ContentOnPage, Token.Boolean));
					list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.Width, Token.Double));
					return new Declaration(ObjectType.PageCornerCell, ObjectType.PageTablixCell, list);
				}
				return m_declaration;
			}

			internal void CalculateHorizontal(int colIndex, ref List<SizeInfo> colWidths, PageContext context)
			{
				if (m_cellItem != null)
				{
					bool anyAncestorHasKT = false;
					PageContext pageContext = new PageContext(context);
					m_cellItem.CalculateHorizontal(pageContext, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true, m_sourceWidth);
					double size = Math.Max(m_sourceWidth, m_cellItem.ItemPageSizes.Width);
					UpdateSizes(colIndex, m_colSpan, size, ref colWidths);
				}
			}

			internal void AlignToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (m_cellItem != null && (m_cellItem.KTVIsUnresolved || m_cellItem.NeedResolve))
				{
					double num = 0.0;
					for (int i = rowIndex; i < targetRowIndex; i++)
					{
						num += rowHeights[i].SizeValue;
					}
					double topInParentSystem = Math.Max(0.0, startInTablix - rowHeights[rowIndex].StartPos);
					double bottomInParentSystem = endInTablix - rowHeights[rowIndex].StartPos;
					PageContext pageContext2 = pageContext;
					if (!pageContext.IgnorePageBreaks)
					{
						pageContext2 = new PageContext(pageContext);
						pageContext2.IgnorePageBreaks = true;
						pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
					}
					m_cellItem.ResolveVertical(pageContext2, topInParentSystem, bottomInParentSystem, null, resolveItem: true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
					num = m_cellItem.ItemPageSizes.Bottom - num;
					if (num > 0.0)
					{
						SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + m_rowSpan - targetRowIndex, num);
					}
				}
			}

			internal bool ResolveDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (m_cellItem == null)
				{
					return false;
				}
				double topInParentSystem = Math.Max(0.0, startInTablix - rowHeights[rowIndex].StartPos);
				bool flag = m_cellItem.ResolveDuplicates(pageContext, topInParentSystem, null, recalculate: false);
				if (flag)
				{
					double num = 0.0;
					for (int i = rowIndex; i < targetRowIndex; i++)
					{
						num += rowHeights[i].SizeValue;
					}
					num = m_cellItem.ItemPageSizes.Bottom - num;
					if (num > 0.0)
					{
						SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + m_rowSpan - targetRowIndex, num);
					}
				}
				return flag;
			}

			internal void AlignToPageHorizontal(int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (m_cellItem == null || (!m_cellItem.KTHIsUnresolved && !m_cellItem.NeedResolve))
				{
					return;
				}
				double num = 0.0;
				double num2 = 0.0;
				if (isLTR)
				{
					num2 = colWidths[colIndex].StartPos;
					for (int i = colIndex; i < targetColIndex; i++)
					{
						num += colWidths[i].SizeValue;
					}
				}
				else
				{
					num2 = colWidths[colIndex + m_colSpan - 1].StartPos;
					for (int num3 = colIndex + m_colSpan - 1; num3 > targetColIndex; num3--)
					{
						num += colWidths[num3].SizeValue;
					}
				}
				double leftInParentSystem = Math.Max(0.0, startInTablix - num2);
				double rightInParentSystem = endInTablix - num2;
				m_cellItem.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, resolveItem: true);
				num = m_cellItem.ItemPageSizes.Right - num;
				if (num > 0.0)
				{
					if (isLTR)
					{
						SizeInfo.UpdateSize(colWidths[targetColIndex], colIndex + m_colSpan - targetColIndex, num);
					}
					else
					{
						SizeInfo.UpdateSize(colWidths[targetColIndex], targetColIndex - colIndex + 1, num);
					}
				}
			}

			internal void AddToPageContent(List<SizeInfo> rowHeights, List<SizeInfo> colWidths, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
			{
				if (m_cellItem != null)
				{
					double startPos = rowHeights[rowIndex].StartPos;
					double pageTop2 = Math.Max(0.0, pageTop - startPos);
					double pageBottom2 = pageBottom - startPos;
					double num = 0.0;
					num = ((!isLTR) ? colWidths[colIndex + m_colSpan - 1].StartPos : colWidths[colIndex].StartPos);
					double pageLeft2 = Math.Max(0.0, pageLeft - num);
					double pageRight2 = pageRight - num;
					m_cellItem.ItemPageSizes.AdjustHeightTo(rowHeights[rowIndex + m_rowSpan - 1].EndPos - startPos);
					m_contentOnPage = m_cellItem.AddToPage(rplWriter, pageContext, pageLeft2, pageTop2, pageRight2, pageBottom2, repeatState);
					if (m_contentOnPage)
					{
						m_contentOnPage = m_cellItem.ContentOnPage;
					}
				}
			}
		}

		internal class RowInfo : IStorable, IPersistable
		{
			[Flags]
			private enum RowInfoState : ushort
			{
				Clear = 0x0,
				PageBreakAtStart = 0x1,
				PageBreakAtEnd = 0x2,
				SpanPagesRow = 0x4,
				IgnoreInnerPageBreaks = 0x8,
				IgnoreKeepWith = 0x10,
				Hidden = 0x20,
				RepeatWith = 0x40,
				RepeatOnPage = 0x80,
				ResolvedRow = 0x100,
				KeepTogether = 0x200,
				ContentFullyCreated = 0x400
			}

			internal enum VerticalState : byte
			{
				Unknown,
				Normal,
				Above,
				Below,
				Repeat,
				TopOfNextPage
			}

			private double m_normalizeRowHeight;

			private double m_sourceHeight;

			private ScalableList<PageDetailCell> m_detailCells;

			private RowInfoState m_rowInfoState;

			private VerticalState m_verticalState;

			private double m_height;

			private double m_top = double.MinValue;

			private long m_offset = -1L;

			[StaticReference]
			private RPLTablixRow m_rplRow;

			private PageBreakProperties m_pageBreakPropertiesAtStart;

			private PageBreakProperties m_pageBreakPropertiesAtEnd;

			private static Declaration m_declaration = GetDeclaration();

			internal double Height
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

			internal double Top
			{
				get
				{
					return m_top;
				}
				set
				{
					m_top = value;
				}
			}

			internal double Bottom => m_top + m_height;

			internal double NormalizeRowHeight => m_normalizeRowHeight;

			internal bool PageBreaksAtStart
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.PageBreakAtStart) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.PageBreakAtStart;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.PageBreakAtStart;
					}
				}
			}

			internal bool PageBreaksAtEnd
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.PageBreakAtEnd) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.PageBreakAtEnd;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.PageBreakAtEnd;
					}
				}
			}

			internal bool IgnoreKeepWith
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.IgnoreKeepWith) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.IgnoreKeepWith;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.IgnoreKeepWith;
					}
				}
			}

			internal bool RepeatOnPage
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.RepeatOnPage) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.RepeatOnPage;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.RepeatOnPage;
					}
				}
			}

			internal bool RepeatWith
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.RepeatWith) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.RepeatWith;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.RepeatWith;
					}
				}
			}

			internal bool SpanPagesRow
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.SpanPagesRow) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.SpanPagesRow;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.SpanPagesRow;
					}
				}
			}

			internal bool ContentFullyCreated
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.ContentFullyCreated) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.ContentFullyCreated;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.ContentFullyCreated;
					}
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.Hidden;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.Hidden;
					}
				}
			}

			internal bool ResolvedRow
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.ResolvedRow) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.ResolvedRow;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.ResolvedRow;
					}
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(m_rowInfoState & RowInfoState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						m_rowInfoState |= RowInfoState.KeepTogether;
					}
					else
					{
						m_rowInfoState &= ~RowInfoState.KeepTogether;
					}
				}
			}

			internal VerticalState PageVerticalState
			{
				get
				{
					return m_verticalState;
				}
				set
				{
					m_verticalState = value;
				}
			}

			internal long Offset => m_offset;

			internal RPLTablixRow RPLTablixRow
			{
				get
				{
					return m_rplRow;
				}
				set
				{
					m_rplRow = value;
				}
			}

			internal ScalableList<PageDetailCell> Cells => m_detailCells;

			public PageBreakProperties PageBreakPropertiesAtStart
			{
				get
				{
					return m_pageBreakPropertiesAtStart;
				}
				set
				{
					m_pageBreakPropertiesAtStart = value;
				}
			}

			public PageBreakProperties PageBreakPropertiesAtEnd
			{
				get
				{
					return m_pageBreakPropertiesAtEnd;
				}
				set
				{
					m_pageBreakPropertiesAtEnd = value;
				}
			}

			public int Size => 43 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_detailCells) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageBreakPropertiesAtStart) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageBreakPropertiesAtEnd);

			internal RowInfo()
			{
			}

			internal RowInfo(double sourceHeight)
			{
				m_sourceHeight = sourceHeight;
				m_normalizeRowHeight = sourceHeight;
				m_height = sourceHeight;
				ContentFullyCreated = true;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.RowHeight:
						writer.Write(m_normalizeRowHeight);
						break;
					case MemberName.SourceHeight:
						writer.Write(m_sourceHeight);
						break;
					case MemberName.DetailCells:
						writer.Write(m_detailCells);
						break;
					case MemberName.State:
						writer.Write((ushort)m_rowInfoState);
						break;
					case MemberName.VerticalState:
						writer.Write((byte)m_verticalState);
						break;
					case MemberName.Height:
						writer.Write(m_height);
						break;
					case MemberName.Top:
						writer.Write(m_top);
						break;
					case MemberName.Offset:
						writer.Write(m_offset);
						break;
					case MemberName.RPLTablixRow:
					{
						int value = scalabilityCache.StoreStaticReference(m_rplRow);
						writer.Write(value);
						break;
					}
					case MemberName.PageBreakPropertiesAtStart:
						writer.Write(m_pageBreakPropertiesAtStart);
						break;
					case MemberName.PageBreakPropertiesAtEnd:
						writer.Write(m_pageBreakPropertiesAtEnd);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.RowHeight:
						m_normalizeRowHeight = reader.ReadDouble();
						break;
					case MemberName.SourceHeight:
						m_sourceHeight = reader.ReadDouble();
						break;
					case MemberName.DetailCells:
						m_detailCells = reader.ReadRIFObject<ScalableList<PageDetailCell>>();
						break;
					case MemberName.State:
						m_rowInfoState = (RowInfoState)reader.ReadUInt16();
						break;
					case MemberName.VerticalState:
						m_verticalState = (VerticalState)reader.ReadByte();
						break;
					case MemberName.Height:
						m_height = reader.ReadDouble();
						break;
					case MemberName.Top:
						m_top = reader.ReadDouble();
						break;
					case MemberName.Offset:
						m_offset = reader.ReadInt64();
						break;
					case MemberName.RPLTablixRow:
					{
						int id = reader.ReadInt32();
						m_rplRow = (RPLTablixRow)scalabilityCache.FetchStaticReference(id);
						break;
					}
					case MemberName.PageBreakPropertiesAtStart:
						m_pageBreakPropertiesAtStart = (PageBreakProperties)reader.ReadRIFObject();
						break;
					case MemberName.PageBreakPropertiesAtEnd:
						m_pageBreakPropertiesAtEnd = (PageBreakProperties)reader.ReadRIFObject();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.RowInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.RowHeight, Token.Double));
					list.Add(new MemberInfo(MemberName.SourceHeight, Token.Double));
					list.Add(new MemberInfo(MemberName.DetailCells, ObjectType.ScalableList, ObjectType.PageDetailCell));
					list.Add(new MemberInfo(MemberName.State, Token.UInt16));
					list.Add(new MemberInfo(MemberName.VerticalState, Token.Byte));
					list.Add(new MemberInfo(MemberName.Height, Token.Double));
					list.Add(new MemberInfo(MemberName.Top, Token.Double));
					list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
					list.Add(new MemberInfo(MemberName.RPLTablixRow, Token.Int32));
					list.Add(new MemberInfo(MemberName.PageBreakPropertiesAtStart, ObjectType.PageBreakProperties));
					list.Add(new MemberInfo(MemberName.PageBreakPropertiesAtEnd, ObjectType.PageBreakProperties));
					return new Declaration(ObjectType.RowInfo, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal void DisposeDetailCells()
			{
				if (m_detailCells != null)
				{
					m_detailCells.Dispose();
					m_detailCells = null;
				}
			}

			internal void ReverseDetailCells(PageContext pageContext)
			{
				if (m_detailCells != null && m_detailCells.Count != 0)
				{
					ScalableList<PageDetailCell> scalableList = new ScalableList<PageDetailCell>(0, pageContext.ScalabilityCache);
					for (int num = m_detailCells.Count - 1; num >= 0; num--)
					{
						scalableList.Add(m_detailCells[num]);
					}
					m_detailCells.Dispose();
					m_detailCells = scalableList;
				}
			}

			internal IDisposable UpdateLastDetailCell(double cellColDefWidth, out PageDetailCell lastCell)
			{
				lastCell = null;
				if (m_detailCells == null || m_detailCells.Count == 0)
				{
					return null;
				}
				IDisposable andPin = m_detailCells.GetAndPin(m_detailCells.Count - 1, out lastCell);
				lastCell.ColSpan++;
				lastCell.SourceWidth += cellColDefWidth;
				return andPin;
			}

			internal void AddDetailCell(PageDetailCell detailCell, PageContext pageContext)
			{
				if (m_detailCells == null)
				{
					m_detailCells = new ScalableList<PageDetailCell>(0, pageContext.ScalabilityCache);
				}
				m_detailCells.Add(detailCell);
			}

			internal void CalculateVerticalLastDetailCell(PageContext context, bool firstTouch, bool delayCalc)
			{
				if (Hidden || m_detailCells == null || m_detailCells.Count == 0)
				{
					return;
				}
				PageDetailCell item = null;
				using (m_detailCells.GetAndPin(m_detailCells.Count - 1, out item))
				{
					if (item.CellItem == null || item.Hidden)
					{
						return;
					}
					TextBox textBox = item.CellItem as TextBox;
					if (textBox != null)
					{
						if (item.SourceWidth > 0.0)
						{
							textBox.ItemPageSizes.Width = item.SourceWidth;
						}
						if (firstTouch)
						{
							if (delayCalc)
							{
								textBox.CalcSizeState = TextBox.CalcSize.Delay;
							}
							goto IL_00b0;
						}
						if (textBox.CalcSizeState == TextBox.CalcSize.Delay)
						{
							textBox.CalcSizeState = TextBox.CalcSize.LateCalc;
							goto IL_00b0;
						}
					}
					else if (firstTouch)
					{
						goto IL_00b0;
					}
					goto end_IL_003c;
					IL_00b0:
					bool anyAncestorHasKT = false;
					item.CellItem.CalculateVertical(context, 0.0, context.ColumnHeight, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true, item.SourceWidth);
					if (item.CellItem.ItemPageSizes.Height > m_normalizeRowHeight)
					{
						m_normalizeRowHeight = item.CellItem.ItemPageSizes.Height;
						m_height = m_normalizeRowHeight;
					}
					CheckCellSpanPages(item, context.ColumnHeight);
					if (context.IgnorePageBreaks)
					{
						m_rowInfoState |= RowInfoState.IgnoreInnerPageBreaks;
					}
					end_IL_003c:;
				}
			}

			internal void UpdatePinnedRow()
			{
				SpanPagesRow = false;
				ContentFullyCreated = true;
				m_normalizeRowHeight = m_sourceHeight;
				m_height = m_sourceHeight;
				ResolvedRow = true;
			}

			internal void ResetRowHeight()
			{
				m_height = m_normalizeRowHeight;
			}

			internal void UpdateVerticalDetailCell(PageContext context, double startInTablix, double endInTablix, ref int detailCellIndex)
			{
				if (Hidden || m_detailCells == null || m_detailCells.Count == 0)
				{
					return;
				}
				PageDetailCell item = null;
				using (m_detailCells.GetAndPin(detailCellIndex, out item))
				{
					detailCellIndex++;
					if (item.CellItem == null || item.Hidden)
					{
						return;
					}
					bool anyAncestorHasKT = false;
					double topInParentSystem = startInTablix - m_top;
					double num = endInTablix - m_top;
					PageContext pageContext = new PageContext(context, cacheNonSharedProps: true);
					if ((int)(m_rowInfoState & RowInfoState.IgnoreInnerPageBreaks) > 0)
					{
						pageContext.IgnorePageBreaks = true;
						if (!context.IgnorePageBreaks)
						{
							pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.Unknown;
						}
					}
					item.CellItem.CalculateVertical(context, topInParentSystem, num, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: false, item.SourceWidth);
					if (item.CellItem.ItemPageSizes.Bottom > m_normalizeRowHeight)
					{
						m_normalizeRowHeight = item.CellItem.ItemPageSizes.Bottom;
						m_height = m_normalizeRowHeight;
					}
					CheckCellSpanPages(item, num);
				}
			}

			private void CheckCellSpanPages(PageDetailCell lastCell, double endInRow)
			{
				if (lastCell.CellItem.PBAreUnresolved)
				{
					m_rowInfoState |= RowInfoState.SpanPagesRow;
				}
				else if (new RoundedDouble(m_height) > endInRow)
				{
					m_rowInfoState |= RowInfoState.SpanPagesRow;
					ContentFullyCreated = false;
				}
				if (!lastCell.CellItem.FullyCreated)
				{
					ContentFullyCreated = false;
				}
			}

			internal void CalculateHorizontal(ScalableList<ColumnInfo> columnInfo, PageContext context)
			{
				if (PageVerticalState != VerticalState.Normal || Hidden || m_detailCells == null || m_detailCells.Count == 0)
				{
					return;
				}
				PageDetailCell item = null;
				PageContext pageContext = new PageContext(context);
				int num = 0;
				if (RepeatOnPage)
				{
					pageContext.ResetHorizontal = true;
				}
				else
				{
					pageContext.ResetHorizontal = context.ResetHorizontal;
				}
				for (int i = 0; i < m_detailCells.Count; i++)
				{
					using (m_detailCells.GetAndPin(i, out item))
					{
						if (!item.Hidden)
						{
							if (item.CellItem == null)
							{
								UpdateSizes(num, item.ColSpan, item.SourceWidth, unresolved: false, keepTogether: false, split: false, columnInfo);
							}
							else
							{
								bool anyAncestorHasKT = false;
								item.CellItem.CalculateHorizontal(pageContext, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true, item.SourceWidth);
								double size = Math.Max(item.SourceWidth, item.CellItem.ItemPageSizes.Width);
								bool unresolved = item.CellItem.KTHIsUnresolved || item.CellItem.NeedResolve;
								UpdateSizes(num, item.ColSpan, size, unresolved, item.KeepTogether, split: false, columnInfo);
							}
						}
						else
						{
							UpdateHidden(num, item.ColSpan, columnInfo);
						}
					}
					num += item.ColSpan;
				}
			}

			internal bool ResolveVertical(double startInTablix, double endInTablix, PageContext context)
			{
				if (m_detailCells == null || Hidden)
				{
					return false;
				}
				if (ResolvedRow)
				{
					ResolvedRow = false;
					return false;
				}
				PageDetailCell item = null;
				PageItem pageItem = null;
				double topInParentSystem = Math.Max(0.0, startInTablix - m_top);
				double num = endInTablix - m_top;
				PageContext pageContext = context;
				if ((int)(m_rowInfoState & RowInfoState.IgnoreInnerPageBreaks) > 0)
				{
					pageContext = new PageContext(context);
					pageContext.IgnorePageBreaks = true;
					if (!context.IgnorePageBreaks)
					{
						pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.Unknown;
					}
				}
				SpanPagesRow = false;
				ContentFullyCreated = true;
				double num2 = m_sourceHeight;
				for (int i = 0; i < m_detailCells.Count; i++)
				{
					using (m_detailCells.GetAndPin(i, out item))
					{
						if (item.Hidden || item.Release)
						{
							continue;
						}
						pageItem = item.CellItem;
						if (pageItem != null)
						{
							if (pageItem.KTVIsUnresolved || pageItem.PBAreUnresolved || pageItem.NeedResolve)
							{
								pageItem.ResolveVertical(context, topInParentSystem, num, null, resolveItem: true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
							}
							if (pageItem.ItemPageSizes.Bottom > num2)
							{
								num2 = pageItem.ItemPageSizes.Bottom;
							}
							if (pageItem.PBAreUnresolved)
							{
								m_rowInfoState |= RowInfoState.SpanPagesRow;
							}
							if (!pageItem.FullyCreated)
							{
								ContentFullyCreated = false;
							}
						}
					}
				}
				if (!SpanPagesRow && new RoundedDouble(num2) > num)
				{
					m_rowInfoState |= RowInfoState.SpanPagesRow;
				}
				if (new RoundedDouble(m_normalizeRowHeight - num2) == 0.0)
				{
					return false;
				}
				m_normalizeRowHeight = num2;
				m_height = m_normalizeRowHeight;
				return true;
			}

			internal void ResolveHorizontal(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix, PageContext pageContext)
			{
				if (PageVerticalState != VerticalState.Normal || Hidden || m_detailCells == null || m_detailCells.Count == 0)
				{
					return;
				}
				int num = 0;
				int num2 = 0;
				PageDetailCell pageDetailCell = null;
				PageItem pageItem = null;
				PageContext pageContext2 = new PageContext(pageContext);
				double num3 = 0.0;
				double num4 = 0.0;
				double num5 = 0.0;
				bool flag = false;
				ColumnInfo item = null;
				for (int i = 0; i < m_detailCells.Count; i++)
				{
					pageDetailCell = m_detailCells[i];
					num2 = num + pageDetailCell.ColSpan;
					if (!pageDetailCell.Hidden && num <= colIndex && num2 > colIndex)
					{
						item = columnInfo[num];
						num5 = columnInfo[num2 - 1].Right - item.Left;
						num3 = Math.Max(0.0, startInTablix - item.Left);
						num4 = endInTablix - item.Left;
						using (m_detailCells.GetAndPin(i, out pageDetailCell))
						{
							pageItem = pageDetailCell.CellItem;
							if (pageItem != null && (pageItem.KTHIsUnresolved || pageItem.NeedResolve))
							{
								pageItem.ResolveHorizontal(pageContext2, num3, num4, null, resolveItem: true);
								double num6 = pageItem.ItemPageSizes.Right - num5;
								if (num6 > 0.0)
								{
									int num7 = num2 - 1;
									while (num7 > colIndex && columnInfo[num7].Hidden)
									{
										num7--;
									}
									using (columnInfo.GetAndPin(num7, out item))
									{
										item.SizeValue += num6;
									}
									num7++;
									if (num7 < columnInfo.Count)
									{
										using (columnInfo.GetAndPin(num7, out item))
										{
											item.PageHorizontalState = ColumnInfo.HorizontalState.Unknown;
										}
									}
								}
								if (pageItem.KTHIsUnresolved || pageItem.NeedResolve)
								{
									flag = true;
								}
							}
						}
					}
					num = num2;
				}
				if (flag)
				{
					using (columnInfo.GetAndPin(colIndex, out item))
					{
						item.Unresolved = flag;
					}
				}
			}

			internal int AddToPageContent(ScalableList<ColumnInfo> columnInfo, out int colsOnPage, bool isLTR, bool pinnedToParentCell, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
			{
				colsOnPage = 0;
				if (PageVerticalState != VerticalState.Normal)
				{
					return 0;
				}
				if (m_detailCells == null || m_detailCells.Count == 0)
				{
					return 1;
				}
				PageDetailCell item = null;
				int i = 0;
				int num = 0;
				double pageTop2 = Math.Max(0.0, pageTop - m_top);
				double num2 = pageBottom - m_top;
				double num3 = 0.0;
				double num4 = 0.0;
				double num5 = 0.0;
				RTLTextBoxes delayedTB = null;
				RepeatState repeatState2 = repeatState;
				if (RepeatWith && !SpanPagesRow)
				{
					repeatState2 |= RepeatState.Vertical;
				}
				rplWriter?.EnterDelayedTBLevel(isLTR, ref delayedTB);
				for (int j = 0; j < m_detailCells.Count; j++)
				{
					using (m_detailCells.GetAndPin(j, out item))
					{
						num = i + item.ColSpan - 1;
						if (columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							break;
						}
						if (columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
						{
							i = num + 1;
							continue;
						}
						PageItem cellItem = item.CellItem;
						if (cellItem != null)
						{
							num5 = columnInfo[i].Left;
							num3 = Math.Max(0.0, pageLeft - num5);
							num4 = pageRight - num5;
							if (pinnedToParentCell)
							{
								cellItem.ItemPageSizes.AdjustHeightTo(m_height);
							}
							item.ContentOnPage = cellItem.AddToPage(rplWriter, pageContext, num3, pageTop2, num4, num2, repeatState2);
							if (item.ContentOnPage)
							{
								item.ContentOnPage = cellItem.ContentOnPage;
							}
							if (item.ContentOnPage)
							{
								rplWriter?.RegisterCellTextBoxes(isLTR, delayedTB);
							}
							if (repeatState2 == RepeatState.None && num2 >= m_normalizeRowHeight && cellItem.Release(num2, num4))
							{
								item.Release = true;
							}
						}
						if (!item.Hidden && !Hidden)
						{
							ColumnInfo columnInfo2 = null;
							for (; i <= num; i++)
							{
								columnInfo2 = columnInfo[i];
								if (columnInfo2.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
								{
									break;
								}
							}
							colsOnPage += num - i + 1;
							for (; i <= num; i++)
							{
								columnInfo2 = columnInfo[i];
								if (columnInfo2.Hidden)
								{
									colsOnPage--;
								}
								else if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
								{
									colsOnPage -= num - i + 1;
									i = num;
								}
							}
						}
						i = num + 1;
						continue;
					}
				}
				rplWriter?.LeaveDelayedTBLevel(isLTR, delayedTB, pageContext);
				if (Hidden)
				{
					return 0;
				}
				return 1;
			}

			internal int AddToPage(ScalableList<ColumnInfo> columnInfo, RPLWriter rplWriter, int pageRowIndex, int colsBeforeRH, int headerRowCols)
			{
				if (PageVerticalState != VerticalState.Normal || Hidden)
				{
					return 0;
				}
				if (m_detailCells == null || m_detailCells.Count == 0)
				{
					return 1;
				}
				List<RPLTablixCell> list = null;
				PageDetailCell pageDetailCell = null;
				PageItem pageItem = null;
				int i = 0;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					m_offset = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)18);
					binaryWriter.Write(pageRowIndex);
				}
				else
				{
					list = new List<RPLTablixCell>();
					m_rplRow = new RPLTablixRow(list);
				}
				for (int j = 0; j < m_detailCells.Count; j++)
				{
					pageDetailCell = m_detailCells[j];
					num = i + pageDetailCell.ColSpan - 1;
					if (columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
					{
						break;
					}
					if (columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
					{
						i = num + 1;
						continue;
					}
					double num4 = 0.0;
					for (; i <= num && columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft; i++)
					{
						num4 += columnInfo[i].SizeValue;
					}
					num3 = num - i + 1;
					bool flag = false;
					for (; i <= num; i++)
					{
						if (columnInfo[i].Hidden)
						{
							num3--;
						}
						else if (columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							num3 -= num - i + 1;
							i = num;
							flag = true;
						}
					}
					if (num2 == colsBeforeRH)
					{
						num2 += headerRowCols;
					}
					if (!pageDetailCell.Hidden)
					{
						pageItem = pageDetailCell.CellItem;
						double num5 = 0.0;
						if (pageItem != null && pageDetailCell.ContentOnPage)
						{
							num5 = pageItem.ItemPageSizes.Left - num4;
							if (flag && !(pageItem is TextBox))
							{
								flag = false;
							}
						}
						if (binaryWriter != null)
						{
							binaryWriter.Write((byte)13);
							if (pageItem != null && pageDetailCell.ContentOnPage)
							{
								binaryWriter.Write((byte)4);
								binaryWriter.Write(pageItem.Offset);
								if (pageItem.RplItemState > 0)
								{
									binaryWriter.Write((byte)13);
									binaryWriter.Write(pageItem.RplItemState);
								}
								if (pageItem.ItemPageSizes.Top != 0.0)
								{
									binaryWriter.Write((byte)0);
									binaryWriter.Write((float)pageItem.ItemPageSizes.Top);
								}
								if (num5 != 0.0)
								{
									binaryWriter.Write((byte)1);
									binaryWriter.Write((float)num5);
								}
								if (flag && pageItem.ItemPageSizes.Width != 0.0)
								{
									binaryWriter.Write((byte)2);
									binaryWriter.Write((float)pageItem.ItemPageSizes.Width);
								}
							}
							if (num3 > 1)
							{
								binaryWriter.Write((byte)5);
								binaryWriter.Write(num3);
							}
							binaryWriter.Write((byte)8);
							binaryWriter.Write(num2);
							binaryWriter.Write(byte.MaxValue);
						}
						else
						{
							RPLTablixCell rPLTablixCell = null;
							if (pageItem != null && pageDetailCell.ContentOnPage)
							{
								rPLTablixCell = new RPLTablixCell(pageItem.RPLElement as RPLItem, pageItem.RplItemState);
								if (pageItem.ItemPageSizes.Top != 0.0)
								{
									rPLTablixCell.ContentSizes = new RPLSizes();
									rPLTablixCell.ContentSizes.Top = (float)pageItem.ItemPageSizes.Top;
								}
								if (num5 != 0.0)
								{
									if (rPLTablixCell.ContentSizes == null)
									{
										rPLTablixCell.ContentSizes = new RPLSizes();
									}
									rPLTablixCell.ContentSizes.Left = (float)num5;
								}
								if (flag && pageItem.ItemPageSizes.Width != 0.0)
								{
									if (rPLTablixCell.ContentSizes == null)
									{
										rPLTablixCell.ContentSizes = new RPLSizes();
									}
									rPLTablixCell.ContentSizes.Width = (float)pageItem.ItemPageSizes.Width;
								}
							}
							else
							{
								rPLTablixCell = new RPLTablixCell();
							}
							rPLTablixCell.ColSpan = num3;
							rPLTablixCell.ColIndex = num2;
							rPLTablixCell.RowIndex = pageRowIndex;
							list.Add(rPLTablixCell);
						}
						num2 += num3;
					}
					if (pageDetailCell.Release)
					{
						using (m_detailCells.GetAndPin(j, out pageDetailCell))
						{
							pageDetailCell.CellItem = null;
						}
					}
				}
				binaryWriter?.Write(byte.MaxValue);
				return 1;
			}

			internal int MergeDetailCells(int destCellIndex, List<int> destState, int srcCellIndex, List<int> srcState)
			{
				PageDetailCell item = null;
				int num = -1;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = srcCellIndex;
				int result = 0;
				while (num3 < destState.Count || num4 < srcState.Count)
				{
					num = -1;
					if (num3 < destState.Count)
					{
						num = destState[num3];
					}
					num2 = num + 1;
					if (num4 < srcState.Count)
					{
						num2 = srcState[num4];
					}
					if (num == num2)
					{
						using (m_detailCells.GetAndPin(destCellIndex, out item))
						{
							(item.CellItem as HiddenPageItem)?.AddToCollection((HiddenPageItem)m_detailCells[srcCellIndex].CellItem);
						}
						num3++;
						num4++;
						destCellIndex++;
						srcCellIndex++;
					}
					else if (num < 0 || num > num2)
					{
						m_detailCells.Insert(destCellIndex, m_detailCells[srcCellIndex]);
						result = m_detailCells[srcCellIndex].ColSpan;
						num5++;
						destCellIndex++;
						srcCellIndex++;
						destState.Insert(num3, num2);
						num3++;
						num4++;
					}
					else
					{
						destCellIndex++;
						num3++;
					}
				}
				m_detailCells.RemoveRange(num5, srcState.Count);
				return result;
			}

			internal void Merge(RowInfo rowSource)
			{
				if (m_detailCells == null)
				{
					return;
				}
				PageDetailCell item = null;
				for (int i = 0; i < m_detailCells.Count; i++)
				{
					using (m_detailCells.GetAndPin(i, out item))
					{
						HiddenPageItem hiddenPageItem = item.CellItem as HiddenPageItem;
						if (hiddenPageItem != null)
						{
							HiddenPageItem hiddenItem = (HiddenPageItem)rowSource.Cells[i].CellItem;
							hiddenPageItem.AddToCollection(hiddenItem);
						}
					}
				}
			}

			internal bool ResolveDuplicates(double startInTablix, PageContext pageContext)
			{
				if (Hidden || m_detailCells == null)
				{
					return false;
				}
				if (PageVerticalState == VerticalState.Above || PageVerticalState == VerticalState.Repeat)
				{
					return false;
				}
				double topInParentSystem = Math.Max(0.0, startInTablix - m_top);
				PageDetailCell item = null;
				bool result = false;
				for (int i = 0; i < m_detailCells.Count; i++)
				{
					using (m_detailCells.GetAndPin(i, out item))
					{
						if (item.CellItem != null && !item.Hidden && item.CellItem.ResolveDuplicates(pageContext, topInParentSystem, null, recalculate: false))
						{
							result = true;
							if (item.CellItem.ItemPageSizes.Bottom > m_normalizeRowHeight)
							{
								m_normalizeRowHeight = item.CellItem.ItemPageSizes.Bottom;
								m_height = m_normalizeRowHeight;
							}
						}
					}
				}
				return result;
			}
		}

		internal class SizeInfo : IStorable, IPersistable
		{
			internal enum PageState : byte
			{
				Unknown,
				Normal,
				Skip
			}

			private double m_size;

			private double m_startPos;

			private Hashtable m_spanSize;

			private PageState m_state;

			private static Declaration m_declaration = GetDeclaration();

			internal Hashtable SpanSize
			{
				get
				{
					return m_spanSize;
				}
				set
				{
					m_spanSize = value;
				}
			}

			internal double SizeValue
			{
				get
				{
					return m_size;
				}
				set
				{
					m_size = value;
				}
			}

			internal double StartPos
			{
				get
				{
					return m_startPos;
				}
				set
				{
					m_startPos = value;
				}
			}

			internal double EndPos => m_startPos + m_size;

			internal PageState State
			{
				get
				{
					return m_state;
				}
				set
				{
					m_state = value;
				}
			}

			public int Size => 16 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_spanSize) + 1;

			internal SizeInfo()
			{
			}

			internal SizeInfo(double size)
			{
				m_size = size;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Size:
						writer.Write(m_size);
						break;
					case MemberName.StartPos:
						writer.Write(m_startPos);
						break;
					case MemberName.SpanSize:
						writer.WriteVariantVariantHashtable(m_spanSize);
						break;
					case MemberName.State:
						writer.Write(m_state);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Size:
						m_size = reader.ReadDouble();
						break;
					case MemberName.StartPos:
						m_startPos = reader.ReadDouble();
						break;
					case MemberName.SpanSize:
						m_spanSize = reader.ReadVariantVariantHashtable();
						break;
					case MemberName.State:
						m_state = (PageState)reader.ReadByte();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.SizeInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.StartPos, Token.Double));
					list.Add(new MemberInfo(MemberName.SpanSize, ObjectType.VariantVariantHashtable));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					return new Declaration(ObjectType.SizeInfo, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal void AddSpanSize(int span, double spanSize)
			{
				RSTrace.RenderingTracer.Assert(span > 1, string.Empty);
				if (m_spanSize == null)
				{
					m_spanSize = new Hashtable();
					m_spanSize.Add(span, spanSize);
					return;
				}
				object obj = m_spanSize[span];
				if (obj == null)
				{
					m_spanSize.Add(span, spanSize);
				}
				else
				{
					m_spanSize[span] = Math.Max(spanSize, (double)obj);
				}
			}

			internal static void UpdateSize(SizeInfo sizeInfo, int span, double size)
			{
				RSTrace.RenderingTracer.Assert(span > 0, string.Empty);
				if (span == 1)
				{
					sizeInfo.SizeValue = Math.Max(sizeInfo.SizeValue, size);
				}
				else
				{
					sizeInfo.AddSpanSize(span, size);
				}
			}
		}

		internal class ColumnInfo : IStorable, IPersistable
		{
			internal enum HorizontalState : byte
			{
				Unknown,
				Normal,
				AtLeft,
				AtRight,
				LeftOfNextPage
			}

			[Flags]
			private enum ColumnInfoState : byte
			{
				BlockedBySpan = 0x1,
				Unresolved = 0x2,
				Hidden = 0x4,
				KeepTogether = 0x8
			}

			private double m_size;

			internal double m_left = double.MinValue;

			private Hashtable m_spanSize;

			private ColumnInfoState m_state;

			internal HorizontalState m_horizontalState;

			private static Declaration m_declaration = GetDeclaration();

			internal Hashtable SpanSize
			{
				get
				{
					return m_spanSize;
				}
				set
				{
					m_spanSize = value;
				}
			}

			internal double SizeValue
			{
				get
				{
					return m_size;
				}
				set
				{
					m_size = value;
				}
			}

			internal bool Empty
			{
				get
				{
					if (m_size == 0.0)
					{
						return !Hidden;
					}
					return false;
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(m_state & ColumnInfoState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= ColumnInfoState.Hidden;
					}
					else
					{
						m_state &= ~ColumnInfoState.Hidden;
					}
				}
			}

			internal bool BlockedBySpan
			{
				get
				{
					return (int)(m_state & ColumnInfoState.BlockedBySpan) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= ColumnInfoState.BlockedBySpan;
					}
					else
					{
						m_state &= ~ColumnInfoState.BlockedBySpan;
					}
				}
			}

			internal bool Unresolved
			{
				get
				{
					return (int)(m_state & ColumnInfoState.Unresolved) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= ColumnInfoState.Unresolved;
					}
					else
					{
						m_state &= ~ColumnInfoState.Unresolved;
					}
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(m_state & ColumnInfoState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= ColumnInfoState.KeepTogether;
					}
					else
					{
						m_state &= ~ColumnInfoState.KeepTogether;
					}
				}
			}

			internal HorizontalState PageHorizontalState
			{
				get
				{
					return m_horizontalState;
				}
				set
				{
					m_horizontalState = value;
				}
			}

			internal double Left
			{
				get
				{
					return m_left;
				}
				set
				{
					m_left = value;
				}
			}

			internal double Right => m_left + m_size;

			public int Size => 16 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_spanSize) + 2;

			internal ColumnInfo()
			{
			}

			internal ColumnInfo(double size)
			{
				m_size = size;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Size:
						writer.Write(m_size);
						break;
					case MemberName.Left:
						writer.Write(m_left);
						break;
					case MemberName.SpanSize:
						writer.WriteVariantVariantHashtable(m_spanSize);
						break;
					case MemberName.State:
						writer.Write((byte)m_state);
						break;
					case MemberName.HorizontalState:
						writer.Write((byte)m_horizontalState);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Size:
						m_size = reader.ReadDouble();
						break;
					case MemberName.Left:
						m_left = reader.ReadDouble();
						break;
					case MemberName.SpanSize:
						m_spanSize = reader.ReadVariantVariantHashtable();
						break;
					case MemberName.State:
						m_state = (ColumnInfoState)reader.ReadByte();
						break;
					case MemberName.HorizontalState:
						m_horizontalState = (HorizontalState)reader.ReadByte();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.ColumnInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.Left, Token.Double));
					list.Add(new MemberInfo(MemberName.SpanSize, ObjectType.VariantVariantHashtable));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.HorizontalState, Token.Byte));
					return new Declaration(ObjectType.ColumnInfo, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal void AddSpanSize(int span, double spanSize)
			{
				if (m_spanSize == null)
				{
					m_spanSize = new Hashtable();
					m_spanSize.Add(span, spanSize);
					return;
				}
				object obj = m_spanSize[span];
				if (obj == null)
				{
					m_spanSize.Add(span, spanSize);
				}
				else
				{
					m_spanSize[span] = Math.Max(spanSize, (double)obj);
				}
			}
		}

		private TablixState m_tablixStateFlags;

		[StaticReference]
		private TablixRowCollection m_bodyRows;

		private double[] m_bodyRowsHeights;

		private double[] m_bodyColWidths;

		private int m_rowMembersDepth;

		private int m_colMembersDepth;

		[StaticReference]
		private List<RPLTablixMemberDef> m_rowMemberDefList;

		private Hashtable m_rowMemberDefIndexes;

		[StaticReference]
		private List<RPLTablixMemberDef> m_colMemberDefList;

		private Hashtable m_colMemberDefIndexes;

		private Hashtable m_rowMemberInstanceIndexes;

		private Hashtable m_memberAtLevelIndexes;

		private int m_ignoreCellPageBreaks;

		private int m_headerRowCols;

		private int m_headerColumnRows;

		private int m_rowMemberIndexCell = -1;

		private int m_colMemberIndexCell = -1;

		private int m_colsBeforeRowHeaders;

		private List<PageStructMemberCell> m_columnHeaders;

		private List<SizeInfo> m_colHeaderHeights;

		private List<PageStructMemberCell> m_rowHeaders;

		private List<SizeInfo> m_rowHeaderWidths;

		private ScalableList<RowInfo> m_detailRows;

		private PageCornerCell[,] m_cornerCells;

		private int m_ignoreGroupPageBreaks;

		private ScalableList<ColumnInfo> m_columnInfo;

		private int m_ignoreCol;

		private int m_ignoreRow;

		private static Declaration m_declaration = GetDeclaration();

		internal bool PinnedToParentCell
		{
			get
			{
				if (base.TablixCellTopItem && m_ignoreCellPageBreaks > 0)
				{
					return true;
				}
				return false;
			}
		}

		private bool NoRows
		{
			get
			{
				return GetFlagValue(TablixState.NoRows);
			}
			set
			{
				SetFlagValue(TablixState.NoRows, value);
			}
		}

		private bool IsLTR
		{
			get
			{
				return GetFlagValue(TablixState.IsLTR);
			}
			set
			{
				SetFlagValue(TablixState.IsLTR, value);
			}
		}

		private bool RepeatColumnHeaders
		{
			get
			{
				return GetFlagValue(TablixState.RepeatColumnHeaders);
			}
			set
			{
				SetFlagValue(TablixState.RepeatColumnHeaders, value);
			}
		}

		private bool RepeatedColumnHeaders
		{
			get
			{
				return GetFlagValue(TablixState.RepeatedColumnHeaders);
			}
			set
			{
				SetFlagValue(TablixState.RepeatedColumnHeaders, value);
			}
		}

		private bool AddToPageColumnHeaders
		{
			get
			{
				return GetFlagValue(TablixState.AddToPageColumnHeaders);
			}
			set
			{
				SetFlagValue(TablixState.AddToPageColumnHeaders, value);
			}
		}

		private bool SplitColumnHeaders
		{
			get
			{
				return GetFlagValue(TablixState.SplitColumnHeaders);
			}
			set
			{
				SetFlagValue(TablixState.SplitColumnHeaders, value);
			}
		}

		private bool RepeatRowHeaders
		{
			get
			{
				return GetFlagValue(TablixState.RepeatRowHeaders);
			}
			set
			{
				SetFlagValue(TablixState.RepeatRowHeaders, value);
			}
		}

		private bool AddToPageRowHeaders
		{
			get
			{
				return GetFlagValue(TablixState.AddToPageRowHeaders);
			}
			set
			{
				SetFlagValue(TablixState.AddToPageRowHeaders, value);
			}
		}

		private bool SplitRowHeaders
		{
			get
			{
				return GetFlagValue(TablixState.SplitRowHeaders);
			}
			set
			{
				SetFlagValue(TablixState.SplitRowHeaders, value);
			}
		}

		private bool ColumnHeadersCreated
		{
			get
			{
				return GetFlagValue(TablixState.ColumnHeadersCreated);
			}
			set
			{
				SetFlagValue(TablixState.ColumnHeadersCreated, value);
			}
		}

		private bool RowHeadersCreated
		{
			get
			{
				return GetFlagValue(TablixState.RowHeadersCreated);
			}
			set
			{
				SetFlagValue(TablixState.RowHeadersCreated, value);
			}
		}

		public override int Size => base.Size + 2 + 3 * Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 44 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_bodyRowsHeights) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_bodyColWidths) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_rowMemberDefIndexes) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_colMemberDefIndexes) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_memberAtLevelIndexes) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_rowMemberInstanceIndexes) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_columnHeaders) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_colHeaderHeights) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_rowHeaders) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_rowHeaderWidths) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_detailRows) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_cornerCells) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_columnInfo);

		internal Tablix()
		{
		}

		internal Tablix(Microsoft.ReportingServices.OnDemandReportRendering.Tablix source, PageContext pageContext)
			: base(source)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source;
			TablixInstance tablixInstance = (TablixInstance)source.Instance;
			m_itemPageSizes = new ItemSizes(source);
			m_pageBreakProperties = PageBreakProperties.Create(tablix.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				m_pageName = tablixInstance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && tablix.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
			base.KeepTogetherHorizontal = source.KeepTogether;
			base.KeepTogetherVertical = source.KeepTogether;
			base.UnresolvedKTV = (base.UnresolvedKTH = source.KeepTogether);
			base.UnresolvedPBE = (base.UnresolvedPBS = true);
			base.NeedResolve = true;
			m_bodyRows = source.Body.RowCollection;
			m_bodyRowsHeights = new double[m_bodyRows.Count];
			for (int i = 0; i < m_bodyRows.Count; i++)
			{
				m_bodyRowsHeights[i] = m_bodyRows[i].Height.ToMillimeters();
			}
			TablixColumnCollection columnCollection = source.Body.ColumnCollection;
			m_bodyColWidths = new double[columnCollection.Count];
			for (int j = 0; j < columnCollection.Count; j++)
			{
				m_bodyColWidths[j] = columnCollection[j].Width.ToMillimeters();
			}
			if (source.Body.IgnoreCellPageBreaks)
			{
				m_ignoreCellPageBreaks = 1;
			}
			m_rowMembersDepth = TablixMembersDepthTree(source.RowHierarchy.MemberCollection);
			m_colMembersDepth = TablixMembersDepthTree(source.ColumnHierarchy.MemberCollection);
			NoRows = tablixInstance.NoRows;
			base.FullyCreated = false;
		}

		internal bool CheckPageBreaks(PageContext pageContext)
		{
			if (pageContext.IgnorePageBreaks)
			{
				return false;
			}
			if (m_ignoreGroupPageBreaks > 0)
			{
				return false;
			}
			return true;
		}

		private static int RemoveHeadersAbove(List<PageStructMemberCell> members, int endIndex, ScalableList<RowInfo> detailRows, ref int rowEndIndex)
		{
			if (members == null)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			int num3 = rowEndIndex;
			int num4 = endIndex;
			PageStructStaticMemberCell pageStructStaticMemberCell = null;
			while (num4 >= 0)
			{
				pageStructStaticMemberCell = (members[num4] as PageStructStaticMemberCell);
				if (pageStructStaticMemberCell == null || !pageStructStaticMemberCell.Header || !pageStructStaticMemberCell.RepeatWith)
				{
					break;
				}
				num4--;
				num3 -= pageStructStaticMemberCell.Span;
			}
			num4++;
			RowInfo rowInfo = null;
			for (int i = num3; i < rowEndIndex; i++)
			{
				rowInfo = detailRows[i];
				if (rowInfo.PageVerticalState != RowInfo.VerticalState.Above)
				{
					break;
				}
				rowInfo.DisposeDetailCells();
				num2++;
			}
			if (num2 > 0)
			{
				detailRows.RemoveRange(num3, num2);
				rowEndIndex -= num2;
				for (int j = num4; j <= endIndex; j++)
				{
					pageStructStaticMemberCell = (members[j] as PageStructStaticMemberCell);
					if (num2 >= pageStructStaticMemberCell.Span)
					{
						num2 -= pageStructStaticMemberCell.Span;
						num++;
						pageStructStaticMemberCell.DisposeInstances();
						continue;
					}
					pageStructStaticMemberCell.Span -= num2;
					break;
				}
				members.RemoveRange(num4, num);
			}
			return num;
		}

		private static double ResolveStartPos(List<SizeInfo> sizeInfoList, double startPos)
		{
			if (sizeInfoList == null || sizeInfoList.Count == 0)
			{
				return 0.0;
			}
			sizeInfoList[0].StartPos = startPos;
			for (int i = 1; i < sizeInfoList.Count; i++)
			{
				sizeInfoList[i].StartPos = sizeInfoList[i - 1].EndPos;
			}
			return sizeInfoList[sizeInfoList.Count - 1].EndPos;
		}

		private static double ResolveStartPosRTL(List<SizeInfo> sizeInfoList, double startPos)
		{
			if (sizeInfoList == null || sizeInfoList.Count == 0)
			{
				return 0.0;
			}
			sizeInfoList[sizeInfoList.Count - 1].StartPos = startPos;
			for (int num = sizeInfoList.Count - 2; num >= 0; num--)
			{
				sizeInfoList[num].StartPos = sizeInfoList[num + 1].EndPos;
			}
			return sizeInfoList[0].EndPos;
		}

		private static double ResolveStartPosAndState(List<SizeInfo> sizeInfoList, double startPos, SizeInfo.PageState state)
		{
			if (sizeInfoList == null || sizeInfoList.Count == 0)
			{
				return 0.0;
			}
			sizeInfoList[0].StartPos = startPos;
			sizeInfoList[0].State = state;
			for (int i = 1; i < sizeInfoList.Count; i++)
			{
				sizeInfoList[i].StartPos = sizeInfoList[i - 1].EndPos;
				sizeInfoList[i].State = state;
			}
			return sizeInfoList[sizeInfoList.Count - 1].EndPos;
		}

		private static double ResolveStartPosAndStateRTL(List<SizeInfo> sizeInfoList, double startPos, SizeInfo.PageState state)
		{
			if (sizeInfoList == null || sizeInfoList.Count == 0)
			{
				return 0.0;
			}
			sizeInfoList[sizeInfoList.Count - 1].StartPos = startPos;
			sizeInfoList[sizeInfoList.Count - 1].State = state;
			for (int num = sizeInfoList.Count - 2; num >= 0; num--)
			{
				sizeInfoList[num].StartPos = sizeInfoList[num + 1].EndPos;
				sizeInfoList[num].State = state;
			}
			return sizeInfoList[0].EndPos;
		}

		private static void ResolveSizes(List<SizeInfo> sizeInfoList)
		{
			if (sizeInfoList == null)
			{
				return;
			}
			SizeInfo sizeInfo = null;
			Hashtable hashtable = null;
			double num = 0.0;
			double num2 = 0.0;
			int num3 = 0;
			for (int i = 0; i < sizeInfoList.Count; i++)
			{
				int num4 = 2;
				sizeInfo = sizeInfoList[i];
				hashtable = sizeInfo.SpanSize;
				if (hashtable == null)
				{
					continue;
				}
				while (hashtable.Count > 0)
				{
					if (hashtable[num4] != null)
					{
						num3 = 0;
						num2 = 0.0;
						num = (double)hashtable[num4];
						for (int j = i; j < i + num4; j++)
						{
							if (sizeInfoList[j] == null || sizeInfoList[j].SizeValue == 0.0)
							{
								num3++;
							}
							else
							{
								num2 += sizeInfoList[j].SizeValue;
							}
						}
						if (num2 < num)
						{
							if (num3 == 0)
							{
								sizeInfoList[i + num4 - 1].SizeValue += num - num2;
							}
							else
							{
								num2 = (num - num2) / (double)num3;
								for (int k = i; k < i + num4; k++)
								{
									if (sizeInfoList[k] == null)
									{
										sizeInfoList[k] = new SizeInfo(num2);
									}
									else if (sizeInfoList[k].SizeValue == 0.0)
									{
										sizeInfoList[k].SizeValue += num2;
									}
								}
							}
						}
						hashtable.Remove(num4);
					}
					num4++;
				}
				sizeInfo.SpanSize = null;
			}
		}

		private static void UpdateSizes(int start, int span, double size, ref List<SizeInfo> sizeInfoList)
		{
			if (span == 0)
			{
				return;
			}
			if (sizeInfoList == null)
			{
				sizeInfoList = new List<SizeInfo>();
			}
			while (sizeInfoList.Count <= start + span - 1)
			{
				sizeInfoList.Add(null);
			}
			if (span == 1)
			{
				if (sizeInfoList[start] == null)
				{
					sizeInfoList[start] = new SizeInfo(size);
				}
				else
				{
					sizeInfoList[start].SizeValue = Math.Max(sizeInfoList[start].SizeValue, size);
				}
				return;
			}
			if (sizeInfoList[start] == null)
			{
				sizeInfoList[start] = new SizeInfo();
			}
			sizeInfoList[start].AddSpanSize(span, size);
		}

		private static List<ColumnSpan> GetColumnSpans(ScalableList<ColumnInfo> columnInfoList)
		{
			List<ColumnSpan> list = new List<ColumnSpan>();
			ColumnInfo item = null;
			Hashtable hashtable = null;
			for (int i = 0; i < columnInfoList.Count; i++)
			{
				int num = 2;
				using (columnInfoList.GetAndPin(i, out item))
				{
					hashtable = item.SpanSize;
					item.SpanSize = null;
				}
				if (hashtable == null)
				{
					continue;
				}
				while (hashtable.Count > 0)
				{
					if (hashtable[num] != null)
					{
						double spanSize = (double)hashtable[num];
						list.Add(new ColumnSpan(i, num, spanSize));
						hashtable.Remove(num);
					}
					num++;
				}
			}
			return list;
		}

		private static void ResolveSizes(ScalableList<ColumnInfo> columnInfoList)
		{
			if (columnInfoList == null)
			{
				return;
			}
			List<ColumnSpan> columnSpans = GetColumnSpans(columnInfoList);
			while (columnSpans.Count > 0)
			{
				int index = 0;
				ColumnSpan columnSpan = columnSpans[index];
				int num = columnSpan.CalculateEmptyColumnns(columnInfoList);
				for (int i = 1; i < columnSpans.Count; i++)
				{
					if (num == 0)
					{
						break;
					}
					int num2 = columnSpans[i].CalculateEmptyColumnns(columnInfoList);
					if (num2 < num)
					{
						index = i;
						columnSpan = columnSpans[index];
						num = num2;
					}
				}
				ColumnInfo item = null;
				double num3 = 0.0;
				for (int j = columnSpan.Start; j < columnSpan.Start + columnSpan.Span; j++)
				{
					item = columnInfoList[j];
					if (item != null && !item.Empty)
					{
						num3 += item.SizeValue;
					}
				}
				if (num3 < columnSpan.SpanSize)
				{
					if (num == 0)
					{
						int num4 = columnSpan.Start + columnSpan.Span - 1;
						while (num4 > columnSpan.Start && columnInfoList[num4].Hidden)
						{
							num4--;
						}
						using (columnInfoList.GetAndPin(num4, out item))
						{
							item.SizeValue += columnSpan.SpanSize - num3;
						}
					}
					else
					{
						num3 = (columnSpan.SpanSize - num3) / (double)num;
						for (int k = columnSpan.Start; k < columnSpan.Start + columnSpan.Span; k++)
						{
							using (columnInfoList.GetAndPin(k, out item))
							{
								if (item == null)
								{
									columnInfoList[k] = new ColumnInfo(num3);
								}
								else if (item.Empty)
								{
									item.SizeValue += num3;
								}
							}
						}
					}
				}
				columnSpans.RemoveAt(index);
			}
		}

		private static void UpdateSizes(int start, int span, double size, bool unresolved, bool keepTogether, bool split, ScalableList<ColumnInfo> columnInfoList)
		{
			if (columnInfoList == null)
			{
				return;
			}
			ColumnInfo item = null;
			if (split)
			{
				int num = 0;
				double num2 = 0.0;
				for (int i = start; i < start + span; i++)
				{
					if (i >= columnInfoList.Count)
					{
						num++;
						continue;
					}
					item = columnInfoList[i];
					if (item == null || item.Empty)
					{
						num++;
					}
					else
					{
						num2 += item.SizeValue;
					}
				}
				if (!(num2 < size))
				{
					return;
				}
				if (num == 0)
				{
					int num3 = start + span - 1;
					while (num3 > start && columnInfoList[num3].Hidden)
					{
						num3--;
					}
					using (columnInfoList.GetAndPin(num3, out item))
					{
						item.SizeValue += size - num2;
					}
					return;
				}
				num2 = (size - num2) / (double)num;
				for (int j = start; j < start + span; j++)
				{
					if (j >= columnInfoList.Count)
					{
						while (columnInfoList.Count < j)
						{
							columnInfoList.Add(new ColumnInfo());
						}
						columnInfoList.Add(new ColumnInfo(num2));
						continue;
					}
					using (columnInfoList.GetAndPin(j, out item))
					{
						if (item == null)
						{
							columnInfoList[j] = new ColumnInfo(num2);
						}
						else if (item.Empty)
						{
							item.SizeValue += num2;
						}
					}
				}
				return;
			}
			while (columnInfoList.Count <= start + span - 1)
			{
				columnInfoList.Add(new ColumnInfo());
			}
			using (columnInfoList.GetAndPin(start, out item))
			{
				if (span == 1)
				{
					item.SizeValue = Math.Max(item.SizeValue, size);
				}
				else
				{
					item.AddSpanSize(span, size);
				}
				item.Unresolved |= unresolved;
				item.KeepTogether |= keepTogether;
			}
			for (int k = 1; k < span; k++)
			{
				using (columnInfoList.GetAndPin(start + k, out item))
				{
					item.BlockedBySpan = true;
					item.Unresolved |= unresolved;
				}
			}
		}

		private static void UpdateHidden(int start, int span, ScalableList<ColumnInfo> columnInfoList)
		{
			if (columnInfoList == null)
			{
				return;
			}
			while (columnInfoList.Count <= start + span - 1)
			{
				columnInfoList.Add(new ColumnInfo());
			}
			ColumnInfo item = null;
			for (int i = 0; i < span; i++)
			{
				using (columnInfoList.GetAndPin(start + i, out item))
				{
					item.Hidden = true;
				}
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, spbifWriter, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, rplStyleProps, pageContext);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(styleDef, writeShared: false, spbifWriter, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(styleDef, writeShared: false, rplStyleProps, pageContext);
				break;
			}
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)13);
					WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				}
				else if (m_rplElement == null)
				{
					m_rplElement = new RPLTablix();
					WriteElementProps(m_rplElement.ElementProps, pageContext);
				}
				else
				{
					RPLItemProps rplElementProps = m_rplElement.ElementProps as RPLItemProps;
					m_rplElement = new RPLTablix(rplElementProps);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, int columnsOnPage, int rowsOnPage, bool hasLabelsOnCH, bool hasLabelsOnRH)
		{
			if (rplWriter != null)
			{
				WriteDetailRows(rplWriter, columnsOnPage, rowsOnPage);
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)17);
					binaryWriter.Write(m_offset);
					WriteTablixMeasurements(rplWriter, columnsOnPage, rowsOnPage, hasLabelsOnCH, hasLabelsOnRH);
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Flush();
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write(byte.MaxValue);
				}
				else
				{
					WriteTablixMeasurements(rplWriter, columnsOnPage, rowsOnPage, hasLabelsOnCH, hasLabelsOnRH);
				}
			}
		}

		private void WriteDetailRows(RPLWriter rplWriter, int columnsOnPage, int rowsOnPage)
		{
			if (rplWriter == null || m_detailRows == null || columnsOnPage == 0 || rowsOnPage == 0)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int i = 0;
			if (m_colsBeforeRowHeaders > 0)
			{
				ColumnInfo columnInfo = null;
				for (; i < m_colsBeforeRowHeaders; i++)
				{
					columnInfo = m_columnInfo[i];
					if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
					{
						i = m_columnInfo.Count;
						break;
					}
					if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
					{
						num++;
					}
				}
			}
			if (AddToPageColumnHeaders && m_colHeaderHeights != null)
			{
				for (int j = 0; j < m_colHeaderHeights.Count; j++)
				{
					if (m_colHeaderHeights[j].State == SizeInfo.PageState.Normal)
					{
						num2++;
					}
				}
			}
			if (AddToPageRowHeaders && m_rowHeaderWidths != null)
			{
				for (int k = 0; k < m_rowHeaderWidths.Count; k++)
				{
					if (m_rowHeaderWidths[k].State == SizeInfo.PageState.Normal)
					{
						num3++;
					}
				}
			}
			int num4 = num2;
			RowInfo item = null;
			for (int l = 0; l < m_detailRows.Count; l++)
			{
				using (m_detailRows.GetAndPin(l, out item))
				{
					num4 += item.AddToPage(m_columnInfo, rplWriter, num4, num, num3);
				}
			}
		}

		internal void WriteTablixMeasurements(RPLWriter rplWriter, int columnsOnPage, int rowsOnPage, bool hasLabelsOnCH, bool hasLabelsOnRH)
		{
			if (rplWriter == null || (columnsOnPage == 0 && rowsOnPage == 0))
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			RPLTablix rPLTablix = m_rplElement as RPLTablix;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			double num5 = 0.0;
			double num6 = 0.0;
			if (columnsOnPage > 0)
			{
				if (binaryWriter != null)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(columnsOnPage);
				}
				else
				{
					rPLTablix = (m_rplElement as RPLTablix);
					rPLTablix.ColumnWidths = new float[columnsOnPage];
				}
				int i = 0;
				ColumnInfo columnInfo = null;
				if (m_colsBeforeRowHeaders > 0)
				{
					for (; i < m_colsBeforeRowHeaders; i++)
					{
						columnInfo = m_columnInfo[i];
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							i = m_columnInfo.Count;
							break;
						}
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
						{
							if (num4 == 0)
							{
								num6 = columnInfo.Left;
							}
							if (binaryWriter != null)
							{
								binaryWriter.Write((float)columnInfo.SizeValue);
								binaryWriter.Write(value: false);
							}
							else
							{
								rPLTablix.ColumnWidths[num4] = (float)columnInfo.SizeValue;
							}
							num4++;
							num++;
						}
					}
				}
				if (AddToPageRowHeaders && m_rowHeaderWidths != null)
				{
					if (IsLTR)
					{
						for (int j = 0; j < m_rowHeaderWidths.Count; j++)
						{
							if (m_rowHeaderWidths[j].State == SizeInfo.PageState.Normal)
							{
								if (num4 == 0)
								{
									num6 = m_rowHeaderWidths[j].StartPos;
								}
								if (binaryWriter != null)
								{
									binaryWriter.Write((float)m_rowHeaderWidths[j].SizeValue);
									binaryWriter.Write(value: false);
								}
								else
								{
									rPLTablix.ColumnWidths[num4] = (float)m_rowHeaderWidths[j].SizeValue;
								}
								num4++;
								num3++;
							}
						}
					}
					else
					{
						for (int num7 = m_rowHeaderWidths.Count - 1; num7 >= 0; num7--)
						{
							if (m_rowHeaderWidths[num7].State == SizeInfo.PageState.Normal)
							{
								if (num4 == 0)
								{
									num6 = m_rowHeaderWidths[num7].StartPos;
								}
								if (binaryWriter != null)
								{
									binaryWriter.Write((float)m_rowHeaderWidths[num7].SizeValue);
									binaryWriter.Write(value: false);
								}
								else
								{
									rPLTablix.ColumnWidths[num4] = (float)m_rowHeaderWidths[num7].SizeValue;
								}
								num4++;
								num3++;
							}
						}
					}
				}
				for (; i < m_columnInfo.Count; i++)
				{
					columnInfo = m_columnInfo[i];
					if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
					{
						i = m_columnInfo.Count;
						break;
					}
					if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
					{
						if (num4 == 0)
						{
							num6 = columnInfo.Left;
						}
						if (binaryWriter != null)
						{
							binaryWriter.Write((float)columnInfo.SizeValue);
							binaryWriter.Write(value: false);
						}
						else
						{
							rPLTablix.ColumnWidths[num4] = (float)columnInfo.SizeValue;
						}
						num4++;
					}
				}
			}
			if (rowsOnPage > 0)
			{
				num4 = 0;
				if (binaryWriter != null)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(rowsOnPage);
				}
				else
				{
					rPLTablix.RowHeights = new float[rowsOnPage];
				}
				if (AddToPageColumnHeaders && m_colHeaderHeights != null)
				{
					for (int k = 0; k < m_colHeaderHeights.Count; k++)
					{
						if (m_colHeaderHeights[k].State == SizeInfo.PageState.Normal)
						{
							if (num4 == 0)
							{
								num5 = m_colHeaderHeights[k].StartPos;
							}
							if (binaryWriter != null)
							{
								binaryWriter.Write((float)m_colHeaderHeights[k].SizeValue);
								binaryWriter.Write(value: false);
							}
							else
							{
								rPLTablix.RowHeights[num4] = (float)m_colHeaderHeights[k].SizeValue;
							}
							num4++;
							num2++;
						}
					}
				}
				if (m_detailRows != null)
				{
					RowInfo rowInfo = null;
					for (int l = 0; l < m_detailRows.Count; l++)
					{
						rowInfo = m_detailRows[l];
						if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
						{
							if (num4 == 0)
							{
								num5 = rowInfo.Top;
							}
							if (binaryWriter != null)
							{
								binaryWriter.Write((float)rowInfo.Height);
								binaryWriter.Write(value: false);
							}
							else
							{
								rPLTablix.RowHeights[num4] = (float)rowInfo.Height;
							}
							num4++;
						}
					}
				}
			}
			if (binaryWriter != null)
			{
				binaryWriter.Write((byte)0);
				binaryWriter.Write(num2);
				binaryWriter.Write((byte)1);
				binaryWriter.Write(num3);
				binaryWriter.Write((byte)2);
				binaryWriter.Write(num);
				if (num5 > 0.0)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write((float)num5);
				}
				if (num6 > 0.0)
				{
					binaryWriter.Write((byte)7);
					binaryWriter.Write((float)num6);
				}
				if (!IsLTR)
				{
					binaryWriter.Write((byte)3);
					binaryWriter.Write((byte)1);
				}
			}
			else
			{
				rPLTablix.ColumnHeaderRows = num2;
				rPLTablix.RowHeaderColumns = num3;
				rPLTablix.ColsBeforeRowHeaders = num;
				rPLTablix.ContentTop = (float)num5;
				rPLTablix.ContentLeft = (float)num6;
				if (!IsLTR)
				{
					rPLTablix.LayoutDirection = RPLFormat.Directions.RTL;
				}
			}
			if (binaryWriter != null)
			{
				WriteTablixMemberDefList(binaryWriter, m_rowMemberDefList, TablixRegion.RowHeader);
				WriteTablixMemberDefList(binaryWriter, m_colMemberDefList, TablixRegion.ColumnHeader);
			}
			WriteTablixContent(rplWriter, num, num3, num2, hasLabelsOnCH, hasLabelsOnRH);
		}

		private void WriteTablixMemberDefList(BinaryWriter spbifWriter, List<RPLTablixMemberDef> membersDefList, TablixRegion region)
		{
			if (membersDefList == null || membersDefList.Count == 0)
			{
				return;
			}
			if (region == TablixRegion.RowHeader)
			{
				spbifWriter.Write((byte)14);
			}
			else
			{
				spbifWriter.Write((byte)15);
			}
			spbifWriter.Write(membersDefList.Count);
			RPLTablixMemberDef rPLTablixMemberDef = null;
			for (int i = 0; i < membersDefList.Count; i++)
			{
				rPLTablixMemberDef = membersDefList[i];
				spbifWriter.Write((byte)16);
				if (rPLTablixMemberDef.DefinitionPath != null)
				{
					spbifWriter.Write((byte)0);
					spbifWriter.Write(rPLTablixMemberDef.DefinitionPath);
				}
				if (rPLTablixMemberDef.Level > 0)
				{
					spbifWriter.Write((byte)1);
					spbifWriter.Write(rPLTablixMemberDef.Level);
				}
				if (rPLTablixMemberDef.MemberCellIndex > 0)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write(rPLTablixMemberDef.MemberCellIndex);
				}
				if (rPLTablixMemberDef.State > 0)
				{
					spbifWriter.Write((byte)3);
					spbifWriter.Write(rPLTablixMemberDef.State);
				}
				spbifWriter.Write(byte.MaxValue);
			}
		}

		private void OpenDetailRow(RPLWriter rplWriter, int rowIndex, bool newRow)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			RowInfo item = m_detailRows[rowIndex];
			if (binaryWriter != null)
			{
				if (newRow)
				{
					binaryWriter.Write((byte)8);
				}
				if (item.Offset >= 0)
				{
					binaryWriter.Write((byte)9);
					binaryWriter.Write(item.Offset);
				}
				return;
			}
			if (newRow)
			{
				RPLTablixFullRow rPLTablixFullRow = new RPLTablixFullRow(-1, 0);
				((RPLTablix)m_rplElement).AddRow(rPLTablixFullRow);
				rplWriter.TablixRow = rPLTablixFullRow;
			}
			if (item.RPLTablixRow != null)
			{
				rplWriter.TablixRow.SetBodyStart();
				rplWriter.TablixRow.AddCells(item.RPLTablixRow.RowCells);
				using (m_detailRows.GetAndPin(rowIndex, out item))
				{
					item.RPLTablixRow = null;
				}
			}
		}

		private void CloseRow(RPLWriter rplWriter)
		{
			rplWriter.BinaryWriter?.Write(byte.MaxValue);
		}

		private void OpenHeaderRow(RPLWriter rplWriter, bool omittedRow, int headerStart)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				binaryWriter.Write((byte)8);
				return;
			}
			RPLTablixRow rPLTablixRow = null;
			rPLTablixRow = ((!omittedRow) ? ((RPLTablixRow)new RPLTablixFullRow(headerStart, -1)) : ((RPLTablixRow)new RPLTablixOmittedRow()));
			((RPLTablix)m_rplElement).AddRow(rPLTablixRow);
			rplWriter.TablixRow = rPLTablixRow;
		}

		private void WriteMemberToStream(RPLWriter rplWriter, PageStructMemberCell structMember, PageMemberCell memberCell, int rowIndex, int colIndex, int rowSpan, int colSpan, TablixRegion region)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				if (region == TablixRegion.ColumnHeader)
				{
					binaryWriter.Write((byte)11);
				}
				else
				{
					binaryWriter.Write((byte)12);
				}
				if (memberCell.MemberItem != null && memberCell.ContentOnPage)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(memberCell.MemberItem.Offset);
					if (memberCell.MemberItem.RplItemState > 0)
					{
						binaryWriter.Write((byte)13);
						binaryWriter.Write(memberCell.MemberItem.RplItemState);
					}
					if (memberCell.MemberItem.ItemPageSizes.Top != 0.0)
					{
						binaryWriter.Write((byte)0);
						binaryWriter.Write((float)memberCell.MemberItem.ItemPageSizes.Top);
					}
					if (memberCell.MemberItem.ItemPageSizes.Left != 0.0)
					{
						binaryWriter.Write((byte)1);
						binaryWriter.Write((float)memberCell.MemberItem.ItemPageSizes.Left);
					}
				}
				if (colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(colSpan);
				}
				if (rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(rowSpan);
				}
				if (structMember.MemberDefIndex >= 0)
				{
					binaryWriter.Write((byte)7);
					binaryWriter.Write(structMember.MemberDefIndex);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				if (memberCell.UniqueName != null)
				{
					binaryWriter.Write((byte)11);
					binaryWriter.Write(memberCell.UniqueName);
				}
				if (memberCell.Label != null)
				{
					binaryWriter.Write((byte)10);
					binaryWriter.Write(memberCell.Label);
				}
				binaryWriter.Write(byte.MaxValue);
				return;
			}
			RPLTablixMemberCell rPLTablixMemberCell = null;
			if (memberCell.MemberItem == null || !memberCell.ContentOnPage)
			{
				rPLTablixMemberCell = new RPLTablixMemberCell(null, 0, rowSpan, colSpan);
			}
			else
			{
				rPLTablixMemberCell = new RPLTablixMemberCell(memberCell.MemberItem.RPLElement as RPLItem, memberCell.MemberItem.RplItemState, rowSpan, colSpan);
				if (memberCell.MemberItem.ItemPageSizes.Top != 0.0)
				{
					rPLTablixMemberCell.ContentSizes = new RPLSizes();
					rPLTablixMemberCell.ContentSizes.Top = (float)memberCell.MemberItem.ItemPageSizes.Top;
				}
				if (memberCell.MemberItem.ItemPageSizes.Left != 0.0)
				{
					if (rPLTablixMemberCell.ContentSizes == null)
					{
						rPLTablixMemberCell.ContentSizes = new RPLSizes();
					}
					rPLTablixMemberCell.ContentSizes.Left = (float)memberCell.MemberItem.ItemPageSizes.Left;
				}
			}
			rPLTablixMemberCell.RowIndex = rowIndex;
			rPLTablixMemberCell.ColIndex = colIndex;
			if (region == TablixRegion.RowHeader)
			{
				rPLTablixMemberCell.TablixMemberDef = m_rowMemberDefList[structMember.MemberDefIndex];
			}
			else
			{
				rPLTablixMemberCell.TablixMemberDef = m_colMemberDefList[structMember.MemberDefIndex];
			}
			rPLTablixMemberCell.UniqueName = memberCell.UniqueName;
			rPLTablixMemberCell.GroupLabel = memberCell.Label;
			if (rPLTablixMemberCell.ColSpan == 0 || rPLTablixMemberCell.RowSpan == 0)
			{
				rplWriter.TablixRow.AddOmittedHeader(rPLTablixMemberCell);
				return;
			}
			rplWriter.TablixRow.SetHeaderStart();
			rplWriter.TablixRow.RowCells.Add(rPLTablixMemberCell);
		}

		private void WriteCornerCellToStream(RPLWriter rplWriter, PageCornerCell cornerCell, int rowIndex, int colIndex, int rowSpan, int colSpan)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				binaryWriter.Write((byte)10);
				if (cornerCell.CellItem != null && cornerCell.ContentOnPage)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(cornerCell.CellItem.Offset);
					if (cornerCell.CellItem.RplItemState > 0)
					{
						binaryWriter.Write((byte)13);
						binaryWriter.Write(cornerCell.CellItem.RplItemState);
					}
					if (cornerCell.CellItem.ItemPageSizes.Top != 0.0)
					{
						binaryWriter.Write((byte)0);
						binaryWriter.Write((float)cornerCell.CellItem.ItemPageSizes.Top);
					}
					if (cornerCell.CellItem.ItemPageSizes.Left != 0.0)
					{
						binaryWriter.Write((byte)1);
						binaryWriter.Write((float)cornerCell.CellItem.ItemPageSizes.Left);
					}
				}
				if (colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(colSpan);
				}
				if (rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(rowSpan);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				binaryWriter.Write(byte.MaxValue);
				return;
			}
			RPLTablixCornerCell rPLTablixCornerCell = null;
			if (cornerCell.CellItem != null && cornerCell.ContentOnPage)
			{
				rPLTablixCornerCell = new RPLTablixCornerCell(cornerCell.CellItem.RPLElement as RPLItem, cornerCell.CellItem.RplItemState, rowSpan, colSpan);
				if (cornerCell.CellItem.ItemPageSizes.Top != 0.0)
				{
					rPLTablixCornerCell.ContentSizes = new RPLSizes();
					rPLTablixCornerCell.ContentSizes.Top = (float)cornerCell.CellItem.ItemPageSizes.Top;
				}
				if (cornerCell.CellItem.ItemPageSizes.Left != 0.0)
				{
					if (rPLTablixCornerCell.ContentSizes == null)
					{
						rPLTablixCornerCell.ContentSizes = new RPLSizes();
					}
					rPLTablixCornerCell.ContentSizes.Left = (float)cornerCell.CellItem.ItemPageSizes.Left;
				}
			}
			else
			{
				rPLTablixCornerCell = new RPLTablixCornerCell(null, 0, rowSpan, colSpan);
			}
			rPLTablixCornerCell.RowIndex = rowIndex;
			rPLTablixCornerCell.ColIndex = colIndex;
			rplWriter.TablixRow.RowCells.Add(rPLTablixCornerCell);
		}

		private void WriteTablixContent(RPLWriter rplWriter, int colsBeforeRH, int headerRowCols, int headerColumnRows, bool hasLabelsOnCH, bool hasLabelsOnRH)
		{
			if (m_columnHeaders != null)
			{
				WriteCornerAndColumnMembers(rplWriter, colsBeforeRH, headerRowCols, headerColumnRows, hasLabelsOnCH);
			}
			else
			{
				WriteCornerOnly(rplWriter, headerRowCols, headerColumnRows);
			}
			if (AddToPageRowHeaders && (headerRowCols > 0 || hasLabelsOnRH) && m_rowHeaders != null)
			{
				int pageRowIndex = headerColumnRows;
				int num = 0;
				bool newRow = true;
				if (IsLTR)
				{
					for (int i = 0; i < m_rowHeaders.Count; i++)
					{
						WriteRowMembersLTR(m_rowHeaders[i], num, 0, rplWriter, ref pageRowIndex, colsBeforeRH, ref newRow);
						num += m_rowHeaders[i].Span;
					}
					return;
				}
				int[] state = new int[m_rowMembersDepth + 1];
				int targetRow = 0;
				int pageColIndex = colsBeforeRH;
				for (int j = 0; j < m_rowHeaders.Count; j++)
				{
					WriteRowMembersRTL(m_rowHeaders[j], num, 0, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, 0, colsBeforeRH);
					num += m_rowHeaders[j].Span;
					targetRow = num;
				}
			}
			else
			{
				WriteDetailOffsetRows(rplWriter);
			}
		}

		private void WriteDetailOffsetRows(RPLWriter rplWriter)
		{
			if (m_detailRows == null)
			{
				return;
			}
			RowInfo rowInfo = null;
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			RPLTablix rPLTablix = m_rplElement as RPLTablix;
			for (int i = 0; i < m_detailRows.Count; i++)
			{
				rowInfo = m_detailRows[i];
				if (rowInfo.PageVerticalState != RowInfo.VerticalState.Normal || rowInfo.Hidden)
				{
					continue;
				}
				if (binaryWriter != null)
				{
					binaryWriter.Write((byte)8);
					if (rowInfo.Offset >= 0)
					{
						binaryWriter.Write((byte)9);
						binaryWriter.Write(rowInfo.Offset);
					}
					binaryWriter.Write(byte.MaxValue);
				}
				else
				{
					rPLTablix.AddRow(rowInfo.RPLTablixRow);
					using (m_detailRows.GetAndPin(i, out rowInfo))
					{
						rowInfo.RPLTablixRow = null;
					}
				}
			}
		}

		private bool NeedWrite(PageMemberCell memberCell, int memberDefIndex, TablixRegion region)
		{
			if (memberCell == null)
			{
				return false;
			}
			if (memberCell.Label != null)
			{
				return true;
			}
			if (region == TablixRegion.RowHeader)
			{
				return m_rowMemberDefList[memberDefIndex].StaticHeadersTree;
			}
			return m_colMemberDefList[memberDefIndex].StaticHeadersTree;
		}

		private void WriteRowMembersLTR(PageStructMemberCell structMember, int rowIndex, int colIndex, RPLWriter rplWriter, ref int pageRowIndex, int pageColIndex, ref bool newRow)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				WriteRowMembersLTR(pageStructStaticMemberCell.MemberInstance, pageStructStaticMemberCell, rowIndex, colIndex, rplWriter, ref pageRowIndex, pageColIndex, ref newRow);
				return;
			}
			PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
			if (pageStructDynamicMemberCell.MemberInstances == null)
			{
				return;
			}
			PageMemberCell pageMemberCell = null;
			for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
			{
				if (m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
				{
					break;
				}
				pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
				WriteRowMembersLTR(pageMemberCell, pageStructDynamicMemberCell, rowIndex, colIndex, rplWriter, ref pageRowIndex, pageColIndex, ref newRow);
				rowIndex += pageMemberCell.RowSpan;
			}
		}

		private void WriteRowMembersLTR(PageMemberCell memberCell, PageStructMemberCell structMember, int rowIndex, int colIndex, RPLWriter rplWriter, ref int pageRowIndex, int pageColIndex, ref bool newRow)
		{
			if (memberCell == null || memberCell.Hidden || !memberCell.RHOnVerticalPage(m_detailRows, rowIndex))
			{
				return;
			}
			int colSpan = 0;
			int num = 0;
			if (memberCell.RHColsOnHorizontalPage(m_rowHeaderWidths, colIndex, out colSpan) && (colSpan > 0 || NeedWrite(memberCell, structMember.MemberDefIndex, TablixRegion.RowHeader)))
			{
				RowInfo rowInfo = null;
				for (int i = 0; i < memberCell.RowSpan; i++)
				{
					rowInfo = m_detailRows[rowIndex + i];
					if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
					{
						num++;
					}
				}
				if (newRow)
				{
					OpenHeaderRow(rplWriter, omittedRow: false, -1);
					newRow = false;
				}
				WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, num, colSpan, TablixRegion.RowHeader);
				pageColIndex += colSpan;
			}
			if (memberCell.Children == null)
			{
				OpenDetailRow(rplWriter, rowIndex, newRow);
				CloseRow(rplWriter);
				pageRowIndex++;
				newRow = true;
				for (int j = 1; j < memberCell.RowSpan; j++)
				{
					if (m_detailRows[rowIndex + j].PageVerticalState == RowInfo.VerticalState.Normal)
					{
						OpenDetailRow(rplWriter, rowIndex + j, newRow);
						CloseRow(rplWriter);
						pageRowIndex++;
					}
				}
			}
			else
			{
				for (int k = 0; k < memberCell.Children.Count; k++)
				{
					WriteRowMembersLTR(memberCell.Children[k], rowIndex, colIndex + memberCell.ColSpan, rplWriter, ref pageRowIndex, pageColIndex, ref newRow);
					rowIndex += memberCell.Children[k].Span;
				}
			}
		}

		private void WriteRowMembersRTL(PageStructMemberCell structMember, int rowIndex, int colIndex, ref int targetRow, ref bool newRow, RPLWriter rplWriter, ref int pageRowIndex, ref int pageColIndex, int[] state, int stateIndex, int startColIndex)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				bool num = WriteRowMembersRTL(pageStructStaticMemberCell.MemberInstance, pageStructStaticMemberCell, rowIndex, colIndex, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex, startColIndex);
				CloseRTLRow(stateIndex, ref newRow, ref pageRowIndex, rplWriter);
				if (num)
				{
					rowIndex += pageStructStaticMemberCell.Span;
					targetRow = rowIndex;
				}
				return;
			}
			PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
			if (pageStructDynamicMemberCell.MemberInstances == null)
			{
				return;
			}
			PageMemberCell pageMemberCell = null;
			for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
			{
				pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
				if (rowIndex + pageMemberCell.RowSpan <= targetRow)
				{
					rowIndex += pageMemberCell.RowSpan;
					continue;
				}
				if (m_detailRows[rowIndex].PageVerticalState != RowInfo.VerticalState.Below)
				{
					bool num2 = WriteRowMembersRTL(pageMemberCell, pageStructDynamicMemberCell, rowIndex, colIndex, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex, startColIndex);
					CloseRTLRow(stateIndex, ref newRow, ref pageRowIndex, rplWriter);
					if (num2)
					{
						rowIndex += pageMemberCell.RowSpan;
						targetRow = rowIndex;
						continue;
					}
					break;
				}
				break;
			}
		}

		private bool WriteRowMembersRTL(PageMemberCell memberCell, PageStructMemberCell structMember, int rowIndex, int colIndex, ref int targetRow, ref bool newRow, RPLWriter rplWriter, ref int pageRowIndex, ref int pageColIndex, int[] state, int stateIndex, int startColIndex)
		{
			if (memberCell == null || memberCell.Hidden)
			{
				return true;
			}
			if (!memberCell.RHOnVerticalPage(m_detailRows, rowIndex))
			{
				newRow = false;
				return true;
			}
			int num = -1;
			int num2 = 0;
			int colSpan = 0;
			bool flag = false;
			if (state[stateIndex] == 0)
			{
				flag = true;
				RowInfo rowInfo = null;
				for (int i = 0; i < memberCell.RowSpan; i++)
				{
					rowInfo = m_detailRows[rowIndex + i];
					if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
					{
						if (num < 0)
						{
							num = rowIndex + i;
							state[stateIndex] = rowIndex + i;
						}
						num2++;
					}
				}
			}
			if (flag)
			{
				if (memberCell.RHColsOnHorizontalPage(m_rowHeaderWidths, colIndex, out colSpan))
				{
					if (colSpan <= 0 && !NeedWrite(memberCell, structMember.MemberDefIndex, TablixRegion.RowHeader))
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (memberCell.Children == null)
			{
				OpenDetailRow(rplWriter, rowIndex, newRow: true);
				pageColIndex = startColIndex;
				newRow = true;
				if (flag)
				{
					WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, num2, colSpan, TablixRegion.RowHeader);
					pageColIndex += colSpan;
				}
			}
			else
			{
				int num3 = rowIndex + memberCell.RowSpan - 1;
				int num4 = 0;
				for (int j = 0; j < memberCell.Children.Count; j++)
				{
					num4 = rowIndex + memberCell.Children[j].Span - 1;
					if (num4 < targetRow)
					{
						rowIndex += memberCell.Children[j].Span;
						continue;
					}
					if (memberCell.ColSpan == 0)
					{
						WriteRowMembersRTL(memberCell.Children[j], rowIndex, colIndex, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex + 1, startColIndex);
					}
					else
					{
						WriteRowMembersRTL(memberCell.Children[j], rowIndex, colIndex + memberCell.ColSpan, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex + memberCell.ColSpan, startColIndex);
					}
					if (flag)
					{
						WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, num2, colSpan, TablixRegion.RowHeader);
						pageColIndex += colSpan;
						flag = false;
					}
					state[stateIndex]++;
					if (stateIndex == 0)
					{
						CloseRTLRow(ref newRow, ref pageRowIndex, rplWriter);
						if (targetRow < num4)
						{
							j--;
							targetRow++;
						}
						else
						{
							rowIndex += memberCell.Children[j].Span;
							targetRow = rowIndex;
						}
					}
					else if (targetRow < num3)
					{
						return false;
					}
				}
			}
			state[stateIndex] = 0;
			return stateIndex == 0;
		}

		private void CloseRTLRow(int stateIndex, ref bool newRow, ref int pageRowIndex, RPLWriter rplWriter)
		{
			if (stateIndex == 0)
			{
				CloseRTLRow(ref newRow, ref pageRowIndex, rplWriter);
			}
		}

		private void CloseRTLRow(ref bool newRow, ref int pageRowIndex, RPLWriter rplWriter)
		{
			if (newRow)
			{
				CloseRow(rplWriter);
				pageRowIndex++;
				newRow = false;
			}
		}

		private void WriteCornerAndColumnMembers(RPLWriter rplWriter, int colsBeforeRH, int headerRowCols, int headerColumnRows, bool hasLabelsOnCH)
		{
			if (!AddToPageColumnHeaders || (headerColumnRows == 0 && !hasLabelsOnCH))
			{
				return;
			}
			int num = 0;
			int i = 0;
			int rowIndex = 0;
			int num2 = 0;
			for (int j = 0; j < m_columnHeaders.Count; j++)
			{
				num2 += m_columnHeaders[j].Span;
			}
			bool writeOmittedHeaders = false;
			int num3 = 0;
			int num4 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			int num5 = 0;
			int num6 = -1;
			for (int k = 0; k < m_columnHeaders.Count; k++)
			{
				if (m_columnHeaders[k].HasOmittedChildren)
				{
					writeOmittedHeaders = true;
					break;
				}
			}
			for (; i < m_headerColumnRows; i++)
			{
				if (m_colHeaderHeights[i].State != SizeInfo.PageState.Normal)
				{
					continue;
				}
				num = 0;
				num4 = 0;
				num5 = 0;
				num6 = -1;
				if (writeOmittedHeaders)
				{
					WriteOmittedColHeadersRows(rplWriter, rowIndex, num3, headerRowCols);
					writeOmittedHeaders = false;
				}
				OpenHeaderRow(rplWriter, omittedRow: false, 0);
				if (m_colsBeforeRowHeaders > 0)
				{
					for (; num5 < m_columnHeaders.Count; num5++)
					{
						if (num >= m_colsBeforeRowHeaders)
						{
							break;
						}
						pageStructMemberCell = m_columnHeaders[num5];
						if (num + pageStructMemberCell.Span <= m_colsBeforeRowHeaders)
						{
							WriteColMembers(pageStructMemberCell, i, 0, num, rplWriter, num3, ref num4, ref writeOmittedHeaders);
							num += pageStructMemberCell.Span;
							continue;
						}
						PageStructDynamicMemberCell pageStructDynamicMemberCell = pageStructMemberCell as PageStructDynamicMemberCell;
						PageMemberCell pageMemberCell = null;
						for (int l = 0; l < pageStructDynamicMemberCell.MemberInstances.Count; l++)
						{
							if (m_columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								num6 = pageStructDynamicMemberCell.MemberInstances.Count;
								break;
							}
							pageMemberCell = pageStructDynamicMemberCell.MemberInstances[l];
							WriteColMembers(pageMemberCell, pageStructDynamicMemberCell, i, 0, num, rplWriter, num3, ref num4, ref writeOmittedHeaders);
							num += pageMemberCell.ColSpan;
							if (num >= m_colsBeforeRowHeaders)
							{
								num6 = l + 1;
								break;
							}
						}
						break;
					}
				}
				if (headerRowCols > 0)
				{
					WriteCornerCells(i, rplWriter, num3, num4);
					num4 += headerRowCols;
				}
				for (; num5 < m_columnHeaders.Count; num5++)
				{
					pageStructMemberCell = m_columnHeaders[num5];
					if (num6 >= 0)
					{
						PageStructDynamicMemberCell pageStructDynamicMemberCell2 = pageStructMemberCell as PageStructDynamicMemberCell;
						PageMemberCell pageMemberCell2 = null;
						for (int m = num6; m < pageStructDynamicMemberCell2.MemberInstances.Count; m++)
						{
							if (m_columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								num6 = pageStructDynamicMemberCell2.MemberInstances.Count;
								break;
							}
							pageMemberCell2 = pageStructDynamicMemberCell2.MemberInstances[m];
							WriteColMembers(pageMemberCell2, pageStructDynamicMemberCell2, i, 0, num, rplWriter, num3, ref num4, ref writeOmittedHeaders);
							num += pageMemberCell2.ColSpan;
						}
						num6 = -1;
					}
					else
					{
						WriteColMembers(pageStructMemberCell, i, 0, num, rplWriter, num3, ref num4, ref writeOmittedHeaders);
						num += pageStructMemberCell.Span;
					}
				}
				CloseRow(rplWriter);
				num3++;
				rowIndex = i + 1;
			}
			if (writeOmittedHeaders)
			{
				WriteOmittedColHeadersRows(rplWriter, rowIndex, num3, headerRowCols);
			}
		}

		private void WriteCornerOnly(RPLWriter rplWriter, int headerRowCols, int headerColumnRows)
		{
			if (headerColumnRows == 0 || m_colHeaderHeights == null || headerRowCols == 0)
			{
				return;
			}
			int i = 0;
			int num = 0;
			for (; i < m_headerColumnRows; i++)
			{
				if (m_colHeaderHeights[i].State == SizeInfo.PageState.Normal)
				{
					OpenHeaderRow(rplWriter, omittedRow: false, 0);
					WriteCornerCells(i, rplWriter, num, 0);
					CloseRow(rplWriter);
					num++;
				}
			}
		}

		private void WriteColMembers(PageStructMemberCell structMember, int targetRow, int rowIndex, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				WriteColMembers(pageStructStaticMemberCell.MemberInstance, pageStructStaticMemberCell, targetRow, rowIndex, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
				return;
			}
			PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
			if (pageStructDynamicMemberCell.MemberInstances == null)
			{
				return;
			}
			PageMemberCell pageMemberCell = null;
			for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
			{
				if (m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					break;
				}
				pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
				WriteColMembers(pageMemberCell, pageStructDynamicMemberCell, targetRow, rowIndex, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
				colIndex += pageMemberCell.ColSpan;
			}
		}

		private void WriteColMembers(PageMemberCell memberCell, PageStructMemberCell structMember, int targetRow, int rowIndex, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders)
		{
			if (memberCell == null || memberCell.Hidden || !memberCell.CHOnHorizontalPage(m_columnInfo, colIndex))
			{
				return;
			}
			bool flag = false;
			if (targetRow == rowIndex)
			{
				flag = true;
			}
			else if (targetRow < rowIndex + memberCell.RowSpan)
			{
				int num = rowIndex;
				for (int i = 0; i < memberCell.RowSpan; i++)
				{
					if (m_colHeaderHeights[rowIndex + i].State == SizeInfo.PageState.Skip)
					{
						num++;
					}
				}
				if (targetRow == num)
				{
					flag = true;
				}
			}
			int num2 = 0;
			int rowSpan = 0;
			bool flag2 = false;
			if (memberCell.RowSpan > 0 && (flag || targetRow < rowIndex + memberCell.RowSpan))
			{
				flag2 = memberCell.CHRowsOnVerticalPage(m_colHeaderHeights, rowIndex, out rowSpan);
				if (flag2)
				{
					ColumnInfo columnInfo = null;
					for (int j = 0; j < memberCell.ColSpan; j++)
					{
						columnInfo = m_columnInfo[colIndex + j];
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
						{
							num2++;
						}
					}
				}
			}
			if (flag)
			{
				if (memberCell.RowSpan > 0)
				{
					if (flag2)
					{
						if (memberCell.HasOmittedChildren && memberCell.RowSpan == 1)
						{
							writeOmittedHeaders = true;
						}
						WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, rowSpan, num2, TablixRegion.ColumnHeader);
						pageColIndex += num2;
					}
				}
				else if (memberCell.Children != null)
				{
					for (int k = 0; k < memberCell.Children.Count; k++)
					{
						WriteColMembers(memberCell.Children[k], targetRow, rowIndex, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
						colIndex += memberCell.Children[k].Span;
					}
				}
			}
			else if (targetRow < rowIndex + memberCell.RowSpan)
			{
				if (targetRow == rowIndex + memberCell.RowSpan - 1 && memberCell.RowSpan > 0 && memberCell.HasOmittedChildren)
				{
					writeOmittedHeaders = true;
				}
				pageColIndex += num2;
			}
			else if (memberCell.Children != null)
			{
				for (int l = 0; l < memberCell.Children.Count; l++)
				{
					WriteColMembers(memberCell.Children[l], targetRow, rowIndex + memberCell.RowSpan, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
					colIndex += memberCell.Children[l].Span;
				}
			}
		}

		private void WriteOmittedColHeadersRows(RPLWriter rplWriter, int rowIndex, int pageRowIndex, int headerRowCols)
		{
			int num = 0;
			bool writeOmittedHeaders = true;
			int num2 = 0;
			bool flag = true;
			int num3 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			int num4 = 0;
			int num5 = -1;
			while (writeOmittedHeaders)
			{
				num = 0;
				writeOmittedHeaders = false;
				num3 = 0;
				num4 = 0;
				num5 = -1;
				flag = true;
				if (m_colsBeforeRowHeaders > 0)
				{
					for (; num4 < m_columnHeaders.Count; num4++)
					{
						if (num >= m_colsBeforeRowHeaders)
						{
							break;
						}
						pageStructMemberCell = m_columnHeaders[num4];
						if (num + pageStructMemberCell.Span <= m_colsBeforeRowHeaders)
						{
							WriteOmittedColMembers(pageStructMemberCell, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref writeOmittedHeaders, ref flag);
							num += pageStructMemberCell.Span;
							continue;
						}
						PageStructDynamicMemberCell pageStructDynamicMemberCell = pageStructMemberCell as PageStructDynamicMemberCell;
						PageMemberCell pageMemberCell = null;
						for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
						{
							if (m_columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								num5 = pageStructDynamicMemberCell.MemberInstances.Count;
								break;
							}
							pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
							WriteOmittedColMembers(pageMemberCell, pageStructDynamicMemberCell, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref writeOmittedHeaders, ref flag);
							num += pageMemberCell.ColSpan;
							if (num >= m_colsBeforeRowHeaders)
							{
								num5 = i + 1;
								break;
							}
						}
						break;
					}
				}
				num3 += headerRowCols;
				for (; num4 < m_columnHeaders.Count; num4++)
				{
					pageStructMemberCell = m_columnHeaders[num4];
					if (num5 >= 0)
					{
						PageStructDynamicMemberCell pageStructDynamicMemberCell2 = pageStructMemberCell as PageStructDynamicMemberCell;
						PageMemberCell pageMemberCell2 = null;
						for (int j = num5; j < pageStructDynamicMemberCell2.MemberInstances.Count; j++)
						{
							if (m_columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								num5 = pageStructDynamicMemberCell2.MemberInstances.Count;
								break;
							}
							pageMemberCell2 = pageStructDynamicMemberCell2.MemberInstances[j];
							WriteOmittedColMembers(pageMemberCell2, pageStructDynamicMemberCell2, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref writeOmittedHeaders, ref flag);
							num += pageMemberCell2.ColSpan;
						}
						num5 = -1;
					}
					else
					{
						WriteOmittedColMembers(pageStructMemberCell, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref writeOmittedHeaders, ref flag);
						num += pageStructMemberCell.Span;
					}
				}
				if (!flag)
				{
					CloseRow(rplWriter);
				}
				num2++;
			}
		}

		private void WriteOmittedColMembers(PageStructMemberCell structMember, int targetRow, int rowIndex, int targetLevel, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				WriteOmittedColMembers(pageStructStaticMemberCell.MemberInstance, pageStructStaticMemberCell, targetRow, rowIndex, targetLevel, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
				return;
			}
			PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
			if (pageStructDynamicMemberCell.MemberInstances == null)
			{
				return;
			}
			PageMemberCell pageMemberCell = null;
			for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
			{
				if (m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					break;
				}
				pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
				WriteOmittedColMembers(pageMemberCell, pageStructDynamicMemberCell, targetRow, rowIndex, targetLevel, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
				colIndex += pageMemberCell.ColSpan;
			}
		}

		private void WriteOmittedColMembers(PageMemberCell memberCell, PageStructMemberCell structMember, int targetRow, int rowIndex, int targetLevel, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			if (memberCell == null || memberCell.Hidden)
			{
				return;
			}
			if (targetRow == rowIndex)
			{
				WriteOmittedColMembersLevel(memberCell, structMember, targetLevel, 0, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
			}
			else if (targetRow >= rowIndex + memberCell.RowSpan && memberCell.Children != null)
			{
				for (int i = 0; i < memberCell.Children.Count; i++)
				{
					WriteOmittedColMembers(memberCell.Children[i], targetRow, rowIndex + memberCell.RowSpan, targetLevel, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
					colIndex += memberCell.Children[i].Span;
				}
			}
		}

		private void WriteOmittedColMembersLevel(PageMemberCell memberCell, PageStructMemberCell structMember, int targetLevel, int level, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			if (memberCell == null || memberCell.Hidden || !memberCell.CHOnHorizontalPage(m_columnInfo, colIndex))
			{
				return;
			}
			if (targetLevel == level)
			{
				if (memberCell.RowSpan != 0)
				{
					return;
				}
				if (NeedWrite(memberCell, structMember.MemberDefIndex, TablixRegion.ColumnHeader))
				{
					if (openRow)
					{
						OpenHeaderRow(rplWriter, omittedRow: true, -1);
						openRow = false;
					}
					int num = 0;
					ColumnInfo columnInfo = null;
					for (int i = 0; i < memberCell.ColSpan; i++)
					{
						columnInfo = m_columnInfo[colIndex + i];
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
						{
							num++;
						}
					}
					WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, 0, num, TablixRegion.ColumnHeader);
					pageColIndex += num;
				}
				if (memberCell.HasOmittedChildren)
				{
					writeOmittedHeaders = true;
				}
				return;
			}
			bool writeOmittedHeaders2 = false;
			if (memberCell.RowSpan == 0 && memberCell.HasOmittedChildren && memberCell.Children != null)
			{
				for (int j = 0; j < memberCell.Children.Count; j++)
				{
					WriteOmittedColMembersLevel(memberCell.Children[j], targetLevel, level + 1, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders2, ref openRow);
					colIndex += memberCell.Children[j].Span;
				}
				if (writeOmittedHeaders2)
				{
					writeOmittedHeaders = true;
				}
			}
		}

		private void WriteOmittedColMembersLevel(PageStructMemberCell structMember, int targetLevel, int level, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				WriteOmittedColMembersLevel(pageStructStaticMemberCell.MemberInstance, pageStructStaticMemberCell, targetLevel, level, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
				return;
			}
			PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
			if (pageStructDynamicMemberCell.MemberInstances == null)
			{
				return;
			}
			PageMemberCell pageMemberCell = null;
			for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
			{
				if (m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					break;
				}
				pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
				WriteOmittedColMembersLevel(pageMemberCell, pageStructDynamicMemberCell, targetLevel, level, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
				colIndex += pageMemberCell.ColSpan;
			}
		}

		private void WriteCornerCells(int targetRow, RPLWriter rplWriter, int pageRowIndex, int pageColIndex)
		{
			PageCornerCell pageCornerCell = null;
			int num = 0;
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i <= targetRow; i++)
			{
				while (num < m_headerRowCols)
				{
					pageCornerCell = ((!IsLTR) ? m_cornerCells[i, m_headerRowCols - num - 1] : m_cornerCells[i, num]);
					if (pageCornerCell != null)
					{
						if (i + pageCornerCell.RowSpan > targetRow)
						{
							flag = false;
							if (targetRow == i)
							{
								flag = true;
							}
							else
							{
								int num4 = i;
								for (int j = 0; j < pageCornerCell.RowSpan; j++)
								{
									if (m_colHeaderHeights[i + j].State == SizeInfo.PageState.Skip)
									{
										num4++;
									}
								}
								if (targetRow == num4)
								{
									flag = true;
								}
							}
							if (flag)
							{
								num2 = 0;
								if (IsLTR)
								{
									for (int k = 0; k < pageCornerCell.ColSpan; k++)
									{
										if (m_rowHeaderWidths[num + k].State == SizeInfo.PageState.Normal)
										{
											num2++;
										}
									}
								}
								else
								{
									for (int l = 0; l < pageCornerCell.ColSpan; l++)
									{
										if (m_rowHeaderWidths[m_headerRowCols - num - 1 + l].State == SizeInfo.PageState.Normal)
										{
											num2++;
										}
									}
								}
								if (num2 > 0)
								{
									num3 = 0;
									for (int m = 0; m < pageCornerCell.RowSpan; m++)
									{
										if (m_colHeaderHeights[i + m].State == SizeInfo.PageState.Normal)
										{
											num3++;
										}
									}
									WriteCornerCellToStream(rplWriter, pageCornerCell, pageRowIndex, pageColIndex, num3, num2);
									pageColIndex += num2;
								}
							}
						}
						num += pageCornerCell.ColSpan;
					}
					else
					{
						num++;
					}
				}
				num = 0;
			}
		}

		private int AddToPageCornerCells(out int rowsOnPage, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			rowsOnPage = 0;
			if (m_colHeaderHeights == null && m_rowHeaderWidths == null)
			{
				return 0;
			}
			PageCornerCell pageCornerCell = null;
			int num = -1;
			int num2 = -1;
			if (AddToPageColumnHeaders && m_colHeaderHeights != null)
			{
				for (int i = 0; i < m_colHeaderHeights.Count; i++)
				{
					if (m_colHeaderHeights[i].State == SizeInfo.PageState.Normal)
					{
						if (num < 0)
						{
							num = (num2 = i);
						}
						else
						{
							num2++;
						}
						rowsOnPage++;
					}
				}
			}
			int num3 = 0;
			int num4 = -1;
			int num5 = -1;
			if (AddToPageRowHeaders && m_rowHeaderWidths != null)
			{
				for (int j = 0; j < m_rowHeaderWidths.Count; j++)
				{
					if (m_rowHeaderWidths[j].State == SizeInfo.PageState.Normal)
					{
						if (num4 < 0)
						{
							num4 = (num5 = j);
						}
						else
						{
							num5++;
						}
						num3++;
					}
				}
			}
			for (int k = 0; k <= num2; k++)
			{
				for (int l = 0; l <= num5; l++)
				{
					pageCornerCell = m_cornerCells[k, l];
					if (pageCornerCell != null && k + pageCornerCell.RowSpan >= num && l + pageCornerCell.ColSpan >= num4)
					{
						pageCornerCell.AddToPageContent(m_colHeaderHeights, m_rowHeaderWidths, k, l, IsLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState);
					}
				}
			}
			return num3;
		}

		private bool ResolveDuplicatesCornerCells(PageContext pageContext, double startInTablix, ref int startRowIndex)
		{
			bool flag = false;
			PageCornerCell pageCornerCell = null;
			if (SplitColumnHeaders && m_colHeaderHeights != null)
			{
				RoundedDouble roundedDouble = new RoundedDouble(0.0);
				for (int i = 0; i < m_colHeaderHeights.Count; i++)
				{
					roundedDouble.Value = m_colHeaderHeights[i].EndPos;
					if (!(roundedDouble <= startInTablix))
					{
						break;
					}
					startRowIndex++;
				}
			}
			for (int j = 0; j < m_headerColumnRows; j++)
			{
				for (int k = 0; k < m_headerRowCols; k++)
				{
					pageCornerCell = m_cornerCells[j, k];
					if (pageCornerCell != null && j + pageCornerCell.RowSpan >= startRowIndex)
					{
						flag |= pageCornerCell.ResolveDuplicates(j, startRowIndex, startInTablix, m_colHeaderHeights, pageContext);
					}
				}
			}
			return flag;
		}

		internal override void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (rplWriter != null && ((Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source).OmitBorderOnPageBreak)
			{
				OmitBorderOnPageBreak(pageLeft, pageTop, pageRight, pageBottom);
			}
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				WriteStartItemToStream(rplWriter, pageContext);
				OmitBorderOnPageBreak(rplWriter, pageLeft, pageTop, pageRight, pageBottom);
				double num = Math.Max(0.0, pageLeft - base.ItemPageSizes.Left);
				double num2 = Math.Max(0.0, pageTop - base.ItemPageSizes.Top);
				double num3 = pageRight - base.ItemPageSizes.Left;
				double num4 = pageBottom - base.ItemPageSizes.Top;
				bool hasLabels = false;
				bool hasLabels2 = false;
				int rowsOnPage = 0;
				RepeatState repeatState2 = repeatState;
				if (RepeatColumnHeaders)
				{
					repeatState2 |= RepeatState.Vertical;
				}
				if (RepeatRowHeaders)
				{
					repeatState2 |= RepeatState.Horizontal;
				}
				int num5 = AddToPageCornerCells(out rowsOnPage, rplWriter, pageContext, num, num2, num3, num4, repeatState2);
				int num6 = 0;
				if (AddToPageColumnHeaders && m_columnHeaders != null)
				{
					repeatState2 = repeatState;
					int num7 = 0;
					if (RepeatColumnHeaders)
					{
						repeatState2 |= RepeatState.Vertical;
					}
					for (int i = 0; i < m_columnHeaders.Count; i++)
					{
						num6 += m_columnHeaders[i].AddToPageCHContent(m_colHeaderHeights, m_columnInfo, 0, num7, IsLTR, rplWriter, pageContext, num, num2, num3, num4, repeatState2, ref hasLabels);
						num7 += m_columnHeaders[i].Span;
					}
				}
				if (AddToPageRowHeaders && m_rowHeaders != null)
				{
					repeatState2 = repeatState;
					int num8 = 0;
					if (RepeatRowHeaders && repeatState2 == RepeatState.None && !(new RoundedDouble(base.ItemPageSizes.Right) <= pageRight))
					{
						repeatState2 |= RepeatState.Horizontal;
					}
					for (int j = 0; j < m_rowHeaders.Count; j++)
					{
						m_rowHeaders[j].AddToPageRHContent(m_rowHeaderWidths, m_detailRows, num8, 0, IsLTR, rplWriter, pageContext, num, num2, num3, num4, repeatState2, ref hasLabels2);
						num8 += m_rowHeaders[j].Span;
					}
				}
				if (m_detailRows != null)
				{
					int colsOnPage = 0;
					bool pinnedToParentCell = m_ignoreCellPageBreaks > 0;
					for (int k = 0; k < m_detailRows.Count; k++)
					{
						rowsOnPage += m_detailRows[k].AddToPageContent(m_columnInfo, out colsOnPage, IsLTR, pinnedToParentCell, rplWriter, pageContext, num, num2, num3, num4, repeatState);
						if (colsOnPage > num6)
						{
							num6 = colsOnPage;
						}
					}
				}
				num5 += num6;
				WriteEndItemToStream(rplWriter, num5, rowsOnPage, hasLabels, hasLabels2);
				RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Right);
				if ((repeatState & RepeatState.Horizontal) == 0 || x > pageRight)
				{
					UpdateColumns(num, num3);
				}
				if (x <= pageRight)
				{
					bool delete = true;
					if (repeatState != 0)
					{
						delete = false;
					}
					UpdateRows(num2, num4, delete);
					if ((repeatState & RepeatState.Horizontal) == 0)
					{
						m_columnInfo.Dispose();
						m_columnInfo = null;
						m_rowHeaderWidths = null;
					}
				}
				return true;
			}
			return false;
		}

		internal override bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			base.ResolveDuplicates(pageContext, topInParentSystem, siblings, recalculate);
			if (!ColumnHeadersCreated)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			double num = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double num2 = 0.0;
			if (m_colHeaderHeights != null && (num == 0.0 || SplitColumnHeaders))
			{
				int startRowIndex = 0;
				flag = ResolveDuplicatesCornerCells(pageContext, num, ref startRowIndex);
				if (m_columnHeaders != null)
				{
					for (int i = 0; i < m_columnHeaders.Count; i++)
					{
						flag |= m_columnHeaders[i].ResolveCHDuplicates(0, startRowIndex, num, m_colHeaderHeights, pageContext);
					}
				}
				if (flag)
				{
					if (SplitColumnHeaders)
					{
						flag = false;
					}
					else
					{
						ResolveSizes(m_colHeaderHeights);
						ResolveStartPos(m_colHeaderHeights, 0.0);
						num2 = m_colHeaderHeights[m_headerColumnRows - 1].EndPos;
					}
				}
			}
			if (!SplitColumnHeaders)
			{
				if (m_rowHeaders != null)
				{
					int num3 = 0;
					for (int j = 0; j < m_rowHeaders.Count; j++)
					{
						flag2 |= m_rowHeaders[j].ResolveRHDuplicates(num3, num, m_detailRows, pageContext);
						num3 += m_rowHeaders[j].Span;
					}
				}
				if (m_detailRows != null)
				{
					for (int k = 0; k < m_detailRows.Count; k++)
					{
						flag2 |= m_detailRows[k].ResolveDuplicates(num, pageContext);
					}
				}
			}
			if (flag)
			{
				NormalizeRowHeadersHeights(num + num2, double.MaxValue, update: false);
			}
			else if (flag2)
			{
				if (RowHeadersCreated && num == 0.0)
				{
					NormalizeRowHeadersHeights(num + num2, double.MaxValue, update: false);
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag || flag2)
			{
				return true;
			}
			return false;
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			double num = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double num2 = bottomInParentSystem - base.ItemPageSizes.Top;
			double num3 = double.MaxValue;
			bool flag = true;
			if (!pageContext.FullOnPage)
			{
				num3 = ((!(anyAncestorHasKT || base.KeepTogetherVertical || hasUnpinnedAncestors)) ? (num2 - num) : pageContext.ColumnHeight);
				if (ColumnHeadersCreated && m_detailRows != null && m_detailRows.Count > 0)
				{
					RowInfo rowInfo = null;
					if (base.ItemPageSizes.Top >= topInParentSystem)
					{
						rowInfo = m_detailRows[m_detailRows.Count - 1];
						num3 = Math.Min(num3, Math.Max(0.0, num2 - rowInfo.Bottom));
					}
					else
					{
						for (int num4 = m_detailRows.Count - 1; num4 >= 0; num4--)
						{
							rowInfo = m_detailRows[num4];
							if (rowInfo.PageVerticalState == RowInfo.VerticalState.Below)
							{
								num3 = Math.Min(num3, Math.Max(0.0, num2 - rowInfo.Bottom));
								break;
							}
						}
					}
				}
			}
			else if (ColumnHeadersCreated && base.ItemPageSizes.Top >= topInParentSystem)
			{
				flag = false;
			}
			if (flag)
			{
				CreateItemsContext createItems = new CreateItemsContext(num3);
				PageContext pageContext2 = pageContext;
				if (!pageContext.IgnorePageBreaks && base.IgnorePageBreaks)
				{
					pageContext2 = new PageContext(pageContext, pageContext.CacheNonSharedProps);
					pageContext2.IgnorePageBreaks = true;
					pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
				}
				CreateVertically(pageContext2, createItems, num, num2, topInParentSystem);
			}
			if (RowHeadersCreated)
			{
				if (m_detailRows != null && m_detailRows.Count > 0)
				{
					double bottom = m_detailRows[m_detailRows.Count - 1].Bottom;
					base.ItemPageSizes.AdjustHeightTo(bottom);
				}
				base.FullyCreated = true;
			}
			else
			{
				if (new RoundedDouble(base.ItemPageSizes.Height) <= num2)
				{
					base.ItemPageSizes.AdjustHeightTo(num2 + 0.02);
				}
				base.FullyCreated = false;
			}
		}

		protected override bool InvalidateVerticalKT(bool anyAncestorHasKT, PageContext pageContext)
		{
			if (!RowHeadersCreated)
			{
				if (pageContext.Common.DiagnosticsEnabled && base.KeepTogetherVertical)
				{
					TraceInvalidatedKeepTogetherVertical(pageContext);
				}
				return true;
			}
			return base.InvalidateVerticalKT(anyAncestorHasKT, pageContext);
		}

		protected override bool ResolveKeepTogetherVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			PageBreakProperties pageBreakProperties = null;
			if (!canOverwritePageBreak)
			{
				pageBreakProperties = pageContext.Common.RegisteredPageBreakProperties;
			}
			string text = null;
			if (!canSetPageName)
			{
				text = pageContext.Common.PageName;
			}
			double num = bottomInParentSystem - base.ItemPageSizes.Top;
			if (base.UnresolvedKTV)
			{
				base.UnresolvedKTV = false;
				if (base.KeepTogetherVertical)
				{
					RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Height);
					if (x > num && x <= pageContext.ColumnHeight)
					{
						base.ItemPageSizes.MoveVertical(num);
						base.OnThisVerticalPage = false;
						TraceKeepTogetherVertical(pageContext);
						return true;
					}
				}
			}
			bool canPush = false;
			if (base.ItemPageSizes.Top > topInParentSystem)
			{
				canPush = true;
			}
			double startInTablix = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			AddToPageColumnHeaders = true;
			RepeatedColumnHeaders = false;
			if (resolveItem)
			{
				NormalizeRowHeadersHeights(startInTablix, num, update: false);
			}
			MarkTablixRowsForVerticalPage(startInTablix, num, pageContext, canOverwritePageBreak, canPush);
			if (StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
			{
				pageContext.Common.SetPageName(base.PageName, canSetPageName);
			}
			if (RowHeadersCreated && !base.PageBreakAtEnd && m_detailRows != null && m_detailRows.Count > 0 && m_detailRows[m_detailRows.Count - 1].PageBreaksAtEnd)
			{
				if (base.ItemPageSizes.Bottom < bottomInParentSystem)
				{
					double num2 = bottomInParentSystem - base.ItemPageSizes.Bottom;
					base.ItemPageSizes.DeltaY += num2;
					PageBreakProperties pageBreakPropertiesAtEnd = m_detailRows[m_detailRows.Count - 1].PageBreakPropertiesAtEnd;
					pageContext.Common.RegisterPageBreakProperties(pageBreakPropertiesAtEnd, canOverwritePageBreak);
				}
				if (pageContext.Common.DiagnosticsEnabled && base.ItemPageSizes.Bottom == bottomInParentSystem)
				{
					object source = m_detailRows[m_detailRows.Count - 1].PageBreakPropertiesAtEnd.Source;
					pageContext.Common.TracePageBreakIgnoredAtBottomOfPage(source);
				}
			}
			if (pageBreakProperties != null)
			{
				pageContext.Common.RegisterPageBreakProperties(pageBreakProperties, overwrite: true);
			}
			if (text != null)
			{
				pageContext.Common.SetPageName(text, overwrite: true);
			}
			return false;
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			double startInTablix = Math.Max(0.0, leftInParentSystem - base.ItemPageSizes.Left);
			double endInTablix = rightInParentSystem - base.ItemPageSizes.Left;
			if (anyAncestorHasKT || base.KeepTogetherHorizontal || hasUnpinnedAncestors)
			{
				_ = pageContext.ColumnWidth;
			}
			CalculateContentHorizontally(startInTablix, endInTablix, pageContext);
		}

		protected override bool ResolveKeepTogetherHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, bool resolveItem)
		{
			double num = rightInParentSystem - base.ItemPageSizes.Left;
			if (base.UnresolvedKTH)
			{
				base.UnresolvedKTH = false;
				if (base.KeepTogetherHorizontal)
				{
					RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Width);
					if (x > num && x <= pageContext.ColumnWidth)
					{
						base.ItemPageSizes.MoveHorizontal(num);
						TraceKeepTogetherHorizontal(pageContext);
						return true;
					}
				}
			}
			double startInTablix = Math.Max(0.0, leftInParentSystem - base.ItemPageSizes.Left);
			AddToPageRowHeaders = true;
			if (m_columnInfo != null && m_columnInfo.Count > 0)
			{
				if (resolveItem)
				{
					NormalizeColHeadersWidths(startInTablix, num, update: false);
				}
				MarkTablixColumnsForHorizontalPage(startInTablix, num, pageContext);
			}
			else
			{
				SplitRowHeaders = true;
				MarkSplitRowHeaders(startInTablix, num, pageContext);
			}
			return false;
		}

		private void SaveRowMemberInstanceIndex(TablixMember rowMember, TablixDynamicMemberInstance rowMemberInstance)
		{
			if (m_rowMemberInstanceIndexes == null)
			{
				m_rowMemberInstanceIndexes = new Hashtable(20, 0.1f);
			}
			m_rowMemberInstanceIndexes[rowMember.ID] = rowMemberInstance.GetInstanceIndex();
		}

		private void RestoreRowMemberInstanceIndex(TablixMember rowMember, TablixDynamicMemberInstance rowMemberInstance)
		{
			if (m_rowMemberInstanceIndexes == null)
			{
				return;
			}
			int? num = (int?)m_rowMemberInstanceIndexes[rowMember.ID];
			if (num.HasValue)
			{
				int value = num.Value;
				if (value != rowMemberInstance.GetInstanceIndex())
				{
					rowMemberInstance.SetInstanceIndex(value);
				}
			}
		}

		private void UpdateRows(double startInTablix, double endInTablix, bool delete)
		{
			if (m_rowHeaders == null)
			{
				return;
			}
			PageStructMemberCell pageStructMemberCell = null;
			int rowEndIndex = 0;
			for (int i = 0; i < m_rowHeaders.Count; i++)
			{
				pageStructMemberCell = m_rowHeaders[i];
				pageStructMemberCell.UpdateRows(m_detailRows, rowEndIndex, parentKeepRepeatWith: true, 0, delete, startInTablix, endInTablix);
				if (pageStructMemberCell.Span == 0 && !pageStructMemberCell.PartialItem)
				{
					pageStructMemberCell.DisposeInstances();
					m_rowHeaders.RemoveAt(i);
					i--;
					i -= RemoveHeadersAbove(m_rowHeaders, i, m_detailRows, ref rowEndIndex);
				}
				rowEndIndex += pageStructMemberCell.Span;
			}
			if (m_rowHeaders.Count == 0)
			{
				m_rowHeaders = null;
			}
		}

		private void CreateVertically(PageContext pageContext, CreateItemsContext createItems, double startInTablix, double endInTablix, double topInParentSystem)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source;
			if (!ColumnHeadersCreated)
			{
				if (tablix.LayoutDirection == TablixLayoutDirection.LTR)
				{
					IsLTR = true;
				}
				m_headerColumnRows = tablix.Columns;
				m_headerRowCols = tablix.Rows;
				RepeatColumnHeaders = tablix.RepeatColumnHeaders;
				RepeatRowHeaders = tablix.RepeatRowHeaders;
				AddToPageColumnHeaders = true;
				AddToPageRowHeaders = true;
				base.ItemPageSizes.AdjustHeightTo(0.0);
			}
			double num = 0.0;
			if (createItems.SpaceToFill <= 0.01)
			{
				NormalizeRowHeadersHeights(startInTablix + num, endInTablix, update: false);
				return;
			}
			num = CreateTablixItems(tablix, pageContext, createItems, startInTablix, endInTablix);
			NormalizeRowHeadersHeights(startInTablix + num, endInTablix, update: false);
		}

		private double CreateTablixItems(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext, CreateItemsContext createItems, double startInTablix, double endInTablix)
		{
			double result = CreateColumnsHeaders(tablix, pageContext);
			if (RowHeadersCreated)
			{
				return result;
			}
			bool finishLevel = false;
			CreateTablixRows(tablix, null, m_rowMembersDepth, parentBorderHeader: false, 0, 0, ref m_rowHeaders, ignoreTotals: false, ref finishLevel, parentHasFooters: false, createItems, startInTablix, endInTablix, pageContext);
			if (finishLevel)
			{
				RowHeadersCreated = true;
				if (m_rowHeaders != null && m_rowHeaders.Count == 0)
				{
					m_rowHeaders = null;
				}
			}
			m_ignoreGroupPageBreaks = 0;
			return result;
		}

		private void AlignSplitColumnHeaders(int startRowIndex, int lastRowIndex, double startInTablix, double endInTablix, PageContext pageContext, bool normalize)
		{
			m_colHeaderHeights[lastRowIndex].State = SizeInfo.PageState.Normal;
			for (int i = 0; i < m_columnHeaders.Count; i++)
			{
				m_columnHeaders[i].AlignCHToPageVertical(0, lastRowIndex, startInTablix, endInTablix, m_colHeaderHeights, pageContext);
			}
			AlignCornerCellsToPageVertical(lastRowIndex, startInTablix, endInTablix, pageContext);
			ResolveSizes(m_colHeaderHeights);
			ResolveStartPos(m_colHeaderHeights, 0.0);
			double num = 0.0;
			for (int j = 0; j < startRowIndex; j++)
			{
				m_colHeaderHeights[j].State = SizeInfo.PageState.Skip;
			}
			for (int k = startRowIndex; k <= lastRowIndex; k++)
			{
				m_colHeaderHeights[k].State = SizeInfo.PageState.Normal;
			}
			int num2 = m_colHeaderHeights.Count - 1;
			for (int l = lastRowIndex + 1; l <= num2; l++)
			{
				m_colHeaderHeights[l].State = SizeInfo.PageState.Skip;
			}
			num = m_colHeaderHeights[num2].EndPos;
			if (!normalize)
			{
				return;
			}
			if (m_detailRows != null && m_detailRows.Count > 0)
			{
				RowInfo item = null;
				using (m_detailRows.GetAndPin(0, out item))
				{
					item.PageVerticalState = RowInfo.VerticalState.Normal;
					item.Top = num;
				}
				NormalizeRowHeadersHeights(startInTablix, endInTablix, update: true);
			}
			else
			{
				base.ItemPageSizes.AdjustHeightTo(num);
			}
		}

		private void MarkDetailRowVerticalState(int rowIndex, RowInfo.VerticalState state)
		{
			MarkDetailRowVerticalState(rowIndex, state, null, canOverwritePageBreak: false);
		}

		private void MarkDetailRowVerticalState(int rowIndex, RowInfo.VerticalState state, PageContext pageContext, bool canOverwritePageBreak)
		{
			RowInfo item = null;
			using (m_detailRows.GetAndPin(rowIndex, out item))
			{
				item.PageVerticalState = state;
				if (item.PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
				{
					pageContext?.Common.RegisterPageBreakProperties(item.PageBreakPropertiesAtStart, canOverwritePageBreak);
				}
			}
		}

		private void MarkTablixRowsForVerticalPage(double startInTablix, double endInTablix, PageContext pageContext, bool canOverwritePageBreak, bool canPush)
		{
			bool flag = false;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			int rowsOnPage = 0;
			if (startInTablix == 0.0)
			{
				if (m_headerColumnRows > 0 && m_colHeaderHeights != null)
				{
					int num = 0;
					for (int i = 0; i < m_colHeaderHeights.Count; i++)
					{
						roundedDouble.Value += m_colHeaderHeights[i].SizeValue;
						if (roundedDouble < endInTablix)
						{
							num++;
						}
					}
					if (!(roundedDouble <= endInTablix))
					{
						if (roundedDouble <= pageContext.ColumnHeight && !PinnedToParentCell)
						{
							base.ItemPageSizes.MoveVertical(endInTablix);
							base.OnThisVerticalPage = false;
						}
						else
						{
							SplitColumnHeaders = true;
							RepeatColumnHeaders = false;
							AlignSplitColumnHeaders(0, num, startInTablix, endInTablix, pageContext, normalize: true);
						}
						return;
					}
					if (roundedDouble > 0.0)
					{
						rowsOnPage = 1;
					}
					roundedDouble2.Value = endInTablix;
					if (roundedDouble2 == pageContext.ColumnHeight || PinnedToParentCell)
					{
						startInTablix += roundedDouble.Value;
						roundedDouble.Value = 0.0;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					if (m_detailRows == null)
					{
						return;
					}
					roundedDouble2.Value = endInTablix;
					if (roundedDouble2 == pageContext.ColumnHeight)
					{
						if (pageContext.Common.DiagnosticsEnabled && m_detailRows.Count > 0)
						{
							RowInfo rowInfo = m_detailRows[0];
							if (rowInfo.PageBreaksAtStart)
							{
								object source = rowInfo.PageBreakPropertiesAtStart.Source;
								pageContext.Common.TracePageBreakIgnoredAtTopOfPage(source);
							}
						}
					}
					else if (!PinnedToParentCell)
					{
						if (m_detailRows.Count > 0 && m_detailRows[0].PageBreaksAtStart && canPush)
						{
							base.ItemPageSizes.MoveVertical(endInTablix);
							base.OnThisVerticalPage = false;
							pageContext.Common.RegisterPageBreakProperties(m_detailRows[0].PageBreakPropertiesAtStart, canOverwritePageBreak);
							return;
						}
						rowsOnPage = 1;
						flag = true;
					}
				}
			}
			else
			{
				if (SplitColumnHeaders)
				{
					int num2 = 0;
					int num3 = 0;
					for (int j = 0; j < m_colHeaderHeights.Count; j++)
					{
						roundedDouble2.Value = m_colHeaderHeights[j].EndPos;
						if (roundedDouble2 <= startInTablix)
						{
							m_colHeaderHeights[j].State = SizeInfo.PageState.Skip;
							num3++;
							num2++;
							continue;
						}
						roundedDouble.Value += m_colHeaderHeights[j].SizeValue;
						roundedDouble.Value -= Math.Max(0.0, startInTablix - m_colHeaderHeights[j].StartPos);
						roundedDouble2.Value = startInTablix + roundedDouble.Value;
						if (roundedDouble2 < endInTablix)
						{
							m_colHeaderHeights[j].State = SizeInfo.PageState.Normal;
							num2++;
						}
					}
					roundedDouble.Value += startInTablix;
					if (roundedDouble <= endInTablix)
					{
						AlignSplitColumnHeaders(num3, num2 - 1, startInTablix, endInTablix, pageContext, normalize: false);
						if (m_detailRows != null && m_detailRows.Count > 0)
						{
							MarkDetailRowVerticalState(0, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak);
							NormalizeRowHeadersHeights(startInTablix, endInTablix, update: false);
						}
						else
						{
							base.ItemPageSizes.AdjustHeightTo(roundedDouble.Value);
						}
						SplitColumnHeaders = false;
					}
					else
					{
						AlignSplitColumnHeaders(num3, num2, startInTablix, endInTablix, pageContext, normalize: true);
					}
					return;
				}
				if (RepeatColumnHeaders && m_colHeaderHeights != null)
				{
					if (TryRepeatColumnHeaders(startInTablix, endInTablix))
					{
						for (int k = 0; k < m_colHeaderHeights.Count; k++)
						{
							roundedDouble += m_colHeaderHeights[k].SizeValue;
						}
						RowInfo item = null;
						for (int l = 0; l < m_detailRows.Count; l++)
						{
							using (m_detailRows.GetAndPin(l, out item))
							{
								if (item.PageVerticalState != RowInfo.VerticalState.Above)
								{
									item.PageVerticalState = RowInfo.VerticalState.Unknown;
									break;
								}
							}
						}
						RepeatedColumnHeaders = true;
						if (roundedDouble > 0.0)
						{
							rowsOnPage = 1;
						}
					}
					else
					{
						AddToPageColumnHeaders = false;
					}
				}
				else
				{
					AddToPageColumnHeaders = false;
				}
			}
			HeaderFooterRows prevHeaders = null;
			HeaderFooterRows prevFooters = null;
			bool leafsOnPage = false;
			bool prevFootersOnPage = false;
			int rowIndex = 0;
			double endInTablix2 = endInTablix;
			if (MarkDetailRowsForVerticalPage(m_rowHeaders, startInTablix + roundedDouble.Value, ref endInTablix2, prevHeaders, prevFooters, ref prevFootersOnPage, ref leafsOnPage, rowIndex, rowsOnPage, pageContext) == 0)
			{
				if (flag)
				{
					if (m_detailRows.Count > 0)
					{
						MarkDetailRowVerticalState(0, RowInfo.VerticalState.Normal);
					}
					base.ItemPageSizes.MoveVertical(endInTablix);
					base.OnThisVerticalPage = false;
					return;
				}
				if (roundedDouble > 0.0)
				{
					RowInfo item2 = null;
					for (int m = 0; m < m_detailRows.Count; m++)
					{
						using (m_detailRows.GetAndPin(m, out item2))
						{
							if (item2.PageVerticalState != RowInfo.VerticalState.Above)
							{
								item2.PageVerticalState = RowInfo.VerticalState.Unknown;
								break;
							}
						}
					}
					prevHeaders = null;
					prevFooters = null;
					leafsOnPage = false;
					prevFootersOnPage = false;
					rowIndex = 0;
					endInTablix2 = endInTablix;
					rowsOnPage = MarkDetailRowsForVerticalPage(m_rowHeaders, startInTablix, ref endInTablix2, prevHeaders, prevFooters, ref prevFootersOnPage, ref leafsOnPage, rowIndex, 0, pageContext);
					AddToPageColumnHeaders = false;
					roundedDouble.Value = 0.0;
				}
			}
			if (AddToPageColumnHeaders && roundedDouble > 0.0 && !flag)
			{
				ResolveStartPosAndState(m_colHeaderHeights, startInTablix, SizeInfo.PageState.Normal);
			}
			NormalizeRowHeadersHeights(startInTablix + roundedDouble.Value, endInTablix, update: true);
		}

		private void AlignCornerCellsToPageVertical(int targetRowIndex, double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (m_cornerCells == null)
			{
				return;
			}
			PageCornerCell pageCornerCell = null;
			for (int i = 0; i < m_headerColumnRows; i++)
			{
				for (int j = 0; j < m_headerRowCols; j++)
				{
					pageCornerCell = m_cornerCells[i, j];
					if (pageCornerCell != null && i <= targetRowIndex && i + pageCornerCell.RowSpan > targetRowIndex)
					{
						pageCornerCell.AlignToPageVertical(i, targetRowIndex, startInTablix, endInTablix, m_colHeaderHeights, pageContext);
					}
				}
			}
		}

		private void NormalizeRowHeadersHeights(double startInTablix, double endInTablix, bool update)
		{
			if (m_rowHeaders != null)
			{
				int num = 0;
				int lastRowOnPage = -1;
				int lastRowBelow = -1;
				double addedHeight = 0.0;
				for (int i = 0; i < m_rowHeaders.Count; i++)
				{
					m_rowHeaders[i].NormalizeDetailRowHeight(m_detailRows, num, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
					num += m_rowHeaders[i].Span;
				}
				double num2 = 0.0;
				num2 = ((lastRowBelow < 0) ? m_detailRows[lastRowOnPage].Bottom : m_detailRows[lastRowBelow].Bottom);
				if (num2 > base.ItemPageSizes.Height)
				{
					base.ItemPageSizes.AdjustHeightTo(num2);
				}
			}
		}

		private bool TryRepeatColumnHeaders(double startInTablix, double endInTablix)
		{
			if (m_detailRows == null || m_detailRows.Count == 0)
			{
				return false;
			}
			RowInfo rowInfo = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			for (int i = 0; i < m_detailRows.Count; i++)
			{
				rowInfo = m_detailRows[i];
				roundedDouble.Value = rowInfo.Bottom;
				if (roundedDouble > startInTablix)
				{
					roundedDouble.Value = rowInfo.Top;
					if (roundedDouble < startInTablix)
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		private bool ResolveFooters(HeaderFooterRows prevFooters, ref double endInTablix, double delta, bool lastMember, ref bool prevFootersOnPage, PageContext pageContext)
		{
			if (prevFooters == null)
			{
				return false;
			}
			if (delta <= 0.0)
			{
				return true;
			}
			int num = 0;
			double addHeight = 0.0;
			num = ((!lastMember) ? prevFooters.AddToPageRepeat(ref delta, ref addHeight, pageContext, this) : prevFooters.AddKeepWithAndRepeat(ref delta, ref addHeight, pageContext, this));
			if (num > 0)
			{
				endInTablix -= addHeight;
				prevFootersOnPage = true;
			}
			if (delta <= 0.0)
			{
				return true;
			}
			return false;
		}

		private bool ResolveHeaders(HeaderFooterRows prevHeaders, ref double endInTablix, double delta, PageContext pageContext)
		{
			if (prevHeaders == null)
			{
				return false;
			}
			if (delta <= 0.0)
			{
				return true;
			}
			double addHeight = 0.0;
			prevHeaders.AddToPageRepeat(ref delta, ref addHeight, pageContext, this);
			endInTablix -= addHeight;
			if (delta <= 0.0)
			{
				return true;
			}
			return false;
		}

		private void MoveRowToNextPage(int rowIndex, PageContext pageContext)
		{
			if (rowIndex < m_detailRows.Count)
			{
				MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: false);
			}
		}

		private int CalculateCurrentRowSpan(PageMemberCell memberCell, int rowIndex)
		{
			int num = memberCell.RowSpan - memberCell.CurrRowSpan;
			if (num == 0)
			{
				return memberCell.CurrRowSpan;
			}
			int num2 = 0;
			for (int i = rowIndex; i < rowIndex + num; i++)
			{
				if (m_detailRows[i].PageVerticalState == RowInfo.VerticalState.Repeat)
				{
					num2++;
				}
			}
			return memberCell.CurrRowSpan + num2;
		}

		private bool MarkSpanRHMemberOnPage(PageMemberCell memberCell, double startInTablix, double endInTablix, int rowIndex, int rowsOnPage, int spanContent, PageContext pageContext)
		{
			int num = MarkSpanRHMemberDetailRows(memberCell.Children, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
			memberCell.ResolveRHVertical(m_detailRows, rowIndex, startInTablix, endInTablix, pageContext);
			if (num < memberCell.CurrRowSpan)
			{
				memberCell.CurrRowSpan = num;
				return true;
			}
			if (new RoundedDouble(memberCell.EndPos) > endInTablix)
			{
				return true;
			}
			RowInfo rowInfo = m_detailRows[rowIndex + memberCell.RowSpan - 1];
			if (rowInfo.PageBreaksAtEnd && spanContent == 0)
			{
				pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtEnd, overwrite: true);
				MoveRowToNextPage(rowIndex + memberCell.RowSpan, pageContext);
				return true;
			}
			return false;
		}

		private bool MarkLeafOnPage(ref bool leafsOnPage, HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, HeaderFooterRows headers, HeaderFooterRows footers, ref double endInTablix, int rowIndex, bool lastMember, double spaceOnPage, ref bool prevFootersOnPage, PageContext pageContext)
		{
			bool flag = false;
			if (!leafsOnPage)
			{
				leafsOnPage = true;
				double endInTablix2 = spaceOnPage;
				flag = ResolveHeaders(headers, ref endInTablix2, endInTablix2, pageContext);
				if (!flag)
				{
					flag = ResolveHeaders(prevHeaders, ref endInTablix2, endInTablix2, pageContext);
					if (!flag)
					{
						flag = ResolveFooters(footers, ref endInTablix2, endInTablix2, lastMember, ref prevFootersOnPage, pageContext);
						if (!flag)
						{
							flag = ResolveFooters(prevFooters, ref endInTablix2, endInTablix2, lastMember, ref prevFootersOnPage, pageContext);
						}
					}
				}
				endInTablix -= spaceOnPage - endInTablix2;
			}
			if (!flag)
			{
				RowInfo rowInfo = m_detailRows[rowIndex - 1];
				if (rowInfo.PageBreaksAtEnd)
				{
					flag = true;
					pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtEnd, overwrite: true);
				}
			}
			if (flag)
			{
				MoveRowToNextPage(rowIndex, pageContext);
			}
			return flag;
		}

		internal int MarkSpanRHMemberDetailRows(List<PageStructMemberCell> members, double startInTablix, double endInTablix, int rowIndex, int rowsOnPage, int spanContent, PageContext pageContext)
		{
			if (members == null)
			{
				if (m_detailRows[rowIndex].ResolveVertical(startInTablix, endInTablix, pageContext) && rowIndex + 1 < m_detailRows.Count)
				{
					MarkDetailRowVerticalState(rowIndex + 1, RowInfo.VerticalState.Unknown);
				}
				return 1;
			}
			int num = 0;
			int levelRowsOnPage = 0;
			PageStructMemberCell pageStructMemberCell = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble5 = new RoundedDouble(0.0);
			while (num < members.Count)
			{
				NormalizeHeights(members, num, rowIndex, startInTablix, endInTablix);
				pageStructMemberCell = members[num];
				if (!pageStructMemberCell.HasInstances)
				{
					return levelRowsOnPage;
				}
				roundedDouble.Value = pageStructMemberCell.StartPos;
				roundedDouble2.Value = pageStructMemberCell.EndPos;
				if (roundedDouble >= endInTablix || m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
				{
					if (roundedDouble == endInTablix)
					{
						MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
					}
					return levelRowsOnPage;
				}
				if (pageStructMemberCell is PageStructStaticMemberCell)
				{
					PageStructStaticMemberCell pageStructStaticMemberCell = (PageStructStaticMemberCell)pageStructMemberCell;
					PageMemberCell memberInstance = pageStructStaticMemberCell.MemberInstance;
					roundedDouble3.Value = memberInstance.StartPos;
					roundedDouble4.Value = memberInstance.EndPos;
					roundedDouble5.Value = memberInstance.ContentBottom;
					if (roundedDouble4 <= startInTablix)
					{
						rowIndex += memberInstance.RowSpan;
						num++;
						continue;
					}
					if (roundedDouble3 < startInTablix)
					{
						if (memberInstance.MemberItem != null)
						{
							spanContent++;
						}
						bool num2 = MarkSpanRHMemberOnPage(memberInstance, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
						if (memberInstance.MemberItem != null)
						{
							spanContent--;
						}
						levelRowsOnPage += memberInstance.CurrRowSpan;
						if (num2)
						{
							return levelRowsOnPage;
						}
						rowsOnPage += memberInstance.CurrRowSpan;
						rowIndex += memberInstance.RowSpan;
						num++;
						continue;
					}
					if (rowsOnPage > 0 && spanContent == 0)
					{
						bool flag = false;
						RowInfo rowInfo = m_detailRows[rowIndex];
						if (rowInfo.PageBreaksAtStart)
						{
							flag = true;
						}
						else if (roundedDouble5 > endInTablix)
						{
							flag = true;
						}
						else if (roundedDouble4 > endInTablix && new RoundedDouble(memberInstance.SizeValue) <= pageContext.ColumnHeight)
						{
							if (memberInstance.KeepTogether)
							{
								flag = true;
								TraceKeepTogetherRowMember(pageContext, pageStructStaticMemberCell);
							}
							else if (memberInstance.Children == null && !memberInstance.SpanPages && rowInfo.KeepTogether)
							{
								flag = true;
								TraceKeepTogetherRow(pageContext, rowInfo);
							}
						}
						if (flag)
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
							return levelRowsOnPage;
						}
					}
					if (roundedDouble4 <= endInTablix && !memberInstance.SpanPages && !pageStructStaticMemberCell.PartialItem)
					{
						rowIndex += memberInstance.RowSpan;
						levelRowsOnPage += memberInstance.CurrRowSpan;
						if (!memberInstance.Hidden)
						{
							rowsOnPage += memberInstance.CurrRowSpan;
						}
					}
					else if (MarkSpanRHMember(memberInstance, roundedDouble5, startInTablix, endInTablix, ref rowIndex, ref levelRowsOnPage, rowsOnPage, spanContent, pageContext))
					{
						return levelRowsOnPage;
					}
					num++;
					continue;
				}
				PageStructDynamicMemberCell pageStructDynamicMemberCell = (PageStructDynamicMemberCell)pageStructMemberCell;
				PageMemberCell item = pageStructDynamicMemberCell.MemberInstances[0];
				int num3 = pageStructDynamicMemberCell.MemberInstances.Count - 1;
				int num4 = num3;
				int num5 = 0;
				int num6 = rowIndex + pageStructDynamicMemberCell.Span;
				if (pageStructDynamicMemberCell.PartialItem && !pageStructDynamicMemberCell.CreateItem)
				{
					num4--;
				}
				roundedDouble3.Value = item.StartPos;
				if (roundedDouble3 < startInTablix)
				{
					if (item.MemberItem != null)
					{
						spanContent++;
					}
					using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(0, out item))
					{
						bool num7 = MarkSpanRHMemberOnPage(item, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
						if (item.MemberItem != null)
						{
							spanContent--;
						}
						levelRowsOnPage += item.CurrRowSpan;
						if (num7)
						{
							return levelRowsOnPage;
						}
						rowsOnPage += item.CurrRowSpan;
						rowIndex += item.RowSpan;
					}
					num5++;
				}
				while (num5 <= num3)
				{
					string text = null;
					if (!pageContext.Common.CanSetPageName)
					{
						text = pageContext.Common.PageName;
					}
					NormalizeHeights(pageStructDynamicMemberCell.MemberInstances, num5, rowIndex, startInTablix, endInTablix);
					if (num5 > 0)
					{
						item = pageStructDynamicMemberCell.MemberInstances[num5];
					}
					roundedDouble3.Value = item.StartPos;
					roundedDouble4.Value = item.EndPos;
					roundedDouble5.Value = item.ContentBottom;
					if (roundedDouble3 >= endInTablix || m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
					{
						if (roundedDouble3 == endInTablix)
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
						}
						return levelRowsOnPage;
					}
					if (rowsOnPage > 0 && spanContent == 0)
					{
						bool flag2 = false;
						RowInfo rowInfo2 = m_detailRows[rowIndex];
						if (rowInfo2.PageBreaksAtStart)
						{
							flag2 = true;
						}
						else
						{
							RoundedDouble roundedDouble6 = new RoundedDouble(0.0);
							if (roundedDouble5 > endInTablix)
							{
								flag2 = true;
							}
							if (!item.SpanPages && roundedDouble4 > endInTablix && (item.KeepTogether || (item.Children == null && rowInfo2.KeepTogether)))
							{
								roundedDouble6.Value = item.SizeValue;
								if (roundedDouble6 <= pageContext.ColumnHeight)
								{
									flag2 = true;
									TraceKeepTogetherRowMemberRow(pageContext, rowInfo2, pageStructDynamicMemberCell, item.KeepTogether);
								}
							}
						}
						if (flag2)
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
							return levelRowsOnPage;
						}
					}
					if (!pageStructDynamicMemberCell.SpanPages && !pageStructDynamicMemberCell.Hidden)
					{
						int endRowIndex = num6;
						int num8 = AdvanceInDynamicMember(pageStructDynamicMemberCell, rowIndex, ref endRowIndex, endInTablix, num5, num3);
						if (num8 > num5)
						{
							rowsOnPage += endRowIndex - rowIndex;
							levelRowsOnPage += endRowIndex - rowIndex;
							rowIndex = endRowIndex;
							ProcessVisibleDynamicMemberOnPage(pageContext, pageStructDynamicMemberCell, num5, num8);
							num5 = num8;
							continue;
						}
					}
					if (num5 <= num4 && !item.SpanPages && roundedDouble4 <= endInTablix)
					{
						rowIndex += item.RowSpan;
						levelRowsOnPage += item.CurrRowSpan;
						if (!item.Hidden)
						{
							rowsOnPage += item.CurrRowSpan;
							ProcessFullVisibleMemberCellOnPage(pageContext, item);
						}
					}
					else
					{
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(num5, out item))
						{
							if (MarkSpanRHMember(item, roundedDouble5, startInTablix, endInTablix, ref rowIndex, ref levelRowsOnPage, rowsOnPage, spanContent, pageContext))
							{
								if (text != null)
								{
									pageContext.Common.SetPageName(text, overwrite: true);
								}
								return levelRowsOnPage;
							}
						}
					}
					num5++;
					if (text != null)
					{
						pageContext.Common.SetPageName(text, overwrite: true);
					}
				}
				num++;
			}
			return levelRowsOnPage;
		}

		private bool MarkSpanRHMember(PageMemberCell memberCell, RoundedDouble contentBottom, double startInTablix, double endInTablix, ref int rowIndex, ref int levelRowsOnPage, int rowsOnPage, int spanContent, PageContext pageContext)
		{
			if (contentBottom <= endInTablix && memberCell.Children != null)
			{
				int num = 0;
				num = MarkSpanRHMemberDetailRows(memberCell.Children, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
				if (num == 0)
				{
					return true;
				}
				ProcessVisibleMemberCellOnPage(pageContext, memberCell);
				rowIndex += memberCell.RowSpan;
				if (num < memberCell.CurrRowSpan)
				{
					memberCell.CurrRowSpan = num;
					levelRowsOnPage += memberCell.CurrRowSpan;
					return true;
				}
				levelRowsOnPage += memberCell.CurrRowSpan;
				RowInfo rowInfo = m_detailRows[rowIndex - 1];
				if (rowInfo.PageBreaksAtEnd && spanContent == 0)
				{
					pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtEnd, overwrite: true);
					MoveRowToNextPage(rowIndex, pageContext);
					return true;
				}
				return false;
			}
			if (memberCell.MemberItem != null)
			{
				spanContent++;
			}
			MarkSpanRHMemberOnPage(memberCell, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
			if (memberCell.MemberItem != null)
			{
				spanContent--;
			}
			levelRowsOnPage += memberCell.CurrRowSpan;
			ProcessVisibleMemberCellOnPage(pageContext, memberCell);
			return true;
		}

		private void ProcessVisibleDynamicMemberOnPage(PageContext pageContext, PageStructDynamicMemberCell dynamicMember, int startIndex, int endIndex)
		{
			if (!pageContext.Common.CanSetPageName || !dynamicMember.HasInnerPageName)
			{
				return;
			}
			PageMemberCell pageMemberCell = null;
			if (startIndex >= endIndex)
			{
				return;
			}
			pageMemberCell = dynamicMember.MemberInstances[startIndex];
			pageContext.Common.SetPageName(pageMemberCell.PageName, overwrite: false);
			if (!pageContext.Common.CanSetPageName)
			{
				return;
			}
			if (pageMemberCell.HasInnerPageName)
			{
				PageStructMemberCell pageStructMemberCell = null;
				int num = 0;
				while (true)
				{
					if (num < pageMemberCell.Children.Count)
					{
						pageStructMemberCell = pageMemberCell.Children[num];
						if (pageStructMemberCell.HasInnerPageName)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				ProcessVisibleDynamicMemberOnPage(pageContext, pageStructMemberCell as PageStructDynamicMemberCell);
			}
			else
			{
				startIndex++;
			}
		}

		private void ProcessVisibleDynamicMemberOnPage(PageContext pageContext, PageStructDynamicMemberCell dynamicMember)
		{
			if (dynamicMember != null && dynamicMember.MemberInstances != null)
			{
				ProcessVisibleDynamicMemberOnPage(pageContext, dynamicMember, 0, dynamicMember.MemberInstances.Count);
			}
		}

		private void ProcessFullVisibleMemberCellOnPage(PageContext pageContext, PageMemberCell memberCell)
		{
			pageContext.Common.SetPageName(memberCell.PageName, overwrite: false);
			if (!pageContext.Common.CanSetPageName || !memberCell.HasInnerPageName)
			{
				return;
			}
			PageStructMemberCell pageStructMemberCell = null;
			int num = 0;
			while (true)
			{
				if (num < memberCell.Children.Count)
				{
					pageStructMemberCell = memberCell.Children[num];
					if (pageStructMemberCell.HasInnerPageName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			ProcessVisibleDynamicMemberOnPage(pageContext, pageStructMemberCell as PageStructDynamicMemberCell);
		}

		private void ProcessVisibleMemberCellOnPage(PageContext pageContext, PageMemberCell memberCell)
		{
			pageContext.Common.SetPageName(memberCell.PageName, overwrite: true);
		}

		internal int MarkDetailRowsForVerticalPage(List<PageStructMemberCell> members, double startInTablix, ref double endInTablix, HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, ref bool prevFootersOnPage, ref bool leafsOnPage, int rowIndex, int rowsOnPage, PageContext pageContext)
		{
			if (members == null)
			{
				return 1;
			}
			int num = 0;
			HeaderFooterRows headerFooterRows = null;
			HeaderFooterRows headerFooterRows2 = null;
			int num2 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble5 = new RoundedDouble(0.0);
			while (num < members.Count)
			{
				NormalizeHeights(members, num, rowIndex, startInTablix, endInTablix);
				pageStructMemberCell = members[num];
				if (!pageStructMemberCell.HasInstances)
				{
					return num2;
				}
				roundedDouble.Value = pageStructMemberCell.StartPos;
				roundedDouble2.Value = pageStructMemberCell.EndPos;
				if (roundedDouble >= endInTablix || m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
				{
					if (roundedDouble == endInTablix)
					{
						MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
					}
					return num2;
				}
				if (pageStructMemberCell is PageStructStaticMemberCell)
				{
					PageStructStaticMemberCell pageStructStaticMemberCell = (PageStructStaticMemberCell)pageStructMemberCell;
					PageMemberCell memberInstance = pageStructStaticMemberCell.MemberInstance;
					roundedDouble3.Value = memberInstance.StartPos;
					roundedDouble4.Value = memberInstance.EndPos;
					roundedDouble5.Value = memberInstance.ContentBottom;
					if (roundedDouble4 <= startInTablix)
					{
						if (pageStructStaticMemberCell.Header && pageStructStaticMemberCell.RepeatWith)
						{
							if (headerFooterRows == null)
							{
								headerFooterRows = new HeaderFooterRows();
							}
							headerFooterRows.AddHeaderRow(rowIndex, memberInstance.RowSpan, memberInstance.SizeValue, onPage: false, repeatWith: true, keepWith: false, pageStructStaticMemberCell);
						}
						rowIndex += memberInstance.RowSpan;
						num++;
						continue;
					}
					if (roundedDouble3 < startInTablix)
					{
						leafsOnPage = true;
						int num3 = 0;
						if (memberInstance.MemberItem != null)
						{
							num3++;
						}
						bool num4 = MarkSpanRHMemberOnPage(memberInstance, startInTablix, endInTablix, rowIndex, rowsOnPage, num3, pageContext);
						num2 += memberInstance.CurrRowSpan;
						if (num4)
						{
							return num2;
						}
						rowsOnPage += memberInstance.CurrRowSpan;
						rowIndex += memberInstance.RowSpan;
						num++;
						headerFooterRows?.CheckInvalidatedPageRepeat(pageContext, this);
						headerFooterRows2?.CheckInvalidatedPageRepeat(pageContext, this);
						continue;
					}
					if (rowsOnPage > 0)
					{
						bool flag = false;
						RowInfo rowInfo = m_detailRows[rowIndex];
						RoundedDouble roundedDouble6 = new RoundedDouble(rowInfo.Top);
						if (rowInfo.PageBreaksAtStart && roundedDouble6 > startInTablix)
						{
							flag = true;
						}
						else if (roundedDouble5 > endInTablix)
						{
							flag = true;
						}
						else if (((memberInstance.Children == null) & prevFootersOnPage) && (memberInstance.SpanPages || roundedDouble4 > endInTablix))
						{
							flag = true;
						}
						else if (roundedDouble4 > endInTablix)
						{
							roundedDouble6.Value = memberInstance.SizeValue;
							if (roundedDouble6 <= pageContext.ColumnHeight)
							{
								if (memberInstance.KeepTogether)
								{
									flag = true;
									TraceKeepTogetherRowMember(pageContext, pageStructStaticMemberCell);
								}
								else if (memberInstance.Children == null && !memberInstance.SpanPages && rowInfo.KeepTogether)
								{
									flag = true;
									TraceKeepTogetherRow(pageContext, rowInfo);
								}
							}
						}
						if (flag)
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
							return num2;
						}
					}
					if (!memberInstance.Hidden)
					{
						int index = num;
						if (pageStructStaticMemberCell.Header && pageStructStaticMemberCell.KeepWith && rowsOnPage > 0 && !memberInstance.SpanPages)
						{
							double pageHeight = endInTablix - memberInstance.StartPos;
							if (MoveKeepWithHeaders(members, ref index, rowIndex, pageHeight, prevHeaders, prevFooters, headerFooterRows, leafsOnPage, prevFootersOnPage, pageContext))
							{
								if (num < index)
								{
									rowIndex += pageStructMemberCell.Span;
									num2 += pageStructMemberCell.Span;
									for (num++; num < index; num++)
									{
										pageStructMemberCell = members[num];
										roundedDouble2.Value = pageStructMemberCell.EndPos;
										memberInstance = ((PageStructStaticMemberCell)pageStructMemberCell).MemberInstance;
										if (roundedDouble2 <= endInTablix)
										{
											rowIndex += memberInstance.CurrRowSpan;
											num2 += memberInstance.CurrRowSpan;
										}
									}
								}
								MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
								return num2 + BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
							}
						}
						if (num < index)
						{
							bool flag2 = false;
							while (num < index)
							{
								pageStructMemberCell = members[num];
								rowIndex += pageStructMemberCell.Span;
								if (!pageStructMemberCell.Hidden)
								{
									rowsOnPage += pageStructMemberCell.Span;
								}
								num2 += pageStructMemberCell.Span;
								num++;
								if (pageStructMemberCell is PageStructDynamicMemberCell)
								{
									flag2 = true;
									ProcessVisibleDynamicMemberOnPage(pageContext, pageStructMemberCell as PageStructDynamicMemberCell);
								}
								else if (!flag2)
								{
									memberInstance = ((PageStructStaticMemberCell)pageStructMemberCell).MemberInstance;
									if (headerFooterRows == null)
									{
										headerFooterRows = new HeaderFooterRows();
									}
									headerFooterRows.AddHeaderRow(rowIndex, memberInstance.RowSpan, memberInstance.SizeValue, onPage: true, pageStructStaticMemberCell.RepeatWith, keepWith: true, pageStructMemberCell);
								}
							}
							if (flag2)
							{
								headerFooterRows = null;
								if (MarkLeafOnPage(ref leafsOnPage, prevHeaders, prevFooters, null, null, ref endInTablix, rowIndex, IsLastVisibleMember(index - 1, members), endInTablix - pageStructMemberCell.EndPos, ref prevFootersOnPage, pageContext))
								{
									return num2;
								}
							}
							continue;
						}
						if (pageStructStaticMemberCell.Header && (pageStructStaticMemberCell.KeepWith || pageStructStaticMemberCell.RepeatWith))
						{
							if (headerFooterRows == null)
							{
								headerFooterRows = new HeaderFooterRows();
							}
							headerFooterRows.AddHeaderRow(rowIndex, memberInstance.RowSpan, memberInstance.SizeValue, onPage: true, pageStructStaticMemberCell.RepeatWith, pageStructStaticMemberCell.KeepWith, pageStructStaticMemberCell);
						}
					}
					if (roundedDouble4 <= endInTablix && !memberInstance.SpanPages && !pageStructStaticMemberCell.PartialItem)
					{
						if (memberInstance.Hidden)
						{
							rowIndex += memberInstance.RowSpan;
							num2 += pageStructMemberCell.Span;
							num++;
							continue;
						}
						num++;
						if (leafsOnPage && IsLastVisibleMember(num - 1, members) && prevFooters != null && ResolveKeepWithFooters(prevFooters, null, memberInstance, endInTablix, pageContext))
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
							return num2;
						}
						bool flag3 = false;
						if (pageStructStaticMemberCell.Footer && pageStructStaticMemberCell.KeepWith && rowsOnPage == 0)
						{
							flag3 = true;
						}
						rowIndex += memberInstance.RowSpan;
						rowsOnPage += memberInstance.CurrRowSpan;
						num2 += memberInstance.CurrRowSpan;
						if (!pageStructStaticMemberCell.KeepWith || flag3)
						{
							ProcessFullVisibleMemberCellOnPage(pageContext, memberInstance);
							if (MarkLeafOnPage(ref leafsOnPage, prevHeaders, prevFooters, null, null, ref endInTablix, rowIndex, IsLastVisibleMember(num - 1, members), endInTablix - memberInstance.EndPos, ref prevFootersOnPage, pageContext))
							{
								return num2 + BringHeadersFootersOnPage(headerFooterRows, null);
							}
						}
						continue;
					}
					if (roundedDouble5 <= endInTablix && memberInstance.Children != null)
					{
						int num5 = 0;
						num5 = MarkDetailRowsForVerticalPage(memberInstance.Children, startInTablix, ref endInTablix, prevHeaders, prevFooters, ref prevFootersOnPage, ref leafsOnPage, rowIndex, rowsOnPage, pageContext);
						if (num5 == 0)
						{
							return num2 + BringHeadersFootersOnPage(headerFooterRows, null);
						}
						int num6 = CalculateCurrentRowSpan(memberInstance, rowIndex);
						rowIndex += memberInstance.RowSpan;
						if (num5 < num6)
						{
							memberInstance.CurrRowSpan = num5;
							num2 += memberInstance.CurrRowSpan;
							return num2 + BringHeadersFootersOnPage(headerFooterRows, null);
						}
						memberInstance.CurrRowSpan = num6;
						num2 += memberInstance.CurrRowSpan;
						RowInfo rowInfo2 = m_detailRows[rowIndex - 1];
						if (rowInfo2.PageBreaksAtEnd)
						{
							pageContext.Common.RegisterPageBreakProperties(rowInfo2.PageBreakPropertiesAtEnd, overwrite: true);
							MoveRowToNextPage(rowIndex, pageContext);
							return num2 + BringHeadersFootersOnPage(headerFooterRows, null);
						}
						rowsOnPage += memberInstance.CurrRowSpan;
						num++;
						continue;
					}
					int num7 = 0;
					if (memberInstance.MemberItem != null)
					{
						num7++;
					}
					MarkSpanRHMemberOnPage(memberInstance, startInTablix, endInTablix, rowIndex, rowsOnPage, num7, pageContext);
					num2 += memberInstance.CurrRowSpan;
					headerFooterRows?.CheckInvalidatedPageRepeat(pageContext, this);
					headerFooterRows2?.CheckInvalidatedPageRepeat(pageContext, this);
					return num2;
				}
				PageStructDynamicMemberCell pageStructDynamicMemberCell = (PageStructDynamicMemberCell)pageStructMemberCell;
				PageMemberCell item = pageStructDynamicMemberCell.MemberInstances[0];
				int num8 = pageStructDynamicMemberCell.MemberInstances.Count - 1;
				int num9 = 0;
				bool lastMember = false;
				bool footersKeepWith = false;
				int num10 = rowIndex + pageStructDynamicMemberCell.Span;
				headerFooterRows2 = CollectFooters(members, num + 1, rowIndex + pageStructDynamicMemberCell.Span, ref lastMember, ref footersKeepWith);
				if (headerFooterRows2 != null && footersKeepWith)
				{
					if (pageStructDynamicMemberCell.PartialItem)
					{
						footersKeepWith = false;
						headerFooterRows2.SetRowsKeepWith(keepWith: false);
					}
					else if (m_detailRows[rowIndex + pageStructDynamicMemberCell.Span - 1].PageBreaksAtEnd)
					{
						footersKeepWith = false;
						headerFooterRows2.SetRowsKeepWith(keepWith: false);
					}
					else if (num8 > 0)
					{
						headerFooterRows2.SetRowsKeepWith(keepWith: false);
					}
				}
				roundedDouble3.Value = item.StartPos;
				if (roundedDouble3 < startInTablix)
				{
					leafsOnPage = true;
					int num11 = 0;
					if (item.MemberItem != null)
					{
						num11++;
					}
					using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(0, out item))
					{
						bool num12 = MarkSpanRHMemberOnPage(item, startInTablix, endInTablix, rowIndex, rowsOnPage, num11, pageContext);
						num2 += item.CurrRowSpan;
						if (num12)
						{
							return num2;
						}
						rowsOnPage += item.CurrRowSpan;
						rowIndex += item.RowSpan;
					}
					num9++;
					headerFooterRows?.CheckInvalidatedPageRepeat(pageContext, this);
					headerFooterRows2?.CheckInvalidatedPageRepeat(pageContext, this);
				}
				HeaderFooterRows prevHeaders2 = MergeHeadersFooters(prevHeaders, headerFooterRows);
				HeaderFooterRows prevFooters2 = MergeHeadersFooters(prevFooters, headerFooterRows2);
				while (num9 <= num8)
				{
					string text = null;
					if (!pageContext.Common.CanSetPageName)
					{
						text = pageContext.Common.PageName;
					}
					NormalizeHeights(pageStructDynamicMemberCell.MemberInstances, num9, rowIndex, startInTablix, endInTablix);
					if (num9 > 0)
					{
						item = pageStructDynamicMemberCell.MemberInstances[num9];
					}
					roundedDouble3.Value = item.StartPos;
					roundedDouble4.Value = item.EndPos;
					roundedDouble5.Value = item.ContentBottom;
					if (roundedDouble3 >= endInTablix || m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
					{
						num2 += BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
						if (roundedDouble3 == endInTablix)
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
						}
						return num2;
					}
					if (headerFooterRows2 != null && footersKeepWith && !pageStructDynamicMemberCell.PartialItem && num9 == num8)
					{
						headerFooterRows2.SetRowsKeepWith(keepWith: true);
					}
					if (rowsOnPage > 0)
					{
						bool flag4 = false;
						RowInfo rowInfo3 = m_detailRows[rowIndex];
						RoundedDouble roundedDouble7 = new RoundedDouble(rowInfo3.Top);
						if (rowInfo3.PageBreaksAtStart && roundedDouble7 > startInTablix)
						{
							flag4 = true;
						}
						else
						{
							if (roundedDouble5 > endInTablix)
							{
								flag4 = true;
							}
							else if (item.KeepTogether && roundedDouble4 > endInTablix && (num9 > 0 || headerFooterRows == null || headerFooterRows.CountNotOnPage == headerFooterRows.Count))
							{
								RoundedDouble roundedDouble8 = new RoundedDouble(item.SizeValue);
								if (headerFooterRows != null)
								{
									if (num9 == 0)
									{
										roundedDouble8.Value += headerFooterRows.Height;
									}
									else
									{
										roundedDouble8.Value += headerFooterRows.RepeatHeight;
									}
								}
								if (headerFooterRows2 != null)
								{
									if (num9 == num8)
									{
										roundedDouble8.Value += headerFooterRows2.KeepWithAndRepeatHeight;
									}
									else
									{
										roundedDouble8.Value += headerFooterRows2.RepeatHeight;
									}
								}
								roundedDouble8.Value += SpaceNeededForFullPage(prevHeaders, prevFooters, lastMember);
								if (roundedDouble8 <= pageContext.ColumnHeight)
								{
									flag4 = true;
									TraceKeepTogetherRowMember(pageContext, pageStructDynamicMemberCell);
								}
							}
							if (!flag4 && item.Children == null)
							{
								if (item.SpanPages)
								{
									if (prevFootersOnPage)
									{
										flag4 = true;
									}
								}
								else if (roundedDouble4 > endInTablix)
								{
									if (prevFootersOnPage)
									{
										flag4 = true;
									}
									else if (rowInfo3.KeepTogether || item.KeepTogether)
									{
										roundedDouble7.Value = item.SizeValue;
										if (roundedDouble7 <= pageContext.ColumnHeight)
										{
											flag4 = true;
											TraceKeepTogetherRowMemberRow(pageContext, rowInfo3, pageStructDynamicMemberCell, item.KeepTogether);
										}
									}
								}
							}
						}
						if (flag4)
						{
							MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
							return num2 + BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
						}
					}
					if (num9 > 0 && num9 < num8)
					{
						if (!pageStructDynamicMemberCell.SpanPages)
						{
							int endRowIndex = num10;
							int num13 = AdvanceInDynamicMember(pageStructDynamicMemberCell, rowIndex, ref endRowIndex, endInTablix, num9, num8);
							if (num13 > num9)
							{
								rowsOnPage += endRowIndex - rowIndex;
								num2 += endRowIndex - rowIndex;
								rowIndex = endRowIndex;
								ProcessVisibleDynamicMemberOnPage(pageContext, pageStructDynamicMemberCell, num9, num13);
								num9 = num13;
								continue;
							}
						}
						else if (roundedDouble4 <= endInTablix && !item.SpanPages)
						{
							rowIndex += item.RowSpan;
							rowsOnPage += item.CurrRowSpan;
							num2 += item.CurrRowSpan;
							num9++;
							ProcessFullVisibleMemberCellOnPage(pageContext, item);
							continue;
						}
					}
					if (roundedDouble5 <= endInTablix && item.Children != null)
					{
						if (item.Hidden)
						{
							rowIndex += item.RowSpan;
							num2 += item.CurrRowSpan;
						}
						else
						{
							int num14 = 0;
							using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(num9, out item))
							{
								num14 = MarkDetailRowsForVerticalPage(item.Children, startInTablix, ref endInTablix, prevHeaders2, prevFooters2, ref prevFootersOnPage, ref leafsOnPage, rowIndex, rowsOnPage, pageContext);
								if (num14 == 0)
								{
									num2 += BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
									return num2;
								}
								ProcessVisibleMemberCellOnPage(pageContext, item);
								int num15 = CalculateCurrentRowSpan(item, rowIndex);
								rowIndex += item.RowSpan;
								if (num14 < num15)
								{
									item.CurrRowSpan = num14;
									num2 += item.CurrRowSpan;
									num2 += BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
									if (text != null)
									{
										pageContext.Common.SetPageName(text, overwrite: true);
									}
									return num2;
								}
								item.CurrRowSpan = num15;
								num2 += item.CurrRowSpan;
								RowInfo rowInfo4 = m_detailRows[rowIndex - 1];
								if (rowInfo4.PageBreaksAtEnd)
								{
									if (!rowInfo4.SpanPagesRow)
									{
										pageContext.Common.RegisterPageBreakProperties(rowInfo4.PageBreakPropertiesAtEnd, overwrite: true);
									}
									MoveRowToNextPage(rowIndex, pageContext);
									num2 += BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
									if (text != null)
									{
										pageContext.Common.SetPageName(text, overwrite: true);
									}
									return num2;
								}
								rowsOnPage += item.CurrRowSpan;
							}
						}
					}
					else
					{
						if (item.Children != null || !(roundedDouble4 <= endInTablix) || item.SpanPages)
						{
							int num16 = 0;
							if (item.MemberItem != null)
							{
								num16++;
							}
							using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(num9, out item))
							{
								MarkSpanRHMemberOnPage(item, startInTablix, endInTablix, rowIndex, rowsOnPage, num16, pageContext);
								ProcessVisibleMemberCellOnPage(pageContext, item);
								num2 += item.CurrRowSpan;
							}
							if (text != null)
							{
								pageContext.Common.SetPageName(text, overwrite: true);
							}
							headerFooterRows?.CheckInvalidatedPageRepeat(pageContext, this);
							headerFooterRows2?.CheckInvalidatedPageRepeat(pageContext, this);
							return num2;
						}
						if (item.Hidden)
						{
							rowIndex += item.RowSpan;
							num2 += item.CurrRowSpan;
						}
						else
						{
							if (leafsOnPage && lastMember && num9 == num8 && prevFooters != null && ResolveKeepWithFooters(prevFooters2, headerFooterRows2, item, endInTablix, pageContext))
							{
								MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak: true);
								return num2;
							}
							rowIndex += item.RowSpan;
							rowsOnPage += item.CurrRowSpan;
							num2 += item.CurrRowSpan;
							ProcessFullVisibleMemberCellOnPage(pageContext, item);
							if (MarkLeafOnPage(ref leafsOnPage, prevHeaders, prevFooters, headerFooterRows, headerFooterRows2, ref endInTablix, rowIndex, lastMember, endInTablix - item.EndPos, ref prevFootersOnPage, pageContext))
							{
								num2 += BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
								if (text != null)
								{
									pageContext.Common.SetPageName(text, overwrite: true);
								}
								return num2;
							}
						}
					}
					num9++;
					if (text != null)
					{
						pageContext.Common.SetPageName(text, overwrite: true);
					}
				}
				int num17 = BringHeadersFootersOnPage(headerFooterRows, null);
				num2 += num17;
				rowsOnPage += num17;
				headerFooterRows = null;
				num++;
				if (headerFooterRows2 != null)
				{
					num17 = headerFooterRows2.Count - headerFooterRows2.CountNotOnPage;
					num += num17;
					num2 += num17;
					rowsOnPage += num17;
					headerFooterRows2 = null;
				}
			}
			return num2;
		}

		private int AdvanceInDynamicMember(PageStructDynamicMemberCell dynamicMember, int startRowIndex, ref int endRowIndex, double endInTablix, int start, int end)
		{
			int num = (end - start) / 2;
			int num2 = AdvanceInDynamicMember(dynamicMember, endInTablix, start, end);
			if (num2 > num)
			{
				while (end >= num2)
				{
					endRowIndex -= dynamicMember.MemberInstances[end].RowSpan;
					end--;
				}
			}
			else
			{
				while (start < num2)
				{
					startRowIndex += dynamicMember.MemberInstances[start].RowSpan;
					start++;
				}
				endRowIndex = startRowIndex;
			}
			return num2;
		}

		private int AdvanceInDynamicMember(PageStructDynamicMemberCell dynamic, double endInTablix, int start, int end)
		{
			ScalableList<PageMemberCell> memberInstances = dynamic.MemberInstances;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			int num = -1;
			int num2 = (end - start) / 2;
			while (num2 > 0)
			{
				roundedDouble.Value = memberInstances[start + num2].EndPos;
				if (roundedDouble < endInTablix)
				{
					start += num2;
					num2 = (end - start) / 2;
					num = start;
				}
				else
				{
					end = start + num2;
					num2 /= 2;
				}
			}
			if (num >= 0)
			{
				return num + 1;
			}
			return start;
		}

		internal HeaderFooterRows CollectFooters(List<PageStructMemberCell> members, int index, int rowIndex, ref bool lastMember, ref bool footersKeepWith)
		{
			if (members == null)
			{
				return null;
			}
			HeaderFooterRows headerFooterRows = null;
			PageStructStaticMemberCell pageStructStaticMemberCell = null;
			bool pageBreaksAtEnd = m_detailRows[rowIndex - 1].PageBreaksAtEnd;
			for (int i = index; i < members.Count; i++)
			{
				pageStructStaticMemberCell = (members[i] as PageStructStaticMemberCell);
				if (pageStructStaticMemberCell == null)
				{
					return headerFooterRows;
				}
				if (pageStructStaticMemberCell.Footer && (pageStructStaticMemberCell.KeepWith || pageStructStaticMemberCell.RepeatWith))
				{
					if (pageBreaksAtEnd)
					{
						pageStructStaticMemberCell.KeepWith = false;
					}
					if (pageStructStaticMemberCell.KeepWith || pageStructStaticMemberCell.RepeatWith)
					{
						if (headerFooterRows == null)
						{
							headerFooterRows = new HeaderFooterRows();
						}
						headerFooterRows.AddFooterRow(rowIndex, pageStructStaticMemberCell.Span, pageStructStaticMemberCell.MemberInstance.SizeValue, onPage: false, pageStructStaticMemberCell.RepeatWith, pageStructStaticMemberCell.KeepWith, pageStructStaticMemberCell);
						footersKeepWith = pageStructStaticMemberCell.KeepWith;
					}
					rowIndex += pageStructStaticMemberCell.Span;
					continue;
				}
				return headerFooterRows;
			}
			lastMember = true;
			return headerFooterRows;
		}

		private bool ResolveKeepWithFooters(HeaderFooterRows prevFooters, HeaderFooterRows footers, PageMemberCell memberCell, double endInTablix, PageContext pageContext)
		{
			RoundedDouble roundedDouble = new RoundedDouble(prevFooters.KeepWithNoRepeatNoPageHeight);
			roundedDouble.Value += memberCell.EndPos;
			if (roundedDouble > endInTablix)
			{
				if (new RoundedDouble(memberCell.SizeValue) + prevFooters.KeepWithNoRepeatNoPageHeight <= pageContext.ColumnHeight)
				{
					return true;
				}
				if (footers != null)
				{
					RoundedDouble roundedDouble2 = new RoundedDouble(footers.KeepWithNoRepeatNoPageHeight);
					roundedDouble2.Value += memberCell.EndPos;
					if (roundedDouble2 > endInTablix && new RoundedDouble(memberCell.SizeValue) + footers.KeepWithNoRepeatNoPageHeight <= pageContext.ColumnHeight)
					{
						return true;
					}
				}
			}
			return false;
		}

		private HeaderFooterRows MergeHeadersFooters(HeaderFooterRows prevRows, HeaderFooterRows levelRows)
		{
			int num = 0;
			if (prevRows != null)
			{
				num += prevRows.Count;
			}
			if (levelRows != null)
			{
				num += levelRows.Count;
			}
			if (num > 0)
			{
				return new HeaderFooterRows(num, prevRows, levelRows);
			}
			return null;
		}

		private int BringHeadersFootersOnPage(HeaderFooterRows levelHeaders, HeaderFooterRows levelFooters)
		{
			int num = 0;
			if (levelHeaders != null)
			{
				num = levelHeaders.BringDetailRowOnPage(m_detailRows);
			}
			if (levelFooters != null)
			{
				num += levelFooters.BringDetailRowOnPage(m_detailRows);
			}
			return num;
		}

		private void NormalizeHeights(List<PageStructMemberCell> members, int index, int rowIndex, double startInTablix, double endInTablix)
		{
			if (members == null)
			{
				return;
			}
			int lastRowOnPage = rowIndex - 1;
			RowInfo rowInfo = null;
			for (int i = rowIndex; i < m_detailRows.Count; i++)
			{
				rowInfo = m_detailRows[i];
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
				{
					continue;
				}
				if (rowInfo.PageVerticalState != 0)
				{
					return;
				}
				if (i > rowIndex)
				{
					lastRowOnPage = -1;
				}
				break;
			}
			double addedHeight = 0.0;
			int lastRowBelow = -1;
			for (int j = index; j < members.Count; j++)
			{
				members[j].NormalizeDetailRowHeight(m_detailRows, rowIndex, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update: false, ref addedHeight);
				rowIndex += members[j].Span;
			}
			if (rowIndex < m_detailRows.Count)
			{
				MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.Unknown);
			}
		}

		private void NormalizeHeights(ScalableList<PageMemberCell> members, int index, int rowIndex, double startInTablix, double endInTablix)
		{
			if (members == null)
			{
				return;
			}
			int lastRowOnPage = rowIndex - 1;
			RowInfo rowInfo = null;
			for (int i = rowIndex; i < m_detailRows.Count; i++)
			{
				rowInfo = m_detailRows[i];
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
				{
					continue;
				}
				if (rowInfo.PageVerticalState != 0)
				{
					return;
				}
				if (i > rowIndex)
				{
					lastRowOnPage = -1;
				}
				break;
			}
			double addedHeight = 0.0;
			int lastRowBelow = -1;
			PageMemberCell item = null;
			for (int j = index; j < members.Count; j++)
			{
				using (members.GetAndPin(j, out item))
				{
					item.NormalizeDetailRowHeight(m_detailRows, rowIndex, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update: false, ref addedHeight);
					rowIndex += item.RowSpan;
				}
			}
			if (rowIndex < m_detailRows.Count)
			{
				MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.Unknown);
			}
		}

		private bool MoveKeepWithHeaders(List<PageStructMemberCell> members, ref int index, int rowIndex, double pageHeight, HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, HeaderFooterRows headers, bool leafsOnPage, bool prevFootersOnPage, PageContext pageContext)
		{
			if (members == null)
			{
				return false;
			}
			PageStructStaticMemberCell pageStructStaticMemberCell = null;
			PageStructDynamicMemberCell pageStructDynamicMemberCell = null;
			PageStructMemberCell pageStructMemberCell = null;
			HeaderFooterRows headerFooterRows = null;
			HeaderFooterRows headerFooterRows2 = null;
			int i = index;
			int num = -1;
			int rowIndex2 = rowIndex;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			for (; i < members.Count; i++)
			{
				pageStructStaticMemberCell = (members[i] as PageStructStaticMemberCell);
				if (pageStructStaticMemberCell != null)
				{
					if (pageStructStaticMemberCell.Header)
					{
						if (pageStructDynamicMemberCell != null || !pageStructStaticMemberCell.HasInstances)
						{
							flag = false;
							break;
						}
						if (!pageStructStaticMemberCell.KeepWith)
						{
							pageStructMemberCell = pageStructStaticMemberCell;
							rowIndex2 = rowIndex;
							num = i;
							if (i + 1 < members.Count)
							{
								flag = false;
							}
							break;
						}
						if (headerFooterRows == null)
						{
							headerFooterRows = new HeaderFooterRows();
						}
						headerFooterRows.AddHeaderRow(rowIndex, pageStructStaticMemberCell.Span, pageStructStaticMemberCell.SizeValue, onPage: false, pageStructStaticMemberCell.RepeatWith, keepWith: true, pageStructStaticMemberCell);
						if (headerFooterRows.Height > pageContext.ColumnHeight)
						{
							return false;
						}
						rowIndex += pageStructStaticMemberCell.Span;
						continue;
					}
					if (pageStructDynamicMemberCell == null)
					{
						if (!pageStructStaticMemberCell.HasInstances)
						{
							flag = false;
							break;
						}
						pageStructMemberCell = pageStructStaticMemberCell;
						rowIndex2 = rowIndex;
						num = i;
						if (i + 1 < members.Count)
						{
							flag = false;
						}
						break;
					}
					if (!pageStructStaticMemberCell.Footer || (!pageStructStaticMemberCell.KeepWith && !pageStructStaticMemberCell.RepeatWith))
					{
						flag = false;
						break;
					}
					if (pageStructDynamicMemberCell.PartialItem)
					{
						flag3 = false;
					}
					else
					{
						if (flag2)
						{
							pageStructStaticMemberCell.KeepWith = false;
						}
						flag3 = pageStructStaticMemberCell.KeepWith;
					}
					if (flag3 || pageStructStaticMemberCell.RepeatWith)
					{
						if (headerFooterRows2 == null)
						{
							headerFooterRows2 = new HeaderFooterRows();
						}
						headerFooterRows2.AddFooterRow(rowIndex, pageStructStaticMemberCell.Span, pageStructStaticMemberCell.SizeValue, onPage: false, pageStructStaticMemberCell.RepeatWith, flag3, pageStructStaticMemberCell);
					}
					rowIndex += pageStructStaticMemberCell.Span;
				}
				else
				{
					if (pageStructDynamicMemberCell != null)
					{
						flag = false;
						break;
					}
					pageStructMemberCell = members[i];
					rowIndex2 = rowIndex;
					num = i;
					pageStructDynamicMemberCell = (pageStructMemberCell as PageStructDynamicMemberCell);
					rowIndex += pageStructMemberCell.Span;
					flag2 = m_detailRows[rowIndex - 1].PageBreaksAtEnd;
				}
			}
			if (pageStructMemberCell == null || pageStructMemberCell.Hidden)
			{
				return false;
			}
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			if (!pageStructMemberCell.SpanPages && !pageStructMemberCell.PartialItem)
			{
				roundedDouble.Value = pageStructMemberCell.SizeValue;
				if (headerFooterRows != null)
				{
					roundedDouble.Value += headerFooterRows.Height;
				}
				if (headerFooterRows2 != null)
				{
					roundedDouble.Value += headerFooterRows2.Height;
				}
				if (roundedDouble <= pageContext.ColumnHeight)
				{
					RoundedDouble roundedDouble2 = new RoundedDouble(roundedDouble.Value);
					roundedDouble2.Value += SpaceNeededForThisPage(prevHeaders, prevFooters, headers, flag, leafsOnPage);
					if (roundedDouble2 <= pageHeight)
					{
						index = num;
						if (headerFooterRows2 != null)
						{
							index += headerFooterRows2.Count;
						}
						return false;
					}
				}
			}
			PageMemberCell pageMemberCell = null;
			bool flag4 = false;
			if (pageStructDynamicMemberCell != null)
			{
				pageMemberCell = pageStructDynamicMemberCell.MemberInstances[0];
				if (pageStructDynamicMemberCell.PartialItem || pageStructDynamicMemberCell.MemberInstances.Count > 1)
				{
					headerFooterRows2?.SetRowsKeepWith(keepWith: false);
				}
				if (pageStructDynamicMemberCell.PartialItem && !pageStructDynamicMemberCell.CreateItem)
				{
					flag4 = true;
				}
			}
			else
			{
				pageMemberCell = pageStructStaticMemberCell.MemberInstance;
			}
			if (pageMemberCell.Children != null)
			{
				bool flag5 = false;
				int index2 = 0;
				HeaderFooterRows headerFooterRows3 = null;
				HeaderFooterRows headerFooterRows4 = null;
				if (headerFooterRows2 != null && prevFootersOnPage && ResolveFooters(headerFooterRows2, ref pageHeight, pageHeight, flag, ref prevFootersOnPage, pageContext))
				{
					return true;
				}
				headerFooterRows4 = new HeaderFooterRows(prevFooters);
				headerFooterRows4.AddClone(headerFooterRows2);
				if (headerFooterRows4.Count == 0)
				{
					headerFooterRows4 = null;
				}
				if (headerFooterRows != null)
				{
					pageHeight -= headerFooterRows.Height;
					headerFooterRows.SetRowsOnPage(onPage: true);
				}
				headerFooterRows3 = new HeaderFooterRows(prevHeaders);
				headerFooterRows3.AddClone(headers);
				headerFooterRows3.AddClone(headerFooterRows);
				if (headerFooterRows3.Count == 0)
				{
					headerFooterRows3 = null;
				}
				RoundedDouble roundedDouble3 = new RoundedDouble(pageMemberCell.ContentHeight);
				if (roundedDouble3 > pageHeight)
				{
					int num2 = MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
					index = num - num2;
					return true;
				}
				roundedDouble3.Value = pageMemberCell.SizeValue;
				if (!flag4 && !pageMemberCell.SpanPages)
				{
					if (roundedDouble3 < pageHeight)
					{
						double endInTablix = pageHeight - pageMemberCell.SizeValue;
						bool flag6 = false;
						if (!prevFootersOnPage && headerFooterRows2 != null)
						{
							flag6 = ResolveFooters(headerFooterRows2, ref endInTablix, endInTablix, flag, ref prevFootersOnPage, pageContext);
						}
						if (!flag6)
						{
							endInTablix -= SpaceNeededForThisPage(prevHeaders, prevFooters, headers, flag, leafsOnPage);
							if (endInTablix >= 0.0)
							{
								index = num;
								return false;
							}
						}
					}
					else if (pageMemberCell.KeepTogether)
					{
						int num3 = MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
						TraceKeepTogetherRowMember(pageContext, (pageStructDynamicMemberCell != null) ? ((PageStructMemberCell)pageStructDynamicMemberCell) : ((PageStructMemberCell)pageStructStaticMemberCell));
						index = num - num3;
						return true;
					}
				}
				if (pageStructDynamicMemberCell != null)
				{
					using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(0, out pageMemberCell))
					{
						flag5 = MoveKeepWithHeaders(pageMemberCell.Children, ref index2, rowIndex2, pageHeight, headerFooterRows3, headerFooterRows4, null, leafsOnPage, prevFootersOnPage, pageContext);
					}
				}
				else
				{
					flag5 = MoveKeepWithHeaders(pageMemberCell.Children, ref index2, rowIndex2, pageHeight, headerFooterRows3, headerFooterRows4, null, leafsOnPage, prevFootersOnPage, pageContext);
				}
				if (flag5 && index2 == 0)
				{
					int num4 = MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
					index = num - num4;
					return true;
				}
				index = num;
				return flag5;
			}
			if (pageMemberCell.SpanPages)
			{
				return false;
			}
			if (headerFooterRows != null)
			{
				pageHeight -= headerFooterRows.Height;
				headerFooterRows.SetRowsOnPage(onPage: true);
			}
			if (headerFooterRows2 != null && prevFootersOnPage && ResolveFooters(headerFooterRows2, ref pageHeight, pageHeight, flag, ref prevFootersOnPage, pageContext))
			{
				return true;
			}
			if (new RoundedDouble(pageMemberCell.SizeValue) > pageHeight)
			{
				int num5 = MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
				index = num - num5;
				return true;
			}
			double endInTablix2 = pageHeight - pageMemberCell.SizeValue;
			bool flag7 = false;
			bool flag8 = false;
			if (!prevFootersOnPage && headerFooterRows2 != null)
			{
				flag7 = ResolveFooters(headerFooterRows2, ref endInTablix2, endInTablix2, flag, ref prevFootersOnPage, pageContext);
			}
			if (!flag7)
			{
				endInTablix2 -= SpaceNeededForThisPage(prevHeaders, prevFooters, headers, flag, leafsOnPage);
				if (endInTablix2 >= 0.0)
				{
					index = num;
					return false;
				}
			}
			else
			{
				flag8 = true;
			}
			endInTablix2 = pageContext.ColumnHeight - pageMemberCell.SizeValue;
			prevFootersOnPage = false;
			if (headerFooterRows != null)
			{
				endInTablix2 -= headerFooterRows.Height;
			}
			if (flag)
			{
				flag7 = ResolveFooters(headerFooterRows2, ref endInTablix2, endInTablix2, flag, ref prevFootersOnPage, pageContext);
			}
			if (!flag7)
			{
				if (flag8 && !flag && !ResolveFooters(headerFooterRows2, ref endInTablix2, endInTablix2, flag, ref prevFootersOnPage, pageContext))
				{
					return true;
				}
				if (headers != null)
				{
					endInTablix2 -= headers.Height;
				}
				endInTablix2 -= SpaceNeededForFullPage(prevHeaders, prevFooters, flag);
				if (endInTablix2 <= pageContext.ColumnHeight)
				{
					return true;
				}
			}
			return false;
		}

		private int MoveToNextPageWithHeaders(PageContext pageContext, PageMemberCell memberInstance, HeaderFooterRows currHeaders, HeaderFooterRows headers, HeaderFooterRows prevHeaders)
		{
			double delta = pageContext.ColumnHeight - memberInstance.SizeValue;
			int result = 0;
			if (currHeaders != null)
			{
				result = currHeaders.MoveToNextPage(ref delta);
			}
			if (delta > 0.0 && headers == null)
			{
				prevHeaders?.MoveToNextPage(ref delta);
			}
			return result;
		}

		private double SpaceNeededForThisPage(HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, HeaderFooterRows headers, bool lastMember, bool leafsOnPage)
		{
			double num = 0.0;
			if (leafsOnPage)
			{
				if (prevFooters != null && lastMember)
				{
					num += prevFooters.KeepWithNoRepeatNoPageHeight;
				}
			}
			else
			{
				if (headers != null)
				{
					num += headers.RepeatNotOnPageHeight;
				}
				if (prevHeaders != null)
				{
					num += prevHeaders.RepeatNotOnPageHeight;
				}
				if (prevFooters != null)
				{
					num = ((!lastMember) ? (num + prevFooters.RepeatHeight) : (num + prevFooters.KeepWithAndRepeatHeight));
				}
			}
			return num;
		}

		private double SpaceNeededForFullPage(HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, bool lastMember)
		{
			double num = 0.0;
			if (prevHeaders != null)
			{
				num += prevHeaders.RepeatHeight;
			}
			if (prevFooters != null)
			{
				num = ((!lastMember) ? (num + prevFooters.RepeatHeight) : (num + prevFooters.KeepWithAndRepeatHeight));
			}
			return num;
		}

		private void UpdateColumns(double startInTablix, double endInTablix)
		{
			if (m_columnHeaders != null && m_columnInfo != null && m_columnInfo.Count != 0)
			{
				PageStructMemberCell pageStructMemberCell = null;
				int num = 0;
				for (int i = 0; i < m_columnHeaders.Count; i++)
				{
					pageStructMemberCell = m_columnHeaders[i];
					pageStructMemberCell.UpdateColumns(m_columnInfo, num, startInTablix, endInTablix);
					num += pageStructMemberCell.Span;
				}
			}
		}

		private void AlignCornerCellsToPageHorizontally(int targetColIndex, double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (m_cornerCells == null || m_colHeaderHeights == null || !AddToPageColumnHeaders)
			{
				return;
			}
			PageCornerCell pageCornerCell = null;
			bool flag = false;
			for (int i = 0; i < m_colHeaderHeights.Count; i++)
			{
				for (int j = 0; j < m_headerRowCols; j++)
				{
					pageCornerCell = m_cornerCells[i, j];
					if (pageCornerCell == null || j > targetColIndex || j + pageCornerCell.ColSpan <= targetColIndex)
					{
						continue;
					}
					flag = false;
					int num = 0;
					while (!flag && num < pageCornerCell.RowSpan)
					{
						if (m_colHeaderHeights[i + num].State == SizeInfo.PageState.Normal)
						{
							pageCornerCell.AlignToPageHorizontal(j, targetColIndex, startInTablix, endInTablix, m_rowHeaderWidths, IsLTR, pageContext);
							flag = true;
						}
						num++;
					}
				}
			}
		}

		private void CornerRowsHorizontalSize(PageContext pageContext)
		{
			if (m_cornerCells == null || m_colHeaderHeights == null || !AddToPageColumnHeaders)
			{
				return;
			}
			int num = 0;
			PageCornerCell pageCornerCell = null;
			for (int i = 0; i < m_colHeaderHeights.Count; i++)
			{
				if (m_colHeaderHeights[i].State != SizeInfo.PageState.Normal)
				{
					continue;
				}
				num = 0;
				while (num < m_headerRowCols)
				{
					pageCornerCell = m_cornerCells[i, num];
					if (pageCornerCell != null)
					{
						pageCornerCell.CalculateHorizontal(num, ref m_rowHeaderWidths, pageContext);
						num += pageCornerCell.ColSpan;
					}
					else
					{
						num++;
					}
				}
			}
		}

		private bool TryRepeatRowHeaders(double startInTablix, double endInTablix, ref int colIndex)
		{
			if (m_columnInfo == null || m_columnInfo.Count == 0)
			{
				return false;
			}
			ColumnInfo columnInfo = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			for (int i = 0; i < m_columnInfo.Count; i++)
			{
				columnInfo = m_columnInfo[i];
				if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					continue;
				}
				roundedDouble.Value = columnInfo.Right;
				if (roundedDouble > startInTablix)
				{
					roundedDouble.Value = columnInfo.Left;
					if (roundedDouble < startInTablix)
					{
						return false;
					}
					colIndex = i;
					return true;
				}
			}
			return false;
		}

		private void CalculateContentHorizontally(double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (startInTablix == 0.0 && m_columnInfo == null)
			{
				double num = 0.0;
				double startPos = startInTablix;
				CornerRowsHorizontalSize(pageContext);
				if (m_rowHeaders != null)
				{
					int num2 = 0;
					for (int i = 0; i < m_rowHeaders.Count; i++)
					{
						m_rowHeaders[i].CalculateRHHorizontal(m_detailRows, num2, 0, ref m_rowHeaderWidths, pageContext);
						num2 += m_rowHeaders[i].Span;
					}
				}
				ResolveSizes(m_rowHeaderWidths);
				if (m_headerRowCols > 0 && m_rowHeaderWidths != null)
				{
					for (int j = 0; j < m_rowHeaderWidths.Count; j++)
					{
						num += m_rowHeaderWidths[j].SizeValue;
					}
				}
				m_columnInfo = new ScalableList<ColumnInfo>(0, pageContext.ScalabilityCache);
				if (m_columnHeaders != null && AddToPageColumnHeaders)
				{
					PageContext pageContext2 = pageContext;
					if (RepeatedColumnHeaders && !pageContext.ResetHorizontal)
					{
						pageContext2 = new PageContext(pageContext);
						pageContext2.ResetHorizontal = true;
					}
					int num3 = 0;
					for (int k = 0; k < m_columnHeaders.Count; k++)
					{
						m_columnHeaders[k].CalculateCHHorizontal(m_colHeaderHeights, 0, num3, m_columnInfo, pageContext2, AddToPageColumnHeaders);
						num3 += m_columnHeaders[k].Span;
					}
				}
				if (m_detailRows != null)
				{
					for (int l = 0; l < m_detailRows.Count; l++)
					{
						m_detailRows[l].CalculateHorizontal(m_columnInfo, pageContext);
					}
				}
				ResolveSizes(m_columnInfo);
				if (m_columnInfo != null && m_columnInfo.Count > 0)
				{
					bool flag = true;
					if (m_columnHeaders != null)
					{
						int num4 = 0;
						int lastColOnPage = -1;
						int lastColBelow = -1;
						double addedWidth = 0.0;
						for (int m = 0; m < m_columnHeaders.Count; m++)
						{
							if (num4 == m_colsBeforeRowHeaders && (IsLTR || num4 > 0))
							{
								if (num4 > 0)
								{
									startInTablix += m_columnInfo[num4 - 1].Right;
									startPos = startInTablix;
								}
								startInTablix += num;
								lastColOnPage = -1;
								lastColBelow = -1;
								flag = false;
							}
							m_columnHeaders[m].NormalizeDetailColWidth(m_columnInfo, num4, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update: false, ref addedWidth);
							num4 += m_columnHeaders[m].Span;
						}
						if (num4 == m_colsBeforeRowHeaders && num4 > 0)
						{
							startInTablix += m_columnInfo[num4 - 1].Right;
							startPos = startInTablix;
						}
					}
					if (flag)
					{
						base.ItemPageSizes.AdjustWidthTo(m_columnInfo[m_columnInfo.Count - 1].Right + num);
					}
					else
					{
						base.ItemPageSizes.AdjustWidthTo(m_columnInfo[m_columnInfo.Count - 1].Right);
					}
				}
				else
				{
					base.ItemPageSizes.AdjustWidthTo(num);
				}
				if (IsLTR)
				{
					ResolveStartPosAndState(m_rowHeaderWidths, startPos, SizeInfo.PageState.Normal);
				}
				else
				{
					ResolveStartPosAndStateRTL(m_rowHeaderWidths, startPos, SizeInfo.PageState.Normal);
				}
				return;
			}
			NormalizeColHeadersWidths(startInTablix, endInTablix, update: false);
			if (m_colsBeforeRowHeaders <= 0)
			{
				return;
			}
			bool flag2 = false;
			if (m_columnInfo != null && m_columnInfo.Count > 0)
			{
				if (m_columnInfo[m_colsBeforeRowHeaders - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft && m_colsBeforeRowHeaders == m_columnInfo.Count)
				{
					flag2 = true;
				}
			}
			else
			{
				flag2 = true;
			}
			if (flag2)
			{
				double amount = 0.0;
				if (m_rowHeaderWidths != null)
				{
					amount = (SplitRowHeaders ? ((!IsLTR) ? m_rowHeaderWidths[0].EndPos : m_rowHeaderWidths[m_rowHeaderWidths.Count - 1].EndPos) : ((!IsLTR) ? ResolveStartPosAndStateRTL(m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal) : ResolveStartPosAndState(m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal)));
				}
				base.ItemPageSizes.AdjustWidthTo(amount);
			}
		}

		private int FindRHColumnSpanHorizontal(double startInTablix, double endInTablix, out int firstColIndex, ref double rowHeadersWidth)
		{
			int num = 0;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			if (IsLTR)
			{
				firstColIndex = 0;
				for (int i = 0; i < m_rowHeaderWidths.Count; i++)
				{
					roundedDouble.Value = m_rowHeaderWidths[i].EndPos;
					if (roundedDouble <= startInTablix)
					{
						m_rowHeaderWidths[i].State = SizeInfo.PageState.Skip;
						num++;
						firstColIndex++;
						continue;
					}
					rowHeadersWidth += m_rowHeaderWidths[i].SizeValue;
					rowHeadersWidth -= Math.Max(0.0, startInTablix - m_rowHeaderWidths[i].StartPos);
					roundedDouble.Value = startInTablix + rowHeadersWidth;
					if (roundedDouble < endInTablix)
					{
						m_rowHeaderWidths[i].State = SizeInfo.PageState.Normal;
						num++;
					}
				}
				if (num == m_rowHeaderWidths.Count)
				{
					num--;
				}
			}
			else
			{
				num = (firstColIndex = m_rowHeaderWidths.Count - 1);
				for (int num2 = num; num2 >= 0; num2--)
				{
					roundedDouble.Value = m_rowHeaderWidths[num2].EndPos;
					if (roundedDouble <= startInTablix)
					{
						m_rowHeaderWidths[num2].State = SizeInfo.PageState.Skip;
						num--;
						firstColIndex--;
					}
					else
					{
						rowHeadersWidth += m_rowHeaderWidths[num2].SizeValue;
						rowHeadersWidth -= Math.Max(0.0, startInTablix - m_rowHeaderWidths[num2].StartPos);
						roundedDouble.Value = startInTablix + rowHeadersWidth;
						if (roundedDouble < endInTablix)
						{
							m_rowHeaderWidths[num2].State = SizeInfo.PageState.Normal;
							num--;
						}
					}
				}
				if (num < 0)
				{
					num = 0;
				}
			}
			return num;
		}

		private void MarkColumnHorizontalState(int colIndex, ColumnInfo.HorizontalState state)
		{
			ColumnInfo item = null;
			using (m_columnInfo.GetAndPin(colIndex, out item))
			{
				item.PageHorizontalState = state;
			}
		}

		private void MarkSplitRowHeaders(double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (m_rowHeaderWidths == null || m_rowHeaderWidths.Count == 0)
			{
				return;
			}
			double rowHeadersWidth = 0.0;
			int firstColIndex = 0;
			int num = FindRHColumnSpanHorizontal(startInTablix, endInTablix, out firstColIndex, ref rowHeadersWidth);
			if (new RoundedDouble(startInTablix + rowHeadersWidth) <= endInTablix)
			{
				if (m_columnInfo != null && m_columnInfo.Count > 0)
				{
					AlignSplitRowHeaders(firstColIndex, num, startInTablix, endInTablix, pageContext, normalize: false);
					if (m_colsBeforeRowHeaders > 0)
					{
						if (m_colsBeforeRowHeaders < m_columnInfo.Count)
						{
							MarkColumnHorizontalState(m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.LeftOfNextPage);
							NormalizeColHeadersWidths(startInTablix, endInTablix, update: false);
						}
						else
						{
							base.ItemPageSizes.AdjustWidthTo(m_rowHeaderWidths[num].EndPos);
						}
					}
					else
					{
						MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.LeftOfNextPage);
						NormalizeColHeadersWidths(startInTablix, endInTablix, update: false);
					}
				}
				else
				{
					base.ItemPageSizes.AdjustWidthTo(m_rowHeaderWidths[num].EndPos);
				}
				SplitRowHeaders = false;
			}
			else
			{
				AlignSplitRowHeaders(firstColIndex, num, startInTablix, endInTablix, pageContext, normalize: true);
			}
		}

		private void AlignSplitRowHeaders(int firstColIndex, int lastColIndex, double startInTablix, double endInTablix, PageContext pageContext, bool normalize)
		{
			m_rowHeaderWidths[lastColIndex].State = SizeInfo.PageState.Normal;
			if (m_rowHeaders != null)
			{
				for (int i = 0; i < m_rowHeaders.Count; i++)
				{
					m_rowHeaders[i].AlignRHToPageHorizontal(m_detailRows, 0, 0, lastColIndex, startInTablix, endInTablix, m_rowHeaderWidths, IsLTR, pageContext);
				}
			}
			AlignCornerCellsToPageHorizontally(lastColIndex, startInTablix, endInTablix, pageContext);
			ResolveSizes(m_rowHeaderWidths);
			int num = m_rowHeaderWidths.Count - 1;
			double num2 = 0.0;
			if (IsLTR)
			{
				ResolveStartPos(m_rowHeaderWidths, m_rowHeaderWidths[0].StartPos);
				for (int j = 0; j < firstColIndex; j++)
				{
					m_rowHeaderWidths[j].State = SizeInfo.PageState.Skip;
				}
				for (int k = firstColIndex; k <= lastColIndex; k++)
				{
					m_rowHeaderWidths[k].State = SizeInfo.PageState.Normal;
				}
				for (int l = lastColIndex + 1; l < m_rowHeaderWidths.Count; l++)
				{
					m_rowHeaderWidths[l].State = SizeInfo.PageState.Skip;
				}
				num2 = m_rowHeaderWidths[num].EndPos;
			}
			else
			{
				ResolveStartPosRTL(m_rowHeaderWidths, m_rowHeaderWidths[num].StartPos);
				for (int m = 0; m < lastColIndex; m++)
				{
					m_rowHeaderWidths[m].State = SizeInfo.PageState.Skip;
				}
				for (int n = lastColIndex; n <= firstColIndex; n++)
				{
					m_rowHeaderWidths[n].State = SizeInfo.PageState.Normal;
				}
				for (int num3 = firstColIndex + 1; num3 <= num; num3++)
				{
					m_rowHeaderWidths[num3].State = SizeInfo.PageState.Skip;
				}
				num2 = m_rowHeaderWidths[0].EndPos;
			}
			if (!normalize)
			{
				return;
			}
			if (m_columnInfo != null && m_columnInfo.Count > 0)
			{
				if (m_colsBeforeRowHeaders > 0)
				{
					if (m_colsBeforeRowHeaders < m_columnInfo.Count)
					{
						MarkColumnHorizontalState(m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.Normal);
						m_columnInfo[m_colsBeforeRowHeaders].Left = num2;
						NormalizeColHeadersWidths(startInTablix, endInTablix, update: true);
					}
					else
					{
						base.ItemPageSizes.AdjustWidthTo(num2);
					}
				}
				else
				{
					MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.Normal);
					m_columnInfo[m_colsBeforeRowHeaders].Left = num2;
					NormalizeColHeadersWidths(startInTablix, endInTablix, update: true);
				}
			}
			else
			{
				base.ItemPageSizes.AdjustWidthTo(num2);
			}
		}

		private void MarkTablixRTLColumnsForHorizontalPage(double startInTablix, double endInTablix, PageContext pageContext)
		{
			double num = 0.0;
			bool flag = false;
			bool flag2 = true;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			if (m_headerRowCols > 0 && m_rowHeaderWidths != null)
			{
				for (int i = 0; i < m_rowHeaderWidths.Count; i++)
				{
					num += m_rowHeaderWidths[i].SizeValue;
				}
				roundedDouble.Value = startInTablix + num;
				if (roundedDouble < endInTablix)
				{
					flag = true;
					if (startInTablix == 0.0)
					{
						roundedDouble.Value = endInTablix;
						if (roundedDouble == pageContext.ColumnWidth || PinnedToParentCell)
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = false;
					}
				}
				else if (startInTablix == 0.0)
				{
					if (PinnedToParentCell)
					{
						flag2 = false;
					}
					else
					{
						roundedDouble.Value = num;
						if (roundedDouble < pageContext.ColumnWidth)
						{
							base.ItemPageSizes.MoveHorizontal(endInTablix);
							return;
						}
					}
				}
				else
				{
					flag2 = false;
				}
			}
			else if (PinnedToParentCell)
			{
				flag2 = false;
			}
			int colsOnPage = 0;
			bool allowSpanAtRight = true;
			double num2 = endInTablix;
			if (flag)
			{
				colsOnPage = 1;
				allowSpanAtRight = false;
				num2 -= num;
			}
			if (MarkDetailColsForHorizontalPage(m_columnHeaders, startInTablix, num2, endInTablix, 0, colsOnPage, 0, m_colsBeforeRowHeaders, allowSpanAtRight, pageContext) == 0)
			{
				if (flag2)
				{
					if (m_columnInfo != null && m_columnInfo.Count > 0)
					{
						MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.Normal);
					}
					base.ItemPageSizes.MoveHorizontal(endInTablix);
					return;
				}
				if (flag)
				{
					flag = false;
					if (m_columnInfo != null && m_columnInfo.Count > 0)
					{
						for (int j = 0; j < m_columnInfo.Count; j++)
						{
							if (m_columnInfo[j].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
							{
								MarkColumnHorizontalState(j, ColumnInfo.HorizontalState.Normal);
								break;
							}
						}
					}
					colsOnPage = MarkDetailColsForHorizontalPage(m_columnHeaders, startInTablix, endInTablix, endInTablix, 0, 0, 0, m_colsBeforeRowHeaders, allowSpanAtRight: true, pageContext);
				}
			}
			NormalizeColHeadersWidths(startInTablix, endInTablix, update: true);
			if (flag)
			{
				AddToPageRowHeaders = true;
				double num3 = startInTablix;
				int num4 = m_colsBeforeRowHeaders - 1;
				ColumnInfo columnInfo = null;
				while (num4 >= 0)
				{
					columnInfo = m_columnInfo[num4];
					if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal)
					{
						num3 = columnInfo.Right;
						break;
					}
					num4--;
				}
				if (IsLTR)
				{
					ResolveStartPosAndState(m_rowHeaderWidths, num3, SizeInfo.PageState.Normal);
				}
				else
				{
					ResolveStartPosAndStateRTL(m_rowHeaderWidths, num3, SizeInfo.PageState.Normal);
				}
				columnInfo = m_columnInfo[m_colsBeforeRowHeaders - 1];
				if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal)
				{
					if (m_colsBeforeRowHeaders < m_columnInfo.Count)
					{
						MarkColumnHorizontalState(m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.Unknown);
						NormalizeColHeadersWidths(num3 + num, endInTablix, update: false, m_colsBeforeRowHeaders);
						MarkTablixLTRColumnsForHorizontalPage(num3, endInTablix, rowHeadersOnPageInFlow: true, pageContext);
					}
					else
					{
						base.ItemPageSizes.AdjustWidthTo(columnInfo.Right + num);
					}
				}
				return;
			}
			AddToPageRowHeaders = false;
			if (IsLTR)
			{
				ResolveStartPosAndState(m_rowHeaderWidths, endInTablix, SizeInfo.PageState.Skip);
			}
			else
			{
				ResolveStartPosAndStateRTL(m_rowHeaderWidths, endInTablix, SizeInfo.PageState.Skip);
			}
			if (m_columnInfo[m_colsBeforeRowHeaders - 1].PageHorizontalState == ColumnInfo.HorizontalState.Normal)
			{
				if (m_colsBeforeRowHeaders < m_columnInfo.Count)
				{
					MarkColumnHorizontalState(m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.LeftOfNextPage);
					NormalizeColHeadersWidths(startInTablix, endInTablix + num, update: false);
				}
				else if (num > 0.0)
				{
					base.ItemPageSizes.AdjustWidthTo(endInTablix + num);
				}
			}
		}

		private void MarkTablixLTRColumnsForHorizontalPage(double startInTablix, double endInTablix, bool rowHeadersOnPageInFlow, PageContext pageContext)
		{
			double rowHeadersWidth = 0.0;
			bool flag = false;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			int colsOnPage = 0;
			if (rowHeadersOnPageInFlow)
			{
				colsOnPage = 1;
			}
			if (startInTablix == 0.0 || rowHeadersOnPageInFlow)
			{
				if (m_headerRowCols > 0 && m_rowHeaderWidths != null)
				{
					int firstColIndex = 0;
					int lastColIndex = FindRHColumnSpanHorizontal(startInTablix, endInTablix, out firstColIndex, ref rowHeadersWidth);
					if (rowHeadersWidth > 0.0)
					{
						colsOnPage = 1;
					}
					roundedDouble.Value = startInTablix + rowHeadersWidth;
					if (!(roundedDouble <= endInTablix))
					{
						roundedDouble.Value = rowHeadersWidth;
						if (!rowHeadersOnPageInFlow && roundedDouble <= pageContext.ColumnWidth && !PinnedToParentCell)
						{
							base.ItemPageSizes.MoveHorizontal(endInTablix);
							return;
						}
						SplitRowHeaders = true;
						RepeatRowHeaders = false;
						AlignSplitRowHeaders(firstColIndex, lastColIndex, startInTablix, endInTablix, pageContext, normalize: true);
						return;
					}
					roundedDouble.Value = endInTablix;
					if (rowHeadersOnPageInFlow || roundedDouble == pageContext.ColumnWidth || PinnedToParentCell)
					{
						startInTablix += rowHeadersWidth;
						rowHeadersWidth = 0.0;
					}
					else
					{
						flag = true;
					}
				}
				else if (!rowHeadersOnPageInFlow)
				{
					roundedDouble.Value = endInTablix;
					if (!(roundedDouble == pageContext.ColumnWidth) && !PinnedToParentCell)
					{
						colsOnPage = 1;
						flag = true;
					}
				}
			}
			else
			{
				if (SplitRowHeaders)
				{
					MarkSplitRowHeaders(startInTablix, endInTablix, pageContext);
					return;
				}
				if (!rowHeadersOnPageInFlow)
				{
					if (RepeatRowHeaders && m_rowHeaderWidths != null)
					{
						int colIndex = 0;
						if (TryRepeatRowHeaders(startInTablix, endInTablix, ref colIndex))
						{
							for (int i = 0; i < m_rowHeaderWidths.Count; i++)
							{
								rowHeadersWidth += m_rowHeaderWidths[i].SizeValue;
							}
							for (int j = colIndex; j < m_columnInfo.Count; j++)
							{
								if (m_columnInfo[j].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
								{
									MarkColumnHorizontalState(j, ColumnInfo.HorizontalState.Unknown);
									break;
								}
							}
							if (rowHeadersWidth > 0.0)
							{
								colsOnPage = 1;
							}
						}
						else
						{
							AddToPageRowHeaders = false;
						}
					}
					else
					{
						AddToPageRowHeaders = false;
					}
				}
			}
			if (MarkDetailColsForHorizontalPage(m_columnHeaders, startInTablix + rowHeadersWidth, endInTablix, endInTablix, 0, colsOnPage, 0, 0, allowSpanAtRight: true, pageContext) == 0)
			{
				if (flag)
				{
					if (m_columnInfo != null && m_columnInfo.Count > 0)
					{
						MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.Normal);
					}
					base.ItemPageSizes.MoveHorizontal(endInTablix);
					return;
				}
				if (rowHeadersOnPageInFlow)
				{
					MarkColumnHorizontalState(m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.LeftOfNextPage);
				}
				else if (rowHeadersWidth > 0.0)
				{
					for (int k = 0; k < m_columnInfo.Count; k++)
					{
						if (m_columnInfo[k].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
						{
							MarkColumnHorizontalState(k, ColumnInfo.HorizontalState.Unknown);
							break;
						}
					}
					colsOnPage = MarkDetailColsForHorizontalPage(m_columnHeaders, startInTablix, endInTablix, endInTablix, 0, 0, 0, 0, allowSpanAtRight: true, pageContext);
					AddToPageRowHeaders = false;
					rowHeadersWidth = 0.0;
				}
			}
			if (AddToPageRowHeaders && rowHeadersWidth > 0.0 && !flag)
			{
				if (IsLTR)
				{
					ResolveStartPosAndState(m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal);
				}
				else
				{
					ResolveStartPosAndStateRTL(m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal);
				}
			}
			NormalizeColHeadersWidths(startInTablix + rowHeadersWidth, endInTablix, update: true);
		}

		private void MarkTablixColumnsForHorizontalPage(double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (m_colsBeforeRowHeaders > 0)
			{
				if (m_columnInfo[m_colsBeforeRowHeaders - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
				{
					MarkTablixRTLColumnsForHorizontalPage(startInTablix, endInTablix, pageContext);
				}
				else if (m_colsBeforeRowHeaders < m_columnInfo.Count)
				{
					bool rowHeadersOnPageInFlow = false;
					if (startInTablix > 0.0 && !SplitRowHeaders && m_rowHeaderWidths != null)
					{
						double num = 0.0;
						num = ((!IsLTR) ? m_rowHeaderWidths[m_rowHeaderWidths.Count - 1].StartPos : m_rowHeaderWidths[0].StartPos);
						if (num == startInTablix)
						{
							rowHeadersOnPageInFlow = true;
						}
					}
					MarkTablixLTRColumnsForHorizontalPage(startInTablix, endInTablix, rowHeadersOnPageInFlow, pageContext);
				}
				else
				{
					SplitRowHeaders = true;
					MarkSplitRowHeaders(startInTablix, endInTablix, pageContext);
				}
			}
			else
			{
				MarkTablixLTRColumnsForHorizontalPage(startInTablix, endInTablix, rowHeadersOnPageInFlow: false, pageContext);
			}
		}

		private int MarkDetailColsForHorizontalPage(List<PageStructMemberCell> members, double startInTablix, double endInTablix, double realEndInTablix, int colIndex, int colsOnPage, int spanContent, int stopIndex, bool allowSpanAtRight, PageContext pageContext)
		{
			if (members == null)
			{
				if (m_detailRows != null)
				{
					bool flag = false;
					ColumnInfo item = null;
					using (m_columnInfo.GetAndPin(colIndex, out item))
					{
						if (item.Unresolved)
						{
							flag = true;
							item.Unresolved = false;
						}
					}
					if (flag)
					{
						for (int i = 0; i < m_detailRows.Count; i++)
						{
							m_detailRows[i].ResolveHorizontal(m_columnInfo, colIndex, startInTablix, realEndInTablix, pageContext);
						}
					}
				}
				return 1;
			}
			int num = 0;
			int num2 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble5 = new RoundedDouble(0.0);
			while (num < members.Count)
			{
				NormalizeWidths(members, num, colIndex, startInTablix, endInTablix);
				pageStructMemberCell = members[num];
				roundedDouble.Value = pageStructMemberCell.StartPos;
				roundedDouble2.Value = pageStructMemberCell.EndPos;
				if (stopIndex > 0 && colIndex >= stopIndex)
				{
					return num2;
				}
				if (roundedDouble >= endInTablix || m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.LeftOfNextPage)
				{
					return num2;
				}
				if (roundedDouble2 <= startInTablix)
				{
					colIndex += pageStructMemberCell.Span;
					num++;
					continue;
				}
				if (pageStructMemberCell is PageStructStaticMemberCell)
				{
					PageStructStaticMemberCell pageStructStaticMemberCell = (PageStructStaticMemberCell)pageStructMemberCell;
					PageMemberCell memberInstance = pageStructStaticMemberCell.MemberInstance;
					roundedDouble3.Value = memberInstance.StartPos;
					roundedDouble4.Value = memberInstance.EndPos;
					roundedDouble5.Value = memberInstance.ContentRight;
					if (roundedDouble3 < startInTablix)
					{
						if (memberInstance.MemberItem != null)
						{
							spanContent++;
						}
						bool num3 = MarkSpanCHMemberOnPage(memberInstance, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
						num2 += memberInstance.CurrColSpan;
						if (num3)
						{
							return num2;
						}
						colsOnPage += memberInstance.CurrColSpan;
						colIndex += memberInstance.ColSpan;
						num++;
						continue;
					}
					if (colsOnPage > 0 && spanContent == 0)
					{
						bool flag2 = false;
						ColumnInfo columnInfo = m_columnInfo[colIndex];
						if (!columnInfo.BlockedBySpan)
						{
							if (roundedDouble5 > endInTablix)
							{
								flag2 = true;
							}
							else if (roundedDouble4 > endInTablix && new RoundedDouble(memberInstance.SizeValue) <= pageContext.ColumnWidth)
							{
								if (memberInstance.KeepTogether)
								{
									flag2 = true;
									TraceKeepTogetherColumnMember(pageContext, pageStructStaticMemberCell);
								}
								else if (memberInstance.Children == null && columnInfo.KeepTogether)
								{
									flag2 = true;
									TraceKeepTogetherColumn(pageContext, columnInfo);
								}
							}
						}
						if (flag2)
						{
							MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.LeftOfNextPage);
							return num2;
						}
					}
					if (roundedDouble4 <= endInTablix)
					{
						colIndex += memberInstance.ColSpan;
						num2 += memberInstance.CurrColSpan;
						if (!memberInstance.Hidden)
						{
							colsOnPage += memberInstance.CurrColSpan;
						}
						num++;
						continue;
					}
					if (roundedDouble5 <= endInTablix && memberInstance.Children != null)
					{
						int num4 = 0;
						int colIndex2 = colIndex;
						num4 = MarkDetailColsForHorizontalPage(memberInstance.Children, startInTablix, endInTablix, realEndInTablix, colIndex2, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
						if (num4 == 0)
						{
							return num2;
						}
						colIndex += memberInstance.ColSpan;
						if (num4 < memberInstance.CurrColSpan)
						{
							memberInstance.CurrColSpan = num4;
							return num2 + memberInstance.CurrColSpan;
						}
						num2 += memberInstance.CurrColSpan;
						colsOnPage += memberInstance.CurrColSpan;
						num++;
						continue;
					}
					if (allowSpanAtRight)
					{
						if (memberInstance.MemberItem != null)
						{
							spanContent++;
						}
						MarkSpanCHMemberOnPage(memberInstance, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight: true, pageContext);
						num2 += memberInstance.CurrColSpan;
					}
					return num2;
				}
				PageStructDynamicMemberCell pageStructDynamicMemberCell = (PageStructDynamicMemberCell)pageStructMemberCell;
				PageMemberCell item2 = null;
				int num5 = pageStructDynamicMemberCell.MemberInstances.Count - 1;
				int j;
				for (j = 0; j <= num5; j++)
				{
					item2 = pageStructDynamicMemberCell.MemberInstances[j];
					roundedDouble4.Value = item2.EndPos;
					if (!(roundedDouble4 <= startInTablix))
					{
						break;
					}
					colIndex += item2.ColSpan;
				}
				if (stopIndex > 0 && colIndex >= stopIndex)
				{
					return num2;
				}
				roundedDouble3.Value = item2.StartPos;
				if (roundedDouble3 < startInTablix)
				{
					if (item2.MemberItem != null)
					{
						spanContent++;
					}
					using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(j, out item2))
					{
						bool num6 = MarkSpanCHMemberOnPage(item2, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
						num2 += item2.CurrColSpan;
						if (num6)
						{
							return num2;
						}
						colsOnPage += item2.CurrColSpan;
						colIndex += item2.ColSpan;
					}
					j++;
				}
				for (; j <= num5; j++)
				{
					NormalizeWidths(pageStructDynamicMemberCell.MemberInstances, j, colIndex, startInTablix, endInTablix);
					if (stopIndex > 0 && colIndex >= stopIndex)
					{
						return num2;
					}
					item2 = pageStructDynamicMemberCell.MemberInstances[j];
					roundedDouble3.Value = item2.StartPos;
					roundedDouble4.Value = item2.EndPos;
					roundedDouble5.Value = item2.ContentRight;
					if (colsOnPage > 0 && spanContent == 0)
					{
						bool flag3 = false;
						ColumnInfo columnInfo2 = m_columnInfo[colIndex];
						if (!columnInfo2.BlockedBySpan)
						{
							RoundedDouble roundedDouble6 = new RoundedDouble(0.0);
							if (roundedDouble5 > endInTablix)
							{
								flag3 = true;
							}
							if (roundedDouble4 > endInTablix && (item2.KeepTogether || (item2.Children == null && columnInfo2.KeepTogether)))
							{
								roundedDouble6.Value = item2.SizeValue;
								if (item2.SizeValue <= pageContext.ColumnWidth)
								{
									flag3 = true;
									TraceKeepTogetherColumnMemberColumn(pageContext, columnInfo2, pageStructDynamicMemberCell, item2.KeepTogether);
								}
							}
						}
						if (flag3)
						{
							MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.LeftOfNextPage);
							return num2;
						}
					}
					if (roundedDouble4 <= endInTablix)
					{
						colIndex += item2.ColSpan;
						num2 += item2.CurrColSpan;
						if (!item2.Hidden)
						{
							colsOnPage += item2.CurrColSpan;
						}
						continue;
					}
					if (roundedDouble5 <= endInTablix && item2.Children != null)
					{
						int num7 = 0;
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(j, out item2))
						{
							num7 = MarkDetailColsForHorizontalPage(item2.Children, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
							if (num7 == 0)
							{
								return num2;
							}
							colIndex += item2.ColSpan;
							if (num7 < item2.CurrColSpan)
							{
								item2.CurrColSpan = num7;
								num2 += item2.CurrColSpan;
								return num2;
							}
							num2 += item2.CurrColSpan;
							colsOnPage += item2.CurrColSpan;
						}
						continue;
					}
					if (allowSpanAtRight)
					{
						if (item2.MemberItem != null)
						{
							spanContent++;
						}
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(j, out item2))
						{
							MarkSpanCHMemberOnPage(item2, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight: true, pageContext);
							return num2 + item2.CurrColSpan;
						}
					}
					return num2;
				}
				num++;
			}
			return num2;
		}

		private bool MarkSpanCHMemberOnPage(PageMemberCell memberCell, double startInTablix, double endInTablix, double realEndInTablix, int colIndex, int colsOnPage, int spanContent, int stopIndex, bool allowSpanAtRight, PageContext pageContext)
		{
			int num = MarkDetailColsForHorizontalPage(memberCell.Children, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
			memberCell.ResolveCHHorizontal(m_columnInfo, colIndex, startInTablix, realEndInTablix, pageContext);
			if (num < memberCell.CurrColSpan)
			{
				memberCell.CurrColSpan = num;
				return true;
			}
			if (new RoundedDouble(memberCell.EndPos) > endInTablix)
			{
				return true;
			}
			return false;
		}

		private void NormalizeColHeadersWidths(double startInTablix, double endInTablix, bool update)
		{
			if (m_columnHeaders != null)
			{
				int num = 0;
				int lastColOnPage = -1;
				int lastColBelow = -1;
				double addedWidth = 0.0;
				for (int i = 0; i < m_columnHeaders.Count; i++)
				{
					m_columnHeaders[i].NormalizeDetailColWidth(m_columnInfo, num, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update, ref addedWidth);
					num += m_columnHeaders[i].Span;
				}
				if (lastColBelow >= 0)
				{
					base.ItemPageSizes.AdjustWidthTo(m_columnInfo[lastColBelow].Right);
				}
				else if (lastColOnPage >= 0)
				{
					base.ItemPageSizes.AdjustWidthTo(m_columnInfo[lastColOnPage].Right);
				}
			}
		}

		private void NormalizeColHeadersWidths(double startInTablix, double endInTablix, bool update, int startColIndex)
		{
			if (m_columnHeaders == null)
			{
				return;
			}
			int num = 0;
			int lastColOnPage = -1;
			int lastColBelow = -1;
			double addedWidth = 0.0;
			for (int i = 0; i < m_columnHeaders.Count; i++)
			{
				if (num >= startColIndex)
				{
					m_columnHeaders[i].NormalizeDetailColWidth(m_columnInfo, num, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update, ref addedWidth);
				}
				num += m_columnHeaders[i].Span;
			}
			if (lastColBelow >= 0)
			{
				base.ItemPageSizes.AdjustWidthTo(m_columnInfo[lastColBelow].Right);
			}
			else if (lastColOnPage >= 0)
			{
				base.ItemPageSizes.AdjustWidthTo(m_columnInfo[lastColOnPage].Right);
			}
		}

		private void NormalizeWidths(List<PageStructMemberCell> members, int index, int colIndex, double startInTablix, double endInTablix)
		{
			if (members == null)
			{
				return;
			}
			ColumnInfo columnInfo = null;
			bool flag = false;
			while (index < members.Count)
			{
				columnInfo = m_columnInfo[colIndex + members[index].Span - 1];
				if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
				{
					break;
				}
				colIndex += members[index].Span;
				index++;
				flag = true;
			}
			int lastColOnPage = colIndex - 1;
			if (flag)
			{
				lastColOnPage = -1;
			}
			for (int i = colIndex; i < m_columnInfo.Count; i++)
			{
				columnInfo = m_columnInfo[i];
				if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					continue;
				}
				if (columnInfo.PageHorizontalState != 0)
				{
					return;
				}
				if (i > colIndex)
				{
					lastColOnPage = -1;
				}
				break;
			}
			double addedWidth = 0.0;
			int lastColBelow = -1;
			for (int j = index; j < members.Count; j++)
			{
				members[j].NormalizeDetailColWidth(m_columnInfo, colIndex, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update: false, ref addedWidth);
				colIndex += members[j].Span;
			}
			if (colIndex < m_columnInfo.Count)
			{
				MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.Unknown);
			}
		}

		private void NormalizeWidths(ScalableList<PageMemberCell> members, int index, int colIndex, double startInTablix, double endInTablix)
		{
			if (members == null)
			{
				return;
			}
			ColumnInfo columnInfo = null;
			bool flag = false;
			while (index < members.Count)
			{
				columnInfo = m_columnInfo[colIndex + members[index].ColSpan - 1];
				if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
				{
					break;
				}
				colIndex += members[index].ColSpan;
				index++;
				flag = true;
			}
			int lastColOnPage = colIndex - 1;
			if (flag)
			{
				lastColOnPage = -1;
			}
			for (int i = colIndex; i < m_columnInfo.Count; i++)
			{
				columnInfo = m_columnInfo[i];
				if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					continue;
				}
				if (columnInfo.PageHorizontalState != 0)
				{
					return;
				}
				if (i > colIndex)
				{
					lastColOnPage = -1;
				}
				break;
			}
			double addedWidth = 0.0;
			int lastColAtRight = -1;
			PageMemberCell item = null;
			for (int j = index; j < members.Count; j++)
			{
				using (members.GetAndPin(j, out item))
				{
					item.NormalizeDetailColWidth(m_columnInfo, colIndex, startInTablix, endInTablix, ref lastColOnPage, ref lastColAtRight, update: false, ref addedWidth);
					colIndex += item.ColSpan;
				}
			}
			if (colIndex < m_columnInfo.Count)
			{
				MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.Unknown);
			}
		}

		private bool StaticDecendents(TablixMemberCollection children)
		{
			if (children == null || children.Count == 0)
			{
				return true;
			}
			bool flag = true;
			for (int i = 0; i < children.Count && flag; i++)
			{
				flag = (children[i].IsStatic && StaticDecendents(children[i].Children));
			}
			return flag;
		}

		private int TablixMembersDepthTree(TablixMemberCollection memberCollection)
		{
			if (memberCollection == null || memberCollection.Count == 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < memberCollection.Count; i++)
			{
				num = Math.Max(num, TablixMembersDepthTree(memberCollection[i].Children));
			}
			return num + 1;
		}

		private int AddTablixMemberDef(ref Hashtable memberDefIndexes, ref List<RPLTablixMemberDef> memberDefList, TablixMember tablixMember, bool borderHeader, int defTreeLevel, int sourceIndex, PageContext pageContext)
		{
			int? num = null;
			if (memberDefIndexes == null)
			{
				memberDefIndexes = new Hashtable();
				memberDefList = new List<RPLTablixMemberDef>();
				m_memberAtLevelIndexes = new Hashtable();
			}
			else
			{
				num = (int?)memberDefIndexes[tablixMember.DefinitionPath];
			}
			if (!num.HasValue)
			{
				num = memberDefList.Count;
				memberDefIndexes.Add(tablixMember.DefinitionPath, num);
				byte state = 0;
				if (borderHeader)
				{
					state = 4;
				}
				RPLTablixMemberDef item = new RPLTablixMemberDef(tablixMember.DefinitionPath, tablixMember.MemberCellIndex, state, defTreeLevel);
				memberDefList.Add(item);
				m_memberAtLevelIndexes.Add(tablixMember.DefinitionPath, sourceIndex);
				if (pageContext.Common.DiagnosticsEnabled && pageContext.IgnorePageBreaks && tablixMember.Group != null && tablixMember.Group.PageBreak.BreakLocation != 0)
				{
					pageContext.Common.TracePageBreakIgnored(tablixMember, pageContext.IgnorePageBreaksReason);
				}
			}
			return num.Value;
		}

		internal bool AlwayHiddenMember(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (visibility == null)
			{
				return false;
			}
			if (visibility.HiddenState == SharedHiddenState.Always && !evalPageHeaderFooter)
			{
				return true;
			}
			return false;
		}

		internal bool EnterColMemberInstance(TablixMember colMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (colMember.IsTotal)
			{
				return false;
			}
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Always)
			{
				if (evalPageHeaderFooter)
				{
					m_ignoreCol++;
					return true;
				}
				return false;
			}
			if (visibility.ToggleItem != null)
			{
				if (colMember.Instance.Visibility.CurrentlyHidden)
				{
					if (!evalPageHeaderFooter)
					{
						return false;
					}
					m_ignoreCol++;
				}
				return true;
			}
			return !colMember.Instance.Visibility.CurrentlyHidden;
		}

		internal void LeaveColMemberInstance(TablixMember colMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (colMember.IsTotal || visibility == null || visibility.HiddenState == SharedHiddenState.Never)
			{
				return;
			}
			if (visibility.HiddenState == SharedHiddenState.Always && evalPageHeaderFooter)
			{
				m_ignoreCol--;
			}
			if (visibility.ToggleItem != null)
			{
				bool flag = false;
				if (evalPageHeaderFooter)
				{
					flag = colMember.Instance.Visibility.CurrentlyHidden;
				}
				if (flag)
				{
					m_ignoreCol--;
				}
			}
		}

		internal bool EnterRowMember(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (rowMember.IsTotal)
			{
				m_ignoreGroupPageBreaks++;
				return false;
			}
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Always)
			{
				if (evalPageHeaderFooter)
				{
					m_ignoreRow++;
					return true;
				}
				return false;
			}
			if (visibility.ToggleItem != null)
			{
				VisibilityInstance visibility2 = rowMember.Instance.Visibility;
				m_ignoreGroupPageBreaks++;
				if (visibility2.CurrentlyHidden)
				{
					if (!evalPageHeaderFooter)
					{
						return false;
					}
					m_ignoreRow++;
				}
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		internal void LeaveRowMember(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			bool currentlyHidden = false;
			if (visibility != null && visibility.ToggleItem != null)
			{
				currentlyHidden = rowMember.Instance.Visibility.CurrentlyHidden;
			}
			LeaveRowMember(rowMember, visibility, evalPageHeaderFooter, currentlyHidden);
		}

		internal void LeaveRowMember(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter, bool currentlyHidden)
		{
			if (rowMember.IsTotal)
			{
				m_ignoreGroupPageBreaks--;
			}
			else
			{
				if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
				{
					return;
				}
				if (visibility.HiddenState == SharedHiddenState.Always && evalPageHeaderFooter)
				{
					m_ignoreRow--;
				}
				if (visibility.ToggleItem != null)
				{
					m_ignoreGroupPageBreaks--;
					bool flag = false;
					if (evalPageHeaderFooter)
					{
						flag = currentlyHidden;
					}
					if (flag)
					{
						m_ignoreRow--;
					}
				}
			}
		}

		internal bool EnterRowMemberInstance(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (rowMember.IsTotal)
			{
				return false;
			}
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Always)
			{
				if (evalPageHeaderFooter)
				{
					m_ignoreRow++;
					return true;
				}
				return false;
			}
			if (visibility.ToggleItem != null)
			{
				if (rowMember.Instance.Visibility.CurrentlyHidden)
				{
					if (!evalPageHeaderFooter)
					{
						return false;
					}
					m_ignoreRow++;
				}
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		internal void LeaveRowMemberInstance(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter, bool currentlyHidden)
		{
			if (rowMember.IsTotal || visibility == null || visibility.HiddenState == SharedHiddenState.Never)
			{
				return;
			}
			if (visibility.HiddenState == SharedHiddenState.Always && evalPageHeaderFooter)
			{
				m_ignoreRow--;
			}
			if (visibility.ToggleItem != null)
			{
				bool flag = false;
				if (evalPageHeaderFooter)
				{
					flag = currentlyHidden;
				}
				if (flag)
				{
					m_ignoreRow--;
				}
			}
		}

		internal PageMemberCell AddColMember(TablixMember colMember, int rowIndex, int rowSpan, int colSpan, LevelInfo childInfo, PageContext pageContext, double updateHeight)
		{
			PageMemberCell pageMemberCell = null;
			if (colMember.TablixHeader == null)
			{
				if (m_ignoreCol > 0 || childInfo.HiddenLevel)
				{
					pageMemberCell = new PageMemberCell(null, 0, colSpan, 0.0, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					pageMemberCell = new PageMemberCell(null, 0, colSpan, childInfo.SourceSize, colMember.Group);
				}
				return pageMemberCell;
			}
			ReportItem reportItem = colMember.TablixHeader.CellContents.ReportItem;
			double num = colMember.TablixHeader.Size.ToMillimeters() + childInfo.SizeForParent - updateHeight;
			if (reportItem != null)
			{
				PageContext.IgnorePageBreakReason ignorePageBreakReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
				if (pageContext.IgnorePageBreaks)
				{
					ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
				}
				PageContext pageContext2 = new PageContext(pageContext, fullOnPage: true, ignorePageBreaks: true, ignorePageBreakReason, cacheNonSharedProps: true);
				if (m_ignoreCol > 0 || childInfo.HiddenLevel)
				{
					PageItem item = new HiddenPageItem(reportItem, pageContext2, checkHiddenState: true);
					pageMemberCell = new PageMemberCell(item, rowSpan + childInfo.SpanForParent, colSpan, 0.0, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					PageItem item = PageItem.Create(reportItem, tablixCellParent: true, ignoreKT: true, pageContext2);
					item.ItemPageSizes.Width = childInfo.SourceSize;
					bool anyAncestorHasKT = false;
					item.CalculateVertical(pageContext2, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
					pageMemberCell = new PageMemberCell(item, rowSpan + childInfo.SpanForParent, colSpan, childInfo.SourceSize, colMember.Group);
					num = Math.Max(num, item.ItemPageSizes.Height);
					UpdateSizes(rowIndex, pageMemberCell.RowSpan, num, ref m_colHeaderHeights);
				}
			}
			else if (m_ignoreCol > 0 || childInfo.HiddenLevel)
			{
				pageMemberCell = new PageMemberCell(null, rowSpan + childInfo.SpanForParent, colSpan, 0.0, null);
				pageMemberCell.Hidden = true;
			}
			else
			{
				pageMemberCell = new PageMemberCell(null, rowSpan + childInfo.SpanForParent, colSpan, childInfo.SourceSize, colMember.Group);
				UpdateSizes(rowIndex, pageMemberCell.RowSpan, num, ref m_colHeaderHeights);
			}
			return pageMemberCell;
		}

		internal PageMemberCell AddTotalColMember(TablixMember colMember, int rowIndex, int rowSpan, int colSpan, LevelInfo parentInfo, LevelInfo childInfo, PageContext pageContext)
		{
			PageMemberCell pageMemberCell = null;
			double updateHeight = 0.0;
			if (parentInfo.SpanForParent > 0)
			{
				rowIndex += parentInfo.SpanForParent;
				rowSpan -= parentInfo.SpanForParent;
				updateHeight = parentInfo.SizeForParent;
				if (rowSpan == 0)
				{
					if (childInfo.HiddenLevel)
					{
						pageMemberCell = new PageMemberCell(null, 0, colSpan, 0.0, null);
						pageMemberCell.Hidden = true;
					}
					else
					{
						pageMemberCell = new PageMemberCell(null, 0, colSpan, childInfo.SourceSize, null);
					}
					return pageMemberCell;
				}
			}
			return AddColMember(colMember, rowIndex, rowSpan, colSpan, childInfo, pageContext, updateHeight);
		}

		internal PageMemberCell AddRowMember(TablixMember rowMember, int colIndex, int rowSpan, int colSpan, LevelInfo childInfo, PageContext pageContext, double updateWidth)
		{
			PageMemberCell pageMemberCell = null;
			if (rowMember.TablixHeader == null)
			{
				if (m_ignoreRow > 0 || childInfo.HiddenLevel)
				{
					pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, rowMember.Group);
					if (rowMember.Group != null && CheckPageBreaks(pageContext))
					{
						pageMemberCell.PageName = rowMember.Group.Instance.PageName;
					}
				}
				if (childInfo.IgnoreTotals)
				{
					pageMemberCell.IgnoreTotals = true;
				}
				return pageMemberCell;
			}
			ReportItem reportItem = rowMember.TablixHeader.CellContents.ReportItem;
			double num = rowMember.TablixHeader.Size.ToMillimeters() + childInfo.SizeForParent - updateWidth;
			if (reportItem != null)
			{
				PageContext.IgnorePageBreakReason ignorePageBreakReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
				if (pageContext.IgnorePageBreaks)
				{
					ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
				}
				PageContext pageContext2 = new PageContext(pageContext, fullOnPage: true, ignorePageBreaks: true, ignorePageBreakReason, cacheNonSharedProps: true);
				if (m_ignoreRow > 0 || childInfo.HiddenLevel)
				{
					PageItem item = new HiddenPageItem(reportItem, pageContext2, checkHiddenState: true);
					pageMemberCell = new PageMemberCell(item, rowSpan, colSpan + childInfo.SpanForParent, num, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					PageItem item = PageItem.Create(reportItem, tablixCellParent: true, ignoreKT: true, pageContext2);
					item.ItemPageSizes.Height = childInfo.SourceSize;
					if (item is TextBox)
					{
						item.ItemPageSizes.Width = num;
					}
					bool anyAncestorHasKT = false;
					item.CalculateVertical(pageContext2, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
					pageMemberCell = new PageMemberCell(item, rowSpan, colSpan + childInfo.SpanForParent, num, rowMember.Group);
					if (rowMember.Group != null && CheckPageBreaks(pageContext))
					{
						pageMemberCell.PageName = rowMember.Group.Instance.PageName;
					}
				}
			}
			else if (m_ignoreRow > 0 || childInfo.HiddenLevel)
			{
				pageMemberCell = new PageMemberCell(null, rowSpan, colSpan + childInfo.SpanForParent, num, null);
				pageMemberCell.Hidden = true;
			}
			else
			{
				pageMemberCell = new PageMemberCell(null, rowSpan, colSpan + childInfo.SpanForParent, num, rowMember.Group);
				if (rowMember.Group != null && CheckPageBreaks(pageContext))
				{
					pageMemberCell.PageName = rowMember.Group.Instance.PageName;
				}
			}
			if (childInfo.IgnoreTotals)
			{
				pageMemberCell.IgnoreTotals = true;
			}
			return pageMemberCell;
		}

		internal PageMemberCell AddTotalRowMember(TablixMember rowMember, int colIndex, int rowSpan, int colSpan, LevelInfo parentInfo, LevelInfo childInfo, PageContext pageContext)
		{
			PageMemberCell pageMemberCell = null;
			double updateWidth = 0.0;
			if (parentInfo.SpanForParent > 0)
			{
				colIndex += parentInfo.SpanForParent;
				colSpan -= parentInfo.SpanForParent;
				updateWidth = parentInfo.SizeForParent;
				if (colSpan == 0)
				{
					if (childInfo.HiddenLevel)
					{
						pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, null);
						pageMemberCell.Hidden = true;
					}
					else
					{
						pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, null);
					}
					return pageMemberCell;
				}
			}
			return AddRowMember(rowMember, colIndex, rowSpan, colSpan, childInfo, pageContext, updateWidth);
		}

		internal void CreateCornerCell(PageItem topItem, CellContents cellContents, int rowIndex, int colIndex, double sourceWidth, double sourceHeight)
		{
			if (m_cornerCells == null)
			{
				m_cornerCells = new PageCornerCell[m_headerColumnRows, m_headerRowCols];
			}
			m_cornerCells[rowIndex, colIndex] = new PageCornerCell(topItem, cellContents.RowSpan, cellContents.ColSpan, sourceWidth, sourceHeight);
		}

		private bool CreateDetailCell(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int colGridIndex, RowInfo rowInfo, PageContext pageContext)
		{
			bool result = false;
			int num = colMemberParent.MemberCellIndex;
			TablixCell tablixCell = m_bodyRows[m_rowMemberIndexCell][num];
			double num2 = m_bodyColWidths[num];
			if (tablixCell == null)
			{
				while (num > 0)
				{
					num--;
					tablixCell = m_bodyRows[m_rowMemberIndexCell][num];
					if (tablixCell != null)
					{
						break;
					}
				}
			}
			if (m_colMemberIndexCell >= 0 && num == m_colMemberIndexCell)
			{
				PageDetailCell lastCell = null;
				IDisposable disposable = null;
				if (m_ignoreCol > 0 || m_ignoreRow > 0)
				{
					disposable = rowInfo.UpdateLastDetailCell(0.0, out lastCell);
				}
				else
				{
					disposable = rowInfo.UpdateLastDetailCell(num2, out lastCell);
					if (lastCell.Hidden)
					{
						lastCell.Hidden = false;
						if (lastCell.CellItem != null)
						{
							PageContext pageContext2 = new PageContext(pageContext, cacheNonSharedProps: true);
							if (m_ignoreCellPageBreaks > 0 || m_ignoreGroupPageBreaks > 0)
							{
								pageContext2.IgnorePageBreaks = true;
								if (!pageContext.IgnorePageBreaks)
								{
									if (m_ignoreCellPageBreaks > 0)
									{
										pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
									}
									else
									{
										pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
									}
								}
							}
							lastCell.CellItem = PageItem.Create(lastCell.CellItem.Source, tablixCellParent: true, ignoreKT: false, pageContext2);
							rowInfo.CalculateVerticalLastDetailCell(pageContext2, firstTouch: true, delayCalc: true);
							UpdateTopItemKT(lastCell.CellItem, rowInfo, lastCell);
						}
					}
				}
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
			}
			else
			{
				PageContext pageContext3 = new PageContext(pageContext, cacheNonSharedProps: true);
				bool delayCalc = false;
				if (m_ignoreCellPageBreaks > 0 || m_ignoreGroupPageBreaks > 0)
				{
					pageContext3.IgnorePageBreaks = true;
					if (!pageContext.IgnorePageBreaks)
					{
						if (m_ignoreCellPageBreaks > 0)
						{
							pageContext3.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						else
						{
							pageContext3.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
						}
					}
				}
				rowInfo.CalculateVerticalLastDetailCell(pageContext3, firstTouch: false, delayCalc);
				result = true;
				m_colMemberIndexCell = num;
				if (tablixCell.CellContents.ColSpan == 1)
				{
					m_colMemberIndexCell = -1;
				}
				else
				{
					delayCalc = true;
				}
				PageDetailCell pageDetailCell = null;
				ReportItem reportItem = null;
				if (tablixCell.CellContents != null)
				{
					reportItem = tablixCell.CellContents.ReportItem;
				}
				if (m_ignoreCol > 0 || m_ignoreRow > 0)
				{
					pageDetailCell = ((reportItem == null) ? new PageDetailCell(null, 0.0) : new PageDetailCell(new HiddenPageItem(reportItem, pageContext3, checkHiddenState: true), 0.0));
					pageDetailCell.Hidden = true;
					rowInfo.AddDetailCell(pageDetailCell, pageContext);
				}
				else
				{
					if (reportItem != null)
					{
						PageItem pageItem = PageItem.Create(reportItem, tablixCellParent: true, ignoreKT: false, pageContext3);
						pageDetailCell = new PageDetailCell(pageItem, num2);
						UpdateTopItemKT(pageItem, rowInfo, pageDetailCell);
					}
					else
					{
						pageDetailCell = new PageDetailCell(null, num2);
					}
					rowInfo.AddDetailCell(pageDetailCell, pageContext);
					rowInfo.CalculateVerticalLastDetailCell(pageContext3, firstTouch: true, delayCalc);
				}
			}
			return result;
		}

		private void UpdateTopItemKT(PageItem topItem, RowInfo rowInfo, PageDetailCell currCell)
		{
			if (topItem.KeepTogetherHorizontal)
			{
				currCell.KeepTogether = true;
				topItem.KeepTogetherHorizontal = false;
				topItem.UnresolvedKTH = false;
			}
			if (topItem.KeepTogetherVertical)
			{
				rowInfo.KeepTogether = true;
				topItem.KeepTogetherVertical = false;
				topItem.UnresolvedKTV = false;
			}
		}

		private bool UpdateDetailCell(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int colGridIndex, RowInfo rowInfo, double startInTablix, double endInTablix, ref int detailCellIndex, PageContext pageContext)
		{
			bool result = false;
			int num = colMemberParent.MemberCellIndex;
			TablixCell tablixCell = m_bodyRows[m_rowMemberIndexCell][num];
			if (tablixCell == null)
			{
				while (num > 0)
				{
					num--;
					tablixCell = m_bodyRows[m_rowMemberIndexCell][num];
					if (tablixCell != null)
					{
						break;
					}
				}
			}
			if (m_colMemberIndexCell < 0 || num != m_colMemberIndexCell)
			{
				PageContext pageContext2 = new PageContext(pageContext, cacheNonSharedProps: true);
				if (m_ignoreCellPageBreaks > 0 || m_ignoreGroupPageBreaks > 0)
				{
					pageContext2.IgnorePageBreaks = true;
					if (!pageContext.IgnorePageBreaks)
					{
						if (m_ignoreCellPageBreaks > 0)
						{
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						else
						{
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
						}
					}
				}
				rowInfo.UpdateVerticalDetailCell(pageContext2, startInTablix, endInTablix, ref detailCellIndex);
				result = true;
				m_colMemberIndexCell = num;
				if (tablixCell.CellContents.ColSpan == 1)
				{
					m_colMemberIndexCell = -1;
				}
			}
			return result;
		}

		internal void CreateCorner(TablixCorner corner, PageContext pageContext)
		{
			if (m_cornerCells != null || m_headerColumnRows == 0 || m_headerRowCols == 0)
			{
				return;
			}
			TablixCornerRowCollection rowCollection = corner.RowCollection;
			TablixCornerRow tablixCornerRow = null;
			for (int i = 0; i < rowCollection.Count; i++)
			{
				tablixCornerRow = rowCollection[i];
				for (int j = 0; j < tablixCornerRow.Count; j++)
				{
					AddCornerCell(tablixCornerRow[j], i, j, pageContext);
				}
			}
		}

		private void AddCornerCell(TablixCornerCell tablixCornerCell, int rowIndex, int colIndex, PageContext pageContext)
		{
			if (tablixCornerCell == null || tablixCornerCell.CellContents == null)
			{
				return;
			}
			CellContents cellContents = tablixCornerCell.CellContents;
			ReportItem reportItem = cellContents.ReportItem;
			PageItem pageItem = null;
			if (reportItem != null)
			{
				PageContext.IgnorePageBreakReason ignorePageBreakReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
				if (pageContext.IgnorePageBreaks)
				{
					ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
				}
				PageContext pageContext2 = new PageContext(pageContext, fullOnPage: true, ignorePageBreaks: true, ignorePageBreakReason, cacheNonSharedProps: true);
				pageItem = PageItem.Create(reportItem, tablixCellParent: true, ignoreKT: true, pageContext2);
				bool anyAncestorHasKT = false;
				pageItem.CalculateVertical(pageContext2, 0.0, double.MaxValue, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
				UpdateSizes(rowIndex, cellContents.RowSpan, pageItem.ItemPageSizes.Height, ref m_colHeaderHeights);
				CreateCornerCell(pageItem, cellContents, rowIndex, colIndex, pageItem.ItemPageSizes.Width, pageItem.ItemPageSizes.Height);
			}
			else
			{
				CreateCornerCell(null, cellContents, rowIndex, colIndex, 0.0, 0.0);
			}
		}

		private double CreateColumnsHeaders(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext)
		{
			if (ColumnHeadersCreated)
			{
				return 0.0;
			}
			CreateCorner(tablix.Corner, pageContext);
			LevelInfo parentLevelInfo = null;
			CreateColumnMemberChildren(tablix, null, m_colMembersDepth, parentBorderHeader: false, 0, m_headerRowCols, out parentLevelInfo, pageContext);
			ColumnHeadersCreated = true;
			m_columnHeaders = parentLevelInfo.MemberCells;
			if (m_columnHeaders != null && m_columnHeaders.Count > 0)
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = m_columnHeaders[0].Split(m_colsBeforeRowHeaders, pageContext);
				if (pageStructDynamicMemberCell != null)
				{
					m_columnHeaders.Insert(0, pageStructDynamicMemberCell);
				}
				if (!IsLTR)
				{
					int num = 0;
					m_columnHeaders.Reverse();
					for (int i = 0; i < m_columnHeaders.Count; i++)
					{
						m_columnHeaders[i].Reverse(pageContext);
						num += m_columnHeaders[i].Span;
					}
					m_colsBeforeRowHeaders = num - m_colsBeforeRowHeaders;
				}
			}
			ResolveSizes(m_colHeaderHeights);
			double num2 = ResolveStartPosAndState(m_colHeaderHeights, 0.0, SizeInfo.PageState.Normal);
			base.ItemPageSizes.AdjustHeightTo(num2);
			return num2;
		}

		private int CreateColumnMemberChildren(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, out LevelInfo parentLevelInfo, PageContext pageContext)
		{
			parentLevelInfo = new LevelInfo(0.0);
			TablixMemberCollection tablixMemberCollection = null;
			int num = 0;
			if (colMemberParent == null)
			{
				if (m_colsBeforeRowHeaders == 0)
				{
					num = tablix.GroupsBeforeRowHeaders;
				}
				tablixMemberCollection = tablix.ColumnHierarchy.MemberCollection;
			}
			else
			{
				tablixMemberCollection = colMemberParent.Children;
			}
			if (tablixMemberCollection == null)
			{
				if (m_ignoreCol == 0)
				{
					parentLevelInfo.SourceSize += m_bodyColWidths[colMemberParent.MemberCellIndex];
				}
				return 1;
			}
			int num2 = parentColIndex;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			bool flag = true;
			LevelInfo parentLevelInfo2 = null;
			bool flag2 = true;
			int num6 = 0;
			bool flag3 = false;
			int num7 = 0;
			int num8 = -1;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			int num9 = 0;
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				flag3 = parentBorderHeader;
				if ((NoRows && tablixMember.HideIfNoRows) || AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					continue;
				}
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					num = 0;
					flag2 = EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (tablixMember.IsTotal)
					{
						if (!flag2 && num3 <= num6 && m_ignoreCol == 0 && num9 == 0)
						{
							if (list == null)
							{
								list = new List<int>();
							}
							list.Add(i);
						}
					}
					else if (!flag3)
					{
						flag3 = StaticDecendents(tablixMember.Children);
					}
				}
				else
				{
					num6 = num2;
					num3 = num2;
					if (i > 0)
					{
						num = 0;
					}
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					}
				}
				num8 = AddTablixMemberDef(ref m_colMemberDefIndexes, ref m_colMemberDefList, tablixMember, flag3, defTreeLevel, i, pageContext);
				while (flag)
				{
					num7 = defTreeLevel;
					if (flag2)
					{
						num4 = 0;
						if (tablixMember.TablixHeader != null)
						{
							num4 = tablixMember.TablixHeader.CellContents.RowSpan;
							num7 -= num4;
						}
						else
						{
							num7--;
						}
						num5 = CreateColumnMemberChildren(tablix, tablixMember, num7, flag3, parentRowIndex + num4, num2, out parentLevelInfo2, pageContext);
						if (num5 > 0)
						{
							PageMemberCell pageMemberCell = AddColMember(tablixMember, parentRowIndex, num4, num5, parentLevelInfo2, pageContext, 0.0);
							if (!parentLevelInfo2.OmittedList)
							{
								pageMemberCell.Children = parentLevelInfo2.MemberCells;
							}
							MergeDetailRows mergeDetailRows = parentLevelInfo.AddMemberCell(tablixMember, i, pageMemberCell, num8, TablixRegion.ColumnHeader, m_bodyColWidths, parentLevelInfo2, pageContext, this);
							if (mergeDetailRows == null)
							{
								num2 += num5;
								if (!pageMemberCell.Hidden)
								{
									num3 += num5;
									num9 += num5;
									list = null;
								}
							}
						}
					}
					LeaveColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (tablixMember.IsStatic)
					{
						flag = false;
					}
					else
					{
						flag = tablixDynamicMemberInstance.MoveNext();
						if (num > 0)
						{
							m_colsBeforeRowHeaders += num5;
							num--;
						}
						if (flag)
						{
							flag2 = EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						}
					}
					num5 = 0;
				}
				tablixDynamicMemberInstance = null;
			}
			if (num9 == 0)
			{
				num2 += CreateColumnMemberTotals(list, tablixMemberCollection, defTreeLevel, parentBorderHeader, parentRowIndex, num2, parentLevelInfo, pageContext);
			}
			return num2 - parentColIndex;
		}

		private int CreateColumnMemberTotals(List<int> totals, TablixMemberCollection columnMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			if (totals == null || totals.Count == 0)
			{
				return 0;
			}
			double sizeForParent = 0.0;
			int num = int.MaxValue;
			bool flag = false;
			int num2 = 0;
			TablixMember tablixMember = null;
			if (parentRowIndex > 0)
			{
				for (int i = 0; i < totals.Count; i++)
				{
					num2 = totals[i];
					tablixMember = columnMembers[num2];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents.RowSpan < num)
					{
						num = tablixMember.TablixHeader.CellContents.RowSpan;
						sizeForParent = tablixMember.TablixHeader.Size.ToMillimeters();
						flag = true;
					}
				}
				if (flag)
				{
					parentLevelInfo.SpanForParent = num;
					parentLevelInfo.SizeForParent = sizeForParent;
				}
			}
			int num3 = parentColIndex;
			int num4 = 0;
			int num5 = 0;
			LevelInfo parentLevelInfo2 = null;
			int num6 = 0;
			int num7 = -1;
			for (int j = 0; j < totals.Count; j++)
			{
				num2 = totals[j];
				tablixMember = columnMembers[num2];
				num7 = AddTablixMemberDef(ref m_colMemberDefIndexes, ref m_colMemberDefList, tablixMember, parentBorderHeader, defTreeLevel, j, pageContext);
				num4 = 0;
				num6 = defTreeLevel;
				if (tablixMember.TablixHeader != null)
				{
					num4 = tablixMember.TablixHeader.CellContents.RowSpan;
					num6 -= num4;
				}
				else
				{
					num6--;
				}
				num5 = CreateColumnMemberChildren(null, tablixMember, num6, parentBorderHeader, parentRowIndex + num4, num3, out parentLevelInfo2, pageContext);
				if (num5 > 0)
				{
					PageMemberCell pageMemberCell = AddTotalColMember(tablixMember, parentRowIndex, num4, num5, parentLevelInfo, parentLevelInfo2, pageContext);
					if (!parentLevelInfo2.OmittedList)
					{
						pageMemberCell.Children = parentLevelInfo2.MemberCells;
					}
					parentLevelInfo.AddMemberCell(tablixMember, num2, pageMemberCell, num7, TablixRegion.ColumnHeader, m_bodyColWidths, parentLevelInfo2, pageContext, this);
					num3 += num5;
				}
			}
			return num3 - parentColIndex;
		}

		private int TraverseColumnMembers(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int parentColIndex, RowInfo currRowInfo, bool create, double startInTablix, double endInTablix, ref int detailCellIndex, out int visibleSpan, List<int> detailCellsState, PageContext pageContext)
		{
			visibleSpan = 0;
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((colMemberParent != null) ? colMemberParent.Children : tablix.ColumnHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				bool flag = false;
				flag = ((!create) ? UpdateDetailCell(tablix, colMemberParent, parentColIndex, currRowInfo, startInTablix, endInTablix, ref detailCellIndex, pageContext) : CreateDetailCell(tablix, colMemberParent, parentColIndex, currRowInfo, pageContext));
				if (m_ignoreCol == 0)
				{
					visibleSpan = 1;
				}
				if (flag)
				{
					detailCellsState?.Add(m_colMemberIndexCell);
				}
				return 1;
			}
			int num = parentColIndex;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = true;
			bool flag3 = true;
			int num4 = 0;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			int num5 = 0;
			List<int> list2 = null;
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				if ((NoRows && tablixMember.HideIfNoRows) || AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					continue;
				}
				flag2 = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					flag3 = EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (tablixMember.IsTotal && !flag3 && num2 <= num4 && m_ignoreCol == 0 && num5 == 0)
					{
						if (list == null)
						{
							list = new List<int>();
						}
						list.Add(i);
					}
				}
				else
				{
					m_colMemberIndexCell = -1;
					num4 = num;
					num2 = num;
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag2 = tablixDynamicMemberInstance.MoveNext();
					if (flag2)
					{
						flag3 = EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					}
				}
				if (detailCellsState != null && list2 != null)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						detailCellsState.Add(list2[j]);
					}
					list2 = null;
				}
				int visibleSpan2 = 0;
				int destCellIndex = 0;
				int num6 = 0;
				List<int> list3 = null;
				while (flag2)
				{
					if (flag3)
					{
						if (m_ignoreCol > 0)
						{
							list3 = new List<int>(1);
							if (currRowInfo.Cells != null)
							{
								num6 = currRowInfo.Cells.Count;
							}
						}
						num3 = TraverseColumnMembers(tablix, tablixMember, num, currRowInfo, create, startInTablix, endInTablix, ref detailCellIndex, out visibleSpan2, list3, pageContext);
						if (m_ignoreCol > 0)
						{
							if (list2 == null)
							{
								num += num3;
								list2 = list3;
								destCellIndex = num6;
							}
							else
							{
								num += currRowInfo.MergeDetailCells(destCellIndex, list2, num6, list3);
							}
							list3 = null;
						}
						else
						{
							num += num3;
							num2 += visibleSpan2;
							num5 += visibleSpan2;
							visibleSpan += visibleSpan2;
							list = null;
							list2 = null;
						}
					}
					LeaveColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (tablixMember.IsStatic)
					{
						flag2 = false;
						continue;
					}
					m_colMemberIndexCell = -1;
					flag2 = tablixDynamicMemberInstance.MoveNext();
					if (flag2)
					{
						flag3 = EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					}
				}
				tablixDynamicMemberInstance = null;
			}
			if (detailCellsState != null && list2 != null)
			{
				for (int k = 0; k < list2.Count; k++)
				{
					detailCellsState.Add(list2[k]);
				}
				list2 = null;
			}
			if (num5 == 0)
			{
				num += TraverseTotalColumnMembers(tablix, list, tablixMemberCollection, num, currRowInfo, create, startInTablix, endInTablix, ref detailCellIndex, ref visibleSpan, pageContext);
			}
			return num - parentColIndex;
		}

		private int TraverseTotalColumnMembers(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection columnMembers, int parentColIndex, RowInfo currRowInfo, bool create, double startInTablix, double endInTablix, ref int detailCellIndex, ref int visibleSpan, PageContext pageContext)
		{
			if (totals == null || totals.Count == 0)
			{
				return 0;
			}
			TablixMember tablixMember = null;
			int num = 0;
			int visibleSpan2 = 0;
			int num2 = parentColIndex;
			int num3 = 0;
			for (int i = 0; i < totals.Count; i++)
			{
				num = totals[i];
				tablixMember = columnMembers[num];
				num3 = TraverseColumnMembers(tablix, tablixMember, num2, currRowInfo, create, startInTablix, endInTablix, ref detailCellIndex, out visibleSpan2, null, pageContext);
				num2 += num3;
				visibleSpan += visibleSpan2;
			}
			return num2 - parentColIndex;
		}

		private int CreateTablixRows(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, ref List<PageStructMemberCell> pageStructMemberCell, bool ignoreTotals, ref bool finishLevel, bool parentHasFooters, CreateItemsContext createItems, double startInTablix, double endInTablix, PageContext pageContext)
		{
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				if (parentRowIndex >= m_detailRows.Count)
				{
					finishLevel = true;
					return 0;
				}
				RowInfo item = null;
				using (m_detailRows.GetAndPin(parentRowIndex, out item))
				{
					if (item.Top < startInTablix)
					{
						double num = startInTablix - item.Top;
						UpdateDetailRow(tablix, rowMemberParent, item, parentColIndex, startInTablix, endInTablix, pageContext);
						createItems.UpdateInfo(item, 0.0 - num);
						if (item.ContentFullyCreated)
						{
							finishLevel = true;
						}
					}
					else
					{
						createItems.UpdateInfo(item, 0.0);
						item.ResetRowHeight();
					}
				}
				return 0;
			}
			int num2 = -1;
			PageStructMemberCell pageStructMemberCell2 = null;
			TablixMember tablixMember = null;
			int rowEndIndex = parentRowIndex;
			int num3 = 0;
			bool flag = false;
			int num4 = -1;
			if (pageStructMemberCell != null)
			{
				for (int i = 0; i < pageStructMemberCell.Count; i++)
				{
					flag = false;
					pageStructMemberCell2 = pageStructMemberCell[i];
					num2 = pageStructMemberCell2.SourceIndex;
					tablixMember = tablixMemberCollection[num2];
					if (pageStructMemberCell2.PartialItem)
					{
						bool flag2 = parentHasFooters;
						if (!flag2 && !tablixMember.IsStatic && i + 1 == pageStructMemberCell.Count)
						{
							bool repeatWith = true;
							num4 = CheckKeepWithGroupDown(tablixMemberCollection, num2 + 1, KeepWithGroup.Before, ref repeatWith);
							if (num4 > num2)
							{
								flag2 = true;
							}
						}
						if (pageStructMemberCell2.CreateItem)
						{
							if (pageStructMemberCell2.Span > 0 && rowEndIndex + pageStructMemberCell2.Span - 1 < m_detailRows.Count)
							{
								RowInfo item2 = null;
								using (m_detailRows.GetAndPin(rowEndIndex + pageStructMemberCell2.Span - 1, out item2))
								{
									item2.ResetRowHeight();
								}
							}
							if (!pageStructMemberCell2.HasInstances)
							{
								LevelInfo levelInfo = new LevelInfo(pageStructMemberCell, -1, num2, createItems);
								if (ignoreTotals)
								{
									levelInfo.IgnoreTotals = true;
								}
								int num5 = CreateRowMemberChildren(tablix, rowMemberParent, defTreeLevel, parentBorderHeader, rowEndIndex, parentColIndex, num2, resetContext: false, flag2, levelInfo, pageContext);
								num3 += num5;
								if (!levelInfo.PartialStruct)
								{
									finishLevel = true;
								}
								if (pageStructMemberCell2.Span == 0)
								{
									pageStructMemberCell2.DisposeInstances();
									pageStructMemberCell.RemoveAt(i);
									i--;
								}
								return num3;
							}
							LevelInfo levelInfo2 = new LevelInfo(pageStructMemberCell, i, num2, createItems);
							int num6 = CreateDynamicRowMemberChildren(tablix, rowMemberParent, defTreeLevel, num2, rowEndIndex + pageStructMemberCell2.Span, parentColIndex, flag2, levelInfo2, pageContext);
							num3 += num6;
							if (levelInfo2.PartialStruct)
							{
								return num3;
							}
							if (pageStructMemberCell2.Span == 0)
							{
								pageStructMemberCell2.DisposeInstances();
								pageStructMemberCell.RemoveAt(i);
								i--;
								i -= RemoveHeadersAbove(pageStructMemberCell, i, m_detailRows, ref rowEndIndex);
							}
							rowEndIndex += pageStructMemberCell2.Span;
							continue;
						}
						PageMemberCell memberCell = null;
						IDisposable disposable2 = pageStructMemberCell2.PartialMemberInstance(out memberCell);
						if (tablixMember.IsTotal || (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null))
						{
							m_ignoreGroupPageBreaks++;
						}
						bool parentBorderHeader2 = parentBorderHeader;
						int num7 = defTreeLevel;
						if (!parentBorderHeader && tablixMember.IsStatic)
						{
							parentBorderHeader2 = StaticDecendents(tablixMember.Children);
						}
						num7 = ((memberCell.ColSpan <= 0) ? (num7 - 1) : (num7 - memberCell.ColSpan));
						List<PageStructMemberCell> pageStructMemberCell3 = memberCell.Children;
						if (!tablixMember.IsStatic)
						{
							RestoreRowMemberInstanceIndex(tablixMember, (TablixDynamicMemberInstance)tablixMember.Instance);
						}
						int num8 = CreateTablixRows(tablix, tablixMember, num7, parentBorderHeader2, rowEndIndex + pageStructMemberCell2.PartialRowSpan, parentColIndex + memberCell.ColSpan, ref pageStructMemberCell3, memberCell.IgnoreTotals, ref flag, flag2, createItems, startInTablix, endInTablix, pageContext);
						if (createItems.InnerSpanPages)
						{
							memberCell.SpanPages = true;
							pageStructMemberCell2.SpanPages = true;
						}
						memberCell.Children = pageStructMemberCell3;
						bool flag3 = CheckPageBreaks(pageContext);
						if (tablixMember.IsTotal || (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null))
						{
							m_ignoreGroupPageBreaks--;
						}
						pageStructMemberCell2.Span += num8;
						memberCell.RowSpan += num8;
						memberCell.CurrRowSpan += num8;
						num3 += num8;
						rowEndIndex += pageStructMemberCell2.Span;
						if (disposable2 != null)
						{
							disposable2.Dispose();
							disposable2 = null;
						}
						if (!flag)
						{
							return num3;
						}
						if (tablixMember.IsStatic)
						{
							if (pageStructMemberCell2.Span == 0)
							{
								pageStructMemberCell2.DisposeInstances();
								pageStructMemberCell.RemoveAt(i);
								i--;
							}
							pageStructMemberCell2.PartialItem = false;
							continue;
						}
						PageBreakProperties pageBreakProperties = PageBreakProperties.Create(tablixMember.Group.PageBreak, tablixMember, pageContext);
						pageStructMemberCell2.CreateItem = true;
						TablixDynamicMemberInstance rowDynamicInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
						if (AdvanceToNextVisibleInstance(tablix, tablixMember, rowDynamicInstance))
						{
							if (memberCell.RowSpan == 0)
							{
								pageStructMemberCell2.RemoveLastMemberCell();
							}
							bool checkVisibleChildren = false;
							if (flag3 && pageStructMemberCell2.Span > 0)
							{
								bool keepWith = false;
								if (ApplyDynamicPageBreaks(tablixMember, rowEndIndex, flag2, ref keepWith, ref checkVisibleChildren, pageBreakProperties, pageContext))
								{
									return num3;
								}
							}
							if (createItems.AdvanceRow)
							{
								i--;
								rowEndIndex -= pageStructMemberCell2.Span;
								continue;
							}
							bool flag4 = false;
							if (!checkVisibleChildren)
							{
								flag4 = DynamicWithVisibleChildren(tablixMember);
							}
							if (flag4)
							{
								return num3;
							}
							i--;
							rowEndIndex -= pageStructMemberCell2.Span;
							continue;
						}
						pageStructMemberCell2.PartialItem = false;
						if (pageStructMemberCell2.Span == 0)
						{
							pageStructMemberCell2.DisposeInstances();
							pageStructMemberCell.RemoveAt(i);
							i--;
							int num9 = RemoveHeadersAbove(pageStructMemberCell, i, m_detailRows, ref rowEndIndex);
							i -= num9;
							num3 -= num9;
						}
						else if (flag3 && pageBreakProperties != null && pageBreakProperties.PageBreakAtEnd)
						{
							createItems.GroupPageBreaks = true;
							RowInfo item3 = null;
							using (m_detailRows.GetAndPin(rowEndIndex - 1, out item3))
							{
								item3.PageBreaksAtEnd = true;
								item3.PageBreakPropertiesAtEnd = pageBreakProperties;
							}
						}
					}
					else
					{
						rowEndIndex += pageStructMemberCell2.Span;
					}
				}
			}
			if (num2 + 1 <= num4)
			{
				LevelInfo levelInfo3 = new LevelInfo(pageStructMemberCell, -1, num2, createItems);
				num2++;
				int num10 = CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, rowEndIndex, parentColIndex, num2, num4, keepWith: true, setPrevHeader: false, levelInfo3, pageContext);
				pageStructMemberCell = levelInfo3.MemberCells;
				num3 += num10;
				if (levelInfo3.PartialStruct)
				{
					return num3;
				}
				num2 = num4;
				rowEndIndex += num10;
			}
			if (num2 + 1 < tablixMemberCollection.Count)
			{
				LevelInfo levelInfo4 = new LevelInfo(pageStructMemberCell, -1, num2, createItems);
				if (ignoreTotals)
				{
					levelInfo4.IgnoreTotals = true;
				}
				num2++;
				int num11 = CreateRowMemberChildren(tablix, rowMemberParent, defTreeLevel, parentBorderHeader, rowEndIndex, parentColIndex, num2, resetContext: true, parentHasFooters, levelInfo4, pageContext);
				pageStructMemberCell = levelInfo4.MemberCells;
				num3 += num11;
				if (!levelInfo4.PartialStruct)
				{
					finishLevel = true;
				}
			}
			else
			{
				finishLevel = true;
			}
			return num3;
		}

		private void SetPageBreakAndKeepWith(int index, PageBreakProperties pageBreakProperties, bool overwrite)
		{
			RowInfo item = null;
			using (m_detailRows.GetAndPin(index, out item))
			{
				item.PageBreaksAtEnd = true;
				if (overwrite || item.PageBreakPropertiesAtEnd == null)
				{
					item.PageBreakPropertiesAtEnd = pageBreakProperties;
				}
				item.IgnoreKeepWith = true;
			}
		}

		private int MergeToggledDetailRows(int destIndex, int srcIndex, List<MergeDetailRows.DetailRowState> detailRowsState)
		{
			RowInfo rowInfo = null;
			int num = 0;
			for (int i = 0; i < detailRowsState.Count; i++)
			{
				switch (detailRowsState[i])
				{
				case MergeDetailRows.DetailRowState.Merge:
				{
					RowInfo rowInfo2 = m_detailRows[destIndex];
					rowInfo = m_detailRows[srcIndex];
					rowInfo2.Merge(rowInfo);
					rowInfo.DisposeDetailCells();
					destIndex++;
					srcIndex++;
					break;
				}
				case MergeDetailRows.DetailRowState.Insert:
					m_detailRows.Insert(destIndex, m_detailRows[srcIndex]);
					destIndex++;
					srcIndex += 2;
					num++;
					break;
				default:
					destIndex++;
					break;
				}
			}
			return num;
		}

		private int CreateRowMemberChildren(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, int sourceIndex, bool resetContext, bool parentHasFooters, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				RowInfo rowInfo = CreateDetailRow(tablix, rowMemberParent, parentRowIndex, parentColIndex, pageContext);
				parentLevelInfo.CreateItems.UpdateInfo(rowInfo, 0.0);
				if (m_ignoreRow == 0)
				{
					parentLevelInfo.SourceSize += m_bodyRowsHeights[rowMemberParent.MemberCellIndex];
				}
				return 1;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			bool flag = true;
			LevelInfo levelInfo = null;
			bool flag2 = false;
			bool flag3 = true;
			int num5 = -1;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool keepWithGroup = false;
			bool flag7 = false;
			bool flag8 = false;
			int num6 = 0;
			int num7 = 0;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			int num8 = 0;
			for (int i = sourceIndex; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				flag8 = parentBorderHeader;
				flag2 = parentHasFooters;
				if ((NoRows && tablixMember.HideIfNoRows) || AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					continue;
				}
				flag4 = false;
				flag3 = true;
				flag = true;
				levelInfo = null;
				flag7 = tablixMember.KeepTogether;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					if (!parentLevelInfo.CreateItems.AdvanceRow && !tablixMember.IsTotal)
					{
						if (num5 >= 0 && num2 > num5)
						{
							bool repeatWith = false;
							bool flag9 = (!m_detailRows[num - 1].IgnoreKeepWith) ? true : false;
							int num9 = CheckKeepWithGroupDown(tablixMemberCollection, i, KeepWithGroup.Before, ref repeatWith);
							if (num9 >= i && (repeatWith || flag9))
							{
								bool keepWith = (!m_detailRows[num - 1].PageBreaksAtEnd) ? true : false;
								bool partialDetailRow = parentLevelInfo.CreateItems.PartialDetailRow;
								parentLevelInfo.CreateItems.PartialDetailRow = false;
								m_ignoreCellPageBreaks++;
								num += CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i, num9, keepWith, setPrevHeader: false, parentLevelInfo, pageContext);
								m_ignoreCellPageBreaks--;
								if (parentLevelInfo.CreateItems.PartialDetailRow)
								{
									return num - parentRowIndex;
								}
								parentLevelInfo.CreateItems.PartialDetailRow = partialDetailRow;
								i = num9;
								num5 = -1;
								continue;
							}
						}
						bool flag10 = true;
						if (flag2)
						{
							flag10 = StaticWithVisibleChildren(tablixMember);
						}
						if (flag10)
						{
							RowInfo item = null;
							using (m_detailRows.GetAndPin(num - 1, out item))
							{
								item.IgnoreKeepWith = true;
							}
							if (!flag8)
							{
								flag8 = StaticDecendents(tablixMember.Children);
							}
							num7 = AddTablixMemberDef(ref m_rowMemberDefIndexes, ref m_rowMemberDefList, tablixMember, flag8, defTreeLevel, i, pageContext);
							parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, createItem: true);
							return num - parentRowIndex;
						}
					}
					flag3 = EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (tablixMember.IsTotal)
					{
						if (!flag3 && num2 <= num5 && m_ignoreRow == 0 && num8 == 0 && !parentLevelInfo.IgnoreTotals)
						{
							if (list == null)
							{
								list = new List<int>();
							}
							list.Add(i);
						}
					}
					else if (!flag8)
					{
						flag8 = StaticDecendents(tablixMember.Children);
					}
					if (tablixMember.KeepWithGroup != 0)
					{
						m_ignoreCellPageBreaks++;
						keepWithGroup = false;
						if (KeepTogetherStaticHeader(tablixMemberCollection, tablixMember, i, ref keepWithGroup))
						{
							flag6 = true;
						}
						else if (num5 >= 0 && num2 > num5 && tablixMember.KeepWithGroup == KeepWithGroup.Before)
						{
							flag6 = true;
							keepWithGroup = true;
						}
					}
					num5 = -1;
				}
				else
				{
					num5 = num;
					num2 = num;
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					if (resetContext || i > sourceIndex)
					{
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						flag4 = true;
						if (flag)
						{
							flag = CheckAndAdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance);
						}
						SaveRowMemberInstanceIndex(tablixMember, tablixDynamicMemberInstance);
						if (flag && !flag2)
						{
							bool repeatWith2 = false;
							if (CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref repeatWith2) > i)
							{
								flag2 = true;
							}
						}
					}
					if (!parentLevelInfo.CreateItems.AdvanceRow)
					{
						if (!flag)
						{
							continue;
						}
						bool flag11 = true;
						if (flag2)
						{
							flag11 = DynamicWithVisibleChildren(tablixMember);
						}
						if (flag11)
						{
							RowInfo item2 = null;
							using (m_detailRows.GetAndPin(num - 1, out item2))
							{
								item2.IgnoreKeepWith = true;
							}
							num7 = AddTablixMemberDef(ref m_rowMemberDefIndexes, ref m_rowMemberDefList, tablixMember, flag8, defTreeLevel, i, pageContext);
							parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, createItem: true);
							return num - parentRowIndex;
						}
					}
					if (flag)
					{
						flag3 = EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					}
				}
				num7 = AddTablixMemberDef(ref m_rowMemberDefIndexes, ref m_rowMemberDefList, tablixMember, flag8, defTreeLevel, i, pageContext);
				while (flag)
				{
					num6 = defTreeLevel;
					levelInfo = ((!(flag6 || flag7)) ? new LevelInfo(parentLevelInfo.CreateItems.SpaceToFill) : ((levelInfo != null) ? new LevelInfo(levelInfo.CreateItems.SpaceToFill) : new LevelInfo(Math.Max(pageContext.ColumnHeight, parentLevelInfo.CreateItems.SpaceToFill))));
					if (flag3)
					{
						if (!tablixMember.IsStatic && flag4 && CheckPageBreaks(pageContext))
						{
							PageBreakProperties pageBreakProperties = PageBreakProperties.Create(tablixMember.Group.PageBreak, tablixMember, pageContext);
							if (pageBreakProperties != null && pageBreakProperties.PageBreakAtStart)
							{
								if (num > 1 && m_detailRows[num - 1].PageVerticalState != RowInfo.VerticalState.Above)
								{
									if (DynamicWithVisibleChildren(tablixMember))
									{
										parentLevelInfo.CreateItems.GroupPageBreaks = true;
										SetPageBreakAndKeepWith(num - 1, pageBreakProperties, overwrite: false);
										parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, createItem: true);
										LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
										return num - parentRowIndex;
									}
								}
								else
								{
									flag5 = true;
								}
							}
						}
						num4 = 0;
						if (tablixMember.TablixHeader != null)
						{
							num4 = tablixMember.TablixHeader.CellContents.ColSpan;
							num6 -= num4;
						}
						else
						{
							num6--;
						}
						num3 = CreateRowMemberChildren(tablix, tablixMember, num6, flag8, num, parentColIndex + num4, 0, resetContext: true, flag2, levelInfo, pageContext);
						if (num3 > 0)
						{
							if (flag4)
							{
								flag4 = false;
								if (flag5)
								{
									flag5 = false;
									RowInfo item3 = null;
									using (m_detailRows.GetAndPin(num, out item3))
									{
										item3.PageBreaksAtStart = true;
										item3.PageBreakPropertiesAtStart = PageBreakProperties.Create(tablixMember.Group.PageBreak, tablixMember, pageContext);
									}
									flag7 = false;
								}
							}
							PageMemberCell pageMemberCell = AddRowMember(tablixMember, parentColIndex, num3, num4, levelInfo, pageContext, 0.0);
							if (!levelInfo.OmittedList)
							{
								pageMemberCell.Children = levelInfo.MemberCells;
							}
							MergeDetailRows mergeDetailRows = parentLevelInfo.AddMemberCell(tablixMember, i, pageMemberCell, num7, TablixRegion.RowHeader, m_bodyRowsHeights, levelInfo, pageContext, this);
							if (mergeDetailRows != null)
							{
								num += MergeToggledDetailRows(num - mergeDetailRows.Span, num, mergeDetailRows.DetailRowsState);
								m_detailRows.RemoveRange(num, num3);
							}
							else
							{
								num += num3;
								if (!pageMemberCell.Hidden)
								{
									num2 += num3;
									num8 += num3;
									list = null;
									parentLevelInfo.IgnoreTotals = true;
								}
							}
						}
						else if (levelInfo.PartialLevel)
						{
							parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, createItem: true);
						}
					}
					if (tablixMember.IsStatic)
					{
						flag = false;
						flag7 = false;
						LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						if (tablixMember.KeepWithGroup != 0)
						{
							m_ignoreCellPageBreaks--;
						}
						if (!parentLevelInfo.CreateItems.ContentFullyCreated || levelInfo.PartialLevel)
						{
							return num - parentRowIndex;
						}
						if (!parentLevelInfo.CreateItems.PartialDetailRow && flag6)
						{
							int num10 = num - num3;
							bool flag12 = false;
							bool repeatWith3 = false;
							int num11 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, tablixMember.KeepWithGroup, ref repeatWith3);
							if (num3 > 0)
							{
								if (num11 <= i || tablixMember.KeepWithGroup == KeepWithGroup.Before)
								{
									parentLevelInfo.SetKeepWith(1, keepWithGroup, tablixMember.RepeatOnNewPage);
									RowInfo item4 = null;
									for (int j = 0; j < num3; j++)
									{
										using (m_detailRows.GetAndPin(j + num10, out item4))
										{
											item4.RepeatWith = tablixMember.RepeatOnNewPage;
										}
									}
								}
								else
								{
									flag12 = true;
								}
							}
							if (num11 > i)
							{
								m_ignoreCellPageBreaks++;
								num += CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num11, keepWithGroup, flag12, parentLevelInfo, pageContext);
								m_ignoreCellPageBreaks--;
								if (parentLevelInfo.CreateItems.PartialDetailRow)
								{
									parentLevelInfo.CreateItems.ContentFullyCreated = false;
									return num - parentRowIndex;
								}
								if (flag12)
								{
									RowInfo item5 = null;
									for (int k = 0; k < num3; k++)
									{
										using (m_detailRows.GetAndPin(k + num10, out item5))
										{
											item5.RepeatWith = tablixMember.RepeatOnNewPage;
										}
									}
								}
								i = num11;
							}
							flag6 = false;
							if (tablixMember.KeepWithGroup == KeepWithGroup.After)
							{
								parentLevelInfo.CreateItems.AdvanceRow = true;
								flag7 = true;
							}
							else if (tablixMember.KeepWithGroup == KeepWithGroup.Before)
							{
								flag7 = true;
							}
						}
						if (!flag7 && num > parentRowIndex && parentLevelInfo.CreateItems.SpaceToFill <= 0.01)
						{
							parentLevelInfo.CreateItems.AdvanceRow = false;
						}
					}
					else
					{
						if (levelInfo.PartialLevel || !parentLevelInfo.CreateItems.ContentFullyCreated)
						{
							LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
							bool repeatWith4 = false;
							int num12 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref repeatWith4);
							if (num12 > i && repeatWith4)
							{
								m_ignoreCellPageBreaks++;
								num += CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num12, keepWith: true, setPrevHeader: false, parentLevelInfo, pageContext);
								m_ignoreCellPageBreaks--;
							}
							return num - parentRowIndex;
						}
						PageBreakProperties pageBreakProperties2 = PageBreakProperties.Create(tablixMember.Group.PageBreak, tablixMember, pageContext);
						bool currentlyHidden = false;
						if (visibility != null && visibility.ToggleItem != null)
						{
							currentlyHidden = tablixMember.Instance.Visibility.CurrentlyHidden;
						}
						flag = AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance);
						bool checkVisibleChildren = false;
						bool keepWith2 = true;
						if (CheckPageBreaks(pageContext) && num2 > num5)
						{
							if (flag)
							{
								if (ApplyDynamicPageBreaks(tablixMember, num, flag2, ref keepWith2, ref checkVisibleChildren, pageBreakProperties2, pageContext))
								{
									parentLevelInfo.CreateItems.GroupPageBreaks = true;
									num5 = -1;
									LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
									bool repeatWith5 = false;
									int num13 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref repeatWith5);
									if (num13 > i && repeatWith5)
									{
										m_ignoreCellPageBreaks++;
										num += CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num13, keepWith2, setPrevHeader: false, parentLevelInfo, pageContext);
										m_ignoreCellPageBreaks--;
									}
									return num - parentRowIndex;
								}
							}
							else if (pageBreakProperties2 != null && pageBreakProperties2.PageBreakAtEnd)
							{
								parentLevelInfo.CreateItems.GroupPageBreaks = true;
								SetPageBreakAndKeepWith(num - 1, pageBreakProperties2, overwrite: true);
								num5 = -1;
							}
						}
						if (flag)
						{
							if (num > num5)
							{
								if (flag7)
								{
									if (levelInfo.CreateItems.SpaceToFill <= -0.01)
									{
										parentLevelInfo.CreateItems.AdvanceRow = false;
									}
								}
								else if (parentLevelInfo.CreateItems.SpaceToFill <= -0.01)
								{
									parentLevelInfo.CreateItems.AdvanceRow = false;
								}
							}
							if (!parentLevelInfo.CreateItems.AdvanceRow)
							{
								bool flag13 = false;
								if (!checkVisibleChildren)
								{
									flag13 = DynamicWithVisibleChildren(tablixMember);
								}
								if (flag13)
								{
									LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
									num5 = -1;
									RowInfo item6 = null;
									using (m_detailRows.GetAndPin(num - 1, out item6))
									{
										item6.IgnoreKeepWith = true;
									}
									bool repeatWith6 = false;
									int num14 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref repeatWith6);
									if (num14 > i && repeatWith6)
									{
										m_ignoreCellPageBreaks++;
										num += CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num14, keepWith2, setPrevHeader: false, parentLevelInfo, pageContext);
										m_ignoreCellPageBreaks--;
									}
									return num - parentRowIndex;
								}
							}
							LeaveRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
							flag3 = EnterRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						}
						else
						{
							parentLevelInfo.FinishPartialStructMember();
							LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
						}
					}
					num3 = 0;
				}
				tablixDynamicMemberInstance = null;
			}
			if (num8 == 0)
			{
				num += CreateRowMemberTotals(tablix, list, tablixMemberCollection, defTreeLevel, parentBorderHeader, num, parentColIndex, parentHasFooters, parentLevelInfo, pageContext);
				if (num > parentRowIndex && parentLevelInfo.CreateItems.SpaceToFill <= 0.01)
				{
					parentLevelInfo.CreateItems.AdvanceRow = false;
				}
			}
			if (num >= 1)
			{
				RowInfo item7 = null;
				using (m_detailRows.GetAndPin(num - 1, out item7))
				{
					item7.IgnoreKeepWith = item7.PageBreaksAtEnd;
				}
			}
			return num - parentRowIndex;
		}

		private int CreateRowMemberTotals(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection rowMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, bool parentHasFooters, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			if (totals == null || totals.Count == 0)
			{
				return 0;
			}
			double sizeForParent = 0.0;
			int num = int.MaxValue;
			bool flag = false;
			int num2 = 0;
			TablixMember tablixMember = null;
			if (parentColIndex > 0)
			{
				for (int i = 0; i < totals.Count; i++)
				{
					num2 = totals[i];
					tablixMember = rowMembers[num2];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents.ColSpan < num)
					{
						num = tablixMember.TablixHeader.CellContents.ColSpan;
						sizeForParent = tablixMember.TablixHeader.Size.ToMillimeters();
						flag = true;
					}
				}
				if (flag)
				{
					parentLevelInfo.SpanForParent = num;
					parentLevelInfo.SizeForParent = sizeForParent;
				}
			}
			int num3 = parentRowIndex;
			int num4 = 0;
			int num5 = 0;
			LevelInfo levelInfo = null;
			int num6 = 0;
			int num7 = -1;
			for (int j = 0; j < totals.Count; j++)
			{
				num2 = totals[j];
				tablixMember = rowMembers[num2];
				EnterRowMember(tablixMember, null, evalPageHeaderFooter: false);
				num6 = defTreeLevel;
				num7 = AddTablixMemberDef(ref m_rowMemberDefIndexes, ref m_rowMemberDefList, tablixMember, parentBorderHeader, defTreeLevel, j, pageContext);
				levelInfo = new LevelInfo(double.MaxValue);
				num5 = 0;
				if (tablixMember.TablixHeader != null)
				{
					num5 = tablixMember.TablixHeader.CellContents.ColSpan;
					num6 -= num5;
				}
				else
				{
					num6--;
				}
				num4 = CreateRowMemberChildren(tablix, tablixMember, num6, parentBorderHeader, num3, parentColIndex + num5, 0, resetContext: true, parentHasFooters, levelInfo, pageContext);
				if (num4 > 0)
				{
					PageMemberCell pageMemberCell = AddTotalRowMember(tablixMember, parentColIndex, num4, num5, parentLevelInfo, levelInfo, pageContext);
					if (!levelInfo.OmittedList)
					{
						pageMemberCell.Children = levelInfo.MemberCells;
					}
					parentLevelInfo.AddMemberCell(tablixMember, num2, pageMemberCell, num7, TablixRegion.RowHeader, m_bodyRowsHeights, levelInfo, pageContext, this);
					num3 += num4;
				}
				LeaveRowMember(tablixMember, null, evalPageHeaderFooter: false);
			}
			return num3 - parentRowIndex;
		}

		private RowInfo CreateDetailRow(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int parentRowIndex, int parentColIndex, PageContext pageContext)
		{
			m_rowMemberIndexCell = rowMemberParent.MemberCellIndex;
			RowInfo rowInfo = null;
			if (m_ignoreRow <= 0)
			{
				rowInfo = ((m_ignoreCellPageBreaks <= 0) ? new RowInfo(0.0) : new RowInfo(m_bodyRowsHeights[m_rowMemberIndexCell]));
			}
			else
			{
				rowInfo = new RowInfo(0.0);
				rowInfo.Hidden = true;
			}
			int detailCellIndex = 0;
			int visibleSpan = 0;
			TraverseColumnMembers(tablix, null, parentColIndex, rowInfo, create: true, 0.0, pageContext.ColumnHeight, ref detailCellIndex, out visibleSpan, null, pageContext);
			PageContext pageContext2 = new PageContext(pageContext, cacheNonSharedProps: true);
			if (m_ignoreCellPageBreaks > 0 || m_ignoreGroupPageBreaks > 0)
			{
				pageContext2.IgnorePageBreaks = true;
				if (!pageContext.IgnorePageBreaks)
				{
					if (m_ignoreCellPageBreaks > 0)
					{
						pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
					}
					else
					{
						pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
					}
				}
			}
			rowInfo.CalculateVerticalLastDetailCell(pageContext2, firstTouch: false, delayCalc: false);
			if (!IsLTR)
			{
				rowInfo.ReverseDetailCells(pageContext);
			}
			if (m_detailRows == null)
			{
				m_detailRows = new ScalableList<RowInfo>(0, pageContext.ScalabilityCache);
			}
			if (parentRowIndex >= m_detailRows.Count)
			{
				m_detailRows.Add(rowInfo);
			}
			else
			{
				m_detailRows.Insert(parentRowIndex, rowInfo);
			}
			m_colMemberIndexCell = -1;
			return rowInfo;
		}

		private void UpdateDetailRow(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, RowInfo currRowInfo, int parentColIndex, double startInTablix, double endInTablix, PageContext pageContext)
		{
			m_rowMemberIndexCell = rowMemberParent.MemberCellIndex;
			currRowInfo.UpdatePinnedRow();
			int detailCellIndex = 0;
			int visibleSpan = 0;
			TraverseColumnMembers(tablix, null, parentColIndex, currRowInfo, create: false, startInTablix, endInTablix, ref detailCellIndex, out visibleSpan, null, pageContext);
			m_colMemberIndexCell = -1;
		}

		private int CreateDynamicRowMemberChildren(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, int sourceIndex, int parentRowIndex, int parentColIndex, bool parentHasFooters, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				RowInfo rowInfo = CreateDetailRow(tablix, rowMemberParent, parentRowIndex, parentColIndex, pageContext);
				parentLevelInfo.CreateItems.UpdateInfo(rowInfo, 0.0);
				parentLevelInfo.SourceSize += m_bodyRowsHeights[rowMemberParent.MemberCellIndex];
				return 1;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			bool flag = true;
			LevelInfo levelInfo = null;
			int num4 = 0;
			int num5 = num;
			TablixMember tablixMember = tablixMemberCollection[sourceIndex];
			Visibility visibility = tablixMember.Visibility;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
			RestoreRowMemberInstanceIndex(tablixMember, tablixDynamicMemberInstance);
			bool flag2 = EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
			while (flag)
			{
				levelInfo = new LevelInfo(parentLevelInfo.CreateItems.SpaceToFill);
				num4 = defTreeLevel;
				if (flag2)
				{
					num3 = 0;
					if (tablixMember.TablixHeader != null)
					{
						num3 = tablixMember.TablixHeader.CellContents.ColSpan;
						num4 -= num3;
					}
					else
					{
						num4--;
					}
					num2 = CreateRowMemberChildren(tablix, tablixMember, num4, parentBorderHeader: false, num, parentColIndex + num3, 0, resetContext: true, parentHasFooters, levelInfo, pageContext);
					if (num2 > 0)
					{
						PageMemberCell pageMemberCell = AddRowMember(tablixMember, parentColIndex, num2, num3, levelInfo, pageContext, 0.0);
						if (!levelInfo.OmittedList)
						{
							pageMemberCell.Children = levelInfo.MemberCells;
						}
						MergeDetailRows mergeDetailRows = parentLevelInfo.AddMemberCell(tablixMember, sourceIndex, pageMemberCell, -1, TablixRegion.RowHeader, m_bodyRowsHeights, levelInfo, pageContext, this);
						if (mergeDetailRows != null)
						{
							num += MergeToggledDetailRows(num - mergeDetailRows.Span, num, mergeDetailRows.DetailRowsState);
							m_detailRows.RemoveRange(num, num2);
						}
						else
						{
							num += num2;
						}
					}
				}
				if (levelInfo.PartialLevel)
				{
					LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					return num - parentRowIndex;
				}
				if (!parentLevelInfo.CreateItems.ContentFullyCreated)
				{
					LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					return num - parentRowIndex;
				}
				PageBreakProperties pageBreakProperties = PageBreakProperties.Create(tablixMember.Group.PageBreak, tablixMember, pageContext);
				bool currentlyHidden = false;
				if (visibility != null && visibility.ToggleItem != null)
				{
					currentlyHidden = tablixMember.Instance.Visibility.CurrentlyHidden;
				}
				flag = AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance);
				bool checkVisibleChildren = false;
				if (CheckPageBreaks(pageContext) && num > num5)
				{
					if (flag)
					{
						bool keepWith = false;
						if (ApplyDynamicPageBreaks(tablixMember, num, parentHasFooters, ref keepWith, ref checkVisibleChildren, pageBreakProperties, pageContext))
						{
							LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
							parentLevelInfo.CreateItems.GroupPageBreaks = true;
							return num - parentRowIndex;
						}
					}
					else if (pageBreakProperties != null && pageBreakProperties.PageBreakAtEnd)
					{
						parentLevelInfo.CreateItems.GroupPageBreaks = true;
						SetPageBreakAndKeepWith(num - 1, pageBreakProperties, overwrite: true);
					}
				}
				if (flag)
				{
					if (parentLevelInfo.CreateItems.SpaceToFill <= 0.01)
					{
						parentLevelInfo.CreateItems.AdvanceRow = false;
					}
					if (!parentLevelInfo.CreateItems.AdvanceRow)
					{
						bool flag3 = false;
						if (!checkVisibleChildren)
						{
							flag3 = DynamicWithVisibleChildren(tablixMember);
						}
						if (flag3)
						{
							LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
							return num - parentRowIndex;
						}
					}
					LeaveRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
					flag2 = EnterRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
				}
				else
				{
					parentLevelInfo.FinishPartialStructMember();
					LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
				}
			}
			return num - parentRowIndex;
		}

		private bool ApplyDynamicPageBreaks(TablixMember rowMember, int rowIndex, bool memberHasFooters, ref bool keepWith, ref bool checkVisibleChildren, PageBreakProperties pageBreakProperties, PageContext pageContext)
		{
			bool flag = false;
			PageBreak pageBreak = rowMember.Group.PageBreak;
			if (pageBreakProperties != null && pageBreakProperties.PageBreakAtEnd)
			{
				keepWith = false;
				bool flag2 = true;
				if (memberHasFooters)
				{
					checkVisibleChildren = true;
					flag2 = DynamicWithVisibleChildren(rowMember);
				}
				if (flag2)
				{
					SetPageBreakAndKeepWith(rowIndex - 1, pageBreakProperties, overwrite: true);
					flag = true;
				}
			}
			if (!flag && !checkVisibleChildren)
			{
				if (!pageBreak.Instance.Disabled && (pageBreak.BreakLocation == PageBreakLocation.Between || pageBreak.BreakLocation == PageBreakLocation.Start))
				{
					checkVisibleChildren = true;
					if (DynamicWithVisibleChildren(rowMember))
					{
						SetPageBreakAndKeepWith(rowIndex - 1, PageBreakProperties.Create(pageBreak, rowMember, pageContext), overwrite: true);
						flag = true;
					}
				}
				else if (pageContext.Common.DiagnosticsEnabled && pageBreak.Instance.Disabled && (pageBreak.BreakLocation == PageBreakLocation.Between || pageBreak.BreakLocation == PageBreakLocation.Start))
				{
					pageContext.Common.TracePageBreakIgnoredDisabled(rowMember);
				}
			}
			return flag;
		}

		private bool KeepTogetherStaticHeader(TablixMemberCollection rowMembers, TablixMember staticMember, int staticIndex, ref bool keepWithGroup)
		{
			if (staticMember.KeepWithGroup != KeepWithGroup.After)
			{
				return false;
			}
			bool repeatWith = staticMember.RepeatOnNewPage;
			int num = CheckKeepWithGroupDown(rowMembers, staticIndex + 1, KeepWithGroup.After, ref repeatWith);
			num++;
			if (num < rowMembers.Count)
			{
				TablixMember tablixMember = rowMembers[num];
				TablixDynamicMemberInstance tablixDynamicMemberInstance = tablixMember.Instance as TablixDynamicMemberInstance;
				if (tablixDynamicMemberInstance == null)
				{
					return false;
				}
				if (tablixMember.Visibility != null)
				{
					if (tablixMember.Visibility.HiddenState == SharedHiddenState.Always)
					{
						return false;
					}
					if (tablixMember.Visibility.ToggleItem != null)
					{
						int num2 = num + 1;
						if (num2 < rowMembers.Count && rowMembers[num2].IsTotal)
						{
							keepWithGroup = true;
							return true;
						}
					}
				}
				bool childPageBreakAtStart = false;
				bool flag = false;
				tablixDynamicMemberInstance.ResetContext();
				bool flag2 = tablixDynamicMemberInstance.MoveNext();
				while (flag2)
				{
					flag = false;
					if (RenderRowMemberInstance(tablixMember))
					{
						flag = MemberWithVisibleChildren(tablixMember, ref childPageBreakAtStart);
					}
					if (flag)
					{
						if (childPageBreakAtStart)
						{
							keepWithGroup = false;
							return repeatWith;
						}
						keepWithGroup = true;
						return true;
					}
					flag2 = tablixDynamicMemberInstance.MoveNext();
				}
			}
			return false;
		}

		private bool DynamicWithVisibleChildren(TablixMember rowMemberParent)
		{
			bool childPageBreakAtStart = false;
			return MemberWithVisibleChildren(rowMemberParent, ref childPageBreakAtStart);
		}

		private bool StaticWithVisibleChildren(TablixMember rowMemberParent)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			bool flag = RenderRowMemberInstance(rowMemberParent);
			if (flag)
			{
				bool childPageBreakAtStart = false;
				flag = MemberWithVisibleChildren(rowMemberParent, ref childPageBreakAtStart);
			}
			return flag;
		}

		private bool MemberWithVisibleChildren(TablixMember rowMemberParent, ref bool childPageBreakAtStart)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			TablixMemberCollection children = rowMemberParent.Children;
			if (children == null || children.Count == 0)
			{
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			for (int i = 0; i < children.Count; i++)
			{
				tablixMember = children[i];
				if (tablixMember.Visibility != null && tablixMember.Visibility.HiddenState == SharedHiddenState.Always)
				{
					continue;
				}
				flag2 = true;
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					flag2 = RenderRowMemberInstance(tablixMember);
				}
				else
				{
					if (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null)
					{
						int num = i + 1;
						if (num < children.Count && children[num].IsTotal)
						{
							return true;
						}
					}
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = RenderRowMemberInstance(tablixMember);
					}
				}
				while (flag)
				{
					if (flag2 && MemberWithVisibleChildren(tablixMember, ref childPageBreakAtStart))
					{
						Group group = rowMemberParent.Group;
						if (group != null)
						{
							PageBreakLocation breakLocation = group.PageBreak.BreakLocation;
							if (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd)
							{
								childPageBreakAtStart = true;
							}
						}
						return true;
					}
					if (tablixMember.IsStatic)
					{
						flag = false;
						continue;
					}
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = RenderRowMemberInstance(tablixMember);
					}
				}
				tablixDynamicMemberInstance = null;
			}
			return false;
		}

		private bool CheckAndAdvanceToNextVisibleInstance(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance)
		{
			bool flag = true;
			Visibility visibility = rowMember.Visibility;
			if (visibility != null && visibility.HiddenState == SharedHiddenState.Sometimes && visibility.ToggleItem == null)
			{
				while (flag && rowDynamicInstance.Visibility.CurrentlyHidden)
				{
					flag = rowDynamicInstance.MoveNext();
				}
			}
			return flag;
		}

		private bool AdvanceToNextVisibleInstance(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance)
		{
			bool flag = rowDynamicInstance.MoveNext();
			if (flag)
			{
				flag = CheckAndAdvanceToNextVisibleInstance(tablix, rowMember, rowDynamicInstance);
			}
			SaveRowMemberInstanceIndex(rowMember, rowDynamicInstance);
			return flag;
		}

		private int CheckKeepWithGroupDown(TablixMemberCollection rowMembers, int start, KeepWithGroup keepWith, ref bool repeatWith)
		{
			TablixMember tablixMember = null;
			while (start < rowMembers.Count)
			{
				tablixMember = rowMembers[start];
				if (tablixMember.IsStatic && tablixMember.KeepWithGroup == keepWith)
				{
					start++;
					repeatWith = tablixMember.RepeatOnNewPage;
					continue;
				}
				return start - 1;
			}
			return start - 1;
		}

		private int CreateKeepWithRowMember(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, int parentRowIndex, int parentColIndex, int start, int end, bool keepWith, bool setPrevHeader, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			if (start > end)
			{
				return 0;
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				RowInfo rowInfo = CreateDetailRow(tablix, rowMemberParent, parentRowIndex, parentColIndex, pageContext);
				parentLevelInfo.CreateItems.UpdateInfo(rowInfo, 0.0);
				parentLevelInfo.SourceSize += m_bodyRowsHeights[rowMemberParent.MemberCellIndex];
				return 1;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			LevelInfo levelInfo = null;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			int num4 = -1;
			int num5 = 0;
			int num6 = 0;
			double spaceToFill = Math.Max(pageContext.ColumnHeight, parentLevelInfo.CreateItems.SpaceToFill);
			if (start == -1 && end == -1)
			{
				start = 0;
				end = tablixMemberCollection.Count - 1;
			}
			for (int i = start; i <= end; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				num5 = defTreeLevel;
				if (AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					continue;
				}
				bool num7 = EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
				if (num7)
				{
					num4 = AddTablixMemberDef(ref m_rowMemberDefIndexes, ref m_rowMemberDefList, tablixMember, borderHeader: true, defTreeLevel, i, pageContext);
					num2 = 0;
					if (tablixMember.TablixHeader != null)
					{
						num2 = tablixMember.TablixHeader.CellContents.ColSpan;
						num5 -= num2;
					}
					else
					{
						num5--;
					}
					levelInfo = new LevelInfo(spaceToFill);
					num3 = CreateKeepWithRowMember(tablix, tablixMember, num5, num, parentColIndex + num2, -1, -1, keepWith: true, setPrevHeader: false, levelInfo, pageContext);
					if (num3 > 0)
					{
						PageMemberCell pageMemberCell = AddRowMember(tablixMember, parentColIndex, num3, num2, levelInfo, pageContext, 0.0);
						if (!levelInfo.OmittedList)
						{
							pageMemberCell.Children = levelInfo.MemberCells;
						}
						parentLevelInfo.AddMemberCell(tablixMember, i, pageMemberCell, num4, TablixRegion.RowHeader, m_bodyRowsHeights, levelInfo, pageContext, this);
						num6++;
						num += num3;
					}
				}
				LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
				if (num7 && !levelInfo.CreateItems.AdvanceRow)
				{
					num6--;
					break;
				}
			}
			if (num6 > 0)
			{
				if (tablixMember.KeepWithGroup == KeepWithGroup.After && !levelInfo.CreateItems.AdvanceRow)
				{
					return num - parentRowIndex;
				}
				if (setPrevHeader)
				{
					num6++;
				}
				parentLevelInfo.SetKeepWith(num6, keepWith, tablixMember.RepeatOnNewPage);
				for (int j = parentRowIndex; j < num; j++)
				{
					m_detailRows[j].RepeatWith = tablixMember.RepeatOnNewPage;
				}
			}
			else if (setPrevHeader)
			{
				parentLevelInfo.SetKeepWith(1, keepWith, tablixMember.RepeatOnNewPage);
			}
			return num - parentRowIndex;
		}

		private bool RenderRowMemberInstance(TablixMember rowMember)
		{
			if (rowMember.IsTotal)
			{
				return false;
			}
			Visibility visibility = rowMember.Visibility;
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		private bool IsLastVisibleMember(int memberIndex, List<PageStructMemberCell> members)
		{
			if (members.Count == 0 || memberIndex < 0)
			{
				return false;
			}
			int num = -1;
			for (int i = 0; i < members.Count; i++)
			{
				if (!members[i].Hidden)
				{
					num = i;
				}
			}
			return memberIndex == num;
		}

		private void TraceMember(PageContext pageContext, TablixMember tablixMember, RenderingArea renderingArea, TraceLevel traceLevel, string message)
		{
			if (pageContext.Common.DiagnosticsEnabled)
			{
				string text = DiagnosticsUtilities.BuildTablixMemberPath(m_source.Name, tablixMember, m_memberAtLevelIndexes);
				RenderingDiagnostics.Trace(renderingArea, traceLevel, message, pageContext.Common.PageNumber, text);
			}
		}

		public void TraceKeepTogetherRowMember(PageContext pageContext, PageStructMemberCell memberCell)
		{
			TraceMember(pageContext, memberCell.TablixMember, RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Member '{1}' kept together - Explicit - Pushed to next page");
		}

		public void TraceKeepTogetherColumnMember(PageContext pageContext, PageStructMemberCell memberCell)
		{
			TraceMember(pageContext, memberCell.TablixMember, RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Member '{1}' kept together - Explicit - Pushed to next page");
		}

		public void TraceInvalidatedKeepTogetherMember(PageContext pageContext, PageStructMemberCell memberCell)
		{
			TraceMember(pageContext, memberCell.TablixMember, RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] KeepTogether on Member '{1}' not honored - larger than page");
		}

		public void TraceKeepTogetherRow(PageContext pageContext, RowInfo rowInfo)
		{
			if (!pageContext.Common.DiagnosticsEnabled)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PageDetailCell cell in rowInfo.Cells)
			{
				if (cell.KeepTogether && cell.CellItem != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(cell.CellItem.Source.Name);
				}
			}
			RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item(s) '{1}' kept together - RowContext - Pushed to next page", pageContext.Common.PageNumber, stringBuilder.ToString());
		}

		public void TraceKeepTogetherColumn(PageContext pageContext, ColumnInfo columnInfo)
		{
			if (pageContext.Common.DiagnosticsEnabled)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item kept together - ColumnContext - Pushed to next page", pageContext.Common.PageNumber);
			}
		}

		public void TraceKeepTogetherRowMemberRow(PageContext pageContext, RowInfo rowInfo, PageStructMemberCell memberCell, bool memberKeepTogether)
		{
			if (memberKeepTogether)
			{
				TraceKeepTogetherRowMember(pageContext, memberCell);
			}
			if (rowInfo.KeepTogether)
			{
				TraceKeepTogetherRow(pageContext, rowInfo);
			}
		}

		public void TraceKeepTogetherColumnMemberColumn(PageContext pageContext, ColumnInfo columnInfo, PageStructMemberCell memberCell, bool memberKeepTogether)
		{
			if (memberKeepTogether)
			{
				TraceKeepTogetherColumnMember(pageContext, memberCell);
			}
			if (columnInfo.KeepTogether)
			{
				TraceKeepTogetherColumn(pageContext, columnInfo);
			}
		}

		public void TraceRepeatOnNewPage(PageContext pageContext, HeaderFooterRow headerFooterRow)
		{
			TablixMember tablixMember = headerFooterRow.StructMemberCell.TablixMember;
			TraceMember(pageContext, tablixMember, RenderingArea.RepeatOnNewPage, TraceLevel.Info, "PR-DIAG [Page {0}] '{1}' appears on page due to RepeatOnNewPage");
		}

		public void TraceInvalidatedRepeatOnNewPageSize(PageContext pageContext, HeaderFooterRow headerFooterRow)
		{
			TablixMember tablixMember = headerFooterRow.StructMemberCell.TablixMember;
			TraceMember(pageContext, tablixMember, RenderingArea.RepeatOnNewPage, TraceLevel.Info, "PR-DIAG [Page {0}] '{1}' not repeated due to page size contraints");
		}

		public void TraceInvalidatedRepeatOnNewPageSplitMember(PageContext pageContext, HeaderFooterRow headerFooterRow)
		{
			TablixMember tablixMember = headerFooterRow.StructMemberCell.TablixMember;
			TraceMember(pageContext, tablixMember, RenderingArea.RepeatOnNewPage, TraceLevel.Info, "PR-DIAG [Page {0}] '{1}' not repeated because child member spans pages");
		}

		private bool GetFlagValue(TablixState flag)
		{
			return (m_tablixStateFlags & flag) > (TablixState)0;
		}

		private void SetFlagValue(TablixState flag, bool value)
		{
			if (value)
			{
				m_tablixStateFlags |= flag;
			}
			else
			{
				m_tablixStateFlags &= (TablixState)(short)(~(int)flag);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TablixState:
					writer.Write((short)m_tablixStateFlags);
					break;
				case MemberName.BodyRows:
				{
					int value3 = scalabilityCache.StoreStaticReference(m_bodyRows);
					writer.Write(value3);
					break;
				}
				case MemberName.BodyRowHeights:
					writer.Write(m_bodyRowsHeights);
					break;
				case MemberName.BodyColWidths:
					writer.Write(m_bodyColWidths);
					break;
				case MemberName.RowMembersDepth:
					writer.Write(m_rowMembersDepth);
					break;
				case MemberName.ColMembersDepth:
					writer.Write(m_colMembersDepth);
					break;
				case MemberName.RowMemberDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_rowMemberDefList);
					writer.Write(value2);
					break;
				}
				case MemberName.RowMemberDefIndexes:
					writer.WriteStringInt32Hashtable(m_rowMemberDefIndexes);
					break;
				case MemberName.ColMemberDef:
				{
					int value = scalabilityCache.StoreStaticReference(m_colMemberDefList);
					writer.Write(value);
					break;
				}
				case MemberName.ColMemberDefIndexes:
					writer.WriteStringInt32Hashtable(m_colMemberDefIndexes);
					break;
				case MemberName.RowMemberInstanceIndexes:
					writer.WriteStringInt32Hashtable(m_rowMemberInstanceIndexes);
					break;
				case MemberName.MemberAtLevelIndexes:
					writer.WriteStringInt32Hashtable(m_memberAtLevelIndexes);
					break;
				case MemberName.CellPageBreaks:
					writer.Write(m_ignoreCellPageBreaks);
					break;
				case MemberName.HeaderRowCols:
					writer.Write(m_headerRowCols);
					break;
				case MemberName.HeaderColumnRows:
					writer.Write(m_headerColumnRows);
					break;
				case MemberName.RowMemberIndexCell:
					writer.Write(m_rowMemberIndexCell);
					break;
				case MemberName.ColMemberIndexCell:
					writer.Write(m_colMemberIndexCell);
					break;
				case MemberName.ColsBeforeRowHeaders:
					writer.Write(m_colsBeforeRowHeaders);
					break;
				case MemberName.ColumnHeaders:
					writer.Write(m_columnHeaders);
					break;
				case MemberName.ColumnHeadersHeights:
					writer.Write(m_colHeaderHeights);
					break;
				case MemberName.RowHeaders:
					writer.Write(m_rowHeaders);
					break;
				case MemberName.RowHeadersWidths:
					writer.Write(m_rowHeaderWidths);
					break;
				case MemberName.DetailRows:
					writer.Write(m_detailRows);
					break;
				case MemberName.CornerCells:
					writer.Write(m_cornerCells);
					break;
				case MemberName.GroupPageBreaks:
					writer.Write(m_ignoreGroupPageBreaks);
					break;
				case MemberName.ColumnInfo:
					writer.Write(m_columnInfo);
					break;
				case MemberName.IgnoreCol:
					writer.Write(m_ignoreCol);
					break;
				case MemberName.IgnoreRow:
					writer.Write(m_ignoreRow);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.TablixState:
					m_tablixStateFlags = (TablixState)reader.ReadInt16();
					break;
				case MemberName.BodyRows:
				{
					int id3 = reader.ReadInt32();
					m_bodyRows = (TablixRowCollection)scalabilityCache.FetchStaticReference(id3);
					break;
				}
				case MemberName.BodyRowHeights:
					m_bodyRowsHeights = reader.ReadDoubleArray();
					break;
				case MemberName.BodyColWidths:
					m_bodyColWidths = reader.ReadDoubleArray();
					break;
				case MemberName.RowMembersDepth:
					m_rowMembersDepth = reader.ReadInt32();
					break;
				case MemberName.ColMembersDepth:
					m_colMembersDepth = reader.ReadInt32();
					break;
				case MemberName.RowMemberDef:
				{
					int id2 = reader.ReadInt32();
					m_rowMemberDefList = (List<RPLTablixMemberDef>)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.RowMemberDefIndexes:
					m_rowMemberDefIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.ColMemberDef:
				{
					int id = reader.ReadInt32();
					m_colMemberDefList = (List<RPLTablixMemberDef>)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ColMemberDefIndexes:
					m_colMemberDefIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.RowMemberInstanceIndexes:
					m_rowMemberInstanceIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.MemberAtLevelIndexes:
					m_memberAtLevelIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.CellPageBreaks:
					m_ignoreCellPageBreaks = reader.ReadInt32();
					break;
				case MemberName.HeaderRowCols:
					m_headerRowCols = reader.ReadInt32();
					break;
				case MemberName.HeaderColumnRows:
					m_headerColumnRows = reader.ReadInt32();
					break;
				case MemberName.RowMemberIndexCell:
					m_rowMemberIndexCell = reader.ReadInt32();
					break;
				case MemberName.ColMemberIndexCell:
					m_colMemberIndexCell = reader.ReadInt32();
					break;
				case MemberName.ColsBeforeRowHeaders:
					m_colsBeforeRowHeaders = reader.ReadInt32();
					break;
				case MemberName.ColumnHeaders:
					m_columnHeaders = reader.ReadGenericListOfRIFObjects<PageStructMemberCell>();
					break;
				case MemberName.ColumnHeadersHeights:
					m_colHeaderHeights = reader.ReadGenericListOfRIFObjects<SizeInfo>();
					break;
				case MemberName.RowHeaders:
					m_rowHeaders = reader.ReadGenericListOfRIFObjects<PageStructMemberCell>();
					break;
				case MemberName.RowHeadersWidths:
					m_rowHeaderWidths = reader.ReadGenericListOfRIFObjects<SizeInfo>();
					break;
				case MemberName.DetailRows:
					m_detailRows = reader.ReadRIFObject<ScalableList<RowInfo>>();
					break;
				case MemberName.CornerCells:
					m_cornerCells = reader.Read2DArrayOfRIFObjects<PageCornerCell>();
					break;
				case MemberName.GroupPageBreaks:
					m_ignoreGroupPageBreaks = reader.ReadInt32();
					break;
				case MemberName.ColumnInfo:
					m_columnInfo = reader.ReadRIFObject<ScalableList<ColumnInfo>>();
					break;
				case MemberName.IgnoreCol:
					m_ignoreCol = reader.ReadInt32();
					break;
				case MemberName.IgnoreRow:
					m_ignoreRow = reader.ReadInt32();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Tablix;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TablixState, Token.Int16));
				list.Add(new MemberInfo(MemberName.BodyRows, Token.Int32));
				list.Add(new MemberInfo(MemberName.BodyRowHeights, ObjectType.PrimitiveTypedArray, Token.Double));
				list.Add(new MemberInfo(MemberName.BodyColWidths, ObjectType.PrimitiveTypedArray, Token.Double));
				list.Add(new MemberInfo(MemberName.RowMembersDepth, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMembersDepth, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberDefIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMemberDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMemberDefIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberInstanceIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.MemberAtLevelIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.CellPageBreaks, Token.Int32));
				list.Add(new MemberInfo(MemberName.HeaderRowCols, Token.Int32));
				list.Add(new MemberInfo(MemberName.HeaderColumnRows, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberIndexCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMemberIndexCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColsBeforeRowHeaders, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColumnHeaders, ObjectType.RIFObjectList, ObjectType.PageStructMemberCell));
				list.Add(new MemberInfo(MemberName.ColumnHeadersHeights, ObjectType.RIFObjectList, ObjectType.SizeInfo));
				list.Add(new MemberInfo(MemberName.RowHeaders, ObjectType.RIFObjectList, ObjectType.PageStructMemberCell));
				list.Add(new MemberInfo(MemberName.RowHeadersWidths, ObjectType.RIFObjectList, ObjectType.SizeInfo));
				list.Add(new MemberInfo(MemberName.DetailRows, ObjectType.ScalableList, ObjectType.RowInfo));
				list.Add(new MemberInfo(MemberName.CornerCells, ObjectType.Array2D, ObjectType.PageCornerCell));
				list.Add(new MemberInfo(MemberName.GroupPageBreaks, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColumnInfo, ObjectType.ScalableList, ObjectType.ColumnInfo));
				list.Add(new MemberInfo(MemberName.IgnoreCol, Token.Int32));
				list.Add(new MemberInfo(MemberName.IgnoreRow, Token.Int32));
				return new Declaration(ObjectType.Tablix, ObjectType.PageItem, list);
			}
			return m_declaration;
		}
	}
}
