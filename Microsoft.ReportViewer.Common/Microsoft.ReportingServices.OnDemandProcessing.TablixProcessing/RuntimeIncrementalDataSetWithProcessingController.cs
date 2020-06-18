using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal abstract class RuntimeIncrementalDataSetWithProcessingController : RuntimeIncrementalDataSet
	{
		protected DataProcessingController m_dataProcessingController;

		internal IOnDemandScopeInstance GroupTreeRoot => m_dataProcessingController.GroupTreeRoot;

		public RuntimeIncrementalDataSetWithProcessingController(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		protected override void InitializeDataSet()
		{
			base.InitializeDataSet();
			m_odpContext.SetComparisonInformation(m_dataSet.DataSetCore);
		}

		protected override void TeardownDataSet()
		{
			base.TeardownDataSet();
			CleanupController();
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (m_dataSetInstance != null)
			{
				m_odpContext.SetTablixProcessingComplete(m_dataSet.IndexInCollection);
			}
		}

		private void CleanupController()
		{
			if (m_dataProcessingController != null)
			{
				m_dataProcessingController.TeardownDataProcessing();
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			m_dataProcessingController = new DataProcessingController(m_odpContext, m_dataSet, m_dataSetInstance);
			PopulateFieldsWithReaderFlags();
			m_odpContext.ClrCompareOptions = m_dataSet.GetCLRCompareOptions();
			m_dataProcessingController.InitializeDataProcessing();
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}
	}
}
