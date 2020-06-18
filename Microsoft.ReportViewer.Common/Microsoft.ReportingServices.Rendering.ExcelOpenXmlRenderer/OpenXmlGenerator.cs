using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class OpenXmlGenerator : IExcelGenerator
	{
		internal struct MergeInfo
		{
			private int _firstRow;

			private int _rowCount;

			private int _firstColumn;

			private int _columnCount;

			public int FirstColumn => _firstColumn;

			public int FirstRow => _firstRow;

			public int RowCount => _rowCount;

			public int ColumnCount => _columnCount;

			public MergeInfo(int firstRow, int lastRow, int firstColumn, int lastColumn)
			{
				_firstRow = firstRow;
				_rowCount = lastRow - firstRow + 1;
				_firstColumn = firstColumn;
				_columnCount = lastColumn - firstColumn + 1;
			}
		}

		internal struct BookmarkTargetInfo
		{
			private readonly string _sheet;

			private readonly string _cell;

			public string Sheet => _sheet;

			public string Cell => _cell;

			public BookmarkTargetInfo(string sheet, string cell)
			{
				_sheet = sheet;
				_cell = cell;
			}
		}

		internal struct LinkInfo
		{
			private readonly string _areaFormula;

			private readonly string _href;

			private string _label;

			public string AreaFormula => _areaFormula;

			public string Href => _href;

			public string Label => _label;

			public LinkInfo(string areaFormula, string href, string label)
			{
				_areaFormula = areaFormula;
				_href = href;
				_label = label;
			}
		}

		internal struct PictureLinkInfo
		{
			private readonly Picture _picture;

			private readonly string _target;

			public Picture Picture => _picture;

			public string Target => _target;

			public PictureLinkInfo(Picture picture, string target)
			{
				_picture = picture;
				_target = target;
			}
		}

		internal struct UnresolvedStreamsheet
		{
			private readonly Streamsheet _streamsheet;

			private readonly List<LinkInfo> _bookmarks;

			public Streamsheet Streamsheet => _streamsheet;

			public List<LinkInfo> Bookmarks => _bookmarks;

			public UnresolvedStreamsheet(Streamsheet sheet, List<LinkInfo> bookmarks)
			{
				_streamsheet = sheet;
				_bookmarks = bookmarks;
			}

			public bool ResolveTarget(string label, string target)
			{
				for (int num = _bookmarks.Count - 1; num >= 0; num--)
				{
					if (_bookmarks[num].Href == label)
					{
						_streamsheet.CreateHyperlink(_bookmarks[num].AreaFormula, target, _bookmarks[num].Label);
						_bookmarks.RemoveAt(num);
					}
				}
				if (_bookmarks.Count == 0)
				{
					_streamsheet.Cleanup();
					return true;
				}
				return false;
			}
		}

		private const int MAX_HEADER_FOOTER_SIZE = 255;

		private const int HEADER_FOOTER_SECTION_CODE_SIZE = 2;

		private const string CELL_BOOKMARK_FORMAT = "'{0}'!{1}";

		private const string PICTURE_BOOKMARK_FORMAT = "#{0}!{1}";

		private readonly ExcelApplication _application;

		private Workbook _workbook;

		private Streamsheet _currentSheet;

		private Row _currentRow;

		private Cell _currentCell;

		private bool _currentCellHasBeenModified;

		private RichTextInfo _currentRichText;

		private ExcelGeneratorConstants.CreateTempStream _createTempStream;

		private Stream _workbookStream;

		private Dictionary<string, int> _worksheetNames;

		private List<MergeInfo> _mergedCells;

		private List<LinkInfo> _hyperlinks;

		private List<LinkInfo> _bookmarkLinks;

		private List<UnresolvedStreamsheet> _unresolvedStreamsheets;

		private Dictionary<string, BookmarkTargetInfo> _bookmarkTargets;

		private Style _currentStyle;

		private Style _cachedStyle;

		private Dictionary<string, Style> _cachedstyles;

		private int _nextPaletteIndex;

		private bool _checkForRotatedEastAsianChars;

		private List<Picture> _picturesStartingOnCurrentRow;

		private Dictionary<int, List<Picture>> _picturesToUpdateByEndRow;

		private Dictionary<int, List<Picture>> _picturesToUpdateByStartColumn;

		private Dictionary<int, List<Picture>> _picturesToUpdateByEndColumn;

		private List<PictureLinkInfo> _unresolvedPictureBookmarks;

		private int _maxRowIndex;

		private int _currentColumn;

		private const int BIFF8_BLOCK_SIZE = 32;

		private const int MAX_OUTLINE_LEVEL = 7;

		public int MaxRows => _currentSheet.MaxRowIndex;

		public int MaxColumns => _currentSheet.MaxColIndex;

		public int RowBlockSize => 32;

		public OpenXmlGenerator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			_createTempStream = createTempStream;
			_application = new ExcelApplication();
			_workbookStream = _createTempStream("oxmlWorkbook");
			_workbook = _application.CreateStreaming(_workbookStream);
			CreateNewSheet();
			_worksheetNames = new Dictionary<string, int>();
			_worksheetNames.Add(ExcelRenderRes.SheetName.ToUpperInvariant(), 1);
			_mergedCells = new List<MergeInfo>();
			_hyperlinks = new List<LinkInfo>();
			_bookmarkLinks = new List<LinkInfo>();
			_unresolvedStreamsheets = new List<UnresolvedStreamsheet>();
			_bookmarkTargets = new Dictionary<string, BookmarkTargetInfo>();
			_cachedstyles = new Dictionary<string, Style>();
			_nextPaletteIndex = 0;
			_picturesStartingOnCurrentRow = new List<Picture>();
			_picturesToUpdateByEndRow = new Dictionary<int, List<Picture>>();
			_picturesToUpdateByStartColumn = new Dictionary<int, List<Picture>>();
			_picturesToUpdateByEndColumn = new Dictionary<int, List<Picture>>();
			_unresolvedPictureBookmarks = new List<PictureLinkInfo>();
		}

		private void CreateNewSheet()
		{
			_currentSheet = _workbook.Worksheets.CreateStreamsheet(ExcelRenderRes.SheetName + (_workbook.Worksheets.Count + 1).ToString(CultureInfo.InvariantCulture), _createTempStream);
			_currentSheet.ShowGridlines = false;
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

		private string GetUsablePortionOfHeaderFooterString(string candidate, ref int spaceRemaining)
		{
			if (string.IsNullOrEmpty(candidate) || spaceRemaining <= 2)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(candidate.Length);
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(candidate, stringBuilder);
			int num = Math.Min(spaceRemaining - 2, stringBuilder.Length);
			spaceRemaining -= num + 2;
			return stringBuilder.ToString(0, num);
		}

		private void FinalizeWorksheet()
		{
			FinalizeCell();
			for (int i = _currentRow.RowNumber; i < _maxRowIndex; i++)
			{
				_currentSheet.CreateRow().Height = 0.0;
			}
			_maxRowIndex = 0;
			foreach (MergeInfo mergedCell in _mergedCells)
			{
				_currentSheet.MergeCells(mergedCell.FirstRow, mergedCell.FirstColumn, mergedCell.RowCount, mergedCell.ColumnCount);
			}
			_mergedCells = new List<MergeInfo>();
			foreach (LinkInfo hyperlink in _hyperlinks)
			{
				_currentSheet.CreateHyperlink(hyperlink.AreaFormula, hyperlink.Href, hyperlink.Label);
			}
			_hyperlinks.Clear();
			for (int num = _bookmarkLinks.Count - 1; num >= 0; num--)
			{
				if (_bookmarkTargets.TryGetValue(_bookmarkLinks[num].Href, out BookmarkTargetInfo value))
				{
					_currentSheet.CreateHyperlink(_bookmarkLinks[num].AreaFormula, GetCellBookmarkLink(value), _bookmarkLinks[num].Label);
					_bookmarkLinks.RemoveAt(num);
				}
			}
			if (_bookmarkLinks.Count == 0)
			{
				_currentSheet.Cleanup();
			}
			else
			{
				_unresolvedStreamsheets.Add(new UnresolvedStreamsheet(_currentSheet, _bookmarkLinks));
				_bookmarkLinks = new List<LinkInfo>();
			}
			_picturesStartingOnCurrentRow.Clear();
			_picturesToUpdateByEndColumn.Clear();
			_picturesToUpdateByEndRow.Clear();
			_picturesToUpdateByStartColumn.Clear();
		}

		private void FinalizeCell()
		{
			if (_currentRow != null && !_currentCellHasBeenModified && (_currentStyle == null || !_currentStyle.HasBeenModified))
			{
				_currentRow.ClearCell(_currentColumn);
			}
			else
			{
				bool foundEastAsianChar = false;
				if (_currentRichText != null)
				{
					_currentRichText.Commit(_currentStyle);
					foundEastAsianChar = _currentRichText.FoundRotatedEastAsianChar;
					_currentRichText = null;
				}
				else if (_currentCell != null && _currentCell.ValueType == Cell.CellValueType.Text && _currentCell.Value != null)
				{
					string text = _currentCell.Value.ToString();
					if (text.Length > 0)
					{
						StringBuilder stringBuilder = new StringBuilder(text.Length);
						ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(text, stringBuilder, _checkForRotatedEastAsianChars, out foundEastAsianChar);
						_currentCell.Value = stringBuilder.ToString();
					}
				}
				if (foundEastAsianChar)
				{
					_currentStyle.Orientation = Orientation.Vertical;
				}
				if (_currentCell != null)
				{
					if (_currentStyle != null && (_currentCell.ValueType == Cell.CellValueType.Text || _currentCell.ValueType == Cell.CellValueType.Blank || _currentCell.ValueType == Cell.CellValueType.Error))
					{
						_currentStyle.NumberFormat = null;
					}
					_currentCell.Style = _currentStyle;
				}
				_currentStyle = null;
			}
			_checkForRotatedEastAsianChars = false;
		}

		private void LimitPercentage(ref double percentage)
		{
			if (percentage < 0.0)
			{
				percentage = 0.0;
			}
			if (percentage > 100.0)
			{
				percentage = 100.0;
			}
		}

		private double ValidMargin(double margin)
		{
			if (margin < 0.0)
			{
				return 0.0;
			}
			if (margin > 49.0)
			{
				return 49.0;
			}
			return margin;
		}

		private Style GetStyle()
		{
			return _workbook.CreateStyle();
		}

		private void AddPictureToUpdateCollection(ref Dictionary<int, List<Picture>> collection, int key, Picture value)
		{
			if (collection.TryGetValue(key, out List<Picture> value2))
			{
				if (value2 == null)
				{
					collection[key] = new List<Picture>();
					collection[key].Add(value);
				}
				else
				{
					value2.Add(value);
				}
			}
			else
			{
				collection[key] = new List<Picture>();
				collection[key].Add(value);
			}
		}

		private string GetCellBookmarkLink(BookmarkTargetInfo bookmark)
		{
			return string.Format(CultureInfo.InvariantCulture, "'{0}'!{1}", bookmark.Sheet, bookmark.Cell);
		}

		private string GetPictureBookmarkLink(BookmarkTargetInfo bookmark)
		{
			return string.Format(CultureInfo.InvariantCulture, "#{0}!{1}", bookmark.Sheet, bookmark.Cell);
		}

		public void NextWorksheet()
		{
			FinalizeWorksheet();
			CreateNewSheet();
		}

		public void SetCurrentSheetName(string name)
		{
			_currentSheet.Name = name;
		}

		public void AdjustFirstWorksheetName(string reportName, bool addedDocMap)
		{
			if (string.IsNullOrEmpty(reportName))
			{
				return;
			}
			Streamsheet streamsheet = null;
			if (addedDocMap)
			{
				if (_workbook.Worksheets.Count >= 2)
				{
					streamsheet = _workbook[1];
				}
			}
			else if (_workbook.Worksheets.Count >= 1)
			{
				streamsheet = _workbook[0];
			}
			if (streamsheet != null)
			{
				streamsheet.Name = ExcelGeneratorStringUtil.SanitizeSheetName(reportName).ToString();
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
			while (_worksheetNames.TryGetValue(pageName.ToUpperInvariant(), out value) || CheckForSheetNameConflict(pageName))
			{
				value++;
				_worksheetNames[pageName.ToUpperInvariant()] = value;
				string text = string.Format(CultureInfo.InvariantCulture, ExcelRenderRes.SheetNameCounterSuffix, value);
				if (pageName.Length + text.Length > 31)
				{
					pageName = pageName.Remove(31 - text.Length);
				}
				pageName += text;
			}
			_currentSheet.Name = pageName;
			_worksheetNames.Add(pageName.ToUpperInvariant(), 1);
		}

		public void SetSummaryRowAfter(bool after)
		{
			_currentSheet.PageSetup.SummaryRowsBelow = after;
		}

		public void SetSummaryColumnToRight(bool after)
		{
			_currentSheet.PageSetup.SummaryColumnsRight = after;
		}

		public void SetColumnProperties(int columnIndex, double widthInPoints, byte columnOutlineLevel, bool collapsed)
		{
			if (columnIndex > MaxColumns)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxColExceededInSheet(columnIndex.ToString(CultureInfo.InvariantCulture), MaxColumns.ToString(CultureInfo.InvariantCulture)));
			}
			ColumnProperties columnProperties = _currentSheet.GetColumnProperties(columnIndex);
			columnProperties.Width = widthInPoints;
			if (columnOutlineLevel > 7)
			{
				columnOutlineLevel = 7;
			}
			columnProperties.OutlineLevel = columnOutlineLevel;
			columnProperties.OutlineCollapsed = collapsed;
			columnProperties.Hidden = collapsed;
			if (_picturesToUpdateByStartColumn.TryGetValue(columnIndex, out List<Picture> value))
			{
				foreach (Picture item in value)
				{
					item.UpdateColumnOffset(widthInPoints, start: true);
				}
			}
			if (!_picturesToUpdateByEndColumn.TryGetValue(columnIndex, out value))
			{
				return;
			}
			foreach (Picture item2 in value)
			{
				item2.UpdateColumnOffset(widthInPoints, start: false);
			}
		}

		public void AddRow(int rowIndex)
		{
			if (rowIndex > MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(rowIndex.ToString(CultureInfo.InvariantCulture), MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			_maxRowIndex = Math.Max(_maxRowIndex, rowIndex);
		}

		public void SetRowProperties(int rowIndex, int heightIn20thPoints, byte rowOutlineLevel, bool collapsed, bool autoSize)
		{
			if (rowIndex > MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(rowIndex.ToString(CultureInfo.InvariantCulture), MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			double num = (double)heightIn20thPoints / 20.0;
			_currentRow.Height = num;
			_currentRow.CustomHeight = !autoSize;
			if (rowOutlineLevel > 7)
			{
				rowOutlineLevel = 7;
			}
			_currentRow.OutlineLevel = rowOutlineLevel;
			_currentRow.OutlineCollapsed = collapsed;
			_currentRow.Hidden = collapsed;
			foreach (Picture item in _picturesStartingOnCurrentRow)
			{
				item.UpdateRowOffset(num, start: true);
			}
			_picturesStartingOnCurrentRow.Clear();
			if (!_picturesToUpdateByEndRow.TryGetValue(rowIndex, out List<Picture> value))
			{
				return;
			}
			foreach (Picture item2 in value)
			{
				item2.UpdateRowOffset(num, start: false);
			}
		}

		public void SetRowContext(int rowIndex)
		{
			FinalizeCell();
			if (rowIndex > MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(rowIndex.ToString(CultureInfo.InvariantCulture), MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			_currentRow = _currentSheet.CreateRow(rowIndex);
		}

		public void SetColumnExtents(int min, int max)
		{
		}

		public void SetColumnContext(int columnIndex)
		{
			FinalizeCell();
			_currentCell = _currentRow[columnIndex];
			_currentColumn = columnIndex;
			_currentCellHasBeenModified = false;
		}

		public void SetCellValue(object value, TypeCode type)
		{
			string text = value as string;
			if (text != null && text.Length > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(_currentRow.RowNumber.ToString(CultureInfo.InvariantCulture), _currentColumn.ToString(CultureInfo.InvariantCulture)));
			}
			_currentCell.Value = value;
			_currentCellHasBeenModified = true;
		}

		public void SetCellError(ExcelErrorCode errorCode)
		{
			SetCellValue(errorCode, TypeCode.Empty);
			_currentCellHasBeenModified = true;
		}

		public void SetModifiedRotationForEastAsianChars(bool value)
		{
			_checkForRotatedEastAsianChars = value;
		}

		public IRichTextInfo GetCellRichTextInfo()
		{
			_currentRichText = new RichTextInfo(_currentCell, GetStyle, _currentRow.RowNumber, _currentColumn);
			GetCellStyle().WrapText = true;
			_currentCellHasBeenModified = true;
			return _currentRichText;
		}

		public IStyle GetCellStyle()
		{
			if (_cachedStyle != null)
			{
				return _cachedStyle;
			}
			if (_currentStyle == null)
			{
				_currentStyle = GetStyle();
			}
			return _currentStyle;
		}

		public TypeCode GetCellValueType()
		{
			Cell.CellValueType valueType = _currentCell.ValueType;
			if (valueType == Cell.CellValueType.Boolean)
			{
				return TypeCode.Boolean;
			}
			if (valueType == Cell.CellValueType.Date)
			{
				return TypeCode.DateTime;
			}
			if (valueType == Cell.CellValueType.Error)
			{
				return TypeCode.String;
			}
			if (valueType == Cell.CellValueType.Double)
			{
				return TypeCode.Double;
			}
			if (valueType == Cell.CellValueType.Integer)
			{
				return TypeCode.Int32;
			}
			return TypeCode.String;
		}

		public void AddImage(string imageName, Stream imageData, ImageFormat format, int rowStart, double rowStartPercentage, int columnStart, double columnStartPercentage, int rowEnd, double rowEndPercentage, int columnEnd, double colEndPercentage, string hyperlinkURL, bool isBookmarkLink)
		{
			if (imageData.CanSeek)
			{
				imageData.Seek(0L, SeekOrigin.Begin);
			}
			if (rowStart < 0)
			{
				rowStart = 0;
			}
			if (columnStart < 0)
			{
				columnStart = 0;
			}
			if (rowEnd < 0)
			{
				rowEnd = 0;
			}
			if (columnEnd < 0)
			{
				columnEnd = 0;
			}
			LimitPercentage(ref rowStartPercentage);
			LimitPercentage(ref columnStartPercentage);
			LimitPercentage(ref rowEndPercentage);
			LimitPercentage(ref colEndPercentage);
			Anchor startPosition = _currentSheet.CreateAnchor(rowStart, columnStart, columnStartPercentage, rowStartPercentage);
			Anchor endPosition = _currentSheet.CreateAnchor(rowEnd, columnEnd, colEndPercentage, rowEndPercentage);
			string extension = (format.Guid == ImageFormat.Bmp.Guid) ? "bmp" : ((format.Guid == ImageFormat.Gif.Guid) ? "gif" : ((!(format.Guid == ImageFormat.Jpeg.Guid)) ? "png" : "jpg"));
			string uniqueId = Convert.ToBase64String(new OfficeImageHasher(imageData).Hash);
			imageData.Seek(0L, SeekOrigin.Begin);
			Picture picture = _currentSheet.Pictures.CreatePicture(uniqueId, extension, imageData, startPosition, endPosition);
			_picturesStartingOnCurrentRow.Add(picture);
			AddPictureToUpdateCollection(ref _picturesToUpdateByEndRow, rowEnd, picture);
			AddPictureToUpdateCollection(ref _picturesToUpdateByStartColumn, columnStart, picture);
			AddPictureToUpdateCollection(ref _picturesToUpdateByEndColumn, columnEnd, picture);
			if (string.IsNullOrEmpty(hyperlinkURL))
			{
				return;
			}
			if (isBookmarkLink)
			{
				if (_bookmarkTargets.TryGetValue(hyperlinkURL, out BookmarkTargetInfo value))
				{
					picture.Hyperlink = GetPictureBookmarkLink(value);
				}
				else
				{
					_unresolvedPictureBookmarks.Add(new PictureLinkInfo(picture, hyperlinkURL));
				}
			}
			else
			{
				picture.Hyperlink = hyperlinkURL;
			}
		}

		public void AddMergeCell(int rowStart, int columnStart, int rowStop, int columnStop)
		{
			_mergedCells.Add(new MergeInfo(rowStart, rowStop, columnStart, columnStop));
		}

		public void AddHyperlink(string label, string reportUrl)
		{
			_hyperlinks.Add(new LinkInfo(_currentCell.Name, reportUrl, label));
		}

		public void AddHeader(string left, string center, string right)
		{
			int spaceRemaining = 255;
			_currentSheet.PageSetup.LeftHeader = GetUsablePortionOfHeaderFooterString(left, ref spaceRemaining);
			_currentSheet.PageSetup.CenterHeader = GetUsablePortionOfHeaderFooterString(center, ref spaceRemaining);
			_currentSheet.PageSetup.RightHeader = GetUsablePortionOfHeaderFooterString(right, ref spaceRemaining);
		}

		public void AddFooter(string left, string center, string right)
		{
			int spaceRemaining = 255;
			_currentSheet.PageSetup.LeftFooter = GetUsablePortionOfHeaderFooterString(left, ref spaceRemaining);
			_currentSheet.PageSetup.CenterFooter = GetUsablePortionOfHeaderFooterString(center, ref spaceRemaining);
			_currentSheet.PageSetup.RightFooter = GetUsablePortionOfHeaderFooterString(right, ref spaceRemaining);
		}

		public void AddFreezePane(int aRow, int aColumn)
		{
			_currentSheet.SetFreezePanes(aRow, aColumn);
		}

		public void AddBookmarkTarget(string value)
		{
			BookmarkTargetInfo bookmarkTargetInfo = new BookmarkTargetInfo(_currentSheet.Name, _currentCell.Name);
			string cellBookmarkLink = GetCellBookmarkLink(bookmarkTargetInfo);
			for (int num = _unresolvedStreamsheets.Count - 1; num >= 0; num--)
			{
				if (_unresolvedStreamsheets[num].ResolveTarget(value, cellBookmarkLink))
				{
					_unresolvedStreamsheets.RemoveAt(num);
				}
			}
			string pictureBookmarkLink = GetPictureBookmarkLink(bookmarkTargetInfo);
			for (int num2 = _unresolvedPictureBookmarks.Count - 1; num2 >= 0; num2--)
			{
				if (_unresolvedPictureBookmarks[num2].Target == value)
				{
					_unresolvedPictureBookmarks[num2].Picture.Hyperlink = pictureBookmarkLink;
					_unresolvedPictureBookmarks.RemoveAt(num2);
				}
			}
			_bookmarkTargets[value] = bookmarkTargetInfo;
		}

		public void AddBookmarkLink(string label, string link)
		{
			_bookmarkLinks.Add(new LinkInfo(_currentCell.Name, link, label));
		}

		public void AddBackgroundImage(byte[] data, string imageName, ref Stream backgroundImage, ref ushort backgroundImageWidth, ref ushort backgroundImageHeight)
		{
			using (MemoryStream pictureStream = new MemoryStream(data))
			{
				_currentSheet.SetBackgroundPicture(imageName, "png", pictureStream);
			}
		}

		public void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin)
		{
			PageSetup pageSetup = _currentSheet.PageSetup;
			pageSetup.HeaderMargin = ValidMargin(headerMargin);
			pageSetup.FooterMargin = ValidMargin(footerMargin);
			pageSetup.Orientation = (isPortrait ? PageSetup.PageOrientation.Portrait : PageSetup.PageOrientation.Landscape);
			pageSetup.PaperSize = PageSetup.PagePaperSize.findByValue(paperSize);
		}

		public void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			PageSetup pageSetup = _currentSheet.PageSetup;
			pageSetup.TopMargin = ValidMargin(topMargin);
			pageSetup.BottomMargin = ValidMargin(bottomMargin);
			pageSetup.LeftMargin = ValidMargin(leftMargin);
			pageSetup.RightMargin = ValidMargin(rightMargin);
		}

		public IColor AddColor(string colorString)
		{
			System.Drawing.Color color = LayoutConvert.ToColor(colorString);
			if (_nextPaletteIndex < 56)
			{
				int colorIndex = _workbook.Palette.GetColorIndex(color.R, color.G, color.B);
				if (colorIndex == -1)
				{
					_workbook.Palette.SetColorAt(_nextPaletteIndex, color.R, color.G, color.B);
					_nextPaletteIndex++;
				}
				else if (colorIndex >= _nextPaletteIndex)
				{
					if (colorIndex == _nextPaletteIndex)
					{
						_nextPaletteIndex++;
					}
					else
					{
						Color colorAt = _workbook.Palette.GetColorAt(colorIndex);
						Color colorAt2 = _workbook.Palette.GetColorAt(_nextPaletteIndex);
						_workbook.Palette.SetColorAt(colorIndex, colorAt2.Red, colorAt2.Green, colorAt2.Blue);
						_workbook.Palette.SetColorAt(_nextPaletteIndex, colorAt.Red, colorAt.Green, colorAt.Blue);
						_nextPaletteIndex++;
					}
				}
			}
			return _workbook.Palette.GetColor(color);
		}

		public void SaveSpreadsheet(Stream outputStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight)
		{
			FinalizeWorksheet();
			_application.Save(_workbook);
			byte[] buffer = new byte[1024];
			long num = _workbookStream.Length;
			_workbookStream.Seek(0L, SeekOrigin.Begin);
			while (num > 0)
			{
				int num2 = _workbookStream.Read(buffer, 0, 1024);
				num -= num2;
				outputStream.Write(buffer, 0, num2);
			}
		}

		public Stream CreateStream(string name)
		{
			return _createTempStream(name);
		}

		public bool UseCachedStyle(string id)
		{
			if (_cachedstyles.TryGetValue(id, out Style value))
			{
				_currentStyle = value;
				return true;
			}
			return false;
		}

		public void DefineCachedStyle(string id)
		{
			_cachedStyle = _workbook.CreateStyle();
			_cachedstyles.Add(id, _cachedStyle);
		}

		public void EndCachedStyle()
		{
			_cachedStyle = null;
		}

		public void AddPrintTitle(int rowStart, int rowEnd)
		{
			if (rowStart < 0 || rowStart > MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.ValueOutOfRange(0.ToString(CultureInfo.InvariantCulture), MaxRows.ToString(CultureInfo.InvariantCulture), rowStart.ToString(CultureInfo.InvariantCulture)));
			}
			if (rowEnd < 0 || rowEnd > MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.ValueOutOfRange(0.ToString(CultureInfo.InvariantCulture), MaxRows.ToString(CultureInfo.InvariantCulture), rowEnd.ToString(CultureInfo.InvariantCulture)));
			}
			_currentSheet.PageSetup.SetPrintTitleToRows(rowStart, rowEnd);
		}

		public void BuildHeaderFooterString(StringBuilder str, RPLTextBoxProps textBox, ref string lastFont, ref double lastFontSize)
		{
			RPLElementStyle style = textBox.Style;
			string text = (string)style[20];
			str.Append("&").Append("\"");
			if (!string.IsNullOrEmpty(text) && !text.Equals(lastFont))
			{
				FormulaHandler.EncodeHeaderFooterString(str, text);
				lastFont = text;
			}
			else
			{
				str.Append("-");
			}
			str.Append(",");
			bool flag = false;
			bool flag2 = false;
			object obj = textBox.Style[22];
			if (obj != null && LayoutConvert.ToFontWeight((RPLFormat.FontWeights)obj) >= 600)
			{
				str.Append("Bold");
				flag = true;
			}
			object obj2 = style[19];
			if (obj2 != null && (RPLFormat.FontStyles)obj2 == RPLFormat.FontStyles.Italic)
			{
				if (flag)
				{
					str.Append(" ");
				}
				str.Append("Italic");
				flag2 = true;
			}
			if (!(flag || flag2))
			{
				str.Append("Regular");
			}
			str.Append("\"");
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
			object obj3 = textBox.Style[24];
			if (obj3 != null)
			{
				RPLFormat.TextDecorations num2 = (RPLFormat.TextDecorations)obj3;
				if (num2 == RPLFormat.TextDecorations.Underline)
				{
					str.Append("&u");
				}
				if (num2 == RPLFormat.TextDecorations.LineThrough)
				{
					str.Append("&s");
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
			str.Append(" ");
		}
	}
}
