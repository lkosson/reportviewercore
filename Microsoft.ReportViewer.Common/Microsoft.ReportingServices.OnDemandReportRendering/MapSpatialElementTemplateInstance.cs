namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElementTemplateInstance : BaseInstance
	{
		private MapSpatialElementTemplate m_defObject;

		private StyleInstance m_style;

		private bool? m_hidden;

		private double? m_offsetX;

		private double? m_offsetY;

		private string m_label;

		private bool m_labelEvaluated;

		private string m_toolTip;

		private bool m_toolTipEvaluated;

		private string m_dataElementLabel;

		private bool m_dataElementLabelEvaluated;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.ReportScope, m_defObject.MapDef.RenderingContext);
				}
				return m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.MapSpatialElementTemplateDef.EvaluateHidden(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public double OffsetX
		{
			get
			{
				if (!m_offsetX.HasValue)
				{
					m_offsetX = m_defObject.MapSpatialElementTemplateDef.EvaluateOffsetX(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_offsetX.Value;
			}
		}

		public double OffsetY
		{
			get
			{
				if (!m_offsetY.HasValue)
				{
					m_offsetY = m_defObject.MapSpatialElementTemplateDef.EvaluateOffsetY(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_offsetY.Value;
			}
		}

		public string Label
		{
			get
			{
				if (!m_labelEvaluated)
				{
					m_label = m_defObject.MapSpatialElementTemplateDef.EvaluateLabel(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_labelEvaluated = true;
				}
				return m_label;
			}
		}

		public string ToolTip
		{
			get
			{
				if (!m_toolTipEvaluated)
				{
					m_toolTip = m_defObject.MapSpatialElementTemplateDef.EvaluateToolTip(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_toolTipEvaluated = true;
				}
				return m_toolTip;
			}
		}

		public string DataElementLabel
		{
			get
			{
				if (!m_dataElementLabelEvaluated)
				{
					m_dataElementLabel = m_defObject.MapSpatialElementTemplateDef.EvaluateDataElementLabel(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_dataElementLabelEvaluated = true;
				}
				if (m_dataElementLabel == null)
				{
					return Label;
				}
				return m_dataElementLabel;
			}
		}

		internal MapSpatialElementTemplateInstance(MapSpatialElementTemplate defObject)
			: base(defObject.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_hidden = null;
			m_offsetX = null;
			m_offsetY = null;
			m_label = null;
			m_labelEvaluated = false;
			m_toolTip = null;
			m_toolTipEvaluated = false;
			m_dataElementLabel = null;
			m_dataElementLabelEvaluated = false;
		}
	}
}
