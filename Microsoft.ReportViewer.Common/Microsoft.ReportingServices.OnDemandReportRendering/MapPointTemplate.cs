using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapPointTemplate : MapSpatialElementTemplate
	{
		private ReportSizeProperty m_size;

		private ReportEnumProperty<MapPointLabelPlacement> m_labelPlacement;

		public ReportSizeProperty Size
		{
			get
			{
				if (m_size == null && MapPointTemplateDef.Size != null)
				{
					m_size = new ReportSizeProperty(MapPointTemplateDef.Size);
				}
				return m_size;
			}
		}

		public ReportEnumProperty<MapPointLabelPlacement> LabelPlacement
		{
			get
			{
				if (m_labelPlacement == null && MapPointTemplateDef.LabelPlacement != null)
				{
					m_labelPlacement = new ReportEnumProperty<MapPointLabelPlacement>(MapPointTemplateDef.LabelPlacement.IsExpression, MapPointTemplateDef.LabelPlacement.OriginalText, EnumTranslator.TranslateMapPointLabelPlacement(MapPointTemplateDef.LabelPlacement.StringValue, null));
				}
				return m_labelPlacement;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate MapPointTemplateDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate)base.MapSpatialElementTemplateDef;

		internal new MapPointTemplateInstance Instance => (MapPointTemplateInstance)GetInstance();

		internal MapPointTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
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
