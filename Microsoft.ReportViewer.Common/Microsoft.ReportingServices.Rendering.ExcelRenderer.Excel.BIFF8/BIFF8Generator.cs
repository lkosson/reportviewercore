using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Generator : IExcelGenerator
	{
		private ExcelGeneratorConstants.CreateTempStream m_createTempStream;

		private List<BIFF8Color> m_colors = new List<BIFF8Color>();

		private Dictionary<string, BIFF8Color> m_colorLookup = new Dictionary<string, BIFF8Color>();

		private Escher.DrawingGroupContainer m_drawingGroupContainer;

		private SSTHandler m_stringTable = new SSTHandler();

		private WorksheetInfo m_worksheet;

		private BinaryWriter m_worksheetOut;

		private int m_numRowsThisBlock;

		private RowHandler m_rowHandler;

		private int m_column = -1;

		private object m_cellValue;

		private TypeCode m_cellValueType = TypeCode.String;

		private ExcelErrorCode m_cellErrorCode = ExcelErrorCode.None;

		private StyleContainer m_styleContainer;

		private bool m_checkForRotatedEastAsianChars;

		private int m_rowIndexStartOfBlock;

		private long m_startOfBlock = -1L;

		private long m_startOfFirstCellData;

		private long m_lastDataCheckPoint = -1L;

		private List<WorksheetInfo> m_worksheets = new List<WorksheetInfo>();

		private ExternSheetInfo m_externSheet;

		private Dictionary<string, string> m_bookmarks = new Dictionary<string, string>();

		private Dictionary<string, int> m_worksheetNames = new Dictionary<string, int>();

		public int MaxRows => 65536;

		public int MaxColumns => 256;

		public int RowBlockSize => 32;

		internal BIFF8Generator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			m_createTempStream = createTempStream;
			m_styleContainer = new StyleContainer();
			m_worksheetNames.Add(ExcelRenderRes.SheetName.ToUpperInvariant(), 1);
			AddNewWorksheet();
		}

		private void RenderCell()
		{
			if (m_column == -1)
			{
				return;
			}
			ExcelDataType excelDataType = ExcelDataType.Blank;
			RichTextInfo richTextInfo = null;
			if (m_cellValue != null)
			{
				if (m_cellValueType == TypeCode.Object)
				{
					richTextInfo = (m_cellValue as RichTextInfo);
					if (richTextInfo != null)
					{
						excelDataType = ExcelDataType.RichString;
					}
				}
				else
				{
					excelDataType = FormatHandler.GetDataType(m_cellValueType);
				}
			}
			bool foundEastAsianChar = false;
			switch (excelDataType)
			{
			case ExcelDataType.String:
			{
				string text = m_cellValue.ToString();
				if (text.Length > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(text.Length);
					ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(text, stringBuilder, m_checkForRotatedEastAsianChars, out foundEastAsianChar);
					m_cellValue = stringBuilder.ToString();
				}
				break;
			}
			case ExcelDataType.RichString:
				foundEastAsianChar = richTextInfo.FoundRotatedFarEastChar;
				break;
			}
			if (foundEastAsianChar)
			{
				m_styleContainer.Orientation = Orientation.Vertical;
			}
			m_styleContainer.Finish();
			if (m_cellValue != null || m_styleContainer.CellIxfe != 15)
			{
				m_rowHandler.Add(m_cellValue, richTextInfo, m_cellValueType, excelDataType, m_cellErrorCode, (short)m_column, (ushort)m_styleContainer.CellIxfe);
			}
			m_cellValue = null;
			m_cellValueType = TypeCode.String;
			m_cellErrorCode = ExcelErrorCode.None;
			m_styleContainer.Reset();
			m_checkForRotatedEastAsianChars = false;
		}

		private void FinishRow()
		{
			RenderCell();
			if (m_rowHandler != null)
			{
				m_rowHandler.FlushRow();
			}
			m_column = -1;
			long num = m_worksheetOut.BaseStream.Position - m_lastDataCheckPoint;
			m_worksheet.SizeOfCellData.Add((ushort)num);
			m_lastDataCheckPoint = m_worksheetOut.BaseStream.Position;
		}

		private void CalcAndWriteDBCells()
		{
			long num = m_worksheetOut.BaseStream.Position - m_startOfBlock;
			m_worksheet.DBCellOffsets.Add((uint)m_worksheetOut.BaseStream.Position);
			RecordFactory.DBCELL(m_worksheetOut, (uint)num, m_worksheet.SizeOfCellData);
			m_numRowsThisBlock = 0;
			m_worksheet.SizeOfCellData.Clear();
			m_startOfBlock = -1L;
		}

		private void AddNewWorksheet()
		{
			m_worksheet = new WorksheetInfo(m_createTempStream("Page" + m_worksheets.Count + 1), ExcelRenderRes.SheetName + (m_worksheets.Count + 1));
			m_worksheet.SheetIndex = m_worksheets.Count;
			m_worksheets.Add(m_worksheet);
			m_worksheetOut = new BinaryWriter(m_worksheet.CellData, Encoding.Unicode);
			m_numRowsThisBlock = 0;
			m_rowHandler = null;
			m_column = -1;
			m_cellValue = null;
			m_cellValueType = TypeCode.String;
			m_checkForRotatedEastAsianChars = false;
			m_startOfBlock = -1L;
			m_rowIndexStartOfBlock = 0;
			m_startOfFirstCellData = 0L;
			m_lastDataCheckPoint = -1L;
			m_styleContainer.Reset();
		}

		private int AddExternSheet(int supBookIndex, int firstTab, int lastTab)
		{
			if (m_externSheet == null)
			{
				m_externSheet = new ExternSheetInfo();
			}
			return m_externSheet.AddXTI((ushort)supBookIndex, (ushort)firstTab, (ushort)lastTab);
		}

		private void CompleteCurrentWorksheet()
		{
			FinishRow();
			CalcAndWriteDBCells();
			m_worksheetOut.Flush();
		}

		private List<long> WriteGlobalStream(BinaryWriter writer)
		{
			writer.BaseStream.Write(Constants.GLOBAL1, 0, Constants.GLOBAL1.Length);
			m_styleContainer.Write(writer);
			writer.BaseStream.Write(Constants.GLOBAL3, 0, Constants.GLOBAL3.Length);
			RecordFactory.PALETTE(writer, m_colors);
			List<long> list = new List<long>();
			foreach (WorksheetInfo worksheet in m_worksheets)
			{
				list.Add(writer.BaseStream.Position);
				RecordFactory.BOUNDSHEET(writer, 0u, worksheet.SheetName);
			}
			if (m_externSheet != null)
			{
				RecordFactory.SUPBOOK(writer, (ushort)m_worksheets.Count);
				RecordFactory.EXTERNSHEET(writer, m_externSheet);
				foreach (WorksheetInfo worksheet2 in m_worksheets)
				{
					if (worksheet2.PrintTitle != null)
					{
						RecordFactory.NAME_PRINTTITLE(writer, worksheet2.PrintTitle);
					}
				}
			}
			if (m_drawingGroupContainer != null)
			{
				MsoDrawingGroup.WriteToStream(writer, m_drawingGroupContainer);
			}
			m_stringTable.Write(writer.BaseStream);
			writer.BaseStream.Write(Constants.GLOBAL4, 0, Constants.GLOBAL4.Length);
			return list;
		}

		private void AdjustWorksheetName(string reportName, WorksheetInfo workSheet)
		{
			if (!string.IsNullOrEmpty(reportName))
			{
				if (reportName.Length <= 31)
				{
					workSheet.SheetName = reportName;
				}
				else
				{
					workSheet.SheetName = reportName.Substring(0, 31);
				}
			}
		}

		private bool CheckForSheetNameConflict(string pageName)
		{
			if (pageName.Length < ExcelRenderRes.SheetName.Length || !pageName.StartsWith(ExcelRenderRes.SheetName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			int result = 0;
			return int.TryParse(pageName.Substring(ExcelRenderRes.SheetName.Length - 1), out result);
		}

		public void NextWorksheet()
		{
			CompleteCurrentWorksheet();
			AddNewWorksheet();
		}

		public void SetCurrentSheetName(string name)
		{
			m_worksheet.SheetName = name;
		}

		public void AdjustFirstWorksheetName(string reportName, bool addedDocMap)
		{
			if (addedDocMap)
			{
				if (m_worksheets.Count == 2)
				{
					AdjustWorksheetName(reportName, m_worksheets[1]);
				}
			}
			else if (m_worksheets.Count == 1)
			{
				AdjustWorksheetName(reportName, m_worksheets[0]);
			}
		}

		public void GenerateWorksheetName(string pageName)
		{
			if (string.IsNullOrEmpty(pageName))
			{
				return;
			}
			StringBuilder stringBuilder = ExcelGeneratorStringUtil.SanitizeSheetName(pageName);
			if (stringBuilder.Length == 0)
			{
				return;
			}
			pageName = stringBuilder.ToString();
			int value = 0;
			while (m_worksheetNames.TryGetValue(pageName.ToUpperInvariant(), out value) || CheckForSheetNameConflict(pageName))
			{
				value++;
				m_worksheetNames[pageName.ToUpperInvariant()] = value;
				string text = string.Format(CultureInfo.InvariantCulture, ExcelRenderRes.SheetNameCounterSuffix, value);
				if (pageName.Length + text.Length > 31)
				{
					pageName = pageName.Remove(31 - text.Length);
				}
				pageName += text;
			}
			m_worksheet.SheetName = pageName;
			m_worksheetNames.Add(pageName.ToUpperInvariant(), 1);
		}

		public void SetSummaryRowAfter(bool after)
		{
			m_worksheet.SummaryRowAfter = after;
		}

		public void SetSummaryColumnToRight(bool after)
		{
			m_worksheet.SummaryColumnToRight = after;
		}

		public void SetColumnProperties(int columnIndex, double widthInPoints, byte outlineLevel, bool collapsed)
		{
			WorksheetInfo.ColumnInfo columnInfo = new WorksheetInfo.ColumnInfo(widthInPoints);
			columnInfo.OutlineLevel = outlineLevel;
			m_worksheet.MaxColumnOutline = Math.Max(m_worksheet.MaxColumnOutline, outlineLevel);
			columnInfo.Collapsed = collapsed;
			m_worksheet.Columns[columnIndex] = columnInfo;
		}

		public void SetColumnExtents(int min, int max)
		{
			m_worksheet.ColFirst = (ushort)min;
			m_worksheet.ColLast = (ushort)max;
		}

		public void AddRow(int rowIndex)
		{
			if (m_startOfBlock == -1)
			{
				m_startOfBlock = m_worksheetOut.BaseStream.Position;
				m_startOfFirstCellData = -20L;
				m_rowIndexStartOfBlock = rowIndex;
			}
			if (m_worksheet.RowFirst > rowIndex)
			{
				m_worksheet.RowFirst = (ushort)rowIndex;
			}
			if (m_worksheet.RowLast < rowIndex)
			{
				m_worksheet.RowLast = (ushort)rowIndex;
			}
			m_startOfFirstCellData += RecordFactory.ROW(m_worksheetOut, (ushort)rowIndex, m_worksheet.ColFirst, m_worksheet.ColLast, 0, 0, collapsed: false, autoSize: false);
		}

		public void SetRowProperties(int rowIndex, int heightIn20thPoints, byte rowOutlineLevel, bool collapsed, bool autoSize)
		{
			m_numRowsThisBlock++;
			int num = rowIndex - m_rowIndexStartOfBlock;
			long position = m_startOfBlock + num * 20;
			m_worksheetOut.BaseStream.Position = position;
			RecordFactory.ROW(m_worksheetOut, (ushort)rowIndex, m_worksheet.ColFirst, m_worksheet.ColLast, (ushort)heightIn20thPoints, rowOutlineLevel, collapsed, autoSize);
			m_worksheetOut.BaseStream.Seek(0L, SeekOrigin.End);
			m_worksheet.MaxRowOutline = Math.Max(m_worksheet.MaxRowOutline, rowOutlineLevel);
			FinishRow();
			if (m_numRowsThisBlock == 32)
			{
				CalcAndWriteDBCells();
			}
		}

		public void SetRowContext(int row)
		{
			if (m_rowHandler == null)
			{
				m_rowHandler = new RowHandler(m_worksheetOut, row, m_stringTable);
			}
			m_rowHandler.Row = row;
		}

		public void SetColumnContext(int column)
		{
			RenderCell();
			m_column = column;
		}

		public void SetCellValue(object value, TypeCode type)
		{
			if (m_lastDataCheckPoint == -1)
			{
				m_lastDataCheckPoint = m_worksheetOut.BaseStream.Position;
				m_worksheet.SizeOfCellData.Add((ushort)m_startOfFirstCellData);
				m_startOfFirstCellData = 0L;
			}
			m_cellValue = value;
			m_cellValueType = type;
			m_cellErrorCode = ExcelErrorCode.None;
		}

		public void SetModifiedRotationForEastAsianChars(bool value)
		{
			m_checkForRotatedEastAsianChars = value;
		}

		public IRichTextInfo GetCellRichTextInfo()
		{
			m_cellValueType = TypeCode.Object;
			m_cellValue = new RichTextInfo(m_styleContainer);
			return (IRichTextInfo)m_cellValue;
		}

		public void SetCellError(ExcelErrorCode errorCode)
		{
			SetCellValue(null, TypeCode.Empty);
			m_cellErrorCode = errorCode;
		}

		public void AddMergeCell(int rowStart, int columnStart, int rowStop, int columnStop)
		{
			m_worksheet.MergeCellAreas.Add(new AreaInfo(rowStart, rowStop, columnStart, columnStop));
		}

		public IColor AddColor(string color)
		{
			if (m_colorLookup.ContainsKey(color))
			{
				return m_colorLookup[color];
			}
			Color color2 = LayoutConvert.ToColor(color);
			BIFF8Color bIFF8Color = null;
			for (int i = 0; i < m_colors.Count; i++)
			{
				if (m_colors[i].Equals(color2))
				{
					bIFF8Color = m_colors[i];
				}
			}
			if (bIFF8Color == null)
			{
				if (m_colors.Count >= 56)
				{
					return m_colors[0];
				}
				bIFF8Color = new BIFF8Color(color2, m_colors.Count + 8);
				m_colors.Add(bIFF8Color);
			}
			m_colorLookup[color] = bIFF8Color;
			return bIFF8Color;
		}

		public IStyle GetCellStyle()
		{
			return m_styleContainer;
		}

		public TypeCode GetCellValueType()
		{
			return m_cellValueType;
		}

		public void AddImage(string imageName, Stream imageData, ImageFormat format, int rowStart, double rowStartPercentage, int columnStart, double columnStartPercentage, int rowEnd, double rowEndPercentage, int columnEnd, double columnEndPercentage, string hyperlinkURL, bool isBookmarkLink)
		{
			if (imageData != null && imageData.Length != 0L)
			{
				if (m_drawingGroupContainer == null)
				{
					m_drawingGroupContainer = new Escher.DrawingGroupContainer();
				}
				uint startSPID = 0u;
				ushort dgID = 0;
				uint referenceIndex = m_drawingGroupContainer.AddImage(imageData, format, imageName, m_worksheets.Count - 1, out startSPID, out dgID);
				Escher.ClientAnchor.SPRC clientAnchor = new Escher.ClientAnchor.SPRC((ushort)columnStart, (short)(columnStartPercentage * 1024.0), (ushort)rowStart, (short)(rowStartPercentage * 256.0), (ushort)columnEnd, (short)(columnEndPercentage * 1024.0), (ushort)rowEnd, (short)(rowEndPercentage * 256.0));
				m_worksheet.AddImage(dgID, startSPID, imageName, clientAnchor, referenceIndex, hyperlinkURL, isBookmarkLink);
			}
		}

		public void AddBackgroundImage(byte[] data, string imageName, ref Stream backgroundImage, ref ushort backgroundImageWidth, ref ushort backgroundImageHeight)
		{
			Image image = null;
			Bitmap bitmap = null;
			ushort num = 0;
			ushort num2 = 0;
			if (backgroundImage != null || imageName == null || string.IsNullOrEmpty(imageName))
			{
				return;
			}
			backgroundImage = m_createTempStream("BACKGROUNDIMAGE");
			backgroundImage.Write(data, 0, data.Length);
			backgroundImage.Position = 0L;
			try
			{
				image = Image.FromStream(backgroundImage);
			}
			catch
			{
			}
			if (image != null)
			{
				bitmap = new Bitmap(image);
				Color white = Color.White;
				num = (ushort)image.Width;
				num2 = (ushort)image.Height;
				int num3 = num * 3;
				int num4 = num3 % 4;
				if (num4 > 0)
				{
					num4 = 4 - num4;
				}
				byte[] array = new byte[num3 + num4];
				backgroundImage.SetLength(0L);
				backgroundImage.Position = 0L;
				for (int num5 = num2 - 1; num5 >= 0; num5--)
				{
					for (int i = 0; i < num; i++)
					{
						Color pixel = bitmap.GetPixel(i, num5);
						if (pixel.A == 0)
						{
							array[i * 3] = white.B;
							array[i * 3 + 1] = white.G;
							array[i * 3 + 2] = white.R;
						}
						else
						{
							array[i * 3] = pixel.B;
							array[i * 3 + 1] = pixel.G;
							array[i * 3 + 2] = pixel.R;
						}
					}
					backgroundImage.Write(array, 0, array.Length);
					Array.Clear(array, 0, array.Length);
				}
				backgroundImageWidth = num;
				backgroundImageHeight = num2;
				if (bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
				}
				if (image != null)
				{
					image.Dispose();
					image = null;
				}
			}
			else if (backgroundImage != null)
			{
				backgroundImage.Close();
				backgroundImage = null;
			}
		}

		public void AddHeader(string left, string center, string right)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(left, stringBuilder.Append("&L"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(center, stringBuilder.Append("&C"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(right, stringBuilder.Append("&R"));
			m_worksheet.HeaderString = stringBuilder.ToString();
		}

		public void AddFooter(string left, string center, string right)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(left, stringBuilder.Append("&L"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(center, stringBuilder.Append("&C"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(right, stringBuilder.Append("&R"));
			m_worksheet.FooterString = stringBuilder.ToString();
		}

		public void AddPrintTitle(int rowStart, int rowEnd)
		{
			int externSheetIndex = AddExternSheet(0, m_worksheet.SheetIndex, m_worksheet.SheetIndex);
			m_worksheet.AddPrintTitle(externSheetIndex, rowStart, rowEnd);
		}

		public void AddFreezePane(int row, int column)
		{
			m_worksheet.AddFreezePane(row, column);
		}

		public void AddHyperlink(string label, string url)
		{
			m_worksheet.AddHyperlink(m_rowHandler.Row, m_column, url, label);
		}

		public void AddBookmarkLink(string label, string bookmark)
		{
			m_worksheet.AddBookmark(m_rowHandler.Row, m_column, bookmark, label);
		}

		public void AddBookmarkTarget(string targetName)
		{
			m_bookmarks[targetName] = CellReference.CreateExcelReference(m_worksheet.SheetName, m_rowHandler.Row, m_column);
		}

		public void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin)
		{
			m_worksheet.SetPageContraints(paperSize, isPortrait, headerMargin, footerMargin);
		}

		public void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			m_worksheet.SetMargins(topMargin, bottomMargin, leftMargin, rightMargin);
		}

		public void SaveSpreadsheet(Stream outputStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight)
		{
			CompleteCurrentWorksheet();
			m_worksheet = null;
			m_worksheetOut = null;
			Stream stream = m_createTempStream("Workbook");
			BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.Unicode);
			List<long> list = WriteGlobalStream(binaryWriter);
			bool isFirstPage = true;
			for (int i = 0; i < m_worksheets.Count; i++)
			{
				WorksheetInfo worksheetInfo = m_worksheets[i];
				worksheetInfo.ResolveCellReferences(m_bookmarks);
				worksheetInfo.Write(binaryWriter, isFirstPage, m_createTempStream, backgroundImage, backgroundImageWidth, backgroundImageHeight);
				isFirstPage = false;
			}
			for (int j = 0; j < list.Count; j++)
			{
				stream.Seek(list[j] + 4, SeekOrigin.Begin);
				binaryWriter.Write((uint)m_worksheets[j].BOFStartOffset);
			}
			m_worksheets = null;
			stream.Flush();
			stream.Seek(0L, SeekOrigin.Begin);
			StructuredStorage.CreateSingleStreamFile(stream, "Workbook", "00020820-0000-0000-c000-000000000046", outputStream, forceInMemory: false);
			stream.Close();
			stream = null;
			outputStream.Flush();
		}

		public bool UseCachedStyle(string id)
		{
			return m_styleContainer.UseSharedStyle(id);
		}

		public void DefineCachedStyle(string id)
		{
			m_styleContainer.DefineSharedStyle(id);
		}

		public void EndCachedStyle()
		{
			m_styleContainer.Finish();
		}

		public Stream CreateStream(string name)
		{
			return m_createTempStream(name);
		}

		public void BuildHeaderFooterString(StringBuilder str, RPLTextBoxProps textBox, ref string lastFont, ref double lastFontSize)
		{
			RPLElementStyle style = textBox.Style;
			string text = (string)style[20];
			if (!string.IsNullOrEmpty(text) && !text.Equals(lastFont))
			{
				str.Append("&").Append("\"");
				FormulaHandler.EncodeHeaderFooterString(str, text);
				str.Append("\"");
				lastFont = text;
			}
			string text2 = (string)style[21];
			if (!string.IsNullOrEmpty(text2))
			{
				double num = LayoutConvert.ToPoints(text2);
				if (num != 0.0 && num != lastFontSize)
				{
					str.Append("&").Append((int)num);
				}
				lastFontSize = num;
			}
			StringBuilder stringBuilder = new StringBuilder();
			object obj = style[19];
			if (obj != null && (RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic)
			{
				str.Append("&I");
				stringBuilder.Append("&I");
			}
			object obj2 = textBox.Style[22];
			if (obj2 != null && LayoutConvert.ToFontWeight((RPLFormat.FontWeights)obj2) >= 600)
			{
				str.Append("&B");
				stringBuilder.Append("&B");
			}
			object obj3 = textBox.Style[24];
			if (obj3 != null)
			{
				RPLFormat.TextDecorations num2 = (RPLFormat.TextDecorations)obj3;
				if (num2 == RPLFormat.TextDecorations.Underline)
				{
					str.Append("&u");
					stringBuilder.Append("&u");
				}
				if (num2 == RPLFormat.TextDecorations.LineThrough)
				{
					str.Append("&s");
					stringBuilder.Append("&s");
				}
			}
			string text3 = string.Empty;
			string text4 = ((RPLTextBoxPropsDef)textBox.Definition).Formula;
			if (text4 != null && text4.Length != 0)
			{
				if (text4.StartsWith("=", StringComparison.Ordinal))
				{
					text4 = text4.Remove(0, 1);
				}
				text3 = FormulaHandler.ProcessHeaderFooterFormula(text4);
			}
			if (text3 != null && text3.Length != 0)
			{
				str.Append(text3);
			}
			else
			{
				string text5 = textBox.Value;
				if (text5 == null)
				{
					text5 = ((RPLTextBoxPropsDef)textBox.Definition).Value;
				}
				if (text5 == null && textBox.OriginalValue != null)
				{
					text5 = textBox.OriginalValue.ToString();
				}
				if (text5 != null)
				{
					FormulaHandler.EncodeHeaderFooterString(str, text5.Trim());
				}
			}
			if (stringBuilder.Length > 0)
			{
				str.Append(stringBuilder);
			}
			str.Append(" ");
		}
	}
}
