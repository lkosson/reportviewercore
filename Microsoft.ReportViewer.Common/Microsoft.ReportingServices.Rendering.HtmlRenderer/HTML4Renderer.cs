using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal abstract class HTML4Renderer : IHtmlReportWriter, IHtmlWriter, IHtmlRenderer
	{
		internal enum RequestType
		{
			Render,
			Search,
			Bookmark
		}

		internal enum Border
		{
			All,
			Left,
			Top,
			Right,
			Bottom
		}

		internal enum BorderAttribute
		{
			BorderWidth,
			BorderStyle,
			BorderColor
		}

		internal enum Direction
		{
			Row,
			Column
		}

		internal enum PageSection
		{
			Body,
			PageHeader,
			PageFooter
		}

		internal enum FontAttributes
		{
			None,
			Partial,
			All
		}

		private const float MaxWordSize = 558.8f;

		private const string FixedRowMarker = "r";

		private const string FixedColMarker = "c";

		private const string EmptyColMarker = "e";

		private const string EmptyHeightColMarker = "h";

		internal const string FixedRowGroupHeaderPrefix = "frgh";

		internal const string FixedCornerHeaderPrefix = "fch";

		internal const string FixedColGroupHeaderPrefix = "fcgh";

		internal const string FixedRGHArrayPrefix = "frhArr";

		internal const string FixedCGHArrayPrefix = "fcghArr";

		internal const string FixedCHArrayPrefix = "fchArr";

		internal const string ReportDiv = "oReportDiv";

		private const char Space = ' ';

		private const char Comma = ',';

		private const string MSuffix = "_m";

		private const string SSuffix = "_s";

		private const string ASuffix = "_a";

		private const string PSuffix = "_p";

		private const string FitVertTextSuffix = "_fvt";

		private const string GrowRectanglesSuffix = "_gr";

		private const string ImageConImageSuffix = "_ici";

		private const string ImageFitDivSuffix = "_ifd";

		private const long FitProptionalDefaultSize = 5L;

		protected const int SecondaryStreamBufferSize = 4096;

		internal const string SortAction = "Sort";

		internal const string ToggleAction = "Toggle";

		internal const string DrillthroughAction = "Drillthrough";

		internal const string BookmarkAction = "Bookmark";

		internal const string GetImageKey = "GetImage";

		internal const string SectionKey = "Section";

		internal const string PrefixIdKey = "PrefixId";

		internal const int IgnoreLeft = 1;

		internal const int IgnoreRight = 2;

		internal const int IgnoreTop = 4;

		internal const int IgnoreBottom = 8;

		internal const int IgnoreAll = 15;

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

		internal static byte[] m_space;

		internal static byte[] m_closeBracket;

		internal static byte[] m_semiColon;

		internal static byte[] m_border;

		internal static byte[] m_borderBottom;

		internal static byte[] m_borderLeft;

		internal static byte[] m_borderRight;

		internal static byte[] m_borderTop;

		internal static byte[] m_marginBottom;

		internal static byte[] m_marginLeft;

		internal static byte[] m_marginRight;

		internal static byte[] m_marginTop;

		internal static byte[] m_textIndent;

		internal static byte[] m_mm;

		internal static byte[] m_styleWidth;

		internal static byte[] m_styleHeight;

		internal static byte[] m_percent;

		internal static byte[] m_ninetyninepercent;

		internal static byte[] m_degree90;

		internal static byte[] m_newLine;

		internal static byte[] m_closeAccol;

		internal static byte[] m_backgroundRepeat;

		internal static byte[] m_closeBrace;

		internal static byte[] m_backgroundColor;

		internal static byte[] m_backgroundImage;

		internal static byte[] m_overflowHidden;

		internal static byte[] m_wordWrap;

		internal static byte[] m_whiteSpacePreWrap;

		internal static byte[] m_leftValue;

		internal static byte[] m_rightValue;

		internal static byte[] m_centerValue;

		internal static byte[] m_textAlign;

		internal static byte[] m_verticalAlign;

		internal static byte[] m_lineHeight;

		internal static byte[] m_color;

		internal static byte[] m_writingMode;

		internal static byte[] m_tbrl;

		internal static byte[] m_btrl;

		internal static byte[] m_lrtb;

		internal static byte[] m_rltb;

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

		protected static byte[] m_classAction;

		internal static byte[] m_styleAction;

		internal static byte[] m_emptyTextBox;

		internal static byte[] m_percentSizeInlineTable;

		internal static byte[] m_classPercentSizeInlineTable;

		internal static byte[] m_percentHeightInlineTable;

		internal static byte[] m_classPercentHeightInlineTable;

		internal static byte[] m_dot;

		internal static byte[] m_popupAction;

		internal static byte[] m_tableLayoutFixed;

		internal static byte[] m_borderCollapse;

		internal static byte[] m_none;

		internal static byte[] m_displayNone;

		internal static byte[] m_rtlEmbed;

		internal static byte[] m_classRtlEmbed;

		internal static byte[] m_noVerticalMarginClassName;

		internal static byte[] m_zeroPoint;

		internal static byte[] m_smallPoint;

		internal static byte[] m_filter;

		internal static byte[] m_basicImageRotation180;

		internal static byte[] m_msoRotation;

		internal static byte[] m_styleMinWidth;

		internal static byte[] m_styleMinHeight;

		private static byte[] m_styleDisplayInlineBlock;

		internal static byte[] m_reportItemCustomAttr;

		protected static byte[] m_br;

		protected static byte[] m_tabIndex;

		protected static byte[] m_closeTable;

		protected static byte[] m_openTable;

		protected static byte[] m_closeDiv;

		protected static byte[] m_openDiv;

		protected static byte[] m_zeroBorder;

		protected static byte[] m_cols;

		protected static byte[] m_colSpan;

		protected static byte[] m_rowSpan;

		protected static byte[] m_headers;

		protected static byte[] m_closeTD;

		protected static byte[] m_closeTR;

		protected static byte[] m_firstTD;

		protected static byte[] m_lastTD;

		protected static byte[] m_openTD;

		protected static byte[] m_openTR;

		protected static byte[] m_valign;

		protected static byte[] m_closeQuote;

		internal static string m_closeQuoteString;

		protected static byte[] m_closeSpan;

		protected static byte[] m_openSpan;

		protected static byte[] m_quote;

		internal static string m_quoteString;

		protected static byte[] m_closeTag;

		protected static byte[] m_id;

		protected static byte[] m_px;

		protected static byte[] m_zeroWidth;

		protected static byte[] m_zeroHeight;

		protected static byte[] m_openHtml;

		protected static byte[] m_closeHtml;

		protected static byte[] m_openBody;

		protected static byte[] m_closeBody;

		protected static byte[] m_openHead;

		protected static byte[] m_closeHead;

		protected static byte[] m_openTitle;

		protected static byte[] m_closeTitle;

		protected static byte[] m_openA;

		protected static byte[] m_target;

		protected static byte[] m_closeA;

		protected static string m_hrefString;

		protected static byte[] m_href;

		protected static byte[] m_nohref;

		protected static byte[] m_inlineHeight;

		protected static byte[] m_inlineWidth;

		protected static byte[] m_img;

		protected static byte[] m_imgOnError;

		protected static byte[] m_src;

		protected static byte[] m_topValue;

		protected static byte[] m_alt;

		protected static byte[] m_title;

		protected static byte[] m_classID;

		protected static byte[] m_codeBase;

		protected static byte[] m_valueObject;

		protected static byte[] m_paramObject;

		protected static byte[] m_openObject;

		protected static byte[] m_closeObject;

		protected static byte[] m_equal;

		protected static byte[] m_encodedAmp;

		protected static byte[] m_nbsp;

		protected static byte[] m_questionMark;

		protected static byte[] m_checked;

		protected static byte[] m_checkForEnterKey;

		protected static byte[] m_unchecked;

		protected static byte[] m_showHideOnClick;

		protected static byte[] m_cursorHand;

		protected static byte[] m_rtlDir;

		protected static byte[] m_ltrDir;

		protected static byte[] m_classStyle;

		protected static byte[] m_openStyle;

		protected static byte[] m_underscore;

		protected static byte[] m_lineBreak;

		protected static byte[] m_ssClassID;

		protected static byte[] m_ptClassID;

		protected static byte[] m_xmlData;

		protected static byte[] m_useMap;

		protected static byte[] m_openMap;

		protected static byte[] m_closeMap;

		protected static byte[] m_mapArea;

		protected static byte[] m_mapCoords;

		protected static byte[] m_mapShape;

		protected static byte[] m_name;

		protected static byte[] m_dataName;

		protected static byte[] m_circleShape;

		protected static byte[] m_polyShape;

		protected static byte[] m_rectShape;

		protected static byte[] m_comma;

		private static string m_mapPrefixString;

		protected static byte[] m_mapPrefix;

		protected static byte[] m_classPopupAction;

		protected static byte[] m_closeLi;

		protected static byte[] m_openLi;

		protected static byte[] m_firstNonHeaderPostfix;

		protected static byte[] m_fixedMatrixCornerPostfix;

		protected static byte[] m_fixedRowGroupingHeaderPostfix;

		protected static byte[] m_fixedColumnGroupingHeaderPostfix;

		protected static byte[] m_fixedRowHeaderPostfix;

		protected static byte[] m_fixedColumnHeaderPostfix;

		protected static byte[] m_fixedTableCornerPostfix;

		internal static byte[] m_language;

		private static byte[] m_zeroBorderWidth;

		internal static byte[] m_onLoadFitProportionalPv;

		private static byte[] m_normalWordWrap;

		private static byte[] m_classPercentSizes;

		private static byte[] m_classPercentSizesOverflow;

		private static byte[] m_classPercentWidthOverflow;

		private static byte[] m_classPercentHeight;

		private static byte[] m_classLayoutBorder;

		private static byte[] m_classLayoutFixed;

		private static byte[] m_strokeColor;

		private static byte[] m_strokeWeight;

		private static byte[] m_slineStyle;

		private static byte[] m_dashStyle;

		private static byte[] m_closeVGroup;

		private static byte[] m_openVGroup;

		private static byte[] m_openVLine;

		private static byte[] m_leftSlant;

		private static byte[] m_rightSlant;

		private static byte[] m_pageBreakDelimiter;

		private static byte[] m_nogrowAttribute;

		private static byte[] m_stylePositionAbsolute;

		private static byte[] m_stylePositionRelative;

		private static byte[] m_styleClipRectOpenBrace;

		private static byte[] m_styleTop;

		private static byte[] m_styleLeft;

		private static byte[] m_pxSpace;

		internal static char[] m_cssDelimiters;

		protected bool m_hasOnePage = true;

		protected Stream m_mainStream;

		internal Encoding m_encoding;

		protected RPLReport m_rplReport;

		protected RPLPageContent m_pageContent;

		protected RPLReportSection m_rplReportSection;

		protected IReportWrapper m_report;

		protected ISPBProcessing m_spbProcessing;

		protected Hashtable m_usedStyles;

		protected NameValueCollection m_serverParams;

		protected DeviceInfo m_deviceInfo;

		protected NameValueCollection m_rawDeviceInfo;

		protected Dictionary<string, string> m_images;

		protected byte[] m_stylePrefixIdBytes;

		protected int m_pageNum;

		protected CreateAndRegisterStream m_createAndRegisterStreamCallback;

		protected bool m_fitPropImages;

		protected bool m_browserIE = true;

		protected RequestType m_requestType;

		protected bool m_htmlFragment;

		protected Stream m_styleStream;

		protected Stream m_growRectangleIdsStream;

		protected Stream m_fitVertTextIdsStream;

		protected Stream m_imgFitDivIdsStream;

		protected Stream m_imgConImageIdsStream;

		protected bool m_useInlineStyle;

		protected bool m_pageWithBookmarkLinks;

		protected bool m_pageWithSortClicks;

		protected bool m_allPages;

		protected int m_outputLineLength;

		protected bool m_onlyVisibleStyles;

		private SecondaryStreams m_createSecondaryStreams = SecondaryStreams.Server;

		protected int m_tabIndexNum;

		protected int m_currentHitCount;

		protected Hashtable m_duplicateItems;

		protected string m_searchText;

		protected bool m_emitImageConsolidationScaling;

		protected bool m_needsCanGrowFalseScript;

		protected bool m_needsGrowRectangleScript;

		protected bool m_needsFitVertTextScript;

		internal static string m_searchHitIdPrefix;

		internal static string m_standardLineBreak;

		protected Stack m_linkToChildStack;

		protected PageSection m_pageSection;

		internal const char StreamNameSeparator = '_';

		internal const string PageStyleName = "p";

		internal const string MHTMLPrefix = "cid:";

		internal const string CSSSuffix = "style";

		protected const string m_resourceNamespace = "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources";

		protected bool m_pageHasStyle;

		protected bool m_isBody;

		protected bool m_usePercentWidth;

		protected bool m_hasSlantedLines;

		internal bool m_expandItem;

		protected ArrayList m_fixedHeaders;

		private bool m_isStyleOpen;

		private bool m_renderTableHeight;

		private string m_contextLanguage;

		private bool m_allowBandTable = true;

		protected byte[] m_styleClassPrefix;

		internal string SearchText
		{
			set
			{
				m_searchText = value;
			}
		}

		internal bool NeedResizeImages => m_fitPropImages;

		protected bool IsFragment
		{
			get
			{
				if (m_htmlFragment)
				{
					return !m_deviceInfo.HasActionScript;
				}
				return false;
			}
		}

		public bool IsBrowserIE => m_deviceInfo.IsBrowserIE;

		protected virtual bool FillPageHeight => m_deviceInfo.IsBrowserIE;

		static HTML4Renderer()
		{
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
			m_space = null;
			m_closeBracket = null;
			m_semiColon = null;
			m_border = null;
			m_borderBottom = null;
			m_borderLeft = null;
			m_borderRight = null;
			m_borderTop = null;
			m_marginBottom = null;
			m_marginLeft = null;
			m_marginRight = null;
			m_marginTop = null;
			m_textIndent = null;
			m_mm = null;
			m_styleWidth = null;
			m_styleHeight = null;
			m_percent = null;
			m_ninetyninepercent = null;
			m_degree90 = null;
			m_newLine = null;
			m_closeAccol = null;
			m_backgroundRepeat = null;
			m_closeBrace = null;
			m_backgroundColor = null;
			m_backgroundImage = null;
			m_overflowHidden = null;
			m_wordWrap = null;
			m_whiteSpacePreWrap = null;
			m_leftValue = null;
			m_rightValue = null;
			m_centerValue = null;
			m_textAlign = null;
			m_verticalAlign = null;
			m_lineHeight = null;
			m_color = null;
			m_writingMode = null;
			m_tbrl = null;
			m_btrl = null;
			m_lrtb = null;
			m_rltb = null;
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
			m_borderCollapse = null;
			m_none = null;
			m_displayNone = null;
			m_rtlEmbed = null;
			m_classRtlEmbed = null;
			m_noVerticalMarginClassName = null;
			m_zeroPoint = null;
			m_smallPoint = null;
			m_filter = null;
			m_basicImageRotation180 = null;
			m_msoRotation = null;
			m_styleMinWidth = null;
			m_styleMinHeight = null;
			m_styleDisplayInlineBlock = null;
			m_reportItemCustomAttr = null;
			m_br = null;
			m_tabIndex = null;
			m_closeTable = null;
			m_openTable = null;
			m_closeDiv = null;
			m_openDiv = null;
			m_zeroBorder = null;
			m_cols = null;
			m_colSpan = null;
			m_rowSpan = null;
			m_headers = null;
			m_closeTD = null;
			m_closeTR = null;
			m_firstTD = null;
			m_lastTD = null;
			m_openTD = null;
			m_openTR = null;
			m_valign = null;
			m_closeQuote = null;
			m_closeQuoteString = "\">";
			m_closeSpan = null;
			m_openSpan = null;
			m_quote = null;
			m_quoteString = "\"";
			m_closeTag = null;
			m_id = null;
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
			m_openA = null;
			m_target = null;
			m_closeA = null;
			m_hrefString = " href=\"";
			m_href = null;
			m_nohref = null;
			m_inlineHeight = null;
			m_inlineWidth = null;
			m_img = null;
			m_imgOnError = null;
			m_src = null;
			m_topValue = null;
			m_alt = null;
			m_title = null;
			m_classID = null;
			m_codeBase = null;
			m_valueObject = null;
			m_paramObject = null;
			m_openObject = null;
			m_closeObject = null;
			m_equal = null;
			m_encodedAmp = null;
			m_nbsp = null;
			m_questionMark = null;
			m_checked = null;
			m_checkForEnterKey = null;
			m_unchecked = null;
			m_showHideOnClick = null;
			m_cursorHand = null;
			m_rtlDir = null;
			m_ltrDir = null;
			m_classStyle = null;
			m_openStyle = null;
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
			m_mapPrefixString = "Map";
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
			m_normalWordWrap = null;
			m_classPercentSizes = null;
			m_classPercentSizesOverflow = null;
			m_classPercentWidthOverflow = null;
			m_classPercentHeight = null;
			m_classLayoutBorder = null;
			m_classLayoutFixed = null;
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
			m_nogrowAttribute = null;
			m_stylePositionAbsolute = null;
			m_stylePositionRelative = null;
			m_styleClipRectOpenBrace = null;
			m_styleTop = null;
			m_styleLeft = null;
			m_pxSpace = null;
			m_cssDelimiters = new char[13]
			{
				'[',
				']',
				'"',
				'\'',
				'<',
				'>',
				'{',
				'}',
				'(',
				')',
				'/',
				'%',
				' '
			};
			m_searchHitIdPrefix = "oHit";
			m_standardLineBreak = "\n";
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
			m_space = uTF8Encoding.GetBytes(" ");
			m_closeBracket = uTF8Encoding.GetBytes(">");
			m_closeTable = uTF8Encoding.GetBytes("</TABLE>");
			m_openDiv = uTF8Encoding.GetBytes("<DIV");
			m_closeDiv = uTF8Encoding.GetBytes("</DIV>");
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
			m_img = uTF8Encoding.GetBytes("<IMG");
			m_imgOnError = uTF8Encoding.GetBytes(" onerror=\"this.errored=true;\"");
			m_src = uTF8Encoding.GetBytes(" SRC=\"");
			m_topValue = uTF8Encoding.GetBytes("top");
			m_leftValue = uTF8Encoding.GetBytes("left");
			m_rightValue = uTF8Encoding.GetBytes("right");
			m_centerValue = uTF8Encoding.GetBytes("center");
			m_classID = uTF8Encoding.GetBytes(" CLASSID=\"CLSID:");
			m_codeBase = uTF8Encoding.GetBytes(" CODEBASE=\"");
			m_title = uTF8Encoding.GetBytes(" TITLE=\"");
			m_alt = uTF8Encoding.GetBytes(" ALT=\"");
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
			m_classLayoutFixed = uTF8Encoding.GetBytes(" class=\"r9");
			m_classLayoutBorder = uTF8Encoding.GetBytes(" class=\"r10");
			m_classPercentWidthOverflow = uTF8Encoding.GetBytes(" class=\"r11\"");
			m_classPopupAction = uTF8Encoding.GetBytes(" class=\"r12\"");
			m_classAction = uTF8Encoding.GetBytes(" class=\"r13\"");
			m_rtlEmbed = uTF8Encoding.GetBytes("r15");
			m_classRtlEmbed = uTF8Encoding.GetBytes(" class=\"r15\"");
			m_noVerticalMarginClassName = uTF8Encoding.GetBytes("r16");
			m_percentSizeInlineTable = uTF8Encoding.GetBytes("r17");
			m_classPercentSizeInlineTable = uTF8Encoding.GetBytes(" class=\"r17\"");
			m_percentHeightInlineTable = uTF8Encoding.GetBytes("r18");
			m_classPercentHeightInlineTable = uTF8Encoding.GetBytes(" class=\"r18\"");
			m_underscore = uTF8Encoding.GetBytes("_");
			m_openAccol = uTF8Encoding.GetBytes("{");
			m_closeAccol = uTF8Encoding.GetBytes("}");
			m_classStyle = uTF8Encoding.GetBytes(" class=\"");
			m_openStyle = uTF8Encoding.GetBytes(" style=\"");
			m_styleHeight = uTF8Encoding.GetBytes("HEIGHT:");
			m_styleMinHeight = uTF8Encoding.GetBytes("min-height:");
			m_styleWidth = uTF8Encoding.GetBytes("WIDTH:");
			m_styleMinWidth = uTF8Encoding.GetBytes("min-width:");
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
			m_semiColon = uTF8Encoding.GetBytes(";");
			m_wordWrap = uTF8Encoding.GetBytes("word-wrap:break-word");
			m_whiteSpacePreWrap = uTF8Encoding.GetBytes("white-space:pre-wrap");
			m_normalWordWrap = uTF8Encoding.GetBytes("word-wrap:normal");
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
			m_fontStyle = uTF8Encoding.GetBytes("font-style:");
			m_fontFamily = uTF8Encoding.GetBytes("font-family:");
			m_fontSize = uTF8Encoding.GetBytes("font-size:");
			m_fontWeight = uTF8Encoding.GetBytes("font-weight:");
			m_textDecoration = uTF8Encoding.GetBytes("text-decoration:");
			m_textAlign = uTF8Encoding.GetBytes("text-align:");
			m_verticalAlign = uTF8Encoding.GetBytes("vertical-align:");
			m_color = uTF8Encoding.GetBytes("color:");
			m_lineHeight = uTF8Encoding.GetBytes("line-height:");
			m_direction = uTF8Encoding.GetBytes("direction:");
			m_unicodeBiDi = uTF8Encoding.GetBytes("unicode-bidi:");
			m_writingMode = uTF8Encoding.GetBytes("writing-mode:");
			m_msoRotation = uTF8Encoding.GetBytes("mso-rotate:");
			m_tbrl = uTF8Encoding.GetBytes("tb-rl");
			m_btrl = uTF8Encoding.GetBytes("bt-rl");
			m_lrtb = uTF8Encoding.GetBytes("lr-tb");
			m_rltb = uTF8Encoding.GetBytes("rl-tb");
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
			m_styleClipRectOpenBrace = uTF8Encoding.GetBytes("clip:rect(");
			m_styleTop = uTF8Encoding.GetBytes("top:");
			m_styleLeft = uTF8Encoding.GetBytes("left:");
			m_pxSpace = uTF8Encoding.GetBytes("px ");
			m_nogrowAttribute = uTF8Encoding.GetBytes(" nogrow=\"true\"");
			m_styleMinWidth = uTF8Encoding.GetBytes("min-width: ");
			m_styleMinHeight = uTF8Encoding.GetBytes("min-height: ");
			m_styleDisplayInlineBlock = uTF8Encoding.GetBytes("display: inline-block;");
			m_reportItemCustomAttr = uTF8Encoding.GetBytes(" data-reportitem=\"");
		}

		public HTML4Renderer(IReportWrapper report, ISPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams)
		{
			SearchText = deviceInfo.FindString;
			m_report = report;
			m_spbProcessing = spbProcessing;
			m_createSecondaryStreams = secondaryStreams;
			m_usedStyles = new Hashtable();
			m_images = new Dictionary<string, string>();
			m_browserIE = deviceInfo.IsBrowserIE;
			m_deviceInfo = deviceInfo;
			m_rawDeviceInfo = rawDeviceInfo;
			m_serverParams = reportServerParams;
			m_createAndRegisterStreamCallback = createAndRegisterStreamCallback;
			m_htmlFragment = deviceInfo.HTMLFragment;
			m_onlyVisibleStyles = deviceInfo.OnlyVisibleStyles;
			m_pageNum = deviceInfo.Section;
			rawDeviceInfo.Remove("Section");
			rawDeviceInfo.Remove("FindString");
			rawDeviceInfo.Remove("BookmarkId");
			SPBContext context = new SPBContext
			{
				StartPage = m_pageNum,
				EndPage = m_pageNum,
				SecondaryStreams = m_createSecondaryStreams,
				AddSecondaryStreamNames = true,
				UseImageConsolidation = m_deviceInfo.ImageConsolidation
			};
			m_spbProcessing.SetContext(context);
			m_linkToChildStack = new Stack(1);
			m_stylePrefixIdBytes = Encoding.UTF8.GetBytes(m_deviceInfo.StylePrefixId);
			if (!m_deviceInfo.StyleStream)
			{
				m_useInlineStyle = m_htmlFragment;
			}
		}

		internal void InitializeReport()
		{
			m_rplReport = GetNextPage();
			if (m_rplReport == null)
			{
				throw new InvalidSectionException();
			}
			m_pageContent = m_rplReport.RPLPaginatedPages[0];
			m_rplReportSection = m_pageContent.GetNextReportSection();
			CheckBodyStyle();
			m_contextLanguage = m_rplReport.Language;
			m_expandItem = false;
		}

		protected static string GetStyleStreamName(string aReportName, int aPageNumber)
		{
			return GetStreamName(aReportName, aPageNumber, "style");
		}

		internal static string GetStreamName(string aReportName, int aPageNumber, string suffix)
		{
			if (aPageNumber > 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}", aReportName, '_', suffix, aPageNumber);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", aReportName, '_', suffix);
		}

		internal static string HandleSpecialFontCharacters(string fontName)
		{
			if (fontName.IndexOfAny(m_cssDelimiters) != -1)
			{
				fontName = fontName.Trim();
				if (fontName.StartsWith("'", StringComparison.Ordinal))
				{
					fontName = fontName.Substring(1);
				}
				if (fontName.EndsWith("'", StringComparison.Ordinal))
				{
					fontName = fontName.Substring(0, fontName.Length - 1);
				}
				return "'" + fontName.Replace("'", "&quot;") + "'";
			}
			return fontName;
		}

		protected abstract void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState);

		protected abstract void RenderInternalImageSrc();

		protected abstract void RenderToggleImage(RPLTextBoxProps textBoxProps);

		public abstract void Render(HtmlTextWriter outputWriter);

		internal void RenderStylesOnly(string streamName)
		{
			m_encoding = Encoding.UTF8;
			Stream stream = CreateStream(streamName, "css", Encoding.UTF8, "text/css", willSeek: false, StreamOper.CreateAndRegister);
			StyleContext styleContext = new StyleContext();
			int num = 0;
			for (m_styleStream = new BufferedStream(stream); m_rplReportSection != null; m_rplReportSection = m_pageContent.GetNextReportSection())
			{
				num = 0;
				RPLItemMeasurement header = m_rplReportSection.Header;
				if (header != null)
				{
					RPLHeaderFooter rPLHeaderFooter = (RPLHeaderFooter)header.Element;
					RPLElementProps elementProps = rPLHeaderFooter.ElementProps;
					RPLElementPropsDef definition = elementProps.Definition;
					styleContext.StyleOnCell = true;
					RenderSharedStyle(rPLHeaderFooter, elementProps, definition, definition.SharedStyle, header, definition.ID + "c", styleContext, ref num);
					styleContext.StyleOnCell = false;
					RenderSharedStyle(rPLHeaderFooter, elementProps, definition, definition.SharedStyle, header, definition.ID, styleContext, ref num);
					RPLItemMeasurement[] children = rPLHeaderFooter.Children;
					if (children != null)
					{
						for (int i = 0; i < children.Length; i++)
						{
							RenderStylesOnlyRecursive(children[i], new StyleContext());
						}
					}
					header.Element = null;
				}
				RPLItemMeasurement footer = m_rplReportSection.Footer;
				if (footer != null)
				{
					RPLHeaderFooter rPLHeaderFooter2 = (RPLHeaderFooter)footer.Element;
					RPLElementProps elementProps2 = rPLHeaderFooter2.ElementProps;
					RPLElementPropsDef definition2 = elementProps2.Definition;
					styleContext.StyleOnCell = true;
					RenderSharedStyle(rPLHeaderFooter2, elementProps2, definition2, definition2.SharedStyle, footer, definition2.ID + "c", styleContext, ref num);
					styleContext.StyleOnCell = false;
					RenderSharedStyle(rPLHeaderFooter2, elementProps2, definition2, definition2.SharedStyle, footer, definition2.ID, styleContext, ref num);
					RPLItemMeasurement[] children2 = rPLHeaderFooter2.Children;
					if (children2 != null)
					{
						for (int j = 0; j < children2.Length; j++)
						{
							RenderStylesOnlyRecursive(children2[j], new StyleContext());
						}
					}
					footer.Element = null;
				}
				RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
				rPLItemMeasurement.Width = m_pageContent.MaxSectionWidth;
				rPLItemMeasurement.Height = m_rplReportSection.BodyArea.Height;
				RPLItemMeasurement rPLItemMeasurement2 = m_rplReportSection.Columns[0];
				RPLBody rPLBody = (RPLBody)m_rplReportSection.Columns[0].Element;
				RPLElementProps elementProps3 = rPLBody.ElementProps;
				RPLElementPropsDef definition3 = elementProps3.Definition;
				RenderSharedStyle(rPLBody, elementProps3, definition3, definition3.SharedStyle, rPLItemMeasurement, definition3.ID, styleContext, ref num);
				RPLItemMeasurement[] children3 = rPLBody.Children;
				if (children3 != null && children3.Length != 0)
				{
					for (int k = 0; k < children3.Length; k++)
					{
						RenderStylesOnlyRecursive(children3[k], new StyleContext());
					}
				}
				rPLItemMeasurement2.Element = null;
			}
			m_styleStream.Flush();
		}

		internal void RenderStylesOnlyRecursive(RPLItemMeasurement measurement, StyleContext styleContext)
		{
			int borderContext = 0;
			RPLElement element = measurement.Element;
			RPLElementProps elementProps = element.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLStyleProps sharedStyle = definition.SharedStyle;
			string iD = definition.ID;
			object obj = elementProps.Style[26];
			if (element is RPLTextBox)
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
				bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
				if (rPLTextBoxPropsDef.CanSort && !m_usedStyles.ContainsKey(iD + "p"))
				{
					if (rPLTextBoxPropsDef.CanGrow || rPLTextBoxPropsDef.CanShrink)
					{
						styleContext.StyleOnCell = true;
					}
					if (!rPLTextBoxPropsDef.CanGrow && rPLTextBoxPropsDef.CanShrink)
					{
						styleContext.IgnoreVerticalAlign = true;
					}
					RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "p", styleContext, ref borderContext);
					styleContext.StyleOnCell = false;
				}
				if (!m_deviceInfo.IsBrowserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || m_deviceInfo.OutlookCompat || (obj != null && (RPLFormat.VerticalAlignments)obj != 0))
				{
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				}
				if (rPLTextBoxPropsDef.CanShrink && !m_usedStyles.ContainsKey(iD + "s"))
				{
					styleContext.NoBorders = true;
					RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "s", styleContext, ref borderContext);
					if (!rPLTextBoxPropsDef.CanGrow)
					{
						styleContext.IgnoreVerticalAlign = true;
					}
				}
				if (rPLTextBoxPropsDef.CanSort && !rPLTextBoxPropsDef.IsSimple && !IsFragment && rPLTextBoxPropsDef.IsToggleParent)
				{
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				}
				styleContext.RenderMeasurements = false;
				if (!m_usedStyles.ContainsKey(iD))
				{
					int borderContext2 = borderContext;
					RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD, styleContext, ref borderContext2);
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
					borderContext2 = borderContext;
					RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "l", styleContext, ref borderContext2);
					RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "r", styleContext, ref borderContext);
				}
				RPLTextBoxProps rPLTextBoxProps = elementProps as RPLTextBoxProps;
				if (!m_usedStyles.ContainsKey(iD + "a") && HasAction(rPLTextBoxProps.ActionInfo))
				{
					TextRunStyleWriter textRunStyleWriter = new TextRunStyleWriter(this);
					RenderSharedStyle(textRunStyleWriter, definition.SharedStyle, styleContext, iD + "a");
					textRunStyleWriter.WriteStyles(StyleWriterMode.Shared, definition.SharedStyle);
				}
				if (rPLTextBoxPropsDef.IsSimple)
				{
					return;
				}
				RPLTextBox rPLTextBox = element as RPLTextBox;
				ParagraphStyleWriter paragraphStyleWriter = new ParagraphStyleWriter(this, rPLTextBox);
				TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
				for (RPLParagraph nextParagraph = rPLTextBox.GetNextParagraph(); nextParagraph != null; nextParagraph = rPLTextBox.GetNextParagraph())
				{
					paragraphStyleWriter.Paragraph = nextParagraph;
					string iD2 = nextParagraph.ElementProps.Definition.ID;
					paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
					RenderSharedStyle(paragraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2);
					paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
					RenderSharedStyle(paragraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "l");
					paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
					RenderSharedStyle(paragraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "p");
					for (RPLTextRun nextTextRun = nextParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = nextParagraph.GetNextTextRun())
					{
						RenderSharedStyle(styleWriter, nextTextRun.ElementProps.Definition.SharedStyle, styleContext, nextTextRun.ElementProps.Definition.ID);
					}
				}
				return;
			}
			if (!m_usedStyles.ContainsKey(iD))
			{
				RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD, styleContext, ref borderContext);
			}
			if (element is RPLSubReport)
			{
				RPLItemMeasurement[] children = ((RPLSubReport)element).Children;
				if (children != null)
				{
					for (int i = 0; i < children.Length; i++)
					{
						RPLContainer rPLContainer = children[i].Element as RPLContainer;
						if (rPLContainer != null && rPLContainer.Children != null && rPLContainer.Children.Length != 0)
						{
							for (int j = 0; j < rPLContainer.Children.Length; j++)
							{
								RenderStylesOnlyRecursive(rPLContainer.Children[j], styleContext);
								rPLContainer.Children[j] = null;
							}
						}
						children[i] = null;
					}
					measurement.Element = null;
				}
			}
			else if (element is RPLContainer)
			{
				styleContext.InTablix = false;
				RPLItemMeasurement[] children2 = ((RPLContainer)element).Children;
				if (children2 != null && children2.Length != 0)
				{
					for (int k = 0; k < children2.Length; k++)
					{
						RenderStylesOnlyRecursive(children2[k], styleContext);
						children2[k] = null;
					}
				}
			}
			else if (element is RPLTablix)
			{
				RPLTablix rPLTablix = (RPLTablix)element;
				RPLTablixRow nextRow = rPLTablix.GetNextRow();
				bool inTablix = styleContext.InTablix;
				while (nextRow != null)
				{
					for (int l = 0; l < nextRow.NumCells; l++)
					{
						RPLTablixCell rPLTablixCell = nextRow[l];
						RPLElement element2 = rPLTablixCell.Element;
						RPLElementProps elementProps2 = element2.ElementProps;
						RPLElementPropsDef definition2 = elementProps2.Definition;
						RPLStyleProps sharedStyle2 = definition2.SharedStyle;
						bool zeroWidth = styleContext.ZeroWidth;
						float columnWidth = rPLTablix.GetColumnWidth(rPLTablixCell.ColIndex, rPLTablixCell.ColSpan);
						styleContext.ZeroWidth = (columnWidth == 0f);
						if (element2 == null)
						{
							continue;
						}
						string iD3 = definition2.ID;
						if (!(element2 is RPLLine) && !m_usedStyles.ContainsKey(iD3 + "c"))
						{
							styleContext.StyleOnCell = true;
							borderContext = GetNewContext(borderContext, rPLTablixCell.ColIndex == 0, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == rPLTablix.ColumnWidths.Length, rPLTablixCell.RowIndex == 0, rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == rPLTablix.RowHeights.Length);
							int borderContext3 = borderContext;
							RPLTextBox rPLTextBox2 = (RPLTextBox)element2;
							bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
							if (rPLTextBox2 != null && IsWritingModeVertical(sharedStyle2) && m_deviceInfo.IsBrowserIE && m_deviceInfo.BrowserMode == BrowserMode.Standards)
							{
								styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
							}
							RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, iD3 + "c", styleContext, ref borderContext3);
							borderContext3 = borderContext;
							RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, iD3 + "cl", styleContext, ref borderContext3);
							RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, iD3 + "cr", styleContext, ref borderContext);
							styleContext.StyleOnCell = false;
							styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
						}
						styleContext.InTablix = true;
						if (element2 is RPLContainer)
						{
							RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
							rPLItemMeasurement.Width = rPLTablix.GetColumnWidth(rPLTablixCell.ColIndex, rPLTablixCell.ColSpan);
							rPLItemMeasurement.Height = rPLTablix.GetRowHeight(rPLTablixCell.RowIndex, rPLTablixCell.RowSpan);
							rPLItemMeasurement.Element = (element2 as RPLItem);
							RenderStylesOnlyRecursive(rPLItemMeasurement, styleContext);
						}
						else if (!m_usedStyles.ContainsKey(iD3))
						{
							if (element2 is RPLTextBox)
							{
								object obj2 = element2.ElementProps.Style[26];
								RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)element2.ElementProps.Definition;
								bool flag = obj2 != null && (RPLFormat.VerticalAlignments)obj2 != 0 && !rPLTextBoxPropsDef2.CanGrow;
								if (rPLTextBoxPropsDef2.CanSort || flag)
								{
									styleContext.RenderMeasurements = false;
								}
							}
							RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, element2.ElementProps.Definition.ID, styleContext, ref borderContext);
						}
						styleContext.InTablix = inTablix;
						nextRow[l] = null;
						styleContext.ZeroWidth = zeroWidth;
					}
					nextRow = rPLTablix.GetNextRow();
				}
			}
			measurement.Element = null;
		}

		internal void RenderEmptyTopTablixRow(RPLTablix tablix, List<RPLTablixOmittedRow> omittedRows, string tablixID, bool emptyCol, TablixFixedHeaderStorage headerStorage)
		{
			bool flag = headerStorage.RowHeaders != null || headerStorage.ColumnHeaders != null;
			WriteStream(m_openTR);
			if (flag)
			{
				string text = tablixID + "r";
				RenderReportItemId(text);
				if (headerStorage.RowHeaders != null)
				{
					headerStorage.RowHeaders.Add(text);
				}
				if (headerStorage.ColumnHeaders != null)
				{
					headerStorage.ColumnHeaders.Add(text);
				}
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			WriteStream(m_zeroHeight);
			WriteStream(m_closeBracket);
			if (emptyCol)
			{
				headerStorage.HasEmptyCol = true;
				WriteStream(m_openTD);
				if (headerStorage.RowHeaders != null)
				{
					string text2 = tablixID + "e";
					RenderReportItemId(text2);
					headerStorage.RowHeaders.Add(text2);
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text2);
					}
				}
				WriteStream(m_openStyle);
				WriteStream(m_styleWidth);
				WriteStream("0");
				WriteStream(m_px);
				WriteStream(m_closeQuote);
				WriteStream(m_closeTD);
			}
			int[] array = new int[omittedRows.Count];
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				WriteStream(m_openTD);
				if (tablix.FixedColumns[i] && headerStorage.RowHeaders != null)
				{
					string text3 = tablixID + "e" + i;
					RenderReportItemId(text3);
					headerStorage.RowHeaders.Add(text3);
					if (i == tablix.ColumnWidths.Length - 1 || !tablix.FixedColumns[i + 1])
					{
						headerStorage.LastRowGroupCol = text3;
					}
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text3);
					}
				}
				WriteStream(m_openStyle);
				if (tablix.ColumnWidths[i] == 0f)
				{
					WriteStream(m_displayNone);
				}
				WriteStream(m_styleWidth);
				WriteDStream(tablix.ColumnWidths[i]);
				WriteStream(m_mm);
				WriteStream(m_semiColon);
				WriteStream(m_styleMinWidth);
				WriteDStream(tablix.ColumnWidths[i]);
				WriteStream(m_mm);
				WriteStream(m_closeQuote);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					List<RPLTablixMemberCell> omittedHeaders = omittedRows[j].OmittedHeaders;
					RenderTablixOmittedHeaderCells(omittedHeaders, i, lastCol: false, ref array[j]);
				}
				WriteStream(m_closeTD);
			}
			WriteStream(m_closeTR);
		}

		internal void RenderEmptyHeightCell(float height, string tablixID, bool fixedRow, int row, TablixFixedHeaderStorage headerStorage)
		{
			WriteStream(m_openTD);
			if (headerStorage.RowHeaders != null)
			{
				string text = tablixID + "h" + row;
				RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (fixedRow && headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			WriteStream(m_openStyle);
			WriteStream(m_styleHeight);
			WriteDStream(height);
			WriteStream(m_mm);
			WriteStream(m_closeQuote);
			WriteStream(m_closeTD);
		}

		protected static int GetNewContext(int borderContext, bool left, bool right, bool top, bool bottom)
		{
			int num = 0;
			if (borderContext > 0)
			{
				if (top)
				{
					num |= (borderContext & 4);
				}
				if (bottom)
				{
					num |= (borderContext & 8);
				}
				if (left)
				{
					num |= (borderContext & 1);
				}
				if (right)
				{
					num |= (borderContext & 2);
				}
			}
			return num;
		}

		protected static int GetNewContext(int borderContext, int x, int y, int xMax, int yMax)
		{
			int num = 0;
			if (borderContext > 0)
			{
				if (x == 1)
				{
					num |= (borderContext & 4);
				}
				if (x == xMax)
				{
					num |= (borderContext & 8);
				}
				if (y == 1)
				{
					num |= (borderContext & 1);
				}
				if (y == yMax)
				{
					num |= (borderContext & 2);
				}
			}
			return num;
		}

		protected System.Drawing.Rectangle RenderDynamicImage(RPLItemMeasurement measurement, RPLDynamicImageProps dynamicImageProps)
		{
			if (m_createSecondaryStreams != 0)
			{
				return dynamicImageProps.ImageConsolidationOffsets;
			}
			Stream stream = null;
			stream = CreateStream(dynamicImageProps.StreamName, "png", null, "image/png", willSeek: false, StreamOper.CreateAndRegister);
			if (dynamicImageProps.DynamicImageContentOffset >= 0)
			{
				m_rplReport.GetImage(dynamicImageProps.DynamicImageContentOffset, stream);
			}
			else if (dynamicImageProps.DynamicImageContent != null)
			{
				byte[] array = new byte[4096];
				dynamicImageProps.DynamicImageContent.Position = 0L;
				int num = (int)dynamicImageProps.DynamicImageContent.Length;
				while (num > 0)
				{
					int num2 = dynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
					stream.Write(array, 0, num2);
					num -= num2;
				}
			}
			return System.Drawing.Rectangle.Empty;
		}

		protected bool IsCollectionWithoutContent(RPLContainer container, ref bool empty)
		{
			bool result = false;
			if (container != null)
			{
				result = true;
				if (container.Children == null)
				{
					empty = true;
				}
			}
			return result;
		}

		private void RenderOpenStyle(string id)
		{
			WriteStreamLineBreak();
			if (m_styleClassPrefix != null)
			{
				WriteStream(m_styleClassPrefix);
			}
			WriteStream(m_dot);
			WriteStream(m_stylePrefixIdBytes);
			WriteStream(id);
			WriteStream(m_openAccol);
		}

		protected virtual RPLReport GetNextPage()
		{
			m_spbProcessing.GetNextPage(out RPLReport rplReport);
			return rplReport;
		}

		protected virtual bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
		{
			if (!IsFragment)
			{
				return textBoxProps.IsToggleParent;
			}
			return false;
		}

		protected virtual bool CanSort(RPLTextBoxPropsDef textBoxDef)
		{
			if (!IsFragment)
			{
				return textBoxDef.CanSort;
			}
			return false;
		}

		protected void RenderSortImage(RPLTextBoxProps textBoxProps)
		{
			if (m_deviceInfo.BrowserMode == BrowserMode.Quirks || m_deviceInfo.IsBrowserIE)
			{
				WriteStream(m_nbsp);
			}
			WriteStream(m_openA);
			WriteStream(m_tabIndex);
			WriteStream(++m_tabIndexNum);
			WriteStream(m_quote);
			RPLFormat.SortOptions sortState = textBoxProps.SortState;
			RenderSortAction(textBoxProps, sortState);
			WriteStream(m_img);
			WriteStream(m_alt);
			switch (sortState)
			{
			case RPLFormat.SortOptions.Ascending:
				WriteAttrEncoded(RenderRes.SortAscAltText);
				break;
			case RPLFormat.SortOptions.Descending:
				WriteAttrEncoded(RenderRes.SortDescAltText);
				break;
			default:
				WriteAttrEncoded(RenderRes.UnsortedAltText);
				break;
			}
			WriteStream(m_quote);
			if (m_browserIE)
			{
				WriteStream(m_imgOnError);
			}
			WriteStream(m_zeroBorder);
			WriteStream(m_src);
			RenderSortImageText(sortState);
			WriteStream(m_closeTag);
			WriteStream(m_closeA);
		}

		protected virtual void RenderSortImageText(RPLFormat.SortOptions sortState)
		{
			RenderInternalImageSrc();
			switch (sortState)
			{
			case RPLFormat.SortOptions.Ascending:
				WriteStream(m_report.GetImageName("sortAsc.gif"));
				break;
			case RPLFormat.SortOptions.Descending:
				WriteStream(m_report.GetImageName("sortDesc.gif"));
				break;
			default:
				WriteStream(m_report.GetImageName("unsorted.gif"));
				break;
			}
		}

		internal void RenderOnClickActionScript(string actionType, string actionArg)
		{
			WriteStream(" onclick=\"");
			WriteStream(m_deviceInfo.ActionScript);
			WriteStream("('");
			WriteStream(actionType);
			WriteStream("','");
			WriteStream(actionArg);
			WriteStream("');return false;\"");
			WriteStream(" onkeypress=\"");
			WriteStream(m_checkForEnterKey);
			WriteStream(m_deviceInfo.ActionScript);
			WriteStream("('");
			WriteStream(actionType);
			WriteStream("','");
			WriteStream(actionArg);
			WriteStream("');return false;}\"");
		}

		protected PaddingSharedInfo GetPaddings(RPLElementStyle style, PaddingSharedInfo paddingInfo)
		{
			int num = 0;
			RPLReportSize rPLReportSize = null;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			PaddingSharedInfo result = paddingInfo;
			if (paddingInfo != null)
			{
				num = paddingInfo.PaddingContext;
				num2 = paddingInfo.PadH;
				num3 = paddingInfo.PadV;
			}
			if ((num & 4) == 0)
			{
				string text = (string)style[17];
				if (text != null)
				{
					rPLReportSize = new RPLReportSize(text);
					flag = true;
					num |= 4;
					num3 += rPLReportSize.ToMillimeters();
				}
			}
			if ((num & 8) == 0)
			{
				flag = true;
				string text2 = (string)style[18];
				if (text2 != null)
				{
					rPLReportSize = new RPLReportSize(text2);
					num |= 8;
					num3 += rPLReportSize.ToMillimeters();
				}
			}
			if ((num & 1) == 0)
			{
				flag = true;
				string text3 = (string)style[15];
				if (text3 != null)
				{
					rPLReportSize = new RPLReportSize(text3);
					num |= 1;
					num2 += rPLReportSize.ToMillimeters();
				}
			}
			if ((num & 2) == 0)
			{
				flag = true;
				string text4 = (string)style[16];
				if (text4 != null)
				{
					rPLReportSize = new RPLReportSize(text4);
					num |= 2;
					num2 += rPLReportSize.ToMillimeters();
				}
			}
			if (flag)
			{
				result = new PaddingSharedInfo(num, num2, num3);
			}
			return result;
		}

		protected bool NeedReportItemId(RPLElement repItem, RPLElementProps props)
		{
			if (m_pageSection != 0)
			{
				return false;
			}
			bool flag = m_linkToChildStack.Count > 0 && props.Definition.ID.Equals(m_linkToChildStack.Peek());
			if (flag)
			{
				m_linkToChildStack.Pop();
			}
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			string bookmark = rPLItemProps.Bookmark;
			if (bookmark == null)
			{
				bookmark = rPLItemPropsDef.Bookmark;
			}
			string label = rPLItemProps.Label;
			if (label == null)
			{
				label = rPLItemPropsDef.Label;
			}
			return bookmark != null || label != null || flag;
		}

		protected void RenderHtmlBody()
		{
			int num = 0;
			m_isBody = true;
			m_hasOnePage = (m_spbProcessing.Done || m_pageNum != 0);
			RenderPageStart(firstPage: true, m_spbProcessing.Done, m_pageContent.PageLayout.Style);
			m_pageSection = PageSection.Body;
			bool flag = m_rplReport != null;
			while (flag)
			{
				bool flag2 = m_pageContent.ReportSectionSizes.Length > 1 || m_rplReportSection.Header != null || m_rplReportSection.Footer != null;
				if (flag2)
				{
					WriteStream(m_openTable);
					WriteStream(m_closeBracket);
				}
				while (m_rplReportSection != null)
				{
					num = 0;
					RPLItemMeasurement header = m_rplReportSection.Header;
					RPLItemMeasurement footer = m_rplReportSection.Footer;
					bool flag3 = header != null || footer != null;
					StyleContext styleContext = new StyleContext();
					RPLItemMeasurement rPLItemMeasurement = m_rplReportSection.Columns[0];
					RPLBody rPLBody = rPLItemMeasurement.Element as RPLBody;
					RPLItemProps rPLItemProps = rPLBody.ElementProps as RPLItemProps;
					RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
					if (flag2)
					{
						if (header != null)
						{
							m_pageSection = PageSection.PageHeader;
							m_isBody = false;
							RenderPageHeaderFooter(header);
							m_isBody = true;
						}
						WriteStream(m_firstTD);
						styleContext.StyleOnCell = true;
						RenderReportItemStyle(rPLBody, rPLItemProps, rPLItemPropsDef, null, styleContext, ref num, rPLItemPropsDef.ID + "c");
						styleContext.StyleOnCell = false;
						WriteStream(m_closeBracket);
					}
					m_pageSection = PageSection.Body;
					m_isBody = true;
					RPLItemMeasurement rPLItemMeasurement2 = new RPLItemMeasurement();
					rPLItemMeasurement2.Width = m_pageContent.MaxSectionWidth;
					rPLItemMeasurement2.Height = m_rplReportSection.BodyArea.Height;
					RenderRectangle(rPLBody, rPLItemProps, rPLItemPropsDef, rPLItemMeasurement2, ref num, renderId: false, styleContext);
					if (flag2)
					{
						WriteStream(m_closeTD);
						WriteStream(m_closeTR);
						if (footer != null)
						{
							m_pageSection = PageSection.PageFooter;
							m_isBody = false;
							RenderPageHeaderFooter(footer);
							m_isBody = true;
						}
					}
					m_rplReportSection = m_pageContent.GetNextReportSection();
					rPLItemMeasurement.Element = null;
				}
				if (flag2)
				{
					WriteStream(m_closeTable);
				}
				RenderPageEnd();
				if (m_pageNum == 0)
				{
					if (!m_spbProcessing.Done)
					{
						if (m_rplReport != null)
						{
							m_rplReport.Release();
						}
						RPLReport rPLReport = null;
						rPLReport = GetNextPage();
						m_pageContent = rPLReport.RPLPaginatedPages[0];
						m_rplReportSection = m_pageContent.GetNextReportSection();
						m_rplReport = rPLReport;
						WriteStream(m_pageBreakDelimiter);
						RenderPageStart(firstPage: false, m_spbProcessing.Done, m_pageContent.PageLayout.Style);
						num = 0;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (m_rplReport != null)
			{
				m_rplReport.Release();
			}
		}

		protected abstract void WriteScrollbars();

		protected abstract void WriteFixedHeaderOnScrollScript();

		protected abstract void WriteFixedHeaderPropertyChangeScript();

		protected virtual void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
		{
			WriteStream(m_openDiv);
			WriteStream(m_ltrDir);
			RenderPageStartDimensionStyles(lastPage);
			if (firstPage)
			{
				RenderReportItemId("oReportDiv");
			}
			bool flag = m_hasOnePage && m_deviceInfo.AllowScript && m_deviceInfo.HTMLFragment;
			if (flag)
			{
				WriteFixedHeaderOnScrollScript();
			}
			if (m_pageHasStyle)
			{
				WriteStream(m_closeBracket);
				WriteStream(m_openDiv);
				OpenStyle();
				if (FillPageHeight)
				{
					WriteStream(m_styleHeight);
					WriteStream(m_percent);
					WriteStream(m_semiColon);
				}
				WriteStream(m_styleWidth);
				WriteStream(m_percent);
				WriteStream(m_semiColon);
				RenderPageStyle(pageStyle);
				CloseStyle(renderQuote: true);
			}
			WriteStream(m_closeBracket);
			WriteStream(m_openTable);
			WriteStream(m_closeBracket);
			WriteStream(m_firstTD);
			if (firstPage)
			{
				RenderReportItemId("oReportCell");
			}
			RenderZoom();
			if (flag)
			{
				WriteFixedHeaderPropertyChangeScript();
			}
			WriteStream(m_closeBracket);
		}

		protected virtual void RenderPageStartDimensionStyles(bool lastPage)
		{
			if (m_pageNum != 0 || lastPage)
			{
				WriteStream(m_openStyle);
				WriteScrollbars();
				if (m_deviceInfo.IsBrowserIE)
				{
					WriteStream(m_styleHeight);
					WriteStream(m_percent);
					WriteStream(m_semiColon);
				}
				WriteStream(m_styleWidth);
				WriteStream(m_percent);
				WriteStream(m_semiColon);
				WriteStream("direction:ltr");
				WriteStream(m_quote);
			}
			else
			{
				OpenStyle();
				WriteStream("direction:ltr");
				CloseStyle(renderQuote: true);
			}
		}

		private void RenderPageStyle(RPLElementStyle style)
		{
			int borderContext = 0;
			if (m_useInlineStyle)
			{
				OpenStyle();
				RenderBackgroundStyleProps(style);
				RenderHtmlBorders(style, ref borderContext, 0, renderPadding: true, isNonShared: true, null);
				CloseStyle(renderQuote: true);
				return;
			}
			RPLStyleProps sharedProperties = style.SharedProperties;
			RPLStyleProps nonSharedProperties = style.NonSharedProperties;
			if (sharedProperties != null && sharedProperties.Count > 0)
			{
				CloseStyle(renderQuote: true);
				string text = "p";
				byte[] array = (byte[])m_usedStyles[text];
				if (array == null)
				{
					array = m_encoding.GetBytes(text);
					m_usedStyles.Add(text, array);
					if (m_onlyVisibleStyles)
					{
						Stream mainStream = m_mainStream;
						m_mainStream = m_styleStream;
						RenderOpenStyle(text);
						RenderBackgroundStyleProps(sharedProperties);
						RenderHtmlBorders(sharedProperties, ref borderContext, 0, renderPadding: true, isNonShared: true, null);
						WriteStream(m_closeAccol);
						m_mainStream = mainStream;
					}
				}
				WriteClassStyle(array, close: true);
			}
			if (nonSharedProperties != null && nonSharedProperties.Count > 0)
			{
				OpenStyle();
				borderContext = 0;
				RenderHtmlBorders(nonSharedProperties, ref borderContext, 0, renderPadding: true, isNonShared: true, sharedProperties);
				RenderBackgroundStyleProps(nonSharedProperties);
				CloseStyle(renderQuote: true);
			}
		}

		protected void OpenStyle()
		{
			if (!m_isStyleOpen)
			{
				m_isStyleOpen = true;
				WriteStream(m_openStyle);
			}
		}

		protected void CloseStyle(bool renderQuote)
		{
			if (m_isStyleOpen && renderQuote)
			{
				WriteStream(m_quote);
			}
			m_isStyleOpen = false;
		}

		public void WriteClassName(byte[] className, byte[] classNameIfNoPrefix)
		{
			if (m_deviceInfo.HtmlPrefixId.Length > 0 || classNameIfNoPrefix == null)
			{
				WriteStream(m_classStyle);
				WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
				WriteStream(className);
				WriteStream(m_quote);
			}
			else
			{
				WriteStream(classNameIfNoPrefix);
			}
		}

		protected virtual void WriteClassStyle(byte[] styleBytes, bool close)
		{
			WriteStream(m_classStyle);
			WriteStream(m_stylePrefixIdBytes);
			WriteStream(styleBytes);
			if (close)
			{
				WriteStream(m_quote);
			}
		}

		protected void RenderBackgroundStyleProps(IRPLStyle style)
		{
			object obj = style[34];
			if (obj != null)
			{
				WriteStream(m_backgroundColor);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = style[33];
			if (obj != null)
			{
				WriteStream(m_backgroundImage);
				RenderImageUrl(useSessionId: true, (RPLImageData)obj);
				WriteStream(m_closeBrace);
				WriteStream(m_semiColon);
			}
			obj = style[35];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BackgroundRepeatTypes)obj);
				WriteStream(m_backgroundRepeat);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
		}

		protected virtual void RenderPageEnd()
		{
			if (m_deviceInfo.ExpandContent)
			{
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
			}
			else
			{
				WriteStream(m_closeTD);
				WriteStream(m_openTD);
				WriteStream(m_inlineWidth);
				WriteStream(m_percent);
				WriteStream(m_quote);
				WriteStream(m_inlineHeight);
				WriteStream("0");
				WriteStream(m_closeQuote);
				WriteStream(m_lastTD);
				WriteStream(m_firstTD);
				WriteStream(m_inlineWidth);
				if (m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(m_percent);
				}
				else
				{
					WriteStream("0");
				}
				WriteStream(m_quote);
				WriteStream(m_inlineHeight);
				WriteStream(m_percent);
				WriteStream(m_closeQuote);
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
			}
			if (m_pageHasStyle)
			{
				WriteStream(m_closeDiv);
			}
			WriteStream(m_closeDiv);
		}

		public virtual void WriteStream(string theString)
		{
			if (theString.Length != 0)
			{
				byte[] array = null;
				array = m_encoding.GetBytes(theString);
				m_mainStream.Write(array, 0, array.Length);
			}
		}

		internal void WriteStream(object theString)
		{
			if (theString != null)
			{
				WriteStream(theString.ToString());
			}
		}

		public virtual void WriteStream(byte[] theBytes)
		{
			m_mainStream.Write(theBytes, 0, theBytes.Length);
		}

		protected void WriteStreamCR(string theString)
		{
			WriteStream(theString);
		}

		protected void WriteStreamCR(byte[] theBytes)
		{
			WriteStream(theBytes);
		}

		protected void WriteStreamEncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected void WriteAttrEncoded(byte[] attributeName, string theString)
		{
			WriteAttribute(attributeName, m_encoding.GetBytes(HttpUtility.HtmlAttributeEncode(theString)));
		}

		protected virtual void WriteAttribute(byte[] attributeName, byte[] value)
		{
			WriteStream(attributeName);
			WriteStream(value);
			WriteStream(m_quote);
		}

		protected void WriteAttrEncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlAttributeEncode(theString));
		}

		protected void WriteStreamCREncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected virtual void WriteStreamLineBreak()
		{
		}

		protected void WriteRSStream(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			WriteStream(m_mm);
		}

		protected void WriteRSStreamCR(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			WriteStreamCR(m_mm);
		}

		protected void WriteDStream(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
		}

		private void WriteIdToSecondaryStream(Stream secondaryStream, string tagId)
		{
			Stream mainStream = m_mainStream;
			m_mainStream = secondaryStream;
			WriteReportItemId(tagId);
			WriteStream(',');
			m_mainStream = mainStream;
		}

		internal static void QuoteString(StringBuilder output, string input)
		{
			if (output == null || input == null || input.Length == 0)
			{
				return;
			}
			int i = output.Length;
			output.Append(input);
			for (; i < output.Length; i++)
			{
				if (output[i] == '\\' || output[i] == '"' || output[i] == '\'')
				{
					output.Insert(i, '\\');
					i++;
				}
			}
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
		{
			return RenderSharedStyle(reportItem, props, definition, sharedStyle, null, measurement, id, styleContext, ref borderContext);
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLStyleProps nonSharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
		{
			Stream mainStream = m_mainStream;
			m_mainStream = m_styleStream;
			RenderOpenStyle(id);
			byte omitBordersState = styleContext.OmitBordersState;
			styleContext.OmitBordersState = 0;
			RenderStyleProps(reportItem, props, definition, measurement, sharedStyle, nonSharedStyle, styleContext, ref borderContext, isNonSharedStyles: false);
			styleContext.OmitBordersState = omitBordersState;
			WriteStream(m_closeAccol);
			m_mainStream = mainStream;
			byte[] bytes = m_encoding.GetBytes(id);
			m_usedStyles.Add(id, bytes);
			return bytes;
		}

		protected byte[] RenderSharedStyle(ElementStyleWriter styleWriter, RPLStyleProps sharedStyle, StyleContext styleContext, string id)
		{
			if (sharedStyle == null || id == null)
			{
				return null;
			}
			Stream mainStream = m_mainStream;
			m_mainStream = m_styleStream;
			RenderOpenStyle(id);
			byte omitBordersState = styleContext.OmitBordersState;
			styleContext.OmitBordersState = 0;
			styleWriter.WriteStyles(StyleWriterMode.Shared, sharedStyle);
			styleContext.OmitBordersState = omitBordersState;
			WriteStream(m_closeAccol);
			m_mainStream = mainStream;
			byte[] bytes = m_encoding.GetBytes(id);
			m_usedStyles.Add(id, bytes);
			return bytes;
		}

		protected void RenderMeasurementStyle(float height, float width)
		{
			RenderMeasurementStyle(height, width, renderMin: false);
		}

		protected void RenderMeasurementStyle(float height, float width, bool renderMin)
		{
			RenderMeasurementHeight(height, renderMin);
			RenderMeasurementWidth(width, renderMinWidth: true);
		}

		protected void RenderMeasurementHeight(float height, bool renderMin)
		{
			if (renderMin)
			{
				WriteStream(m_styleMinHeight);
			}
			else
			{
				WriteStream(m_styleHeight);
			}
			WriteRSStream(height);
			WriteStream(m_semiColon);
		}

		protected void RenderMeasurementMinHeight(float height)
		{
			WriteStream(m_styleMinHeight);
			WriteRSStream(height);
			WriteStream(m_semiColon);
		}

		protected void RenderMeasurementWidth(float width, bool renderMinWidth)
		{
			WriteStream(m_styleWidth);
			WriteRSStream(width);
			WriteStream(m_semiColon);
			if (renderMinWidth)
			{
				RenderMeasurementMinWidth(width);
			}
		}

		protected void RenderMeasurementMinWidth(float minWidth)
		{
			WriteStream(m_styleMinWidth);
			WriteRSStream(minWidth);
			WriteStream(m_semiColon);
		}

		protected void RenderMeasurementHeight(float height)
		{
			RenderMeasurementHeight(height, renderMin: false);
		}

		protected void RenderMeasurementWidth(float width)
		{
			RenderMeasurementWidth(width, renderMinWidth: false);
		}

		private bool ReportPageHasBorder(IRPLStyle style, string backgroundColor)
		{
			bool flag = ReportPageBorder(style, Border.All, backgroundColor);
			if (!flag)
			{
				flag = ReportPageBorder(style, Border.Left, backgroundColor);
				if (!flag)
				{
					flag = ReportPageBorder(style, Border.Right, backgroundColor);
					if (!flag)
					{
						flag = ReportPageBorder(style, Border.Bottom, backgroundColor);
						if (!flag)
						{
							flag = ReportPageBorder(style, Border.Top, backgroundColor);
						}
					}
				}
			}
			return flag;
		}

		protected virtual void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
		{
			MemoryStream imageStream = dynamicImageProps.DynamicImageContent as MemoryStream;
			if (imageStream != null)
			{
				WriteStream("data:image/png;base64," + Convert.ToBase64String(imageStream.ToArray()));
				return;
			}

			string text = null;
			string streamName = dynamicImageProps.StreamName;
			if (streamName != null)
			{
				text = m_report.GetStreamUrl(useSessionId: true, streamName);
			}
			if (text != null)
			{
				WriteStream(text);
			}
		}

		protected void RenderHtmlBorders(IRPLStyle styleProps, ref int borderContext, byte omitBordersState, bool renderPadding, bool isNonShared, IRPLStyle sharedStyleProps)
		{
			if (renderPadding)
			{
				RenderPaddingStyle(styleProps);
			}
			if (styleProps == null || borderContext == 15)
			{
				return;
			}
			object obj = styleProps[10];
			object obj2 = styleProps[5];
			object obj3 = styleProps[0];
			IRPLStyle iRPLStyle = styleProps;
			if (isNonShared && sharedStyleProps != null && !OnlyGeneralBorder(sharedStyleProps) && !OnlyGeneralBorder(styleProps))
			{
				iRPLStyle = new RPLElementStyle(styleProps as RPLStyleProps, sharedStyleProps as RPLStyleProps);
			}
			if (borderContext != 0 || omitBordersState != 0 || !OnlyGeneralBorder(iRPLStyle))
			{
				if (obj2 == null || (RPLFormat.BorderStyles)obj2 == RPLFormat.BorderStyles.None)
				{
					RenderBorderStyle(obj, obj2, obj3, Border.All);
				}
				if ((borderContext & 8) == 0 && (omitBordersState & 2) == 0 && RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Bottom))
				{
					borderContext |= 8;
				}
				if ((borderContext & 1) == 0 && (omitBordersState & 4) == 0 && RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Left))
				{
					borderContext |= 1;
				}
				if ((borderContext & 2) == 0 && (omitBordersState & 8) == 0 && RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Right))
				{
					borderContext |= 2;
				}
				if ((borderContext & 4) == 0 && (omitBordersState & 1) == 0 && RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Top))
				{
					borderContext |= 4;
				}
			}
			else
			{
				if (obj2 != null && (RPLFormat.BorderStyles)obj2 != 0)
				{
					borderContext = 15;
				}
				RenderBorderStyle(obj, obj2, obj3, Border.All);
			}
		}

		protected void RenderPaddingStyle(IRPLStyle styleProps)
		{
			if (styleProps != null)
			{
				object obj = styleProps[15];
				if (obj != null)
				{
					WriteStream(m_paddingLeft);
					WriteStream(obj);
					WriteStream(m_semiColon);
				}
				obj = styleProps[17];
				if (obj != null)
				{
					WriteStream(m_paddingTop);
					WriteStream(obj);
					WriteStream(m_semiColon);
				}
				obj = styleProps[16];
				if (obj != null)
				{
					WriteStream(m_paddingRight);
					WriteStream(obj);
					WriteStream(m_semiColon);
				}
				obj = styleProps[18];
				if (obj != null)
				{
					WriteStream(m_paddingBottom);
					WriteStream(obj);
					WriteStream(m_semiColon);
				}
			}
		}

		protected void RenderMultiLineText(string text)
		{
			if (text == null)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			int length = text.Length;
			string text2 = null;
			for (int i = 0; i < length; i++)
			{
				switch (text[i])
				{
				case '\r':
					text2 = text.Substring(num2, num - num2);
					WriteStreamEncoded(text2);
					num2 = num + 1;
					break;
				case '\n':
					text2 = text.Substring(num2, num - num2);
					if (!string.IsNullOrEmpty(text2.Trim()))
					{
						WriteStreamEncoded(text2);
					}
					WriteStreamCR(m_br);
					num2 = num + 1;
					break;
				}
				num++;
			}
			if (num2 == 0)
			{
				WriteStreamEncoded(text);
			}
			else
			{
				WriteStreamEncoded(text.Substring(num2, num - num2));
			}
		}

		protected bool IsLineSlanted(RPLItemMeasurement measurement)
		{
			if (measurement == null)
			{
				return false;
			}
			if (measurement.Width != 0f && measurement.Height != 0f)
			{
				return true;
			}
			return false;
		}

		protected void RenderCellItem(PageTableCell currCell, int borderContext, bool layoutExpand)
		{
			bool flag = false;
			RPLItemMeasurement rPLItemMeasurement = null;
			rPLItemMeasurement = currCell.Measurement;
			RPLItem element = rPLItemMeasurement.Element;
			if (element == null)
			{
				return;
			}
			RPLItemProps rPLItemProps = element.ElementProps as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			flag = NeedReportItemId(rPLItemMeasurement.Element, rPLItemProps);
			bool flag2 = false;
			if (rPLItemProps is RPLImageProps)
			{
				RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)rPLItemPropsDef;
				if (rPLImagePropsDef.Sizing == RPLFormat.Sizings.FitProportional)
				{
					flag2 = true;
				}
			}
			if (!flag2 && currCell.ConsumedByEmptyWhiteSpace)
			{
				if (rPLItemProps is RPLImageProps)
				{
					RPLImageProps rPLImageProps = (RPLImageProps)rPLItemProps;
					RPLImagePropsDef rPLImagePropsDef2 = (RPLImagePropsDef)rPLItemProps.Definition;
					if (rPLImageProps != null && !rPLImageProps.Image.ImageConsolidationOffsets.IsEmpty)
					{
						flag2 = true;
					}
				}
				if (!flag2 && rPLItemProps is RPLDynamicImageProps)
				{
					RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)rPLItemProps;
					if (rPLDynamicImageProps != null && !rPLDynamicImageProps.ImageConsolidationOffsets.IsEmpty)
					{
						flag2 = true;
					}
				}
			}
			if (flag2)
			{
				WriteStream(m_openDiv);
				OpenStyle();
				if (currCell.DXValue > rPLItemMeasurement.Width)
				{
					RenderMeasurementWidth(rPLItemMeasurement.Width);
				}
				if (currCell.DYValue > rPLItemMeasurement.Height)
				{
					RenderMeasurementHeight(rPLItemMeasurement.Height);
				}
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
			}
			RenderReportItem(element, rPLItemProps, rPLItemPropsDef, rPLItemMeasurement, new StyleContext(), borderContext, flag);
			if (flag2)
			{
				WriteStream(m_closeDiv);
			}
			rPLItemMeasurement.Element = null;
		}

		protected virtual void RenderBlankImage()
		{
			WriteStream(m_img);
			if (m_browserIE)
			{
				WriteStream(m_imgOnError);
			}
			WriteStream(m_src);
			RenderInternalImageSrc();
			WriteStream(m_report.GetImageName("Blank.gif"));
			WriteStream(m_quote);
			WriteStream(m_alt);
			WriteAttrEncoded(RenderRes.BlankAltText);
			WriteStream(m_closeTag);
		}

		protected virtual void RenderImageUrl(bool useSessionId, RPLImageData image)
		{
			long imageDataOffset = image.ImageDataOffset;
			byte[] blob;
			if (image.ImageDataOffset >= 0)
			{
				using var stream = new MemoryStream();
				m_rplReport.GetImage(imageDataOffset, stream);
				blob = stream.ToArray();
			}
			else if (image.ImageData != null)
			{
				blob = image.ImageData;
			}
			else return;

			WriteStream("data:" + image.ImageMimeType + ";base64," + Convert.ToBase64String(blob));
		}

		protected virtual void RenderReportItemId(string repItemId)
		{
			WriteStream(m_id);
			WriteReportItemId(repItemId);
			WriteStream(m_quote);
		}

		private void WriteReportItemId(string repItemId)
		{
			WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
			WriteStream(repItemId);
		}

		protected void RenderTextBox(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId)
		{
			string text = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			RPLElementStyle style = textBoxProps.Style;
			bool flag4 = CanSort(textBoxPropsDef);
			bool flag5 = NeedSharedToggleParent(textBoxProps);
			bool flag6 = false;
			bool isSimple = textBoxPropsDef.IsSimple;
			bool flag7 = !isSimple && flag5;
			bool flag8 = flag4 || flag7;
			bool flag9 = IsDirectionRTL(style);
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			bool flag10 = IsWritingModeVertical(style);
			bool flag11 = flag10 && m_deviceInfo.IsBrowserIE;
			bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
			if (isSimple)
			{
				text = textBoxProps.Value;
				if (string.IsNullOrEmpty(text))
				{
					text = textBoxPropsDef.Value;
				}
				if (string.IsNullOrEmpty(text) && !(flag4 || flag5))
				{
					flag = true;
				}
			}
			if (textBoxProps.UniqueName == null)
			{
				flag4 = false;
				flag5 = false;
				renderId = false;
			}
			float adjustedWidth = GetAdjustedWidth(measurement, textBoxProps.Style);
			float adjustedHeight = GetAdjustedHeight(measurement, textBoxProps.Style);
			if (flag)
			{
				styleContext.EmptyTextBox = true;
				WriteStream(m_openTable);
				RenderReportLanguage();
				WriteStream(m_closeBracket);
				WriteStream(m_firstTD);
				if (m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(m_openDiv);
				}
				OpenStyle();
				float width = measurement.Width;
				float height = measurement.Height;
				if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
				{
					width = adjustedWidth;
					height = adjustedHeight;
				}
				RenderMeasurementWidth(width, renderMinWidth: false);
				RenderMeasurementMinWidth(adjustedWidth);
				if (!textBoxPropsDef.CanShrink)
				{
					RenderMeasurementHeight(height);
				}
			}
			else
			{
				if (flag11)
				{
					WriteStream(m_openDiv);
					OpenStyle();
					RenderDirectionStyles(textBox, textBoxProps, textBoxPropsDef, null, textBoxProps.Style, nonSharedStyle, isNonSharedStyles: false, styleContext);
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
					{
						RenderMeasurementHeight(adjustedHeight);
						RenderHtmlBorders(textBoxProps.Style, ref borderContext, styleContext.OmitBordersState, renderPadding: true, isNonShared: true, null);
						styleContext.NoBorders = true;
					}
					WriteStream("display: inline;");
					bool flag12 = false;
					if (m_deviceInfo.BrowserMode == BrowserMode.Standards)
					{
						RenderMeasurementHeight(measurement.Height);
						flag12 = true;
					}
					CloseStyle(renderQuote: true);
					if (flag12 && m_deviceInfo.AllowScript)
					{
						if (!m_needsFitVertTextScript)
						{
							CreateFitVertTextIdsStream();
						}
						WriteIdToSecondaryStream(m_fitVertTextIdsStream, textBoxProps.UniqueName + "_fvt");
						RenderReportItemId(textBoxProps.UniqueName + "_fvt");
					}
					WriteStream(m_closeBracket);
				}
				object obj = style[26];
				if (textBoxPropsDef.CanGrow)
				{
					WriteStream(m_openTable);
					RenderReportLanguage();
					OpenStyle();
					if (flag11)
					{
						if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							RenderMeasurementWidth(adjustedWidth, renderMinWidth: false);
							if (!textBoxPropsDef.CanShrink)
							{
								RenderMeasurementHeight(adjustedHeight);
							}
						}
						else
						{
							RenderMeasurementWidth(measurement.Width, renderMinWidth: true);
						}
					}
					if (isSimple && (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim())))
					{
						WriteStream(m_borderCollapse);
					}
					CloseStyle(renderQuote: true);
					WriteStream(m_closeBracket);
					WriteStream(m_firstTD);
					OpenStyle();
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
					{
						RenderMeasurementWidth(adjustedWidth, renderMinWidth: false);
					}
					else
					{
						RenderMeasurementWidth(measurement.Width, renderMinWidth: false);
					}
					RenderMeasurementMinWidth(adjustedWidth);
					if (!textBoxPropsDef.CanShrink)
					{
						if (m_deviceInfo.IsBrowserIE6Or7StandardsMode || (m_deviceInfo.IsBrowserSafari && m_deviceInfo.BrowserMode != BrowserMode.Quirks))
						{
							if (!flag11)
							{
								RenderMeasurementHeight(adjustedHeight);
							}
						}
						else
						{
							RenderMeasurementHeight(measurement.Height);
						}
					}
					styleContext.RenderMeasurements = false;
					if (flag8)
					{
						styleContext.StyleOnCell = true;
						RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "p");
						styleContext.StyleOnCell = false;
						styleContext.NoBorders = true;
					}
					if (textBoxPropsDef.CanShrink)
					{
						if (flag10 || (flag5 && flag9))
						{
							flag2 = true;
						}
						if (!flag2 && obj != null && !styleContext.IgnoreVerticalAlign)
						{
							obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
							WriteStream(m_verticalAlign);
							WriteStream(obj);
							WriteStream(m_semiColon);
						}
						CloseStyle(renderQuote: true);
						WriteStreamCR(m_closeBracket);
						if (flag2)
						{
							WriteStream(m_openTable);
							WriteStream(m_inlineWidth);
							WriteStream(m_percent);
							WriteStream(m_quote);
							WriteStream(m_closeBracket);
							WriteStream(m_firstTD);
						}
						else
						{
							WriteStream(m_openDiv);
							if (!flag8)
							{
								styleContext.IgnoreVerticalAlign = true;
							}
						}
					}
				}
				else
				{
					WriteStream(m_openDiv);
					styleContext.IgnoreVerticalAlign = true;
					if (!m_deviceInfo.IsBrowserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || (obj != null && (RPLFormat.VerticalAlignments)obj != 0) || m_deviceInfo.OutlookCompat)
					{
						if (!flag8)
						{
							bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
							bool noBorders = styleContext.NoBorders;
							styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
							int borderContext2 = 0;
							if (textBoxPropsDef.CanShrink)
							{
								styleContext.NoBorders = true;
							}
							RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext2, textBoxPropsDef.ID + "v");
							styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
							measurement = null;
							if (textBoxPropsDef.CanShrink)
							{
								styleContext.NoBorders = noBorders;
							}
							else
							{
								styleContext.NoBorders = true;
							}
						}
						WriteStreamCR(m_closeBracket);
						styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
						if (obj != null && (RPLFormat.VerticalAlignments)obj != 0)
						{
							WriteStream(m_openTable);
							if (!flag4 || flag10)
							{
								WriteStream(m_inlineWidth);
								WriteStream(m_percent);
								WriteStream(m_quote);
							}
							if (!textBoxPropsDef.CanShrink)
							{
								WriteStream(m_inlineHeight);
								WriteStream(m_percent);
								WriteStream(m_quote);
							}
							WriteStream(m_zeroBorder);
							WriteStream(m_closeBracket);
							WriteStream(m_firstTD);
							flag2 = true;
						}
						else
						{
							WriteStream(m_openDiv);
							flag3 = true;
						}
					}
					if (flag8)
					{
						OpenStyle();
						if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
						{
							RenderMeasurementWidth(adjustedWidth, renderMinWidth: false);
						}
						else
						{
							RenderMeasurementWidth(measurement.Width, renderMinWidth: false);
						}
						RenderMeasurementMinWidth(adjustedWidth);
						WriteStream(m_semiColon);
					}
					if (textBoxPropsDef.CanShrink)
					{
						bool noBorders2 = styleContext.NoBorders;
						styleContext.NoBorders = true;
						RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "s");
						CloseStyle(renderQuote: true);
						WriteStreamCR(m_closeBracket);
						WriteStream(m_openDiv);
						styleContext.IgnoreVerticalAlign = true;
						styleContext.NoBorders = noBorders2;
						styleContext.StyleOnCell = true;
					}
					if (flag8)
					{
						RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "p");
						styleContext.StyleOnCell = false;
					}
				}
			}
			if (flag8)
			{
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				CloseStyle(renderQuote: true);
				WriteStreamCR(m_closeBracket);
				WriteStream(m_openTable);
				WriteStream(m_zeroBorder);
				RenderReportLanguage();
				styleContext.RenderMeasurements = false;
				WriteStream(m_closeBracket);
				WriteStream(m_firstTD);
				if (flag10)
				{
					WriteStream(" ROWS='2'");
				}
				RenderAtStart(textBoxProps, style, flag4 && flag9, flag7 && !flag9);
				styleContext.InTablix = true;
			}
			string textBoxClass = GetTextBoxClass(textBoxPropsDef, textBoxProps, nonSharedStyle, textBoxPropsDef.ID);
			RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxClass);
			CloseStyle(renderQuote: true);
			styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
			if (renderId || flag5 || flag4)
			{
				RenderReportItemId(textBoxProps.UniqueName);
			}
			WriteToolTip(textBoxProps);
			if (!flag)
			{
				string language = (string)style[32];
				RenderLanguage(language);
			}
			WriteStreamCR(m_closeBracket);
			if (renderId)
			{
				WriteStream(m_openA);
				WriteStream(m_name);
				WriteStream(textBoxProps.UniqueName);
				WriteStream(m_closeTag);
			}
			if ((!m_deviceInfo.IsBrowserIE || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10)) && isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
			{
				WriteStream(m_openDiv);
				if (measurement != null)
				{
					OpenStyle();
					float num = GetInnerContainerWidth(measurement, textBoxProps.Style);
					if (flag4 && !flag9)
					{
						num -= 4.233333f;
					}
					if (num > 0f)
					{
						WriteStream(m_styleWidth);
						WriteRSStream(num);
						WriteStream(m_semiColon);
					}
					CloseStyle(renderQuote: true);
				}
				WriteStream(m_closeBracket);
			}
			if (flag5 && isSimple)
			{
				RenderToggleImage(textBoxProps);
			}
			RPLAction rPLAction = null;
			if (HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				RenderElementHyperlinkAllTextStyles(textBoxProps.Style, rPLAction, textBoxPropsDef.ID + "a");
				flag6 = true;
				if (flag)
				{
					WriteStream(m_openDiv);
					OpenStyle();
					float num2 = 0f;
					if (measurement != null)
					{
						num2 = measurement.Height;
					}
					if (num2 > 0f)
					{
						num2 = GetInnerContainerHeightSubtractBorders(measurement, textBoxProps.Style);
						if (m_deviceInfo.IsBrowserIE && m_deviceInfo.BrowserMode == BrowserMode.Quirks)
						{
							RenderMeasurementHeight(num2);
						}
						else
						{
							RenderMeasurementMinHeight(num2);
						}
					}
					WriteStream(m_cursorHand);
					WriteStream(m_semiColon);
					CloseStyle(renderQuote: true);
					WriteStream(m_closeBracket);
				}
			}
			RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag5 || flag4, measurement, rPLAction);
			if (flag6)
			{
				if (flag)
				{
					WriteStream(m_closeDiv);
				}
				WriteStream(m_closeA);
			}
			if ((!m_deviceInfo.IsBrowserIE || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10)) && isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
			{
				WriteStream(m_closeDiv);
			}
			if (flag8)
			{
				RenderAtEnd(textBoxProps, style, flag4 && !flag9, flag7 && flag9);
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
			}
			if (flag)
			{
				if (m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(m_closeDiv);
				}
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
				return;
			}
			if (textBoxPropsDef.CanGrow)
			{
				if (textBoxPropsDef.CanShrink)
				{
					if (flag2)
					{
						WriteStream(m_lastTD);
						WriteStream(m_closeTable);
					}
					else
					{
						WriteStream(m_closeDiv);
					}
				}
				WriteStream(m_lastTD);
				WriteStreamCR(m_closeTable);
			}
			else
			{
				if (flag2)
				{
					WriteStream(m_lastTD);
					WriteStream(m_closeTable);
				}
				if (flag3)
				{
					WriteStream(m_closeDiv);
				}
				WriteStreamCR(m_closeDiv);
			}
			if (flag11)
			{
				WriteStream(m_closeDiv);
			}
		}

		private string GetTextBoxClass(RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, RPLStyleProps nonSharedStyle, string defaultClass)
		{
			if (textBoxPropsDef.SharedTypeCode == TypeCode.Object && (nonSharedStyle == null || nonSharedStyle.Count == 0 || nonSharedStyle[25] == null))
			{
				object obj = textBoxProps.Style[25];
				if (obj != null && (RPLFormat.TextAlignments)obj == RPLFormat.TextAlignments.General)
				{
					if (GetTextAlignForType(textBoxProps))
					{
						return defaultClass + "r";
					}
					return defaultClass + "l";
				}
			}
			return defaultClass;
		}

		private void WriteToolTip(RPLElementProps props)
		{
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			string toolTip = rPLItemProps.ToolTip;
			if (toolTip == null)
			{
				toolTip = rPLItemPropsDef.ToolTip;
			}
			if (toolTip != null)
			{
				WriteToolTipAttribute(toolTip);
			}
		}

		private void WriteToolTipAttribute(string tooltip)
		{
			WriteAttrEncoded(m_alt, tooltip);
			WriteAttrEncoded(m_title, tooltip);
		}

		private void WriteOuterConsolidation(System.Drawing.Rectangle consolidationOffsets, RPLFormat.Sizings sizing, string propsUniqueName)
		{
			bool flag = false;
			switch (sizing)
			{
			case RPLFormat.Sizings.Fit:
				WriteStream(" imgConDiv=\"true\"");
				m_emitImageConsolidationScaling = true;
				flag = true;
				break;
			case RPLFormat.Sizings.FitProportional:
				WriteStream(" imgConFitProp=\"true\"");
				break;
			}
			if (m_deviceInfo.AllowScript)
			{
				if (m_imgConImageIdsStream == null)
				{
					CreateImgConImageIdsStream();
				}
				WriteIdToSecondaryStream(m_imgConImageIdsStream, propsUniqueName + "_ici");
				RenderReportItemId(propsUniqueName + "_ici");
			}
			WriteStream(" imgConImage=\"" + sizing.ToString() + "\"");
			if (flag)
			{
				WriteStream(" imgConWidth=\"" + consolidationOffsets.Width + "\"");
				WriteStream(" imgConHeight=\"" + consolidationOffsets.Height + "\"");
			}
			OpenStyle();
			WriteStream(m_styleWidth);
			if (flag)
			{
				WriteStream("1");
			}
			else
			{
				WriteStream(consolidationOffsets.Width);
			}
			WriteStream(m_px);
			WriteStream(m_semiColon);
			WriteStream(m_styleHeight);
			if (flag)
			{
				WriteStream("1");
			}
			else
			{
				WriteStream(consolidationOffsets.Height);
			}
			WriteStream(m_px);
			WriteStream(m_semiColon);
			WriteStream(m_overflowHidden);
			WriteStream(m_semiColon);
			if (m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				WriteStream(m_stylePositionAbsolute);
			}
		}

		private void WriteClippedDiv(System.Drawing.Rectangle clipCoordinates)
		{
			OpenStyle();
			WriteStream(m_styleTop);
			if (clipCoordinates.Top > 0)
			{
				WriteStream("-");
			}
			WriteStream(clipCoordinates.Top);
			WriteStream(m_px);
			WriteStream(m_semiColon);
			WriteStream(m_styleLeft);
			if (clipCoordinates.Left > 0)
			{
				WriteStream("-");
			}
			WriteStream(clipCoordinates.Left);
			WriteStream(m_px);
			WriteStream(m_semiColon);
			WriteStream(m_stylePositionRelative);
			CloseStyle(renderQuote: true);
		}

		protected void RenderNavigationId(string navigationId)
		{
			if (!IsFragment)
			{
				WriteStream(m_openSpan);
				WriteStream(m_id);
				WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
				WriteStream(navigationId);
				WriteStream(m_closeTag);
			}
		}

		protected void RenderTablix(RPLTablix tablix, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId)
		{
			string uniqueName = props.UniqueName;
			TablixFixedHeaderStorage tablixFixedHeaderStorage = new TablixFixedHeaderStorage();
			if (tablix.ColumnWidths == null)
			{
				tablix.ColumnWidths = new float[0];
			}
			if (tablix.RowHeights == null)
			{
				tablix.RowHeights = new float[0];
			}
			bool flag = InitFixedColumnHeaders(tablix, uniqueName, tablixFixedHeaderStorage);
			bool flag2 = InitFixedRowHeaders(tablix, uniqueName, tablixFixedHeaderStorage);
			bool flag3 = tablix.ColumnHeaderRows == 0 && tablix.RowHeaderColumns == 0 && !m_deviceInfo.AccessibleTablix && m_deviceInfo.BrowserMode != BrowserMode.Standards;
			if (flag && flag2)
			{
				tablixFixedHeaderStorage.CornerHeaders = new List<string>();
			}
			WriteStream(m_openTable);
			int columns = (tablix.ColumnHeaderRows > 0 || tablix.RowHeaderColumns > 0 || !flag3) ? (tablix.ColumnWidths.Length + 1) : tablix.ColumnWidths.Length;
			WriteStream(m_cols);
			WriteStream(columns.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
			if (renderId || flag || flag2)
			{
				RenderReportItemId(uniqueName);
			}
			WriteToolTip(tablix.ElementProps);
			WriteStream(m_zeroBorder);
			OpenStyle();
			WriteStream(m_borderCollapse);
			WriteStream(m_semiColon);
			if (m_deviceInfo.OutlookCompat && measurement != null)
			{
				RenderMeasurementWidth(measurement.Width, renderMinWidth: true);
			}
			RenderReportItemStyle(tablix, props, def, measurement, styleContext, ref borderContext, def.ID);
			CloseStyle(renderQuote: true);
			WriteStream(m_closeBracket);
			int colsBeforeRowHeaders = tablix.ColsBeforeRowHeaders;
			RPLTablixRow nextRow = tablix.GetNextRow();
			List<RPLTablixOmittedRow> list = new List<RPLTablixOmittedRow>();
			while (nextRow != null && nextRow is RPLTablixOmittedRow)
			{
				list.Add((RPLTablixOmittedRow)nextRow);
				nextRow = tablix.GetNextRow();
			}
			if (flag3)
			{
				RenderEmptyTopTablixRow(tablix, list, uniqueName, emptyCol: false, tablixFixedHeaderStorage);
				RenderSimpleTablixRows(tablix, uniqueName, nextRow, borderContext, tablixFixedHeaderStorage);
			}
			else
			{
				styleContext = new StyleContext();
				float[] columnWidths = tablix.ColumnWidths;
				float[] rowHeights = tablix.RowHeights;
				int num = columnWidths.Length;
				int numRows = rowHeights.Length;
				RenderEmptyTopTablixRow(tablix, list, uniqueName, emptyCol: true, tablixFixedHeaderStorage);
				bool flag4 = flag;
				int num2 = 0;
				list = new List<RPLTablixOmittedRow>();
				HTMLHeader[] array = null;
				string[] array2 = null;
				OmittedHeaderStack omittedHeaders = null;
				if (m_deviceInfo.AccessibleTablix)
				{
					array = new HTMLHeader[tablix.RowHeaderColumns];
					array2 = new string[num];
					omittedHeaders = new OmittedHeaderStack();
				}
				while (nextRow != null)
				{
					if (nextRow is RPLTablixOmittedRow)
					{
						list.Add((RPLTablixOmittedRow)nextRow);
						nextRow = tablix.GetNextRow();
						continue;
					}
					if (rowHeights[num2] == 0f && num2 > 1 && nextRow.NumCells == 1 && nextRow[0].Element is RPLRectangle)
					{
						RPLRectangle rPLRectangle = (RPLRectangle)nextRow[0].Element;
						if (rPLRectangle.Children == null || rPLRectangle.Children.Length == 0)
						{
							nextRow = tablix.GetNextRow();
							num2++;
							continue;
						}
					}
					WriteStream(m_openTR);
					if (tablix.FixedRow(num2) || flag2 || flag4)
					{
						string text = uniqueName + "r" + num2;
						RenderReportItemId(text);
						if (tablix.FixedRow(num2))
						{
							tablixFixedHeaderStorage.ColumnHeaders.Add(text);
							if (tablixFixedHeaderStorage.CornerHeaders != null)
							{
								tablixFixedHeaderStorage.CornerHeaders.Add(text);
							}
						}
						else if (flag4)
						{
							tablixFixedHeaderStorage.BodyID = text;
							flag4 = false;
						}
						if (flag2)
						{
							tablixFixedHeaderStorage.RowHeaders.Add(text);
						}
					}
					WriteStream(m_valign);
					WriteStream(m_topValue);
					WriteStream(m_quote);
					WriteStream(m_closeBracket);
					RenderEmptyHeightCell(rowHeights[num2], uniqueName, tablix.FixedRow(num2), num2, tablixFixedHeaderStorage);
					int num3 = 0;
					int numCells = nextRow.NumCells;
					int num4 = numCells;
					if (nextRow.BodyStart == -1)
					{
						int[] omittedIndices = new int[list.Count];
						for (int i = num3; i < num4; i++)
						{
							RPLTablixCell rPLTablixCell = nextRow[i];
							RenderColumnHeaderTablixCell(tablix, uniqueName, num, rPLTablixCell.ColIndex, rPLTablixCell.ColSpan, num2, borderContext, rPLTablixCell, styleContext, tablixFixedHeaderStorage, list, omittedIndices);
							if (array2 != null && num2 < tablix.ColumnHeaderRows)
							{
								string text2 = null;
								if (rPLTablixCell is RPLTablixMemberCell)
								{
									text2 = ((RPLTablixMemberCell)rPLTablixCell).UniqueName;
									if (text2 == null && rPLTablixCell.Element != null)
									{
										text2 = rPLTablixCell.Element.ElementProps.UniqueName;
										((RPLTablixMemberCell)rPLTablixCell).UniqueName = text2;
									}
									if (text2 == null)
									{
										continue;
									}
									for (int j = 0; j < rPLTablixCell.ColSpan; j++)
									{
										string text3 = array2[rPLTablixCell.ColIndex + j];
										text3 = ((text3 != null) ? (text3 + " " + HttpUtility.HtmlAttributeEncode(m_deviceInfo.HtmlPrefixId) + text2) : (HttpUtility.HtmlAttributeEncode(m_deviceInfo.HtmlPrefixId) + text2));
										array2[rPLTablixCell.ColIndex + j] = text3;
									}
								}
							}
							nextRow[i] = null;
						}
						list = new List<RPLTablixOmittedRow>();
					}
					else
					{
						if (array != null)
						{
							int headerStart = nextRow.HeaderStart;
							bool flag5 = colsBeforeRowHeaders > 0 && flag2;
							int num5 = 0;
							for (int k = 0; k < array.Length; k++)
							{
								HTMLHeader hTMLHeader = array[k];
								if (array[k] == null)
								{
									hTMLHeader = (array[k] = new HTMLHeader());
								}
								else if (array[k].Span > 1)
								{
									array[k].Span--;
									continue;
								}
								RPLTablixCell rPLTablixCell2 = nextRow[num5 + headerStart];
								hTMLHeader.ID = CalculateRowHeaderId(rPLTablixCell2, tablix.FixedColumns[rPLTablixCell2.ColIndex], uniqueName, num2, k + tablix.ColsBeforeRowHeaders, null, m_deviceInfo.AccessibleTablix, fixedCornerHeader: false);
								hTMLHeader.Span = rPLTablixCell2.RowSpan;
								num5++;
							}
						}
						if (list != null && list.Count > 0)
						{
							for (int l = 0; l < list.Count; l++)
							{
								RenderTablixOmittedRow(columns, list[l]);
							}
							list = null;
						}
						List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
						if (colsBeforeRowHeaders > 0)
						{
							int omittedIndex = 0;
							int headerStart2 = nextRow.HeaderStart;
							int bodyStart = nextRow.BodyStart;
							int m = headerStart2;
							int n = bodyStart;
							int num6 = 0;
							for (; n < num4; n++)
							{
								if (num6 >= colsBeforeRowHeaders)
								{
									break;
								}
								RPLTablixCell rPLTablixCell3 = nextRow[n];
								int colSpan = rPLTablixCell3.ColSpan;
								RenderTablixCell(tablix, fixedHeader: false, uniqueName, num, numRows, num6, colSpan, num2, borderContext, rPLTablixCell3, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								num6 += colSpan;
								nextRow[n] = null;
							}
							num4 = ((bodyStart > headerStart2) ? bodyStart : num4);
							if (m >= 0)
							{
								for (; m < num4; m++)
								{
									RPLTablixCell rPLTablixCell4 = nextRow[m];
									int colSpan2 = rPLTablixCell4.ColSpan;
									RenderTablixCell(tablix, flag2, uniqueName, num, numRows, num6, colSpan2, num2, borderContext, rPLTablixCell4, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
									num6 += colSpan2;
									nextRow[m] = null;
								}
							}
							num3 = n;
							num4 = ((bodyStart < headerStart2) ? headerStart2 : numCells);
							for (int num7 = num3; num7 < num4; num7++)
							{
								RPLTablixCell rPLTablixCell5 = nextRow[num7];
								RenderTablixCell(tablix, fixedHeader: false, uniqueName, num, numRows, rPLTablixCell5.ColIndex, rPLTablixCell5.ColSpan, num2, borderContext, rPLTablixCell5, omittedHeaders2, ref omittedIndex, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num7] = null;
							}
						}
						else
						{
							int omittedIndex2 = 0;
							for (int num8 = num3; num8 < num4; num8++)
							{
								RPLTablixCell rPLTablixCell6 = nextRow[num8];
								int colIndex = rPLTablixCell6.ColIndex;
								RenderTablixCell(tablix, tablix.FixedColumns[colIndex], uniqueName, num, numRows, colIndex, rPLTablixCell6.ColSpan, num2, borderContext, rPLTablixCell6, omittedHeaders2, ref omittedIndex2, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num8] = null;
							}
						}
					}
					WriteStream(m_closeTR);
					nextRow = tablix.GetNextRow();
					num2++;
				}
			}
			WriteStream(m_closeTable);
			if (flag || flag2)
			{
				if (m_fixedHeaders == null)
				{
					m_fixedHeaders = new ArrayList();
				}
				m_fixedHeaders.Add(tablixFixedHeaderStorage);
			}
		}

		private void RenderTablixOmittedRow(int columns, RPLTablixRow currentRow)
		{
			int i = 0;
			List<RPLTablixMemberCell> omittedHeaders;
			for (omittedHeaders = currentRow.OmittedHeaders; i < omittedHeaders.Count && omittedHeaders[i].GroupLabel == null; i++)
			{
			}
			if (i >= omittedHeaders.Count)
			{
				return;
			}
			int num = omittedHeaders[i].ColIndex;
			WriteStream(m_openTR);
			WriteStream(m_zeroHeight);
			WriteStream(m_closeBracket);
			WriteStream(m_openTD);
			WriteStream(m_colSpan);
			WriteStream(num.ToString(CultureInfo.InvariantCulture));
			WriteStream(m_quote);
			WriteStream(m_closeBracket);
			WriteStream(m_closeTD);
			for (; i < omittedHeaders.Count; i++)
			{
				if (omittedHeaders[i].GroupLabel != null)
				{
					WriteStream(m_openTD);
					int colIndex = omittedHeaders[i].ColIndex;
					int num2 = colIndex - num;
					if (num2 > 1)
					{
						WriteStream(m_colSpan);
						WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						WriteStream(m_quote);
						WriteStream(m_closeBracket);
						WriteStream(m_closeTD);
						WriteStream(m_openTD);
					}
					int colSpan = omittedHeaders[i].ColSpan;
					if (colSpan > 1)
					{
						WriteStream(m_colSpan);
						WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
						WriteStream(m_quote);
					}
					RenderReportItemId(omittedHeaders[i].UniqueName);
					WriteStream(m_closeBracket);
					WriteStream(m_closeTD);
					num = colIndex + colSpan;
				}
			}
			if (num < columns)
			{
				WriteStream(m_openTD);
				WriteStream(m_colSpan);
				WriteStream((columns - num).ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
				WriteStream(m_closeBracket);
				WriteStream(m_closeTD);
			}
			WriteStream(m_closeTR);
		}

		protected void RenderSimpleTablixRows(RPLTablix tablix, string tablixID, RPLTablixRow currentRow, int borderContext, TablixFixedHeaderStorage headerStorage)
		{
			int num = 0;
			StyleContext styleContext = new StyleContext();
			float[] rowHeights = tablix.RowHeights;
			int num2 = tablix.ColumnWidths.Length;
			int num3 = rowHeights.Length;
			bool flag = headerStorage.ColumnHeaders != null;
			SharedListLayoutState sharedListLayoutState = SharedListLayoutState.None;
			while (currentRow != null)
			{
				List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
				int omittedIndex = 0;
				if (num2 == 1)
				{
					sharedListLayoutState = SharedListLayoutState.None;
					bool flag2 = tablix.SharedLayoutRow(num);
					bool flag3 = tablix.UseSharedLayoutRow(num);
					bool flag4 = tablix.RowsState.Length > num + 1 && tablix.UseSharedLayoutRow(num + 1);
					if (flag2 && flag4)
					{
						sharedListLayoutState = SharedListLayoutState.Start;
					}
					else if (flag3)
					{
						sharedListLayoutState = ((!flag4) ? SharedListLayoutState.End : SharedListLayoutState.Continue);
					}
				}
				if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.Start)
				{
					if (rowHeights[num] == 0f && num > 1 && currentRow.NumCells == 1 && currentRow[0].Element is RPLRectangle)
					{
						RPLRectangle rPLRectangle = (RPLRectangle)currentRow[0].Element;
						if (rPLRectangle.Children == null || rPLRectangle.Children.Length == 0)
						{
							currentRow = tablix.GetNextRow();
							num++;
							continue;
						}
					}
					WriteStream(m_openTR);
					if (tablix.FixedRow(num) || headerStorage.RowHeaders != null || flag)
					{
						string text = tablixID + "tr" + num;
						RenderReportItemId(text);
						if (tablix.FixedRow(num))
						{
							headerStorage.ColumnHeaders.Add(text);
							if (headerStorage.CornerHeaders != null)
							{
								headerStorage.CornerHeaders.Add(text);
							}
						}
						else if (flag)
						{
							headerStorage.BodyID = text;
							flag = false;
						}
						if (headerStorage.RowHeaders != null)
						{
							headerStorage.RowHeaders.Add(text);
						}
					}
					WriteStream(m_valign);
					WriteStream(m_topValue);
					WriteStream(m_quote);
					WriteStream(m_closeBracket);
				}
				int numCells = currentRow.NumCells;
				bool firstRow = num == 0;
				bool lastRow = num == num3 - 1;
				RPLTablixCell rPLTablixCell = currentRow[0];
				currentRow[0] = null;
				if (sharedListLayoutState != 0)
				{
					RenderListReportItem(tablix, rPLTablixCell, omittedHeaders, borderContext, styleContext, firstRow, lastRow, sharedListLayoutState, rPLTablixCell.Element);
				}
				else
				{
					RenderSimpleTablixCellWithHeight(rowHeights[num], tablix, tablixID, num2, num, borderContext, rPLTablixCell, omittedHeaders, ref omittedIndex, styleContext, firstRow, lastRow, headerStorage);
				}
				int i;
				for (i = 1; i < numCells - 1; i++)
				{
					rPLTablixCell = currentRow[i];
					RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref omittedIndex, lastCol: false, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (numCells > 1)
				{
					rPLTablixCell = currentRow[i];
					RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref omittedIndex, lastCol: true, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.End)
				{
					WriteStream(m_closeTR);
				}
				currentRow = tablix.GetNextRow();
				num++;
			}
		}

		private void RenderSimpleTablixCellWithHeight(float height, RPLTablix tablix, string tablixID, int numCols, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			int colIndex = cell.ColIndex;
			int colSpan = cell.ColSpan;
			bool lastCol = colIndex + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, colSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
			WriteStream(m_openTD);
			RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (colSpan > 1)
			{
				WriteStream(m_colSpan);
				WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
			}
			OpenStyle();
			WriteStream(m_styleHeight);
			WriteDStream(height);
			WriteStream(m_mm);
			RPLElement element = cell.Element;
			if (element != null)
			{
				WriteStream(m_semiColon);
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, firstCol: true, lastCol, firstRow, lastRow, element, ref borderContext);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				RenderTablixReportItem(tablix, tablixContext, cell, styleContext, firstCol: true, lastCol, firstRow, lastRow, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					WriteStream(m_displayNone);
				}
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				WriteStream(m_nbsp);
			}
			WriteStream(m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixReportItemStyle(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			styleContext.OmitBordersState = cell.ElementState;
			if (!(cellItem is RPLLine))
			{
				styleContext.StyleOnCell = true;
				borderContext = GetNewContext(tablixContext, firstCol, lastCol, firstRow, lastRow);
				if (rPLTextBox != null)
				{
					bool ignorePadding = styleContext.IgnorePadding;
					styleContext.IgnorePadding = true;
					RPLItemMeasurement rPLItemMeasurement = null;
					if (m_deviceInfo.OutlookCompat || !m_deviceInfo.IsBrowserIE)
					{
						rPLItemMeasurement = new RPLItemMeasurement();
						rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					}
					styleContext.EmptyTextBox = (rPLTextBoxPropsDef.IsSimple && string.IsNullOrEmpty(rPLTextBoxProps.Value) && string.IsNullOrEmpty(rPLTextBoxPropsDef.Value) && !NeedSharedToggleParent(rPLTextBoxProps) && !CanSort(rPLTextBoxPropsDef));
					string textBoxClass = GetTextBoxClass(rPLTextBoxPropsDef, rPLTextBoxProps, rPLTextBoxProps.NonSharedStyle, definition.ID + "c");
					bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
					if (IsWritingModeVertical(rPLTextBoxProps.Style) && m_deviceInfo.IsBrowserIE && (rPLTextBoxPropsDef.CanGrow || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode)))
					{
						styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
					}
					RenderReportItemStyle(cellItem, elementProps, definition, rPLTextBoxProps.NonSharedStyle, rPLTextBoxPropsDef.SharedStyle, rPLItemMeasurement, styleContext, ref borderContext, textBoxClass);
					styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
					styleContext.IgnorePadding = ignorePadding;
				}
				else
				{
					RenderReportItemStyle(cellItem, elementProps, definition, null, styleContext, ref borderContext, definition.ID + "c");
				}
				styleContext.StyleOnCell = false;
			}
			else if (styleContext.ZeroWidth)
			{
				WriteStream(m_displayNone);
			}
			CloseStyle(renderQuote: true);
			if (styleContext.EmptyTextBox && rPLTextBox != null && elementProps != null)
			{
				WriteToolTip(elementProps);
			}
			WriteStream(m_closeBracket);
		}

		private void RenderTablixReportItem(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			styleContext.OmitBordersState = cell.ElementState;
			if (styleContext.EmptyTextBox)
			{
				bool flag = false;
				RPLActionInfo actionInfo = rPLTextBoxProps.ActionInfo;
				if (HasAction(actionInfo))
				{
					RenderElementHyperlinkAllTextStyles(rPLTextBoxProps.Style, actionInfo.Actions[0], rPLTextBoxPropsDef.ID + "a");
					WriteStream(m_openDiv);
					OpenStyle();
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					rPLItemMeasurement.Height = GetInnerContainerHeightSubtractBorders(rPLItemMeasurement, rPLTextBoxProps.Style);
					if (m_deviceInfo.BrowserMode == BrowserMode.Quirks && m_deviceInfo.IsBrowserIE)
					{
						RenderMeasurementHeight(rPLItemMeasurement.Height);
					}
					else
					{
						RenderMeasurementMinHeight(rPLItemMeasurement.Height);
					}
					WriteStream(m_semiColon);
					WriteStream(m_cursorHand);
					WriteStream(m_semiColon);
					CloseStyle(renderQuote: true);
					WriteStream(m_closeBracket);
					flag = true;
				}
				WriteStream(m_nbsp);
				if (flag)
				{
					WriteStream(m_closeDiv);
					WriteStream(m_closeA);
				}
			}
			else
			{
				styleContext.InTablix = true;
				bool renderId = NeedReportItemId(cellItem, elementProps);
				if (rPLTextBox != null)
				{
					styleContext.RenderMeasurements = false;
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					RenderTextBoxPercent(rPLTextBox, rPLTextBoxProps, rPLTextBoxPropsDef, rPLItemMeasurement, styleContext, renderId);
				}
				else
				{
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					if (cellItem is RPLRectangle || cellItem is RPLSubReport || cellItem is RPLLine)
					{
						styleContext.RenderMeasurements = false;
					}
					RenderReportItem(cellItem, elementProps, definition, rPLItemMeasurement, styleContext, borderContext, renderId);
				}
			}
			styleContext.Reset();
		}

		private void RenderListReportItem(RPLTablix tablix, RPLTablixCell cell, List<RPLTablixMemberCell> omittedHeaders, int tablixContext, StyleContext styleContext, bool firstRow, bool lastRow, SharedListLayoutState layoutState, RPLElement cellItem)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLItemMeasurement rPLItemMeasurement = null;
			rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Width = tablix.ColumnWidths[0];
			rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
			rPLItemMeasurement.State = cell.ElementState;
			bool zeroWidth = styleContext.ZeroWidth;
			styleContext.ZeroWidth = (rPLItemMeasurement.Width == 0f);
			if (layoutState == SharedListLayoutState.Start)
			{
				WriteStream(m_openTD);
				if (styleContext.ZeroWidth)
				{
					OpenStyle();
					WriteStream(m_displayNone);
					CloseStyle(renderQuote: true);
				}
				WriteStream(m_closeBracket);
			}
			if (cellItem is RPLRectangle)
			{
				int num = tablix.ColumnWidths.Length;
				int colIndex = cell.ColIndex;
				int colSpan = cell.ColSpan;
				bool right = colIndex + colSpan == num;
				int newContext = GetNewContext(tablixContext, left: true, right, firstRow, lastRow);
				RenderListRectangle((RPLRectangle)cellItem, omittedHeaders, rPLItemMeasurement, elementProps, definition, layoutState, newContext);
				if (layoutState == SharedListLayoutState.End)
				{
					WriteStream(m_closeTD);
				}
			}
			else
			{
				int omittedIndex = 0;
				RenderTablixOmittedHeaderCells(omittedHeaders, 0, lastCol: true, ref omittedIndex);
				RenderReportItem(cellItem, elementProps, definition, rPLItemMeasurement, styleContext, 0, NeedReportItemId(cellItem, elementProps));
				styleContext.Reset();
				if (layoutState == SharedListLayoutState.End)
				{
					WriteStream(m_closeTD);
				}
			}
			styleContext.ZeroWidth = zeroWidth;
		}

		protected void RenderListRectangle(RPLContainer rectangle, List<RPLTablixMemberCell> omittedHeaders, RPLItemMeasurement measurement, RPLElementProps props, RPLElementPropsDef def, SharedListLayoutState layoutState, int borderContext)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, expandLayout: false, layoutState, omittedHeaders, props.Style);
		}

		private void RenderSimpleTablixCell(RPLTablix tablix, string tablixID, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, bool lastCol, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			StyleContext styleContext = new StyleContext();
			int colIndex = cell.ColIndex;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
			WriteStream(m_openTD);
			RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (colSpan > 1)
			{
				WriteStream(m_colSpan);
				WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, firstCol: false, lastCol, firstRow, lastRow, element, ref borderContext);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				RenderTablixReportItem(tablix, tablixContext, cell, styleContext, firstCol: false, lastCol, firstRow, lastRow, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					OpenStyle();
					WriteStream(m_displayNone);
					CloseStyle(renderQuote: true);
				}
				WriteStream(m_closeBracket);
				WriteStream(m_nbsp);
				RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			}
			WriteStream(m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private int GetColSpanMinusZeroWidthColumns(int startColIndex, int colSpan, RPLTablix tablix)
		{
			int num = colSpan;
			for (int i = startColIndex; i < startColIndex + colSpan; i++)
			{
				if (tablix.ColumnWidths[i] == 0f)
				{
					num--;
				}
			}
			return num;
		}

		private int RenderZeroWidthTDsForTablix(int startIndex, int colSpan, RPLTablix tablix)
		{
			int i;
			for (i = startIndex; i < startIndex + colSpan && tablix.ColumnWidths[i] == 0f; i++)
			{
				WriteStream(m_openTD);
				OpenStyle();
				WriteStream(m_displayNone);
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
				WriteStream(m_closeTD);
			}
			return i;
		}

		private void RenderSimpleTablixCellID(RPLTablix tablix, string tablixID, int row, TablixFixedHeaderStorage headerStorage, int col)
		{
			if (tablix.FixedColumns[col])
			{
				string text = tablixID + "r" + row + "c" + col;
				RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null && tablix.FixedRow(row))
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
		}

		protected void RenderMultiLineTextWithHits(string text, List<int> hits)
		{
			if (text == null)
			{
				return;
			}
			int num = 0;
			int startPos = 0;
			int currentHitIndex = 0;
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				switch (text[i])
				{
				case '\r':
					RenderTextWithHits(text, startPos, num, hits, ref currentHitIndex);
					startPos = num + 1;
					break;
				case '\n':
					RenderTextWithHits(text, startPos, num, hits, ref currentHitIndex);
					WriteStreamCR(m_br);
					startPos = num + 1;
					break;
				}
				num++;
			}
			RenderTextWithHits(text, startPos, num, hits, ref currentHitIndex);
		}

		protected void RenderTextWithHits(string text, int startPos, int endPos, List<int> hitIndices, ref int currentHitIndex)
		{
			int length = m_searchText.Length;
			while (currentHitIndex < hitIndices.Count && hitIndices[currentHitIndex] < endPos)
			{
				int num = hitIndices[currentHitIndex];
				string theString = text.Substring(startPos, num - startPos);
				WriteStreamEncoded(theString);
				theString = text.Substring(num, length);
				OutputFindString(theString, 0);
				startPos = num + length;
				currentHitIndex++;
				m_currentHitCount++;
			}
			if (startPos <= endPos)
			{
				string theString = text.Substring(startPos, endPos - startPos);
				WriteStreamEncoded(theString);
			}
		}

		private void OutputFindString(string findString, int offset)
		{
			WriteStream(m_openSpan);
			WriteStream(m_id);
			WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
			WriteStream(m_searchHitIdPrefix);
			WriteStream(m_currentHitCount.ToString(CultureInfo.InvariantCulture));
			if (offset > 0)
			{
				WriteStream("_");
				WriteStream(offset.ToString(CultureInfo.InvariantCulture));
			}
			WriteStream(m_quote);
			if (m_currentHitCount == 0)
			{
				if (m_deviceInfo.IsBrowserSafari)
				{
					WriteStream(" style=\"COLOR:black;BACKGROUND-COLOR:#B5D4FE;\">");
				}
				else
				{
					WriteStream(" style=\"COLOR:highlighttext;BACKGROUND-COLOR:highlight;\">");
				}
			}
			else
			{
				WriteStream(m_closeBracket);
			}
			WriteStreamEncoded(findString);
			WriteStream(m_closeSpan);
		}

		private bool IsImageNotFitProportional(RPLElement reportItem, RPLElementPropsDef definition)
		{
			RPLImagePropsDef rPLImagePropsDef = null;
			if (definition is RPLImagePropsDef)
			{
				rPLImagePropsDef = (RPLImagePropsDef)definition;
			}
			if (reportItem is RPLImage && rPLImagePropsDef != null)
			{
				return rPLImagePropsDef.Sizing != RPLFormat.Sizings.FitProportional;
			}
			return false;
		}

		protected void RenderImage(RPLImage image, RPLImageProps imageProps, RPLImagePropsDef imagePropsDef, RPLItemMeasurement measurement, ref int borderContext, bool renderId)
		{
			bool flag = false;
			bool flag2 = false;
			RPLImageData image2 = imageProps.Image;
			RPLActionInfo actionInfo = imageProps.ActionInfo;
			StyleContext styleContext = new StyleContext();
			RPLFormat.Sizings sizing = imagePropsDef.Sizing;
			bool flag3 = false;
			if (sizing == RPLFormat.Sizings.AutoSize)
			{
				flag3 = true;
				WriteStream(m_openTable);
				WriteStream(m_closeBracket);
				WriteStream(m_firstTD);
				WriteStream(m_closeBracket);
			}
			WriteStream(m_openDiv);
			int xOffset = 0;
			int yOffset = 0;
			System.Drawing.Rectangle imageConsolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
			bool flag4 = !imageConsolidationOffsets.IsEmpty;
			if (flag4)
			{
				if (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit)
				{
					styleContext.RenderMeasurements = (styleContext.InTablix || sizing != RPLFormat.Sizings.AutoSize);
					RenderReportItemStyle(image, imageProps, imagePropsDef, measurement, styleContext, ref borderContext, imagePropsDef.ID);
					WriteStream(m_closeBracket);
					WriteStream(m_openDiv);
				}
				WriteOuterConsolidation(imageConsolidationOffsets, sizing, imageProps.UniqueName);
				RenderReportItemStyle(image, imageProps, imagePropsDef, null, styleContext, ref borderContext, imagePropsDef.ID);
				xOffset = imageConsolidationOffsets.Left;
				yOffset = imageConsolidationOffsets.Top;
			}
			else
			{
				styleContext.RenderMeasurements = (styleContext.InTablix || sizing != RPLFormat.Sizings.AutoSize);
				RenderReportItemStyle(image, imageProps, imagePropsDef, measurement, styleContext, ref borderContext, imagePropsDef.ID);
			}
			WriteStream(m_closeBracket);
			if (HasAction(actionInfo))
			{
				flag2 = RenderElementHyperlink(imageProps.Style, actionInfo.Actions[0]);
			}
			WriteStream(m_img);
			if (m_browserIE)
			{
				WriteStream(m_imgOnError);
			}
			if (renderId || flag)
			{
				RenderReportItemId(imageProps.UniqueName);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length != 0)
			{
				WriteAttrEncoded(m_useMap, "#" + m_deviceInfo.HtmlPrefixId + m_mapPrefixString + imageProps.UniqueName);
				WriteStream(m_zeroBorder);
			}
			else if (flag2)
			{
				WriteStream(m_zeroBorder);
			}
			switch (sizing)
			{
			case RPLFormat.Sizings.FitProportional:
			{
				PaddingSharedInfo paddings = GetPaddings(image.ElementProps.Style, null);
				bool writeSmallSize = !flag4 && m_deviceInfo.BrowserMode == BrowserMode.Standards;
				RenderImageFitProportional(image, measurement, paddings, writeSmallSize);
				break;
			}
			case RPLFormat.Sizings.Fit:
				if (!flag4)
				{
					if (m_useInlineStyle)
					{
						PercentSizes();
					}
					else
					{
						ClassPercentSizes();
					}
				}
				break;
			}
			if (flag4)
			{
				WriteClippedDiv(imageConsolidationOffsets);
			}
			WriteToolTip(imageProps);
			WriteStream(m_src);
			RenderImageUrl(useSessionId: true, image2);
			WriteStreamCR(m_closeTag);
			if (flag2)
			{
				WriteStream(m_closeA);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length != 0)
			{
				RenderImageMapAreas(imageProps.ActionImageMapAreas, measurement.Width, measurement.Height, imageProps.UniqueName, xOffset, yOffset);
			}
			if (flag4 && (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit))
			{
				WriteStream(m_closeDiv);
			}
			WriteStreamCR(m_closeDiv);
			if (flag3)
			{
				WriteStreamCR(m_lastTD);
				WriteStreamCR(m_closeTable);
			}
		}

		protected int RenderReportItem(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, int borderContext, bool renderId)
		{
			int borderContext2 = borderContext;
			if (reportItem == null)
			{
				return borderContext2;
			}
			if (measurement != null)
			{
				styleContext.OmitBordersState = measurement.State;
			}
			RPLTextBox rPLTextBox = reportItem as RPLTextBox;
			if (rPLTextBox != null)
			{
				if (styleContext.InTablix)
				{
					RenderTextBoxPercent(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, renderId);
				}
				else
				{
					RenderTextBox(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, ref borderContext2, renderId);
				}
			}
			else if (reportItem is RPLTablix)
			{
				RenderTablix((RPLTablix)reportItem, props, def, measurement, styleContext, ref borderContext2, renderId);
			}
			else if (reportItem is RPLRectangle)
			{
				RenderRectangle((RPLContainer)reportItem, props, (RPLRectanglePropsDef)def, measurement, ref borderContext2, renderId, styleContext);
			}
			else if (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)
			{
				RenderServerDynamicImage(reportItem, (RPLDynamicImageProps)props, def, measurement, borderContext2, renderId, styleContext);
			}
			else if (reportItem is RPLSubReport)
			{
				RenderSubReport((RPLSubReport)reportItem, props, def, measurement, ref borderContext2, renderId, styleContext);
			}
			else if (reportItem is RPLImage)
			{
				if (styleContext.InTablix)
				{
					RenderImagePercent((RPLImage)reportItem, (RPLImageProps)props, (RPLImagePropsDef)def, measurement);
				}
				else
				{
					RenderImage((RPLImage)reportItem, (RPLImageProps)props, (RPLImagePropsDef)def, measurement, ref borderContext2, renderId);
				}
			}
			else if (reportItem is RPLLine)
			{
				RenderLine((RPLLine)reportItem, props, (RPLLinePropsDef)def, measurement, renderId, styleContext);
			}
			return borderContext2;
		}

		protected void RenderSubReport(RPLSubReport subReport, RPLElementProps subReportProps, RPLElementPropsDef subReportDef, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext)
		{
			if (!styleContext.InTablix || renderId)
			{
				styleContext.RenderMeasurements = false;
				WriteStream(m_openDiv);
				RenderReportItemStyle(subReport, subReportProps, subReportDef, measurement, styleContext, ref borderContext, subReportDef.ID);
				if (renderId)
				{
					RenderReportItemId(subReportProps.UniqueName);
				}
				WriteStreamCR(m_closeBracket);
			}
			RPLItemMeasurement[] children = subReport.Children;
			int num = 0;
			int num2 = borderContext;
			bool usePercentWidth = children.Length != 0;
			int num3 = children.Length;
			for (int i = 0; i < num3; i++)
			{
				if (i == 0 && num3 > 1 && (borderContext & 8) > 0)
				{
					num2 &= -9;
				}
				else if (i == 1 && (borderContext & 4) > 0)
				{
					num2 &= -5;
				}
				if (i > 0 && i == num3 - 1 && (borderContext & 8) > 0)
				{
					num2 |= 8;
				}
				num = num2;
				RPLItemMeasurement rPLItemMeasurement = children[i];
				RPLContainer rPLContainer = (RPLContainer)rPLItemMeasurement.Element;
				RPLElementProps elementProps = rPLContainer.ElementProps;
				RPLElementPropsDef definition = elementProps.Definition;
				m_isBody = true;
				m_usePercentWidth = usePercentWidth;
				RenderRectangle(rPLContainer, elementProps, definition, rPLItemMeasurement, ref num, renderId: false, new StyleContext());
			}
			if (!styleContext.InTablix || renderId)
			{
				WriteStreamCR(m_closeDiv);
			}
		}

		protected void RenderRectangleMeasurements(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float adjustedWidth = GetAdjustedWidth(measurement, style);
			float adjustedHeight = GetAdjustedHeight(measurement, style);
			RenderMeasurementWidth(adjustedWidth, renderMinWidth: true);
			if (m_deviceInfo.IsBrowserIE && m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6)
			{
				RenderMeasurementMinHeight(adjustedHeight);
			}
			else
			{
				RenderMeasurementHeight(adjustedHeight);
			}
		}

		private void WriteFontSizeSmallPoint()
		{
			if (m_deviceInfo.IsBrowserGeckoEngine)
			{
				WriteStream(m_smallPoint);
			}
			else
			{
				WriteStream(m_zeroPoint);
			}
		}

		protected void RenderRectangle(RPLContainer rectangle, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			RPLRectanglePropsDef rPLRectanglePropsDef = def as RPLRectanglePropsDef;
			if (rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null)
			{
				m_linkToChildStack.Push(rPLRectanglePropsDef.LinkToChildId);
			}
			bool expandItem = m_expandItem;
			bool flag = renderId;
			string text = props.UniqueName;
			bool flag2 = children == null || children.Length == 0;
			if (flag2 && styleContext.InTablix)
			{
				return;
			}
			bool flag3 = m_deviceInfo.OutlookCompat || !m_browserIE || (flag2 && m_usePercentWidth);
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					WriteStream(m_openTable);
					WriteStream(m_zeroBorder);
				}
				else
				{
					WriteStream(m_openDiv);
					if (m_deviceInfo.IsBrowserIE && m_deviceInfo.AllowScript)
					{
						if (!m_needsGrowRectangleScript)
						{
							CreateGrowRectIdsStream();
						}
						flag = true;
						if (!renderId)
						{
							text = props.UniqueName + "_gr";
						}
						WriteIdToSecondaryStream(m_growRectangleIdsStream, text);
					}
				}
				if (flag)
				{
					RenderReportItemId(text);
				}
				if (m_isBody)
				{
					m_isBody = false;
					styleContext.RenderMeasurements = false;
					if (flag2)
					{
						OpenStyle();
						if (m_usePercentWidth)
						{
							RenderMeasurementHeight(measurement.Height);
							WriteStream(m_styleWidth);
							WriteStream(m_percent);
							WriteStream(m_semiColon);
						}
						else
						{
							RenderRectangleMeasurements(measurement, props.Style);
						}
					}
					else if (flag3 && m_usePercentWidth)
					{
						OpenStyle();
						WriteStream(m_styleWidth);
						WriteStream(m_percent);
						WriteStream(m_semiColon);
					}
					m_usePercentWidth = false;
				}
				if (!styleContext.InTablix)
				{
					if (styleContext.RenderMeasurements)
					{
						OpenStyle();
						RenderRectangleMeasurements(measurement, props.Style);
					}
					RenderReportItemStyle(rectangle, props, def, measurement, styleContext, ref borderContext, def.ID);
				}
				CloseStyle(renderQuote: true);
				WriteToolTip(props);
				WriteStreamCR(m_closeBracket);
				if (flag3)
				{
					WriteStream(m_firstTD);
					OpenStyle();
					if (flag2)
					{
						RenderMeasurementStyle(measurement.Height, measurement.Width);
						WriteStream(m_fontSize);
						WriteStream("1pt");
					}
					else
					{
						WriteStream(m_verticalAlign);
						WriteStream(m_topValue);
					}
					CloseStyle(renderQuote: true);
					WriteStream(m_closeBracket);
				}
			}
			if (flag2)
			{
				WriteStream(m_nbsp);
			}
			else
			{
				bool inTablix = styleContext.InTablix;
				styleContext.InTablix = false;
				flag2 = GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, expandItem, SharedListLayoutState.None, null, props.Style);
				if (inTablix)
				{
					styleContext.InTablix = true;
				}
			}
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					WriteStream(m_lastTD);
					WriteStream(m_closeTable);
				}
				else
				{
					WriteStreamCR(m_closeDiv);
				}
			}
			if (m_linkToChildStack.Count > 0 && rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null && rPLRectanglePropsDef.LinkToChildId.Equals(m_linkToChildStack.Peek()))
			{
				m_linkToChildStack.Pop();
			}
		}

		private void RenderElementHyperlinkAllTextStyles(RPLElementStyle style, RPLAction action, string id)
		{
			WriteStream(m_openA);
			RenderTabIndex();
			bool hasHref = false;
			if (action.Hyperlink != null)
			{
				WriteStream(m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + m_quoteString);
				hasHref = true;
			}
			else
			{
				RenderInteractionAction(action, ref hasHref);
			}
			TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
			WriteStyles(id, style.NonSharedProperties, style.SharedProperties, styleWriter);
			if (m_deviceInfo.LinkTarget != null)
			{
				WriteStream(m_target);
				WriteStream(m_deviceInfo.LinkTarget);
				WriteStream(m_quote);
			}
			WriteStream(m_closeBracket);
		}

		private bool RenderElementHyperlink(IRPLStyle style, RPLAction action)
		{
			object obj = style[24];
			obj = ((obj != null) ? obj : ((object)RPLFormat.TextDecorations.None));
			string color = (string)style[27];
			return RenderHyperlink(action, (RPLFormat.TextDecorations)obj, color);
		}

		protected void RenderTextBoxPercent(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, bool renderId)
		{
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			RPLElementStyle style = textBoxProps.Style;
			bool flag = CanSort(textBoxPropsDef);
			bool flag2 = NeedSharedToggleParent(textBoxProps);
			bool flag3 = false;
			bool isSimple = textBoxPropsDef.IsSimple;
			bool flag4 = IsDirectionRTL(style);
			bool flag5 = IsWritingModeVertical(style);
			bool flag6 = flag5 && m_deviceInfo.IsBrowserIE;
			if (flag6)
			{
				if (textBoxPropsDef.CanGrow)
				{
					WriteStream(m_openDiv);
					OpenStyle();
					RenderDirectionStyles(textBox, textBoxProps, textBoxPropsDef, null, textBoxProps.Style, nonSharedStyle, isNonSharedStyles: false, styleContext);
					WriteStream("display: inline;");
					CloseStyle(renderQuote: true);
					ClassPercentHeight();
					if (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode && m_deviceInfo.AllowScript)
					{
						if (!m_needsFitVertTextScript)
						{
							CreateFitVertTextIdsStream();
						}
						WriteIdToSecondaryStream(m_fitVertTextIdsStream, textBoxProps.UniqueName + "_fvt");
						RenderReportItemId(textBoxProps.UniqueName + "_fvt");
					}
					WriteStreamCR(m_closeBracket);
					WriteStream(m_openTable);
					ClassPercentHeight();
					WriteStreamCR(m_closeBracket);
					WriteStream(m_firstTD);
				}
				else
				{
					WriteStream(m_openDiv);
				}
			}
			else
			{
				WriteStream(m_openDiv);
			}
			if (renderId || flag2 || flag)
			{
				RenderReportItemId(textBoxProps.UniqueName);
			}
			bool flag7 = flag2 && !isSimple;
			bool flag8 = flag || flag7;
			if (!textBoxPropsDef.CanGrow)
			{
				if ((!m_browserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || flag6) && measurement != null)
				{
					styleContext.RenderMeasurements = false;
					float innerContainerHeight = GetInnerContainerHeight(measurement, style);
					OpenStyle();
					RenderMeasurementHeight(innerContainerHeight);
					WriteStream(m_overflowHidden);
					WriteStream(m_semiColon);
				}
				else
				{
					styleContext.RenderMeasurements = true;
				}
				if (!flag8)
				{
					object obj = style[26];
					bool flag9 = obj != null && (RPLFormat.VerticalAlignments)obj != 0 && !textBoxPropsDef.CanGrow;
					flag8 = flag9;
				}
				measurement = null;
			}
			if (flag8)
			{
				CloseStyle(renderQuote: true);
				styleContext.RenderMeasurements = false;
				WriteStreamCR(m_closeBracket);
				WriteStream(m_openTable);
				WriteStream(m_zeroBorder);
				if (isSimple && (flag || flag7))
				{
					WriteClassName(m_percentHeightInlineTable, m_classPercentHeightInlineTable);
				}
				else
				{
					WriteClassName(m_percentSizeInlineTable, m_classPercentSizeInlineTable);
				}
				RenderReportLanguage();
				WriteStream(m_closeBracket);
				WriteStream(m_firstTD);
				if (flag || flag7)
				{
					if (flag5)
					{
						WriteStream(" ROWS='2'");
					}
					RenderAtStart(textBoxProps, style, flag && flag4, flag7 && !flag4);
				}
			}
			int borderContext = 0;
			RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID);
			WriteToolTip(textBoxProps);
			WriteStreamCR(m_closeBracket);
			if (flag2 && isSimple)
			{
				RenderToggleImage(textBoxProps);
			}
			RPLAction rPLAction = null;
			if (HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				RenderElementHyperlinkAllTextStyles(style, rPLAction, textBoxPropsDef.ID + "a");
				flag3 = true;
			}
			string text = null;
			if (textBoxPropsDef.IsSimple)
			{
				text = textBoxProps.Value;
				if (string.IsNullOrEmpty(text))
				{
					text = textBoxPropsDef.Value;
				}
			}
			RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag2 || flag, measurement, rPLAction);
			if (flag3)
			{
				WriteStream(m_closeA);
			}
			if (flag8)
			{
				RenderAtEnd(textBoxProps, style, flag && !flag4, flag7 && flag4);
				WriteStream(m_lastTD);
				WriteStream(m_closeTable);
			}
			if (flag6)
			{
				if (textBoxPropsDef.CanGrow)
				{
					WriteStreamCR(m_lastTD);
					WriteStreamCR(m_closeTable);
					WriteStreamCR(m_closeDiv);
				}
				else
				{
					WriteStream(m_closeDiv);
				}
			}
			else
			{
				WriteStreamCR(m_closeDiv);
			}
		}

		protected void RenderPageHeaderFooter(RPLItemMeasurement hfMeasurement)
		{
			if (hfMeasurement.Height != 0f)
			{
				RPLHeaderFooter rPLHeaderFooter = (RPLHeaderFooter)hfMeasurement.Element;
				int borderContext = 0;
				StyleContext styleContext = new StyleContext();
				WriteStream(m_openTR);
				WriteStream(m_closeBracket);
				WriteStream(m_openTD);
				styleContext.StyleOnCell = true;
				RenderReportItemStyle(rPLHeaderFooter, rPLHeaderFooter.ElementProps, rPLHeaderFooter.ElementProps.Definition, null, styleContext, ref borderContext, rPLHeaderFooter.ElementProps.Definition.ID + "c");
				styleContext.StyleOnCell = false;
				WriteStream(m_closeBracket);
				WriteStream(m_openDiv);
				if (!m_deviceInfo.IsBrowserIE)
				{
					styleContext.RenderMeasurements = false;
					styleContext.RenderMinMeasurements = true;
				}
				RenderReportItemStyle(rPLHeaderFooter, hfMeasurement, ref borderContext, styleContext);
				WriteStreamCR(m_closeBracket);
				RPLItemMeasurement[] children = rPLHeaderFooter.Children;
				if (children != null && children.Length != 0)
				{
					m_renderTableHeight = true;
					GenerateHTMLTable(children, 0f, 0f, m_pageContent.MaxSectionWidth, hfMeasurement.Height, borderContext, expandLayout: false, SharedListLayoutState.None, null, rPLHeaderFooter.ElementProps.Style);
				}
				else
				{
					WriteStream(m_nbsp);
				}
				m_renderTableHeight = false;
				WriteStreamCR(m_closeDiv);
				WriteStream(m_closeTD);
				WriteStream(m_closeTR);
			}
		}

		protected void RenderStyleProps(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, StyleContext styleContext, ref int borderContext, bool isNonSharedStyles)
		{
			if (styleContext.ZeroWidth)
			{
				WriteStream(m_displayNone);
			}
			IRPLStyle iRPLStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
			if (iRPLStyle == null)
			{
				return;
			}
			object obj = null;
			if (styleContext.StyleOnCell)
			{
				bool renderPadding = true;
				if ((IsWritingModeVertical(sharedStyleProps) || IsWritingModeVertical(nonSharedStyleProps)) && styleContext.IgnorePadding && m_deviceInfo.IsBrowserIE)
				{
					renderPadding = false;
				}
				if (!styleContext.NoBorders)
				{
					RenderHtmlBorders(iRPLStyle, ref borderContext, styleContext.OmitBordersState, renderPadding, isNonSharedStyles, sharedStyleProps);
					RenderBackgroundStyleProps(iRPLStyle);
				}
				if (!styleContext.OnlyRenderMeasurementsBackgroundBorders)
				{
					obj = iRPLStyle[26];
					if (obj != null && !styleContext.IgnoreVerticalAlign)
					{
						obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
						WriteStream(m_verticalAlign);
						WriteStream(obj);
						WriteStream(m_semiColon);
					}
					obj = iRPLStyle[25];
					if (obj != null)
					{
						if ((RPLFormat.TextAlignments)obj != 0)
						{
							obj = EnumStrings.GetValue((RPLFormat.TextAlignments)obj);
							WriteStream(m_textAlign);
							WriteStream(obj);
							WriteStream(m_semiColon);
						}
						else
						{
							RenderTextAlign(props as RPLTextBoxProps, props.Style);
						}
					}
					RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
				}
				if (measurement == null || (!m_deviceInfo.OutlookCompat && m_deviceInfo.IsBrowserIE))
				{
					return;
				}
				float num = measurement.Width;
				if ((reportItem is RPLTextBox || IsImageNotFitProportional(reportItem, definition)) && !styleContext.InTablix)
				{
					float adjustedWidth = GetAdjustedWidth(measurement, props.Style);
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						num = adjustedWidth;
					}
					RenderMeasurementMinWidth(adjustedWidth);
				}
				else
				{
					RenderMeasurementMinWidth(num);
				}
				RenderMeasurementWidth(num, renderMinWidth: false);
				return;
			}
			if (reportItem is RPLTextBox)
			{
				WriteStream(m_wordWrap);
				WriteStream(m_semiColon);
				WriteStream(m_whiteSpacePreWrap);
				WriteStream(m_semiColon);
			}
			if (styleContext.RenderMeasurements || styleContext.RenderMinMeasurements)
			{
				bool empty = false;
				IsCollectionWithoutContent(reportItem as RPLContainer, ref empty);
				if (measurement == null || (styleContext.InTablix && !empty && (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)))
				{
					if (reportItem is RPLTextBox)
					{
						RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
						if (styleContext.RenderMeasurements)
						{
							WriteStream(m_styleWidth);
						}
						else if (styleContext.RenderMinMeasurements)
						{
							WriteStream(m_styleMinWidth);
						}
						if (styleContext.InTablix && m_deviceInfo.BrowserMode == BrowserMode.Quirks)
						{
							WriteStream(m_ninetyninepercent);
						}
						else
						{
							WriteStream(m_percent);
						}
						WriteStream(m_semiColon);
						if (rPLTextBoxPropsDef.CanGrow)
						{
							WriteStream(m_overflowXHidden);
						}
						else
						{
							if (styleContext.RenderMeasurements)
							{
								WriteStream(m_styleHeight);
							}
							else if (styleContext.RenderMinMeasurements)
							{
								WriteStream(m_styleMinHeight);
							}
							WriteStream(m_percent);
							WriteStream(m_semiColon);
							WriteStream(m_overflowHidden);
						}
						WriteStream(m_semiColon);
					}
					else if (!(reportItem is RPLTablix))
					{
						RenderPercentSizes();
					}
				}
				else if (reportItem is RPLTextBox)
				{
					float num2 = measurement.Width;
					float height = measurement.Height;
					if (!styleContext.NoBorders && !styleContext.InTablix)
					{
						float adjustedWidth2 = GetAdjustedWidth(measurement, props.Style);
						if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							num2 = adjustedWidth2;
							height = GetAdjustedHeight(measurement, props.Style);
						}
						RenderMeasurementMinWidth(adjustedWidth2);
					}
					else
					{
						RenderMeasurementMinWidth(num2);
					}
					RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)definition;
					if (rPLTextBoxPropsDef2.CanGrow && rPLTextBoxPropsDef2.CanShrink)
					{
						RenderMeasurementWidth(num2, renderMinWidth: false);
					}
					else
					{
						WriteStream(m_overflowHidden);
						WriteStream(m_semiColon);
						RenderMeasurementWidth(num2, renderMinWidth: false);
						RenderMeasurementHeight(height);
					}
				}
				else if (!(reportItem is RPLTablix))
				{
					if (!(reportItem is RPLRectangle))
					{
						float height2 = measurement.Height;
						float num3 = measurement.Width;
						if (!styleContext.InTablix && IsImageNotFitProportional(reportItem, definition) && !styleContext.NoBorders)
						{
							float adjustedWidth3 = GetAdjustedWidth(measurement, props.Style);
							if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
							{
								num3 = adjustedWidth3;
								height2 = GetAdjustedHeight(measurement, props.Style);
							}
							RenderMeasurementMinWidth(adjustedWidth3);
						}
						else
						{
							RenderMeasurementMinWidth(num3);
						}
						if (reportItem is RPLHeaderFooter && (!m_deviceInfo.IsBrowserIE || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6)))
						{
							RenderMeasurementMinHeight(height2);
						}
						else
						{
							RenderMeasurementHeight(height2);
						}
						RenderMeasurementWidth(num3, renderMinWidth: false);
					}
					if (empty || reportItem is RPLImage)
					{
						WriteStream(m_overflowHidden);
						WriteStream(m_semiColon);
					}
				}
			}
			if (!styleContext.InTablix && !styleContext.NoBorders)
			{
				RenderHtmlBorders(iRPLStyle, ref borderContext, styleContext.OmitBordersState, !styleContext.EmptyTextBox || m_deviceInfo.IsBrowserIE6Or7StandardsMode, isNonSharedStyles, sharedStyleProps);
				RenderBackgroundStyleProps(iRPLStyle);
			}
			if (styleContext.OnlyRenderMeasurementsBackgroundBorders || (styleContext.EmptyTextBox && isNonSharedStyles))
			{
				return;
			}
			obj = iRPLStyle[19];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.FontStyles)obj);
				WriteStream(m_fontStyle);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[20];
			if (obj != null)
			{
				WriteStream(m_fontFamily);
				WriteStream(HandleSpecialFontCharacters(obj.ToString()));
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[21];
			if (obj != null)
			{
				WriteStream(m_fontSize);
				if (string.Compare(obj.ToString(), "0pt", StringComparison.OrdinalIgnoreCase) != 0)
				{
					WriteStream(obj);
				}
				else
				{
					WriteFontSizeSmallPoint();
				}
				WriteStream(m_semiColon);
			}
			else
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef3 = definition as RPLTextBoxPropsDef;
				RPLStyleProps sharedStyle = reportItem.ElementPropsDef.SharedStyle;
				if ((!isNonSharedStyles || sharedStyle == null || sharedStyle.Count == 0) && rPLTextBoxPropsDef3 != null && !rPLTextBoxPropsDef3.IsSimple)
				{
					WriteStream(m_fontSize);
					WriteFontSizeSmallPoint();
					WriteStream(m_semiColon);
				}
			}
			obj = iRPLStyle[22];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.FontWeights)obj);
				WriteStream(m_fontWeight);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[24];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
				WriteStream(m_textDecoration);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[31];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.UnicodeBiDiTypes)obj);
				WriteStream(m_unicodeBiDi);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[27];
			if (obj != null)
			{
				WriteStream(m_color);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[28];
			if (obj != null)
			{
				WriteStream(m_lineHeight);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			if ((IsWritingModeVertical(sharedStyleProps) || IsWritingModeVertical(nonSharedStyleProps)) && reportItem is RPLTextBox && styleContext.InTablix && m_deviceInfo.IsBrowserIE && !styleContext.IgnorePadding)
			{
				RenderPaddingStyle(iRPLStyle);
			}
			RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
			obj = iRPLStyle[26];
			if (obj != null && !styleContext.IgnoreVerticalAlign)
			{
				obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
				WriteStream(m_verticalAlign);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[25];
			if (obj != null)
			{
				if ((RPLFormat.TextAlignments)obj != 0)
				{
					WriteStream(m_textAlign);
					WriteStream(EnumStrings.GetValue((RPLFormat.TextAlignments)obj));
					WriteStream(m_semiColon);
				}
				else
				{
					RenderTextAlign(props as RPLTextBoxProps, props.Style);
				}
			}
		}

		protected void RenderLine(RPLLine reportItem, RPLElementProps rplProps, RPLLinePropsDef rplPropsDef, RPLItemMeasurement measurement, bool renderId, StyleContext styleContext)
		{
			if (IsLineSlanted(measurement))
			{
				if (renderId)
				{
					RenderNavigationId(rplProps.UniqueName);
				}
				if (m_deviceInfo.BrowserMode == BrowserMode.Quirks)
				{
					RenderVMLLine(reportItem, measurement, styleContext);
				}
				return;
			}
			bool flag = measurement.Height == 0f;
			WriteStream(m_openSpan);
			if (renderId)
			{
				RenderReportItemId(rplProps.UniqueName);
			}
			int borderContext = 0;
			object obj = rplProps.Style[10];
			if (obj != null)
			{
				OpenStyle();
				if (flag)
				{
					WriteStream(m_styleHeight);
				}
				else
				{
					WriteStream(m_styleWidth);
				}
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = rplProps.Style[0];
			if (obj != null)
			{
				OpenStyle();
				WriteStream(m_backgroundColor);
				WriteStream(obj);
			}
			RenderReportItemStyle(reportItem, measurement, ref borderContext);
			CloseStyle(renderQuote: true);
			WriteStream(m_closeBracket);
			WriteStream(m_closeSpan);
		}

		protected bool GenerateHTMLTable(RPLItemMeasurement[] repItemCol, float ownerTop, float ownerLeft, float dxParent, float dyParent, int borderContext, bool expandLayout, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style)
		{
			int num = 0;
			bool result = false;
			object defaultBorderStyle = null;
			object specificBorderStyle = null;
			object specificBorderStyle2 = null;
			object defaultBorderWidth = null;
			object specificBorderWidth = null;
			object specificBorderWidth2 = null;
			if (style != null)
			{
				defaultBorderStyle = style[5];
				specificBorderStyle = style[6];
				specificBorderStyle2 = style[7];
				defaultBorderWidth = style[10];
				specificBorderWidth = style[11];
				specificBorderWidth2 = style[12];
			}
			if (repItemCol == null || repItemCol.Length == 0)
			{
				if (omittedHeaders != null)
				{
					for (int i = 0; i < omittedHeaders.Count; i++)
					{
						if (omittedHeaders[i].GroupLabel != null)
						{
							RenderNavigationId(omittedHeaders[i].UniqueName);
						}
					}
				}
				return result;
			}
			PageTableLayout tableLayout = null;
			PageTableLayout.GenerateTableLayout(repItemCol, dxParent, dyParent, 0f, out tableLayout, expandLayout, m_rplReport.ConsumeContainerWhitespace);
			if (tableLayout == null)
			{
				return result;
			}
			if (tableLayout.BandTable && m_allowBandTable && layoutState == SharedListLayoutState.None && (!m_renderTableHeight || tableLayout.NrRows == 1))
			{
				if (omittedHeaders != null)
				{
					for (int j = 0; j < omittedHeaders.Count; j++)
					{
						if (omittedHeaders[j].GroupLabel != null)
						{
							RenderNavigationId(omittedHeaders[j].UniqueName);
						}
					}
				}
				int borderContext2 = 0;
				int k;
				for (k = 0; k < tableLayout.NrRows - 1; k++)
				{
					if (borderContext > 0)
					{
						borderContext2 = GetNewContext(borderContext, k + 1, 1, tableLayout.NrRows, 1);
					}
					RenderCellItem(tableLayout.GetCell(k), borderContext2, layoutExpand: false);
				}
				if (borderContext > 0)
				{
					borderContext2 = GetNewContext(borderContext, k + 1, 1, tableLayout.NrRows, 1);
				}
				RenderCellItem(tableLayout.GetCell(k), borderContext2, layoutExpand: false);
				return result;
			}
			m_allowBandTable = true;
			bool flag = false;
			bool renderHeight = true;
			bool flag2 = expandLayout;
			int num2 = tableLayout.NrCols;
			if (!flag2)
			{
				flag2 = tableLayout.AreSpansInColOne();
			}
			if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.Start)
			{
				WriteStream(m_openTable);
				WriteStream(m_zeroBorder);
				if (flag2)
				{
					num2++;
				}
				if (!m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(m_cols);
					WriteStream(num2.ToString(CultureInfo.InvariantCulture));
					WriteStream(m_quote);
				}
				RenderReportLanguage();
				if (m_useInlineStyle)
				{
					OpenStyle();
					WriteStream(m_borderCollapse);
					if (expandLayout)
					{
						WriteStream(m_semiColon);
						WriteStream(m_styleHeight);
						WriteStream(m_percent);
					}
				}
				else
				{
					ClassLayoutBorder();
					if (expandLayout)
					{
						WriteStream(m_space);
						WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
						WriteStream(m_percentHeight);
					}
					WriteStream(m_quote);
				}
				if (m_renderTableHeight)
				{
					if (m_isStyleOpen)
					{
						WriteStream(m_semiColon);
					}
					else
					{
						OpenStyle();
					}
					WriteStream(m_styleHeight);
					WriteDStream(dyParent);
					WriteStream(m_mm);
					m_renderTableHeight = false;
				}
				if (m_deviceInfo.OutlookCompat || m_deviceInfo.IsBrowserSafari)
				{
					if (m_isStyleOpen)
					{
						WriteStream(m_semiColon);
					}
					else
					{
						OpenStyle();
					}
					WriteStream(m_styleWidth);
					float num3 = dxParent;
					if (num3 > 0f)
					{
						num3 = SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
						num3 = SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
						if (num3 < 0f)
						{
							num3 = 1f;
						}
					}
					WriteStream(num3);
					WriteStream(m_mm);
				}
				CloseStyle(renderQuote: true);
				WriteStream(m_closeBracket);
				if (tableLayout.NrCols > 1)
				{
					flag = tableLayout.NeedExtraRow();
					if (flag)
					{
						WriteStream(m_openTR);
						WriteStream(m_zeroHeight);
						WriteStream(m_closeBracket);
						if (flag2)
						{
							WriteStream(m_openTD);
							WriteStream(m_openStyle);
							WriteStream(m_styleWidth);
							WriteStream("0");
							WriteStream(m_px);
							WriteStream(m_closeQuote);
							WriteStream(m_closeTD);
						}
						for (num = 0; num < tableLayout.NrCols; num++)
						{
							WriteStream(m_openTD);
							WriteStream(m_openStyle);
							WriteStream(m_styleWidth);
							float num4 = tableLayout.GetCell(num).DXValue.Value;
							if (num4 > 0f)
							{
								if (num == 0)
								{
									num4 = SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
								}
								if (num == tableLayout.NrCols - 1)
								{
									num4 = SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
								}
								if (num4 <= 0f)
								{
									num4 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : tableLayout.GetCell(num).DXValue.Value);
								}
							}
							WriteDStream(num4);
							WriteStream(m_mm);
							WriteStream(m_semiColon);
							WriteStream(m_styleMinWidth);
							WriteDStream(num4);
							WriteStream(m_mm);
							WriteStream(m_closeQuote);
							WriteStream(m_closeTD);
						}
						WriteStream(m_closeTR);
					}
				}
			}
			GenerateTableLayoutContent(tableLayout, repItemCol, flag, flag2, renderHeight, borderContext, expandLayout, layoutState, omittedHeaders, style);
			if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
			{
				if (expandLayout)
				{
					WriteStream(m_firstTD);
					ClassPercentHeight();
					WriteStream(m_cols);
					WriteStream(num2.ToString(CultureInfo.InvariantCulture));
					WriteStream(m_closeQuote);
					WriteStream(m_lastTD);
				}
				WriteStreamCR(m_closeTable);
			}
			return result;
		}

		protected void RenderZoom()
		{
			if (m_deviceInfo.Zoom != 100)
			{
				WriteStream(m_openStyle);
				WriteStream("zoom:");
				WriteStream(m_deviceInfo.Zoom.ToString(CultureInfo.InvariantCulture));
				WriteStream("%\"");
			}
		}

		protected void PredefinedStyles()
		{
			PredefinedStyles(m_deviceInfo, this, m_styleClassPrefix);
		}

		internal static void PredefinedStyles(DeviceInfo m_deviceInfo, IHtmlRenderer writer)
		{
			PredefinedStyles(m_deviceInfo, writer, null);
		}

		internal static void PredefinedStyles(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix)
		{
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentSizes);
			writer.WriteStream(m_styleHeight);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_styleWidth);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentSizesOverflow);
			writer.WriteStream(m_styleHeight);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_styleWidth);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_overflowHidden);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentHeight);
			writer.WriteStream(m_styleHeight);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorder);
			writer.WriteStream(m_borderStyle);
			writer.WriteStream(m_none);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderL);
			writer.WriteStream(m_borderLeftStyle);
			writer.WriteStream(m_none);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderR);
			writer.WriteStream(m_borderRightStyle);
			writer.WriteStream(m_none);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderT);
			writer.WriteStream(m_borderTopStyle);
			writer.WriteStream(m_none);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_ignoreBorderB);
			writer.WriteStream(m_borderBottomStyle);
			writer.WriteStream(m_none);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_layoutBorder);
			writer.WriteStream(m_borderCollapse);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_layoutFixed);
			writer.WriteStream(m_borderCollapse);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_tableLayoutFixed);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentWidthOverflow);
			writer.WriteStream(m_styleWidth);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_overflowXHidden);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_popupAction);
			writer.WriteStream("position:absolute;display:none;background-color:white;border:1px solid black;");
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_styleAction);
			writer.WriteStream("text-decoration:none;color:black;cursor:pointer;");
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_emptyTextBox);
			writer.WriteStream(m_fontSize);
			writer.WriteStream(deviceInfo.IsBrowserGeckoEngine ? m_smallPoint : m_zeroPoint);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_rtlEmbed);
			writer.WriteStream(m_direction);
			writer.WriteStream("RTL;");
			writer.WriteStream(m_unicodeBiDi);
			writer.WriteStream(EnumStrings.GetValue(RPLFormat.UnicodeBiDiTypes.Embed));
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_noVerticalMarginClassName);
			writer.WriteStream(m_marginTop);
			writer.WriteStream(m_zeroPoint);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_marginBottom);
			writer.WriteStream(m_zeroPoint);
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentSizeInlineTable);
			writer.WriteStream(m_styleHeight);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream(m_styleWidth);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, m_percentHeightInlineTable);
			writer.WriteStream(m_styleHeight);
			writer.WriteStream(m_percent);
			writer.WriteStream(m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(m_closeAccol);
			if (classStylePrefix != null)
			{
				writer.WriteStream(classStylePrefix);
			}
			writer.WriteStream(" * { ");
			string value = null;
			if (deviceInfo.IsBrowserSafari)
			{
				value = "-webkit-";
			}
			else if (deviceInfo.IsBrowserGeckoEngine)
			{
				value = "-moz-";
			}
			if (!string.IsNullOrEmpty(value))
			{
				writer.WriteStream(value);
				writer.WriteStream("box-sizing: border-box; ");
			}
			writer.WriteStream("box-sizing: border-box }");
		}

		private static void StartPredefinedStyleClass(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix, byte[] className)
		{
			if (classStylePrefix != null)
			{
				writer.WriteStream(classStylePrefix);
			}
			writer.WriteStream(m_dot);
			writer.WriteStream(deviceInfo.HtmlPrefixId);
			writer.WriteStream(className);
			writer.WriteStream(m_openAccol);
		}

		private void CheckBodyStyle()
		{
			RPLElementStyle style = m_pageContent.PageLayout.Style;
			string text = (string)style[34];
			m_pageHasStyle = (text != null || style[33] != null || ReportPageHasBorder(style, text));
		}

		private bool ReportPageBorder(IRPLStyle pageStyle, Border border, string backgroundColor)
		{
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			bool result = false;
			string text = null;
			string text2 = null;
			switch (border)
			{
			case Border.All:
				b = 10;
				b2 = 5;
				b3 = 0;
				break;
			case Border.Bottom:
				b = 14;
				b2 = 9;
				b3 = 4;
				break;
			case Border.Left:
				b = 11;
				b2 = 6;
				b3 = 1;
				break;
			case Border.Right:
				b = 12;
				b2 = 7;
				b3 = 2;
				break;
			default:
				b = 13;
				b2 = 8;
				b3 = 3;
				break;
			}
			object obj = pageStyle[b2];
			if (obj != null && (RPLFormat.BorderStyles)obj != 0)
			{
				text = (string)pageStyle[b];
				if (text != null && new RPLReportSize(text).ToMillimeters() > 0.0)
				{
					text2 = (string)pageStyle[b3];
					if (text2 != backgroundColor)
					{
						result = true;
					}
				}
			}
			return result;
		}

		private void BorderBottomAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(m_borderBottomColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(m_borderBottomStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(m_borderBottomWidth);
			}
		}

		private void BorderLeftAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(m_borderLeftColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(m_borderLeftStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(m_borderLeftWidth);
			}
		}

		private void BorderRightAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(m_borderRightColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(m_borderRightStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(m_borderRightWidth);
			}
		}

		private void BorderTopAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(m_borderTopColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(m_borderTopStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(m_borderTopWidth);
			}
		}

		private void BorderAllAtribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(m_borderColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(m_borderStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(m_borderWidth);
			}
		}

		private void RenderBorder(object styleAttribute, Border border, BorderAttribute borderAttribute)
		{
			if (styleAttribute != null)
			{
				switch (border)
				{
				case Border.All:
					BorderAllAtribute(borderAttribute);
					break;
				case Border.Bottom:
					BorderBottomAttribute(borderAttribute);
					break;
				case Border.Right:
					BorderRightAttribute(borderAttribute);
					break;
				case Border.Top:
					BorderTopAttribute(borderAttribute);
					break;
				default:
					BorderLeftAttribute(borderAttribute);
					break;
				}
				WriteStream(styleAttribute);
				WriteStream(m_semiColon);
			}
		}

		private void RenderBorderStyle(object width, object style, object color, Border border)
		{
			if (width == null && color == null && style == null)
			{
				return;
			}
			if (width != null && color != null && style != null)
			{
				string value = EnumStrings.GetValue((RPLFormat.BorderStyles)style);
				switch (border)
				{
				case Border.All:
					WriteStream(m_border);
					break;
				case Border.Bottom:
					WriteStream(m_borderBottom);
					break;
				case Border.Left:
					WriteStream(m_borderLeft);
					break;
				case Border.Right:
					WriteStream(m_borderRight);
					break;
				default:
					WriteStream(m_borderTop);
					break;
				}
				WriteStream(width);
				WriteStream(m_space);
				WriteStream(value);
				WriteStream(m_space);
				WriteStream(color);
				WriteStream(m_semiColon);
			}
			else
			{
				RenderBorder(color, border, BorderAttribute.BorderColor);
				if (style != null)
				{
					string value2 = EnumStrings.GetValue((RPLFormat.BorderStyles)style);
					RenderBorder(value2, border, BorderAttribute.BorderStyle);
				}
				RenderBorder(width, border, BorderAttribute.BorderWidth);
			}
		}

		protected bool BorderInstance(IRPLStyle reportItemStyle, object defWidth, object defStyle, object defColor, ref object borderWidth, ref object borderStyle, ref object borderColor, Border border)
		{
			byte styleName = 0;
			byte styleName2 = 0;
			byte styleName3 = 0;
			switch (border)
			{
			case Border.Bottom:
				styleName = 14;
				styleName2 = 9;
				styleName3 = 4;
				break;
			case Border.Left:
				styleName = 11;
				styleName2 = 6;
				styleName3 = 1;
				break;
			case Border.Right:
				styleName = 12;
				styleName2 = 7;
				styleName3 = 2;
				break;
			case Border.Top:
				styleName = 13;
				styleName2 = 8;
				styleName3 = 3;
				break;
			}
			if (reportItemStyle != null)
			{
				borderStyle = reportItemStyle[styleName2];
			}
			if (borderStyle == null)
			{
				borderStyle = defStyle;
			}
			if (borderStyle != null && (RPLFormat.BorderStyles)borderStyle == RPLFormat.BorderStyles.None)
			{
				return false;
			}
			object obj = reportItemStyle[styleName];
			if (obj == null)
			{
				borderWidth = defWidth;
			}
			else
			{
				borderWidth = obj;
			}
			object obj2 = reportItemStyle[styleName3];
			if (obj2 == null)
			{
				borderColor = defColor;
			}
			else
			{
				borderColor = obj2;
			}
			if (borderStyle == null && obj == null)
			{
				return obj2 != null;
			}
			return true;
		}

		private bool RenderBorderInstance(IRPLStyle reportItemStyle, object defWidth, object defStyle, object defColor, Border border)
		{
			object borderWidth = null;
			object borderColor = null;
			object borderStyle = null;
			bool flag = BorderInstance(reportItemStyle, defWidth, defStyle, defColor, ref borderWidth, ref borderStyle, ref borderColor, border);
			if (flag)
			{
				RenderBorderStyle(borderWidth, borderStyle, borderColor, border);
			}
			return flag;
		}

		private bool OnlyGeneralBorder(IRPLStyle style)
		{
			bool result = true;
			if (style[1] != null || style[11] != null || style[6] != null || style[3] != null || style[13] != null || style[8] != null || style[2] != null || style[12] != null || style[7] != null || style[4] != null || style[14] != null || style[9] != null)
			{
				result = false;
			}
			return result;
		}

		protected string CreateImageStream(RPLImageData image)
		{
			if (image.ImageName == null)
			{
				return null;
			}
			if (image.IsShared && m_images.ContainsKey(image.ImageName))
			{
				return image.ImageName;
			}
			if (m_createSecondaryStreams == SecondaryStreams.Embedded)
			{
				Stream stream = CreateStream(image.ImageName, string.Empty, null, image.ImageMimeType, willSeek: false, StreamOper.CreateAndRegister);
				long imageDataOffset = image.ImageDataOffset;
				if (imageDataOffset >= 0)
				{
					m_rplReport.GetImage(imageDataOffset, stream);
				}
				else if (image.ImageData != null)
				{
					stream.Write(image.ImageData, 0, image.ImageData.Length);
				}
			}
			if (image.IsShared)
			{
				m_images.Add(image.ImageName, null);
			}
			return image.ImageName;
		}

		private void RenderAtStart(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
		{
			if (!(renderSort || renderToggle))
			{
				return;
			}
			object obj = style[26];
			RPLFormat.VerticalAlignments verticalAlignments = RPLFormat.VerticalAlignments.Top;
			if (obj != null)
			{
				verticalAlignments = (RPLFormat.VerticalAlignments)obj;
			}
			if (IsWritingModeVertical(style) && m_deviceInfo.IsBrowserIE)
			{
				WriteStream(m_openStyle);
				WriteStream(m_textAlign);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					WriteStream(m_rightValue);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					WriteStream(m_centerValue);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					WriteStream(m_leftValue);
					break;
				}
				WriteStream(m_quote);
				WriteStream(m_closeBracket);
				if (renderSort)
				{
					RenderSortImage(textBoxProps);
				}
				if (renderToggle)
				{
					RenderToggleImage(textBoxProps);
				}
				WriteStream(m_closeTD);
				WriteStream(m_closeTR);
				WriteStream(m_firstTD);
			}
			else
			{
				WriteStream(m_openStyle);
				WriteStream(m_verticalAlign);
				WriteStream(EnumStrings.GetValue(verticalAlignments));
				WriteStream(m_quote);
				WriteStream(m_closeBracket);
				if (renderSort)
				{
					RenderSortImage(textBoxProps);
				}
				if (renderToggle)
				{
					RenderToggleImage(textBoxProps);
				}
				WriteStream(m_closeTD);
				WriteStream(m_openTD);
			}
		}

		private void RenderAtEnd(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
		{
			if (!(renderSort || renderToggle))
			{
				return;
			}
			object obj = style[26];
			RPLFormat.VerticalAlignments verticalAlignments = RPLFormat.VerticalAlignments.Top;
			if (obj != null)
			{
				verticalAlignments = (RPLFormat.VerticalAlignments)obj;
			}
			WriteStream(m_closeTD);
			if (IsWritingModeVertical(style) && m_deviceInfo.IsBrowserIE)
			{
				WriteStream(m_closeTR);
				WriteStream(m_firstTD);
				WriteStream(m_openStyle);
				WriteStream(m_textAlign);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					WriteStream(m_rightValue);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					WriteStream(m_centerValue);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					WriteStream(m_leftValue);
					break;
				}
				WriteStream(m_quote);
			}
			else
			{
				WriteStream(m_openTD);
				WriteStream(m_openStyle);
				WriteStream(m_verticalAlign);
				WriteStream(EnumStrings.GetValue(verticalAlignments));
				WriteStream(m_quote);
			}
			WriteStream(m_closeBracket);
			if (renderSort)
			{
				RenderSortImage(textBoxProps);
			}
			if (renderToggle)
			{
				RenderToggleImage(textBoxProps);
			}
		}

		private bool RenderHyperlink(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			WriteStream(m_openA);
			RenderTabIndex();
			RenderActionHref(action, textDec, color);
			WriteStream(m_closeBracket);
			return true;
		}

		private void RenderTabIndex()
		{
			WriteStream(m_tabIndex);
			WriteStream(++m_tabIndexNum);
			WriteStream(m_quote);
		}

		private bool HasAction(RPLAction action)
		{
			if (action.BookmarkLink == null && action.DrillthroughId == null && action.DrillthroughUrl == null)
			{
				return action.Hyperlink != null;
			}
			return true;
		}

		private bool HasAction(RPLActionInfo actionInfo)
		{
			if (actionInfo != null && actionInfo.Actions != null)
			{
				return HasAction(actionInfo.Actions[0]);
			}
			return false;
		}

		protected abstract void RenderInteractionAction(RPLAction action, ref bool hasHref);

		private bool RenderActionHref(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			bool hasHref = false;
			if (action.Hyperlink != null)
			{
				WriteStream(m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + m_quoteString);
				hasHref = true;
			}
			else
			{
				RenderInteractionAction(action, ref hasHref);
			}
			if (textDec != RPLFormat.TextDecorations.Underline)
			{
				OpenStyle();
				WriteStream(m_textDecoration);
				WriteStream(m_none);
				WriteStream(m_semiColon);
			}
			if (color != null)
			{
				OpenStyle();
				WriteStream(m_color);
				WriteStream(color);
			}
			CloseStyle(renderQuote: true);
			if (m_deviceInfo.LinkTarget != null)
			{
				WriteStream(m_target);
				WriteStream(m_deviceInfo.LinkTarget);
				WriteStream(m_quote);
			}
			return hasHref;
		}

		protected void RenderControlActionScript(RPLAction action)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			if (action.DrillthroughId != null)
			{
				QuoteString(stringBuilder, action.DrillthroughId);
				text = "Drillthrough";
			}
			else
			{
				QuoteString(stringBuilder, action.BookmarkLink);
				text = "Bookmark";
			}
			RenderOnClickActionScript(text, stringBuilder.ToString());
		}

		internal static bool IsDirectionRTL(IRPLStyle style)
		{
			object obj = style[29];
			if (obj != null)
			{
				return (RPLFormat.Directions)obj == RPLFormat.Directions.RTL;
			}
			return false;
		}

		internal static bool IsWritingModeVertical(IRPLStyle style)
		{
			if (style == null)
			{
				return false;
			}
			object obj = style[30];
			if (obj != null)
			{
				return IsWritingModeVertical((RPLFormat.WritingModes)obj);
			}
			return false;
		}

		internal static bool IsWritingModeVertical(RPLFormat.WritingModes writingMode)
		{
			if (writingMode != RPLFormat.WritingModes.Vertical)
			{
				return writingMode == RPLFormat.WritingModes.Rotate270;
			}
			return true;
		}

		internal static bool HasHorizontalPaddingStyles(IRPLStyle style)
		{
			if (style != null)
			{
				if (style[15] == null)
				{
					return style[16] != null;
				}
				return true;
			}
			return false;
		}

		private void PercentSizes()
		{
			WriteStream(m_openStyle);
			WriteStream(m_styleHeight);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			WriteStream(m_styleWidth);
			WriteStream(m_percent);
			WriteStream(m_quote);
		}

		private void PercentSizesOverflow()
		{
			WriteStream(m_openStyle);
			WriteStream(m_styleHeight);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			WriteStream(m_styleWidth);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			WriteStream(m_overflowHidden);
			WriteStream(m_quote);
		}

		private void ClassLayoutBorder()
		{
			WriteClassName(m_layoutBorder, m_classLayoutBorder);
		}

		private void ClassPercentSizes()
		{
			WriteClassName(m_percentSizes, m_classPercentSizes);
		}

		private void ClassPercentSizesOverflow()
		{
			WriteClassName(m_percentSizesOverflow, m_classPercentSizesOverflow);
		}

		private void ClassPercentHeight()
		{
			WriteClassName(m_percentHeight, m_classPercentHeight);
		}

		private void RenderLanguage(string language)
		{
			if (!string.IsNullOrEmpty(language))
			{
				WriteStream(m_language);
				WriteAttrEncoded(language);
				WriteStream(m_quote);
			}
		}

		private void RenderReportLanguage()
		{
			RenderLanguage(m_contextLanguage);
		}

		private bool InitFixedColumnHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.RowHeights.Length; i++)
			{
				if (tablix.FixedRow(i))
				{
					storage.HtmlId = tablixID;
					storage.ColumnHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private bool InitFixedRowHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				if (tablix.FixedColumns[i])
				{
					storage.HtmlId = tablixID;
					storage.RowHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private void RenderVMLLine(RPLLine line, RPLItemMeasurement measurement, StyleContext styleContext)
		{
			if (!m_hasSlantedLines)
			{
				WriteStream("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
				m_hasSlantedLines = true;
			}
			WriteStream(m_openVGroup);
			WriteStream(m_openStyle);
			WriteStream(m_styleWidth);
			if (styleContext.InTablix)
			{
				WriteStream(m_percent);
				WriteStream(m_semiColon);
				WriteStream(m_styleHeight);
				WriteStream(m_percent);
			}
			else
			{
				WriteRSStream(measurement.Width);
				WriteStream(m_semiColon);
				WriteStream(m_styleHeight);
				WriteRSStream(measurement.Height);
			}
			WriteStream(m_closeQuote);
			WriteStream(m_openVLine);
			if (((RPLLinePropsDef)line.ElementProps.Definition).Slant)
			{
				WriteStream(m_rightSlant);
			}
			else
			{
				WriteStream(m_leftSlant);
			}
			IRPLStyle style = line.ElementProps.Style;
			string text = (string)style[0];
			string text2 = (string)style[10];
			if (text != null && text2 != null)
			{
				int value = new RPLReportColor(text).ToColor().ToArgb() & 0xFFFFFF;
				WriteStream(m_strokeColor);
				WriteStream("#");
				WriteStream(Convert.ToString(value, 16));
				WriteStream(m_quote);
				WriteStream(m_strokeWeight);
				WriteStream(text2);
				WriteStream(m_closeQuote);
			}
			string theString = "solid";
			string text3 = null;
			object obj = style[5];
			if (obj != null)
			{
				string value2 = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				if (string.CompareOrdinal(value2, "dashed") == 0)
				{
					theString = "dash";
				}
				else if (string.CompareOrdinal(value2, "dotted") == 0)
				{
					theString = "dot";
				}
				if (string.CompareOrdinal(value2, "double") == 0)
				{
					text3 = "thinthin";
				}
			}
			WriteStream(m_dashStyle);
			WriteStream(theString);
			if (text3 != null)
			{
				WriteStream(m_quote);
				WriteStream(m_slineStyle);
				WriteStream(text3);
			}
			WriteStream(m_quote);
			WriteStream(m_closeTag);
			WriteStreamCR(m_closeVGroup);
		}

		private List<string> RenderTableCellBorder(PageTableCell currCell, Hashtable renderedLines)
		{
			RPLLine rPLLine = null;
			List<string> list = new List<string>(4);
			if (m_isStyleOpen)
			{
				WriteStream(m_semiColon);
			}
			else
			{
				OpenStyle();
			}
			WriteStream(m_zeroBorderWidth);
			rPLLine = currCell.BorderLeft;
			if (rPLLine != null)
			{
				WriteStream(m_semiColon);
				WriteStream(m_borderLeft);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderRight;
			if (rPLLine != null)
			{
				WriteStream(m_semiColon);
				WriteStream(m_borderRight);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderTop;
			if (rPLLine != null)
			{
				WriteStream(m_semiColon);
				WriteStream(m_borderTop);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderBottom;
			if (rPLLine != null)
			{
				WriteStream(m_semiColon);
				WriteStream(m_borderBottom);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			return list;
		}

		private void CheckForLineID(RPLLine line, List<string> lineIDs, Hashtable renderedLines)
		{
			RPLElementProps elementProps = line.ElementProps;
			string uniqueName = elementProps.UniqueName;
			if (!renderedLines.ContainsKey(uniqueName))
			{
				if (NeedReportItemId(line, elementProps))
				{
					lineIDs.Add(elementProps.UniqueName);
				}
				renderedLines.Add(uniqueName, uniqueName);
			}
		}

		private int GenerateTableLayoutContent(PageTableLayout rgTableGrid, RPLItemMeasurement[] repItemCol, bool bfZeroRowReq, bool bfZeroColReq, bool renderHeight, int borderContext, bool layoutExpand, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style)
		{
			int num = 0;
			int i = 0;
			int num2 = 1;
			int num3 = 1;
			int num4 = 0;
			int skipHeight = 0;
			bool flag = false;
			bool flag2 = true;
			PageTableCell pageTableCell = null;
			PageTableCell pageTableCell2 = null;
			Hashtable renderedLines = new Hashtable();
			int nrRows = rgTableGrid.NrRows;
			int nrCols = rgTableGrid.NrCols;
			int num5 = 0;
			int result = 0;
			bool flag3 = true;
			object defaultBorderStyle = null;
			object specificBorderStyle = null;
			object specificBorderStyle2 = null;
			object specificBorderStyle3 = null;
			object specificBorderStyle4 = null;
			object defaultBorderWidth = null;
			object specificBorderWidth = null;
			object specificBorderWidth2 = null;
			object specificBorderWidth3 = null;
			object specificBorderWidth4 = null;
			if (style != null)
			{
				defaultBorderStyle = style[5];
				specificBorderStyle = style[6];
				specificBorderStyle2 = style[7];
				specificBorderStyle3 = style[8];
				specificBorderStyle4 = style[9];
				defaultBorderWidth = style[10];
				specificBorderWidth = style[11];
				specificBorderWidth2 = style[12];
				specificBorderWidth3 = style[13];
				specificBorderWidth4 = style[14];
			}
			for (; i < nrRows; i++)
			{
				num4 = nrCols * i;
				pageTableCell = rgTableGrid.GetCell(num4);
				flag = rgTableGrid.EmptyRow(repItemCol, ignoreLines: false, num4, renderHeight, ref skipHeight);
				WriteStream(m_openTR);
				if (!flag)
				{
					WriteStream(m_valign);
					WriteStream(m_topValue);
					WriteStream(m_quote);
				}
				WriteStream(m_closeBracket);
				flag3 = true;
				for (num = 0; num < nrCols; num++)
				{
					int num6 = num + num4;
					bool flag4 = num == 0;
					if (flag4 && bfZeroColReq)
					{
						WriteStream(m_openTD);
						if (renderHeight || skipHeight <= 0)
						{
							WriteStream(m_openStyle);
							if (m_deviceInfo.OutlookCompat)
							{
								for (int j = 0; j < nrCols; j++)
								{
									pageTableCell2 = rgTableGrid.GetCell(num4 + j);
									if (!pageTableCell2.NeedsRowHeight)
									{
										flag3 = false;
										break;
									}
								}
							}
							if (flag3)
							{
								WriteStream(m_styleHeight);
								float num7 = pageTableCell.DYValue.Value;
								if (num7 > 0f)
								{
									if (i == 0)
									{
										num7 = SubtractBorderStyles(num7, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
									}
									if (i == rgTableGrid.NrRows - num2)
									{
										num7 = SubtractBorderStyles(num7, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
									}
									if (num7 <= 0f)
									{
										num7 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : pageTableCell.DYValue.Value);
									}
								}
								WriteDStream(num7);
								WriteStream(m_mm);
								WriteStream(m_semiColon);
							}
							WriteStream(m_styleWidth);
							WriteDStream(0f);
							WriteStream(m_mm);
							WriteStream(m_quote);
						}
						else
						{
							WriteStream(m_openStyle);
							WriteStream(m_styleWidth);
							WriteDStream(0f);
							WriteStream(m_mm);
							WriteStream(m_quote);
						}
						WriteStream(m_closeBracket);
						if (omittedHeaders != null)
						{
							for (int k = 0; k < omittedHeaders.Count; k++)
							{
								if (omittedHeaders[k].GroupLabel != null)
								{
									RenderNavigationId(omittedHeaders[k].UniqueName);
								}
							}
						}
						WriteStream(m_closeTD);
					}
					pageTableCell2 = rgTableGrid.GetCell(num6);
					if (pageTableCell2.Eaten)
					{
						continue;
					}
					if (!pageTableCell2.InUse)
					{
						MergeEmptyCells(rgTableGrid, num, i, num4, flag2, pageTableCell2, nrRows, nrCols, num6);
					}
					WriteStream(m_openTD);
					num2 = pageTableCell2.RowSpan;
					if (num2 != 1)
					{
						WriteStream(m_rowSpan);
						WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						WriteStream(m_quote);
					}
					if (!flag2 || bfZeroRowReq || layoutState == SharedListLayoutState.Continue || layoutState == SharedListLayoutState.End)
					{
						num3 = pageTableCell2.ColSpan;
						if (num3 != 1)
						{
							WriteStream(m_colSpan);
							WriteStream(num3.ToString(CultureInfo.InvariantCulture));
							WriteStream(m_quote);
						}
					}
					if (flag4 && !bfZeroColReq && (renderHeight || skipHeight <= 0))
					{
						float num8 = pageTableCell.DYValue.Value;
						if (num8 >= 0f && flag3 && (!(i == nrRows - 1 && flag) || layoutState != 0) && (!m_deviceInfo.OutlookCompat || pageTableCell2.NeedsRowHeight))
						{
							OpenStyle();
							WriteStream(m_styleHeight);
							if (i == 0)
							{
								num8 = SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
							}
							if (i == rgTableGrid.NrRows - num2)
							{
								num8 = SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
							}
							if (num8 <= 0f)
							{
								num8 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : pageTableCell.DYValue.Value);
							}
							WriteDStream(num8);
							WriteStream(m_mm);
						}
					}
					if (m_deviceInfo.OutlookCompat || (flag2 && !bfZeroRowReq && (layoutState == SharedListLayoutState.Start || layoutState == SharedListLayoutState.None)))
					{
						float num9 = 0f;
						for (int l = 0; l < num3; l++)
						{
							num9 += rgTableGrid.GetCell(num + l).DXValue.Value;
						}
						float num10 = num9;
						if (m_isStyleOpen)
						{
							WriteStream(m_semiColon);
						}
						else
						{
							OpenStyle();
						}
						WriteStream(m_styleWidth);
						if (num10 > 0f)
						{
							if (num == 0)
							{
								num10 = SubtractBorderStyles(num10, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
							}
							if (num == rgTableGrid.NrCols - num3)
							{
								num10 = SubtractBorderStyles(num10, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
							}
							if (num10 <= 0f)
							{
								num10 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : num9);
							}
						}
						WriteDStream(num10);
						WriteStream(m_mm);
						WriteStream(m_semiColon);
						WriteStream(m_styleMinWidth);
						WriteDStream(num10);
						WriteStream(m_mm);
						WriteStream(m_semiColon);
						if (flag3 && !pageTableCell2.InUse && m_deviceInfo.OutlookCompat)
						{
							float num11 = pageTableCell2.DYValue.Value;
							if (num11 < 558.8f)
							{
								WriteStream(m_styleHeight);
								if (num11 > 0f)
								{
									if (i == 0)
									{
										num11 = SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
									}
									if (i == rgTableGrid.NrRows - num2)
									{
										num11 = SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
									}
									if (num11 <= 0f)
									{
										num11 = ((m_deviceInfo.BrowserMode != BrowserMode.Standards || !m_deviceInfo.IsBrowserIE) ? 1f : pageTableCell2.DYValue.Value);
									}
								}
								WriteDStream(num11);
								WriteStream(m_mm);
								WriteStream(m_semiColon);
							}
						}
					}
					List<string> list = null;
					if (pageTableCell2.HasBorder)
					{
						list = RenderTableCellBorder(pageTableCell2, renderedLines);
					}
					if (m_isStyleOpen)
					{
						CloseStyle(renderQuote: false);
						WriteStream(m_closeQuote);
					}
					else
					{
						WriteStream(m_closeBracket);
					}
					if (flag4 && !bfZeroColReq && omittedHeaders != null)
					{
						for (int m = 0; m < omittedHeaders.Count; m++)
						{
							if (omittedHeaders[m].GroupLabel != null)
							{
								RenderNavigationId(omittedHeaders[m].UniqueName);
							}
						}
					}
					if (list != null && list.Count > 0)
					{
						for (int n = 0; n < list.Count; n++)
						{
							RenderNavigationId(list[n]);
						}
					}
					if (pageTableCell2.InUse)
					{
						int num12 = nrRows - pageTableCell2.RowSpan + 1;
						if (num12 == i + 1 && pageTableCell2.KeepBottomBorder)
						{
							num12++;
						}
						int num13 = nrCols - pageTableCell2.ColSpan + 1;
						if (num13 == num + 1 && pageTableCell2.KeepRightBorder)
						{
							num13++;
						}
						num5 = GetNewContext(borderContext, i + 1, num + 1, num12, num13);
						if ((num5 & 8) > 0 && pageTableCell2.Measurement != null)
						{
							float height = pageTableCell2.Measurement.Height;
							float num14 = pageTableCell2.DYValue.Value;
							for (int num15 = 1; num15 < pageTableCell2.RowSpan; num15++)
							{
								num14 += rgTableGrid.GetCell(num6 + num15 * rgTableGrid.NrCols).DYValue.Value;
							}
							if (height < num14)
							{
								num5 &= -9;
							}
						}
						if ((num5 & 2) > 0 && pageTableCell2.Measurement != null)
						{
							float width = pageTableCell2.Measurement.Width;
							float num16 = pageTableCell2.DXValue.Value;
							for (int num17 = 1; num17 < pageTableCell2.ColSpan; num17++)
							{
								num16 += rgTableGrid.GetCell(num6 + num17).DXValue.Value;
							}
							if (width < num16)
							{
								num5 &= -3;
							}
						}
						RenderCellItem(pageTableCell2, num5, layoutExpand);
					}
					else if (!m_browserIE && pageTableCell2.HasBorder && pageTableCell2.BorderTop == null && pageTableCell2.BorderBottom == null && pageTableCell2.BorderLeft == null && pageTableCell2.BorderRight == null)
					{
						RenderBlankImage();
					}
					WriteStream(m_closeTD);
				}
				WriteStream(m_closeTR);
				flag2 = false;
				skipHeight--;
			}
			return result;
		}

		private static void MergeEmptyCells(PageTableLayout rgTableGrid, int x, int y, int currRow, bool firstRow, PageTableCell currCell, int numRows, int numCols, int index)
		{
			int num = index + 1;
			int num2 = currRow + numCols;
			if (currCell.BorderLeft == null && !firstRow)
			{
				while (num < num2)
				{
					PageTableCell cell = rgTableGrid.GetCell(num++);
					if (cell.Eaten || cell.InUse || cell.BorderTop != currCell.BorderTop || cell.BorderBottom != currCell.BorderBottom || cell.BorderLeft != null)
					{
						break;
					}
					cell.Eaten = true;
					currCell.ColSpan++;
					currCell.BorderRight = cell.BorderRight;
				}
			}
			int num3 = index;
			int num4 = y + 1;
			num = numCols * num4 + x;
			num2 = numCols * numRows;
			while (num < num2)
			{
				PageTableCell cell2 = rgTableGrid.GetCell(num);
				if (cell2.Eaten || cell2.InUse || cell2.BorderLeft != currCell.BorderLeft || cell2.BorderRight != currCell.BorderRight || cell2.BorderTop != null || (currCell.ColSpan == 1 && currCell.BorderLeft == null && currCell.BorderRight == null))
				{
					break;
				}
				int i = 1;
				PageTableCell pageTableCell = cell2;
				for (; i < currCell.ColSpan; i++)
				{
					PageTableCell cell3 = rgTableGrid.GetCell(num3 + i);
					PageTableCell cell4 = rgTableGrid.GetCell(num + i);
					if (cell4.InUse || cell4.Eaten || cell4.BorderLeft != null || cell4.BorderRight != cell3.BorderRight || cell4.BorderTop != null || cell4.BorderBottom != pageTableCell.BorderBottom)
					{
						break;
					}
					pageTableCell = cell4;
				}
				if (i == currCell.ColSpan)
				{
					currCell.RowSpan++;
					currCell.BorderBottom = cell2.BorderBottom;
					for (i = 0; i < currCell.ColSpan; i++)
					{
						PageTableCell cell5 = rgTableGrid.GetCell(num + i);
						cell5.Eaten = true;
					}
					num3 = num;
					num4++;
					num = numCols * num4 + x;
					continue;
				}
				break;
			}
		}

		private void RenderIE7WritingMode(RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, StyleContext styleContext)
		{
			WriteStream(m_writingMode);
			if (IsWritingModeVertical(writingMode))
			{
				if (direction == RPLFormat.Directions.RTL)
				{
					WriteStream(m_btrl);
				}
				else
				{
					WriteStream(m_tbrl);
				}
				if (writingMode == RPLFormat.WritingModes.Rotate270)
				{
					WriteRotate270(m_deviceInfo, styleContext, WriteStream);
				}
			}
			else if (direction == RPLFormat.Directions.RTL)
			{
				WriteStream(m_rltb);
			}
			else
			{
				WriteStream(m_lrtb);
			}
			WriteStream(m_semiColon);
		}

		internal static void WriteRotate270(DeviceInfo deviceInfo, StyleContext styleContext, Action<byte[]> WriteStream)
		{
			if (deviceInfo.IsBrowserIE && styleContext != null && !styleContext.StyleOnCell)
			{
				if (!styleContext.RotationApplied)
				{
					WriteStream(m_semiColon);
					WriteStream(m_filter);
					WriteStream(m_basicImageRotation180);
					styleContext.RotationApplied = true;
				}
				if (deviceInfo.OutlookCompat)
				{
					WriteStream(m_semiColon);
					WriteStream(m_msoRotation);
					WriteStream(m_degree90);
				}
			}
		}

		private void RenderDirectionStyles(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, bool isNonSharedStyles, StyleContext styleContext)
		{
			IRPLStyle iRPLStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
			bool flag = HasHorizontalPaddingStyles(sharedStyleProps);
			bool flag2 = HasHorizontalPaddingStyles(nonSharedStyleProps);
			object obj = iRPLStyle[29];
			RPLFormat.Directions? directions = null;
			if (obj != null)
			{
				directions = (RPLFormat.Directions)obj;
				obj = EnumStrings.GetValue(directions.Value);
				WriteStream(m_direction);
				WriteStream(obj);
				WriteStream(m_semiColon);
			}
			obj = iRPLStyle[30];
			RPLFormat.WritingModes? writingModes = null;
			if (obj != null)
			{
				writingModes = (RPLFormat.WritingModes)obj;
				WriteStream(m_layoutFlow);
				if (IsWritingModeVertical(writingModes.Value))
				{
					WriteStream(m_verticalIdeographic);
				}
				else
				{
					WriteStream(m_horizontal);
				}
				WriteStream(m_semiColon);
				if (m_deviceInfo.IsBrowserIE && IsWritingModeVertical(writingModes.Value) && measurement != null && reportItem is RPLTextBox)
				{
					RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
					float height = measurement.Height;
					float num = measurement.Width;
					float adjustedWidth = GetAdjustedWidth(measurement, props.Style);
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						num = adjustedWidth;
						height = GetAdjustedHeight(measurement, props.Style);
					}
					if (rPLTextBoxPropsDef.CanGrow)
					{
						if (styleContext != null && styleContext.InTablix && !m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							obj = null;
							if (flag2)
							{
								obj = nonSharedStyleProps[15];
							}
							if (obj == null && flag)
							{
								obj = sharedStyleProps[15];
							}
							if (obj != null)
							{
								RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
								float num2 = (float)rPLReportSize.ToMillimeters();
								num -= num2;
							}
							obj = null;
							if (flag2)
							{
								obj = nonSharedStyleProps[16];
							}
							if (obj == null && flag)
							{
								obj = sharedStyleProps[16];
							}
							if (obj != null)
							{
								RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
								float num3 = (float)rPLReportSize2.ToMillimeters();
								num += num3;
							}
						}
						RenderMeasurementWidth((num >= 0f) ? num : 0f);
					}
					else
					{
						WriteStream(m_overflowHidden);
						WriteStream(m_semiColon);
						RenderMeasurementWidth(num, renderMinWidth: false);
						RenderMeasurementHeight(height);
					}
					RenderMeasurementMinWidth(adjustedWidth);
				}
			}
			if (writingModes.HasValue && directions.HasValue)
			{
				RenderIE7WritingMode(writingModes.Value, directions.Value, styleContext);
			}
			else if ((writingModes.HasValue || directions.HasValue) && isNonSharedStyles)
			{
				if (!writingModes.HasValue)
				{
					obj = definition.SharedStyle[30];
					writingModes = (RPLFormat.WritingModes)obj;
				}
				else if (!directions.HasValue)
				{
					obj = definition.SharedStyle[29];
					directions = (RPLFormat.Directions)obj;
				}
				RenderIE7WritingMode(writingModes.Value, directions.Value, styleContext);
			}
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RenderReportItemStyle(reportItem, elementProps, definition, measurement, new StyleContext(), ref borderContext, definition.ID);
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext, StyleContext styleContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RenderReportItemStyle(reportItem, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLStyleProps nonSharedStyle, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
		{
			if (m_useInlineStyle)
			{
				OpenStyle();
				RPLElementStyle sharedStyleProps = new RPLElementStyle(nonSharedStyle, sharedStyle);
				RenderStyleProps(reportItem, elementProps, definition, measurement, sharedStyleProps, null, styleContext, ref borderContext, isNonSharedStyles: false);
				if (styleContext.EmptyTextBox)
				{
					WriteStream(m_fontSize);
					WriteFontSizeSmallPoint();
				}
				CloseStyle(renderQuote: true);
				return;
			}
			int borderContext2 = borderContext;
			bool flag = sharedStyle != null && sharedStyle.Count > 0;
			if (nonSharedStyle != null && nonSharedStyle.Count > 0)
			{
				bool renderMeasurements = styleContext.RenderMeasurements;
				if (flag)
				{
					styleContext.RenderMeasurements = false;
				}
				OpenStyle();
				RenderStyleProps(reportItem, elementProps, definition, measurement, sharedStyle, nonSharedStyle, styleContext, ref borderContext2, isNonSharedStyles: true);
				CloseStyle(renderQuote: true);
				styleContext.RenderMeasurements = renderMeasurements;
			}
			if (flag)
			{
				byte[] array = (byte[])m_usedStyles[styleID];
				if (array == null)
				{
					if (m_onlyVisibleStyles)
					{
						int borderContext3 = 0;
						array = RenderSharedStyle(reportItem, elementProps, definition, sharedStyle, nonSharedStyle, measurement, styleID, styleContext, ref borderContext3);
					}
					else
					{
						array = m_encoding.GetBytes(styleID);
						m_usedStyles.Add(styleID, array);
					}
				}
				CloseStyle(renderQuote: true);
				WriteClassStyle(array, close: false);
				byte omitBordersState = styleContext.OmitBordersState;
				if (borderContext != 0 || omitBordersState != 0)
				{
					if (borderContext == 15)
					{
						WriteStream(m_space);
						WriteStream(m_deviceInfo.HtmlPrefixId);
						WriteStream(m_ignoreBorder);
					}
					else
					{
						if ((borderContext & 4) != 0 || (omitBordersState & 1) != 0)
						{
							WriteStream(m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(m_ignoreBorderT);
						}
						if ((borderContext & 1) != 0 || (omitBordersState & 4) != 0)
						{
							WriteStream(m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(m_ignoreBorderL);
						}
						if ((borderContext & 8) != 0 || (omitBordersState & 2) != 0)
						{
							WriteStream(m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(m_ignoreBorderB);
						}
						if ((borderContext & 2) != 0 || (omitBordersState & 8) != 0)
						{
							WriteStream(m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(m_ignoreBorderR);
						}
					}
				}
				if (styleContext.EmptyTextBox)
				{
					WriteStream(m_space);
					WriteStream(m_deviceInfo.HtmlPrefixId);
					WriteStream(m_emptyTextBox);
				}
				WriteStream(m_quote);
				if (!styleContext.NoBorders)
				{
					GetBorderContext(sharedStyle, ref borderContext, omitBordersState);
				}
			}
			borderContext |= borderContext2;
		}

		private void GetBorderContext(IRPLStyle styleProps, ref int borderContext, byte omitBordersState)
		{
			object defWidth = styleProps[10];
			object obj = styleProps[5];
			object defColor = styleProps[0];
			object borderWidth = null;
			object borderStyle = null;
			object borderColor = null;
			if (borderContext != 0 || omitBordersState != 0 || !OnlyGeneralBorder(styleProps))
			{
				if ((borderContext & 8) == 0 && (omitBordersState & 2) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Bottom))
				{
					borderContext |= 8;
				}
				if ((borderContext & 1) == 0 && (omitBordersState & 4) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Left))
				{
					borderContext |= 1;
				}
				if ((borderContext & 2) == 0 && (omitBordersState & 8) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Right))
				{
					borderContext |= 2;
				}
				if ((borderContext & 4) == 0 && (omitBordersState & 1) == 0 && BorderInstance(styleProps, defWidth, obj, defColor, ref borderWidth, ref borderStyle, ref borderColor, Border.Top))
				{
					borderContext |= 4;
				}
			}
			else if (obj != null && (RPLFormat.BorderStyles)obj != 0)
			{
				borderContext = 15;
			}
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
		{
			RenderReportItemStyle(reportItem, elementProps, definition, elementProps.NonSharedStyle, definition.SharedStyle, measurement, styleContext, ref borderContext, styleID);
		}

		private void RenderPercentSizes()
		{
			WriteStream(m_styleHeight);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
			WriteStream(m_styleWidth);
			WriteStream(m_percent);
			WriteStream(m_semiColon);
		}

		private void RenderTextAlign(RPLTextBoxProps props, RPLElementStyle style)
		{
			if (props != null)
			{
				WriteStream(m_textAlign);
				bool flag = GetTextAlignForType(props);
				if (IsDirectionRTL(style))
				{
					flag = ((!flag) ? true : false);
				}
				if (flag)
				{
					WriteStream(m_rightValue);
				}
				else
				{
					WriteStream(m_leftValue);
				}
				WriteStream(m_semiColon);
			}
		}

		internal static bool GetTextAlignForType(RPLTextBoxProps textBoxProps)
		{
			TypeCode typeCode = textBoxProps.TypeCode;
			return GetTextAlignForType(typeCode);
		}

		internal static bool GetTextAlignForType(TypeCode typeCode)
		{
			bool result = false;
			if ((uint)(typeCode - 4) <= 12u)
			{
				result = true;
			}
			return result;
		}

		private bool HasBorderStyle(object borderStyle)
		{
			if (borderStyle != null)
			{
				return (RPLFormat.BorderStyles)borderStyle != RPLFormat.BorderStyles.None;
			}
			return false;
		}

		private float SubtractBorderStyles(float width, object defaultBorderStyle, object specificBorderStyle, object defaultBorderWidth, object specificBorderWidth)
		{
			object obj = null;
			obj = specificBorderWidth;
			if (obj == null)
			{
				obj = defaultBorderWidth;
			}
			if (obj != null && (HasBorderStyle(specificBorderStyle) || HasBorderStyle(defaultBorderStyle)))
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				width -= (float)rPLReportSize.ToMillimeters();
			}
			return width;
		}

		private float GetInnerContainerWidth(RPLMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float width = measurement.Width;
			float num = 0f;
			object obj = containerStyle[15];
			if (obj != null)
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				num += (float)rPLReportSize.ToMillimeters();
			}
			obj = containerStyle[16];
			if (obj != null)
			{
				RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
				num += (float)rPLReportSize2.ToMillimeters();
			}
			return width - num;
		}

		private float GetInnerContainerWidthSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float innerContainerWidth = GetInnerContainerWidth(measurement, containerStyle);
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderWidth = containerStyle[11];
			object specificBorderStyle = containerStyle[6];
			innerContainerWidth = SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			specificBorderWidth = containerStyle[12];
			specificBorderStyle = containerStyle[7];
			innerContainerWidth = SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			if (innerContainerWidth <= 0f)
			{
				innerContainerWidth = 1f;
			}
			return innerContainerWidth;
		}

		private float GetAdjustedWidth(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float result = measurement.Width;
			if (m_deviceInfo.BrowserMode == BrowserMode.Standards || !m_deviceInfo.IsBrowserIE)
			{
				result = GetInnerContainerWidthSubtractBorders(measurement, style);
			}
			return result;
		}

		private float GetAdjustedHeight(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float result = measurement.Height;
			if (m_deviceInfo.BrowserMode == BrowserMode.Standards || !m_deviceInfo.IsBrowserIE)
			{
				result = GetInnerContainerHeightSubtractBorders(measurement, style);
			}
			return result;
		}

		private float GetInnerContainerHeight(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float height = measurement.Height;
			float num = 0f;
			object obj = containerStyle[17];
			if (obj != null)
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				num += (float)rPLReportSize.ToMillimeters();
			}
			obj = containerStyle[18];
			if (obj != null)
			{
				RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
				num += (float)rPLReportSize2.ToMillimeters();
			}
			return height - num;
		}

		private float GetInnerContainerHeightSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float innerContainerHeight = GetInnerContainerHeight(measurement, containerStyle);
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderWidth = containerStyle[13];
			object specificBorderStyle = containerStyle[8];
			innerContainerHeight = SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			specificBorderWidth = containerStyle[14];
			specificBorderStyle = containerStyle[9];
			innerContainerHeight = SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			if (innerContainerHeight <= 0f)
			{
				innerContainerHeight = 1f;
			}
			return innerContainerHeight;
		}

		private void RenderTextBoxContent(RPLTextBox textBox, RPLTextBoxProps tbProps, RPLTextBoxPropsDef tbDef, string textBoxValue, RPLStyleProps actionStyle, bool renderImages, RPLItemMeasurement measurement, RPLAction textBoxAction)
		{
			if (tbDef.IsSimple)
			{
				bool flag = false;
				object obj = null;
				bool flag2 = string.IsNullOrEmpty(textBoxValue);
				if (!flag2 && renderImages)
				{
					obj = tbProps.Style[24];
					if (obj != null && (RPLFormat.TextDecorations)obj != 0)
					{
						obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
						flag = true;
						WriteStream(m_openSpan);
						WriteStream(m_openStyle);
						WriteStream(m_textDecoration);
						WriteStream(obj);
						WriteStream(m_closeQuote);
					}
				}
				if (flag2)
				{
					if (!NeedSharedToggleParent(tbProps))
					{
						WriteStream(m_nbsp);
					}
				}
				else
				{
					List<int> list = null;
					if (!string.IsNullOrEmpty(m_searchText))
					{
						int startIndex = 0;
						int length = m_searchText.Length;
						string text = textBoxValue;
						string text2 = m_searchText;
						if (text2.IndexOf(' ') >= 0)
						{
							text2 = text2.Replace('\u00a0', ' ');
							text = text.Replace('\u00a0', ' ');
						}
						while ((startIndex = text.IndexOf(text2, startIndex, StringComparison.OrdinalIgnoreCase)) != -1)
						{
							if (list == null)
							{
								list = new List<int>(2);
							}
							list.Add(startIndex);
							startIndex += length;
						}
						if (list == null)
						{
							RenderMultiLineText(textBoxValue);
						}
						else
						{
							RenderMultiLineTextWithHits(textBoxValue, list);
						}
					}
					else
					{
						RenderMultiLineText(textBoxValue);
					}
				}
				if (flag)
				{
					WriteStream(m_closeSpan);
				}
				return;
			}
			WriteStream(m_openDiv);
			RPLElementStyle style = tbProps.Style;
			bool flag3 = false;
			bool flag4 = IsWritingModeVertical(style);
			if (!m_deviceInfo.IsBrowserIE || !flag4)
			{
				OpenStyle();
				double num = 0.0;
				if (m_deviceInfo.IsBrowserIE)
				{
					WriteStream(m_overflowXHidden);
					WriteStream(m_semiColon);
				}
				num = 0.0;
				if (measurement != null)
				{
					num = GetInnerContainerWidthSubtractBorders(measurement, tbProps.Style);
				}
				if (tbDef.CanSort && !IsFragment && !IsDirectionRTL(tbProps.Style))
				{
					num -= 4.2333331108093262;
				}
				if (num > 0.0)
				{
					WriteStream(m_styleWidth);
					WriteRSStream((float)num);
					WriteStream(m_semiColon);
				}
			}
			if (IsDirectionRTL(style))
			{
				OpenStyle();
				WriteStream(m_direction);
				WriteStream("rtl");
				CloseStyle(renderQuote: true);
				flag3 = true;
				WriteStream(m_classStyle);
				WriteStream(m_rtlEmbed);
			}
			else
			{
				CloseStyle(renderQuote: true);
			}
			if (textBoxAction != null)
			{
				if (!flag3)
				{
					flag3 = true;
					WriteStream(m_classStyle);
				}
				else
				{
					WriteStream(m_space);
				}
				WriteStream(m_styleAction);
			}
			if (flag3)
			{
				WriteStream(m_quote);
			}
			WriteStream(m_closeBracket);
			TextRunStyleWriter trsw = new TextRunStyleWriter(this);
			ParagraphStyleWriter paragraphStyleWriter = new ParagraphStyleWriter(this, textBox);
			RPLStyleProps nonSharedStyle = tbProps.NonSharedStyle;
			if (nonSharedStyle != null && (nonSharedStyle[30] != null || nonSharedStyle[29] != null))
			{
				paragraphStyleWriter.OutputSharedInNonShared = true;
			}
			RPLParagraph nextParagraph = textBox.GetNextParagraph();
			ListLevelStack listLevelStack = null;
			while (nextParagraph != null)
			{
				RPLParagraphProps rPLParagraphProps = nextParagraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
				int num2 = rPLParagraphProps.ListLevel ?? rPLParagraphPropsDef.ListLevel;
				RPLFormat.ListStyles listStyles = rPLParagraphProps.ListStyle ?? rPLParagraphPropsDef.ListStyle;
				string text3 = null;
				RPLStyleProps nonSharedStyle2 = rPLParagraphProps.NonSharedStyle;
				RPLStyleProps shared = null;
				if (rPLParagraphPropsDef != null)
				{
					if (num2 == 0)
					{
						num2 = rPLParagraphPropsDef.ListLevel;
					}
					if (listStyles == RPLFormat.ListStyles.None)
					{
						listStyles = rPLParagraphPropsDef.ListStyle;
					}
					text3 = rPLParagraphPropsDef.ID;
					if (!paragraphStyleWriter.OutputSharedInNonShared)
					{
						shared = rPLParagraphPropsDef.SharedStyle;
					}
				}
				paragraphStyleWriter.Paragraph = nextParagraph;
				paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
				paragraphStyleWriter.CurrentListLevel = num2;
				byte[] array = null;
				if (num2 > 0)
				{
					if (listLevelStack == null)
					{
						listLevelStack = new ListLevelStack();
					}
					bool writeNoVerticalMargin = !m_deviceInfo.IsBrowserIE || !flag4 || (m_deviceInfo.BrowserMode == BrowserMode.Standards && !m_deviceInfo.IsBrowserIE6Or7StandardsMode);
					listLevelStack.PushTo(this, num2, listStyles, writeNoVerticalMargin);
					if (listStyles != 0)
					{
						if (m_deviceInfo.BrowserMode == BrowserMode.Quirks || m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							WriteStream(m_openDiv);
							WriteStream(m_closeBracket);
						}
						WriteStream(m_openLi);
						paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
						WriteStyles(text3 + "l", nonSharedStyle2, shared, paragraphStyleWriter);
						WriteStream(m_closeBracket);
						array = m_closeLi;
						paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
						text3 += "p";
					}
				}
				else if (listLevelStack != null)
				{
					listLevelStack.PopAll();
					listLevelStack = null;
				}
				WriteStream(m_openDiv);
				WriteStyles(text3, nonSharedStyle2, shared, paragraphStyleWriter);
				WriteStream(m_closeBracket);
				RPLReportSize hangingIndent = rPLParagraphProps.HangingIndent;
				if (hangingIndent == null)
				{
					hangingIndent = rPLParagraphPropsDef.HangingIndent;
				}
				float num3 = 0f;
				if (hangingIndent != null)
				{
					num3 = (float)hangingIndent.ToMillimeters();
				}
				if (num3 > 0f)
				{
					WriteStream(m_openSpan);
					OpenStyle();
					RenderMeasurementWidth(num3, renderMinWidth: true);
					WriteStream(m_styleDisplayInlineBlock);
					CloseStyle(renderQuote: true);
					WriteStream(m_closeBracket);
					if (m_deviceInfo.IsBrowserGeckoEngine)
					{
						WriteStream(m_nbsp);
					}
					WriteStream(m_closeSpan);
				}
				RenderTextRuns(nextParagraph, trsw, textBoxAction);
				WriteStream(m_closeDiv);
				if (array != null)
				{
					WriteStream(array);
					if (m_deviceInfo.BrowserMode == BrowserMode.Quirks || m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						WriteStream(m_closeDiv);
					}
				}
				nextParagraph = textBox.GetNextParagraph();
			}
			listLevelStack?.PopAll();
			WriteStream(m_closeDiv);
		}

		private void RenderTextRuns(RPLParagraph paragraph, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			int num = 0;
			RPLTextRun rPLTextRun = null;
			if (!string.IsNullOrEmpty(m_searchText))
			{
				RPLTextRun nextTextRun = paragraph.GetNextTextRun();
				rPLTextRun = nextTextRun;
				List<RPLTextRun> list = new List<RPLTextRun>();
				StringBuilder stringBuilder = new StringBuilder();
				while (nextTextRun != null)
				{
					list.Add(nextTextRun);
					string value = (nextTextRun.ElementProps as RPLTextRunProps).Value;
					if (string.IsNullOrEmpty(value))
					{
						value = (nextTextRun.ElementPropsDef as RPLTextRunPropsDef).Value;
					}
					stringBuilder.Append(value);
					nextTextRun = paragraph.GetNextTextRun();
				}
				string text = stringBuilder.ToString();
				string text2 = m_searchText;
				if (text2.IndexOf(' ') >= 0)
				{
					text2 = text2.Replace('\u00a0', ' ');
					text = text.Replace('\u00a0', ' ');
				}
				int num2 = text.IndexOf(text2, StringComparison.OrdinalIgnoreCase);
				List<int> list2 = new List<int>();
				int num3 = 0;
				int num4 = 0;
				int runOffsetCount = 0;
				int length = m_searchText.Length;
				for (int i = 0; i < list.Count; i++)
				{
					nextTextRun = list[i];
					string value2 = (nextTextRun.ElementProps as RPLTextRunProps).Value;
					if (string.IsNullOrEmpty(value2))
					{
						value2 = (nextTextRun.ElementPropsDef as RPLTextRunPropsDef).Value;
					}
					if (string.IsNullOrEmpty(value2))
					{
						continue;
					}
					while (num2 > -1 && num2 < num3 + value2.Length)
					{
						list2.Add(num2 - num3);
						num2 = text.IndexOf(text2, num2 + length, StringComparison.OrdinalIgnoreCase);
					}
					if (list2.Count > 0 || num4 > 0)
					{
						num += RenderTextRunFindString(nextTextRun, list2, num4, ref runOffsetCount, trsw, textBoxAction);
						if (num4 > 0)
						{
							num4 -= value2.Length;
							if (num4 < 0)
							{
								num4 = 0;
							}
						}
						if (list2.Count > 0)
						{
							int num5 = list2[list2.Count - 1];
							list2.Clear();
							if (value2.Length < num5 + length)
							{
								num4 = length - (value2.Length - num5);
							}
						}
					}
					else
					{
						num += RenderTextRun(nextTextRun, trsw, textBoxAction);
					}
					num3 += value2.Length;
				}
			}
			else
			{
				RPLTextRun nextTextRun2 = paragraph.GetNextTextRun();
				rPLTextRun = nextTextRun2;
				while (nextTextRun2 != null)
				{
					num += RenderTextRun(nextTextRun2, trsw, textBoxAction);
					nextTextRun2 = paragraph.GetNextTextRun();
				}
			}
			if (num == 0 && rPLTextRun != null)
			{
				RPLTextRunProps rPLTextRunProps = rPLTextRun.ElementProps as RPLTextRunProps;
				RPLElementPropsDef definition = rPLTextRunProps.Definition;
				WriteStream(m_openSpan);
				WriteStyles(definition.ID, rPLTextRunProps.NonSharedStyle, definition.SharedStyle, trsw);
				WriteStream(m_closeBracket);
				WriteStream(m_nbsp);
				WriteStream(m_closeSpan);
			}
		}

		private int RenderTextRunFindString(RPLTextRun textRun, List<int> hits, int remainingChars, ref int runOffsetCount, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			RPLTextRunProps rPLTextRunProps = textRun.ElementProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			RPLStyleProps shared = null;
			string id = null;
			string value = rPLTextRunProps.Value;
			string toolTip = rPLTextRunProps.ToolTip;
			if (rPLTextRunPropsDef != null)
			{
				shared = rPLTextRunPropsDef.SharedStyle;
				id = rPLTextRunPropsDef.ID;
				if (string.IsNullOrEmpty(value))
				{
					value = rPLTextRunPropsDef.Value;
				}
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = rPLTextRunPropsDef.ToolTip;
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			byte[] theBytes = m_closeSpan;
			RPLAction rPLAction = null;
			if (textBoxAction == null && HasAction(rPLTextRunProps.ActionInfo))
			{
				rPLAction = rPLTextRunProps.ActionInfo.Actions[0];
			}
			if (rPLAction != null)
			{
				WriteStream(m_openA);
				RenderTabIndex();
				RenderActionHref(rPLAction, RPLFormat.TextDecorations.Underline, null);
				theBytes = m_closeA;
			}
			else
			{
				WriteStream(m_openSpan);
			}
			if (toolTip != null)
			{
				WriteToolTipAttribute(toolTip);
			}
			WriteStyles(id, rPLTextRunProps.NonSharedStyle, shared, trsw);
			WriteStream(m_closeBracket);
			int num = 0;
			int num2 = 0;
			int length = value.Length;
			if (remainingChars > 0)
			{
				int num3 = remainingChars;
				if (num3 > length)
				{
					num3 = length;
				}
				if (num3 > 0)
				{
					OutputFindString(value.Substring(0, num3), runOffsetCount++);
					num += num3;
					if (num3 >= remainingChars)
					{
						m_currentHitCount++;
						runOffsetCount = 0;
					}
				}
			}
			int num4 = hits.Count - 1;
			bool flag = false;
			int length2 = m_searchText.Length;
			if (hits.Count > 0)
			{
				num2 = hits[hits.Count - 1];
				if (num2 + length2 > length)
				{
					flag = true;
				}
				else
				{
					num4 = hits.Count;
				}
			}
			for (int i = 0; i < num4; i++)
			{
				num2 = hits[i];
				if (num < num2)
				{
					RenderMultiLineText(value.Substring(num, num2 - num));
				}
				OutputFindString(value.Substring(num2, length2), 0);
				m_currentHitCount++;
				runOffsetCount = 0;
				num = num2 + length2;
			}
			if (flag)
			{
				num2 = hits[hits.Count - 1];
				if (num < num2)
				{
					RenderMultiLineText(value.Substring(num, num2 - num));
				}
				OutputFindString(value.Substring(num2, length - num2), runOffsetCount++);
			}
			else if (num < length)
			{
				RenderMultiLineText(value.Substring(num));
			}
			WriteStream(theBytes);
			return length;
		}

		private int RenderTextRun(RPLTextRun textRun, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			RPLTextRunProps rPLTextRunProps = textRun.ElementProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			RPLStyleProps shared = null;
			string id = null;
			string value = rPLTextRunProps.Value;
			string toolTip = rPLTextRunProps.ToolTip;
			if (rPLTextRunPropsDef != null)
			{
				shared = rPLTextRunPropsDef.SharedStyle;
				id = rPLTextRunPropsDef.ID;
				if (string.IsNullOrEmpty(value))
				{
					value = rPLTextRunPropsDef.Value;
				}
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = rPLTextRunPropsDef.ToolTip;
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			byte[] theBytes = m_closeSpan;
			RPLAction rPLAction = null;
			if (textBoxAction == null)
			{
				rPLAction = textBoxAction;
				if (HasAction(rPLTextRunProps.ActionInfo))
				{
					rPLAction = rPLTextRunProps.ActionInfo.Actions[0];
				}
			}
			if (rPLAction != null)
			{
				WriteStream(m_openA);
				RenderTabIndex();
				RenderActionHref(rPLAction, RPLFormat.TextDecorations.Underline, null);
				theBytes = m_closeA;
			}
			else
			{
				WriteStream(m_openSpan);
			}
			if (toolTip != null)
			{
				WriteToolTipAttribute(toolTip);
			}
			WriteStyles(id, rPLTextRunProps.NonSharedStyle, shared, trsw);
			RenderLanguage(rPLTextRunProps.Style[32] as string);
			WriteStream(m_closeBracket);
			RenderMultiLineText(value);
			WriteStream(theBytes);
			return value.Length;
		}

		private void WriteStyles(string id, RPLStyleProps nonShared, RPLStyleProps shared, ElementStyleWriter styleWriter)
		{
			bool flag = (shared != null && shared.Count > 0) || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.Shared);
			if (m_useInlineStyle || (flag && id == null))
			{
				OpenStyle();
				styleWriter.WriteStyles(StyleWriterMode.All, new RPLElementStyle(nonShared, shared));
				CloseStyle(renderQuote: true);
				return;
			}
			if ((nonShared != null && nonShared.Count > 0) || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.NonShared))
			{
				OpenStyle();
				styleWriter.WriteStyles(StyleWriterMode.NonShared, nonShared);
				CloseStyle(renderQuote: true);
			}
			if (!flag || id == null)
			{
				return;
			}
			byte[] array = (byte[])m_usedStyles[id];
			if (array == null)
			{
				if (m_onlyVisibleStyles)
				{
					Stream mainStream = m_mainStream;
					m_mainStream = m_styleStream;
					RenderOpenStyle(id);
					styleWriter.WriteStyles(StyleWriterMode.Shared, shared);
					WriteStream(m_closeAccol);
					m_mainStream = mainStream;
					array = m_encoding.GetBytes(id);
					m_usedStyles.Add(id, array);
				}
				else
				{
					array = m_encoding.GetBytes(id);
					m_usedStyles.Add(id, array);
				}
			}
			CloseStyle(renderQuote: true);
			WriteClassStyle(array, close: true);
		}

		protected abstract void WriteFitProportionalScript(double pv, double ph);

		private void RenderImageFitProportional(RPLImage image, RPLItemMeasurement measurement, PaddingSharedInfo padds, bool writeSmallSize)
		{
			if (!m_deviceInfo.AllowScript)
			{
				return;
			}
			m_fitPropImages = true;
			double pv = 0.0;
			double ph = 0.0;
			if (padds != null)
			{
				pv = padds.PadV;
				ph = padds.PadH;
			}
			WriteFitProportionalScript(pv, ph);
			if (writeSmallSize || !m_browserIE)
			{
				long num = 1L;
				WriteStream(m_inlineHeight);
				if (m_deviceInfo.IsBrowserSafari || m_deviceInfo.IsBrowserGeckoEngine)
				{
					num = 5L;
					if (measurement != null)
					{
						double num2 = measurement.Height;
						if ((double)measurement.Width < num2)
						{
							num2 = measurement.Width;
						}
						num = Utility.MMToPx(num2);
						if (num < 5)
						{
							num = 5L;
						}
					}
				}
				WriteStream(num.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_px);
				WriteStream(m_quote);
			}
			if (writeSmallSize)
			{
				WriteStream(m_inlineWidth);
				WriteStream("1");
				WriteStream(m_px);
				WriteStream(m_quote);
			}
		}

		private void RenderImagePercent(RPLImage image, RPLImageProps imageProps, RPLImagePropsDef imagePropsDef, RPLItemMeasurement measurement)
		{
			bool flag = false;
			bool flag2 = false;
			RPLImageData image2 = imageProps.Image;
			RPLActionInfo actionInfo = imageProps.ActionInfo;
			RPLFormat.Sizings sizing = imagePropsDef.Sizing;
			if (sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit || sizing == RPLFormat.Sizings.Clip)
			{
				flag = true;
				WriteStream(m_openDiv);
				if (m_useInlineStyle)
				{
					PercentSizesOverflow();
				}
				else
				{
					ClassPercentSizesOverflow();
				}
				if (measurement != null)
				{
					OpenStyle();
					RenderMeasurementMinWidth(GetInnerContainerWidth(measurement, imageProps.Style));
					RenderMeasurementMinHeight(GetInnerContainerHeight(measurement, imageProps.Style));
					CloseStyle(renderQuote: true);
				}
			}
			int xOffset = 0;
			int yOffset = 0;
			System.Drawing.Rectangle imageConsolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
			bool flag3 = !imageConsolidationOffsets.IsEmpty;
			if (flag3)
			{
				if (!flag)
				{
					flag = true;
					WriteStream(m_openDiv);
					if (sizing != 0)
					{
						if (m_useInlineStyle)
						{
							PercentSizesOverflow();
						}
						else
						{
							ClassPercentSizesOverflow();
						}
					}
				}
				if (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit)
				{
					WriteStream(m_closeBracket);
					WriteStream(m_openDiv);
					if (m_deviceInfo.IsBrowserIE6 && m_deviceInfo.IsBrowserIE6Or7StandardsMode && measurement != null)
					{
						WriteStream(" origWidth=\"");
						WriteRSStream(measurement.Width);
						WriteStream("\" origHeight=\"");
						WriteStream("\"");
					}
				}
				WriteOuterConsolidation(imageConsolidationOffsets, sizing, imageProps.UniqueName);
				CloseStyle(renderQuote: true);
				xOffset = imageConsolidationOffsets.Left;
				yOffset = imageConsolidationOffsets.Top;
			}
			else if (m_deviceInfo.AllowScript && sizing == RPLFormat.Sizings.Fit && m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				flag = true;
				WriteStream(m_openDiv);
				if (m_imgFitDivIdsStream == null)
				{
					CreateImgFitDivImageIdsStream();
				}
				WriteIdToSecondaryStream(m_imgFitDivIdsStream, imageProps.UniqueName + "_ifd");
				RenderReportItemId(imageProps.UniqueName + "_ifd");
			}
			if (flag)
			{
				WriteStream(m_closeBracket);
			}
			if (HasAction(actionInfo))
			{
				flag2 = RenderElementHyperlink(imageProps.Style, actionInfo.Actions[0]);
			}
			WriteStream(m_img);
			if (m_browserIE)
			{
				WriteStream(m_imgOnError);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length != 0)
			{
				WriteAttrEncoded(m_useMap, "#" + m_deviceInfo.HtmlPrefixId + m_mapPrefixString + imageProps.UniqueName);
				WriteStream(m_zeroBorder);
			}
			else if (flag2)
			{
				WriteStream(m_zeroBorder);
			}
			switch (sizing)
			{
			case RPLFormat.Sizings.FitProportional:
			{
				PaddingSharedInfo padds = null;
				if (m_deviceInfo.IsBrowserSafari)
				{
					padds = GetPaddings(image.ElementProps.Style, null);
				}
				bool writeSmallSize = !flag3 && m_deviceInfo.BrowserMode == BrowserMode.Standards;
				RenderImageFitProportional(image, null, padds, writeSmallSize);
				break;
			}
			case RPLFormat.Sizings.Fit:
				if (!flag3)
				{
					if (m_deviceInfo.AllowScript && m_deviceInfo.BrowserMode == BrowserMode.Standards)
					{
						WriteStream(" width=\"1px\" height=\"1px\"");
					}
					else if (m_useInlineStyle)
					{
						PercentSizes();
					}
					else
					{
						ClassPercentSizes();
					}
				}
				break;
			}
			if (flag3)
			{
				WriteClippedDiv(imageConsolidationOffsets);
			}
			WriteToolTip(imageProps);
			WriteStream(m_src);
			RenderImageUrl(useSessionId: true, image2);
			WriteStream(m_closeTag);
			if (flag2)
			{
				WriteStream(m_closeA);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length != 0)
			{
				RenderImageMapAreas(imageProps.ActionImageMapAreas, measurement.Width, measurement.Height, imageProps.UniqueName, xOffset, yOffset);
			}
			if (flag3 && (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit))
			{
				WriteStream(m_closeDiv);
			}
			if (flag)
			{
				WriteStreamCR(m_closeDiv);
			}
		}

		private void RenderImageMapAreas(RPLActionInfoWithImageMap[] actionImageMaps, double width, double height, string uniqueName, int xOffset, int yOffset)
		{
			RPLActionInfoWithImageMap rPLActionInfoWithImageMap = null;
			double imageWidth = width * 96.0 * 0.03937007874;
			double imageHeight = height * 96.0 * 0.03937007874;
			WriteStream(m_openMap);
			WriteAttrEncoded(m_name, m_deviceInfo.HtmlPrefixId + m_mapPrefixString + uniqueName);
			WriteStreamCR(m_closeBracket);
			for (int i = 0; i < actionImageMaps.Length; i++)
			{
				rPLActionInfoWithImageMap = actionImageMaps[i];
				if (rPLActionInfoWithImageMap != null)
				{
					RenderImageMapArea(rPLActionInfoWithImageMap, imageWidth, imageHeight, uniqueName, xOffset, yOffset);
				}
			}
			WriteStream(m_closeMap);
		}

		protected void RenderImageMapArea(RPLActionInfoWithImageMap actionImageMap, double imageWidth, double imageHeight, string uniqueName, int xOffset, int yOffset)
		{
			RPLAction rPLAction = null;
			if (actionImageMap.Actions != null && actionImageMap.Actions.Length != 0)
			{
				rPLAction = actionImageMap.Actions[0];
				if (!HasAction(rPLAction))
				{
					rPLAction = null;
				}
			}
			if (actionImageMap.ImageMaps == null || actionImageMap.ImageMaps.Count <= 0)
			{
				return;
			}
			RPLImageMap rPLImageMap = null;
			for (int i = 0; i < actionImageMap.ImageMaps.Count; i++)
			{
				rPLImageMap = actionImageMap.ImageMaps[i];
				string toolTip = rPLImageMap.ToolTip;
				if (rPLAction == null && toolTip == null)
				{
					continue;
				}
				WriteStream(m_mapArea);
				RenderTabIndex();
				if (toolTip != null)
				{
					WriteToolTipAttribute(toolTip);
				}
				if (rPLAction != null)
				{
					RenderActionHref(rPLAction, RPLFormat.TextDecorations.None, null);
				}
				else
				{
					WriteStream(m_nohref);
				}
				WriteStream(m_mapShape);
				switch (rPLImageMap.Shape)
				{
				case RPLFormat.ShapeType.Circle:
					WriteStream(m_circleShape);
					break;
				case RPLFormat.ShapeType.Polygon:
					WriteStream(m_polyShape);
					break;
				default:
					WriteStream(m_rectShape);
					break;
				}
				WriteStream(m_quote);
				WriteStream(m_mapCoords);
				float[] coordinates = rPLImageMap.Coordinates;
				long num = 0L;
				bool flag = true;
				int j = 0;
				if (coordinates != null)
				{
					for (; j < coordinates.Length - 1; j += 2)
					{
						if (!flag)
						{
							WriteStream(m_comma);
						}
						num = (long)((double)(coordinates[j] / 100f) * imageWidth) + xOffset;
						WriteStream(num);
						WriteStream(m_comma);
						num = (long)((double)(coordinates[j + 1] / 100f) * imageHeight) + yOffset;
						WriteStream(num);
						flag = false;
					}
					if (j < coordinates.Length)
					{
						WriteStream(m_comma);
						num = (long)((double)(coordinates[j] / 100f) * imageWidth);
						WriteStream(num);
					}
				}
				WriteStream(m_quote);
				WriteStreamCR(m_closeBracket);
			}
		}

		protected void RenderCreateFixedHeaderFunction(string prefix, string fixedHeaderObject, StringBuilder function, StringBuilder arrayBuilder, bool createHeadersWithArray)
		{
			int num = 0;
			StringBuilder stringBuilder = function;
			if (createHeadersWithArray)
			{
				stringBuilder = arrayBuilder;
			}
			foreach (TablixFixedHeaderStorage fixedHeader in m_fixedHeaders)
			{
				string text = "frgh" + num + "_" + fixedHeader.HtmlId;
				string text2 = "fcgh" + num + "_" + fixedHeader.HtmlId;
				string text3 = "fch" + num + "_" + fixedHeader.HtmlId;
				string value = m_deviceInfo.HtmlPrefixId + text;
				string value2 = m_deviceInfo.HtmlPrefixId + text2;
				string value3 = m_deviceInfo.HtmlPrefixId + text3;
				if (fixedHeader.ColumnHeaders != null)
				{
					string value4 = prefix + "fcghArr" + num;
					arrayBuilder.Append(value4);
					arrayBuilder.Append("=new Array('");
					arrayBuilder.Append(fixedHeader.HtmlId);
					arrayBuilder.Append('\'');
					for (int i = 0; i < fixedHeader.ColumnHeaders.Count; i++)
					{
						arrayBuilder.Append(",'");
						arrayBuilder.Append(fixedHeader.ColumnHeaders[i]);
						arrayBuilder.Append('\'');
					}
					arrayBuilder.Append(");");
					if (!createHeadersWithArray)
					{
						arrayBuilder.Append(value2);
						arrayBuilder.Append("=null;");
						function.Append("if (!");
						function.Append(value2);
						function.Append("){");
						function.Append(value2);
						function.Append("=");
					}
					stringBuilder.Append(fixedHeaderObject);
					stringBuilder.Append(".CreateFixedColumnHeader(");
					stringBuilder.Append(value4);
					stringBuilder.Append(",'");
					stringBuilder.Append(text2);
					stringBuilder.Append("');");
					if (!createHeadersWithArray)
					{
						function.Append("}");
					}
				}
				if (fixedHeader.RowHeaders != null)
				{
					string value5 = prefix + "frhArr" + num;
					arrayBuilder.Append(value5);
					arrayBuilder.Append("=new Array('");
					arrayBuilder.Append(fixedHeader.HtmlId);
					arrayBuilder.Append('\'');
					for (int j = 0; j < fixedHeader.RowHeaders.Count; j++)
					{
						arrayBuilder.Append(",'");
						arrayBuilder.Append(fixedHeader.RowHeaders[j]);
						arrayBuilder.Append('\'');
					}
					arrayBuilder.Append(");");
					if (!createHeadersWithArray)
					{
						arrayBuilder.Append(value);
						arrayBuilder.Append("=null;");
						function.Append("if (!");
						function.Append(value);
						function.Append("){");
						function.Append(value);
						function.Append("=");
					}
					stringBuilder.Append(fixedHeaderObject);
					stringBuilder.Append(".CreateFixedRowHeader(");
					stringBuilder.Append(value5);
					stringBuilder.Append(",'");
					stringBuilder.Append(text);
					stringBuilder.Append("');");
					if (!createHeadersWithArray)
					{
						function.Append("}");
					}
				}
				if (fixedHeader.CornerHeaders != null)
				{
					string value6 = prefix + "fchArr" + num;
					arrayBuilder.Append(value6);
					arrayBuilder.Append("=new Array('");
					arrayBuilder.Append(fixedHeader.HtmlId);
					arrayBuilder.Append('\'');
					for (int k = 0; k < fixedHeader.CornerHeaders.Count; k++)
					{
						arrayBuilder.Append(",'");
						arrayBuilder.Append(fixedHeader.CornerHeaders[k]);
						arrayBuilder.Append('\'');
					}
					arrayBuilder.Append(");");
					if (!createHeadersWithArray)
					{
						arrayBuilder.Append(value3);
						arrayBuilder.Append("=null;");
						function.Append("if (!");
						function.Append(value3);
						function.Append("){");
						function.Append(value3);
						function.Append("=");
					}
					stringBuilder.Append(fixedHeaderObject);
					stringBuilder.Append(".CreateFixedRowHeader(");
					stringBuilder.Append(value6);
					stringBuilder.Append(",'");
					stringBuilder.Append(text3);
					stringBuilder.Append("');");
					if (!createHeadersWithArray)
					{
						function.Append("}");
					}
				}
				function.Append(fixedHeaderObject);
				function.Append(".ShowFixedTablixHeaders('");
				function.Append(fixedHeader.HtmlId);
				function.Append("','");
				function.Append((fixedHeader.BodyID != null) ? fixedHeader.BodyID : fixedHeader.HtmlId);
				function.Append("','");
				function.Append(text);
				function.Append("','");
				function.Append(text2);
				function.Append("','");
				function.Append(text3);
				function.Append("','");
				function.Append(fixedHeader.FirstRowGroupCol);
				function.Append("','");
				function.Append(fixedHeader.LastRowGroupCol);
				function.Append("','");
				function.Append(fixedHeader.LastColGroupRow);
				function.Append("');");
				num++;
			}
		}

		private void RenderServerDynamicImage(RPLElement dynamicImage, RPLDynamicImageProps dynamicImageProps, RPLElementPropsDef def, RPLItemMeasurement measurement, int borderContext, bool renderId, StyleContext styleContext)
		{
			if (dynamicImage == null)
			{
				return;
			}
			bool flag = dynamicImageProps.ActionImageMapAreas != null && dynamicImageProps.ActionImageMapAreas.Length != 0;
			System.Drawing.Rectangle rectangle = RenderDynamicImage(measurement, dynamicImageProps);
			int xOffset = 0;
			int yOffset = 0;
			bool flag2 = !rectangle.IsEmpty;
			bool flag3 = !m_deviceInfo.IsBrowserSafari || m_deviceInfo.AllowScript || !styleContext.InTablix;
			if (flag3)
			{
				WriteStream(m_openDiv);
			}
			bool flag4 = m_deviceInfo.DataVisualizationFitSizing == DataVisualizationFitSizing.Exact && styleContext.InTablix;
			if (flag2)
			{
				RPLFormat.Sizings sizing = flag4 ? RPLFormat.Sizings.Fit : RPLFormat.Sizings.AutoSize;
				WriteOuterConsolidation(rectangle, sizing, dynamicImageProps.UniqueName);
				RenderReportItemStyle(dynamicImage, null, ref borderContext);
				xOffset = rectangle.Left;
				yOffset = rectangle.Top;
			}
			else if (flag4 && m_deviceInfo.AllowScript)
			{
				if (m_imgFitDivIdsStream == null)
				{
					CreateImgFitDivImageIdsStream();
				}
				WriteIdToSecondaryStream(m_imgFitDivIdsStream, dynamicImageProps.UniqueName + "_ifd");
				RenderReportItemId(dynamicImageProps.UniqueName + "_ifd");
			}
			if (flag3)
			{
				WriteStream(m_closeBracket);
			}
			WriteStream(m_img);
			if (m_browserIE)
			{
				WriteStream(m_imgOnError);
			}
			if (renderId)
			{
				RenderReportItemId(dynamicImageProps.UniqueName);
			}
			WriteStream(m_zeroBorder);
			bool flag5 = dynamicImage is RPLChart;
			if (flag)
			{
				WriteAttrEncoded(m_useMap, "#" + m_deviceInfo.HtmlPrefixId + m_mapPrefixString + dynamicImageProps.UniqueName);
				if (flag4)
				{
					OpenStyle();
					if (m_useInlineStyle && !flag2)
					{
						WriteStream(m_styleHeight);
						WriteStream(m_percent);
						WriteStream(m_semiColon);
						WriteStream(m_styleWidth);
						WriteStream(m_percent);
						WriteStream(m_semiColon);
						flag5 = false;
					}
					WriteStream("border-style:none;");
				}
			}
			else if (flag4 && m_useInlineStyle && !flag2)
			{
				PercentSizes();
				flag5 = false;
			}
			StyleContext styleContext2 = new StyleContext();
			if (!flag4 && (m_deviceInfo.IsBrowserIE7 || m_deviceInfo.IsBrowserIE6))
			{
				styleContext2.RenderMeasurements = false;
				styleContext2.RenderMinMeasurements = false;
			}
			if (!flag2)
			{
				if (flag4)
				{
					RenderReportItemStyle(dynamicImage, null, ref borderContext, styleContext2);
				}
				else if (flag5)
				{
					RPLElementProps elementProps = dynamicImage.ElementProps;
					StyleContext styleContext3 = new StyleContext();
					styleContext3.RenderMeasurements = false;
					OpenStyle();
					RenderMeasurementStyle(measurement.Height, measurement.Width);
					RenderReportItemStyle(dynamicImage, elementProps, def, measurement, styleContext3, ref borderContext, def.ID);
				}
				else
				{
					RenderReportItemStyle(dynamicImage, measurement, ref borderContext, styleContext2);
				}
			}
			else
			{
				WriteClippedDiv(rectangle);
			}
			WriteToolTip(dynamicImageProps);
			WriteStream(m_src);
			RenderDynamicImageSrc(dynamicImageProps);
			WriteStreamCR(m_closeTag);
			if (flag)
			{
				RenderImageMapAreas(dynamicImageProps.ActionImageMapAreas, measurement.Width, measurement.Height, dynamicImageProps.UniqueName, xOffset, yOffset);
			}
			if (flag3)
			{
				WriteStream(m_closeDiv);
			}
		}

		private void RenderBorderLine(RPLElement reportItem)
		{
			object obj = null;
			IRPLStyle style = reportItem.ElementProps.Style;
			obj = style[10];
			if (obj != null)
			{
				WriteStream(obj.ToString());
				WriteStream(m_space);
			}
			obj = style[5];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				WriteStream(obj);
				WriteStream(m_space);
			}
			obj = style[0];
			if (obj != null)
			{
				WriteStream((string)obj);
			}
		}

		private string CalculateRowHeaderId(RPLTablixCell cell, bool fixedHeader, string tablixID, int row, int col, TablixFixedHeaderStorage headerStorage, bool useElementName, bool fixedCornerHeader)
		{
			string text = null;
			if (cell is RPLTablixMemberCell)
			{
				if (((RPLTablixMemberCell)cell).GroupLabel != null)
				{
					text = ((RPLTablixMemberCell)cell).UniqueName;
				}
				else if (!fixedHeader && useElementName && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
				}
			}
			if (fixedHeader)
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
				}
				if (headerStorage != null)
				{
					headerStorage.RowHeaders.Add(text);
					if (headerStorage.CornerHeaders != null && fixedCornerHeader)
					{
						headerStorage.CornerHeaders.Add(text);
					}
				}
			}
			return text;
		}

		private void RenderAccessibleHeaders(RPLTablix tablix, bool fixedHeader, int numCols, int col, int colSpan, int row, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders, ref string id)
		{
			int currentLevel = -1;
			if (tablix.RowHeaderColumns == 0 && omittedCells != null && omittedCells.Count > 0)
			{
				foreach (RPLTablixMemberCell omittedCell in omittedCells)
				{
					RPLTablixMemberDef tablixMemberDef = omittedCell.TablixMemberDef;
					if (tablixMemberDef != null && tablixMemberDef.IsStatic && tablixMemberDef.StaticHeadersTree)
					{
						if (id == null && cell.Element != null && cell.Element.ElementProps.UniqueName != null)
						{
							id = cell.Element.ElementProps.UniqueName;
						}
						currentLevel = tablixMemberDef.Level;
						omittedHeaders.Push(tablixMemberDef.Level, col, colSpan, id, numCols);
					}
				}
			}
			if (row < tablix.ColumnHeaderRows || fixedHeader || (col >= tablix.ColsBeforeRowHeaders && tablix.RowHeaderColumns > 0 && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders))
			{
				return;
			}
			bool flag = false;
			string text = colHeaderIds[cell.ColIndex];
			if (!string.IsNullOrEmpty(text))
			{
				WriteStream(m_headers);
				WriteStream(text);
				flag = true;
			}
			foreach (HTMLHeader hTMLHeader in rowHeaderIds)
			{
				string iD = hTMLHeader.ID;
				if (!string.IsNullOrEmpty(iD))
				{
					if (flag)
					{
						WriteStream(m_space);
					}
					else
					{
						WriteStream(m_headers);
					}
					WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
					WriteStream(iD);
					flag = true;
				}
			}
			string headers = omittedHeaders.GetHeaders(col, currentLevel, HttpUtility.HtmlAttributeEncode(m_deviceInfo.HtmlPrefixId));
			if (!string.IsNullOrEmpty(headers))
			{
				if (flag)
				{
					WriteStream(m_space);
				}
				else
				{
					WriteStream(m_headers);
				}
				WriteStream(headers);
				flag = true;
			}
			if (flag)
			{
				WriteStream(m_quote);
			}
		}

		private void RenderTablixCell(RPLTablix tablix, bool fixedHeader, string tablixID, int numCols, int numRows, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			bool useElementName = m_deviceInfo.AccessibleTablix && tablix.RowHeaderColumns > 0 && col >= tablix.ColsBeforeRowHeaders && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders;
			bool fixedCornerHeader = fixedHeader && tablix.FixedColumns[col] && tablix.FixedRow(row);
			string id = CalculateRowHeaderId(cell, fixedHeader, tablixID, cell.RowIndex, cell.ColIndex, headerStorage, useElementName, fixedCornerHeader);
			WriteStream(m_openTD);
			if (m_deviceInfo.AccessibleTablix)
			{
				RenderAccessibleHeaders(tablix, fixedHeader, numCols, cell.ColIndex, colSpan, cell.RowIndex, cell, omittedCells, rowHeaderIds, colHeaderIds, omittedHeaders, ref id);
			}
			if (id != null)
			{
				RenderReportItemId(id);
			}
			int rowSpan = cell.RowSpan;
			if (cell.RowSpan > 1)
			{
				WriteStream(m_rowSpan);
				WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
				WriteStream(m_inlineHeight);
				WriteStream(Utility.MmToPxAsString(tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				WriteStream(m_quote);
			}
			if (colSpan > 1)
			{
				WriteStream(m_colSpan);
				WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
				RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					OpenStyle();
					WriteStream(m_displayNone);
					CloseStyle(renderQuote: true);
				}
				WriteStream(m_closeBracket);
				RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				WriteStream(m_nbsp);
			}
			WriteStream(m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixOmittedHeaderCells(List<RPLTablixMemberCell> omittedHeaders, int colIndex, bool lastCol, ref int omittedIndex)
		{
			if (omittedHeaders == null)
			{
				return;
			}
			while (omittedIndex < omittedHeaders.Count && (omittedHeaders[omittedIndex].ColIndex == colIndex || (lastCol && omittedHeaders[omittedIndex].ColIndex > colIndex)))
			{
				RPLTablixMemberCell rPLTablixMemberCell = omittedHeaders[omittedIndex];
				if (rPLTablixMemberCell.GroupLabel != null)
				{
					RenderNavigationId(rPLTablixMemberCell.UniqueName);
				}
				omittedIndex++;
			}
		}

		private void RenderColumnHeaderTablixCell(RPLTablix tablix, string tablixID, int numCols, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, List<RPLTablixOmittedRow> omittedRows, int[] omittedIndices)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(col, colSpan);
			styleContext.ZeroWidth = (columnWidth == 0f);
			int startIndex = RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			WriteStream(m_openTD);
			int rowSpan = cell.RowSpan;
			string text = null;
			if (cell is RPLTablixMemberCell && (((RPLTablixMemberCell)cell).GroupLabel != null || m_deviceInfo.AccessibleTablix))
			{
				text = ((RPLTablixMemberCell)cell).UniqueName;
				if (text == null && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
					((RPLTablixMemberCell)cell).UniqueName = text;
				}
				if (text != null)
				{
					RenderReportItemId(text);
				}
			}
			if (tablix.FixedColumns[col])
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
					RenderReportItemId(text);
				}
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			if (rowSpan > 1)
			{
				WriteStream(m_rowSpan);
				WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
				WriteStream(m_inlineHeight);
				WriteStream(Utility.MmToPxAsString(tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				WriteStream(m_quote);
			}
			if (colSpan > 1)
			{
				WriteStream(m_colSpan);
				WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				WriteStream(m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int borderContext = 0;
				RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, lastRow: false, element, ref borderContext);
				for (int i = 0; i < omittedRows.Count; i++)
				{
					RenderTablixOmittedHeaderCells(omittedRows[i].OmittedHeaders, col, lastCol, ref omittedIndices[i]);
				}
				RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, lastRow: false, element, ref borderContext);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					OpenStyle();
					WriteStream(m_displayNone);
					CloseStyle(renderQuote: true);
				}
				WriteStream(m_closeBracket);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					RenderTablixOmittedHeaderCells(omittedRows[j].OmittedHeaders, col, lastCol, ref omittedIndices[j]);
				}
				WriteStream(m_nbsp);
			}
			WriteStream(m_closeTD);
			RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		protected void CreateGrowRectIdsStream()
		{
			string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_gr");
			Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			m_growRectangleIdsStream = new BufferedStream(stream);
			m_needsGrowRectangleScript = true;
		}

		protected void CreateFitVertTextIdsStream()
		{
			string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_fvt");
			Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			m_fitVertTextIdsStream = new BufferedStream(stream);
			m_needsFitVertTextScript = true;
		}

		protected void CreateImgConImageIdsStream()
		{
			string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_ici");
			Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			m_imgConImageIdsStream = new BufferedStream(stream);
		}

		protected void CreateImgFitDivImageIdsStream()
		{
			string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_ifd");
			Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			m_imgFitDivIdsStream = new BufferedStream(stream);
			m_emitImageConsolidationScaling = true;
		}

		[SecurityCritical]
		[SecuritySafeCritical]
		protected Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation)
		{
			return m_createAndRegisterStreamCallback(name, extension, encoding, mimeType, willSeek, operation);
		}

		protected void RenderSecondaryStreamIdsSpanTag(Stream secondaryStream, string tagId)
		{
			if (secondaryStream != null && secondaryStream.CanSeek)
			{
				WriteStream(m_openSpan);
				RenderReportItemId(tagId);
				WriteStream(" ids=\"");
				secondaryStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = new byte[4096];
				int count;
				while ((count = secondaryStream.Read(array, 0, array.Length)) > 0)
				{
					m_mainStream.Write(array, 0, count);
				}
				WriteStream("\"");
				WriteStream(m_closeBracket);
				WriteStreamCR(m_closeSpan);
			}
		}

		protected void RenderSecondaryStreamSpanTagsForJavascriptFunctions()
		{
			RenderSecondaryStreamIdsSpanTag(m_growRectangleIdsStream, "growRectangleIdsTag");
			RenderSecondaryStreamIdsSpanTag(m_fitVertTextIdsStream, "fitVertTextIdsTag");
			RenderSecondaryStreamIdsSpanTag(m_imgFitDivIdsStream, "imgFitDivIdsTag");
			RenderSecondaryStreamIdsSpanTag(m_imgConImageIdsStream, "imgConImageIdsTag");
		}
	}
}
