using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColor : MapObjectCollectionItem
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor m_defObject;

		private ReportColorProperty m_color;

		public ReportColorProperty Color
		{
			get
			{
				if (m_color == null && m_defObject.Color != null)
				{
					ExpressionInfo color = m_defObject.Color;
					if (color != null)
					{
						m_color = new ReportColorProperty(color.IsExpression, m_defObject.Color.OriginalText, color.IsExpression ? null : new ReportColor(color.StringValue.Trim(), allowTransparency: true), color.IsExpression ? new ReportColor("", System.Drawing.Color.Empty, parsed: true) : null);
					}
				}
				return m_color;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor MapCustomColorDef => m_defObject;

		public MapCustomColorInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapCustomColorInstance(this);
				}
				return (MapCustomColorInstance)m_instance;
			}
		}

		internal MapCustomColor(Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor defObject, Map map)
		{
			m_defObject = defObject;
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
