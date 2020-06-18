using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing.Upgrade
{
	internal sealed class Upgrader
	{
		private sealed class DataSetUpgrader
		{
			internal void Upgrade(Report report)
			{
				List<DataSet> datasets = new List<DataSet>();
				int maxID = 0;
				DetermineCurrentMaxID(report, ref datasets, ref maxID);
				for (int i = 0; i < datasets.Count; i++)
				{
					maxID++;
					datasets[i].ID = maxID;
				}
				report.LastID = maxID;
			}

			private void DetermineCurrentMaxID(Report report, ref List<DataSet> datasets, ref int maxID)
			{
				if (report == null)
				{
					return;
				}
				maxID += report.LastID;
				if (report.DataSources != null)
				{
					for (int i = 0; i < report.DataSources.Count; i++)
					{
						DataSource dataSource = report.DataSources[i];
						if (dataSource.DataSets == null)
						{
							continue;
						}
						for (int j = 0; j < dataSource.DataSets.Count; j++)
						{
							DataSet dataSet = dataSource.DataSets[j];
							if (dataSet.ID <= 0)
							{
								datasets.Add(dataSet);
							}
						}
					}
				}
				if (report.SubReports == null)
				{
					return;
				}
				for (int k = 0; k < report.SubReports.Count; k++)
				{
					if (report.SubReports[k].Report != null)
					{
						DetermineCurrentMaxID(report.SubReports[k].Report, ref datasets, ref maxID);
					}
				}
			}
		}

		private sealed class BookmarkDrillthroughUpgrader
		{
			private ChunkManager.RenderingChunkManager m_chunkManager;

			private ChunkManager.UpgradeManager m_upgradeManager;

			private BookmarksHashtable m_bookmarks;

			private ReportDrillthroughInfo m_drillthroughs;

			internal void Upgrade(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ChunkManager.UpgradeManager upgradeManager)
			{
				Global.Tracer.Assert(chunkManager != null && upgradeManager != null && reportSnapshot != null, "(null != chunkManager && null != upgradeManager && null != reportSnapshot)");
				m_chunkManager = chunkManager;
				m_upgradeManager = upgradeManager;
				m_bookmarks = new BookmarksHashtable();
				m_drillthroughs = new ReportDrillthroughInfo();
				ProcessReport(reportSnapshot.Report, reportSnapshot.ReportInstance, 0, reportSnapshot.ReportInstance.NumberOfPages);
				bool flag = m_bookmarks.Count != 0;
				if (reportSnapshot.HasBookmarks)
				{
					Global.Tracer.Assert(flag, "(hasBookmarks)");
					reportSnapshot.BookmarksInfo = m_bookmarks;
				}
				else if (flag)
				{
					reportSnapshot.HasBookmarks = true;
					reportSnapshot.BookmarksInfo = m_bookmarks;
				}
				else
				{
					reportSnapshot.HasBookmarks = false;
					reportSnapshot.BookmarksInfo = null;
				}
				if (m_drillthroughs.Count == 0)
				{
					reportSnapshot.DrillthroughInfo = null;
				}
				else
				{
					reportSnapshot.DrillthroughInfo = m_drillthroughs;
				}
			}

			private void ProcessReport(Report report, ReportInstance reportInstance, int startPage, int endPage)
			{
				long chunkOffset = reportInstance.ChunkOffset;
				ReportItemInstanceInfo instanceInfo = reportInstance.GetInstanceInfo(m_chunkManager);
				m_upgradeManager.AddInstance(instanceInfo, reportInstance, chunkOffset);
				ProcessReportItemColInstance(reportInstance.ReportItemColInstance, startPage, endPage);
			}

			private void ProcessReportItemColInstance(ReportItemColInstance reportItemColInstance, int startPage, int endPage)
			{
				if (reportItemColInstance == null)
				{
					return;
				}
				long chunkOffset = reportItemColInstance.ChunkOffset;
				ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(m_chunkManager, inPageSection: false);
				m_upgradeManager.AddInstance(instanceInfo, reportItemColInstance, chunkOffset);
				reportItemColInstance.ChildrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
				Global.Tracer.Assert(reportItemColInstance.ReportItemColDef != null, "(null != reportItemColInstance.ReportItemColDef)");
				ReportItemIndexerList sortedReportItems = reportItemColInstance.ReportItemColDef.SortedReportItems;
				if (sortedReportItems == null)
				{
					return;
				}
				int count = sortedReportItems.Count;
				for (int i = 0; i < count; i++)
				{
					int startPage2 = startPage;
					int endPage2 = endPage;
					if (reportItemColInstance.ChildrenStartAndEndPages != null)
					{
						if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].StartPage)
						{
							startPage2 = reportItemColInstance.ChildrenStartAndEndPages[i].StartPage;
						}
						if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].EndPage)
						{
							endPage2 = reportItemColInstance.ChildrenStartAndEndPages[i].EndPage;
						}
					}
					if (sortedReportItems[i].IsComputed)
					{
						ProcessReportItemInstance(reportItemColInstance[sortedReportItems[i].Index], startPage2, endPage2);
						continue;
					}
					Global.Tracer.Assert(reportItemColInstance.ChildrenNonComputedUniqueNames != null && sortedReportItems[i].Index < reportItemColInstance.ChildrenNonComputedUniqueNames.Length);
					ProcessReportItem(reportItemColInstance.ReportItemColDef[i], reportItemColInstance.ChildrenNonComputedUniqueNames[sortedReportItems[i].Index], startPage2);
				}
			}

			private void ProcessReportItem(ReportItem reportItem, NonComputedUniqueNames uniqueName, int startPage)
			{
				if (reportItem.Bookmark != null && reportItem.Bookmark.Value != null)
				{
					m_bookmarks.Add(reportItem.Bookmark.Value, startPage, uniqueName.UniqueName.ToString(CultureInfo.InvariantCulture));
				}
				Rectangle rectangle = reportItem as Rectangle;
				if (rectangle != null && rectangle.ReportItems != null && rectangle.ReportItems.Count != 0)
				{
					Global.Tracer.Assert(uniqueName.ChildrenUniqueNames != null && rectangle.ReportItems.Count == uniqueName.ChildrenUniqueNames.Length);
					for (int i = 0; i < rectangle.ReportItems.Count; i++)
					{
						ProcessReportItem(rectangle.ReportItems[i], uniqueName.ChildrenUniqueNames[i], startPage);
					}
					return;
				}
				Image image = reportItem as Image;
				if (image != null && image.Action != null)
				{
					ProcessNonComputedAction(image.Action, uniqueName.UniqueName);
					return;
				}
				TextBox textBox = reportItem as TextBox;
				if (textBox != null && textBox.Action != null)
				{
					ProcessNonComputedAction(textBox.Action, uniqueName.UniqueName);
				}
			}

			private void ProcessAction(Action action, ActionInstance actionInstance, int uniqueName)
			{
				if (action != null)
				{
					if (actionInstance != null)
					{
						ProcessComputedAction(action, actionInstance, uniqueName);
					}
					else
					{
						ProcessNonComputedAction(action, uniqueName);
					}
				}
			}

			private void ProcessComputedAction(Action action, ActionInstance actionInstance, int uniqueName)
			{
				if (action == null || actionInstance == null)
				{
					return;
				}
				int count = action.ActionItems.Count;
				for (int i = 0; i < count; i++)
				{
					int computedIndex = action.ActionItems[i].ComputedIndex;
					if (computedIndex >= 0)
					{
						Global.Tracer.Assert(computedIndex < action.ComputedActionItemsCount && computedIndex < actionInstance.ActionItemsValues.Count);
						ProcessComputedActionItem(action.ActionItems[i], actionInstance.ActionItemsValues[computedIndex], uniqueName, i);
					}
					else
					{
						ProcessNonComputedActionItem(action.ActionItems[i], uniqueName, i);
					}
				}
			}

			private void ProcessNonComputedAction(Action action, int uniqueName)
			{
				if (action != null && action.ActionItems != null)
				{
					int count = action.ActionItems.Count;
					for (int i = 0; i < count; i++)
					{
						ProcessNonComputedActionItem(action.ActionItems[i], uniqueName, i);
					}
				}
			}

			private void ProcessNonComputedActionItem(ActionItem actionItem, int uniqueName, int index)
			{
				if (actionItem.DrillthroughReportName == null)
				{
					return;
				}
				Global.Tracer.Assert(actionItem.DrillthroughReportName.Type == ExpressionInfo.Types.Constant, "(actionItem.DrillthroughReportName.Type == ExpressionInfo.Types.Constant)");
				DrillthroughParameters drillthroughParameters = null;
				if (actionItem.DrillthroughParameters != null)
				{
					ParameterValue parameterValue = null;
					for (int i = 0; i < actionItem.DrillthroughParameters.Count; i++)
					{
						parameterValue = actionItem.DrillthroughParameters[i];
						if (parameterValue.Omit != null)
						{
							Global.Tracer.Assert(parameterValue.Omit.Type == ExpressionInfo.Types.Constant, "(paramValue.Omit.Type == ExpressionInfo.Types.Constant)");
							if (parameterValue.Omit.BoolValue)
							{
								continue;
							}
						}
						Global.Tracer.Assert(parameterValue.Value.Type == ExpressionInfo.Types.Constant, "(paramValue.Value.Type == ExpressionInfo.Types.Constant)");
						if (drillthroughParameters == null)
						{
							drillthroughParameters = new DrillthroughParameters();
						}
						drillthroughParameters.Add(parameterValue.Name, parameterValue.Value.Value);
					}
				}
				DrillthroughInformation drillthroughInfo = new DrillthroughInformation(actionItem.DrillthroughReportName.Value, drillthroughParameters, null);
				string drillthroughId = uniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
				m_drillthroughs.AddDrillthrough(drillthroughId, drillthroughInfo);
			}

			private void ProcessComputedActionItem(ActionItem actionItem, ActionItemInstance actionInstance, int uniqueName, int index)
			{
				Global.Tracer.Assert(actionItem != null, "(null != actionItem)");
				if (actionItem.DrillthroughReportName == null)
				{
					return;
				}
				string text = null;
				if (ExpressionInfo.Types.Constant == actionItem.DrillthroughReportName.Type)
				{
					text = actionItem.DrillthroughReportName.Value;
				}
				else
				{
					Global.Tracer.Assert(actionInstance != null, "(null != actionInstance)");
					text = actionInstance.DrillthroughReportName;
				}
				if (text == null)
				{
					return;
				}
				DrillthroughParameters drillthroughParameters = null;
				if (actionItem.DrillthroughParameters != null)
				{
					int count = actionItem.DrillthroughParameters.Count;
					Global.Tracer.Assert(count == actionInstance.DrillthroughParametersOmits.Count && count == actionInstance.DrillthroughParametersValues.Length && count == actionItem.DrillthroughParameters.Count);
					for (int i = 0; i < count; i++)
					{
						if (!actionInstance.DrillthroughParametersOmits[i])
						{
							if (drillthroughParameters == null)
							{
								drillthroughParameters = new DrillthroughParameters();
							}
							drillthroughParameters.Add(actionItem.DrillthroughParameters[i].Name, actionInstance.DrillthroughParametersValues[i]);
						}
					}
				}
				DrillthroughInformation drillthroughInfo = new DrillthroughInformation(text, drillthroughParameters, null);
				string drillthroughId = uniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
				m_drillthroughs.AddDrillthrough(drillthroughId, drillthroughInfo);
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance, int startPage, int endPage)
			{
				Global.Tracer.Assert(reportItemInstance != null, "(null != reportItemInstance)");
				ReportItem reportItemDef = reportItemInstance.ReportItemDef;
				long chunkOffset = reportItemInstance.ChunkOffset;
				ReportItemInstanceInfo instanceInfo = reportItemInstance.GetInstanceInfo(m_chunkManager);
				if (reportItemDef is TextBox)
				{
					bool isSimple;
					SimpleTextBoxInstanceInfo instanceInfo2 = ((TextBoxInstance)reportItemInstance).UpgradeToSimpleTextbox(instanceInfo as TextBoxInstanceInfo, out isSimple);
					if (isSimple)
					{
						m_upgradeManager.AddInstance(instanceInfo2, reportItemInstance, chunkOffset);
						return;
					}
				}
				m_upgradeManager.AddInstance(instanceInfo, reportItemInstance, chunkOffset);
				if (instanceInfo.Bookmark != null)
				{
					m_bookmarks.Add(instanceInfo.Bookmark, startPage, reportItemInstance.UniqueName.ToString(CultureInfo.InvariantCulture));
				}
				if (reportItemDef is Rectangle)
				{
					ProcessReportItemColInstance(((RectangleInstance)reportItemInstance).ReportItemColInstance, startPage, endPage);
				}
				else if (reportItemDef is Image)
				{
					Image image = reportItemDef as Image;
					if (image.Action != null)
					{
						ProcessAction(image.Action, ((ImageInstanceInfo)instanceInfo).Action, reportItemInstance.UniqueName);
					}
				}
				else if (reportItemDef is TextBox)
				{
					TextBox textBox = reportItemDef as TextBox;
					if (textBox.Action != null)
					{
						ProcessAction(textBox.Action, ((TextBoxInstanceInfo)instanceInfo).Action, reportItemInstance.UniqueName);
					}
				}
				else if (reportItemDef is SubReport)
				{
					SubReport subReport = (SubReport)reportItemDef;
					SubReportInstance subReportInstance = (SubReportInstance)reportItemInstance;
					if (subReportInstance.ReportInstance != null)
					{
						ProcessReport(subReport.Report, subReportInstance.ReportInstance, startPage, endPage);
					}
				}
				else if (reportItemDef is DataRegion)
				{
					if (reportItemDef is List)
					{
						ProcessList((List)reportItemDef, (ListInstance)reportItemInstance, startPage);
					}
					else if (reportItemDef is Matrix)
					{
						ProcessMatrix((Matrix)reportItemDef, (MatrixInstance)reportItemInstance, startPage, endPage);
					}
					else if (reportItemDef is Table)
					{
						ProcessTable((Table)reportItemDef, (TableInstance)reportItemInstance, startPage, endPage);
					}
					else if (reportItemDef is Chart)
					{
						ProcessChart((Chart)reportItemDef, (ChartInstance)reportItemInstance, startPage);
					}
					else if (reportItemDef is CustomReportItem)
					{
						ProcessCustomReportItem((CustomReportItem)reportItemDef, (CustomReportItemInstance)reportItemInstance, startPage, endPage);
					}
					else if (!(reportItemDef is OWCChart) && !(reportItemDef is ActiveXControl))
					{
						_ = (reportItemDef is CheckBox);
					}
				}
			}

			private void ProcessCustomReportItem(CustomReportItem criDef, CustomReportItemInstance criInstance, int startPage, int endPage)
			{
				ReportItem reportItem = null;
				Global.Tracer.Assert(criDef.AltReportItem != null, "(null != criDef.AltReportItem)");
				if (criDef.RenderReportItem != null && 1 == criDef.RenderReportItem.Count)
				{
					reportItem = criDef.RenderReportItem[0];
				}
				else if (criDef.AltReportItem != null && 1 == criDef.AltReportItem.Count)
				{
					Global.Tracer.Assert(criDef.RenderReportItem == null, "(null == criDef.RenderReportItem)");
					reportItem = criDef.AltReportItem[0];
				}
				if (reportItem != null && criInstance.AltReportItemColInstance != null)
				{
					Global.Tracer.Assert(criInstance.AltReportItemColInstance.ReportItemInstances != null && 1 == criInstance.AltReportItemColInstance.ReportItemInstances.Count);
					ProcessReportItemInstance(criInstance.AltReportItemColInstance.ReportItemInstances[0], startPage, endPage);
				}
			}

			private void ProcessChart(Chart chartDef, ChartInstance chartInstance, int startPage)
			{
				ProcessChartHeadings(chartInstance.RowInstances);
				ProcessChartHeadings(chartInstance.ColumnInstances);
				ProcessChartDataPoints(chartDef, chartInstance, startPage);
			}

			private void ProcessChartHeadings(ChartHeadingInstanceList headings)
			{
				if (headings == null)
				{
					return;
				}
				for (int i = 0; i < headings.Count; i++)
				{
					ChartHeadingInstance chartHeadingInstance = headings[i];
					Global.Tracer.Assert(chartHeadingInstance != null, "(null != headingInstance)");
					long chunkOffset = chartHeadingInstance.ChunkOffset;
					ChartHeadingInstanceInfo instanceInfo = chartHeadingInstance.GetInstanceInfo(m_chunkManager);
					m_upgradeManager.AddInstance(instanceInfo, chartHeadingInstance, chunkOffset);
					if (chartHeadingInstance.SubHeadingInstances != null)
					{
						ProcessChartHeadings(chartHeadingInstance.SubHeadingInstances);
					}
				}
			}

			private void ProcessChartDataPoints(Chart chartDef, ChartInstance chartInstance, int startPage)
			{
				if (chartInstance.DataPoints == null)
				{
					return;
				}
				for (int i = 0; i < chartInstance.DataPointSeriesCount; i++)
				{
					for (int j = 0; j < chartInstance.DataPointCategoryCount; j++)
					{
						ChartDataPointInstance chartDataPointInstance = chartInstance.DataPoints[i][j];
						Global.Tracer.Assert(chartDataPointInstance != null, "(null != instance)");
						long chunkOffset = chartDataPointInstance.ChunkOffset;
						ChartDataPointInstanceInfo instanceInfo = chartDataPointInstance.GetInstanceInfo(m_chunkManager, chartDef.ChartDataPoints);
						m_upgradeManager.AddInstance(instanceInfo, chartDataPointInstance, chunkOffset);
						if (instanceInfo.Action != null)
						{
							Global.Tracer.Assert(instanceInfo.DataPointIndex < chartDef.ChartDataPoints.Count, "(instanceInfo.DataPointIndex < chartDef.ChartDataPoints.Count)");
							Action action = chartDef.ChartDataPoints[instanceInfo.DataPointIndex].Action;
							Global.Tracer.Assert(action != null, "(null != actionDef)");
							ProcessAction(action, instanceInfo.Action, chartDataPointInstance.UniqueName);
						}
					}
				}
			}

			private void ProcessList(List listDef, ListInstance listInstance, int startPage)
			{
				if (listInstance.ListContents == null)
				{
					return;
				}
				if (listDef.Grouping != null)
				{
					Global.Tracer.Assert(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count, "(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count)");
					for (int i = 0; i < listInstance.ChildrenStartAndEndPages.Count; i++)
					{
						ListContentInstance listContentInstance = listInstance.ListContents[i];
						long chunkOffset = listContentInstance.ChunkOffset;
						ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(m_chunkManager);
						m_upgradeManager.AddInstance(instanceInfo, listContentInstance, chunkOffset);
						RenderingPagesRanges renderingPagesRanges = listInstance.ChildrenStartAndEndPages[i];
						ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
					}
				}
				else if (listInstance.ChildrenStartAndEndPages == null)
				{
					ProcessListContents(listDef, listInstance, startPage, 0, listInstance.ListContents.Count - 1);
				}
				else
				{
					for (int j = 0; j < listInstance.ChildrenStartAndEndPages.Count; j++)
					{
						RenderingPagesRanges renderingPagesRanges2 = listInstance.ChildrenStartAndEndPages[j];
						ProcessListContents(listDef, listInstance, startPage + j, renderingPagesRanges2.StartRow, renderingPagesRanges2.StartRow + renderingPagesRanges2.NumberOfDetails - 1);
					}
				}
			}

			private void ProcessListContents(List listDef, ListInstance listInstance, int page, int startRow, int endRow)
			{
				for (int i = startRow; i <= endRow; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					long chunkOffset = listContentInstance.ChunkOffset;
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(m_chunkManager);
					m_upgradeManager.AddInstance(instanceInfo, listContentInstance, chunkOffset);
					ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, page, page);
				}
			}

			private void ProcessTable(Table tableDef, TableInstance tableInstance, int startPage, int endPage)
			{
				Global.Tracer.Assert(tableInstance != null, "(null != tableInstance)");
				ProcessTableRowInstances(tableInstance.HeaderRowInstances, startPage);
				ProcessTableRowInstances(tableInstance.FooterRowInstances, endPage);
				if (tableInstance.TableGroupInstances != null)
				{
					ProcessTableGroupInstances(tableInstance.TableGroupInstances, startPage, tableInstance.ChildrenStartAndEndPages);
				}
				else
				{
					ProcessTableDetailInstances(tableInstance.TableDetailInstances, startPage, tableInstance.ChildrenStartAndEndPages);
				}
			}

			private void ProcessTableRowInstances(TableRowInstance[] rowInstances, int startPage)
			{
				if (rowInstances != null)
				{
					foreach (TableRowInstance tableRowInstance in rowInstances)
					{
						Global.Tracer.Assert(tableRowInstance != null, "(null != rowInstance)");
						long chunkOffset = tableRowInstance.ChunkOffset;
						TableRowInstanceInfo instanceInfo = tableRowInstance.GetInstanceInfo(m_chunkManager);
						m_upgradeManager.AddInstance(instanceInfo, tableRowInstance, chunkOffset);
						ProcessReportItemColInstance(tableRowInstance.TableRowReportItemColInstance, startPage, startPage);
					}
				}
			}

			private void ProcessTableDetailInstances(TableDetailInstanceList detailInstances, int startPage, RenderingPagesRangesList instancePages)
			{
				if (detailInstances == null)
				{
					return;
				}
				int count = instancePages.Count;
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					RenderingPagesRanges renderingPagesRanges = instancePages[i];
					for (int j = 0; j < renderingPagesRanges.NumberOfDetails; j++)
					{
						TableDetailInstance tableDetailInstance = detailInstances[num + j];
						Global.Tracer.Assert(tableDetailInstance != null && tableDetailInstance.TableDetailDef != null);
						long chunkOffset = tableDetailInstance.ChunkOffset;
						TableDetailInstanceInfo instanceInfo = tableDetailInstance.GetInstanceInfo(m_chunkManager);
						m_upgradeManager.AddInstance(instanceInfo, tableDetailInstance, chunkOffset);
						ProcessTableRowInstances(tableDetailInstance.DetailRowInstances, startPage + i);
					}
					num += renderingPagesRanges.NumberOfDetails;
				}
			}

			private void ProcessTableGroupInstances(TableGroupInstanceList groupInstances, int startPage, RenderingPagesRangesList groupInstancePages)
			{
				if (groupInstances == null)
				{
					return;
				}
				for (int i = 0; i < groupInstances.Count; i++)
				{
					TableGroupInstance tableGroupInstance = groupInstances[i];
					Global.Tracer.Assert(tableGroupInstance != null, "(null != groupInstance)");
					long chunkOffset = tableGroupInstance.ChunkOffset;
					TableGroupInstanceInfo instanceInfo = tableGroupInstance.GetInstanceInfo(m_chunkManager);
					m_upgradeManager.AddInstance(instanceInfo, tableGroupInstance, chunkOffset);
					RenderingPagesRanges renderingPagesRanges = groupInstancePages[i];
					ProcessTableRowInstances(tableGroupInstance.HeaderRowInstances, renderingPagesRanges.StartPage);
					ProcessTableRowInstances(tableGroupInstance.FooterRowInstances, renderingPagesRanges.EndPage);
					if (tableGroupInstance.SubGroupInstances != null)
					{
						ProcessTableGroupInstances(tableGroupInstance.SubGroupInstances, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
					}
					else
					{
						ProcessTableDetailInstances(tableGroupInstance.TableDetailInstances, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
					}
				}
			}

			private void ProcessMatrix(Matrix matrixDef, MatrixInstance matrixInstance, int startPage, int endPage)
			{
				if (matrixInstance.CornerContent != null)
				{
					ProcessReportItemInstance(matrixInstance.CornerContent, startPage, startPage);
				}
				int[] rowPages = new int[matrixInstance.CellRowCount];
				ProcessMatrixHeadings(matrixInstance.RowInstances, matrixInstance.ChildrenStartAndEndPages, ref rowPages, startPage, endPage);
				ProcessMatrixHeadings(matrixInstance.ColumnInstances, null, ref rowPages, startPage, endPage);
				ProcessMatrixCells(matrixInstance, rowPages);
			}

			private void ProcessMatrixCells(MatrixInstance matrixInstance, int[] rowPages)
			{
				if (matrixInstance.Cells == null)
				{
					return;
				}
				for (int i = 0; i < matrixInstance.CellRowCount; i++)
				{
					for (int j = 0; j < matrixInstance.CellColumnCount; j++)
					{
						MatrixCellInstance matrixCellInstance = matrixInstance.Cells[i][j];
						Global.Tracer.Assert(matrixCellInstance != null, "(null != cellInstance)");
						long chunkOffset = matrixCellInstance.ChunkOffset;
						MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(m_chunkManager);
						m_upgradeManager.AddInstance(instanceInfo, matrixCellInstance, chunkOffset);
						if (matrixCellInstance.Content != null)
						{
							ProcessReportItemInstance(matrixCellInstance.Content, rowPages[i], rowPages[i]);
							continue;
						}
						ReportItem cellReportItem = matrixInstance.MatrixDef.GetCellReportItem(instanceInfo.RowIndex, instanceInfo.ColumnIndex);
						if (cellReportItem != null)
						{
							ProcessReportItem(cellReportItem, instanceInfo.ContentUniqueNames, rowPages[i]);
						}
					}
				}
			}

			private void ProcessMatrixHeadings(MatrixHeadingInstanceList headings, RenderingPagesRangesList pagesList, ref int[] rowPages, int startPage, int endPage)
			{
				if (headings == null)
				{
					return;
				}
				for (int i = 0; i < headings.Count; i++)
				{
					MatrixHeadingInstance matrixHeadingInstance = headings[i];
					Global.Tracer.Assert(matrixHeadingInstance != null, "(null != headingInstance)");
					long chunkOffset = matrixHeadingInstance.ChunkOffset;
					MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
					MatrixHeadingInstanceInfo instanceInfo = matrixHeadingInstance.GetInstanceInfo(m_chunkManager);
					m_upgradeManager.AddInstance(instanceInfo, matrixHeadingInstance, chunkOffset);
					if (pagesList != null && pagesList.Count != 0)
					{
						Global.Tracer.Assert(headings.Count == pagesList.Count, "(headings.Count == pagesList.Count)");
						startPage = pagesList[i].StartPage;
						endPage = pagesList[i].EndPage;
					}
					if (!matrixHeadingDef.IsColumn)
					{
						SetRowPages(ref rowPages, instanceInfo.HeadingCellIndex, instanceInfo.HeadingSpan, startPage);
					}
					if (matrixHeadingInstance.Content != null)
					{
						ProcessReportItemInstance(matrixHeadingInstance.Content, startPage, endPage);
					}
					else
					{
						ReportItem reportItem = (matrixHeadingInstance.IsSubtotal && matrixHeadingDef.Subtotal != null) ? matrixHeadingDef.Subtotal.ReportItem : matrixHeadingDef.ReportItem;
						if (reportItem != null)
						{
							ProcessReportItem(reportItem, instanceInfo.ContentUniqueNames, startPage);
						}
					}
					if (matrixHeadingInstance.SubHeadingInstances != null)
					{
						ProcessMatrixHeadings(matrixHeadingInstance.SubHeadingInstances, matrixHeadingInstance.ChildrenStartAndEndPages, ref rowPages, startPage, endPage);
					}
				}
			}
		}

		private sealed class PageSectionsGenerator
		{
			private ChunkManager.RenderingChunkManager m_chunkManager;

			private ReportProcessing.CreateReportChunk m_createChunkCallback;

			private ReportProcessing.PageSectionContext m_pageSectionContext;

			private ReportInstanceInfo m_reportInstanceInfo;

			internal void Upgrade(ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection)
			{
				m_chunkManager = chunkManager;
				m_createChunkCallback = createChunkCallback;
				Report report = reportSnapshot.Report;
				m_pageSectionContext = new ReportProcessing.PageSectionContext(hasPageSections: true, report?.MergeOnePass ?? false);
				ReportInstance reportInstance = reportSnapshot.ReportInstance;
				ProcessReport(report, reportInstance);
				ProcessPageHeaderFooter(report, reportInstance, reportSnapshot, reportContext, createChunkCallback, getResourceCallback, renderingContext, dataProtection);
			}

			private void ProcessPageHeaderFooter(Report report, ReportInstance reportInstance, ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, ReportProcessing.CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection)
			{
				ChunkManager.ProcessingChunkManager processingChunkManager = new ChunkManager.ProcessingChunkManager(createChunkCallback, isOnePass: false);
				ReportProcessing.PageMergeInteractive pageMergeInteractive = new ReportProcessing.PageMergeInteractive();
				ProcessingErrorContext errorContext = new ProcessingErrorContext();
				UserProfileState allowUserProfileState = renderingContext.AllowUserProfileState;
				ReportRuntimeSetup reportRuntimeSetup = renderingContext.ReportRuntimeSetup;
				ReportDrillthroughInfo drillthroughInfo = null;
				pageMergeInteractive.Process(m_pageSectionContext.PageTextboxes, reportSnapshot, reportContext, m_reportInstanceInfo.ReportName, reportSnapshot.Parameters, processingChunkManager, createChunkCallback, getResourceCallback, errorContext, allowUserProfileState, reportRuntimeSetup, 0, dataProtection, ref drillthroughInfo);
				processingChunkManager.PageSectionFlush(reportSnapshot);
				processingChunkManager.Close();
			}

			private void ProcessReport(Report report, ReportInstance reportInstance)
			{
				m_reportInstanceInfo = (reportInstance.GetInstanceInfo(m_chunkManager) as ReportInstanceInfo);
				if (Visibility.IsVisible(report, reportInstance, m_reportInstanceInfo))
				{
					ProcessReportItemColInstance(reportInstance.ReportItemColInstance, report.StartPage, reportInstance.NumberOfPages - 1);
				}
			}

			private void ProcessReportItemColInstance(ReportItemColInstance reportItemColInstance, int startPage, int endPage)
			{
				ProcessReportItemColInstance(reportItemColInstance, startPage, endPage, null, null);
			}

			private void ProcessReportItemColInstance(ReportItemColInstance reportItemColInstance, int startPage, int endPage, bool[] tableColumnsVisible, IntList colSpans)
			{
				if (reportItemColInstance == null)
				{
					return;
				}
				ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(m_chunkManager, inPageSection: false);
				reportItemColInstance.ChildrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
				Global.Tracer.Assert(reportItemColInstance.ReportItemColDef != null, "(null != reportItemColInstance.ReportItemColDef)");
				ReportItemIndexerList sortedReportItems = reportItemColInstance.ReportItemColDef.SortedReportItems;
				if (sortedReportItems == null)
				{
					return;
				}
				int count = sortedReportItems.Count;
				List<DataRegion> list = new List<DataRegion>();
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					if (colSpans != null)
					{
						Global.Tracer.Assert(tableColumnsVisible != null && colSpans.Count <= tableColumnsVisible.Length);
						bool num2 = Visibility.IsTableCellVisible(tableColumnsVisible, num, colSpans[i]);
						num += colSpans[i];
						if (!num2)
						{
							continue;
						}
					}
					int num3 = startPage;
					int num4 = endPage;
					if (reportItemColInstance.ChildrenStartAndEndPages != null)
					{
						if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].StartPage)
						{
							num3 = reportItemColInstance.ChildrenStartAndEndPages[i].StartPage;
						}
						if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].EndPage)
						{
							num4 = reportItemColInstance.ChildrenStartAndEndPages[i].EndPage;
						}
					}
					ReportItem reportItem;
					if (sortedReportItems[i].IsComputed)
					{
						ProcessReportItemInstance(reportItemColInstance[sortedReportItems[i].Index], num3, num4, list);
						reportItem = reportItemColInstance[sortedReportItems[i].Index].ReportItemDef;
					}
					else
					{
						Global.Tracer.Assert(reportItemColInstance.ChildrenNonComputedUniqueNames != null && sortedReportItems[i].Index < reportItemColInstance.ChildrenNonComputedUniqueNames.Length);
						ProcessReportItem(reportItemColInstance.ReportItemColDef[i], reportItemColInstance.ChildrenNonComputedUniqueNames[sortedReportItems[i].Index], num3);
						reportItem = reportItemColInstance.ReportItemColDef[i];
					}
					Global.Tracer.Assert(reportItem != null, "(null != currentReportItem)");
					if (reportItem.RepeatedSiblingTextboxes != null)
					{
						m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem.RepeatedSiblingTextboxes, num3, num4);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					DataRegion dataRegion = list[j];
					Global.Tracer.Assert(dataRegion.RepeatSiblings != null, "(null != dataRegion.RepeatSiblings)");
					for (int k = 0; k < dataRegion.RepeatSiblings.Count; k++)
					{
						ReportItem reportItem2 = reportItemColInstance.ReportItemColDef[dataRegion.RepeatSiblings[k]];
						Global.Tracer.Assert(reportItem2 != null, "(null != sibling)");
						m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem2.RepeatedSiblingTextboxes, dataRegion.StartPage, dataRegion.EndPage);
					}
				}
			}

			private void ProcessReportItem(ReportItem reportItem, NonComputedUniqueNames uniqueName, int startPage)
			{
				if (!Visibility.IsVisible(reportItem))
				{
					return;
				}
				if (reportItem.RepeatedSibling)
				{
					m_pageSectionContext.EnterRepeatingItem();
				}
				Rectangle rectangle = reportItem as Rectangle;
				if (rectangle != null && rectangle.ReportItems != null && rectangle.ReportItems.Count != 0)
				{
					Global.Tracer.Assert(uniqueName.ChildrenUniqueNames != null && rectangle.ReportItems.Count == uniqueName.ChildrenUniqueNames.Length);
					for (int i = 0; i < rectangle.ReportItems.Count; i++)
					{
						ProcessReportItem(rectangle.ReportItems[i], uniqueName.ChildrenUniqueNames[i], startPage);
					}
				}
				else if (reportItem is TextBox)
				{
					ProcessTextbox(reportItem as TextBox, null, null, null, uniqueName.UniqueName, startPage);
				}
				if (reportItem.RepeatedSibling)
				{
					reportItem.RepeatedSiblingTextboxes = m_pageSectionContext.ExitRepeatingItem();
				}
			}

			private void ProcessTextbox(TextBox textbox, TextBoxInstance textboxInstance, TextBoxInstanceInfo textboxInstanceInfo, SimpleTextBoxInstanceInfo simpleTextboxInstanceInfo, int uniqueName, int startPage)
			{
				if (textbox == null || !m_pageSectionContext.IsParentVisible() || !Visibility.IsVisible(textbox, textboxInstance, textboxInstanceInfo))
				{
					return;
				}
				if (!m_pageSectionContext.InMatrixCell)
				{
					_ = m_pageSectionContext.InRepeatingItem;
				}
				if (textboxInstance != null)
				{
					if (textboxInstanceInfo != null)
					{
						m_pageSectionContext.PageTextboxes.AddTextboxValue(startPage, textbox.Name, textboxInstanceInfo.OriginalValue);
					}
					else if (simpleTextboxInstanceInfo != null)
					{
						m_pageSectionContext.PageTextboxes.AddTextboxValue(startPage, textbox.Name, simpleTextboxInstanceInfo.OriginalValue);
					}
				}
				else
				{
					Global.Tracer.Assert(textbox.Value != null && ExpressionInfo.Types.Constant == textbox.Value.Type);
					m_pageSectionContext.PageTextboxes.AddTextboxValue(startPage, textbox.Name, textbox.Value.Value);
				}
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance, int startPage, int endPage)
			{
				List<DataRegion> list = new List<DataRegion>();
				ProcessReportItemInstance(reportItemInstance, startPage, endPage, list);
				Global.Tracer.Assert(list.Count == 0, "(0 == dataRegionsWithRepeatSiblings.Count)");
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance, int startPage, int endPage, List<DataRegion> dataRegionsWithRepeatSiblings)
			{
				Global.Tracer.Assert(reportItemInstance != null, "(null != reportItemInstance)");
				ReportItem reportItemDef = reportItemInstance.ReportItemDef;
				TextBox textBox = reportItemDef as TextBox;
				TextBoxInstance textBoxInstance = reportItemInstance as TextBoxInstance;
				ReportItemInstanceInfo reportItemInstanceInfo = null;
				SimpleTextBoxInstanceInfo simpleTextboxInstanceInfo = null;
				if (textBox != null && textBox.IsSimpleTextBox() && textBoxInstance != null)
				{
					simpleTextboxInstanceInfo = textBoxInstance.GetSimpleInstanceInfo(m_chunkManager, inPageSection: false);
				}
				else
				{
					reportItemInstanceInfo = reportItemInstance.GetInstanceInfo(m_chunkManager);
				}
				if (!Visibility.IsVisible(reportItemDef, reportItemInstance, reportItemInstanceInfo))
				{
					return;
				}
				if (reportItemDef.RepeatedSibling)
				{
					m_pageSectionContext.EnterRepeatingItem();
				}
				if (textBox != null)
				{
					ProcessTextbox(textBox, textBoxInstance, reportItemInstanceInfo as TextBoxInstanceInfo, simpleTextboxInstanceInfo, textBoxInstance.UniqueName, startPage);
				}
				else if (reportItemDef is Rectangle)
				{
					ProcessReportItemColInstance(((RectangleInstance)reportItemInstance).ReportItemColInstance, startPage, endPage);
				}
				else if (!(reportItemDef is SubReport) && reportItemDef is DataRegion)
				{
					DataRegion dataRegion = reportItemDef as DataRegion;
					if (dataRegion.RepeatSiblings != null)
					{
						dataRegion.StartPage = startPage;
						dataRegion.EndPage = endPage;
						dataRegionsWithRepeatSiblings.Add(reportItemDef as DataRegion);
					}
					if (reportItemDef is List)
					{
						ProcessList((List)reportItemDef, (ListInstance)reportItemInstance, startPage);
					}
					else if (reportItemDef is Matrix)
					{
						ProcessMatrix((Matrix)reportItemDef, (MatrixInstance)reportItemInstance, (MatrixInstanceInfo)reportItemInstanceInfo, startPage, endPage);
					}
					else if (reportItemDef is Table)
					{
						ProcessTable((Table)reportItemDef, (TableInstance)reportItemInstance, (TableInstanceInfo)reportItemInstanceInfo, startPage, endPage);
					}
				}
				if (reportItemDef.RepeatedSibling)
				{
					reportItemDef.RepeatedSiblingTextboxes = m_pageSectionContext.ExitRepeatingItem();
				}
			}

			private void ProcessList(List listDef, ListInstance listInstance, int startPage)
			{
				if (listInstance.ListContents == null)
				{
					return;
				}
				if (listDef.Grouping != null)
				{
					Global.Tracer.Assert(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count, "(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count)");
					for (int i = 0; i < listInstance.ChildrenStartAndEndPages.Count; i++)
					{
						ListContentInstance listContentInstance = listInstance.ListContents[i];
						ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(m_chunkManager);
						RenderingPagesRanges renderingPagesRanges = listInstance.ChildrenStartAndEndPages[i];
						if (Visibility.IsVisible(listDef.Visibility, instanceInfo.StartHidden))
						{
							ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
						}
					}
				}
				else if (listInstance.ChildrenStartAndEndPages == null)
				{
					ProcessListContents(listDef, listInstance, startPage, 0, listInstance.ListContents.Count - 1);
				}
				else
				{
					for (int j = 0; j < listInstance.ChildrenStartAndEndPages.Count; j++)
					{
						RenderingPagesRanges renderingPagesRanges2 = listInstance.ChildrenStartAndEndPages[j];
						ProcessListContents(listDef, listInstance, startPage + j, renderingPagesRanges2.StartRow, renderingPagesRanges2.StartRow + renderingPagesRanges2.NumberOfDetails - 1);
					}
				}
			}

			private void ProcessListContents(List listDef, ListInstance listInstance, int page, int startRow, int endRow)
			{
				for (int i = startRow; i <= endRow; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(m_chunkManager);
					if (Visibility.IsVisible(listDef.Visibility, instanceInfo.StartHidden))
					{
						ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, page, page);
					}
				}
			}

			private void ProcessTable(Table tableDef, TableInstance tableInstance, TableInstanceInfo tableInstanceInfo, int startPage, int endPage)
			{
				Global.Tracer.Assert(tableInstance != null && tableDef.TableColumns != null, "(null != tableInstance && null != tableDef.TableColumns)");
				bool[] array = new bool[tableDef.TableColumns.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Visibility.IsVisible(tableDef.TableColumns[i].Visibility, tableInstanceInfo.ColumnInstances[i].StartHidden);
				}
				ProcessTableRowInstances(tableInstance.HeaderRowInstances, array, startPage, tableDef.HeaderRepeatOnNewPage, endPage);
				if (tableInstance.TableGroupInstances != null)
				{
					ProcessTableGroupInstances(tableInstance.TableGroupInstances, array, startPage, tableInstance.ChildrenStartAndEndPages);
				}
				else
				{
					ProcessTableDetailInstances(tableInstance.TableDetailInstances, array, startPage, tableInstance.ChildrenStartAndEndPages);
				}
				ProcessTableRowInstances(tableInstance.FooterRowInstances, array, endPage, tableDef.FooterRepeatOnNewPage, startPage);
			}

			private void ProcessTableRowInstances(TableRowInstance[] rowInstances, bool[] tableColumnsVisible, int startPage, bool repeat, int endPage)
			{
				if (!repeat)
				{
					ProcessTableRowInstances(rowInstances, tableColumnsVisible, startPage);
					return;
				}
				int num = Math.Min(startPage, endPage);
				int num2 = Math.Max(startPage, endPage);
				Global.Tracer.Assert(0 <= num && 0 <= num2, "(0 <= minPage && 0 <= maxPage)");
				for (int i = num; i <= num2; i++)
				{
					ProcessTableRowInstances(rowInstances, tableColumnsVisible, i);
				}
			}

			private void ProcessTableRowInstances(TableRowInstance[] rowInstances, bool[] tableColumnsVisible, int startPage)
			{
				if (rowInstances == null)
				{
					return;
				}
				foreach (TableRowInstance tableRowInstance in rowInstances)
				{
					Global.Tracer.Assert(tableRowInstance != null && tableRowInstance.TableRowDef != null);
					TableRowInstanceInfo instanceInfo = tableRowInstance.GetInstanceInfo(m_chunkManager);
					if (Visibility.IsVisible(tableRowInstance.TableRowDef.Visibility, instanceInfo.StartHidden))
					{
						ProcessReportItemColInstance(tableRowInstance.TableRowReportItemColInstance, startPage, startPage, tableColumnsVisible, tableRowInstance.TableRowDef.ColSpans);
					}
				}
			}

			private void ProcessTableDetailInstances(TableDetailInstanceList detailInstances, bool[] tableColumnsVisible, int startPage, RenderingPagesRangesList instancePages)
			{
				if (detailInstances == null)
				{
					return;
				}
				Global.Tracer.Assert(instancePages != null, "(null != instancePages)");
				int count = instancePages.Count;
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					RenderingPagesRanges renderingPagesRanges = instancePages[i];
					for (int j = 0; j < renderingPagesRanges.NumberOfDetails; j++)
					{
						TableDetailInstance tableDetailInstance = detailInstances[num + j];
						Global.Tracer.Assert(tableDetailInstance != null && tableDetailInstance.TableDetailDef != null);
						TableDetailInstanceInfo instanceInfo = tableDetailInstance.GetInstanceInfo(m_chunkManager);
						if (Visibility.IsVisible(tableDetailInstance.TableDetailDef.Visibility, instanceInfo.StartHidden))
						{
							ProcessTableRowInstances(tableDetailInstance.DetailRowInstances, tableColumnsVisible, startPage + i);
						}
					}
					num += renderingPagesRanges.NumberOfDetails;
				}
			}

			private void ProcessTableGroupInstances(TableGroupInstanceList groupInstances, bool[] tableColumnsVisible, int startPage, RenderingPagesRangesList groupInstancePages)
			{
				if (groupInstances == null)
				{
					return;
				}
				int count = groupInstances.Count;
				Global.Tracer.Assert(groupInstancePages != null && count == groupInstancePages.Count, "(null != groupInstancePages && instanceCount == groupInstancePages.Count)");
				for (int i = 0; i < count; i++)
				{
					TableGroupInstance tableGroupInstance = groupInstances[i];
					Global.Tracer.Assert(tableGroupInstance != null && tableGroupInstance.TableGroupDef != null);
					TableGroupInstanceInfo instanceInfo = tableGroupInstance.GetInstanceInfo(m_chunkManager);
					RenderingPagesRanges renderingPagesRanges = groupInstancePages[i];
					if (Visibility.IsVisible(tableGroupInstance.TableGroupDef.Visibility, instanceInfo.StartHidden))
					{
						ProcessTableRowInstances(tableGroupInstance.HeaderRowInstances, tableColumnsVisible, renderingPagesRanges.StartPage, tableGroupInstance.TableGroupDef.HeaderRepeatOnNewPage, renderingPagesRanges.EndPage);
						if (tableGroupInstance.SubGroupInstances != null)
						{
							ProcessTableGroupInstances(tableGroupInstance.SubGroupInstances, tableColumnsVisible, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
						}
						else
						{
							ProcessTableDetailInstances(tableGroupInstance.TableDetailInstances, tableColumnsVisible, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
						}
						ProcessTableRowInstances(tableGroupInstance.FooterRowInstances, tableColumnsVisible, renderingPagesRanges.EndPage, tableGroupInstance.TableGroupDef.FooterRepeatOnNewPage, renderingPagesRanges.StartPage);
					}
				}
			}

			private void ProcessMatrix(Matrix matrixDef, MatrixInstance matrixInstance, MatrixInstanceInfo matrixInstanceInfo, int startPage, int endPage)
			{
				ReportProcessing.PageSectionContext pageSectionContext = m_pageSectionContext;
				m_pageSectionContext = new ReportProcessing.PageSectionContext(pageSectionContext);
				ReportProcessing.PageTextboxes source = null;
				if (matrixInstance.CornerContent != null)
				{
					m_pageSectionContext.EnterRepeatingItem();
					ProcessReportItemInstance(matrixInstance.CornerContent, startPage, endPage);
					source = m_pageSectionContext.ExitRepeatingItem();
				}
				matrixDef.InitializePageSectionProcessing();
				bool[] cellsCanGetReferenced = new bool[matrixInstance.CellRowCount];
				int[] rowPages = new int[matrixInstance.CellRowCount];
				bool[] cellsCanGetReferenced2 = new bool[matrixInstance.CellColumnCount];
				ProcessMatrixHeadings(matrixInstance, matrixInstance.ColumnInstances, null, ref cellsCanGetReferenced2, ref rowPages, startPage, endPage);
				ProcessMatrixHeadings(matrixInstance, matrixInstance.RowInstances, matrixInstance.ChildrenStartAndEndPages, ref cellsCanGetReferenced, ref rowPages, startPage, endPage);
				ProcessMatrixCells(matrixInstance, cellsCanGetReferenced, rowPages, cellsCanGetReferenced2);
				m_pageSectionContext = pageSectionContext;
				m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, startPage, endPage);
				m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(matrixInstance.MatrixDef.ColumnHeaderPageTextboxes, startPage, endPage);
				m_pageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.RowHeaderPageTextboxes);
				m_pageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.CellPageTextboxes);
			}

			private void ProcessMatrixCells(MatrixInstance matrixInstance, bool[] rowCanGetReferenced, int[] rowPages, bool[] colCanGetReferenced)
			{
				if (matrixInstance.Cells == null)
				{
					return;
				}
				m_pageSectionContext.EnterRepeatingItem();
				m_pageSectionContext.InMatrixCell = true;
				for (int i = 0; i < matrixInstance.CellRowCount; i++)
				{
					if (!rowCanGetReferenced[i])
					{
						continue;
					}
					for (int j = 0; j < matrixInstance.CellColumnCount; j++)
					{
						if (!colCanGetReferenced[j])
						{
							continue;
						}
						MatrixCellInstance matrixCellInstance = matrixInstance.Cells[i][j];
						Global.Tracer.Assert(matrixCellInstance != null, "(null != cellInstance)");
						MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(m_chunkManager);
						if (matrixCellInstance.Content != null)
						{
							ProcessReportItemInstance(matrixCellInstance.Content, rowPages[i], rowPages[i]);
							continue;
						}
						ReportItem cellReportItem = matrixInstance.MatrixDef.GetCellReportItem(instanceInfo.RowIndex, instanceInfo.ColumnIndex);
						if (cellReportItem != null)
						{
							ProcessReportItem(cellReportItem, instanceInfo.ContentUniqueNames, rowPages[i]);
						}
					}
				}
				m_pageSectionContext.InMatrixCell = false;
				matrixInstance.MatrixDef.CellPageTextboxes.IntegrateNonRepeatingTextboxValues(m_pageSectionContext.ExitRepeatingItem());
			}

			private static void SetCellVisiblity(ref bool[] cellVisiblity, int start, int span, bool visible)
			{
				Global.Tracer.Assert(cellVisiblity != null && start >= 0 && span > 0 && start + span <= cellVisiblity.Length, "(null != cellVisiblity && start >= 0 && span > 0 && start + span <= cellVisiblity.Length)");
				for (int i = 0; i < span; i++)
				{
					cellVisiblity[start + i] = visible;
				}
			}

			private void ProcessMatrixHeadings(MatrixInstance matrixInstance, MatrixHeadingInstanceList headings, RenderingPagesRangesList pagesList, ref bool[] cellsCanGetReferenced, ref int[] rowPages, int startPage, int endPage)
			{
				if (headings == null)
				{
					return;
				}
				for (int i = 0; i < headings.Count; i++)
				{
					MatrixHeadingInstance matrixHeadingInstance = headings[i];
					MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
					MatrixHeadingInstanceInfo instanceInfo = matrixHeadingInstance.GetInstanceInfo(m_chunkManager);
					m_pageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixHeadingDef.Visibility, instanceInfo.StartHidden), matrixHeadingDef.IsColumn);
					if (pagesList != null && pagesList.Count != 0)
					{
						Global.Tracer.Assert(headings.Count == pagesList.Count, "(headings.Count == pagesList.Count)");
						startPage = pagesList[i].StartPage;
						endPage = pagesList[i].EndPage;
					}
					if (!matrixHeadingDef.IsColumn)
					{
						SetRowPages(ref rowPages, instanceInfo.HeadingCellIndex, instanceInfo.HeadingSpan, startPage);
					}
					m_pageSectionContext.EnterRepeatingItem();
					if (matrixHeadingInstance.Content != null)
					{
						ProcessReportItemInstance(matrixHeadingInstance.Content, startPage, endPage);
					}
					else
					{
						ReportItem reportItem = (matrixHeadingInstance.IsSubtotal && matrixHeadingDef.Subtotal != null) ? matrixHeadingDef.Subtotal.ReportItem : matrixHeadingDef.ReportItem;
						if (reportItem != null)
						{
							ProcessReportItem(reportItem, instanceInfo.ContentUniqueNames, startPage);
						}
					}
					if (matrixHeadingDef.IsColumn)
					{
						matrixInstance.MatrixDef.ColumnHeaderPageTextboxes.IntegrateNonRepeatingTextboxValues(m_pageSectionContext.ExitRepeatingItem());
					}
					else
					{
						matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(m_pageSectionContext.ExitRepeatingItem(), startPage, endPage);
					}
					if (matrixHeadingInstance.IsSubtotal)
					{
						m_pageSectionContext.EnterMatrixSubtotalScope(matrixHeadingDef.IsColumn);
					}
					SetCellVisiblity(ref cellsCanGetReferenced, instanceInfo.HeadingCellIndex, instanceInfo.HeadingSpan, m_pageSectionContext.IsParentVisible());
					if (matrixHeadingInstance.SubHeadingInstances != null)
					{
						ProcessMatrixHeadings(matrixInstance, matrixHeadingInstance.SubHeadingInstances, matrixHeadingInstance.ChildrenStartAndEndPages, ref cellsCanGetReferenced, ref rowPages, startPage, endPage);
					}
					if (matrixHeadingInstance.IsSubtotal)
					{
						m_pageSectionContext.ExitMatrixHeadingScope(matrixHeadingDef.IsColumn);
					}
					m_pageSectionContext.ExitMatrixHeadingScope(matrixHeadingDef.IsColumn);
				}
			}
		}

		private sealed class UpgraderForV1Beta2
		{
			private ReportProcessing.Pagination m_pagination;

			private ReportProcessing.NavigationInfo m_navigationInfo;

			private bool m_onePass;

			private ChunkManager.RenderingChunkManager m_chunkManager;

			private bool m_hasDocMap;

			internal void Upgrade(Report report)
			{
				ReportPublishing.CalculateChildrenPostions(report);
				ReportPublishing.CalculateChildrenDependencies(report);
			}

			internal void Upgrade(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback)
			{
				Report report = reportSnapshot.Report;
				ReportInstance reportInstance = reportSnapshot.ReportInstance;
				Upgrade(report);
				m_pagination = new ReportProcessing.Pagination(report.InteractiveHeightValue);
				m_navigationInfo = new ReportProcessing.NavigationInfo();
				m_onePass = report.MergeOnePass;
				m_chunkManager = chunkManager;
				m_hasDocMap = reportSnapshot.HasDocumentMap;
				ProcessReport(report, reportInstance);
			}

			private void ProcessReport(Report report, ReportInstance reportInstance)
			{
				m_pagination.SetReportItemStartPage(report, softPageAtStart: false);
				ProcessReportItemColInstance(reportInstance.ReportItemColInstance);
				m_pagination.ProcessEndPage(reportInstance, report, pageBreakAtEnd: false, childrenOnThisPage: false);
				reportInstance.NumberOfPages = report.EndPage + 1;
			}

			private void ProcessReportItemColInstance(ReportItemColInstance collectionInstance)
			{
				if (collectionInstance == null)
				{
					return;
				}
				ReportItemCollection reportItemColDef = collectionInstance.ReportItemColDef;
				if (reportItemColDef == null || reportItemColDef.Count < 1)
				{
					return;
				}
				ReportItemColInstanceInfo instanceInfo = collectionInstance.GetInstanceInfo(m_chunkManager, inPageSection: false);
				collectionInstance.ChildrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
				ReportItem parent = reportItemColDef[0].Parent;
				int num = parent.StartPage;
				bool flag = false;
				if (parent is Report || parent is List || parent is Rectangle || parent is SubReport || parent is CustomReportItem)
				{
					flag = true;
					collectionInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				}
				int i = 0;
				int index = 0;
				for (; i < reportItemColDef.Count; i++)
				{
					reportItemColDef.GetReportItem(i, out bool computed, out int internalIndex, out ReportItem reportItem);
					if (computed)
					{
						reportItem = reportItemColDef.ComputedReportItems[internalIndex];
						ProcessReportItemInstance(collectionInstance.ReportItemInstances[index]);
					}
					else
					{
						collectionInstance.SetPaginationForNonComputedChild(m_pagination, reportItem, parent);
						reportItem.ProcessNavigationAction(m_navigationInfo, collectionInstance.ChildrenNonComputedUniqueNames[internalIndex], reportItem.StartPage);
					}
					num = Math.Max(num, reportItem.EndPage);
					if (flag)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartPage = reportItem.StartPage;
						renderingPagesRanges.EndPage = reportItem.EndPage;
						collectionInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					}
				}
				if (num > parent.EndPage)
				{
					parent.EndPage = num;
					m_pagination.SetCurrentPageHeight(parent, 1.0);
				}
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance)
			{
				ReportItem reportItemDef = reportItemInstance.ReportItemDef;
				bool isContainer = reportItemDef is SubReport || reportItemDef is Rectangle || reportItemDef is DataRegion;
				m_pagination.EnterIgnorePageBreak(reportItemDef.Visibility, ignoreAlways: false);
				ReportItemInstanceInfo instanceInfo = reportItemInstance.GetInstanceInfo(m_chunkManager);
				reportItemDef.StartHidden = instanceInfo.StartHidden;
				m_pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
				string label = instanceInfo.Label;
				if (reportItemDef is Rectangle)
				{
					Rectangle rectangle = (Rectangle)reportItemDef;
					RectangleInstance rectangleInstance = (RectangleInstance)reportItemInstance;
					bool softPageAtStart = m_pagination.CalculateSoftPageBreak(rectangle, 0.0, rectangle.DistanceBeforeTop, ignoreSoftPageBreak: false);
					m_pagination.SetReportItemStartPage(rectangle, softPageAtStart);
					if (label != null)
					{
						m_navigationInfo.EnterDocumentMapChildren();
					}
					ProcessReportItemColInstance(rectangleInstance.ReportItemColInstance);
					m_pagination.ProcessEndPage(rectangleInstance, reportItemDef, rectangle.PageBreakAtEnd || rectangle.PageBreakAtStart, childrenOnThisPage: false);
				}
				else if (reportItemDef is DataRegion)
				{
					DataRegion dataRegion = (DataRegion)reportItemDef;
					bool softPageAtStart2 = m_pagination.CalculateSoftPageBreak(reportItemDef, 0.0, dataRegion.DistanceBeforeTop, ignoreSoftPageBreak: false);
					m_pagination.SetReportItemStartPage(reportItemDef, softPageAtStart2);
					if (reportItemDef is List)
					{
						ListInstance listInstance = (ListInstance)reportItemInstance;
						List list = (List)reportItemDef;
						if (-1 == list.ContentStartPage)
						{
							list.ContentStartPage = list.StartPage;
						}
						if (label != null)
						{
							m_navigationInfo.EnterDocumentMapChildren();
						}
						ProcessList(list, listInstance);
						m_pagination.ProcessListRenderingPages(listInstance, list);
					}
					else if (reportItemDef is Matrix)
					{
						if (label != null)
						{
							m_navigationInfo.EnterDocumentMapChildren();
						}
						ProcessMatrix((Matrix)reportItemDef, (MatrixInstance)reportItemInstance);
					}
					else if (reportItemDef is Chart)
					{
						if (label != null)
						{
							m_navigationInfo.EnterDocumentMapChildren();
						}
						ChartInstance riInstance = (ChartInstance)reportItemInstance;
						m_pagination.ProcessEndPage(riInstance, reportItemDef, ((Chart)reportItemDef).PageBreakAtEnd, childrenOnThisPage: false);
					}
					else if (reportItemDef is Table)
					{
						if (label != null)
						{
							m_navigationInfo.EnterDocumentMapChildren();
						}
						TableInstance tableInstance = (TableInstance)reportItemInstance;
						Table tableDef = (Table)reportItemDef;
						ProcessTable(tableDef, tableInstance);
						m_pagination.ProcessTableRenderingPages(tableInstance, (Table)reportItemDef);
					}
					else if (reportItemDef is OWCChart)
					{
						if (label != null)
						{
							m_navigationInfo.EnterDocumentMapChildren();
						}
						m_pagination.ProcessEndPage((OWCChartInstance)reportItemInstance, reportItemDef, ((OWCChart)reportItemDef).PageBreakAtEnd, childrenOnThisPage: false);
					}
				}
				else
				{
					if (reportItemDef.Parent != null && (reportItemDef.Parent is Rectangle || reportItemDef.Parent is Report || reportItemDef.Parent is List))
					{
						bool softPageAtStart3 = m_pagination.CalculateSoftPageBreak(reportItemDef, 0.0, reportItemDef.DistanceBeforeTop, ignoreSoftPageBreak: false, logicalPageBreak: false);
						m_pagination.SetReportItemStartPage(reportItemDef, softPageAtStart3);
					}
					if (reportItemDef is SubReport)
					{
						if (label != null)
						{
							m_navigationInfo.EnterDocumentMapChildren();
						}
						SubReport subReport = (SubReport)reportItemDef;
						SubReportInstance subReportInstance = (SubReportInstance)reportItemInstance;
						if (subReportInstance.ReportInstance != null)
						{
							ProcessReport(subReport.Report, subReportInstance.ReportInstance);
						}
						m_pagination.ProcessEndPage(subReportInstance, subReport, pageBreakAtEnd: false, childrenOnThisPage: false);
					}
				}
				if (label != null)
				{
					m_navigationInfo.AddToDocumentMap(reportItemInstance.GetDocumentMapUniqueName(), isContainer, reportItemDef.StartPage, label);
				}
				if (reportItemDef.Parent != null)
				{
					if (reportItemDef.EndPage > reportItemDef.Parent.EndPage)
					{
						reportItemDef.Parent.EndPage = reportItemDef.EndPage;
						reportItemDef.Parent.BottomInEndPage = reportItemDef.BottomInEndPage;
						if (reportItemDef.Parent is List)
						{
							((List)reportItemDef.Parent).ContentStartPage = reportItemDef.EndPage;
						}
					}
					else if (reportItemDef.EndPage == reportItemDef.Parent.EndPage)
					{
						reportItemDef.Parent.BottomInEndPage = Math.Max(reportItemDef.Parent.BottomInEndPage, reportItemDef.BottomInEndPage);
					}
				}
				m_pagination.LeaveIgnorePageBreak(reportItemDef.Visibility, ignoreAlways: false);
			}

			private void ProcessList(List listDef, ListInstance listInstance)
			{
				listInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				m_pagination.EnterIgnorePageBreak(listDef.Visibility, ignoreAlways: false);
				if (listDef.Grouping != null)
				{
					ProcessListGroupContents(listDef, listInstance);
				}
				else
				{
					ProcessListDetailContents(listDef, listInstance);
				}
				m_pagination.LeaveIgnorePageBreak(listDef.Visibility, ignoreAlways: false);
			}

			private void ProcessListGroupContents(List listDef, ListInstance listInstance)
			{
				for (int i = 0; i < listInstance.ListContents.Count; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(m_chunkManager);
					listDef.StartHidden = instanceInfo.StartHidden;
					m_pagination.EnterIgnoreHeight(listDef.StartHidden);
					string label = instanceInfo.Label;
					bool flag = false;
					if (i > 0)
					{
						flag = m_pagination.CalculateSoftPageBreak(null, 0.0, 0.0, ignoreSoftPageBreak: false, listDef.Grouping.PageBreakAtStart);
						if (!m_pagination.IgnorePageBreak && (m_pagination.CanMoveToNextPage(listDef.Grouping.PageBreakAtStart) || flag))
						{
							listDef.ContentStartPage++;
							m_pagination.SetCurrentPageHeight(listDef, 0.0);
						}
					}
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					renderingPagesRanges.StartPage = listDef.ContentStartPage;
					if (listDef.Grouping.GroupLabel != null && label != null)
					{
						m_navigationInfo.EnterDocumentMapChildren();
					}
					int startPage = listDef.StartPage;
					listDef.StartPage = listDef.ContentStartPage;
					ProcessReportItemColInstance(listContentInstance.ReportItemColInstance);
					m_pagination.ProcessEndGroupPage(listDef.IsListMostInner ? listDef.HeightValue : 0.0, listDef.Grouping.PageBreakAtEnd, listDef, childrenOnThisPage: true, listDef.StartHidden);
					listDef.ContentStartPage = listDef.EndPage;
					listDef.StartPage = startPage;
					if (m_pagination.ShouldItemMoveToChildStartPage(listDef))
					{
						renderingPagesRanges.StartPage = listContentInstance.ReportItemColInstance.ChildrenStartAndEndPages[0].StartPage;
					}
					renderingPagesRanges.EndPage = listDef.EndPage;
					listInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					if (listDef.Grouping.GroupLabel != null && label != null)
					{
						m_navigationInfo.AddToDocumentMap(listContentInstance.UniqueName, isContainer: true, startPage, label);
					}
				}
			}

			private void ProcessListDetailContents(List listDef, ListInstance listInstance)
			{
				double heightValue = listDef.HeightValue;
				for (int i = 0; i < listInstance.ListContents.Count; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(m_chunkManager);
					listDef.StartHidden = instanceInfo.StartHidden;
					m_pagination.EnterIgnoreHeight(listDef.StartHidden);
					_ = instanceInfo.Label;
					if (!m_pagination.IgnoreHeight)
					{
						m_pagination.AddToCurrentPageHeight(listDef, heightValue);
					}
					if (!m_pagination.IgnorePageBreak && m_pagination.CurrentPageHeight >= m_pagination.PageHeight && listInstance.NumberOfContentsOnThisPage > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = i + 1 - listInstance.NumberOfContentsOnThisPage;
						renderingPagesRanges.NumberOfDetails = listInstance.NumberOfContentsOnThisPage;
						m_pagination.SetCurrentPageHeight(listDef, 0.0);
						listDef.ContentStartPage++;
						listDef.BottomInEndPage = 0.0;
						listInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
						listInstance.NumberOfContentsOnThisPage = 1;
					}
					else
					{
						listInstance.NumberOfContentsOnThisPage++;
					}
					int startPage = listDef.StartPage;
					listDef.StartPage = listDef.ContentStartPage;
					m_pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
					m_pagination.EnterIgnoreHeight(startHidden: true);
					ProcessReportItemColInstance(listContentInstance.ReportItemColInstance);
					m_pagination.LeaveIgnoreHeight(startHidden: true);
					m_pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
					m_pagination.ProcessEndGroupPage(0.0, pageBreakAtEnd: false, listDef, listInstance.NumberOfContentsOnThisPage > 0, listDef.StartHidden);
					listDef.StartPage = startPage;
				}
				listDef.EndPage = Math.Max(listDef.ContentStartPage, listDef.EndPage);
			}

			private void ProcessTable(Table tableDef, TableInstance tableInstance)
			{
				tableInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				tableInstance.CurrentPage = tableDef.StartPage;
				tableDef.CurrentPage = tableDef.StartPage;
				m_pagination.InitProcessTableRenderingPages(tableInstance, tableDef);
				if (tableInstance.HeaderRowInstances != null)
				{
					for (int i = 0; i < tableInstance.HeaderRowInstances.Length; i++)
					{
						ProcessReportItemColInstance(tableInstance.HeaderRowInstances[i].TableRowReportItemColInstance);
					}
				}
				if (tableInstance.FooterRowInstances != null)
				{
					for (int j = 0; j < tableInstance.FooterRowInstances.Length; j++)
					{
						ProcessReportItemColInstance(tableInstance.FooterRowInstances[j].TableRowReportItemColInstance);
					}
				}
				if (tableInstance.TableGroupInstances != null)
				{
					ProcessTableGroups(tableDef, tableInstance, tableInstance.TableGroupInstances, tableInstance.ChildrenStartAndEndPages);
				}
				else if (tableInstance.TableDetailInstances != null)
				{
					int num2 = tableInstance.NumberOfChildrenOnThisPage = ProcessTableDetails(tableDef, tableInstance, tableDef.TableDetail, tableInstance.TableDetailInstances, tableInstance.ChildrenStartAndEndPages);
					if (num2 > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = tableInstance.TableDetailInstances.Count - num2;
						renderingPagesRanges.NumberOfDetails = num2;
						tableInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					}
				}
			}

			private void ProcessTableGroups(Table tableDef, TableInstance tableInstance, TableGroupInstanceList tableGroupInstances, RenderingPagesRangesList pagesList)
			{
				for (int i = 0; i < tableGroupInstances.Count; i++)
				{
					TableGroupInstance tableGroupInstance = tableGroupInstances[i];
					TableGroup tableGroupDef = tableGroupInstance.TableGroupDef;
					tableGroupInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
					TableGroupInstanceInfo instanceInfo = tableGroupInstance.GetInstanceInfo(m_chunkManager);
					tableGroupDef.StartHidden = instanceInfo.StartHidden;
					m_pagination.EnterIgnoreHeight(tableGroupDef.StartHidden);
					string label = instanceInfo.Label;
					if (label != null)
					{
						m_navigationInfo.EnterDocumentMapChildren();
					}
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					m_pagination.InitProcessingTableGroup(tableInstance, tableDef, tableGroupInstance, tableGroupDef, ref renderingPagesRanges, i == 0);
					if (tableGroupInstance.HeaderRowInstances != null)
					{
						for (int j = 0; j < tableGroupInstance.HeaderRowInstances.Length; j++)
						{
							ProcessReportItemColInstance(tableGroupInstance.HeaderRowInstances[j].TableRowReportItemColInstance);
						}
					}
					if (tableGroupInstance.FooterRowInstances != null)
					{
						for (int k = 0; k < tableGroupInstance.FooterRowInstances.Length; k++)
						{
							ProcessReportItemColInstance(tableGroupInstance.FooterRowInstances[k].TableRowReportItemColInstance);
						}
					}
					if (tableGroupInstance.SubGroupInstances != null)
					{
						ProcessTableGroups(tableDef, tableInstance, tableGroupInstance.SubGroupInstances, tableGroupInstance.ChildrenStartAndEndPages);
					}
					else if (tableGroupInstance.TableDetailInstances != null)
					{
						int num2 = tableGroupInstance.NumberOfChildrenOnThisPage = ProcessTableDetails(tableDef, tableInstance, tableDef.TableDetail, tableGroupInstance.TableDetailInstances, tableGroupInstance.ChildrenStartAndEndPages);
						if (num2 > 0)
						{
							RenderingPagesRanges renderingPagesRanges2 = default(RenderingPagesRanges);
							renderingPagesRanges2.StartRow = tableGroupInstance.TableDetailInstances.Count - num2;
							renderingPagesRanges2.NumberOfDetails = num2;
							tableGroupInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges2);
						}
					}
					double footerHeightValue = tableGroupDef.FooterHeightValue;
					tableGroupDef.EndPage = tableInstance.CurrentPage;
					m_pagination.ProcessEndGroupPage(footerHeightValue, tableGroupDef.PropagatedPageBreakAtEnd || tableGroupDef.Grouping.PageBreakAtEnd, tableDef, tableGroupInstance.NumberOfChildrenOnThisPage > 0, tableGroupDef.StartHidden);
					renderingPagesRanges.EndPage = tableGroupDef.EndPage;
					pagesList.Add(renderingPagesRanges);
					m_pagination.LeaveIgnorePageBreak(tableGroupDef.Visibility, ignoreAlways: false);
				}
			}

			private int ProcessTableDetails(Table tableDef, TableInstance tableInstance, TableDetail detailDef, TableDetailInstanceList detailInstances, RenderingPagesRangesList pagesList)
			{
				TableRowList detailRows = detailDef.DetailRows;
				double detailHeightValue = -1.0;
				m_pagination.EnterIgnorePageBreak(detailRows[0].Visibility, ignoreAlways: false);
				int numberOfChildrenOnThisPage = 0;
				for (int i = 0; i < detailInstances.Count; i++)
				{
					TableDetailInstance tableDetailInstance = detailInstances[i];
					TableDetailInstanceInfo instanceInfo = tableDetailInstance.GetInstanceInfo(m_chunkManager);
					detailDef.StartHidden = instanceInfo.StartHidden;
					m_pagination.EnterIgnoreHeight(detailDef.StartHidden);
					m_pagination.ProcessTableDetails(tableDef, tableDetailInstance, detailInstances, ref detailHeightValue, detailRows, pagesList, ref numberOfChildrenOnThisPage);
					tableInstance.CurrentPage = tableDef.CurrentPage;
					tableInstance.NumberOfChildrenOnThisPage = numberOfChildrenOnThisPage;
					m_pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
					m_pagination.EnterIgnoreHeight(startHidden: true);
					if (tableDetailInstance.DetailRowInstances != null)
					{
						for (int j = 0; j < tableDetailInstance.DetailRowInstances.Length; j++)
						{
							ProcessReportItemColInstance(tableDetailInstance.DetailRowInstances[i].TableRowReportItemColInstance);
						}
					}
					m_pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
					m_pagination.LeaveIgnoreHeight(startHidden: true);
					m_pagination.LeaveIgnoreHeight(detailDef.StartHidden);
				}
				m_pagination.LeaveIgnorePageBreak(detailRows[0].Visibility, ignoreAlways: false);
				return numberOfChildrenOnThisPage;
			}

			private void ProcessMatrix(Matrix matrixDef, MatrixInstance matrixInstance)
			{
				matrixInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				m_pagination.EnterIgnorePageBreak(matrixDef.Visibility, ignoreAlways: false);
				matrixDef.CurrentPage = matrixDef.StartPage;
				((IPageItem)matrixInstance).StartPage = matrixDef.StartPage;
				MatrixInstanceInfo matrixInstanceInfo = (MatrixInstanceInfo)matrixInstance.GetInstanceInfo(m_chunkManager);
				matrixDef.StartHidden = matrixInstanceInfo.StartHidden;
				m_pagination.EnterIgnoreHeight(matrixDef.StartHidden);
				if (matrixInstance.CornerContent != null)
				{
					ProcessReportItemInstance(matrixInstance.CornerContent);
				}
				else
				{
					ReportItem cornerReportItem = matrixDef.CornerReportItem;
					if (cornerReportItem != null)
					{
						NonComputedUniqueNames cornerNonComputedNames = matrixInstanceInfo.CornerNonComputedNames;
						cornerReportItem.ProcessNavigationAction(m_navigationInfo, cornerNonComputedNames, matrixDef.CurrentPage);
					}
				}
				ProcessMatrixColumnHeadings(matrixDef, matrixInstance, matrixInstance.ColumnInstances);
				ProcessMatrixRowHeadings(matrixDef, matrixInstance, matrixInstance.RowInstances, matrixInstance.ChildrenStartAndEndPages, 0, 0, 0);
				int count = matrixInstance.ChildrenStartAndEndPages.Count;
				if (count > 0)
				{
					matrixDef.EndPage = matrixInstance.ChildrenStartAndEndPages[count - 1].EndPage;
				}
				else
				{
					matrixDef.EndPage = ((IPageItem)matrixInstance).StartPage;
				}
				m_pagination.ProcessEndPage(matrixInstance, matrixDef, matrixDef.PageBreakAtEnd || matrixDef.PropagatedPageBreakAtEnd, matrixInstance.NumberOfChildrenOnThisPage > 0);
			}

			private void ProcessMatrixColumnHeadings(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeadingInstanceList headingInstances)
			{
				for (int i = 0; i < headingInstances.Count; i++)
				{
					MatrixHeadingInstance matrixHeadingInstance = headingInstances[i];
					MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
					MatrixHeadingInstanceInfo instanceInfo = matrixHeadingInstance.GetInstanceInfo(m_chunkManager);
					ProcessMatrixHeadingContent(matrixDef, matrixHeadingDef, matrixHeadingInstance, instanceInfo);
					if (matrixHeadingInstance.SubHeadingInstances != null)
					{
						ProcessMatrixColumnHeadings(matrixDef, matrixInstance, matrixHeadingInstance.SubHeadingInstances);
					}
					if (instanceInfo.Label != null)
					{
						m_navigationInfo.AddToDocumentMap(matrixHeadingInstance.UniqueName, isContainer: true, matrixDef.CurrentPage, instanceInfo.Label);
					}
				}
			}

			private void ProcessMatrixRowHeadings(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeadingInstanceList headingInstances, RenderingPagesRangesList pagesList, int rowDefIndex, int headingCellIndex, int rowIndex)
			{
				if (headingInstances == null)
				{
					if (!m_pagination.IgnoreHeight)
					{
						m_pagination.AddToCurrentPageHeight(matrixDef, matrixDef.MatrixRows[rowDefIndex].HeightValue);
					}
					if (!m_pagination.IgnorePageBreak && m_pagination.CurrentPageHeight >= m_pagination.PageHeight && matrixInstance.RowInstances.Count > 1)
					{
						m_pagination.SetCurrentPageHeight(matrixDef, 0.0);
						matrixInstance.ExtraPagesFilled++;
						matrixDef.CurrentPage++;
						matrixInstance.NumberOfChildrenOnThisPage = 0;
					}
					else
					{
						matrixInstance.NumberOfChildrenOnThisPage++;
					}
					for (int i = 0; i < matrixInstance.CellColumnCount; i++)
					{
						MatrixCellInstance matrixCellInstance = matrixInstance.Cells[rowIndex][i];
						ReportItemInstance content = matrixCellInstance.Content;
						if (content != null)
						{
							ProcessReportItemInstance(content);
							continue;
						}
						MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(m_chunkManager);
						ReportItem cellReportItem = matrixDef.GetCellReportItem(instanceInfo.RowIndex, instanceInfo.ColumnIndex);
						if (cellReportItem != null)
						{
							NonComputedUniqueNames contentUniqueNames = instanceInfo.ContentUniqueNames;
							cellReportItem.ProcessNavigationAction(m_navigationInfo, contentUniqueNames, matrixDef.CurrentPage);
						}
					}
					return;
				}
				for (int j = 0; j < headingInstances.Count; j++)
				{
					MatrixHeadingInstance matrixHeadingInstance = headingInstances[j];
					MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
					matrixHeadingInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
					MatrixHeadingInstanceInfo instanceInfo2 = matrixHeadingInstance.GetInstanceInfo(m_chunkManager);
					matrixHeadingDef.StartHidden = instanceInfo2.StartHidden;
					m_pagination.EnterIgnoreHeight(matrixHeadingDef.StartHidden);
					int startPage = (!matrixHeadingInstance.IsSubtotal && matrixHeadingDef.Grouping != null) ? ProcessMatrixDynamicRowHeading(matrixDef, matrixInstance, matrixHeadingDef, matrixHeadingInstance, instanceInfo2, j == 0, pagesList, rowDefIndex, instanceInfo2.HeadingCellIndex, j) : ProcessMatrixRowSubtotalOrStaticHeading(matrixDef, matrixInstance, matrixHeadingDef, matrixHeadingInstance, instanceInfo2, pagesList, matrixHeadingInstance.IsSubtotal ? rowDefIndex : j, instanceInfo2.HeadingCellIndex, j);
					if (instanceInfo2.Label != null)
					{
						m_navigationInfo.AddToDocumentMap(matrixHeadingInstance.UniqueName, isContainer: true, startPage, instanceInfo2.Label);
					}
				}
			}

			private void ProcessMatrixHeadingContent(Matrix matrixDef, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, MatrixHeadingInstanceInfo headingInstanceInfo)
			{
				if (headingInstanceInfo.Label != null)
				{
					m_navigationInfo.EnterDocumentMapChildren();
				}
				if (headingInstance.Content != null)
				{
					ProcessReportItemInstance(headingInstance.Content);
					return;
				}
				ReportItem reportItem = headingDef.ReportItem;
				if (reportItem != null)
				{
					NonComputedUniqueNames contentUniqueNames = headingInstanceInfo.ContentUniqueNames;
					reportItem.ProcessNavigationAction(m_navigationInfo, contentUniqueNames, matrixDef.CurrentPage);
				}
			}

			private int ProcessMatrixRowSubtotalOrStaticHeading(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, MatrixHeadingInstanceInfo headingInstanceInfo, RenderingPagesRangesList pagesList, int rowDefIndex, int headingCellIndex, int rowIndex)
			{
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				bool isSubtotal = headingInstance.IsSubtotal;
				m_pagination.EnterIgnorePageBreak(headingDef.Visibility, isSubtotal);
				renderingPagesRanges.StartPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
				ProcessMatrixHeadingContent(matrixDef, headingDef, headingInstance, headingInstanceInfo);
				ProcessMatrixRowHeadings(matrixDef, matrixInstance, headingInstance.SubHeadingInstances, headingInstance.ChildrenStartAndEndPages, rowDefIndex, headingCellIndex, rowIndex);
				m_pagination.LeaveIgnorePageBreak(headingDef.Visibility, isSubtotal);
				renderingPagesRanges.EndPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
				if (headingInstance.ChildrenStartAndEndPages == null || headingInstance.ChildrenStartAndEndPages.Count < 1)
				{
					renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
				}
				pagesList.Add(renderingPagesRanges);
				return renderingPagesRanges.StartPage;
			}

			private int ProcessMatrixDynamicRowHeading(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, MatrixHeadingInstanceInfo headingInstanceInfo, bool firstHeading, RenderingPagesRangesList pagesList, int rowDefIndex, int headingCellIndex, int rowIndex)
			{
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				m_pagination.EnterIgnorePageBreak(headingDef.Visibility, ignoreAlways: false);
				if (!m_pagination.IgnorePageBreak && !firstHeading && headingDef.Grouping.PageBreakAtStart && matrixInstance.NumberOfChildrenOnThisPage > 0)
				{
					m_pagination.SetCurrentPageHeight(matrixInstance.ReportItemDef, 0.0);
					matrixInstance.ExtraPagesFilled++;
					matrixDef.CurrentPage++;
					matrixInstance.NumberOfChildrenOnThisPage = 0;
				}
				renderingPagesRanges.StartPage = matrixDef.CurrentPage;
				ProcessMatrixHeadingContent(matrixDef, headingDef, headingInstance, headingInstanceInfo);
				ProcessMatrixRowHeadings(matrixDef, matrixInstance, headingInstance.SubHeadingInstances, headingInstance.ChildrenStartAndEndPages, rowDefIndex, headingCellIndex, rowIndex);
				renderingPagesRanges.EndPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
				if (headingInstance.SubHeadingInstances == null || headingInstance.SubHeadingInstances.Count < 1)
				{
					renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
				}
				else
				{
					renderingPagesRanges.EndPage = headingInstance.ChildrenStartAndEndPages[headingInstance.ChildrenStartAndEndPages.Count - 1].EndPage;
				}
				if (!m_pagination.IgnorePageBreak && matrixInstance.NumberOfChildrenOnThisPage > 0 && m_pagination.CanMoveToNextPage(headingDef.Grouping.PageBreakAtEnd))
				{
					m_pagination.SetCurrentPageHeight(matrixDef, 0.0);
					matrixInstance.ExtraPagesFilled++;
					matrixDef.CurrentPage++;
					matrixInstance.NumberOfChildrenOnThisPage = 0;
				}
				pagesList.Add(renderingPagesRanges);
				m_pagination.LeaveIgnoreHeight(headingDef.StartHidden);
				m_pagination.LeaveIgnorePageBreak(headingDef.Visibility, ignoreAlways: false);
				return renderingPagesRanges.StartPage;
			}
		}

		internal static void UpgradeToCurrent(Report report)
		{
			if (report.IntermediateFormatVersion.IsRS2000_Beta2_orOlder)
			{
				new UpgraderForV1Beta2().Upgrade(report);
			}
		}

		internal static bool UpgradeToCurrent(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback)
		{
			if (reportSnapshot.Report.IntermediateFormatVersion.IsRS2000_Beta2_orOlder)
			{
				new UpgraderForV1Beta2().Upgrade(reportSnapshot, chunkManager, createChunkCallback);
				return true;
			}
			return false;
		}

		internal static void UpgradeDatasetIDs(Report report)
		{
			if (!report.IntermediateFormatVersion.Is_WithUserSort)
			{
				new DataSetUpgrader().Upgrade(report);
			}
		}

		internal static bool CreateBookmarkDrillthroughChunks(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ChunkManager.UpgradeManager upgradeManager)
		{
			if (!reportSnapshot.Report.IntermediateFormatVersion.IsRS2005_WithSpecialChunkSplit)
			{
				new BookmarkDrillthroughUpgrader().Upgrade(reportSnapshot, chunkManager, upgradeManager);
				return true;
			}
			return false;
		}

		internal static void UpgradeToPageSectionsChunk(ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection)
		{
			if (reportSnapshot.PageSectionOffsets == null && (reportSnapshot.Report.PageHeaderEvaluation || reportSnapshot.Report.PageFooterEvaluation) && !chunkManager.PageSectionChunkExists())
			{
				new PageSectionsGenerator().Upgrade(reportSnapshot, reportContext, chunkManager, createChunkCallback, getResourceCallback, renderingContext, dataProtection);
			}
		}

		private static void SetRowPages(ref int[] rowPages, int start, int span, int pageNumber)
		{
			Global.Tracer.Assert(rowPages != null && start >= 0 && span > 0 && start + span <= rowPages.Length, "(null != rowPages && start >= 0 && span > 0 && start + span <= rowPages.Length)");
			for (int i = 0; i < span; i++)
			{
				rowPages[start + i] = pageNumber;
			}
		}
	}
}
