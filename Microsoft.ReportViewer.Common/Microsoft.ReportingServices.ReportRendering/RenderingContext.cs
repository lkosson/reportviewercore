using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class RenderingContext
	{
		private sealed class CommonInfo
		{
			private string m_rendererID;

			private DateTime m_executionTime;

			private string m_replacementRoot;

			private RenderingInfoManager m_renderingInfoManager;

			private ChunkManager.RenderingChunkManager m_chunkManager;

			private IGetResource m_getResourceCallback;

			private bool m_cacheState;

			private ICatalogItemContext m_reportContext;

			private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk m_getChunkCallback;

			private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetChunkMimeType m_getChunkMimeType;

			private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters m_storeServerParameters;

			private UserProfileState m_allowUserProfileState;

			private UserProfileState m_usedUserProfileState;

			private ReportRuntimeSetup m_reportRuntimeSetup;

			private IntermediateFormatVersion m_intermediateFormatVersion;

			internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk GetChunkCallback => m_getChunkCallback;

			internal string RendererID => m_rendererID;

			internal DateTime ExecutionTime => m_executionTime;

			internal string ReplacementRoot => m_replacementRoot;

			internal RenderingInfoManager RenderingInfoManager => m_renderingInfoManager;

			internal bool CacheState
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

			internal ChunkManager.RenderingChunkManager ChunkManager => m_chunkManager;

			internal IGetResource GetResourceCallback => m_getResourceCallback;

			internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetChunkMimeType GetChunkMimeType => m_getChunkMimeType;

			internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters StoreServerParameters => m_storeServerParameters;

			internal ICatalogItemContext TopLevelReportContext => m_reportContext;

			internal UserProfileState AllowUserProfileState => m_allowUserProfileState;

			internal UserProfileState UsedUserProfileState
			{
				get
				{
					return m_usedUserProfileState;
				}
				set
				{
					m_usedUserProfileState = value;
				}
			}

			internal ReportRuntimeSetup ReportRuntimeSetup => m_reportRuntimeSetup;

			internal IntermediateFormatVersion IntermediateFormatVersion => m_intermediateFormatVersion;

			internal CommonInfo(string rendererID, DateTime executionTime, ICatalogItemContext reportContext, NameValueCollection reportParameters, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback, ChunkManager.RenderingChunkManager chunkManager, IGetResource getResourceCallback, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetChunkMimeType getChunkMimeType, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, bool retrieveRenderingInfo, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, IntermediateFormatVersion intermediateFormatVersion)
			{
				m_rendererID = rendererID;
				m_executionTime = executionTime;
				m_reportContext = reportContext;
				if (reportParameters != null)
				{
					m_replacementRoot = reportParameters["ReplacementRoot"];
				}
				m_renderingInfoManager = new RenderingInfoManager(rendererID, getChunkCallback, retrieveRenderingInfo);
				m_chunkManager = chunkManager;
				m_getResourceCallback = getResourceCallback;
				m_getChunkCallback = getChunkCallback;
				m_getChunkMimeType = getChunkMimeType;
				m_storeServerParameters = storeServerParameters;
				m_allowUserProfileState = allowUserProfileState;
				m_reportRuntimeSetup = reportRuntimeSetup;
				m_intermediateFormatVersion = intermediateFormatVersion;
			}
		}

		private CommonInfo m_commonInfo;

		private bool m_inPageSection;

		private string m_prefix;

		private EventInformation m_eventInfo;

		private ReportSnapshot m_reportSnapshot;

		private Hashtable m_processedItems;

		private Hashtable m_cachedHiddenInfo;

		private Uri m_contextUri;

		private EmbeddedImageHashtable m_embeddedImages;

		private ImageStreamNames m_imageStreamNames;

		private MatrixHeadingInstance m_headingInstance;

		private ICatalogItemContext m_currentReportICatalogItemContext;

		private bool m_nativeAllCRITypes;

		private Hashtable m_nativeCRITypes;

		private IJobContext m_jobContext;

		private IDataProtection m_dataProtection;

		internal ICatalogItemContext TopLevelReportContext => m_commonInfo.TopLevelReportContext;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk GetChunkCallback => m_commonInfo.GetChunkCallback;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetChunkMimeType GetChunkMimeType => m_commonInfo.GetChunkMimeType;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters StoreServerParameters => m_commonInfo.StoreServerParameters;

		internal string RendererID => m_commonInfo.RendererID;

		internal DateTime ExecutionTime => m_commonInfo.ExecutionTime;

		internal string ReplacementRoot => m_commonInfo.ReplacementRoot;

		internal bool CacheState
		{
			get
			{
				return m_commonInfo.CacheState;
			}
			set
			{
				m_commonInfo.CacheState = value;
			}
		}

		internal RenderingInfoManager RenderingInfoManager => m_commonInfo.RenderingInfoManager;

		internal ChunkManager.RenderingChunkManager ChunkManager => m_commonInfo.ChunkManager;

		internal IGetResource GetResourceCallback => m_commonInfo.GetResourceCallback;

		internal ReportRuntimeSetup ReportRuntimeSetup => m_commonInfo.ReportRuntimeSetup;

		internal IntermediateFormatVersion IntermediateFormatVersion => m_commonInfo.IntermediateFormatVersion;

		internal ImageStreamNames ImageStreamNames => m_imageStreamNames;

		internal EmbeddedImageHashtable EmbeddedImages => m_embeddedImages;

		internal bool InPageSection => m_inPageSection;

		internal string UniqueNamePrefix
		{
			get
			{
				Global.Tracer.Assert(m_inPageSection);
				return m_prefix;
			}
		}

		internal Uri ContextUri => m_contextUri;

		internal ReportSnapshot ReportSnapshot
		{
			get
			{
				Global.Tracer.Assert(m_reportSnapshot != null);
				return m_reportSnapshot;
			}
		}

		private SenderInformationHashtable ShowHideSenderInfo
		{
			get
			{
				if (m_reportSnapshot != null)
				{
					return m_reportSnapshot.GetShowHideSenderInfo(ChunkManager);
				}
				return null;
			}
		}

		private ReceiverInformationHashtable ShowHideReceiverInfo
		{
			get
			{
				if (m_reportSnapshot != null)
				{
					return m_reportSnapshot.GetShowHideReceiverInfo(ChunkManager);
				}
				return null;
			}
		}

		internal MatrixHeadingInstance HeadingInstance
		{
			get
			{
				return m_headingInstance;
			}
			set
			{
				m_headingInstance = value;
			}
		}

		internal UserProfileState AllowUserProfileState => m_commonInfo.AllowUserProfileState;

		internal UserProfileState UsedUserProfileState
		{
			get
			{
				return m_commonInfo.UsedUserProfileState;
			}
			set
			{
				m_commonInfo.UsedUserProfileState = value;
			}
		}

		internal ICatalogItemContext CurrentReportContext => m_currentReportICatalogItemContext;

		internal bool NativeAllCRITypes
		{
			get
			{
				return m_nativeAllCRITypes;
			}
			set
			{
				m_nativeAllCRITypes = value;
			}
		}

		internal Hashtable NativeCRITypes
		{
			get
			{
				return m_nativeCRITypes;
			}
			set
			{
				m_nativeCRITypes = value;
			}
		}

		internal IJobContext JobContext => m_jobContext;

		internal IDataProtection DataProtection => m_dataProtection;

		internal bool ShowHideStateChanged
		{
			get
			{
				if (m_eventInfo != null && m_eventInfo.ToggleStateInfo != null && m_eventInfo.HiddenInfo != null)
				{
					return true;
				}
				return false;
			}
		}

		internal RenderingContext(ReportSnapshot reportSnapshot, string rendererID, DateTime executionTime, EmbeddedImageHashtable embeddedImages, ImageStreamNames imageStreamNames, EventInformation eventInfo, ICatalogItemContext reportContext, Uri contextUri, NameValueCollection reportParameters, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback, ChunkManager.RenderingChunkManager chunkManager, IGetResource getResourceCallback, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetChunkMimeType getChunkMimeType, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, bool retrieveRenderingInfo, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IDataProtection dataProtection)
		{
			m_commonInfo = new CommonInfo(rendererID, executionTime, reportContext, reportParameters, getChunkCallback, chunkManager, getResourceCallback, getChunkMimeType, storeServerParameters, retrieveRenderingInfo, allowUserProfileState, reportRuntimeSetup, reportSnapshot.Report.IntermediateFormatVersion);
			m_inPageSection = false;
			m_prefix = null;
			m_eventInfo = eventInfo;
			m_reportSnapshot = reportSnapshot;
			m_processedItems = null;
			m_cachedHiddenInfo = null;
			m_contextUri = contextUri;
			m_embeddedImages = embeddedImages;
			m_imageStreamNames = imageStreamNames;
			m_currentReportICatalogItemContext = m_commonInfo.TopLevelReportContext;
			m_jobContext = jobContext;
			m_dataProtection = dataProtection;
		}

		internal RenderingContext(RenderingContext copy, Uri contextUri, EmbeddedImageHashtable embeddedImages, ImageStreamNames imageStreamNames, ICatalogItemContext subreportICatalogItemContext)
		{
			m_commonInfo = copy.m_commonInfo;
			m_inPageSection = false;
			m_prefix = null;
			m_eventInfo = copy.m_eventInfo;
			m_reportSnapshot = copy.ReportSnapshot;
			m_processedItems = null;
			m_cachedHiddenInfo = copy.m_cachedHiddenInfo;
			m_contextUri = contextUri;
			m_embeddedImages = embeddedImages;
			m_imageStreamNames = imageStreamNames;
			m_currentReportICatalogItemContext = subreportICatalogItemContext;
			m_jobContext = copy.m_jobContext;
			m_dataProtection = copy.m_dataProtection;
		}

		internal RenderingContext(RenderingContext copy, string prefix)
		{
			m_commonInfo = copy.m_commonInfo;
			m_inPageSection = true;
			m_prefix = prefix;
			m_eventInfo = null;
			m_reportSnapshot = null;
			m_processedItems = null;
			m_cachedHiddenInfo = null;
			m_contextUri = copy.m_contextUri;
			m_embeddedImages = copy.EmbeddedImages;
			m_imageStreamNames = copy.ImageStreamNames;
			m_currentReportICatalogItemContext = m_commonInfo.TopLevelReportContext;
			m_jobContext = copy.m_jobContext;
			m_dataProtection = copy.m_dataProtection;
		}

		internal ReportItem FindReportItemInBody(int uniqueName)
		{
			object obj = null;
			NonComputedUniqueNames nonCompNames = null;
			QuickFindHashtable quickFind = ReportSnapshot.GetQuickFind(ChunkManager);
			if (quickFind != null)
			{
				obj = quickFind[uniqueName];
			}
			if (obj == null)
			{
				Global.Tracer.Assert(ReportSnapshot.ReportInstance != null);
				obj = ((ISearchByUniqueName)ReportSnapshot.ReportInstance).Find(uniqueName, ref nonCompNames, ChunkManager);
				if (obj == null)
				{
					return null;
				}
			}
			if (obj is Microsoft.ReportingServices.ReportProcessing.ReportItem)
			{
				Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef = (Microsoft.ReportingServices.ReportProcessing.ReportItem)obj;
				return ReportItem.CreateItem(-1, reportItemDef, null, this, nonCompNames);
			}
			Microsoft.ReportingServices.ReportProcessing.ReportItemInstance reportItemInstance = (Microsoft.ReportingServices.ReportProcessing.ReportItemInstance)obj;
			return ReportItem.CreateItem(-1, reportItemInstance.ReportItemDef, reportItemInstance, this, nonCompNames);
		}

		internal bool IsItemHidden(int uniqueName, bool potentialSender)
		{
			try
			{
				if (ShowHideReceiverInfo == null || ShowHideSenderInfo == null)
				{
					return false;
				}
				if (m_processedItems == null)
				{
					m_processedItems = new Hashtable();
				}
				return RecursiveIsItemHidden(uniqueName, potentialSender);
			}
			finally
			{
				if (m_processedItems != null)
				{
					m_processedItems.Clear();
				}
			}
		}

		internal bool IsToggleStateNegated(int uniqueName)
		{
			if (m_eventInfo == null || m_eventInfo.ToggleStateInfo == null || m_eventInfo.HiddenInfo == null)
			{
				return false;
			}
			return m_eventInfo.ToggleStateInfo.ContainsKey(uniqueName);
		}

		internal bool IsToggleParent(int uniqueName)
		{
			if (ShowHideSenderInfo == null)
			{
				return false;
			}
			return ShowHideSenderInfo.ContainsKey(uniqueName);
		}

		internal bool IsToggleChild(int uniqueName)
		{
			if (ShowHideReceiverInfo == null)
			{
				return false;
			}
			return ShowHideReceiverInfo.ContainsKey(uniqueName);
		}

		internal TextBox GetToggleParent(int uniqueName)
		{
			if (ShowHideReceiverInfo != null)
			{
				ReceiverInformation receiverInformation = ShowHideReceiverInfo[uniqueName];
				if (receiverInformation != null)
				{
					ReportItem reportItem = FindReportItemInBody(receiverInformation.SenderUniqueName);
					Global.Tracer.Assert(reportItem != null);
					Global.Tracer.Assert(reportItem is TextBox);
					return (TextBox)reportItem;
				}
			}
			return null;
		}

		internal static bool GetDefinitionHidden(Microsoft.ReportingServices.ReportProcessing.Visibility visibility)
		{
			if (visibility == null)
			{
				return false;
			}
			if (visibility.Hidden == null)
			{
				return false;
			}
			if (ExpressionInfo.Types.Constant == visibility.Hidden.Type)
			{
				return visibility.Hidden.BoolValue;
			}
			return false;
		}

		internal SortOptions GetSortState(int uniqueName)
		{
			if (m_eventInfo != null && m_eventInfo.SortInfo != null)
			{
				return m_eventInfo.SortInfo.GetSortState(uniqueName);
			}
			return SortOptions.None;
		}

		private bool RecursiveIsItemHidden(int uniqueName, bool potentialSender)
		{
			Global.Tracer.Assert(m_processedItems != null);
			if (m_processedItems.ContainsKey(uniqueName))
			{
				return false;
			}
			m_processedItems.Add(uniqueName, null);
			if (m_cachedHiddenInfo == null)
			{
				m_cachedHiddenInfo = new Hashtable();
			}
			else
			{
				object obj = m_cachedHiddenInfo[uniqueName];
				if (obj != null)
				{
					return (bool)obj;
				}
			}
			ReceiverInformation receiverInformation = ShowHideReceiverInfo[uniqueName];
			if (receiverInformation != null)
			{
				if (IsHidden(uniqueName, receiverInformation.StartHidden))
				{
					m_cachedHiddenInfo[uniqueName] = true;
					return true;
				}
				if (RecursiveIsItemHidden(receiverInformation.SenderUniqueName, potentialSender: true))
				{
					m_cachedHiddenInfo[uniqueName] = true;
					return true;
				}
			}
			if (potentialSender)
			{
				SenderInformation senderInformation = ShowHideSenderInfo[uniqueName];
				if (senderInformation != null)
				{
					if (IsHidden(uniqueName, senderInformation.StartHidden))
					{
						m_cachedHiddenInfo[uniqueName] = true;
						return true;
					}
					if (senderInformation.ContainerUniqueNames != null)
					{
						for (int num = senderInformation.ContainerUniqueNames.Length - 1; num >= 0; num--)
						{
							if (RecursiveIsItemHidden(senderInformation.ContainerUniqueNames[num], potentialSender: false))
							{
								m_cachedHiddenInfo[uniqueName] = true;
								return true;
							}
						}
					}
				}
			}
			m_cachedHiddenInfo[uniqueName] = false;
			return false;
		}

		private bool IsHidden(int uniqueName, bool startHidden)
		{
			if (IsHiddenNegated(uniqueName))
			{
				return !startHidden;
			}
			return startHidden;
		}

		private bool IsHiddenNegated(int uniqueName)
		{
			if (m_eventInfo == null || m_eventInfo.ToggleStateInfo == null || m_eventInfo.HiddenInfo == null)
			{
				return false;
			}
			return m_eventInfo.HiddenInfo.ContainsKey(uniqueName);
		}

		internal static void FindRange(RenderingPagesRangesList pagesRangesList, int startIndex, int endIndex, int page, ref int startChild, ref int endChild)
		{
			FindRange(pagesRangesList, startIndex, endIndex, page, checkStart: true, checkEnd: true, ref startChild, ref endChild);
		}

		internal static void FindRange(RenderingPagesRangesList pagesRangesList, int startIndex, int endIndex, int page, bool checkStart, bool checkEnd, ref int startChild, ref int endChild)
		{
			int num = 0;
			bool flag = false;
			int endChild2 = 0;
			while (!flag && endIndex >= startIndex)
			{
				num = startIndex + (endIndex - startIndex) / 2;
				RenderingPagesRanges renderingPagesRanges = pagesRangesList[num];
				if (renderingPagesRanges.StartPage > page)
				{
					endIndex = num - 1;
					continue;
				}
				if (renderingPagesRanges.EndPage < page)
				{
					startIndex = num + 1;
					continue;
				}
				flag = true;
				startChild = num;
				endChild = num;
				if (checkStart && renderingPagesRanges.StartPage == page)
				{
					FindRange(pagesRangesList, startIndex, num - 1, page, checkStart: true, checkEnd: false, ref startChild, ref endChild2);
				}
				if (checkEnd && renderingPagesRanges.EndPage == page)
				{
					FindRange(pagesRangesList, num + 1, endIndex, page, checkStart: false, checkEnd: true, ref endChild2, ref endChild);
				}
			}
		}
	}
}
