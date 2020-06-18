namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class FrameBackgroundInstance : BaseInstance
	{
		private FrameBackground m_defObject;

		private StyleInstance m_style;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.GaugePanelDef, m_defObject.GaugePanelDef.RenderingContext);
				}
				return m_style;
			}
		}

		internal FrameBackgroundInstance(FrameBackground defObject)
			: base(defObject.GaugePanelDef)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
