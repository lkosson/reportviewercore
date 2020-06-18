using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RenderingContext
	{
		private bool m_isSubReportContext;

		private bool m_subReportProcessedWithError;

		private bool m_instanceAccessDisallowed;

		private bool m_subReportHasNoInstance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot m_reportSnapshot;

		private Microsoft.ReportingServices.ReportProcessing.ReportSnapshot m_oldReportSnapshot;

		private EventInformation m_eventInfo;

		private OnDemandProcessingContext m_odpContext;

		private List<IDynamicInstance> m_dynamicInstances;

		private PageEvaluation m_pageEvaluation;

		private bool m_nativeAllCRITypes;

		private Hashtable m_nativeCRITypes;

		private RenderingChunkManager m_chunkManager;

		private string m_rendererID;

		internal bool IsSubReportContext => m_isSubReportContext;

		internal bool SubReportProcessedWithError
		{
			get
			{
				return m_subReportProcessedWithError;
			}
			set
			{
				m_subReportProcessedWithError = value;
			}
		}

		internal bool SubReportHasNoInstance
		{
			get
			{
				return m_subReportHasNoInstance;
			}
			set
			{
				m_subReportHasNoInstance = value;
			}
		}

		internal bool SubReportHasErrorOrNoInstance
		{
			get
			{
				if (!m_subReportProcessedWithError)
				{
					return m_subReportHasNoInstance;
				}
				return true;
			}
		}

		internal bool InstanceAccessDisallowed
		{
			get
			{
				if (!m_instanceAccessDisallowed)
				{
					if (IsSubReportContext)
					{
						return SubReportHasErrorOrNoInstance;
					}
					return false;
				}
				return true;
			}
			set
			{
				m_instanceAccessDisallowed = value;
			}
		}

		internal OnDemandProcessingContext OdpContext => m_odpContext;

		internal IErrorContext ErrorContext
		{
			get
			{
				if (m_odpContext != null)
				{
					return m_odpContext.ReportRuntime;
				}
				return null;
			}
		}

		internal EventInformation EventInfo => m_eventInfo;

		internal bool EventInfoChanged => m_eventInfo.Changed;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot ReportSnapshot => m_reportSnapshot;

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

		internal RenderingContext(string rendererID, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, EventInformation eventInfo, OnDemandProcessingContext processingContext)
		{
			m_rendererID = rendererID;
			m_isSubReportContext = false;
			m_reportSnapshot = reportSnapshot;
			InitEventInfo(eventInfo);
			m_odpContext = processingContext;
			if (processingContext.ChunkFactory != null)
			{
				m_chunkManager = new RenderingChunkManager(rendererID, processingContext.ChunkFactory);
			}
		}

		internal RenderingContext(string rendererID, Microsoft.ReportingServices.ReportProcessing.ReportSnapshot reportSnapshot, IChunkFactory chunkFactory, EventInformation eventInfo)
		{
			m_rendererID = rendererID;
			m_isSubReportContext = false;
			m_oldReportSnapshot = reportSnapshot;
			InitEventInfo(eventInfo);
			if (chunkFactory != null)
			{
				m_chunkManager = new RenderingChunkManager(rendererID, chunkFactory);
			}
		}

		internal RenderingContext(RenderingContext parentContext)
			: this(parentContext, hasReportItemReferences: false)
		{
		}

		internal RenderingContext(RenderingContext parentContext, bool hasReportItemReferences)
		{
			m_rendererID = parentContext.m_rendererID;
			m_isSubReportContext = true;
			m_pageEvaluation = null;
			m_dynamicInstances = null;
			m_eventInfo = parentContext.EventInfo;
			m_reportSnapshot = parentContext.m_reportSnapshot;
			m_oldReportSnapshot = parentContext.m_oldReportSnapshot;
			m_chunkManager = parentContext.m_chunkManager;
			if (m_oldReportSnapshot != null)
			{
				m_odpContext = parentContext.OdpContext;
			}
			else
			{
				m_odpContext = new OnDemandProcessingContext(parentContext.m_odpContext, hasReportItemReferences, m_reportSnapshot.Report);
			}
		}

		private void InitEventInfo(EventInformation eventInfo)
		{
			if (eventInfo == null)
			{
				m_eventInfo = new EventInformation();
			}
			else
			{
				m_eventInfo = eventInfo;
			}
			m_eventInfo.Changed = false;
		}

		internal RenderingContext(RenderingContext parentContext, OnDemandProcessingContext onDemandProcessingContext)
		{
			m_rendererID = parentContext.m_rendererID;
			m_isSubReportContext = true;
			m_pageEvaluation = null;
			m_dynamicInstances = null;
			m_oldReportSnapshot = parentContext.m_oldReportSnapshot;
			m_eventInfo = parentContext.EventInfo;
			m_reportSnapshot = parentContext.m_reportSnapshot;
			m_chunkManager = parentContext.m_chunkManager;
			m_odpContext = onDemandProcessingContext;
		}

		internal void AddDynamicInstance(IDynamicInstance instance)
		{
			if (m_dynamicInstances == null)
			{
				m_dynamicInstances = new List<IDynamicInstance>();
			}
			m_dynamicInstances.Add(instance);
		}

		internal void ResetContext()
		{
			if (m_dynamicInstances != null)
			{
				for (int i = 0; i < m_dynamicInstances.Count; i++)
				{
					m_dynamicInstances[i].ResetContext();
				}
			}
		}

		internal void SetPageEvaluation(PageEvaluation pageEvaluation)
		{
			m_pageEvaluation = pageEvaluation;
		}

		internal void AddToCurrentPage(string textboxName, object textboxValue)
		{
			if (m_pageEvaluation != null)
			{
				m_pageEvaluation.Add(textboxName, textboxValue);
			}
		}

		internal void AddDrillthroughAction(string drillthroughId, string reportName, DrillthroughParameters reportParameters)
		{
			if (m_rendererID != null)
			{
				CheckResetEventInfo();
				EventInformation.RendererEventInformation rendererEventInformation = m_eventInfo.GetRendererEventInformation(m_rendererID);
				if (rendererEventInformation.DrillthroughInfo == null)
				{
					rendererEventInformation.DrillthroughInfo = new Hashtable();
				}
				if (!rendererEventInformation.DrillthroughInfo.ContainsKey(drillthroughId))
				{
					rendererEventInformation.DrillthroughInfo.Add(drillthroughId, new DrillthroughInfo(reportName, reportParameters));
					m_eventInfo.Changed = true;
				}
			}
		}

		private void CheckResetEventInfo()
		{
			if (!m_eventInfo.Changed)
			{
				m_eventInfo.GetRendererEventInformation(m_rendererID).Reset();
				m_eventInfo.Changed = true;
			}
		}

		internal void AddValidToggleSender(string senderUniqueName)
		{
			CheckResetEventInfo();
			EventInformation.RendererEventInformation rendererEventInformation = m_eventInfo.GetRendererEventInformation(m_rendererID);
			if (rendererEventInformation.ValidToggleSenders == null)
			{
				rendererEventInformation.ValidToggleSenders = new Hashtable();
			}
			if (!rendererEventInformation.ValidToggleSenders.ContainsKey(senderUniqueName))
			{
				rendererEventInformation.ValidToggleSenders.Add(senderUniqueName, null);
				m_eventInfo.Changed = true;
			}
		}

		internal bool IsSenderToggled(string uniqueName)
		{
			EventInformation eventInfo = EventInfo;
			if (eventInfo != null)
			{
				return eventInfo.ToggleStateInfo?.ContainsKey(uniqueName) ?? false;
			}
			return false;
		}

		internal SortOptions GetSortState(string eventSourceUniqueName)
		{
			if (m_eventInfo != null && m_eventInfo.OdpSortInfo != null)
			{
				return m_eventInfo.OdpSortInfo.GetSortState(eventSourceUniqueName);
			}
			return SortOptions.None;
		}

		internal string GenerateShimUniqueName(string baseUniqueName)
		{
			return "x" + baseUniqueName;
		}

		internal Stream GetOrCreateChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName, bool createChunkIfNotExists, out bool isNewChunk)
		{
			if (!IsChunkManagerValid())
			{
				isNewChunk = false;
				return null;
			}
			return m_chunkManager.GetOrCreateChunk(type, chunkName, createChunkIfNotExists, out isNewChunk);
		}

		internal Stream CreateChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName)
		{
			if (!IsChunkManagerValid())
			{
				return null;
			}
			return m_chunkManager.CreateChunk(type, chunkName);
		}

		private bool IsChunkManagerValid()
		{
			bool result = true;
			if (m_chunkManager == null)
			{
				if (m_odpContext != null)
				{
					m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsRenderingChunksUnavailable, Severity.Warning, ObjectType.Report, "Report", "Report");
					m_odpContext.TraceOneTimeWarning(ProcessingErrorCode.rsRenderingChunksUnavailable);
				}
				result = false;
			}
			return result;
		}

		internal void CloseRenderingChunkManager()
		{
			if (m_chunkManager != null)
			{
				m_chunkManager.CloseAllChunks();
			}
		}

		internal bool IsRenderAsNativeCri(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem criDef)
		{
			if (m_nativeAllCRITypes)
			{
				return true;
			}
			if (m_nativeCRITypes != null && m_nativeCRITypes.ContainsKey(criDef.Type))
			{
				return true;
			}
			return false;
		}
	}
}
