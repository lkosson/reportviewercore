using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class InternalChartSeries : ChartSeries, IROMActionOwner
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries m_chartSeriesDef;

		private ChartSeriesInstance m_instance;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportEnumProperty<ChartSeriesType> m_type;

		private ReportEnumProperty<ChartSeriesSubtype> m_subtype;

		private ChartSmartLabel m_smartLabel;

		private ChartEmptyPoints m_emptyPoints;

		private ReportStringProperty m_legendName;

		private ReportStringProperty m_legendText;

		private ReportBoolProperty m_hideInLegend;

		private ReportStringProperty m_chartAreaName;

		private ReportStringProperty m_valueAxisName;

		private ReportStringProperty m_categoryAxisName;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ChartDataLabel m_dataLabel;

		private ChartMarker m_marker;

		private IReportScope m_reportScope;

		private ChartDerivedSeries m_parentDerivedSeries;

		private List<ChartDerivedSeries> m_childrenDerivedSeries;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ChartItemInLegend m_chartItemInLegend;

		public override ChartDataPoint this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_chartDataPoints == null)
				{
					m_chartDataPoints = new ChartDataPoint[Count];
				}
				if (m_chartDataPoints[index] == null)
				{
					m_chartDataPoints[index] = new InternalChartDataPoint(m_chart, m_seriesIndex, index, m_chartSeriesDef.DataPoints[index]);
				}
				return m_chartDataPoints[index];
			}
		}

		public override int Count => m_chartSeriesDef.Cells.Count;

		public override string Name => m_chartSeriesDef.Name;

		public string UniqueName => m_chartSeriesDef.UniqueName;

		public override Style Style
		{
			get
			{
				if (m_style == null && m_chartSeriesDef.StyleClass != null)
				{
					m_style = new Style(m_chart, ReportScope, m_chartSeriesDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		internal override ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && m_chartSeriesDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, ReportScope, m_chartSeriesDef.Action, m_chartSeriesDef, m_chart, ObjectType.Chart, m_chart.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customProperties == null)
				{
					m_customPropertiesReady = true;
					m_customProperties = new CustomPropertyCollection(m_chart.ReportScope.ReportScopeInstance, m_chart.RenderingContext, null, m_chartSeriesDef, ObjectType.Chart, m_chart.Name);
				}
				else if (!m_customPropertiesReady)
				{
					m_customPropertiesReady = true;
					m_customProperties.UpdateCustomProperties(ReportScope.ReportScopeInstance, m_chartSeriesDef, m_chart.RenderingContext.OdpContext, ObjectType.Chart, m_chart.Name);
				}
				return m_customProperties;
			}
		}

		public override ReportEnumProperty<ChartSeriesType> Type
		{
			get
			{
				if (m_type == null && m_chartSeriesDef.Type != null)
				{
					m_type = new ReportEnumProperty<ChartSeriesType>(m_chartSeriesDef.Type.IsExpression, m_chartSeriesDef.Type.OriginalText, EnumTranslator.TranslateChartSeriesType(m_chartSeriesDef.Type.StringValue, null));
				}
				return m_type;
			}
		}

		public override ReportEnumProperty<ChartSeriesSubtype> Subtype
		{
			get
			{
				if (m_subtype == null && m_chartSeriesDef.Subtype != null)
				{
					m_subtype = new ReportEnumProperty<ChartSeriesSubtype>(m_chartSeriesDef.Subtype.IsExpression, m_chartSeriesDef.Subtype.OriginalText, EnumTranslator.TranslateChartSeriesSubtype(m_chartSeriesDef.Subtype.StringValue, null));
				}
				return m_subtype;
			}
		}

		public override ChartSmartLabel SmartLabel
		{
			get
			{
				if (m_smartLabel == null && m_chartSeriesDef.ChartSmartLabel != null)
				{
					m_smartLabel = new ChartSmartLabel(this, m_chartSeriesDef.ChartSmartLabel, m_chart);
				}
				return m_smartLabel;
			}
		}

		public override ChartEmptyPoints EmptyPoints
		{
			get
			{
				if (m_emptyPoints == null && m_chartSeriesDef.EmptyPoints != null)
				{
					m_emptyPoints = new ChartEmptyPoints(this, m_chartSeriesDef.EmptyPoints, m_chart);
				}
				return m_emptyPoints;
			}
		}

		public override ReportStringProperty LegendName
		{
			get
			{
				if (m_legendName == null && m_chartSeriesDef.LegendName != null)
				{
					m_legendName = new ReportStringProperty(m_chartSeriesDef.LegendName);
				}
				return m_legendName;
			}
		}

		internal override ReportStringProperty LegendText
		{
			get
			{
				if (m_legendText == null && m_chartSeriesDef.LegendText != null)
				{
					m_legendText = new ReportStringProperty(m_chartSeriesDef.LegendText);
				}
				return m_legendText;
			}
		}

		internal override ReportBoolProperty HideInLegend
		{
			get
			{
				if (m_hideInLegend == null && m_chartSeriesDef.HideInLegend != null)
				{
					m_hideInLegend = new ReportBoolProperty(m_chartSeriesDef.HideInLegend);
				}
				return m_hideInLegend;
			}
		}

		public override ReportStringProperty ChartAreaName
		{
			get
			{
				if (m_chartAreaName == null && m_chartSeriesDef.ChartAreaName != null)
				{
					m_chartAreaName = new ReportStringProperty(m_chartSeriesDef.ChartAreaName);
				}
				return m_chartAreaName;
			}
		}

		public override ReportStringProperty ValueAxisName
		{
			get
			{
				if (m_valueAxisName == null && m_chartSeriesDef.ValueAxisName != null)
				{
					m_valueAxisName = new ReportStringProperty(m_chartSeriesDef.ValueAxisName);
				}
				return m_valueAxisName;
			}
		}

		public override ReportStringProperty CategoryAxisName
		{
			get
			{
				if (m_categoryAxisName == null && m_chartSeriesDef.CategoryAxisName != null)
				{
					m_categoryAxisName = new ReportStringProperty(m_chartSeriesDef.CategoryAxisName);
				}
				return m_categoryAxisName;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				if (m_dataLabel == null && m_chartSeriesDef.DataLabel != null)
				{
					m_dataLabel = new ChartDataLabel(this, m_chartSeriesDef.DataLabel, m_chart);
				}
				return m_dataLabel;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				if (m_marker == null && m_chartSeriesDef.Marker != null)
				{
					m_marker = new ChartMarker(this, m_chartSeriesDef.Marker, m_chart);
				}
				return m_marker;
			}
		}

		internal override ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && m_chartSeriesDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartSeriesDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && m_chartSeriesDef.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_chartSeriesDef.Hidden);
				}
				return m_hidden;
			}
		}

		public override ChartItemInLegend ChartItemInLegend
		{
			get
			{
				if (m_chartItemInLegend == null && m_chartSeriesDef.ChartItemInLegend != null)
				{
					m_chartItemInLegend = new ChartItemInLegend(this, m_chartSeriesDef.ChartItemInLegend, m_chart);
				}
				return m_chartItemInLegend;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries ChartSeriesDef => m_chartSeriesDef;

		internal Chart ChartDef => m_chart;

		internal IReportScope ReportScope
		{
			get
			{
				if (m_reportScope == null)
				{
					if (m_parentDerivedSeries == null)
					{
						m_reportScope = m_chart.GetChartMember(this);
					}
					else
					{
						m_reportScope = m_parentDerivedSeries.ReportScope;
					}
				}
				return m_reportScope;
			}
		}

		public override ChartSeriesInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartSeriesInstance(this);
				}
				return m_instance;
			}
		}

		internal List<ChartDerivedSeries> ChildrenDerivedSeries
		{
			get
			{
				if (m_childrenDerivedSeries == null)
				{
					m_childrenDerivedSeries = m_chart.GetChildrenDerivedSeries(Name);
				}
				return m_childrenDerivedSeries;
			}
		}

		internal InternalChartSeries(ChartDerivedSeries parentDerivedSeries)
			: this(parentDerivedSeries.ChartDef, 0, parentDerivedSeries.ChartDerivedSeriesDef.Series)
		{
			m_parentDerivedSeries = parentDerivedSeries;
		}

		internal InternalChartSeries(Chart chart, int seriesIndex, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries seriesDef)
			: base(chart, seriesIndex)
		{
			m_chartSeriesDef = seriesDef;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_emptyPoints != null)
			{
				m_emptyPoints.SetNewContext();
			}
			if (m_smartLabel != null)
			{
				m_smartLabel.SetNewContext();
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.SetNewContext();
			}
			if (m_marker != null)
			{
				m_marker.SetNewContext();
			}
			if (m_chartDataPoints != null)
			{
				ChartDataPoint[] chartDataPoints = m_chartDataPoints;
				for (int i = 0; i < chartDataPoints.Length; i++)
				{
					chartDataPoints[i]?.SetNewContext();
				}
			}
			List<ChartDerivedSeries> childrenDerivedSeries = ChildrenDerivedSeries;
			if (childrenDerivedSeries != null)
			{
				foreach (ChartDerivedSeries item in childrenDerivedSeries)
				{
					item?.SetNewContext();
				}
			}
			if (m_chartItemInLegend != null)
			{
				m_chartItemInLegend.SetNewContext();
			}
			m_customPropertiesReady = false;
		}
	}
}
