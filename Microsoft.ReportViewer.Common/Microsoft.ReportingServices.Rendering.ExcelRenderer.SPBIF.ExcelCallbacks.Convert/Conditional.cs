namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class Conditional : Operator
	{
		private string m_gotoLabel;

		private string m_label;

		internal string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal string GotoLabel
		{
			get
			{
				return m_gotoLabel;
			}
			set
			{
				m_gotoLabel = value;
			}
		}

		internal Conditional(string op, int precedence, OperatorType ot, ushort biffCode)
			: base(op, precedence, ot, biffCode)
		{
		}

		internal Conditional(string op, int precedence, OperatorType ot, ushort biffCode, uint functionCode, short numOfArgs)
			: base(op, precedence, ot, biffCode, functionCode, numOfArgs)
		{
		}

		internal Conditional(Conditional conditionalOp)
			: base(conditionalOp.Name, conditionalOp.Precedence, conditionalOp.Type, conditionalOp.BCode, conditionalOp.FCode, conditionalOp.ArgumentCount)
		{
			m_label = conditionalOp.Label;
		}
	}
}
