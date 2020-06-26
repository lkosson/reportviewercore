using Microsoft.ReportingServices;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.Reporting.NETCore
{
	internal class ProcessingContextForDataSets : ProcessingContext
	{
		private IEnumerable m_dataSources;

		private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subReportInfoCallback;

		internal override bool EnableDataBackedParameters => false;

		internal override IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => new DataSetExtensionConnection(m_subReportInfoCallback, m_dataSources);

		internal override RuntimeDataSourceInfoCollection DataSources => null;

		internal override RuntimeDataSetInfoCollection SharedDataSetReferences => null;

		internal override bool CanShareDataSets => false;

		public ProcessingContextForDataSets(PreviewItemContext reportContext, ParameterInfoCollection parameters, IEnumerable dataSources, ReportProcessing.OnDemandSubReportCallback subReportCallback, LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subReportInfoCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup reportRuntimeSetup, CreateAndRegisterStream createStreamCallback)
			: base(reportContext, Environment.UserName, parameters, subReportCallback, getResourceFunction, chunkFactory, ReportProcessing.ExecutionType.Live, Thread.CurrentThread.CurrentCulture, UserProfileState.Both, UserProfileState.None, reportRuntimeSetup, createStreamCallback, isHistorySnapshot: false, new ViewerJobContextImpl(), new ViewerExtensionFactory(), DataProtectionLocal.Instance)
		{
			m_dataSources = dataSources;
			m_subReportInfoCallback = subReportInfoCallback;
		}

		internal override ReportProcessing.ProcessingContext CreateInternalProcessingContext(string chartName, Microsoft.ReportingServices.ReportProcessing.Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, ReportProcessing.GetReportChunk getChunkCallback, ReportProcessing.CreateReportChunk cacheDataCallback)
		{
			Microsoft.ReportingServices.ReportProcessing.Global.Tracer.Assert(condition: false, "CreateInternalProcessingContext is not used for ODP Engine Controls");
			return null;
		}

		internal override ReportProcessing.ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot)
		{
			Microsoft.ReportingServices.ReportProcessing.Global.Tracer.Assert(condition: false, "ParametersInternalProcessingContext is not used for ODP Engine Controls");
			return null;
		}
	}
}
