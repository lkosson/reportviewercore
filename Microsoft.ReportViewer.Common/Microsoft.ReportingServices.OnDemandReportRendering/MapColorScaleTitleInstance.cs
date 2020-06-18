namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScaleTitleInstance : BaseInstance
	{
		private MapColorScaleTitle m_defObject;

		private StyleInstance m_style;

		private string m_caption;

		private bool m_captionEvaluated;

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
					m_caption = m_defObject.MapColorScaleTitleDef.EvaluateCaption(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_captionEvaluated = true;
				}
				return m_caption;
			}
		}

		internal MapColorScaleTitleInstance(MapColorScaleTitle defObject)
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
		}
	}
}
