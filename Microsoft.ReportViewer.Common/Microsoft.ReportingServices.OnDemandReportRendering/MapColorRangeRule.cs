using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorRangeRule : MapColorRule
	{
		private ReportColorProperty m_startColor;

		private ReportColorProperty m_middleColor;

		private ReportColorProperty m_endColor;

		public ReportColorProperty StartColor
		{
			get
			{
				if (m_startColor == null && MapColorRangeRuleDef.StartColor != null)
				{
					ExpressionInfo startColor = MapColorRangeRuleDef.StartColor;
					if (startColor != null)
					{
						m_startColor = new ReportColorProperty(startColor.IsExpression, MapColorRangeRuleDef.StartColor.OriginalText, startColor.IsExpression ? null : new ReportColor(startColor.StringValue.Trim(), allowTransparency: true), startColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_startColor;
			}
		}

		public ReportColorProperty MiddleColor
		{
			get
			{
				if (m_middleColor == null && MapColorRangeRuleDef.MiddleColor != null)
				{
					ExpressionInfo middleColor = MapColorRangeRuleDef.MiddleColor;
					if (middleColor != null)
					{
						m_middleColor = new ReportColorProperty(middleColor.IsExpression, MapColorRangeRuleDef.MiddleColor.OriginalText, middleColor.IsExpression ? null : new ReportColor(middleColor.StringValue.Trim(), allowTransparency: true), middleColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_middleColor;
			}
		}

		public ReportColorProperty EndColor
		{
			get
			{
				if (m_endColor == null && MapColorRangeRuleDef.EndColor != null)
				{
					ExpressionInfo endColor = MapColorRangeRuleDef.EndColor;
					if (endColor != null)
					{
						m_endColor = new ReportColorProperty(endColor.IsExpression, MapColorRangeRuleDef.EndColor.OriginalText, endColor.IsExpression ? null : new ReportColor(endColor.StringValue.Trim(), allowTransparency: true), endColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_endColor;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule MapColorRangeRuleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)base.MapAppearanceRuleDef;

		public new MapColorRangeRuleInstance Instance => (MapColorRangeRuleInstance)GetInstance();

		internal MapColorRangeRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapColorRangeRuleInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
