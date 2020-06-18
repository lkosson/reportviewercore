using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_RPr : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_String _rStyle;

		private CT_Fonts _rFonts;

		private CT_OnOff _b;

		private CT_OnOff _bCs;

		private CT_OnOff _i;

		private CT_OnOff _iCs;

		private CT_OnOff _caps;

		private CT_OnOff _smallCaps;

		private CT_OnOff _strike;

		private CT_OnOff _dstrike;

		private CT_OnOff _outline;

		private CT_OnOff _shadow;

		private CT_OnOff _emboss;

		private CT_OnOff _imprint;

		private CT_OnOff _noProof;

		private CT_OnOff _snapToGrid;

		private CT_OnOff _vanish;

		private CT_OnOff _webHidden;

		private CT_HpsMeasure _kern;

		private CT_HpsMeasure _sz;

		private CT_HpsMeasure _szCs;

		private CT_OnOff _rtl;

		private CT_OnOff _cs;

		private CT_OnOff _specVanish;

		private CT_OnOff _oMath;

		public CT_String RStyle
		{
			get
			{
				return _rStyle;
			}
			set
			{
				_rStyle = value;
			}
		}

		public CT_Fonts RFonts
		{
			get
			{
				return _rFonts;
			}
			set
			{
				_rFonts = value;
			}
		}

		public CT_OnOff B
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

		public CT_OnOff BCs
		{
			get
			{
				return _bCs;
			}
			set
			{
				_bCs = value;
			}
		}

		public CT_OnOff I
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

		public CT_OnOff ICs
		{
			get
			{
				return _iCs;
			}
			set
			{
				_iCs = value;
			}
		}

		public CT_OnOff Caps
		{
			get
			{
				return _caps;
			}
			set
			{
				_caps = value;
			}
		}

		public CT_OnOff SmallCaps
		{
			get
			{
				return _smallCaps;
			}
			set
			{
				_smallCaps = value;
			}
		}

		public CT_OnOff Strike
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

		public CT_OnOff Dstrike
		{
			get
			{
				return _dstrike;
			}
			set
			{
				_dstrike = value;
			}
		}

		public CT_OnOff Outline
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

		public CT_OnOff Shadow
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

		public CT_OnOff Emboss
		{
			get
			{
				return _emboss;
			}
			set
			{
				_emboss = value;
			}
		}

		public CT_OnOff Imprint
		{
			get
			{
				return _imprint;
			}
			set
			{
				_imprint = value;
			}
		}

		public CT_OnOff NoProof
		{
			get
			{
				return _noProof;
			}
			set
			{
				_noProof = value;
			}
		}

		public CT_OnOff SnapToGrid
		{
			get
			{
				return _snapToGrid;
			}
			set
			{
				_snapToGrid = value;
			}
		}

		public CT_OnOff Vanish
		{
			get
			{
				return _vanish;
			}
			set
			{
				_vanish = value;
			}
		}

		public CT_OnOff WebHidden
		{
			get
			{
				return _webHidden;
			}
			set
			{
				_webHidden = value;
			}
		}

		public CT_HpsMeasure Kern
		{
			get
			{
				return _kern;
			}
			set
			{
				_kern = value;
			}
		}

		public CT_HpsMeasure Sz
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

		public CT_HpsMeasure SzCs
		{
			get
			{
				return _szCs;
			}
			set
			{
				_szCs = value;
			}
		}

		public CT_OnOff Rtl
		{
			get
			{
				return _rtl;
			}
			set
			{
				_rtl = value;
			}
		}

		public CT_OnOff Cs
		{
			get
			{
				return _cs;
			}
			set
			{
				_cs = value;
			}
		}

		public CT_OnOff SpecVanish
		{
			get
			{
				return _specVanish;
			}
			set
			{
				_specVanish = value;
			}
		}

		public CT_OnOff OMath
		{
			get
			{
				return _oMath;
			}
			set
			{
				_oMath = value;
			}
		}

		public static string RStyleElementName => "rStyle";

		public static string RFontsElementName => "rFonts";

		public static string BElementName => "b";

		public static string BCsElementName => "bCs";

		public static string IElementName => "i";

		public static string ICsElementName => "iCs";

		public static string CapsElementName => "caps";

		public static string SmallCapsElementName => "smallCaps";

		public static string StrikeElementName => "strike";

		public static string DstrikeElementName => "dstrike";

		public static string OutlineElementName => "outline";

		public static string ShadowElementName => "shadow";

		public static string EmbossElementName => "emboss";

		public static string ImprintElementName => "imprint";

		public static string NoProofElementName => "noProof";

		public static string SnapToGridElementName => "snapToGrid";

		public static string VanishElementName => "vanish";

		public static string WebHiddenElementName => "webHidden";

		public static string KernElementName => "kern";

		public static string SzElementName => "sz";

		public static string SzCsElementName => "szCs";

		public static string RtlElementName => "rtl";

		public static string CsElementName => "cs";

		public static string SpecVanishElementName => "specVanish";

		public static string OMathElementName => "oMath";

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
			Write_rStyle(s);
			Write_rFonts(s);
			Write_b(s);
			Write_bCs(s);
			Write_i(s);
			Write_iCs(s);
			Write_caps(s);
			Write_smallCaps(s);
			Write_strike(s);
			Write_dstrike(s);
			Write_outline(s);
			Write_shadow(s);
			Write_emboss(s);
			Write_imprint(s);
			Write_noProof(s);
			Write_snapToGrid(s);
			Write_vanish(s);
			Write_webHidden(s);
			Write_kern(s);
			Write_sz(s);
			Write_szCs(s);
			Write_rtl(s);
			Write_cs(s);
			Write_specVanish(s);
			Write_oMath(s);
		}

		public void Write_rStyle(TextWriter s)
		{
			if (_rStyle != null)
			{
				_rStyle.Write(s, "rStyle");
			}
		}

		public void Write_rFonts(TextWriter s)
		{
			if (_rFonts != null)
			{
				_rFonts.Write(s, "rFonts");
			}
		}

		public void Write_b(TextWriter s)
		{
			if (_b != null)
			{
				_b.Write(s, "b");
			}
		}

		public void Write_bCs(TextWriter s)
		{
			if (_bCs != null)
			{
				_bCs.Write(s, "bCs");
			}
		}

		public void Write_i(TextWriter s)
		{
			if (_i != null)
			{
				_i.Write(s, "i");
			}
		}

		public void Write_iCs(TextWriter s)
		{
			if (_iCs != null)
			{
				_iCs.Write(s, "iCs");
			}
		}

		public void Write_caps(TextWriter s)
		{
			if (_caps != null)
			{
				_caps.Write(s, "caps");
			}
		}

		public void Write_smallCaps(TextWriter s)
		{
			if (_smallCaps != null)
			{
				_smallCaps.Write(s, "smallCaps");
			}
		}

		public void Write_strike(TextWriter s)
		{
			if (_strike != null)
			{
				_strike.Write(s, "strike");
			}
		}

		public void Write_dstrike(TextWriter s)
		{
			if (_dstrike != null)
			{
				_dstrike.Write(s, "dstrike");
			}
		}

		public void Write_outline(TextWriter s)
		{
			if (_outline != null)
			{
				_outline.Write(s, "outline");
			}
		}

		public void Write_shadow(TextWriter s)
		{
			if (_shadow != null)
			{
				_shadow.Write(s, "shadow");
			}
		}

		public void Write_emboss(TextWriter s)
		{
			if (_emboss != null)
			{
				_emboss.Write(s, "emboss");
			}
		}

		public void Write_imprint(TextWriter s)
		{
			if (_imprint != null)
			{
				_imprint.Write(s, "imprint");
			}
		}

		public void Write_noProof(TextWriter s)
		{
			if (_noProof != null)
			{
				_noProof.Write(s, "noProof");
			}
		}

		public void Write_snapToGrid(TextWriter s)
		{
			if (_snapToGrid != null)
			{
				_snapToGrid.Write(s, "snapToGrid");
			}
		}

		public void Write_vanish(TextWriter s)
		{
			if (_vanish != null)
			{
				_vanish.Write(s, "vanish");
			}
		}

		public void Write_webHidden(TextWriter s)
		{
			if (_webHidden != null)
			{
				_webHidden.Write(s, "webHidden");
			}
		}

		public void Write_kern(TextWriter s)
		{
			if (_kern != null)
			{
				_kern.Write(s, "kern");
			}
		}

		public void Write_sz(TextWriter s)
		{
			if (_sz != null)
			{
				_sz.Write(s, "sz");
			}
		}

		public void Write_szCs(TextWriter s)
		{
			if (_szCs != null)
			{
				_szCs.Write(s, "szCs");
			}
		}

		public void Write_rtl(TextWriter s)
		{
			if (_rtl != null)
			{
				_rtl.Write(s, "rtl");
			}
		}

		public void Write_cs(TextWriter s)
		{
			if (_cs != null)
			{
				_cs.Write(s, "cs");
			}
		}

		public void Write_specVanish(TextWriter s)
		{
			if (_specVanish != null)
			{
				_specVanish.Write(s, "specVanish");
			}
		}

		public void Write_oMath(TextWriter s)
		{
			if (_oMath != null)
			{
				_oMath.Write(s, "oMath");
			}
		}
	}
}
