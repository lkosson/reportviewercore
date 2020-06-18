using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Policy;

namespace Microsoft.Reporting
{
	[Serializable]
	internal abstract class LocalService : ILocalProcessingHost, IDisposable
	{
		internal delegate void LocalModeSecurityValidatorCallback(PreviewItemContext itemContext, PublishingResult publishingResult);

		protected class SubreportCallbackHandler
		{
			private LocalService m_service;

			protected LocalService Service => m_service;

			public SubreportCallbackHandler(LocalService service)
			{
				m_service = service;
			}

			public virtual void OnDemandSubReportCallback(ICatalogItemContext itemContext, string subreportPath, string newChunkName, ReportProcessing.NeedsUpgrade upgradeCheck, ParameterInfoCollection parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out IChunkFactory chunkFactory, out ParameterInfoCollection parameters)
			{
				m_service.OnGetSubReportDefinition(itemContext, subreportPath, newChunkName, upgradeCheck, parentQueryParameters, out subreportContext, out description, out chunkFactory, out parameters);
			}
		}

		[NonSerialized]
		private LocalDataRetrieval m_dataRetrieval;

		private ILocalCatalog m_catalog;

		private LocalCatalogTempDB m_catalogTempDB = new LocalCatalogTempDB();

		private bool m_showDetailedSubreportMessages = true;

		[NonSerialized]
		private LocalModeSecurityValidatorCallback m_securityValidator;

		private ReportRuntimeSetupHandler m_reportRuntimeSetupHandler = new ReportRuntimeSetupHandler();

		private PreviewItemContext m_itemContext;

		private LocalExecutionSession m_executionSession = new LocalExecutionSession();

		public PreviewItemContext ItemContext
		{
			[DebuggerStepThrough]
			get
			{
				return m_itemContext;
			}
			set
			{
				m_itemContext = value;
				ResetExecution(forceRecompile: true);
			}
		}

		private bool GenerateExpressionHostWithRefusedPermissions
		{
			[DebuggerStepThrough]
			get
			{
				return m_reportRuntimeSetupHandler.RequireExpressionHostWithRefusedPermissions;
			}
		}

		public ILocalCatalog Catalog
		{
			[DebuggerStepThrough]
			get
			{
				return m_catalog;
			}
		}

		public LocalDataRetrieval DataRetrieval
		{
			[DebuggerStepThrough]
			get
			{
				return m_dataRetrieval;
			}
			[DebuggerStepThrough]
			set
			{
				m_dataRetrieval = value;
			}
		}

		public bool SupportsQueries => m_dataRetrieval.SupportsQueries;

		public bool CanSelfCancel => false;

		public bool ShowDetailedSubreportMessages
		{
			[DebuggerStepThrough]
			get
			{
				return m_showDetailedSubreportMessages;
			}
			set
			{
				if (m_showDetailedSubreportMessages != value)
				{
					m_showDetailedSubreportMessages = value;
					ResetExecution();
				}
			}
		}

		public virtual LocalProcessingHostMapTileServerConfiguration MapTileServerConfiguration => null;

		public LocalModeSecurityValidatorCallback SecurityValidator
		{
			[DebuggerStepThrough]
			set
			{
				m_securityValidator = value;
			}
		}

		protected virtual bool RecompileOnResetExecution
		{
			[DebuggerStepThrough]
			get
			{
				return false;
			}
		}

		protected LocalExecutionSession ExecutionSession
		{
			[DebuggerStepThrough]
			get
			{
				return m_executionSession;
			}
		}

		public LocalExecutionInfo ExecutionInfo
		{
			[DebuggerStepThrough]
			get
			{
				return m_executionSession.ExecutionInfo;
			}
		}

		protected LocalService(ILocalCatalog catalog, Evidence sandboxEvidence, PolicyManager policyManager)
		{
			m_catalog = catalog;
			ReportRuntimeSetupHandler.InitAppDomainPool(sandboxEvidence, policyManager);
		}

		public void Dispose()
		{
			m_reportRuntimeSetupHandler.Dispose();
			m_catalogTempDB.Dispose();
		}

