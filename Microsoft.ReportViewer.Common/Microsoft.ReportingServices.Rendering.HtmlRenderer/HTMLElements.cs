using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal static class HTMLElements
	{
		internal static string m_standardLineBreak;

		internal static string m_quoteString;

		internal static string m_spaceString;

		internal static string m_closeQuoteString;

		internal static string m_blank;

		internal static string m_hiddenString;

		internal static string m_containString;

		internal static string m_reportItemCustomAttrStr;

		internal static string m_reportItemDataName;

		internal static string m_resize100WidthClassName;

		internal static string m_resize100HeightClassName;

		internal static string m_mapPrefixString;

		internal static string m_hrefString;

		internal static string m_flexStart;

		internal static string m_msFlexStart;

		internal static string m_flexCenter;

		internal static string m_flexEnd;

		internal static string m_msFlexEnd;

		internal static string m_hashTag;

		internal static string m_role;

		internal static string m_navigationRole;

		internal static string m_presentationRole;

		internal static string m_buttonRole;

		internal static string m_ariaLabel;

		internal static string m_ariaLabeledBy;

		internal static string m_ariaExpanded;

		internal static string m_ariaSuffix;

		internal static string m_ariaLive;

		internal static string m_ariaLivePoliteString;

		internal static string m_ariaHidden;

		internal static string m_useMapName;

		internal static string Ie5ContentType;

		internal static string Ie7ContentType;

		internal static string EdgeContentType;

		internal static string Html40DocType;

		internal static string XhtmlStrictDocType;

		internal static string Utf8Charset;

		internal static string HtmlStandardsDocType;

		internal static string SegoeUi;

		internal static byte[] m_overflowXHidden;

		internal static byte[] m_percentWidthOverflow;

		internal static byte[] m_layoutFixed;

		internal static byte[] m_layoutBorder;

		internal static byte[] m_ignoreBorder;

		internal static byte[] m_ignoreBorderL;

		internal static byte[] m_ignoreBorderR;

		internal static byte[] m_ignoreBorderT;

		internal static byte[] m_ignoreBorderB;

		internal static byte[] m_percentHeight;

		internal static byte[] m_percentSizesOverflow;

		internal static byte[] m_percentSizes;

		internal static byte[] m_border;

		internal static byte[] m_borderBottom;

		internal static byte[] m_borderLeft;

		internal static byte[] m_borderRight;

		internal static byte[] m_borderTop;

		internal static byte[] m_boxSizingBorderBox;

		internal static byte[] m_marginBottom;

		internal static byte[] m_marginLeft;

		internal static byte[] m_marginRight;

		internal static byte[] m_marginTop;

		internal static byte[] m_textIndent;

		internal static byte[] m_percent;

		internal static byte[] m_ninetyninepercent;

		internal static byte[] m_degree90;

		internal static byte[] m_newLine;

		internal static byte[] m_closeAccol;

		internal static byte[] m_closeParenthesis;

		internal static byte[] m_backgroundRepeat;

		internal static byte[] m_backgroundSize;

		internal static byte[] m_closeBrace;

		internal static byte[] m_backgroundColor;

		internal static byte[] m_backgroundImage;

		internal static byte[] m_overflowHidden;

		internal static byte[] m_wordWrap;

		internal static byte[] m_wordBreak;

		internal static byte[] m_wordWrapNormal;

		internal static byte[] m_wordBreakAll;

		internal static byte[] m_whiteSpacePreWrap;

		internal static byte[] m_overflow;

		internal static byte[] m_leftValue;

		internal static byte[] m_rightValue;

		internal static byte[] m_centerValue;

		internal static byte[] m_textAlign;

		internal static byte[] m_verticalAlign;

		internal static byte[] m_flexAlignItems;

		internal static byte[] m_msFlexAlignItems;

		internal static byte[] m_webkitFlexAlignItems;

		internal static byte[] m_flexJustifyContent;

		internal static byte[] m_msFlexJustifyContent;

		internal static byte[] m_webkitFlexJustifyContent;

		internal static byte[] m_flexFlowRow;

		internal static byte[] m_flexFlowRowReverse;

		internal static byte[] m_lineHeight;

		internal static byte[] m_color;

		internal static byte[] m_writingMode;

		internal static byte[] m_tbrl;

		internal static byte[] m_btrl;

		internal static byte[] m_lrtb;

		internal static byte[] m_rltb;

		internal static byte[] m_rotate180deg;

		internal static byte[] m_webkit_vertical;

		internal static byte[] m_ms_vertical;

		internal static byte[] m_ms_verticalRTL;

		internal static byte[] m_ff_vertical;

		internal static byte[] m_layoutFlow;

		internal static byte[] m_verticalIdeographic;

		internal static byte[] m_horizontal;

		internal static byte[] m_unicodeBiDi;

		internal static byte[] m_direction;

		internal static byte[] m_textDecoration;

		internal static byte[] m_fontWeight;

		internal static byte[] m_fontSize;

		internal static byte[] m_fontFamily;

		internal static byte[] m_fontStyle;

		internal static byte[] m_openAccol;

		internal static byte[] m_borderColor;

		internal static byte[] m_borderStyle;

		internal static byte[] m_borderWidth;

		internal static byte[] m_borderBottomColor;

		internal static byte[] m_borderBottomStyle;

		internal static byte[] m_borderBottomWidth;

		internal static byte[] m_borderLeftColor;

		internal static byte[] m_borderLeftStyle;

		internal static byte[] m_borderLeftWidth;

		internal static byte[] m_borderRightColor;

		internal static byte[] m_borderRightStyle;

		internal static byte[] m_borderRightWidth;

		internal static byte[] m_borderTopColor;

		internal static byte[] m_borderTopStyle;

		internal static byte[] m_borderTopWidth;

		internal static byte[] m_paddingBottom;

		internal static byte[] m_paddingLeft;

		internal static byte[] m_paddingRight;

		internal static byte[] m_paddingTop;

		internal static byte[] m_classAction;

		internal static byte[] m_styleAction;

		internal static byte[] m_emptyTextBox;

		internal static byte[] m_percentSizeInlineTable;

		internal static byte[] m_classPercentSizeInlineTable;

		internal static byte[] m_percentHeightInlineTable;

		internal static byte[] m_classPercentHeightInlineTable;

		internal static byte[] m_dot;

		internal static byte[] m_popupAction;

		internal static byte[] m_tableLayoutFixed;

		internal static byte[] m_none;

		internal static byte[] m_rtlEmbed;

		internal static byte[] m_classRtlEmbed;

		internal static byte[] m_noVerticalMarginClassName;

		internal static byte[] m_classNoVerticalMargin;

		internal static byte[] m_zeroPoint;

		internal static byte[] m_smallPoint;

		internal static byte[] m_filter;

		internal static byte[] m_basicImageRotation180;

		internal static byte[] m_msoRotation;

		internal static byte[] m_opacity;

		internal static byte[] m_reportItemCustomAttr;

		internal static byte[] m_noRepeat;

		internal static byte[] m_openInlineStyle;

		internal static byte[] m_closeInlineStyle;

		internal static byte[] m_openInlineJavaScript;

		internal static byte[] m_closeInlineJavaScript;

		internal static byte[] m_closeUL;

		internal static byte[] m_closeOL;

		internal static byte[] m_olArabic;

		internal static byte[] m_olRoman;

		internal static byte[] m_olAlpha;

		internal static byte[] m_ulCircle;

		internal static byte[] m_ulDisc;

		internal static byte[] m_ulSquare;

		internal static byte[] m_nohref;

		internal static byte[] m_img;

		internal static byte[] m_imgOnError;

		internal static byte[] m_src;

		internal static byte[] m_classID;

		internal static byte[] m_codeBase;

		internal static byte[] m_valueObject;

		internal static byte[] m_paramObject;

		internal static byte[] m_openObject;

		internal static byte[] m_closeObject;

		internal static byte[] m_equal;

		internal static byte[] m_encodedAmp;

		internal static byte[] m_questionMark;

		internal static byte[] m_checked;

		internal static byte[] m_unchecked;

		internal static byte[] m_showHideOnClick;

		internal static byte[] m_rtlDir;

		internal static byte[] m_ltrDir;

		internal static byte[] m_classStyle;

		internal static byte[] m_underscore;

		internal static byte[] m_lineBreak;

		internal static byte[] m_ssClassID;

		internal static byte[] m_ptClassID;

		internal static byte[] m_xmlData;

		internal static byte[] m_useMap;

		internal static byte[] m_openMap;

		internal static byte[] m_closeMap;

		internal static byte[] m_mapArea;

		internal static byte[] m_mapCoords;

		internal static byte[] m_mapShape;

		internal static byte[] m_name;

		internal static byte[] m_dataName;

		internal static byte[] m_circleShape;

		internal static byte[] m_polyShape;

		internal static byte[] m_rectShape;

		internal static byte[] m_comma;

		internal static byte[] m_mapPrefix;

		internal static byte[] m_classPopupAction;

		internal static byte[] m_closeLi;

		internal static byte[] m_openLi;

		internal static byte[] m_firstNonHeaderPostfix;

		internal static byte[] m_fixedMatrixCornerPostfix;

		internal static byte[] m_fixedRowGroupingHeaderPostfix;

		internal static byte[] m_fixedColumnGroupingHeaderPostfix;

		internal static byte[] m_fixedRowHeaderPostfix;

		internal static byte[] m_fixedColumnHeaderPostfix;

		internal static byte[] m_fixedTableCornerPostfix;

		internal static byte[] m_language;

		internal static byte[] m_zeroBorderWidth;

		internal static byte[] m_onLoadFitProportionalPv;

		internal static byte[] m_percentSizeString;

		internal static byte[] m_classPercentSizes;

		internal static byte[] m_classPercentSizesOverflow;

		internal static byte[] m_classPercentHeight;

		internal static byte[] m_classLayoutBorder;

		internal static byte[] m_classCanGrowVerticalTextBox;

		internal static byte[] m_classCanShrinkVerticalTextBox;

		internal static byte[] m_classCanGrowBothTextBox;

		internal static byte[] m_classCannotGrowTextBoxInTablix;

		internal static byte[] m_classCannotShrinkTextBoxInTablix;

		internal static byte[] m_classCanGrowTextBoxInTablix;

		internal static byte[] m_classCanShrinkTextBoxInTablix;

		internal static byte[] m_strokeColor;

		internal static byte[] m_strokeWeight;

		internal static byte[] m_slineStyle;

		internal static byte[] m_dashStyle;

		internal static byte[] m_closeVGroup;

		internal static byte[] m_openVGroup;

		internal static byte[] m_openVLine;

		internal static byte[] m_leftSlant;

		internal static byte[] m_rightSlant;

		internal static byte[] m_pageBreakDelimiter;

		internal static byte[] m_stylePositionAbsolute;

		internal static byte[] m_stylePositionRelative;

		internal static byte[] m_styleTop;

		internal static byte[] m_styleLeft;

		internal static byte[] m_openTable;

		internal static byte[] m_cols;

		internal static byte[] m_quote;

		internal static byte[] m_zeroBorder;

		internal static byte[] m_styleWidth;

		internal static byte[] m_styleHeight;

		internal static byte[] m_mm;

		internal static byte[] m_topValue;

		internal static byte[] m_displayNone;

		internal static byte[] m_styleDisplayInlineBlock;

		internal static byte[] m_styleDisplayFlex;

		internal static byte[] m_styleDisplayTableCell;

		internal static byte[] m_nbsp;

		internal static byte[] m_id;

		internal static byte[] m_alt;

		internal static byte[] m_aria;

		internal static byte[] m_title;

		internal static byte[] m_openStyle;

		internal static byte[] m_styleMinWidth;

		internal static byte[] m_styleMaxWidth;

		internal static byte[] m_styleMinHeight;

		internal static byte[] m_styleMaxHeight;

		internal static byte[] m_semiColon;

		internal static byte[] m_borderCollapse;

		internal static byte[] m_closeBracket;

		internal static byte[] m_openTR;

		internal static byte[] m_br;

		internal static byte[] m_tabIndex;

		internal static byte[] m_closeTable;

		internal static byte[] m_closeDiv;

		internal static byte[] m_openDiv;

		internal static byte[] m_colSpan;

		internal static byte[] m_rowSpan;

		internal static byte[] m_headers;

		internal static byte[] m_closeTD;

		internal static byte[] m_closeTR;

		internal static byte[] m_firstTD;

		internal static byte[] m_lastTD;

		internal static byte[] m_openTD;

		internal static byte[] m_valign;

		internal static byte[] m_closeQuote;

		internal static byte[] m_closeSingleTag;

		internal static byte[] m_closeSpan;

		internal static byte[] m_openSpan;

		internal static byte[] m_closeTag;

		internal static byte[] m_px;

		internal static byte[] m_zeroWidth;

		internal static byte[] m_zeroHeight;

		internal static byte[] m_openHtml;

		internal static byte[] m_closeHtml;

		internal static byte[] m_openBody;

		internal static byte[] m_closeBody;

		internal static byte[] m_openHead;

		internal static byte[] m_closeHead;

		internal static byte[] m_openTitle;

		internal static byte[] m_closeTitle;

		internal static byte[] m_cursorHand;

		internal static byte[] m_openA;

		internal static byte[] m_closeA;

		internal static byte[] m_inlineHeight;

		internal static byte[] m_inlineWidth;

		internal static byte[] m_space;

		internal static byte[] m_href;

		internal static byte[] m_target;

		internal static byte[] m_checkForEnterKey;

		internal static byte[] m_defaultPixelSize;

		internal static byte[] m_auto;

		static HTMLElements()
		{
			m_standardLineBreak = "\n";
			m_quoteString = "\"";
			m_spaceString = " ";
			m_closeQuoteString = "\">";
			m_blank = "_blank";
			m_hiddenString = "hidden";
			m_containString = "contain";
			m_reportItemCustomAttrStr = "data-reportitem";
			m_reportItemDataName = "data-name";
			m_resize100WidthClassName = "resize100Width";
			m_resize100HeightClassName = "resize100Height";
			m_mapPrefixString = "Map";
			m_hrefString = " href=\"";
			m_flexStart = "flex-start;";
			m_msFlexStart = "start;";
			m_flexCenter = "center;";
			m_flexEnd = "flex-end;";
			m_msFlexEnd = "end;";
			m_hashTag = "#";
			m_role = "role";
			m_navigationRole = "navigation";
			m_presentationRole = "presentation";
			m_buttonRole = "button";
			m_ariaLabel = "aria-label";
			m_ariaLabeledBy = " aria-labelledby=\"";
			m_ariaExpanded = " aria-expanded=\"";
			m_ariaSuffix = "_aria";
			m_ariaLive = "aria-live";
			m_ariaLivePoliteString = "polite";
			m_ariaHidden = "aria-hidden=\"true\"";
			m_useMapName = "USEMAP";
			Ie5ContentType = "\n\t<META HTTP-EQUIV=\"X-UA-Compatible\" CONTENT=\"IE=5\">\n";
			Ie7ContentType = "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=7\" />";
			EdgeContentType = "\n\t<META HTTP-EQUIV=\"X-UA-Compatible\" CONTENT=\"IE=edge\">\n";
			Html40DocType = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >\n";
			XhtmlStrictDocType = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";
			Utf8Charset = "\n\t<meta charset=\"utf-8\">\n";
			HtmlStandardsDocType = "<!DOCTYPE html>\n";
			SegoeUi = "Segoe UI";
			m_overflowXHidden = null;
			m_percentWidthOverflow = null;
			m_layoutFixed = null;
			m_layoutBorder = null;
			m_ignoreBorder = null;
			m_ignoreBorderL = null;
			m_ignoreBorderR = null;
			m_ignoreBorderT = null;
			m_ignoreBorderB = null;
			m_percentHeight = null;
			m_percentSizesOverflow = null;
			m_percentSizes = null;
			m_border = null;
			m_borderBottom = null;
			m_borderLeft = null;
			m_borderRight = null;
			m_borderTop = null;
			m_boxSizingBorderBox = null;
			m_marginBottom = null;
			m_marginLeft = null;
			m_marginRight = null;
			m_marginTop = null;
			m_textIndent = null;
			m_percent = null;
			m_ninetyninepercent = null;
			m_degree90 = null;
			m_newLine = null;
			m_closeAccol = null;
			m_closeParenthesis = null;
			m_backgroundRepeat = null;
			m_backgroundSize = null;
			m_closeBrace = null;
			m_backgroundColor = null;
			m_backgroundImage = null;
			m_overflowHidden = null;
			m_wordWrap = null;
			m_wordBreak = null;
			m_wordWrapNormal = null;
			m_wordBreakAll = null;
			m_whiteSpacePreWrap = null;
			m_overflow = null;
			m_leftValue = null;
			m_rightValue = null;
			m_centerValue = null;
			m_textAlign = null;
			m_verticalAlign = null;
			m_flexAlignItems = null;
			m_msFlexAlignItems = null;
			m_webkitFlexAlignItems = null;
			m_flexJustifyContent = null;
			m_msFlexJustifyContent = null;
			m_webkitFlexJustifyContent = null;
			m_flexFlowRow = null;
			m_flexFlowRowReverse = null;
			m_lineHeight = null;
			m_color = null;
			m_writingMode = null;
			m_tbrl = null;
			m_btrl = null;
			m_lrtb = null;
			m_rltb = null;
			m_rotate180deg = null;
			m_webkit_vertical = null;
			m_ms_vertical = null;
			m_ms_verticalRTL = null;
			m_ff_vertical = null;
			m_layoutFlow = null;
			m_verticalIdeographic = null;
			m_horizontal = null;
			m_unicodeBiDi = null;
			m_direction = null;
			m_textDecoration = null;
			m_fontWeight = null;
			m_fontSize = null;
			m_fontFamily = null;
			m_fontStyle = null;
			m_openAccol = null;
			m_borderColor = null;
			m_borderStyle = null;
			m_borderWidth = null;
			m_borderBottomColor = null;
			m_borderBottomStyle = null;
			m_borderBottomWidth = null;
			m_borderLeftColor = null;
			m_borderLeftStyle = null;
			m_borderLeftWidth = null;
			m_borderRightColor = null;
			m_borderRightStyle = null;
			m_borderRightWidth = null;
			m_borderTopColor = null;
			m_borderTopStyle = null;
			m_borderTopWidth = null;
			m_paddingBottom = null;
			m_paddingLeft = null;
			m_paddingRight = null;
			m_paddingTop = null;
			m_classAction = null;
			m_styleAction = null;
			m_emptyTextBox = null;
			m_percentSizeInlineTable = null;
			m_classPercentSizeInlineTable = null;
			m_percentHeightInlineTable = null;
			m_classPercentHeightInlineTable = null;
			m_dot = null;
			m_popupAction = null;
			m_tableLayoutFixed = null;
			m_none = null;
			m_rtlEmbed = null;
			m_classRtlEmbed = null;
			m_noVerticalMarginClassName = null;
			m_classNoVerticalMargin = null;
			m_zeroPoint = null;
			m_smallPoint = null;
			m_filter = null;
			m_basicImageRotation180 = null;
			m_msoRotation = null;
			m_opacity = null;
			m_reportItemCustomAttr = null;
			m_noRepeat = null;
			m_openInlineStyle = null;
			m_closeInlineStyle = null;
			m_openInlineJavaScript = null;
			m_closeInlineJavaScript = null;
			m_nohref = null;
			m_img = null;
			m_imgOnError = null;
			m_src = null;
			m_classID = null;
			m_codeBase = null;
			m_valueObject = null;
			m_paramObject = null;
			m_openObject = null;
			m_closeObject = null;
			m_equal = null;
			m_encodedAmp = null;
			m_questionMark = null;
			m_checked = null;
			m_unchecked = null;
			m_showHideOnClick = null;
			m_rtlDir = null;
			m_ltrDir = null;
			m_classStyle = null;
			m_underscore = null;
			m_lineBreak = null;
			m_ssClassID = null;
			m_ptClassID = null;
			m_xmlData = null;
			m_useMap = null;
			m_openMap = null;
			m_closeMap = null;
			m_mapArea = null;
			m_mapCoords = null;
			m_mapShape = null;
			m_name = null;
			m_dataName = null;
			m_circleShape = null;
			m_polyShape = null;
			m_rectShape = null;
			m_comma = null;
			m_mapPrefix = null;
			m_classPopupAction = null;
			m_closeLi = null;
			m_openLi = null;
			m_firstNonHeaderPostfix = null;
			m_fixedMatrixCornerPostfix = null;
			m_fixedRowGroupingHeaderPostfix = null;
			m_fixedColumnGroupingHeaderPostfix = null;
			m_fixedRowHeaderPostfix = null;
			m_fixedColumnHeaderPostfix = null;
			m_fixedTableCornerPostfix = null;
			m_language = null;
			m_zeroBorderWidth = null;
			m_onLoadFitProportionalPv = null;
			m_percentSizeString = null;
			m_classPercentSizes = null;
			m_classPercentSizesOverflow = null;
			m_classPercentHeight = null;
			m_classLayoutBorder = null;
			m_classCanGrowVerticalTextBox = null;
			m_classCanShrinkVerticalTextBox = null;
			m_classCanGrowBothTextBox = null;
			m_classCannotGrowTextBoxInTablix = null;
			m_classCannotShrinkTextBoxInTablix = null;
			m_classCanGrowTextBoxInTablix = null;
			m_classCanShrinkTextBoxInTablix = null;
			m_strokeColor = null;
			m_strokeWeight = null;
			m_slineStyle = null;
			m_dashStyle = null;
			m_closeVGroup = null;
			m_openVGroup = null;
			m_openVLine = null;
			m_leftSlant = null;
			m_rightSlant = null;
			m_pageBreakDelimiter = null;
			m_stylePositionAbsolute = null;
			m_stylePositionRelative = null;
			m_styleTop = null;
			m_styleLeft = null;
			m_openTable = null;
			m_cols = null;
			m_quote = null;
			m_zeroBorder = null;
			m_styleWidth = null;
			m_styleHeight = null;
			m_mm = null;
			m_topValue = null;
			m_displayNone = null;
			m_styleDisplayInlineBlock = null;
			m_styleDisplayFlex = null;
			m_styleDisplayTableCell = null;
			m_nbsp = null;
			m_id = null;
			m_alt = null;
			m_aria = null;
			m_title = null;
			m_openStyle = null;
			m_styleMinWidth = null;
			m_styleMaxWidth = null;
			m_styleMinHeight = null;
			m_styleMaxHeight = null;
			m_semiColon = null;
			m_borderCollapse = null;
			m_closeBracket = null;
			m_openTR = null;
			m_br = null;
			m_tabIndex = null;
			m_closeTable = null;
			m_closeDiv = null;
			m_openDiv = null;
			m_colSpan = null;
			m_rowSpan = null;
			m_headers = null;
			m_closeTD = null;
			m_closeTR = null;
			m_firstTD = null;
			m_lastTD = null;
			m_openTD = null;
			m_valign = null;
			m_closeQuote = null;
			m_closeSingleTag = null;
			m_closeSpan = null;
			m_openSpan = null;
			m_closeTag = null;
			m_px = null;
			m_zeroWidth = null;
			m_zeroHeight = null;
			m_openHtml = null;
			m_closeHtml = null;
			m_openBody = null;
			m_closeBody = null;
			m_openHead = null;
			m_closeHead = null;
			m_openTitle = null;
			m_closeTitle = null;
			m_cursorHand = null;
			m_openA = null;
			m_closeA = null;
			m_inlineHeight = null;
			m_inlineWidth = null;
			m_space = null;
			m_href = null;
			m_target = null;
			m_checkForEnterKey = null;
			m_defaultPixelSize = null;
			m_auto = null;
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			m_newLine = uTF8Encoding.GetBytes("\r\n");
			m_openTable = uTF8Encoding.GetBytes("<TABLE CELLSPACING=\"0\" CELLPADDING=\"0\"");
			m_zeroBorder = uTF8Encoding.GetBytes(" BORDER=\"0\"");
			m_zeroPoint = uTF8Encoding.GetBytes("0pt");
			m_smallPoint = uTF8Encoding.GetBytes("1px");
			m_cols = uTF8Encoding.GetBytes(" COLS=\"");
			m_colSpan = uTF8Encoding.GetBytes(" COLSPAN=\"");
			m_rowSpan = uTF8Encoding.GetBytes(" ROWSPAN=\"");
			m_headers = uTF8Encoding.GetBytes(" HEADERS=\"");
			m_space = uTF8Encoding.GetBytes(m_spaceString);
			m_closeBracket = uTF8Encoding.GetBytes(">");
			m_closeTable = uTF8Encoding.GetBytes("</TABLE>");
			m_openDiv = uTF8Encoding.GetBytes("<div");
			m_closeDiv = uTF8Encoding.GetBytes("</div>");
			m_openBody = uTF8Encoding.GetBytes("<body");
			m_closeBody = uTF8Encoding.GetBytes("</body>");
			m_openHtml = uTF8Encoding.GetBytes("<html>");
			m_closeHtml = uTF8Encoding.GetBytes("</html>");
			m_openHead = uTF8Encoding.GetBytes("<head>");
			m_closeHead = uTF8Encoding.GetBytes("</head>");
			m_openTitle = uTF8Encoding.GetBytes("<title>");
			m_closeTitle = uTF8Encoding.GetBytes("</title>");
			m_firstTD = uTF8Encoding.GetBytes("<TR><TD");
			m_lastTD = uTF8Encoding.GetBytes("</TD></TR>");
			m_openTD = uTF8Encoding.GetBytes("<TD");
			m_closeTD = uTF8Encoding.GetBytes("</TD>");
			m_closeTR = uTF8Encoding.GetBytes("</TR>");
			m_openTR = uTF8Encoding.GetBytes("<TR");
			m_valign = uTF8Encoding.GetBytes(" VALIGN=\"");
			m_openSpan = uTF8Encoding.GetBytes("<span");
			m_closeSpan = uTF8Encoding.GetBytes("</span>");
			m_quote = uTF8Encoding.GetBytes(m_quoteString);
			m_closeQuote = uTF8Encoding.GetBytes(m_closeQuoteString);
			m_closeSingleTag = uTF8Encoding.GetBytes("/>");
			m_id = uTF8Encoding.GetBytes(" ID=\"");
			m_mm = uTF8Encoding.GetBytes("mm");
			m_px = uTF8Encoding.GetBytes("px");
			m_zeroWidth = uTF8Encoding.GetBytes(" WIDTH=\"0\"");
			m_zeroHeight = uTF8Encoding.GetBytes(" HEIGHT=\"0\"");
			m_closeTag = uTF8Encoding.GetBytes("\"/>");
			m_openA = uTF8Encoding.GetBytes("<a");
			m_target = uTF8Encoding.GetBytes(" TARGET=\"");
			m_closeA = uTF8Encoding.GetBytes("</a>");
			m_href = uTF8Encoding.GetBytes(m_hrefString);
			m_nohref = uTF8Encoding.GetBytes(" nohref=\"true\"");
			m_inlineHeight = uTF8Encoding.GetBytes(" HEIGHT=\"");
			m_inlineWidth = uTF8Encoding.GetBytes(" WIDTH=\"");
			m_img = uTF8Encoding.GetBytes("<img");
			m_imgOnError = uTF8Encoding.GetBytes(" onerror=\"this.errored=true;\"");
			m_src = uTF8Encoding.GetBytes(" src=\"");
			m_topValue = uTF8Encoding.GetBytes("top");
			m_leftValue = uTF8Encoding.GetBytes("left");
			m_rightValue = uTF8Encoding.GetBytes("right");
			m_centerValue = uTF8Encoding.GetBytes("center");
			m_classID = uTF8Encoding.GetBytes(" CLASSID=\"CLSID:");
			m_codeBase = uTF8Encoding.GetBytes(" CODEBASE=\"");
			m_title = uTF8Encoding.GetBytes(" TITLE=\"");
			m_alt = uTF8Encoding.GetBytes(" ALT=\"");
			m_aria = uTF8Encoding.GetBytes(" " + m_ariaLabel + "=\"");
			m_openObject = uTF8Encoding.GetBytes("<OBJECT");
			m_closeObject = uTF8Encoding.GetBytes("</OBJECT>");
			m_paramObject = uTF8Encoding.GetBytes("<PARAM NAME=\"");
			m_valueObject = uTF8Encoding.GetBytes(" VALUE=\"");
			m_equal = uTF8Encoding.GetBytes("=");
			m_encodedAmp = uTF8Encoding.GetBytes("&amp;");
			m_nbsp = uTF8Encoding.GetBytes("&nbsp;");
			m_questionMark = uTF8Encoding.GetBytes("?");
			m_none = uTF8Encoding.GetBytes("none");
			m_displayNone = uTF8Encoding.GetBytes("display: none;");
			m_styleDisplayInlineBlock = uTF8Encoding.GetBytes("display: inline-block;");
			m_styleDisplayFlex = uTF8Encoding.GetBytes("display: -ms-flexbox;display: -webkit-flex;display: flex;");
			m_styleDisplayTableCell = uTF8Encoding.GetBytes("display: -ms-table-cell;display: -webkit-table-cell;display: table-cell;");
			m_checkForEnterKey = uTF8Encoding.GetBytes("if(event.keyCode == 13 || event.which == 13){");
			m_percent = uTF8Encoding.GetBytes("100%");
			m_ninetyninepercent = uTF8Encoding.GetBytes("99%");
			m_degree90 = uTF8Encoding.GetBytes("90");
			m_lineBreak = uTF8Encoding.GetBytes(m_standardLineBreak);
			m_closeBrace = uTF8Encoding.GetBytes(")");
			m_rtlDir = uTF8Encoding.GetBytes(" dir=\"RTL\"");
			m_ltrDir = uTF8Encoding.GetBytes(" dir=\"LTR\"");
			m_br = uTF8Encoding.GetBytes("<br/>");
			m_tabIndex = uTF8Encoding.GetBytes(" tabindex=\"");
			m_useMap = uTF8Encoding.GetBytes(" USEMAP=\"");
			m_openMap = uTF8Encoding.GetBytes("<MAP ");
			m_closeMap = uTF8Encoding.GetBytes("</MAP>");
			m_mapArea = uTF8Encoding.GetBytes("<AREA ");
			m_mapCoords = uTF8Encoding.GetBytes(" COORDS=\"");
			m_mapShape = uTF8Encoding.GetBytes(" SHAPE=\"");
			m_name = uTF8Encoding.GetBytes(" NAME=\"");
			m_dataName = uTF8Encoding.GetBytes(" data-name=\"");
			m_circleShape = uTF8Encoding.GetBytes("circle");
			m_polyShape = uTF8Encoding.GetBytes("poly");
			m_rectShape = uTF8Encoding.GetBytes("rect");
			m_comma = uTF8Encoding.GetBytes(",");
			m_mapPrefix = uTF8Encoding.GetBytes(m_mapPrefixString);
			m_openLi = uTF8Encoding.GetBytes("<li");
			m_closeLi = uTF8Encoding.GetBytes("</li>");
			m_firstNonHeaderPostfix = uTF8Encoding.GetBytes("_FNHR");
			m_fixedMatrixCornerPostfix = uTF8Encoding.GetBytes("_MCC");
			m_fixedRowGroupingHeaderPostfix = uTF8Encoding.GetBytes("_FRGH");
			m_fixedColumnGroupingHeaderPostfix = uTF8Encoding.GetBytes("_FCGH");
			m_fixedRowHeaderPostfix = uTF8Encoding.GetBytes("_FRH");
			m_fixedColumnHeaderPostfix = uTF8Encoding.GetBytes("_FCH");
			m_fixedTableCornerPostfix = uTF8Encoding.GetBytes("_FCC");
			m_dot = uTF8Encoding.GetBytes(".");
			m_percentSizes = uTF8Encoding.GetBytes("r1");
			m_percentSizesOverflow = uTF8Encoding.GetBytes("r2");
			m_percentHeight = uTF8Encoding.GetBytes("r3");
			m_ignoreBorder = uTF8Encoding.GetBytes("r4");
			m_ignoreBorderL = uTF8Encoding.GetBytes("r5");
			m_ignoreBorderR = uTF8Encoding.GetBytes("r6");
			m_ignoreBorderT = uTF8Encoding.GetBytes("r7");
			m_ignoreBorderB = uTF8Encoding.GetBytes("r8");
			m_layoutFixed = uTF8Encoding.GetBytes("r9");
			m_layoutBorder = uTF8Encoding.GetBytes("r10");
			m_percentWidthOverflow = uTF8Encoding.GetBytes("r11");
			m_popupAction = uTF8Encoding.GetBytes("r12");
			m_styleAction = uTF8Encoding.GetBytes("r13");
			m_emptyTextBox = uTF8Encoding.GetBytes("r14");
			m_classPercentSizes = uTF8Encoding.GetBytes(" class=\"r1\"");
			m_classPercentSizesOverflow = uTF8Encoding.GetBytes(" class=\"r2\"");
			m_classPercentHeight = uTF8Encoding.GetBytes(" class=\"r3\"");
			m_classLayoutBorder = uTF8Encoding.GetBytes(" class=\"r10");
			m_classPopupAction = uTF8Encoding.GetBytes(" class=\"r12\"");
			m_classAction = uTF8Encoding.GetBytes(" class=\"r13\"");
			m_rtlEmbed = uTF8Encoding.GetBytes("r15");
			m_classRtlEmbed = uTF8Encoding.GetBytes(" class=\"r15\"");
			m_noVerticalMarginClassName = uTF8Encoding.GetBytes("r16");
			m_classNoVerticalMargin = uTF8Encoding.GetBytes(" class=\"r16\"");
			m_percentSizeInlineTable = uTF8Encoding.GetBytes("r17");
			m_classPercentSizeInlineTable = uTF8Encoding.GetBytes(" class=\"r17\"");
			m_percentHeightInlineTable = uTF8Encoding.GetBytes("r18");
			m_classPercentHeightInlineTable = uTF8Encoding.GetBytes(" class=\"r18\"");
			m_classCanGrowVerticalTextBox = uTF8Encoding.GetBytes(" class=\"canGrowVerticalTextBox\"");
			m_classCanShrinkVerticalTextBox = uTF8Encoding.GetBytes(" class=\"canShrinkVerticalTextBox\"");
			m_classCanGrowBothTextBox = uTF8Encoding.GetBytes(" class=\"canGrowVerticalTextBox canShrinkVerticalTextBox\"");
			m_classCannotGrowTextBoxInTablix = uTF8Encoding.GetBytes(" cannotGrowTextBoxInTablix");
			m_classCannotShrinkTextBoxInTablix = uTF8Encoding.GetBytes(" cannotShrinkTextBoxInTablix");
			m_classCanGrowTextBoxInTablix = uTF8Encoding.GetBytes(" canGrowTextBoxInTablix");
			m_classCanShrinkTextBoxInTablix = uTF8Encoding.GetBytes(" canShrinkTextBoxInTablix");
			m_underscore = uTF8Encoding.GetBytes("_");
			m_openAccol = uTF8Encoding.GetBytes("{");
			m_closeAccol = uTF8Encoding.GetBytes("}");
			m_closeParenthesis = uTF8Encoding.GetBytes(")");
			m_classStyle = uTF8Encoding.GetBytes(" class=\"");
			m_openStyle = uTF8Encoding.GetBytes(" style=\"");
			m_styleHeight = uTF8Encoding.GetBytes("height:");
			m_styleMinHeight = uTF8Encoding.GetBytes("min-height:");
			m_styleMaxHeight = uTF8Encoding.GetBytes("max-height:");
			m_styleWidth = uTF8Encoding.GetBytes("width:");
			m_styleMinWidth = uTF8Encoding.GetBytes("min-width:");
			m_styleMaxWidth = uTF8Encoding.GetBytes("max-width:");
			m_zeroBorderWidth = uTF8Encoding.GetBytes("border-width:0px");
			m_border = uTF8Encoding.GetBytes("border:");
			m_borderLeft = uTF8Encoding.GetBytes("border-left:");
			m_borderTop = uTF8Encoding.GetBytes("border-top:");
			m_borderBottom = uTF8Encoding.GetBytes("border-bottom:");
			m_borderRight = uTF8Encoding.GetBytes("border-right:");
			m_borderColor = uTF8Encoding.GetBytes("border-color:");
			m_borderStyle = uTF8Encoding.GetBytes("border-style:");
			m_borderWidth = uTF8Encoding.GetBytes("border-width:");
			m_borderBottomColor = uTF8Encoding.GetBytes("border-bottom-color:");
			m_borderBottomStyle = uTF8Encoding.GetBytes("border-bottom-style:");
			m_borderBottomWidth = uTF8Encoding.GetBytes("border-bottom-width:");
			m_borderLeftColor = uTF8Encoding.GetBytes("border-left-color:");
			m_borderLeftStyle = uTF8Encoding.GetBytes("border-left-style:");
			m_borderLeftWidth = uTF8Encoding.GetBytes("border-left-width:");
			m_borderRightColor = uTF8Encoding.GetBytes("border-right-color:");
			m_borderRightStyle = uTF8Encoding.GetBytes("border-right-style:");
			m_borderRightWidth = uTF8Encoding.GetBytes("border-right-width:");
			m_borderTopColor = uTF8Encoding.GetBytes("border-top-color:");
			m_borderTopStyle = uTF8Encoding.GetBytes("border-top-style:");
			m_borderTopWidth = uTF8Encoding.GetBytes("border-top-width:");
			m_boxSizingBorderBox = uTF8Encoding.GetBytes("box-sizing: border-box;");
			m_semiColon = uTF8Encoding.GetBytes(";");
			m_wordWrap = uTF8Encoding.GetBytes("word-wrap:break-word");
			m_wordBreak = uTF8Encoding.GetBytes("word-break:break-word");
			m_wordWrapNormal = uTF8Encoding.GetBytes("word-wrap:normal;");
			m_wordBreakAll = uTF8Encoding.GetBytes("word-break:break-all;");
			m_whiteSpacePreWrap = uTF8Encoding.GetBytes("white-space:pre-wrap");
			m_overflow = uTF8Encoding.GetBytes("overflow:");
			m_overflowHidden = uTF8Encoding.GetBytes("overflow:hidden");
			m_overflowXHidden = uTF8Encoding.GetBytes("overflow-x:hidden");
			m_borderCollapse = uTF8Encoding.GetBytes("border-collapse:collapse");
			m_tableLayoutFixed = uTF8Encoding.GetBytes("table-layout:fixed");
			m_paddingLeft = uTF8Encoding.GetBytes("padding-left:");
			m_paddingRight = uTF8Encoding.GetBytes("padding-right:");
			m_paddingTop = uTF8Encoding.GetBytes("padding-top:");
			m_paddingBottom = uTF8Encoding.GetBytes("padding-bottom:");
			m_backgroundColor = uTF8Encoding.GetBytes("background-color:");
			m_backgroundImage = uTF8Encoding.GetBytes("background-image:url(");
			m_backgroundRepeat = uTF8Encoding.GetBytes("background-repeat:");
			m_backgroundSize = uTF8Encoding.GetBytes("background-size:");
			m_fontStyle = uTF8Encoding.GetBytes("font-style:");
			m_fontFamily = uTF8Encoding.GetBytes("font-family:");
			m_fontSize = uTF8Encoding.GetBytes("font-size:");
			m_fontWeight = uTF8Encoding.GetBytes("font-weight:");
			m_textDecoration = uTF8Encoding.GetBytes("text-decoration:");
			m_textAlign = uTF8Encoding.GetBytes("text-align:");
			m_verticalAlign = uTF8Encoding.GetBytes("vertical-align:");
			m_flexAlignItems = uTF8Encoding.GetBytes("align-items:");
			m_msFlexAlignItems = uTF8Encoding.GetBytes("-ms-flex-align:");
			m_webkitFlexAlignItems = uTF8Encoding.GetBytes("-webkit-align-items:");
			m_flexJustifyContent = uTF8Encoding.GetBytes("justify-content:");
			m_msFlexJustifyContent = uTF8Encoding.GetBytes("-ms-flex-pack:");
			m_webkitFlexJustifyContent = uTF8Encoding.GetBytes("-webkit-justify-content:");
			m_flexFlowRow = uTF8Encoding.GetBytes("-ms-flex-flow: row;-webkit-flex-flow: row;flex-flow: row;");
			m_flexFlowRowReverse = uTF8Encoding.GetBytes("-ms-flex-flow: row-reverse;-webkit-flex-flow: row-reverse;flex-flow: row-reverse;");
			m_color = uTF8Encoding.GetBytes("color:");
			m_lineHeight = uTF8Encoding.GetBytes("line-height:");
			m_direction = uTF8Encoding.GetBytes("direction:");
			m_unicodeBiDi = uTF8Encoding.GetBytes("unicode-bidi:");
			m_writingMode = uTF8Encoding.GetBytes("writing-mode:");
			m_msoRotation = uTF8Encoding.GetBytes("mso-rotate:");
			m_opacity = uTF8Encoding.GetBytes("opacity:");
			m_tbrl = uTF8Encoding.GetBytes("tb-rl");
			m_btrl = uTF8Encoding.GetBytes("bt-rl");
			m_lrtb = uTF8Encoding.GetBytes("lr-tb");
			m_rltb = uTF8Encoding.GetBytes("rl-tb");
			m_rotate180deg = uTF8Encoding.GetBytes("transform: rotate(180deg);");
			m_webkit_vertical = uTF8Encoding.GetBytes("-webkit-writing-mode: vertical-rl;");
			m_ms_vertical = uTF8Encoding.GetBytes("-ms-writing-mode: tb-rl;");
			m_ms_verticalRTL = uTF8Encoding.GetBytes("-ms-writing-mode: bt-rl;");
			m_ff_vertical = uTF8Encoding.GetBytes("writing-mode: vertical-rl;");
			m_layoutFlow = uTF8Encoding.GetBytes("layout-flow:");
			m_verticalIdeographic = uTF8Encoding.GetBytes("vertical-ideographic");
			m_horizontal = uTF8Encoding.GetBytes("horizontal");
			m_cursorHand = uTF8Encoding.GetBytes("cursor:pointer");
			m_filter = uTF8Encoding.GetBytes("filter:");
			m_language = uTF8Encoding.GetBytes(" LANG=\"");
			m_marginLeft = uTF8Encoding.GetBytes("margin-left:");
			m_marginTop = uTF8Encoding.GetBytes("margin-top:");
			m_marginBottom = uTF8Encoding.GetBytes("margin-bottom:");
			m_marginRight = uTF8Encoding.GetBytes("margin-right:");
			m_textIndent = uTF8Encoding.GetBytes("text-indent:");
			m_percentSizeString = uTF8Encoding.GetBytes("%");
			m_onLoadFitProportionalPv = uTF8Encoding.GetBytes(" onload=\"this.fitproportional=true;this.pv=");
			m_basicImageRotation180 = uTF8Encoding.GetBytes("progid:DXImageTransform.Microsoft.BasicImage(rotation=2)");
			m_openVGroup = uTF8Encoding.GetBytes("<v:group coordsize=\"100,100\" coordorigin=\"0,0\"");
			m_openVLine = uTF8Encoding.GetBytes("<v:line from=\"0,");
			m_strokeColor = uTF8Encoding.GetBytes(" strokecolor=\"");
			m_strokeWeight = uTF8Encoding.GetBytes(" strokeWeight=\"");
			m_dashStyle = uTF8Encoding.GetBytes("<v:stroke dashstyle=\"");
			m_slineStyle = uTF8Encoding.GetBytes(" slineStyle=\"");
			m_closeVGroup = uTF8Encoding.GetBytes("</v:line></v:group>");
			m_rightSlant = uTF8Encoding.GetBytes("100\" to=\"100,0\"");
			m_leftSlant = uTF8Encoding.GetBytes("0\" to=\"100,100\"");
			m_pageBreakDelimiter = uTF8Encoding.GetBytes("<div style=\"page-break-after:always\"><hr/></div>");
			m_stylePositionAbsolute = uTF8Encoding.GetBytes("position:absolute;");
			m_stylePositionRelative = uTF8Encoding.GetBytes("position:relative;");
			m_styleTop = uTF8Encoding.GetBytes("top:");
			m_styleLeft = uTF8Encoding.GetBytes("left:");
			m_closeUL = uTF8Encoding.GetBytes("</ul>");
			m_closeOL = uTF8Encoding.GetBytes("</ol>");
			m_olArabic = uTF8Encoding.GetBytes("<ol");
			m_olRoman = uTF8Encoding.GetBytes("<ol type=\"i\"");
			m_olAlpha = uTF8Encoding.GetBytes("<ol type=\"a\"");
			m_ulDisc = uTF8Encoding.GetBytes("<ul type=\"disc\"");
			m_ulSquare = uTF8Encoding.GetBytes("<ul type=\"square\"");
			m_ulCircle = uTF8Encoding.GetBytes("<ul type=\"circle\"");
			m_styleMinWidth = uTF8Encoding.GetBytes("min-width: ");
			m_styleMinHeight = uTF8Encoding.GetBytes("min-height: ");
			m_openInlineStyle = uTF8Encoding.GetBytes("<style type=\"text/css\">");
			m_closeInlineStyle = uTF8Encoding.GetBytes("</style>");
			m_openInlineJavaScript = uTF8Encoding.GetBytes("<script type=\"text/javascript\">");
			m_closeInlineJavaScript = uTF8Encoding.GetBytes("</script>");
			m_reportItemCustomAttr = uTF8Encoding.GetBytes(" data-reportitem=\"");
			m_noRepeat = uTF8Encoding.GetBytes("no-repeat");
			m_defaultPixelSize = uTF8Encoding.GetBytes("5px");
			m_auto = uTF8Encoding.GetBytes("auto");
		}
	}
}
