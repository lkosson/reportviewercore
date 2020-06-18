using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapLayer : MapObjectCollectionItem
	{
		protected Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer m_defObject;

		private ReportEnumProperty<MapVisibilityMode> m_visibilityMode;

		private ReportDoubleProperty m_minimumZoom;

		private ReportDoubleProperty m_maximumZoom;

		private ReportDoubleProperty m_transparency;

		public string Name => m_defObject.Name;

		public ReportEnumProperty<MapVisibilityMode> VisibilityMode
		{
			get
			{
				if (m_visibilityMode == null && m_defObject.VisibilityMode != null)
				{
					m_visibilityMode = new ReportEnumProperty<MapVisibilityMode>(m_defObject.VisibilityMode.IsExpression, m_defObject.VisibilityMode.OriginalText, EnumTranslator.TranslateMapVisibilityMode(m_defObject.VisibilityMode.StringValue, null));
				}
				return m_visibilityMode;
			}
		}

		public ReportDoubleProperty MinimumZoom
		{
			get
			{
				if (m_minimumZoom == null && m_defObject.MinimumZoom != null)
				{
					m_minimumZoom = new ReportDoubleProperty(m_defObject.MinimumZoom);
				}
				return m_minimumZoom;
			}
		}

		public ReportDoubleProperty MaximumZoom
		{
			get
			{
				if (m_maximumZoom == null && m_defObject.MaximumZoom != null)
				{
					m_maximumZoom = new ReportDoubleProperty(m_defObject.MaximumZoom);
				}
				return m_maximumZoom;
			}
		}

		public ReportDoubleProperty Transparency
		{
			get
			{
				if (m_transparency == null && m_defObject.Transparency != null)
				{
					m_transparency = new ReportDoubleProperty(m_defObject.Transparency);
				}
				return m_transparency;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer MapLayerDef => m_defObject;

		internal MapLayerInstance Instance => GetInstance();

		internal MapLayer(Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal abstract MapLayerInstance GetInstance();

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
