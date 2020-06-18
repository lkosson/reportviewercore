using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class Merge
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		private OnDemandProcessingContext m_odpContext;

		private RetrievalManager m_retrievalManager;

		private string m_reportLanguage;

		private bool m_initialized;

		private ParameterInfoCollection m_parameters;

		internal Merge(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext odpContext)
		{
			m_report = report;
			m_odpContext = odpContext;
			m_retrievalManager = new RetrievalManager(report, odpContext);
		}

		internal void Init(bool includeParameters, bool parametersOnly)
		{
			if (m_odpContext.ReportRuntime == null)
			{
				m_odpContext.ReportObjectModel.Initialize(m_report, m_odpContext.CurrentReportInstance);
				m_odpContext.ReportRuntime = new Microsoft.ReportingServices.RdlExpressions.ReportRuntime(m_report.ObjectType, m_odpContext.ReportObjectModel, m_odpContext.ErrorContext);
			}
			if (!m_initialized && !m_odpContext.InitializedRuntime)
			{
				m_initialized = true;
				m_odpContext.ReportRuntime.LoadCompiledCode(m_report, includeParameters, parametersOnly, m_odpContext.ReportObjectModel, m_odpContext.ReportRuntimeSetup);
				if (m_odpContext.ReportRuntime.ReportExprHost != null)
				{
					m_report.SetExprHost(m_odpContext.ReportRuntime.ReportExprHost, m_odpContext.ReportObjectModel);
				}
			}
		}

		internal void Init(ParameterInfoCollection parameters)
		{
			if (m_odpContext.ReportRuntime == null)
			{
				Init(includeParameters: false, parametersOnly: false);
			}
			m_odpContext.ReportObjectModel.Initialize(parameters);
			m_odpContext.ReportRuntime.CustomCodeOnInit(m_odpContext.ReportDefinition);
		}

		internal void EvaluateReportLanguage(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, string snapshotLanguage)
		{
			CultureInfo language = null;
			if (snapshotLanguage != null)
			{
				m_reportLanguage = snapshotLanguage;
				language = GetSpecificCultureInfoFromLanguage(snapshotLanguage, m_odpContext.ErrorContext);
			}
			else if (m_report.Language != null)
			{
				if (m_report.Language.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
				{
					m_odpContext.LanguageInstanceId++;
					m_reportLanguage = m_odpContext.ReportRuntime.EvaluateReportLanguageExpression(m_report, out language);
				}
				else
				{
					language = GetSpecificCultureInfoFromLanguage(m_report.Language.StringValue, m_odpContext.ErrorContext);
				}
			}
			if (language == null && !m_odpContext.InSubreport)
			{
				language = Localization.DefaultReportServerSpecificCulture;
			}
			if (language != null)
			{
				Thread.CurrentThread.CurrentCulture = language;
				reportInstance.Language = language.ToString();
				m_odpContext.ThreadCulture = language;
			}
		}

		private static CultureInfo GetSpecificCultureInfoFromLanguage(string language, ErrorContext errorContext)
		{
			CultureInfo cultureInfo = null;
			try
			{
				cultureInfo = new CultureInfo(language, useUserOverride: false);
				if (cultureInfo.IsNeutralCulture)
				{
					cultureInfo = CultureInfo.CreateSpecificCulture(language);
					cultureInfo = new CultureInfo(cultureInfo.Name, useUserOverride: false);
					return cultureInfo;
				}
				return cultureInfo;
			}
			catch (Exception ex)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, ObjectType.Report, null, "Language", ex.Message);
				return cultureInfo;
			}
		}

		internal void FetchData(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, bool mergeTransaction)
		{
			if (m_odpContext.ProcessWithCachedData)
			{
				if (reportInstance.IsMissingExpectedDataChunk(m_odpContext))
				{
					throw new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataCacheUnavailableException();
				}
			}
			else if (!m_odpContext.SnapshotProcessing)
			{
				if (m_odpContext.InSubreport)
				{
					m_odpContext.CreateAndSetupDataExtensionFunction.DataSetRetrieveForReportInstance(m_odpContext.ReportContext, m_parameters);
				}
				if (!m_retrievalManager.PrefetchData(reportInstance, m_parameters, mergeTransaction))
				{
					throw new ProcessingAbortedException();
				}
				reportInstance.NoRows = m_retrievalManager.NoRows;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance PrepareReportInstance(IReportInstanceContainer reportInstanceContainer)
		{
			IReference<Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance> reference;
			if (reportInstanceContainer.ReportInstance == null || reportInstanceContainer.ReportInstance.Value() == null)
			{
				reference = Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance.CreateInstance(reportInstanceContainer, m_odpContext, m_report, m_parameters);
			}
			else
			{
				reference = reportInstanceContainer.ReportInstance;
				reference.Value().InitializeFromSnapshot(m_odpContext);
			}
			return reference.Value();
		}

		internal void SetupReport(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			m_odpContext.CurrentReportInstance = reportInstance;
			if (m_odpContext.InitializedRuntime)
			{
				return;
			}
			m_odpContext.InitializedRuntime = true;
			List<ReportSection> reportSections = m_report.ReportSections;
			if (reportSections != null)
			{
				foreach (ReportSection item in reportSections)
				{
					m_odpContext.RuntimeInitializeReportItemObjs(item.ReportItems, traverseDataRegions: true);
					m_odpContext.RuntimeInitializeTextboxObjs(item.ReportItems, setExprHost: true);
				}
			}
			if (m_report.HasVariables)
			{
				m_odpContext.RuntimeInitializeVariables(m_report);
			}
			if (m_report.HasLookups)
			{
				m_odpContext.RuntimeInitializeLookups(m_report);
			}
			m_report.RegisterDataSetScopedAggregates(m_odpContext);
		}

		internal bool InitAndSetupSubReport(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			IReference<Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance> currentSubReportInstance = subReport.CurrentSubReportInstance;
			bool num = InitSubReport(subReport, currentSubReportInstance);
			if (num)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = currentSubReportInstance.Value().ReportInstance.Value();
				m_odpContext.SetupEnvironment(reportInstance);
				m_odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.OnDemandExpressions);
				m_odpContext.IsUnrestrictedRenderFormatReferenceMode = true;
			}
			currentSubReportInstance.Value().Initialized = true;
			return num;
		}

		private bool InitSubReport(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, IReference<Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance> subReportInstanceRef)
		{
			bool flag = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance = subReportInstanceRef.Value();
			if (m_odpContext.SnapshotProcessing && subReportInstance.ProcessedWithError)
			{
				return false;
			}
			try
			{
				if (!m_odpContext.SnapshotProcessing)
				{
					subReportInstance.RetrievalStatus = subReport.RetrievalStatus;
				}
				if (subReportInstance.RetrievalStatus == Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed)
				{
					subReportInstance.ProcessedWithError = true;
					return false;
				}
				if (m_odpContext.CurrentReportInstance == null && subReport.Report != null)
				{
					if (!m_odpContext.SnapshotProcessing)
					{
						subReport.OdpSubReportInfo.UserSortDataSetGlobalId = m_odpContext.ParentContext.UserSortFilterContext.DataSetGlobalId;
					}
					m_odpContext.UserSortFilterContext.UpdateContextForFirstSubreportInstance(m_odpContext.ParentContext.UserSortFilterContext);
				}
				if (!m_odpContext.SnapshotProcessing || m_odpContext.ReprocessSnapshot)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = PrepareReportInstance(subReportInstance);
					m_odpContext.CurrentReportInstance = reportInstance;
					Init(includeParameters: true, parametersOnly: false);
					subReportInstance.InstanceUniqueName = m_odpContext.CreateUniqueID().ToString(CultureInfo.InvariantCulture);
					m_odpContext.SetSubReportContext(subReportInstance, setupReportOM: false);
					SetupReport(reportInstance);
					m_parameters = OnDemandProcessingContext.EvaluateSubReportParameters(m_odpContext.ParentContext, subReport);
					bool hasMissingValidValue;
					if (!m_odpContext.SnapshotProcessing && !m_odpContext.ProcessWithCachedData)
					{
						try
						{
							m_odpContext.ProcessReportParameters = true;
							m_odpContext.ReportObjectModel.ParametersImpl.Clear();
							Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessReportParameters(subReport.Report, m_odpContext, m_parameters);
						}
						finally
						{
							m_odpContext.ProcessReportParameters = false;
						}
						if (!m_parameters.ValuesAreValid())
						{
							subReportInstance.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.ParametersNotSpecified;
							throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
						}
						m_odpContext.ReportParameters = m_parameters;
					}
					else if (!m_parameters.ValuesAreValid(out hasMissingValidValue) && hasMissingValidValue)
					{
						subReportInstance.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.ParametersNotSpecified;
						throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
					}
					Init(m_parameters);
					subReportInstance.Parameters = new ParametersImpl(m_odpContext.ReportObjectModel.ParametersImpl);
					m_odpContext.SetSubReportNameModifierAndParameters(subReportInstance, !m_odpContext.SnapshotProcessing);
					if (m_odpContext.ReprocessSnapshot)
					{
						reportInstance.InitializeFromSnapshot(m_odpContext);
					}
					EvaluateReportLanguage(reportInstance, null);
					subReportInstance.ThreadCulture = m_odpContext.ThreadCulture;
					if (!m_odpContext.SnapshotProcessing || m_odpContext.FoundExistingSubReportInstance)
					{
						flag = FetchSubReportData(subReport, subReportInstance);
						if (flag)
						{
							subReportInstance.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataRetrieved;
						}
						else
						{
							subReportInstance.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataRetrieveFailed;
							subReportInstance.ProcessedWithError = true;
						}
					}
					else
					{
						subReportInstance.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataNotRetrieved;
						subReportInstance.ProcessedWithError = true;
					}
					m_odpContext.ReportParameters = null;
					return flag;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance2 = subReportInstance.ReportInstance.Value();
				m_odpContext.CurrentReportInstance = reportInstance2;
				m_odpContext.LoadExistingSubReportDataChunkNameModifier(subReportInstance);
				reportInstance2.InitializeFromSnapshot(m_odpContext);
				Init(includeParameters: true, parametersOnly: false);
				m_odpContext.ThreadCulture = subReportInstance.ThreadCulture;
				SetupReport(reportInstance2);
				m_odpContext.SetSubReportContext(subReportInstance, setupReportOM: true);
				m_odpContext.ReportRuntime.CustomCodeOnInit(m_odpContext.ReportDefinition);
				m_odpContext.OdpMetadata.SetUpdatedVariableValues(m_odpContext, reportInstance2);
				return flag;
			}
			catch (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataCacheUnavailableException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				flag = false;
				subReportInstance.ProcessedWithError = true;
				if (subReportInstance.ReportInstance != null)
				{
					subReportInstance.ReportInstance.Value().NoRows = false;
				}
				if (ex2 is RSException)
				{
					m_odpContext.ErrorContext.Register((RSException)ex2, subReport.ObjectType);
					return flag;
				}
				return flag;
			}
		}

		internal bool FetchSubReportData(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = subReportInstance.ReportInstance.Value();
			reportInstance.ResetReportVariables(subReport.OdpContext);
			bool flag;
			try
			{
				FetchData(reportInstance, subReport.MergeTransactions);
				if (subReport.OdpContext.ReprocessSnapshot && reportInstance.IsMissingExpectedDataChunk(subReport.OdpContext))
				{
					flag = false;
				}
				else
				{
					if (subReport.OdpContext.ReprocessSnapshot && !subReport.InDataRegion)
					{
						PreProcessTablixes(subReport.Report, subReport.OdpContext, onlyWithSubReports: false);
					}
					flag = true;
				}
			}
			catch (ProcessingAbortedException)
			{
				flag = false;
			}
			if (flag)
			{
				reportInstance.CalculateAndStoreReportVariables(subReport.OdpContext);
			}
			return flag;
		}

		internal static void TablixDataProcessing(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet specificDataSetOnly)
		{
			bool flag = false;
			while (!flag)
			{
				int indexInCollection = specificDataSetOnly.IndexInCollection;
				int unprocessedDataSetCount;
				bool[] exclusionList = odpContext.GenerateDataSetExclusionList(out unprocessedDataSetCount);
				indexInCollection = odpContext.ReportDefinition.CalculateDatasetRootIndex(indexInCollection, exclusionList, unprocessedDataSetCount);
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[indexInCollection];
				FullAtomicDataPipelineManager fullAtomicDataPipelineManager = new FullAtomicDataPipelineManager(odpContext, dataSet);
				fullAtomicDataPipelineManager.StartProcessing();
				fullAtomicDataPipelineManager.StopProcessing();
				flag = (indexInCollection == specificDataSetOnly.IndexInCollection);
			}
		}

		internal static bool PreProcessTablixes(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext odpContext, bool onlyWithSubReports)
		{
			bool result = false;
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource in report.DataSources)
			{
				if (dataSource.DataSets == null)
				{
					continue;
				}
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet in dataSource.DataSets)
				{
					if (odpContext.CurrentReportInstance.GetDataSetInstance(dataSet, odpContext) != null && dataSet.DataRegions.Count != 0 && !odpContext.IsTablixProcessingComplete(dataSet.IndexInCollection) && (!onlyWithSubReports || dataSet.HasSubReports || (odpContext.InSubreport && odpContext.HasUserSortFilter)))
					{
						result = true;
						TablixDataProcessing(odpContext, dataSet);
					}
				}
			}
			return result;
		}
	}
}
