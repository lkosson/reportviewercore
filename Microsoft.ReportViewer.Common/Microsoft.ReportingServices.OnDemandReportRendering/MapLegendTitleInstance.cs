namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendTitleInstance : BaseInstance
	{
		private MapLegendTitle m_defObject;

		private StyleInstance m_style;

		private string m_caption;

		private bool m_captionEvaluated;

		private MapLegendTitleSeparator? m_titleSeparator;

		private ReportColor m_titleSeparatorColor;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.MapDef.ReportScope, m_defObject.MapDef.RenderingContext);
				}
				return m_style;
			}
		}

		public string Caption
		{
			get
			{
				if (!m_captionEvaluated)
				{
					m_caption = m_defObject.MapLegendTitleDef.EvaluateCaption(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_captionEvaluated = true;
				}
				return m_caption;
			}
		}

		public MapLegendTitleSeparator TitleSeparator
		{
			get
			{
				if (!m_titleSeparator.HasValue)
				{
					m_titleSeparator = m_defObject.MapLegendTitleDef.EvaluateTitleSeparator(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_titleSeparator.Value;
			}
		}

		public ReportColor TitleSeparatorColor
		{
			get
			{
				if (m_titleSeparatorColor == null)
				{
					m_titleSeparatorColor = new ReportColor(m_defObject.MapLegendTitleDef.EvaluateTitleSeparatorColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_titleSeparatorColor;
			}
		}

		internal MapLegendTitleInstance(MapLegendTitle defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_caption = null;
			m_captionEvaluated = false;
			m_titleSeparator = null;
			m_titleSeparatorColor = null;
		}
	}
}
