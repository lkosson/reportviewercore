using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Worksheet : OoxmlComplexType
	{
		private CT_SheetPr _sheetPr;

		private CT_SheetViews _sheetViews;

		private CT_SheetFormatPr _sheetFormatPr;

		private CT_SheetData _sheetData;

		private CT_MergeCells _mergeCells;

		private CT_Hyperlinks _hyperlinks;

		private CT_PageMargins _pageMargins;

		private CT_PageSetup _pageSetup;

		private CT_HeaderFooter _headerFooter;

		private CT_Drawing _drawing;

		private CT_SheetBackgroundPicture _picture;

		private List<CT_Cols> _cols;

		public CT_SheetPr SheetPr
		{
			get
			{
				return _sheetPr;
			}
			set
			{
				_sheetPr = value;
			}
		}

		public CT_SheetViews SheetViews
		{
			get
			{
				return _sheetViews;
			}
			set
			{
				_sheetViews = value;
			}
		}

		public CT_SheetFormatPr SheetFormatPr
		{
			get
			{
				return _sheetFormatPr;
			}
			set
			{
				_sheetFormatPr = value;
			}
		}

		public CT_SheetData SheetData
		{
			get
			{
				return _sheetData;
			}
			set
			{
				_sheetData = value;
			}
		}

		public CT_MergeCells MergeCells
		{
			get
			{
				return _mergeCells;
			}
			set
			{
				_mergeCells = value;
			}
		}

		public CT_Hyperlinks Hyperlinks
		{
			get
			{
				return _hyperlinks;
			}
			set
			{
				_hyperlinks = value;
			}
		}

		public CT_PageMargins PageMargins
		{
			get
			{
				return _pageMargins;
			}
			set
			{
				_pageMargins = value;
			}
		}

		public CT_PageSetup PageSetup
		{
			get
			{
				return _pageSetup;
			}
			set
			{
				_pageSetup = value;
			}
		}

		public CT_HeaderFooter HeaderFooter
		{
			get
			{
				return _headerFooter;
			}
			set
			{
				_headerFooter = value;
			}
		}

		public CT_Drawing Drawing
		{
			get
			{
				return _drawing;
			}
			set
			{
				_drawing = value;
			}
		}

		public CT_SheetBackgroundPicture Picture
		{
			get
			{
				return _picture;
			}
			set
			{
				_picture = value;
			}
		}

		public List<CT_Cols> Cols
		{
			get
			{
				return _cols;
			}
			set
			{
				_cols = value;
			}
		}

		public static string SheetPrElementName => "sheetPr";

		public static string SheetViewsElementName => "sheetViews";

		public static string SheetFormatPrElementName => "sheetFormatPr";

		public static string SheetDataElementName => "sheetData";

		public static string MergeCellsElementName => "mergeCells";

		public static string HyperlinksElementName => "hyperlinks";

		public static string PageMarginsElementName => "pageMargins";

		public static string PageSetupElementName => "pageSetup";

		public static string HeaderFooterElementName => "headerFooter";

		public static string DrawingElementName => "drawing";

		public static string PictureElementName => "picture";

		public static string ColsElementName => "cols";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_sheetData = new CT_SheetData();
		}

		protected override void InitCollections()
		{
			_cols = new List<CT_Cols>();
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: true);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: false);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_sheetPr(s, depth, namespaces);
			Write_sheetViews(s, depth, namespaces);
			Write_sheetFormatPr(s, depth, namespaces);
			Write_cols(s, depth, namespaces);
			Write_sheetData(s, depth, namespaces);
			Write_mergeCells(s, depth, namespaces);
			Write_hyperlinks(s, depth, namespaces);
			Write_pageMargins(s, depth, namespaces);
			Write_pageSetup(s, depth, namespaces);
			Write_headerFooter(s, depth, namespaces);
			Write_drawing(s, depth, namespaces);
			Write_picture(s, depth, namespaces);
		}

		public void Write_sheetPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_sheetPr != null)
			{
				_sheetPr.Write(s, "sheetPr", depth + 1, namespaces);
			}
		}

		public void Write_sheetViews(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_sheetViews != null)
			{
				_sheetViews.Write(s, "sheetViews", depth + 1, namespaces);
			}
		}

		public void Write_sheetFormatPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_sheetFormatPr != null)
			{
				_sheetFormatPr.Write(s, "sheetFormatPr", depth + 1, namespaces);
			}
		}

		public void Write_sheetData(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_sheetData != null)
			{
				_sheetData.Write(s, "sheetData", depth + 1, namespaces);
			}
		}

		public void Write_mergeCells(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_mergeCells != null)
			{
				_mergeCells.Write(s, "mergeCells", depth + 1, namespaces);
			}
		}

		public void Write_hyperlinks(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_hyperlinks != null)
			{
				_hyperlinks.Write(s, "hyperlinks", depth + 1, namespaces);
			}
		}

		public void Write_pageMargins(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_pageMargins != null)
			{
				_pageMargins.Write(s, "pageMargins", depth + 1, namespaces);
			}
		}

		public void Write_pageSetup(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_pageSetup != null)
			{
				_pageSetup.Write(s, "pageSetup", depth + 1, namespaces);
			}
		}

		public void Write_headerFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_headerFooter != null)
			{
				_headerFooter.Write(s, "headerFooter", depth + 1, namespaces);
			}
		}

		public void Write_drawing(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_drawing != null)
			{
				_drawing.Write(s, "drawing", depth + 1, namespaces);
			}
		}

		public void Write_picture(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_picture != null)
			{
				_picture.Write(s, "picture", depth + 1, namespaces);
			}
		}

		public void Write_cols(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_cols == null)
			{
				return;
			}
			foreach (CT_Cols col in _cols)
			{
				col?.Write(s, "cols", depth + 1, namespaces);
			}
		}
	}
}
