using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class NumericIndicatorExprHost : GaugePanelItemExprHost
	{
		public GaugeInputValueExprHost GaugeInputValueHost;

		[CLSCompliant(false)]
		protected IList<NumericIndicatorRangeExprHost> m_numericIndicatorRangesHostsRemotable;

		public GaugeInputValueExprHost MinimumValueHost;

		public GaugeInputValueExprHost MaximumValueHost;

		[CLSCompliant(false)]
		public IList<NumericIndicatorRangeExprHost> NumericIndicatorRangesHostsRemotable
		{
			get
			{
				return m_numericIndicatorRangesHostsRemotable;
			}
		}

		public virtual object DecimalDigitColorExpr => null;

		public virtual object DigitColorExpr => null;

		public virtual object UseFontPercentExpr => null;

		public virtual object DecimalDigitsExpr => null;

		public virtual object DigitsExpr => null;

		public virtual object MultiplierExpr => null;

		public virtual object NonNumericStringExpr => null;

		public virtual object OutOfRangeStringExpr => null;

		public virtual object ResizeModeExpr => null;

		public virtual object ShowDecimalPointExpr => null;

		public virtual object ShowLeadingZerosExpr => null;

		public virtual object IndicatorStyleExpr => null;

		public virtual object ShowSignExpr => null;

		public virtual object SnappingEnabledExpr => null;

		public virtual object SnappingIntervalExpr => null;

		public virtual object LedDimColorExpr => null;

		public virtual object SeparatorWidthExpr => null;

		public virtual object SeparatorColorExpr => null;
	}
}
