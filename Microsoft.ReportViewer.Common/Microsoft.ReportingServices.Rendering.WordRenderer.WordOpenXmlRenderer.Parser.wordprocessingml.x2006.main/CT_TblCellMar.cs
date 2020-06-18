using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TblCellMar : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_TblWidth _top;

		private CT_TblWidth _left;

		private CT_TblWidth _bottom;

		private CT_TblWidth _right;

		public CT_TblWidth Top
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

		public CT_TblWidth Left
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

		public CT_TblWidth Bottom
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

		public CT_TblWidth Right
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

		public static string TopElementName => "top";

		public static string LeftElementName => "left";

		public static string BottomElementName => "bottom";

		public static string RightElementName => "right";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			WriteOpenTag(s, tagName, null);
			WriteElements(s);
			WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			Write_top(s);
			Write_left(s);
			Write_bottom(s);
			Write_right(s);
		}

		public void Write_top(TextWriter s)
		{
			if (_top != null)
			{
				_top.Write(s, "top");
			}
		}

		public void Write_left(TextWriter s)
		{
			if (_left != null)
			{
				_left.Write(s, "left");
			}
		}

		public void Write_bottom(TextWriter s)
		{
			if (_bottom != null)
			{
				_bottom.Write(s, "bottom");
			}
		}

		public void Write_right(TextWriter s)
		{
			if (_right != null)
			{
				_right.Write(s, "right");
			}
		}
	}
}
