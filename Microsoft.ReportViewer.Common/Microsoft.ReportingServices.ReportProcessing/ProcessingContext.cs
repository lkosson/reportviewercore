using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class ProcessingContext
	{
		private ICatalogItemContext m_reportContext;

		private string m_requestUserName;

		private ParameterInfoCollection m_parameters;

		private ParameterInfoCollection m_queryParameters;

		private ReportProcessing.OnDemandSubReportCallback m_subReportCallback;

		private IGetResource m_getResourceFunction;

		private ReportProcessing.ExecutionType m_interactiveExecution;

		private CultureInfo m_userLanguage;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_initialUserProfileState;

		private ReportRuntimeSetup m_reportRuntimeSetup;

		private bool m_isHistorySnapshot;

		private IChunkFactory m_chunkFactory;

		private CreateAndRegisterStream m_createStreamCallback;

		private IJobContext m_jobContext;

		private IExtensionFactory m_extFactory;

		private IDataProtection m_dataProtection;

		private ReportProcessing.CreateReportChunk m_createReportChunkCallback;

		internal abstract bool EnableDataBackedParameters
		{
			get;
		}

		internal ICatalogItemContext ReportContext => m_reportContext;

		internal string RequestUserName => m_requestUserName;

		public ParameterInfoCollection Parameters => m_parameters;

		internal ReportProcessing.OnDemandSubReportCallback OnDemandSubReportCallback => m_subReportCallback;

		internal ReportProcessing.CreateReportChunk CreateReportChunkCallback
		{
			get
			{
				return m_createReportChunkCallback;
			}
			set
			{
				m_createReportChunkCallback = value;
			}
		}

		public IChunkFactory ChunkFactory
		{
			get
			{
				return m_chunkFactory;
			}
			set
			{
				m_chunkFactory = value;
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(m_chunkFactory);
				m_createReportChunkCallback = @object.CreateReportChunk;
			}
		}

		internal IGetResource GetResourceCallback => m_getResourceFunction;

		internal ReportProcessing.ExecutionType InteractiveExecution => m_interactiveExecution;

		internal CultureInfo UserLanguage => m_userLanguage;

		internal UserProfileState AllowUserProfileState => m_allowUserProfileState;

		internal UserProfileState InitialUserProfileState => m_initialUserProfileState;

		internal ReportRuntimeSetup ReportRuntimeSetup => m_reportRuntimeSetup;

		internal bool IsHistorySnapshot => m_isHistorySnapshot;

		internal ReportProcessingFlags ReportProcessingFlags
		{
			get
			{
				if (m_chunkFactory == null)
				{
					return ReportProcessingFlags.NotSet;
				}
				return m_chunkFactory.ReportProcessingFlags;
			}
		}

		internal abstract IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
		{
			get;
		}

		internal abstract RuntimeDataSourceInfoCollection DataSources
		{
			get;
		}

		internal virtual RuntimeDataSetInfoCollection SharedDataSetReferences => null;

		internal abstract bool CanShareDataSets
		{
			get;
		}

		internal CreateAndRegisterStream CreateStreamCallback => m_createStreamCallback;

		public ParameterInfoCollection QueryParameters => m_queryParameters;

		public IJobContext JobContext => m_jobContext;

		public IExtensionFactory ExtFactory => m_extFactory;

		public IDataProtection DataProtection => m_dataProtection;

		internal ProcessingContext(ICatalogItemContext reportContext, string requestUserName, ParameterInfoCollection parameters, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory createChunkFactory, ReportProcessing.ExecutionType interactiveExecution, CultureInfo culture, UserProfileState allowUserProfileState, UserProfileState initialUserProfileState, ReportRuntimeSetup reportRuntimeSetup, CreateAndRegisterStream createStreamCallback, bool isHistorySnapshot, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection)
		{
			Global.Tracer.Assert(reportContext != null, "(null != reportContext)");
			m_reportContext = reportContext;
			m_requestUserName = requestUserName;
			m_parameters = parameters;
			m_queryParameters = m_parameters.GetQueryParameters();
			m_subReportCallback = subReportCallback;
			m_getResourceFunction = getResourceFunction;
			m_chunkFactory = createChunkFactory;
			m_interactiveExecution = interactiveExecution;
			m_userLanguage = culture;
			m_allowUserProfileState = allowUserProfileState;
			m_initialUserProfileState = initialUserProfileState;
			m_reportRuntimeSetup = reportRuntimeSetup;
			m_createStreamCallback = createStreamCallback;
			m_isHistorySnapshot = isHistorySnapshot;
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(m_chunkFactory);
			m_createReportChunkCallback = @object.CreateReportChunk;
			m_jobContext = jobContext;
			m_extFactory = extFactory;
			m_dataProtection = dataProtection;
		}

		internal abstract ReportProcessing.ProcessingContext CreateInternalProcessingContext(string chartName, Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, ReportProcessing.GetReportChunk getChunkCallback, ReportProcessing.CreateReportChunk cacheDataCallback);

		internal abstract ReportProcessing.ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot);
	}
}
