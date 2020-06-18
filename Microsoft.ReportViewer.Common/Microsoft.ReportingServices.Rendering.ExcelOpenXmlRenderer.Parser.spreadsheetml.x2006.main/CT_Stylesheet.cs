using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Stylesheet : OoxmlComplexType
	{
		private CT_NumFmts _numFmts;

		private CT_Fonts _fonts;

		private CT_Fills _fills;

		private CT_Borders _borders;

		private CT_CellStyleXfs _cellStyleXfs;

		private CT_CellXfs _cellXfs;

		private CT_CellStyles _cellStyles;

		private CT_Dxfs _dxfs;

		private CT_TableStyles _tableStyles;

		private CT_Colors _colors;

		public CT_NumFmts NumFmts
		{
			get
			{
				return _numFmts;
			}
			set
			{
				_numFmts = value;
			}
		}

		public CT_Fonts Fonts
		{
			get
			{
				return _fonts;
			}
			set
			{
				_fonts = value;
			}
		}

		public CT_Fills Fills
		{
			get
			{
				return _fills;
			}
			set
			{
				_fills = value;
			}
		}

		public CT_Borders Borders
		{
			get
			{
				return _borders;
			}
			set
			{
				_borders = value;
			}
		}

		public CT_CellStyleXfs CellStyleXfs
		{
			get
			{
				return _cellStyleXfs;
			}
			set
			{
				_cellStyleXfs = value;
			}
		}

		public CT_CellXfs CellXfs
		{
			get
			{
				return _cellXfs;
			}
			set
			{
				_cellXfs = value;
			}
		}

		public CT_CellStyles CellStyles
		{
			get
			{
				return _cellStyles;
			}
			set
			{
				_cellStyles = value;
			}
		}

		public CT_Dxfs Dxfs
		{
			get
			{
				return _dxfs;
			}
			set
			{
				_dxfs = value;
			}
		}

		public CT_TableStyles TableStyles
		{
			get
			{
				return _tableStyles;
			}
			set
			{
				_tableStyles = value;
			}
		}

		public CT_Colors Colors
		{
			get
			{
				return _colors;
			}
			set
			{
				_colors = value;
			}
		}

		public static string NumFmtsElementName => "numFmts";

		public static string FontsElementName => "fonts";

		public static string FillsElementName => "fills";

		public static string BordersElementName => "borders";

		public static string CellStyleXfsElementName => "cellStyleXfs";

		public static string CellXfsElementName => "cellXfs";

		public static string CellStylesElementName => "cellStyles";

		public static string DxfsElementName => "dxfs";

		public static string TableStylesElementName => "tableStyles";

		public static string ColorsElementName => "colors";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
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
			Write_numFmts(s, depth, namespaces);
			Write_fonts(s, depth, namespaces);
			Write_fills(s, depth, namespaces);
			Write_borders(s, depth, namespaces);
			Write_cellStyleXfs(s, depth, namespaces);
			Write_cellXfs(s, depth, namespaces);
			Write_cellStyles(s, depth, namespaces);
			Write_dxfs(s, depth, namespaces);
			Write_tableStyles(s, depth, namespaces);
			Write_colors(s, depth, namespaces);
		}

		public void Write_numFmts(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_numFmts != null)
			{
				_numFmts.Write(s, "numFmts", depth + 1, namespaces);
			}
		}

		public void Write_fonts(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_fonts != null)
			{
				_fonts.Write(s, "fonts", depth + 1, namespaces);
			}
		}

		public void Write_fills(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_fills != null)
			{
				_fills.Write(s, "fills", depth + 1, namespaces);
			}
		}

		public void Write_borders(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_borders != null)
			{
				_borders.Write(s, "borders", depth + 1, namespaces);
			}
		}

		public void Write_cellStyleXfs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_cellStyleXfs != null)
			{
				_cellStyleXfs.Write(s, "cellStyleXfs", depth + 1, namespaces);
			}
		}

		public void Write_cellXfs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_cellXfs != null)
			{
				_cellXfs.Write(s, "cellXfs", depth + 1, namespaces);
			}
		}

		public void Write_cellStyles(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_cellStyles != null)
			{
				_cellStyles.Write(s, "cellStyles", depth + 1, namespaces);
			}
		}

		public void Write_dxfs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_dxfs != null)
			{
				_dxfs.Write(s, "dxfs", depth + 1, namespaces);
			}
		}

		public void Write_tableStyles(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_tableStyles != null)
			{
				_tableStyles.Write(s, "tableStyles", depth + 1, namespaces);
			}
		}

		public void Write_colors(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_colors != null)
			{
				_colors.Write(s, "colors", depth + 1, namespaces);
			}
		}
	}
}
