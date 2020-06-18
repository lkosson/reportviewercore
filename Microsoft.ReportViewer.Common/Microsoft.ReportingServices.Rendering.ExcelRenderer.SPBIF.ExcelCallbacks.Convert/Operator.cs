using System;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal class Operator
	{
		internal enum OperatorType
		{
			ARITHMETIC,
			LOGICAL,
			FUNCTION,
			DELIMITER
		}

		private string m_operator;

		private int m_precedence;

		private OperatorType m_type;

		private ushort m_biffCode;

		private uint m_functionCode;

		private short m_numOfArgs;

		private bool m_variableArgs;

		internal string Name => m_operator;

		internal int Precedence => m_precedence;

		internal OperatorType Type => m_type;

		internal ushort BCode => m_biffCode;

		internal uint FCode => m_functionCode;

		internal short ArgumentCount
		{
			get
			{
				return m_numOfArgs;
			}
			set
			{
				m_numOfArgs = value;
			}
		}

		internal byte[] BiffOperator => BitConverter.GetBytes(m_biffCode);

		internal byte[] FunctionCode => BitConverter.GetBytes(m_functionCode);

		internal byte[] NumberOfArguments => BitConverter.GetBytes(m_numOfArgs);

		internal Operator(string op, int precedence, OperatorType ot, ushort biffCode)
		{
			m_operator = op;
			m_precedence = precedence;
			m_type = ot;
			m_biffCode = biffCode;
		}

		internal Operator(string op, int precedence, OperatorType ot, ushort biffCode, uint functionCode)
		{
			m_operator = op;
			m_precedence = precedence;
			m_type = ot;
			m_biffCode = biffCode;
			m_functionCode = functionCode;
		}

		internal Operator(string op, int precedence, OperatorType ot, ushort biffCode, uint functionCode, short numOfArgs)
		{
			m_operator = op;
			m_precedence = precedence;
			m_type = ot;
			m_biffCode = biffCode;
			m_functionCode = functionCode;
			if (biffCode == 66 && numOfArgs == -1)
			{
				m_numOfArgs = 0;
				m_variableArgs = true;
			}
			else
			{
				m_numOfArgs = numOfArgs;
			}
		}

		internal Operator(Operator op)
		{
			m_operator = op.m_operator;
			m_precedence = op.m_precedence;
			m_type = op.m_type;
			m_biffCode = op.m_biffCode;
			m_functionCode = op.m_functionCode;
			m_numOfArgs = op.m_numOfArgs;
		}

		internal bool HasVariableArguments()
		{
			return m_variableArgs;
		}
	}
}
