using Microsoft.ReportingServices.OnDemandProcessing;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class OnDemandProcessingResult
	{
		private readonly bool m_hasInteractivity;

		private readonly ParameterInfoCollection m_parameters;

		private readonly int m_autoRefresh;

		private readonly ProcessingMessageList m_warnings;

		private readonly int m_numberOfPages;

		private readonly bool m_hasDocumentMap;

		protected readonly IChunkFactory m_createChunkFactory;

		private readonly bool m_eventInfoChanged;

		private readonly EventInformation m_newEventInfo;

		private readonly PaginationMode m_updatedPaginationMode;

		private readonly ReportProcessingFlags m_updatedReportProcessingFlags;

		private readonly UserProfileState m_usedUserProfileState;

		private readonly ExecutionLogContext m_executionLogContext;

		public abstract bool SnapshotChanged
		{
			get;
		}

		public bool HasInteractivity => m_hasInteractivity;

		public bool HasDocumentMap => m_hasDocumentMap;

		public ParameterInfoCollection Parameters => m_parameters;

		public int AutoRefresh => m_autoRefresh;

		public ProcessingMessageList Warnings => m_warnings;

		public int NumberOfPages => m_numberOfPages;

		public bool EventInfoChanged => m_eventInfoChanged;

		public EventInformation NewEventInfo => m_newEventInfo;

		public PaginationMode UpdatedPaginationMode => m_updatedPaginationMode;

		public ReportProcessingFlags UpdatedReportProcessingFlags => m_updatedReportProcessingFlags;

		public UserProfileState UsedUserProfileState => m_usedUserProfileState;

		public ExecutionLogContext ExecutionLogContext => m_executionLogContext;

		protected OnDemandProcessingResult(IChunkFactory createChunkFactory, bool hasDocumentMap, bool hasInteractivity, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
		{
			m_createChunkFactory = createChunkFactory;
			m_hasDocumentMap = hasDocumentMap;
			m_numberOfPages = numberOfPages;
			m_hasInteractivity = hasInteractivity;
			m_parameters = parameters;
			m_autoRefresh = autoRefresh;
			m_warnings = warnings;
			m_eventInfoChanged = eventInfoChanged;
			m_newEventInfo = newEventInfo;
			m_parameters = parameters;
			m_updatedPaginationMode = updatedPaginationMode;
			m_updatedReportProcessingFlags = updatedProcessingFlags;
			m_usedUserProfileState = usedUserProfileState;
			m_executionLogContext = executionLogContext;
		}

		public abstract void Save();
	}
}
