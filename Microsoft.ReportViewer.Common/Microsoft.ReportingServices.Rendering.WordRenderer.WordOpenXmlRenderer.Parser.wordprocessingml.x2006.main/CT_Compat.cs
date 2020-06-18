using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Compat : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_OnOff _useSingleBorderforContiguousCells;

		private CT_OnOff _wpJustification;

		private CT_OnOff _noTabHangInd;

		private CT_OnOff _noLeading;

		private CT_OnOff _spaceForUL;

		private CT_OnOff _noColumnBalance;

		private CT_OnOff _balanceSingleByteDoubleByteWidth;

		private CT_OnOff _noExtraLineSpacing;

		private CT_OnOff _doNotLeaveBackslashAlone;

		private CT_OnOff _ulTrailSpace;

		private CT_OnOff _doNotExpandShiftReturn;

		private CT_OnOff _spacingInWholePoints;

		private CT_OnOff _lineWrapLikeWord6;

		private CT_OnOff _printBodyTextBeforeHeader;

		private CT_OnOff _printColBlack;

		private CT_OnOff _wpSpaceWidth;

		private CT_OnOff _showBreaksInFrames;

		private CT_OnOff _subFontBySize;

		private CT_OnOff _suppressBottomSpacing;

		private CT_OnOff _suppressTopSpacing;

		private CT_OnOff _suppressSpacingAtTopOfPage;

		private CT_OnOff _suppressTopSpacingWP;

		private CT_OnOff _suppressSpBfAfterPgBrk;

		private CT_OnOff _swapBordersFacingPages;

		private CT_OnOff _convMailMergeEsc;

		private CT_OnOff _truncateFontHeightsLikeWP6;

		private CT_OnOff _mwSmallCaps;

		private CT_OnOff _usePrinterMetrics;

		private CT_OnOff _doNotSuppressParagraphBorders;

		private CT_OnOff _wrapTrailSpaces;

		private CT_OnOff _footnoteLayoutLikeWW8;

		private CT_OnOff _shapeLayoutLikeWW8;

		private CT_OnOff _alignTablesRowByRow;

		private CT_OnOff _forgetLastTabAlignment;

		private CT_OnOff _adjustLineHeightInTable;

		private CT_OnOff _autoSpaceLikeWord95;

		private CT_OnOff _noSpaceRaiseLower;

		private CT_OnOff _doNotUseHTMLParagraphAutoSpacing;

		private CT_OnOff _layoutRawTableWidth;

		private CT_OnOff _layoutTableRowsApart;

		private CT_OnOff _useWord97LineBreakRules;

		private CT_OnOff _doNotBreakWrappedTables;

		private CT_OnOff _doNotSnapToGridInCell;

		private CT_OnOff _selectFldWithFirstOrLastChar;

		private CT_OnOff _applyBreakingRules;

		private CT_OnOff _doNotWrapTextWithPunct;

		private CT_OnOff _doNotUseEastAsianBreakRules;

		private CT_OnOff _useWord2002TableStyleRules;

		private CT_OnOff _growAutofit;

		private CT_OnOff _useFELayout;

		private CT_OnOff _useNormalStyleForList;

		private CT_OnOff _doNotUseIndentAsNumberingTabStop;

		private CT_OnOff _useAltKinsokuLineBreakRules;

		private CT_OnOff _allowSpaceOfSameStyleInTable;

		private CT_OnOff _doNotSuppressIndentation;

		private CT_OnOff _doNotAutofitConstrainedTables;

		private CT_OnOff _autofitToFirstFixedWidthCell;

		private CT_OnOff _underlineTabInNumList;

		private CT_OnOff _displayHangulFixedWidth;

		private CT_OnOff _splitPgBreakAndParaMark;

		private CT_OnOff _doNotVertAlignCellWithSp;

		private CT_OnOff _doNotBreakConstrainedForcedTable;

		private CT_OnOff _doNotVertAlignInTxbx;

		private CT_OnOff _useAnsiKerningPairs;

		private CT_OnOff _cachedColBalance;

		public CT_OnOff UseSingleBorderforContiguousCells
		{
			get
			{
				return _useSingleBorderforContiguousCells;
			}
			set
			{
				_useSingleBorderforContiguousCells = value;
			}
		}

		public CT_OnOff WpJustification
		{
			get
			{
				return _wpJustification;
			}
			set
			{
				_wpJustification = value;
			}
		}

		public CT_OnOff NoTabHangInd
		{
			get
			{
				return _noTabHangInd;
			}
			set
			{
				_noTabHangInd = value;
			}
		}

		public CT_OnOff NoLeading
		{
			get
			{
				return _noLeading;
			}
			set
			{
				_noLeading = value;
			}
		}

		public CT_OnOff SpaceForUL
		{
			get
			{
				return _spaceForUL;
			}
			set
			{
				_spaceForUL = value;
			}
		}

		public CT_OnOff NoColumnBalance
		{
			get
			{
				return _noColumnBalance;
			}
			set
			{
				_noColumnBalance = value;
			}
		}

		public CT_OnOff BalanceSingleByteDoubleByteWidth
		{
			get
			{
				return _balanceSingleByteDoubleByteWidth;
			}
			set
			{
				_balanceSingleByteDoubleByteWidth = value;
			}
		}

		public CT_OnOff NoExtraLineSpacing
		{
			get
			{
				return _noExtraLineSpacing;
			}
			set
			{
				_noExtraLineSpacing = value;
			}
		}

		public CT_OnOff DoNotLeaveBackslashAlone
		{
			get
			{
				return _doNotLeaveBackslashAlone;
			}
			set
			{
				_doNotLeaveBackslashAlone = value;
			}
		}

		public CT_OnOff UlTrailSpace
		{
			get
			{
				return _ulTrailSpace;
			}
			set
			{
				_ulTrailSpace = value;
			}
		}

		public CT_OnOff DoNotExpandShiftReturn
		{
			get
			{
				return _doNotExpandShiftReturn;
			}
			set
			{
				_doNotExpandShiftReturn = value;
			}
		}

		public CT_OnOff SpacingInWholePoints
		{
			get
			{
				return _spacingInWholePoints;
			}
			set
			{
				_spacingInWholePoints = value;
			}
		}

		public CT_OnOff LineWrapLikeWord6
		{
			get
			{
				return _lineWrapLikeWord6;
			}
			set
			{
				_lineWrapLikeWord6 = value;
			}
		}

		public CT_OnOff PrintBodyTextBeforeHeader
		{
			get
			{
				return _printBodyTextBeforeHeader;
			}
			set
			{
				_printBodyTextBeforeHeader = value;
			}
		}

		public CT_OnOff PrintColBlack
		{
			get
			{
				return _printColBlack;
			}
			set
			{
				_printColBlack = value;
			}
		}

		public CT_OnOff WpSpaceWidth
		{
			get
			{
				return _wpSpaceWidth;
			}
			set
			{
				_wpSpaceWidth = value;
			}
		}

		public CT_OnOff ShowBreaksInFrames
		{
			get
			{
				return _showBreaksInFrames;
			}
			set
			{
				_showBreaksInFrames = value;
			}
		}

		public CT_OnOff SubFontBySize
		{
			get
			{
				return _subFontBySize;
			}
			set
			{
				_subFontBySize = value;
			}
		}

		public CT_OnOff SuppressBottomSpacing
		{
			get
			{
				return _suppressBottomSpacing;
			}
			set
			{
				_suppressBottomSpacing = value;
			}
		}

		public CT_OnOff SuppressTopSpacing
		{
			get
			{
				return _suppressTopSpacing;
			}
			set
			{
				_suppressTopSpacing = value;
			}
		}

		public CT_OnOff SuppressSpacingAtTopOfPage
		{
			get
			{
				return _suppressSpacingAtTopOfPage;
			}
			set
			{
				_suppressSpacingAtTopOfPage = value;
			}
		}

		public CT_OnOff SuppressTopSpacingWP
		{
			get
			{
				return _suppressTopSpacingWP;
			}
			set
			{
				_suppressTopSpacingWP = value;
			}
		}

		public CT_OnOff SuppressSpBfAfterPgBrk
		{
			get
			{
				return _suppressSpBfAfterPgBrk;
			}
			set
			{
				_suppressSpBfAfterPgBrk = value;
			}
		}

		public CT_OnOff SwapBordersFacingPages
		{
			get
			{
				return _swapBordersFacingPages;
			}
			set
			{
				_swapBordersFacingPages = value;
			}
		}

		public CT_OnOff ConvMailMergeEsc
		{
			get
			{
				return _convMailMergeEsc;
			}
			set
			{
				_convMailMergeEsc = value;
			}
		}

		public CT_OnOff TruncateFontHeightsLikeWP6
		{
			get
			{
				return _truncateFontHeightsLikeWP6;
			}
			set
			{
				_truncateFontHeightsLikeWP6 = value;
			}
		}

		public CT_OnOff MwSmallCaps
		{
			get
			{
				return _mwSmallCaps;
			}
			set
			{
				_mwSmallCaps = value;
			}
		}

		public CT_OnOff UsePrinterMetrics
		{
			get
			{
				return _usePrinterMetrics;
			}
			set
			{
				_usePrinterMetrics = value;
			}
		}

		public CT_OnOff DoNotSuppressParagraphBorders
		{
			get
			{
				return _doNotSuppressParagraphBorders;
			}
			set
			{
				_doNotSuppressParagraphBorders = value;
			}
		}

		public CT_OnOff WrapTrailSpaces
		{
			get
			{
				return _wrapTrailSpaces;
			}
			set
			{
				_wrapTrailSpaces = value;
			}
		}

		public CT_OnOff FootnoteLayoutLikeWW8
		{
			get
			{
				return _footnoteLayoutLikeWW8;
			}
			set
			{
				_footnoteLayoutLikeWW8 = value;
			}
		}

		public CT_OnOff ShapeLayoutLikeWW8
		{
			get
			{
				return _shapeLayoutLikeWW8;
			}
			set
			{
				_shapeLayoutLikeWW8 = value;
			}
		}

		public CT_OnOff AlignTablesRowByRow
		{
			get
			{
				return _alignTablesRowByRow;
			}
			set
			{
				_alignTablesRowByRow = value;
			}
		}

		public CT_OnOff ForgetLastTabAlignment
		{
			get
			{
				return _forgetLastTabAlignment;
			}
			set
			{
				_forgetLastTabAlignment = value;
			}
		}

		public CT_OnOff AdjustLineHeightInTable
		{
			get
			{
				return _adjustLineHeightInTable;
			}
			set
			{
				_adjustLineHeightInTable = value;
			}
		}

		public CT_OnOff AutoSpaceLikeWord95
		{
			get
			{
				return _autoSpaceLikeWord95;
			}
			set
			{
				_autoSpaceLikeWord95 = value;
			}
		}

		public CT_OnOff NoSpaceRaiseLower
		{
			get
			{
				return _noSpaceRaiseLower;
			}
			set
			{
				_noSpaceRaiseLower = value;
			}
		}

		public CT_OnOff DoNotUseHTMLParagraphAutoSpacing
		{
			get
			{
				return _doNotUseHTMLParagraphAutoSpacing;
			}
			set
			{
				_doNotUseHTMLParagraphAutoSpacing = value;
			}
		}

		public CT_OnOff LayoutRawTableWidth
		{
			get
			{
				return _layoutRawTableWidth;
			}
			set
			{
				_layoutRawTableWidth = value;
			}
		}

		public CT_OnOff LayoutTableRowsApart
		{
			get
			{
				return _layoutTableRowsApart;
			}
			set
			{
				_layoutTableRowsApart = value;
			}
		}

		public CT_OnOff UseWord97LineBreakRules
		{
			get
			{
				return _useWord97LineBreakRules;
			}
			set
			{
				_useWord97LineBreakRules = value;
			}
		}

		public CT_OnOff DoNotBreakWrappedTables
		{
			get
			{
				return _doNotBreakWrappedTables;
			}
			set
			{
				_doNotBreakWrappedTables = value;
			}
		}

		public CT_OnOff DoNotSnapToGridInCell
		{
			get
			{
				return _doNotSnapToGridInCell;
			}
			set
			{
				_doNotSnapToGridInCell = value;
			}
		}

		public CT_OnOff SelectFldWithFirstOrLastChar
		{
			get
			{
				return _selectFldWithFirstOrLastChar;
			}
			set
			{
				_selectFldWithFirstOrLastChar = value;
			}
		}

		public CT_OnOff ApplyBreakingRules
		{
			get
			{
				return _applyBreakingRules;
			}
			set
			{
				_applyBreakingRules = value;
			}
		}

		public CT_OnOff DoNotWrapTextWithPunct
		{
			get
			{
				return _doNotWrapTextWithPunct;
			}
			set
			{
				_doNotWrapTextWithPunct = value;
			}
		}

		public CT_OnOff DoNotUseEastAsianBreakRules
		{
			get
			{
				return _doNotUseEastAsianBreakRules;
			}
			set
			{
				_doNotUseEastAsianBreakRules = value;
			}
		}

		public CT_OnOff UseWord2002TableStyleRules
		{
			get
			{
				return _useWord2002TableStyleRules;
			}
			set
			{
				_useWord2002TableStyleRules = value;
			}
		}

		public CT_OnOff GrowAutofit
		{
			get
			{
				return _growAutofit;
			}
			set
			{
				_growAutofit = value;
			}
		}

		public CT_OnOff UseFELayout
		{
			get
			{
				return _useFELayout;
			}
			set
			{
				_useFELayout = value;
			}
		}

		public CT_OnOff UseNormalStyleForList
		{
			get
			{
				return _useNormalStyleForList;
			}
			set
			{
				_useNormalStyleForList = value;
			}
		}

		public CT_OnOff DoNotUseIndentAsNumberingTabStop
		{
			get
			{
				return _doNotUseIndentAsNumberingTabStop;
			}
			set
			{
				_doNotUseIndentAsNumberingTabStop = value;
			}
		}

		public CT_OnOff UseAltKinsokuLineBreakRules
		{
			get
			{
				return _useAltKinsokuLineBreakRules;
			}
			set
			{
				_useAltKinsokuLineBreakRules = value;
			}
		}

		public CT_OnOff AllowSpaceOfSameStyleInTable
		{
			get
			{
				return _allowSpaceOfSameStyleInTable;
			}
			set
			{
				_allowSpaceOfSameStyleInTable = value;
			}
		}

		public CT_OnOff DoNotSuppressIndentation
		{
			get
			{
				return _doNotSuppressIndentation;
			}
			set
			{
				_doNotSuppressIndentation = value;
			}
		}

		public CT_OnOff DoNotAutofitConstrainedTables
		{
			get
			{
				return _doNotAutofitConstrainedTables;
			}
			set
			{
				_doNotAutofitConstrainedTables = value;
			}
		}

		public CT_OnOff AutofitToFirstFixedWidthCell
		{
			get
			{
				return _autofitToFirstFixedWidthCell;
			}
			set
			{
				_autofitToFirstFixedWidthCell = value;
			}
		}

		public CT_OnOff UnderlineTabInNumList
		{
			get
			{
				return _underlineTabInNumList;
			}
			set
			{
				_underlineTabInNumList = value;
			}
		}

		public CT_OnOff DisplayHangulFixedWidth
		{
			get
			{
				return _displayHangulFixedWidth;
			}
			set
			{
				_displayHangulFixedWidth = value;
			}
		}

		public CT_OnOff SplitPgBreakAndParaMark
		{
			get
			{
				return _splitPgBreakAndParaMark;
			}
			set
			{
				_splitPgBreakAndParaMark = value;
			}
		}

		public CT_OnOff DoNotVertAlignCellWithSp
		{
			get
			{
				return _doNotVertAlignCellWithSp;
			}
			set
			{
				_doNotVertAlignCellWithSp = value;
			}
		}

		public CT_OnOff DoNotBreakConstrainedForcedTable
		{
			get
			{
				return _doNotBreakConstrainedForcedTable;
			}
			set
			{
				_doNotBreakConstrainedForcedTable = value;
			}
		}

		public CT_OnOff DoNotVertAlignInTxbx
		{
			get
			{
				return _doNotVertAlignInTxbx;
			}
			set
			{
				_doNotVertAlignInTxbx = value;
			}
		}

		public CT_OnOff UseAnsiKerningPairs
		{
			get
			{
				return _useAnsiKerningPairs;
			}
			set
			{
				_useAnsiKerningPairs = value;
			}
		}

		public CT_OnOff CachedColBalance
		{
			get
			{
				return _cachedColBalance;
			}
			set
			{
				_cachedColBalance = value;
			}
		}

		public static string UseSingleBorderforContiguousCellsElementName => "useSingleBorderforContiguousCells";

		public static string WpJustificationElementName => "wpJustification";

		public static string NoTabHangIndElementName => "noTabHangInd";

		public static string NoLeadingElementName => "noLeading";

		public static string SpaceForULElementName => "spaceForUL";

		public static string NoColumnBalanceElementName => "noColumnBalance";

		public static string BalanceSingleByteDoubleByteWidthElementName => "balanceSingleByteDoubleByteWidth";

		public static string NoExtraLineSpacingElementName => "noExtraLineSpacing";

		public static string DoNotLeaveBackslashAloneElementName => "doNotLeaveBackslashAlone";

		public static string UlTrailSpaceElementName => "ulTrailSpace";

		public static string DoNotExpandShiftReturnElementName => "doNotExpandShiftReturn";

		public static string SpacingInWholePointsElementName => "spacingInWholePoints";

		public static string LineWrapLikeWord6ElementName => "lineWrapLikeWord6";

		public static string PrintBodyTextBeforeHeaderElementName => "printBodyTextBeforeHeader";

		public static string PrintColBlackElementName => "printColBlack";

		public static string WpSpaceWidthElementName => "wpSpaceWidth";

		public static string ShowBreaksInFramesElementName => "showBreaksInFrames";

		public static string SubFontBySizeElementName => "subFontBySize";

		public static string SuppressBottomSpacingElementName => "suppressBottomSpacing";

		public static string SuppressTopSpacingElementName => "suppressTopSpacing";

		public static string SuppressSpacingAtTopOfPageElementName => "suppressSpacingAtTopOfPage";

		public static string SuppressTopSpacingWPElementName => "suppressTopSpacingWP";

		public static string SuppressSpBfAfterPgBrkElementName => "suppressSpBfAfterPgBrk";

		public static string SwapBordersFacingPagesElementName => "swapBordersFacingPages";

		public static string ConvMailMergeEscElementName => "convMailMergeEsc";

		public static string TruncateFontHeightsLikeWP6ElementName => "truncateFontHeightsLikeWP6";

		public static string MwSmallCapsElementName => "mwSmallCaps";

		public static string UsePrinterMetricsElementName => "usePrinterMetrics";

		public static string DoNotSuppressParagraphBordersElementName => "doNotSuppressParagraphBorders";

		public static string WrapTrailSpacesElementName => "wrapTrailSpaces";

		public static string FootnoteLayoutLikeWW8ElementName => "footnoteLayoutLikeWW8";

		public static string ShapeLayoutLikeWW8ElementName => "shapeLayoutLikeWW8";

		public static string AlignTablesRowByRowElementName => "alignTablesRowByRow";

		public static string ForgetLastTabAlignmentElementName => "forgetLastTabAlignment";

		public static string AdjustLineHeightInTableElementName => "adjustLineHeightInTable";

		public static string AutoSpaceLikeWord95ElementName => "autoSpaceLikeWord95";

		public static string NoSpaceRaiseLowerElementName => "noSpaceRaiseLower";

		public static string DoNotUseHTMLParagraphAutoSpacingElementName => "doNotUseHTMLParagraphAutoSpacing";

		public static string LayoutRawTableWidthElementName => "layoutRawTableWidth";

		public static string LayoutTableRowsApartElementName => "layoutTableRowsApart";

		public static string UseWord97LineBreakRulesElementName => "useWord97LineBreakRules";

		public static string DoNotBreakWrappedTablesElementName => "doNotBreakWrappedTables";

		public static string DoNotSnapToGridInCellElementName => "doNotSnapToGridInCell";

		public static string SelectFldWithFirstOrLastCharElementName => "selectFldWithFirstOrLastChar";

		public static string ApplyBreakingRulesElementName => "applyBreakingRules";

		public static string DoNotWrapTextWithPunctElementName => "doNotWrapTextWithPunct";

		public static string DoNotUseEastAsianBreakRulesElementName => "doNotUseEastAsianBreakRules";

		public static string UseWord2002TableStyleRulesElementName => "useWord2002TableStyleRules";

		public static string GrowAutofitElementName => "growAutofit";

		public static string UseFELayoutElementName => "useFELayout";

		public static string UseNormalStyleForListElementName => "useNormalStyleForList";

		public static string DoNotUseIndentAsNumberingTabStopElementName => "doNotUseIndentAsNumberingTabStop";

		public static string UseAltKinsokuLineBreakRulesElementName => "useAltKinsokuLineBreakRules";

		public static string AllowSpaceOfSameStyleInTableElementName => "allowSpaceOfSameStyleInTable";

		public static string DoNotSuppressIndentationElementName => "doNotSuppressIndentation";

		public static string DoNotAutofitConstrainedTablesElementName => "doNotAutofitConstrainedTables";

		public static string AutofitToFirstFixedWidthCellElementName => "autofitToFirstFixedWidthCell";

		public static string UnderlineTabInNumListElementName => "underlineTabInNumList";

		public static string DisplayHangulFixedWidthElementName => "displayHangulFixedWidth";

		public static string SplitPgBreakAndParaMarkElementName => "splitPgBreakAndParaMark";

		public static string DoNotVertAlignCellWithSpElementName => "doNotVertAlignCellWithSp";

		public static string DoNotBreakConstrainedForcedTableElementName => "doNotBreakConstrainedForcedTable";

		public static string DoNotVertAlignInTxbxElementName => "doNotVertAlignInTxbx";

		public static string UseAnsiKerningPairsElementName => "useAnsiKerningPairs";

		public static string CachedColBalanceElementName => "cachedColBalance";

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
			Write_useSingleBorderforContiguousCells(s);
			Write_wpJustification(s);
			Write_noTabHangInd(s);
			Write_noLeading(s);
			Write_spaceForUL(s);
			Write_noColumnBalance(s);
			Write_balanceSingleByteDoubleByteWidth(s);
			Write_noExtraLineSpacing(s);
			Write_doNotLeaveBackslashAlone(s);
			Write_ulTrailSpace(s);
			Write_doNotExpandShiftReturn(s);
			Write_spacingInWholePoints(s);
			Write_lineWrapLikeWord6(s);
			Write_printBodyTextBeforeHeader(s);
			Write_printColBlack(s);
			Write_wpSpaceWidth(s);
			Write_showBreaksInFrames(s);
			Write_subFontBySize(s);
			Write_suppressBottomSpacing(s);
			Write_suppressTopSpacing(s);
			Write_suppressSpacingAtTopOfPage(s);
			Write_suppressTopSpacingWP(s);
			Write_suppressSpBfAfterPgBrk(s);
			Write_swapBordersFacingPages(s);
			Write_convMailMergeEsc(s);
			Write_truncateFontHeightsLikeWP6(s);
			Write_mwSmallCaps(s);
			Write_usePrinterMetrics(s);
			Write_doNotSuppressParagraphBorders(s);
			Write_wrapTrailSpaces(s);
			Write_footnoteLayoutLikeWW8(s);
			Write_shapeLayoutLikeWW8(s);
			Write_alignTablesRowByRow(s);
			Write_forgetLastTabAlignment(s);
			Write_adjustLineHeightInTable(s);
			Write_autoSpaceLikeWord95(s);
			Write_noSpaceRaiseLower(s);
			Write_doNotUseHTMLParagraphAutoSpacing(s);
			Write_layoutRawTableWidth(s);
			Write_layoutTableRowsApart(s);
			Write_useWord97LineBreakRules(s);
			Write_doNotBreakWrappedTables(s);
			Write_doNotSnapToGridInCell(s);
			Write_selectFldWithFirstOrLastChar(s);
			Write_applyBreakingRules(s);
			Write_doNotWrapTextWithPunct(s);
			Write_doNotUseEastAsianBreakRules(s);
			Write_useWord2002TableStyleRules(s);
			Write_growAutofit(s);
			Write_useFELayout(s);
			Write_useNormalStyleForList(s);
			Write_doNotUseIndentAsNumberingTabStop(s);
			Write_useAltKinsokuLineBreakRules(s);
			Write_allowSpaceOfSameStyleInTable(s);
			Write_doNotSuppressIndentation(s);
			Write_doNotAutofitConstrainedTables(s);
			Write_autofitToFirstFixedWidthCell(s);
			Write_underlineTabInNumList(s);
			Write_displayHangulFixedWidth(s);
			Write_splitPgBreakAndParaMark(s);
			Write_doNotVertAlignCellWithSp(s);
			Write_doNotBreakConstrainedForcedTable(s);
			Write_doNotVertAlignInTxbx(s);
			Write_useAnsiKerningPairs(s);
			Write_cachedColBalance(s);
		}

		public void Write_useSingleBorderforContiguousCells(TextWriter s)
		{
			if (_useSingleBorderforContiguousCells != null)
			{
				_useSingleBorderforContiguousCells.Write(s, "useSingleBorderforContiguousCells");
			}
		}

		public void Write_wpJustification(TextWriter s)
		{
			if (_wpJustification != null)
			{
				_wpJustification.Write(s, "wpJustification");
			}
		}

		public void Write_noTabHangInd(TextWriter s)
		{
			if (_noTabHangInd != null)
			{
				_noTabHangInd.Write(s, "noTabHangInd");
			}
		}

		public void Write_noLeading(TextWriter s)
		{
			if (_noLeading != null)
			{
				_noLeading.Write(s, "noLeading");
			}
		}

		public void Write_spaceForUL(TextWriter s)
		{
			if (_spaceForUL != null)
			{
				_spaceForUL.Write(s, "spaceForUL");
			}
		}

		public void Write_noColumnBalance(TextWriter s)
		{
			if (_noColumnBalance != null)
			{
				_noColumnBalance.Write(s, "noColumnBalance");
			}
		}

		public void Write_balanceSingleByteDoubleByteWidth(TextWriter s)
		{
			if (_balanceSingleByteDoubleByteWidth != null)
			{
				_balanceSingleByteDoubleByteWidth.Write(s, "balanceSingleByteDoubleByteWidth");
			}
		}

		public void Write_noExtraLineSpacing(TextWriter s)
		{
			if (_noExtraLineSpacing != null)
			{
				_noExtraLineSpacing.Write(s, "noExtraLineSpacing");
			}
		}

		public void Write_doNotLeaveBackslashAlone(TextWriter s)
		{
			if (_doNotLeaveBackslashAlone != null)
			{
				_doNotLeaveBackslashAlone.Write(s, "doNotLeaveBackslashAlone");
			}
		}

		public void Write_ulTrailSpace(TextWriter s)
		{
			if (_ulTrailSpace != null)
			{
				_ulTrailSpace.Write(s, "ulTrailSpace");
			}
		}

		public void Write_doNotExpandShiftReturn(TextWriter s)
		{
			if (_doNotExpandShiftReturn != null)
			{
				_doNotExpandShiftReturn.Write(s, "doNotExpandShiftReturn");
			}
		}

		public void Write_spacingInWholePoints(TextWriter s)
		{
			if (_spacingInWholePoints != null)
			{
				_spacingInWholePoints.Write(s, "spacingInWholePoints");
			}
		}

		public void Write_lineWrapLikeWord6(TextWriter s)
		{
			if (_lineWrapLikeWord6 != null)
			{
				_lineWrapLikeWord6.Write(s, "lineWrapLikeWord6");
			}
		}

		public void Write_printBodyTextBeforeHeader(TextWriter s)
		{
			if (_printBodyTextBeforeHeader != null)
			{
				_printBodyTextBeforeHeader.Write(s, "printBodyTextBeforeHeader");
			}
		}

		public void Write_printColBlack(TextWriter s)
		{
			if (_printColBlack != null)
			{
				_printColBlack.Write(s, "printColBlack");
			}
		}

		public void Write_wpSpaceWidth(TextWriter s)
		{
			if (_wpSpaceWidth != null)
			{
				_wpSpaceWidth.Write(s, "wpSpaceWidth");
			}
		}

		public void Write_showBreaksInFrames(TextWriter s)
		{
			if (_showBreaksInFrames != null)
			{
				_showBreaksInFrames.Write(s, "showBreaksInFrames");
			}
		}

		public void Write_subFontBySize(TextWriter s)
		{
			if (_subFontBySize != null)
			{
				_subFontBySize.Write(s, "subFontBySize");
			}
		}

		public void Write_suppressBottomSpacing(TextWriter s)
		{
			if (_suppressBottomSpacing != null)
			{
				_suppressBottomSpacing.Write(s, "suppressBottomSpacing");
			}
		}

		public void Write_suppressTopSpacing(TextWriter s)
		{
			if (_suppressTopSpacing != null)
			{
				_suppressTopSpacing.Write(s, "suppressTopSpacing");
			}
		}

		public void Write_suppressSpacingAtTopOfPage(TextWriter s)
		{
			if (_suppressSpacingAtTopOfPage != null)
			{
				_suppressSpacingAtTopOfPage.Write(s, "suppressSpacingAtTopOfPage");
			}
		}

		public void Write_suppressTopSpacingWP(TextWriter s)
		{
			if (_suppressTopSpacingWP != null)
			{
				_suppressTopSpacingWP.Write(s, "suppressTopSpacingWP");
			}
		}

		public void Write_suppressSpBfAfterPgBrk(TextWriter s)
		{
			if (_suppressSpBfAfterPgBrk != null)
			{
				_suppressSpBfAfterPgBrk.Write(s, "suppressSpBfAfterPgBrk");
			}
		}

		public void Write_swapBordersFacingPages(TextWriter s)
		{
			if (_swapBordersFacingPages != null)
			{
				_swapBordersFacingPages.Write(s, "swapBordersFacingPages");
			}
		}

		public void Write_convMailMergeEsc(TextWriter s)
		{
			if (_convMailMergeEsc != null)
			{
				_convMailMergeEsc.Write(s, "convMailMergeEsc");
			}
		}

		public void Write_truncateFontHeightsLikeWP6(TextWriter s)
		{
			if (_truncateFontHeightsLikeWP6 != null)
			{
				_truncateFontHeightsLikeWP6.Write(s, "truncateFontHeightsLikeWP6");
			}
		}

		public void Write_mwSmallCaps(TextWriter s)
		{
			if (_mwSmallCaps != null)
			{
				_mwSmallCaps.Write(s, "mwSmallCaps");
			}
		}

		public void Write_usePrinterMetrics(TextWriter s)
		{
			if (_usePrinterMetrics != null)
			{
				_usePrinterMetrics.Write(s, "usePrinterMetrics");
			}
		}

		public void Write_doNotSuppressParagraphBorders(TextWriter s)
		{
			if (_doNotSuppressParagraphBorders != null)
			{
				_doNotSuppressParagraphBorders.Write(s, "doNotSuppressParagraphBorders");
			}
		}

		public void Write_wrapTrailSpaces(TextWriter s)
		{
			if (_wrapTrailSpaces != null)
			{
				_wrapTrailSpaces.Write(s, "wrapTrailSpaces");
			}
		}

		public void Write_footnoteLayoutLikeWW8(TextWriter s)
		{
			if (_footnoteLayoutLikeWW8 != null)
			{
				_footnoteLayoutLikeWW8.Write(s, "footnoteLayoutLikeWW8");
			}
		}

		public void Write_shapeLayoutLikeWW8(TextWriter s)
		{
			if (_shapeLayoutLikeWW8 != null)
			{
				_shapeLayoutLikeWW8.Write(s, "shapeLayoutLikeWW8");
			}
		}

		public void Write_alignTablesRowByRow(TextWriter s)
		{
			if (_alignTablesRowByRow != null)
			{
				_alignTablesRowByRow.Write(s, "alignTablesRowByRow");
			}
		}

		public void Write_forgetLastTabAlignment(TextWriter s)
		{
			if (_forgetLastTabAlignment != null)
			{
				_forgetLastTabAlignment.Write(s, "forgetLastTabAlignment");
			}
		}

		public void Write_adjustLineHeightInTable(TextWriter s)
		{
			if (_adjustLineHeightInTable != null)
			{
				_adjustLineHeightInTable.Write(s, "adjustLineHeightInTable");
			}
		}

		public void Write_autoSpaceLikeWord95(TextWriter s)
		{
			if (_autoSpaceLikeWord95 != null)
			{
				_autoSpaceLikeWord95.Write(s, "autoSpaceLikeWord95");
			}
		}

		public void Write_noSpaceRaiseLower(TextWriter s)
		{
			if (_noSpaceRaiseLower != null)
			{
				_noSpaceRaiseLower.Write(s, "noSpaceRaiseLower");
			}
		}

		public void Write_doNotUseHTMLParagraphAutoSpacing(TextWriter s)
		{
			if (_doNotUseHTMLParagraphAutoSpacing != null)
			{
				_doNotUseHTMLParagraphAutoSpacing.Write(s, "doNotUseHTMLParagraphAutoSpacing");
			}
		}

		public void Write_layoutRawTableWidth(TextWriter s)
		{
			if (_layoutRawTableWidth != null)
			{
				_layoutRawTableWidth.Write(s, "layoutRawTableWidth");
			}
		}

		public void Write_layoutTableRowsApart(TextWriter s)
		{
			if (_layoutTableRowsApart != null)
			{
				_layoutTableRowsApart.Write(s, "layoutTableRowsApart");
			}
		}

		public void Write_useWord97LineBreakRules(TextWriter s)
		{
			if (_useWord97LineBreakRules != null)
			{
				_useWord97LineBreakRules.Write(s, "useWord97LineBreakRules");
			}
		}

		public void Write_doNotBreakWrappedTables(TextWriter s)
		{
			if (_doNotBreakWrappedTables != null)
			{
				_doNotBreakWrappedTables.Write(s, "doNotBreakWrappedTables");
			}
		}

		public void Write_doNotSnapToGridInCell(TextWriter s)
		{
			if (_doNotSnapToGridInCell != null)
			{
				_doNotSnapToGridInCell.Write(s, "doNotSnapToGridInCell");
			}
		}

		public void Write_selectFldWithFirstOrLastChar(TextWriter s)
		{
			if (_selectFldWithFirstOrLastChar != null)
			{
				_selectFldWithFirstOrLastChar.Write(s, "selectFldWithFirstOrLastChar");
			}
		}

		public void Write_applyBreakingRules(TextWriter s)
		{
			if (_applyBreakingRules != null)
			{
				_applyBreakingRules.Write(s, "applyBreakingRules");
			}
		}

		public void Write_doNotWrapTextWithPunct(TextWriter s)
		{
			if (_doNotWrapTextWithPunct != null)
			{
				_doNotWrapTextWithPunct.Write(s, "doNotWrapTextWithPunct");
			}
		}

		public void Write_doNotUseEastAsianBreakRules(TextWriter s)
		{
			if (_doNotUseEastAsianBreakRules != null)
			{
				_doNotUseEastAsianBreakRules.Write(s, "doNotUseEastAsianBreakRules");
			}
		}

		public void Write_useWord2002TableStyleRules(TextWriter s)
		{
			if (_useWord2002TableStyleRules != null)
			{
				_useWord2002TableStyleRules.Write(s, "useWord2002TableStyleRules");
			}
		}

		public void Write_growAutofit(TextWriter s)
		{
			if (_growAutofit != null)
			{
				_growAutofit.Write(s, "growAutofit");
			}
		}

		public void Write_useFELayout(TextWriter s)
		{
			if (_useFELayout != null)
			{
				_useFELayout.Write(s, "useFELayout");
			}
		}

		public void Write_useNormalStyleForList(TextWriter s)
		{
			if (_useNormalStyleForList != null)
			{
				_useNormalStyleForList.Write(s, "useNormalStyleForList");
			}
		}

		public void Write_doNotUseIndentAsNumberingTabStop(TextWriter s)
		{
			if (_doNotUseIndentAsNumberingTabStop != null)
			{
				_doNotUseIndentAsNumberingTabStop.Write(s, "doNotUseIndentAsNumberingTabStop");
			}
		}

		public void Write_useAltKinsokuLineBreakRules(TextWriter s)
		{
			if (_useAltKinsokuLineBreakRules != null)
			{
				_useAltKinsokuLineBreakRules.Write(s, "useAltKinsokuLineBreakRules");
			}
		}

		public void Write_allowSpaceOfSameStyleInTable(TextWriter s)
		{
			if (_allowSpaceOfSameStyleInTable != null)
			{
				_allowSpaceOfSameStyleInTable.Write(s, "allowSpaceOfSameStyleInTable");
			}
		}

		public void Write_doNotSuppressIndentation(TextWriter s)
		{
			if (_doNotSuppressIndentation != null)
			{
				_doNotSuppressIndentation.Write(s, "doNotSuppressIndentation");
			}
		}

		public void Write_doNotAutofitConstrainedTables(TextWriter s)
		{
			if (_doNotAutofitConstrainedTables != null)
			{
				_doNotAutofitConstrainedTables.Write(s, "doNotAutofitConstrainedTables");
			}
		}

		public void Write_autofitToFirstFixedWidthCell(TextWriter s)
		{
			if (_autofitToFirstFixedWidthCell != null)
			{
				_autofitToFirstFixedWidthCell.Write(s, "autofitToFirstFixedWidthCell");
			}
		}

		public void Write_underlineTabInNumList(TextWriter s)
		{
			if (_underlineTabInNumList != null)
			{
				_underlineTabInNumList.Write(s, "underlineTabInNumList");
			}
		}

		public void Write_displayHangulFixedWidth(TextWriter s)
		{
			if (_displayHangulFixedWidth != null)
			{
				_displayHangulFixedWidth.Write(s, "displayHangulFixedWidth");
			}
		}

		public void Write_splitPgBreakAndParaMark(TextWriter s)
		{
			if (_splitPgBreakAndParaMark != null)
			{
				_splitPgBreakAndParaMark.Write(s, "splitPgBreakAndParaMark");
			}
		}

		public void Write_doNotVertAlignCellWithSp(TextWriter s)
		{
			if (_doNotVertAlignCellWithSp != null)
			{
				_doNotVertAlignCellWithSp.Write(s, "doNotVertAlignCellWithSp");
			}
		}

		public void Write_doNotBreakConstrainedForcedTable(TextWriter s)
		{
			if (_doNotBreakConstrainedForcedTable != null)
			{
				_doNotBreakConstrainedForcedTable.Write(s, "doNotBreakConstrainedForcedTable");
			}
		}

		public void Write_doNotVertAlignInTxbx(TextWriter s)
		{
			if (_doNotVertAlignInTxbx != null)
			{
				_doNotVertAlignInTxbx.Write(s, "doNotVertAlignInTxbx");
			}
		}

		public void Write_useAnsiKerningPairs(TextWriter s)
		{
			if (_useAnsiKerningPairs != null)
			{
				_useAnsiKerningPairs.Write(s, "useAnsiKerningPairs");
			}
		}

		public void Write_cachedColBalance(TextWriter s)
		{
			if (_cachedColBalance != null)
			{
				_cachedColBalance.Write(s, "cachedColBalance");
			}
		}
	}
}
