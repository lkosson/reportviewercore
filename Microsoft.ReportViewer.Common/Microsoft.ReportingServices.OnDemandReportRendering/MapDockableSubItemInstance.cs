using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapDockableSubItemInstance : MapSubItemInstance
	{
		private MapDockableSubItem m_defObject;

		private MapPosition? m_position;

		private bool? m_dockOutsideViewport;

		private bool? m_hidden;

		private string m_toolTip;

		private bool m_toolTipEvaluated;

		public MapPosition Position
		{
			get
			{
				if (!m_position.HasValue)
				{
					m_position = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)m_defObject.MapSubItemDef).EvaluatePosition(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		public bool DockOutsideViewport
		{
			get
			{
				if (!m_dockOutsideViewport.HasValue)
				{
					m_dockOutsideViewport = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)m_defObject.MapSubItemDef).EvaluateDockOutsideViewport(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_dockOutsideViewport.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)m_defObject.MapSubItemDef).EvaluateHidden(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (!m_toolTipEvaluated)
				{
					m_toolTip = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)m_defObject.MapSubItemDef).EvaluateToolTip(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_toolTipEvaluated = true;
				}
				return m_toolTip;
			}
		}

		internal MapDockableSubItemInstance(MapDockableSubItem defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_position = null;
			m_dockOutsideViewport = null;
			m_hidden = null;
			m_toolTip = null;
			m_toolTipEvaluated = false;
		}
	}
}
