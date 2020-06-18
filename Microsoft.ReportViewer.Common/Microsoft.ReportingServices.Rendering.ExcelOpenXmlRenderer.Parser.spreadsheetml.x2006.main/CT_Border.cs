using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Border : OoxmlComplexType
	{
		private OoxmlBool _outline_attr;

		private OoxmlBool _diagonalUp_attr;

		private bool _diagonalUp_attr_is_specified;

		private OoxmlBool _diagonalDown_attr;

		private bool _diagonalDown_attr_is_specified;

		private CT_BorderPr _left;

		private CT_BorderPr _right;

		private CT_BorderPr _top;

		private CT_BorderPr _bottom;

		private CT_BorderPr _diagonal;

		private CT_BorderPr _vertical;

		private CT_BorderPr _horizontal;

		public OoxmlBool Outline_Attr
		{
			get
			{
				return _outline_attr;
			}
			set
			{
				_outline_attr = value;
			}
		}

		public OoxmlBool DiagonalUp_Attr
		{
			get
			{
				return _diagonalUp_attr;
			}
			set
			{
				_diagonalUp_attr = value;
				_diagonalUp_attr_is_specified = true;
			}
		}

		public bool DiagonalUp_Attr_Is_Specified
		{
			get
			{
				return _diagonalUp_attr_is_specified;
			}
			set
			{
				_diagonalUp_attr_is_specified = value;
			}
		}

		public OoxmlBool DiagonalDown_Attr
		{
			get
			{
				return _diagonalDown_attr;
			}
			set
			{
				_diagonalDown_attr = value;
				_diagonalDown_attr_is_specified = true;
			}
		}

		public bool DiagonalDown_Attr_Is_Specified
		{
			get
			{
				return _diagonalDown_attr_is_specified;
			}
			set
			{
				_diagonalDown_attr_is_specified = value;
			}
		}

		public CT_BorderPr Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		public CT_BorderPr Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
			}
		}

		public CT_BorderPr Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
			}
		}

		public CT_BorderPr Bottom
		{
			get
			{
				return _bottom;
			}
			set
			{
				_bottom = value;
			}
		}

		public CT_BorderPr Diagonal
		{
			get
			{
				return _diagonal;
			}
			set
			{
				_diagonal = value;
			}
		}

		public CT_BorderPr Vertical
		{
			get
			{
				return _vertical;
			}
			set
			{
				_vertical = value;
			}
		}

		public CT_BorderPr Horizontal
		{
			get
			{
				return _horizontal;
			}
			set
			{
				_horizontal = value;
			}
		}

		public static string LeftElementName => "left";

		public static string RightElementName => "right";

		public static string TopElementName => "top";

		public static string BottomElementName => "bottom";

		public static string DiagonalElementName => "diagonal";

		public static string VerticalElementName => "vertical";

		public static string HorizontalElementName => "horizontal";

		protected override void InitAttributes()
		{
			_outline_attr = OoxmlBool.OoxmlTrue;
			_diagonalUp_attr_is_specified = false;
			_diagonalDown_attr_is_specified = false;
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
			if ((bool)(_outline_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" outline=\"");
				OoxmlComplexType.WriteData(s, _outline_attr);
				s.Write("\"");
			}
			if (_diagonalUp_attr_is_specified)
			{
				s.Write(" diagonalUp=\"");
				OoxmlComplexType.WriteData(s, _diagonalUp_attr);
				s.Write("\"");
			}
			if (_diagonalDown_attr_is_specified)
			{
				s.Write(" diagonalDown=\"");
				OoxmlComplexType.WriteData(s, _diagonalDown_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_left(s, depth, namespaces);
			Write_right(s, depth, namespaces);
			Write_top(s, depth, namespaces);
			Write_bottom(s, depth, namespaces);
			Write_diagonal(s, depth, namespaces);
			Write_vertical(s, depth, namespaces);
			Write_horizontal(s, depth, namespaces);
		}

		public void Write_left(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_left != null)
			{
				_left.Write(s, "left", depth + 1, namespaces);
			}
		}

		public void Write_right(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_right != null)
			{
				_right.Write(s, "right", depth + 1, namespaces);
			}
		}

		public void Write_top(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_top != null)
			{
				_top.Write(s, "top", depth + 1, namespaces);
			}
		}

		public void Write_bottom(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_bottom != null)
			{
				_bottom.Write(s, "bottom", depth + 1, namespaces);
			}
		}

		public void Write_diagonal(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_diagonal != null)
			{
				_diagonal.Write(s, "diagonal", depth + 1, namespaces);
			}
		}

		public void Write_vertical(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_vertical != null)
			{
				_vertical.Write(s, "vertical", depth + 1, namespaces);
			}
		}

		public void Write_horizontal(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_horizontal != null)
			{
				_horizontal.Write(s, "horizontal", depth + 1, namespaces);
			}
		}
	}
}
