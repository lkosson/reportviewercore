using Microsoft.ReportingServices.RdlExpressions;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class ChartMemberInstance : BaseInstance
	{
		protected Chart m_owner;

		protected ChartMember m_memberDef;

		protected bool m_labelEvaluated;

		protected string m_label;

		protected StyleInstance m_style;

		private VariantResult? m_labelObject;

		public object LabelObject => GetLabelObject().Value;

		internal TypeCode LabelTypeCode => GetLabelObject().TypeCode;

		public string Label
		{
			get
			{
				if (!m_labelEvaluated)
				{
					m_labelEvaluated = true;
					if (m_owner.IsOldSnapshot)
					{
						object value = GetLabelObject().Value;
						if (value != null)
						{
							m_label = value.ToString();
						}
					}
					else
					{
						m_label = m_memberDef.MemberDefinition.GetFormattedLabelValue(GetLabelObject(), m_owner.RenderingContext.OdpContext);
					}
				}
				return m_label;
			}
		}

		internal ChartMemberInstance(Chart owner, ChartMember memberDef)
			: base(memberDef.ReportScope)
		{
			m_owner = owner;
			m_memberDef = memberDef;
		}

		private VariantResult GetLabelObject()
		{
			if (!m_labelObject.HasValue)
			{
				if (m_owner.IsOldSnapshot)
				{
					m_labelObject = new VariantResult(errorOccurred: false, ((ShimChartMember)m_memberDef).LabelInstanceValue);
				}
				else
				{
					m_labelObject = m_memberDef.MemberDefinition.EvaluateLabel(this, m_owner.RenderingContext.OdpContext);
				}
			}
			return m_labelObject.Value;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_labelEvaluated = false;
			m_labelObject = null;
		}
	}
}
