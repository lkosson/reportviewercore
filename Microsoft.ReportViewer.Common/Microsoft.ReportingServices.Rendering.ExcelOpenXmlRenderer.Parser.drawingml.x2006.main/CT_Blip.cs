using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Blip : OoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			alphaBiLevel,
			alphaCeiling,
			alphaFloor,
			alphaInv,
			alphaMod,
			alphaModFix,
			alphaRepl,
			biLevel,
			blur,
			clrChange,
			clrRepl,
			duotone,
			fillOverlay,
			grayscl,
			hsl,
			lum,
			tint
		}

		private string _embed_attr;

		private string _link_attr;

		private ST_BlipCompression _cstate_attr;

		private ChoiceBucket_0 _choice_0;

		public string Embed_Attr
		{
			get
			{
				return _embed_attr;
			}
			set
			{
				_embed_attr = value;
			}
		}

		public string Link_Attr
		{
			get
			{
				return _link_attr;
			}
			set
			{
				_link_attr = value;
			}
		}

		public ST_BlipCompression Cstate_Attr
		{
			get
			{
				return _cstate_attr;
			}
			set
			{
				_cstate_attr = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return _choice_0;
			}
			set
			{
				_choice_0 = value;
			}
		}

		protected override void InitAttributes()
		{
			_embed_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_link_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_cstate_attr = ST_BlipCompression.none;
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_embed_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" r:embed=\"");
				OoxmlComplexType.WriteData(s, _embed_attr);
				s.Write("\"");
			}
			if (_link_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" r:link=\"");
				OoxmlComplexType.WriteData(s, _link_attr);
				s.Write("\"");
			}
			if (_cstate_attr != ST_BlipCompression.none)
			{
				s.Write(" cstate=\"");
				OoxmlComplexType.WriteData(s, _cstate_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
