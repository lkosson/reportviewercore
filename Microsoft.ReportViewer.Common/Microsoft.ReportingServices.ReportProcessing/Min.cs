using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Min : DataAggregate
	{
		private DataTypeCode m_expressionType;

		private object m_currentMin;

		private CompareInfo m_compareInfo;

		private CompareOptions m_compareOptions;

		internal Min(CompareInfo compareInfo, CompareOptions compareOptions)
		{
			m_currentMin = null;
			m_compareInfo = compareInfo;
			m_compareOptions = compareOptions;
		}

		internal override void Init()
		{
			m_currentMin = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			object obj = expressions[0];
			DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (DataAggregate.IsNull(typeCode))
			{
				return;
			}
			if (!DataAggregate.IsVariant(typeCode))
			{
				iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
				throw new InvalidOperationException();
			}
			if (m_expressionType == DataTypeCode.Null)
			{
				m_expressionType = typeCode;
			}
			else if (typeCode != m_expressionType)
			{
				iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
				throw new InvalidOperationException();
			}
			if (m_currentMin == null)
			{
				m_currentMin = obj;
				return;
			}
			try
			{
				if (ReportProcessing.CompareTo(m_currentMin, obj, m_compareInfo, m_compareOptions) > 0)
				{
					m_currentMin = obj;
				}
			}
			catch
			{
				iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
			}
		}

		internal override object Result()
		{
			return m_currentMin;
		}
	}
}
