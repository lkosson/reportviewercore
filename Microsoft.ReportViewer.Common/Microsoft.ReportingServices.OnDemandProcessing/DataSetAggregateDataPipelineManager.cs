using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSetAggregateDataPipelineManager : DataPipelineManager
	{
		private RuntimeAggregationIncrementalDataSource m_runtimeDataSource;

		public override IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				Global.Tracer.Assert(condition: false, "DataSetAggregateDataPipelineManager GroupTreeRoot property must not be accessed");
				throw new NotImplementedException();
			}
		}

		protected override RuntimeDataSource RuntimeDataSource => m_runtimeDataSource;

		public DataSetAggregateDataPipelineManager(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override void InternalStartProcessing()
		{
			Global.Tracer.Assert(m_runtimeDataSource == null, "Cannot StartProcessing twice for the same pipeline manager");
			m_runtimeDataSource = new RuntimeAggregationIncrementalDataSource(m_dataSet, m_odpContext);
			m_runtimeDataSource.Initialize();
			m_runtimeDataSource.CalculateDataSetAggregates();
		}

		protected override void InternalStopProcessing()
		{
			if (m_runtimeDataSource != null)
			{
				m_runtimeDataSource.Teardown();
				m_odpContext.ReportRuntime.CurrentScope = null;
			}
		}

		public override void Advance()
		{
		}

		public override void Abort()
		{
			base.Abort();
			if (RuntimeDataSource != null)
			{
				RuntimeDataSource.Abort();
			}
		}
	}
}
