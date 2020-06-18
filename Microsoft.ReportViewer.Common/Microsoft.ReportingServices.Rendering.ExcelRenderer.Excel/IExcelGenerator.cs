using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel
{
	internal interface IExcelGenerator
	{
		int MaxRows
		{
			get;
		}

		int MaxColumns
		{
			get;
		}

		int RowBlockSize
		{
			get;
		}

		void NextWorksheet();

		void SetCurrentSheetName(string name);

		void AdjustFirstWorksheetName(string reportName, bool addedDocMap);

		void GenerateWorksheetName(string pageName);

		void SetSummaryRowAfter(bool after);

		void SetSummaryColumnToRight(bool after);

		void SetColumnProperties(int columnIndex, double widthInPoints, byte columnOutlineLevel, bool collapsed);

		void AddRow(int rowIndex);

		void SetRowProperties(int rowIndex, int heightIn20thPoints, byte rowOutlineLevel, bool collapsed, bool autoSize);

		void SetRowContext(int rowIndex);

		void SetColumnExtents(int min, int max);

		void SetColumnContext(int columnIndex);

		void SetCellValue(object value, TypeCode type);

		void SetCellError(ExcelErrorCode errorCode);

		void SetModifiedRotationForEastAsianChars(bool value);

		IRichTextInfo GetCellRichTextInfo();

		IStyle GetCellStyle();

		TypeCode GetCellValueType();

		void AddImage(string imageName, Stream imageData, ImageFormat format, int rowStart, double rowStartPercentage, int columnStart, double columnStartPercentage, int rowEnd, double rowEndPercentage, int columnEnd, double colEndPercentage, string hyperlinkURL, bool isBookmarkLink);

		void AddMergeCell(int rowStart, int columnStart, int rowStop, int columnStop);

		void AddHyperlink(string label, string reportURL);

		void AddHeader(string left, string center, string right);

		void AddFooter(string left, string center, string right);

		void AddFreezePane(int aRow, int aColumn);

		void AddBookmarkTarget(string value);

		void AddBookmarkLink(string label, string link);

		void AddBackgroundImage(byte[] data, string imageName, ref Stream backgroundImage, ref ushort backgroundImageWidth, ref ushort backgroundImageHeight);

		void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin);

		void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin);

		IColor AddColor(string colorString);

		void SaveSpreadsheet(Stream outputStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight);

		Stream CreateStream(string name);

		bool UseCachedStyle(string id);

		void DefineCachedStyle(string id);

		void EndCachedStyle();

		void AddPrintTitle(int rowStart, int rowEnd);

		void BuildHeaderFooterString(StringBuilder str, RPLTextBoxProps textBox, ref string lastFont, ref double lastFontSize);
	}
}
