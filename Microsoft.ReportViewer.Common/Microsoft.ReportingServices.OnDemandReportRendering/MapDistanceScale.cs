using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDistanceScale : MapDockableSubItem
	{
		private ReportColorProperty m_scaleColor;

		private ReportColorProperty m_scaleBorderColor;

		public ReportColorProperty ScaleColor
		{
			get
			{
				if (m_scaleColor == null && MapDistanceScaleDef.ScaleColor != null)
				{
					ExpressionInfo scaleColor = MapDistanceScaleDef.ScaleColor;
					if (scaleColor != null)
					{
						m_scaleColor = new ReportColorProperty(scaleColor.IsExpression, MapDistanceScaleDef.ScaleColor.OriginalText, scaleColor.IsExpression ? null : new ReportColor(scaleColor.StringValue.Trim(), allowTransparency: true), scaleColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_scaleColor;
			}
		}

		public ReportColorProperty ScaleBorderColor
		{
			get
			{
				if (m_scaleBorderColor == null && MapDistanceScaleDef.ScaleBorderColor != null)
				{
					ExpressionInfo scaleBorderColor = MapDistanceScaleDef.ScaleBorderColor;
					if (scaleBorderColor != null)
					{
						m_scaleBorderColor = new ReportColorProperty(scaleBorderColor.IsExpression, MapDistanceScaleDef.ScaleBorderColor.OriginalText, scaleBorderColor.IsExpression ? null : new ReportColor(scaleBorderColor.StringValue.Trim(), allowTransparency: true), scaleBorderColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_scaleBorderColor;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale MapDistanceScaleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale)m_defObject;

		public new MapDistanceScaleInstance Instance => (MapDistanceScaleInstance)GetInstance();

		internal MapDistanceScale(Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapDistanceScaleInstance(this);
			}
			return (MapSubItemInstance)m_instance;
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
