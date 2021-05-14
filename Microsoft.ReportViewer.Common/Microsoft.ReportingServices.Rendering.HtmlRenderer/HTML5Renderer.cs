using Microsoft.ReportingServices.Diagnostics;
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
using System.Web.Security.AntiXss;
using System.Web.UI;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal abstract class HTML5Renderer : IHtmlReportWriter, IHtmlWriter, IHtmlRenderer
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

		internal enum FontAttributes
		{
			None,
			Partial,
			All
		}

		internal enum PageSection
		{
			Body,
			PageHeader,
			PageFooter
		}

		protected internal DeviceInfo m_deviceInfo;

		protected Stream m_mainStream;

		protected bool m_isStyleOpen;

		protected internal ArrayList m_fixedHeaders;

		protected PageSection m_pageSection;

		protected internal Stack m_linkToChildStack;

		protected bool m_htmlFragment;

		protected int m_tabIndexNum;

		internal int m_currentHitCount;

		internal const string DrillthroughAction = "Drillthrough";

		internal const string BookmarkAction = "Bookmark";

		internal const string GetImageKey = "GetImage";

		internal Encoding m_encoding;

		internal const int IgnoreLeft = 1;

		internal const int IgnoreRight = 2;

		internal const int IgnoreTop = 4;

		internal const int IgnoreBottom = 8;

		internal const int IgnoreAll = 15;

		internal const float MaxWordSize = 558.8f;

		internal const string FixedRowGroupHeaderPrefix = "frgh";

		internal const string FixedCornerHeaderPrefix = "fch";

		internal const string FixedColGroupHeaderPrefix = "fcgh";

		internal const string FixedRGHArrayPrefix = "frhArr";

		internal const string FixedCGHArrayPrefix = "fcghArr";

		internal const string FixedCHArrayPrefix = "fchArr";

		internal const string ReportDiv = "oReportDiv";

		internal const string ReportCell = "oReportCell";

		private const char PathSeparator = '/';

		protected const string ImageConImageSuffix = "_ici";

		protected internal const string ImageFitDivSuffix = "_ifd";

		protected internal const string NavigationAnchorSuffix = "_na";

		protected internal const long FitProptionalDefaultSize = 5L;

		protected const int SecondaryStreamBufferSize = 4096;

		internal const string SortAction = "Sort";

		internal const string ToggleAction = "Toggle";

		internal static char[] m_cssDelimiters = new char[13]
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

		protected bool m_hasOnePage = true;

		protected internal RPLReport m_rplReport;

		protected RPLPageContent m_pageContent;

		protected RPLReportSection m_rplReportSection;

		protected IReportWrapper m_report;

		protected ISPBProcessing m_spbProcessing;

		protected internal IElementExtender m_elementExtender;

		protected Hashtable m_usedStyles;

		protected NameValueCollection m_serverParams;

		protected NameValueCollection m_rawDeviceInfo;

		protected Dictionary<string, string> m_images;

		protected byte[] m_stylePrefixIdBytes;

		protected internal int m_pageNum;

		protected CreateAndRegisterStream m_createAndRegisterStreamCallback;

		protected internal bool m_fitPropImages;

		protected internal bool m_browserIE = true;

		protected RequestType m_requestType;

		protected Stream m_styleStream;

		protected internal Stream m_growRectangleIdsStream;

		protected internal Stream m_fitVertTextIdsStream;

		protected internal Stream m_imgFitDivIdsStream;

		protected Stream m_imgConImageIdsStream;

		protected internal bool m_useInlineStyle;

		protected bool m_pageWithBookmarkLinks;

		protected bool m_pageWithSortClicks;

		protected bool m_allPages;

		protected int m_outputLineLength;

		protected bool m_onlyVisibleStyles;

		protected internal SecondaryStreams m_createSecondaryStreams = SecondaryStreams.Server;

		protected Hashtable m_duplicateItems;

		protected internal string m_searchText;

		protected bool m_emitImageConsolidationScaling;

		protected bool m_needsCanGrowFalseScript;

		protected internal bool m_needsFitVertTextScript;

		protected internal bool m_needsGrowRectangleScript;

		internal static string m_searchHitIdPrefix = "oHit";

		internal static string m_standardLineBreak = "\n";

		internal const char StreamNameSeparator = '_';

		internal const string PageStyleName = "p";

		internal const string MHTMLPrefix = "cid:";

		internal const string CSSSuffix = "style";

		protected const string m_resourceNamespace = "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources";

		protected bool m_pageHasStyle;

		protected internal bool m_isBody;

		protected internal bool m_usePercentWidth;

		internal bool m_expandItem;

		protected internal ReportContext m_reportContext;

		private bool m_renderTableHeight;

		private string m_contextLanguage;

		private bool m_allowBandTable = true;

		protected byte[] m_styleClassPrefix;

		protected internal bool IsFragment
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

		internal string SearchText
		{
			set
			{
				m_searchText = value;
			}
		}

		internal bool NeedResizeImages => m_fitPropImages;

		public bool IsBrowserIE => m_deviceInfo.IsBrowserIE;

		protected virtual bool FillPageHeight => m_deviceInfo.IsBrowserIE;

		private string GetCurrentHtmlAsText()
		{
			MemoryStream memoryStream = new MemoryStream();
			m_mainStream.Seek(0L, SeekOrigin.Begin);
			StreamSupport.CopyStreamUsingBuffer(m_mainStream, memoryStream, 1024);
			m_mainStream.Seek(0L, SeekOrigin.End);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}

		public HTML5Renderer(IReportWrapper report, ISPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams, IElementExtender elementExtender = null)
		{
			SearchText = deviceInfo.FindString;
			m_report = report;
			m_spbProcessing = spbProcessing;
			m_elementExtender = (elementExtender ?? new NoOpElementExtender());
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
			m_reportContext = new ReportContext();
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

		private static bool HasBorderStyle(object borderStyle)
		{
			if (borderStyle != null)
			{
				return (RPLFormat.BorderStyles)borderStyle != RPLFormat.BorderStyles.None;
			}
			return false;
		}

		protected virtual void RenderInteractionAction(RPLAction action, ref bool hasHref)
		{
			RenderControlActionScript(action);
			WriteStream(HTMLElements.m_href);
			WriteStream(HTMLElements.m_quote);
			OpenStyle();
			WriteStream(HTMLElements.m_cursorHand);
			WriteStream(HTMLElements.m_semiColon);
			hasHref = true;
		}

		protected void RenderControlActionScript(RPLAction action)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			string actionUrl = null;
			if (action.DrillthroughId != null)
			{
				QuoteString(stringBuilder, action.DrillthroughId);
				text = "Drillthrough";
				actionUrl = action.DrillthroughUrl;
			}
			else
			{
				stringBuilder.Append(HttpUtility.HtmlAttributeEncode(action.BookmarkLink));
				text = "Bookmark";
			}
			RenderOnClickActionScript(text, stringBuilder.ToString(), actionUrl);
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

		public virtual void WriteStream(byte[] theBytes)
		{
			m_mainStream.Write(theBytes, 0, theBytes.Length);
		}

		internal void WriteAccesibilityTags(string nameToUseIfNoTooltip, RPLElementProps itemProperties, bool reportItemIsTopLevel)
		{
			if (!reportItemIsTopLevel)
			{
				return;
			}
			if (itemProperties.Definition is RPLTextBoxPropsDef)
			{
				string tooltip = GetTooltip(itemProperties);
				if (!string.IsNullOrEmpty(tooltip))
				{
					return;
				}
			}
			WriteAriaAccessibleTags(nameToUseIfNoTooltip);
		}

		protected void WriteAriaRole(string roleName)
		{
			WriteNameValuePair(HTMLElements.m_role, roleName);
		}

		internal void WriteAriaPresentationRole()
		{
			WriteNameValuePair(HTMLElements.m_role, HTMLElements.m_presentationRole);
		}

		internal void WriteAriaAccessibleTags(string accessibleAriaName)
		{
			WriteNameValuePair(HTMLElements.m_role, HTMLElements.m_navigationRole);
			if (!string.IsNullOrEmpty(accessibleAriaName))
			{
				WriteNameValuePair(HTMLElements.m_ariaLabel, AntiXssEncoder.XmlAttributeEncode(accessibleAriaName));
			}
		}

		protected internal virtual void RenderReportItemId(string repItemId)
		{
			WriteStream(HTMLElements.m_id);
			WriteReportItemId(repItemId);
			WriteStream(HTMLElements.m_quote);
		}

		internal string GetTooltip(RPLElementProps props)
		{
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			string toolTip = rPLItemProps.ToolTip;
			if (toolTip == null)
			{
				toolTip = rPLItemPropsDef.ToolTip;
			}
			return toolTip;
		}

		protected internal void WriteToolTip(RPLElementProps props)
		{
			string tooltip = GetTooltip(props);
			if (tooltip != null)
			{
				WriteToolTipAttribute(tooltip);
			}
		}

		protected internal void WriteToolTipAttribute(string tooltip)
		{
			WriteAttrEncoded(HTMLElements.m_alt, tooltip);
			WriteAttrEncoded(HTMLElements.m_aria, tooltip);
			WriteAttrEncoded(HTMLElements.m_title, tooltip);
		}

		protected internal void WriteNameValuePair(string name, string value)
		{
			WriteStream(HTMLElements.m_space);
			WriteStream(name);
			WriteStream(HTMLElements.m_equal);
			WriteStream(HTMLElements.m_quote);
			WriteStream(value);
			WriteStream(HTMLElements.m_quote);
		}

		internal void OpenStyle()
		{
			if (!m_isStyleOpen)
			{
				m_isStyleOpen = true;
				WriteStream(HTMLElements.m_openStyle);
			}
		}

		internal void CloseStyle(bool renderQuote)
		{
			if (m_isStyleOpen && renderQuote)
			{
				WriteStream(HTMLElements.m_quote);
			}
			m_isStyleOpen = false;
		}

		protected internal void RenderMeasurementHeight(float height, bool renderMin)
		{
			if (renderMin)
			{
				WriteStream(HTMLElements.m_styleMinHeight);
			}
			else
			{
				WriteStream(HTMLElements.m_styleHeight);
			}
			WriteRSStream(height);
			WriteStream(HTMLElements.m_semiColon);
		}

		internal void RenderMeasurementMinHeight(float height)
		{
			WriteStream(HTMLElements.m_styleMinHeight);
			WriteRSStream(height);
			WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementWidth(float width, bool renderMinWidth)
		{
			WriteStream(HTMLElements.m_styleWidth);
			WriteRSStream(width);
			WriteStream(HTMLElements.m_semiColon);
			if (renderMinWidth)
			{
				RenderMeasurementMinWidth(width);
			}
		}

		protected internal void RenderMeasurementMaxHeight(float maxHeight)
		{
			WriteStream(HTMLElements.m_styleMaxHeight);
			WriteRSStream(maxHeight);
			WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementMinWidth(float minWidth)
		{
			WriteStream(HTMLElements.m_styleMinWidth);
			WriteRSStream(minWidth);
			WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementMaxWidth(float maxWidth)
		{
			WriteStream(HTMLElements.m_styleMaxWidth);
			WriteRSStream(maxWidth);
			WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementLeft(float left)
		{
			WriteStream(HTMLElements.m_styleLeft);
			WriteRSStream(left);
			WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementHeight(float height)
		{
			RenderMeasurementHeight(height, renderMin: false);
		}

		protected void RenderMeasurementWidth(float width)
		{
			RenderMeasurementWidth(width, renderMinWidth: false);
		}

		internal void WriteReportItemId(string repItemId)
		{
			WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
			WriteStream(repItemId);
		}

		internal void WriteAttrEncoded(byte[] attributeName, string theString)
		{
			WriteAttribute(attributeName, m_encoding.GetBytes(AntiXssEncoder.XmlAttributeEncode(theString)));
		}

		internal void WriteRSStream(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			WriteStream(HTMLElements.m_mm);
		}

		internal void WriteAttrEncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlAttributeEncode(theString));
		}

		protected virtual void WriteAttribute(byte[] attributeName, byte[] value)
		{
			WriteStream(attributeName);
			WriteStream(value);
			WriteStream(HTMLElements.m_quote);
		}

		internal void RenderNavigationId(string navigationId)
		{
			if (!IsFragment)
			{
				WriteStream(HTMLElements.m_openSpan);
				WriteStream(HTMLElements.m_id);
				WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
				WriteStream(navigationId);
				WriteStream(HTMLElements.m_closeTag);
			}
		}

		protected internal void WriteDStream(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
		}

		protected internal bool NeedReportItemId(RPLElement repItem, RPLElementProps props)
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

		protected internal static float GetInnerContainerWidthSubtractHalfBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			return GetInnerContainerMeasurementSubtractingHalfBorders(measurement, containerStyle, 6, 11, 7, 12, GetInnerContainerWidth(measurement, containerStyle));
		}

		internal static float GetInnerContainerHeightSubtractHalfBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			return GetInnerContainerMeasurementSubtractingHalfBorders(measurement, containerStyle, 8, 13, 9, 14, GetInnerContainerHeight(measurement, containerStyle));
		}

		private static float GetInnerContainerMeasurementSubtractingHalfBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle, byte border1StyleProps, byte border1WidthProps, byte border2StyleProps, byte border2WidthProps, float length)
		{
			if (measurement == null)
			{
				return -1f;
			}
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderStyle = containerStyle[border1StyleProps];
			object specificBorderWidth = containerStyle[border1WidthProps];
			object specificBorderStyle2 = containerStyle[border2StyleProps];
			object specificBorderWidth2 = containerStyle[border2WidthProps];
			float width = 0f;
			width = SubtractBorderStyles(width, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			width = SubtractBorderStyles(width, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
			length += 0.5f * width;
			return Math.Max(length, 1f);
		}

		internal float GetInnerContainerHeightSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
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

		protected internal void RenderElementHyperlinkAllTextStyles(RPLElementStyle style, RPLAction action, string id)
		{
			WriteStream(HTMLElements.m_openA);
			RenderTabIndex();
			bool hasHref = false;
			if (action.Hyperlink != null)
			{
				WriteStream(HTMLElements.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTMLElements.m_quoteString);
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
				WriteStream(HTMLElements.m_target);
				WriteStream(m_deviceInfo.LinkTarget);
				WriteStream(HTMLElements.m_quote);
			}
			WriteStream(HTMLElements.m_closeBracket);
		}

		protected internal static int GetNewContext(int borderContext, bool left, bool right, bool top, bool bottom)
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

		protected internal virtual bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
		{
			if (!IsFragment)
			{
				return textBoxProps.IsToggleParent;
			}
			return false;
		}

		protected internal virtual bool CanSort(RPLTextBoxPropsDef textBoxDef)
		{
			if (!IsFragment)
			{
				return textBoxDef.CanSort;
			}
			return false;
		}

		protected internal string GetTextBoxClass(RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, RPLStyleProps nonSharedStyle, string defaultClass)
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

		internal bool HasAction(RPLActionInfo actionInfo)
		{
			if (actionInfo != null && actionInfo.Actions != null)
			{
				return HasAction(actionInfo.Actions[0]);
			}
			return false;
		}

		internal static float GetInnerContainerHeight(RPLItemMeasurement measurement, IRPLStyle containerStyle)
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

		protected static float SubtractBorderStyles(float width, object defaultBorderStyle, object specificBorderStyle, object defaultBorderWidth, object specificBorderWidth)
		{
			object obj = specificBorderWidth ?? defaultBorderWidth;
			if (obj != null && (HasBorderStyle(specificBorderStyle) || (specificBorderStyle == null && HasBorderStyle(defaultBorderStyle))))
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				width -= (float)rPLReportSize.ToMillimeters();
			}
			return width;
		}

		internal void RenderTabIndex()
		{
			WriteStream(HTMLElements.m_tabIndex);
			WriteStream(++m_tabIndexNum);
			WriteStream(HTMLElements.m_quote);
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

		internal void RenderOnClickActionScript(string actionType, string actionArg, string actionUrl = null)
		{
			WriteStream(" onclick=\"");
			WriteStream(m_deviceInfo.ActionScript);
			WriteStream("('");
			WriteStream(actionType);
			WriteStream("','");
			WriteStream(actionArg);
			WriteStream("',event);return false;\"");
			WriteStream(" onkeypress=\"");
			WriteStream(HTMLElements.m_checkForEnterKey);
			WriteStream(m_deviceInfo.ActionScript);
			WriteStream("('");
			WriteStream(actionType);
			WriteStream("','");
			WriteStream(actionArg);
			WriteStream("',event);return false;}\"");
			if (!string.IsNullOrEmpty(actionUrl))
			{
				WriteStream(" data-drillThroughUrl=\"");
				WriteStream(actionUrl);
				WriteStream(HTMLElements.m_quote);
			}
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

		internal string GetReportItemPath(string reportItemName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in m_reportContext.GetPath())
			{
				stringBuilder.Append(item).Append('/');
			}
			stringBuilder.Append(reportItemName);
			return stringBuilder.ToString();
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

		protected internal abstract void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState);

		protected abstract void RenderInternalImageSrc();

		protected internal abstract void RenderToggleImage(RPLTextBoxProps textBoxProps);

		public abstract void Render(HtmlTextWriter outputWriter);

		protected abstract void WriteScrollbars();

		protected abstract void WriteFixedHeaderOnScrollScript();

		protected abstract void WriteFixedHeaderPropertyChangeScript();

		protected internal abstract void WriteFitProportionalScript(double pv, double ph);

		internal void RenderStylesOnly(string streamName)
		{
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
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
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
				HTML5ParagraphStyleWriter hTML5ParagraphStyleWriter = new HTML5ParagraphStyleWriter(this, rPLTextBox);
				TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
				for (RPLParagraph nextParagraph = rPLTextBox.GetNextParagraph(); nextParagraph != null; nextParagraph = rPLTextBox.GetNextParagraph())
				{
					hTML5ParagraphStyleWriter.Paragraph = nextParagraph;
					string iD2 = nextParagraph.ElementProps.Definition.ID;
					hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.All;
					RenderSharedStyle(hTML5ParagraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2);
					hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.ListOnly;
					RenderSharedStyle(hTML5ParagraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "l");
					hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.ParagraphOnly;
					RenderSharedStyle(hTML5ParagraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "p");
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
							if (rPLTextBox2 != null && IsWritingModeVertical(sharedStyle2))
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
			WriteStream(HTMLElements.m_dot);
			WriteStream(m_stylePrefixIdBytes);
			WriteStream(id);
			WriteStream(HTMLElements.m_openAccol);
		}

		protected virtual RPLReport GetNextPage()
		{
			m_spbProcessing.GetNextPage(out RPLReport rplReport);
			return rplReport;
		}

		protected internal void RenderSortImage(RPLTextBoxProps textBoxProps)
		{
			WriteStream(HTMLElements.m_openA);
			WriteStream(HTMLElements.m_tabIndex);
			WriteStream(++m_tabIndexNum);
			WriteStream(HTMLElements.m_quote);
			RPLFormat.SortOptions sortState = textBoxProps.SortState;
			RenderSortAction(textBoxProps, sortState);
			WriteStream(HTMLElements.m_img);
			WriteStream(HTMLElements.m_alt);
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
			WriteStream(HTMLElements.m_quote);
			if (m_browserIE)
			{
				WriteStream(HTMLElements.m_imgOnError);
			}
			WriteStream(HTMLElements.m_zeroBorder);
			WriteStream(HTMLElements.m_src);
			RenderSortImageText(sortState);
			WriteStream(HTMLElements.m_quote);
			WriteStream(HTMLElements.m_openStyle);
			WriteStream("float:left;");
			WriteStream(HTMLElements.m_closeTag);
			WriteStream(HTMLElements.m_closeA);
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

		protected internal PaddingSharedInfo GetPaddings(RPLElementStyle style, PaddingSharedInfo paddingInfo)
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
					WriteStream(HTMLElements.m_openTable);
					WriteStream(HTMLElements.m_closeBracket);
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
						WriteStream(HTMLElements.m_firstTD);
						styleContext.StyleOnCell = true;
						RenderReportItemStyle(rPLBody, rPLItemProps, rPLItemPropsDef, null, styleContext, ref num, rPLItemPropsDef.ID + "c");
						styleContext.StyleOnCell = false;
						WriteStream(HTMLElements.m_closeBracket);
					}
					m_pageSection = PageSection.Body;
					m_isBody = true;
					RPLItemMeasurement rPLItemMeasurement2 = new RPLItemMeasurement();
					rPLItemMeasurement2.Width = m_pageContent.MaxSectionWidth;
					rPLItemMeasurement2.Height = m_rplReportSection.BodyArea.Height;
					new RectangleRenderer(this).RenderReportItem(rPLBody, rPLItemMeasurement2, styleContext, ref num, renderId: false, treatAsTopLevel: true);
					if (flag2)
					{
						WriteStream(HTMLElements.m_closeTD);
						WriteStream(HTMLElements.m_closeTR);
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
					WriteStream(HTMLElements.m_closeTable);
				}
				if (m_elementExtender.HasSetupRequirements())
				{
					WriteStream(m_elementExtender.SetupRequirements());
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
						WriteStream(HTMLElements.m_pageBreakDelimiter);
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

		protected virtual void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
		{
			WriteStream(HTMLElements.m_openDiv);
			WriteStream(HTMLElements.m_ltrDir);
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
				WriteStream(HTMLElements.m_closeBracket);
				WriteStream(HTMLElements.m_openDiv);
				OpenStyle();
				if (FillPageHeight)
				{
					WriteStream(HTMLElements.m_styleHeight);
					WriteStream(HTMLElements.m_percent);
					WriteStream(HTMLElements.m_semiColon);
				}
				WriteStream(HTMLElements.m_styleWidth);
				WriteStream(HTMLElements.m_percent);
				WriteStream(HTMLElements.m_semiColon);
				RenderPageStyle(pageStyle);
				CloseStyle(renderQuote: true);
			}
			WriteStream(HTMLElements.m_closeBracket);
			WriteStream(HTMLElements.m_openTable);
			WriteStream(HTMLElements.m_closeBracket);
			WriteStream(HTMLElements.m_firstTD);
			if (firstPage)
			{
				RenderReportItemId("oReportCell");
			}
			if (flag)
			{
				WriteFixedHeaderPropertyChangeScript();
			}
			WriteStream(HTMLElements.m_closeBracket);
		}

		protected virtual void RenderPageStartDimensionStyles(bool lastPage)
		{
			if (m_pageNum != 0 || lastPage)
			{
				WriteStream(HTMLElements.m_openStyle);
				WriteScrollbars();
				if (m_deviceInfo.IsBrowserIE)
				{
					WriteStream(HTMLElements.m_styleHeight);
					WriteStream(HTMLElements.m_percent);
					WriteStream(HTMLElements.m_semiColon);
				}
				WriteStream(HTMLElements.m_styleWidth);
				WriteStream(HTMLElements.m_percent);
				WriteStream(HTMLElements.m_semiColon);
				WriteStream("direction:ltr");
				WriteStream(HTMLElements.m_quote);
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
						WriteStream(HTMLElements.m_closeAccol);
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

		public void WriteClassName(byte[] className, byte[] classNameIfNoPrefix)
		{
			if (m_deviceInfo.HtmlPrefixId.Length > 0 || classNameIfNoPrefix == null)
			{
				WriteStream(HTMLElements.m_classStyle);
				WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
				WriteStream(className);
				WriteStream(HTMLElements.m_quote);
			}
			else
			{
				WriteStream(classNameIfNoPrefix);
			}
		}

		protected virtual void WriteClassStyle(byte[] styleBytes, bool close)
		{
			WriteStream(HTMLElements.m_classStyle);
			WriteStream(m_stylePrefixIdBytes);
			WriteStream(styleBytes);
			if (close)
			{
				WriteStream(HTMLElements.m_quote);
			}
		}

		protected void RenderBackgroundStyleProps(IRPLStyle style)
		{
			object obj = style[34];
			if (obj != null)
			{
				WriteStream(HTMLElements.m_backgroundColor);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = style[33];
			if (obj != null)
			{
				WriteStream(HTMLElements.m_backgroundImage);
				RenderImageUrl(useSessionId: true, (RPLImageData)obj);
				WriteStream(HTMLElements.m_closeBrace);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = style[35];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BackgroundRepeatTypes)obj);
				WriteStream(HTMLElements.m_backgroundRepeat);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
		}

		protected virtual void RenderPageEnd()
		{
			if (m_deviceInfo.ExpandContent)
			{
				WriteStream(HTMLElements.m_lastTD);
				WriteStream(HTMLElements.m_closeTable);
			}
			else
			{
				WriteStream(HTMLElements.m_closeTD);
				WriteStream(HTMLElements.m_openTD);
				WriteStream(HTMLElements.m_inlineWidth);
				WriteStream(HTMLElements.m_percent);
				WriteStream(HTMLElements.m_quote);
				WriteStream(HTMLElements.m_inlineHeight);
				WriteStream("0");
				WriteStream(HTMLElements.m_closeQuote);
				WriteStream(HTMLElements.m_lastTD);
				WriteStream(HTMLElements.m_firstTD);
				WriteStream(HTMLElements.m_inlineWidth);
				if (m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(HTMLElements.m_percent);
				}
				else
				{
					WriteStream("0");
				}
				WriteStream(HTMLElements.m_quote);
				WriteStream(HTMLElements.m_inlineHeight);
				WriteStream(HTMLElements.m_percent);
				WriteStream(HTMLElements.m_closeQuote);
				WriteStream(HTMLElements.m_lastTD);
				WriteStream(HTMLElements.m_closeTable);
			}
			if (m_pageHasStyle)
			{
				WriteStream(HTMLElements.m_closeDiv);
			}
			WriteStream(HTMLElements.m_closeDiv);
		}

		internal void WriteStream(object theString)
		{
			if (theString != null)
			{
				WriteStream(theString.ToString());
			}
		}

		public void WriteStreamCR(string theString)
		{
			WriteStream(theString);
		}

		public void WriteStreamCR(byte[] theBytes)
		{
			WriteStream(theBytes);
		}

		protected internal void WriteStreamEncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected void WriteStreamCREncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected virtual void WriteStreamLineBreak()
		{
		}

		protected internal void WriteRSStreamCR(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			WriteStreamCR(HTMLElements.m_mm);
		}

		public void WriteIdToSecondaryStream(Stream secondaryStream, string tagId)
		{
			Stream mainStream = m_mainStream;
			m_mainStream = secondaryStream;
			WriteReportItemId(tagId);
			WriteStream(HTMLElements.m_comma);
			m_mainStream = mainStream;
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
		{
			return RenderSharedStyle(reportItem, props, definition, sharedStyle, null, measurement, id, styleContext, ref borderContext);
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLStyleProps nonSharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext, bool renderDirectionStyles = false)
		{
			Stream mainStream = m_mainStream;
			m_mainStream = m_styleStream;
			RenderOpenStyle(id);
			byte omitBordersState = styleContext.OmitBordersState;
			styleContext.OmitBordersState = 0;
			RenderStyleProps(reportItem, props, definition, measurement, sharedStyle, nonSharedStyle, styleContext, ref borderContext, isNonSharedStyles: false, renderDirectionStyles);
			styleContext.OmitBordersState = omitBordersState;
			WriteStream(HTMLElements.m_closeAccol);
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
			WriteStream(HTMLElements.m_closeAccol);
			m_mainStream = mainStream;
			byte[] bytes = m_encoding.GetBytes(id);
			m_usedStyles.Add(id, bytes);
			return bytes;
		}

		protected internal void RenderMeasurementStyle(float height, float width)
		{
			RenderMeasurementStyle(height, width, renderMin: false);
		}

		protected void RenderMeasurementStyle(float height, float width, bool renderMin)
		{
			RenderMeasurementHeight(height, renderMin);
			RenderMeasurementWidth(width, renderMinWidth: true);
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

		protected internal virtual void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
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

		protected internal void RenderHtmlBorders(IRPLStyle styleProps, ref int borderContext, byte omitBordersState, bool renderPadding, bool isNonShared, IRPLStyle sharedStyleProps)
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
					WriteStream(HTMLElements.m_paddingLeft);
					WriteStream(obj);
					WriteStream(HTMLElements.m_semiColon);
				}
				obj = styleProps[17];
				if (obj != null)
				{
					WriteStream(HTMLElements.m_paddingTop);
					WriteStream(obj);
					WriteStream(HTMLElements.m_semiColon);
				}
				obj = styleProps[16];
				if (obj != null)
				{
					WriteStream(HTMLElements.m_paddingRight);
					WriteStream(obj);
					WriteStream(HTMLElements.m_semiColon);
				}
				obj = styleProps[18];
				if (obj != null)
				{
					WriteStream(HTMLElements.m_paddingBottom);
					WriteStream(obj);
					WriteStream(HTMLElements.m_semiColon);
				}
			}
		}

		internal bool IsLineSlanted(RPLItemMeasurement measurement)
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

		protected void RenderCellItem(PageTableCell currCell, int borderContext, bool layoutExpand, bool treatAsTopLevel)
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
				WriteStream(HTMLElements.m_openDiv);
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
				WriteStream(HTMLElements.m_closeBracket);
			}
			RenderReportItem(element, rPLItemProps, rPLItemPropsDef, rPLItemMeasurement, new StyleContext(), borderContext, flag, treatAsTopLevel);
			if (flag2)
			{
				WriteStream(HTMLElements.m_closeDiv);
			}
			rPLItemMeasurement.Element = null;
		}

		protected virtual void RenderBlankImage()
		{
			WriteStream(HTMLElements.m_img);
			if (m_browserIE)
			{
				WriteStream(HTMLElements.m_imgOnError);
			}
			WriteStream(HTMLElements.m_src);
			RenderInternalImageSrc();
			WriteStream(m_report.GetImageName("Blank.gif"));
			WriteStream(HTMLElements.m_quote);
			WriteStream(HTMLElements.m_alt);
			WriteAttrEncoded(RenderRes.BlankAltText);
			WriteStream(HTMLElements.m_closeTag);
		}

		internal virtual string GetImageUrl(bool useSessionId, RPLImageData image)
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
			else return null;

			return "data:" + image.ImageMimeType + ";base64," + Convert.ToBase64String(blob);
		}

		internal void RenderImageUrl(bool useSessionId, RPLImageData image)
		{
			string imageUrl = GetImageUrl(useSessionId, image);
			if (imageUrl != null)
			{
				WriteStream(imageUrl.Replace("(", "%28").Replace(")", "%29"));
			}
		}

		internal void WriteOuterConsolidation(System.Drawing.Rectangle consolidationOffsets, RPLFormat.Sizings sizing, string propsUniqueName)
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
			WriteStream(HTMLElements.m_styleWidth);
			if (flag)
			{
				WriteStream("1");
			}
			else
			{
				WriteStream(consolidationOffsets.Width);
			}
			WriteStream(HTMLElements.m_px);
			WriteStream(HTMLElements.m_semiColon);
			WriteStream(HTMLElements.m_styleHeight);
			if (flag)
			{
				WriteStream("1");
			}
			else
			{
				WriteStream(consolidationOffsets.Height);
			}
			WriteStream(HTMLElements.m_px);
			WriteStream(HTMLElements.m_semiColon);
			WriteStream(HTMLElements.m_overflowHidden);
			WriteStream(HTMLElements.m_semiColon);
			if (m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				WriteStream(HTMLElements.m_stylePositionAbsolute);
			}
		}

		protected internal void WriteClippedDiv(System.Drawing.Rectangle clipCoordinates)
		{
			OpenStyle();
			WriteStream(HTMLElements.m_styleTop);
			if (clipCoordinates.Top > 0)
			{
				WriteStream("-");
			}
			WriteStream(clipCoordinates.Top);
			WriteStream(HTMLElements.m_px);
			WriteStream(HTMLElements.m_semiColon);
			WriteStream(HTMLElements.m_styleLeft);
			if (clipCoordinates.Left > 0)
			{
				WriteStream("-");
			}
			WriteStream(clipCoordinates.Left);
			WriteStream(HTMLElements.m_px);
			WriteStream(HTMLElements.m_semiColon);
			WriteStream(HTMLElements.m_stylePositionRelative);
			CloseStyle(renderQuote: true);
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

		protected internal int RenderReportItem(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, int borderContext, bool renderId, bool treatAsTopLevel)
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
				new TextBoxRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLTablix)
			{
				new TablixRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLRectangle)
			{
				new RectangleRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)
			{
				new ServerDynamicImageRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLSubReport)
			{
				new SubReportRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLImage)
			{
				new ImageRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLLine)
			{
				new LineRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref borderContext2, renderId, treatAsTopLevel);
			}
			return borderContext2;
		}

		private void WriteFontSizeSmallPoint()
		{
			if (m_deviceInfo.IsBrowserGeckoEngine)
			{
				WriteStream(HTMLElements.m_smallPoint);
			}
			else
			{
				WriteStream(HTMLElements.m_zeroPoint);
			}
		}

		protected void RenderPageHeaderFooter(RPLItemMeasurement hfMeasurement)
		{
			if (hfMeasurement.Height != 0f)
			{
				RPLHeaderFooter rPLHeaderFooter = (RPLHeaderFooter)hfMeasurement.Element;
				int borderContext = 0;
				StyleContext styleContext = new StyleContext();
				WriteStream(HTMLElements.m_openTR);
				WriteStream(HTMLElements.m_closeBracket);
				WriteStream(HTMLElements.m_openTD);
				styleContext.StyleOnCell = true;
				RenderReportItemStyle(rPLHeaderFooter, rPLHeaderFooter.ElementProps, rPLHeaderFooter.ElementProps.Definition, null, styleContext, ref borderContext, rPLHeaderFooter.ElementProps.Definition.ID + "c");
				styleContext.StyleOnCell = false;
				WriteStream(HTMLElements.m_closeBracket);
				WriteStream(HTMLElements.m_openDiv);
				if (!m_deviceInfo.IsBrowserIE)
				{
					styleContext.RenderMeasurements = false;
					styleContext.RenderMinMeasurements = true;
				}
				RenderReportItemStyle(rPLHeaderFooter, hfMeasurement, ref borderContext, styleContext);
				WriteStreamCR(HTMLElements.m_closeBracket);
				RPLItemMeasurement[] children = rPLHeaderFooter.Children;
				if (children != null && children.Length != 0)
				{
					m_renderTableHeight = true;
					GenerateHTMLTable(children, 0f, 0f, m_pageContent.MaxSectionWidth, hfMeasurement.Height, borderContext, expandLayout: false, SharedListLayoutState.None, null, rPLHeaderFooter.ElementProps.Style, treatAsTopLevel: false);
				}
				else
				{
					WriteStream(HTMLElements.m_nbsp);
				}
				m_renderTableHeight = false;
				WriteStreamCR(HTMLElements.m_closeDiv);
				WriteStream(HTMLElements.m_closeTD);
				WriteStream(HTMLElements.m_closeTR);
			}
		}

		protected void RenderStyleProps(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, StyleContext styleContext, ref int borderContext, bool isNonSharedStyles, bool renderDirectionStyles)
		{
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTMLElements.m_displayNone);
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
						WriteStream(HTMLElements.m_verticalAlign);
						WriteStream(obj);
						WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[25];
					if (obj != null)
					{
						if ((RPLFormat.TextAlignments)obj != 0)
						{
							obj = EnumStrings.GetValue((RPLFormat.TextAlignments)obj);
							WriteStream(HTMLElements.m_textAlign);
							WriteStream(obj);
							WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							RenderTextAlign(props as RPLTextBoxProps, props.Style);
						}
					}
					if (renderDirectionStyles)
					{
						RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
					}
				}
				if (measurement != null && m_deviceInfo.OutlookCompat)
				{
					float width = measurement.Width;
					if ((!(reportItem is RPLTextBox) && !IsImageNotFitProportional(reportItem, definition)) || styleContext.InTablix)
					{
						RenderMeasurementMinWidth(width);
					}
					RenderMeasurementWidth(width, renderMinWidth: false);
				}
				RenderTextWrapping(reportItem, renderDirectionStyles);
				return;
			}
			RenderTextWrapping(reportItem, renderDirectionStyles);
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
							WriteStream(HTMLElements.m_styleWidth);
						}
						else if (styleContext.RenderMinMeasurements)
						{
							WriteStream(HTMLElements.m_styleMinWidth);
						}
						WriteStream(HTMLElements.m_percent);
						WriteStream(HTMLElements.m_semiColon);
						if (rPLTextBoxPropsDef.CanGrow)
						{
							WriteStream(HTMLElements.m_overflowXHidden);
						}
						else
						{
							if (styleContext.RenderMeasurements)
							{
								WriteStream(HTMLElements.m_styleHeight);
							}
							else if (styleContext.RenderMinMeasurements)
							{
								WriteStream(HTMLElements.m_styleMinHeight);
							}
							WriteStream(HTMLElements.m_percent);
							WriteStream(HTMLElements.m_semiColon);
							WriteStream(HTMLElements.m_overflowHidden);
						}
						WriteStream(HTMLElements.m_semiColon);
					}
					else if (!(reportItem is RPLTablix))
					{
						RenderPercentSizes();
					}
				}
				else if (!renderDirectionStyles)
				{
					if (reportItem is RPLTextBox)
					{
						float width2 = measurement.Width;
						float height = measurement.Height;
						RenderMeasurementMinWidth(width2);
						RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)definition;
						if (rPLTextBoxPropsDef2.CanGrow && rPLTextBoxPropsDef2.CanShrink)
						{
							RenderMeasurementWidth(width2, renderMinWidth: false);
						}
						else
						{
							WriteStream(HTMLElements.m_overflowHidden);
							WriteStream(HTMLElements.m_semiColon);
							RenderMeasurementWidth(width2, renderMinWidth: false);
							if (rPLTextBoxPropsDef2.CanShrink)
							{
								RenderMeasurementMaxHeight(height);
							}
							else if (!rPLTextBoxPropsDef2.CanGrow)
							{
								RenderMeasurementHeight(height);
							}
						}
					}
					else if (!(reportItem is RPLTablix))
					{
						if (!(reportItem is RPLRectangle))
						{
							float height2 = measurement.Height;
							float width3 = measurement.Width;
							RenderMeasurementMinWidth(width3);
							if (reportItem is RPLHeaderFooter)
							{
								RenderMeasurementMinHeight(height2);
							}
							else
							{
								RenderMeasurementHeight(height2);
							}
							RenderMeasurementWidth(width3, renderMinWidth: false);
						}
						if (empty || reportItem is RPLImage)
						{
							WriteStream(HTMLElements.m_overflowHidden);
							WriteStream(HTMLElements.m_semiColon);
						}
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
				WriteStream(HTMLElements.m_fontStyle);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[20];
			if (obj != null)
			{
				WriteStream(HTMLElements.m_fontFamily);
				WriteStream(HandleSpecialFontCharacters(obj.ToString()));
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[21];
			if (obj != null)
			{
				WriteStream(HTMLElements.m_fontSize);
				if (string.Compare(obj.ToString(), "0pt", StringComparison.OrdinalIgnoreCase) != 0)
				{
					WriteStream(obj);
				}
				else
				{
					WriteFontSizeSmallPoint();
				}
				WriteStream(HTMLElements.m_semiColon);
			}
			else
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef3 = definition as RPLTextBoxPropsDef;
				RPLStyleProps sharedStyle = reportItem.ElementPropsDef.SharedStyle;
				if ((!isNonSharedStyles || sharedStyle == null || sharedStyle.Count == 0) && rPLTextBoxPropsDef3 != null && !rPLTextBoxPropsDef3.IsSimple)
				{
					WriteStream(HTMLElements.m_fontSize);
					WriteFontSizeSmallPoint();
					WriteStream(HTMLElements.m_semiColon);
				}
			}
			obj = iRPLStyle[22];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.FontWeights)obj);
				WriteStream(HTMLElements.m_fontWeight);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[24];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
				WriteStream(HTMLElements.m_textDecoration);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[31];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.UnicodeBiDiTypes)obj);
				WriteStream(HTMLElements.m_unicodeBiDi);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[27];
			if (obj != null)
			{
				WriteStream(HTMLElements.m_color);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[28];
			if (obj != null)
			{
				WriteStream(HTMLElements.m_lineHeight);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			if ((IsWritingModeVertical(sharedStyleProps) || IsWritingModeVertical(nonSharedStyleProps)) && reportItem is RPLTextBox && styleContext.InTablix && m_deviceInfo.IsBrowserIE && !styleContext.IgnorePadding)
			{
				RenderPaddingStyle(iRPLStyle);
			}
			if (renderDirectionStyles)
			{
				RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
			}
			obj = iRPLStyle[26];
			if (obj != null && !styleContext.IgnoreVerticalAlign)
			{
				obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
				WriteStream(HTMLElements.m_verticalAlign);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[25];
			if (obj != null)
			{
				if ((RPLFormat.TextAlignments)obj != 0)
				{
					WriteStream(HTMLElements.m_textAlign);
					WriteStream(EnumStrings.GetValue((RPLFormat.TextAlignments)obj));
					WriteStream(HTMLElements.m_semiColon);
				}
				else
				{
					RenderTextAlign(props as RPLTextBoxProps, props.Style);
				}
			}
		}

		protected internal bool GenerateHTMLTable(RPLItemMeasurement[] repItemCol, float ownerTop, float ownerLeft, float dxParent, float dyParent, int borderContext, bool expandLayout, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style, bool treatAsTopLevel)
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
					RenderCellItem(tableLayout.GetCell(k), borderContext2, layoutExpand: false, treatAsTopLevel);
				}
				if (borderContext > 0)
				{
					borderContext2 = GetNewContext(borderContext, k + 1, 1, tableLayout.NrRows, 1);
				}
				RenderCellItem(tableLayout.GetCell(k), borderContext2, layoutExpand: false, treatAsTopLevel);
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
				WriteStream(HTMLElements.m_openTable);
				WriteStream(HTMLElements.m_zeroBorder);
				if (flag2)
				{
					num2++;
				}
				if (!m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(HTMLElements.m_cols);
					WriteStream(num2.ToString(CultureInfo.InvariantCulture));
					WriteStream(HTMLElements.m_quote);
				}
				RenderReportLanguage();
				WriteAriaPresentationRole();
				if (m_useInlineStyle)
				{
					OpenStyle();
					WriteStream(HTMLElements.m_borderCollapse);
					if (expandLayout)
					{
						WriteStream(HTMLElements.m_semiColon);
						WriteStream(HTMLElements.m_styleHeight);
						WriteStream(HTMLElements.m_percent);
					}
				}
				else
				{
					ClassLayoutBorder();
					if (expandLayout)
					{
						WriteStream(HTMLElements.m_space);
						WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
						WriteStream(HTMLElements.m_percentHeight);
					}
					WriteStream(HTMLElements.m_quote);
				}
				if (m_renderTableHeight)
				{
					if (m_isStyleOpen)
					{
						WriteStream(HTMLElements.m_semiColon);
					}
					else
					{
						OpenStyle();
					}
					WriteStream(HTMLElements.m_styleHeight);
					WriteDStream(dyParent);
					WriteStream(HTMLElements.m_mm);
					m_renderTableHeight = false;
				}
				if (m_deviceInfo.OutlookCompat || m_deviceInfo.IsBrowserSafari)
				{
					if (m_isStyleOpen)
					{
						WriteStream(HTMLElements.m_semiColon);
					}
					else
					{
						OpenStyle();
					}
					WriteStream(HTMLElements.m_styleWidth);
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
					WriteStream(HTMLElements.m_mm);
				}
				CloseStyle(renderQuote: true);
				WriteStream(HTMLElements.m_closeBracket);
				if (tableLayout.NrCols > 1)
				{
					flag = tableLayout.NeedExtraRow();
					if (flag)
					{
						WriteStream(HTMLElements.m_openTR);
						WriteStream(HTMLElements.m_zeroHeight);
						WriteStream(HTMLElements.m_closeBracket);
						if (flag2)
						{
							WriteStream(HTMLElements.m_openTD);
							WriteStream(HTMLElements.m_openStyle);
							WriteStream(HTMLElements.m_styleWidth);
							WriteStream("0");
							WriteStream(HTMLElements.m_px);
							WriteStream(HTMLElements.m_closeQuote);
							WriteStream(HTMLElements.m_closeTD);
						}
						for (num = 0; num < tableLayout.NrCols; num++)
						{
							WriteStream(HTMLElements.m_openTD);
							WriteStream(HTMLElements.m_openStyle);
							WriteStream(HTMLElements.m_styleWidth);
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
							WriteStream(HTMLElements.m_mm);
							WriteStream(HTMLElements.m_semiColon);
							WriteStream(HTMLElements.m_styleMinWidth);
							WriteDStream(num4);
							WriteStream(HTMLElements.m_mm);
							WriteStream(HTMLElements.m_closeQuote);
							WriteStream(HTMLElements.m_closeTD);
						}
						WriteStream(HTMLElements.m_closeTR);
					}
				}
			}
			GenerateTableLayoutContent(tableLayout, repItemCol, flag, flag2, renderHeight, borderContext, expandLayout, layoutState, omittedHeaders, style, treatAsTopLevel);
			if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
			{
				if (expandLayout)
				{
					WriteStream(HTMLElements.m_firstTD);
					ClassPercentHeight();
					WriteStream(HTMLElements.m_cols);
					WriteStream(num2.ToString(CultureInfo.InvariantCulture));
					WriteStream(HTMLElements.m_closeQuote);
					WriteStream(HTMLElements.m_lastTD);
				}
				WriteStreamCR(HTMLElements.m_closeTable);
			}
			return result;
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
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentSizes);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentSizesOverflow);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_overflowHidden);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentHeight);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorder);
			writer.WriteStream(HTMLElements.m_borderStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderL);
			writer.WriteStream(HTMLElements.m_borderLeftStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderR);
			writer.WriteStream(HTMLElements.m_borderRightStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderT);
			writer.WriteStream(HTMLElements.m_borderTopStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderB);
			writer.WriteStream(HTMLElements.m_borderBottomStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_layoutBorder);
			writer.WriteStream(HTMLElements.m_borderCollapse);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_layoutFixed);
			writer.WriteStream(HTMLElements.m_borderCollapse);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_tableLayoutFixed);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentWidthOverflow);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_overflowXHidden);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_popupAction);
			writer.WriteStream("position:absolute;display:none;background-color:white;border:1px solid black;");
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_styleAction);
			writer.WriteStream("text-decoration:none;color:black;cursor:pointer;");
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_emptyTextBox);
			writer.WriteStream(HTMLElements.m_fontSize);
			writer.WriteStream(deviceInfo.IsBrowserGeckoEngine ? HTMLElements.m_smallPoint : HTMLElements.m_zeroPoint);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_rtlEmbed);
			writer.WriteStream(HTMLElements.m_direction);
			writer.WriteStream("RTL;");
			writer.WriteStream(HTMLElements.m_unicodeBiDi);
			writer.WriteStream(EnumStrings.GetValue(RPLFormat.UnicodeBiDiTypes.Embed));
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_noVerticalMarginClassName);
			writer.WriteStream(HTMLElements.m_marginTop);
			writer.WriteStream(HTMLElements.m_zeroPoint);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_marginBottom);
			writer.WriteStream(HTMLElements.m_zeroPoint);
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentSizeInlineTable);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(HTMLElements.m_closeAccol);
			StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentHeightInlineTable);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(HTMLElements.m_closeAccol);
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
				writer.WriteStream(HTMLElements.m_boxSizingBorderBox);
			}
			writer.WriteStream(HTMLElements.m_boxSizingBorderBox);
			writer.WriteStream(HTMLElements.m_closeAccol);
		}

		private static void StartPredefinedStyleClass(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix, byte[] className)
		{
			if (classStylePrefix != null)
			{
				writer.WriteStream(classStylePrefix);
			}
			writer.WriteStream(HTMLElements.m_dot);
			writer.WriteStream(deviceInfo.HtmlPrefixId);
			writer.WriteStream(className);
			writer.WriteStream(HTMLElements.m_openAccol);
		}

		private void CheckBodyStyle()
		{
			RPLElementStyle style = m_pageContent.PageLayout.Style;
			string text = (string)style[34];
			m_pageHasStyle = (text != null || style[33] != null || ReportPageHasBorder(style, text));
		}

		private void RenderTextWrapping(RPLElement reportItem, bool renderDirectionStyles)
		{
			if (reportItem is RPLTextBox)
			{
				if (!renderDirectionStyles)
				{
					WriteStream(HTMLElements.m_wordWrap);
					WriteStream(HTMLElements.m_semiColon);
					WriteStream(HTMLElements.m_wordBreak);
					WriteStream(HTMLElements.m_semiColon);
				}
				WriteStream(HTMLElements.m_whiteSpacePreWrap);
				WriteStream(HTMLElements.m_semiColon);
			}
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
				WriteStream(HTMLElements.m_borderBottomColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(HTMLElements.m_borderBottomStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(HTMLElements.m_borderBottomWidth);
			}
		}

		private void BorderLeftAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(HTMLElements.m_borderLeftColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(HTMLElements.m_borderLeftStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(HTMLElements.m_borderLeftWidth);
			}
		}

		private void BorderRightAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(HTMLElements.m_borderRightColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(HTMLElements.m_borderRightStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(HTMLElements.m_borderRightWidth);
			}
		}

		private void BorderTopAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(HTMLElements.m_borderTopColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(HTMLElements.m_borderTopStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(HTMLElements.m_borderTopWidth);
			}
		}

		private void BorderAllAtribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				WriteStream(HTMLElements.m_borderColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				WriteStream(HTMLElements.m_borderStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				WriteStream(HTMLElements.m_borderWidth);
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
				WriteStream(HTMLElements.m_semiColon);
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
					WriteStream(HTMLElements.m_border);
					break;
				case Border.Bottom:
					WriteStream(HTMLElements.m_borderBottom);
					break;
				case Border.Left:
					WriteStream(HTMLElements.m_borderLeft);
					break;
				case Border.Right:
					WriteStream(HTMLElements.m_borderRight);
					break;
				default:
					WriteStream(HTMLElements.m_borderTop);
					break;
				}
				WriteStream(width);
				WriteStream(HTMLElements.m_space);
				WriteStream(value);
				WriteStream(HTMLElements.m_space);
				WriteStream(color);
				WriteStream(HTMLElements.m_semiColon);
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

		internal string CreateImageStream(RPLImageData image)
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

		private bool HasAction(RPLAction action)
		{
			if (action.BookmarkLink == null && action.DrillthroughId == null && action.DrillthroughUrl == null)
			{
				return action.Hyperlink != null;
			}
			return true;
		}

		internal bool RenderActionHref(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			bool hasHref = false;
			if (action.Hyperlink != null)
			{
				WriteStream(HTMLElements.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTMLElements.m_quoteString);
				hasHref = true;
			}
			else
			{
				RenderInteractionAction(action, ref hasHref);
			}
			if (textDec != RPLFormat.TextDecorations.Underline)
			{
				OpenStyle();
				WriteStream(HTMLElements.m_textDecoration);
				WriteStream(HTMLElements.m_none);
				WriteStream(HTMLElements.m_semiColon);
			}
			if (color != null)
			{
				OpenStyle();
				WriteStream(HTMLElements.m_color);
				WriteStream(color);
			}
			CloseStyle(renderQuote: true);
			if (m_deviceInfo.LinkTarget != null)
			{
				WriteStream(HTMLElements.m_target);
				WriteStream(m_deviceInfo.LinkTarget);
				WriteStream(HTMLElements.m_quote);
			}
			return hasHref;
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

		internal void PercentSizes()
		{
			WriteStream(HTMLElements.m_openStyle);
			WriteStream(HTMLElements.m_styleHeight);
			WriteStream(HTMLElements.m_percent);
			WriteStream(HTMLElements.m_semiColon);
			WriteStream(HTMLElements.m_styleWidth);
			WriteStream(HTMLElements.m_percent);
			WriteStream(HTMLElements.m_quote);
		}

		private void ClassLayoutBorder()
		{
			WriteClassName(HTMLElements.m_layoutBorder, HTMLElements.m_classLayoutBorder);
		}

		protected internal void ClassPercentHeight()
		{
			WriteClassName(HTMLElements.m_percentHeight, HTMLElements.m_classPercentHeight);
		}

		internal void RenderLanguage(string language)
		{
			if (!string.IsNullOrEmpty(language))
			{
				WriteStream(HTMLElements.m_language);
				WriteAttrEncoded(language);
				WriteStream(HTMLElements.m_quote);
			}
		}

		internal void RenderReportLanguage()
		{
			RenderLanguage(m_contextLanguage);
		}

		private List<string> RenderTableCellBorder(PageTableCell currCell, Hashtable renderedLines)
		{
			RPLLine rPLLine = null;
			List<string> list = new List<string>(4);
			if (m_isStyleOpen)
			{
				WriteStream(HTMLElements.m_semiColon);
			}
			else
			{
				OpenStyle();
			}
			WriteStream(HTMLElements.m_zeroBorderWidth);
			rPLLine = currCell.BorderLeft;
			if (rPLLine != null)
			{
				WriteStream(HTMLElements.m_semiColon);
				WriteStream(HTMLElements.m_borderLeft);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderRight;
			if (rPLLine != null)
			{
				WriteStream(HTMLElements.m_semiColon);
				WriteStream(HTMLElements.m_borderRight);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderTop;
			if (rPLLine != null)
			{
				WriteStream(HTMLElements.m_semiColon);
				WriteStream(HTMLElements.m_borderTop);
				RenderBorderLine(rPLLine);
				CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderBottom;
			if (rPLLine != null)
			{
				WriteStream(HTMLElements.m_semiColon);
				WriteStream(HTMLElements.m_borderBottom);
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

		private int GenerateTableLayoutContent(PageTableLayout rgTableGrid, RPLItemMeasurement[] repItemCol, bool bfZeroRowReq, bool bfZeroColReq, bool renderHeight, int borderContext, bool layoutExpand, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style, bool treatAsTopLevel)
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
				WriteStream(HTMLElements.m_openTR);
				if (!flag)
				{
					WriteStream(HTMLElements.m_valign);
					WriteStream(HTMLElements.m_topValue);
					WriteStream(HTMLElements.m_quote);
				}
				WriteStream(HTMLElements.m_closeBracket);
				flag3 = true;
				for (num = 0; num < nrCols; num++)
				{
					int num6 = num + num4;
					bool flag4 = num == 0;
					if (flag4 && bfZeroColReq)
					{
						WriteStream(HTMLElements.m_openTD);
						if (renderHeight || skipHeight <= 0)
						{
							WriteStream(HTMLElements.m_openStyle);
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
								WriteStream(HTMLElements.m_styleHeight);
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
								WriteStream(HTMLElements.m_mm);
								WriteStream(HTMLElements.m_semiColon);
							}
							WriteStream(HTMLElements.m_styleWidth);
							WriteDStream(0f);
							WriteStream(HTMLElements.m_mm);
							WriteStream(HTMLElements.m_quote);
						}
						else
						{
							WriteStream(HTMLElements.m_openStyle);
							WriteStream(HTMLElements.m_styleWidth);
							WriteDStream(0f);
							WriteStream(HTMLElements.m_mm);
							WriteStream(HTMLElements.m_quote);
						}
						WriteStream(HTMLElements.m_closeBracket);
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
						WriteStream(HTMLElements.m_closeTD);
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
					WriteStream(HTMLElements.m_openTD);
					num2 = pageTableCell2.RowSpan;
					if (num2 != 1)
					{
						WriteStream(HTMLElements.m_rowSpan);
						WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						WriteStream(HTMLElements.m_quote);
					}
					if (!flag2 || bfZeroRowReq || layoutState == SharedListLayoutState.Continue || layoutState == SharedListLayoutState.End)
					{
						num3 = pageTableCell2.ColSpan;
						if (num3 != 1)
						{
							WriteStream(HTMLElements.m_colSpan);
							WriteStream(num3.ToString(CultureInfo.InvariantCulture));
							WriteStream(HTMLElements.m_quote);
						}
					}
					if (flag4 && !bfZeroColReq && (renderHeight || skipHeight <= 0))
					{
						float num8 = pageTableCell.DYValue.Value;
						if (num8 >= 0f && flag3 && (!(i == nrRows - 1 && flag) || layoutState != 0) && (!m_deviceInfo.OutlookCompat || pageTableCell2.NeedsRowHeight))
						{
							OpenStyle();
							WriteStream(HTMLElements.m_styleHeight);
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
							WriteStream(HTMLElements.m_mm);
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
							WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							OpenStyle();
						}
						WriteStream(HTMLElements.m_styleWidth);
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
						WriteStream(HTMLElements.m_mm);
						WriteStream(HTMLElements.m_semiColon);
						WriteStream(HTMLElements.m_styleMinWidth);
						WriteDStream(num10);
						WriteStream(HTMLElements.m_mm);
						WriteStream(HTMLElements.m_semiColon);
						if (flag3 && !pageTableCell2.InUse && m_deviceInfo.OutlookCompat)
						{
							float num11 = pageTableCell2.DYValue.Value;
							if (num11 < 558.8f)
							{
								WriteStream(HTMLElements.m_styleHeight);
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
								WriteStream(HTMLElements.m_mm);
								WriteStream(HTMLElements.m_semiColon);
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
						WriteStream(HTMLElements.m_closeQuote);
					}
					else
					{
						WriteStream(HTMLElements.m_closeBracket);
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
						RenderCellItem(pageTableCell2, num5, layoutExpand, treatAsTopLevel);
					}
					else if (!m_browserIE && pageTableCell2.HasBorder && pageTableCell2.BorderTop == null && pageTableCell2.BorderBottom == null && pageTableCell2.BorderLeft == null && pageTableCell2.BorderRight == null)
					{
						RenderBlankImage();
					}
					WriteStream(HTMLElements.m_closeTD);
				}
				WriteStream(HTMLElements.m_closeTR);
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

		private void RenderWritingMode(RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, StyleContext styleContext, bool isRTL)
		{
			if (IsWritingModeVertical(writingMode))
			{
				if (isRTL)
				{
					WriteStream(HTMLElements.m_ms_verticalRTL);
				}
				else
				{
					WriteStream(HTMLElements.m_ms_vertical);
				}
				WriteStream(HTMLElements.m_ff_vertical);
				WriteStream(HTMLElements.m_webkit_vertical);
				if (writingMode == RPLFormat.WritingModes.Rotate270)
				{
					WriteStream(HTMLElements.m_rotate180deg);
				}
			}
		}

		protected internal void RenderDirectionStyles(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, bool isNonSharedStyles, StyleContext styleContext)
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
				WriteStream(HTMLElements.m_direction);
				WriteStream(obj);
				WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[30];
			RPLFormat.WritingModes? writingModes = null;
			if (obj != null)
			{
				writingModes = (RPLFormat.WritingModes)obj;
				WriteStream(HTMLElements.m_layoutFlow);
				if (!IsWritingModeVertical(writingModes.Value))
				{
					WriteStream(HTMLElements.m_horizontal);
				}
				WriteStream(HTMLElements.m_semiColon);
				if (IsWritingModeVertical(writingModes.Value) && measurement != null && reportItem is RPLTextBox)
				{
					RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
					float height = measurement.Height;
					float num = measurement.Width;
					if (!rPLTextBoxPropsDef.CanGrow && !rPLTextBoxPropsDef.CanShrink && styleContext.InTablix)
					{
						RenderMeasurementWidth(num);
					}
					if (rPLTextBoxPropsDef.CanGrow)
					{
						if (styleContext != null && styleContext.InTablix)
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
					}
					else
					{
						WriteStream(HTMLElements.m_overflowHidden);
						WriteStream(HTMLElements.m_semiColon);
					}
					if (styleContext != null && !styleContext.InTablix)
					{
						WriteStream(HTMLElements.m_styleHeight);
						WriteStream(HTMLElements.m_percent);
						WriteStream(HTMLElements.m_semiColon);
						WriteStream(HTMLElements.m_overflowHidden);
						WriteStream(HTMLElements.m_semiColon);
						if (!rPLTextBoxPropsDef.CanGrow && !rPLTextBoxPropsDef.CanShrink)
						{
							WriteStream(HTMLElements.m_wordWrap);
							WriteStream(HTMLElements.m_semiColon);
							WriteStream(HTMLElements.m_styleWidth);
							WriteStream(HTMLElements.m_percent);
							WriteStream(HTMLElements.m_semiColon);
						}
					}
					else
					{
						WriteStream(HTMLElements.m_wordWrap);
						WriteStream(HTMLElements.m_semiColon);
						WriteStream(HTMLElements.m_styleHeight);
						WriteStream(HTMLElements.m_percent);
						WriteStream(HTMLElements.m_semiColon);
					}
				}
			}
			if (writingModes.HasValue && directions.HasValue)
			{
				RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)props;
				RenderWritingMode(writingModes.Value, directions.Value, styleContext, IsDirectionRTL(rPLTextBoxProps.Style));
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
				RPLTextBoxProps rPLTextBoxProps2 = (RPLTextBoxProps)props;
				RenderWritingMode(writingModes.Value, directions.Value, styleContext, IsDirectionRTL(rPLTextBoxProps2.Style));
			}
		}

		internal void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RenderReportItemStyle(reportItem, elementProps, definition, measurement, new StyleContext(), ref borderContext, definition.ID);
		}

		public void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext, StyleContext styleContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RenderReportItemStyle(reportItem, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
		}

		internal void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLStyleProps nonSharedStyle, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID, bool renderDirectionStyles = false)
		{
			if (m_useInlineStyle)
			{
				OpenStyle();
				RPLElementStyle sharedStyleProps = new RPLElementStyle(nonSharedStyle, sharedStyle);
				RenderStyleProps(reportItem, elementProps, definition, measurement, sharedStyleProps, null, styleContext, ref borderContext, isNonSharedStyles: false, renderDirectionStyles);
				if (styleContext.EmptyTextBox)
				{
					WriteStream(HTMLElements.m_fontSize);
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
				RenderStyleProps(reportItem, elementProps, definition, measurement, sharedStyle, nonSharedStyle, styleContext, ref borderContext2, isNonSharedStyles: true, renderDirectionStyles);
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
						array = RenderSharedStyle(reportItem, elementProps, definition, sharedStyle, nonSharedStyle, measurement, styleID, styleContext, ref borderContext3, renderDirectionStyles);
					}
					else
					{
						array = m_encoding.GetBytes(styleID);
						m_usedStyles.Add(styleID, array);
					}
				}
				CloseStyle(renderQuote: true);
				WriteClassStyle(array, close: false);
				if (styleContext.InTablix && reportItem is RPLTextBox)
				{
					if (!((RPLTextBoxPropsDef)definition).CanGrow)
					{
						WriteStream(HTMLElements.m_classCannotGrowTextBoxInTablix);
					}
					else
					{
						WriteStream(HTMLElements.m_classCanGrowTextBoxInTablix);
					}
					if (((RPLTextBoxPropsDef)definition).CanShrink)
					{
						WriteStream(HTMLElements.m_classCanShrinkTextBoxInTablix);
					}
					else
					{
						WriteStream(HTMLElements.m_classCannotShrinkTextBoxInTablix);
					}
				}
				byte omitBordersState = styleContext.OmitBordersState;
				if (borderContext != 0 || omitBordersState != 0)
				{
					if (borderContext == 15)
					{
						WriteStream(HTMLElements.m_space);
						WriteStream(m_deviceInfo.HtmlPrefixId);
						WriteStream(HTMLElements.m_ignoreBorder);
					}
					else
					{
						if ((borderContext & 4) != 0 || (omitBordersState & 1) != 0)
						{
							WriteStream(HTMLElements.m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(HTMLElements.m_ignoreBorderT);
						}
						if ((borderContext & 1) != 0 || (omitBordersState & 4) != 0)
						{
							WriteStream(HTMLElements.m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(HTMLElements.m_ignoreBorderL);
						}
						if ((borderContext & 8) != 0 || (omitBordersState & 2) != 0)
						{
							WriteStream(HTMLElements.m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(HTMLElements.m_ignoreBorderB);
						}
						if ((borderContext & 2) != 0 || (omitBordersState & 8) != 0)
						{
							WriteStream(HTMLElements.m_space);
							WriteStream(m_deviceInfo.HtmlPrefixId);
							WriteStream(HTMLElements.m_ignoreBorderR);
						}
					}
				}
				if (styleContext.EmptyTextBox)
				{
					WriteStream(HTMLElements.m_space);
					WriteStream(m_deviceInfo.HtmlPrefixId);
					WriteStream(HTMLElements.m_emptyTextBox);
				}
				WriteStream(HTMLElements.m_quote);
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

		internal void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
		{
			RenderReportItemStyle(reportItem, elementProps, definition, elementProps.NonSharedStyle, definition.SharedStyle, measurement, styleContext, ref borderContext, styleID);
		}

		private void RenderPercentSizes()
		{
			WriteStream(HTMLElements.m_styleHeight);
			WriteStream(HTMLElements.m_percent);
			WriteStream(HTMLElements.m_semiColon);
			WriteStream(HTMLElements.m_styleWidth);
			WriteStream(HTMLElements.m_percent);
			WriteStream(HTMLElements.m_semiColon);
		}

		private void RenderTextAlign(RPLTextBoxProps props, RPLElementStyle style)
		{
			if (props != null)
			{
				WriteStream(HTMLElements.m_textAlign);
				bool flag = GetTextAlignForType(props);
				if (IsDirectionRTL(style))
				{
					flag = ((!flag) ? true : false);
				}
				if (flag)
				{
					WriteStream(HTMLElements.m_rightValue);
				}
				else
				{
					WriteStream(HTMLElements.m_leftValue);
				}
				WriteStream(HTMLElements.m_semiColon);
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

		protected internal static float GetInnerContainerWidth(RPLMeasurement measurement, IRPLStyle containerStyle)
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

		protected internal float GetInnerContainerWidthSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
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

		protected internal void WriteStyles(string id, RPLStyleProps nonShared, RPLStyleProps shared, ElementStyleWriter styleWriter)
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
					WriteStream(HTMLElements.m_closeAccol);
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

		public void RenderImageMapAreas(RPLActionInfoWithImageMap[] actionImageMaps, double width, double height, string uniqueName, int xOffset, int yOffset)
		{
			RPLActionInfoWithImageMap rPLActionInfoWithImageMap = null;
			double imageWidth = width * 96.0 * 0.03937007874;
			double imageHeight = height * 96.0 * 0.03937007874;
			WriteStream(HTMLElements.m_openMap);
			WriteAttrEncoded(HTMLElements.m_name, m_deviceInfo.HtmlPrefixId + HTMLElements.m_mapPrefixString + uniqueName);
			WriteStreamCR(HTMLElements.m_closeBracket);
			for (int i = 0; i < actionImageMaps.Length; i++)
			{
				rPLActionInfoWithImageMap = actionImageMaps[i];
				if (rPLActionInfoWithImageMap != null)
				{
					RenderImageMapArea(rPLActionInfoWithImageMap, imageWidth, imageHeight, uniqueName, xOffset, yOffset);
				}
			}
			WriteStream(HTMLElements.m_closeMap);
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
				WriteStream(HTMLElements.m_mapArea);
				if (rPLAction != null)
				{
					RenderTabIndex();
				}
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
					WriteStream(HTMLElements.m_nohref);
				}
				WriteStream(HTMLElements.m_mapShape);
				switch (rPLImageMap.Shape)
				{
				case RPLFormat.ShapeType.Circle:
					WriteStream(HTMLElements.m_circleShape);
					break;
				case RPLFormat.ShapeType.Polygon:
					WriteStream(HTMLElements.m_polyShape);
					break;
				default:
					WriteStream(HTMLElements.m_rectShape);
					break;
				}
				WriteStream(HTMLElements.m_quote);
				WriteStream(HTMLElements.m_mapCoords);
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
							WriteStream(HTMLElements.m_comma);
						}
						num = (long)((double)(coordinates[j] / 100f) * imageWidth) + xOffset;
						WriteStream(num);
						WriteStream(HTMLElements.m_comma);
						num = (long)((double)(coordinates[j + 1] / 100f) * imageHeight) + yOffset;
						WriteStream(num);
						flag = false;
					}
					if (j < coordinates.Length)
					{
						WriteStream(HTMLElements.m_comma);
						num = (long)((double)(coordinates[j] / 100f) * imageWidth);
						WriteStream(num);
					}
				}
				WriteStream(HTMLElements.m_quote);
				WriteStreamCR(HTMLElements.m_closeBracket);
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

		private void RenderBorderLine(RPLElement reportItem)
		{
			object obj = null;
			IRPLStyle style = reportItem.ElementProps.Style;
			obj = style[10];
			if (obj != null)
			{
				WriteStream(obj.ToString());
				WriteStream(HTMLElements.m_space);
			}
			obj = style[5];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				WriteStream(obj);
				WriteStream(HTMLElements.m_space);
			}
			obj = style[0];
			if (obj != null)
			{
				WriteStream((string)obj);
			}
		}

		protected void CreateImgConImageIdsStream()
		{
			string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_ici");
			Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			m_imgConImageIdsStream = new BufferedStream(stream);
		}

		internal void CreateImgFitDivImageIdsStream()
		{
			string streamName = GetStreamName(m_rplReport.ReportName, m_pageNum, "_ifd");
			Stream stream = CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			m_imgFitDivIdsStream = new BufferedStream(stream);
			m_emitImageConsolidationScaling = true;
		}

		[SecurityCritical]
		[SecuritySafeCritical]
		protected internal Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation)
		{
			return m_createAndRegisterStreamCallback(name, extension, encoding, mimeType, willSeek, operation);
		}

		protected void RenderSecondaryStreamIdsSpanTag(Stream secondaryStream, string tagId)
		{
			if (secondaryStream != null && secondaryStream.CanSeek)
			{
				WriteStream(HTMLElements.m_openSpan);
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
				WriteStream(HTMLElements.m_closeBracket);
				WriteStreamCR(HTMLElements.m_closeSpan);
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
