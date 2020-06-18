using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Blip : OoxmlComplexType, IOoxmlComplexType
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

		private string _cstate_attr;

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

		public string Cstate_Attr
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
			_embed_attr = "";
			_cstate_attr = "none";
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			WriteEmptyTag(s, tagName, "a");
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "a", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</a:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_embed_attr != "")
			{
				s.Write(" r:embed=\"");
				OoxmlComplexType.WriteData(s, _embed_attr);
				s.Write("\"");
			}
			if (_cstate_attr != "none")
			{
				s.Write(" cstate=\"");
				OoxmlComplexType.WriteData(s, _cstate_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
