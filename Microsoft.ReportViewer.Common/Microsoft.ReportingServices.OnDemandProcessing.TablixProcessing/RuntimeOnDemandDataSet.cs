using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeOnDemandDataSet : RuntimePrefetchDataSet
	{
		private bool? m_originalTablixProcessingMode;

		private bool m_processFromLiveDataReader;

		private readonly bool m_generateGroupTree;

		private DataProcessingController m_dataProcessingController;

		internal IOnDemandScopeInstance GroupTreeRoot => m_dataProcessingController.GroupTreeRoot;

		internal override bool ProcessFromLiveDataReader => m_processFromLiveDataReader;

		public RuntimeOnDemandDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext, bool processFromLiveDataReader, bool generateGroupTree, bool canWriteDataChunk)
			: base(dataSource, dataSet, dataSetInstance, odpContext, canWriteDataChunk, processRetrievedData: true)
		{
			m_processFromLiveDataReader = processFromLiveDataReader;
			m_generateGroupTree = generateGroupTree;
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			m_dataProcessingController = new DataProcessingController(m_odpContext, m_dataSet, m_dataSetInstance);
			if (m_processFromLiveDataReader)
			{
				base.InitializeBeforeProcessingRows(aReaderExtensionsSupported);
				m_odpContext.ClrCompareOptions = m_dataSet.GetCLRCompareOptions();
			}
			else
			{
				Global.Tracer.Assert(m_dataReader == null, "(null == m_dataReader)");
				if (!m_dataSetInstance.NoRows)
				{
					m_dataReader = new ProcessingDataReader(m_dataSetInstance, m_dataSet, m_odpContext, overrideWithSharedDataSetChunkSettings: false);
				}
			}
			PopulateFieldsWithReaderFlags();
			m_dataProcessingController.InitializeDataProcessing();
		}

		protected override void InitializeRowSourceAndProcessRows(ExecutedQuery existingQuery)
		{
			if (m_processFromLiveDataReader)
			{
				base.InitializeRowSourceAndProcessRows(existingQuery);
				return;
			}
			InitializeBeforeProcessingRows(aReaderExtensionsSupported: false);
			m_odpContext.CheckAndThrowIfAborted();
			ProcessRows();
		}

		protected override void InitializeDataSet()
		{
			base.InitializeDataSet();
			m_originalTablixProcessingMode = m_odpContext.IsTablixProcessingMode;
			m_odpContext.IsTablixProcessingMode = true;
			m_odpContext.SetComparisonInformation(m_dataSet.DataSetCore);
		}

		protected override void CleanupDataReader()
		{
			if (m_processFromLiveDataReader)
			{
				base.CleanupDataReader();
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (m_generateGroupTree)
			{
				CleanupController();
			}
			if (m_originalTablixProcessingMode.HasValue)
			{
				m_odpContext.IsTablixProcessingMode = m_originalTablixProcessingMode.Value;
			}
			if (m_dataSetInstance != null)
			{
				m_odpContext.SetTablixProcessingComplete(m_dataSet.IndexInCollection);
			}
		}

		protected override void AllRowsRead()
		{
			base.AllRowsRead();
			m_dataProcessingController.AllRowsRead();
			if (m_generateGroupTree)
			{
				m_dataProcessingController.GenerateGroupTree();
			}
		}

		protected override void CleanupForException()
		{
			base.CleanupForException();
			CleanupController();
		}

		private void CleanupController()
		{
			if (m_dataProcessingController != null)
			{
				m_dataProcessingController.TeardownDataProcessing();
			}
		}

		protected override void ProcessRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber)
		{
			if (m_processFromLiveDataReader && !m_dataSet.IsReferenceToSharedDataSet)
			{
				base.ProcessRow(row, rowNumber);
			}
			m_dataProcessingController.NextRow(row, rowNumber, m_processFromLiveDataReader && !m_canWriteDataChunk, base.HasServerAggregateMetadata);
		}

		protected override void ProcessExtendedPropertyMappings()
		{
			if (m_processFromLiveDataReader)
			{
				base.ProcessExtendedPropertyMappings();
			}
		}

		protected override void CleanupProcess()
		{
			if (m_processFromLiveDataReader)
			{
				base.CleanupProcess();
			}
		}

		internal override void EraseDataChunk()
		{
			if (m_processFromLiveDataReader)
			{
				base.EraseDataChunk();
			}
		}
	}
}
