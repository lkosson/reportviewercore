using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class CountDistinct : DataAggregate
	{
		private Hashtable m_distinctValues = new Hashtable();

		internal override void Init()
		{
			m_distinctValues.Clear();
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			object obj = expressions[0];
			DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataAggregate.IsVariant(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning);
					throw new InvalidOperationException();
				}
				if (!m_distinctValues.ContainsKey(obj))
				{
					m_distinctValues.Add(obj, null);
				}
			}
		}

		internal override object Result()
		{
			return m_distinctValues.Count;
		}
	}
}
