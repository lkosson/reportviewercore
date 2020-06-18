using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeDataSet : RuntimeLiveQueryExecutor
	{
		protected DataSetInstance m_dataSetInstance;

		protected IProcessingDataReader m_dataReader;

		protected int m_dataRowsRead;

		private bool m_allDataRowsRead;

		private readonly bool m_processRetrievedData = true;

		private readonly DataSetQueryRestartPosition m_restartPosition;

		internal virtual bool ProcessFromLiveDataReader => false;

		internal bool NoRows => m_dataRowsRead <= 0;

		internal int NumRowsRead => m_dataRowsRead;

		internal bool UsedOnlyInParameters => m_dataSet.UsedOnlyInParameters;

		protected virtual bool WritesDataChunk => false;

		protected bool ProcessRetrievedData => m_processRetrievedData;

		protected bool HasServerAggregateMetadata
		{
			get
			{
				if (!m_dataSet.HasAggregateIndicatorFields)
				{
					if (m_dataReader != null)
					{
						return m_dataReader.ReaderExtensionsSupported;
					}
					return false;
				}
				return true;
			}
		}

		protected virtual bool ShouldCancelCommandDuringCleanup => false;

		internal RuntimeDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext, bool processRetrievedData)
			: base(dataSource, dataSet, odpContext)
		{
			m_dataSetInstance = dataSetInstance;
			m_processRetrievedData = processRetrievedData;
			if (m_odpContext.QueryRestartInfo == null)
			{
				m_restartPosition = null;
			}
			else
			{
				m_restartPosition = m_odpContext.QueryRestartInfo.GetRestartPositionForDataSet(m_dataSet);
			}
		}

		internal void InitProcessingParams(IDbConnection conn, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo transInfo)
		{
			m_dataSourceConnection = conn;
			m_transInfo = transInfo;
		}

		protected virtual void InitializeDataSet()
		{
			m_odpContext.EnsureCultureIsSetOnCurrentThread();
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == m_dataSet.LCID)
			{
				if (m_odpContext.ShouldExecuteLiveQueries)
				{
					m_dataSet.LCID = DataSetValidator.LCIDfromRDLCollation(m_dataSet.Collation);
				}
				else
				{
					m_dataSet.LCID = m_dataSetInstance.LCID;
				}
			}
			m_isConnectionOwner = false;
			InitRuntime();
		}

		private void InitRuntime()
		{
			Global.Tracer.Assert(m_odpContext.ReportObjectModel != null && m_odpContext.ReportRuntime != null);
			if (m_odpContext.ReportRuntime.ReportExprHost != null)
			{
				m_dataSet.SetExprHost(m_odpContext.ReportRuntime.ReportExprHost, m_odpContext.ReportObjectModel);
			}
		}

		protected virtual void TeardownDataSet()
		{
			m_odpContext.CheckAndThrowIfAborted();
			if (NoRows)
			{
				m_dataSet.MarkDataRegionsAsNoRows();
			}
		}

		protected virtual void FinalCleanup()
		{
			CleanupDataReader();
			CloseConnection();
			if (m_odpContext.ExecutionLogContext != null)
			{
				m_odpContext.ExecutionLogContext.AddDataSetMetrics(m_dataSet.Name, m_executionMetrics);
			}
		}

		protected virtual void CleanupForException()
		{
			if (m_transInfo != null)
			{
				m_transInfo.RollbackRequired = true;
			}
		}

		protected virtual void CleanupDataReader()
		{
			m_executionMetrics.AddRowCount(m_dataRowsRead);
			if (m_odpContext.DataSetRetrievalComplete != null)
			{
				m_odpContext.DataSetRetrievalComplete[m_dataSet.IndexInCollection] = true;
			}
			if (m_dataSet.IsReferenceToSharedDataSet)
			{
				m_dataSetInstance.RecordSetSize = NumRowsRead;
			}
			CleanupCommandAndDataReader();
		}

		protected abstract void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported);

		protected void PopulateFieldsWithReaderFlags()
		{
			if (m_dataReader != null)
			{
				m_odpContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = HasServerAggregateMetadata;
				m_odpContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = m_dataReader.ReaderFieldProperties;
			}
		}

		protected virtual void CleanupProcess()
		{
			if (!m_dataSet.IsReferenceToSharedDataSet)
			{
				CleanupCommandAndDataReader();
			}
		}

		private void CleanupCommandAndDataReader()
		{
			if (m_dataSet.IsReferenceToSharedDataSet && ProcessFromLiveDataReader)
			{
				return;
			}
			try
			{
				if (ShouldCancelCommandDuringCleanup && !m_allDataRowsRead)
				{
					CancelCommand();
				}
				DisposeDataReader();
			}
			finally
			{
				DisposeCommand();
			}
		}

		private void DisposeDataReader()
		{
			DisposeDataExtensionObject(ref m_dataReader, "data reader", DataProcessingMetrics.MetricType.DisposeDataReader);
		}

		protected abstract void ProcessExtendedPropertyMappings();

		protected virtual void InitializeBeforeFirstRow(bool hasRows)
		{
			if (hasRows && !m_dataSet.IsReferenceToSharedDataSet)
			{
				MapExtendedProperties();
				ProcessExtendedPropertyMappings();
			}
		}

		protected Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow ReadOneRow(out int rowIndex)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow result = null;
			rowIndex = -1;
			if (m_allDataRowsRead)
			{
				return result;
			}
			do
			{
				bool flag = m_dataReader != null && m_dataReader.GetNextRow();
				if (m_dataRowsRead == 0)
				{
					InitializeBeforeFirstRow(flag);
				}
				if (flag)
				{
					m_odpContext.CheckAndThrowIfAborted();
					result = ReadRow();
					rowIndex = m_dataRowsRead;
					IncrementRowCounterAndTrace();
				}
				else
				{
					result = null;
					m_allDataRowsRead = true;
				}
			}
			while (!m_allDataRowsRead && m_restartPosition != null && m_restartPosition.ShouldSkip(m_odpContext, result));
			if (m_restartPosition != null)
			{
				m_restartPosition.DisableRowSkipping(result);
			}
			return result;
		}

		protected void IncrementRowCounterAndTrace()
		{
			m_dataRowsRead++;
			if (Global.Tracer.TraceVerbose && m_dataRowsRead % 100000 == 0)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Read data row: {0}", m_dataRowsRead);
			}
		}

		private void MapExtendedProperties()
		{
			if (!m_dataReader.ReaderFieldProperties)
			{
				return;
			}
			int count = m_dataSet.Fields.Count;
			for (int i = 0; i < count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field = m_dataSet.Fields[i];
				if (field.IsCalculatedField)
				{
					continue;
				}
				try
				{
					int propertyCount = m_dataReader.GetPropertyCount(i);
					List<int> list = new List<int>();
					List<string> list2 = new List<string>();
					for (int j = 0; j < propertyCount; j++)
					{
						string text = null;
						try
						{
							text = m_dataReader.GetPropertyName(i, j);
							list.Add(j);
							list2.Add(text);
						}
						catch (ReportProcessingException_FieldError reportProcessingException_FieldError)
						{
							m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingFieldProperty, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_dataSet.Name, "FieldExtendedProperty", field.Name.MarkAsModelInfo(), text.MarkAsModelInfo(), reportProcessingException_FieldError.Message);
						}
					}
					if (list.Count > 0)
					{
						if (m_dataSetInstance.FieldInfos == null)
						{
							m_dataSetInstance.FieldInfos = new FieldInfo[count];
						}
						m_dataSetInstance.FieldInfos[i] = new FieldInfo(list, list2);
					}
				}
				catch (ReportProcessingException_FieldError aException)
				{
					HandleFieldError(aException, i, field.Name);
				}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow ReadRow()
		{
			_ = m_dataSet.Fields.Count;
			Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow underlyingRecordRowObject = m_dataReader.GetUnderlyingRecordRowObject();
			if (underlyingRecordRowObject != null)
			{
				return underlyingRecordRowObject;
			}
			m_executionMetrics.StartTotalTimer();
			underlyingRecordRowObject = ConstructRecordRow();
			m_executionMetrics.RecordTotalTimerMeasurement();
			return underlyingRecordRowObject;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow ConstructRecordRow()
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow recordRow = new Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow();
			bool flag = m_dataReader.ReaderExtensionsSupported && !m_dataSet.HasAggregateIndicatorFields;
			bool flag2 = HasServerAggregateMetadata && (m_dataSet.InterpretSubtotalsAsDetails == Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState.False || (m_odpContext.IsSharedDataSetExecutionOnly && m_dataSet.InterpretSubtotalsAsDetails == Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState.Auto));
			Microsoft.ReportingServices.ReportIntermediateFormat.RecordField[] array2 = recordRow.RecordFields = new Microsoft.ReportingServices.ReportIntermediateFormat.RecordField[m_dataSet.NonCalculatedFieldCount];
			for (int i = 0; i < array2.Length; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field = m_dataSet.Fields[i];
				if (!m_dataSetInstance.IsFieldMissing(i))
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RecordField recordField = new Microsoft.ReportingServices.ReportIntermediateFormat.RecordField();
					try
					{
						array2[i] = recordField;
						recordField.FieldValue = m_dataReader.GetColumn(i);
						if (flag2)
						{
							if (flag)
							{
								recordField.IsAggregationField = m_dataReader.IsAggregationField(i);
							}
						}
						else
						{
							recordField.IsAggregationField = true;
						}
						recordField.FieldStatus = DataFieldStatus.None;
					}
					catch (ReportProcessingException_FieldError aException)
					{
						recordField = (array2[i] = HandleFieldError(aException, i, field.Name));
						if (recordField != null && !flag2)
						{
							recordField.IsAggregationField = true;
						}
					}
					ReadExtendedPropertiesForRecordField(i, field, recordField);
				}
				else
				{
					array2[i] = null;
				}
			}
			if (flag2)
			{
				if (flag)
				{
					recordRow.IsAggregateRow = m_dataReader.IsAggregateRow;
					recordRow.AggregationFieldCount = m_dataReader.AggregationFieldCount;
				}
				else
				{
					PopulateServerAggregateInformationFromIndicatorFields(recordRow);
				}
			}
			else
			{
				recordRow.AggregationFieldCount = m_dataSet.Fields.Count;
			}
			return recordRow;
		}

		private void PopulateServerAggregateInformationFromIndicatorFields(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow recordRow)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < recordRow.RecordFields.Length; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.RecordField recordField = recordRow.RecordFields[i];
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field = m_dataSet.Fields[i];
				if (recordField == null || !field.HasAggregateIndicatorField)
				{
					continue;
				}
				num++;
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field2 = m_dataSet.Fields[field.AggregateIndicatorFieldIndex];
				bool processedValue = false;
				bool flag;
				if (field2.IsCalculatedField)
				{
					if (field2.Value.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
					{
						processedValue = field2.Value.BoolValue;
						flag = false;
					}
					else
					{
						flag = !Microsoft.ReportingServices.RdlExpressions.ReportRuntime.TryProcessObjectToBoolean(field2.Value.LiteralInfo.Value, out processedValue);
					}
				}
				else
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RecordField recordField2 = recordRow.RecordFields[field.AggregateIndicatorFieldIndex];
					flag = (recordField2 == null || recordField2.FieldStatus != 0 || !Microsoft.ReportingServices.RdlExpressions.ReportRuntime.TryProcessObjectToBoolean(recordField2.FieldValue, out processedValue));
				}
				if (flag)
				{
					m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsMissingOrInvalidAggregateIndicatorFieldValue, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", m_dataSet.Name.MarkAsPrivate(), field.Name.MarkAsModelInfo());
				}
				else if (processedValue)
				{
					num2++;
					recordRow.IsAggregateRow = true;
				}
				recordField.IsAggregationField = !processedValue;
			}
			recordRow.AggregationFieldCount = num - num2;
		}

		private void ReadExtendedPropertiesForRecordField(int fieldIndex, Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef, Microsoft.ReportingServices.ReportIntermediateFormat.RecordField field)
		{
			if (!m_dataReader.ReaderFieldProperties || m_dataSetInstance.GetFieldPropertyCount(fieldIndex) <= 0)
			{
				return;
			}
			FieldInfo orCreateFieldInfo = m_dataSetInstance.GetOrCreateFieldInfo(fieldIndex);
			field.FieldPropertyValues = new List<object>(orCreateFieldInfo.PropertyCount);
			for (int i = 0; i < orCreateFieldInfo.PropertyCount; i++)
			{
				int propertyIndex = orCreateFieldInfo.PropertyReaderIndices[i];
				string modelInfo = orCreateFieldInfo.PropertyNames[i];
				try
				{
					object propertyValue = m_dataReader.GetPropertyValue(fieldIndex, propertyIndex);
					field.FieldPropertyValues.Add(propertyValue);
				}
				catch (ReportProcessingException_FieldError reportProcessingException_FieldError)
				{
					if (!orCreateFieldInfo.IsPropertyErrorRegistered(i))
					{
						m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingFieldProperty, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_dataSet.Name, "FieldExtendedProperty", fieldDef.Name.MarkAsModelInfo(), modelInfo.MarkAsModelInfo(), reportProcessingException_FieldError.Message);
						orCreateFieldInfo.SetPropertyErrorRegistered(i);
					}
					field.FieldPropertyValues.Add(null);
				}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordField HandleFieldError(ReportProcessingException_FieldError aException, int aFieldIndex, string aFieldName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.RecordField recordField = null;
			bool flag = false;
			FieldInfo orCreateFieldInfo = m_dataSetInstance.GetOrCreateFieldInfo(aFieldIndex);
			if (m_dataRowsRead == 0 && DataFieldStatus.UnSupportedDataType != aException.Status && DataFieldStatus.Overflow != aException.Status)
			{
				orCreateFieldInfo.Missing = true;
				recordField = null;
				flag = true;
				m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsMissingFieldInDataSet, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_dataSet.Name, "Field", aFieldName.MarkAsModelInfo());
			}
			if (!flag)
			{
				recordField = new Microsoft.ReportingServices.ReportIntermediateFormat.RecordField();
				recordField.FieldStatus = aException.Status;
				recordField.IsAggregationField = false;
				recordField.FieldValue = null;
			}
			if (!orCreateFieldInfo.ErrorRegistered)
			{
				orCreateFieldInfo.ErrorRegistered = true;
				if (DataFieldStatus.UnSupportedDataType == aException.Status)
				{
					if (!m_odpContext.ProcessReportParameters)
					{
						m_odpContext.ErrorSavingSnapshotData = true;
					}
					m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsDataSetFieldTypeNotSupported, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_dataSet.Name, "Field", aFieldName.MarkAsModelInfo());
				}
				else
				{
					m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingDataSetField, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_dataSet.Name, "Field", aFieldName.MarkAsModelInfo(), aException.Message);
				}
			}
			return recordField;
		}

		protected void InitializeAndRunLiveQuery()
		{
			if (m_dataSourceConnection == null)
			{
				m_isConnectionOwner = true;
			}
			bool readerExtensionsSupported = RunDataSetQuery();
			InitializeToProcessData(readerExtensionsSupported);
		}

		private void InitializeToProcessData(bool readerExtensionsSupported)
		{
			if (m_processRetrievedData)
			{
				InitializeBeforeProcessingRows(readerExtensionsSupported);
				m_odpContext.CheckAndThrowIfAborted();
			}
		}

		protected void InitializeAndRunFromExistingQuery(ExecutedQuery query)
		{
			bool readerExtensionsSupported = RunFromExistingQuery(query);
			InitializeToProcessData(readerExtensionsSupported);
		}

		private bool RunFromExistingQuery(ExecutedQuery query)
		{
			if (m_dataSetInstance != null)
			{
				m_dataSetInstance.SetQueryExecutionTime(query.QueryExecutionTimestamp);
				m_dataSetInstance.CommandText = query.CommandText;
			}
			bool result = TakeOwnershipFromExistingQuery(query);
			if (!m_odpContext.IsSharedDataSetExecutionOnly && m_dataSetInstance != null)
			{
				m_dataSetInstance.SaveCollationSettings(m_dataSet);
				UpdateReportOMDataSet();
			}
			return result;
		}

		private bool TakeOwnershipFromExistingQuery(ExecutedQuery query)
		{
			IDataReader dataReader = null;
			try
			{
				m_executionMetrics.Add(query.ExecutionMetrics);
				m_executionMetrics.CommandText = query.ExecutionMetrics.CommandText;
				query.ReleaseOwnership(ref m_command, ref m_commandWrappedForCancel, ref dataReader);
				ExtractRewrittenCommandText(m_command);
				StoreDataReader(dataReader, query.ErrorInspector);
				return ReaderExtensionsSupported(dataReader);
			}
			catch (RSException)
			{
				EagerInlineReaderCleanup(ref dataReader);
				throw;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				EagerInlineReaderCleanup(ref dataReader);
				throw;
			}
		}

		private bool RunDataSetQuery()
		{
			bool result = false;
			if (m_dataSetInstance != null)
			{
				m_dataSetInstance.SetQueryExecutionTime(DateTime.Now);
			}
			if (m_dataSet.Query == null)
			{
				return result;
			}
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters = m_dataSet.Query.Parameters;
			object[] array = new object[parameters?.Count ?? 0];
			for (int i = 0; i < array.Length; i++)
			{
				if (m_odpContext.IsSharedDataSetExecutionOnly)
				{
					DataSetParameterValue dataSetParameterValue = parameters[i] as DataSetParameterValue;
					if (!dataSetParameterValue.OmitFromQuery)
					{
						array[i] = m_odpContext.ReportObjectModel.ParametersImpl[dataSetParameterValue.UniqueName].Value;
					}
				}
				else
				{
					array[i] = parameters[i].EvaluateQueryParameterValue(m_odpContext, m_dataSet.ExprHost);
				}
			}
			m_odpContext.CheckAndThrowIfAborted();
			m_executionMetrics.StartTotalTimer();
			try
			{
				result = RunEmbeddedQuery(parameters, array);
			}
			finally
			{
				m_executionMetrics.RecordTotalTimerMeasurement();
			}
			if (!m_odpContext.IsSharedDataSetExecutionOnly && m_dataSetInstance != null)
			{
				m_dataSetInstance.SaveCollationSettings(m_dataSet);
				UpdateReportOMDataSet();
			}
			return result;
		}

		private void UpdateReportOMDataSet()
		{
			((DataSetImpl)m_odpContext.ReportObjectModel.DataSetsImpl[m_dataSet.Name]).Update(m_dataSetInstance, m_odpContext.ExecutionTime);
		}

		protected bool ProcessSharedDataSetReference()
		{
			DataSetInfo dataSetInfo = null;
			if (m_odpContext.SharedDataSetReferences != null)
			{
				if (Guid.Empty != m_dataSet.DataSetCore.CatalogID)
				{
					dataSetInfo = m_odpContext.SharedDataSetReferences.GetByID(m_dataSet.DataSetCore.CatalogID);
				}
				if (dataSetInfo == null)
				{
					dataSetInfo = m_odpContext.SharedDataSetReferences.GetByName(m_dataSet.DataSetCore.Name, m_odpContext.ReportContext);
				}
			}
			if (dataSetInfo == null)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidSharedDataSetReference, m_dataSet.Name.MarkAsPrivate(), m_dataSet.SharedDataSetQuery.SharedDataSetReference);
			}
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters = m_dataSet.SharedDataSetQuery.Parameters;
			SharedDataSetParameterNameMapper.MakeUnique(parameters);
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			object[] array = new object[parameters?.Count ?? 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].EvaluateQueryParameterValue(m_odpContext, m_dataSet.ExprHost);
				if (m_dataSet.IsReferenceToSharedDataSet)
				{
					ParameterInfo parameterInfo = new ParameterInfo(parameters[i]);
					parameterInfo.Name = parameters[i].UniqueName;
					parameterInfo.SetValuesFromQueryParameter(array[i]);
					parameterInfo.DataType = DataType.Object;
					parameterInfoCollection.Add(parameterInfo);
				}
			}
			m_odpContext.CheckAndThrowIfAborted();
			m_executionMetrics.StartTotalTimer();
			try
			{
				GetSharedDataSetChunkAndProcess(processAsIRowConsumer: true, dataSetInfo, parameterInfoCollection);
			}
			finally
			{
				m_executionMetrics.RecordTotalTimerMeasurement();
			}
			if (!m_odpContext.IsSharedDataSetExecutionOnly && m_dataSetInstance != null)
			{
				m_dataSetInstance.SaveCollationSettings(m_dataSet);
				UpdateReportOMDataSet();
			}
			return false;
		}

		private void GetSharedDataSetChunkAndProcess(bool processAsIRowConsumer, DataSetInfo dataSetInfo, ParameterInfoCollection datasetParameterCollection)
		{
			Global.Tracer.Assert(m_odpContext.ExternalProcessingContext != null && m_odpContext.ExternalProcessingContext.DataSetExecute != null, "Missing handler for shared dataset reference execution");
			string text = null;
			if (!m_odpContext.ProcessReportParameters)
			{
				text = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.GenerateDataChunkName(m_odpContext, m_dataSet.ID, m_odpContext.InSubreport);
			}
			IRowConsumer originalRequest = processAsIRowConsumer ? ((IRowConsumer)this) : null;
			bool originalRequestNeedsDataChunk = !processAsIRowConsumer || WritesDataChunk;
			m_odpContext.ExternalProcessingContext.DataSetExecute.Process(dataSetInfo, text, originalRequestNeedsDataChunk, originalRequest, datasetParameterCollection, m_odpContext.ExternalProcessingContext);
			if (processAsIRowConsumer)
			{
				if (!m_odpContext.ProcessReportParameters)
				{
					m_odpContext.OdpMetadata.AddDataChunk(text, m_dataSetInstance);
				}
			}
			else
			{
				m_dataReader = new ProcessingDataReader(m_dataSetInstance, m_dataSet, m_odpContext, overrideWithSharedDataSetChunkSettings: true);
			}
		}

		private bool RunEmbeddedQuery(List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParams, object[] paramValues)
		{
			Global.Tracer.Assert(m_odpContext.StateManager.ExecutedQueryCache == null, "When query execution caching is enabled, new queries must not be run outside query prefetch.");
			return ReaderExtensionsSupported(RunLiveQuery(queryParams, paramValues));
		}

		protected override void StoreDataReader(IDataReader reader, DataSourceErrorInspector errorInspector)
		{
			bool readerExtensionsSupportedLocal = ReaderExtensionsSupported(reader);
			if (reader.FieldCount > 0 || m_odpContext.IsSharedDataSetExecutionOnly)
			{
				CreateProcessingDataReader(reader, errorInspector, readerExtensionsSupportedLocal);
				return;
			}
			EagerInlineReaderCleanup(ref reader);
			DisposeCommand();
		}

		private static bool ReaderExtensionsSupported(IDataReader reader)
		{
			return reader is IDataReaderExtension;
		}

		protected override void SetRestartPosition(IDbCommand command)
		{
			if (m_odpContext.StreamingMode && !(command is IRestartable))
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidDataExtension);
			}
			try
			{
				if (m_restartPosition == null || !(command is IRestartable))
				{
					return;
				}
				List<ScopeValueFieldName> queryRestartPosition = m_restartPosition.GetQueryRestartPosition(m_dataSet);
				if (queryRestartPosition != null)
				{
					IDataParameter[] startAtParameters = ((IRestartable)command).StartAt(queryRestartPosition);
					if (m_odpContext.UseVerboseExecutionLogging)
					{
						m_executionMetrics.SetStartAtParameters(startAtParameters);
					}
				}
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingStartAt, innerException, m_dataSet.Name.MarkAsPrivate());
			}
		}

		protected override void ExtractRewrittenCommandText(IDbCommand command)
		{
			if (command is IDbCommandRewriter && m_dataSetInstance != null)
			{
				m_dataSetInstance.RewrittenCommandText = ((IDbCommandRewriter)command).RewrittenCommandText;
			}
		}

		protected override void StoreCommandText(string commandText)
		{
			m_dataSetInstance.CommandText = commandText;
		}

		private void CreateProcessingDataReader(IDataReader reader, DataSourceErrorInspector errorInspector, bool readerExtensionsSupportedLocal)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Field> fields = m_dataSet.Fields;
			int num = 0;
			if (fields != null)
			{
				num = ((!m_odpContext.IsSharedDataSetExecutionOnly) ? m_dataSet.NonCalculatedFieldCount : m_dataSet.Fields.Count);
			}
			string[] array = new string[num];
			string[] array2 = new string[num];
			for (int i = 0; i < num; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field = fields[i];
				array[i] = field.DataField;
				array2[i] = field.Name;
			}
			m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.DataReaderMapping);
			m_dataReader = new ProcessingDataReader(m_odpContext, m_dataSetInstance, m_dataSet.Name, reader, readerExtensionsSupportedLocal || m_dataSet.HasAggregateIndicatorFields, array2, array, errorInspector);
			m_executionMetrics.RecordTimerMeasurement(DataProcessingMetrics.MetricType.DataReaderMapping);
		}

		protected override void EagerInlineReaderCleanup(ref IDataReader reader)
		{
			if (m_dataReader != null)
			{
				reader = null;
				DisposeDataReader();
			}
			else
			{
				DisposeDataExtensionObject(ref reader, "data reader");
			}
		}

		internal virtual void EraseDataChunk()
		{
		}

		protected static void EraseDataChunk(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, ref Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter dataChunkWriter)
		{
			if (dataChunkWriter == null)
			{
				dataChunkWriter = new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter(dataSetInstance, odpContext);
			}
			dataChunkWriter.CloseAndEraseChunk();
			dataChunkWriter = null;
		}
	}
}
