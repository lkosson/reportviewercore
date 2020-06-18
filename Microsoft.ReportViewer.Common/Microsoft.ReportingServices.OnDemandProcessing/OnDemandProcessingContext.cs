using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandProcessingContext : IInternalProcessingContext, IStaticReferenceable
	{
		internal enum Mode
		{
			Full,
			Streaming,
			DefinitionOnly
		}

		private sealed class CommonInfo
		{
			private readonly bool m_enableDataBackedParameters;

			private readonly IChunkFactory m_chunkFactory;

			private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback m_subReportCallback;

			private readonly IGetResource m_getResourceCallback;

			private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters m_storeServerParameters;

			private readonly ReportRuntimeSetup m_reportRuntimeSetup;

			private readonly UserProfileState m_allowUserProfileState;

			private readonly string m_requestUserName;

			private readonly CultureInfo m_userLanguage;

			private readonly DateTime m_executionTime;

			private readonly bool m_reprocessSnapshot;

			private readonly bool m_processWithCachedData;

			private int m_uniqueIDCounter;

			private EventInformation m_userSortFilterInfo;

			private SortFilterEventInfoMap m_oldSortFilterEventInfo;

			private SortFilterEventInfoMap m_newSortFilterEventInfo;

			private List<IReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> m_reportRuntimeUserSortFilterInfo;

			private EventInformation.OdpSortEventInfo m_newOdpSortEventInfo;

			private string m_userSortFilterEventSourceUniqueName;

			private readonly CreateAndRegisterStream m_createStreamCallback;

			private CustomReportItemControls m_criControls;

			private readonly IJobContext m_jobContext;

			private readonly IExtensionFactory m_extFactory;

			private readonly IDataProtection m_dataProtection;

			private readonly ExecutionLogContext m_executionLogContext;

			private readonly RuntimeDataSourceInfoCollection m_dataSourceInfos;

			private readonly RuntimeDataSetInfoCollection m_sharedDataSetReferences;

			private readonly IProcessingDataExtensionConnection m_createAndSetupDataExtensionFunction;

			private readonly IConfiguration m_configuration;

			private readonly Dictionary<ProcessingErrorCode, bool> m_hasTracedOneTimeMessage;

			private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable m_globalDataSourceInfo;

			private readonly ReportProcessingContext m_externalProcessingContext;

			private AbortHelper m_abortInfo;

			private readonly bool m_abortInfoInherited;

			private uint m_languageInstanceId;

			[NonSerialized]
			private readonly object m_hasUserProfileStateLock = new object();

			private UserProfileState m_hasUserProfileState;

			private bool m_hasRenderFormatDependencyInDocumentMap;

			private readonly OnDemandProcessingContext m_topLevelContext;

			private readonly Mode m_contextMode;

			private readonly ImageCacheManager m_imageCacheManager;

			internal IGetResource GetResourceCallback => m_getResourceCallback;

			internal string RequestUserName => m_requestUserName;

			internal DateTime ExecutionTime => m_executionTime;

			internal CultureInfo UserLanguage => m_userLanguage;

			internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback SubReportCallback => m_subReportCallback;

			internal UserProfileState AllowUserProfileState => m_allowUserProfileState;

			internal bool StreamingMode => m_contextMode == Mode.Streaming;

			internal bool ReprocessSnapshot => m_reprocessSnapshot;

			internal bool ProcessWithCachedData => m_processWithCachedData;

			internal IChunkFactory ChunkFactory => m_chunkFactory;

			internal ReportRuntimeSetup ReportRuntimeSetup => m_reportRuntimeSetup;

			internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters StoreServerParameters => m_storeServerParameters;

			internal EventInformation UserSortFilterInfo
			{
				get
				{
					return m_userSortFilterInfo;
				}
				set
				{
					m_userSortFilterInfo = value;
				}
			}

			internal SortFilterEventInfoMap OldSortFilterEventInfo
			{
				get
				{
					return m_oldSortFilterEventInfo;
				}
				set
				{
					m_oldSortFilterEventInfo = value;
				}
			}

			internal SortFilterEventInfoMap NewSortFilterEventInfo
			{
				get
				{
					return m_newSortFilterEventInfo;
				}
				set
				{
					m_newSortFilterEventInfo = value;
				}
			}

			internal string UserSortFilterEventSourceUniqueName
			{
				get
				{
					return m_userSortFilterEventSourceUniqueName;
				}
				set
				{
					m_userSortFilterEventSourceUniqueName = value;
				}
			}

			internal List<IReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> ReportRuntimeUserSortFilterInfo
			{
				get
				{
					return m_reportRuntimeUserSortFilterInfo;
				}
				set
				{
					m_reportRuntimeUserSortFilterInfo = value;
				}
			}

			internal CreateAndRegisterStream CreateStreamCallback => m_createStreamCallback;

			internal ReportProcessingContext ExternalProcessingContext => m_externalProcessingContext;

			internal CustomReportItemControls CriProcessingControls
			{
				get
				{
					if (m_criControls == null)
					{
						m_criControls = new CustomReportItemControls();
					}
					return m_criControls;
				}
				set
				{
					m_criControls = value;
				}
			}

			internal bool EnableDataBackedParameters => m_enableDataBackedParameters;

			internal IJobContext JobContext => m_jobContext;

			internal IExtensionFactory ExtFactory => m_extFactory;

			internal IDataProtection DataProtection => m_dataProtection;

			internal ExecutionLogContext ExecutionLogContext => m_executionLogContext;

			internal RuntimeDataSourceInfoCollection DataSourceInfos => m_dataSourceInfos;

			internal RuntimeDataSetInfoCollection SharedDataSetReferences => m_sharedDataSetReferences;

			internal IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => m_createAndSetupDataExtensionFunction;

			internal IConfiguration Configuration => m_configuration;

			internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable GlobalDataSourceInfo => m_globalDataSourceInfo;

			internal AbortHelper AbortInfo => m_abortInfo;

			internal uint LanguageInstanceId
			{
				get
				{
					return m_languageInstanceId;
				}
				set
				{
					m_languageInstanceId = value;
				}
			}

			internal UserProfileState HasUserProfileState => m_hasUserProfileState;

			internal bool HasRenderFormatDependencyInDocumentMap
			{
				get
				{
					return m_hasRenderFormatDependencyInDocumentMap;
				}
				set
				{
					m_hasRenderFormatDependencyInDocumentMap = value;
				}
			}

			internal OnDemandProcessingContext TopLevelContext => m_topLevelContext;

			internal Mode ContextMode => m_contextMode;

			internal ImageCacheManager ImageCacheManager => m_imageCacheManager;

			internal CommonInfo(IChunkFactory chunkFactory, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceCallback, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, ReportRuntimeSetup reportRuntimeSetup, UserProfileState allowUserProfileState, string requestUserName, CultureInfo userLanguage, DateTime executionTime, bool reprocessSnapshot, bool processWithCachedData, CreateAndRegisterStream createStreamCallback, bool enableDataBackedParameters, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection, ExecutionLogContext executionLogContext, RuntimeDataSourceInfoCollection dataSourceInfos, RuntimeDataSetInfoCollection sharedDataSetReferences, IProcessingDataExtensionConnection createAndSetupDataExtensionFunction, IConfiguration configuration, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable globalDataSourceInfo, ReportProcessingContext externalProcessingContext, AbortHelper abortInfo, bool abortInfoInherited, UserProfileState hasUserProfileState, OnDemandProcessingContext topLevelContext, Mode contextMode, ImageCacheManager imageCacheManager)
			{
				m_chunkFactory = chunkFactory;
				m_subReportCallback = subReportCallback;
				m_getResourceCallback = getResourceCallback;
				m_storeServerParameters = storeServerParameters;
				m_reportRuntimeSetup = reportRuntimeSetup;
				m_allowUserProfileState = allowUserProfileState;
				m_requestUserName = requestUserName;
				m_userLanguage = userLanguage;
				m_executionTime = executionTime;
				m_reprocessSnapshot = reprocessSnapshot;
				m_processWithCachedData = processWithCachedData;
				m_createStreamCallback = createStreamCallback;
				m_enableDataBackedParameters = enableDataBackedParameters;
				m_jobContext = jobContext;
				m_extFactory = extFactory;
				m_dataProtection = dataProtection;
				m_executionLogContext = executionLogContext;
				m_dataSourceInfos = dataSourceInfos;
				m_sharedDataSetReferences = sharedDataSetReferences;
				m_createAndSetupDataExtensionFunction = createAndSetupDataExtensionFunction;
				m_configuration = configuration;
				m_hasTracedOneTimeMessage = new Dictionary<ProcessingErrorCode, bool>();
				m_globalDataSourceInfo = globalDataSourceInfo;
				m_externalProcessingContext = externalProcessingContext;
				m_abortInfo = abortInfo;
				m_abortInfoInherited = abortInfoInherited;
				m_hasUserProfileState = hasUserProfileState;
				m_topLevelContext = topLevelContext;
				m_contextMode = contextMode;
				m_imageCacheManager = imageCacheManager;
			}

			internal void MergeHasUserProfileState(UserProfileState newProfileStateFlags)
			{
				lock (m_hasUserProfileStateLock)
				{
					m_hasUserProfileState |= newProfileStateFlags;
				}
			}

			internal void UnregisterAbortInfo()
			{
				if (m_abortInfo != null && !m_abortInfoInherited)
				{
					m_abortInfo.Dispose();
					m_abortInfo = null;
				}
			}

			internal int CreateUniqueID()
			{
				return ++m_uniqueIDCounter;
			}

			internal EventInformation GetUserSortFilterInformation(out string sourceUniqueName)
			{
				sourceUniqueName = m_userSortFilterEventSourceUniqueName;
				if (m_newOdpSortEventInfo == null)
				{
					return null;
				}
				return new EventInformation
				{
					OdpSortInfo = m_newOdpSortEventInfo
				};
			}

			internal void MergeNewUserSortFilterInformation()
			{
				int num = (m_reportRuntimeUserSortFilterInfo != null) ? m_reportRuntimeUserSortFilterInfo.Count : 0;
				if (num == 0)
				{
					return;
				}
				if (m_newOdpSortEventInfo == null)
				{
					m_newOdpSortEventInfo = new EventInformation.OdpSortEventInfo();
				}
				for (int i = 0; i < num; i++)
				{
					IReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo> reference = m_reportRuntimeUserSortFilterInfo[i];
					using (reference.PinValue())
					{
						Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (runtimeSortFilterEventInfo.NewUniqueName == null)
						{
							runtimeSortFilterEventInfo.NewUniqueName = runtimeSortFilterEventInfo.OldUniqueName;
						}
						Hashtable hashtable = null;
						if (runtimeSortFilterEventInfo.PeerSortFilters != null)
						{
							int count = runtimeSortFilterEventInfo.PeerSortFilters.Count;
							if (count > 0)
							{
								hashtable = new Hashtable(count);
								IDictionaryEnumerator enumerator = runtimeSortFilterEventInfo.PeerSortFilters.GetEnumerator();
								while (enumerator.MoveNext())
								{
									if (enumerator.Value != null)
									{
										hashtable.Add(enumerator.Value, null);
									}
								}
							}
						}
						m_newOdpSortEventInfo.Add(runtimeSortFilterEventInfo.NewUniqueName, runtimeSortFilterEventInfo.SortDirection, hashtable);
						if (runtimeSortFilterEventInfo.OldUniqueName == m_userSortFilterEventSourceUniqueName)
						{
							m_userSortFilterEventSourceUniqueName = runtimeSortFilterEventInfo.NewUniqueName;
						}
					}
				}
				m_reportRuntimeUserSortFilterInfo = null;
			}

			internal void TraceOneTimeWarning(ProcessingErrorCode errorCode, ICatalogItemContext itemContext)
			{
				if (m_hasTracedOneTimeMessage != null && !m_hasTracedOneTimeMessage.ContainsKey(errorCode))
				{
					string text = itemContext.ItemPathAsString.MarkAsPrivate();
					switch (errorCode)
					{
					case ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength:
						Global.Tracer.Trace(TraceLevel.Info, "RDL Sandboxing: Item: '{0}' attempted to use an array that violated the maximum allowed length.", text);
						break;
					case ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength:
						Global.Tracer.Trace(TraceLevel.Info, "RDL Sandboxing: Item: '{0}' attempted to use a String that violated the maximum allowed length.", text);
						break;
					case ProcessingErrorCode.rsSandboxingExternalResourceExceedsMaximumSize:
						Global.Tracer.Trace(TraceLevel.Info, "RDL Sandboxing: Item: '{0}' attempted to reference an external resource larger than the maximum allowed size.", text);
						break;
					case ProcessingErrorCode.rsRenderingChunksUnavailable:
						Global.Tracer.Trace(TraceLevel.Info, "A rendering extension attempted to use Report.GetOrCreateChunk or Report.CreateChunk while rendering item '{0}'. Rendering chunks are not available using the current report execution method.", text);
						break;
					default:
						Global.Tracer.Assert(false, "Invalid error code: '{0}'.  Expected an error code", errorCode);
						break;
					}
					m_hasTracedOneTimeMessage[errorCode] = true;
				}
			}
		}

		internal sealed class CustomReportItemControls
		{
			private class CustomControlInfo
			{
				private bool m_valid;

				private ICustomReportItem m_instance;

				internal bool IsValid
				{
					get
					{
						return m_valid;
					}
					set
					{
						m_valid = value;
					}
				}

				internal ICustomReportItem Instance
				{
					get
					{
						return m_instance;
					}
					set
					{
						m_instance = value;
					}
				}
			}

			private Hashtable m_controls;

			internal CustomReportItemControls()
			{
				m_controls = new Hashtable();
			}

			internal ICustomReportItem GetControlInstance(string name, IExtensionFactory extFactory)
			{
				lock (this)
				{
					CustomControlInfo customControlInfo = m_controls[name] as CustomControlInfo;
					if (customControlInfo == null)
					{
						ICustomReportItem customReportItem = null;
						Global.Tracer.Assert(extFactory != null, "extFactory != null");
						customReportItem = (extFactory.GetNewCustomReportItemProcessingInstanceClass(name) as ICustomReportItem);
						customControlInfo = new CustomControlInfo();
						customControlInfo.Instance = customReportItem;
						customControlInfo.IsValid = (customReportItem != null);
						m_controls.Add(name, customControlInfo);
					}
					Global.Tracer.Assert(customControlInfo != null, "(null != info)");
					if (customControlInfo.IsValid)
					{
						return customControlInfo.Instance;
					}
					return null;
				}
			}
		}

		private readonly DataSetContext m_externalDataSetContext;

		private readonly OnDemandProcessingContext m_parentContext;

		private readonly CommonInfo m_commonInfo;

		private readonly ICatalogItemContext m_catalogItemContext;

		private ObjectModelImpl m_reportObjectModel;

		private readonly bool m_reportItemsReferenced;

		private bool m_reportItemThisDotValueReferenced;

		private readonly Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> m_embeddedImages;

		private bool m_processReportParameters;

		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRuntime;

		private ParameterInfoCollection m_reportParameters;

		private readonly ErrorContext m_errorContext;

		private bool m_snapshotProcessing;

		private CultureInfo m_threadCulture;

		private CompareInfo m_compareInfo = Thread.CurrentThread.CurrentCulture.CompareInfo;

		private CompareOptions m_clrCompareOptions;

		private bool m_nullsAsBlanks;

		private bool m_useOrdinalStringKeyGeneration;

		private IDataComparer m_processingComparer;

		private StringKeyGenerator m_stringKeyGenerator;

		private readonly bool m_inSubreport;

		private readonly bool m_inSubreportInDataRegion;

		private bool m_isTablixProcessingMode;

		private bool m_isTopLevelSubReportProcessing;

		private bool m_isUnrestrictedRenderFormatReferenceMode;

		private readonly bool m_isSharedDataSetExecutionOnly;

		private readonly Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> m_reportAggregates = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>();

		private bool m_errorSavingSnapshotData;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.Report m_reportDefinition;

		private readonly OnDemandMetadata m_odpMetadata;

		private bool m_hasBookmarks;

		private bool m_hasShowHide;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance m_currentReportInstance;

		private int m_currentDataSetIndex = -1;

		private List<object> m_groupExprValues = new List<object>();

		private bool m_peerOuterGroupProcessing;

		private string m_subReportInstanceOrSharedDatasetUniqueName;

		private bool m_foundExistingSubReportInstance;

		private string m_subReportDataChunkNameModifier;

		private SubReportInfo m_subReportInfo;

		private readonly bool m_specialRecursiveAggregates;

		private SecondPassOperations m_secondPassOperation;

		private List<Filters> m_specialDataRegionFilters;

		private List<IReference<IDataRowSortOwner>> m_dataRowSortOwners;

		private readonly Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext m_userSortFilterContext;

		private bool m_initializedRuntime;

		private readonly bool m_isPageHeaderFooter;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader[] m_dataSetToDataReader;

		private bool[] m_dataSetRetrievalComplete;

		private IScalabilityCache m_tablixProcessingScalabilityCache;

		private CommonRowCache m_tablixProcessingLookupRowCache;

		private int m_staticRefId = int.MaxValue;

		private DomainScopeContext m_domainScopeContext;

		private readonly OnDemandStateManager m_stateManager;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance m_currentDataSetInstance;

		internal UserProfileState HasUserProfileState => m_commonInfo.HasUserProfileState;

		internal bool HasRenderFormatDependencyInDocumentMap
		{
			get
			{
				return m_commonInfo.HasRenderFormatDependencyInDocumentMap;
			}
			set
			{
				m_commonInfo.HasRenderFormatDependencyInDocumentMap = value;
			}
		}

		internal ExecutionLogContext ExecutionLogContext => m_commonInfo.ExecutionLogContext;

		internal IJobContext JobContext => m_commonInfo.JobContext;

		internal IExtensionFactory ExtFactory => m_commonInfo.ExtFactory;

		internal IDataProtection DataProtection => m_commonInfo.DataProtection;

		public bool EnableDataBackedParameters => m_commonInfo.EnableDataBackedParameters;

		internal IChunkFactory ChunkFactory => m_commonInfo.ChunkFactory;

		internal string RequestUserName => m_commonInfo.RequestUserName;

		public DateTime ExecutionTime => m_commonInfo.ExecutionTime;

		internal CultureInfo UserLanguage => m_commonInfo.UserLanguage;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback SubReportCallback => m_commonInfo.SubReportCallback;

		internal bool HasBookmarks
		{
			get
			{
				return m_hasBookmarks;
			}
			set
			{
				if (m_parentContext != null)
				{
					m_parentContext.HasBookmarks |= value;
				}
				else if (!SnapshotProcessing || m_commonInfo.ReprocessSnapshot)
				{
					m_odpMetadata.ReportSnapshot.HasBookmarks |= value;
				}
				m_hasBookmarks |= value;
			}
		}

		internal bool HasShowHide
		{
			get
			{
				return m_hasShowHide;
			}
			set
			{
				if (m_parentContext != null)
				{
					m_parentContext.HasShowHide |= value;
				}
				else if (!SnapshotProcessing || m_commonInfo.ReprocessSnapshot)
				{
					m_odpMetadata.ReportSnapshot.HasShowHide |= value;
				}
				m_hasShowHide |= value;
			}
		}

		internal bool HasUserSortFilter
		{
			get
			{
				if (m_reportDefinition == null)
				{
					return false;
				}
				return m_reportDefinition.ReportOrDescendentHasUserSortFilter;
			}
		}

		internal UserProfileState AllowUserProfileState => m_commonInfo.AllowUserProfileState;

		public bool SnapshotProcessing
		{
			get
			{
				return m_snapshotProcessing;
			}
			set
			{
				m_snapshotProcessing = value;
			}
		}

		public bool ReprocessSnapshot => m_commonInfo.ReprocessSnapshot;

		internal bool ProcessWithCachedData => m_commonInfo.ProcessWithCachedData;

		internal bool StreamingMode => m_commonInfo.StreamingMode;

		internal bool UseVerboseExecutionLogging
		{
			get
			{
				if (JobContext != null && JobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
				{
					return m_commonInfo.StreamingMode;
				}
				return false;
			}
		}

		internal bool ShouldExecuteLiveQueries
		{
			get
			{
				if (!StreamingMode)
				{
					if (!SnapshotProcessing)
					{
						return !ReprocessSnapshot;
					}
					return false;
				}
				return true;
			}
		}

		internal ReportRuntimeSetup ReportRuntimeSetup => m_commonInfo.ReportRuntimeSetup;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters StoreServerParameters => m_commonInfo.StoreServerParameters;

		internal bool HasPreviousAggregates
		{
			get
			{
				if (m_reportDefinition == null)
				{
					return false;
				}
				return m_reportDefinition.HasPreviousAggregates;
			}
		}

		internal EventInformation UserSortFilterInfo
		{
			get
			{
				return m_commonInfo.UserSortFilterInfo;
			}
			set
			{
				m_commonInfo.UserSortFilterInfo = value;
			}
		}

		internal SortFilterEventInfoMap OldSortFilterEventInfo
		{
			get
			{
				return m_commonInfo.OldSortFilterEventInfo;
			}
			set
			{
				m_commonInfo.OldSortFilterEventInfo = value;
			}
		}

		internal string UserSortFilterEventSourceUniqueName
		{
			get
			{
				return m_commonInfo.UserSortFilterEventSourceUniqueName;
			}
			set
			{
				m_commonInfo.UserSortFilterEventSourceUniqueName = value;
			}
		}

		internal SortFilterEventInfoMap NewSortFilterEventInfo
		{
			get
			{
				return m_commonInfo.NewSortFilterEventInfo;
			}
			set
			{
				m_commonInfo.NewSortFilterEventInfo = value;
			}
		}

		internal List<IReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> ReportRuntimeUserSortFilterInfo
		{
			get
			{
				return m_commonInfo.ReportRuntimeUserSortFilterInfo;
			}
			set
			{
				m_commonInfo.ReportRuntimeUserSortFilterInfo = value;
			}
		}

		internal CreateAndRegisterStream CreateStreamCallback => m_commonInfo.CreateStreamCallback;

		internal bool IsPageHeaderFooter => m_isPageHeaderFooter;

		internal Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> ReportAggregates => m_reportAggregates;

		internal OnDemandStateManager StateManager => m_stateManager;

		internal ReportProcessingContext ExternalProcessingContext => m_commonInfo.ExternalProcessingContext;

		internal DataSetContext ExternalDataSetContext => m_externalDataSetContext;

		internal bool ErrorSavingSnapshotData
		{
			get
			{
				return m_errorSavingSnapshotData;
			}
			set
			{
				m_errorSavingSnapshotData = value;
			}
		}

		internal OnDemandProcessingContext ParentContext => m_parentContext;

		internal OnDemandProcessingContext TopLevelContext => m_commonInfo.TopLevelContext;

		internal RuntimeDataSourceInfoCollection DataSourceInfos => m_commonInfo.DataSourceInfos;

		internal RuntimeDataSetInfoCollection SharedDataSetReferences => m_commonInfo.SharedDataSetReferences;

		internal IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => m_commonInfo.CreateAndSetupDataExtensionFunction;

		internal CultureInfo ThreadCulture
		{
			get
			{
				return m_threadCulture;
			}
			set
			{
				m_threadCulture = value;
			}
		}

		internal uint LanguageInstanceId
		{
			get
			{
				return m_commonInfo.LanguageInstanceId;
			}
			set
			{
				m_commonInfo.LanguageInstanceId = value;
			}
		}

		internal ICatalogItemContext ReportContext => m_catalogItemContext;

		internal Microsoft.ReportingServices.RdlExpressions.ReportRuntime ReportRuntime
		{
			get
			{
				return m_reportRuntime;
			}
			set
			{
				m_reportRuntime = value;
			}
		}

		internal ObjectModelImpl ReportObjectModel => m_reportObjectModel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Report ReportDefinition => m_reportDefinition;

		internal OnDemandMetadata OdpMetadata => m_odpMetadata;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance CurrentReportInstance
		{
			get
			{
				return m_currentReportInstance;
			}
			set
			{
				m_currentReportInstance = value;
			}
		}

		internal int CurrentDataSetIndex
		{
			get
			{
				if (m_reportObjectModel.CurrentFields == null || m_reportObjectModel.CurrentFields.DataSet == null)
				{
					return -1;
				}
				return m_currentDataSetIndex;
			}
		}

		internal ImageCacheManager ImageCacheManager => m_commonInfo.ImageCacheManager;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance CurrentOdpDataSetInstance => m_currentDataSetInstance;

		internal IReportScope CurrentReportScope => m_stateManager.LastROMInstance?.ReportScope;

		internal List<object> GroupExpressionValues => m_groupExprValues;

		internal bool PeerOuterGroupProcessing
		{
			get
			{
				return m_peerOuterGroupProcessing;
			}
			set
			{
				m_peerOuterGroupProcessing = value;
			}
		}

		internal bool ReportItemsReferenced => m_reportItemsReferenced;

		internal bool ReportItemThisDotValueReferenced => m_reportItemThisDotValueReferenced;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable GlobalDataSourceInfo => m_commonInfo.GlobalDataSourceInfo;

		internal Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> EmbeddedImages => m_embeddedImages;

		public ErrorContext ErrorContext => m_errorContext;

		internal bool ProcessReportParameters
		{
			get
			{
				return m_processReportParameters;
			}
			set
			{
				m_processReportParameters = value;
			}
		}

		internal CompareInfo CompareInfo => m_compareInfo;

		internal CompareOptions ClrCompareOptions
		{
			get
			{
				return m_clrCompareOptions;
			}
			set
			{
				SetComparisonInformation(m_compareInfo, value, m_nullsAsBlanks, m_useOrdinalStringKeyGeneration);
			}
		}

		internal bool NullsAsBlanks => m_nullsAsBlanks;

		internal bool UseOrdinalStringKeyGeneration => m_useOrdinalStringKeyGeneration;

		internal IDataComparer ProcessingComparer
		{
			get
			{
				if (m_processingComparer == null)
				{
					if (m_commonInfo.StreamingMode)
					{
						m_processingComparer = new CommonDataComparer(m_compareInfo, m_clrCompareOptions, m_nullsAsBlanks);
					}
					else
					{
						m_processingComparer = new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingComparer(m_compareInfo, m_clrCompareOptions, m_nullsAsBlanks);
					}
				}
				return m_processingComparer;
			}
		}

		internal StringKeyGenerator StringKeyGenerator
		{
			get
			{
				if (m_stringKeyGenerator == null)
				{
					m_stringKeyGenerator = new StringKeyGenerator(m_compareInfo, m_clrCompareOptions, m_nullsAsBlanks, m_useOrdinalStringKeyGeneration);
				}
				return m_stringKeyGenerator;
			}
		}

		internal IEqualityComparer<object> EqualityComparer => ProcessingComparer;

		internal string SubReportDataChunkNameModifier => m_subReportDataChunkNameModifier;

		internal string ProcessingAbortItemUniqueIdentifier
		{
			get
			{
				if (m_inSubreport || m_isSharedDataSetExecutionOnly)
				{
					return m_subReportInstanceOrSharedDatasetUniqueName;
				}
				return null;
			}
		}

		internal bool FoundExistingSubReportInstance => m_foundExistingSubReportInstance;

		internal string SubReportUniqueName
		{
			get
			{
				if (!m_inSubreport || m_subReportInfo == null)
				{
					return null;
				}
				return m_subReportInfo.UniqueName;
			}
		}

		internal string ReportFolder
		{
			get
			{
				if (m_inSubreport)
				{
					string text = m_subReportInfo.CommonSubReportInfo.OriginalCatalogPath;
					if (!string.IsNullOrEmpty(text))
					{
						int num = text.LastIndexOf('/');
						if (num > 0)
						{
							return text.Substring(0, num);
						}
					}
					else
					{
						text = "/";
					}
					return text;
				}
				return m_catalogItemContext.ParentPath;
			}
		}

		internal bool InSubreport => m_inSubreport;

		internal bool InSubreportInDataRegion => m_inSubreportInDataRegion;

		internal AbortHelper AbortInfo => m_commonInfo.AbortInfo;

		internal SecondPassOperations SecondPassOperation
		{
			get
			{
				return m_secondPassOperation;
			}
			set
			{
				m_secondPassOperation = value;
			}
		}

		internal bool SpecialRecursiveAggregates => m_specialRecursiveAggregates;

		internal bool InitializedRuntime
		{
			get
			{
				return m_initializedRuntime;
			}
			set
			{
				m_initializedRuntime = value;
			}
		}

		internal bool[] DataSetRetrievalComplete => m_dataSetRetrievalComplete;

		internal ParameterInfoCollection ReportParameters
		{
			get
			{
				return m_reportParameters;
			}
			set
			{
				m_reportParameters = value;
			}
		}

		internal IScalabilityCache TablixProcessingScalabilityCache => m_tablixProcessingScalabilityCache;

		internal CommonRowCache TablixProcessingLookupRowCache
		{
			get
			{
				return m_tablixProcessingLookupRowCache;
			}
			set
			{
				m_tablixProcessingLookupRowCache = value;
			}
		}

		internal CustomReportItemControls CriProcessingControls
		{
			get
			{
				return m_commonInfo.CriProcessingControls;
			}
			set
			{
				m_commonInfo.CriProcessingControls = value;
			}
		}

		internal IConfiguration Configuration => m_commonInfo.Configuration;

		internal DomainScopeContext DomainScopeContext
		{
			get
			{
				return m_domainScopeContext;
			}
			set
			{
				m_domainScopeContext = value;
			}
		}

		internal Mode ContextMode => m_commonInfo.ContextMode;

		internal bool ProhibitSerializableValues
		{
			get
			{
				if (Configuration != null)
				{
					return Configuration.ProhibitSerializableValues;
				}
				return false;
			}
		}

		internal bool IsTablixProcessingMode
		{
			get
			{
				return m_isTablixProcessingMode;
			}
			set
			{
				m_isTablixProcessingMode = value;
				m_stateManager.ResetOnDemandState();
			}
		}

		internal bool IsUnrestrictedRenderFormatReferenceMode
		{
			get
			{
				return m_isUnrestrictedRenderFormatReferenceMode;
			}
			set
			{
				m_isUnrestrictedRenderFormatReferenceMode = value;
			}
		}

		internal bool IsTopLevelSubReportProcessing
		{
			get
			{
				return m_isTopLevelSubReportProcessing;
			}
			set
			{
				m_isTopLevelSubReportProcessing = value;
			}
		}

		internal bool IsSharedDataSetExecutionOnly => m_isSharedDataSetExecutionOnly;

		internal IInstancePath LastRIFObject
		{
			get
			{
				return m_stateManager.LastRIFObject;
			}
			set
			{
				m_stateManager.LastRIFObject = value;
			}
		}

		internal QueryRestartInfo QueryRestartInfo => m_stateManager.QueryRestartInfo;

		internal IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				return m_stateManager.LastTablixProcessingReportScope;
			}
			set
			{
				m_stateManager.LastTablixProcessingReportScope = value;
			}
		}

		internal Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext UserSortFilterContext => m_userSortFilterContext;

		internal List<IReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> RuntimeSortFilterInfo => m_userSortFilterContext.RuntimeSortFilterInfo;

		int IStaticReferenceable.ID => m_staticRefId;

		internal OnDemandProcessingContext(ProcessingContext externalProcessingContext, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandMetadata odpMetadata, ErrorContext errorContext, DateTime executionTime, bool snapshotProcessing, bool reprocessSnapshot, bool processWithCachedData, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, UserProfileState userProfileState, ExecutionLogContext executionLogContext, IConfiguration configuration, Mode contextMode, IAbortHelper abortHelper)
		{
			_ = externalProcessingContext.JobContext;
			AbortHelper abortHelper2 = null;
			bool abortInfoInherited = false;
			abortHelper2 = (abortHelper as AbortHelper);
			if (abortHelper2 == null)
			{
				if (!snapshotProcessing && !reprocessSnapshot)
				{
					abortHelper2 = new ReportAbortHelper(externalProcessingContext.JobContext, contextMode == Mode.Streaming);
				}
			}
			else
			{
				abortInfoInherited = true;
			}
			m_commonInfo = new CommonInfo(externalProcessingContext.ChunkFactory, externalProcessingContext.OnDemandSubReportCallback, externalProcessingContext.GetResourceCallback, storeServerParameters, externalProcessingContext.ReportRuntimeSetup, externalProcessingContext.AllowUserProfileState, externalProcessingContext.RequestUserName, externalProcessingContext.UserLanguage, executionTime, reprocessSnapshot, processWithCachedData, externalProcessingContext.CreateStreamCallback, externalProcessingContext.EnableDataBackedParameters, externalProcessingContext.JobContext, externalProcessingContext.ExtFactory, externalProcessingContext.DataProtection, executionLogContext, externalProcessingContext.DataSources, externalProcessingContext.SharedDataSetReferences, externalProcessingContext.CreateAndSetupDataExtensionFunction, configuration, new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable(), externalProcessingContext as ReportProcessingContext, abortHelper2, abortInfoInherited, userProfileState, this, contextMode, CreateImageCacheManager(contextMode, odpMetadata, externalProcessingContext.ChunkFactory));
			m_errorContext = errorContext;
			m_snapshotProcessing = snapshotProcessing;
			m_catalogItemContext = externalProcessingContext.ReportContext;
			m_reportDefinition = report;
			m_odpMetadata = odpMetadata;
			m_parentContext = null;
			m_reportItemsReferenced = report.HasReportItemReferences;
			m_reportItemThisDotValueReferenced = false;
			m_embeddedImages = report.EmbeddedImages;
			m_processReportParameters = false;
			m_reportRuntime = null;
			m_inSubreport = false;
			m_inSubreportInDataRegion = false;
			m_isSharedDataSetExecutionOnly = false;
			m_externalDataSetContext = null;
			m_stateManager = CreateStateManager(contextMode);
			m_reportObjectModel = new ObjectModelImpl(this);
			if (contextMode != Mode.DefinitionOnly)
			{
				m_specialRecursiveAggregates = report.HasSpecialRecursiveAggregates;
				m_userSortFilterContext = new Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
				InitializeDataSetMembers(report.MappingNameToDataSet.Count);
			}
			InitFlags(report);
			m_odpMetadata.OdpContexts.Add(this);
		}

		internal OnDemandProcessingContext(ProcessingContext externalProcessingContext, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandMetadata odpMetadata, ErrorContext errorContext, DateTime executionTime, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, UserProfileState userProfileState, ExecutionLogContext executionLogContext, IConfiguration configuration, IAbortHelper abortHelper)
			: this(externalProcessingContext, report, odpMetadata, errorContext, executionTime, snapshotProcessing: false, reprocessSnapshot: false, processWithCachedData: false, storeServerParameters, userProfileState, executionLogContext, configuration, Mode.DefinitionOnly, abortHelper)
		{
		}

		internal OnDemandProcessingContext(OnDemandProcessingContext aContext, bool aReportItemsReferenced, Microsoft.ReportingServices.ReportIntermediateFormat.Report aReport)
		{
			m_isPageHeaderFooter = true;
			m_reportDefinition = aReport;
			m_parentContext = aContext;
			m_odpMetadata = aContext.OdpMetadata;
			m_commonInfo = aContext.m_commonInfo;
			m_errorContext = aContext.ErrorContext;
			m_inSubreport = aContext.m_inSubreport;
			m_inSubreportInDataRegion = aContext.m_inSubreportInDataRegion;
			m_isSharedDataSetExecutionOnly = false;
			m_externalDataSetContext = null;
			m_snapshotProcessing = aContext.m_snapshotProcessing;
			m_catalogItemContext = aContext.m_catalogItemContext;
			m_reportItemsReferenced = aReportItemsReferenced;
			m_reportItemThisDotValueReferenced = false;
			m_embeddedImages = aContext.m_embeddedImages;
			m_processReportParameters = false;
			m_initializedRuntime = false;
			m_specialRecursiveAggregates = false;
			m_userSortFilterContext = new Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
			m_threadCulture = aContext.m_threadCulture;
			m_compareInfo = aContext.m_compareInfo;
			m_clrCompareOptions = aContext.m_clrCompareOptions;
			m_stateManager = CreateStateManager(m_commonInfo.ContextMode);
			if (m_commonInfo.ContextMode != Mode.DefinitionOnly)
			{
				m_reportObjectModel = new ObjectModelImpl(aContext.ReportObjectModel, this);
				m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.OnDemandExpressions);
				m_reportRuntime = new Microsoft.ReportingServices.RdlExpressions.ReportRuntime(aReport.ObjectType, m_reportObjectModel, ErrorContext);
				m_reportRuntime.LoadCompiledCode(aReport, includeParameters: false, parametersOnly: false, m_reportObjectModel, ReportRuntimeSetup);
				m_reportRuntime.CustomCodeOnInit(aReport);
			}
			m_isUnrestrictedRenderFormatReferenceMode = true;
			m_odpMetadata.OdpContexts.Add(this);
		}

		internal OnDemandProcessingContext(ProcessingContext originalProcessingContext, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, DateTime executionTime, bool snapshotProcessing, IConfiguration configuration)
		{
			m_commonInfo = new CommonInfo(null, null, null, null, originalProcessingContext.ReportRuntimeSetup, originalProcessingContext.AllowUserProfileState, originalProcessingContext.RequestUserName, originalProcessingContext.UserLanguage, executionTime, reprocessSnapshot: false, processWithCachedData: false, originalProcessingContext.CreateStreamCallback, originalProcessingContext.EnableDataBackedParameters, originalProcessingContext.JobContext, originalProcessingContext.ExtFactory, originalProcessingContext.DataProtection, new ExecutionLogContext(originalProcessingContext.JobContext), originalProcessingContext.DataSources, originalProcessingContext.SharedDataSetReferences, originalProcessingContext.CreateAndSetupDataExtensionFunction, configuration, new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable(), originalProcessingContext as ReportProcessingContext, new ReportAbortHelper(originalProcessingContext.JobContext, enforceSingleAbortException: false), abortInfoInherited: false, UserProfileState.None, this, Mode.Full, null);
			m_errorContext = errorContext;
			m_snapshotProcessing = snapshotProcessing;
			m_catalogItemContext = originalProcessingContext.ReportContext;
			m_reportDefinition = report;
			m_odpMetadata = null;
			m_reportItemsReferenced = false;
			m_reportItemThisDotValueReferenced = false;
			m_embeddedImages = null;
			m_processReportParameters = true;
			m_initializedRuntime = false;
			m_specialRecursiveAggregates = false;
			m_userSortFilterContext = new Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
			m_inSubreport = false;
			m_inSubreportInDataRegion = false;
			m_isSharedDataSetExecutionOnly = false;
			m_externalDataSetContext = null;
			m_stateManager = new OnDemandStateManagerFull(this);
			if (report != null)
			{
				m_reportObjectModel = new ObjectModelImpl(this);
				m_reportObjectModel.Initialize(report, null);
				m_reportRuntime = new Microsoft.ReportingServices.RdlExpressions.ReportRuntime(report.ObjectType, m_reportObjectModel, ErrorContext);
				m_reportRuntime.LoadCompiledCode(report, includeParameters: true, parametersOnly: true, m_reportObjectModel, ReportRuntimeSetup);
				m_reportRuntime.CustomCodeOnInit(report);
			}
		}

		internal OnDemandProcessingContext(DataSetContext dc, DataSetDefinition dataSetDefinition, ErrorContext errorContext, IConfiguration configuration)
		{
			m_externalDataSetContext = dc;
			AbortHelper abortHelper = dc.JobContext.GetAbortHelper() as AbortHelper;
			bool abortInfoInherited;
			if (abortHelper != null)
			{
				abortInfoInherited = true;
			}
			else
			{
				abortHelper = new ReportAbortHelper(dc.JobContext, enforceSingleAbortException: false);
				abortInfoInherited = false;
			}
			m_commonInfo = new CommonInfo(dc.CreateChunkFactory, null, null, null, dc.DataSetRuntimeSetup, dc.AllowUserProfileState, dc.RequestUserName, dc.Culture, dc.ExecutionTimeStamp, reprocessSnapshot: false, processWithCachedData: false, dc.CreateStreamCallbackForScalability, enableDataBackedParameters: false, dc.JobContext, null, dc.DataProtection, new ExecutionLogContext(dc.JobContext), dc.DataSources, null, dc.CreateAndSetupDataExtensionFunction, configuration, new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable(), null, abortHelper, abortInfoInherited, UserProfileState.None, this, Mode.Full, null);
			m_errorContext = errorContext;
			m_snapshotProcessing = false;
			m_isSharedDataSetExecutionOnly = true;
			m_catalogItemContext = dc.ItemContext;
			m_odpMetadata = null;
			m_reportItemsReferenced = false;
			m_reportItemThisDotValueReferenced = false;
			m_embeddedImages = null;
			m_processReportParameters = false;
			m_initializedRuntime = false;
			m_specialRecursiveAggregates = false;
			m_userSortFilterContext = new Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
			m_inSubreport = false;
			m_inSubreportInDataRegion = false;
			m_stateManager = new OnDemandStateManagerFull(this);
			m_reportObjectModel = new ObjectModelImpl(this);
			m_reportObjectModel.Initialize(dataSetDefinition);
			IExpressionHostAssemblyHolder dataSetCore = dataSetDefinition.DataSetCore;
			m_reportRuntime = new Microsoft.ReportingServices.RdlExpressions.ReportRuntime(dataSetCore.ObjectType, m_reportObjectModel, ErrorContext);
			m_reportRuntime.LoadCompiledCode(dataSetCore, includeParameters: true, parametersOnly: true, m_reportObjectModel, ReportRuntimeSetup);
		}

		internal OnDemandProcessingContext(OnDemandProcessingContext aContext, ICatalogItemContext reportContext, Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			m_parentContext = aContext;
			m_snapshotProcessing = aContext.SnapshotProcessing;
			m_reportDefinition = subReport.Report;
			m_odpMetadata = aContext.OdpMetadata;
			m_inSubreport = true;
			m_inSubreportInDataRegion = (aContext.InSubreportInDataRegion | subReport.InDataRegion);
			m_processReportParameters = false;
			m_isSharedDataSetExecutionOnly = false;
			m_initializedRuntime = false;
			m_catalogItemContext = reportContext;
			m_externalDataSetContext = null;
			m_errorContext = new ProcessingErrorContext();
			if (subReport.Report != null)
			{
				m_subReportInfo = m_odpMetadata.GetSubReportInfo(aContext.InSubreport, subReport.SubReportDefinitionPath, subReport.ReportName);
				_ = m_subReportInfo.LastID;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report = subReport.Report;
			m_commonInfo = aContext.m_commonInfo;
			m_stateManager = CreateStateManager(m_commonInfo.ContextMode);
			m_subReportInstanceOrSharedDatasetUniqueName = null;
			m_reportItemThisDotValueReferenced = aContext.m_reportItemThisDotValueReferenced;
			if (m_commonInfo.ContextMode != Mode.DefinitionOnly)
			{
				if (report != null)
				{
					m_reportItemsReferenced = report.HasReportItemReferences;
					m_embeddedImages = report.EmbeddedImages;
					InitializeDataSetMembers(report.MappingNameToDataSet.Count);
				}
				else
				{
					m_reportItemsReferenced = false;
					m_embeddedImages = null;
					InitializeDataSetMembers(-1);
				}
				m_reportObjectModel = new ObjectModelImpl(this);
				m_reportRuntime = null;
				m_userSortFilterContext = new Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext(aContext.UserSortFilterContext, subReport);
			}
			m_compareInfo = aContext.m_compareInfo;
			m_clrCompareOptions = aContext.m_clrCompareOptions;
			m_threadCulture = aContext.m_threadCulture;
			InitFlags(report);
			m_odpMetadata.OdpContexts.Add(this);
		}

		private OnDemandStateManager CreateStateManager(Mode contextMode)
		{
			switch (contextMode)
			{
			case Mode.Streaming:
				return new OnDemandStateManagerStreaming(this);
			case Mode.Full:
				return new OnDemandStateManagerFull(this);
			case Mode.DefinitionOnly:
				return new OnDemandStateManagerDefinitionOnly(this);
			default:
				Global.Tracer.Assert(condition: false, "CreateStateManager: invalid contextMode.");
				throw new InvalidOperationException("CreateStateManager: invalid contextMode.");
			}
		}

		private static ImageCacheManager CreateImageCacheManager(Mode contextMode, OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
		{
			switch (contextMode)
			{
			case Mode.Streaming:
				return new StreamingImageCacheManager(odpMetadata, chunkFactory);
			case Mode.Full:
			case Mode.DefinitionOnly:
				return new SnapshotImageCacheManager(odpMetadata, chunkFactory);
			default:
				Global.Tracer.Assert(condition: false, "CreateImageCacheManager: invalid contextMode.");
				throw new InvalidOperationException("CreateImageCacheManager: invalid contextMode.");
			}
		}

		internal void MergeHasUserProfileState(UserProfileState newProfileStateFlags)
		{
			m_commonInfo.MergeHasUserProfileState(newProfileStateFlags);
		}

		internal void CreatedScopeInstance(IRIFReportDataScope scope)
		{
			m_stateManager.CreatedScopeInstance(scope);
		}

		internal void EnsureCultureIsSetOnCurrentThread()
		{
			if (m_threadCulture != null && Thread.CurrentThread.CurrentCulture.LCID != m_threadCulture.LCID)
			{
				Thread.CurrentThread.CurrentCulture = m_threadCulture;
			}
		}

		internal void SetupEnvironment(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			m_currentDataSetIndex = -1;
			m_currentReportInstance = reportInstance;
			reportInstance.SetupEnvironment(this);
		}

		internal void SetComparisonInformation(DataSetCore dataSet)
		{
			SetComparisonInformation(dataSet.CreateCultureInfoFromLcid().CompareInfo, dataSet.GetCLRCompareOptions(), dataSet.NullsAsBlanks, dataSet.UseOrdinalStringKeyGeneration);
		}

		internal void SetComparisonInformation(CompareInfo compareInfo, CompareOptions clrCompareOptions, bool nullsAsBlanks, bool useOrdinalStringKeyGeneration)
		{
			m_compareInfo = compareInfo;
			m_clrCompareOptions = clrCompareOptions;
			m_nullsAsBlanks = nullsAsBlanks;
			m_useOrdinalStringKeyGeneration = useOrdinalStringKeyGeneration;
			m_processingComparer = null;
			m_stringKeyGenerator = null;
		}

		internal void UnregisterAbortInfo()
		{
			m_commonInfo.UnregisterAbortInfo();
		}

		internal bool HasSecondPassOperation(SecondPassOperations op)
		{
			return (m_secondPassOperation & op) != 0;
		}

		internal void ResetUserSortFilterContext()
		{
			if (m_userSortFilterContext != null)
			{
				m_userSortFilterContext.ResetContextForTopLevelDataSet();
			}
		}

		internal bool IsRdlSandboxingEnabled()
		{
			if (Configuration != null)
			{
				return Configuration.RdlSandboxing != null;
			}
			return false;
		}

		internal int GetActiveCompatibilityVersion()
		{
			return ReportProcessingCompatibilityVersion.GetCompatibilityVersion(Configuration);
		}

		private void InitFlags(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (report != null)
			{
				if (!SnapshotProcessing || m_commonInfo.ReprocessSnapshot)
				{
					m_odpMetadata.ReportSnapshot.DefinitionTreeHasDocumentMap |= report.HasLabels;
					m_odpMetadata.ReportSnapshot.HasDocumentMap |= report.HasLabels;
				}
				HasBookmarks = report.HasBookmarks;
				HasShowHide = (report.ShowHideType == Microsoft.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.Interactive);
			}
		}

		internal void InitializeDataSetMembers(int dataSetCount)
		{
			if (dataSetCount >= 0)
			{
				m_dataSetRetrievalComplete = new bool[dataSetCount];
			}
			else
			{
				m_dataSetRetrievalComplete = null;
			}
		}

		internal void RuntimeInitializePageSectionVariables(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, object[] reportVariableValues)
		{
			if (report.Variables != null)
			{
				AddVariablesToReportObjectModel(report.Variables, null, report.ObjectType, null, reportVariableValues);
			}
			if (report.GroupsWithVariables != null)
			{
				int count = report.GroupsWithVariables.Count;
				for (int i = 0; i < count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = report.GroupsWithVariables[i].Grouping;
					AddVariablesToReportObjectModel(grouping.Variables, null, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, null);
				}
			}
		}

		internal void RuntimeInitializeVariables(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (report.Variables != null)
			{
				AddVariablesToReportObjectModel(report.Variables, (report.ReportExprHost == null) ? null : report.ReportExprHost.VariableValueHosts, report.ObjectType, report.Name, null);
			}
			if (report.GroupsWithVariables != null)
			{
				int count = report.GroupsWithVariables.Count;
				for (int i = 0; i < count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = report.GroupsWithVariables[i].Grouping;
					AddVariablesToReportObjectModel(grouping.Variables, (grouping.ExprHost == null) ? null : grouping.ExprHost.VariableValueHosts, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, null);
				}
			}
		}

		private void AddVariablesToReportObjectModel(List<Microsoft.ReportingServices.ReportIntermediateFormat.Variable> variableDef, IndexedExprHost variableValuesHost, Microsoft.ReportingServices.ReportProcessing.ObjectType parentObjectType, string parentObjectName, object[] variableValues)
		{
			if (variableDef == null)
			{
				return;
			}
			int count = variableDef.Count;
			for (int i = 0; i < count; i++)
			{
				VariableImpl variableImpl = new VariableImpl(variableDef[i], variableValuesHost, parentObjectType, parentObjectName, m_reportRuntime, i);
				if (variableValues != null)
				{
					variableImpl.SetValue(variableValues[i], internalSet: true);
				}
				m_reportObjectModel.VariablesImpl.Add(variableImpl);
			}
		}

		internal void RuntimeInitializeLookups(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (report.DataSources == null)
			{
				return;
			}
			for (int i = 0; i < report.DataSources.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource = report.DataSources[i];
				if (dataSource.DataSets == null)
				{
					continue;
				}
				for (int j = 0; j < dataSource.DataSets.Count; j++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = dataSource.DataSets[j];
					if (dataSet.Lookups != null)
					{
						for (int k = 0; k < dataSet.Lookups.Count; k++)
						{
							LookupImpl lookup = new LookupImpl(dataSet.Lookups[k], m_reportRuntime);
							m_reportObjectModel.LookupsImpl.Add(lookup);
						}
					}
				}
			}
		}

		internal void RuntimeInitializeTextboxObjs(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItems, bool setExprHost)
		{
			for (int i = 0; i < reportItems.Count; i++)
			{
				RuntimeInitializeTextboxObjs(reportItems[i], setExprHost);
			}
		}

		internal void RuntimeInitializeTextboxObjs(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, bool setExprHost)
		{
			if (setExprHost && m_reportRuntime.ReportExprHost != null)
			{
				reportItem.SetExprHost(m_reportRuntime.ReportExprHost, m_reportObjectModel);
			}
			switch (reportItem.ObjectType)
			{
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix = (Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)reportItem;
				if (tablix.Corner != null && tablix.Corner.Count != 0)
				{
					foreach (List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> item in tablix.Corner)
					{
						foreach (Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell item2 in item)
						{
							if (item2.CellContents != null)
							{
								RuntimeInitializeTextboxObjs(item2.CellContents, setExprHost);
								if (item2.AltCellContents != null)
								{
									RuntimeInitializeTextboxObjs(item2.AltCellContents, setExprHost);
								}
							}
						}
					}
				}
				if (tablix.Rows != null && tablix.RowCount != 0)
				{
					for (int k = 0; k < tablix.RowCount; k++)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TablixRow tablixRow = tablix.TablixRows[k];
						for (int l = 0; l < tablix.ColumnCount; l++)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = tablixRow.TablixCells[l];
							if (tablixCell.CellContents != null)
							{
								RuntimeInitializeTextboxObjs(tablixCell.CellContents, setExprHost);
								if (tablixCell.AltCellContents != null)
								{
									RuntimeInitializeTextboxObjs(tablixCell.AltCellContents, setExprHost);
								}
							}
						}
					}
				}
				RuntimeInitializeTextboxObjsInMemberTree(tablix.ColumnMembers, setExprHost);
				RuntimeInitializeTextboxObjsInMemberTree(tablix.RowMembers, setExprHost);
				break;
			}
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle:
				RuntimeInitializeTextboxObjs(((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)reportItem).ReportItems, setExprHost);
				break;
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Textbox:
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textBox = (Microsoft.ReportingServices.ReportIntermediateFormat.TextBox)reportItem;
				TextBoxImpl textBoxImpl = new TextBoxImpl(textBox, m_reportRuntime, m_reportRuntime);
				if (setExprHost)
				{
					if (textBox.ValueReferenced)
					{
						Global.Tracer.Assert(textBox.ExprHost != null, "(textBoxDef.ExprHost != null)");
						m_reportItemThisDotValueReferenced = true;
						textBox.TextBoxExprHost.SetTextBox(textBoxImpl);
					}
					if (textBox.TextRunValueReferenced)
					{
						for (int i = 0; i < textBox.Paragraphs.Count; i++)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph = textBox.Paragraphs[i];
							if (!paragraph.TextRunValueReferenced)
							{
								continue;
							}
							for (int j = 0; j < paragraph.TextRuns.Count; j++)
							{
								Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun = paragraph.TextRuns[j];
								if (textRun.ValueReferenced)
								{
									Global.Tracer.Assert(textRun.ExprHost != null);
									m_reportItemThisDotValueReferenced = true;
									textRun.ExprHost.SetTextRun(textBoxImpl.Paragraphs[i].TextRuns[j]);
								}
							}
						}
					}
				}
				m_reportObjectModel.ReportItemsImpl.Add(textBoxImpl);
				break;
			}
			}
		}

		private void RuntimeInitializeTextboxObjsInMemberTree(HierarchyNodeList memberNodes, bool setExprHost)
		{
			if (memberNodes == null)
			{
				return;
			}
			for (int i = 0; i < memberNodes.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = (Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember)memberNodes[i];
				if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null)
				{
					RuntimeInitializeTextboxObjs(tablixMember.TablixHeader.CellContents, setExprHost);
					if (tablixMember.TablixHeader.AltCellContents != null)
					{
						RuntimeInitializeTextboxObjs(tablixMember.TablixHeader.AltCellContents, setExprHost);
					}
				}
				if (tablixMember.InnerHierarchy != null)
				{
					RuntimeInitializeTextboxObjsInMemberTree(tablixMember.InnerHierarchy, setExprHost);
				}
			}
		}

		internal void RuntimeInitializeReportItemObjs(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItems, bool traverseDataRegions)
		{
			for (int i = 0; i < reportItems.Count; i++)
			{
				RuntimeInitializeReportItemObjs(reportItems[i], traverseDataRegions);
			}
		}

		internal void RuntimeInitializeReportItemObjs(List<Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion> mapDataRegions, bool traverseDataRegions)
		{
			for (int i = 0; i < mapDataRegions.Count; i++)
			{
				RuntimeInitializeReportItemObjs(mapDataRegions[i], traverseDataRegions);
			}
		}

		internal void RuntimeInitializeReportItemObjs(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, bool traverseDataRegions)
		{
			if (reportItem.IsDataRegion)
			{
				if (traverseDataRegions)
				{
					if (m_reportRuntime.ReportExprHost != null)
					{
						reportItem.SetExprHost(m_reportRuntime.ReportExprHost, m_reportObjectModel);
					}
					Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = reportItem as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion;
					dataRegion.DataRegionContentsSetExprHost(m_reportObjectModel, traverseDataRegions);
					switch (dataRegion.ObjectType)
					{
					case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
						RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)dataRegion).TablixExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)dataRegion).TablixExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart:
						RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.Chart)dataRegion).ChartExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.Chart)dataRegion).ChartExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
						RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)dataRegion).GaugePanelExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)dataRegion).GaugePanelExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
						RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem)dataRegion).CustomReportItemExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem)dataRegion).CustomReportItemExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case Microsoft.ReportingServices.ReportProcessing.ObjectType.MapDataRegion:
						RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion)dataRegion).MapDataRegionExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion)dataRegion).MapDataRegionExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					}
				}
			}
			else if (reportItem.ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle)
			{
				RuntimeInitializeReportItemObjs(((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)reportItem).ReportItems, traverseDataRegions);
			}
			else if (reportItem.ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Map && ((Microsoft.ReportingServices.ReportIntermediateFormat.Map)reportItem).MapDataRegions != null)
			{
				RuntimeInitializeReportItemObjs(((Microsoft.ReportingServices.ReportIntermediateFormat.Map)reportItem).MapDataRegions, traverseDataRegions);
			}
		}

		private void RuntimeInitializeMemberTree(HierarchyNodeList memberNodes, IList<IMemberNode> memberExprHosts, bool traverseDataRegions)
		{
			if (memberNodes == null)
			{
				return;
			}
			for (int i = 0; i < memberNodes.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = memberNodes[i];
				IList<IMemberNode> memberExprHosts2;
				if (reportHierarchyNode.ExprHostID >= 0 && memberExprHosts != null)
				{
					reportHierarchyNode.SetExprHost(memberExprHosts[reportHierarchyNode.ExprHostID], m_reportObjectModel);
					memberExprHosts2 = memberExprHosts[reportHierarchyNode.ExprHostID].MemberTreeHostsRemotable;
				}
				else
				{
					memberExprHosts2 = null;
				}
				if (reportHierarchyNode.InnerHierarchy != null && 0 < reportHierarchyNode.InnerHierarchy.Count)
				{
					RuntimeInitializeMemberTree(reportHierarchyNode.InnerHierarchy, memberExprHosts2, traverseDataRegions);
				}
				reportHierarchyNode.MemberContentsSetExprHost(m_reportObjectModel, traverseDataRegions);
			}
		}

		internal void RuntimeInitializeAggregates<AggregateType>(List<AggregateType> aggregates) where AggregateType : Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggregates == null)
			{
				return;
			}
			int count = aggregates.Count;
			for (int i = 0; i < count; i++)
			{
				AggregateType val = aggregates[i];
				if (m_reportAggregates.ContainsKey(val.Name))
				{
					continue;
				}
				m_reportAggregates.Add(val.Name, val);
				if (val.DuplicateNames != null)
				{
					int count2 = val.DuplicateNames.Count;
					for (int j = 0; j < count2; j++)
					{
						m_reportAggregates.Add(val.DuplicateNames[j], val);
					}
				}
			}
		}

		internal int RecursiveLevel(string scopeName)
		{
			return m_stateManager.RecursiveLevel(scopeName);
		}

		internal bool InScope(string scopeName)
		{
			return m_stateManager.InScope(scopeName);
		}

		internal Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			return m_stateManager.GetCurrentSpecialGroupingValues();
		}

		internal IRecordRowReader CreateSequentialDataReader(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			return m_stateManager.CreateSequentialDataReader(dataSet, out dataSetInstance);
		}

		internal bool CalculateAggregate(string aggregateName)
		{
			return m_stateManager.CalculateAggregate(aggregateName);
		}

		internal bool CalculateLookup(LookupInfo lookup)
		{
			return m_stateManager.CalculateLookup(lookup);
		}

		internal bool PrepareFieldsCollectionForDirectFields()
		{
			return m_stateManager.PrepareFieldsCollectionForDirectFields();
		}

		internal void RestoreContext(IInstancePath originalObject)
		{
			m_stateManager.RestoreContext(originalObject);
		}

		internal void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			m_stateManager.SetupContext(rifObject, romInstance);
		}

		internal void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			m_stateManager.SetupContext(rifObject, romInstance, moveNextInstanceIndex);
		}

		internal void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			CheckAndThrowIfAborted();
			m_stateManager.BindNextMemberInstance(rifObject, romInstance, moveNextInstanceIndex);
		}

		internal void OnDemandProcessDataPipelineWithRestore(DataSetAggregateDataPipelineManager pipeline)
		{
			FieldsContext currentFields = ReportObjectModel.CurrentFields;
			IScalabilityCache tablixProcessingScalabilityCache = m_tablixProcessingScalabilityCache;
			m_tablixProcessingScalabilityCache = null;
			pipeline.StartProcessing();
			pipeline.StopProcessing();
			m_tablixProcessingScalabilityCache = tablixProcessingScalabilityCache;
			if (currentFields != null)
			{
				ReportObjectModel.RestoreFields(currentFields);
			}
		}

		internal void SetupEmptyTopLevelFields()
		{
			m_reportObjectModel.SetupEmptyTopLevelFields();
			m_currentDataSetIndex = -1;
			m_currentDataSetInstance = null;
		}

		internal void SetupFieldsForNewDataSetPageSection(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataset)
		{
			m_reportObjectModel.SetupPageSectionDataSetFields(dataset);
			m_currentDataSetIndex = dataset.IndexInCollection;
			m_currentDataSetInstance = null;
		}

		internal void SetupFieldsForNewDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataset, Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance, bool addRowIndex, bool noRows)
		{
			m_reportObjectModel.SetupFieldsForNewDataSet(dataset, addRowIndex, noRows, forceNewFieldsContext: false);
			m_currentDataSetIndex = dataset.IndexInCollection;
			m_currentDataSetInstance = dataSetInstance;
		}

		internal void EnsureRuntimeEnvironmentForDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, bool noRows)
		{
			if (m_currentDataSetIndex != dataSet.IndexInCollection)
			{
				dataSet.SetupRuntimeEnvironment(this);
				SetupFieldsForNewDataSet(dataSet, null, addRowIndex: true, noRows);
			}
		}

		internal void AddSpecialDataRowSort(IReference<IDataRowSortOwner> sortOwner)
		{
			if (m_dataRowSortOwners == null)
			{
				m_dataRowSortOwners = new List<IReference<IDataRowSortOwner>>();
			}
			m_dataRowSortOwners.Add(sortOwner);
		}

		internal void AddSpecialDataRegionFilters(Filters filters)
		{
			if (m_specialDataRegionFilters == null)
			{
				m_specialDataRegionFilters = new List<Filters>();
			}
			m_specialDataRegionFilters.Add(filters);
		}

		private bool ProcessDataRegionsWithSpecialFiltersOrDataRowSorting()
		{
			bool flag = false;
			int num = (m_dataRowSortOwners != null) ? m_dataRowSortOwners.Count : 0;
			if (m_specialDataRegionFilters != null)
			{
				int count = m_specialDataRegionFilters.Count;
				for (int i = 0; i < count; i++)
				{
					m_specialDataRegionFilters[i].FinishReadingRows();
				}
				m_specialDataRegionFilters.RemoveRange(0, count);
				flag |= (m_specialDataRegionFilters.Count > 0);
			}
			if (num != 0)
			{
				for (int j = 0; j < num; j++)
				{
					using (m_dataRowSortOwners[j].PinValue())
					{
						m_dataRowSortOwners[j].Value().DataRowSortTraverse();
					}
				}
				m_dataRowSortOwners.RemoveRange(0, num);
				flag |= (m_dataRowSortOwners.Count > 0);
			}
			return flag;
		}

		internal bool PopulateRuntimeSortFilterEventInfo(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			return m_userSortFilterContext.PopulateRuntimeSortFilterEventInfo(this, dataSet);
		}

		internal bool IsSortFilterTarget(bool[] isSortFilterTarget, IReference<IScope> outerScope, IReference<IHierarchyObj> target, ref Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeUserSortTargetInfo userSortTargetInfo)
		{
			return m_userSortFilterContext.IsSortFilterTarget(isSortFilterTarget, outerScope, target, ref userSortTargetInfo);
		}

		internal void ProcessUserSortForTarget(IReference<IHierarchyObj> target, ref ScalableList<DataFieldRow> dataRows, bool targetForNonDetailSort)
		{
			m_userSortFilterContext.ProcessUserSortForTarget(m_reportObjectModel, m_reportRuntime, target, ref dataRows, targetForNonDetailSort);
		}

		internal void RegisterSortFilterExpressionScope(IReference<IScope> container, IReference<RuntimeDataRegionObj> scopeObj, bool[] isSortFilterExpressionScope)
		{
			m_userSortFilterContext.RegisterSortFilterExpressionScope(container, scopeObj, isSortFilterExpressionScope);
		}

		internal EventInformation GetUserSortFilterInformation(out string oldUniqueName)
		{
			return m_commonInfo.GetUserSortFilterInformation(out oldUniqueName);
		}

		internal void MergeNewUserSortFilterInformation()
		{
			m_commonInfo.MergeNewUserSortFilterInformation();
		}

		internal void FirstPassPostProcess()
		{
			while (ProcessDataRegionsWithSpecialFiltersOrDataRowSorting())
			{
			}
		}

		internal void ApplyUserSorts()
		{
			while (m_userSortFilterContext.ProcessUserSort(this))
			{
			}
		}

		internal List<object>[] GetScopeValues(Microsoft.ReportingServices.ReportIntermediateFormat.GroupingList containingScopes, IScope containingScope)
		{
			List<object>[] array = null;
			if (containingScopes != null && 0 < containingScopes.Count)
			{
				array = new List<object>[containingScopes.Count];
				int index = 0;
				containingScope.GetScopeValues(null, array, ref index);
			}
			return array;
		}

		internal ProcessingMessageList RegisterComparisonErrorForSortFilterEvent(string propertyName)
		{
			Global.Tracer.Assert(m_userSortFilterContext.CurrentSortFilterEventSource != null, "(null != m_userSortFilterContext.CurrentSortFilterEventSource)");
			ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, m_userSortFilterContext.CurrentSortFilterEventSource.ObjectType, m_userSortFilterContext.CurrentSortFilterEventSource.Name, propertyName);
			return ErrorContext.Messages;
		}

		internal void CheckAndThrowIfAborted()
		{
			m_commonInfo.AbortInfo?.ThrowIfAborted(CancelationTrigger.ReportProcessing, m_subReportInstanceOrSharedDatasetUniqueName);
		}

		internal void AddDataChunkReader(int dataSetIndexInCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader)
		{
			if (m_dataSetToDataReader == null)
			{
				m_dataSetToDataReader = new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader[m_reportDefinition.MappingNameToDataSet.Count];
			}
			Global.Tracer.Assert(m_dataSetToDataReader[dataSetIndexInCollection] == null, "(null == m_dataSetToDataReader[dataSetIndexInCollection])");
			m_dataSetToDataReader[dataSetIndexInCollection] = dataReader;
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader GetDataChunkReader(int dataSetIndex)
		{
			if (IsPageHeaderFooter)
			{
				return m_parentContext.GetDataChunkReader(dataSetIndex);
			}
			if (m_dataSetToDataReader == null || m_dataSetToDataReader[dataSetIndex] == null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_reportDefinition.MappingDataSetIndexToDataSet[dataSetIndex];
				string chunkName;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = GetDataSetInstance(dataSet, out chunkName);
				Global.Tracer.Assert(dataSetInstance != null, "Missing expected DataSetInstance. Report: {0} DataSet: {1} DataSetIndex: {2}", m_reportDefinition.Name.MarkAsPrivate(), dataSet.Name.MarkAsModelInfo(), dataSetIndex);
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader(dataSetInstance, this, chunkName);
				AddDataChunkReader(dataSetIndex, dataChunkReader);
				return dataChunkReader;
			}
			return m_dataSetToDataReader[dataSetIndex];
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance GetDataSetInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			if (StreamingMode)
			{
				return null;
			}
			string chunkName;
			return GetDataSetInstance(dataSet, out chunkName);
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance GetDataSetInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out string chunkName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance value = null;
			chunkName = null;
			if (dataSet.UsedOnlyInParameters)
			{
				return null;
			}
			chunkName = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.GenerateDataChunkName(this, dataSet.ID, m_inSubreport);
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance> dataChunkMap = m_odpMetadata.DataChunkMap;
			if (dataChunkMap == null || !dataChunkMap.TryGetValue(chunkName, out value))
			{
				chunkName = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.GenerateLegacySharedSubReportDataChunkName(this, dataSet.ID);
				if ((dataChunkMap == null || !dataChunkMap.TryGetValue(chunkName, out value)) && SnapshotProcessing)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Dataset not found in data chunk map. Name={0}, Chunkname={1}", dataSet.Name.MarkAsPrivate(), chunkName);
				}
			}
			if (value != null && value.DataSetDef == null)
			{
				value.DataSetDef = dataSet;
			}
			return value;
		}

		internal bool[] GenerateDataSetExclusionList(out int unprocessedDataSetCount)
		{
			int num = unprocessedDataSetCount = m_reportDefinition.DataSetCount;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_reportDefinition.MappingDataSetIndexToDataSet[i];
				if (dataSet.UsedOnlyInParameters)
				{
					array[i] = true;
					unprocessedDataSetCount--;
					continue;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = m_currentReportInstance.GetDataSetInstance(dataSet, this);
				if (dataSetInstance == null || IsTablixProcessingComplete(i))
				{
					array[i] = true;
					unprocessedDataSetCount--;
				}
			}
			return array;
		}

		internal void FreeAllResources()
		{
			if (m_odpMetadata == null)
			{
				FreeResources();
				return;
			}
			foreach (OnDemandProcessingContext odpContext in m_odpMetadata.OdpContexts)
			{
				odpContext.FreeResources();
			}
		}

		private void FreeResources()
		{
			if (m_dataSetToDataReader != null)
			{
				for (int i = 0; i < m_dataSetToDataReader.Length; i++)
				{
					if (m_dataSetToDataReader[i] != null)
					{
						m_dataSetToDataReader[i].Close();
						m_dataSetToDataReader[i] = null;
					}
				}
				m_dataSetToDataReader = null;
			}
			if (m_stateManager != null)
			{
				m_stateManager.FreeResources();
			}
			EnsureScalabilityCleanup();
		}

		internal void EnsureScalabilitySetup()
		{
			if (m_tablixProcessingScalabilityCache == null)
			{
				m_tablixProcessingScalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(CreateStreamCallback, "RGT", StorageObjectCreator.Instance, RuntimeReferenceCreator.Instance, ComponentType.Processing, 5);
				ExecutionLogContext.RegisterTablixProcessingScaleCache((!m_isSharedDataSetExecutionOnly) ? m_reportDefinition.GlobalID : 0);
			}
		}

		internal void EnsureScalabilityCleanup()
		{
			if (m_tablixProcessingScalabilityCache != null)
			{
				ExecutionLogContext.UnRegisterTablixProcessingScaleCache((!m_isSharedDataSetExecutionOnly) ? m_reportDefinition.GlobalID : 0, m_tablixProcessingScalabilityCache);
				m_tablixProcessingScalabilityCache.Dispose();
				m_tablixProcessingScalabilityCache = null;
			}
		}

		internal bool IsTablixProcessingComplete(int dataSetIndexInCollection)
		{
			return m_odpMetadata.IsTablixProcessingComplete(this, dataSetIndexInCollection);
		}

		internal void SetTablixProcessingComplete(int dataSetIndexInCollection)
		{
			m_odpMetadata.SetTablixProcessingComplete(this, dataSetIndexInCollection);
		}

		internal int CreateUniqueID()
		{
			int num = m_commonInfo.CreateUniqueID();
			if (m_subReportInfo != null)
			{
				m_odpMetadata.MetadataHasChanged = true;
				m_subReportInfo.LastID = num;
			}
			return num;
		}

		internal bool GetResource(string path, out byte[] resource, out string mimeType, out bool registerInvalidSizeWarning)
		{
			if (m_commonInfo.GetResourceCallback != null)
			{
				m_commonInfo.ExecutionLogContext.ExternalImageCount++;
				m_commonInfo.ExecutionLogContext.StartExternalImageTimer();
				bool registerExternalWarning;
				try
				{
					m_commonInfo.GetResourceCallback.GetResource(m_catalogItemContext, path, out resource, out mimeType, out registerExternalWarning, out registerInvalidSizeWarning);
					if (resource != null)
					{
						m_commonInfo.ExecutionLogContext.ExternalImageBytes += resource.LongLength;
					}
				}
				finally
				{
					m_commonInfo.ExecutionLogContext.StopExternalImageTimer();
				}
				if (registerExternalWarning)
				{
					ErrorContext.Register(ProcessingErrorCode.rsWarningFetchingExternalImages, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, null, null);
				}
				if (registerInvalidSizeWarning)
				{
					TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingExternalResourceExceedsMaximumSize);
				}
				return true;
			}
			resource = null;
			mimeType = null;
			registerInvalidSizeWarning = false;
			return false;
		}

		internal void TraceOneTimeWarning(ProcessingErrorCode errorCode)
		{
			m_commonInfo.TraceOneTimeWarning(errorCode, m_catalogItemContext);
		}

		internal void LoadExistingSubReportDataChunkNameModifier(Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance)
		{
			Global.Tracer.Assert(m_subReportInfo != null, "Cannot set DataChunkName modifier if the subreport definition could not be found");
			m_subReportDataChunkNameModifier = subReportInstance.GetChunkNameModifier(m_subReportInfo, useCachedValue: true, addEntry: false, out m_foundExistingSubReportInstance);
		}

		internal void SetSubReportNameModifierAndParameters(Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance, bool addEntry)
		{
			Global.Tracer.Assert(m_subReportInfo != null, "Cannot set DataChunkName modifier and parameters if the subreport definition could not be found");
			m_subReportDataChunkNameModifier = subReportInstance.GetChunkNameModifier(m_subReportInfo, useCachedValue: false, addEntry, out m_foundExistingSubReportInstance);
			if (addEntry && !m_foundExistingSubReportInstance)
			{
				m_odpMetadata.MetadataHasChanged = true;
			}
			if (SnapshotProcessing)
			{
				ParametersImpl parameters = subReportInstance.Parameters;
				if (parameters != null)
				{
					m_reportObjectModel.ParametersImpl = parameters;
				}
			}
		}

		internal void SetSharedDataSetUniqueName(string chunkName)
		{
			m_subReportInstanceOrSharedDatasetUniqueName = chunkName;
			m_commonInfo.AbortInfo?.AddSubreportInstanceOrSharedDataSet(m_subReportInstanceOrSharedDatasetUniqueName);
		}

		internal void SetSubReportContext(Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance, bool setupReportOM)
		{
			Global.Tracer.Assert(m_subReportInfo != null, "Cannot SetSubReportContext if the subreport definition could not be found");
			string text = m_subReportInfo.UniqueName + "x" + subReportInstance.InstanceUniqueName;
			if (!SnapshotProcessing && m_reportDefinition != null)
			{
				InitializeDataSetMembers(m_reportDefinition.MappingNameToDataSet.Count);
			}
			if (m_subReportInstanceOrSharedDatasetUniqueName != text)
			{
				m_subReportInstanceOrSharedDatasetUniqueName = text;
				m_commonInfo.AbortInfo?.AddSubreportInstanceOrSharedDataSet(m_subReportInstanceOrSharedDatasetUniqueName);
				ResetDataSetToDataReader();
				if (subReportInstance.ThreadCulture != null)
				{
					m_threadCulture = subReportInstance.ThreadCulture;
				}
				m_currentReportInstance = subReportInstance.ReportInstance.Value();
				m_currentDataSetIndex = -1;
				m_stateManager.ResetOnDemandState();
				if (setupReportOM && m_reportObjectModel != null)
				{
					m_reportObjectModel.SetForNewSubReportContext(subReportInstance.Parameters);
				}
			}
		}

		private void ResetDataSetToDataReader()
		{
			if (m_dataSetToDataReader == null || m_currentReportInstance == null)
			{
				return;
			}
			for (int i = 0; i < m_dataSetToDataReader.Length; i++)
			{
				if (m_dataSetToDataReader[i] != null)
				{
					((IDisposable)m_dataSetToDataReader[i]).Dispose();
					m_dataSetToDataReader[i] = null;
				}
			}
		}

		internal static ParameterInfoCollection EvaluateSubReportParameters(OnDemandProcessingContext parentContext, Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			if (subReport.Parameters != null && subReport.ParametersFromCatalog != null)
			{
				for (int i = 0; i < subReport.Parameters.Count; i++)
				{
					string name = subReport.Parameters[i].Name;
					ParameterInfo parameterInfo = subReport.ParametersFromCatalog[name];
					if (parameterInfo == null)
					{
						throw new UnknownReportParameterException(name);
					}
					parentContext.LastRIFObject = subReport;
					Microsoft.ReportingServices.RdlExpressions.ParameterValueResult parameterValueResult = parentContext.ReportRuntime.EvaluateParameterValueExpression(subReport.Parameters[i], subReport.ObjectType, subReport.Name, "ParameterValue");
					if (parameterValueResult.ErrorOccurred)
					{
						throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, name);
					}
					object[] array = null;
					object[] array2 = parameterValueResult.Value as object[];
					array = ((array2 == null) ? new object[1]
					{
						parameterValueResult.Value
					} : array2);
					ParameterInfo parameterInfo2 = new ParameterInfo();
					parameterInfo2.Name = name;
					parameterInfo2.Values = array;
					parameterInfo2.DataType = parameterValueResult.Type;
					parameterInfoCollection.Add(parameterInfo2);
				}
			}
			ParameterInfoCollection parameterInfoCollection2 = new ParameterInfoCollection();
			subReport.ParametersFromCatalog.CopyTo(parameterInfoCollection2);
			return ParameterInfoCollection.Combine(parameterInfoCollection2, parameterInfoCollection, checkReadOnly: true, ignoreNewQueryParams: false, isParameterDefinitionUpdate: false, isSharedDataSetParameter: false, Localization.ClientPrimaryCulture);
		}

		internal bool StoreUpdatedVariableValue(int index, object value)
		{
			return m_odpMetadata.StoreUpdatedVariableValue(this, m_currentReportInstance, index, value);
		}

		internal int CompareAndStopOnError(object value1, object value2, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, bool extendedTypeComparisons)
		{
			try
			{
				bool validComparisonResult;
				return ProcessingComparer.Compare(value1, value2, throwExceptionOnComparisonFailure: true, extendedTypeComparisons, out validComparisonResult);
			}
			catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
			{
				throw new ReportProcessingException(RegisterSpatialTypeComparisonError(objectType, objectName, reportProcessingException_SpatialTypeComparisonError.Type));
			}
			catch (ReportProcessingException_ComparisonError e)
			{
				throw new ReportProcessingException(RegisterComparisonError(e, objectType, objectName, propertyName));
			}
			catch (CommonDataComparerException e2)
			{
				throw new ReportProcessingException(RegisterComparisonError(e2, objectType, objectName, propertyName));
			}
		}

		internal ProcessingMessageList RegisterComparisonError(IDataComparisonError e, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (e == null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, objectType, objectName, propertyName);
			}
			else
			{
				m_errorContext.Register(ProcessingErrorCode.rsComparisonTypeError, Severity.Error, objectType, objectName, propertyName, e.TypeX, e.TypeY);
			}
			return m_errorContext.Messages;
		}

		internal ProcessingMessageList RegisterSpatialTypeComparisonError(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string spatialTypeName)
		{
			m_errorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, objectType, objectName, spatialTypeName);
			return m_errorContext.Messages;
		}

		void IStaticReferenceable.SetID(int id)
		{
			m_staticRefId = id;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.OnDemandProcessingContext;
		}
	}
}
