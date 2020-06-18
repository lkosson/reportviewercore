using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Fonts : OoxmlComplexType, IOoxmlComplexType
	{
		private string _ascii_attr;

		private bool _ascii_attr_is_specified;

		private string _hAnsi_attr;

		private bool _hAnsi_attr_is_specified;

		private string _eastAsia_attr;

		private bool _eastAsia_attr_is_specified;

		private string _cs_attr;

		private bool _cs_attr_is_specified;

		public string Ascii_Attr
		{
			get
			{
				return _ascii_attr;
			}
			set
			{
				_ascii_attr = value;
				_ascii_attr_is_specified = (value != null);
			}
		}

		public string HAnsi_Attr
		{
			get
			{
				return _hAnsi_attr;
			}
			set
			{
				_hAnsi_attr = value;
				_hAnsi_attr_is_specified = (value != null);
			}
		}

		public string EastAsia_Attr
		{
			get
			{
				return _eastAsia_attr;
			}
			set
			{
				_eastAsia_attr = value;
				_eastAsia_attr_is_specified = (value != null);
			}
		}

		public string Cs_Attr
		{
			get
			{
				return _cs_attr;
			}
			set
			{
				_cs_attr = value;
				_cs_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_ascii_attr_is_specified = false;
			_hAnsi_attr_is_specified = false;
			_eastAsia_attr_is_specified = false;
			_cs_attr_is_specified = false;
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
			if (_ascii_attr_is_specified)
			{
				s.Write(" w:ascii=\"");
				OoxmlComplexType.WriteData(s, _ascii_attr);
				s.Write("\"");
			}
			if (_hAnsi_attr_is_specified)
			{
				s.Write(" w:hAnsi=\"");
				OoxmlComplexType.WriteData(s, _hAnsi_attr);
				s.Write("\"");
			}
			if (_eastAsia_attr_is_specified)
			{
				s.Write(" w:eastAsia=\"");
				OoxmlComplexType.WriteData(s, _eastAsia_attr);
				s.Write("\"");
			}
			if (_cs_attr_is_specified)
			{
				s.Write(" w:cs=\"");
				OoxmlComplexType.WriteData(s, _cs_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
