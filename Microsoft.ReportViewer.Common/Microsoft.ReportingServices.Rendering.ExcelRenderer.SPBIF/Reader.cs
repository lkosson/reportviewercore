using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF
{
	internal class Reader
	{
		private class RelativeStaticLocation
		{
			private int m_toggleIndex = -1;

			private int m_staticBeforeIndex = -1;

			private int m_staticAfterIndex = -1;

			internal bool? IsSummaryAfter()
			{
				if (m_toggleIndex > 0)
				{
					if (m_staticBeforeIndex > -1)
					{
						if (m_staticAfterIndex > -1)
						{
							if (m_staticBeforeIndex - m_toggleIndex <= m_staticAfterIndex - m_toggleIndex)
							{
								return false;
							}
							return true;
						}
						return false;
					}
					if (m_staticAfterIndex > -1)
					{
						return true;
					}
				}
				return null;
			}

			internal void SetIndexes(RPLTablixMemberCell member, bool isColumn)
			{
				if (member.TablixMemberDef.IsStatic)
				{
					if (m_staticBeforeIndex == -1 && m_toggleIndex == -1)
					{
						m_staticBeforeIndex = (isColumn ? member.ColIndex : member.RowIndex);
					}
					if (m_staticAfterIndex == -1 && m_toggleIndex > -1)
					{
						m_staticAfterIndex = (isColumn ? member.ColIndex : member.RowIndex);
					}
				}
				else if (member.HasToggle && m_toggleIndex == -1)
				{
					m_toggleIndex = (isColumn ? member.ColIndex : member.RowIndex);
				}
			}
		}

		internal static void ReadReportMeasurements(RPLReport report, ALayout layout, bool suppressOutlines, RPLReportSection firstReportSection)
		{
			int num = 0;
			int left = 0;
			int num2 = 0;
			int num3 = 0;
			int left2 = 0;
			int num4 = 0;
			int num5 = 0;
			int left3 = 0;
			int num6 = 0;
			RPLPageContent rPLPageContent = report.RPLPaginatedPages[0];
			RPLPageLayout pageLayout = rPLPageContent.PageLayout;
			int width = LayoutConvert.ConvertMMTo20thPoints(rPLPageContent.MaxSectionWidth);
			for (RPLReportSection rPLReportSection = firstReportSection; rPLReportSection != null; rPLReportSection = rPLPageContent.GetNextReportSection())
			{
				layout.SetIsLastSection(!rPLPageContent.HasNextReportSection());
				RPLItemMeasurement rPLItemMeasurement = rPLReportSection.Columns[0];
				ALayout pageHeaderLayout;
				if (rPLReportSection.Header != null)
				{
					num += LayoutConvert.ConvertMMTo20thPoints(rPLReportSection.Header.Top);
					left = LayoutConvert.ConvertMMTo20thPoints(rPLReportSection.Header.Left);
					num2 = LayoutConvert.ConvertMMTo20thPoints(rPLReportSection.Header.Height);
					pageHeaderLayout = layout.GetPageHeaderLayout(rPLReportSection.Header.Width, rPLReportSection.Header.Height);
					if (num2 > 0)
					{
						if (pageLayout.Style != null)
						{
							pageHeaderLayout.AddReportItem(pageLayout, num, left, width, num2, 0, 0, null, null);
						}
						pageHeaderLayout.AddReportItem(rPLReportSection.Header.Element.RPLSource, num, left, width, num2, 0, 0, null, null);
					}
				}
				else
				{
					pageHeaderLayout = layout.GetPageHeaderLayout(0f, 0f);
				}
				if (rPLItemMeasurement != null)
				{
					num3 += LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Top);
					left2 = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Left);
					num4 = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Height);
					if (layout.HeaderInBody)
					{
						num3 += num2;
					}
					if (layout.FooterInBody)
					{
						num5 = num3 + num4;
					}
					if (pageLayout.Style != null)
					{
						layout.AddReportItem(pageLayout, num3, left2, width, num4, 0, rPLItemMeasurement.State, null, null);
					}
					layout.AddReportItem(rPLItemMeasurement.Element.RPLSource, num3, left2, width, num4, 0, rPLItemMeasurement.State, null, null);
				}
				ALayout pageFooterLayout;
				if (rPLReportSection.Footer != null)
				{
					num5 += LayoutConvert.ConvertMMTo20thPoints(rPLReportSection.Footer.Top);
					left3 = LayoutConvert.ConvertMMTo20thPoints(rPLReportSection.Footer.Left);
					num6 = LayoutConvert.ConvertMMTo20thPoints(rPLReportSection.Footer.Height);
					pageFooterLayout = layout.GetPageFooterLayout(rPLReportSection.Footer.Width, rPLReportSection.Footer.Height);
					if (num6 > 0)
					{
						if (pageLayout.Style != null)
						{
							pageFooterLayout.AddReportItem(pageLayout, num5, left3, width, num6, 0, 0, null, null);
						}
						pageFooterLayout.AddReportItem(rPLReportSection.Footer.Element.RPLSource, num5, left3, width, num6, 0, 0, null, null);
					}
				}
				else
				{
					pageFooterLayout = layout.GetPageFooterLayout(0f, 0f);
				}
				Dictionary<string, ToggleParent> toggleParents = null;
				if (!suppressOutlines)
				{
					toggleParents = new Dictionary<string, ToggleParent>();
				}
				if (rPLReportSection.Header != null)
				{
					ReadMeasurement(rPLReportSection.Header.Element, pageHeaderLayout, num, left, 1, BlockOutlines.None, toggleParents, suppressOutlines, 0, null);
				}
				if (rPLItemMeasurement != null)
				{
					ReadMeasurement(rPLItemMeasurement.Element, layout, num3, left2, 1, BlockOutlines.None, toggleParents, suppressOutlines, 0, null);
				}
				if (rPLReportSection.Footer != null)
				{
					ReadMeasurement(rPLReportSection.Footer.Element, pageFooterLayout, num5, left3, 1, BlockOutlines.None, toggleParents, suppressOutlines, 0, null);
				}
				int num7 = num3 + num4;
				if (layout.FooterInBody)
				{
					num7 += num6;
				}
				num3 = (num = (num5 = num7));
				num4 = 0;
				num6 = 0;
				num2 = 0;
				rPLReportSection.Columns[0] = null;
				rPLReportSection.Header = null;
				rPLReportSection.Footer = null;
				layout.CompleteSection();
			}
			layout.CompletePage();
		}

		private static void ReadMeasurement(RPLElement element, ALayout layout, int top, int left, int generationIndex, BlockOutlines blockOutlines, Dictionary<string, ToggleParent> toggleParents, bool suppressOutlines, int elementWidth, string subreportLanguage)
		{
			RPLContainer rPLContainer = element as RPLContainer;
			if (rPLContainer != null)
			{
				if (rPLContainer.Children == null)
				{
					return;
				}
				bool flag = false;
				int num = 0;
				RPLSubReport rPLSubReport = element as RPLSubReport;
				if (rPLSubReport != null)
				{
					flag = true;
					if (!suppressOutlines)
					{
						toggleParents = new Dictionary<string, ToggleParent>();
					}
					string language = ((RPLSubReportProps)rPLSubReport.ElementProps).Language;
					if (!string.IsNullOrEmpty(language))
					{
						subreportLanguage = language;
					}
				}
				int[] array = new int[rPLContainer.Children.Length];
				int[] array2 = new int[rPLContainer.Children.Length];
				int[] array3 = new int[rPLContainer.Children.Length];
				RPLItem[] array4 = new RPLItem[rPLContainer.Children.Length];
				for (int i = 0; i < rPLContainer.Children.Length; i++)
				{
					RPLItemMeasurement rPLItemMeasurement = rPLContainer.Children[i];
					if (0f == rPLItemMeasurement.Width && 0f == rPLItemMeasurement.Height)
					{
						rPLContainer.Children[i] = null;
						continue;
					}
					if ((0f == rPLItemMeasurement.Width || 0f == rPLItemMeasurement.Height) && !(rPLItemMeasurement.Element is RPLLine))
					{
						rPLContainer.Children[i] = null;
						continue;
					}
					int num2 = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Height);
					int num3 = 0;
					if (flag)
					{
						num3 = elementWidth;
						array2[i] = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Top) + top + num;
						num += num2;
					}
					else
					{
						num3 = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Width);
						array2[i] = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Top) + top;
					}
					array[i] = num3;
					array3[i] = LayoutConvert.ConvertMMTo20thPoints(rPLItemMeasurement.Left) + left;
					RPLItem rPLItem = array4[i] = rPLItemMeasurement.Element;
					byte elementType;
					RPLItemProps itemProps = layout.RPLReport.GetItemProps(rPLItem.RPLSource, out elementType);
					if (elementType == 7)
					{
						RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)itemProps.Definition;
						if (rPLTextBoxPropsDef.IsSimple)
						{
							layout.AddReportItem(rPLItem.RPLSource, array2[i], array3[i], num3, num2, generationIndex, rPLItemMeasurement.State, subreportLanguage, toggleParents);
						}
						else
						{
							layout.AddReportItem(rPLItem, array2[i], array3[i], num3, num2, generationIndex, rPLItemMeasurement.State, subreportLanguage, toggleParents);
						}
						if (!suppressOutlines && rPLTextBoxPropsDef.IsToggleParent && !toggleParents.ContainsKey(rPLTextBoxPropsDef.Name))
						{
							toggleParents.Add(rPLTextBoxPropsDef.Name, new ToggleParent(array2[i], array3[i], num3, num2));
						}
					}
					else
					{
						layout.AddReportItem(rPLItem.RPLSource, array2[i], array3[i], num3, num2, generationIndex, rPLItemMeasurement.State, subreportLanguage, toggleParents);
					}
				}
				for (int j = 0; j < rPLContainer.Children.Length; j++)
				{
					if (rPLContainer.Children[j] != null)
					{
						ReadMeasurement(array4[j], layout, array2[j], array3[j], generationIndex + 1, blockOutlines, toggleParents, suppressOutlines, array[j], subreportLanguage);
						rPLContainer.Children[j] = null;
						array4[j] = null;
					}
				}
				rPLContainer.Children = null;
			}
			else
			{
				ReadTablixStructure(element as RPLTablix, layout, top, left, generationIndex, blockOutlines, toggleParents, suppressOutlines, subreportLanguage);
			}
		}

		private static void ReadTablixStructure(RPLTablix tablix, ALayout layout, int top, int left, int generationIndex, BlockOutlines blockOutlines, Dictionary<string, ToggleParent> toggleParents, bool suppressOutlines, string subreportLanguage)
		{
			if (tablix == null)
			{
				return;
			}
			int num = 0;
			if (tablix.ColumnWidths != null)
			{
				num = tablix.ColumnWidths.Length;
			}
			int[] array = new int[num + 1];
			array[0] = left;
			double num2 = left;
			int num3 = 0;
			int rowHeaderWidth = 0;
			for (int i = 1; i <= num; i++)
			{
				num2 += LayoutConvert.ConvertMMTo20thPointsUnrounded(tablix.ColumnWidths[i - 1]);
				array[i] = (int)Math.Round(num2);
				if (tablix.ColsBeforeRowHeaders == 0 && i == tablix.RowHeaderColumns)
				{
					rowHeaderWidth = (int)Math.Round(num2 - (double)left);
				}
			}
			num3 = (int)Math.Round(num2 - (double)left);
			int num4 = 0;
			if (tablix.RowHeights != null)
			{
				num4 = tablix.RowHeights.Length;
			}
			int[] array2 = new int[num4 + 1];
			array2[0] = top;
			double num5 = top;
			int num6 = 0;
			int columnHeaderHeight = 0;
			for (int j = 1; j <= num4; j++)
			{
				num5 += LayoutConvert.ConvertMMTo20thPointsUnrounded(tablix.RowHeights[j - 1]);
				array2[j] = (int)Math.Round(num5);
				if (j == tablix.ColumnHeaderRows)
				{
					columnHeaderHeight = (int)Math.Round(num5 - (double)top);
				}
			}
			num6 = (int)Math.Round(num5 - (double)top);
			List<RelativeStaticLocation> list = new List<RelativeStaticLocation>();
			bool flag = false;
			RPLTablixRow nextRow;
			while ((nextRow = tablix.GetNextRow()) != null)
			{
				bool isColumn = false;
				RelativeStaticLocation relativeStaticLocation = new RelativeStaticLocation();
				if (!suppressOutlines && nextRow.OmittedHeaders != null)
				{
					for (int k = 0; k < nextRow.OmittedHeaders.Count; k++)
					{
						RPLTablixMemberCell rPLTablixMemberCell = nextRow.OmittedHeaders[k];
						if (SetRelativeStaticLocation(rPLTablixMemberCell, layout, relativeStaticLocation, list, ref isColumn))
						{
							flag = true;
						}
						ReadTablixCellProperties(rPLTablixMemberCell, layout, array2, array, generationIndex + 1, tablix.LayoutDirection == RPLFormat.Directions.RTL, blockOutlines, toggleParents, suppressOutlines, subreportLanguage);
						nextRow.OmittedHeaders[k] = null;
					}
				}
				for (int l = 0; l < nextRow.NumCells; l++)
				{
					RPLTablixCell rPLTablixCell = nextRow[l];
					if (!suppressOutlines)
					{
						RPLTablixMemberCell rPLTablixMemberCell2 = rPLTablixCell as RPLTablixMemberCell;
						if (rPLTablixMemberCell2 != null && SetRelativeStaticLocation(rPLTablixMemberCell2, layout, relativeStaticLocation, list, ref isColumn))
						{
							flag = true;
						}
					}
					ReadTablixCellProperties(rPLTablixCell, layout, array2, array, generationIndex + 1, tablix.LayoutDirection == RPLFormat.Directions.RTL, blockOutlines, toggleParents, suppressOutlines, subreportLanguage);
					nextRow[l] = null;
				}
				if (!suppressOutlines && isColumn && !layout.SummaryColumnAfter.HasValue)
				{
					layout.SummaryColumnAfter = relativeStaticLocation.IsSummaryAfter();
				}
			}
			if (!suppressOutlines)
			{
				int num7 = 0;
				while (!layout.SummaryRowAfter.HasValue && num7 < list.Count)
				{
					RelativeStaticLocation relativeStaticLocation2 = list[num7];
					layout.SummaryRowAfter = relativeStaticLocation2.IsSummaryAfter();
					num7++;
				}
				if (flag)
				{
					layout.AddStructuralItem(top, left, num3, num6, ALayout.TablixStructStart + ALayout.TablixStructGenerationOffset * generationIndex, rowHeaderWidth, columnHeaderHeight, tablix.LayoutDirection == RPLFormat.Directions.RTL);
				}
			}
		}

		private static void ReadTablixCellProperties(RPLTablixCell cell, ALayout layout, int[] rowTops, int[] columnLefts, int generationIndex, bool isRTL, BlockOutlines blockOutlines, Dictionary<string, ToggleParent> toggleParents, bool suppressOutlines, string subreportLanguage)
		{
			int num = columnLefts[cell.ColIndex];
			int num2 = rowTops[cell.RowIndex];
			int num3 = columnLefts[cell.ColIndex + cell.ColSpan] - num;
			int num4 = rowTops[cell.RowIndex + cell.RowSpan] - num2;
			bool flag = false;
			if ((num3 == 0 && cell.ColSpan != 0) || (num4 == 0 && cell.RowSpan != 0))
			{
				return;
			}
			if (!suppressOutlines && cell is RPLTablixMemberCell && blockOutlines != (BlockOutlines)3)
			{
				RPLTablixMemberCell rPLTablixMemberCell = (RPLTablixMemberCell)cell;
				TogglePosition togglePosition = TogglePosition.None;
				flag = rPLTablixMemberCell.HasToggle;
				if (rPLTablixMemberCell.TablixMemberDef.IsColumn)
				{
					if ((blockOutlines & BlockOutlines.Columns) != 0)
					{
						flag = false;
					}
					togglePosition = TogglePosition.Above;
				}
				else
				{
					if ((blockOutlines & BlockOutlines.Rows) != 0)
					{
						flag = false;
					}
					togglePosition = ((!isRTL) ? TogglePosition.Left : TogglePosition.Right);
				}
				int left = num;
				if (isRTL && num3 == 0 && cell.ColIndex > 0)
				{
					left = columnLefts[cell.ColIndex - 1];
				}
				layout.AddStructuralItem(num2, left, num3, num4, flag, generationIndex, rPLTablixMemberCell, togglePosition);
			}
			RPLItem element = cell.Element;
			if (element == null)
			{
				return;
			}
			byte elementType;
			RPLItemProps itemProps = layout.RPLReport.GetItemProps(element.RPLSource, out elementType);
			if (elementType == 7)
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)itemProps.Definition;
				if (rPLTextBoxPropsDef.IsSimple)
				{
					layout.AddReportItem(element.RPLSource, num2, num, num3, num4, generationIndex, cell.ElementState, subreportLanguage, toggleParents);
				}
				else
				{
					layout.AddReportItem(element, num2, num, num3, num4, generationIndex, cell.ElementState, subreportLanguage, toggleParents);
				}
				if (!suppressOutlines && rPLTextBoxPropsDef.IsToggleParent && !toggleParents.ContainsKey(rPLTextBoxPropsDef.Name))
				{
					toggleParents.Add(rPLTextBoxPropsDef.Name, new ToggleParent(num2, num, num3, num4));
				}
			}
			else
			{
				layout.AddReportItem(element.RPLSource, num2, num, num3, num4, generationIndex, cell.ElementState, subreportLanguage, toggleParents);
			}
			if (!suppressOutlines)
			{
				if (rowTops.Length > 2)
				{
					blockOutlines |= BlockOutlines.Columns;
				}
				if (columnLefts.Length > 2)
				{
					blockOutlines |= BlockOutlines.Rows;
				}
			}
			ReadMeasurement(element, layout, num2, num, generationIndex + 1, blockOutlines, toggleParents, suppressOutlines, num3, subreportLanguage);
		}

		private static bool SetRelativeStaticLocation(RPLTablixMemberCell member, ALayout layout, RelativeStaticLocation staticLocation, List<RelativeStaticLocation> rowHeaderStaticLocations, ref bool isColumn)
		{
			if (member.TablixMemberDef.IsColumn)
			{
				isColumn = true;
				if (!layout.SummaryColumnAfter.HasValue)
				{
					if (member.IsRecursiveToggle)
					{
						layout.SummaryColumnAfter = false;
					}
					else
					{
						staticLocation.SetIndexes(member, isColumn: true);
					}
				}
			}
			else
			{
				isColumn = false;
				if (!layout.SummaryRowAfter.HasValue)
				{
					if (member.IsRecursiveToggle)
					{
						layout.SummaryRowAfter = false;
					}
					else
					{
						int level = member.TablixMemberDef.Level;
						for (int i = rowHeaderStaticLocations.Count; i <= level; i++)
						{
							rowHeaderStaticLocations.Add(new RelativeStaticLocation());
						}
						rowHeaderStaticLocations[level].SetIndexes(member, isColumn: false);
					}
				}
			}
			return member.HasToggle;
		}
	}
}
