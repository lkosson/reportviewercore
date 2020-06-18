namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class GaugePanelItemInstance : BaseInstance
	{
		private GaugePanelItem m_defObject;

		private StyleInstance m_style;

		private double? m_top;

		private double? m_left;

		private double? m_height;

		private double? m_width;

		private int? m_zIndex;

		private bool? m_hidden;

		private string m_toolTip;

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

		public double Top
		{
			get
			{
				if (!m_top.HasValue)
				{
					m_top = m_defObject.GaugePanelItemDef.EvaluateTop(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_top.Value;
			}
		}

		public double Left
		{
			get
			{
				if (!m_left.HasValue)
				{
					m_left = m_defObject.GaugePanelItemDef.EvaluateLeft(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_left.Value;
			}
		}

		public double Height
		{
			get
			{
				if (!m_height.HasValue)
				{
					m_height = m_defObject.GaugePanelItemDef.EvaluateHeight(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_height.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!m_width.HasValue)
				{
					m_width = m_defObject.GaugePanelItemDef.EvaluateWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		public int ZIndex
		{
			get
			{
				if (!m_zIndex.HasValue)
				{
					m_zIndex = m_defObject.GaugePanelItemDef.EvaluateZIndex(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_zIndex.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.GaugePanelItemDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_defObject.GaugePanelItemDef.EvaluateToolTip(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		internal GaugePanelItemInstance(GaugePanelItem defObject)
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
			m_top = null;
			m_left = null;
			m_height = null;
			m_width = null;
			m_zIndex = null;
			m_hidden = null;
			m_toolTip = null;
		}
	}
}
