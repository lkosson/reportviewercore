using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Max : DataAggregate
	{
		private DataTypeCode m_expressionType;

		private object m_currentMax;

		private CompareInfo m_compareInfo;

		private CompareOptions m_compareOptions;

		internal Max(CompareInfo compareInfo, CompareOptions compareOptions)
		{
			m_currentMax = null;
			m_compareInfo = compareInfo;
			m_compareOptions = compareOptions;
		}

		internal override void Init()
		{
			m_currentMax = null;
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
			if (m_currentMax == null)
			{
				m_currentMax = obj;
				return;
			}
			try
			{
				if (ReportProcessing.CompareTo(m_currentMax, obj, m_compareInfo, m_compareOptions) < 0)
				{
					m_currentMax = obj;
				}
			}
			catch
			{
				iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
			}
		}

		internal override object Result()
		{
			return m_currentMax;
		}
	}
}
