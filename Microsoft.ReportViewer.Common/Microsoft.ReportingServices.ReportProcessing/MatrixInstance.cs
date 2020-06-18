using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixInstance : ReportItemInstance, IShowHideContainer, IPageItem
	{
		private ReportItemInstance m_cornerContent;

		private MatrixHeadingInstanceList m_columnInstances;

		private MatrixHeadingInstanceList m_rowInstances;

		private MatrixCellInstancesList m_cells;

		private int m_instanceCountOfInnerRowWithPageBreak;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		private int m_currentCellOuterIndex;

		[NonSerialized]
		private int m_currentCellInnerIndex;

		[NonSerialized]
		private int m_currentOuterStaticIndex;

		[NonSerialized]
		private int m_currentInnerStaticIndex;

		[NonSerialized]
		private MatrixHeadingInstanceList m_innerHeadingInstanceList;

		[NonSerialized]
		private bool m_inFirstPage;

		[NonSerialized]
		private int m_extraPagesFilled;

		[NonSerialized]
		private int m_numberOfChildrenOnThisPage;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportItemInstance CornerContent
		{
			get
			{
				return m_cornerContent;
			}
			set
			{
				m_cornerContent = value;
			}
		}

		internal MatrixHeadingInstanceList ColumnInstances
		{
			get
			{
				return m_columnInstances;
			}
			set
			{
				m_columnInstances = value;
			}
		}

		internal MatrixHeadingInstanceList RowInstances
		{
			get
			{
				return m_rowInstances;
			}
			set
			{
				m_rowInstances = value;
			}
		}

		internal MatrixCellInstancesList Cells
		{
			get
			{
				return m_cells;
			}
			set
			{
				m_cells = value;
			}
		}

		internal int CellColumnCount
		{
			get
			{
				if (0 < m_cells.Count)
				{
					return m_cells[0].Count;
				}
				return 0;
			}
		}

		internal int CellRowCount => m_cells.Count;

		internal int InstanceCountOfInnerRowWithPageBreak
		{
			get
			{
				return m_instanceCountOfInnerRowWithPageBreak;
			}
			set
			{
				m_instanceCountOfInnerRowWithPageBreak = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return m_renderingPages;
			}
			set
			{
				m_renderingPages = value;
			}
		}

		internal int CurrentCellOuterIndex => m_currentCellOuterIndex;

		internal int CurrentCellInnerIndex => m_currentCellInnerIndex;

		internal int CurrentOuterStaticIndex
		{
			set
			{
				m_currentOuterStaticIndex = value;
			}
		}

		internal int CurrentInnerStaticIndex
		{
			set
			{
				m_currentInnerStaticIndex = value;
			}
		}

		internal MatrixHeadingInstanceList InnerHeadingInstanceList
		{
			get
			{
				return m_innerHeadingInstanceList;
			}
			set
			{
				m_innerHeadingInstanceList = value;
			}
		}

		internal bool InFirstPage
		{
			get
			{
				return m_inFirstPage;
			}
			set
			{
				m_inFirstPage = value;
			}
		}

		internal int ExtraPagesFilled
		{
			get
			{
				if (m_extraPagesFilled < 1)
				{
					return 0;
				}
				if (m_numberOfChildrenOnThisPage > 1)
				{
					return m_extraPagesFilled;
				}
				return m_extraPagesFilled - 1;
			}
			set
			{
				m_extraPagesFilled = value;
			}
		}

		internal int NumberOfChildrenOnThisPage
		{
			get
			{
				return m_numberOfChildrenOnThisPage;
			}
			set
			{
				m_numberOfChildrenOnThisPage = value;
			}
		}

		int IPageItem.StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal MatrixInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (MatrixInstanceInfo)m_instanceInfo;
			}
		}

		internal Matrix MatrixDef => m_reportItemDef as Matrix;

		internal MatrixInstance(ReportProcessing.ProcessingContext pc, Matrix reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new MatrixInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			m_columnInstances = new MatrixHeadingInstanceList();
			m_rowInstances = new MatrixHeadingInstanceList();
			m_cells = new MatrixCellInstancesList();
			m_renderingPages = new RenderingPagesRangesList();
			reportItemDef.CurrentPage = reportItemDef.StartPage;
			m_startPage = reportItemDef.StartPage;
			if (reportItemDef.FirstCellInstances == null)
			{
				int count = reportItemDef.CellReportItems.Count;
				reportItemDef.FirstCellInstances = new BoolList(count);
				for (int i = 0; i < count; i++)
				{
					reportItemDef.FirstCellInstances.Add(true);
				}
			}
			m_inFirstPage = pc.ChunkManager.InFirstPage;
		}

		internal MatrixInstance()
		{
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			Matrix matrix = (Matrix)base.ReportItemDef;
			if (matrix.CornerReportItems.Count > 0)
			{
				if (m_cornerContent != null)
				{
					obj = ((ISearchByUniqueName)m_cornerContent).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
				else
				{
					NonComputedUniqueNames nonCompNames2 = ((MatrixInstanceInfo)GetInstanceInfo(chunkManager, inPageSection: false)).CornerNonComputedNames;
					obj = ((ISearchByUniqueName)matrix.CornerReportItems[0]).Find(targetUniqueName, ref nonCompNames2, chunkManager);
					if (obj != null)
					{
						nonCompNames = nonCompNames2;
						return obj;
					}
				}
			}
			obj = ((ISearchByUniqueName)m_columnInstances).Find(targetUniqueName, ref nonCompNames, chunkManager);
			if (obj != null)
			{
				return obj;
			}
			obj = ((ISearchByUniqueName)m_rowInstances).Find(targetUniqueName, ref nonCompNames, chunkManager);
			if (obj != null)
			{
				return obj;
			}
			int count = m_cells.Count;
			for (int i = 0; i < count; i++)
			{
				MatrixCellInstanceList matrixCellInstanceList = m_cells[i];
				int count2 = matrixCellInstanceList.Count;
				for (int j = 0; j < count2; j++)
				{
					MatrixCellInstance matrixCellInstance = matrixCellInstanceList[j];
					MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(chunkManager);
					int index = instanceInfo.RowIndex * matrix.MatrixColumns.Count + instanceInfo.ColumnIndex;
					if (matrix.CellReportItems.IsReportItemComputed(index))
					{
						if (matrixCellInstance.Content != null)
						{
							obj = ((ISearchByUniqueName)matrixCellInstance.Content).Find(targetUniqueName, ref nonCompNames, chunkManager);
							if (obj != null)
							{
								return obj;
							}
						}
					}
					else
					{
						NonComputedUniqueNames nonCompNames3 = instanceInfo.ContentUniqueNames;
						obj = ((ISearchByUniqueName)matrix.CellReportItems[index]).Find(targetUniqueName, ref nonCompNames3, chunkManager);
						if (obj != null)
						{
							nonCompNames = nonCompNames3;
							return obj;
						}
					}
				}
			}
			return null;
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(m_uniqueName, m_reportItemDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_reportItemDef.Visibility);
		}

		internal ReportItem GetCellReportItemDef(int cellRIIndex, out bool computed)
		{
			if (-1 == cellRIIndex)
			{
				cellRIIndex = GetCurrentCellRIIndex();
			}
			computed = false;
			((Matrix)m_reportItemDef).CellReportItems.GetReportItem(cellRIIndex, out computed, out int _, out ReportItem reportItem);
			return reportItem;
		}

		internal MatrixCellInstance AddCell(ReportProcessing.ProcessingContext pc, out NonComputedUniqueNames cellNonComputedUniqueNames)
		{
			Matrix matrix = (Matrix)m_reportItemDef;
			int currentCellRIIndex = GetCurrentCellRIIndex();
			bool flag = matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column;
			int num;
			int colIndex;
			if (flag)
			{
				num = m_currentOuterStaticIndex;
				colIndex = m_currentInnerStaticIndex;
			}
			else
			{
				colIndex = m_currentOuterStaticIndex;
				num = m_currentInnerStaticIndex;
			}
			MatrixCellInstance matrixCellInstance = null;
			matrixCellInstance = ((pc.HeadingInstance == null || pc.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass == null) ? new MatrixCellInstance(num, colIndex, matrix, currentCellRIIndex, pc, out cellNonComputedUniqueNames) : new MatrixSubtotalCellInstance(num, colIndex, matrix, currentCellRIIndex, pc, out cellNonComputedUniqueNames));
			if ((!flag && m_currentCellOuterIndex == 0) || (flag && m_currentCellInnerIndex == 0))
			{
				if (!pc.Pagination.IgnoreHeight)
				{
					pc.Pagination.AddToCurrentPageHeight(matrix, matrix.MatrixRows[num].HeightValue);
				}
				if (!pc.Pagination.IgnorePageBreak && pc.Pagination.CurrentPageHeight >= pc.Pagination.PageHeight && m_rowInstances.Count > 1)
				{
					pc.Pagination.SetCurrentPageHeight(matrix, 0.0);
					m_extraPagesFilled++;
					matrix.CurrentPage = m_startPage + m_extraPagesFilled;
					m_numberOfChildrenOnThisPage = 0;
				}
				else
				{
					m_numberOfChildrenOnThisPage++;
				}
			}
			if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				m_cells[m_currentCellOuterIndex].Add(matrixCellInstance);
			}
			else
			{
				if (m_currentCellOuterIndex == 0)
				{
					Global.Tracer.Assert(m_cells.Count == m_currentCellInnerIndex);
					MatrixCellInstanceList value = new MatrixCellInstanceList();
					m_cells.Add(value);
				}
				m_cells[m_currentCellInnerIndex].Add(matrixCellInstance);
			}
			m_currentCellInnerIndex++;
			return matrixCellInstance;
		}

		internal void NewOuterCells()
		{
			if (0 < m_currentCellInnerIndex || m_cells.Count == 0)
			{
				if (((Matrix)m_reportItemDef).ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					MatrixCellInstanceList value = new MatrixCellInstanceList();
					m_cells.Add(value);
				}
				if (0 < m_currentCellInnerIndex)
				{
					m_currentCellOuterIndex++;
					m_currentCellInnerIndex = 0;
				}
			}
		}

		internal int GetCurrentCellRIIndex()
		{
			Matrix obj = (Matrix)m_reportItemDef;
			int count = obj.MatrixColumns.Count;
			if (obj.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				return m_currentOuterStaticIndex * count + m_currentInnerStaticIndex;
			}
			return m_currentInnerStaticIndex * count + m_currentOuterStaticIndex;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CornerContent, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.RowInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.Cells, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixCellInstancesList));
			memberInfoList.Add(new MemberInfo(MemberName.InstanceCountOfInnerRowWithPageBreak, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadMatrixInstanceInfo((Matrix)m_reportItemDef);
		}
	}
}
