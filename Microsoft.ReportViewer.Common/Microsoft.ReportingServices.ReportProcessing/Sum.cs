using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class Sum : DataAggregate
	{
		private DataTypeCode m_expressionType;

		protected DataTypeCode m_currentTotalType;

		protected object m_currentTotal;

		internal override void Init()
		{
			m_currentTotalType = DataTypeCode.Null;
			m_currentTotal = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			object doubleOrDecimalData = expressions[0];
			DataTypeCode doubleOrDecimalType = DataAggregate.GetTypeCode(doubleOrDecimalData);
			if (!DataAggregate.IsNull(doubleOrDecimalType))
			{
				if (!DataTypeUtility.IsNumeric(doubleOrDecimalType))
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfNonNumericData, Severity.Warning);
					throw new InvalidOperationException();
				}
				if (m_expressionType == DataTypeCode.Null)
				{
					m_expressionType = doubleOrDecimalType;
				}
				else if (doubleOrDecimalType != m_expressionType)
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
					throw new InvalidOperationException();
				}
				DataAggregate.ConvertToDoubleOrDecimal(doubleOrDecimalType, doubleOrDecimalData, out doubleOrDecimalType, out doubleOrDecimalData);
				if (m_currentTotal == null)
				{
					m_currentTotalType = doubleOrDecimalType;
					m_currentTotal = doubleOrDecimalData;
				}
				else
				{
					m_currentTotal = DataAggregate.Add(m_currentTotalType, m_currentTotal, doubleOrDecimalType, doubleOrDecimalData);
				}
			}
		}

		internal override object Result()
		{
			return m_currentTotal;
		}
	}
}
