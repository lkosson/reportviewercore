using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Spacing : OoxmlComplexType, IOoxmlComplexType
	{
		private string _after_attr;

		private bool _after_attr_is_specified;

		private string _line_attr;

		private bool _line_attr_is_specified;

		private ST_LineSpacingRule _lineRule_attr;

		private bool _lineRule_attr_is_specified;

		public ST_LineSpacingRule LineRule_Attr
		{
			get
			{
				return _lineRule_attr;
			}
			set
			{
				_lineRule_attr = value;
				_lineRule_attr_is_specified = true;
			}
		}

		public string After_Attr
		{
			get
			{
				return _after_attr;
			}
			set
			{
				_after_attr = value;
				_after_attr_is_specified = (value != null);
			}
		}

		public string Line_Attr
		{
			get
			{
				return _line_attr;
			}
			set
			{
				_line_attr = value;
				_line_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_after_attr_is_specified = false;
			_line_attr_is_specified = false;
			_lineRule_attr_is_specified = false;
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
			if (_after_attr_is_specified)
			{
				s.Write(" w:after=\"");
				OoxmlComplexType.WriteData(s, _after_attr);
				s.Write("\"");
			}
			if (_line_attr_is_specified)
			{
				s.Write(" w:line=\"");
				OoxmlComplexType.WriteData(s, _line_attr);
				s.Write("\"");
			}
			if (_lineRule_attr_is_specified)
			{
				s.Write(" w:lineRule=\"");
				OoxmlComplexType.WriteData(s, _lineRule_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
