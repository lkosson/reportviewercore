using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class FieldsImpl : Fields
	{
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

		private bool m_validAggregateRow;

		private bool m_addRowIndex;

		internal const string Name = "Fields";

		public override Field this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ReportProcessingException_NonExistingFieldReference();
				}
				ValidateFieldCollection();
				try
				{
					FieldImpl obj = m_collection[(int)m_nameMap[key]];
					obj.UsedInExpression = true;
					return obj;
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
					throw new ReportProcessingException_NonExistingFieldReference();
				}
			}
		}

		internal FieldImpl this[int index]
		{
			get
			{
				ValidateFieldCollection();
				try
				{
					FieldImpl obj = m_collection[index];
					obj.UsedInExpression = true;
					return obj;
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
					throw new ReportProcessingException_NonExistingFieldReference();
				}
			}
			set
			{
				Global.Tracer.Assert(m_collection != null);
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

		internal FieldsImpl(int size, bool addRowIndex)
		{
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
			m_noRows = true;
			m_validAggregateRow = true;
			m_addRowIndex = addRowIndex;
		}

		internal FieldsImpl()
		{
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
			m_noRows = false;
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
			m_collection[m_count - 1] = new FieldImpl(rowIndex, isAggregationField: false, null);
		}

		internal void SetFields(FieldImpl[] fields)
		{
			NewRow();
			Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
			if (fields == null)
			{
				for (int i = 0; i < m_count; i++)
				{
					Microsoft.ReportingServices.ReportProcessing.Field fieldDef = m_collection[i]?.FieldDef;
					m_collection[i] = new FieldImpl(null, isAggregationField: false, fieldDef);
				}
				return;
			}
			Global.Tracer.Assert(fields.Length == m_count, "(fields.Length == m_count)");
			for (int j = 0; j < m_count; j++)
			{
				m_collection[j] = fields[j];
			}
			m_isAggregateRow = false;
			m_aggregationFieldCount = m_aggregationFieldCountForDetailRow;
		}

		internal void SetFields(FieldImpl[] fields, bool isAggregateRow, int aggregationFieldCount, bool validAggregateRow)
		{
			SetFields(fields);
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
			Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
			return m_collection;
		}

		internal int GetRowIndex()
		{
			Global.Tracer.Assert(m_addRowIndex, "(m_addRowIndex)");
			Global.Tracer.Assert(m_count > 0, "(m_count > 0)");
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

		internal void ResetUsedInExpression()
		{
			if (m_collection != null)
			{
				for (int i = 0; i < m_collection.Length; i++)
				{
					m_collection[i].UsedInExpression = false;
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
				if (fieldImpl.UsedInExpression && fieldImpl.FieldDef != null)
				{
					fieldsUsedInValueExpression.Add(fieldImpl.FieldDef.DataField);
				}
			}
		}
	}
}
