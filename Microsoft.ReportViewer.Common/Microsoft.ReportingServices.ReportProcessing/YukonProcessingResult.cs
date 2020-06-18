using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class YukonProcessingResult : OnDemandProcessingResult
	{
		private ReportSnapshot m_newSnapshot;

		private ChunkManager.ProcessingChunkManager m_legacyChunkManager;

		private bool m_renderingInfoChanged;

		private RenderingInfoManager m_renderingInfoManager;

		private readonly bool m_snapshotChanged;

		public override bool SnapshotChanged => m_snapshotChanged;

		internal YukonProcessingResult(ReportSnapshot newSnapshot, ChunkManager.ProcessingChunkManager chunkManager, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool renderingInfoChanged, RenderingInfoManager renderingInfoManager, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newSnapshot.HasDocumentMap, newSnapshot.HasShowHide, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			m_snapshotChanged = Initialize(newSnapshot, chunkManager, renderingInfoChanged, renderingInfoManager);
		}

		internal YukonProcessingResult(bool renderingInfoChanged, IChunkFactory createChunkFactory, bool hasInteractivity, RenderingInfoManager renderingInfoManager, bool eventInfoChanged, EventInformation newEventInfo, ParameterInfoCollection parameters, ProcessingMessageList warnings, int autoRefresh, int numberOfPages, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, hasDocumentMap: false, hasInteractivity, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			m_snapshotChanged = Initialize(null, null, renderingInfoChanged, renderingInfoManager);
		}

		internal YukonProcessingResult(ReportSnapshot newSnapshot, ChunkManager.ProcessingChunkManager chunkManager, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(null, newSnapshot.HasDocumentMap, newSnapshot.HasShowHide, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged: false, null, PaginationMode.Progressive, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			m_snapshotChanged = Initialize(newSnapshot, chunkManager, renderingInfoChanged: false, null);
		}

		private bool Initialize(ReportSnapshot newSnapshot, ChunkManager.ProcessingChunkManager chunkManager, bool renderingInfoChanged, RenderingInfoManager renderingInfoManager)
		{
			m_newSnapshot = newSnapshot;
			m_legacyChunkManager = chunkManager;
			m_renderingInfoChanged = renderingInfoChanged;
			m_renderingInfoManager = renderingInfoManager;
			if (!m_renderingInfoChanged)
			{
				return m_newSnapshot != null;
			}
			return true;
		}

		public override void Save()
		{
			lock (this)
			{
				if (m_newSnapshot != null && m_legacyChunkManager != null)
				{
					m_legacyChunkManager.SaveFirstPage();
					m_legacyChunkManager.SaveReportSnapshot(m_newSnapshot);
					m_newSnapshot = null;
				}
				if (m_renderingInfoChanged && m_renderingInfoManager != null)
				{
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(m_createChunkFactory);
					m_renderingInfoManager.Save(@object.CreateReportChunk);
					m_renderingInfoManager = null;
				}
			}
		}
	}
}
