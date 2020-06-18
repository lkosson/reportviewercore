namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class Operand
	{
		internal enum OperandType
		{
			USHORT,
			DOUBLE,
			BOOLEAN,
			STRING,
			NAME
		}

		private object m_operandValue;

		private OperandType m_type;

		internal object OperandValue => m_operandValue;

		internal OperandType Type => m_type;

		internal Operand(object operandValue, OperandType type)
		{
			m_operandValue = operandValue;
			m_type = type;
		}
	}
}
