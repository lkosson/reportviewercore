using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ServerRenderer : HTML4Renderer, UrlWriter
	{
		private Dictionary<string, string> m_pageBookmarks;

		private Dictionary<string, string> m_globalBookmarks = new Dictionary<string, string>();

		private Microsoft.ReportingServices.OnDemandReportRendering.ReportParameterCollection m_parameters;

		protected bool m_isMHTML;

		public Dictionary<string, string> GlobalBookmarks
		{
			set
			{
				m_globalBookmarks = value;
			}
		}

		public Microsoft.ReportingServices.OnDemandReportRendering.ReportParameterCollection Parameters
		{
			set
			{
				m_parameters = value;
			}
		}

		private bool HasFixedHeader
		{
			get
			{
				if (m_fixedHeaders != null)
				{
					return m_fixedHeaders.Count > 0;
				}
				return false;
			}
		}

		protected virtual bool HasFindStringScript
		{
			get
			{
				if (m_htmlFragment)
				{
					return m_searchText != null;
				}
				return false;
			}
		}

		protected virtual bool HasInteractiveScript
		{
			get
			{
				if ((m_htmlFragment || (!m_pageWithSortClicks && !m_report.HasBookmarks)) && m_report.ShowHideToggle == null)
				{
					return m_report.SortItem != null;
				}
				return true;
			}
		}

		public ServerRenderer(ROMReport report, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams)
			: base(report, spbProcessing, reportServerParams, deviceInfo, rawDeviceInfo, browserCaps, createAndRegisterStreamCallback, secondaryStreams)
		{
		}

		public void WriteImage(Stream destinationStream, RPLImageData image)
		{
			Stream mainStream = m_mainStream;
			m_mainStream = destinationStream;
			RenderImageUrl(useSessionId: false, image);
			m_mainStream = mainStream;
		}

		public void UpdateRenderProperties(ref Hashtable renderProperties)
		{
			(m_spbProcessing as Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing)?.UpdateRenderProperties(ref renderProperties);
		}

		public void Dispose()
		{
			(m_spbProcessing as Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing)?.Dispose();
		}

		internal void RenderBodyTagStyle()
		{
			string theString = "BORDER: 0px; MARGIN: 0px; PADDING: 0px";
			WriteStream(HTML4Renderer.m_openStyle);
			WriteStream(theString);
			WriteStream(HTML4Renderer.m_quote);
		}

		public override void Render(HtmlTextWriter outputWriter)
		{
			m_encoding = outputWriter.Encoding;
			m_mainStream = Utility.CreateBufferedStream(outputWriter);
			if (!m_onlyVisibleStyles)
			{
				string styleStreamName = HTML4Renderer.GetStyleStreamName(m_rplReport.ReportName, m_pageNum);
				Stream stream = m_createAndRegisterStreamCallback(styleStreamName, "css", Encoding.UTF8, "text/css", willSeek: false, StreamOper.CreateAndRegister);
				ROMReport rOMReport = m_report as ROMReport;
				if (rOMReport != null)
				{
					HTMLStyleRenderer hTMLStyleRenderer = new HTMLStyleRenderer(rOMReport.Report, m_createAndRegisterStreamCallback, m_deviceInfo, this);
					hTMLStyleRenderer.Render(stream);
				}
				stream.Flush();
				m_styleStream = stream;
			}
			else if (!m_useInlineStyle)
			{
				string styleStreamName2 = HTML4Renderer.GetStyleStreamName(m_rplReport.ReportName, m_pageNum);
				StreamOper operation = StreamOper.CreateOnly;
				if (m_deviceInfo.StyleStream)
				{
					operation = StreamOper.CreateAndRegister;
				}
				Stream stream2 = m_createAndRegisterStreamCallback(styleStreamName2, "css", Encoding.UTF8, "text/css", willSeek: false, operation);
				m_styleStream = new BufferedStream(stream2);
			}
			if (!m_deviceInfo.HTMLFragment)
			{
				WriteStream(HTMLElements.Html40DocType);
				Stream mainStream = m_mainStream;
				Stream stream3 = m_mainStream = new BufferedStream(m_createAndRegisterStreamCallback("BODY", "html", null, null, willSeek: true, StreamOper.CreateOnly));
				RenderHtmlBody();
				RenderSecondaryStreamSpanTagsForJavascriptFunctions();
				WriteStream(HTML4Renderer.m_closeBody);
				m_mainStream = mainStream;
				stream3.Flush();
				WriteStream(HTML4Renderer.m_openHtml);
				WriteStreamLineBreak();
				WriteStream(HTML4Renderer.m_openHead);
				WriteStream(HTML4Renderer.m_openTitle);
				WriteStream(HttpUtility.HtmlEncode(m_rplReport.ReportName));
				WriteStream(HTML4Renderer.m_closeTitle);
				WriteStreamLineBreak();
				WriteStreamCR("<META http-equiv=\"Content-Type\" content=\"text/html; charset=" + outputWriter.Encoding.BodyName + "\"/>");
				WriteStreamCR("<META http-equiv=\"Content-Style-Type\" content=\"text/css\"/>");
				WriteStreamCR("<META http-equiv=\"Content-Script-Type\" content=\"text/javascript\"/>");
				if (!m_isMHTML)
				{
					WriteStreamCR(HTMLElements.Ie5ContentType);
				}
				RenderHead();
				RenderFinalScript();
				WriteStream(HTML4Renderer.m_closeHead);
				WriteStream(HTML4Renderer.m_openBody);
				if (m_deviceInfo.AllowScript)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (base.NeedResizeImages)
					{
						stringBuilder.Append(m_deviceInfo.JavascriptPrefixId);
						stringBuilder.Append("ResizeImages();");
					}
					if (m_deviceInfo.NavigationId != null)
					{
						stringBuilder.Append(m_deviceInfo.JavascriptPrefixId);
						stringBuilder.Append("GoToBookmark(");
						stringBuilder.Append(m_deviceInfo.NavigationId);
						stringBuilder.Append(");");
					}
					if (m_report.SortItem != null)
					{
						stringBuilder.Append(m_deviceInfo.JavascriptPrefixId);
						stringBuilder.Append("GoToSortItem();");
					}
					if (m_report.ShowHideToggle != null)
					{
						stringBuilder.Append(m_deviceInfo.JavascriptPrefixId);
						stringBuilder.Append("GoToShowHideToggle();");
					}
					if (m_emitImageConsolidationScaling)
					{
						stringBuilder.Append("Microsoft_ReportingServices_HTMLRenderer_ScaleImageConsolidation('");
						stringBuilder.Append(m_deviceInfo.HtmlPrefixId);
						stringBuilder.Append("');");
						stringBuilder.Append("Microsoft_ReportingServices_HTMLRenderer_ScaleImageForFit('");
						stringBuilder.Append(m_deviceInfo.HtmlPrefixId);
						stringBuilder.Append("');");
					}
					bool hasFixedHeader = HasFixedHeader;
					if (m_deviceInfo.IsBrowserGeckoEngine && hasFixedHeader)
					{
						stringBuilder.Append(m_deviceInfo.JavascriptPrefixId + "CreateFixedHeaders();");
					}
					if (m_needsCanGrowFalseScript)
					{
						stringBuilder.Append("Microsoft_ReportingServices_HTMLRenderer_GrowTablixTextBoxes();");
					}
					if (m_needsGrowRectangleScript)
					{
						stringBuilder.Append("Microsoft_ReportingServices_HTMLRenderer_GrowRectangles('");
						stringBuilder.Append(m_deviceInfo.HtmlPrefixId);
						stringBuilder.Append("'");
						stringBuilder.Append(",");
						stringBuilder.Append("'");
						stringBuilder.Append(m_deviceInfo.HtmlPrefixId);
						stringBuilder.Append("oReportDiv");
						stringBuilder.Append("');");
					}
					if (m_needsFitVertTextScript)
					{
						stringBuilder.Append("Microsoft_ReportingServices_HTMLRenderer_FitVertText('");
						stringBuilder.Append(m_deviceInfo.HtmlPrefixId);
						stringBuilder.Append("'");
						stringBuilder.Append(",");
						stringBuilder.Append("'");
						stringBuilder.Append(m_deviceInfo.HtmlPrefixId);
						stringBuilder.Append("oReportDiv");
						stringBuilder.Append("');");
					}
					if (stringBuilder.Length > 0)
					{
						WriteStream(string.Concat(" onload=\"", stringBuilder, "\""));
					}
					if (hasFixedHeader && !m_isMHTML && m_hasOnePage)
					{
						WriteStream(" onscroll=\"" + m_deviceInfo.JavascriptPrefixId + "CreateFixedHeaders()\"");
						WriteStream(" onresize=\"" + m_deviceInfo.JavascriptPrefixId + "CreateFixedHeaders()\" ");
					}
				}
				RenderBodyTagStyle();
				WriteStream(HTML4Renderer.m_closeBracket);
				m_mainStream.Flush();
				stream3.Seek(0L, SeekOrigin.Begin);
				Utility.CopyStream(stream3, m_mainStream);
				stream3.Close();
				stream3 = null;
				WriteStream(HTML4Renderer.m_closeHtml);
				m_mainStream.Flush();
			}
			else
			{
				Stream mainStream2 = m_mainStream;
				Stream stream4 = m_mainStream = new BufferedStream(m_createAndRegisterStreamCallback("BODY", "html", null, null, willSeek: true, StreamOper.CreateOnly));
				RenderHtmlBody();
				RenderFinalScript();
				stream4.Flush();
				m_mainStream = mainStream2;
				RenderHead();
				stream4.Seek(0L, SeekOrigin.Begin);
				Utility.CopyStream(stream4, m_mainStream);
				m_mainStream.Flush();
			}
		}

		protected override RPLReport GetNextPage()
		{
			Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing sPBProcessing = m_spbProcessing as Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing;
			bool flag = m_deviceInfo.Section > 0 && !m_deviceInfo.HasActionScript;
			sPBProcessing.GetNextPage(out RPLReport rplReport, flag);
			if (flag)
			{
				m_pageBookmarks = sPBProcessing.PageBookmarks;
			}
			return rplReport;
		}

		protected override void RenderInteractionAction(RPLAction action, ref bool hasHref)
		{
			if (m_deviceInfo.HasActionScript)
			{
				RenderControlActionScript(action);
				WriteStream(HTML4Renderer.m_href);
				WriteStream(HTML4Renderer.m_quote);
				OpenStyle();
				WriteStream(HTML4Renderer.m_cursorHand);
				WriteStream(HTML4Renderer.m_semiColon);
				hasHref = true;
			}
			else if (action.DrillthroughUrl != null)
			{
				string replacementRoot = m_deviceInfo.ReplacementRoot;
				CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(action.DrillthroughUrl);
				catalogItemUrlBuilder.AppendRenderingParameter("Parameters", "Collapsed");
				if (m_rawDeviceInfo["DocMap"] != null)
				{
					catalogItemUrlBuilder.AppendRenderingParameter("DocMap", m_rawDeviceInfo["DocMap"].ToString(CultureInfo.InvariantCulture));
				}
				if (replacementRoot == null)
				{
					if (!m_isMHTML)
					{
						catalogItemUrlBuilder.AppendRenderingParameter("Toolbar", "False");
					}
					if (m_deviceInfo.HasActionScript)
					{
						catalogItemUrlBuilder.AppendRenderingParameter("ActionScript", m_deviceInfo.ActionScript);
					}
					if (m_deviceInfo.HtmlPrefixId != null && m_deviceInfo.HtmlPrefixId.Length > 0)
					{
						catalogItemUrlBuilder.AppendRenderingParameter("PrefixId", m_deviceInfo.HtmlPrefixId);
					}
					WriteStream(HTML4Renderer.m_hrefString + catalogItemUrlBuilder.ToString() + HTML4Renderer.m_quoteString);
				}
				else
				{
					WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(ReplaceRoot(new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri)) + HTML4Renderer.m_quoteString);
				}
				hasHref = true;
			}
			else if (action.BookmarkLink != null)
			{
				if (HasInteractiveScript && m_deviceInfo.AllowScript && m_deviceInfo.Section > 0)
				{
					WriteStream(HTML4Renderer.m_href);
					WriteStream(HTML4Renderer.m_quote);
					RenderBookmarkOnClick(action.BookmarkLink);
				}
				else
				{
					RenderBookmarkHref(action);
					hasHref = true;
				}
			}
		}

		protected override void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState)
		{
			if (m_deviceInfo.AllowScript)
			{
				RenderSortOnClick(textBoxProps, sortState);
				WriteStream(HTML4Renderer.m_closeBracket);
				return;
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = BaseBookmarkSortUrl(isSort: true);
			catalogItemUrlBuilder.AppendCatalogParameter("SortId", textBoxProps.UniqueName);
			if (sortState == RPLFormat.SortOptions.Descending || sortState == RPLFormat.SortOptions.None)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SortDirection", "Ascending");
			}
			else
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SortDirection", "Descending");
			}
			catalogItemUrlBuilder.AppendCatalogParameter("ClearSort", "TRUE");
			WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(ReplaceRoot(new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri)) + HTML4Renderer.m_closeQuoteString);
		}

		private string ReplaceRoot(string uri)
		{
			string replacementRoot = m_deviceInfo.ReplacementRoot;
			if (replacementRoot == null)
			{
				return uri;
			}
			return replacementRoot + HttpUtility.HtmlEncode(uri);
		}

		protected override void RenderInternalImageSrc()
		{
			if (m_deviceInfo.ResourceStreamRoot == null)
			{
				WriteAttrEncoded(m_rplReport.Location);
				WriteStream(HTML4Renderer.m_encodedAmp);
				WriteStream("rs:");
				WriteStream("Command");
				WriteStream(HTML4Renderer.m_equal);
				WriteStream("Get");
				WriteStream(HTML4Renderer.m_encodedAmp);
				WriteStream("rc:");
				WriteStream("GetImage");
				WriteStream(HTML4Renderer.m_equal);
			}
			else
			{
				WriteAttrEncoded(m_deviceInfo.ResourceStreamRoot);
			}
		}

		protected override void RenderToggleImage(RPLTextBoxProps textBoxProps)
		{
			bool toggleState = textBoxProps.ToggleState;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)textBoxProps.Definition;
			if (!textBoxProps.IsToggleParent)
			{
				return;
			}
			WriteStream(HTML4Renderer.m_openA);
			WriteStream(HTML4Renderer.m_tabIndex);
			WriteStream(++m_tabIndexNum);
			WriteStream(HTML4Renderer.m_quote);
			if (m_deviceInfo.HasActionScript)
			{
				WriteStream(HTML4Renderer.m_openStyle);
				WriteStream(HTML4Renderer.m_cursorHand);
				WriteStream(HTML4Renderer.m_semiColon);
				WriteStream(HTML4Renderer.m_quote);
				RenderOnClickActionScript("Toggle", textBoxProps.UniqueName);
				WriteStream(HTML4Renderer.m_closeBracket);
			}
			else
			{
				string reportUrl = m_report.GetReportUrl(addParams: false);
				CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(reportUrl);
				catalogItemUrlBuilder.AppendRenderingParameters(m_rawDeviceInfo);
				if (m_allPages)
				{
					catalogItemUrlBuilder.AppendRenderingParameter("Section", "0");
				}
				else
				{
					catalogItemUrlBuilder.AppendRenderingParameter("Section", m_pageNum.ToString(CultureInfo.InvariantCulture));
				}
				if (m_deviceInfo.HtmlPrefixId != null && m_deviceInfo.HtmlPrefixId.Length > 0)
				{
					catalogItemUrlBuilder.AppendRenderingParameter("PrefixId", m_deviceInfo.HtmlPrefixId);
				}
				m_serverParams["ShowHideToggle"] = textBoxProps.UniqueName;
				catalogItemUrlBuilder.AppendCatalogParameters(m_serverParams);
				WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(ReplaceRoot(new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri)) + HTML4Renderer.m_closeQuoteString);
			}
			WriteStream(HTML4Renderer.m_img);
			if (m_browserIE)
			{
				WriteStream(HTML4Renderer.m_imgOnError);
			}
			WriteStream(HTML4Renderer.m_zeroBorder);
			WriteStream(HTML4Renderer.m_src);
			RenderInternalImageSrc();
			if (toggleState)
			{
				WriteStream(m_report.GetImageName("ToggleMinus.gif"));
			}
			else
			{
				WriteStream(m_report.GetImageName("TogglePlus.gif"));
			}
			WriteStream(HTML4Renderer.m_quote);
			WriteStream(HTML4Renderer.m_alt);
			if (toggleState)
			{
				WriteStream(RenderRes.ToggleStateCollapse);
			}
			else
			{
				WriteStream(RenderRes.ToggleStateExpand);
			}
			WriteStream(HTML4Renderer.m_closeTag);
			WriteStream(HTML4Renderer.m_closeA);
			if (!string.IsNullOrEmpty(textBoxProps.Value) || (rPLTextBoxPropsDef != null && !string.IsNullOrEmpty(rPLTextBoxPropsDef.Value)))
			{
				WriteStream(HTML4Renderer.m_nbsp);
			}
		}

		protected override void WriteScrollbars()
		{
			if (m_htmlFragment)
			{
				WriteStream("overflow:auto;");
			}
		}

		protected override void WriteFixedHeaderOnScrollScript()
		{
			WriteStream(" onscroll=\"" + m_deviceInfo.JavascriptPrefixId + "CreateFixedHeaders()\" ");
			WriteStream(" onresize=\"" + m_deviceInfo.JavascriptPrefixId + "CreateFixedHeaders()\" ");
		}

		protected override void WriteFixedHeaderPropertyChangeScript()
		{
			WriteStream(" onpropertychange=\"" + m_deviceInfo.JavascriptPrefixId + "CreateFixedHeaders()\" ");
		}

		protected override void WriteFitProportionalScript(double pv, double ph)
		{
			WriteStream(HTML4Renderer.m_onLoadFitProportionalPv);
			WriteStream(Utility.MmToPxAsString(pv));
			WriteStream(";this.ph=");
			WriteStream(Utility.MmToPxAsString(ph));
			WriteStream(HTML4Renderer.m_semiColon);
			if (m_htmlFragment)
			{
				WriteStream("if(");
				WriteStream(m_deviceInfo.JavascriptPrefixId);
				WriteStream("ResizeImage){");
				WriteStream(m_deviceInfo.JavascriptPrefixId);
				WriteStream("ResizeImage(this);}");
			}
			WriteStream(HTML4Renderer.m_quote);
		}

		private string BuildAbsoluteBookmarkOrSortBaseUrl(bool isSort)
		{
			CatalogItemUrlBuilder catalogItemUrlBuilder = BaseBookmarkSortUrl(isSort);
			if (isSort)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SortId", string.Empty);
			}
			return ReplaceRoot(new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri);
		}

		private void RenderSortOnClick(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState)
		{
			WriteStream(HTML4Renderer.m_openStyle);
			WriteStream(HTML4Renderer.m_cursorHand);
			WriteStream(HTML4Renderer.m_semiColon);
			WriteStream(HTML4Renderer.m_quote);
			string uniqueName = textBoxProps.UniqueName;
			if (m_deviceInfo.HasActionScript)
			{
				uniqueName = ((sortState != RPLFormat.SortOptions.Descending && sortState != 0) ? (uniqueName + "_D") : (uniqueName + "_A"));
				RenderOnClickActionScript("Sort", uniqueName);
				return;
			}
			m_pageWithSortClicks = true;
			string theString = "Ascending";
			if (sortState == RPLFormat.SortOptions.Ascending)
			{
				theString = "Descending";
			}
			WriteStream(" onclick=\"");
			WriteStream(m_deviceInfo.JavascriptPrefixId);
			WriteStream("Sort('");
			WriteStream(uniqueName);
			WriteStream("', '");
			WriteStream(theString);
			WriteStream("');return false;\"");
			WriteStream(" onkeypress=\"");
			WriteStream(HTML4Renderer.m_checkForEnterKey);
			WriteStream(m_deviceInfo.JavascriptPrefixId);
			WriteStream("Sort('");
			WriteStream(uniqueName);
			WriteStream("','");
			WriteStream(theString);
			WriteStream("');return false;}\"");
		}

		private CatalogItemUrlBuilder BaseBookmarkSortUrl(bool isSort)
		{
			string reportUrl = m_report.GetReportUrl(addParams: false);
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(reportUrl);
			if (isSort)
			{
				string value = m_serverParams["Command"];
				m_serverParams["Command"] = "Sort";
				catalogItemUrlBuilder.AppendCatalogParameters(m_serverParams);
				m_serverParams["Command"] = value;
			}
			else
			{
				catalogItemUrlBuilder.AppendCatalogParameters(m_serverParams);
			}
			catalogItemUrlBuilder.AppendRenderingParameters(m_rawDeviceInfo);
			return catalogItemUrlBuilder;
		}

		private void RenderHead()
		{
			string text = null;
			if (m_mainStream != null)
			{
				m_mainStream.Flush();
			}
			if (!m_htmlFragment)
			{
				if (m_rplReport.Location != null)
				{
					text = m_rplReport.Location;
					if (text.Length > 0)
					{
						WriteStream("<META HTTP-EQUIV=\"Location\" CONTENT=\"");
						WriteAttrEncoded(text);
						WriteStream(HTML4Renderer.m_closeTag);
						WriteStream("<META HTTP-EQUIV=\"Uri\" CONTENT=\"");
						WriteAttrEncoded(text);
						WriteStream(HTML4Renderer.m_closeTag);
					}
				}
				string description = m_rplReport.Description;
				if (description != null && 0 < description.Length)
				{
					text = description;
					WriteStream("<META NAME=\"Description\" CONTENT=\"");
					WriteAttrEncoded(text);
					WriteStream(HTML4Renderer.m_closeTag);
				}
				string author = m_rplReport.Author;
				if (author != null && 0 < author.Length)
				{
					text = author;
					WriteStream("<META NAME=\"Author\" CONTENT=\"");
					WriteAttrEncoded(text);
					WriteStream(HTML4Renderer.m_closeTag);
				}
				int autoRefresh = m_rplReport.AutoRefresh;
				if (0 < autoRefresh)
				{
					WriteStream("<META HTTP-EQUIV=\"Refresh\" CONTENT=\"");
					WriteStream(autoRefresh.ToString(CultureInfo.InvariantCulture));
					WriteStream(";url=");
					string reportUrl = m_report.GetReportUrl(addParams: false);
					CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(reportUrl);
					catalogItemUrlBuilder.AppendCatalogParameter("ResetSession", "True");
					if (m_serverParams["SessionID"] != null)
					{
						catalogItemUrlBuilder.AppendCatalogParameter("SessionID", m_serverParams["SessionID"]);
					}
					catalogItemUrlBuilder.AppendRenderingParameters(m_rawDeviceInfo);
					if (m_allPages)
					{
						catalogItemUrlBuilder.AppendRenderingParameter("Section", "0");
					}
					else
					{
						catalogItemUrlBuilder.AppendRenderingParameter("Section", m_pageNum.ToString(CultureInfo.InvariantCulture));
					}
					if (m_deviceInfo.HtmlPrefixId != null && m_deviceInfo.HtmlPrefixId.Length > 0)
					{
						catalogItemUrlBuilder.AppendRenderingParameter("PrefixId", m_deviceInfo.HtmlPrefixId);
					}
					WriteAttrEncoded(ReplaceRoot(new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri));
					WriteStream(HTML4Renderer.m_closeQuote);
				}
				WriteStream("<META HTTP-EQUIV=\"Last-Modified\" CONTENT=\"");
				WriteStream(m_rplReport.ExecutionTime.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
				WriteStream(HTML4Renderer.m_closeQuote);
				if (m_parameters != null)
				{
					object obj = null;
					foreach (ReportParameter parameter in m_parameters)
					{
						if (parameter.Name != null && parameter.Instance != null)
						{
							WriteStream("<META NAME=\"");
							WriteAttrEncoded(parameter.Name);
							WriteStream("\" CONTENT=\"");
							obj = parameter.Instance.Value;
							if (obj != null)
							{
								WriteAttrEncoded(obj.ToString());
							}
							WriteStream(HTML4Renderer.m_closeQuote);
						}
					}
				}
				WriteStream("<META NAME=\"Generator\" CONTENT=\"Microsoft Report 8.0 \"/>");
				WriteStream("<META NAME=\"Originator\" CONTENT=\"Microsoft Report 8.0 \"/>");
			}
			RenderHTMLHead();
		}

		private void RenderHTMLHead()
		{
			if (!m_useInlineStyle && m_styleStream != null)
			{
				Stream mainStream = m_mainStream;
				m_mainStream = m_styleStream;
				PredefinedStyles();
				m_mainStream = mainStream;
				m_styleStream.Flush();
				if (!m_deviceInfo.StyleStream)
				{
					WriteStreamCR("<style type=\"text/css\">");
					m_mainStream.Flush();
					m_styleStream.Seek(0L, SeekOrigin.Begin);
					Utility.CopyStream(m_styleStream, m_mainStream);
					m_styleStream.Close();
					m_styleStream = null;
					WriteStream("</style>");
					m_mainStream.Flush();
				}
				else if (!m_htmlFragment)
				{
					WriteStream("<link rel=\"stylesheet\" type=\"text/css\" href=\"");
					WriteStream(GetStyleStreamUrl());
					WriteStreamCR("\">");
				}
			}
			m_mainStream.Flush();
		}

		internal virtual string GetStyleStreamUrl()
		{
			string styleStreamName = HTML4Renderer.GetStyleStreamName(m_rplReport.ReportName, m_pageNum);
			return m_report.GetStreamUrl(useSessionId: true, styleStreamName);
		}

		private void RenderBookmarkOnClick(string bookmarkLink)
		{
			m_pageWithBookmarkLinks = true;
			WriteStream(" onClick=\"");
			WriteStream(m_deviceInfo.JavascriptPrefixId);
			WriteStream("GoVisibleBookmark('");
			StringBuilder stringBuilder = new StringBuilder();
			HTML4Renderer.QuoteString(stringBuilder, bookmarkLink);
			WriteAttrEncoded(stringBuilder.ToString());
			WriteStream("');return false;");
			WriteStream(HTML4Renderer.m_quote);
			WriteStream(" onkeypress=\"");
			WriteStream(HTML4Renderer.m_checkForEnterKey);
			WriteStream(m_deviceInfo.JavascriptPrefixId);
			WriteStream("GoVisibleBookmark('");
			WriteAttrEncoded(stringBuilder.ToString());
			WriteStream("');return false;}\"");
		}

		private void RenderBookmarkHref(RPLAction action)
		{
			string bookmarkLink = action.BookmarkLink;
			string text = null;
			string text2 = string.Empty;
			if (m_deviceInfo.Section == 0)
			{
				if (m_globalBookmarks != null && m_globalBookmarks.ContainsKey(bookmarkLink))
				{
					text = m_globalBookmarks[bookmarkLink];
				}
			}
			else if (m_pageBookmarks != null && m_pageBookmarks.ContainsKey(bookmarkLink))
			{
				text = m_pageBookmarks[bookmarkLink];
			}
			else if (m_globalBookmarks != null && m_globalBookmarks.ContainsKey(bookmarkLink))
			{
				text = m_globalBookmarks[bookmarkLink];
				CatalogItemUrlBuilder catalogItemUrlBuilder = BaseBookmarkSortUrl(isSort: false);
				catalogItemUrlBuilder.AppendRenderingParameter("BookmarkId", bookmarkLink);
				text2 = HttpUtility.HtmlAttributeEncode(ReplaceRoot(new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri));
			}
			if (text != null)
			{
				WriteStream(HTML4Renderer.m_hrefString + text2 + "#" + m_deviceInfo.HtmlPrefixId + text + HTML4Renderer.m_quoteString);
			}
		}

		private void RenderFinalScript()
		{
			if (!m_deviceInfo.AllowScript)
			{
				return;
			}
			WriteStream("<script language=\"javascript\" type=\"text/javascript\">");
			WriteStream(HTML4Renderer.m_standardLineBreak);
			WriteStream("//<![CDATA[");
			WriteStream(HTML4Renderer.m_standardLineBreak);
			if (HasFindStringScript)
			{
				WriteStream("window.location.replace('#oHit0');");
			}
			if (HasInteractiveScript)
			{
				if (m_report.ShowHideToggle != null && m_deviceInfo.IsBrowserIE)
				{
					WriteStream("var ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("showHideId=\"");
					StringBuilder stringBuilder = new StringBuilder();
					HTML4Renderer.QuoteString(stringBuilder, m_deviceInfo.HtmlPrefixId + m_report.ShowHideToggle);
					WriteStreamEncoded(stringBuilder.ToString());
					WriteStream(HTML4Renderer.m_quote);
					WriteStream(HTML4Renderer.m_standardLineBreak);
					WriteStream("function ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("GoToShowHideToggle(){if (document.getElementById(");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("showHideId) != null)window.location.replace(\"#\"+ ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("showHideId);}");
					WriteStream(HTML4Renderer.m_standardLineBreak);
				}
				if (m_pageWithSortClicks)
				{
					WriteStream("var ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortUrlBase=\"");
					StringBuilder stringBuilder2 = new StringBuilder();
					HTML4Renderer.QuoteString(stringBuilder2, BuildAbsoluteBookmarkOrSortBaseUrl(isSort: true));
					WriteStream(stringBuilder2.ToString());
					WriteStream(HTML4Renderer.m_quote);
					WriteStream(HTML4Renderer.m_standardLineBreak);
					WriteStream("var ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortDirection=\"&rs:");
					WriteStream("SortDirection");
					WriteStream("=");
					WriteStream(HTML4Renderer.m_quote);
					WriteStream(HTML4Renderer.m_standardLineBreak);
					WriteStream("var ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("clearSort=\"&rs:");
					WriteStream("ClearSort");
					WriteStream("=");
					WriteStream(HTML4Renderer.m_quote);
					WriteStream(HTML4Renderer.m_standardLineBreak);
					WriteStream("function ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("Sort(id, direction){var clear=true;");
					WriteStream("if (window.event && window.event.shiftKey) clear=false;");
					WriteStream("var sortUrl=");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortUrlBase + id + ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortDirection + direction + ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("clearSort;");
					WriteStream("if (clear) ");
					WriteStream("window.location.replace(sortUrl + \"True\");");
					WriteStream("else ");
					WriteStream("window.location.replace(sortUrl + \"False\");}");
				}
				if (m_report.SortItem != null)
				{
					WriteStream("var ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortId=\"");
					StringBuilder stringBuilder3 = new StringBuilder();
					HTML4Renderer.QuoteString(stringBuilder3, m_deviceInfo.HtmlPrefixId + m_report.SortItem);
					WriteStreamEncoded(stringBuilder3.ToString());
					WriteStream(HTML4Renderer.m_quote);
					WriteStream(HTML4Renderer.m_standardLineBreak);
					WriteStream("function ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("GoToSortItem(){if (document.getElementById(");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortId) != null)window.location.replace(\"#\"+ ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("sortId);}");
					WriteStream(HTML4Renderer.m_standardLineBreak);
				}
				if (m_report.HasBookmarks || m_deviceInfo.NavigationId != null)
				{
					WriteStream("function ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("GoToBookmark(id){window.location.replace(\"#\"+id);}");
					WriteStream(HTML4Renderer.m_standardLineBreak);
					if (m_pageWithBookmarkLinks)
					{
						if (m_pageBookmarks == null || m_pageBookmarks.Count < 1)
						{
							WriteStream("var ");
							WriteStream(m_deviceInfo.JavascriptPrefixId);
							WriteStream("bookmarkIds = [];");
						}
						else
						{
							WriteStream("var ");
							WriteStream(m_deviceInfo.JavascriptPrefixId);
							WriteStream("bookmarkIds = new Array(" + m_pageBookmarks.Count + ");");
							ICollection keys = m_pageBookmarks.Keys;
							foreach (string item in keys)
							{
								WriteStream(m_deviceInfo.JavascriptPrefixId + "bookmarkIds[\"" + item + "\"] = \"" + m_pageBookmarks[item] + "\";");
							}
						}
						WriteStream(HTML4Renderer.m_standardLineBreak);
						WriteStream("var ");
						WriteStream(m_deviceInfo.JavascriptPrefixId);
						WriteStream("bookmarkUrlBase=\"");
						StringBuilder stringBuilder4 = new StringBuilder();
						HTML4Renderer.QuoteString(stringBuilder4, BuildAbsoluteBookmarkOrSortBaseUrl(isSort: false));
						WriteStream(stringBuilder4.ToString());
						WriteStream(HTML4Renderer.m_quote);
						WriteStream(";");
						WriteStream(HTML4Renderer.m_standardLineBreak);
						WriteStream("function ");
						WriteStream(m_deviceInfo.JavascriptPrefixId);
						WriteStream("GoVisibleBookmark(idLink){if (");
						WriteStream(m_deviceInfo.JavascriptPrefixId);
						WriteStream("bookmarkIds == null) return;");
						WriteStream("var isHref=true;");
						WriteStream("if (bookmarkIds[idLink] != null) {");
						WriteStream(m_deviceInfo.JavascriptPrefixId);
						WriteStream("GoToBookmark(bookmarkIds[idLink]);}");
						WriteStream("else{window.location.replace(");
						WriteStream(m_deviceInfo.JavascriptPrefixId);
						WriteStream("bookmarkUrlBase + '&rc:BookmarkId=' + idLink);}}");
					}
				}
			}
			WriteStream(HTML4Renderer.m_standardLineBreak);
			WriteStream(HTMLRendererResources.GetBytes("Common.js"));
			WriteStream(HTML4Renderer.m_standardLineBreak);
			RenderFitProportionalScript();
			RenderFixedHeaderScripts();
			if (m_needsCanGrowFalseScript)
			{
				WriteStream(HTMLRendererResources.GetBytes("CanGrowFalse.js"));
			}
			if (m_emitImageConsolidationScaling)
			{
				WriteStream(HTMLRendererResources.GetBytes("ImageConsolidation.js"));
			}
			WriteStream("//]]>");
			WriteStream(HTML4Renderer.m_standardLineBreak);
			WriteStream("</script>");
		}

		private void RenderFitProportionalScript()
		{
			if (base.NeedResizeImages)
			{
				WriteStream(HTMLRendererResources.GetBytes("FitProportional.js"));
				WriteStream(HTML4Renderer.m_standardLineBreak);
				string text = m_deviceInfo.JavascriptPrefixId + "FitProp";
				WriteStream("var ");
				WriteStream(text);
				WriteStream(" = new Microsoft_ReportingServices_HTMLRenderer_FitProportional();");
				WriteStream(HTML4Renderer.m_standardLineBreak);
				if (m_htmlFragment)
				{
					WriteStream("function ");
					WriteStream(m_deviceInfo.JavascriptPrefixId);
					WriteStream("ResizeImage(o){ " + text + ".ResizeImage(o);}");
					WriteStream(HTML4Renderer.m_standardLineBreak);
				}
				WriteStream("function ");
				WriteStream(m_deviceInfo.JavascriptPrefixId);
				WriteStream("ResizeImages(){ " + text + ".PollResizeImages(\"");
				WriteAttrEncoded(m_deviceInfo.HtmlPrefixId);
				WriteStream("oReportDiv");
				WriteStream("\"); }");
				WriteStream(HTML4Renderer.m_standardLineBreak);
			}
		}

		internal void RenderFixedHeaderScripts()
		{
			if (m_fixedHeaders == null || m_fixedHeaders.Count == 0 || !m_hasOnePage)
			{
				WriteStream("function ");
				WriteStream(m_deviceInfo.JavascriptPrefixId);
				WriteStream("CreateFixedHeaders() {}");
				WriteStream(HTML4Renderer.m_standardLineBreak);
				return;
			}
			WriteStream(HTMLRendererResources.GetBytes("FixedHeader.js"));
			StringBuilder stringBuilder = new StringBuilder();
			string text = m_deviceInfo.JavascriptPrefixId + "FixedHeader";
			stringBuilder.Append(text);
			stringBuilder.Append(" = new Microsoft_ReportingServices_HTMLRenderer_FixedHeader(");
			stringBuilder.Append("\"" + m_deviceInfo.HtmlPrefixId + "oReportDiv\",");
			stringBuilder.Append("\"" + m_deviceInfo.HtmlPrefixId + "oReportCell\",");
			stringBuilder.Append("\"" + m_deviceInfo.HtmlPrefixId + "oReportDiv\",");
			stringBuilder.Append("\"" + m_deviceInfo.HtmlPrefixId + "\");");
			stringBuilder.Append("function ");
			stringBuilder.Append(m_deviceInfo.JavascriptPrefixId);
			stringBuilder.Append("CreateFixedHeaders(){");
			StringBuilder stringBuilder2 = new StringBuilder();
			RenderCreateFixedHeaderFunction(m_deviceInfo.JavascriptPrefixId, text, stringBuilder, stringBuilder2, createHeadersWithArray: false);
			WriteStream(stringBuilder2.ToString());
			WriteStream(stringBuilder.ToString());
			WriteStream("}");
			WriteStream(HTML4Renderer.m_standardLineBreak);
		}
	}
}
