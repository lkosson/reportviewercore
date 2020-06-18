using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class RuntimeParameterDataSet : RuntimeAtomicDataSet, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner
	{
		private ReportParameterDataSetCache m_parameterDataSetObj;

		protected bool m_mustEvaluateThroughReportObjectModel;

		private Filters m_filters;

		public RuntimeParameterDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext processingContext, bool mustEvaluateThroughReportObjectModel, ReportParameterDataSetCache aCache)
			: base(dataSource, dataSet, dataSetInstance, processingContext, processRetrievedData: true)
		{
			m_parameterDataSetObj = aCache;
			m_mustEvaluateThroughReportObjectModel = mustEvaluateThroughReportObjectModel;
		}

		protected override void ProcessRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber)
		{
			m_odpContext.ReportObjectModel.FieldsImpl.NewRow();
			m_odpContext.ReportObjectModel.UpdateFieldValues(reuseFieldObjects: false, row, m_dataSetInstance, base.HasServerAggregateMetadata);
			bool flag = true;
			if (m_filters != null)
			{
				flag = m_filters.PassFilters(new DataFieldRow(m_odpContext.ReportObjectModel.FieldsImpl, getAndSave: false));
			}
			if (flag)
			{
				PostFilterNextRow();
			}
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			Global.Tracer.Assert(m_odpContext.ReportObjectModel != null && m_odpContext.ReportRuntime != null);
			m_odpContext.SetupFieldsForNewDataSet(m_dataSet, m_dataSetInstance, addRowIndex: false, noRows: true);
			m_dataSet.SetFilterExprHost(m_odpContext.ReportObjectModel);
			m_dataSet.SetupRuntimeEnvironment(m_odpContext);
			if (m_dataSet.Filters != null)
			{
				m_filters = new Filters(Filters.FilterTypes.DataSetFilter, this, m_dataSet.Filters, m_dataSet.ObjectType, m_dataSet.Name, m_odpContext, 0);
			}
		}

		protected override void AllRowsRead()
		{
			if (m_filters != null)
			{
				m_filters.FinishReadingRows();
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			m_odpContext.EnsureScalabilityCleanup();
		}

		public virtual void PostFilterNextRow()
		{
			if (m_parameterDataSetObj != null)
			{
				m_parameterDataSetObj.NextRow(m_odpContext.ReportObjectModel.FieldsImpl.GetAndSaveFields());
			}
		}
	}
}
