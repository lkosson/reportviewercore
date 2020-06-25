#define TRACE
using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class LocalReport : Report, ISerializable, IDisposable
	{
		private const string TopLevelDirectReportDefinitionPath = "";

		private string m_reportPath;

		private string m_reportEmbeddedResource;

		private Assembly m_embeddedResourceAssembly;

		private bool m_enableHyperlinks;

		private bool m_enableExternalImages;

		private NameValueCollection m_parentSuppliedParameters;

		private ReportDataSourceCollection m_dataSources;

		private ProcessingMessageList m_lastRenderingWarnings;

		private readonly ILocalProcessingHost m_processingHost;

		private RenderingExtension[] m_externalRenderingExtensions;

		[NonSerialized]
		private MapTileServerConfiguration m_mapTileServerConfiguration;

		internal override string DisplayNameForUse
		{
			get
			{
				lock (m_syncObject)
				{
					if (string.IsNullOrEmpty(base.DisplayName))
					{
						PreviewItemContext itemContext = m_processingHost.ItemContext;
						if (itemContext != null)
						{
							string text = itemContext.ItemName;
							if (string.IsNullOrEmpty(text))
							{
								text = CommonStrings.Report;
							}
							return text;
						}
						return string.Empty;
					}
					return base.DisplayName;
				}
			}
		}

		internal bool SupportsQueries => m_processingHost.SupportsQueries;

		internal override bool CanSelfCancel => m_processingHost.CanSelfCancel;

		private DefinitionSource DefinitionSource
		{
			get
			{
				if (!string.IsNullOrEmpty(ReportPath))
				{
					return DefinitionSource.File;
				}
				if (!string.IsNullOrEmpty(ReportEmbeddedResource))
				{
					return DefinitionSource.EmbeddedResource;
				}
				if (m_processingHost.Catalog.HasDirectReportDefinition(""))
				{
					return DefinitionSource.Direct;
				}
				return DefinitionSource.Unknown;
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(null)]
		[SRDescription("LocalReportPathDesc")]
		public string ReportPath
		{
			get
			{
				return m_reportPath;
			}
			set
			{
				DemandFullTrustWithFriendlyMessage();
				lock (m_syncObject)
				{
					if (string.Compare(value, ReportPath, StringComparison.Ordinal) != 0)
					{
						ChangeReportDefinition(DefinitionSource.File, delegate
						{
							m_reportPath = value;
						});
					}
				}
			}
		}

		[NotifyParentProperty(true)]
		[TypeConverter("Microsoft.ReportingServices.ReportSelectionConverter, Microsoft.Reporting.Design, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91")]
		[DefaultValue(null)]
		[SRDescription("ReportEmbeddedResourceDesc")]
		public string ReportEmbeddedResource
		{
			get
			{
				return m_reportEmbeddedResource;
			}
			set
			{
				DemandFullTrustWithFriendlyMessage();
				lock (m_syncObject)
				{
					if (string.Compare(value, ReportEmbeddedResource, StringComparison.Ordinal) != 0)
					{
						SetEmbeddedResourceAsReportDefinition(value, Assembly.GetCallingAssembly());
					}
				}
			}
		}

		[Category("Security")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[SRDescription("EnableExternalImagesDesc")]
		public bool EnableExternalImages
		{
			get
			{
				return m_enableExternalImages;
			}
			set
			{
				lock (m_syncObject)
				{
					m_enableExternalImages = value;
				}
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(true)]
		[SRDescription("ShowDetailedSubreportMessagesDesc")]
		public bool ShowDetailedSubreportMessages
		{
			get
			{
				return m_processingHost.ShowDetailedSubreportMessages;
			}
			set
			{
				lock (m_syncObject)
				{
					if (m_processingHost.ShowDetailedSubreportMessages != value)
					{
						m_processingHost.ShowDetailedSubreportMessages = value;
						OnChange(isRefreshOnly: false);
					}
				}
			}
		}

		[Category("Security")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[SRDescription("EnableHyperlinksDesc")]
		public bool EnableHyperlinks
		{
			get
			{
				return m_enableHyperlinks;
			}
			set
			{
				lock (m_syncObject)
				{
					m_enableHyperlinks = value;
				}
			}
		}

		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("MapTileServerConfigurationDesc")]
		public MapTileServerConfiguration MapTileServerConfiguration => m_mapTileServerConfiguration;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("ReportDataSourcesDesc")]
		[NotifyParentProperty(true)]
		public ReportDataSourceCollection DataSources => m_dataSources;

		internal override bool IsReadyForConnection => DefinitionSource != DefinitionSource.Unknown;

		internal override bool IsPreparedReportReadyForRendering
		{
			get
			{
				foreach (string dataSourceName in GetDataSourceNames())
				{
					if (DataSources[dataSourceName] == null)
					{
						return false;
					}
				}
				GetDataSources(out bool allCredentialsSatisfied);
				if (!allCredentialsSatisfied)
				{
					return false;
				}
				foreach (ReportParameterInfo parameter in GetParameters())
				{
					if (parameter.State != 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		internal bool HasExecutionSession => m_processingHost.ExecutionInfo.IsCompiled;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<ReportParameter> OriginalParametersToDrillthrough => new ReadOnlyCollection<ReportParameter>(ReportParameter.FromNameValueCollection(m_parentSuppliedParameters));

		internal override bool HasDocMap
		{
			get
			{
				try
				{
					lock (m_syncObject)
					{
						return m_processingHost.ExecutionInfo.HasDocMap;
					}
				}
				catch (SecurityException processingException)
				{
					throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
				}
			}
		}

		internal override int AutoRefreshInterval
		{
			get
			{
				lock (m_syncObject)
				{
					return m_processingHost.ExecutionInfo.AutoRefreshInterval;
				}
			}
		}

		[SRDescription("SubreportProcessingEventDesc")]
		public event SubreportProcessingEventHandler SubreportProcessing;

		internal LocalReport(ILocalProcessingHost processingHost)
		{
			m_processingHost = processingHost;
			m_dataSources = new ReportDataSourceCollection(m_syncObject);
			Construct();
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		internal LocalReport(SerializationInfo info, StreamingContext context)
		{
			base.DisplayName = info.GetString("DisplayName");
			m_reportPath = info.GetString("ReportPath");
			m_reportEmbeddedResource = info.GetString("ReportEmbeddedResource");
			m_embeddedResourceAssembly = (Assembly)info.GetValue("EmbeddedResourceAssembly", typeof(Assembly));
			m_dataSources = (ReportDataSourceCollection)info.GetValue("DataSources", typeof(ReportDataSourceCollection));
			m_dataSources.SetSyncObject(m_syncObject);
			m_processingHost = (ILocalProcessingHost)info.GetValue("ControlService", typeof(ILocalProcessingHost));
			base.DrillthroughDepth = info.GetInt32("DrillthroughDepth");
			m_enableExternalImages = info.GetBoolean("EnableExternalImages");
			m_enableHyperlinks = info.GetBoolean("EnableHyperlinks");
			m_parentSuppliedParameters = (NameValueCollection)info.GetValue("ParentSuppliedParameters", typeof(NameValueCollection));
			Construct();
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("DisplayName", base.DisplayName);
			info.AddValue("ReportPath", m_reportPath);
			info.AddValue("ReportEmbeddedResource", m_reportEmbeddedResource);
			info.AddValue("EmbeddedResourceAssembly", m_embeddedResourceAssembly);
			info.AddValue("DataSources", m_dataSources);
			info.AddValue("ControlService", m_processingHost);
			info.AddValue("DrillthroughDepth", base.DrillthroughDepth);
			info.AddValue("EnableExternalImages", m_enableExternalImages);
			info.AddValue("EnableHyperlinks", m_enableHyperlinks);
			info.AddValue("ParentSuppliedParameters", m_parentSuppliedParameters);
		}

		private void Construct()
		{
			LocalService localService = m_processingHost as LocalService;
			if (localService != null)
			{
				localService.DataRetrieval = CreateDataRetrieval();
				localService.SecurityValidator = ValidateReportSecurity;
			}
			DataSources.Change += base.OnChange;
			base.Change += OnLocalReportChange;
			if (m_processingHost.MapTileServerConfiguration != null)
			{
				m_mapTileServerConfiguration = new MapTileServerConfiguration(m_processingHost.MapTileServerConfiguration);
			}
		}

		public void Dispose()
		{
			(m_processingHost as IDisposable)?.Dispose();
		}

		internal override void SetCancelState(bool shouldCancelRequests)
		{
			m_processingHost.SetCancelState(shouldCancelRequests);
		}

		private void DemandFullTrustWithFriendlyMessage()
		{
			try
			{
				new SecurityPermission(PermissionState.Unrestricted).Demand();
			}
			catch (SecurityException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors);
			}
		}

		private void SetEmbeddedResourceAsReportDefinition(string resourceName, Assembly assemblyWithResource)
		{
			ChangeReportDefinition(DefinitionSource.EmbeddedResource, delegate
			{
				if (string.IsNullOrEmpty(resourceName))
				{
					assemblyWithResource = null;
				}
				m_reportEmbeddedResource = resourceName;
				m_embeddedResourceAssembly = assemblyWithResource;
			});
		}

		internal void SetDataSourceCredentials(IEnumerable credentials)
		{
			if (credentials == null)
			{
				throw new ArgumentNullException("credentials");
			}
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				List<DatasourceCredentials> list = new List<DatasourceCredentials>();
				foreach (DataSourceCredentials credential in credentials)
				{
					list.Add(new DatasourceCredentials(credential.Name, credential.UserId, credential.Password));
				}
				m_processingHost.SetReportDataSourceCredentials(list.ToArray());
				OnChange(isRefreshOnly: false);
			}
		}

		internal ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied)
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				DataSourcePromptCollection reportDataSourcePrompts;
				try
				{
					reportDataSourcePrompts = m_processingHost.GetReportDataSourcePrompts(out allCredentialsSatisfied);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				List<ReportDataSourceInfo> list = new List<ReportDataSourceInfo>(reportDataSourcePrompts.Count);
				foreach (DataSourceInfo item in reportDataSourcePrompts)
				{
					list.Add(new ReportDataSourceInfo(item.PromptIdentifier, item.Prompt));
				}
				return new ReportDataSourceInfoCollection(list);
			}
		}

		public IList<string> GetDataSourceNames()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				string[] dataSetNames;
				try
				{
					dataSetNames = m_processingHost.GetDataSetNames(null);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				return new ReadOnlyCollection<string>(dataSetNames);
			}
		}

		public override int GetTotalPages(out PageCountMode pageCountMode)
		{
			lock (m_syncObject)
			{
				LocalExecutionInfo executionInfo = m_processingHost.ExecutionInfo;
				if (executionInfo.PaginationMode == PaginationMode.TotalPages)
				{
					pageCountMode = PageCountMode.Actual;
				}
				else
				{
					pageCountMode = PageCountMode.Estimate;
				}
				return executionInfo.TotalPages;
			}
		}

		public override void LoadReportDefinition(TextReader report)
		{
			lock (m_syncObject)
			{
				if (report == null)
				{
					throw new ArgumentNullException("report");
				}
				SetDirectReportDefinition("", report);
			}
		}

		public void LoadSubreportDefinition(string reportName, TextReader report)
		{
			lock (m_syncObject)
			{
				if (reportName == null)
				{
					throw new ArgumentNullException("reportName");
				}
				if (reportName.Length == 0)
				{
					throw new ArgumentOutOfRangeException("reportName");
				}
				if (report == null)
				{
					throw new ArgumentNullException("report");
				}
				SetDirectReportDefinition(reportName, report);
			}
		}

		public void LoadSubreportDefinition(string reportName, Stream report)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			LoadSubreportDefinition(reportName, new StreamReader(report));
		}

		private void SetDirectReportDefinition(string reportName, TextReader report)
		{
			DemandFullTrustWithFriendlyMessage();
			string s = report.ReadToEnd();
			byte[] reportBytes = Encoding.UTF8.GetBytes(s);
			ChangeReportDefinition(DefinitionSource.Direct, delegate
			{
				m_processingHost.Catalog.SetReportDefinition(reportName, reportBytes);
			});
		}

		internal override int PerformSearch(string searchText, int startPage, int endPage)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					if (string.IsNullOrEmpty(searchText))
					{
						return -1;
					}
					try
					{
						return m_processingHost.PerformSearch(startPage, endPage, searchText);
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		internal override void PerformToggle(string toggleId)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					try
					{
						m_processingHost.PerformToggle(toggleId);
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		internal override int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					try
					{
						return m_processingHost.PerformBookmarkNavigation(bookmarkId, out uniqueName);
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		internal override int PerformDocumentMapNavigation(string documentMapId)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					try
					{
						return m_processingHost.PerformDocumentMapNavigation(documentMapId);
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		internal override Report PerformDrillthrough(string drillthroughId, out string reportPath)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					if (drillthroughId == null)
					{
						throw new ArgumentNullException("drillthroughId");
					}
					NameValueCollection parametersForDrillthroughReport;
					try
					{
						reportPath = m_processingHost.PerformDrillthrough(drillthroughId, out parametersForDrillthroughReport);
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
					LocalReport localReport = CreateNewLocalReport();
					string reportPath2 = CreateItemContext().MapUserProvidedPath(reportPath);
					PopulateDrillthroughReport(reportPath2, parametersForDrillthroughReport, localReport);
					return localReport;
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		public override ReportPageSettings GetDefaultPageSettings()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				PageProperties pageProperties = m_processingHost.ExecutionInfo.PageProperties;
				return new ReportPageSettings(pageProperties.PageHeight, pageProperties.PageWidth, pageProperties.LeftMargin, pageProperties.RightMargin, pageProperties.TopMargin, pageProperties.BottomMargin);
			}
		}

		private void PopulateDrillthroughReport(string reportPath, NameValueCollection drillParams, LocalReport drillReport)
		{
			drillReport.CopySecuritySettings(this);
			if (ReportPath != null)
			{
				drillReport.ReportPath = reportPath;
			}
			else if (ReportEmbeddedResource != null)
			{
				drillReport.SetEmbeddedResourceAsReportDefinition(reportPath, m_embeddedResourceAssembly);
			}
			drillReport.DrillthroughDepth = base.DrillthroughDepth + 1;
			drillReport.m_parentSuppliedParameters = drillParams;
		}

		internal override int PerformSort(string sortId, SortOrder sortDirection, bool clearSort, PageCountMode pageCountMode, out string uniqueName)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					SortOptions sortOptions = (sortDirection == SortOrder.Ascending) ? SortOptions.Ascending : SortOptions.Descending;
					string paginationMode = PageCountModeToProcessingPaginationMode(pageCountMode);
					try
					{
						return m_processingHost.PerformSort(paginationMode, sortId, sortOptions, clearSort, out uniqueName);
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		[Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
		public void ExecuteReportInCurrentAppDomain(ReportingServices.Evidence reportEvidence)
		{
			try
			{
				lock (m_syncObject)
				{
					m_processingHost.ExecuteReportInCurrentAppDomain(reportEvidence);
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}

		[Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
		public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			try
			{
				lock (m_syncObject)
				{
					m_processingHost.AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}

		[Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
		public void ExecuteReportInSandboxAppDomain()
		{
			try
			{
				lock (m_syncObject)
				{
					m_processingHost.ExecuteReportInSandboxAppDomain();
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}

		public void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
		{
			try
			{
				lock (m_syncObject)
				{
					m_processingHost.AddFullTrustModuleInSandboxAppDomain(assemblyName);
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}

		public void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
		{
			try
			{
				lock (m_syncObject)
				{
					m_processingHost.SetBasePermissionsForSandboxAppDomain(permissions);
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}

		public void ReleaseSandboxAppDomain()
		{
			lock (m_syncObject)
			{
				m_processingHost.ReleaseSandboxAppDomain();
			}
		}

		private void CopySecuritySettings(LocalReport parentReport)
		{
			m_processingHost.CopySecuritySettingsFrom(parentReport.m_processingHost);
			m_enableExternalImages = parentReport.EnableExternalImages;
			m_enableHyperlinks = parentReport.EnableHyperlinks;
			ShowDetailedSubreportMessages = parentReport.ShowDetailedSubreportMessages;
		}

		internal override void EnsureExecutionSession()
		{
			if (DefinitionSource == DefinitionSource.Unknown)
			{
				throw new MissingReportSourceException();
			}
			try
			{
				if (!HasExecutionSession)
				{
					m_processingHost.CompileReport();
					m_processingHost.SetReportParameters(m_parentSuppliedParameters);
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
			catch (Exception processingException2)
			{
				throw WrapProcessingException(processingException2);
			}
		}

		private void ValidateReportSecurity(PreviewItemContext itemContext, PublishingResult publishingResult)
		{
			if (publishingResult.HasExternalImages && !EnableExternalImages)
			{
				throw new ReportSecurityException(CommonStrings.ExternalImagesError(itemContext.ItemName));
			}
			if (publishingResult.HasHyperlinks && !EnableHyperlinks)
			{
				throw new ReportSecurityException(CommonStrings.HyperlinkSecurityError(itemContext.ItemName));
			}
		}

		public override void Refresh()
		{
			try
			{
				lock (m_syncObject)
				{
					if (m_processingHost.ExecutionInfo.HasSnapshot)
					{
						m_processingHost.ResetExecution();
						OnChange(isRefreshOnly: true);
					}
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
		}

		private void ChangeReportDefinition(DefinitionSource updatingSourceType, System.Action changeAction)
		{
			DefinitionSource definitionSource = DefinitionSource;
			changeAction();
			DefinitionSource definitionSource2 = DefinitionSource;
			if (definitionSource2 == updatingSourceType || definitionSource2 != definitionSource)
			{
				m_processingHost.ItemContext = CreateItemContext();
				OnChange(isRefreshOnly: false);
			}
		}

		public override ReportParameterInfoCollection GetParameters()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				return ParameterInfoCollectionToApi(m_processingHost.ExecutionInfo.ReportParameters, SupportsQueries);
			}
		}

		internal override ParametersPaneLayout GetParametersPaneLayout()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				if (m_processingHost.ExecutionInfo.ReportParameters != null && m_processingHost.ExecutionInfo.ReportParameters.ParametersLayout != null)
				{
					ReportParameterInfoCollection paramsInfo = ParameterInfoCollectionToApi(m_processingHost.ExecutionInfo.ReportParameters, SupportsQueries);
					return BuildParameterPaneLayout(m_processingHost.ExecutionInfo.ReportParameters.ParametersLayout, paramsInfo);
				}
				return null;
			}
		}

		private ParametersPaneLayout BuildParameterPaneLayout(ParametersGridLayout processingParameterLayout, ReportParameterInfoCollection paramsInfo)
		{
			List<GridLayoutCellDefinition> list = new List<GridLayoutCellDefinition>(processingParameterLayout.CellDefinitions.Count);
			foreach (ParameterGridLayoutCellDefinition cellDefinition in processingParameterLayout.CellDefinitions)
			{
				list.Add(new GridLayoutCellDefinition
				{
					Column = cellDefinition.ColumnIndex,
					Row = cellDefinition.RowIndex,
					ParameterName = cellDefinition.ParameterName
				});
			}
			return new ParametersPaneLayout
			{
				GridLayoutDefinition = new GridLayoutDefinition(new GridLayoutCellDefinitionCollection(list), processingParameterLayout.NumberOfRows, processingParameterLayout.NumberOfColumns, paramsInfo)
			};
		}

		public override void SetParameters(IEnumerable<ReportParameter> parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				NameValueCollection reportParameters = ReportParameter.ToNameValueCollection(parameters);
				try
				{
					m_processingHost.SetReportParameters(reportParameters);
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				OnChange(isRefreshOnly: false);
			}
		}

		internal override byte[] InternalRenderStream(string format, string streamID, string deviceInfo, out string mimeType, out string encoding)
		{
			try
			{
				encoding = null;
				lock (m_syncObject)
				{
					return m_processingHost.RenderStream(format, deviceInfo, streamID, out mimeType);
				}
			}
			catch (SecurityException processingException)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
			}
			catch (Exception processingException2)
			{
				throw WrapProcessingException(processingException2);
			}
		}

		internal override void InternalDeliverReportItem(string format, string deviceInfo, ExtensionSettings settings, string description, string eventType, string matchData)
		{
			throw new NotImplementedException();
		}

		internal override DocumentMapNode GetDocumentMap(string rootLabel)
		{
			try
			{
				lock (m_syncObject)
				{
					if (!m_processingHost.ExecutionInfo.HasSnapshot)
					{
						throw new InvalidOperationException(CommonStrings.ReportNotReady);
					}
					IDocumentMap documentMap;
					try
					{
						documentMap = m_processingHost.GetDocumentMap();
					}
					catch (Exception processingException)
					{
						throw WrapProcessingException(processingException);
					}
					return DocumentMapNode.CreateTree(documentMap, rootLabel);
				}
			}
			catch (SecurityException processingException2)
			{
				throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
			}
		}

		public override byte[] Render(string format, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
		{
			return InternalRender(format, allowInternalRenderers: false, deviceInfo, pageCountMode, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
		}

		internal byte[] InternalRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
		{
			lock (m_syncObject)
			{
				using (StreamCache streamCache = new StreamCache())
				{
					InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, streamCache.StreamCallback, out warnings);
					streams = new string[0];
					return streamCache.GetMainStream(out encoding, out mimeType, out fileNameExtension);
				}
			}
		}

		public void Render(string format, string deviceInfo, CreateStreamCallback createStream, out Warning[] warnings)
		{
			Render(format, deviceInfo, PageCountMode.Estimate, createStream, out warnings);
		}

		public void Render(string format, string deviceInfo, PageCountMode pageCountMode, CreateStreamCallback createStream, out Warning[] warnings)
		{
			if (createStream == null)
			{
				throw new ArgumentNullException("createStream");
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler((string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation) => createStream(name, extension, encoding, mimeType, willSeek)))
			{
				InternalRender(format, allowInternalRenderers: false, deviceInfo, pageCountMode, CreateAndRegisterStreamTypeConverter.ToInnerType(@object.StreamCallback), out warnings);
			}
		}

		internal void InternalRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, CreateAndRegisterStream createStreamCallback, out Warning[] warnings)
		{
			lock (m_syncObject)
			{
				if (createStreamCallback == null)
				{
					throw new ArgumentNullException("createStreamCallback");
				}
				if (!ValidateRenderingFormat(format))
				{
					throw new ArgumentOutOfRangeException("format");
				}
				EnsureExecutionSession();
				try
				{
					m_lastRenderingWarnings = m_processingHost.Render(format, deviceInfo, PageCountModeToProcessingPaginationMode(pageCountMode), allowInternalRenderers, m_dataSources, createStreamCallback.ToOuterType());
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				warnings = Warning.FromProcessingMessageList(m_lastRenderingWarnings);
				WriteDebugResults(warnings);
			}
		}

		private void WriteDebugResults(Warning[] warnings)
		{
			foreach (Warning warning in warnings)
			{
				Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1} ({2})", warning.Severity, warning.Message, warning.Code));
			}
		}

		private bool ValidateRenderingFormat(string format)
		{
			try
			{
				foreach (LocalRenderingExtensionInfo item in m_processingHost.ListRenderingExtensions())
				{
					if (string.Compare(item.Name, format, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
				return false;
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
		}

		private IEnumerable ControlSubReportInfoCallback(PreviewItemContext subReportContext, ParameterInfoCollection initialParameters)
		{
			if (this.SubreportProcessing == null)
			{
				return null;
			}
			string[] dataSetNames;
			try
			{
				dataSetNames = m_processingHost.GetDataSetNames(subReportContext);
			}
			catch (Exception processingException)
			{
				throw WrapProcessingException(processingException);
			}
			SubreportProcessingEventArgs subreportProcessingEventArgs = new SubreportProcessingEventArgs(subReportContext.OriginalItemPath, ParameterInfoCollectionToApi(initialParameters, SupportsQueries), dataSetNames);
			this.SubreportProcessing(this, subreportProcessingEventArgs);
			return subreportProcessingEventArgs.DataSources;
		}

		public override RenderingExtension[] ListRenderingExtensions()
		{
			if (m_externalRenderingExtensions == null)
			{
				List<RenderingExtension> list = new List<RenderingExtension>();
				try
				{
					foreach (LocalRenderingExtensionInfo item in m_processingHost.ListRenderingExtensions())
					{
						if (item.IsExposedExternally)
						{
							list.Add(new RenderingExtension(item.Name, item.LocalizedName, item.IsVisible));
						}
					}
				}
				catch (Exception processingException)
				{
					throw WrapProcessingException(processingException);
				}
				m_externalRenderingExtensions = list.ToArray();
			}
			return m_externalRenderingExtensions;
		}

		private string GetFullyQualifiedReportPath()
		{
			switch (DefinitionSource)
			{
			case DefinitionSource.File:
				return GetReportNameForFile(ReportPath);
			case DefinitionSource.EmbeddedResource:
				return ReportEmbeddedResource;
			case DefinitionSource.Direct:
				return "";
			default:
				return string.Empty;
			}
		}

		private static string GetReportNameForFile(string path)
		{
			if (Path.IsPathRooted(path))
			{
				return path;
			}
			return Path.Combine(Environment.CurrentDirectory, path);
		}

		private PreviewItemContext CreateItemContext()
		{
			return CreateItemContext(ReportPath, GetFullyQualifiedReportPath(), DefinitionSource, m_embeddedResourceAssembly);
		}

		internal static PreviewItemContext CreateItemContextForFilePath(string filePath)
		{
			return CreateItemContext(filePath, GetReportNameForFile(filePath), DefinitionSource.File, null);
		}

		private static PreviewItemContext CreateItemContext(string pathForFileDefinitionSource, string fullyQualifiedPath, DefinitionSource definitionSource, Assembly embeddedResourceAssembly)
		{
			if (definitionSource == DefinitionSource.Unknown)
			{
				return null;
			}
			PreviewItemContext previewItemContext = InstantiatePreviewItemContext();
			previewItemContext.SetPath(pathForFileDefinitionSource, fullyQualifiedPath, definitionSource, embeddedResourceAssembly);
			return previewItemContext;
		}

		private LocalProcessingException WrapProcessingException(Exception processingException)
		{
			Exception ex = processingException;
			while (ex != null && ex.InnerException != null && (ex is ReportRenderingException || ex is UnhandledReportRenderingException || ex is HandledReportRenderingException))
			{
				ex = ex.InnerException;
			}
			LocalProcessingException ex2 = ex as LocalProcessingException;
			if (ex2 != null)
			{
				return ex2;
			}
			return new LocalProcessingException(ex);
		}

		private static string PageCountModeToProcessingPaginationMode(PageCountMode pageCountMode)
		{
			if (pageCountMode == PageCountMode.Actual)
			{
				return "Actual";
			}
			return "Estimate";
		}

		private static ReportParameterInfoCollection ParameterInfoCollectionToApi(ParameterInfoCollection processingMetadata, bool supportsQueries)
		{
			if (processingMetadata == null)
			{
				return new ReportParameterInfoCollection();
			}
			ReportParameterInfo[] array = new ReportParameterInfo[processingMetadata.Count];
			for (int i = 0; i < processingMetadata.Count; i++)
			{
				array[i] = ParameterInfoToApi(processingMetadata[i], supportsQueries);
			}
			return new ReportParameterInfoCollection(array);
		}

		private static ReportParameterInfo ParameterInfoToApi(Microsoft.ReportingServices.ReportProcessing.ParameterInfo paramInfo, bool supportsQueries)
		{
			string[] array = null;
			if (paramInfo.DependencyList != null)
			{
				array = new string[paramInfo.DependencyList.Count];
				for (int i = 0; i < paramInfo.DependencyList.Count; i++)
				{
					array[i] = paramInfo.DependencyList[i].Name;
				}
			}
			string[] array2 = null;
			if (paramInfo.Values != null)
			{
				array2 = new string[paramInfo.Values.Length];
				for (int j = 0; j < paramInfo.Values.Length; j++)
				{
					array2[j] = paramInfo.CastToString(paramInfo.Values[j], CultureInfo.CurrentCulture);
				}
			}
			List<ValidValue> list = null;
			if (paramInfo.ValidValues != null)
			{
				list = new List<ValidValue>(paramInfo.ValidValues.Count);
				foreach (Microsoft.ReportingServices.ReportProcessing.ValidValue validValue in paramInfo.ValidValues)
				{
					string value = paramInfo.CastToString(validValue.Value, CultureInfo.CurrentCulture);
					list.Add(new ValidValue(validValue.Label, value));
				}
			}
			ParameterState state;
			switch (paramInfo.State)
			{
			case ReportParameterState.HasValidValue:
				state = ParameterState.HasValidValue;
				break;
			case ReportParameterState.InvalidValueProvided:
			case ReportParameterState.DefaultValueInvalid:
			case ReportParameterState.MissingValidValue:
				state = ParameterState.MissingValidValue;
				break;
			case ReportParameterState.HasOutstandingDependencies:
				state = ParameterState.HasOutstandingDependencies;
				break;
			case ReportParameterState.DynamicValuesUnavailable:
				state = ParameterState.DynamicValuesUnavailable;
				break;
			default:
				state = ParameterState.MissingValidValue;
				break;
			}
			return new ReportParameterInfo(paramInfo.Name, (ParameterDataType)Enum.Parse(typeof(ParameterDataType), paramInfo.DataType.ToString()), paramInfo.Nullable, paramInfo.AllowBlank, paramInfo.MultiValue, supportsQueries && paramInfo.UsedInQuery, paramInfo.Prompt, paramInfo.PromptUser, supportsQueries && paramInfo.DynamicDefaultValue, supportsQueries && paramInfo.DynamicValidValues, null, array2, list, array, state);
		}

		private void OnLocalReportChange(object sender, EventArgs e)
		{
			m_processingHost.ResetExecution();
		}

		public LocalReport()
			: this(new ControlService(new StandalonePreviewStore()))
		{
		}

		private LocalReport CreateNewLocalReport()
		{
			return new LocalReport();
		}

		private LocalDataRetrieval CreateDataRetrieval()
		{
			return new LocalDataRetrievalFromDataSet
			{
				SubReportDataSetCallback = ControlSubReportInfoCallback
			};
		}

		private static PreviewItemContext InstantiatePreviewItemContext()
		{
			return new PreviewItemContext();
		}
	}
}
