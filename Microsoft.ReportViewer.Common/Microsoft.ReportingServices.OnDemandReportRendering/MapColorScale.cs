using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScale : MapDockableSubItem
	{
		private MapColorScaleTitle m_mapColorScaleTitle;

		private ReportSizeProperty m_tickMarkLength;

		private ReportColorProperty m_colorBarBorderColor;

		private ReportIntProperty m_labelInterval;

		private ReportStringProperty m_labelFormat;

		private ReportEnumProperty<MapLabelPlacement> m_labelPlacement;

		private ReportEnumProperty<MapLabelBehavior> m_labelBehavior;

		private ReportBoolProperty m_hideEndLabels;

		private ReportColorProperty m_rangeGapColor;

		private ReportStringProperty m_noDataText;

		public MapColorScaleTitle MapColorScaleTitle
		{
			get
			{
				if (m_mapColorScaleTitle == null && MapColorScaleDef.MapColorScaleTitle != null)
				{
					m_mapColorScaleTitle = new MapColorScaleTitle(MapColorScaleDef.MapColorScaleTitle, m_map);
				}
				return m_mapColorScaleTitle;
			}
		}

		public ReportSizeProperty TickMarkLength
		{
			get
			{
				if (m_tickMarkLength == null && MapColorScaleDef.TickMarkLength != null)
				{
					m_tickMarkLength = new ReportSizeProperty(MapColorScaleDef.TickMarkLength);
				}
				return m_tickMarkLength;
			}
		}

		public ReportColorProperty ColorBarBorderColor
		{
			get
			{
				if (m_colorBarBorderColor == null && MapColorScaleDef.ColorBarBorderColor != null)
				{
					ExpressionInfo colorBarBorderColor = MapColorScaleDef.ColorBarBorderColor;
					if (colorBarBorderColor != null)
					{
						m_colorBarBorderColor = new ReportColorProperty(colorBarBorderColor.IsExpression, MapColorScaleDef.ColorBarBorderColor.OriginalText, colorBarBorderColor.IsExpression ? null : new ReportColor(colorBarBorderColor.StringValue.Trim(), allowTransparency: true), colorBarBorderColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_colorBarBorderColor;
			}
		}

		public ReportIntProperty LabelInterval
		{
			get
			{
				if (m_labelInterval == null && MapColorScaleDef.LabelInterval != null)
				{
					m_labelInterval = new ReportIntProperty(MapColorScaleDef.LabelInterval.IsExpression, MapColorScaleDef.LabelInterval.OriginalText, MapColorScaleDef.LabelInterval.IntValue, 0);
				}
				return m_labelInterval;
			}
		}

		public ReportStringProperty LabelFormat
		{
			get
			{
				if (m_labelFormat == null && MapColorScaleDef.LabelFormat != null)
				{
					m_labelFormat = new ReportStringProperty(MapColorScaleDef.LabelFormat);
				}
				return m_labelFormat;
			}
		}

		public ReportEnumProperty<MapLabelPlacement> LabelPlacement
		{
			get
			{
				if (m_labelPlacement == null && MapColorScaleDef.LabelPlacement != null)
				{
					m_labelPlacement = new ReportEnumProperty<MapLabelPlacement>(MapColorScaleDef.LabelPlacement.IsExpression, MapColorScaleDef.LabelPlacement.OriginalText, EnumTranslator.TranslateLabelPlacement(MapColorScaleDef.LabelPlacement.StringValue, null));
				}
				return m_labelPlacement;
			}
		}

		public ReportEnumProperty<MapLabelBehavior> LabelBehavior
		{
			get
			{
				if (m_labelBehavior == null && MapColorScaleDef.LabelBehavior != null)
				{
					m_labelBehavior = new ReportEnumProperty<MapLabelBehavior>(MapColorScaleDef.LabelBehavior.IsExpression, MapColorScaleDef.LabelBehavior.OriginalText, EnumTranslator.TranslateLabelBehavior(MapColorScaleDef.LabelBehavior.StringValue, null));
				}
				return m_labelBehavior;
			}
		}

		public ReportBoolProperty HideEndLabels
		{
			get
			{
				if (m_hideEndLabels == null && MapColorScaleDef.HideEndLabels != null)
				{
					m_hideEndLabels = new ReportBoolProperty(MapColorScaleDef.HideEndLabels);
				}
				return m_hideEndLabels;
			}
		}

		public ReportColorProperty RangeGapColor
		{
			get
			{
				if (m_rangeGapColor == null && MapColorScaleDef.RangeGapColor != null)
				{
					ExpressionInfo rangeGapColor = MapColorScaleDef.RangeGapColor;
					if (rangeGapColor != null)
					{
						m_rangeGapColor = new ReportColorProperty(rangeGapColor.IsExpression, MapColorScaleDef.RangeGapColor.OriginalText, rangeGapColor.IsExpression ? null : new ReportColor(rangeGapColor.StringValue.Trim(), allowTransparency: true), rangeGapColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_rangeGapColor;
			}
		}

		public ReportStringProperty NoDataText
		{
			get
			{
				if (m_noDataText == null && MapColorScaleDef.NoDataText != null)
				{
					m_noDataText = new ReportStringProperty(MapColorScaleDef.NoDataText);
				}
				return m_noDataText;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale MapColorScaleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject;

		public new MapColorScaleInstance Instance => (MapColorScaleInstance)GetInstance();

		internal MapColorScale(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale defObject, Map map)
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
				m_instance = new MapColorScaleInstance(this);
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
			if (m_mapColorScaleTitle != null)
			{
				m_mapColorScaleTitle.SetNewContext();
			}
		}
	}
}
