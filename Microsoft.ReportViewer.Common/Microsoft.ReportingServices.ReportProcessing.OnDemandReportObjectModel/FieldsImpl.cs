using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class FieldsImpl : Fields
	{
		private ObjectModelImpl m_reportOM;

		private Hashtable m_nameMap;

		private bool[] m_fieldMissing;

		private bool[] m_fieldError;

		private FieldImpl[] m_collection;

		private int m_count;

		private bool m_referenced;

		private bool m_readerExtensionsSupported;

		private bool m_readerFieldProperties;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		private int m_aggregationFieldCountForDetailRow;

		private bool m_noRows;

		private long m_streamOffset = DataFieldRow.UnInitializedStreamOffset;

		private bool m_validAggregateRow;

		private bool m_addRowIndex;

		private bool m_needsInlineSetup;

		public override Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.Field this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ReportProcessingException_NonExistingFieldReference(key.MarkAsPrivate());
				}
				ValidateFieldCollection();
				try
				{
					int index = (int)m_nameMap[key];
					return CheckedGetFieldByIndex(index);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException_NonExistingFieldReference(key.MarkAsPrivate());
				}
			}
		}

		internal bool IsCollectionInitialized => m_collection != null;

		internal FieldImpl this[int index]
		{
			get
			{
				ValidateFieldCollection();
				return CheckedGetFieldByIndex(index);
			}
			set
			{
				Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
				m_collection[index] = value;
			}
		}

		internal int Count => m_count - (m_addRowIndex ? 1 : 0);

		internal int CountWithRowIndex => m_count;

		internal bool ReaderExtensionsSupported
		{
			get
			{
				return m_readerExtensionsSupported;
			}
			set
			{
				m_readerExtensionsSupported = value;
			}
		}

		internal bool ReaderFieldProperties
		{
			get
			{
				return m_readerFieldProperties;
			}
			set
			{
				m_readerFieldProperties = value;
			}
		}

		internal bool IsAggregateRow
		{
			get
			{
				return m_isAggregateRow;
			}
			set
			{
				m_isAggregateRow = value;
			}
		}

		internal int AggregationFieldCount
		{
			get
			{
				return m_aggregationFieldCount;
			}
			set
			{
				m_aggregationFieldCount = value;
			}
		}

		internal int AggregationFieldCountForDetailRow
		{
			set
			{
				m_aggregationFieldCountForDetailRow = value;
			}
		}

		internal bool ValidAggregateRow
		{
			get
			{
				return m_validAggregateRow;
			}
			set
			{
				m_validAggregateRow = value;
			}
		}

		internal bool AddRowIndex => m_addRowIndex;

		internal bool NeedsInlineSetup
		{
			get
			{
				return m_needsInlineSetup;
			}
			set
			{
				m_needsInlineSetup = value;
			}
		}

		internal long StreamOffset => m_streamOffset;

		internal FieldsImpl(ObjectModelImpl reportOM, int size, bool addRowIndex, bool noRows)
		{
			m_reportOM = reportOM;
			if (addRowIndex)
			{
				m_collection = new FieldImpl[size + 1];
			}
			else
			{
				m_collection = new FieldImpl[size];
			}
			m_nameMap = new Hashtable(size);
			m_fieldMissing = null;
			m_count = 0;
			m_referenced = false;
			m_readerExtensionsSupported = false;
			m_isAggregateRow = false;
			m_aggregationFieldCount = size;
			m_aggregationFieldCountForDetailRow = size;
			m_noRows = noRows;
			m_validAggregateRow = true;
			m_addRowIndex = addRowIndex;
		}

		internal FieldsImpl(ObjectModelImpl reportOM)
		{
			m_reportOM = reportOM;
			m_collection = null;
			m_nameMap = null;
			m_fieldMissing = null;
			m_count = 0;
			m_referenced = false;
			m_readerExtensionsSupported = false;
			m_isAggregateRow = false;
			m_aggregationFieldCount = 0;
			m_aggregationFieldCountForDetailRow = 0;
			m_noRows = true;
			m_validAggregateRow = true;
			m_addRowIndex = false;
		}

		internal FieldImpl GetFieldByIndex(int index)
		{
			return m_collection[index];
		}

		private FieldImpl CheckedGetFieldByIndex(int index)
		{
			try
			{
				if (m_collection[index] == null || m_collection[index].IsCalculatedField)
				{
					m_reportOM.PerformPendingFieldValueUpdate();
				}
				return m_collection[index];
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				throw new ReportProcessingException_NonExistingFieldReference();
			}
		}

		internal void Add(string name, FieldImpl field)
		{
			Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
			Global.Tracer.Assert(m_nameMap != null, "(null != m_nameMap)");
			Global.Tracer.Assert(m_count < m_collection.Length, "(m_count < m_collection.Length)");
			m_nameMap.Add(name, m_count);
			m_collection[m_count] = field;
			m_count++;
		}

		internal void AddRowIndexField()
		{
			Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
			Global.Tracer.Assert(m_count < m_collection.Length, "(m_count < m_collection.Length)");
			m_collection[m_count] = null;
			m_count++;
		}

		internal void SetFieldIsMissing(int index)
		{
			if (m_fieldMissing == null)
			{
				m_fieldMissing = new bool[m_collection.Length];
			}
			m_fieldMissing[index] = true;
		}

		internal bool IsFieldMissing(int index)
		{
			if (m_fieldMissing == null)
			{
				return false;
			}
			return m_fieldMissing[index];
		}

		internal void SetFieldErrorRegistered(int index)
		{
			if (m_fieldError == null)
			{
				m_fieldError = new bool[m_collection.Length];
			}
			m_fieldError[index] = true;
		}

		internal bool IsFieldErrorRegistered(int index)
		{
			if (m_fieldError == null)
			{
				return false;
			}
			return m_fieldError[index];
		}

		internal void NewRow()
		{
			NewRow(DataFieldRow.UnInitializedStreamOffset);
		}

		internal void NewRow(long streamOffset)
		{
			m_noRows = false;
			m_validAggregateRow = true;
			m_streamOffset = streamOffset;
			if (m_referenced)
			{
				m_collection = new FieldImpl[m_count];
				m_referenced = false;
			}
		}

		internal void SetRowIndex(int rowIndex)
		{
			Global.Tracer.Assert(m_addRowIndex, "(m_addRowIndex)");
			Global.Tracer.Assert(m_count > 0, "(m_count > 0)");
			m_collection[m_count - 1] = new FieldImpl(m_reportOM, rowIndex, isAggregationField: false, null);
		}

		internal void SetFields(FieldImpl[] fields, long streamOffset)
		{
			bool flag = m_referenced || streamOffset == DataFieldRow.UnInitializedStreamOffset || m_streamOffset != streamOffset;
			NewRow(streamOffset);
			if (m_collection == null)
			{
				Global.Tracer.Assert(condition: false, "Invalid FieldsImpl.  m_collection should not be null.");
			}
			if (fields == null)
			{
				for (int i = 0; i < m_count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef = m_collection[i]?.FieldDef;
					m_collection[i] = new FieldImpl(m_reportOM, null, isAggregationField: false, fieldDef);
				}
			}
			else if (flag)
			{
				if (fields.Length != m_count)
				{
					Global.Tracer.Assert(false, "Wrong number of fields during ReportOM update.  Usually this means the data set is wrong.  Expected: {0}.  Actual: {1}", m_count, fields.Length);
				}
				for (int j = 0; j < m_count; j++)
				{
					m_collection[j] = fields[j];
				}
				m_isAggregateRow = false;
				m_aggregationFieldCount = m_aggregationFieldCountForDetailRow;
			}
		}

		internal void SetFields(FieldImpl[] fields, long streamOffset, bool isAggregateRow, int aggregationFieldCount, bool validAggregateRow)
		{
			SetFields(fields, streamOffset);
			m_isAggregateRow = isAggregateRow;
			m_aggregationFieldCount = aggregationFieldCount;
			m_validAggregateRow = validAggregateRow;
		}

		internal FieldImpl[] GetAndSaveFields()
		{
			Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
			m_referenced = true;
			return m_collection;
		}

		internal FieldImpl[] GetFields()
		{
			return m_collection;
		}

		internal int GetRowIndex()
		{
			return (int)m_collection[m_count - 1].Value;
		}

		internal void Clone(FieldsImpl fields)
		{
			if (fields != null)
			{
				m_collection = fields.m_collection;
				m_nameMap = fields.m_nameMap;
				m_count = fields.m_count;
				m_referenced = fields.m_referenced;
				m_noRows = fields.m_noRows;
				m_fieldMissing = fields.m_fieldMissing;
			}
		}

		private bool ValidateFieldCollection()
		{
			if (m_needsInlineSetup)
			{
				m_needsInlineSetup = false;
				m_reportOM.OdpContext.PrepareFieldsCollectionForDirectFields();
			}
			if (m_nameMap == null || m_collection == null)
			{
				throw new ReportProcessingException_NonExistingFieldReference();
			}
			if (m_noRows)
			{
				throw new ReportProcessingException_NoRowsFieldAccess();
			}
			return true;
		}

		internal void ResetFieldsUsedInExpression()
		{
			if (m_collection == null)
			{
				return;
			}
			for (int i = 0; i < m_collection.Length; i++)
			{
				FieldImpl fieldImpl = m_collection[i];
				if (fieldImpl != null)
				{
					fieldImpl.UsedInExpression = false;
				}
			}
		}

		internal void AddFieldsUsedInExpression(List<string> fieldsUsedInValueExpression)
		{
			if (m_collection == null)
			{
				return;
			}
			for (int i = 0; i < m_collection.Length; i++)
			{
				FieldImpl fieldImpl = m_collection[i];
				if (fieldImpl != null && fieldImpl.UsedInExpression && fieldImpl.FieldDef != null && fieldImpl.FieldDef.DataField != null)
				{
					fieldsUsedInValueExpression.Add(fieldImpl.FieldDef.DataField);
				}
			}
		}

		internal void ConsumeAggregationField(int fieldIndex)
		{
			FieldImpl fieldImpl = this[fieldIndex];
			if (!fieldImpl.AggregationFieldChecked && fieldImpl.IsAggregationField)
			{
				fieldImpl.AggregationFieldChecked = true;
				m_aggregationFieldCount--;
			}
		}
	}
}
