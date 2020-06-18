using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_RPrElt : OoxmlComplexType
	{
		private CT_BooleanProperty _b;

		private CT_BooleanProperty _i;

		private CT_BooleanProperty _strike;

		private CT_BooleanProperty _condense;

		private CT_BooleanProperty _extend;

		private CT_BooleanProperty _outline;

		private CT_BooleanProperty _shadow;

		private CT_UnderlineProperty _u;

		private CT_VerticalAlignFontProperty _vertAlign;

		private CT_FontSize _sz;

		private CT_Color _color;

		private CT_FontName _rFont;

		private CT_IntProperty _family;

		private CT_IntProperty _charset;

		private CT_FontScheme _scheme;

		public CT_BooleanProperty B
		{
			get
			{
				return _b;
			}
			set
			{
				_b = value;
			}
		}

		public CT_BooleanProperty I
		{
			get
			{
				return _i;
			}
			set
			{
				_i = value;
			}
		}

		public CT_BooleanProperty Strike
		{
			get
			{
				return _strike;
			}
			set
			{
				_strike = value;
			}
		}

		public CT_BooleanProperty Condense
		{
			get
			{
				return _condense;
			}
			set
			{
				_condense = value;
			}
		}

		public CT_BooleanProperty Extend
		{
			get
			{
				return _extend;
			}
			set
			{
				_extend = value;
			}
		}

		public CT_BooleanProperty Outline
		{
			get
			{
				return _outline;
			}
			set
			{
				_outline = value;
			}
		}

		public CT_BooleanProperty Shadow
		{
			get
			{
				return _shadow;
			}
			set
			{
				_shadow = value;
			}
		}

		public CT_UnderlineProperty U
		{
			get
			{
				return _u;
			}
			set
			{
				_u = value;
			}
		}

		public CT_VerticalAlignFontProperty VertAlign
		{
			get
			{
				return _vertAlign;
			}
			set
			{
				_vertAlign = value;
			}
		}

		public CT_FontSize Sz
		{
			get
			{
				return _sz;
			}
			set
			{
				_sz = value;
			}
		}

		public CT_Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		public CT_FontName RFont
		{
			get
			{
				return _rFont;
			}
			set
			{
				_rFont = value;
			}
		}

		public CT_IntProperty Family
		{
			get
			{
				return _family;
			}
			set
			{
				_family = value;
			}
		}

		public CT_IntProperty Charset
		{
			get
			{
				return _charset;
			}
			set
			{
				_charset = value;
			}
		}

		public CT_FontScheme Scheme
		{
			get
			{
				return _scheme;
			}
			set
			{
				_scheme = value;
			}
		}

		public static string BElementName => "b";

		public static string IElementName => "i";

		public static string StrikeElementName => "strike";

		public static string CondenseElementName => "condense";

		public static string ExtendElementName => "extend";

		public static string OutlineElementName => "outline";

		public static string ShadowElementName => "shadow";

		public static string UElementName => "u";

		public static string VertAlignElementName => "vertAlign";

		public static string SzElementName => "sz";

		public static string ColorElementName => "color";

		public static string RFontElementName => "rFont";

		public static string FamilyElementName => "family";

		public static string CharsetElementName => "charset";

		public static string SchemeElementName => "scheme";

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
			Write_b(s, depth, namespaces);
			Write_i(s, depth, namespaces);
			Write_strike(s, depth, namespaces);
			Write_condense(s, depth, namespaces);
			Write_extend(s, depth, namespaces);
			Write_outline(s, depth, namespaces);
			Write_shadow(s, depth, namespaces);
			Write_u(s, depth, namespaces);
			Write_vertAlign(s, depth, namespaces);
			Write_sz(s, depth, namespaces);
			Write_color(s, depth, namespaces);
			Write_rFont(s, depth, namespaces);
			Write_family(s, depth, namespaces);
			Write_charset(s, depth, namespaces);
			Write_scheme(s, depth, namespaces);
		}

		public void Write_b(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_b != null)
			{
				_b.Write(s, "b", depth + 1, namespaces);
			}
		}

		public void Write_i(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_i != null)
			{
				_i.Write(s, "i", depth + 1, namespaces);
			}
		}

		public void Write_strike(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_strike != null)
			{
				_strike.Write(s, "strike", depth + 1, namespaces);
			}
		}

		public void Write_condense(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_condense != null)
			{
				_condense.Write(s, "condense", depth + 1, namespaces);
			}
		}

		public void Write_extend(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_extend != null)
			{
				_extend.Write(s, "extend", depth + 1, namespaces);
			}
		}

		public void Write_outline(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_outline != null)
			{
				_outline.Write(s, "outline", depth + 1, namespaces);
			}
		}

		public void Write_shadow(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_shadow != null)
			{
				_shadow.Write(s, "shadow", depth + 1, namespaces);
			}
		}

		public void Write_u(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_u != null)
			{
				_u.Write(s, "u", depth + 1, namespaces);
			}
		}

		public void Write_vertAlign(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_vertAlign != null)
			{
				_vertAlign.Write(s, "vertAlign", depth + 1, namespaces);
			}
		}

		public void Write_sz(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_sz != null)
			{
				_sz.Write(s, "sz", depth + 1, namespaces);
			}
		}

		public void Write_color(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_color != null)
			{
				_color.Write(s, "color", depth + 1, namespaces);
			}
		}

		public void Write_rFont(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_rFont != null)
			{
				_rFont.Write(s, "rFont", depth + 1, namespaces);
			}
		}

		public void Write_family(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_family != null)
			{
				_family.Write(s, "family", depth + 1, namespaces);
			}
		}

		public void Write_charset(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_charset != null)
			{
				_charset.Write(s, "charset", depth + 1, namespaces);
			}
		}

		public void Write_scheme(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_scheme != null)
			{
				_scheme.Write(s, "scheme", depth + 1, namespaces);
			}
		}
	}
}
