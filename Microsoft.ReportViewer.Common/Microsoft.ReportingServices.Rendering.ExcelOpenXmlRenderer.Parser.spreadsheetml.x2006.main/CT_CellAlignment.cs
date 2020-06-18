using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_CellAlignment : OoxmlComplexType
	{
		private ST_HorizontalAlignment _horizontal_attr;

		private bool _horizontal_attr_is_specified;

		private ST_VerticalAlignment _vertical_attr;

		private bool _vertical_attr_is_specified;

		private uint _textRotation_attr;

		private bool _textRotation_attr_is_specified;

		private OoxmlBool _wrapText_attr;

		private bool _wrapText_attr_is_specified;

		private uint _indent_attr;

		private bool _indent_attr_is_specified;

		private int _relativeIndent_attr;

		private bool _relativeIndent_attr_is_specified;

		private OoxmlBool _justifyLastLine_attr;

		private bool _justifyLastLine_attr_is_specified;

		private OoxmlBool _shrinkToFit_attr;

		private bool _shrinkToFit_attr_is_specified;

		private uint _readingOrder_attr;

		private bool _readingOrder_attr_is_specified;

		public ST_HorizontalAlignment Horizontal_Attr
		{
			get
			{
				return _horizontal_attr;
			}
			set
			{
				_horizontal_attr = value;
				_horizontal_attr_is_specified = true;
			}
		}

		public bool Horizontal_Attr_Is_Specified
		{
			get
			{
				return _horizontal_attr_is_specified;
			}
			set
			{
				_horizontal_attr_is_specified = value;
			}
		}

		public ST_VerticalAlignment Vertical_Attr
		{
			get
			{
				return _vertical_attr;
			}
			set
			{
				_vertical_attr = value;
				_vertical_attr_is_specified = true;
			}
		}

		public bool Vertical_Attr_Is_Specified
		{
			get
			{
				return _vertical_attr_is_specified;
			}
			set
			{
				_vertical_attr_is_specified = value;
			}
		}

		public uint TextRotation_Attr
		{
			get
			{
				return _textRotation_attr;
			}
			set
			{
				_textRotation_attr = value;
				_textRotation_attr_is_specified = true;
			}
		}

		public bool TextRotation_Attr_Is_Specified
		{
			get
			{
				return _textRotation_attr_is_specified;
			}
			set
			{
				_textRotation_attr_is_specified = value;
			}
		}

		public OoxmlBool WrapText_Attr
		{
			get
			{
				return _wrapText_attr;
			}
			set
			{
				_wrapText_attr = value;
				_wrapText_attr_is_specified = true;
			}
		}

		public bool WrapText_Attr_Is_Specified
		{
			get
			{
				return _wrapText_attr_is_specified;
			}
			set
			{
				_wrapText_attr_is_specified = value;
			}
		}

		public uint Indent_Attr
		{
			get
			{
				return _indent_attr;
			}
			set
			{
				_indent_attr = value;
				_indent_attr_is_specified = true;
			}
		}

		public bool Indent_Attr_Is_Specified
		{
			get
			{
				return _indent_attr_is_specified;
			}
			set
			{
				_indent_attr_is_specified = value;
			}
		}

		public int RelativeIndent_Attr
		{
			get
			{
				return _relativeIndent_attr;
			}
			set
			{
				_relativeIndent_attr = value;
				_relativeIndent_attr_is_specified = true;
			}
		}

		public bool RelativeIndent_Attr_Is_Specified
		{
			get
			{
				return _relativeIndent_attr_is_specified;
			}
			set
			{
				_relativeIndent_attr_is_specified = value;
			}
		}

		public OoxmlBool JustifyLastLine_Attr
		{
			get
			{
				return _justifyLastLine_attr;
			}
			set
			{
				_justifyLastLine_attr = value;
				_justifyLastLine_attr_is_specified = true;
			}
		}

		public bool JustifyLastLine_Attr_Is_Specified
		{
			get
			{
				return _justifyLastLine_attr_is_specified;
			}
			set
			{
				_justifyLastLine_attr_is_specified = value;
			}
		}

		public OoxmlBool ShrinkToFit_Attr
		{
			get
			{
				return _shrinkToFit_attr;
			}
			set
			{
				_shrinkToFit_attr = value;
				_shrinkToFit_attr_is_specified = true;
			}
		}

		public bool ShrinkToFit_Attr_Is_Specified
		{
			get
			{
				return _shrinkToFit_attr_is_specified;
			}
			set
			{
				_shrinkToFit_attr_is_specified = value;
			}
		}

		public uint ReadingOrder_Attr
		{
			get
			{
				return _readingOrder_attr;
			}
			set
			{
				_readingOrder_attr = value;
				_readingOrder_attr_is_specified = true;
			}
		}

		public bool ReadingOrder_Attr_Is_Specified
		{
			get
			{
				return _readingOrder_attr_is_specified;
			}
			set
			{
				_readingOrder_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			_horizontal_attr_is_specified = false;
			_vertical_attr_is_specified = false;
			_textRotation_attr_is_specified = false;
			_wrapText_attr_is_specified = false;
			_indent_attr_is_specified = false;
			_relativeIndent_attr_is_specified = false;
			_justifyLastLine_attr_is_specified = false;
			_shrinkToFit_attr_is_specified = false;
			_readingOrder_attr_is_specified = false;
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
			if (_horizontal_attr_is_specified)
			{
				s.Write(" horizontal=\"");
				OoxmlComplexType.WriteData(s, _horizontal_attr);
				s.Write("\"");
			}
			if (_vertical_attr_is_specified)
			{
				s.Write(" vertical=\"");
				OoxmlComplexType.WriteData(s, _vertical_attr);
				s.Write("\"");
			}
			if (_textRotation_attr_is_specified)
			{
				s.Write(" textRotation=\"");
				OoxmlComplexType.WriteData(s, _textRotation_attr);
				s.Write("\"");
			}
			if (_wrapText_attr_is_specified)
			{
				s.Write(" wrapText=\"");
				OoxmlComplexType.WriteData(s, _wrapText_attr);
				s.Write("\"");
			}
			if (_indent_attr_is_specified)
			{
				s.Write(" indent=\"");
				OoxmlComplexType.WriteData(s, _indent_attr);
				s.Write("\"");
			}
			if (_relativeIndent_attr_is_specified)
			{
				s.Write(" relativeIndent=\"");
				OoxmlComplexType.WriteData(s, _relativeIndent_attr);
				s.Write("\"");
			}
			if (_justifyLastLine_attr_is_specified)
			{
				s.Write(" justifyLastLine=\"");
				OoxmlComplexType.WriteData(s, _justifyLastLine_attr);
				s.Write("\"");
			}
			if (_shrinkToFit_attr_is_specified)
			{
				s.Write(" shrinkToFit=\"");
				OoxmlComplexType.WriteData(s, _shrinkToFit_attr);
				s.Write("\"");
			}
			if (_readingOrder_attr_is_specified)
			{
				s.Write(" readingOrder=\"");
				OoxmlComplexType.WriteData(s, _readingOrder_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
