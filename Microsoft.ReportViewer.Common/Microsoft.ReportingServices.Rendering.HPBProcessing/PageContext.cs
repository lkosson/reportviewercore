using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PageContext
	{
		internal enum CacheState : byte
		{
			RPLStream,
			RPLObjectModel,
			CountPages
		}

		internal enum IgnorePageBreakReason
		{
			Unknown,
			ToggleableItem,
			InsideTablixCell,
			InsideHeaderFooter
		}

		internal class PageContextCommon
		{
			private PaginationSettings m_pagination;

			private bool m_consumeWhitespace;

			private Bitmap m_hdcBits;

			private Graphics m_bitsGraphics;

			private Hashtable m_textBoxDuplicates;

			private Hashtable m_itemPaddingsStyle;

			private bool m_evaluatePageHeaderFooter;

			private int m_verticalPageNumber;

			private int m_pageNumber;

			private bool m_paginatingHorizontally;

			private string m_pageName;

			private string m_previousPageName;

			private bool m_canSetPageName = true;

			private int m_pageNumberRegion;

			private List<int> m_totalPagesRegion;

			private PageBreakProperties m_pageBreakProperties;

			private List<PageBreakProperties> m_registeredPageBreakProperties;

			private Hashtable m_tracedPageBreakIgnored;

			private Hashtable m_itemPropsStart;

			private Hashtable m_sharedImages;

			private Hashtable m_autosizeSharedImages;

			private Stream m_propertyCache;

			private CacheState m_cacheState;

			private BinaryReader m_propertyCacheReader;

			private BinaryWriter m_propertyCacheWriter;

			private AddTextBoxDelegate m_addTextBox;

			private Hashtable m_cacheSharedImages;

			internal CachedSharedImageInfo m_itemCacheSharedImageInfo;

			private IScalabilityCache m_scalabilityCache;

			private long m_totalScaleTimeMs;

			private long m_peakMemoryUsageKB;

			private CreateAndRegisterStream m_createAndRegisterStream;

			private FontCache m_fontCache;

			private bool m_inHeaderFooter;

			private bool m_inSubReport;

			private bool m_isInSelectiveRendering;

			private bool m_outputDiagnostics = true;

			internal PaginationSettings Pagination => m_pagination;

			internal bool ConsumeWhitespace => m_consumeWhitespace;

			internal bool EvaluatePageHeaderFooter
			{
				get
				{
					return m_evaluatePageHeaderFooter;
				}
				set
				{
					m_evaluatePageHeaderFooter = value;
				}
			}

			internal int VerticalPageNumber
			{
				get
				{
					return m_verticalPageNumber;
				}
				set
				{
					m_verticalPageNumber = value;
				}
			}

			internal int PageNumber
			{
				get
				{
					return m_pageNumber;
				}
				set
				{
					m_pageNumber = value;
				}
			}

			internal string PageName => m_pageName;

			internal int PageNumberRegion
			{
				get
				{
					return m_pageNumberRegion;
				}
				set
				{
					m_pageNumberRegion = value;
				}
			}

			public bool PaginatingHorizontally
			{
				get
				{
					return m_paginatingHorizontally;
				}
				set
				{
					m_paginatingHorizontally = value;
				}
			}

			public PageBreakProperties RegisteredPageBreakProperties => m_pageBreakProperties;

			public bool CanOverwritePageBreak => RegisteredPageBreakProperties == null;

			public bool CanSetPageName => m_canSetPageName;

			internal Hashtable ItemPropsStart
			{
				get
				{
					return m_itemPropsStart;
				}
				set
				{
					m_itemPropsStart = value;
				}
			}

			internal Hashtable SharedImages
			{
				get
				{
					return m_sharedImages;
				}
				set
				{
					m_sharedImages = value;
				}
			}

			internal Hashtable AutoSizeSharedImages
			{
				get
				{
					return m_autosizeSharedImages;
				}
				set
				{
					m_autosizeSharedImages = value;
				}
			}

			internal Hashtable TextBoxDuplicates
			{
				get
				{
					return m_textBoxDuplicates;
				}
				set
				{
					m_textBoxDuplicates = value;
				}
			}

			internal Hashtable ItemPaddingsStyle
			{
				get
				{
					return m_itemPaddingsStyle;
				}
				set
				{
					m_itemPaddingsStyle = value;
				}
			}

			internal Stream PropertyCache
			{
				get
				{
					return m_propertyCache;
				}
				set
				{
					m_propertyCache = value;
					if (m_propertyCache != null)
					{
						BufferedStream bufferedStream = new BufferedStream(m_propertyCache);
						m_propertyCacheReader = new BinaryReader(bufferedStream, Encoding.Unicode);
						m_propertyCacheWriter = new BinaryWriter(bufferedStream, Encoding.Unicode);
					}
				}
			}

			internal CacheState CacheState
			{
				get
				{
					return m_cacheState;
				}
				set
				{
					m_cacheState = value;
				}
			}

			internal Hashtable CacheSharedImages
			{
				get
				{
					return m_cacheSharedImages;
				}
				set
				{
					m_cacheSharedImages = value;
				}
			}

			internal CachedSharedImageInfo ItemCacheSharedImageInfo
			{
				get
				{
					return m_itemCacheSharedImageInfo;
				}
				set
				{
					m_itemCacheSharedImageInfo = value;
				}
			}

			internal BinaryReader PropertyCacheReader => m_propertyCacheReader;

			internal BinaryWriter PropertyCacheWriter => m_propertyCacheWriter;

			internal AddTextBoxDelegate AddTextBox => m_addTextBox;

			internal IScalabilityCache ScalabilityCache => m_scalabilityCache;

			internal long TotalScaleTimeMs => m_totalScaleTimeMs;

			internal long PeakMemoryUsageKB => m_peakMemoryUsageKB;

			internal FontCache FontCache
			{
				get
				{
					if (m_fontCache == null)
					{
						m_fontCache = new FontCache(Pagination.MeasureTextDpi, Pagination.UseEmSquare);
					}
					return m_fontCache;
				}
			}

			internal bool InHeaderFooter
			{
				get
				{
					return m_inHeaderFooter;
				}
				set
				{
					m_inHeaderFooter = value;
				}
			}

			internal bool InSubReport
			{
				get
				{
					return m_inSubReport;
				}
				set
				{
					m_inSubReport = value;
				}
			}

			internal bool IsInSelectiveRendering
			{
				get
				{
					return m_isInSelectiveRendering;
				}
				set
				{
					m_isInSelectiveRendering = value;
				}
			}

			internal bool DiagnosticsEnabled
			{
				get
				{
					if (RenderingDiagnostics.Enabled)
					{
						return m_outputDiagnostics;
					}
					return false;
				}
			}

			internal PageContextCommon(PaginationSettings pagination, AddTextBoxDelegate aAddTextBoxDelegate, bool consumeWhitespace, CreateAndRegisterStream createAndRegisterStream)
			{
				m_pagination = pagination;
				m_addTextBox = aAddTextBoxDelegate;
				m_consumeWhitespace = consumeWhitespace;
				m_createAndRegisterStream = createAndRegisterStream;
			}

			internal void CreateGraphics()
			{
				m_hdcBits = new Bitmap(2, 2);
				m_hdcBits.SetResolution(m_pagination.MeasureTextDpi, m_pagination.MeasureTextDpi);
				m_bitsGraphics = Graphics.FromImage(m_hdcBits);
				m_bitsGraphics.CompositingMode = CompositingMode.SourceOver;
				m_bitsGraphics.PageUnit = GraphicsUnit.Millimeter;
				m_bitsGraphics.PixelOffsetMode = PixelOffsetMode.Default;
				m_bitsGraphics.SmoothingMode = SmoothingMode.Default;
				m_bitsGraphics.TextRenderingHint = TextRenderingHint.SystemDefault;
			}

			internal void DisposeGraphics()
			{
				m_textBoxDuplicates = null;
				m_itemPaddingsStyle = null;
				m_itemPropsStart = null;
				m_sharedImages = null;
				m_autosizeSharedImages = null;
				if (m_propertyCache != null)
				{
					m_propertyCacheReader = null;
					m_propertyCacheWriter = null;
					m_propertyCache.Close();
					m_propertyCache.Dispose();
				}
				if (m_bitsGraphics != null)
				{
					m_bitsGraphics.Dispose();
					m_bitsGraphics = null;
				}
				if (m_hdcBits != null)
				{
					m_hdcBits.Dispose();
					m_hdcBits = null;
				}
				if (m_scalabilityCache != null)
				{
					m_totalScaleTimeMs += m_scalabilityCache.ScalabilityDurationMs;
					m_peakMemoryUsageKB = Math.Max(m_peakMemoryUsageKB, m_scalabilityCache.PeakMemoryUsageKBytes);
					m_scalabilityCache.Dispose();
					m_scalabilityCache = null;
				}
				if (m_fontCache != null)
				{
					m_fontCache.Dispose();
					m_fontCache = null;
				}
			}

			internal float MeasureFullTextBoxHeight(Microsoft.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext, out float contentHeight)
			{
				if (m_bitsGraphics == null)
				{
					CreateGraphics();
				}
				return Microsoft.ReportingServices.Rendering.RichText.TextBox.MeasureFullHeight(textBox, m_bitsGraphics, FontCache, flowContext, out contentHeight);
			}

			internal float MeasureTextBoxHeight(Microsoft.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext)
			{
				if (m_bitsGraphics == null)
				{
					CreateGraphics();
				}
				float height = 0f;
				LineBreaker.Flow(textBox, m_bitsGraphics, FontCache, flowContext, keepLines: false, out height);
				return height;
			}

			internal void InitCache()
			{
				if (m_scalabilityCache == null)
				{
					m_scalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(m_createAndRegisterStream, "HPB", StorageObjectCreator.Instance, HPBReferenceCreator.Instance, ComponentType.Pagination, 1);
				}
			}

			internal T GetFromCache<T>(string id, out Hashtable itemPropsStart) where T : class
			{
				T result = null;
				itemPropsStart = ItemPropsStart;
				if (itemPropsStart != null)
				{
					return itemPropsStart[id] as T;
				}
				itemPropsStart = new Hashtable();
				ItemPropsStart = itemPropsStart;
				return result;
			}

			internal T GetPrimitiveFromCache<T>(string id, out Hashtable itemPropsStart) where T : struct
			{
				T result = default(T);
				itemPropsStart = ItemPropsStart;
				if (itemPropsStart != null)
				{
					object obj = itemPropsStart[id];
					if (obj is T)
					{
						return (T)obj;
					}
					return default(T);
				}
				itemPropsStart = new Hashtable();
				ItemPropsStart = itemPropsStart;
				return result;
			}

			internal int GetTotalPagesRegion(int pageNumber)
			{
				if (m_cacheState != CacheState.CountPages && m_totalPagesRegion != null)
				{
					int num = 0;
					foreach (int item in m_totalPagesRegion)
					{
						num += item;
						if (pageNumber <= num)
						{
							return item;
						}
					}
				}
				return 0;
			}

			internal void UpdateTotalPagesRegionMapping()
			{
				if (m_cacheState == CacheState.CountPages)
				{
					if (m_totalPagesRegion == null)
					{
						m_totalPagesRegion = new List<int>();
					}
					m_totalPagesRegion.Add(m_pageNumberRegion);
				}
			}

			internal void RegisterPageBreakProperties(PageBreakProperties pageBreakProperties, bool overwrite)
			{
				if (DiagnosticsEnabled && pageBreakProperties != null && !m_registeredPageBreakProperties.Contains(pageBreakProperties))
				{
					m_registeredPageBreakProperties.Add(pageBreakProperties);
				}
				if (m_pageBreakProperties == null || overwrite)
				{
					m_pageBreakProperties = pageBreakProperties;
				}
			}

			internal void ProcessPageBreakProperties()
			{
				TracePageCreated();
				if (m_pageBreakProperties != null && !m_paginatingHorizontally)
				{
					if (m_pageBreakProperties.ResetPageNumber)
					{
						UpdateTotalPagesRegionMapping();
						m_pageNumberRegion = 0;
					}
					ResetPageBreakProcessing();
				}
			}

			internal void ResetPageBreakProcessing()
			{
				m_pageBreakProperties = null;
				if (DiagnosticsEnabled)
				{
					if (m_registeredPageBreakProperties == null)
					{
						m_registeredPageBreakProperties = new List<PageBreakProperties>();
					}
					else
					{
						m_registeredPageBreakProperties.Clear();
					}
				}
			}

			internal void SetPageName(string pageName, bool overwrite)
			{
				if (pageName != null && (m_canSetPageName || overwrite))
				{
					m_pageName = pageName;
					m_canSetPageName = false;
				}
			}

			internal void OverwritePageName(string pageName)
			{
				m_pageName = pageName;
			}

			internal void ResetPageNameProcessing()
			{
				m_canSetPageName = true;
			}

			internal void PauseDiagnostics()
			{
				m_outputDiagnostics = false;
			}

			internal void ResumeDiagnostics()
			{
				m_outputDiagnostics = true;
			}

			private void GetItemIdAndName(object item, out string id, out string name)
			{
				if (item is PageItem)
				{
					PageItem pageItem = (PageItem)item;
					id = pageItem.Source.ID;
					name = pageItem.Source.Name;
				}
				else if (item is TablixMember)
				{
					TablixMember tablixMember = (TablixMember)item;
					id = tablixMember.ID;
					name = tablixMember.Group.Name;
				}
				else
				{
					id = "";
					name = "ItemTypeNotFound";
				}
			}

			private string GetItemName(object item)
			{
				GetItemIdAndName(item, out string _, out string name);
				return name;
			}

			private void TracePageCreated(PageCreationType pageCreationType, bool resetPageNumber)
			{
				string text = "PR-DIAG [Page {0}] Page created by {1} page break";
				if (resetPageNumber)
				{
					text += ". Page number reset";
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, text, m_pageNumber + 1, pageCreationType.ToString());
			}

			private void TracePageCreated(PageCreationType pageCreationType)
			{
				TracePageCreated(pageCreationType, resetPageNumber: false);
			}

			public void TracePageCreated()
			{
				if (!DiagnosticsEnabled || m_pageNumber == 0)
				{
					return;
				}
				foreach (PageBreakProperties registeredPageBreakProperty in m_registeredPageBreakProperties)
				{
					if (registeredPageBreakProperty != m_pageBreakProperties)
					{
						string itemName = GetItemName(registeredPageBreakProperty.Source);
						RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – peer item precedence", PageNumber, itemName);
					}
				}
				if (m_pageBreakProperties != null)
				{
					if (m_paginatingHorizontally)
					{
						TracePageCreated(PageCreationType.Horizontal);
					}
					else
					{
						TracePageCreated(PageCreationType.Logical, m_pageBreakProperties.ResetPageNumber);
					}
				}
				else if (m_paginatingHorizontally)
				{
					TracePageCreated(PageCreationType.Horizontal);
				}
				else
				{
					TracePageCreated(PageCreationType.Vertical);
				}
			}

			public void ResetPageNameTracing()
			{
				m_previousPageName = null;
			}

			public void CheckPageNameChanged()
			{
				if (DiagnosticsEnabled && m_pageName != m_previousPageName)
				{
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page name changed", m_pageNumber);
					m_previousPageName = m_pageName;
				}
			}

			public void TracePageBreakIgnored(object item, IgnorePageBreakReason ignorePageBreakReason)
			{
				if (!DiagnosticsEnabled)
				{
					return;
				}
				GetItemIdAndName(item, out string id, out string name);
				if (m_tracedPageBreakIgnored == null || !m_tracedPageBreakIgnored.ContainsKey(id))
				{
					string text = "PR-DIAG [Page {0}] Page break on '{1}' ignored";
					switch (ignorePageBreakReason)
					{
					case IgnorePageBreakReason.InsideTablixCell:
						text += " - inside TablixCell";
						break;
					case IgnorePageBreakReason.ToggleableItem:
						text += " - part of toggleable region";
						break;
					case IgnorePageBreakReason.InsideHeaderFooter:
						text += " - inside header or footer";
						break;
					}
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, text, m_pageNumber, name);
					if (m_tracedPageBreakIgnored == null)
					{
						m_tracedPageBreakIgnored = new Hashtable();
					}
					m_tracedPageBreakIgnored.Add(id, null);
				}
			}

			public void TracePageBreakIgnoredDisabled(object item)
			{
				if (DiagnosticsEnabled)
				{
					string itemName = GetItemName(item);
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – Disable is True", m_pageNumber, itemName);
				}
			}

			public void TracePageBreakIgnoredAtTopOfPage(object item)
			{
				if (DiagnosticsEnabled)
				{
					string itemName = GetItemName(item);
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – at top of page", m_pageNumber, itemName);
				}
			}

			public void TracePageBreakIgnoredAtBottomOfPage(object item)
			{
				if (DiagnosticsEnabled)
				{
					string itemName = GetItemName(item);
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – bottom of page", m_pageNumber, itemName);
				}
			}
		}

		internal const double RoundDelta = 0.01;

		internal const string InvalidImage = "InvalidImage";

		private bool m_ignorePageBreak;

		private IgnorePageBreakReason m_ignorePageBreakReason;

		private bool m_fullOnPage;

		private bool m_cacheNonSharedProps;

		private bool m_resetHorizontal;

		private PageContextCommon m_common;

		private List<TextRunItemizedData> m_paragraphItemizedData;

		internal bool ConsumeWhitespace => m_common.ConsumeWhitespace;

		internal IScalabilityCache ScalabilityCache => m_common.ScalabilityCache;

		internal long TotalScaleTimeMs => m_common.TotalScaleTimeMs;

		internal long PeakMemoryUsageKB => m_common.PeakMemoryUsageKB;

		internal bool UseGenericDefault => m_common.Pagination.UseGenericDefault;

		internal double ColumnHeight => m_common.Pagination.CurrentColumnHeight;

		internal double ColumnWidth => m_common.Pagination.CurrentColumnWidth;

		internal double UsablePageHeight => m_common.Pagination.UsablePageHeight;

		internal bool IgnorePageBreaks
		{
			get
			{
				return m_ignorePageBreak;
			}
			set
			{
				m_ignorePageBreak = value;
			}
		}

		internal IgnorePageBreakReason IgnorePageBreaksReason
		{
			get
			{
				return m_ignorePageBreakReason;
			}
			set
			{
				m_ignorePageBreakReason = value;
			}
		}

		internal bool FullOnPage
		{
			get
			{
				return m_fullOnPage;
			}
			set
			{
				m_fullOnPage = value;
			}
		}

		internal bool ResetHorizontal
		{
			get
			{
				return m_resetHorizontal;
			}
			set
			{
				m_resetHorizontal = value;
			}
		}

		internal bool EvaluatePageHeaderFooter
		{
			get
			{
				return m_common.EvaluatePageHeaderFooter;
			}
			set
			{
				m_common.EvaluatePageHeaderFooter = value;
			}
		}

		public int DpiX => m_common.Pagination.DpiX;

		public int DpiY => m_common.Pagination.DpiY;

		public int DynamicImageDpiX => m_common.Pagination.DynamicImageDpiX;

		public int DynamicImageDpiY => m_common.Pagination.DynamicImageDpiY;

		internal bool EMFDynamicImages => m_common.Pagination.EMFOutputFormat;

		public PaginationSettings.FormatEncoding OutputFormat => m_common.Pagination.OutputFormat;

		internal int VerticalPageNumber
		{
			get
			{
				return m_common.VerticalPageNumber;
			}
			set
			{
				m_common.VerticalPageNumber = value;
			}
		}

		internal int PageNumber
		{
			get
			{
				return m_common.PageNumber;
			}
			set
			{
				m_common.PageNumber = value;
			}
		}

		internal string PageName => m_common.PageName;

		internal int PageNumberRegion
		{
			get
			{
				return m_common.PageNumberRegion;
			}
			set
			{
				m_common.PageNumberRegion = value;
			}
		}

		internal Hashtable ItemPropsStart
		{
			get
			{
				return m_common.ItemPropsStart;
			}
			set
			{
				m_common.ItemPropsStart = value;
			}
		}

		internal Hashtable SharedImages
		{
			get
			{
				return m_common.SharedImages;
			}
			set
			{
				m_common.SharedImages = value;
			}
		}

		internal Hashtable AutoSizeSharedImages
		{
			get
			{
				return m_common.AutoSizeSharedImages;
			}
			set
			{
				m_common.AutoSizeSharedImages = value;
			}
		}

		internal Hashtable TextBoxDuplicates
		{
			get
			{
				return m_common.TextBoxDuplicates;
			}
			set
			{
				m_common.TextBoxDuplicates = value;
			}
		}

		internal Hashtable ItemPaddingsStyle
		{
			get
			{
				return m_common.ItemPaddingsStyle;
			}
			set
			{
				m_common.ItemPaddingsStyle = value;
			}
		}

		internal bool CacheNonSharedProps => m_cacheNonSharedProps;

		internal Stream PropertyCache
		{
			get
			{
				return m_common.PropertyCache;
			}
			set
			{
				m_common.PropertyCache = value;
			}
		}

		internal CacheState PropertyCacheState
		{
			get
			{
				return m_common.CacheState;
			}
			set
			{
				m_common.CacheState = value;
			}
		}

		internal BinaryReader PropertyCacheReader => m_common.PropertyCacheReader;

		internal BinaryWriter PropertyCacheWriter => m_common.PropertyCacheWriter;

		internal Hashtable CacheSharedImages
		{
			get
			{
				return m_common.CacheSharedImages;
			}
			set
			{
				m_common.CacheSharedImages = value;
			}
		}

		internal CachedSharedImageInfo ItemCacheSharedImageInfo
		{
			get
			{
				return m_common.ItemCacheSharedImageInfo;
			}
			set
			{
				m_common.ItemCacheSharedImageInfo = value;
			}
		}

		internal AddTextBoxDelegate AddTextBox => m_common.AddTextBox;

		internal PageContextCommon Common => m_common;

		internal List<TextRunItemizedData> ParagraphItemizedData
		{
			get
			{
				return m_paragraphItemizedData;
			}
			set
			{
				m_paragraphItemizedData = value;
			}
		}

		internal bool IsInSelectiveRendering => Common.IsInSelectiveRendering;

		internal PageContext(PaginationSettings pagination, AddTextBoxDelegate aAddTextBox, bool consumeWhitespace, CreateAndRegisterStream createAndRegisterStream)
		{
			m_common = new PageContextCommon(pagination, aAddTextBox, consumeWhitespace, createAndRegisterStream);
		}

		internal PageContext(PageContext pageContext)
		{
			m_common = pageContext.Common;
		}

		internal PageContext(PageContext pageContext, bool fullOnPage, bool ignorePageBreaks, IgnorePageBreakReason ignorePageBreakReason, bool cacheNonSharedProps)
		{
			m_common = pageContext.Common;
			m_fullOnPage = fullOnPage;
			m_ignorePageBreak = ignorePageBreaks;
			m_ignorePageBreakReason = ignorePageBreakReason;
			m_cacheNonSharedProps = cacheNonSharedProps;
		}

		internal PageContext(PageContext pageContext, bool cacheNonSharedProps)
		{
			m_common = pageContext.Common;
			m_fullOnPage = pageContext.FullOnPage;
			m_ignorePageBreak = pageContext.IgnorePageBreaks;
			m_ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
			m_cacheNonSharedProps = cacheNonSharedProps;
		}

		internal void InitCache()
		{
			m_common.InitCache();
		}

		internal void DisposeGraphics()
		{
			m_common.DisposeGraphics();
		}

		internal double ConvertToMillimeters(int coordinate, float dpi)
		{
			if (0f == dpi)
			{
				return double.MaxValue;
			}
			return 1.0 / (double)dpi * (double)coordinate * 25.399999618530273;
		}

		internal void RegisterTextRunData(TextRunItemizedData runItemizedData)
		{
			if (m_paragraphItemizedData != null)
			{
				m_paragraphItemizedData.Add(runItemizedData);
			}
		}
	}
}
