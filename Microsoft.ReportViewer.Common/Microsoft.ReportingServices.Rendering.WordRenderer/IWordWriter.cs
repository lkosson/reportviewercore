using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal interface IWordWriter : IDisposable
	{
		int SectionCount
		{
			get;
		}

		bool HasTitlePage
		{
			set;
		}

		bool CanBand
		{
			get;
		}

		AutoFit AutoFit
		{
			get;
			set;
		}

		void Init(CreateAndRegisterStream createAndRegisterStream, AutoFit autoFit, string reportName);

		void InitHeaderFooter();

		void FinishHeader(int section);

		void FinishFooter(int section);

		void FinishHeader(int section, Word97Writer.HeaderFooterLocation location);

		void FinishFooter(int section, Word97Writer.HeaderFooterLocation location);

		void FinishHeaderFooterRegion(int section, int index);

		void FinishHeadersFooters(bool hasTitlePage);

		void StartHeader();

		void StartHeader(bool firstPage);

		void FinishHeader();

		void StartFooter();

		void StartFooter(bool firstPage);

		void FinishFooter();

		void SetPageDimensions(float pageHeight, float pageWidth, float leftMargin, float rightMargin, float topMargin, float bottomMargin);

		void AddImage(byte[] imgBuf, float height, float width, RPLFormat.Sizings sizing);

		void WriteText(string text);

		void WriteHyperlinkBegin(string target, bool bookmarkLink);

		void WriteHyperlinkEnd();

		void AddTableStyleProp(byte code, object value);

		void SetTableContext(BorderContext borderContext);

		void AddBodyStyleProp(byte code, object value);

		void AddCellStyleProp(int cellIndex, byte code, object value);

		void AddPadding(int cellIndex, byte code, object value, int defaultValue);

		void ApplyCellBorderContext(BorderContext borderContext);

		void AddTextStyleProp(byte code, object value);

		void AddFirstLineIndent(float indent);

		void AddLeftIndent(float margin);

		void AddRightIndent(float margin);

		void AddSpaceBefore(float space);

		void AddSpaceAfter(float space);

		void RenderTextRunDirection(RPLFormat.Directions direction);

		void RenderTextAlign(TypeCode type, RPLFormat.TextAlignments textAlignments, RPLFormat.Directions direction);

		void RenderFontWeight(RPLFormat.FontWeights fontWeights, RPLFormat.Directions dir);

		void RenderFontWeight(RPLFormat.FontWeights? fontWeights, RPLFormat.Directions dir);

		void RenderFontSize(string size, RPLFormat.Directions dir);

		void RenderFontFamily(string font, RPLFormat.Directions dir);

		void RenderFontStyle(RPLFormat.FontStyles value, RPLFormat.Directions dir);

		void RenderFontStyle(RPLFormat.FontStyles? value, RPLFormat.Directions dir);

		void WriteParagraphEnd();

		void WriteListEnd(int level, RPLFormat.ListStyles listStyle, bool endParagraph);

		void InitListLevels();

		void ResetListlevels();

		void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell);

		void WriteEmptyStyle();

		void WriteTableBegin(float left, bool layoutTable);

		void WriteTableRowBegin(float left, float height, float[] columnWidths);

		void IgnoreRowHeight(bool canGrow);

		void SetWriteExactRowHeight(bool writeExactRowHeight);

		void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge);

		void WriteTableRowEnd();

		void WriteTableEnd();

		void Finish(string title, string author, string comments);

		int WriteFont(string fontName);

		void RenderBookmark(string name);

		void RenderLabel(string label, int level);

		void WritePageNumberField();

		void WriteTotalPagesField();

		void AddListStyle(int level, bool bulleted);

		void WriteCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp);

		void WritePageBreak();

		void WriteEndSection();

		void ClearCellBorder(TableData.Positions position);
	}
}
