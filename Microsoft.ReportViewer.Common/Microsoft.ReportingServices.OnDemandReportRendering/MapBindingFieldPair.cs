using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBindingFieldPair : MapObjectCollectionItem
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair m_defObject;

		private ReportStringProperty m_fieldName;

		private ReportVariantProperty m_bindingExpression;

		public ReportStringProperty FieldName
		{
			get
			{
				if (m_fieldName == null && m_defObject.FieldName != null)
				{
					m_fieldName = new ReportStringProperty(m_defObject.FieldName);
				}
				return m_fieldName;
			}
		}

		public ReportVariantProperty BindingExpression
		{
			get
			{
				if (m_bindingExpression == null && m_defObject.BindingExpression != null)
				{
					m_bindingExpression = new ReportVariantProperty(m_defObject.BindingExpression);
				}
				return m_bindingExpression;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair MapBindingFieldPairDef => m_defObject;

		public MapBindingFieldPairInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapBindingFieldPairInstance(this);
				}
				return (MapBindingFieldPairInstance)m_instance;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (m_mapVectorLayer != null)
				{
					return m_mapVectorLayer.ReportScope;
				}
				return MapDef.ReportScope;
			}
		}

		internal MapBindingFieldPair(Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			m_defObject = defObject;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
