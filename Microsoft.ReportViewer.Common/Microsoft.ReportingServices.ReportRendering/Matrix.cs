using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Matrix : DataRegion
	{
		private ReportItem m_corner;

		private MatrixCellCollection m_cells;

		private MatrixMemberCollection m_columns;

		private MatrixMemberCollection m_rows;

		private int m_groupsBeforeRowHeaders = -1;

		private int m_cellsBeforeRowHeaders = -1;

		private SizeCollection m_cellWidths;

		private SizeCollection m_cellHeights;

		private bool m_noRows;

		private List<int> m_rowMemberMapping;

		private List<int> m_colMemberMapping;

		public MatrixLayoutDirection LayoutDirection
		{
			get
			{
				if (((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).LayoutDirection)
				{
					return MatrixLayoutDirection.RTL;
				}
				return MatrixLayoutDirection.LTR;
			}
		}

		public override bool PageBreakAtEnd
		{
			get
			{
				if (!((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PageBreakAtEnd)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtEnd;
				}
				return true;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (!((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PageBreakAtStart)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtStart;
				}
				return true;
			}
		}

		public bool GroupBreakAtStart => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtStart;

		public bool GroupBreakAtEnd => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtEnd;

		public ReportItem Corner
		{
			get
			{
				ReportItem reportItem = m_corner;
				if (m_corner == null)
				{
					Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef;
					if (matrix.CornerReportItems != null && 0 < matrix.CornerReportItems.Count)
					{
						Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef = matrix.CornerReportItems[0];
						ReportItemInstance reportItemInstance = null;
						NonComputedUniqueNames nonComputedUniqueNames = null;
						if (base.ReportItemInstance != null)
						{
							reportItemInstance = ((MatrixInstance)base.ReportItemInstance).CornerContent;
							nonComputedUniqueNames = ((MatrixInstanceInfo)base.InstanceInfo).CornerNonComputedNames;
						}
						reportItem = ReportItem.CreateItem(0, reportItemDef, reportItemInstance, base.RenderingContext, nonComputedUniqueNames);
						if (base.RenderingContext.CacheState)
						{
							m_corner = reportItem;
						}
					}
				}
				return reportItem;
			}
		}

		public MatrixCellCollection CellCollection
		{
			get
			{
				MatrixCellCollection matrixCellCollection = m_cells;
				if (m_cells == null)
				{
					int num = 0;
					int num2 = 0;
					if (!m_noRows && base.ReportItemInstance != null && 0 < ((MatrixInstance)base.ReportItemInstance).Cells.Count)
					{
						num = ((MatrixInstance)base.ReportItemInstance).CellRowCount;
						num2 = ((MatrixInstance)base.ReportItemInstance).CellColumnCount;
					}
					else
					{
						num = ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).MatrixRows.Count;
						num2 = ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).MatrixColumns.Count;
					}
					matrixCellCollection = new MatrixCellCollection(this, num, num2);
					if (base.RenderingContext.CacheState)
					{
						m_cells = matrixCellCollection;
					}
				}
				return matrixCellCollection;
			}
		}

		public MatrixMemberCollection ColumnMemberCollection
		{
			get
			{
				MatrixMemberCollection matrixMemberCollection = m_columns;
				if (m_columns == null)
				{
					matrixMemberCollection = new MatrixMemberCollection(this, null, ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).Columns, (base.ReportItemInstance == null) ? null : ((MatrixInstance)base.ReportItemInstance).ColumnInstances, m_colMemberMapping, isParentSubTotal: false);
					if (base.RenderingContext.CacheState)
					{
						m_columns = matrixMemberCollection;
					}
				}
				return matrixMemberCollection;
			}
		}

		public MatrixMemberCollection RowMemberCollection
		{
			get
			{
				MatrixMemberCollection matrixMemberCollection = m_rows;
				if (m_rows == null)
				{
					matrixMemberCollection = new MatrixMemberCollection(this, null, ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).Rows, (base.ReportItemInstance == null) ? null : ((MatrixInstance)base.ReportItemInstance).RowInstances, m_rowMemberMapping, isParentSubTotal: false);
					if (base.RenderingContext.CacheState)
					{
						m_rows = matrixMemberCollection;
					}
				}
				return matrixMemberCollection;
			}
		}

		public int CellColumns
		{
			get
			{
				if (m_noRows || base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((MatrixInstance)base.ReportItemInstance).CellColumnCount;
			}
		}

		public int CellRows
		{
			get
			{
				if (m_noRows || base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((MatrixInstance)base.ReportItemInstance).CellRowCount;
			}
		}

		public int MatrixPages
		{
			get
			{
				if (m_noRows || base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((MatrixInstance)base.ReportItemInstance).InstanceCountOfInnerRowWithPageBreak;
			}
		}

		public int PageBreakRow => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).InnerRowLevelWithPageBreak;

		public int Columns => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).ColumnCount;

		public int Rows => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).RowCount;

		public override bool NoRows => m_noRows;

		public int GroupsBeforeRowHeaders
		{
			get
			{
				if (m_groupsBeforeRowHeaders < 0)
				{
					CalculateGroupsCellsBeforeRowHeaders();
				}
				return m_groupsBeforeRowHeaders;
			}
		}

		public int CellsBeforeRowHeaders
		{
			get
			{
				if (m_cellsBeforeRowHeaders < 0)
				{
					CalculateGroupsCellsBeforeRowHeaders();
				}
				return m_cellsBeforeRowHeaders;
			}
		}

		public SizeCollection CellWidths
		{
			get
			{
				SizeCollection sizeCollection = m_cellWidths;
				if (m_cellWidths == null)
				{
					sizeCollection = new SizeCollection(this, widthsCollection: true);
					if (base.RenderingContext.CacheState)
					{
						m_cellWidths = sizeCollection;
					}
				}
				return sizeCollection;
			}
		}

		public SizeCollection CellHeights
		{
			get
			{
				SizeCollection sizeCollection = m_cellHeights;
				if (m_cellHeights == null)
				{
					sizeCollection = new SizeCollection(this, widthsCollection: false);
					if (base.RenderingContext.CacheState)
					{
						m_cellHeights = sizeCollection;
					}
				}
				return sizeCollection;
			}
		}

		public bool UseOWC => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).UseOWC;

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((MatrixInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		public bool RowGroupingFixedHeader => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).RowGroupingFixedHeader;

		public bool ColumnGroupingFixedHeader => ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).ColumnGroupingFixedHeader;

		internal Matrix(int intUniqueName, Microsoft.ReportingServices.ReportProcessing.Matrix reportItemDef, MatrixInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			if (reportItemInstance != null && reportItemInstance.Cells.Count != 0 && reportItemInstance.Cells[0].Count != 0)
			{
				m_rowMemberMapping = CalculateMapping(reportItemDef.Rows, reportItemInstance.RowInstances, inParentSubtotal: false);
				m_colMemberMapping = CalculateMapping(reportItemDef.Columns, reportItemInstance.ColumnInstances, inParentSubtotal: false);
				m_noRows = (m_rowMemberMapping.Count == 0 || m_colMemberMapping.Count == 0);
			}
			else
			{
				m_noRows = true;
			}
		}

		public bool IsRowMemberOnThisPage(int groupIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = ((MatrixInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages == null)
			{
				return true;
			}
			Global.Tracer.Assert(groupIndex >= 0 && groupIndex < childrenStartAndEndPages.Count);
			if (groupIndex >= childrenStartAndEndPages.Count)
			{
				return false;
			}
			RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[groupIndex];
			startPage = renderingPagesRanges.StartPage;
			endPage = renderingPagesRanges.EndPage;
			if (pageNumber >= startPage)
			{
				return pageNumber <= endPage;
			}
			return false;
		}

		public void GetRowMembersOnPage(int page, out int startMember, out int endMember)
		{
			startMember = -1;
			endMember = -1;
			if (base.ReportItemInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = ((MatrixInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startMember, ref endMember);
				}
			}
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch || NoRows)
			{
				return false;
			}
			IntList hiddenColumns = null;
			bool flag = SearchMatrixColumns(ColumnMemberCollection, ref hiddenColumns, searchContext);
			if (!flag)
			{
				flag = SearchMatrixRowsContent(this, null, searchContext, hiddenColumns);
			}
			return flag;
		}

		private static void BuildHiddenColumns(MatrixMember member, ref IntList hiddenColumns)
		{
			if (hiddenColumns == null)
			{
				hiddenColumns = new IntList();
			}
			for (int i = 0; i < member.ColumnSpan; i++)
			{
				hiddenColumns.Add(member.MemberCellIndex + i);
			}
		}

		private static bool HiddenColumn(IntList hiddenColumns, ref int columnIndex, int cellIndex)
		{
			bool result = false;
			if (hiddenColumns != null && columnIndex < hiddenColumns.Count)
			{
				while (columnIndex < hiddenColumns.Count && cellIndex > hiddenColumns[columnIndex])
				{
					columnIndex++;
				}
				if (cellIndex == hiddenColumns[columnIndex])
				{
					columnIndex++;
					result = true;
				}
			}
			return result;
		}

		private static bool SearchMatrixRowsContent(Matrix matrix, MatrixMember member, SearchContext searchContext, IntList hiddenColumns)
		{
			bool flag = false;
			int startMember = 0;
			int endMember = 0;
			int num = 0;
			MatrixMember matrixMember = null;
			MatrixMemberCollection matrixMemberCollection = null;
			MatrixMemberCollection matrixMemberCollection2 = null;
			SearchContext searchContext2 = new SearchContext(searchContext);
			bool flag2 = false;
			matrixMemberCollection2 = ((member != null) ? member.Children : matrix.RowMemberCollection);
			if (searchContext.ItemStartPage != searchContext.ItemEndPage)
			{
				if (member == null)
				{
					matrix.GetRowMembersOnPage(searchContext.SearchPage, out startMember, out endMember);
				}
				else
				{
					member.GetChildRowMembersOnPage(searchContext.SearchPage, out startMember, out endMember);
				}
				flag2 = true;
			}
			else
			{
				startMember = 0;
				endMember = matrixMemberCollection2.Count - 1;
			}
			num = endMember - startMember + 1;
			int num2 = startMember;
			while (!flag && num2 <= endMember)
			{
				matrixMember = matrixMemberCollection2[num2];
				if (matrixMember.Hidden)
				{
					num--;
				}
				else
				{
					matrixMemberCollection = matrixMember.Children;
					if (matrixMemberCollection != null)
					{
						flag = matrixMember.ReportItem.Search(searchContext2);
						if (!flag)
						{
							if (flag2 && (num2 == startMember || num2 == endMember))
							{
								int startPage = 0;
								int endPage = 0;
								SearchContext searchContext3 = new SearchContext(searchContext);
								if (member == null)
								{
									matrix.IsRowMemberOnThisPage(num2, searchContext.SearchPage, out startPage, out endPage);
								}
								else
								{
									member.IsRowMemberOnThisPage(num2, searchContext.SearchPage, out startPage, out endPage);
								}
								searchContext3.ItemStartPage = startPage;
								searchContext3.ItemEndPage = endPage;
								flag = SearchMatrixRowsContent(matrix, matrixMember, searchContext3, hiddenColumns);
							}
							else
							{
								flag = SearchMatrixRowsContent(matrix, matrixMember, searchContext2, hiddenColumns);
							}
						}
					}
					else
					{
						flag = matrixMember.ReportItem.Search(searchContext2);
						if (!flag)
						{
							flag = SearchRangeCells(matrix, matrixMember.MemberCellIndex, hiddenColumns, searchContext2);
						}
					}
				}
				num2++;
			}
			if (!flag && num == 0)
			{
				if (!matrixMember.IsTotal)
				{
					matrixMember = matrixMemberCollection2[0];
				}
				matrixMemberCollection = matrixMember.Children;
				flag = ((matrixMemberCollection == null) ? SearchRangeCells(matrix, matrixMember.MemberCellIndex, hiddenColumns, searchContext2) : SearchRowTotal(matrix, matrixMemberCollection, hiddenColumns, searchContext2));
			}
			return flag;
		}

		private static bool SearchMatrixColumns(MatrixMemberCollection columns, ref IntList hiddenColumns, SearchContext searchContext)
		{
			if (columns == null)
			{
				return false;
			}
			bool flag = false;
			int num = 0;
			int num2 = columns.Count - 1;
			int num3 = 0;
			MatrixMember matrixMember = null;
			MatrixMemberCollection matrixMemberCollection = null;
			SearchContext searchContext2 = new SearchContext(searchContext);
			int index = 0;
			int count = 0;
			num3 = num2 - num + 1;
			int num4 = num;
			while (!flag && num4 <= num2)
			{
				matrixMember = columns[num4];
				if (matrixMember.Hidden)
				{
					if (matrixMember.IsTotal)
					{
						if (hiddenColumns != null)
						{
							index = hiddenColumns.Count;
						}
						count = matrixMember.ColumnSpan;
					}
					BuildHiddenColumns(matrixMember, ref hiddenColumns);
					num3--;
				}
				else
				{
					flag = matrixMember.ReportItem.Search(searchContext2);
					if (!flag)
					{
						matrixMemberCollection = matrixMember.Children;
						flag = SearchMatrixColumns(matrixMemberCollection, ref hiddenColumns, searchContext2);
					}
				}
				num4++;
			}
			if (num3 == 0)
			{
				hiddenColumns.RemoveRange(index, count);
				if (!flag)
				{
					if (!matrixMember.IsTotal)
					{
						matrixMember = columns[0];
					}
					matrixMemberCollection = matrixMember.Children;
					if (matrixMemberCollection != null)
					{
						int num5 = 0;
						while (!flag && num5 < matrixMemberCollection.Count)
						{
							matrixMember = matrixMemberCollection[num5];
							flag = matrixMember.ReportItem.Search(searchContext2);
							num5++;
						}
					}
				}
			}
			return flag;
		}

		private static bool SearchRangeCells(Matrix matrix, int indexRow, IntList hiddenColumns, SearchContext searchContext)
		{
			int num = 0;
			int columnIndex = 0;
			bool flag = false;
			MatrixCellCollection cellCollection = matrix.CellCollection;
			num = 0;
			while (!flag && num < matrix.CellColumns)
			{
				if (!HiddenColumn(hiddenColumns, ref columnIndex, num))
				{
					flag = cellCollection[indexRow, num].ReportItem.Search(searchContext);
				}
				num++;
			}
			return flag;
		}

		private static bool SearchRowTotal(Matrix matrix, MatrixMemberCollection rowTotalChildren, IntList hiddenColumns, SearchContext searchContext)
		{
			bool flag = false;
			MatrixMember matrixMember = null;
			int num = 0;
			while (!flag && num < rowTotalChildren.Count)
			{
				matrixMember = rowTotalChildren[num];
				flag = matrixMember.ReportItem.Search(searchContext);
				if (!flag)
				{
					flag = SearchRangeCells(matrix, matrixMember.MemberCellIndex, hiddenColumns, searchContext);
				}
				num++;
			}
			return flag;
		}

		internal static List<int> CalculateMapping(MatrixHeading headingDef, MatrixHeadingInstanceList headingInstances, bool inParentSubtotal)
		{
			List<int> list = new List<int>();
			if (headingInstances == null)
			{
				return list;
			}
			bool flag = true;
			for (int i = 0; i < headingInstances.Count; i++)
			{
				if (inParentSubtotal || headingInstances[i].IsSubtotal || !IsEmpty(headingDef, headingInstances[i]))
				{
					if (!headingInstances[i].IsSubtotal)
					{
						flag = false;
					}
					list.Add(i);
				}
			}
			if (flag && list.Count <= 1)
			{
				list.Clear();
			}
			return list;
		}

		private static bool IsEmpty(MatrixHeading headingDef, MatrixHeadingInstance headingInstance)
		{
			if (headingDef == null || headingDef.SubHeading == null)
			{
				return false;
			}
			if (headingInstance.SubHeadingInstances == null || headingInstance.SubHeadingInstances.Count == 0)
			{
				return true;
			}
			int count = headingInstance.SubHeadingInstances.Count;
			bool flag = true;
			for (int i = 0; i < count && flag; i++)
			{
				flag = IsEmpty(headingDef.SubHeading, headingInstance.SubHeadingInstances[i]);
			}
			return flag;
		}

		private void CalculateGroupsCellsBeforeRowHeaders()
		{
			m_groupsBeforeRowHeaders = ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).GroupsBeforeRowHeaders;
			if (m_groupsBeforeRowHeaders > 0 && base.ReportItemInstance != null)
			{
				MatrixHeadingInstanceList columnInstances = ((MatrixInstance)base.ReportItemInstance).ColumnInstances;
				MatrixMemberCollection matrixMemberCollection = null;
				int num = -1;
				if (columnInstances != null && 0 < columnInstances.Count)
				{
					num = columnInstances.Count - 1;
					if (columnInstances[0].IsSubtotal || (m_groupsBeforeRowHeaders == num && columnInstances[num].IsSubtotal))
					{
						m_groupsBeforeRowHeaders++;
					}
				}
				m_cellsBeforeRowHeaders = 0;
				if (m_groupsBeforeRowHeaders > num + 1)
				{
					m_groupsBeforeRowHeaders = 0;
				}
				else
				{
					matrixMemberCollection = ColumnMemberCollection;
				}
				for (int i = 0; i < m_groupsBeforeRowHeaders; i++)
				{
					m_cellsBeforeRowHeaders += matrixMemberCollection[i].ColumnSpan;
				}
			}
			else
			{
				m_groupsBeforeRowHeaders = 0;
				m_cellsBeforeRowHeaders = 0;
			}
		}
	}
}
