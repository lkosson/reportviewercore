using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal sealed class StreamsheetParser : WorksheetPart
	{
		private delegate void WriteDelegate(TextWriter s, int depth, Dictionary<string, string> namespaces);

		internal enum StreamSheetSection
		{
			Prelude,
			SheetPr,
			SheetViews,
			SheetFormatPr,
			Cols,
			SheetData,
			MergeCells,
			Hyperlinks,
			PageMargins,
			PageSetup,
			HeaderFooter,
			Drawing,
			Picture,
			AfterWord
		}

		private const int ROOT_DEPTH = 0;

		private readonly WriteDelegate[] _sectionsInOrder;

		private readonly CT_Worksheet _worksheet;

		private StreamSheetSection _currentSection;

		private string _pendingCloseTag;

		private TextWriter _output;

		public StreamSheetSection CurrentSection => _currentSection;

		public StreamsheetParser(TextWriter output, bool startAtPrelude)
		{
			_output = output;
			_worksheet = (CT_Worksheet)Root;
			_sectionsInOrder = new WriteDelegate[14]
			{
				WritePrelude,
				_worksheet.Write_sheetPr,
				_worksheet.Write_sheetViews,
				_worksheet.Write_sheetFormatPr,
				_worksheet.Write_cols,
				_worksheet.Write_sheetData,
				_worksheet.Write_mergeCells,
				_worksheet.Write_hyperlinks,
				_worksheet.Write_pageMargins,
				_worksheet.Write_pageSetup,
				_worksheet.Write_headerFooter,
				_worksheet.Write_drawing,
				_worksheet.Write_picture,
				WriteAfterword
			};
			_pendingCloseTag = null;
			if (startAtPrelude)
			{
				_currentSection = StreamSheetSection.Prelude;
				return;
			}
			TryMoveToSection(StreamSheetSection.SheetData, 1);
			_pendingCloseTag = CT_Worksheet.SheetDataElementName;
			new CT_SheetData().WriteOpenTag(_output, CT_Worksheet.SheetDataElementName, 1, Namespaces, root: false);
		}

		private void TryMoveToSection(StreamSheetSection section, int depth)
		{
			if (_currentSection != section)
			{
				if (section <= _currentSection)
				{
					throw new FatalException();
				}
				FinishCurrentTag();
				FastForward(section, depth);
			}
		}

		private void FastForward(StreamSheetSection section, int depth)
		{
			for (int i = (int)(_currentSection + 1); i <= (int)(section - 1); i++)
			{
				_sectionsInOrder[i](_output, depth, Namespaces);
			}
			_currentSection = section;
		}

		public void FinishCurrentTag()
		{
			if (_pendingCloseTag != null)
			{
				_output.Write(string.Format(CultureInfo.InvariantCulture, "</{0}>", _pendingCloseTag));
				_pendingCloseTag = null;
			}
			_output.Flush();
		}

		public void WritePrelude()
		{
			WritePrelude(_output, 0, Namespaces);
		}

		private void WritePrelude(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			TryMoveToSection(StreamSheetSection.Prelude, depth);
			s.Write(OoxmlPart.XmlDeclaration);
			_worksheet.WriteOpenTag(s, Tag, 0, Namespaces, root: true);
		}

		public void WriteAfterword()
		{
			WriteAfterword(_output, 0, Namespaces);
			_output.Flush();
		}

		private void WriteAfterword(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			TryMoveToSection(StreamSheetSection.AfterWord, depth);
			_worksheet.WriteCloseTag(s, Tag, 0, Namespaces);
		}

		private void WriteTagInCollection<TCol>(OoxmlComplexType tag, StreamSheetSection section, string tagname, string coltagname) where TCol : OoxmlComplexType, new()
		{
			if (tag != null)
			{
				if (_currentSection != section)
				{
					TryMoveToSection(section, 1);
					_pendingCloseTag = coltagname;
					new TCol().WriteOpenTag(_output, coltagname, 1, Namespaces, root: false);
				}
				tag.Write(_output, tagname, 2, Namespaces);
			}
		}

		private void WriteStandaloneTag(OoxmlComplexType tag, StreamSheetSection section, string tagname)
		{
			if (tag != null)
			{
				if (_currentSection != section)
				{
					TryMoveToSection(section, 1);
				}
				tag.Write(_output, tagname, 1, Namespaces);
			}
		}

		public void WriteSheetProperties(CT_SheetPr props)
		{
			WriteStandaloneTag(props, StreamSheetSection.SheetPr, CT_Worksheet.SheetPrElementName);
		}

		public void WriteSheetView(CT_SheetView view)
		{
			WriteTagInCollection<CT_SheetViews>(view, StreamSheetSection.SheetViews, CT_SheetViews.SheetViewElementName, CT_Worksheet.SheetViewsElementName);
		}

		public void WriteSheetFormatProperties(CT_SheetFormatPr props)
		{
			WriteStandaloneTag(props, StreamSheetSection.SheetFormatPr, CT_Worksheet.SheetFormatPrElementName);
		}

		public void WriteColumnProperty(CT_Col prop)
		{
			WriteTagInCollection<CT_Cols>(prop, StreamSheetSection.Cols, CT_Cols.ColElementName, CT_Worksheet.ColsElementName);
		}

		public void WriteRow(CT_Row row)
		{
			WriteTagInCollection<CT_SheetData>(row, StreamSheetSection.SheetData, CT_SheetData.RowElementName, CT_Worksheet.SheetDataElementName);
		}

		public void WriteMergedCell(CT_MergeCell merge)
		{
			WriteTagInCollection<CT_MergeCells>(merge, StreamSheetSection.MergeCells, CT_MergeCells.MergeCellElementName, CT_Worksheet.MergeCellsElementName);
		}

		public void WriteHyperlink(CT_Hyperlink link)
		{
			WriteTagInCollection<CT_Hyperlinks>(link, StreamSheetSection.Hyperlinks, CT_Hyperlinks.HyperlinkElementName, CT_Worksheet.HyperlinksElementName);
		}

		public void WritePageMargins(CT_PageMargins margins)
		{
			WriteStandaloneTag(margins, StreamSheetSection.PageMargins, CT_Worksheet.PageMarginsElementName);
		}

		public void WritePageSetup(CT_PageSetup setup)
		{
			WriteStandaloneTag(setup, StreamSheetSection.PageSetup, CT_Worksheet.PageSetupElementName);
		}

		public void WriteHeaderFooter(CT_HeaderFooter hf)
		{
			WriteStandaloneTag(hf, StreamSheetSection.HeaderFooter, CT_Worksheet.HeaderFooterElementName);
		}

		public void WriteDrawing(CT_Drawing drawing)
		{
			WriteStandaloneTag(drawing, StreamSheetSection.Drawing, CT_Worksheet.DrawingElementName);
		}

		public void WritePicture(CT_SheetBackgroundPicture picture)
		{
			WriteStandaloneTag(picture, StreamSheetSection.Picture, CT_Worksheet.PictureElementName);
		}

		public void WriteTag(OoxmlComplexType tag)
		{
			if (tag == null)
			{
				return;
			}
			if (tag is CT_SheetPr)
			{
				WriteSheetProperties((CT_SheetPr)tag);
				return;
			}
			if (tag is CT_SheetView)
			{
				WriteSheetView((CT_SheetView)tag);
				return;
			}
			if (tag is CT_SheetFormatPr)
			{
				WriteSheetFormatProperties((CT_SheetFormatPr)tag);
				return;
			}
			if (tag is CT_Col)
			{
				WriteColumnProperty((CT_Col)tag);
				return;
			}
			if (tag is CT_Row)
			{
				WriteRow((CT_Row)tag);
				return;
			}
			if (tag is CT_MergeCell)
			{
				WriteMergedCell((CT_MergeCell)tag);
				return;
			}
			if (tag is CT_Hyperlink)
			{
				WriteHyperlink((CT_Hyperlink)tag);
				return;
			}
			if (tag is CT_PageMargins)
			{
				WritePageMargins((CT_PageMargins)tag);
				return;
			}
			if (tag is CT_PageSetup)
			{
				WritePageSetup((CT_PageSetup)tag);
				return;
			}
			if (tag is CT_HeaderFooter)
			{
				WriteHeaderFooter((CT_HeaderFooter)tag);
				return;
			}
			if (tag is CT_Drawing)
			{
				WriteDrawing((CT_Drawing)tag);
				return;
			}
			if (tag is CT_SheetBackgroundPicture)
			{
				WritePicture((CT_SheetBackgroundPicture)tag);
				return;
			}
			throw new FatalException();
		}
	}
}
