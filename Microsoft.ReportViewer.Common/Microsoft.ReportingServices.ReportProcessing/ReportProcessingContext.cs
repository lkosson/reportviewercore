using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ReportProcessingContext : ProcessingContext
	{
		private readonly RuntimeDataSourceInfoCollection m_dataSources;

		private readonly RuntimeDataSetInfoCollection m_sharedDataSetReferences;

		private readonly IProcessingDataExtensionConnection m_createDataExtensionInstanceFunction;

		private ISharedDataSet m_dataSetExecute;

		internal override bool EnableDataBackedParameters => true;

		internal override RuntimeDataSourceInfoCollection DataSources => m_dataSources;

		internal override RuntimeDataSetInfoCollection SharedDataSetReferences => m_sharedDataSetReferences;

		internal IProcessingDataExtensionConnection CreateDataExtensionInstanceFunction => m_createDataExtensionInstanceFunction;

		internal override bool CanShareDataSets => true;

		internal ISharedDataSet DataSetExecute
		{
			get
			{
				return m_dataSetExecute;
			}
			set
			{
				m_dataSetExecute = value;
			}
		}

		internal override IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => m_createDataExtensionInstanceFunction;

		internal ReportProcessingContext(ICatalogItemContext reportContext, string requestUserName, ParameterInfoCollection parameters, RuntimeDataSourceInfoCollection dataSources, RuntimeDataSetInfoCollection sharedDataSetReferences, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory createChunkFactory, ReportProcessing.ExecutionType interactiveExecution, CultureInfo culture, UserProfileState allowUserProfileState, UserProfileState initialUserProfileState, IProcessingDataExtensionConnection createDataExtensionInstanceFunction, ReportRuntimeSetup reportRuntimeSetup, CreateAndRegisterStream createStreamCallback, bool isHistorySnapshot, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection, ISharedDataSet dataSetExecute)
			: base(reportContext, requestUserName, parameters, subReportCallback, getResourceFunction, createChunkFactory, interactiveExecution, culture, allowUserProfileState, initialUserProfileState, reportRuntimeSetup, createStreamCallback, isHistorySnapshot, jobContext, extFactory, dataProtection)
		{
			m_dataSources = dataSources;
			m_sharedDataSetReferences = sharedDataSetReferences;
			m_createDataExtensionInstanceFunction = createDataExtensionInstanceFunction;
			m_dataSetExecute = dataSetExecute;
		}

		internal override ReportProcessing.ProcessingContext CreateInternalProcessingContext(string chartName, Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, ReportProcessing.GetReportChunk getChunkCallback, ReportProcessing.CreateReportChunk cacheDataCallback)
		{
			SubreportCallbackAdapter @object = new SubreportCallbackAdapter(base.OnDemandSubReportCallback, errorContext);
			return new ReportProcessing.ReportProcessingContext(chartName, DataSources, base.RequestUserName, base.UserLanguage, @object.SubReportCallback, base.ReportContext, report, errorContext, base.CreateReportChunkCallback, base.GetResourceCallback, base.InteractiveExecution, executionTime, allowUserProfileState, isHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback, CreateDataExtensionInstanceFunction, base.ReportRuntimeSetup, base.JobContext, base.ExtFactory, base.DataProtection);
		}

		internal override ReportProcessing.ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot)
		{
			return new ReportProcessing.ReportProcessingContext(null, DataSources, base.RequestUserName, base.UserLanguage, base.ReportContext, errorContext, base.InteractiveExecution, executionTimeStamp, base.AllowUserProfileState, isSnapshot, CreateDataExtensionInstanceFunction, base.ReportRuntimeSetup, base.JobContext, base.ExtFactory, base.DataProtection);
		}
	}
}
