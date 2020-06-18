using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeAtomicDataSet : RuntimeDataSet, IRowConsumer
	{
		private int[] m_iRowConsumerMappingDataSetFieldIndexesToDataChunk;

		private bool m_iRowConsumerMappingIdentical;

		public string ReportDataSetName => m_dataSet.Name;

		protected RuntimeAtomicDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext, bool processRetrievedData)
			: base(dataSource, dataSet, dataSetInstance, odpContext, processRetrievedData)
		{
		}

		internal void ProcessConcurrent(object threadSet)
		{
			Global.Tracer.Assert(m_dataSet.Name != null, "The name of a data set cannot be null.");
			try
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data set '{0}'", m_dataSet.Name.MarkAsPrivate());
				}
				Process(null);
			}
			catch (ProcessingAbortedException)
			{
				if (Global.Tracer.TraceWarning)
				{
					Global.Tracer.Trace(TraceLevel.Warning, "Data set '{0}': Report processing has been aborted.", m_dataSet.Name.MarkAsPrivate());
				}
				if (m_odpContext.StreamingMode)
				{
					throw;
				}
			}
			catch (Exception ex2)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data set '{0}'. Details: {1}", m_dataSet.Name.MarkAsPrivate(), ex2.ToString());
				}
				if (m_odpContext.AbortInfo != null)
				{
					m_odpContext.AbortInfo.SetError(ex2, m_odpContext.ProcessingAbortItemUniqueIdentifier);
					return;
				}
				throw;
			}
			finally
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data set '{0}' completed.", m_dataSet.Name.MarkAsPrivate());
				}
				(threadSet as ThreadSet)?.ThreadCompleted();
			}
		}

		public void ProcessInline(ExecutedQuery existingQuery)
		{
			Process(existingQuery);
		}

		private void Process(ExecutedQuery existingQuery)
		{
			InitializeDataSet();
			try
			{
				try
				{
					InitializeRowSourceAndProcessRows(existingQuery);
				}
				finally
				{
					CleanupProcess();
				}
				AllRowsRead();
				TeardownDataSet();
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				CleanupForException();
				throw;
			}
			finally
			{
				FinalCleanup();
			}
		}

		protected virtual void InitializeRowSourceAndProcessRows(ExecutedQuery existingQuery)
		{
			if (m_dataSet.IsReferenceToSharedDataSet)
			{
				ProcessSharedDataSetReference();
				return;
			}
			if (existingQuery != null)
			{
				InitializeAndRunFromExistingQuery(existingQuery);
			}
			else
			{
				InitializeAndRunLiveQuery();
			}
			if (base.ProcessRetrievedData)
			{
				ProcessRows();
			}
		}

		protected virtual void AllRowsRead()
		{
		}

		protected void ProcessRows()
		{
			int rowIndex;
			for (Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow recordRow = ReadOneRow(out rowIndex); recordRow != null; recordRow = ReadOneRow(out rowIndex))
			{
				ProcessRow(recordRow, rowIndex);
			}
		}

		protected abstract void ProcessRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow aRow, int rowNumber);

		public virtual void SetProcessingDataReader(IProcessingDataReader dataReader)
		{
			m_dataReader = dataReader;
			m_dataReader.OverrideWithDataReaderSettings(m_odpContext, m_dataSetInstance, m_dataSet.DataSetCore);
			if (base.ProcessRetrievedData)
			{
				m_dataReader.GetDataReaderMappingForRowConsumer(m_dataSetInstance, out m_iRowConsumerMappingIdentical, out m_iRowConsumerMappingDataSetFieldIndexesToDataChunk);
			}
			InitializeBeforeProcessingRows(base.HasServerAggregateMetadata);
		}

		public void NextRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow originalRow)
		{
			if (base.ProcessRetrievedData)
			{
				m_odpContext.CheckAndThrowIfAborted();
				if (m_dataRowsRead == 0)
				{
					InitializeBeforeFirstRow(hasRows: true);
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow recordRow = null;
				recordRow = ((!m_iRowConsumerMappingIdentical) ? new Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow(originalRow, m_iRowConsumerMappingDataSetFieldIndexesToDataChunk) : originalRow);
				if (m_dataSet.IsReferenceToSharedDataSet && recordRow.IsAggregateRow && m_dataSet.InterpretSubtotalsAsDetails != Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState.False)
				{
					recordRow.IsAggregateRow = false;
				}
				ProcessRow(recordRow, m_dataRowsRead);
				IncrementRowCounterAndTrace();
			}
		}
	}
}
