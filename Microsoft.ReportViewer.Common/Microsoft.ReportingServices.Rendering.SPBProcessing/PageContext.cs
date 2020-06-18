using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class PageContext
	{
		internal enum RPLReportSectionArea : byte
		{
			Header,
			Footer,
			Body
		}

		[Flags]
		internal enum PageContextFlags : ushort
		{
			Clear = 0x0,
			IgnorePageBreak = 0x1,
			FullOnPage = 0x2,
			KeepTogether = 0x4,
			HideDuplicates = 0x8,
			TypeCodeNonString = 0x10,
			StretchPage = 0x20
		}

		internal enum IgnorePBReasonFlag
		{
			None,
			Toggled,
			Repeated,
			TablixParent,
			HeaderFooter
		}

		internal class PageContextCommon
		{
			private double m_pageHeight = double.MaxValue;

			private double m_originalPageHeight = double.MaxValue;

			private bool m_registerEvents;

			private bool m_measureItems;

			private bool m_emfDynamicImage;

			private bool m_consumeWhitespace;

			private bool m_cancelPage;

			private bool m_initCancelPage;

			private SecondaryStreams m_secondaryStreams;

			private CreateAndRegisterStream m_createAndRegisterStream;

			private bool m_addToggledItems;

			private bool m_addSecondaryStreamNames;

			private bool m_addOriginalValue;

			private float m_dpiX = 96f;

			private float m_dpiY = 96f;

			private Bitmap m_hdcBits;

			private Graphics m_bitsGraphics;

			private FontCache m_fontCache;

			private bool m_evaluatePageHeaderFooter;

			private bool m_addFirstPageHeaderFooter;

			private int m_pageNumber;

			private string m_pageName;

			private PageBreakInfo m_pageBreakInfo;

			private PageTotalInfo m_pageTotalInfo;

			private RPLReportSectionArea m_rplSectionArea = RPLReportSectionArea.Body;

			private RPLVersionEnum m_versionPicker;

			private Hashtable m_textboxSharedInfo;

			private Hashtable m_itemPropsStart;

			private Hashtable m_sharedImages;

			private Hashtable m_registeredStreamNames;

			private Hashtable m_autosizeSharedImages;

			private Hashtable m_sharedItemSizes;

			private Hashtable m_sharedRenderItemSizes;

			private Hashtable m_sharedEdgeItemSizes;

			private Hashtable m_sharedRenderEdgeItemSizes;

			private Hashtable m_sharedRenderRepeatItemSizes;

			private DocumentMapLabels m_labels;

			private Bookmarks m_bookmarks;

			private Dictionary<string, string> m_pageBookmarks;

			private IScalabilityCache m_scalabilityCache;

			private long m_totalScaleTimeMs;

			private long m_peakMemoryUsageKB;

			private MemoryPressureListener m_memoryPressure;

			private double m_pageHeightForMemory = 420.0;

			private bool m_useEmSquare;

			private bool m_canTracePagination;

			private ImageConsolidation m_imageConsolidation;

			private bool m_convertImages;

			private Stream m_delayTextBoxCache;

			private BinaryReader m_propertyCacheReader;

			private BinaryWriter m_propertyCacheWriter;

			private Hashtable m_registeredPBIgnored;

			internal double PageHeight
			{
				get
				{
					return m_pageHeight;
				}
				set
				{
					m_pageHeight = value;
				}
			}

			internal double OriginalPageHeight
			{
				get
				{
					return m_originalPageHeight;
				}
				set
				{
					m_originalPageHeight = value;
				}
			}

			internal bool RegisterEvents => m_registerEvents;

			internal bool MeasureItems => m_measureItems;

			internal bool EmfDynamicImage => m_emfDynamicImage;

			internal bool ConsumeContainerWhitespace => m_consumeWhitespace;

			internal CreateAndRegisterStream CreateAndRegisterStream => m_createAndRegisterStream;

			internal SecondaryStreams SecondaryStreams => m_secondaryStreams;

			internal bool AddToggledItems => m_addToggledItems;

			internal bool AddSecondaryStreamNames => m_addSecondaryStreamNames;

			internal bool AddOriginalValue => m_addOriginalValue;

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

			internal bool AddFirstPageHeaderFooter
			{
				get
				{
					return m_addFirstPageHeaderFooter;
				}
				set
				{
					m_addFirstPageHeaderFooter = value;
				}
			}

			internal float DpiX
			{
				get
				{
					return m_dpiX;
				}
				set
				{
					m_dpiX = value;
				}
			}

			internal float DpiY
			{
				get
				{
					return m_dpiY;
				}
				set
				{
					m_dpiY = value;
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

			internal string PageName
			{
				get
				{
					return m_pageName;
				}
				set
				{
					m_pageName = value;
				}
			}

			internal RPLReportSectionArea RPLSectionArea
			{
				get
				{
					return m_rplSectionArea;
				}
				set
				{
					m_rplSectionArea = value;
				}
			}

			internal Hashtable TextBoxSharedInfo
			{
				get
				{
					return m_textboxSharedInfo;
				}
				set
				{
					m_textboxSharedInfo = value;
				}
			}

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

			internal Hashtable RegisteredStreamNames
			{
				get
				{
					return m_registeredStreamNames;
				}
				set
				{
					m_registeredStreamNames = value;
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

			internal DocumentMapLabels Labels
			{
				get
				{
					return m_labels;
				}
				set
				{
					m_labels = value;
				}
			}

			internal Bookmarks Bookmarks
			{
				get
				{
					return m_bookmarks;
				}
				set
				{
					m_bookmarks = value;
				}
			}

			internal Dictionary<string, string> PageBookmarks
			{
				get
				{
					return m_pageBookmarks;
				}
				set
				{
					m_pageBookmarks = value;
				}
			}

			internal PageBreakInfo PageBreakInfo
			{
				get
				{
					return m_pageBreakInfo;
				}
				set
				{
					m_pageBreakInfo = value;
				}
			}

			internal PageTotalInfo PageTotalInfo => m_pageTotalInfo;

			internal IScalabilityCache ScalabilityCache => m_scalabilityCache;

			internal long TotalScaleTimeMs => m_totalScaleTimeMs;

			internal long PeakMemoryUsageKB => m_peakMemoryUsageKB;

			internal bool CancelMode => m_initCancelPage;

			internal bool CancelPage
			{
				get
				{
					if (m_memoryPressure == null)
					{
						return false;
					}
					if (!m_cancelPage)
					{
						m_cancelPage = m_memoryPressure.CheckAndResetNotification();
					}
					return m_cancelPage;
				}
				set
				{
					m_cancelPage = value;
				}
			}

			internal FontCache FontCache
			{
				get
				{
					if (m_fontCache == null)
					{
						m_fontCache = new FontCache(96f, m_useEmSquare);
					}
					return m_fontCache;
				}
			}

			internal bool EmSquare
			{
				get
				{
					return m_useEmSquare;
				}
				set
				{
					m_useEmSquare = value;
				}
			}

			internal bool CanTracePagination
			{
				get
				{
					return m_canTracePagination;
				}
				set
				{
					m_canTracePagination = value;
				}
			}

			internal ImageConsolidation ImageConsolidation
			{
				get
				{
					return m_imageConsolidation;
				}
				set
				{
					m_imageConsolidation = value;
				}
			}

			internal RPLVersionEnum VersionPicker
			{
				get
				{
					return m_versionPicker;
				}
				set
				{
					m_versionPicker = value;
				}
			}

			internal BinaryReader PropertyCacheReader => m_propertyCacheReader;

			internal BinaryWriter PropertyCacheWriter => m_propertyCacheWriter;

			internal bool ConvertImages => m_convertImages;

			internal Hashtable RegisteredPBIgnored => m_registeredPBIgnored;

			internal PageContextCommon(string pageName, double pageHeight, bool registerEvents, bool consumeWhitespace, CreateAndRegisterStream createAndRegisterStream)
			{
				m_pageHeight = pageHeight;
				m_originalPageHeight = pageHeight;
				m_registerEvents = registerEvents;
				m_consumeWhitespace = consumeWhitespace;
				m_sharedItemSizes = new Hashtable();
				m_sharedEdgeItemSizes = new Hashtable();
				m_sharedRenderItemSizes = new Hashtable();
				m_sharedRenderEdgeItemSizes = new Hashtable();
				m_sharedRenderRepeatItemSizes = new Hashtable();
				m_registeredStreamNames = new Hashtable();
				m_registeredPBIgnored = new Hashtable();
				m_createAndRegisterStream = createAndRegisterStream;
				m_pageTotalInfo = new PageTotalInfo(pageName);
			}

			internal void SetContext(bool measureItems, bool emfDynamicImage, SecondaryStreams secondaryStreams, bool addSecondaryStreamNames, bool addToggledItems, bool addOriginalValue, bool addFirstPageHeaderFooter, bool convertImages)
			{
				m_measureItems = measureItems;
				m_emfDynamicImage = emfDynamicImage;
				m_secondaryStreams = secondaryStreams;
				m_addSecondaryStreamNames = addSecondaryStreamNames;
				m_addToggledItems = addToggledItems;
				m_addOriginalValue = addOriginalValue;
				m_addFirstPageHeaderFooter = addFirstPageHeaderFooter;
				m_evaluatePageHeaderFooter = false;
				m_itemPropsStart = null;
				m_sharedImages = null;
				m_convertImages = convertImages;
				if (!measureItems)
				{
					DisposeResources();
				}
			}

			internal void DisposeResources()
			{
				DisposeGraphics();
				DisposeTextboxSharedInfo();
				DisposeDelayTextBox();
				DisposeScalabilityCache();
				DisposeMemoryPressureListener();
			}

			internal SizeF MeasureStringGDI(string text, CanvasFont font, SizeF layoutArea, out int charactersFitted, out int linesFilled)
			{
				if (m_bitsGraphics == null)
				{
					CreateGraphics();
				}
				StringFormat trimStringFormat = font.TrimStringFormat;
				return m_bitsGraphics.MeasureString(text, font.GDIFont, layoutArea, trimStringFormat, out charactersFitted, out linesFilled);
			}

			internal float MeasureFullTextBoxHeight(Microsoft.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext, out float contentHeight)
			{
				if (m_bitsGraphics == null)
				{
					CreateGraphics();
				}
				return Microsoft.ReportingServices.Rendering.RichText.TextBox.MeasureFullHeight(textBox, m_bitsGraphics, FontCache, flowContext, out contentHeight);
			}

			internal ItemSizes GetSharedItemSizesElement(ReportItem reportItem, bool isPadded)
			{
				if (reportItem == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				if (m_sharedItemSizes.ContainsKey(reportItem.ID))
				{
					itemSizes = (ItemSizes)m_sharedItemSizes[reportItem.ID];
					itemSizes.Update(reportItem);
				}
				else
				{
					itemSizes = ((!isPadded) ? new ItemSizes(reportItem) : new PaddItemSizes(reportItem));
					m_sharedItemSizes.Add(reportItem.ID, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedItemSizesElement(ReportSize width, ReportSize height, string id, bool isPadded)
			{
				if (id == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				if (m_sharedItemSizes.ContainsKey(id))
				{
					itemSizes = (ItemSizes)m_sharedItemSizes[id];
					itemSizes.Update(width, height);
				}
				else
				{
					itemSizes = ((!isPadded) ? new ItemSizes(width, height, id) : new PaddItemSizes(width, height, id));
					m_sharedItemSizes.Add(id, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedRenderItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
			{
				if (itemSizes == null)
				{
					return null;
				}
				if (isPadded)
				{
					RSTrace.RenderingTracer.Assert(itemSizes is PaddItemSizes, "The ItemSizes object is not a PaddItemSizes object.");
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID == null)
				{
					itemSizes2 = ((!(isPadded && returnPaddings)) ? new ItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes));
				}
				else if (m_sharedRenderItemSizes.ContainsKey(itemSizes.ID))
				{
					itemSizes2 = (ItemSizes)m_sharedRenderItemSizes[itemSizes.ID];
					itemSizes2.Update(itemSizes, returnPaddings);
				}
				else
				{
					itemSizes2 = ((!isPadded) ? new ItemSizes(itemSizes) : ((!returnPaddings) ? new PaddItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes)));
					m_sharedRenderItemSizes.Add(itemSizes.ID, itemSizes2);
				}
				return itemSizes2;
			}

			internal ItemSizes GetSharedEdgeItemSizesElement(double top, double left, string id)
			{
				if (id == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				if (m_sharedEdgeItemSizes.ContainsKey(id))
				{
					itemSizes = (ItemSizes)m_sharedEdgeItemSizes[id];
					itemSizes.Update(top, left);
				}
				else
				{
					itemSizes = new ItemSizes(top, left, id);
					m_sharedEdgeItemSizes.Add(id, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedRenderEdgeItemSizesElement(ItemSizes itemSizes)
			{
				if (itemSizes == null)
				{
					return null;
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID != null)
				{
					if (m_sharedRenderEdgeItemSizes.ContainsKey(itemSizes.ID))
					{
						itemSizes2 = (ItemSizes)m_sharedRenderEdgeItemSizes[itemSizes.ID];
						itemSizes2.Update(itemSizes, returnPaddings: false);
					}
					else
					{
						itemSizes2 = new ItemSizes(itemSizes);
						m_sharedRenderEdgeItemSizes.Add(itemSizes.ID, itemSizes2);
					}
				}
				else
				{
					itemSizes2 = new ItemSizes(itemSizes);
				}
				return itemSizes2;
			}

			internal ItemSizes GetSharedFromRepeatItemSizesElement(ReportItem reportItem, bool isPadded)
			{
				if (reportItem == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				string key = reportItem.ID + "_REPEAT";
				if (m_sharedItemSizes.ContainsKey(key))
				{
					itemSizes = (ItemSizes)m_sharedItemSizes[key];
					itemSizes.Update(reportItem);
				}
				else
				{
					itemSizes = ((!isPadded) ? new ItemSizes(reportItem) : new PaddItemSizes(reportItem));
					m_sharedItemSizes.Add(key, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedRenderFromRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
			{
				if (itemSizes == null)
				{
					return null;
				}
				if (isPadded)
				{
					RSTrace.RenderingTracer.Assert(itemSizes is PaddItemSizes, "The ItemSizes object is not a PaddItemSizes object.");
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID == null)
				{
					itemSizes2 = ((!(isPadded && returnPaddings)) ? new ItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes));
				}
				else
				{
					string key = itemSizes.ID + "_REPEAT";
					if (m_sharedRenderItemSizes.ContainsKey(key))
					{
						itemSizes2 = (ItemSizes)m_sharedRenderItemSizes[key];
						itemSizes2.Update(itemSizes, returnPaddings);
					}
					else
					{
						itemSizes2 = ((!isPadded) ? new ItemSizes(itemSizes) : ((!returnPaddings) ? new PaddItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes)));
						m_sharedRenderItemSizes.Add(key, itemSizes2);
					}
				}
				return itemSizes2;
			}

			internal ItemSizes GetSharedRenderRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
			{
				if (itemSizes == null)
				{
					return null;
				}
				if (isPadded)
				{
					RSTrace.RenderingTracer.Assert(itemSizes is PaddItemSizes, "The ItemSizes object is not a PaddItemSizes object.");
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID == null)
				{
					itemSizes2 = ((!(isPadded && returnPaddings)) ? new ItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes));
				}
				else if (m_sharedRenderRepeatItemSizes.ContainsKey(itemSizes.ID))
				{
					itemSizes2 = (ItemSizes)m_sharedRenderRepeatItemSizes[itemSizes.ID];
					itemSizes2.Update(itemSizes, returnPaddings);
				}
				else
				{
					itemSizes2 = ((!isPadded) ? new ItemSizes(itemSizes) : ((!returnPaddings) ? new PaddItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes)));
					m_sharedRenderRepeatItemSizes.Add(itemSizes.ID, itemSizes2);
				}
				return itemSizes2;
			}

			internal void RegisterPageBookmark(ReportItemInstance reportItemInstance)
			{
				if (reportItemInstance != null && reportItemInstance.Bookmark != null && !m_pageBookmarks.ContainsKey(reportItemInstance.Bookmark))
				{
					m_pageBookmarks.Add(reportItemInstance.Bookmark, reportItemInstance.UniqueName);
				}
			}

			internal void InitCache()
			{
				if (m_scalabilityCache == null)
				{
					m_scalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(m_createAndRegisterStream, "SPB", StorageObjectCreator.Instance, SPBReferenceCreator.Instance, ComponentType.Pagination, 1);
				}
			}

			internal void InitCancelPage(double pageHeight)
			{
				m_pageHeightForMemory = Math.Min(pageHeight, m_pageHeightForMemory);
				m_initCancelPage = true;
			}

			internal void ResetCancelPage()
			{
				m_initCancelPage = false;
				DisposeMemoryPressureListener();
				if (m_cancelPage)
				{
					m_itemPropsStart = null;
					m_sharedImages = null;
					m_pageBreakInfo = null;
					DisposeTextboxSharedInfo();
					if (m_imageConsolidation != null)
					{
						m_imageConsolidation.ResetCancelPage();
					}
				}
			}

			internal void CheckPageSize(RoundedDouble pageHeight)
			{
				if (m_initCancelPage && m_memoryPressure == null && pageHeight > m_pageHeightForMemory)
				{
					m_memoryPressure = new MemoryPressureListener();
				}
			}

			private void CreateGraphics()
			{
				m_hdcBits = new Bitmap(2, 2);
				m_hdcBits.SetResolution(96f, 96f);
				m_bitsGraphics = Graphics.FromImage(m_hdcBits);
				m_bitsGraphics.CompositingMode = CompositingMode.SourceOver;
				m_bitsGraphics.PageUnit = GraphicsUnit.Millimeter;
				m_bitsGraphics.PixelOffsetMode = PixelOffsetMode.Default;
				m_bitsGraphics.SmoothingMode = SmoothingMode.Default;
				m_bitsGraphics.TextRenderingHint = TextRenderingHint.SystemDefault;
			}

			private void DisposeGraphics()
			{
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
				if (m_fontCache != null)
				{
					m_fontCache.Dispose();
					m_fontCache = null;
				}
			}

			private void DisposeScalabilityCache()
			{
				if (m_scalabilityCache != null)
				{
					m_totalScaleTimeMs += m_scalabilityCache.ScalabilityDurationMs;
					m_peakMemoryUsageKB = Math.Max(m_peakMemoryUsageKB, m_scalabilityCache.PeakMemoryUsageKBytes);
					m_scalabilityCache.Dispose();
					m_scalabilityCache = null;
				}
			}

			private void DisposeMemoryPressureListener()
			{
				if (m_memoryPressure != null)
				{
					m_memoryPressure.Dispose();
					m_memoryPressure = null;
				}
			}

			private void DisposeTextboxSharedInfo()
			{
				if (m_textboxSharedInfo == null)
				{
					return;
				}
				foreach (TextBoxSharedInfo value in m_textboxSharedInfo.Values)
				{
					if (value != null && value.SharedFont != null)
					{
						value.Dispose();
					}
				}
				m_textboxSharedInfo = null;
			}

			internal void DisposeDelayTextBox()
			{
				if (m_delayTextBoxCache != null)
				{
					m_propertyCacheReader = null;
					m_propertyCacheWriter = null;
					m_delayTextBoxCache.Close();
					m_delayTextBoxCache.Dispose();
					m_delayTextBoxCache = null;
				}
			}

			internal void CreateCacheStream(long offsetStart)
			{
				if (m_delayTextBoxCache == null)
				{
					m_delayTextBoxCache = m_createAndRegisterStream("SPBNonSharedCache", "rpl", null, null, willSeek: true, StreamOper.CreateOnly);
					BufferedStream bufferedStream = new BufferedStream(m_delayTextBoxCache);
					m_propertyCacheReader = new BinaryReader(bufferedStream, Encoding.Unicode);
					m_propertyCacheWriter = new BinaryWriter(bufferedStream, Encoding.Unicode);
				}
				m_delayTextBoxCache.Position = offsetStart;
			}
		}

		internal class MemoryPressureListener : IDisposable
		{
			private int m_notificationCount;

			public bool ReceivedPressureNotification => m_notificationCount > 0;

			public void ResetNotificationState()
			{
				m_notificationCount = 0;
			}

			public bool CheckAndResetNotification()
			{
				bool receivedPressureNotification = ReceivedPressureNotification;
				ResetNotificationState();
				return receivedPressureNotification;
			}

			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}
		}

		internal static readonly string PNG_MIME_TYPE = "image/png";

		internal const double RoundDelta = 0.01;

		internal const double RoundOverlapDelta = 0.0001;

		internal const string InvalidImage = "InvalidImage";

		internal const char StreamNameSeparator = '_';

		internal const string PageHeaderSuffix = "H";

		internal const string PageFooterSuffix = "F";

		internal const string ImageStreamNamePrefix = "I";

		internal const string BkGndImageStreamNamePrefix = "B";

		internal const string ChartStreamNamePrefix = "C";

		internal const string GaugeStreamNamePrefix = "G";

		internal const string MapStreamNamePrefix = "M";

		internal const double InvalidImageWidth = 3.8;

		internal const double InvalidImageHeight = 4.0;

		internal const double MinFontSize = 0.3528;

		internal const char RPLVersionSeparator = '.';

		private PageContextFlags m_flags;

		private PageContextCommon m_common;

		private IgnorePBReasonFlag m_ignorePBReason;

		internal ImageConsolidation ImageConsolidation
		{
			get
			{
				return m_common.ImageConsolidation;
			}
			set
			{
				m_common.ImageConsolidation = value;
			}
		}

		internal IScalabilityCache ScalabilityCache => m_common.ScalabilityCache;

		internal long TotalScaleTimeMs => m_common.TotalScaleTimeMs;

		internal long PeakMemoryUsageKB => m_common.PeakMemoryUsageKB;

		internal bool CancelMode => m_common.CancelMode;

		internal bool CancelPage
		{
			get
			{
				return m_common.CancelPage;
			}
			set
			{
				m_common.CancelPage = value;
			}
		}

		internal double PageHeight
		{
			get
			{
				if (KeepTogether)
				{
					return double.MaxValue;
				}
				return m_common.PageHeight;
			}
			set
			{
				m_common.PageHeight = value;
			}
		}

		internal double OriginalPageHeight
		{
			get
			{
				return m_common.OriginalPageHeight;
			}
			set
			{
				m_common.OriginalPageHeight = value;
			}
		}

		internal PageContextFlags Flags => m_flags;

		internal bool IgnorePageBreaks
		{
			get
			{
				if (FullOnPage)
				{
					return true;
				}
				return (int)(m_flags & PageContextFlags.IgnorePageBreak) > 0;
			}
			set
			{
				if (value)
				{
					m_flags |= PageContextFlags.IgnorePageBreak;
				}
				else
				{
					m_flags &= ~PageContextFlags.IgnorePageBreak;
				}
			}
		}

		internal bool FullOnPage
		{
			get
			{
				return (int)(m_flags & PageContextFlags.FullOnPage) > 0;
			}
			set
			{
				if (value)
				{
					m_flags |= PageContextFlags.FullOnPage;
				}
				else
				{
					m_flags &= ~PageContextFlags.FullOnPage;
				}
			}
		}

		internal bool StretchPage => (int)(m_flags & PageContextFlags.StretchPage) > 0;

		internal bool KeepTogether
		{
			get
			{
				return (int)(m_flags & PageContextFlags.KeepTogether) > 0;
			}
			set
			{
				if (value)
				{
					m_flags |= PageContextFlags.KeepTogether;
				}
				else
				{
					m_flags &= ~PageContextFlags.KeepTogether;
				}
			}
		}

		internal bool HideDuplicates
		{
			get
			{
				return (int)(m_flags & PageContextFlags.HideDuplicates) > 0;
			}
			set
			{
				if (value)
				{
					m_flags |= PageContextFlags.HideDuplicates;
				}
				else
				{
					m_flags &= ~PageContextFlags.HideDuplicates;
				}
			}
		}

		internal bool TypeCodeNonString
		{
			get
			{
				return (int)(m_flags & PageContextFlags.TypeCodeNonString) > 0;
			}
			set
			{
				if (value)
				{
					m_flags |= PageContextFlags.TypeCodeNonString;
				}
				else
				{
					m_flags &= ~PageContextFlags.TypeCodeNonString;
				}
			}
		}

		internal bool RegisterEvents => m_common.RegisterEvents;

		internal bool MeasureItems => m_common.MeasureItems;

		internal bool EmfDynamicImage => m_common.EmfDynamicImage;

		internal bool ConsumeContainerWhitespace => m_common.ConsumeContainerWhitespace;

		internal CreateAndRegisterStream CreateAndRegisterStream => m_common.CreateAndRegisterStream;

		internal SecondaryStreams SecondaryStreams => m_common.SecondaryStreams;

		internal bool AddToggledItems => m_common.AddToggledItems;

		internal bool AddOriginalValue => m_common.AddOriginalValue;

		internal bool AddSecondaryStreamNames => m_common.AddSecondaryStreamNames;

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

		internal bool AddFirstPageHeaderFooter
		{
			get
			{
				return m_common.AddFirstPageHeaderFooter;
			}
			set
			{
				m_common.AddFirstPageHeaderFooter = value;
			}
		}

		internal float DpiX
		{
			get
			{
				return m_common.DpiX;
			}
			set
			{
				m_common.DpiY = value;
			}
		}

		internal float DpiY
		{
			get
			{
				return m_common.DpiY;
			}
			set
			{
				m_common.DpiY = value;
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

		internal RPLReportSectionArea RPLSectionArea
		{
			get
			{
				return m_common.RPLSectionArea;
			}
			set
			{
				m_common.RPLSectionArea = value;
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

		internal Hashtable RegisteredStreamNames
		{
			get
			{
				return m_common.RegisteredStreamNames;
			}
			set
			{
				m_common.RegisteredStreamNames = value;
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

		internal Hashtable TextBoxSharedInfo
		{
			get
			{
				return m_common.TextBoxSharedInfo;
			}
			set
			{
				m_common.TextBoxSharedInfo = value;
			}
		}

		internal DocumentMapLabels Labels
		{
			get
			{
				return m_common.Labels;
			}
			set
			{
				m_common.Labels = value;
			}
		}

		internal Bookmarks Bookmarks
		{
			get
			{
				return m_common.Bookmarks;
			}
			set
			{
				m_common.Bookmarks = value;
			}
		}

		internal Dictionary<string, string> PageBookmarks
		{
			get
			{
				return m_common.PageBookmarks;
			}
			set
			{
				m_common.PageBookmarks = value;
			}
		}

		internal bool IsPageBreakRegistered => m_common.PageBreakInfo != null;

		internal bool IsPageNameRegistered => m_common.PageName != null;

		internal PageTotalInfo PageTotalInfo => m_common.PageTotalInfo;

		internal bool CanTracePagination
		{
			get
			{
				return m_common.CanTracePagination;
			}
			set
			{
				m_common.CanTracePagination = value;
			}
		}

		internal RPLVersionEnum VersionPicker
		{
			get
			{
				return m_common.VersionPicker;
			}
			set
			{
				m_common.VersionPicker = value;
			}
		}

		internal BinaryReader PropertyCacheReader => m_common.PropertyCacheReader;

		internal BinaryWriter PropertyCacheWriter => m_common.PropertyCacheWriter;

		internal PageContextCommon Common => m_common;

		internal bool ConvertImages => m_common.ConvertImages;

		internal IgnorePBReasonFlag IgnorePBReason => m_ignorePBReason;

		internal bool TracingEnabled
		{
			get
			{
				if (RenderingDiagnostics.Enabled)
				{
					return CanTracePagination;
				}
				return false;
			}
		}

		internal PageContext(string pageName, double pageHeight, bool registerEvents, bool consumeWhiteSpace, CreateAndRegisterStream createAndRegisterStream)
		{
			m_common = new PageContextCommon(pageName, pageHeight, registerEvents, consumeWhiteSpace, createAndRegisterStream);
		}

		internal PageContext(PageContext pageContext)
		{
			m_common = pageContext.Common;
			m_flags = pageContext.Flags;
			m_ignorePBReason = pageContext.IgnorePBReason;
		}

		internal PageContext(PageContext pageContext, PageContextFlags flags, IgnorePBReasonFlag ignorePBReason)
		{
			m_common = pageContext.Common;
			m_flags = flags;
			if (pageContext.IgnorePBReason == IgnorePBReasonFlag.None)
			{
				m_ignorePBReason = ignorePBReason;
			}
			else
			{
				m_ignorePBReason = pageContext.IgnorePBReason;
			}
			KeepTogether = pageContext.KeepTogether;
		}

		internal void SetContext(bool measureItems, bool emfDynamicImage, SecondaryStreams secondaryStreams, bool addSecondaryStreamNames, bool addToggledItems, bool addOriginalValue, bool addFirstPageHeaderFooter, bool convertImages)
		{
			m_common.SetContext(measureItems, emfDynamicImage, secondaryStreams, addSecondaryStreamNames, addToggledItems, addOriginalValue, addFirstPageHeaderFooter, convertImages);
			m_flags = PageContextFlags.Clear;
		}

		internal void InitCache()
		{
			m_common.InitCache();
		}

		internal void InitCancelPage(double pageHeight)
		{
			m_common.InitCancelPage(pageHeight);
		}

		internal void ResetCancelPage()
		{
			m_common.ResetCancelPage();
		}

		internal void RegisterPageBreak(PageBreakInfo pageBreakInfo)
		{
			RegisterPageBreak(pageBreakInfo, overrideChild: false);
		}

		internal void RegisterPageBreak(PageBreakInfo pageBreakInfo, bool overrideChild)
		{
			if (m_common.PageBreakInfo == null || overrideChild)
			{
				m_common.PageBreakInfo = pageBreakInfo;
			}
			else if (TracingEnabled)
			{
				TracePageBreakIgnoredBecauseOfPeerItem(pageBreakInfo);
			}
		}

		internal void RegisterPageName(string pageName)
		{
			RegisterPageName(pageName, overrideChild: false);
		}

		internal void RegisterPageName(string pageName, bool overrideChild)
		{
			if (pageName != null && (m_common.PageName == null || overrideChild))
			{
				m_common.PageName = pageName;
			}
		}

		internal void ApplyPageBreak(int currentPageNumber)
		{
			if (m_common.PageBreakInfo != null && !m_common.PageBreakInfo.Disabled)
			{
				if (m_common.PageBreakInfo.ResetPageNumber)
				{
					m_common.PageTotalInfo.FinalizePageNumberForTotal();
				}
				m_common.PageBreakInfo = null;
			}
		}

		internal void ApplyPageName(int currentPageNumber)
		{
			if (TracingEnabled)
			{
				TracePageNameChanged();
			}
			m_common.PageTotalInfo.SetPageName(currentPageNumber, m_common.PageName);
			m_common.PageName = null;
		}

		internal void CheckPageSize(RoundedDouble pageHeight)
		{
			m_common.CheckPageSize(pageHeight);
		}

		internal void DisposeResources()
		{
			m_common.DisposeResources();
		}

		internal float MeasureFullTextBoxHeight(Microsoft.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext, out float contentHeight)
		{
			return m_common.MeasureFullTextBoxHeight(textBox, flowContext, out contentHeight);
		}

		internal ItemSizes GetSharedItemSizesElement(ReportItem reportItem, bool isPadded)
		{
			return m_common.GetSharedItemSizesElement(reportItem, isPadded);
		}

		internal ItemSizes GetSharedItemSizesElement(ReportSize width, ReportSize height, string id, bool isPadded)
		{
			return m_common.GetSharedItemSizesElement(width, height, id, isPadded);
		}

		internal ItemSizes GetSharedRenderItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
		{
			return m_common.GetSharedRenderItemSizesElement(itemSizes, isPadded, returnPaddings);
		}

		internal ItemSizes GetSharedEdgeItemSizesElement(double top, double left, string id)
		{
			return m_common.GetSharedEdgeItemSizesElement(top, left, id);
		}

		internal ItemSizes GetSharedRenderEdgeItemSizesElement(ItemSizes itemSizes)
		{
			return m_common.GetSharedRenderEdgeItemSizesElement(itemSizes);
		}

		internal ItemSizes GetSharedFromRepeatItemSizesElement(ReportItem reportItem, bool isPadded)
		{
			return m_common.GetSharedFromRepeatItemSizesElement(reportItem, isPadded);
		}

		internal ItemSizes GetSharedRenderFromRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
		{
			return m_common.GetSharedRenderFromRepeatItemSizesElement(itemSizes, isPadded, returnPaddings);
		}

		internal ItemSizes GetSharedRenderRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
		{
			return m_common.GetSharedRenderRepeatItemSizesElement(itemSizes, isPadded, returnPaddings);
		}

		internal SizeF MeasureStringGDI(string text, CanvasFont font, SizeF layoutArea, out int charactersFitted, out int linesFilled)
		{
			return m_common.MeasureStringGDI(text, font, layoutArea, out charactersFitted, out linesFilled);
		}

		internal double ConvertToMillimeters(int coordinate, float dpi)
		{
			if (0f == dpi)
			{
				return double.MaxValue;
			}
			return 1.0 / (double)dpi * (double)coordinate * 25.399999618530273;
		}

		internal string GenerateStreamName(IImageInstance imageInstance, string ownerUniqueName)
		{
			StringBuilder stringBuilder = null;
			stringBuilder = ((!(imageInstance is BackgroundImageInstance)) ? new StringBuilder("I") : new StringBuilder("B"));
			stringBuilder.Append('_');
			stringBuilder.Append(ownerUniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(PageNumber);
			if (RPLSectionArea == RPLReportSectionArea.Header)
			{
				stringBuilder.Append('_');
				stringBuilder.Append("H");
			}
			else if (RPLSectionArea == RPLReportSectionArea.Footer)
			{
				stringBuilder.Append('_');
				stringBuilder.Append("F");
			}
			return stringBuilder.ToString();
		}

		internal string GenerateStreamName(ChartInstance chartIntance)
		{
			StringBuilder stringBuilder = new StringBuilder("C");
			stringBuilder.Append('_');
			stringBuilder.Append(chartIntance.UniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(PageNumber);
			return stringBuilder.ToString();
		}

		internal string GenerateStreamName(GaugePanelInstance gaugeIntance)
		{
			StringBuilder stringBuilder = new StringBuilder("G");
			stringBuilder.Append('_');
			stringBuilder.Append(gaugeIntance.UniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(PageNumber);
			return stringBuilder.ToString();
		}

		internal string GenerateStreamName(MapInstance mapIntance)
		{
			StringBuilder stringBuilder = new StringBuilder("M");
			stringBuilder.Append('_');
			stringBuilder.Append(mapIntance.UniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(PageNumber);
			return stringBuilder.ToString();
		}

		internal void RegisterPageBookmark(ReportItemInstance reportItemInstance)
		{
			m_common.RegisterPageBookmark(reportItemInstance);
		}

		internal void CreateCacheStream(long value)
		{
			m_common.CreateCacheStream(value);
		}

		private void TracePageBreakIgnoredBecauseOfPeerItem(PageBreakInfo ignoredPageBreak)
		{
			if (ignoredPageBreak != null && !ignoredPageBreak.Disabled)
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – peer item precedence", PageNumber, ignoredPageBreak.ReportItemName);
			}
		}

		private void TracePageNameChanged()
		{
			if (m_common.PageName != null && m_common.PageTotalInfo != null && string.CompareOrdinal(m_common.PageTotalInfo.GetPageName(PageNumber - 1), m_common.PageName) != 0)
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page name changed", PageNumber);
			}
		}
	}
}