		public void CopySecuritySettingsFrom(ILocalProcessingHost sourceProcessingHost)
		{
			LocalService localService = (LocalService)sourceProcessingHost;
			m_reportRuntimeSetupHandler = localService.m_reportRuntimeSetupHandler;
		}

		public void SetCancelState(bool shouldCancelRequests)
		{
			throw new NotSupportedException();
		}

		public void SetReportParameters(NameValueCollection userSpecifiedValues)
		{
			ParameterInfoCollection parameterInfoCollection = m_executionSession.ExecutionInfo.ReportParameters;
			if (parameterInfoCollection == null)
			{
				parameterInfoCollection = GetCompiledReport(m_itemContext, rebuild: false, out ControlSnapshot _).Parameters;
			}
			else if (userSpecifiedValues == null)
			{
				return;
			}
			ParameterInfoCollection parameterInfoCollection2;
			if (userSpecifiedValues != null)
			{
				ParameterInfoCollection newParameters = ParameterInfoCollection.DecodeFromNameValueCollectionAndUserCulture(userSpecifiedValues);
				parameterInfoCollection2 = ParameterInfoCollection.Combine(parameterInfoCollection, newParameters, checkReadOnly: true, ignoreNewQueryParams: false, isParameterDefinitionUpdate: false, isSharedDataSetParameter: false, Localization.ClientPrimaryCulture);
			}
			else
			{
				parameterInfoCollection2 = parameterInfoCollection;
			}
			ParameterInfoCollection parameterInfoCollection3 = new ParameterInfoCollection();
			parameterInfoCollection2.CopyTo(parameterInfoCollection3);
			ProcessAndStoreReportParameters(parameterInfoCollection3);
		}

		public void SetReportDataSourceCredentials(DatasourceCredentials[] credentials)
		{
			DatasourceCredentialsCollection credentials2 = m_executionSession.Credentials;
			credentials2.Clear();
			if (credentials != null)
			{
				foreach (DatasourceCredentials datasourceCred in credentials)
				{
					credentials2.Add(datasourceCred);
				}
			}
			ProcessAndStoreReportParameters(m_executionSession.ExecutionInfo.ReportParameters);
		}

		private void ProcessAndStoreReportParameters(ParameterInfoCollection newParameters)
		{
			GetCompiledReport(m_itemContext, rebuild: false, out ControlSnapshot snapshot);
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext pc = CreateProcessingContext(newParameters, null, m_executionSession.Credentials, snapshot, @object.StreamCallback, CreateSubreportCallbackHandler());
				CreateAndConfigureReportProcessing().ProcessReportParameters(DateTime.Now, pc, isSnapshot: false, out bool _);
				m_executionSession.ExecutionInfo.ReportParameters = newParameters;
			}
		}

