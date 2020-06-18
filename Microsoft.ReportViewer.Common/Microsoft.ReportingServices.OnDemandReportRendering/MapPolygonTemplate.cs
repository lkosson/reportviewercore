using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonTemplate : MapSpatialElementTemplate
	{
		private ReportDoubleProperty m_scaleFactor;

		private ReportDoubleProperty m_centerPointOffsetX;

		private ReportDoubleProperty m_centerPointOffsetY;

		private ReportEnumProperty<MapAutoBool> m_showLabel;

		private ReportEnumProperty<MapPolygonLabelPlacement> m_labelPlacement;

		public ReportDoubleProperty ScaleFactor
		{
			get
			{
				if (m_scaleFactor == null && MapPolygonTemplateDef.ScaleFactor != null)
				{
					m_scaleFactor = new ReportDoubleProperty(MapPolygonTemplateDef.ScaleFactor);
				}
				return m_scaleFactor;
			}
		}

		public ReportDoubleProperty CenterPointOffsetX
		{
			get
			{
				if (m_centerPointOffsetX == null && MapPolygonTemplateDef.CenterPointOffsetX != null)
				{
					m_centerPointOffsetX = new ReportDoubleProperty(MapPolygonTemplateDef.CenterPointOffsetX);
				}
				return m_centerPointOffsetX;
			}
		}

		public ReportDoubleProperty CenterPointOffsetY
		{
			get
			{
				if (m_centerPointOffsetY == null && MapPolygonTemplateDef.CenterPointOffsetY != null)
				{
					m_centerPointOffsetY = new ReportDoubleProperty(MapPolygonTemplateDef.CenterPointOffsetY);
				}
				return m_centerPointOffsetY;
			}
		}

		public ReportEnumProperty<MapAutoBool> ShowLabel
		{
			get
			{
				if (m_showLabel == null && MapPolygonTemplateDef.ShowLabel != null)
				{
					m_showLabel = new ReportEnumProperty<MapAutoBool>(MapPolygonTemplateDef.ShowLabel.IsExpression, MapPolygonTemplateDef.ShowLabel.OriginalText, EnumTranslator.TranslateMapAutoBool(MapPolygonTemplateDef.ShowLabel.StringValue, null));
				}
				return m_showLabel;
			}
		}

		public ReportEnumProperty<MapPolygonLabelPlacement> LabelPlacement
		{
			get
			{
				if (m_labelPlacement == null && MapPolygonTemplateDef.LabelPlacement != null)
				{
					m_labelPlacement = new ReportEnumProperty<MapPolygonLabelPlacement>(MapPolygonTemplateDef.LabelPlacement.IsExpression, MapPolygonTemplateDef.LabelPlacement.OriginalText, EnumTranslator.TranslateMapPolygonLabelPlacement(MapPolygonTemplateDef.LabelPlacement.StringValue, null));
				}
				return m_labelPlacement;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate MapPolygonTemplateDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)base.MapSpatialElementTemplateDef;

		public new MapPolygonTemplateInstance Instance => (MapPolygonTemplateInstance)GetInstance();

		internal MapPolygonTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate defObject, MapPolygonLayer shapeLayer, Map map)
			: base(defObject, shapeLayer, map)
		{
		}

		internal override MapSpatialElementTemplateInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapPolygonTemplateInstance(this);
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
