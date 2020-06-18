using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Archive;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStreamsheetModel : XMLWorksheetModel, IStreamsheetModel, IWorksheetModel, ICloneable
	{
		private readonly Streamsheet _interface;

		private string _partName;

		private Stream _headStream;

		private Stream _tailStream;

		private StreamsheetParser _headWriter;

		private StreamsheetParser _tailWriter;

		private bool _complete;

		private int _currentColNumber = -1;

		private int _currentRowNumber = -1;

		private IOoxmlCtWrapperModel _stagedHeadTag;

		private IOoxmlCtWrapperModel _stagedTailTag;

		private Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main.CT_Drawing _stagedDrawingTag;

		private CT_SheetBackgroundPicture _stagedSheetBackgroundTag;

		private XMLPageSetupModel _stagedPageSetupModel;

		private CT_SheetView _stagedSheetView;

		private static readonly object _writeLock = new object();

		public override Streamsheet Interface => _interface;

		public int MaxRowIndex => 1048575;

		public int MaxColIndex => 16383;

		public override IPictureShapesModel Pictures
		{
			get
			{
				if (PicturesModel == null)
				{
					if (_tailWriter.CurrentSection > StreamsheetParser.StreamSheetSection.Drawing)
					{
						throw new FatalException();
					}
					WsDrPart part = new WsDrPart();
					Relationship relationship = _manager.AddPartToTree(part, "application/vnd.openxmlformats-officedocument.drawing+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing", "xl/drawings/drawing{0}.xml", _manager.GetWorksheetXmlPart(_sheetentry));
					_stagedDrawingTag = new Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main.CT_Drawing();
					_stagedDrawingTag.Id_Attr = relationship.RelationshipId;
					PicturesModel = new XMLPictureShapesModel(_manager, _sheetentry, relationship.RelationshipId);
				}
				return PicturesModel;
			}
		}

		public override IPageSetupModel PageSetup
		{
			get
			{
				if (_tailWriter.CurrentSection > StreamsheetParser.StreamSheetSection.HeaderFooter)
				{
					throw new FatalException();
				}
				if (_stagedPageSetupModel == null)
				{
					_stagedPageSetupModel = new XMLPageSetupModel(new CT_Worksheet(), this);
				}
				return _stagedPageSetupModel;
			}
		}

		public override bool ShowGridlines
		{
			set
			{
				if (_headWriter.CurrentSection > StreamsheetParser.StreamSheetSection.SheetViews)
				{
					throw new FatalException();
				}
				if (_stagedSheetView == null)
				{
					_stagedSheetView = new CT_SheetView();
				}
				_stagedSheetView.ShowGridLines_Attr = value;
			}
		}

		public XMLStreamsheetModel(XMLWorkbookModel workbook, XMLWorksheetsModel sheets, PartManager manager, string name, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			_workbookModel = workbook;
			_worksheetsModel = sheets;
			_manager = manager;
			_interface = new Streamsheet(this);
			Relationship relationship = _manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "xl/worksheets/sheet{0}.xml", (XmlPart)_manager.GetPartByContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"));
			List<CT_Sheet> sheet = ((CT_Workbook)_manager.Workbook.Root).Sheets.Sheet;
			_sheetentry = new CT_Sheet();
			_sheetentry.Id_Attr = relationship.RelationshipId;
			_sheetentry.Name_Attr = name;
			_sheetentry.SheetId_Attr = sheets.NextId;
			sheet.Add(_sheetentry);
			_partName = relationship.RelatedPart;
			_headStream = createTempStream(string.Format(CultureInfo.InvariantCulture, "streamsheetHead{0}", _sheetentry.SheetId_Attr));
			_tailStream = createTempStream(string.Format(CultureInfo.InvariantCulture, "streamsheetTail{0}", _sheetentry.SheetId_Attr));
			_headWriter = new StreamsheetParser(new StreamWriter(_headStream), startAtPrelude: true);
			_tailWriter = new StreamsheetParser(new StreamWriter(_tailStream), startAtPrelude: false);
			_headWriter.WritePrelude();
		}

		public override IColumnModel getColumn(int index)
		{
			AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.Cols, head: true);
			if (index <= _currentColNumber)
			{
				throw new FatalException();
			}
			_currentColNumber = index;
			return (IColumnModel)(_stagedHeadTag = new XMLColumnModel(new CT_Col(), index + 1));
		}

		public IRowModel CreateRow()
		{
			return CreateRow(_currentRowNumber + 1);
		}

		public IRowModel CreateRow(int index)
		{
			AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.SheetData, head: false);
			if (index <= _currentRowNumber)
			{
				throw new FatalException();
			}
			_currentRowNumber = index;
			return (IRowModel)(_stagedTailTag = new XMLRowModel(this, _manager, index));
		}

		public void MergeCells(int firstRow, int firstCol, int rowCount, int colCount)
		{
			AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.MergeCells, head: false);
			CT_MergeCell cT_MergeCell = new CT_MergeCell();
			cT_MergeCell._ref_Attr = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", CellPair.Name(firstRow, firstCol), CellPair.Name(firstRow + rowCount - 1, firstCol + colCount - 1));
			_tailWriter.WriteMergedCell(cT_MergeCell);
		}

		public void CreateHyperlink(string areaFormula, string href, string label)
		{
			AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.Hyperlinks, head: false);
			XMLHyperlinkModel xMLHyperlinkModel = (XMLHyperlinkModel)(_stagedTailTag = new XMLHyperlinkModel(areaFormula, href, label, _manager, _sheetentry));
		}

		public override void SetBackgroundPicture(string uniqueId, string extension, Stream pictureStream)
		{
			if (_tailWriter.CurrentSection > StreamsheetParser.StreamSheetSection.Picture)
			{
				throw new FatalException();
			}
			_stagedSheetBackgroundTag = new CT_SheetBackgroundPicture();
			_stagedSheetBackgroundTag.Id_Attr = InsertBackgroundPicture(uniqueId, extension, pictureStream);
		}

		public void SetFreezePanes(int row, int col)
		{
			if (_headWriter.CurrentSection > StreamsheetParser.StreamSheetSection.SheetViews)
			{
				throw new FatalException();
			}
			if (_stagedSheetView == null)
			{
				_stagedSheetView = new CT_SheetView();
			}
			_stagedSheetView.Pane = new CT_Pane();
			_stagedSheetView.Pane = new CT_Pane();
			_stagedSheetView.Pane.State_Attr = ST_PaneState.frozen;
			_stagedSheetView.Pane.XSplit_Attr = col;
			_stagedSheetView.Pane.YSplit_Attr = row;
			_stagedSheetView.Pane.TopLeftCell_Attr = CellPair.Name(row, col);
		}

		public override void Cleanup()
		{
			if (!_complete)
			{
				AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.AfterWord, head: false);
				AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.Cols, head: true);
				_tailWriter.WriteAfterword();
				_headWriter.FinishCurrentTag();
				_tailStream.Seek(0L, SeekOrigin.Begin);
				_headStream.Seek(0L, SeekOrigin.Begin);
				Stream stream = ((XMLStreambookModel)base.Workbook).ZipPackage.GetPart(new Uri(Utils.CleanName(_partName), UriKind.Relative)).GetStream();
				lock (_writeLock)
				{
					WriteStreamToStream(_headStream, stream);
					WriteStreamToStream(_tailStream, stream);
				}
				_complete = true;
			}
		}

		private void WriteStreamToStream(Stream from, Stream to)
		{
			byte[] buffer = new byte[1024];
			long num = from.Length;
			while (num > 0)
			{
				int num2 = from.Read(buffer, 0, 1024);
				num -= num2;
				to.Write(buffer, 0, num2);
			}
		}

		private void CheckAdvancingStatus(StreamsheetParser.StreamSheetSection newSection, StreamsheetParser writer)
		{
			if (_complete)
			{
				throw new FatalException();
			}
			if (writer.CurrentSection > newSection)
			{
				throw new FatalException();
			}
		}

		private bool IsGoingPast(StreamsheetParser.StreamSheetSection currentSection, StreamsheetParser.StreamSheetSection newSection, StreamsheetParser.StreamSheetSection targetsection)
		{
			if (targetsection >= currentSection)
			{
				return newSection > targetsection;
			}
			return false;
		}

		private void AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection newSection, bool head)
		{
			StreamsheetParser streamsheetParser = head ? _headWriter : _tailWriter;
			IOoxmlCtWrapperModel ooxmlCtWrapperModel = head ? _stagedHeadTag : _stagedTailTag;
			CheckAdvancingStatus(newSection, streamsheetParser);
			if (ooxmlCtWrapperModel != null)
			{
				ooxmlCtWrapperModel.Cleanup();
				streamsheetParser.WriteTag(ooxmlCtWrapperModel.OoxmlTag);
				if (head)
				{
					_stagedHeadTag = null;
				}
				else
				{
					_stagedTailTag = null;
				}
			}
			if (_stagedPageSetupModel != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.SheetPr))
			{
				_stagedPageSetupModel.Cleanup();
				streamsheetParser.WriteSheetProperties(_stagedPageSetupModel.BackingSheet.SheetPr);
			}
			if (_stagedSheetView != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.SheetViews))
			{
				streamsheetParser.WriteSheetView(_stagedSheetView);
				_stagedSheetView = null;
			}
			if (_stagedPageSetupModel != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.PageMargins))
			{
				_stagedPageSetupModel.Cleanup();
				streamsheetParser.WritePageMargins(_stagedPageSetupModel.BackingSheet.PageMargins);
			}
			if (_stagedPageSetupModel != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.PageSetup))
			{
				_stagedPageSetupModel.Cleanup();
				streamsheetParser.WritePageSetup(_stagedPageSetupModel.BackingSheet.PageSetup);
			}
			if (_stagedPageSetupModel != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.HeaderFooter))
			{
				_stagedPageSetupModel.Cleanup();
				if (_stagedPageSetupModel.BackingSheet.HeaderFooter == null)
				{
					_stagedPageSetupModel.BackingSheet.HeaderFooter = new CT_HeaderFooter();
				}
				_stagedPageSetupModel.BackingSheet.HeaderFooter.AlignWithMargins_Attr = false;
				streamsheetParser.WriteHeaderFooter(_stagedPageSetupModel.BackingSheet.HeaderFooter);
			}
			if (_stagedDrawingTag != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.Drawing))
			{
				streamsheetParser.WriteTag(_stagedDrawingTag);
				_stagedDrawingTag = null;
			}
			if (_stagedSheetBackgroundTag != null && IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.Picture))
			{
				streamsheetParser.WriteTag(_stagedSheetBackgroundTag);
				_stagedSheetBackgroundTag = null;
			}
			CheckAdvancingStatus(newSection, streamsheetParser);
		}
	}
}
