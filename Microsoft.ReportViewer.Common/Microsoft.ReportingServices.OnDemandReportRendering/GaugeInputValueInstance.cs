using Microsoft.ReportingServices.RdlExpressions;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeInputValueInstance : BaseInstance
	{
		private GaugeInputValue m_defObject;

		private VariantResult? m_valueResult;

		private GaugeInputValueFormulas? m_formula;

		private double? m_minPercent;

		private double? m_maxPercent;

		private double? m_multiplier;

		private double? m_addConstant;

		public object Value
		{
			get
			{
				EnsureValueIsEvaluated();
				return m_valueResult.Value.Value;
			}
		}

		internal TypeCode ValueTypeCode
		{
			get
			{
				EnsureValueIsEvaluated();
				return m_valueResult.Value.TypeCode;
			}
		}

		internal bool ErrorOccured
		{
			get
			{
				EnsureValueIsEvaluated();
				return m_valueResult.Value.ErrorOccurred;
			}
		}

		public GaugeInputValueFormulas Formula
		{
			get
			{
				if (!m_formula.HasValue)
				{
					m_formula = m_defObject.GaugeInputValueDef.EvaluateFormula(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_formula.Value;
			}
		}

		public double MinPercent
		{
			get
			{
				if (!m_minPercent.HasValue)
				{
					m_minPercent = m_defObject.GaugeInputValueDef.EvaluateMinPercent(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_minPercent.Value;
			}
		}

		public double MaxPercent
		{
			get
			{
				if (!m_maxPercent.HasValue)
				{
					m_maxPercent = m_defObject.GaugeInputValueDef.EvaluateMaxPercent(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_maxPercent.Value;
			}
		}

		public double Multiplier
		{
			get
			{
				if (!m_multiplier.HasValue)
				{
					m_multiplier = m_defObject.GaugeInputValueDef.EvaluateMultiplier(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_multiplier.Value;
			}
		}

		public double AddConstant
		{
			get
			{
				if (!m_addConstant.HasValue)
				{
					m_addConstant = m_defObject.GaugeInputValueDef.EvaluateAddConstant(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_addConstant.Value;
			}
		}

		internal GaugeInputValueInstance(GaugeInputValue defObject)
			: base((GaugeCell)defObject.GaugePanelDef.RowCollection.GetIfExists(0).GetIfExists(0))
		{
			m_defObject = defObject;
		}

		private void EnsureValueIsEvaluated()
		{
			if (!m_valueResult.HasValue)
			{
				m_valueResult = m_defObject.GaugeInputValueDef.EvaluateValue(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
			}
		}

		protected override void ResetInstanceCache()
		{
			m_valueResult = null;
			m_formula = null;
			m_minPercent = null;
			m_maxPercent = null;
			m_multiplier = null;
			m_addConstant = null;
		}
	}
}
