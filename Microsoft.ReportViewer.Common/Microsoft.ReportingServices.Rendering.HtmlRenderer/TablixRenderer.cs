using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class TablixRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		private const string FixedRowMarker = "r";

		private const string FixedColMarker = "c";

		private const string EmptyColMarker = "e";

		private const string EmptyHeightColMarker = "h";

		public TablixRenderer(HTML5Renderer renderer)
		{
			html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel = false)
		{
			RPLTablix rPLTablix = reportItem as RPLTablix;
			RPLElementProps elementProps = rPLTablix.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			string uniqueName = elementProps.UniqueName;
			TablixFixedHeaderStorage tablixFixedHeaderStorage = new TablixFixedHeaderStorage();
			if (rPLTablix.ColumnWidths == null)
			{
				rPLTablix.ColumnWidths = new float[0];
			}
			if (rPLTablix.RowHeights == null)
			{
				rPLTablix.RowHeights = new float[0];
			}
			bool flag = InitFixedColumnHeaders(rPLTablix, uniqueName, tablixFixedHeaderStorage);
			bool flag2 = InitFixedRowHeaders(rPLTablix, uniqueName, tablixFixedHeaderStorage);
			bool flag3 = rPLTablix.ColumnHeaderRows == 0 && rPLTablix.RowHeaderColumns == 0 && !html5Renderer.m_deviceInfo.AccessibleTablix;
			if (treatAsTopLevel)
			{
				html5Renderer.WriteStream(HTMLElements.m_openSpan);
				html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleTableBoxLabel, elementProps, treatAsTopLevel);
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				html5Renderer.WriteStream(HTMLElements.m_closeSpan);
			}
			if (flag && flag2)
			{
				tablixFixedHeaderStorage.CornerHeaders = new List<string>();
			}
			html5Renderer.WriteStream(HTMLElements.m_openTable);
			int columns = (rPLTablix.ColumnHeaderRows > 0 || rPLTablix.RowHeaderColumns > 0 || !flag3) ? (rPLTablix.ColumnWidths.Length + 1) : rPLTablix.ColumnWidths.Length;
			html5Renderer.WriteStream(HTMLElements.m_cols);
			html5Renderer.WriteStream(columns.ToString(CultureInfo.InvariantCulture));
			html5Renderer.WriteStream(HTMLElements.m_quote);
			if (renderId || flag || flag2)
			{
				html5Renderer.RenderReportItemId(uniqueName);
			}
			html5Renderer.WriteToolTip(rPLTablix.ElementProps);
			html5Renderer.WriteStream(HTMLElements.m_zeroBorder);
			html5Renderer.OpenStyle();
			html5Renderer.WriteStream(HTMLElements.m_borderCollapse);
			html5Renderer.WriteStream(HTMLElements.m_semiColon);
			if (html5Renderer.m_deviceInfo.OutlookCompat && measurement != null)
			{
				html5Renderer.RenderMeasurementWidth(measurement.Width, renderMinWidth: true);
			}
			html5Renderer.RenderReportItemStyle(rPLTablix, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
			html5Renderer.CloseStyle(renderQuote: true);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			int colsBeforeRowHeaders = rPLTablix.ColsBeforeRowHeaders;
			RPLTablixRow nextRow = rPLTablix.GetNextRow();
			List<RPLTablixOmittedRow> list = new List<RPLTablixOmittedRow>();
			while (nextRow != null && nextRow is RPLTablixOmittedRow)
			{
				list.Add((RPLTablixOmittedRow)nextRow);
				nextRow = rPLTablix.GetNextRow();
			}
			if (flag3)
			{
				RenderEmptyTopTablixRow(rPLTablix, list, uniqueName, emptyCol: false, tablixFixedHeaderStorage);
				RenderSimpleTablixRows(rPLTablix, uniqueName, nextRow, borderContext, tablixFixedHeaderStorage);
			}
			else
			{
				styleContext = new StyleContext();
				float[] columnWidths = rPLTablix.ColumnWidths;
				float[] rowHeights = rPLTablix.RowHeights;
				int num = columnWidths.Length;
				int numRows = rowHeights.Length;
				RenderEmptyTopTablixRow(rPLTablix, list, uniqueName, emptyCol: true, tablixFixedHeaderStorage);
				bool flag4 = flag;
				int num2 = 0;
				list = new List<RPLTablixOmittedRow>();
				HTMLHeader[] array = null;
				string[] array2 = null;
				OmittedHeaderStack omittedHeaders = null;
				if (html5Renderer.m_deviceInfo.AccessibleTablix)
				{
					array = new HTMLHeader[rPLTablix.RowHeaderColumns];
					array2 = new string[num];
					omittedHeaders = new OmittedHeaderStack();
				}
				while (nextRow != null)
				{
					if (nextRow is RPLTablixOmittedRow)
					{
						list.Add((RPLTablixOmittedRow)nextRow);
						nextRow = rPLTablix.GetNextRow();
						continue;
					}
					if (rowHeights[num2] == 0f && num2 > 1 && nextRow.NumCells == 1 && nextRow[0].Element is RPLRectangle)
					{
						RPLRectangle rPLRectangle = (RPLRectangle)nextRow[0].Element;
						if (rPLRectangle.Children == null || rPLRectangle.Children.Length == 0)
						{
							nextRow = rPLTablix.GetNextRow();
							num2++;
							continue;
						}
					}
					html5Renderer.WriteStream(HTMLElements.m_openTR);
					if (rPLTablix.FixedRow(num2) || flag2 || flag4)
					{
						string text = uniqueName + "r" + num2;
						html5Renderer.RenderReportItemId(text);
						if (rPLTablix.FixedRow(num2))
						{
							tablixFixedHeaderStorage.ColumnHeaders.Add(text);
							if (tablixFixedHeaderStorage.CornerHeaders != null)
							{
								tablixFixedHeaderStorage.CornerHeaders.Add(text);
							}
						}
						else if (flag4)
						{
							tablixFixedHeaderStorage.BodyID = text;
							flag4 = false;
						}
						if (flag2)
						{
							tablixFixedHeaderStorage.RowHeaders.Add(text);
						}
					}
					html5Renderer.WriteStream(HTMLElements.m_valign);
					html5Renderer.WriteStream(HTMLElements.m_topValue);
					html5Renderer.WriteStream(HTMLElements.m_quote);
					html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					RenderEmptyHeightCell(rowHeights[num2], uniqueName, rPLTablix.FixedRow(num2), num2, tablixFixedHeaderStorage);
					int num3 = 0;
					int numCells = nextRow.NumCells;
					int num4 = numCells;
					if (nextRow.BodyStart == -1)
					{
						int[] omittedIndices = new int[list.Count];
						for (int i = num3; i < num4; i++)
						{
							RPLTablixCell rPLTablixCell = nextRow[i];
							RenderColumnHeaderTablixCell(rPLTablix, uniqueName, num, rPLTablixCell.ColIndex, rPLTablixCell.ColSpan, num2, borderContext, rPLTablixCell, styleContext, tablixFixedHeaderStorage, list, omittedIndices);
							if (array2 != null && num2 < rPLTablix.ColumnHeaderRows)
							{
								string text2 = null;
								if (rPLTablixCell is RPLTablixMemberCell)
								{
									text2 = ((RPLTablixMemberCell)rPLTablixCell).UniqueName;
									if (text2 == null && rPLTablixCell.Element != null)
									{
										text2 = rPLTablixCell.Element.ElementProps.UniqueName;
										((RPLTablixMemberCell)rPLTablixCell).UniqueName = text2;
									}
									if (text2 == null)
									{
										continue;
									}
									for (int j = 0; j < rPLTablixCell.ColSpan; j++)
									{
										string text3 = array2[rPLTablixCell.ColIndex + j];
										text3 = ((text3 != null) ? (text3 + " " + HttpUtility.HtmlAttributeEncode(html5Renderer.m_deviceInfo.HtmlPrefixId) + text2) : (HttpUtility.HtmlAttributeEncode(html5Renderer.m_deviceInfo.HtmlPrefixId) + text2));
										array2[rPLTablixCell.ColIndex + j] = text3;
									}
								}
							}
							nextRow[i] = null;
						}
						list = new List<RPLTablixOmittedRow>();
					}
					else
					{
						if (array != null)
						{
							int headerStart = nextRow.HeaderStart;
							bool flag5 = colsBeforeRowHeaders > 0 && flag2;
							int num5 = 0;
							for (int k = 0; k < array.Length; k++)
							{
								HTMLHeader hTMLHeader = array[k];
								if (array[k] != null)
								{
									if (array[k].ID != null)
									{
										RPLTablixCell rPLTablixCell2 = nextRow[num5 + headerStart];
										hTMLHeader.ID = CalculateRowHeaderId(rPLTablixCell2, rPLTablix.FixedColumns[rPLTablixCell2.ColIndex], uniqueName, num2, k + rPLTablix.ColsBeforeRowHeaders, null, html5Renderer.m_deviceInfo.AccessibleTablix, fixedCornerHeader: false);
										hTMLHeader.Span = rPLTablixCell2.RowSpan;
										num5++;
									}
									if (array[k].Span > 1)
									{
										array[k].Span--;
									}
								}
								else
								{
									hTMLHeader = (array[k] = new HTMLHeader());
								}
							}
						}
						if (list != null && list.Count > 0)
						{
							for (int l = 0; l < list.Count; l++)
							{
								RenderTablixOmittedRow(columns, list[l]);
							}
							list = null;
						}
						List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
						if (colsBeforeRowHeaders > 0)
						{
							int omittedIndex = 0;
							int headerStart2 = nextRow.HeaderStart;
							int bodyStart = nextRow.BodyStart;
							int m = headerStart2;
							int n = bodyStart;
							int num6 = 0;
							for (; n < num4; n++)
							{
								if (num6 >= colsBeforeRowHeaders)
								{
									break;
								}
								RPLTablixCell rPLTablixCell3 = nextRow[n];
								int colSpan = rPLTablixCell3.ColSpan;
								RenderTablixCell(rPLTablix, fixedHeader: false, uniqueName, num, numRows, num6, colSpan, num2, borderContext, rPLTablixCell3, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								num6 += colSpan;
								nextRow[n] = null;
							}
							num4 = ((bodyStart > headerStart2) ? bodyStart : num4);
							if (m >= 0)
							{
								for (; m < num4; m++)
								{
									RPLTablixCell rPLTablixCell4 = nextRow[m];
									int colSpan2 = rPLTablixCell4.ColSpan;
									RenderTablixCell(rPLTablix, flag2, uniqueName, num, numRows, num6, colSpan2, num2, borderContext, rPLTablixCell4, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
									num6 += colSpan2;
									nextRow[m] = null;
								}
							}
							num3 = n;
							num4 = ((bodyStart < headerStart2) ? headerStart2 : numCells);
							for (int num7 = num3; num7 < num4; num7++)
							{
								RPLTablixCell rPLTablixCell5 = nextRow[num7];
								RenderTablixCell(rPLTablix, fixedHeader: false, uniqueName, num, numRows, rPLTablixCell5.ColIndex, rPLTablixCell5.ColSpan, num2, borderContext, rPLTablixCell5, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num7] = null;
							}
						}
						else
						{
							int omittedIndex2 = 0;
							for (int num8 = num3; num8 < num4; num8++)
							{
								RPLTablixCell rPLTablixCell6 = nextRow[num8];
								int colIndex = rPLTablixCell6.ColIndex;
								RenderTablixCell(rPLTablix, rPLTablix.FixedColumns[colIndex], uniqueName, num, numRows, colIndex, rPLTablixCell6.ColSpan, num2, borderContext, rPLTablixCell6, omittedHeaders2, ref omittedIndex2, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num8] = null;
							}
						}
					}
					html5Renderer.WriteStream(HTMLElements.m_closeTR);
					nextRow = rPLTablix.GetNextRow();
					num2++;
				}
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTable);
			if (flag || flag2)
			{
				if (html5Renderer.m_fixedHeaders == null)
				{
					html5Renderer.m_fixedHeaders = new ArrayList();
				}
				html5Renderer.m_fixedHeaders.Add(tablixFixedHeaderStorage);
			}
		}

		private void RenderTablixOmittedRow(int columns, RPLTablixRow currentRow)
		{
			int i = 0;
			List<RPLTablixMemberCell> omittedHeaders;
			for (omittedHeaders = currentRow.OmittedHeaders; i < omittedHeaders.Count && omittedHeaders[i].GroupLabel == null; i++)
			{
			}
			if (i >= omittedHeaders.Count)
			{
				return;
			}
			int num = omittedHeaders[i].ColIndex;
			html5Renderer.WriteStream(HTMLElements.m_openTR);
			html5Renderer.WriteStream(HTMLElements.m_zeroHeight);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			html5Renderer.WriteStream(HTMLElements.m_openTD);
			html5Renderer.WriteStream(HTMLElements.m_colSpan);
			html5Renderer.WriteStream(num.ToString(CultureInfo.InvariantCulture));
			html5Renderer.WriteStream(HTMLElements.m_quote);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			html5Renderer.WriteStream(HTMLElements.m_closeTD);
			for (; i < omittedHeaders.Count; i++)
			{
				if (omittedHeaders[i].GroupLabel != null)
				{
					html5Renderer.WriteStream(HTMLElements.m_openTD);
					int colIndex = omittedHeaders[i].ColIndex;
					int num2 = colIndex - num;
					if (num2 > 1)
					{
						html5Renderer.WriteStream(HTMLElements.m_colSpan);
						html5Renderer.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						html5Renderer.WriteStream(HTMLElements.m_quote);
						html5Renderer.WriteStream(HTMLElements.m_closeBracket);
						html5Renderer.WriteStream(HTMLElements.m_closeTD);
						html5Renderer.WriteStream(HTMLElements.m_openTD);
					}
					int colSpan = omittedHeaders[i].ColSpan;
					if (colSpan > 1)
					{
						html5Renderer.WriteStream(HTMLElements.m_colSpan);
						html5Renderer.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
						html5Renderer.WriteStream(HTMLElements.m_quote);
					}
					html5Renderer.RenderReportItemId(omittedHeaders[i].UniqueName);
					html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					html5Renderer.WriteStream(HTMLElements.m_closeTD);
					num = colIndex + colSpan;
				}
			}
			if (num < columns)
			{
				html5Renderer.WriteStream(HTMLElements.m_openTD);
				html5Renderer.WriteStream(HTMLElements.m_colSpan);
				html5Renderer.WriteStream((columns - num).ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTR);
		}

		protected void RenderSimpleTablixRows(RPLTablix tablix, string tablixID, RPLTablixRow currentRow, int borderContext, TablixFixedHeaderStorage headerStorage)
		{
			int num = 0;
			StyleContext styleContext = new StyleContext();
			float[] rowHeights = tablix.RowHeights;
			int num2 = tablix.ColumnWidths.Length;
			int num3 = rowHeights.Length;
			bool flag = headerStorage.ColumnHeaders != null;
			SharedListLayoutState sharedListLayoutState = SharedListLayoutState.None;
			while (currentRow != null)
			{
				List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
				int omittedIndex = 0;
				if (num2 == 1)
				{
					sharedListLayoutState = SharedListLayoutState.None;
					bool flag2 = tablix.SharedLayoutRow(num);
					bool flag3 = tablix.UseSharedLayoutRow(num);
					bool flag4 = tablix.RowsState.Length > num + 1 && tablix.UseSharedLayoutRow(num + 1);
					if (flag2 && flag4)
					{
						sharedListLayoutState = SharedListLayoutState.Start;
					}
					else if (flag3)
					{
						sharedListLayoutState = ((!flag4) ? SharedListLayoutState.End : SharedListLayoutState.Continue);
					}
				}
				if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.Start)
				{
					if (rowHeights[num] == 0f && num > 1 && currentRow.NumCells == 1 && currentRow[0].Element is RPLRectangle)
					{
						RPLRectangle rPLRectangle = (RPLRectangle)currentRow[0].Element;
						if (rPLRectangle.Children == null || rPLRectangle.Children.Length == 0)
						{
							currentRow = tablix.GetNextRow();
							num++;
							continue;
						}
					}
					html5Renderer.WriteStream(HTMLElements.m_openTR);
					if (tablix.FixedRow(num) || headerStorage.RowHeaders != null || flag)
					{
						string text = tablixID + "tr" + num;
						html5Renderer.RenderReportItemId(text);
						if (tablix.FixedRow(num))
						{
							headerStorage.ColumnHeaders.Add(text);
							if (headerStorage.CornerHeaders != null)
							{
								headerStorage.CornerHeaders.Add(text);
							}
						}
						else if (flag)
						{
							headerStorage.BodyID = text;
							flag = false;
						}
						if (headerStorage.RowHeaders != null)
						{
							headerStorage.RowHeaders.Add(text);
						}
					}
					html5Renderer.WriteStream(HTMLElements.m_valign);
					html5Renderer.WriteStream(HTMLElements.m_topValue);
					html5Renderer.WriteStream(HTMLElements.m_quote);
					html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				}
				int numCells = currentRow.NumCells;
				bool firstRow = num == 0;
				bool lastRow = num == num3 - 1;
				RPLTablixCell rPLTablixCell = currentRow[0];
				currentRow[0] = null;
				if (sharedListLayoutState != 0)
				{
					RenderListReportItem(tablix, rPLTablixCell, omittedHeaders, borderContext, styleContext, firstRow, lastRow, sharedListLayoutState, rPLTablixCell.Element);
				}
				else
				{
					RenderSimpleTablixCellWithHeight(rowHeights[num], tablix, tablixID, num2, num, borderContext, rPLTablixCell, omittedHeaders, ref omittedIndex, styleContext, firstRow, lastRow, headerStorage);
				}
				int i;
				for (i = 1; i < numCells - 1; i++)
				{
					rPLTablixCell = currentRow[i];
					RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref omittedIndex, lastCol: false, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (numCells > 1)
				{
					rPLTablixCell = currentRow[i];
					RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref omittedIndex, lastCol: true, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.End)
				{
					html5Renderer.WriteStream(HTMLElements.m_closeTR);
				}
				currentRow = tablix.GetNextRow();
				num++;
			}
		}

		private void RenderSimpleTablixCellWithHeight(float height, RPLTablix tablix, string tablixID, int numCols, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			int colIndex = cell.ColIndex;
			int colSpan = cell.ColSpan;
			bool lastCol = colIndex + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, colSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
			html5Renderer.WriteStream(HTMLElements.m_openTD);
			RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (colSpan > 1)
			{
				html5Renderer.WriteStream(HTMLElements.m_colSpan);
				html5Renderer.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			html5Renderer.OpenStyle();
			html5Renderer.WriteStream(HTMLElements.m_styleHeight);
			html5Renderer.WriteDStream(height);
			html5Renderer.WriteStream(HTMLElements.m_mm);
			RPLElement element = cell.Element;
			if (element != null)
			{
				html5Renderer.WriteStream(HTMLElements.m_semiColon);
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, firstCol: true, lastCol, firstRow, lastRow, element, ref borderContext);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				RenderReportItem(tablix, tablixContext, cell, styleContext, firstCol: true, lastCol, firstRow, lastRow, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					html5Renderer.WriteStream(HTMLElements.m_displayNone);
				}
				html5Renderer.CloseStyle(renderQuote: true);
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixReportItemStyle(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			styleContext.OmitBordersState = cell.ElementState;
			if (!(cellItem is RPLLine))
			{
				styleContext.StyleOnCell = true;
				borderContext = HTML5Renderer.GetNewContext(tablixContext, firstCol, lastCol, firstRow, lastRow);
				if (rPLTextBox != null)
				{
					bool ignorePadding = styleContext.IgnorePadding;
					styleContext.IgnorePadding = true;
					RPLItemMeasurement rPLItemMeasurement = null;
					if (html5Renderer.m_deviceInfo.OutlookCompat || !html5Renderer.m_deviceInfo.IsBrowserIE)
					{
						rPLItemMeasurement = new RPLItemMeasurement();
						rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					}
					styleContext.EmptyTextBox = (rPLTextBoxPropsDef.IsSimple && string.IsNullOrEmpty(rPLTextBoxProps.Value) && string.IsNullOrEmpty(rPLTextBoxPropsDef.Value) && !html5Renderer.NeedSharedToggleParent(rPLTextBoxProps) && !html5Renderer.CanSort(rPLTextBoxPropsDef));
					string textBoxClass = html5Renderer.GetTextBoxClass(rPLTextBoxPropsDef, rPLTextBoxProps, rPLTextBoxProps.NonSharedStyle, definition.ID + "c");
					bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
					if (HTML5Renderer.IsWritingModeVertical(rPLTextBoxProps.Style) && html5Renderer.m_deviceInfo.IsBrowserIE && (rPLTextBoxPropsDef.CanGrow || (html5Renderer.m_deviceInfo.BrowserMode == BrowserMode.Standards && !html5Renderer.m_deviceInfo.IsBrowserIE6Or7StandardsMode)))
					{
						styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
					}
					html5Renderer.RenderReportItemStyle(cellItem, elementProps, definition, rPLTextBoxProps.NonSharedStyle, rPLTextBoxPropsDef.SharedStyle, rPLItemMeasurement, styleContext, ref borderContext, textBoxClass);
					styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
					styleContext.IgnorePadding = ignorePadding;
				}
				else
				{
					html5Renderer.RenderReportItemStyle(cellItem, elementProps, definition, null, styleContext, ref borderContext, definition.ID + "c");
				}
				styleContext.StyleOnCell = false;
			}
			else if (styleContext.ZeroWidth)
			{
				html5Renderer.WriteStream(HTMLElements.m_displayNone);
			}
			html5Renderer.CloseStyle(renderQuote: true);
			if (styleContext.EmptyTextBox && rPLTextBox != null && elementProps != null)
			{
				html5Renderer.WriteToolTip(elementProps);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
		}

		private void RenderReportItem(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			styleContext.OmitBordersState = cell.ElementState;
			if (styleContext.EmptyTextBox)
			{
				bool flag = false;
				RPLActionInfo actionInfo = rPLTextBoxProps.ActionInfo;
				if (html5Renderer.HasAction(actionInfo))
				{
					html5Renderer.RenderElementHyperlinkAllTextStyles(rPLTextBoxProps.Style, actionInfo.Actions[0], rPLTextBoxPropsDef.ID + "a");
					html5Renderer.WriteStream(HTMLElements.m_openDiv);
					html5Renderer.OpenStyle();
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					rPLItemMeasurement.Height = html5Renderer.GetInnerContainerHeightSubtractBorders(rPLItemMeasurement, rPLTextBoxProps.Style);
					html5Renderer.RenderMeasurementMinHeight(rPLItemMeasurement.Height);
					html5Renderer.WriteStream(HTMLElements.m_semiColon);
					html5Renderer.WriteStream(HTMLElements.m_cursorHand);
					html5Renderer.WriteStream(HTMLElements.m_semiColon);
					html5Renderer.CloseStyle(renderQuote: true);
					html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					flag = true;
				}
				html5Renderer.WriteStream(HTMLElements.m_nbsp);
				if (flag)
				{
					html5Renderer.WriteStream(HTMLElements.m_closeDiv);
					html5Renderer.WriteStream(HTMLElements.m_closeA);
				}
			}
			else
			{
				styleContext.InTablix = true;
				bool renderId = html5Renderer.NeedReportItemId(cellItem, elementProps);
				if (rPLTextBox != null)
				{
					styleContext.RenderMeasurements = false;
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					int borderContext2 = 0;
					new TextBoxRenderer(html5Renderer).RenderReportItem(rPLTextBox, rPLItemMeasurement, styleContext, ref borderContext2, renderId, treatAsTopLevel: false);
				}
				else
				{
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					if (cellItem is RPLRectangle || cellItem is RPLSubReport || cellItem is RPLLine)
					{
						styleContext.RenderMeasurements = false;
					}
					html5Renderer.RenderReportItem(cellItem, elementProps, definition, rPLItemMeasurement, styleContext, borderContext, renderId, treatAsTopLevel: false);
				}
			}
			styleContext.Reset();
		}

		internal void RenderEmptyTopTablixRow(RPLTablix tablix, List<RPLTablixOmittedRow> omittedRows, string tablixID, bool emptyCol, TablixFixedHeaderStorage headerStorage)
		{
			bool flag = headerStorage.RowHeaders != null || headerStorage.ColumnHeaders != null;
			html5Renderer.WriteStream(HTMLElements.m_openTR);
			if (flag)
			{
				string text = tablixID + "r";
				html5Renderer.RenderReportItemId(text);
				if (headerStorage.RowHeaders != null)
				{
					headerStorage.RowHeaders.Add(text);
				}
				if (headerStorage.ColumnHeaders != null)
				{
					headerStorage.ColumnHeaders.Add(text);
				}
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			html5Renderer.WriteStream(HTMLElements.m_zeroHeight);
			html5Renderer.WriteStream(HTMLElements.m_quote);
			html5Renderer.WriteStream(HTMLElements.m_space);
			html5Renderer.WriteStream(HTMLElements.m_ariaHidden);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			if (emptyCol)
			{
				headerStorage.HasEmptyCol = true;
				html5Renderer.WriteStream(HTMLElements.m_openTD);
				if (headerStorage.RowHeaders != null)
				{
					string text2 = tablixID + "e";
					html5Renderer.RenderReportItemId(text2);
					headerStorage.RowHeaders.Add(text2);
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text2);
					}
				}
				html5Renderer.WriteStream(HTMLElements.m_openStyle);
				html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				html5Renderer.WriteStream("0");
				html5Renderer.WriteStream(HTMLElements.m_px);
				html5Renderer.WriteStream(HTMLElements.m_closeQuote);
				html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			int[] array = new int[omittedRows.Count];
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				html5Renderer.WriteStream(HTMLElements.m_openTD);
				if (tablix.FixedColumns[i] && headerStorage.RowHeaders != null)
				{
					string text3 = tablixID + "e" + i;
					html5Renderer.RenderReportItemId(text3);
					headerStorage.RowHeaders.Add(text3);
					if (i == tablix.ColumnWidths.Length - 1 || !tablix.FixedColumns[i + 1])
					{
						headerStorage.LastRowGroupCol = text3;
					}
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text3);
					}
				}
				html5Renderer.WriteStream(HTMLElements.m_openStyle);
				if (tablix.ColumnWidths[i] == 0f)
				{
					html5Renderer.WriteStream(HTMLElements.m_displayNone);
				}
				html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				html5Renderer.WriteDStream(tablix.ColumnWidths[i]);
				html5Renderer.WriteStream(HTMLElements.m_mm);
				html5Renderer.WriteStream(HTMLElements.m_semiColon);
				html5Renderer.WriteStream(HTMLElements.m_styleMinWidth);
				html5Renderer.WriteDStream(tablix.ColumnWidths[i]);
				html5Renderer.WriteStream(HTMLElements.m_mm);
				html5Renderer.WriteStream(HTMLElements.m_closeQuote);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					List<RPLTablixMemberCell> omittedHeaders = omittedRows[j].OmittedHeaders;
					RenderTablixOmittedHeaderCells(omittedHeaders, i, lastCol: false, ref array[j]);
				}
				html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTR);
		}

		private void RenderTablixCell(RPLTablix tablix, bool fixedHeader, string tablixID, int numCols, int numRows, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			bool useElementName = html5Renderer.m_deviceInfo.AccessibleTablix && tablix.RowHeaderColumns > 0 && col >= tablix.ColsBeforeRowHeaders && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders;
			bool fixedCornerHeader = fixedHeader && tablix.FixedColumns[col] && tablix.FixedRow(row);
			string id = CalculateRowHeaderId(cell, fixedHeader, tablixID, cell.RowIndex, cell.ColIndex, headerStorage, useElementName, fixedCornerHeader);
			html5Renderer.WriteStream(HTMLElements.m_openTD);
			if (html5Renderer.m_deviceInfo.AccessibleTablix)
			{
				RenderAccessibleHeaders(tablix, fixedHeader, numCols, cell.ColIndex, colSpan, cell.RowIndex, cell, omittedCells, rowHeaderIds, colHeaderIds, omittedHeaders, ref id);
			}
			if (id != null)
			{
				html5Renderer.RenderReportItemId(id);
			}
			int rowSpan = cell.RowSpan;
			if (cell.RowSpan > 1)
			{
				html5Renderer.WriteStream(HTMLElements.m_rowSpan);
				html5Renderer.WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
				html5Renderer.WriteStream(HTMLElements.m_inlineHeight);
				html5Renderer.WriteStream(Utility.MmToPxAsString(tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			if (colSpan > 1)
			{
				html5Renderer.WriteStream(HTMLElements.m_colSpan);
				html5Renderer.WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
				RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				RenderReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					html5Renderer.OpenStyle();
					html5Renderer.WriteStream(HTMLElements.m_displayNone);
					html5Renderer.CloseStyle(renderQuote: true);
				}
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderColumnHeaderTablixCell(RPLTablix tablix, string tablixID, int numCols, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, List<RPLTablixOmittedRow> omittedRows, int[] omittedIndices)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(col, colSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			html5Renderer.WriteStream(HTMLElements.m_openTD);
			int rowSpan = cell.RowSpan;
			string text = null;
			if (cell is RPLTablixMemberCell && (((RPLTablixMemberCell)cell).GroupLabel != null || html5Renderer.m_deviceInfo.AccessibleTablix))
			{
				text = ((RPLTablixMemberCell)cell).UniqueName;
				if (text == null && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
					((RPLTablixMemberCell)cell).UniqueName = text;
				}
				if (text != null)
				{
					html5Renderer.RenderReportItemId(text);
				}
			}
			if (tablix.FixedColumns[col])
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
					html5Renderer.RenderReportItemId(text);
				}
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			if (rowSpan > 1)
			{
				html5Renderer.WriteStream(HTMLElements.m_rowSpan);
				html5Renderer.WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
				html5Renderer.WriteStream(HTMLElements.m_inlineHeight);
				html5Renderer.WriteStream(Utility.MmToPxAsString(tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			if (colSpan > 1)
			{
				html5Renderer.WriteStream(HTMLElements.m_colSpan);
				html5Renderer.WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, lastRow: false, element, ref borderContext);
				for (int i = 0; i < omittedRows.Count; i++)
				{
					RenderTablixOmittedHeaderCells(omittedRows[i].OmittedHeaders, col, lastCol, ref omittedIndices[i]);
				}
				RenderReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, lastRow: false, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					html5Renderer.OpenStyle();
					html5Renderer.WriteStream(HTMLElements.m_displayNone);
					html5Renderer.CloseStyle(renderQuote: true);
				}
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					RenderTablixOmittedHeaderCells(omittedRows[j].OmittedHeaders, col, lastCol, ref omittedIndices[j]);
				}
				html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private int RenderZeroWidthTDsForTablix(int startIndex, int colSpan, RPLTablix tablix)
		{
			int i;
			for (i = startIndex; i < startIndex + colSpan && tablix.ColumnWidths[i] == 0f; i++)
			{
				html5Renderer.WriteStream(HTMLElements.m_openTD);
				html5Renderer.OpenStyle();
				html5Renderer.WriteStream(HTMLElements.m_displayNone);
				html5Renderer.CloseStyle(renderQuote: true);
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			return i;
		}

		private void RenderSimpleTablixCellID(RPLTablix tablix, string tablixID, int row, TablixFixedHeaderStorage headerStorage, int col)
		{
			if (tablix.FixedColumns[col])
			{
				string text = tablixID + "r" + row + "c" + col;
				html5Renderer.RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null && tablix.FixedRow(row))
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
		}

		private void RenderSimpleTablixCell(RPLTablix tablix, string tablixID, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, bool lastCol, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			StyleContext styleContext = new StyleContext();
			int colIndex = cell.ColIndex;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
			html5Renderer.WriteStream(HTMLElements.m_openTD);
			RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (colSpan > 1)
			{
				html5Renderer.WriteStream(HTMLElements.m_colSpan);
				html5Renderer.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, firstCol: false, lastCol, firstRow, lastRow, element, ref borderContext);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				RenderReportItem(tablix, tablixContext, cell, styleContext, firstCol: false, lastCol, firstRow, lastRow, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					html5Renderer.OpenStyle();
					html5Renderer.WriteStream(HTMLElements.m_displayNone);
					html5Renderer.CloseStyle(renderQuote: true);
				}
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				html5Renderer.WriteStream(HTMLElements.m_nbsp);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private bool InitFixedColumnHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.RowHeights.Length; i++)
			{
				if (tablix.FixedRow(i))
				{
					storage.HtmlId = tablixID;
					storage.ColumnHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private bool InitFixedRowHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				if (tablix.FixedColumns[i])
				{
					storage.HtmlId = tablixID;
					storage.RowHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private string CalculateRowHeaderId(RPLTablixCell cell, bool fixedHeader, string tablixID, int row, int col, TablixFixedHeaderStorage headerStorage, bool useElementName, bool fixedCornerHeader)
		{
			string text = null;
			if (cell is RPLTablixMemberCell)
			{
				if (((RPLTablixMemberCell)cell).GroupLabel != null)
				{
					text = ((RPLTablixMemberCell)cell).UniqueName;
				}
				else if (!fixedHeader && useElementName && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
				}
			}
			if (fixedHeader)
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
				}
				if (headerStorage != null)
				{
					headerStorage.RowHeaders.Add(text);
					if (headerStorage.CornerHeaders != null && fixedCornerHeader)
					{
						headerStorage.CornerHeaders.Add(text);
					}
				}
			}
			return text;
		}

		private void RenderEmptyHeightCell(float height, string tablixID, bool fixedRow, int row, TablixFixedHeaderStorage headerStorage)
		{
			html5Renderer.WriteStream(HTMLElements.m_openTD);
			if (headerStorage.RowHeaders != null)
			{
				string text = tablixID + "h" + row;
				html5Renderer.RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (fixedRow && headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			html5Renderer.WriteStream(HTMLElements.m_openStyle);
			html5Renderer.WriteStream(HTMLElements.m_styleHeight);
			html5Renderer.WriteDStream(height);
			html5Renderer.WriteStream(HTMLElements.m_mm);
			html5Renderer.WriteStream(HTMLElements.m_quote);
			html5Renderer.WriteStream(HTMLElements.m_space);
			html5Renderer.WriteStream(HTMLElements.m_ariaHidden);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			html5Renderer.WriteStream(HTMLElements.m_closeTD);
		}

		protected int GetColSpanMinusZeroWidthColumns(int startColIndex, int colSpan, RPLTablix tablix)
		{
			int num = colSpan;
			for (int i = startColIndex; i < startColIndex + colSpan; i++)
			{
				if (tablix.ColumnWidths[i] == 0f)
				{
					num--;
				}
			}
			return num;
		}

		private void RenderListReportItem(RPLTablix tablix, RPLTablixCell cell, List<RPLTablixMemberCell> omittedHeaders, int tablixContext, StyleContext styleContext, bool firstRow, bool lastRow, SharedListLayoutState layoutState, RPLElement cellItem)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLItemMeasurement rPLItemMeasurement = null;
			rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Width = tablix.ColumnWidths[0];
			rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
			rPLItemMeasurement.State = cell.ElementState;
			bool zeroWidth = styleContext.ZeroWidth;
			styleContext.ZeroWidth = (rPLItemMeasurement.Width == 0f);
			if (layoutState == SharedListLayoutState.Start)
			{
				html5Renderer.WriteStream(HTMLElements.m_openTD);
				if (styleContext.ZeroWidth)
				{
					html5Renderer.OpenStyle();
					html5Renderer.WriteStream(HTMLElements.m_displayNone);
					html5Renderer.CloseStyle(renderQuote: true);
				}
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			}
			if (cellItem is RPLRectangle)
			{
				int num = tablix.ColumnWidths.Length;
				int colIndex = cell.ColIndex;
				int colSpan = cell.ColSpan;
				bool right = colIndex + colSpan == num;
				int newContext = HTML5Renderer.GetNewContext(tablixContext, left: true, right, firstRow, lastRow);
				RenderListRectangle((RPLRectangle)cellItem, omittedHeaders, rPLItemMeasurement, elementProps, definition, layoutState, newContext);
				if (layoutState == SharedListLayoutState.End)
				{
					html5Renderer.WriteStream(HTMLElements.m_closeTD);
				}
			}
			else
			{
				int omittedIndex = 0;
				RenderTablixOmittedHeaderCells(omittedHeaders, 0, lastCol: true, ref omittedIndex);
				styleContext.Reset();
				if (layoutState == SharedListLayoutState.End)
				{
					html5Renderer.WriteStream(HTMLElements.m_closeTD);
				}
			}
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixOmittedHeaderCells(List<RPLTablixMemberCell> omittedHeaders, int colIndex, bool lastCol, ref int omittedIndex)
		{
			if (omittedHeaders == null)
			{
				return;
			}
			while (omittedIndex < omittedHeaders.Count && (omittedHeaders[omittedIndex].ColIndex == colIndex || (lastCol && omittedHeaders[omittedIndex].ColIndex > colIndex)))
			{
				RPLTablixMemberCell rPLTablixMemberCell = omittedHeaders[omittedIndex];
				if (rPLTablixMemberCell.GroupLabel != null)
				{
					html5Renderer.RenderNavigationId(rPLTablixMemberCell.UniqueName);
				}
				omittedIndex++;
			}
		}

		private void RenderListRectangle(RPLContainer rectangle, List<RPLTablixMemberCell> omittedHeaders, RPLItemMeasurement measurement, RPLElementProps props, RPLElementPropsDef def, SharedListLayoutState layoutState, int borderContext)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			html5Renderer.GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, expandLayout: false, layoutState, omittedHeaders, props.Style, treatAsTopLevel: false);
		}

		private void RenderAccessibleHeaders(RPLTablix tablix, bool fixedHeader, int numCols, int col, int colSpan, int row, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders, ref string id)
		{
			int currentLevel = -1;
			if (tablix.RowHeaderColumns == 0 && omittedCells != null && omittedCells.Count > 0)
			{
				foreach (RPLTablixMemberCell omittedCell in omittedCells)
				{
					RPLTablixMemberDef tablixMemberDef = omittedCell.TablixMemberDef;
					if (tablixMemberDef != null && tablixMemberDef.IsStatic && tablixMemberDef.StaticHeadersTree)
					{
						if (id == null && cell.Element != null && cell.Element.ElementProps.UniqueName != null)
						{
							id = cell.Element.ElementProps.UniqueName;
						}
						currentLevel = tablixMemberDef.Level;
						omittedHeaders.Push(tablixMemberDef.Level, col, colSpan, id, numCols);
					}
				}
			}
			if (row < tablix.ColumnHeaderRows || fixedHeader || (col >= tablix.ColsBeforeRowHeaders && tablix.RowHeaderColumns > 0 && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders))
			{
				return;
			}
			bool flag = false;
			string text = colHeaderIds[cell.ColIndex];
			if (!string.IsNullOrEmpty(text))
			{
				html5Renderer.WriteStream(HTMLElements.m_headers);
				html5Renderer.WriteStream(text);
				flag = true;
			}
			foreach (HTMLHeader hTMLHeader in rowHeaderIds)
			{
				string iD = hTMLHeader.ID;
				if (!string.IsNullOrEmpty(iD))
				{
					if (flag)
					{
						html5Renderer.WriteStream(HTMLElements.m_space);
					}
					else
					{
						html5Renderer.WriteStream(HTMLElements.m_headers);
					}
					html5Renderer.WriteAttrEncoded(html5Renderer.m_deviceInfo.HtmlPrefixId);
					html5Renderer.WriteStream(iD);
					flag = true;
				}
			}
			string headers = omittedHeaders.GetHeaders(col, currentLevel, HttpUtility.HtmlAttributeEncode(html5Renderer.m_deviceInfo.HtmlPrefixId));
			if (!string.IsNullOrEmpty(headers))
			{
				if (flag)
				{
					html5Renderer.WriteStream(HTMLElements.m_space);
				}
				else
				{
					html5Renderer.WriteStream(HTMLElements.m_headers);
				}
				html5Renderer.WriteStream(headers);
				flag = true;
			}
			if (flag)
			{
				html5Renderer.WriteStream(HTMLElements.m_quote);
			}
		}
	}
}
