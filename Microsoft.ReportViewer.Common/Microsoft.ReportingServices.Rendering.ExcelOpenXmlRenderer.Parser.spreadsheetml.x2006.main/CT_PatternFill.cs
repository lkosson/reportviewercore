using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_PatternFill : OoxmlComplexType
	{
		private ST_PatternType _patternType_attr;

		private bool _patternType_attr_is_specified;

		private CT_Color _fgColor;

		private CT_Color _bgColor;

		public ST_PatternType PatternType_Attr
		{
			get
			{
				return _patternType_attr;
			}
			set
			{
				_patternType_attr = value;
				_patternType_attr_is_specified = true;
			}
		}

		public bool PatternType_Attr_Is_Specified
		{
			get
			{
				return _patternType_attr_is_specified;
			}
			set
			{
				_patternType_attr_is_specified = value;
			}
		}

		public CT_Color FgColor
		{
			get
			{
				return _fgColor;
			}
			set
			{
				_fgColor = value;
			}
		}

		public CT_Color BgColor
		{
			get
			{
				return _bgColor;
			}
			set
			{
				_bgColor = value;
			}
		}

		public static string FgColorElementName => "fgColor";

		public static string BgColorElementName => "bgColor";

		protected override void InitAttributes()
		{
			_patternType_attr_is_specified = false;
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
			if (_patternType_attr_is_specified)
			{
				s.Write(" patternType=\"");
				OoxmlComplexType.WriteData(s, _patternType_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_fgColor(s, depth, namespaces);
			Write_bgColor(s, depth, namespaces);
		}

		public void Write_fgColor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_fgColor != null)
			{
				_fgColor.Write(s, "fgColor", depth + 1, namespaces);
			}
		}

		public void Write_bgColor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_bgColor != null)
			{
				_bgColor.Write(s, "bgColor", depth + 1, namespaces);
			}
		}
	}
}
