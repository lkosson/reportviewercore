using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_Marker : OoxmlComplexType
	{
		private int _col;

		private string _colOff;

		private int _row;

		private string _rowOff;

		public int Col
		{
			get
			{
				return _col;
			}
			set
			{
				_col = value;
			}
		}

		public string ColOff
		{
			get
			{
				return _colOff;
			}
			set
			{
				_colOff = value;
			}
		}

		public int Row
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;
			}
		}

		public string RowOff
		{
			get
			{
				return _rowOff;
			}
			set
			{
				_rowOff = value;
			}
		}

		public static string ColElementName => "col";

		public static string RowElementName => "row";

		public static string ColOffElementName => "colOff";

		public static string RowOffElementName => "rowOff";

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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_col(s, depth, namespaces);
			Write_colOff(s, depth, namespaces);
			Write_row(s, depth, namespaces);
			Write_rowOff(s, depth, namespaces);
		}

		public void Write_col(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "col", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", _col);
		}

		public void Write_row(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "row", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", _row);
		}

		public void Write_colOff(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_colOff != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "colOff", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", _colOff);
			}
		}

		public void Write_rowOff(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_rowOff != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "rowOff", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", _rowOff);
			}
		}
	}
}
