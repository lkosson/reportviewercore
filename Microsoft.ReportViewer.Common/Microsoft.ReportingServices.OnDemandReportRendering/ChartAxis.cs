using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxis : ChartObjectCollectionItem<ChartAxisInstance>, IROMStyleDefinitionContainer
	{
		public enum TickMarks
		{
			None,
			Inside,
			Outside,
			Cross
		}

		public enum Locations
		{
			Default,
			Opposite
		}

		private ChartGridLines m_majorGridlines;

		private ChartGridLines m_minorGridlines;

		private ReportVariantProperty m_crossAt;

		private ReportVariantProperty m_min;

		private ReportVariantProperty m_max;

		private ChartAxisTitle m_title;

		private Style m_style;

		private bool m_isCategory;

		private Chart m_chart;

		private AxisInstance m_renderAxisInstance;

		private Axis m_renderAxisDef;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis m_axisDef;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ChartStripLineCollection m_chartStripLines;

		private ReportEnumProperty<ChartAutoBool> m_visible;

		private ReportEnumProperty<ChartAutoBool> m_margin;

		private ReportDoubleProperty m_interval;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		private ReportDoubleProperty m_labelInterval;

		private ReportEnumProperty<ChartIntervalType> m_labelIntervalType;

		private ReportDoubleProperty m_labelIntervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_labelIntervalOffsetType;

		private ReportBoolProperty m_variableAutoInterval;

		private ChartTickMarks m_majorTickMarks;

		private ChartTickMarks m_minorTickMarks;

		private ReportBoolProperty m_marksAlwaysAtPlotEdge;

		private ReportBoolProperty m_reverse;

		private ReportEnumProperty<ChartAxisLocation> m_location;

		private ReportBoolProperty m_interlaced;

		private ReportColorProperty m_interlacedColor;

		private ReportBoolProperty m_logScale;

		private ReportDoubleProperty m_logBase;

		private ReportBoolProperty m_hideLabels;

		private ReportDoubleProperty m_angle;

		private ReportBoolProperty m_preventFontShrink;

		private ReportBoolProperty m_preventFontGrow;

		private ReportBoolProperty m_preventLabelOffset;

		private ReportBoolProperty m_preventWordWrap;

		private ReportEnumProperty<ChartAxisLabelRotation> m_allowLabelRotation;

		private ReportBoolProperty m_includeZero;

		private ReportBoolProperty m_labelsAutoFitDisabled;

		private ReportSizeProperty m_minFontSize;

		private ReportSizeProperty m_maxFontSize;

		private ReportBoolProperty m_offsetLabels;

		private ReportBoolProperty m_hideEndLabels;

		private ReportEnumProperty<ChartAxisArrow> m_arrows;

		private ChartAxisScaleBreak m_axisScaleBreak;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_style = new Style(m_renderAxisDef.StyleClass, m_renderAxisInstance.StyleAttributeValues, m_chart.RenderingContext);
					}
					else if (m_axisDef.StyleClass != null)
					{
						m_style = new Style(m_chart, m_chart, m_axisDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ChartAxisTitle Title
		{
			get
			{
				if (m_title == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_title = new ChartAxisTitle(m_renderAxisDef.Title, m_renderAxisInstance.Title, m_chart);
					}
					else if (m_axisDef.Title != null)
					{
						m_title = new ChartAxisTitle(m_axisDef.Title, m_chart);
					}
				}
				return m_title;
			}
		}

		public string Name
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					if (!m_isCategory)
					{
						return Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis.Mode.ValueAxis.ToString();
					}
					return Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis.Mode.CategoryAxis.ToString();
				}
				return m_axisDef.AxisName;
			}
		}

		public ChartGridLines MajorGridLines
		{
			get
			{
				if (m_majorGridlines == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_axisDef.MajorGridLines != null)
						{
							m_majorGridlines = new ChartGridLines(m_renderAxisDef.MajorGridLines, m_renderAxisInstance.MajorGridLinesStyleAttributeValues, m_chart);
						}
					}
					else if (m_axisDef.MajorGridLines != null)
					{
						m_majorGridlines = new ChartGridLines(m_axisDef.MajorGridLines, m_chart);
					}
				}
				return m_majorGridlines;
			}
		}

		public ChartGridLines MinorGridLines
		{
			get
			{
				if (m_minorGridlines == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_renderAxisDef.MinorGridLines != null)
						{
							m_minorGridlines = new ChartGridLines(m_renderAxisDef.MinorGridLines, m_renderAxisInstance.MinorGridLinesStyleAttributeValues, m_chart);
						}
					}
					else if (m_axisDef.MinorGridLines != null)
					{
						m_minorGridlines = new ChartGridLines(m_axisDef.MinorGridLines, m_chart);
					}
				}
				return m_minorGridlines;
			}
		}

		public ReportVariantProperty CrossAt
		{
			get
			{
				if (m_crossAt == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_renderAxisDef.CrossAt != null)
						{
							m_crossAt = new ReportVariantProperty(m_renderAxisDef.CrossAt);
						}
					}
					else if (m_axisDef.CrossAt != null)
					{
						m_crossAt = new ReportVariantProperty(m_axisDef.CrossAt);
					}
				}
				return m_crossAt;
			}
		}

		public ChartStripLineCollection StripLines
		{
			get
			{
				if (m_chartStripLines == null && !m_chart.IsOldSnapshot && AxisDef.StripLines != null)
				{
					m_chartStripLines = new ChartStripLineCollection(this, m_chart);
				}
				return m_chartStripLines;
			}
		}

		public bool Scalar
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return m_renderAxisDef.Scalar;
				}
				return m_axisDef.Scalar;
			}
		}

		public ReportVariantProperty Minimum
		{
			get
			{
				if (m_min == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_renderAxisDef.Min != null)
						{
							m_min = new ReportVariantProperty(m_renderAxisDef.Min);
						}
					}
					else if (m_axisDef.Minimum != null)
					{
						m_min = new ReportVariantProperty(m_axisDef.Minimum);
					}
				}
				return m_min;
			}
		}

		public ReportVariantProperty Maximum
		{
			get
			{
				if (m_max == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_renderAxisDef.Max != null)
						{
							m_max = new ReportVariantProperty(m_renderAxisDef.Max);
						}
					}
					else if (m_axisDef.Maximum != null)
					{
						m_max = new ReportVariantProperty(m_axisDef.Maximum);
					}
				}
				return m_max;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customProperties == null)
				{
					m_customPropertiesReady = true;
					if (m_chart.IsOldSnapshot)
					{
						m_customProperties = new CustomPropertyCollection(m_chart.RenderingContext, new Microsoft.ReportingServices.ReportRendering.CustomPropertyCollection(m_renderAxisDef.CustomProperties, m_renderAxisInstance.CustomPropertyInstances));
					}
					else
					{
						m_customProperties = new CustomPropertyCollection(m_chart.ReportScope.ReportScopeInstance, m_chart.RenderingContext, null, m_axisDef, ObjectType.Chart, m_chart.Name);
					}
				}
				else if (!m_customPropertiesReady)
				{
					m_customPropertiesReady = true;
					if (m_chart.IsOldSnapshot)
					{
						m_customProperties.UpdateCustomProperties(new Microsoft.ReportingServices.ReportRendering.CustomPropertyCollection(m_renderAxisDef.CustomProperties, m_renderAxisInstance.CustomPropertyInstances));
					}
					else
					{
						m_customProperties.UpdateCustomProperties(m_chart.ReportScope.ReportScopeInstance, m_axisDef, m_chart.RenderingContext.OdpContext, ObjectType.Chart, m_chart.Name);
					}
				}
				return m_customProperties;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Visible
		{
			get
			{
				if (m_visible == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_visible = new ReportEnumProperty<ChartAutoBool>(m_renderAxisDef.Visible ? ChartAutoBool.True : ChartAutoBool.False);
					}
					else if (m_axisDef.Visible != null)
					{
						m_visible = new ReportEnumProperty<ChartAutoBool>(m_axisDef.Visible.IsExpression, m_axisDef.Visible.OriginalText, (!m_axisDef.Visible.IsExpression) ? EnumTranslator.TranslateChartAutoBool(m_axisDef.Visible.StringValue, null) : ChartAutoBool.Auto);
					}
				}
				return m_visible;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Margin
		{
			get
			{
				if (m_margin == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_margin = new ReportEnumProperty<ChartAutoBool>(m_renderAxisDef.Margin ? ChartAutoBool.True : ChartAutoBool.False);
					}
					else if (m_axisDef.Margin != null)
					{
						m_margin = new ReportEnumProperty<ChartAutoBool>(m_axisDef.Margin.IsExpression, m_axisDef.Margin.OriginalText, (!m_axisDef.Margin.IsExpression) ? EnumTranslator.TranslateChartAutoBool(m_axisDef.Margin.StringValue, null) : ChartAutoBool.Auto);
					}
				}
				return m_margin;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && !m_chart.IsOldSnapshot && m_axisDef.Interval != null)
				{
					m_interval = new ReportDoubleProperty(m_axisDef.Interval);
				}
				return m_interval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (m_intervalType == null && !m_chart.IsOldSnapshot && m_axisDef.IntervalType != null)
				{
					m_intervalType = new ReportEnumProperty<ChartIntervalType>(m_axisDef.IntervalType.IsExpression, m_axisDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_axisDef.IntervalType.StringValue, null));
				}
				return m_intervalType;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && !m_chart.IsOldSnapshot && m_axisDef.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(m_axisDef.IntervalOffset);
				}
				return m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (m_intervalOffsetType == null && !m_chart.IsOldSnapshot && m_axisDef.IntervalOffsetType != null)
				{
					m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(m_axisDef.IntervalOffsetType.IsExpression, m_axisDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_axisDef.IntervalOffsetType.StringValue, null));
				}
				return m_intervalOffsetType;
			}
		}

		public ReportDoubleProperty LabelInterval
		{
			get
			{
				if (m_labelInterval == null && !m_chart.IsOldSnapshot && m_axisDef.LabelInterval != null)
				{
					m_labelInterval = new ReportDoubleProperty(m_axisDef.LabelInterval);
				}
				return m_labelInterval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> LabelIntervalType
		{
			get
			{
				if (m_labelIntervalType == null && !m_chart.IsOldSnapshot && m_axisDef.LabelIntervalType != null)
				{
					m_labelIntervalType = new ReportEnumProperty<ChartIntervalType>(m_axisDef.LabelIntervalType.IsExpression, m_axisDef.LabelIntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_axisDef.LabelIntervalType.StringValue, null));
				}
				return m_labelIntervalType;
			}
		}

		public ReportDoubleProperty LabelIntervalOffset
		{
			get
			{
				if (m_labelIntervalOffset == null && !m_chart.IsOldSnapshot && m_axisDef.LabelIntervalOffset != null)
				{
					m_labelIntervalOffset = new ReportDoubleProperty(m_axisDef.LabelIntervalOffset);
				}
				return m_labelIntervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> LabelIntervalOffsetType
		{
			get
			{
				if (m_labelIntervalOffsetType == null && !m_chart.IsOldSnapshot && m_axisDef.LabelIntervalOffsetType != null)
				{
					m_labelIntervalOffsetType = new ReportEnumProperty<ChartIntervalType>(m_axisDef.LabelIntervalOffsetType.IsExpression, m_axisDef.LabelIntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_axisDef.LabelIntervalOffsetType.StringValue, null));
				}
				return m_labelIntervalOffsetType;
			}
		}

		public ReportBoolProperty VariableAutoInterval
		{
			get
			{
				if (m_variableAutoInterval == null && !m_chart.IsOldSnapshot && m_axisDef.VariableAutoInterval != null)
				{
					m_variableAutoInterval = new ReportBoolProperty(m_axisDef.VariableAutoInterval);
				}
				return m_variableAutoInterval;
			}
		}

		public ChartTickMarks MajorTickMarks
		{
			get
			{
				if (m_majorTickMarks == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_majorTickMarks = new ChartTickMarks(m_renderAxisDef.MajorTickMarks, m_chart);
					}
					else if (m_axisDef.MajorTickMarks != null)
					{
						m_majorTickMarks = new ChartTickMarks(m_axisDef.MajorTickMarks, m_chart);
					}
				}
				return m_majorTickMarks;
			}
		}

		public ChartTickMarks MinorTickMarks
		{
			get
			{
				if (m_minorTickMarks == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_minorTickMarks = new ChartTickMarks(m_renderAxisDef.MinorTickMarks, m_chart);
					}
					else if (m_axisDef.MinorTickMarks != null)
					{
						m_minorTickMarks = new ChartTickMarks(m_axisDef.MinorTickMarks, m_chart);
					}
				}
				return m_minorTickMarks;
			}
		}

		public ReportBoolProperty MarksAlwaysAtPlotEdge
		{
			get
			{
				if (m_marksAlwaysAtPlotEdge == null && !m_chart.IsOldSnapshot && m_axisDef.MarksAlwaysAtPlotEdge != null)
				{
					m_marksAlwaysAtPlotEdge = new ReportBoolProperty(m_axisDef.MarksAlwaysAtPlotEdge);
				}
				return m_marksAlwaysAtPlotEdge;
			}
		}

		public ReportBoolProperty Reverse
		{
			get
			{
				if (m_reverse == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						return new ReportBoolProperty(new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo
						{
							BoolValue = m_renderAxisDef.Reverse
						});
					}
					if (m_axisDef.Reverse != null)
					{
						m_reverse = new ReportBoolProperty(m_axisDef.Reverse);
					}
				}
				return m_reverse;
			}
		}

		public ReportEnumProperty<ChartAxisLocation> Location
		{
			get
			{
				if (m_location == null && !m_chart.IsOldSnapshot && m_axisDef.Location != null)
				{
					m_location = new ReportEnumProperty<ChartAxisLocation>(m_axisDef.Location.IsExpression, m_axisDef.Location.OriginalText, EnumTranslator.TranslateChartAxisLocation(m_axisDef.Location.StringValue, null));
				}
				return m_location;
			}
		}

		public ReportBoolProperty Interlaced
		{
			get
			{
				if (m_interlaced == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
						expressionInfo.BoolValue = m_renderAxisDef.Interlaced;
						m_interlaced = new ReportBoolProperty(expressionInfo);
					}
					else if (m_axisDef.Interlaced != null)
					{
						m_interlaced = new ReportBoolProperty(m_axisDef.Interlaced);
					}
				}
				return m_interlaced;
			}
		}

		public ReportColorProperty InterlacedColor
		{
			get
			{
				if (m_interlacedColor == null && !m_chart.IsOldSnapshot && m_axisDef.InterlacedColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo interlacedColor = m_axisDef.InterlacedColor;
					m_interlacedColor = new ReportColorProperty(interlacedColor.IsExpression, interlacedColor.OriginalText, interlacedColor.IsExpression ? null : new ReportColor(interlacedColor.StringValue.Trim(), allowTransparency: true), interlacedColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
				}
				return m_interlacedColor;
			}
		}

		public ReportBoolProperty LogScale
		{
			get
			{
				if (m_logScale == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
						expressionInfo.BoolValue = m_renderAxisDef.LogScale;
						m_logScale = new ReportBoolProperty(expressionInfo);
					}
					else if (m_axisDef.LogScale != null)
					{
						m_logScale = new ReportBoolProperty(m_axisDef.LogScale);
					}
				}
				return m_logScale;
			}
		}

		public ReportDoubleProperty LogBase
		{
			get
			{
				if (m_logBase == null && !m_chart.IsOldSnapshot && m_axisDef.LogBase != null)
				{
					m_logBase = new ReportDoubleProperty(m_axisDef.LogBase);
				}
				return m_logBase;
			}
		}

		public ReportBoolProperty HideLabels
		{
			get
			{
				if (m_hideLabels == null && !m_chart.IsOldSnapshot && m_axisDef.HideLabels != null)
				{
					m_hideLabels = new ReportBoolProperty(m_axisDef.HideLabels);
				}
				return m_hideLabels;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (m_angle == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						return null;
					}
					if (m_axisDef.Angle != null)
					{
						m_angle = new ReportDoubleProperty(m_axisDef.Angle);
					}
				}
				return m_angle;
			}
		}

		public ReportEnumProperty<ChartAxisArrow> Arrows
		{
			get
			{
				if (m_arrows == null && !m_chart.IsOldSnapshot && m_axisDef.Arrows != null)
				{
					m_arrows = new ReportEnumProperty<ChartAxisArrow>(m_axisDef.Arrows.IsExpression, m_axisDef.Arrows.OriginalText, EnumTranslator.TranslateChartAxisArrow(m_axisDef.Arrows.StringValue, null));
				}
				return m_arrows;
			}
		}

		public ReportBoolProperty PreventFontShrink
		{
			get
			{
				if (m_preventFontShrink == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_preventFontShrink = new ReportBoolProperty();
					}
					else if (m_axisDef.PreventFontShrink != null)
					{
						m_preventFontShrink = new ReportBoolProperty(m_axisDef.PreventFontShrink);
					}
				}
				return m_preventFontShrink;
			}
		}

		public ReportBoolProperty PreventFontGrow
		{
			get
			{
				if (m_preventFontGrow == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_preventFontGrow = new ReportBoolProperty();
					}
					else if (m_axisDef.PreventFontGrow != null)
					{
						m_preventFontGrow = new ReportBoolProperty(m_axisDef.PreventFontGrow);
					}
				}
				return m_preventFontGrow;
			}
		}

		public ReportBoolProperty PreventLabelOffset
		{
			get
			{
				if (m_preventLabelOffset == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_preventLabelOffset = new ReportBoolProperty();
					}
					else if (m_axisDef.PreventLabelOffset != null)
					{
						m_preventLabelOffset = new ReportBoolProperty(m_axisDef.PreventLabelOffset);
					}
				}
				return m_preventLabelOffset;
			}
		}

		public ReportBoolProperty PreventWordWrap
		{
			get
			{
				if (m_preventWordWrap == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_preventWordWrap = new ReportBoolProperty();
					}
					else if (m_axisDef.PreventWordWrap != null)
					{
						m_preventWordWrap = new ReportBoolProperty(m_axisDef.PreventWordWrap);
					}
				}
				return m_preventWordWrap;
			}
		}

		public ReportEnumProperty<ChartAxisLabelRotation> AllowLabelRotation
		{
			get
			{
				if (m_allowLabelRotation == null && !m_chart.IsOldSnapshot && m_axisDef.AllowLabelRotation != null)
				{
					m_allowLabelRotation = new ReportEnumProperty<ChartAxisLabelRotation>(m_axisDef.AllowLabelRotation.IsExpression, m_axisDef.AllowLabelRotation.OriginalText, EnumTranslator.TranslateChartAxisLabelRotation(m_axisDef.AllowLabelRotation.StringValue, null));
				}
				return m_allowLabelRotation;
			}
		}

		public ReportBoolProperty IncludeZero
		{
			get
			{
				if (m_includeZero == null && !m_chart.IsOldSnapshot && m_axisDef.IncludeZero != null)
				{
					m_includeZero = new ReportBoolProperty(m_axisDef.IncludeZero);
				}
				return m_includeZero;
			}
		}

		public ReportBoolProperty LabelsAutoFitDisabled
		{
			get
			{
				if (m_labelsAutoFitDisabled == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_labelsAutoFitDisabled = new ReportBoolProperty();
					}
					else if (m_axisDef.LabelsAutoFitDisabled != null)
					{
						m_labelsAutoFitDisabled = new ReportBoolProperty(m_axisDef.LabelsAutoFitDisabled);
					}
				}
				return m_labelsAutoFitDisabled;
			}
		}

		public ReportSizeProperty MinFontSize
		{
			get
			{
				if (m_minFontSize == null && !m_chart.IsOldSnapshot && m_axisDef.MinFontSize != null)
				{
					m_minFontSize = new ReportSizeProperty(m_axisDef.MinFontSize);
				}
				return m_minFontSize;
			}
		}

		public ReportSizeProperty MaxFontSize
		{
			get
			{
				if (m_maxFontSize == null && !m_chart.IsOldSnapshot && m_axisDef.MaxFontSize != null)
				{
					m_maxFontSize = new ReportSizeProperty(m_axisDef.MaxFontSize);
				}
				return m_maxFontSize;
			}
		}

		public ReportBoolProperty OffsetLabels
		{
			get
			{
				if (m_offsetLabels == null && !m_chart.IsOldSnapshot && m_axisDef.OffsetLabels != null)
				{
					m_offsetLabels = new ReportBoolProperty(m_axisDef.OffsetLabels);
				}
				return m_offsetLabels;
			}
		}

		public ReportBoolProperty HideEndLabels
		{
			get
			{
				if (m_hideEndLabels == null && !m_chart.IsOldSnapshot && m_axisDef.HideEndLabels != null)
				{
					m_hideEndLabels = new ReportBoolProperty(m_axisDef.HideEndLabels);
				}
				return m_hideEndLabels;
			}
		}

		public ChartAxisScaleBreak AxisScaleBreak
		{
			get
			{
				if (m_axisScaleBreak == null && !m_chart.IsOldSnapshot && m_axisDef.AxisScaleBreak != null)
				{
					m_axisScaleBreak = new ChartAxisScaleBreak(m_axisDef.AxisScaleBreak, m_chart);
				}
				return m_axisScaleBreak;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis AxisDef => m_axisDef;

		internal AxisInstance RenderAxisInstance => m_renderAxisInstance;

		public ChartAxisInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartAxisInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartAxis(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis axisDef, Chart chart)
		{
			m_axisDef = axisDef;
			m_chart = chart;
		}

		internal ChartAxis(Axis renderAxisDef, AxisInstance renderAxisInstance, Chart chart, bool isCategory)
		{
			m_renderAxisDef = renderAxisDef;
			m_renderAxisInstance = renderAxisInstance;
			m_chart = chart;
			m_isCategory = isCategory;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_majorGridlines != null)
			{
				m_majorGridlines.SetNewContext();
			}
			if (m_minorGridlines != null)
			{
				m_minorGridlines.SetNewContext();
			}
			if (m_title != null)
			{
				m_title.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_chartStripLines != null)
			{
				m_chartStripLines.SetNewContext();
			}
			if (m_majorTickMarks != null)
			{
				m_majorTickMarks.SetNewContext();
			}
			if (m_minorTickMarks != null)
			{
				m_minorTickMarks.SetNewContext();
			}
			if (m_axisScaleBreak != null)
			{
				m_axisScaleBreak.SetNewContext();
			}
			m_customPropertiesReady = false;
		}
	}
}
