using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemInstance : ReportItemInstance, IPageItem
	{
		private ReportItemColInstance m_altReportItemColInstance;

		private CustomReportItemHeadingInstanceList m_columnInstances;

		private CustomReportItemHeadingInstanceList m_rowInstances;

		private CustomReportItemCellInstancesList m_cells;

		[NonSerialized]
		private int m_currentCellOuterIndex;

		[NonSerialized]
		private int m_currentCellInnerIndex;

		[NonSerialized]
		private int m_currentOuterStaticIndex;

		[NonSerialized]
		private int m_currentInnerStaticIndex;

		[NonSerialized]
		private CustomReportItemHeadingInstanceList m_innerHeadingInstanceList;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportItemColInstance AltReportItemColInstance
		{
			get
			{
				return m_altReportItemColInstance;
			}
			set
			{
				m_altReportItemColInstance = value;
			}
		}

		internal CustomReportItemHeadingInstanceList ColumnInstances
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

		internal CustomReportItemHeadingInstanceList RowInstances
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

		internal CustomReportItemCellInstancesList Cells
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

		internal CustomReportItemHeadingInstanceList InnerHeadingInstanceList
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

		internal CustomReportItemInstance()
		{
		}

		internal CustomReportItemInstance(ReportProcessing.ProcessingContext pc, CustomReportItem reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new CustomReportItemInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			if (reportItemDef.DataSetName != null)
			{
				m_columnInstances = new CustomReportItemHeadingInstanceList();
				m_rowInstances = new CustomReportItemHeadingInstanceList();
				m_cells = new CustomReportItemCellInstancesList();
			}
		}

		internal CustomReportItemCellInstance AddCell(ReportProcessing.ProcessingContext pc)
		{
			CustomReportItem customReportItem = (CustomReportItem)m_reportItemDef;
			bool num = customReportItem.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column;
			int rowIndex;
			int colIndex;
			if (num)
			{
				rowIndex = m_currentOuterStaticIndex;
				colIndex = m_currentInnerStaticIndex;
			}
			else
			{
				colIndex = m_currentOuterStaticIndex;
				rowIndex = m_currentInnerStaticIndex;
			}
			CustomReportItemCellInstance customReportItemCellInstance = new CustomReportItemCellInstance(rowIndex, colIndex, customReportItem, pc);
			if (num)
			{
				m_cells[m_currentCellOuterIndex].Add(customReportItemCellInstance);
			}
			else
			{
				if (m_currentCellOuterIndex == 0)
				{
					Global.Tracer.Assert(m_cells.Count == m_currentCellInnerIndex);
					CustomReportItemCellInstanceList value = new CustomReportItemCellInstanceList();
					m_cells.Add(value);
				}
				m_cells[m_currentCellInnerIndex].Add(customReportItemCellInstance);
			}
			m_currentCellInnerIndex++;
			return customReportItemCellInstance;
		}

		internal void NewOuterCells()
		{
			if (0 < m_currentCellInnerIndex || m_cells.Count == 0)
			{
				if (((CustomReportItem)m_reportItemDef).ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					CustomReportItemCellInstanceList value = new CustomReportItemCellInstanceList();
					m_cells.Add(value);
				}
				if (0 < m_currentCellInnerIndex)
				{
					m_currentCellOuterIndex++;
					m_currentCellInnerIndex = 0;
				}
			}
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadCustomReportItemInstanceInfo((CustomReportItem)m_reportItemDef);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.AltReportItemColInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.RowInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.Cells, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemCellInstancesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}
	}
}
