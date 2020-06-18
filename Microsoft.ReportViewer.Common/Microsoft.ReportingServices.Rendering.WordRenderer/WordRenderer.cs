using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal abstract class WordRenderer
	{
		private delegate void RenderSize(float points);

		private class SplitTablix
		{
			private SplitTablixRow[] m_rows;

			internal SplitTablixRow this[int index]
			{
				get
				{
					return m_rows[index];
				}
				set
				{
					m_rows[index] = value;
				}
			}

			internal SplitTablix(int numRows)
			{
				m_rows = new SplitTablixRow[numRows];
			}
		}

		private class SplitTablixRow
		{
			internal TablixGhostCell FirstCell;

			internal List<RPLTablixCell> Cells = new List<RPLTablixCell>();
		}

		protected class TablixGhostCell
		{
			internal int RowSpan;

			internal BorderContext Context;

			internal RPLTablixCell Cell;

			internal int ColSpan;
		}

		internal const int MaximumListLevel = 9;

		protected Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing m_spbProcessing;

		protected IWordWriter m_writer;

		private int m_labelLevel;

		protected bool m_inHeaderFooter;

		protected RPLReport m_rplReport;

		protected DeviceInfo m_deviceInfo;

		private bool m_omitHyperlinks;

		private bool m_omitDrillthroughs;

		protected float m_pageHeight;

		protected bool m_needsToResetTextboxes;

		internal WordRenderer(CreateAndRegisterStream createAndRegisterStream, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, DeviceInfo deviceInfo, string reportName)
		{
			m_spbProcessing = spbProcessing;
			writer.Init(createAndRegisterStream, deviceInfo.AutoFit, reportName);
			m_writer = writer;
			m_omitHyperlinks = deviceInfo.OmitHyperlinks;
			m_omitDrillthroughs = deviceInfo.OmitDrillthroughs;
			m_deviceInfo = deviceInfo;
		}

		internal abstract bool Render();

		protected abstract void RenderTablixCell(RPLTablix tablix, float left, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext, int nextCell, RPLTablixCell cell, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell);

		protected abstract void RenderTextBox(RPLTextBox textBox, RPLItemMeasurement measurement, int cellIndex, float left, BorderContext borderContext, bool inTablix, bool hasBorder);

		protected abstract bool RenderRectangleItemAndLines(RPLContainer rectangle, BorderContext borderContext, int y, PageTableCell cell, string linkToChildId, float runningLeft, bool rowUsed);

		private void RenderReportItem(RPLElement element, RPLItemMeasurement measurement, int cellIndex, float left, BorderContext borderContext, bool inTablix)
		{
			if (element == null)
			{
				return;
			}
			RenderBookmarksLabels(element);
			bool flag = !inTablix && HasAnyBorder(element.ElementProps.Style);
			if (element is RPLTextBox)
			{
				RenderTextBox(element as RPLTextBox, measurement, cellIndex, left, borderContext, inTablix, flag);
			}
			else if (element is RPLLine)
			{
				RPLElementStyle style = element.ElementProps.Style;
				string color = (string)style[0];
				string width = (string)style[10];
				RPLFormat.BorderStyles style2 = (RPLFormat.BorderStyles)style[5];
				bool slant = ((RPLLinePropsDef)element.ElementProps.Definition).Slant;
				m_writer.WriteCellDiagonal(cellIndex, style2, width, color, slant);
				m_writer.WriteEmptyStyle();
			}
			else if (element is RPLTablix)
			{
				if (cellIndex != -1 && inTablix)
				{
					RenderCellProperties(element.ElementProps.Style, cellIndex, inTablix);
					SetZeroPadding(cellIndex);
				}
				RPLTablix rPLTablix = (RPLTablix)element;
				if (rPLTablix.ColumnWidths == null)
				{
					return;
				}
				int num = -1;
				float num2 = left + measurement.Width;
				if (num2 > 558.8f)
				{
					num2 = left;
					for (int i = 0; i < rPLTablix.ColumnWidths.Length; i++)
					{
						num2 += rPLTablix.ColumnWidths[i];
						if (num2 > 558.8f)
						{
							num = i - 1;
							break;
						}
					}
				}
				else if (rPLTablix.ColumnWidths.Length > 63)
				{
					num = 63;
				}
				if (num > -1)
				{
					WriteBeginTableRowCell(measurement, notCanGrow: false);
					RenderCellProperties(element.ElementProps.Style, 0, needsBorderOrPadding: true);
					SetZeroPadding(0);
					m_writer.ApplyCellBorderContext(borderContext);
					RenderSplitTablix(rPLTablix, measurement, num, borderContext);
					WriteEndCellRowTable(borderContext);
				}
				else
				{
					RenderTablix(rPLTablix, 0f, borderContext, inTablix);
				}
			}
			else if (element is RPLSubReport)
			{
				RenderSubreport(element, measurement, cellIndex, borderContext, inTablix, flag);
			}
			else if (element is RPLImage)
			{
				RPLImageProps rPLImageProps = (RPLImageProps)((RPLImage)element).ElementProps;
				RPLFormat.Sizings sizing = ((RPLImagePropsDef)rPLImageProps.Definition).Sizing;
				bool flag2 = sizing == RPLFormat.Sizings.AutoSize && m_writer.AutoFit == AutoFit.True && !inTablix;
				flag = (flag || flag2);
				if (flag)
				{
					m_writer.WriteTableBegin(0f, layoutTable: true);
					float num3 = measurement.Width;
					if (flag2)
					{
						num3 = 1f;
					}
					m_writer.WriteTableRowBegin(0f, measurement.Height, new float[1]
					{
						num3
					});
					m_writer.WriteTableCellBegin(0, 1, firstVertMerge: false, firstHorzMerge: false, vertMerge: false, horzMerge: false);
					m_writer.ApplyCellBorderContext(borderContext);
					cellIndex = 0;
				}
				if (cellIndex != -1)
				{
					RenderCellStyle(element.ElementProps.Style, cellIndex);
				}
				byte[] array = CreateImageBuf(rPLImageProps.Image);
				RemovePaddingFromMeasurement(measurement, element.ElementProps.Style);
				bool num4 = HasAction(rPLImageProps.ActionInfo);
				if (num4)
				{
					if (sizing == RPLFormat.Sizings.AutoSize && array != null)
					{
						m_writer.IgnoreRowHeight(canGrow: true);
					}
					RenderAction(rPLImageProps.ActionInfo.Actions[0]);
				}
				m_writer.AddImage(array, measurement.Height, measurement.Width, sizing);
				if (num4)
				{
					m_writer.WriteHyperlinkEnd();
				}
				if (flag)
				{
					WriteEndCellRowTable(borderContext);
				}
			}
			else if (element is RPLChart || element is RPLGaugePanel || element is RPLMap)
			{
				if (cellIndex != -1)
				{
					RenderCellStyle(element.ElementProps.Style, cellIndex);
				}
				RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)element.ElementProps;
				byte[] array2 = null;
				if (rPLDynamicImageProps.DynamicImageContent != null)
				{
					array2 = new byte[(int)rPLDynamicImageProps.DynamicImageContent.Length];
					rPLDynamicImageProps.DynamicImageContent.Position = 0L;
					rPLDynamicImageProps.DynamicImageContent.Read(array2, 0, (int)rPLDynamicImageProps.DynamicImageContent.Length);
				}
				else if (rPLDynamicImageProps.DynamicImageContentOffset >= 0)
				{
					array2 = m_rplReport.GetImage(rPLDynamicImageProps.DynamicImageContentOffset);
				}
				m_writer.AddImage(array2, measurement.Height, measurement.Width, RPLFormat.Sizings.Fit);
			}
			else if (element is RPLContainer)
			{
				RenderRPLContainer(element, inTablix, measurement, cellIndex, borderContext, flag);
			}
		}

		protected abstract void RenderRPLContainer(RPLElement element, bool inTablix, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool hasBorder);

		protected void RenderRPLContainerProperties(RPLElement element, bool inTablix, int cellIndex)
		{
			if (inTablix && cellIndex > -1)
			{
				RenderCellProperties(element.ElementProps.Style, cellIndex, needsBorderOrPadding: true);
			}
		}

		protected void RenderRPLContainerContents(RPLElement element, RPLItemMeasurement measurement, BorderContext borderContext, bool inTablix, bool hasBorder)
		{
			if (!RenderRectangle(element as RPLContainer, 0f, measurement, borderContext, inTablix) && !inTablix)
			{
				WriteBeginTableRowCell(measurement, notCanGrow: false);
				RenderCellProperties(element.ElementProps.Style, 0, hasBorder);
				m_writer.ApplyCellBorderContext(borderContext);
				m_writer.WriteEmptyStyle();
				WriteEndCellRowTable(borderContext);
			}
		}

		private void RenderSubreport(RPLElement element, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool inTablix, bool hasBorder)
		{
			if (hasBorder)
			{
				WriteBeginTableRowCell(measurement, notCanGrow: false);
				m_writer.ApplyCellBorderContext(borderContext);
				cellIndex = 0;
			}
			if (cellIndex != -1)
			{
				RenderCellProperties(element.ElementProps.Style, cellIndex, hasBorder || inTablix, needsPadding: false, needsWritingMode: false);
			}
			BorderContext parentBorderContext = HasBorders(element.ElementProps.Style, borderContext);
			RPLContainer rPLContainer = element as RPLContainer;
			int num = rPLContainer.Children.Length;
			bool flag = false;
			if (num == 1)
			{
				RPLContainer rectangle = (RPLContainer)rPLContainer.Children[0].Element;
				flag = RenderRectangle(rectangle, 0f, measurement, parentBorderContext, inTablix);
			}
			else
			{
				float width = measurement.Width;
				float num2 = 0f;
				for (int i = 0; i < num; i++)
				{
					RPLItemMeasurement rPLItemMeasurement = rPLContainer.Children[i];
					rPLItemMeasurement.Width = width;
					rPLItemMeasurement.Top = num2;
					num2 += rPLItemMeasurement.Height;
				}
				measurement.Height = num2;
				flag = RenderRectangle(rPLContainer, 0f, measurement, parentBorderContext, inTablix: false);
			}
			if (hasBorder)
			{
				WriteEndCellRowTable(borderContext);
			}
			else if (!flag && !inTablix)
			{
				WriteBeginTableRowCell(measurement, notCanGrow: false);
				RenderCellProperties(element.ElementProps.Style, 0, hasBorder);
				m_writer.ApplyCellBorderContext(borderContext);
				m_writer.WriteEmptyStyle();
				WriteEndCellRowTable(borderContext);
			}
		}

		private static void RenderSizeProp(RPLReportSize nonShared, RPLReportSize shared, RenderSize sizeFunction)
		{
			double num = 0.0;
			if (nonShared == null)
			{
				if (shared == null)
				{
					return;
				}
				num = shared.ToPoints();
			}
			else
			{
				num = nonShared.ToPoints();
			}
			if (num > 0.0)
			{
				sizeFunction((float)num);
			}
		}

		protected void RenderTextBox(RPLTextBox textBox, bool inTablix, int cellIndex, bool needsTable, RPLElementStyle style, RPLItemMeasurement measurement, bool notCanGrow, RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, bool isSimple, string textBoxValue, BorderContext borderContext, int oldCellIndex)
		{
			if (needsTable)
			{
				if (inTablix)
				{
					RemovePaddingFromMeasurement(measurement, style);
				}
				WriteBeginTableRowCell(measurement, notCanGrow);
				m_writer.ApplyCellBorderContext(borderContext);
				cellIndex = 0;
			}
			if (textBoxPropsDef.CanGrow && textBoxPropsDef.CanShrink)
			{
				m_writer.IgnoreRowHeight(canGrow: true);
			}
			TypeCode typeCode = textBoxProps.TypeCode;
			ArrayList arrayList = null;
			if (m_inHeaderFooter && isSimple)
			{
				string text = textBoxPropsDef.Formula;
				if (!string.IsNullOrEmpty(text))
				{
					if (text.StartsWith("=", StringComparison.Ordinal))
					{
						text = text.Remove(0, 1);
					}
					arrayList = FormulaHandler.ProcessHeaderFooterFormula(text);
				}
			}
			bool flag = HasAction(textBoxProps.ActionInfo);
			RPLAction rPLAction = null;
			if (flag)
			{
				rPLAction = textBoxProps.ActionInfo.Actions[0];
				RenderAction(rPLAction);
			}
			if (arrayList != null)
			{
				for (int i = 0; i < arrayList.Count; i++)
				{
					RenderTextProperties(typeCode, style);
					RenderFormulaString(arrayList[i]);
				}
			}
			else if (isSimple)
			{
				if (!string.IsNullOrEmpty(textBoxValue))
				{
					RPLFormat.Directions directions = RPLFormat.Directions.LTR;
					object obj = style[29];
					if (obj != null)
					{
						directions = (RPLFormat.Directions)obj;
					}
					m_writer.AddTextStyleProp(29, directions);
					object obj2 = style[25];
					if (obj2 != null)
					{
						m_writer.RenderTextAlign(typeCode, (RPLFormat.TextAlignments)obj2, directions);
					}
					RenderTextRunStyle(style, directions);
					m_writer.WriteText(textBoxValue);
				}
			}
			else
			{
				RenderTextBoxRich(textBox, rPLAction);
			}
			if (flag)
			{
				m_writer.WriteHyperlinkEnd();
			}
			if (needsTable)
			{
				RenderCellProperties(style, cellIndex, !inTablix, !inTablix, needsWritingMode: true);
				WriteEndCellRowTable(borderContext);
				cellIndex = oldCellIndex;
			}
		}

		protected void RenderTextBoxProperties(bool inTablix, int cellIndex, bool needsTable, RPLElementStyle style)
		{
			if (inTablix || !needsTable)
			{
				RenderCellProperties(style, cellIndex, needsBorder: true, needsPadding: true, !needsTable);
			}
		}

		protected RPLTextBoxProps GetTextBoxProperties(RPLTextBox textBox, out RPLTextBoxPropsDef textBoxPropsDef, out bool isSimple, out string textBoxValue, bool inTablix, out bool notCanGrow, bool hasBorder, int cellIndex, out bool needsTable, out RPLElementStyle style, out int oldCellIndex)
		{
			RPLTextBoxProps rPLTextBoxProps = textBox.ElementProps as RPLTextBoxProps;
			textBoxPropsDef = (rPLTextBoxProps.Definition as RPLTextBoxPropsDef);
			isSimple = textBoxPropsDef.IsSimple;
			textBoxValue = null;
			if (isSimple)
			{
				textBoxValue = rPLTextBoxProps.Value;
				if (string.IsNullOrEmpty(textBoxValue))
				{
					textBoxValue = textBoxPropsDef.Value;
				}
			}
			notCanGrow = ((!textBoxPropsDef.CanGrow & isSimple) && !string.IsNullOrEmpty(textBoxValue));
			needsTable = ((!inTablix | notCanGrow) || hasBorder);
			oldCellIndex = cellIndex;
			style = textBox.ElementProps.Style;
			return rPLTextBoxProps;
		}

		private void RenderTextBoxRich(RPLTextBox textBox, RPLAction textBoxAction)
		{
			RPLFormat.Directions directions = (RPLFormat.Directions)(textBox.ElementProps as RPLTextBoxProps).Style[29];
			RPLParagraph nextParagraph = textBox.GetNextParagraph();
			RPLTextRunProps rPLTextRunProps = null;
			RPLTextRunPropsDef rPLTextRunPropsDef = null;
			m_writer.ResetListlevels();
			bool flag = directions == RPLFormat.Directions.RTL;
			Queue<RPLParagraph> queue = null;
			Queue<RPLTextRun> queue2 = null;
			if (m_needsToResetTextboxes)
			{
				queue = new Queue<RPLParagraph>();
				queue2 = new Queue<RPLTextRun>();
			}
			while (nextParagraph != null)
			{
				RPLParagraphProps obj = nextParagraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = obj.Definition as RPLParagraphPropsDef;
				object obj2 = obj.Style[25];
				if (obj2 != null)
				{
					m_writer.RenderTextAlign(TypeCode.String, (RPLFormat.TextAlignments)obj2, directions);
				}
				m_writer.AddTextStyleProp(29, directions);
				RPLReportSize hangingIndent = obj.HangingIndent;
				if (hangingIndent == null)
				{
					hangingIndent = rPLParagraphPropsDef.HangingIndent;
				}
				RPLReportSize rPLReportSize = obj.LeftIndent;
				if (rPLReportSize == null)
				{
					rPLReportSize = rPLParagraphPropsDef.LeftIndent;
				}
				RPLReportSize rPLReportSize2 = obj.RightIndent;
				if (rPLReportSize2 == null)
				{
					rPLReportSize2 = rPLParagraphPropsDef.RightIndent;
				}
				if (flag)
				{
					RPLReportSize rPLReportSize3 = rPLReportSize2;
					rPLReportSize2 = rPLReportSize;
					rPLReportSize = rPLReportSize3;
				}
				double num = 0.0;
				if (rPLReportSize != null)
				{
					num = rPLReportSize.ToPoints();
				}
				double num2 = 0.0;
				if (hangingIndent != null)
				{
					num2 = hangingIndent.ToPoints();
				}
				int num3 = obj.ListLevel ?? rPLParagraphPropsDef.ListLevel;
				RPLFormat.ListStyles listStyle = obj.ListStyle ?? rPLParagraphPropsDef.ListStyle;
				if (num3 > 9)
				{
					num3 = 9;
				}
				if (num3 > 0)
				{
					num += (double)(36 * (num3 - 1) + 18);
					num2 -= 18.0;
				}
				if (num2 < 0.0)
				{
					num -= num2;
				}
				if (num2 != 0.0)
				{
					m_writer.AddFirstLineIndent((float)num2);
				}
				if (num > 0.0)
				{
					m_writer.AddLeftIndent((float)num);
				}
				if (rPLReportSize2 != null)
				{
					m_writer.AddRightIndent((float)rPLReportSize2.ToMillimeters());
				}
				RenderSizeProp(obj.SpaceAfter, rPLParagraphPropsDef.SpaceAfter, m_writer.AddSpaceAfter);
				RenderSizeProp(obj.SpaceBefore, rPLParagraphPropsDef.SpaceBefore, m_writer.AddSpaceBefore);
				for (RPLTextRun nextTextRun = nextParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = nextParagraph.GetNextTextRun())
				{
					rPLTextRunProps = (nextTextRun.ElementProps as RPLTextRunProps);
					bool flag2 = HasAction(rPLTextRunProps.ActionInfo);
					if (flag2)
					{
						if (textBoxAction != null)
						{
							m_writer.WriteHyperlinkEnd();
						}
						RPLAction action = rPLTextRunProps.ActionInfo.Actions[0];
						RenderAction(action);
					}
					ArrayList arrayList = null;
					if (m_inHeaderFooter)
					{
						rPLTextRunPropsDef = (rPLTextRunProps.Definition as RPLTextRunPropsDef);
						string text = rPLTextRunPropsDef.Formula;
						if (!string.IsNullOrEmpty(text) && rPLTextRunPropsDef.Markup != RPLFormat.MarkupStyles.HTML && rPLTextRunProps.Markup != RPLFormat.MarkupStyles.HTML)
						{
							if (text.StartsWith("=", StringComparison.Ordinal))
							{
								text = text.Remove(0, 1);
							}
							arrayList = FormulaHandler.ProcessHeaderFooterFormula(text);
						}
					}
					if (arrayList != null)
					{
						for (int i = 0; i < arrayList.Count; i++)
						{
							RenderTextRunStyle(rPLTextRunProps.Style, directions);
							RenderFormulaString(arrayList[i]);
						}
					}
					else
					{
						string value = rPLTextRunProps.Value;
						if (value == null)
						{
							value = (rPLTextRunProps.Definition as RPLTextRunPropsDef).Value;
						}
						if (!string.IsNullOrEmpty(value))
						{
							RenderTextRunStyle(rPLTextRunProps.Style, directions);
							m_writer.WriteText(value);
						}
					}
					if (flag2)
					{
						m_writer.WriteHyperlinkEnd();
						if (textBoxAction != null)
						{
							RenderAction(textBoxAction);
						}
					}
					if (m_needsToResetTextboxes)
					{
						queue2.Enqueue(nextTextRun);
					}
				}
				if (m_needsToResetTextboxes)
				{
					nextParagraph.TextRuns = queue2;
					queue2 = new Queue<RPLTextRun>();
					queue.Enqueue(nextParagraph);
				}
				nextParagraph = textBox.GetNextParagraph();
				if (num3 > 0)
				{
					m_writer.WriteListEnd(num3, listStyle, nextParagraph != null);
				}
				else if (nextParagraph != null)
				{
					m_writer.WriteParagraphEnd();
					m_writer.ResetListlevels();
				}
			}
			if (m_needsToResetTextboxes)
			{
				textBox.Paragraphs = queue;
			}
		}

		public void RenderTextRunStyle(RPLElementStyle runStyle, RPLFormat.Directions dir)
		{
			object obj = runStyle[22];
			if (obj != null)
			{
				m_writer.RenderFontWeight((RPLFormat.FontWeights)obj, dir);
			}
			obj = runStyle[19];
			if (obj != null)
			{
				m_writer.RenderFontStyle((RPLFormat.FontStyles)obj, dir);
			}
			obj = runStyle[32];
			if (obj != null)
			{
				m_writer.AddTextStyleProp(32, obj);
			}
			if (dir != 0)
			{
				m_writer.RenderTextRunDirection(dir);
			}
			m_writer.AddTextStyleProp(24, runStyle[24]);
			m_writer.AddTextStyleProp(27, runStyle[27]);
			string text = runStyle[20] as string;
			if (text != null)
			{
				m_writer.RenderFontFamily(text, dir);
			}
			text = (runStyle[21] as string);
			if (text != null)
			{
				m_writer.RenderFontSize(text, dir);
			}
		}

		public void RemovePaddingFromMeasurement(RPLItemMeasurement measurement, RPLElementStyle style)
		{
			double num = ToMM(style[15], null);
			double num2 = ToMM(style[16], null);
			measurement.Width = (float)((double)measurement.Width - (num + num2));
			double num3 = ToMM(style[17], null);
			double num4 = ToMM(style[18], null);
			measurement.Height = (float)((double)measurement.Height - (num3 + num4));
		}

		public bool HasAnyBorder(RPLElementStyle style)
		{
			if (!HasBorder(style, Positions.Top) && !HasBorder(style, Positions.Bottom) && !HasBorder(style, Positions.Left))
			{
				return HasBorder(style, Positions.Right);
			}
			return true;
		}

		public BorderContext HasBorders(RPLElementStyle style, BorderContext parentBorderContext)
		{
			return new BorderContext
			{
				Top = (parentBorderContext.Top || HasBorder(style, Positions.Top)),
				Left = (parentBorderContext.Left || HasBorder(style, Positions.Left)),
				Bottom = (parentBorderContext.Bottom || HasBorder(style, Positions.Bottom)),
				Right = (parentBorderContext.Right || HasBorder(style, Positions.Right))
			};
		}

		public bool HasBorder(RPLElementStyle style, Positions pos)
		{
			object defaultSize = style[10];
			object obj = style[5];
			object size = null;
			object obj2 = null;
			switch (pos)
			{
			case Positions.Top:
				size = style[13];
				obj2 = style[8];
				break;
			case Positions.Bottom:
				size = style[14];
				obj2 = style[9];
				break;
			case Positions.Right:
				size = style[12];
				obj2 = style[7];
				break;
			case Positions.Left:
				size = style[11];
				obj2 = style[6];
				break;
			}
			if (obj2 != null)
			{
				if ((RPLFormat.BorderStyles)obj2 == RPLFormat.BorderStyles.None)
				{
					return false;
				}
			}
			else if (obj != null && (RPLFormat.BorderStyles)obj == RPLFormat.BorderStyles.None)
			{
				return false;
			}
			return ToMM(size, defaultSize) > 0.0;
		}

		public static bool IsWritingModeVertical(IRPLStyle style)
		{
			object obj = style[30];
			if (obj != null)
			{
				RPLFormat.WritingModes writingModes = (RPLFormat.WritingModes)obj;
				if (writingModes == RPLFormat.WritingModes.Vertical || writingModes == RPLFormat.WritingModes.Rotate270)
				{
					return true;
				}
			}
			return false;
		}

		public double ToMM(object size, object defaultSize)
		{
			if (size == null)
			{
				if (defaultSize == null)
				{
					return 0.0;
				}
				size = defaultSize;
			}
			return new RPLReportSize(size as string).ToMillimeters();
		}

		private void RenderAction(RPLAction action)
		{
			string text = action.Hyperlink;
			bool bookmarkLink = false;
			if (text == null || m_omitHyperlinks)
			{
				text = action.DrillthroughUrl;
				if (text == null || m_omitDrillthroughs)
				{
					text = action.BookmarkLink;
					bookmarkLink = (text != null);
				}
			}
			if (text != null)
			{
				m_writer.WriteHyperlinkBegin(text, bookmarkLink);
			}
		}

		private void RenderFormulaString(object obj)
		{
			if (obj is string)
			{
				m_writer.WriteText((string)obj);
			}
			else if (obj is FormulaHandler.GlobalExpressionType)
			{
				switch ((FormulaHandler.GlobalExpressionType)obj)
				{
				case FormulaHandler.GlobalExpressionType.PageNumber:
					m_writer.WritePageNumberField();
					break;
				case FormulaHandler.GlobalExpressionType.ReportName:
					m_writer.WriteText(m_rplReport.ReportName);
					break;
				case FormulaHandler.GlobalExpressionType.TotalPages:
					m_writer.WriteTotalPagesField();
					break;
				}
			}
		}

		private void RenderBookmarksLabels(RPLElement element)
		{
			RPLItem obj = element as RPLItem;
			RPLItemProps rPLItemProps = obj.ElementProps as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			if (!(obj is RPLRectangle) || (rPLItemPropsDef as RPLRectanglePropsDef).LinkToChildId == null)
			{
				if (!string.IsNullOrEmpty(rPLItemProps.Bookmark))
				{
					m_writer.RenderBookmark(rPLItemProps.Bookmark);
				}
				else if (!string.IsNullOrEmpty(rPLItemPropsDef.Bookmark))
				{
					m_writer.RenderBookmark(rPLItemPropsDef.Bookmark);
				}
				if (!string.IsNullOrEmpty(rPLItemProps.Label))
				{
					m_writer.RenderLabel(rPLItemProps.Label, m_labelLevel);
				}
				else if (!string.IsNullOrEmpty(rPLItemPropsDef.Label))
				{
					m_writer.RenderLabel(rPLItemPropsDef.Label, m_labelLevel);
				}
			}
		}

		private void RenderTextProperties(TypeCode typeCode, IRPLStyle style)
		{
			m_writer.AddTextStyleProp(23, style[23]);
			m_writer.AddTextStyleProp(24, style[24]);
			m_writer.AddTextStyleProp(27, style[27]);
			m_writer.AddTextStyleProp(28, style[28]);
			m_writer.AddTextStyleProp(31, style[31]);
			m_writer.AddTextStyleProp(32, style[32]);
			object obj = style[29];
			RPLFormat.Directions directions = (obj != null) ? ((RPLFormat.Directions)obj) : RPLFormat.Directions.LTR;
			m_writer.AddTextStyleProp(29, obj);
			object obj2 = style[25];
			if (obj2 != null)
			{
				m_writer.RenderTextAlign(typeCode, (RPLFormat.TextAlignments)obj2, directions);
			}
			obj2 = style[21];
			if (obj2 != null)
			{
				m_writer.RenderFontSize(obj2 as string, directions);
			}
			obj2 = style[19];
			if (obj2 != null)
			{
				m_writer.RenderFontStyle((RPLFormat.FontStyles)obj2, directions);
			}
			obj2 = style[22];
			if (obj2 != null)
			{
				m_writer.RenderFontWeight((RPLFormat.FontWeights)obj2, directions);
			}
			obj2 = style[20];
			if (obj2 != null)
			{
				m_writer.RenderFontFamily(obj2 as string, directions);
			}
		}

		private void RenderCellProperties(IRPLStyle style, int cellIndex, bool needsBorderOrPadding)
		{
			RenderCellProperties(style, cellIndex, needsBorderOrPadding, needsBorderOrPadding, needsWritingMode: false);
		}

		private void RenderCellProperties(IRPLStyle style, int cellIndex, bool needsBorder, bool needsPadding, bool needsWritingMode)
		{
			if (needsBorder)
			{
				m_writer.AddCellStyleProp(cellIndex, 0, style[0]);
				m_writer.AddCellStyleProp(cellIndex, 1, style[1]);
				m_writer.AddCellStyleProp(cellIndex, 2, style[2]);
				m_writer.AddCellStyleProp(cellIndex, 3, style[3]);
				m_writer.AddCellStyleProp(cellIndex, 4, style[4]);
				m_writer.AddCellStyleProp(cellIndex, 5, style[5]);
				m_writer.AddCellStyleProp(cellIndex, 6, style[6]);
				m_writer.AddCellStyleProp(cellIndex, 7, style[7]);
				m_writer.AddCellStyleProp(cellIndex, 8, style[8]);
				m_writer.AddCellStyleProp(cellIndex, 9, style[9]);
				m_writer.AddCellStyleProp(cellIndex, 10, style[10]);
				m_writer.AddCellStyleProp(cellIndex, 11, style[11]);
				m_writer.AddCellStyleProp(cellIndex, 12, style[12]);
				m_writer.AddCellStyleProp(cellIndex, 13, style[13]);
				m_writer.AddCellStyleProp(cellIndex, 14, style[14]);
			}
			if (needsPadding)
			{
				m_writer.AddPadding(cellIndex, 18, style[18], 0);
				m_writer.AddPadding(cellIndex, 15, style[15], 0);
				m_writer.AddPadding(cellIndex, 16, style[16], 0);
				m_writer.AddPadding(cellIndex, 17, style[17], 0);
			}
			else
			{
				SetZeroPadding(cellIndex);
			}
			m_writer.AddCellStyleProp(cellIndex, 26, style[26]);
			if (needsWritingMode)
			{
				m_writer.AddCellStyleProp(cellIndex, 30, style[30]);
			}
			m_writer.AddCellStyleProp(cellIndex, 33, style[33]);
			m_writer.AddCellStyleProp(cellIndex, 34, style[34]);
		}

		private void RenderTableProperties(IRPLStyle style, bool isTablix, BorderContext parentBorderContext)
		{
			if (isTablix)
			{
				m_writer.AddTableStyleProp(0, style[0]);
				m_writer.AddTableStyleProp(1, style[1]);
				m_writer.AddTableStyleProp(2, style[2]);
				m_writer.AddTableStyleProp(3, style[3]);
				m_writer.AddTableStyleProp(4, style[4]);
				m_writer.AddTableStyleProp(5, style[5]);
				m_writer.AddTableStyleProp(6, style[6]);
				m_writer.AddTableStyleProp(7, style[7]);
				m_writer.AddTableStyleProp(8, style[8]);
				m_writer.AddTableStyleProp(9, style[9]);
				m_writer.AddTableStyleProp(10, style[10]);
				m_writer.AddTableStyleProp(11, style[11]);
				m_writer.AddTableStyleProp(12, style[12]);
				m_writer.AddTableStyleProp(13, style[13]);
				m_writer.AddTableStyleProp(14, style[14]);
				m_writer.SetTableContext(parentBorderContext);
			}
			m_writer.AddTableStyleProp(26, style[26]);
			m_writer.AddTableStyleProp(30, style[30]);
			m_writer.AddTableStyleProp(33, style[33]);
			m_writer.AddTableStyleProp(34, style[34]);
		}

		private void SetZeroPadding(int cellIndex)
		{
			m_writer.AddPadding(cellIndex, 18, null, 0);
			m_writer.AddPadding(cellIndex, 15, null, 0);
			m_writer.AddPadding(cellIndex, 16, null, 0);
			m_writer.AddPadding(cellIndex, 17, null, 0);
		}

		private void RenderCellStyle(IRPLStyle style, int cellIndex)
		{
			RenderCellProperties(style, cellIndex, needsBorder: true, needsPadding: true, needsWritingMode: false);
		}

		private void RenderSplitTablix(RPLTablix tablix, RPLItemMeasurement measurement, int splitColumn, BorderContext parentBorderContext)
		{
			float[] columnWidths = tablix.ColumnWidths;
			int totalColumns = columnWidths.Length;
			int totalRows = tablix.RowHeights.Length;
			int num = (int)Math.Ceiling((float)columnWidths.Length / (float)splitColumn);
			List<RPLTablixMemberCell> list = new List<RPLTablixMemberCell>();
			SplitTablix[] array = new SplitTablix[num];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SplitTablix(tablix.RowHeights.Length);
			}
			List<RPLTablixMemberCell>[] array2 = new List<RPLTablixMemberCell>[tablix.RowHeights.Length];
			RPLTablixRow nextRow = tablix.GetNextRow();
			int num2 = 0;
			while (nextRow != null)
			{
				if (nextRow is RPLTablixOmittedRow)
				{
					List<RPLTablixMemberCell> omittedHeaders = nextRow.OmittedHeaders;
					for (int j = 0; j < omittedHeaders.Count; j++)
					{
						if (!string.IsNullOrEmpty(omittedHeaders[j].GroupLabel))
						{
							m_writer.RenderLabel(omittedHeaders[j].GroupLabel, m_labelLevel);
						}
					}
					nextRow = tablix.GetNextRow();
					continue;
				}
				if (nextRow.NumCells != 0)
				{
					for (int k = 0; k < array.Length; k++)
					{
						array[k][num2] = new SplitTablixRow();
					}
					array2[num2] = nextRow.OmittedHeaders;
					if (tablix.ColsBeforeRowHeaders > 0 && nextRow.BodyStart != -1)
					{
						int num3 = 0;
						int colsBeforeRowHeaders = tablix.ColsBeforeRowHeaders;
						int l;
						for (l = nextRow.BodyStart; l < nextRow.NumCells; l++)
						{
							if (num3 >= colsBeforeRowHeaders)
							{
								break;
							}
							RPLTablixCell cell = nextRow[l];
							PlaceCellIntoSplitTablix(array, num2, cell, totalColumns, totalRows, splitColumn, parentBorderContext);
						}
						int num4 = (nextRow.BodyStart > nextRow.HeaderStart) ? nextRow.BodyStart : nextRow.NumCells;
						for (int m = nextRow.HeaderStart; m < num4; m++)
						{
							RPLTablixCell cell2 = nextRow[m];
							PlaceCellIntoSplitTablix(array, num2, cell2, totalColumns, totalRows, splitColumn, parentBorderContext);
						}
						num4 = ((nextRow.BodyStart < nextRow.HeaderStart) ? nextRow.HeaderStart : nextRow.NumCells);
						for (int n = l; n < num4; n++)
						{
							RPLTablixCell cell3 = nextRow[n];
							PlaceCellIntoSplitTablix(array, num2, cell3, totalColumns, totalRows, splitColumn, parentBorderContext);
						}
					}
					else
					{
						for (int num5 = 0; num5 < nextRow.NumCells; num5++)
						{
							RPLTablixCell cell4 = nextRow[num5];
							PlaceCellIntoSplitTablix(array, num2, cell4, totalColumns, totalRows, splitColumn, parentBorderContext);
						}
					}
				}
				nextRow = tablix.GetNextRow();
				num2++;
			}
			float[] array3 = new float[num];
			for (int num6 = 0; num6 < columnWidths.Length; num6++)
			{
				array3[num6 / splitColumn] += columnWidths[num6];
			}
			BorderContext borderContext = new BorderContext();
			m_writer.WriteTableBegin(0f, layoutTable: false);
			m_writer.WriteTableRowBegin(0f, measurement.Height, array3);
			BorderContext borderContext2 = HasBorders(tablix.ElementProps.Style, parentBorderContext);
			int num7 = columnWidths.Length % splitColumn;
			for (int num8 = 0; num8 < num; num8++)
			{
				m_writer.WriteTableCellBegin(num8, num, firstVertMerge: false, firstHorzMerge: false, vertMerge: false, horzMerge: false);
				m_writer.WriteTableBegin(0f, layoutTable: false);
				SplitTablix splitTablix = array[num8];
				int num9 = (num7 > 0 && num8 == num - 1) ? num7 : splitColumn;
				float[] array4 = new float[num9];
				Array.Copy(columnWidths, num8 * splitColumn, array4, 0, num9);
				TablixGhostCell[] array5 = new TablixGhostCell[array4.Length];
				for (int num10 = 0; num10 < tablix.RowHeights.Length; num10++)
				{
					m_writer.WriteTableRowBegin(0f, tablix.RowHeights[num10], array4);
					SplitTablixRow splitTablixRow = splitTablix[num10];
					int num11 = 0;
					if (splitTablixRow.FirstCell != null)
					{
						TablixGhostCell firstCell = splitTablixRow.FirstCell;
						for (int num12 = 0; num12 < firstCell.ColSpan; num12++)
						{
							m_writer.WriteTableCellBegin(num11 + num12, num9, firstCell.RowSpan > 1, firstCell.ColSpan > 1 && num12 == 0, vertMerge: false, num12 > 0);
							m_writer.ApplyCellBorderContext(firstCell.Context);
							if (firstCell.Cell.Element != null)
							{
								IRPLStyle style = firstCell.Cell.Element.ElementProps.Style;
								RenderCellStyle(style, num11 + num12);
							}
							m_writer.WriteTableCellEnd(num11 + num12, firstCell.Context, emptyLayoutCell: false);
						}
						num11 += firstCell.ColSpan;
						firstCell.RowSpan--;
						if (firstCell.RowSpan > 0)
						{
							array5[0] = firstCell;
						}
					}
					List<RPLTablixMemberCell>.Enumerator enumerator = list.GetEnumerator();
					if (array2[num10] != null)
					{
						enumerator = array2[num10].GetEnumerator();
					}
					enumerator.MoveNext();
					if (splitTablixRow.Cells.Count == 0)
					{
						RenderGhostCells(array4, array5, num11, num9);
					}
					else
					{
						for (int num13 = 0; num13 < splitTablixRow.Cells.Count; num13++)
						{
							RPLTablixCell rPLTablixCell = splitTablixRow.Cells[num13];
							borderContext.Top = (rPLTablixCell.RowIndex == 0 && borderContext2.Top);
							borderContext.Left = (rPLTablixCell.ColIndex == 0 && borderContext2.Left);
							borderContext.Bottom = (rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == tablix.RowHeights.Length && borderContext2.Bottom);
							borderContext.Right = (rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == tablix.ColumnWidths.Length && borderContext2.Right);
							RenderTablixCell(tablix, 0f, array4, array5, borderContext, num11, rPLTablixCell, enumerator, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == tablix.ColumnWidths.Length);
							num11 = rPLTablixCell.ColIndex + rPLTablixCell.ColSpan;
						}
						if (num11 != num9)
						{
							RenderGhostCells(array4, array5, num11, num9);
						}
						while (enumerator.Current != null)
						{
							if (!string.IsNullOrEmpty(enumerator.Current.GroupLabel))
							{
								m_writer.RenderLabel(enumerator.Current.GroupLabel, m_labelLevel);
							}
							enumerator.MoveNext();
						}
					}
					m_writer.WriteTableRowEnd();
				}
				m_writer.WriteTableEnd();
				m_writer.WriteTableCellEnd(num8, new BorderContext(), emptyLayoutCell: false);
			}
			m_writer.WriteTableRowEnd();
			m_writer.WriteTableEnd();
		}

		private static void PlaceCellIntoSplitTablix(SplitTablix[] tablices, int x, RPLTablixCell cell, int totalColumns, int totalRows, int splitColumn, BorderContext parentBorderContext)
		{
			int num = cell.ColIndex / splitColumn;
			tablices[num][x].Cells.Add(cell);
			int num2 = cell.ColIndex + cell.ColSpan;
			int i = num + 1;
			if (num2 > i * splitColumn)
			{
				cell.ColSpan = i * splitColumn - cell.ColIndex;
				for (; num2 > i * splitColumn; i++)
				{
					int colSpan = Math.Min(num2 - i * splitColumn, splitColumn);
					TablixGhostCell tablixGhostCell = new TablixGhostCell();
					tablixGhostCell.Cell = cell;
					tablixGhostCell.ColSpan = colSpan;
					tablixGhostCell.RowSpan = cell.RowSpan;
					tablixGhostCell.Context = new BorderContext();
					tablixGhostCell.Context.Top = (x == 0);
					tablixGhostCell.Context.Left = (num == 0 && cell.ColIndex == 0);
					tablixGhostCell.Context.Bottom = (x == totalRows - 1);
					tablixGhostCell.Context.Right = (num == tablices.Length - 1 && num2 == totalColumns - 1);
					tablices[i][x].FirstCell = tablixGhostCell;
				}
			}
			cell.ColIndex -= num * splitColumn;
		}

		private void RenderTablix(RPLTablix element, float left, BorderContext parentBorderContext, bool inTablix)
		{
			float[] columnWidths = element.ColumnWidths;
			TablixGhostCell[] ghostCells = new TablixGhostCell[columnWidths.Length];
			m_writer.WriteTableBegin(left, layoutTable: false);
			RPLElementStyle style = element.ElementProps.Style;
			BorderContext borderContext = HasBorders(style, parentBorderContext);
			if (inTablix)
			{
				parentBorderContext = borderContext;
			}
			RenderTablixStyle(style, parentBorderContext);
			RPLTablixRow nextRow = element.GetNextRow();
			BorderContext borderContext2 = new BorderContext();
			List<RPLTablixMemberCell> list = new List<RPLTablixMemberCell>();
			int num = 0;
			while (nextRow != null)
			{
				if (nextRow is RPLTablixOmittedRow)
				{
					List<RPLTablixMemberCell> omittedHeaders = nextRow.OmittedHeaders;
					for (int i = 0; i < omittedHeaders.Count; i++)
					{
						if (!string.IsNullOrEmpty(omittedHeaders[i].GroupLabel))
						{
							m_writer.RenderLabel(omittedHeaders[i].GroupLabel, m_labelLevel);
						}
					}
					nextRow = element.GetNextRow();
					continue;
				}
				m_writer.WriteTableRowBegin(left, element.RowHeights[num], columnWidths);
				if (nextRow.NumCells == 0)
				{
					RenderGhostCells(columnWidths, ghostCells, 0, columnWidths.Length);
				}
				else
				{
					int num2 = 0;
					List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
					List<RPLTablixMemberCell>.Enumerator enumerator = list.GetEnumerator();
					if (omittedHeaders2 != null)
					{
						enumerator = omittedHeaders2.GetEnumerator();
					}
					enumerator.MoveNext();
					if (element.ColsBeforeRowHeaders > 0 && nextRow.BodyStart != -1)
					{
						int num3 = 0;
						int colsBeforeRowHeaders = element.ColsBeforeRowHeaders;
						int j;
						for (j = nextRow.BodyStart; j < nextRow.NumCells; j++)
						{
							if (num3 >= colsBeforeRowHeaders)
							{
								break;
							}
							RPLTablixCell rPLTablixCell = nextRow[j];
							borderContext2.Top = (rPLTablixCell.RowIndex == 0 && borderContext.Top);
							borderContext2.Left = (rPLTablixCell.ColIndex == 0 && borderContext.Left);
							borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == element.RowHeights.Length);
							borderContext2.Right = (borderContext.Left && rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == element.ColumnWidths.Length);
							RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell, enumerator, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == element.ColumnWidths.Length);
							num2 = rPLTablixCell.ColIndex + rPLTablixCell.ColSpan;
							num3 += rPLTablixCell.ColSpan;
						}
						int num4 = (nextRow.BodyStart > nextRow.HeaderStart) ? nextRow.BodyStart : nextRow.NumCells;
						for (int k = (nextRow.HeaderStart >= 0) ? nextRow.HeaderStart : 0; k < num4; k++)
						{
							RPLTablixCell rPLTablixCell2 = nextRow[k];
							borderContext2.Top = (rPLTablixCell2.RowIndex == 0 && borderContext.Top);
							borderContext2.Left = (rPLTablixCell2.ColIndex == 0 && borderContext.Left);
							borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell2.RowIndex + rPLTablixCell2.RowSpan == element.RowHeights.Length);
							borderContext2.Right = (borderContext.Right && rPLTablixCell2.ColIndex + rPLTablixCell2.ColSpan == element.ColumnWidths.Length);
							RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell2, enumerator, rPLTablixCell2.ColIndex + rPLTablixCell2.ColSpan == element.ColumnWidths.Length);
							num2 = rPLTablixCell2.ColIndex + rPLTablixCell2.ColSpan;
						}
						num4 = ((nextRow.BodyStart < nextRow.HeaderStart) ? nextRow.HeaderStart : nextRow.NumCells);
						for (int l = j; l < num4; l++)
						{
							RPLTablixCell rPLTablixCell3 = nextRow[l];
							borderContext2.Top = (rPLTablixCell3.RowIndex == 0 && borderContext.Top);
							borderContext2.Left = (rPLTablixCell3.ColIndex == 0 && borderContext.Left);
							borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell3.RowIndex + rPLTablixCell3.RowSpan == element.RowHeights.Length);
							borderContext2.Right = (borderContext.Right && rPLTablixCell3.ColIndex + rPLTablixCell3.ColSpan == element.ColumnWidths.Length);
							RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell3, enumerator, rPLTablixCell3.ColIndex + rPLTablixCell3.ColSpan == element.ColumnWidths.Length);
							num2 = rPLTablixCell3.ColIndex + rPLTablixCell3.ColSpan;
						}
					}
					else
					{
						for (int m = 0; m < nextRow.NumCells; m++)
						{
							RPLTablixCell rPLTablixCell4 = nextRow[m];
							borderContext2.Top = (rPLTablixCell4.RowIndex == 0 && borderContext.Top);
							borderContext2.Left = (rPLTablixCell4.ColIndex == 0 && borderContext.Left);
							borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell4.RowIndex + rPLTablixCell4.RowSpan == element.RowHeights.Length);
							borderContext2.Right = (borderContext.Right && rPLTablixCell4.ColIndex + rPLTablixCell4.ColSpan == element.ColumnWidths.Length);
							RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell4, enumerator, rPLTablixCell4.ColIndex + rPLTablixCell4.ColSpan == element.ColumnWidths.Length);
							num2 = rPLTablixCell4.ColIndex + rPLTablixCell4.ColSpan;
						}
					}
					if (num2 != columnWidths.Length)
					{
						RenderGhostCells(columnWidths, ghostCells, num2, columnWidths.Length);
					}
				}
				m_writer.WriteTableRowEnd();
				nextRow = element.GetNextRow();
				num++;
			}
			m_writer.WriteTableEnd();
		}

		protected void FinishRenderingTablixCell(RPLTablixCell cell, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext)
		{
			RPLTablixMemberCell rPLTablixMemberCell = cell as RPLTablixMemberCell;
			if (rPLTablixMemberCell != null)
			{
				string groupLabel = rPLTablixMemberCell.GroupLabel;
				if (groupLabel != null)
				{
					m_writer.RenderLabel(groupLabel, m_labelLevel);
				}
			}
			m_writer.WriteTableCellEnd(cell.ColIndex, borderContext, emptyLayoutCell: false);
			if (cell.ColSpan > 1)
			{
				for (int i = 1; i < cell.ColSpan; i++)
				{
					m_writer.WriteTableCellBegin(cell.ColIndex + i, widths.Length, cell.RowSpan > 1, firstHorzMerge: false, vertMerge: false, horzMerge: true);
					m_writer.ApplyCellBorderContext(borderContext);
					if (cell.Element != null)
					{
						RenderCellProperties(cell.Element.ElementProps.Style, cell.ColIndex + i, needsBorderOrPadding: true);
						m_writer.ClearCellBorder(TableData.Positions.Left);
						if (i != cell.ColSpan - 1)
						{
							m_writer.ClearCellBorder(TableData.Positions.Right);
						}
						if (cell.RowSpan > 1)
						{
							m_writer.ClearCellBorder(TableData.Positions.Bottom);
						}
					}
					m_writer.WriteTableCellEnd(cell.ColIndex + i, borderContext, emptyLayoutCell: false);
				}
			}
			if (cell.RowSpan > 1)
			{
				ghostCells[cell.ColIndex] = new TablixGhostCell();
				ghostCells[cell.ColIndex].Cell = cell;
				ghostCells[cell.ColIndex].RowSpan = cell.RowSpan - 1;
				ghostCells[cell.ColIndex].ColSpan = cell.ColSpan;
				ghostCells[cell.ColIndex].Context = new BorderContext(borderContext);
			}
		}

		protected void RenderTablixCellItem(RPLTablixCell cell, float[] widths, RPLItemMeasurement measurement, float left, BorderContext borderContext)
		{
			RenderReportItem(cell.Element, measurement, cell.ColIndex, GetLeft(widths, cell.ColIndex, left), borderContext, inTablix: true);
		}

		protected void ClearTablixCellBorders(RPLTablixCell cell)
		{
			if (cell.ColSpan > 1)
			{
				m_writer.ClearCellBorder(TableData.Positions.Right);
			}
			if (cell.RowSpan > 1)
			{
				m_writer.ClearCellBorder(TableData.Positions.Bottom);
			}
		}

		protected RPLItemMeasurement GetTablixCellMeasurement(RPLTablixCell cell, int nextCell, float[] widths, TablixGhostCell[] ghostCells, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell, RPLTablix tablix)
		{
			if (cell.ColIndex != nextCell)
			{
				RenderGhostCells(widths, ghostCells, nextCell, cell.ColIndex);
			}
			m_writer.WriteTableCellBegin(cell.ColIndex, widths.Length, cell.RowSpan > 1, cell.ColSpan > 1, vertMerge: false, horzMerge: false);
			RenderOmittedCells(omittedCells, cell.ColIndex, lastCell);
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Width = 0f;
			for (int i = 0; i < cell.ColSpan; i++)
			{
				rPLItemMeasurement.Width += tablix.ColumnWidths[i + cell.ColIndex];
			}
			rPLItemMeasurement.Height = 0f;
			for (int j = 0; j < cell.RowSpan; j++)
			{
				rPLItemMeasurement.Height += tablix.RowHeights[j + cell.RowIndex];
			}
			return rPLItemMeasurement;
		}

		private void RenderOmittedCells(List<RPLTablixMemberCell>.Enumerator omittedCells, int colIndex, bool lastCell)
		{
			while (omittedCells.Current != null && (omittedCells.Current.ColIndex == colIndex || lastCell))
			{
				if (!string.IsNullOrEmpty(omittedCells.Current.GroupLabel))
				{
					m_writer.RenderLabel(omittedCells.Current.GroupLabel, m_labelLevel);
				}
				omittedCells.MoveNext();
			}
		}

		private void RenderGhostCells(float[] widths, TablixGhostCell[] ghostCells, int nextCell, int endIndex)
		{
			while (nextCell < ghostCells.Length && ghostCells[nextCell] != null && nextCell < endIndex)
			{
				TablixGhostCell tablixGhostCell = ghostCells[nextCell];
				for (int i = 0; i < tablixGhostCell.ColSpan; i++)
				{
					m_writer.WriteTableCellBegin(nextCell + i, widths.Length, firstVertMerge: false, tablixGhostCell.ColSpan > 1 && i == 0, vertMerge: true, i > 0);
					m_writer.ApplyCellBorderContext(tablixGhostCell.Context);
					if (tablixGhostCell.Cell.Element != null)
					{
						IRPLStyle style = tablixGhostCell.Cell.Element.ElementProps.Style;
						RenderCellStyle(style, nextCell + i);
						m_writer.ClearCellBorder(TableData.Positions.Top);
						if (tablixGhostCell.RowSpan > 1)
						{
							m_writer.ClearCellBorder(TableData.Positions.Bottom);
						}
						if (i != 0)
						{
							m_writer.ClearCellBorder(TableData.Positions.Left);
						}
						if (i != tablixGhostCell.ColSpan - 1)
						{
							m_writer.ClearCellBorder(TableData.Positions.Right);
						}
					}
					m_writer.WriteTableCellEnd(nextCell + i, tablixGhostCell.Context, emptyLayoutCell: false);
				}
				tablixGhostCell.RowSpan--;
				if (tablixGhostCell.RowSpan == 0)
				{
					ghostCells[nextCell] = null;
				}
				nextCell += tablixGhostCell.ColSpan;
			}
		}

		private void RenderTablixStyle(IRPLStyle style, BorderContext borderContext)
		{
			RenderTableProperties(style, isTablix: true, borderContext);
		}

		private float GetLeft(float[] widths, int index, float left)
		{
			float num = left;
			for (int i = 0; i < index; i++)
			{
				num += widths[i];
			}
			return num;
		}

		private static void AssertRectangleColumns(RPLContainer rectangle, PageTableLayout layout)
		{
			if (layout != null && layout.NrCols > 63)
			{
				string text = null;
				if (!(rectangle is RPLRectangle))
				{
					text = ((!(rectangle is RPLBody)) ? WordRenderRes.ColumnsErrorHeaderFooter : WordRenderRes.ColumnsErrorBody);
				}
				else
				{
					RPLRectanglePropsDef rPLRectanglePropsDef = (RPLRectanglePropsDef)rectangle.ElementProps.Definition;
					text = string.Format(CultureInfo.CurrentCulture, WordRenderRes.ColumnsErrorRectangle, rPLRectanglePropsDef.Name);
				}
				throw new ReportRenderingException(text);
			}
		}

		private bool RenderRectangle(RPLContainer rectangle, float left, RPLMeasurement rectangleMeasurement, BorderContext parentBorderContext, bool inTablix)
		{
			return RenderRectangle(rectangle, left, canGrow: false, rectangleMeasurement, parentBorderContext, inTablix, ignoreStyles: false);
		}

		protected bool RenderRectangle(RPLContainer rectangle, float left, bool canGrow, RPLMeasurement rectangleMeasurement, BorderContext parentBorderContext, bool inTablix, bool ignoreStyles)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			if (children == null || children.Length == 0)
			{
				return false;
			}
			m_labelLevel++;
			PageTableLayout tableLayout = null;
			string text = null;
			if (rectangle is RPLRectangle)
			{
				text = ((RPLRectanglePropsDef)rectangle.ElementProps.Definition).LinkToChildId;
			}
			float ownerWidth = 0f;
			float ownerHeight = 0f;
			if (rectangleMeasurement != null)
			{
				ownerWidth = rectangleMeasurement.Width;
				ownerHeight = rectangleMeasurement.Height;
			}
			PageTableLayout.GenerateTableLayout(children, ownerWidth, ownerHeight, 0f, out tableLayout);
			AssertRectangleColumns(rectangle, tableLayout);
			m_writer.WriteTableBegin(left, layoutTable: true);
			RPLElementStyle style = rectangle.ElementProps.Style;
			BorderContext borderContext = new BorderContext();
			BorderContext borderContext2 = null;
			if (ignoreStyles)
			{
				borderContext2 = new BorderContext();
			}
			else
			{
				borderContext2 = HasBorders(style, parentBorderContext);
				RenderTableProperties(style, !inTablix, parentBorderContext);
			}
			if (tableLayout != null && (!tableLayout.BandTable || !m_writer.CanBand))
			{
				float[] array = new float[tableLayout.NrCols];
				for (int i = 0; i < tableLayout.NrCols; i++)
				{
					PageTableCell cell = tableLayout.GetCell(0, i);
					array[i] = cell.DXValue.Value;
				}
				bool flag = false;
				for (int j = 0; j < tableLayout.NrRows; j++)
				{
					float num = left;
					float value = tableLayout.GetCell(j, 0).DYValue.Value;
					flag = false;
					m_writer.WriteTableRowBegin(left, value, array);
					m_writer.IgnoreRowHeight(canGrow);
					borderContext.Top = (j == 0 && borderContext2.Top);
					for (int k = 0; k < tableLayout.NrCols; k++)
					{
						PageTableCell cell2 = tableLayout.GetCell(j, k);
						borderContext.Left = (k == 0 && borderContext2.Left);
						borderContext.Bottom = (borderContext2.Bottom && j + cell2.RowSpan >= tableLayout.NrRows);
						borderContext.Right = (borderContext2.Right && k + cell2.ColSpan >= tableLayout.NrCols);
						m_writer.WriteTableCellBegin(k, tableLayout.NrCols, cell2.FirstVertMerge, cell2.FirstHorzMerge, cell2.VertMerge, cell2.HorzMerge);
						m_writer.ApplyCellBorderContext(borderContext);
						flag = RenderRectangleItemAndLines(rectangle, borderContext, k, cell2, text, num, flag);
						m_writer.WriteTableCellEnd(k, borderContext, !cell2.InUse);
						num += array[k];
					}
					if (value > m_pageHeight && flag)
					{
						m_writer.IgnoreRowHeight(canGrow: true);
					}
					m_writer.WriteTableRowEnd();
				}
			}
			else if (children.Length == 1)
			{
				RPLItemMeasurement rPLItemMeasurement = children[0];
				m_writer.WriteTableRowBegin(left, rPLItemMeasurement.Height, new float[1]
				{
					rPLItemMeasurement.Width
				});
				m_writer.WriteTableCellBegin(0, 1, firstVertMerge: false, firstHorzMerge: false, vertMerge: false, horzMerge: false);
				RPLElement element = rPLItemMeasurement.Element;
				if (element.ElementProps.Definition.ID == text)
				{
					RenderBookmarksLabels(rectangle);
				}
				m_writer.ApplyCellBorderContext(borderContext2);
				RenderReportItem(element, rPLItemMeasurement, 0, left, borderContext2, inTablix: false);
				m_writer.WriteTableCellEnd(0, borderContext2, emptyLayoutCell: false);
				m_writer.WriteTableRowEnd();
			}
			else
			{
				borderContext.Left = borderContext2.Left;
				borderContext.Right = borderContext2.Right;
				for (int l = 0; l < children.Length; l++)
				{
					borderContext.Top = (l == 0 && borderContext2.Top);
					borderContext.Bottom = (l == children.Length - 1 && borderContext2.Bottom);
					RPLItemMeasurement rPLItemMeasurement2 = children[l];
					RPLElement element2 = rPLItemMeasurement2.Element;
					if (element2.ElementProps.Definition.ID == text)
					{
						RenderBookmarksLabels(rectangle);
					}
					m_writer.WriteTableRowBegin(0f, rPLItemMeasurement2.Height, new float[1]
					{
						rPLItemMeasurement2.Width
					});
					m_writer.WriteTableCellBegin(0, 1, firstVertMerge: false, firstHorzMerge: false, vertMerge: false, horzMerge: false);
					m_writer.ApplyCellBorderContext(borderContext);
					RenderReportItem(element2, rPLItemMeasurement2, 0, left, borderContext, inTablix: false);
					m_writer.WriteTableCellEnd(0, borderContext, emptyLayoutCell: false);
					m_writer.WriteTableRowEnd();
				}
			}
			m_writer.WriteTableEnd();
			m_labelLevel--;
			return true;
		}

		protected bool RenderRectangleItem(int y, PageTableCell cell, BorderContext borderContext, string linkToChildId, RPLContainer rectangle, float runningLeft, bool rowUsed)
		{
			if (cell.InUse)
			{
				RPLElement element = cell.Measurement.Element;
				if (element.ElementProps.Definition.ID == linkToChildId)
				{
					RenderBookmarksLabels(rectangle);
				}
				RenderReportItem(element, cell.Measurement, y, runningLeft, borderContext, inTablix: false);
				if (cell.RowSpan == 1 && (element is RPLTablix || element is RPLSubReport || element is RPLRectangle))
				{
					m_writer.IgnoreRowHeight(canGrow: true);
				}
				rowUsed = true;
			}
			else if (cell.Eaten)
			{
				rowUsed = true;
			}
			return rowUsed;
		}

		protected void RenderLines(int cellIndex, PageTableCell cell, BorderContext borderContext)
		{
			if (cell.BorderLeft != null)
			{
				byte lineStyleCode = 6;
				byte widthCode = 11;
				byte colorCode = 1;
				RenderLine(cellIndex, cell.BorderLeft, lineStyleCode, widthCode, colorCode, borderContext.Left);
			}
			if (cell.BorderRight != null)
			{
				byte lineStyleCode2 = 7;
				byte widthCode2 = 12;
				byte colorCode2 = 2;
				RenderLine(cellIndex, cell.BorderRight, lineStyleCode2, widthCode2, colorCode2, borderContext.Right);
			}
			if (cell.BorderTop != null)
			{
				byte lineStyleCode3 = 8;
				byte widthCode3 = 13;
				byte colorCode3 = 3;
				RenderLine(cellIndex, cell.BorderTop, lineStyleCode3, widthCode3, colorCode3, borderContext.Top);
			}
			if (cell.BorderBottom != null)
			{
				byte lineStyleCode4 = 9;
				byte widthCode4 = 14;
				byte colorCode4 = 4;
				RenderLine(cellIndex, cell.BorderBottom, lineStyleCode4, widthCode4, colorCode4, borderContext.Bottom);
			}
		}

		private void RenderLine(int cellIndex, RPLLine line, byte lineStyleCode, byte widthCode, byte colorCode, bool onlyLabel)
		{
			if (!onlyLabel)
			{
				RPLElementStyle style = line.ElementProps.Style;
				object value = style[5];
				object value2 = style[0];
				object value3 = style[10];
				m_writer.AddCellStyleProp(cellIndex, lineStyleCode, value);
				m_writer.AddCellStyleProp(cellIndex, colorCode, value2);
				m_writer.AddCellStyleProp(cellIndex, widthCode, value3);
			}
			RenderBookmarksLabels(line);
		}

		private byte[] CreateImageBuf(RPLImageData imgData)
		{
			if (imgData.ImageDataOffset >= 0)
			{
				return m_rplReport.GetImage(imgData.ImageDataOffset);
			}
			if (imgData.ImageData != null)
			{
				return imgData.ImageData;
			}
			return null;
		}

		private bool HasAction(RPLAction action)
		{
			if (action.BookmarkLink == null && (action.DrillthroughUrl == null || m_omitDrillthroughs))
			{
				if (action.Hyperlink != null)
				{
					return !m_omitHyperlinks;
				}
				return false;
			}
			return true;
		}

		private bool HasAction(RPLActionInfo actionInfo)
		{
			if (actionInfo != null)
			{
				return HasAction(actionInfo.Actions[0]);
			}
			return false;
		}

		private void WriteBeginTableRowCell(RPLItemMeasurement measurement, bool notCanGrow)
		{
			m_writer.WriteTableBegin(0f, layoutTable: true);
			m_writer.WriteTableRowBegin(0f, measurement.Height, new float[1]
			{
				measurement.Width
			});
			if (notCanGrow)
			{
				m_writer.SetWriteExactRowHeight(writeExactRowHeight: true);
			}
			m_writer.WriteTableCellBegin(0, 1, firstVertMerge: false, firstHorzMerge: false, vertMerge: false, horzMerge: false);
		}

		private void WriteEndCellRowTable(BorderContext borderContext)
		{
			m_writer.WriteTableCellEnd(0, borderContext, emptyLayoutCell: false);
			m_writer.WriteTableRowEnd();
			m_writer.WriteTableEnd();
		}

		protected void CachePage(ref bool pageCached, List<RPLReport> rplReportCache)
		{
			if (!pageCached)
			{
				rplReportCache.Add(m_rplReport);
				pageCached = true;
			}
		}

		protected bool SetFirstPageDimensions(bool firstPage, RPLPageContent pageContent, ref RPLPageLayout rplPageLayout, ref float leftMargin, ref float rightMargin, ref float width, ref string title, ref string author, ref string description)
		{
			if (firstPage)
			{
				rplPageLayout = pageContent.PageLayout;
				leftMargin = rplPageLayout.MarginLeft;
				rightMargin = rplPageLayout.MarginRight;
				if (Word97Writer.FixMargins(rplPageLayout.PageWidth, ref leftMargin, ref rightMargin) && RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace("The left or right margin is either <0 or the sum exceeds the page width.");
				}
				m_pageHeight = rplPageLayout.PageHeight;
				width = rplPageLayout.PageWidth;
				_ = rplPageLayout.PageWidth;
				author = m_rplReport.Author;
				title = m_rplReport.ReportName;
				description = m_rplReport.Description;
				firstPage = false;
			}
			return firstPage;
		}

		protected float RevisePageDimensions(float leftMargin, float rightMargin, float width, float bodyWidth, AutoFit initialAutoFit)
		{
			if (!m_deviceInfo.FixedPageWidth)
			{
				float num = bodyWidth + leftMargin + rightMargin;
				if (width < num)
				{
					width = num;
				}
			}
			if (width > 558.8f)
			{
				width = 558.8f;
			}
			if (initialAutoFit == AutoFit.Default)
			{
				if (bodyWidth > 558.8f - (leftMargin + rightMargin))
				{
					m_writer.AutoFit = AutoFit.Never;
				}
				else
				{
					m_writer.AutoFit = AutoFit.True;
				}
			}
			return width;
		}

		protected void RenderHeaderBetweenSections(RPLReportSection section, bool firstSection)
		{
			if (!firstSection && section.Header != null)
			{
				RPLHeaderFooter rPLHeaderFooter = section.Header.Element as RPLHeaderFooter;
				if (rPLHeaderFooter != null && (rPLHeaderFooter.ElementPropsDef as RPLHeaderFooterPropsDef).PrintBetweenSections)
				{
					m_needsToResetTextboxes = true;
					m_inHeaderFooter = true;
					RenderRectangle(rPLHeaderFooter, 0f, canGrow: false, section.Header, new BorderContext(), inTablix: false, ignoreStyles: true);
					m_inHeaderFooter = false;
					m_needsToResetTextboxes = false;
				}
			}
		}

		protected void RenderBodyContent(float bodyWidth, RPLItemMeasurement bodyMeasurement)
		{
			RPLMeasurement rPLMeasurement = new RPLMeasurement(bodyMeasurement);
			rPLMeasurement.Width = bodyWidth;
			RenderRectangle((RPLContainer)bodyMeasurement.Element, 0f, canGrow: false, rPLMeasurement, new BorderContext(), inTablix: false, ignoreStyles: true);
		}

		protected RPLReportSection AdvanceToNextSection(RPLPageContent pageContent, RPLReportSection section, ref bool firstSection, SectionEntry lastSection, RPLHeaderFooter footer, SectionEntry se)
		{
			if (pageContent.HasNextReportSection())
			{
				if (footer == null && se == null)
				{
					se = lastSection;
					if (se.FooterMeasurement != null)
					{
						footer = (section.Footer.Element as RPLHeaderFooter);
					}
				}
				if (footer != null && (footer.ElementPropsDef as RPLHeaderFooterPropsDef).PrintBetweenSections)
				{
					m_needsToResetTextboxes = true;
					m_inHeaderFooter = true;
					RenderRectangle(footer, 0f, canGrow: false, section.Footer, new BorderContext(), inTablix: false, ignoreStyles: true);
					m_inHeaderFooter = false;
					m_needsToResetTextboxes = false;
				}
				m_writer.WriteEndSection();
				firstSection = false;
			}
			section = pageContent.GetNextReportSection();
			return section;
		}

		protected void FinishRendering(List<RPLReport> rplReportCache, string title, string author, string description)
		{
			for (int i = 0; i < rplReportCache.Count; i++)
			{
				rplReportCache[i].Release();
				rplReportCache[i] = null;
			}
			if (m_rplReport != null)
			{
				m_rplReport.Release();
				m_rplReport = null;
			}
			m_writer.Finish(title, author, description);
		}
	}
}
