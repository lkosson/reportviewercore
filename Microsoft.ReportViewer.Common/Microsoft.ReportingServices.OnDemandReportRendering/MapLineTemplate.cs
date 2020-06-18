using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineTemplate : MapSpatialElementTemplate
	{
		private ReportSizeProperty m_width;

		private ReportEnumProperty<MapLineLabelPlacement> m_labelPlacement;

		public ReportSizeProperty Width
		{
			get
			{
				if (m_width == null && MapLineTemplateDef.Width != null)
				{
					m_width = new ReportSizeProperty(MapLineTemplateDef.Width);
				}
				return m_width;
			}
		}

		public ReportEnumProperty<MapLineLabelPlacement> LabelPlacement
		{
			get
			{
				if (m_labelPlacement == null && MapLineTemplateDef.LabelPlacement != null)
				{
					m_labelPlacement = new ReportEnumProperty<MapLineLabelPlacement>(MapLineTemplateDef.LabelPlacement.IsExpression, MapLineTemplateDef.LabelPlacement.OriginalText, EnumTranslator.TranslateMapLineLabelPlacement(MapLineTemplateDef.LabelPlacement.StringValue, null));
				}
				return m_labelPlacement;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate MapLineTemplateDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate)base.MapSpatialElementTemplateDef;

		public new MapLineTemplateInstance Instance => (MapLineTemplateInstance)GetInstance();

		internal MapLineTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate defObject, MapLineLayer mapLineLayer, Map map)
			: base(defObject, mapLineLayer, map)
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
				m_instance = new MapLineTemplateInstance(this);
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
