using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class VarBase : DataAggregate
	{
		private DataTypeCode m_expressionType;

		protected uint m_currentCount;

		protected DataTypeCode m_sumOfXType;

		protected object m_sumOfX;

		protected object m_sumOfXSquared;

		internal override void Init()
		{
			m_currentCount = 0u;
			m_sumOfXType = DataTypeCode.Null;
			m_sumOfX = null;
			m_sumOfXSquared = null;
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
				object obj = DataAggregate.Square(doubleOrDecimalType, doubleOrDecimalData);
				if (m_sumOfX == null)
				{
					m_sumOfXType = doubleOrDecimalType;
					m_sumOfX = doubleOrDecimalData;
					m_sumOfXSquared = obj;
				}
				else
				{
					m_sumOfX = DataAggregate.Add(m_sumOfXType, m_sumOfX, doubleOrDecimalType, doubleOrDecimalData);
					m_sumOfXSquared = DataAggregate.Add(m_sumOfXType, m_sumOfXSquared, doubleOrDecimalType, obj);
				}
				m_currentCount++;
			}
		}
	}
}
