using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class DataSetContext
	{
		private string m_targetChunkNameInSnapshot;

		private string m_cachedDataChunkName;

		private bool m_mustCreateDataChunk;

		private IRowConsumer m_consumerRequest;

		private ICatalogItemContext m_itemContext;

		private RuntimeDataSourceInfoCollection m_dataSources;

		private string m_requestUserName;

		private DateTime m_executionTimeStamp;

		private ParameterInfoCollection m_parameters;

		private IChunkFactory m_createChunkFactory;

		private ReportProcessing.ExecutionType m_interactiveExecution;

		private CultureInfo m_culture;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_initialUserProfileState;

		private IProcessingDataExtensionConnection m_createDataExtensionInstanceFunction;

		private CreateAndRegisterStream m_createStreamCallbackForScalability;

		private ReportRuntimeSetup m_dataSetRuntimeSetup;

		private IJobContext m_jobContext;

		private IDataProtection m_dataProtection;

		public string TargetChunkNameInSnapshot
		{
			get
			{
				return m_targetChunkNameInSnapshot;
			}
			set
			{
				m_targetChunkNameInSnapshot = value;
			}
		}

		public string CachedDataChunkName
		{
			get
			{
				return m_cachedDataChunkName;
			}
			set
			{
				m_cachedDataChunkName = value;
			}
		}

		public bool MustCreateDataChunk
		{
			get
			{
				return m_mustCreateDataChunk;
			}
			set
			{
				m_mustCreateDataChunk = value;
			}
		}

		public IRowConsumer ConsumerRequest => m_consumerRequest;

		public ICatalogItemContext ItemContext => m_itemContext;

		public RuntimeDataSourceInfoCollection DataSources => m_dataSources;

		public string RequestUserName => m_requestUserName;

		public DateTime ExecutionTimeStamp => m_executionTimeStamp;

		public ParameterInfoCollection Parameters => m_parameters;

		public IChunkFactory CreateChunkFactory
		{
			get
			{
				return m_createChunkFactory;
			}
			set
			{
				m_createChunkFactory = value;
			}
		}

		public ReportProcessing.ExecutionType InteractiveExecution => m_interactiveExecution;

		public CultureInfo Culture => m_culture;

		public UserProfileState AllowUserProfileState => m_allowUserProfileState;

		public UserProfileState InitialUserProfileState => m_initialUserProfileState;

		public IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => m_createDataExtensionInstanceFunction;

		public CreateAndRegisterStream CreateStreamCallbackForScalability => m_createStreamCallbackForScalability;

		public ReportRuntimeSetup DataSetRuntimeSetup => m_dataSetRuntimeSetup;

		public IJobContext JobContext => m_jobContext;

		public IDataProtection DataProtection => m_dataProtection;

		public DataSetContext(string targetChunkNameInSnapshot, string cachedDataChunkName, bool mustCreateDataChunk, IRowConsumer consumerRequest, ICatalogItemContext itemContext, RuntimeDataSourceInfoCollection dataSources, string requestUserName, DateTime executionTimeStamp, ParameterInfoCollection parameters, IChunkFactory createChunkFactory, ReportProcessing.ExecutionType interactiveExecution, CultureInfo culture, UserProfileState allowUserProfileState, UserProfileState initialUserProfileState, IProcessingDataExtensionConnection createDataExtensionInstanceFunction, CreateAndRegisterStream createStreamCallbackForScalability, ReportRuntimeSetup dataSetRuntimeSetup, IJobContext jobContext, IDataProtection dataProtection)
		{
			m_targetChunkNameInSnapshot = targetChunkNameInSnapshot;
			m_cachedDataChunkName = cachedDataChunkName;
			m_mustCreateDataChunk = mustCreateDataChunk;
			m_consumerRequest = consumerRequest;
			m_itemContext = itemContext;
			m_dataSources = dataSources;
			m_requestUserName = requestUserName;
			m_executionTimeStamp = executionTimeStamp;
			m_parameters = parameters;
			m_createChunkFactory = createChunkFactory;
			m_interactiveExecution = interactiveExecution;
			m_culture = culture;
			m_allowUserProfileState = allowUserProfileState;
			m_initialUserProfileState = initialUserProfileState;
			m_createDataExtensionInstanceFunction = createDataExtensionInstanceFunction;
			m_createStreamCallbackForScalability = createStreamCallbackForScalability;
			m_dataSetRuntimeSetup = dataSetRuntimeSetup;
			m_jobContext = jobContext;
			m_dataProtection = dataProtection;
		}
	}
}
