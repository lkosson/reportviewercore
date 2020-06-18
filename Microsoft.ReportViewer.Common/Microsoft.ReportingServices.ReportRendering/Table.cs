using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Table : DataRegion
	{
		private TableGroupCollection m_tableGroups;

		private TableRowsCollection m_detailRows;

		private TableHeaderFooterRows m_headerRows;

		private TableHeaderFooterRows m_footerRows;

		private TableColumnCollection m_tableColumns;

		private bool m_calculatedFixedColumnHeaders;

		public override bool PageBreakAtEnd
		{
			get
			{
				if (((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PageBreakAtEnd)
				{
					return true;
				}
				if (((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).FooterRows == null || ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).FooterRepeatOnNewPage)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PropagatedPageBreakAtEnd;
				}
				return false;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PageBreakAtStart)
				{
					return true;
				}
				if (((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).HeaderRows == null || ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).HeaderRepeatOnNewPage)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PropagatedPageBreakAtStart;
				}
				return false;
			}
		}

		public bool GroupBreakAtStart => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).GroupBreakAtStart;

		public bool GroupBreakAtEnd => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).GroupBreakAtEnd;

		public TableColumnCollection Columns
		{
			get
			{
				TableColumnCollection tableColumnCollection = m_tableColumns;
				if (m_tableColumns == null)
				{
					tableColumnCollection = new TableColumnCollection(this, ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableColumns);
					if (base.RenderingContext.CacheState)
					{
						m_tableColumns = tableColumnCollection;
					}
				}
				return tableColumnCollection;
			}
		}

		public TableGroupCollection TableGroups
		{
			get
			{
				TableGroupCollection tableGroupCollection = m_tableGroups;
				if (m_tableGroups == null && ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableGroups != null)
				{
					tableGroupCollection = new TableGroupCollection(this, null, ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableGroups, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).TableGroupInstances);
					if (base.RenderingContext.CacheState)
					{
						m_tableGroups = tableGroupCollection;
					}
				}
				return tableGroupCollection;
			}
		}

		public TableHeaderFooterRows TableHeader
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Table table = (Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				TableHeaderFooterRows tableHeaderFooterRows = m_headerRows;
				if (m_headerRows == null && table.HeaderRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows(this, table.HeaderRepeatOnNewPage, table.HeaderRows, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).HeaderRowInstances);
					if (base.RenderingContext.CacheState)
					{
						m_headerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public TableHeaderFooterRows TableFooter
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Table table = (Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				TableHeaderFooterRows tableHeaderFooterRows = m_footerRows;
				if (m_footerRows == null && table.FooterRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows(this, table.FooterRepeatOnNewPage, table.FooterRows, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).FooterRowInstances);
					if (base.RenderingContext.CacheState)
					{
						m_footerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public TableRowsCollection DetailRows
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Table table = (Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				TableRowsCollection tableRowsCollection = m_detailRows;
				if (m_detailRows == null && table.TableGroups == null && table.TableDetail != null)
				{
					tableRowsCollection = new TableRowsCollection(this, table.TableDetail, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).TableDetailInstances);
					if (base.RenderingContext.CacheState)
					{
						m_detailRows = tableRowsCollection;
					}
				}
				return tableRowsCollection;
			}
		}

		public string DetailDataElementName => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailDataElementName;

		public string DetailDataCollectionName => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailDataCollectionName;

		public DataElementOutputTypes DetailDataElementOutput => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailDataElementOutput;

		public SharedHiddenState DetailSharedHidden
		{
			get
			{
				if (((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailGroup == null)
				{
					return Visibility.GetSharedHidden(((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableDetail.Visibility);
				}
				return Visibility.GetSharedHidden(((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailGroup.Visibility);
			}
		}

		public override bool NoRows
		{
			get
			{
				TableInstance tableInstance = (TableInstance)base.ReportItemInstance;
				if (tableInstance == null || (tableInstance.TableGroupInstances != null && tableInstance.TableGroupInstances.Count == 0) || (tableInstance.TableDetailInstances != null && tableInstance.TableDetailInstances.Count == 0))
				{
					return true;
				}
				return false;
			}
		}

		public bool UseOWC => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).UseOWC;

		public bool ContainsNonSharedStyles => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).OWCNonSharedStyles;

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((TableInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		public bool FixedHeader => ((Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef).FixedHeader;

		internal bool HasFixedColumnHeaders
		{
			get
			{
				if (!FixedHeader)
				{
					return false;
				}
				Microsoft.ReportingServices.ReportProcessing.Table table = (Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				if (!m_calculatedFixedColumnHeaders)
				{
					table.HasFixedColumnHeaders = false;
					if (table.FixedHeader)
					{
						int count = table.TableColumns.Count;
						table.HasFixedColumnHeaders = (table.TableColumns[0].FixedHeader || table.TableColumns[count - 1].FixedHeader);
					}
					m_calculatedFixedColumnHeaders = true;
				}
				return table.HasFixedColumnHeaders;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.Table TableDefinition => (Microsoft.ReportingServices.ReportProcessing.Table)base.ReportItemDef;

		internal Table(int intUniqueName, Microsoft.ReportingServices.ReportProcessing.Table reportItemDef, TableInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch)
			{
				return false;
			}
			if (NoRows && base.NoRowMessage != null)
			{
				return false;
			}
			if (searchContext.ItemStartPage == searchContext.ItemEndPage)
			{
				return SearchFullTable(this, searchContext);
			}
			return SearchPartialTable(this, searchContext);
		}

		private static int TableWithVisibleColumns(TableColumnCollection columns)
		{
			int num = columns.Count;
			for (int i = 0; i < columns.Count; i++)
			{
				if (columns[i].Hidden)
				{
					num--;
				}
			}
			return num;
		}

		private static bool SearchFullTable(Table table, SearchContext searchContext)
		{
			bool result = false;
			TableColumnCollection columns = table.Columns;
			if (TableWithVisibleColumns(columns) == 0)
			{
				return result;
			}
			result = SearchTableRows(table.TableHeader, columns, searchContext);
			if (!table.NoRows && !result)
			{
				TableGroupCollection tableGroups = table.TableGroups;
				if (tableGroups != null)
				{
					int num = 0;
					while (!result && num < tableGroups.Count)
					{
						result = SearchFullTableGroup(tableGroups[num], columns, searchContext);
						num++;
					}
				}
				else
				{
					TableRowsCollection detailRows = table.DetailRows;
					if (detailRows != null)
					{
						int num2 = 0;
						while (!result && num2 < detailRows.Count)
						{
							result = SearchTableRows(detailRows[num2], columns, searchContext);
							num2++;
						}
					}
				}
			}
			if (!result)
			{
				result = SearchTableRows(table.TableFooter, columns, searchContext);
			}
			return result;
		}

		private bool SearchPartialTable(Table table, SearchContext searchContext)
		{
			TableHeaderFooterRows tableHeaderFooterRows = null;
			bool flag = false;
			SearchContext searchContext2 = new SearchContext(searchContext);
			TableColumnCollection columns = table.Columns;
			tableHeaderFooterRows = table.TableHeader;
			if (tableHeaderFooterRows != null && (tableHeaderFooterRows.RepeatOnNewPage || searchContext.ItemStartPage == searchContext.SearchPage))
			{
				flag = SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
				if (flag)
				{
					return true;
				}
			}
			TableGroupCollection tableGroups = table.TableGroups;
			if (tableGroups != null)
			{
				int startGroup = 0;
				int endGroup = 0;
				int startPage = -1;
				int endPage = -1;
				GetTableGroupsOnPage(searchContext.SearchPage, out startGroup, out endGroup);
				if (startGroup >= 0)
				{
					SearchContext searchContext3 = new SearchContext(searchContext);
					IsGroupOnThisPage(startGroup, searchContext.SearchPage, out startPage, out endPage);
					TableGroup tableGroup = tableGroups[startGroup];
					if (startPage != endPage)
					{
						searchContext3.ItemStartPage = startPage;
						searchContext3.ItemEndPage = endPage;
						flag = SearchPartialTableGroup(tableGroup, columns, searchContext3);
					}
					else
					{
						flag = SearchFullTableGroup(tableGroup, columns, searchContext2);
					}
					startGroup++;
					while (!flag && startGroup < endGroup)
					{
						tableGroup = tableGroups[startGroup];
						flag = SearchFullTableGroup(tableGroup, columns, searchContext2);
						startGroup++;
					}
					if (!flag && startGroup == endGroup)
					{
						IsGroupOnThisPage(startGroup, searchContext.SearchPage, out startPage, out endPage);
						tableGroup = tableGroups[startGroup];
						if (startPage != endPage)
						{
							searchContext3.ItemStartPage = startPage;
							searchContext3.ItemEndPage = endPage;
							flag = SearchPartialTableGroup(tableGroup, columns, searchContext3);
						}
						else
						{
							flag = SearchFullTableGroup(tableGroup, columns, searchContext2);
						}
					}
				}
			}
			else
			{
				TableRowsCollection detailRows = table.DetailRows;
				if (detailRows != null)
				{
					int start = 0;
					int numberOfDetails = 0;
					int num = 0;
					GetDetailsOnThisPage(searchContext.SearchPage - searchContext.ItemStartPage, out start, out numberOfDetails);
					if (start >= 0)
					{
						num = start + numberOfDetails - 1;
						while (!flag && start <= num)
						{
							flag = SearchTableRows(detailRows[start], columns, searchContext2);
							start++;
						}
					}
				}
			}
			if (flag)
			{
				return true;
			}
			tableHeaderFooterRows = table.TableFooter;
			if (tableHeaderFooterRows != null && (tableHeaderFooterRows.RepeatOnNewPage || searchContext.ItemEndPage == searchContext.SearchPage))
			{
				flag = SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
			}
			return flag;
		}

		private static bool SearchFullTableGroup(TableGroup tableGroup, TableColumnCollection columns, SearchContext searchContext)
		{
			bool result = false;
			if (tableGroup == null || tableGroup.Hidden)
			{
				return result;
			}
			result = SearchTableRows(tableGroup.GroupHeader, columns, searchContext);
			if (!result)
			{
				TableGroupCollection subGroups = tableGroup.SubGroups;
				if (subGroups != null)
				{
					int num = 0;
					while (!result && num < subGroups.Count)
					{
						result = SearchFullTableGroup(subGroups[num], columns, searchContext);
						num++;
					}
				}
				else
				{
					TableRowsCollection detailRows = tableGroup.DetailRows;
					if (detailRows != null)
					{
						int num2 = 0;
						while (!result && num2 < detailRows.Count)
						{
							result = SearchTableRows(detailRows[num2], columns, searchContext);
							num2++;
						}
					}
				}
			}
			if (!result)
			{
				result = SearchTableRows(tableGroup.GroupFooter, columns, searchContext);
			}
			return result;
		}

		private static bool SearchPartialTableGroup(TableGroup group, TableColumnCollection columns, SearchContext searchContext)
		{
			TableHeaderFooterRows tableHeaderFooterRows = null;
			bool flag = false;
			SearchContext searchContext2 = new SearchContext(searchContext);
			tableHeaderFooterRows = group.GroupHeader;
			if (tableHeaderFooterRows != null)
			{
				if (searchContext.SearchPage == searchContext.ItemStartPage || tableHeaderFooterRows.RepeatOnNewPage)
				{
					flag = SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
				}
				if (flag)
				{
					return true;
				}
			}
			TableGroupCollection subGroups = group.SubGroups;
			if (subGroups != null)
			{
				int startGroup = 0;
				int endGroup = 0;
				int startPage = -1;
				int endPage = -1;
				group.GetSubGroupsOnPage(searchContext.SearchPage, out startGroup, out endGroup);
				if (startGroup >= 0)
				{
					SearchContext searchContext3 = new SearchContext(searchContext);
					group.IsGroupOnThisPage(startGroup, searchContext.SearchPage, out startPage, out endPage);
					TableGroup tableGroup = subGroups[startGroup];
					if (startPage != endPage)
					{
						searchContext3.ItemStartPage = startPage;
						searchContext3.ItemEndPage = endPage;
						flag = SearchPartialTableGroup(tableGroup, columns, searchContext3);
					}
					else
					{
						flag = SearchFullTableGroup(tableGroup, columns, searchContext2);
					}
					startGroup++;
					while (!flag && startGroup < endGroup)
					{
						tableGroup = subGroups[startGroup];
						flag = SearchFullTableGroup(tableGroup, columns, searchContext2);
						startGroup++;
					}
					if (!flag && startGroup == endGroup)
					{
						tableGroup = subGroups[startGroup];
						group.IsGroupOnThisPage(startGroup, searchContext.SearchPage, out startPage, out endPage);
						if (startPage != endPage)
						{
							searchContext3.ItemStartPage = startPage;
							searchContext3.ItemEndPage = endPage;
							flag = SearchPartialTableGroup(tableGroup, columns, searchContext3);
						}
						else
						{
							flag = SearchFullTableGroup(tableGroup, columns, searchContext2);
						}
					}
				}
			}
			else
			{
				TableRowsCollection detailRows = group.DetailRows;
				if (detailRows != null)
				{
					int start = 0;
					int numberOfDetails = 0;
					int num = 0;
					group.GetDetailsOnThisPage(searchContext.SearchPage - searchContext.ItemStartPage, out start, out numberOfDetails);
					if (start >= 0)
					{
						num = start + numberOfDetails - 1;
						while (!flag && start <= num)
						{
							flag = SearchTableRows(detailRows[start], columns, searchContext2);
							start++;
						}
					}
				}
			}
			if (flag)
			{
				return true;
			}
			tableHeaderFooterRows = group.GroupFooter;
			if (tableHeaderFooterRows != null && (tableHeaderFooterRows.RepeatOnNewPage || searchContext.ItemEndPage == searchContext.SearchPage))
			{
				flag = SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
			}
			return flag;
		}

		private static bool SearchTableRows(TableRowCollection tableRows, TableColumnCollection columns, SearchContext searchContext)
		{
			bool flag = false;
			if (tableRows == null)
			{
				return flag;
			}
			TableRow tableRow = null;
			int num = 0;
			while (!flag && num < tableRows.Count)
			{
				tableRow = tableRows[num];
				if (!tableRow.Hidden)
				{
					flag = SearchRowCells(tableRow.TableCellCollection, columns, searchContext);
				}
				num++;
			}
			return flag;
		}

		private static bool SearchRowCells(TableCellCollection rowCells, TableColumnCollection columns, SearchContext searchContext)
		{
			bool flag = false;
			if (rowCells == null)
			{
				return flag;
			}
			int num = 0;
			while (!flag && num < rowCells.Count)
			{
				if (!columns[num].Hidden)
				{
					flag = rowCells[num].ReportItem.Search(searchContext);
				}
				num++;
			}
			return flag;
		}

		public bool IsGroupOnThisPage(int groupIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = ((TableInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
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

		public void GetDetailsOnThisPage(int pageIndex, out int start, out int numberOfDetails)
		{
			start = 0;
			numberOfDetails = 0;
			RenderingPagesRangesList childrenStartAndEndPages = ((TableInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages != null)
			{
				Global.Tracer.Assert(pageIndex >= 0 && pageIndex < childrenStartAndEndPages.Count);
				RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[pageIndex];
				start = renderingPagesRanges.StartRow;
				numberOfDetails = renderingPagesRanges.NumberOfDetails;
			}
		}

		public void GetTableGroupsOnPage(int page, out int startGroup, out int endGroup)
		{
			startGroup = -1;
			endGroup = -1;
			if (base.ReportItemInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = ((TableInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startGroup, ref endGroup);
				}
			}
		}
	}
}
