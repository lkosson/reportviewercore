using System.Collections;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class OptimizedChoose : Operator
	{
		private ArrayList m_gotoLabels;

		internal ArrayList GotoLabelList
		{
			get
			{
				return m_gotoLabels;
			}
			set
			{
				m_gotoLabels = value;
			}
		}

		internal OptimizedChoose(string op, int precedence, OperatorType ot, ushort biffCode)
			: base(op, precedence, ot, biffCode)
		{
		}

		internal OptimizedChoose(OptimizedChoose oc)
			: base(oc.Name, oc.Precedence, oc.Type, oc.BCode)
		{
		}
	}
}
