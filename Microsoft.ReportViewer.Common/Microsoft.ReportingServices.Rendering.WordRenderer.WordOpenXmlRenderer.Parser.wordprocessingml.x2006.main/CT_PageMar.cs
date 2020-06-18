using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_PageMar : OoxmlComplexType, IOoxmlComplexType
	{
		private string _top_attr;

		private string _right_attr;

		private string _bottom_attr;

		private string _left_attr;

		private string _header_attr;

		private string _footer_attr;

		private string _gutter_attr;

		public string Top_Attr
		{
			get
			{
				return _top_attr;
			}
			set
			{
				_top_attr = value;
			}
		}

		public string Right_Attr
		{
			get
			{
				return _right_attr;
			}
			set
			{
				_right_attr = value;
			}
		}

		public string Bottom_Attr
		{
			get
			{
				return _bottom_attr;
			}
			set
			{
				_bottom_attr = value;
			}
		}

		public string Left_Attr
		{
			get
			{
				return _left_attr;
			}
			set
			{
				_left_attr = value;
			}
		}

		public string Header_Attr
		{
			get
			{
				return _header_attr;
			}
			set
			{
				_header_attr = value;
			}
		}

		public string Footer_Attr
		{
			get
			{
				return _footer_attr;
			}
			set
			{
				_footer_attr = value;
			}
		}

		public string Gutter_Attr
		{
			get
			{
				return _gutter_attr;
			}
			set
			{
				_gutter_attr = value;
			}
		}

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
			WriteEmptyTag(s, tagName, "w");
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
			s.Write(" w:top=\"");
			OoxmlComplexType.WriteData(s, _top_attr);
			s.Write("\"");
			s.Write(" w:right=\"");
			OoxmlComplexType.WriteData(s, _right_attr);
			s.Write("\"");
			s.Write(" w:bottom=\"");
			OoxmlComplexType.WriteData(s, _bottom_attr);
			s.Write("\"");
			s.Write(" w:left=\"");
			OoxmlComplexType.WriteData(s, _left_attr);
			s.Write("\"");
			s.Write(" w:header=\"");
			OoxmlComplexType.WriteData(s, _header_attr);
			s.Write("\"");
			s.Write(" w:footer=\"");
			OoxmlComplexType.WriteData(s, _footer_attr);
			s.Write("\"");
			s.Write(" w:gutter=\"");
			OoxmlComplexType.WriteData(s, _gutter_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
