using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class FieldsContext
	{
		private FieldsImpl m_fields;

		private DataSetCore m_dataSet;

		private DataSetInstance m_dataSetInstance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader m_dataReader;

		private bool m_allFieldsCleared;

		private bool m_pendingFieldValueUpdate;

		private long m_lastRowOffset;

		private FieldImpl[] m_noRowsFields;

		internal bool AllFieldsCleared => m_allFieldsCleared;

		internal bool PendingFieldValueUpdate => m_pendingFieldValueUpdate;

		internal long LastRowOffset => m_lastRowOffset;

		internal DataSetCore DataSet => m_dataSet;

		internal DataSetInstance DataSetInstance => m_dataSetInstance;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader DataReader => m_dataReader;

		internal FieldsImpl Fields => m_fields;

		internal FieldsContext(ObjectModelImpl reportOM)
			: this(reportOM, null)
		{
		}

		internal FieldsContext(ObjectModelImpl reportOM, DataSetCore dataSet)
		{
			Initialize(reportOM, new FieldsImpl(reportOM), dataSet, null, null, allFieldsCleared: true, pendingFieldValueUpdate: false, DataFieldRow.UnInitializedStreamOffset);
		}

		internal FieldsContext(ObjectModelImpl reportOM, DataSetCore dataSet, bool addRowIndex, bool noRows)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Field> fields = dataSet.Fields;
			int num = fields?.Count ?? 0;
			FieldsImpl fieldsImpl = new FieldsImpl(reportOM, num, addRowIndex, noRows);
			Initialize(reportOM, fieldsImpl, dataSet, null, null, allFieldsCleared: true, pendingFieldValueUpdate: false, DataFieldRow.UnInitializedStreamOffset);
			for (int i = 0; i < num; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field = fields[i];
				if (dataSet.ExprHost != null)
				{
					field.SetExprHost(dataSet.ExprHost, reportOM);
				}
				fieldsImpl.Add(field.Name, null);
			}
			if (addRowIndex)
			{
				fieldsImpl.AddRowIndexField();
			}
		}

		private void Initialize(ObjectModelImpl reportOM, FieldsImpl fields, DataSetCore dataSet, DataSetInstance dataSetInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader, bool allFieldsCleared, bool pendingFieldValueUpdate, long lastRowOffset)
		{
			m_fields = fields;
			m_dataSet = dataSet;
			m_dataSetInstance = dataSetInstance;
			m_dataReader = dataReader;
			m_allFieldsCleared = allFieldsCleared;
			m_pendingFieldValueUpdate = pendingFieldValueUpdate;
			m_lastRowOffset = lastRowOffset;
			AttachToDataSetCache(reportOM);
		}

		internal void AttachToDataSetCache(ObjectModelImpl reportOM)
		{
			if (m_dataSet != null && reportOM.UseDataSetFieldsCache)
			{
				m_dataSet.FieldsContext = this;
			}
		}

		internal void ResetFieldFlags()
		{
			m_pendingFieldValueUpdate = false;
			m_allFieldsCleared = true;
		}

		internal void UpdateDataSetInfo(DataSetInstance dataSetInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader)
		{
			m_dataSetInstance = dataSetInstance;
			m_dataReader = dataChunkReader;
		}

		internal void CreateNoRows()
		{
			if (m_noRowsFields == null)
			{
				m_fields.SetFields(null, DataFieldRow.UnInitializedStreamOffset);
				m_noRowsFields = m_fields.GetAndSaveFields();
			}
			else
			{
				m_fields.SetFields(m_noRowsFields, DataFieldRow.UnInitializedStreamOffset);
			}
			ResetFieldFlags();
		}

		internal void CreateNullFieldValues()
		{
			int count = m_fields.Count;
			for (int i = 0; i < count; i++)
			{
				m_fields.GetFieldByIndex(i)?.UpdateValue(null, isAggregationField: false, DataFieldStatus.None, null);
			}
			ResetFieldFlags();
		}

		internal void PerformPendingFieldValueUpdate(ObjectModelImpl reportOM, bool useDataSetFieldsCache)
		{
			if (m_pendingFieldValueUpdate)
			{
				m_pendingFieldValueUpdate = false;
				UpdateFieldValues(reportOM, useDataSetFieldsCache, m_lastRowOffset);
			}
		}

		internal void RegisterOnDemandFieldValueUpdate(long firstRowOffsetInScope, DataSetInstance dataSetInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader)
		{
			m_pendingFieldValueUpdate = true;
			m_lastRowOffset = firstRowOffsetInScope;
			m_dataSetInstance = dataSetInstance;
			m_dataReader = dataReader;
		}

		internal void UpdateFieldValues(ObjectModelImpl reportOM, bool useDataSetFieldsCache, long firstRowOffsetInScope)
		{
			if (m_dataReader.ReadOneRowAtPosition(firstRowOffsetInScope) || m_allFieldsCleared)
			{
				UpdateFieldValues(reportOM, useDataSetFieldsCache, reuseFieldObjects: true, m_dataReader.RecordRow, m_dataSetInstance, m_dataReader.ReaderExtensionsSupported);
			}
		}

		internal void UpdateFieldValues(ObjectModelImpl reportOM, bool useDataSetFieldsCache, bool reuseFieldObjects, Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row, DataSetInstance dataSetInstance, bool readerExtensionsSupported)
		{
			Global.Tracer.Assert(row != null, "Empty data row / no data reader");
			if (m_dataSetInstance != dataSetInstance)
			{
				m_dataSetInstance = dataSetInstance;
				m_dataSet = dataSetInstance.DataSetDef.DataSetCore;
				if (m_dataSet.FieldsContext != null && useDataSetFieldsCache)
				{
					m_fields = m_dataSet.FieldsContext.Fields;
				}
				else
				{
					reuseFieldObjects = false;
				}
				m_dataReader = null;
				m_lastRowOffset = DataFieldRow.UnInitializedStreamOffset;
				m_pendingFieldValueUpdate = false;
			}
			m_allFieldsCleared = false;
			FieldInfo[] fieldInfos = dataSetInstance.FieldInfos;
			if (m_fields.ReaderExtensionsSupported && m_dataSet.InterpretSubtotalsAsDetails == Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState.False)
			{
				m_fields.IsAggregateRow = row.IsAggregateRow;
				m_fields.AggregationFieldCount = row.AggregationFieldCount;
				if (!row.IsAggregateRow)
				{
					m_fields.AggregationFieldCountForDetailRow = row.AggregationFieldCount;
				}
			}
			int num = 0;
			int count = m_dataSet.Fields.Count;
			int num2 = row.RecordFields.Length;
			for (num = 0; num < num2; num++)
			{
				FieldImpl fieldImpl = reuseFieldObjects ? m_fields.GetFieldByIndex(num) : null;
				Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef = m_dataSet.Fields[num];
				Microsoft.ReportingServices.ReportIntermediateFormat.RecordField recordField = row.RecordFields[num];
				if (recordField == null)
				{
					if (!reuseFieldObjects || fieldImpl == null)
					{
						fieldImpl = new FieldImpl(reportOM, DataFieldStatus.IsMissing, null, fieldDef);
					}
					else
					{
						fieldImpl.UpdateValue(null, isAggregationField: false, DataFieldStatus.IsMissing, null);
					}
				}
				else if (recordField.FieldStatus == DataFieldStatus.None)
				{
					if (!reuseFieldObjects || fieldImpl == null)
					{
						fieldImpl = new FieldImpl(reportOM, recordField.FieldValue, recordField.IsAggregationField, fieldDef);
					}
					else
					{
						fieldImpl.UpdateValue(recordField.FieldValue, recordField.IsAggregationField, DataFieldStatus.None, null);
					}
				}
				else if (!reuseFieldObjects || fieldImpl == null)
				{
					fieldImpl = new FieldImpl(reportOM, recordField.FieldStatus, ReportRuntime.GetErrorName(recordField.FieldStatus, null), fieldDef);
				}
				else
				{
					fieldImpl.UpdateValue(null, isAggregationField: false, recordField.FieldStatus, ReportRuntime.GetErrorName(recordField.FieldStatus, null));
				}
				if (recordField != null && fieldInfos != null)
				{
					FieldInfo fieldInfo = fieldInfos[num];
					if (fieldInfo != null && fieldInfo.PropertyCount != 0 && recordField.FieldPropertyValues != null)
					{
						for (int i = 0; i < fieldInfo.PropertyCount; i++)
						{
							fieldImpl.SetProperty(fieldInfo.PropertyNames[i], recordField.FieldPropertyValues[i]);
						}
					}
				}
				m_fields[num] = fieldImpl;
			}
			if (num >= count)
			{
				return;
			}
			if (!reuseFieldObjects && reportOM.OdpContext.ReportRuntime.ReportExprHost != null)
			{
				m_dataSet.SetExprHost(reportOM.OdpContext.ReportRuntime.ReportExprHost, reportOM);
			}
			for (; num < count; num++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef2 = m_dataSet.Fields[num];
				FieldImpl fieldImpl2 = reuseFieldObjects ? m_fields.GetFieldByIndex(num) : null;
				if (reuseFieldObjects && fieldImpl2 != null)
				{
					if (!fieldImpl2.ResetCalculatedField())
					{
						CreateAndInitializeCalculatedFieldWrapper(reportOM, readerExtensionsSupported, m_dataSet, num, fieldDef2);
					}
				}
				else
				{
					CreateAndInitializeCalculatedFieldWrapper(reportOM, readerExtensionsSupported, m_dataSet, num, fieldDef2);
				}
			}
		}

		private void CreateAndInitializeCalculatedFieldWrapper(ObjectModelImpl reportOM, bool readerExtensionsSupported, DataSetCore dataSet, int fieldIndex, Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			CalculatedFieldWrapperImpl value = new CalculatedFieldWrapperImpl(fieldDef, reportOM.OdpContext.ReportRuntime);
			bool isAggregationField = (!readerExtensionsSupported) ? true : false;
			if (dataSet.InterpretSubtotalsAsDetails == Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState.True)
			{
				isAggregationField = true;
			}
			m_fields[fieldIndex] = new FieldImpl(reportOM, value, isAggregationField, fieldDef);
			if (dataSet.ExprHost != null && fieldDef.ExprHost == null)
			{
				fieldDef.SetExprHost(dataSet.ExprHost, reportOM);
			}
		}
	}
}
