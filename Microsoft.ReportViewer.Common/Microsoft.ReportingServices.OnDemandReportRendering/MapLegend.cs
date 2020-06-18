using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegend : MapDockableSubItem
	{
		private ReportEnumProperty<MapLegendLayout> m_layout;

		private MapLegendTitle m_mapLegendTitle;

		private ReportBoolProperty m_autoFitTextDisabled;

		private ReportSizeProperty m_minFontSize;

		private ReportBoolProperty m_interlacedRows;

		private ReportColorProperty m_interlacedRowsColor;

		private ReportBoolProperty m_equallySpacedItems;

		private ReportIntProperty m_textWrapThreshold;

		public string Name => MapLegendDef.Name;

		public ReportEnumProperty<MapLegendLayout> Layout
		{
			get
			{
				if (m_layout == null && MapLegendDef.Layout != null)
				{
					m_layout = new ReportEnumProperty<MapLegendLayout>(MapLegendDef.Layout.IsExpression, MapLegendDef.Layout.OriginalText, EnumTranslator.TranslateMapLegendLayout(MapLegendDef.Layout.StringValue, null));
				}
				return m_layout;
			}
		}

		public MapLegendTitle MapLegendTitle
		{
			get
			{
				if (m_mapLegendTitle == null && MapLegendDef.MapLegendTitle != null)
				{
					m_mapLegendTitle = new MapLegendTitle(MapLegendDef.MapLegendTitle, m_map);
				}
				return m_mapLegendTitle;
			}
		}

		public ReportBoolProperty AutoFitTextDisabled
		{
			get
			{
				if (m_autoFitTextDisabled == null && MapLegendDef.AutoFitTextDisabled != null)
				{
					m_autoFitTextDisabled = new ReportBoolProperty(MapLegendDef.AutoFitTextDisabled);
				}
				return m_autoFitTextDisabled;
			}
		}

		public ReportSizeProperty MinFontSize
		{
			get
			{
				if (m_minFontSize == null && MapLegendDef.MinFontSize != null)
				{
					m_minFontSize = new ReportSizeProperty(MapLegendDef.MinFontSize);
				}
				return m_minFontSize;
			}
		}

		public ReportBoolProperty InterlacedRows
		{
			get
			{
				if (m_interlacedRows == null && MapLegendDef.InterlacedRows != null)
				{
					m_interlacedRows = new ReportBoolProperty(MapLegendDef.InterlacedRows);
				}
				return m_interlacedRows;
			}
		}

		public ReportColorProperty InterlacedRowsColor
		{
			get
			{
				if (m_interlacedRowsColor == null && MapLegendDef.InterlacedRowsColor != null)
				{
					ExpressionInfo interlacedRowsColor = MapLegendDef.InterlacedRowsColor;
					if (interlacedRowsColor != null)
					{
						m_interlacedRowsColor = new ReportColorProperty(interlacedRowsColor.IsExpression, MapLegendDef.InterlacedRowsColor.OriginalText, interlacedRowsColor.IsExpression ? null : new ReportColor(interlacedRowsColor.StringValue.Trim(), allowTransparency: true), interlacedRowsColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_interlacedRowsColor;
			}
		}

		public ReportBoolProperty EquallySpacedItems
		{
			get
			{
				if (m_equallySpacedItems == null && MapLegendDef.EquallySpacedItems != null)
				{
					m_equallySpacedItems = new ReportBoolProperty(MapLegendDef.EquallySpacedItems);
				}
				return m_equallySpacedItems;
			}
		}

		public ReportIntProperty TextWrapThreshold
		{
			get
			{
				if (m_textWrapThreshold == null && MapLegendDef.TextWrapThreshold != null)
				{
					m_textWrapThreshold = new ReportIntProperty(MapLegendDef.TextWrapThreshold.IsExpression, MapLegendDef.TextWrapThreshold.OriginalText, MapLegendDef.TextWrapThreshold.IntValue, 0);
				}
				return m_textWrapThreshold;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend MapLegendDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject;

		public new MapLegendInstance Instance => (MapLegendInstance)GetInstance();

		internal MapLegend(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend defObject, Map map)
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
				m_instance = new MapLegendInstance(this);
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
			if (m_mapLegendTitle != null)
			{
				m_mapLegendTitle.SetNewContext();
			}
		}
	}
}