		public int PerformSearch(int startPage, int endPage, string searchText)
		{
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = CreateProcessingContext(@object.StreamCallback);
				OnDemandProcessingResult result;
				return CreateAndConfigureReportProcessing().ProcessFindStringEvent(startPage, endPage, searchText, m_executionSession.EventInfo, processingContext, out result);
			}
		}

		public void PerformToggle(string toggleId)
		{
			if (toggleId != null)
			{
				CreateAndConfigureReportProcessing().ProcessToggleEvent(toggleId, m_executionSession.Snapshot, m_executionSession.EventInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged);
				if (showHideInfoChanged)
				{
					m_executionSession.EventInfo = newShowHideInfo;
				}
			}
		}

		public int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
		{
			uniqueName = null;
			if (bookmarkId == null)
			{
				return 0;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = CreateProcessingContext(@object.StreamCallback);
				OnDemandProcessingResult result;
				return CreateAndConfigureReportProcessing().ProcessBookmarkNavigationEvent(bookmarkId, m_executionSession.EventInfo, processingContext, out uniqueName, out result);
			}
		}

		public int PerformDocumentMapNavigation(string documentMapId)
		{
			if (documentMapId == null)
			{
				return 0;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = CreateProcessingContext(@object.StreamCallback);
				OnDemandProcessingResult result;
				return CreateAndConfigureReportProcessing().ProcessDocumentMapNavigationEvent(documentMapId, m_executionSession.EventInfo, processingContext, out result);
			}
		}

		public string PerformDrillthrough(string drillthroughId, out NameValueCollection resultParameters)
		{
			resultParameters = null;
			if (drillthroughId == null)
			{
				return null;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = CreateProcessingContext(@object.StreamCallback);
				OnDemandProcessingResult result;
				return CreateAndConfigureReportProcessing().ProcessDrillthroughEvent(drillthroughId, m_executionSession.EventInfo, processingContext, out resultParameters, out result);
			}
		}

		public int PerformSort(string paginationMode, string sortId, SortOptions sortDirection, bool clearSort, out string uniqueName)
		{
			SetProcessingCulture();
			ControlSnapshot snapshot = m_executionSession.Snapshot;
			try
			{
				m_executionSession.Snapshot = new ControlSnapshot();
				snapshot.PrepareExecutionSnapshot(m_executionSession.Snapshot, null);
				using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
				{
					m_itemContext.RSRequestParameters.PaginationModeValue = paginationMode;
					ReportProcessing reportProcessing = CreateAndConfigureReportProcessing();
					ProcessingContext pc = CreateProcessingContext(@object.StreamCallback);
					Microsoft.ReportingServices.ReportProcessing.RenderingContext rc = CreateRenderingContext();
					int page;
					OnDemandProcessingResult onDemandProcessingResult = reportProcessing.ProcessUserSortEvent(sortId, sortDirection, clearSort, pc, rc, snapshot, out uniqueName, out page);
					if (onDemandProcessingResult != null && onDemandProcessingResult.SnapshotChanged)
					{
						m_executionSession.SaveProcessingResult(onDemandProcessingResult);
					}
					else
					{
						m_executionSession.Snapshot = snapshot;
					}
					return page;
				}
			}
			catch
			{
				m_executionSession.Snapshot = snapshot;
				throw;
			}
		}

		public IDocumentMap GetDocumentMap()
		{
			if (!m_executionSession.ExecutionInfo.HasDocMap)
			{
				return null;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = CreateProcessingContext(@object.StreamCallback);
				OnDemandProcessingResult result;
				IDocumentMap documentMap = CreateAndConfigureReportProcessing().GetDocumentMap(m_executionSession.EventInfo, processingContext, out result);
				m_executionSession.SaveProcessingResult(result);
				return documentMap;
			}
		}

		public abstract IEnumerable<LocalRenderingExtensionInfo> ListRenderingExtensions();

		protected abstract IRenderingExtension CreateRenderer(string format, bool allowInternal);

		public ProcessingMessageList Render(string format, string deviceInfo, string paginationMode, bool allowInternalRenderers, IEnumerable dataSources, CreateAndRegisterStream createStreamCallback)
		{
			SetProcessingCulture();
			m_itemContext.RSRequestParameters.FormatParamValue = format;
			m_itemContext.RSRequestParameters.SetRenderingParameters(deviceInfo);
			m_itemContext.RSRequestParameters.PaginationModeValue = paginationMode;
			ReportProcessing reportProcessing = CreateAndConfigureReportProcessing();
			IRenderingExtension renderingExtension = CreateRenderer(format, allowInternalRenderers);
			bool flag = false;
			if (format == null || m_executionSession.Snapshot == null)
			{
				ReinitializeSnapshot(null);
				flag = true;
			}
			SubreportCallbackHandler subreportHandler = CreateSubreportCallbackHandler();
			ProcessingContext pc = CreateProcessingContext(m_executionSession.ExecutionInfo.ReportParameters, dataSources, m_executionSession.Credentials, m_executionSession.Snapshot, createStreamCallback, subreportHandler);
			Microsoft.ReportingServices.ReportProcessing.RenderingContext rc = CreateRenderingContext();
			OnDemandProcessingResult onDemandProcessingResult = null;
			if (flag)
			{
				try
				{
					if (renderingExtension == null)
					{
						onDemandProcessingResult = reportProcessing.CreateSnapshot(DateTime.Now, pc, null);
					}
					else
					{
						m_itemContext.RSRequestParameters.SetReportParameters(m_executionSession.ExecutionInfo.ReportParameters.AsNameValueCollectionInUserCulture);
						onDemandProcessingResult = CreateSnapshotAndRender(reportProcessing, renderingExtension, pc, rc, subreportHandler, m_executionSession.ExecutionInfo.ReportParameters, m_executionSession.Credentials);
					}
				}
				catch
				{
					m_executionSession.Snapshot = null;
					throw;
				}
			}
			else if (renderingExtension != null)
			{
				onDemandProcessingResult = reportProcessing.RenderSnapshot(renderingExtension, rc, pc);
			}
			m_executionSession.SaveProcessingResult(onDemandProcessingResult);
			return onDemandProcessingResult.Warnings;
		}

		public byte[] RenderStream(string format, string deviceInfo, string streamID, out string mimeType)
		{
			if (m_executionSession.Snapshot != null)
			{
				Stream chunk = m_executionSession.Snapshot.GetChunk(streamID, ReportProcessing.ReportChunkTypes.StaticImage, out mimeType);
				if (chunk == null)
				{
					chunk = m_executionSession.Snapshot.GetChunk(streamID, ReportProcessing.ReportChunkTypes.Image, out mimeType);
				}
				if (chunk != null)
				{
					byte[] array = new byte[chunk.Length];
					chunk.Read(array, 0, (int)chunk.Length);
					return array;
				}
			}
			using (StreamCache streamCache = new StreamCache())
			{
				m_itemContext.RSRequestParameters.SetRenderingParameters(deviceInfo);
				ReportProcessing reportProcessing = CreateAndConfigureReportProcessing();
				IRenderingExtension newRenderer = CreateRenderer(format, allowInternal: true);
				Microsoft.ReportingServices.ReportProcessing.RenderingContext rc = CreateRenderingContext();
				ProcessingContext pc = CreateProcessingContext(streamCache.StreamCallback);
				OnDemandProcessingResult result = reportProcessing.RenderSnapshotStream(newRenderer, streamID, rc, pc);
				m_executionSession.SaveProcessingResult(result);
				string encoding;
				string fileExtension;
				return streamCache.GetMainStream(out encoding, out mimeType, out fileExtension);
			}
		}

		protected virtual OnDemandProcessingResult CreateSnapshotAndRender(ReportProcessing repProc, IRenderingExtension renderer, ProcessingContext pc, Microsoft.ReportingServices.ReportProcessing.RenderingContext rc, SubreportCallbackHandler subreportHandler, ParameterInfoCollection parameters, DatasourceCredentialsCollection credentials)
		{
			return repProc.RenderReport(renderer, DateTime.Now, pc, rc, null);
		}

		private DataSetInfoCollection ResolveSharedDataSets(DataSetInfoCollection reportDataSets)
		{
			DataSetInfoCollection dataSetInfoCollection = new DataSetInfoCollection();
			foreach (DataSetInfo reportDataSet in reportDataSets)
			{
				DataSetInfo dataSetInfo;
				if (reportDataSet.IsValidReference())
				{
					dataSetInfo = reportDataSet;
				}
				else
				{
					dataSetInfo = m_catalog.GetDataSet(reportDataSet.AbsolutePath);
					if (dataSetInfo == null)
					{
						throw new ItemNotFoundException(reportDataSet.DataSetName);
					}
					dataSetInfo.ID = reportDataSet.ID;
					dataSetInfo.DataSetName = reportDataSet.DataSetName;
				}
				dataSetInfoCollection.Add(dataSetInfo);
			}
			return dataSetInfoCollection;
		}

		private DataSourceInfoCollection ResolveSharedDataSources(DataSourceInfoCollection reportDataSources)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection();
			foreach (DataSourceInfo reportDataSource in reportDataSources)
			{
				if (!reportDataSource.IsReference)
				{
					dataSourceInfoCollection.Add(reportDataSource);
					continue;
				}
				string dataSourceReference = reportDataSource.DataSourceReference;
				DataSourceInfo sharedDataSource = GetSharedDataSource(dataSourceReference);
				if (sharedDataSource == null)
				{
					throw new ItemNotFoundException(dataSourceReference);
				}
				sharedDataSource.ID = reportDataSource.ID;
				sharedDataSource.Name = reportDataSource.Name;
				sharedDataSource.OriginalName = reportDataSource.OriginalName;
				dataSourceInfoCollection.Add(sharedDataSource);
			}
			return dataSourceInfoCollection;
		}

		protected virtual DataSourceInfo GetSharedDataSource(string dataSourcePath)
		{
			return m_catalog.GetDataSource(dataSourcePath);
		}

		public string[] GetDataSetNames(PreviewItemContext itemContext)
		{
			if (DataRetrieval.SupportsQueries)
			{
				return new string[0];
			}
			ControlSnapshot snapshot;
			return GetCompiledReport(itemContext ?? m_itemContext, rebuild: false, out snapshot).DataSetsName ?? new string[0];
		}

		public DataSourcePromptCollection GetReportDataSourcePrompts(out bool allCredentialsSatisfied)
		{
			GetAllReportDataSourcesAndSharedDataSets(out RuntimeDataSourceInfoCollection runtimeDataSources, out RuntimeDataSetInfoCollection _);
			DatasourceCredentialsCollection datasourceCredentialsCollection = new DatasourceCredentialsCollection();
			foreach (DatasourceCredentials credential in m_executionSession.Credentials)
			{
				DataSourceInfo byOriginalName = runtimeDataSources.GetByOriginalName(credential.PromptID);
				if (byOriginalName != null && byOriginalName.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
				{
					datasourceCredentialsCollection.Add(credential);
				}
			}
			runtimeDataSources.SetCredentials(datasourceCredentialsCollection, DataProtectionLocal.Instance);
			ServerDataSourceSettings serverDatasourceSettings = new ServerDataSourceSettings(isSurrogatePresent: true, allowIntegratedSecurity: true);
			DataSourcePromptCollection promptRepresentatives = runtimeDataSources.GetPromptRepresentatives(serverDatasourceSettings);
			allCredentialsSatisfied = !runtimeDataSources.NeedPrompt;
			return promptRepresentatives;
		}

		private void GetAllReportDataSourcesAndSharedDataSets(out RuntimeDataSourceInfoCollection runtimeDataSources, out RuntimeDataSetInfoCollection runtimeDataSets)
		{
			if (!DataRetrieval.SupportsQueries)
			{
				runtimeDataSources = new RuntimeDataSourceInfoCollection();
				runtimeDataSets = new RuntimeDataSetInfoCollection();
				return;
			}
			ControlSnapshot snapshot;
			PublishingResult compiledReport = GetCompiledReport(m_itemContext, rebuild: false, out snapshot);
			DataSourceInfoCollection existingDataSources = ResolveSharedDataSources(compiledReport.DataSources);
			DataSetInfoCollection dataSetInfoCollection = ResolveSharedDataSets(compiledReport.SharedDataSets);
			DataSourceInfoCollection dataSources = CompileDataSetsAndCombineDataSources(dataSetInfoCollection, existingDataSources);
			CreateAndConfigureReportProcessing().GetAllDataSources(m_itemContext, snapshot, OnGetSubReportDataSources, dataSources, dataSetInfoCollection, checkIfUsable: true, new ServerDataSourceSettings(isSurrogatePresent: true, allowIntegratedSecurity: true), out runtimeDataSources, out runtimeDataSets);
		}

		private DataSourceInfoCollection CompileDataSetsAndCombineDataSources(DataSetInfoCollection dataSets, DataSourceInfoCollection existingDataSources)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection(existingDataSources);
			foreach (DataSetInfo dataSet in dataSets)
			{
				ICatalogItemContext dataSetContext = m_itemContext.GetDataSetContext(dataSet.AbsolutePath);
				DataSetPublishingResult compiledDataSet = GetCompiledDataSet(dataSet, dataSetContext);
				compiledDataSet.DataSourceInfo.OriginalName = compiledDataSet.DataSourceInfo.ID.ToString();
				dataSourceInfoCollection.Add(compiledDataSet.DataSourceInfo);
				dataSet.DataSourceId = compiledDataSet.DataSourceInfo.ID;
			}
			return dataSourceInfoCollection;
		}

		private DataSetPublishingResult GetCompiledDataSet(DataSetInfo dataSetInfo, ICatalogItemContext dataSetContext)
		{
			StoredDataSet storedDataSet = m_catalogTempDB.GetCompiledDataSet(dataSetInfo);
			if (storedDataSet != null && !storedDataSet.Definition.SequenceEqual(dataSetInfo.Definition))
			{
				storedDataSet = null;
			}
			if (storedDataSet == null)
			{
				DataSetPublishingResult result;
				try
				{
					using (ControlSnapshot createChunkFactory = new ControlSnapshot())
					{
						ReportProcessing reportProcessing = CreateAndConfigureReportProcessing();
						PublishingContext sharedDataSetPublishingContext = new PublishingContext(dataSetContext, dataSetInfo.Definition, createChunkFactory, AppDomain.CurrentDomain, generateExpressionHostWithRefusedPermissions: true, GetDataSourceForSharedDataSetHandler, reportProcessing.Configuration);
						result = reportProcessing.CreateSharedDataSet(sharedDataSetPublishingContext);
					}
				}
				catch (Exception inner)
				{
					throw new DefinitionInvalidException(dataSetInfo.AbsolutePath, inner);
				}
				storedDataSet = new StoredDataSet(dataSetInfo.Definition, result);
				m_catalogTempDB.SetCompiledDataSet(dataSetInfo, storedDataSet);
			}
			return storedDataSet.PublishingResult;
		}

		private DataSourceInfo GetDataSourceForSharedDataSetHandler(string dataSourcePath, out Guid catalogItemId)
		{
			DataSourceInfo dataSource = m_catalog.GetDataSource(dataSourcePath);
			if (dataSource == null)
			{
				catalogItemId = Guid.Empty;
			}
			else
			{
				catalogItemId = dataSource.ID;
			}
			return dataSource;
		}

		protected abstract SubreportCallbackHandler CreateSubreportCallbackHandler();

		private void OnGetSubReportDefinition(ICatalogItemContext reportContext, string subreportPath, string newChunkName, ReportProcessing.NeedsUpgrade upgradeCheck, ParameterInfoCollection parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out IChunkFactory chunkFactory, out ParameterInfoCollection parameters)
		{
			if (reportContext == null)
			{
				throw new ArgumentException("OnGetSubReportDefinition: Invalid report context");
			}
			if (upgradeCheck(ReportProcessingFlags.OnDemandEngine))
			{
				throw new Exception("Subreport definition is not compatible with this version of viewer controls");
			}
			subreportContext = reportContext.GetSubreportContext(subreportPath);
			ControlSnapshot snapshot;
			PublishingResult compiledReport = GetCompiledReport((PreviewItemContext)subreportContext, rebuild: false, out snapshot);
			snapshot.PrepareExecutionSnapshot(m_executionSession.Snapshot, newChunkName);
			description = compiledReport.ReportDescription;
			chunkFactory = snapshot;
			parameters = compiledReport.Parameters;
		}

		private void OnGetSubReportDataSources(ICatalogItemContext itemContext, string subreportPath, ReportProcessing.NeedsUpgrade upgradeCheck, out ICatalogItemContext subreportContext, out IChunkFactory getCompiledDefinition, out DataSourceInfoCollection dataSources, out DataSetInfoCollection dataSets)
		{
			subreportPath = NormalizeSubReportPath(subreportPath);
			subreportContext = itemContext.GetSubreportContext(subreportPath);
			RSTrace.ReportPreviewTracer.Trace(TraceLevel.Info, "Getting datasources information for {0}.", subreportContext.ItemPathAsString);
			ControlSnapshot snapshot;
			PublishingResult compiledReport = GetCompiledReport((PreviewItemContext)subreportContext, rebuild: false, out snapshot);
			getCompiledDefinition = snapshot;
			dataSources = ResolveSharedDataSources(compiledReport.DataSources);
			dataSets = ResolveSharedDataSets(compiledReport.SharedDataSets);
		}

		private string NormalizeSubReportPath(string pathFromRdl)
		{
			if (pathFromRdl != null && pathFromRdl.Length > 1 && pathFromRdl[0] != '/')
			{
				return "/" + pathFromRdl;
			}
			return pathFromRdl;
		}

		protected abstract void SetProcessingCulture();

		protected abstract IConfiguration CreateProcessingConfiguration();

		private ReportProcessing CreateAndConfigureReportProcessing()
		{
			return new ReportProcessing
			{
				Configuration = CreateProcessingConfiguration()
			};
		}

		protected ProcessingContext CreateProcessingContext(CreateAndRegisterStream createStreamCallback)
		{
			return CreateProcessingContext(m_executionSession.ExecutionInfo.ReportParameters, null, null, m_executionSession.Snapshot, createStreamCallback, CreateSubreportCallbackHandler());
		}

		protected ProcessingContext CreateProcessingContext(ParameterInfoCollection reportParameters, IEnumerable dataSources, DatasourceCredentialsCollection credentials, IChunkFactory chunkFactory, CreateAndRegisterStream createStreamCallback, SubreportCallbackHandler subreportHandler)
		{
			RuntimeDataSourceInfoCollection runtimeDataSources = null;
			RuntimeDataSetInfoCollection runtimeDataSets = null;
			GetAllReportDataSourcesAndSharedDataSets(out runtimeDataSources, out runtimeDataSets);
			return m_dataRetrieval.CreateProcessingContext(m_itemContext, reportParameters, dataSources, runtimeDataSources, runtimeDataSets, GetCompiledDataSet, credentials, subreportHandler.OnDemandSubReportCallback, new GetResourceForLocalService(Catalog), chunkFactory, m_reportRuntimeSetupHandler.ReportRuntimeSetup, createStreamCallback);
		}

		private Microsoft.ReportingServices.ReportProcessing.RenderingContext CreateRenderingContext()
		{
			LocalExecutionInfo executionInfo = m_executionSession.ExecutionInfo;
			int num = executionInfo.TotalPages;
			if (executionInfo.PaginationMode != PaginationMode.TotalPages && num > 0)
			{
				num = -num;
			}
			PaginationMode clientPaginationMode = PaginationMode.Progressive;
			if (string.Compare(m_itemContext.RSRequestParameters.PaginationModeValue, "Actual", StringComparison.OrdinalIgnoreCase) == 0)
			{
				clientPaginationMode = PaginationMode.TotalPages;
			}
			return new Microsoft.ReportingServices.ReportProcessing.RenderingContext(m_itemContext, "", m_executionSession.EventInfo, m_reportRuntimeSetupHandler.ReportRuntimeSetup, null, UserProfileState.Both, clientPaginationMode, num);
		}

		private PublishingResult GetCompiledReport(PreviewItemContext itemContext, bool rebuild, out ControlSnapshot snapshot)
		{
			StoredReport storedReport = null;
			if (!rebuild)
			{
				storedReport = m_catalogTempDB.GetCompiledReport(itemContext);
				if (storedReport != null && storedReport.GeneratedExpressionHostWithRefusedPermissions != GenerateExpressionHostWithRefusedPermissions)
				{
					storedReport = null;
				}
			}
			if (storedReport == null)
			{
				byte[] reportDefinition = m_catalog.GetReportDefinition(itemContext);
				storedReport = new StoredReport(ReportCompiler.CompileReport(itemContext, reportDefinition, GenerateExpressionHostWithRefusedPermissions, out snapshot), snapshot, GenerateExpressionHostWithRefusedPermissions);
				m_catalogTempDB.SetCompiledReport(itemContext, storedReport);
				ILocalCatalog2 localCatalog = m_catalog as ILocalCatalog2;
				foreach (DataSourceInfo dataSource in storedReport.PublishingResult.DataSources)
				{
					string userName = null;
					string password = null;
					if (localCatalog != null && localCatalog.GetReportDataSourceCredentials(itemContext, dataSource, out userName, out password))
					{
						dataSource.SetUserName(userName, DataProtectionLocal.Instance);
						dataSource.SetPassword(password, DataProtectionLocal.Instance);
						dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.Store;
					}
					else
					{
						if (dataSource.IsReference || dataSource.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Integrated)
						{
							continue;
						}
						if (localCatalog == null)
						{
							m_catalog.GetReportDataSourceCredentials(itemContext, dataSource.Name, out userName, out password);
							bool num = !string.IsNullOrEmpty(userName);
							if (num)
							{
								dataSource.SetUserName(userName, DataProtectionLocal.Instance);
							}
							bool flag = password != null;
							if (flag)
							{
								dataSource.SetPassword(password, DataProtectionLocal.Instance);
							}
							if (num || flag)
							{
								dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.Store;
								continue;
							}
						}
						if (string.IsNullOrEmpty(dataSource.Prompt))
						{
							dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.None;
						}
						else
						{
							dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.Prompt;
						}
					}
				}
			}
			m_securityValidator(itemContext, storedReport.PublishingResult);
			snapshot = storedReport.Snapshot;
			return storedReport.PublishingResult;
		}

		void ILocalProcessingHost.CompileReport()
		{
			CompileReport();
		}

		public PublishingResult CompileReport()
		{
			ControlSnapshot snapshot;
			PublishingResult compiledReport = GetCompiledReport(m_itemContext, rebuild: true, out snapshot);
			m_executionSession.CompiledReport = snapshot;
			m_executionSession.ExecutionInfo.PageProperties = compiledReport.PageProperties;
			m_executionSession.CompiledDataSources = compiledReport.DataSources;
			return compiledReport;
		}

		public void ResetExecution()
		{
			ResetExecution(RecompileOnResetExecution);
		}

		private void ResetExecution(bool forceRecompile)
		{
			if (forceRecompile)
			{
				DatasourceCredentialsCollection datasourceCredentialsCollection = null;
				ParameterInfoCollection reportParameters = null;
				if (RecompileOnResetExecution)
				{
					datasourceCredentialsCollection = m_executionSession.Credentials;
					reportParameters = m_executionSession.ExecutionInfo.ReportParameters;
				}
				m_executionSession = new LocalExecutionSession();
				if (datasourceCredentialsCollection != null)
				{
					foreach (DatasourceCredentials item in datasourceCredentialsCollection)
					{
						m_executionSession.Credentials.Add(item);
					}
				}
				m_executionSession.ExecutionInfo.ReportParameters = reportParameters;
			}
			else
			{
				m_executionSession.ResetExecution();
			}
		}

		protected void ReinitializeSnapshot(ProcessingContext pc)
		{
			m_executionSession.Snapshot = new ControlSnapshot();
			m_executionSession.CompiledReport.PrepareExecutionSnapshot(m_executionSession.Snapshot, null);
			if (pc != null)
			{
				pc.ChunkFactory = m_executionSession.Snapshot;
			}
		}

		public void ExecuteReportInCurrentAppDomain(Evidence reportEvidence)
		{
			if (reportEvidence == null)
			{
				m_reportRuntimeSetupHandler.ExecuteReportInCurrentAppDomain();
			}
			else
			{
				m_reportRuntimeSetupHandler.ExecuteReportInCurrentAppDomain(reportEvidence);
			}
		}

		public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			m_reportRuntimeSetupHandler.AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
		}

		public void ExecuteReportInSandboxAppDomain()
		{
			m_reportRuntimeSetupHandler.ExecuteReportInSandboxAppDomain();
		}

		public void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			m_reportRuntimeSetupHandler.AddFullTrustModuleInSandboxAppDomain(assemblyName);
		}

		public void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
		{
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions");
			}
			m_reportRuntimeSetupHandler.SetBasePermissionsForSandboxAppDomain(permissions);
		}

		public void ReleaseSandboxAppDomain()
		{
			m_reportRuntimeSetupHandler.ReleaseSandboxAppDomain();
		}
	}
}
