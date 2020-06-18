namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSubItemInstance : BaseInstance
	{
		private MapSubItem m_defObject;

		private StyleInstance m_style;

		private ReportSize m_leftMargin;

		private ReportSize m_rightMargin;

		private ReportSize m_topMargin;

		private ReportSize m_bottomMargin;

		private int? m_zIndex;

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

		public ReportSize LeftMargin
		{
			get
			{
				if (m_leftMargin == null)
				{
					m_leftMargin = new ReportSize(m_defObject.MapSubItemDef.EvaluateLeftMargin(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_leftMargin;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				if (m_rightMargin == null)
				{
					m_rightMargin = new ReportSize(m_defObject.MapSubItemDef.EvaluateRightMargin(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_rightMargin;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				if (m_topMargin == null)
				{
					m_topMargin = new ReportSize(m_defObject.MapSubItemDef.EvaluateTopMargin(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_topMargin;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				if (m_bottomMargin == null)
				{
					m_bottomMargin = new ReportSize(m_defObject.MapSubItemDef.EvaluateBottomMargin(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_bottomMargin;
			}
		}

		public int ZIndex
		{
			get
			{
				if (!m_zIndex.HasValue)
				{
					m_zIndex = m_defObject.MapSubItemDef.EvaluateZIndex(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_zIndex.Value;
			}
		}

		internal MapSubItemInstance(MapSubItem defObject)
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
			m_leftMargin = null;
			m_rightMargin = null;
			m_topMargin = null;
			m_bottomMargin = null;
			m_zIndex = null;
		}
	}
}
