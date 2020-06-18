using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_HeaderFooter : OoxmlComplexType
	{
		private OoxmlBool _differentOddEven_attr;

		private OoxmlBool _differentFirst_attr;

		private OoxmlBool _scaleWithDoc_attr;

		private OoxmlBool _alignWithMargins_attr;

		private string _oddHeader;

		private string _oddFooter;

		private string _evenHeader;

		private string _evenFooter;

		private string _firstHeader;

		private string _firstFooter;

		public OoxmlBool DifferentOddEven_Attr
		{
			get
			{
				return _differentOddEven_attr;
			}
			set
			{
				_differentOddEven_attr = value;
			}
		}

		public OoxmlBool DifferentFirst_Attr
		{
			get
			{
				return _differentFirst_attr;
			}
			set
			{
				_differentFirst_attr = value;
			}
		}

		public OoxmlBool ScaleWithDoc_Attr
		{
			get
			{
				return _scaleWithDoc_attr;
			}
			set
			{
				_scaleWithDoc_attr = value;
			}
		}

		public OoxmlBool AlignWithMargins_Attr
		{
			get
			{
				return _alignWithMargins_attr;
			}
			set
			{
				_alignWithMargins_attr = value;
			}
		}

		public string OddHeader
		{
			get
			{
				return _oddHeader;
			}
			set
			{
				_oddHeader = value;
			}
		}

		public string OddFooter
		{
			get
			{
				return _oddFooter;
			}
			set
			{
				_oddFooter = value;
			}
		}

		public string EvenHeader
		{
			get
			{
				return _evenHeader;
			}
			set
			{
				_evenHeader = value;
			}
		}

		public string EvenFooter
		{
			get
			{
				return _evenFooter;
			}
			set
			{
				_evenFooter = value;
			}
		}

		public string FirstHeader
		{
			get
			{
				return _firstHeader;
			}
			set
			{
				_firstHeader = value;
			}
		}

		public string FirstFooter
		{
			get
			{
				return _firstFooter;
			}
			set
			{
				_firstFooter = value;
			}
		}

		public static string OddHeaderElementName => "oddHeader";

		public static string OddFooterElementName => "oddFooter";

		public static string EvenHeaderElementName => "evenHeader";

		public static string EvenFooterElementName => "evenFooter";

		public static string FirstHeaderElementName => "firstHeader";

		public static string FirstFooterElementName => "firstFooter";

		protected override void InitAttributes()
		{
			_differentOddEven_attr = OoxmlBool.OoxmlFalse;
			_differentFirst_attr = OoxmlBool.OoxmlFalse;
			_scaleWithDoc_attr = OoxmlBool.OoxmlTrue;
			_alignWithMargins_attr = OoxmlBool.OoxmlTrue;
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
			if ((bool)(_differentOddEven_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" differentOddEven=\"");
				OoxmlComplexType.WriteData(s, _differentOddEven_attr);
				s.Write("\"");
			}
			if ((bool)(_differentFirst_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" differentFirst=\"");
				OoxmlComplexType.WriteData(s, _differentFirst_attr);
				s.Write("\"");
			}
			if ((bool)(_scaleWithDoc_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" scaleWithDoc=\"");
				OoxmlComplexType.WriteData(s, _scaleWithDoc_attr);
				s.Write("\"");
			}
			if ((bool)(_alignWithMargins_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" alignWithMargins=\"");
				OoxmlComplexType.WriteData(s, _alignWithMargins_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_oddHeader(s, depth, namespaces);
			Write_oddFooter(s, depth, namespaces);
			Write_evenHeader(s, depth, namespaces);
			Write_evenFooter(s, depth, namespaces);
			Write_firstHeader(s, depth, namespaces);
			Write_firstFooter(s, depth, namespaces);
		}

		public void Write_oddHeader(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_oddHeader != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "oddHeader", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _oddHeader);
			}
		}

		public void Write_oddFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_oddFooter != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "oddFooter", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _oddFooter);
			}
		}

		public void Write_evenHeader(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_evenHeader != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "evenHeader", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _evenHeader);
			}
		}

		public void Write_evenFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_evenFooter != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "evenFooter", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _evenFooter);
			}
		}

		public void Write_firstHeader(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_firstHeader != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "firstHeader", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _firstHeader);
			}
		}

		public void Write_firstFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_firstFooter != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "firstFooter", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _firstFooter);
			}
		}
	}
}
