using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_PPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_String _pStyle;

		private CT_OnOff _keepNext;

		private CT_OnOff _keepLines;

		private CT_OnOff _pageBreakBefore;

		private CT_OnOff _widowControl;

		private CT_OnOff _suppressLineNumbers;

		private CT_OnOff _suppressAutoHyphens;

		private CT_OnOff _kinsoku;

		private CT_OnOff _wordWrap;

		private CT_OnOff _overflowPunct;

		private CT_OnOff _topLinePunct;

		private CT_OnOff _autoSpaceDE;

		private CT_OnOff _autoSpaceDN;

		private CT_OnOff _bidi;

		private CT_OnOff _adjustRightInd;

		private CT_OnOff _snapToGrid;

		private CT_Spacing _spacing;

		private CT_OnOff _contextualSpacing;

		private CT_OnOff _mirrorIndents;

		private CT_OnOff _suppressOverlap;

		private CT_Jc _jc;

		private CT_DecimalNumber _outlineLvl;

		private CT_DecimalNumber _divId;

		public CT_String PStyle
		{
			get
			{
				return _pStyle;
			}
			set
			{
				_pStyle = value;
			}
		}

		public CT_OnOff KeepNext
		{
			get
			{
				return _keepNext;
			}
			set
			{
				_keepNext = value;
			}
		}

		public CT_OnOff KeepLines
		{
			get
			{
				return _keepLines;
			}
			set
			{
				_keepLines = value;
			}
		}

		public CT_OnOff PageBreakBefore
		{
			get
			{
				return _pageBreakBefore;
			}
			set
			{
				_pageBreakBefore = value;
			}
		}

		public CT_OnOff WidowControl
		{
			get
			{
				return _widowControl;
			}
			set
			{
				_widowControl = value;
			}
		}

		public CT_OnOff SuppressLineNumbers
		{
			get
			{
				return _suppressLineNumbers;
			}
			set
			{
				_suppressLineNumbers = value;
			}
		}

		public CT_OnOff SuppressAutoHyphens
		{
			get
			{
				return _suppressAutoHyphens;
			}
			set
			{
				_suppressAutoHyphens = value;
			}
		}

		public CT_OnOff Kinsoku
		{
			get
			{
				return _kinsoku;
			}
			set
			{
				_kinsoku = value;
			}
		}

		public CT_OnOff WordWrap
		{
			get
			{
				return _wordWrap;
			}
			set
			{
				_wordWrap = value;
			}
		}

		public CT_OnOff OverflowPunct
		{
			get
			{
				return _overflowPunct;
			}
			set
			{
				_overflowPunct = value;
			}
		}

		public CT_OnOff TopLinePunct
		{
			get
			{
				return _topLinePunct;
			}
			set
			{
				_topLinePunct = value;
			}
		}

		public CT_OnOff AutoSpaceDE
		{
			get
			{
				return _autoSpaceDE;
			}
			set
			{
				_autoSpaceDE = value;
			}
		}

		public CT_OnOff AutoSpaceDN
		{
			get
			{
				return _autoSpaceDN;
			}
			set
			{
				_autoSpaceDN = value;
			}
		}

		public CT_OnOff Bidi
		{
			get
			{
				return _bidi;
			}
			set
			{
				_bidi = value;
			}
		}

		public CT_OnOff AdjustRightInd
		{
			get
			{
				return _adjustRightInd;
			}
			set
			{
				_adjustRightInd = value;
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

		public CT_Spacing Spacing
		{
			get
			{
				return _spacing;
			}
			set
			{
				_spacing = value;
			}
		}

		public CT_OnOff ContextualSpacing
		{
			get
			{
				return _contextualSpacing;
			}
			set
			{
				_contextualSpacing = value;
			}
		}

		public CT_OnOff MirrorIndents
		{
			get
			{
				return _mirrorIndents;
			}
			set
			{
				_mirrorIndents = value;
			}
		}

		public CT_OnOff SuppressOverlap
		{
			get
			{
				return _suppressOverlap;
			}
			set
			{
				_suppressOverlap = value;
			}
		}

		public CT_Jc Jc
		{
			get
			{
				return _jc;
			}
			set
			{
				_jc = value;
			}
		}

		public CT_DecimalNumber OutlineLvl
		{
			get
			{
				return _outlineLvl;
			}
			set
			{
				_outlineLvl = value;
			}
		}

		public CT_DecimalNumber DivId
		{
			get
			{
				return _divId;
			}
			set
			{
				_divId = value;
			}
		}

		public static string PStyleElementName => "pStyle";

		public static string KeepNextElementName => "keepNext";

		public static string KeepLinesElementName => "keepLines";

		public static string PageBreakBeforeElementName => "pageBreakBefore";

		public static string WidowControlElementName => "widowControl";

		public static string SuppressLineNumbersElementName => "suppressLineNumbers";

		public static string SuppressAutoHyphensElementName => "suppressAutoHyphens";

		public static string KinsokuElementName => "kinsoku";

		public static string WordWrapElementName => "wordWrap";

		public static string OverflowPunctElementName => "overflowPunct";

		public static string TopLinePunctElementName => "topLinePunct";

		public static string AutoSpaceDEElementName => "autoSpaceDE";

		public static string AutoSpaceDNElementName => "autoSpaceDN";

		public static string BidiElementName => "bidi";

		public static string AdjustRightIndElementName => "adjustRightInd";

		public static string SnapToGridElementName => "snapToGrid";

		public static string SpacingElementName => "spacing";

		public static string ContextualSpacingElementName => "contextualSpacing";

		public static string MirrorIndentsElementName => "mirrorIndents";

		public static string SuppressOverlapElementName => "suppressOverlap";

		public static string JcElementName => "jc";

		public static string OutlineLvlElementName => "outlineLvl";

		public static string DivIdElementName => "divId";

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
			Write_pStyle(s);
			Write_keepNext(s);
			Write_keepLines(s);
			Write_pageBreakBefore(s);
			Write_widowControl(s);
			Write_suppressLineNumbers(s);
			Write_suppressAutoHyphens(s);
			Write_kinsoku(s);
			Write_wordWrap(s);
			Write_overflowPunct(s);
			Write_topLinePunct(s);
			Write_autoSpaceDE(s);
			Write_autoSpaceDN(s);
			Write_bidi(s);
			Write_adjustRightInd(s);
			Write_snapToGrid(s);
			Write_spacing(s);
			Write_contextualSpacing(s);
			Write_mirrorIndents(s);
			Write_suppressOverlap(s);
			Write_jc(s);
			Write_outlineLvl(s);
			Write_divId(s);
		}

		public void Write_pStyle(TextWriter s)
		{
			if (_pStyle != null)
			{
				_pStyle.Write(s, "pStyle");
			}
		}

		public void Write_keepNext(TextWriter s)
		{
			if (_keepNext != null)
			{
				_keepNext.Write(s, "keepNext");
			}
		}

		public void Write_keepLines(TextWriter s)
		{
			if (_keepLines != null)
			{
				_keepLines.Write(s, "keepLines");
			}
		}

		public void Write_pageBreakBefore(TextWriter s)
		{
			if (_pageBreakBefore != null)
			{
				_pageBreakBefore.Write(s, "pageBreakBefore");
			}
		}

		public void Write_widowControl(TextWriter s)
		{
			if (_widowControl != null)
			{
				_widowControl.Write(s, "widowControl");
			}
		}

		public void Write_suppressLineNumbers(TextWriter s)
		{
			if (_suppressLineNumbers != null)
			{
				_suppressLineNumbers.Write(s, "suppressLineNumbers");
			}
		}

		public void Write_suppressAutoHyphens(TextWriter s)
		{
			if (_suppressAutoHyphens != null)
			{
				_suppressAutoHyphens.Write(s, "suppressAutoHyphens");
			}
		}

		public void Write_kinsoku(TextWriter s)
		{
			if (_kinsoku != null)
			{
				_kinsoku.Write(s, "kinsoku");
			}
		}

		public void Write_wordWrap(TextWriter s)
		{
			if (_wordWrap != null)
			{
				_wordWrap.Write(s, "wordWrap");
			}
		}

		public void Write_overflowPunct(TextWriter s)
		{
			if (_overflowPunct != null)
			{
				_overflowPunct.Write(s, "overflowPunct");
			}
		}

		public void Write_topLinePunct(TextWriter s)
		{
			if (_topLinePunct != null)
			{
				_topLinePunct.Write(s, "topLinePunct");
			}
		}

		public void Write_autoSpaceDE(TextWriter s)
		{
			if (_autoSpaceDE != null)
			{
				_autoSpaceDE.Write(s, "autoSpaceDE");
			}
		}

		public void Write_autoSpaceDN(TextWriter s)
		{
			if (_autoSpaceDN != null)
			{
				_autoSpaceDN.Write(s, "autoSpaceDN");
			}
		}

		public void Write_bidi(TextWriter s)
		{
			if (_bidi != null)
			{
				_bidi.Write(s, "bidi");
			}
		}

		public void Write_adjustRightInd(TextWriter s)
		{
			if (_adjustRightInd != null)
			{
				_adjustRightInd.Write(s, "adjustRightInd");
			}
		}

		public void Write_snapToGrid(TextWriter s)
		{
			if (_snapToGrid != null)
			{
				_snapToGrid.Write(s, "snapToGrid");
			}
		}

		public void Write_spacing(TextWriter s)
		{
			if (_spacing != null)
			{
				_spacing.Write(s, "spacing");
			}
		}

		public void Write_contextualSpacing(TextWriter s)
		{
			if (_contextualSpacing != null)
			{
				_contextualSpacing.Write(s, "contextualSpacing");
			}
		}

		public void Write_mirrorIndents(TextWriter s)
		{
			if (_mirrorIndents != null)
			{
				_mirrorIndents.Write(s, "mirrorIndents");
			}
		}

		public void Write_suppressOverlap(TextWriter s)
		{
			if (_suppressOverlap != null)
			{
				_suppressOverlap.Write(s, "suppressOverlap");
			}
		}

		public void Write_jc(TextWriter s)
		{
			if (_jc != null)
			{
				_jc.Write(s, "jc");
			}
		}

		public void Write_outlineLvl(TextWriter s)
		{
			if (_outlineLvl != null)
			{
				_outlineLvl.Write(s, "outlineLvl");
			}
		}

		public void Write_divId(TextWriter s)
		{
			if (_divId != null)
			{
				_divId.Write(s, "divId");
			}
		}
	}
}
