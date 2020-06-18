using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ChartAxis : ChartStyleContainer, IPersistable, ICustomPropertiesHolder
	{
		internal enum Mode
		{
			CategoryAxis,
			ValueAxis
		}

		internal enum ExpressionType
		{
			Min,
			Max,
			CrossAt
		}

		protected string m_name;

		private ChartAxisTitle m_title;

		private ChartGridLines m_majorGridLines;

		private ChartGridLines m_minorGridLines;

		private DataValueList m_customProperties;

		private List<ChartStripLine> m_chartStripLines;

		private ExpressionInfo m_visible;

		private ExpressionInfo m_margin;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		private ChartTickMarks m_majorTickMarks;

		private ChartTickMarks m_minorTickMarks;

		private ExpressionInfo m_marksAlwaysAtPlotEdge;

		private ExpressionInfo m_reverse;

		private ExpressionInfo m_location;

		private ExpressionInfo m_interlaced;

		private ExpressionInfo m_interlacedColor;

		private ExpressionInfo m_logScale;

		private ExpressionInfo m_logBase;

		private ExpressionInfo m_hideLabels;

		private ExpressionInfo m_angle;

		private ExpressionInfo m_arrows;

		private ExpressionInfo m_preventFontShrink;

		private ExpressionInfo m_preventFontGrow;

		private ExpressionInfo m_preventLabelOffset;

		private ExpressionInfo m_preventWordWrap;

		private ExpressionInfo m_allowLabelRotation;

		private ExpressionInfo m_includeZero;

		private ExpressionInfo m_labelsAutoFitDisabled;

		private ExpressionInfo m_minFontSize;

		private ExpressionInfo m_maxFontSize;

		private ExpressionInfo m_offsetLabels;

		private ExpressionInfo m_hideEndLabels;

		private ChartAxisScaleBreak m_axisScaleBreak;

		private ExpressionInfo m_crossAt;

		private ExpressionInfo m_min;

		private ExpressionInfo m_max;

		private bool m_scalar;

		private bool m_autoCrossAt = true;

		private bool m_autoScaleMin = true;

		private bool m_autoScaleMax = true;

		private int m_exprHostID;

		private ExpressionInfo m_variableAutoInterval;

		private ExpressionInfo m_labelInterval;

		private ExpressionInfo m_labelIntervalType;

		private ExpressionInfo m_labelIntervalOffset;

		private ExpressionInfo m_labelIntervalOffsetType;

		[NonSerialized]
		private ChartAxisExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string AxisName
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ChartAxisTitle Title
		{
			get
			{
				return m_title;
			}
			set
			{
				m_title = value;
			}
		}

		internal ChartGridLines MajorGridLines
		{
			get
			{
				return m_majorGridLines;
			}
			set
			{
				m_majorGridLines = value;
			}
		}

		internal ChartGridLines MinorGridLines
		{
			get
			{
				return m_minorGridLines;
			}
			set
			{
				m_minorGridLines = value;
			}
		}

		internal List<ChartStripLine> StripLines
		{
			get
			{
				return m_chartStripLines;
			}
			set
			{
				m_chartStripLines = value;
			}
		}

		public DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
			}
		}

		internal bool Scalar
		{
			get
			{
				return m_scalar;
			}
			set
			{
				m_scalar = value;
			}
		}

		internal ExpressionInfo Minimum
		{
			get
			{
				return m_min;
			}
			set
			{
				m_min = value;
			}
		}

		internal ExpressionInfo Maximum
		{
			get
			{
				return m_max;
			}
			set
			{
				m_max = value;
			}
		}

		internal ExpressionInfo CrossAt
		{
			get
			{
				return m_crossAt;
			}
			set
			{
				m_crossAt = value;
			}
		}

		internal bool AutoCrossAt
		{
			get
			{
				return m_autoCrossAt;
			}
			set
			{
				m_autoCrossAt = value;
			}
		}

		internal bool AutoScaleMin
		{
			get
			{
				return m_autoScaleMin;
			}
			set
			{
				m_autoScaleMin = value;
			}
		}

		internal bool AutoScaleMax
		{
			get
			{
				return m_autoScaleMax;
			}
			set
			{
				m_autoScaleMax = value;
			}
		}

		internal ExpressionInfo Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				m_visible = value;
			}
		}

		internal ExpressionInfo Margin
		{
			get
			{
				return m_margin;
			}
			set
			{
				m_margin = value;
			}
		}

		internal ExpressionInfo Interval
		{
			get
			{
				return m_interval;
			}
			set
			{
				m_interval = value;
			}
		}

		internal ExpressionInfo IntervalType
		{
			get
			{
				return m_intervalType;
			}
			set
			{
				m_intervalType = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return m_intervalOffset;
			}
			set
			{
				m_intervalOffset = value;
			}
		}

		internal ExpressionInfo IntervalOffsetType
		{
			get
			{
				return m_intervalOffsetType;
			}
			set
			{
				m_intervalOffsetType = value;
			}
		}

		internal ChartTickMarks MajorTickMarks
		{
			get
			{
				return m_majorTickMarks;
			}
			set
			{
				m_majorTickMarks = value;
			}
		}

		internal ChartTickMarks MinorTickMarks
		{
			get
			{
				return m_minorTickMarks;
			}
			set
			{
				m_minorTickMarks = value;
			}
		}

		internal ExpressionInfo MarksAlwaysAtPlotEdge
		{
			get
			{
				return m_marksAlwaysAtPlotEdge;
			}
			set
			{
				m_marksAlwaysAtPlotEdge = value;
			}
		}

		internal ExpressionInfo Reverse
		{
			get
			{
				return m_reverse;
			}
			set
			{
				m_reverse = value;
			}
		}

		internal ExpressionInfo Location
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		internal ExpressionInfo Interlaced
		{
			get
			{
				return m_interlaced;
			}
			set
			{
				m_interlaced = value;
			}
		}

		internal ExpressionInfo InterlacedColor
		{
			get
			{
				return m_interlacedColor;
			}
			set
			{
				m_interlacedColor = value;
			}
		}

		internal ExpressionInfo LogScale
		{
			get
			{
				return m_logScale;
			}
			set
			{
				m_logScale = value;
			}
		}

		internal ExpressionInfo LogBase
		{
			get
			{
				return m_logBase;
			}
			set
			{
				m_logBase = value;
			}
		}

		internal ExpressionInfo HideLabels
		{
			get
			{
				return m_hideLabels;
			}
			set
			{
				m_hideLabels = value;
			}
		}

		internal ExpressionInfo Angle
		{
			get
			{
				return m_angle;
			}
			set
			{
				m_angle = value;
			}
		}

		internal ExpressionInfo Arrows
		{
			get
			{
				return m_arrows;
			}
			set
			{
				m_arrows = value;
			}
		}

		internal ExpressionInfo PreventFontShrink
		{
			get
			{
				return m_preventFontShrink;
			}
			set
			{
				m_preventFontShrink = value;
			}
		}

		internal ExpressionInfo PreventFontGrow
		{
			get
			{
				return m_preventFontGrow;
			}
			set
			{
				m_preventFontGrow = value;
			}
		}

		internal ExpressionInfo PreventLabelOffset
		{
			get
			{
				return m_preventLabelOffset;
			}
			set
			{
				m_preventLabelOffset = value;
			}
		}

		internal ExpressionInfo PreventWordWrap
		{
			get
			{
				return m_preventWordWrap;
			}
			set
			{
				m_preventWordWrap = value;
			}
		}

		internal ExpressionInfo AllowLabelRotation
		{
			get
			{
				return m_allowLabelRotation;
			}
			set
			{
				m_allowLabelRotation = value;
			}
		}

		internal ExpressionInfo IncludeZero
		{
			get
			{
				return m_includeZero;
			}
			set
			{
				m_includeZero = value;
			}
		}

		internal ExpressionInfo LabelsAutoFitDisabled
		{
			get
			{
				return m_labelsAutoFitDisabled;
			}
			set
			{
				m_labelsAutoFitDisabled = value;
			}
		}

		internal ExpressionInfo MinFontSize
		{
			get
			{
				return m_minFontSize;
			}
			set
			{
				m_minFontSize = value;
			}
		}

		internal ExpressionInfo MaxFontSize
		{
			get
			{
				return m_maxFontSize;
			}
			set
			{
				m_maxFontSize = value;
			}
		}

		internal ExpressionInfo OffsetLabels
		{
			get
			{
				return m_offsetLabels;
			}
			set
			{
				m_offsetLabels = value;
			}
		}

		internal ExpressionInfo HideEndLabels
		{
			get
			{
				return m_hideEndLabels;
			}
			set
			{
				m_hideEndLabels = value;
			}
		}

		internal ChartAxisScaleBreak AxisScaleBreak
		{
			get
			{
				return m_axisScaleBreak;
			}
			set
			{
				m_axisScaleBreak = value;
			}
		}

		internal ExpressionInfo VariableAutoInterval
		{
			get
			{
				return m_variableAutoInterval;
			}
			set
			{
				m_variableAutoInterval = value;
			}
		}

		internal ExpressionInfo LabelInterval
		{
			get
			{
				return m_labelInterval;
			}
			set
			{
				m_labelInterval = value;
			}
		}

		internal ExpressionInfo LabelIntervalType
		{
			get
			{
				return m_labelIntervalType;
			}
			set
			{
				m_labelIntervalType = value;
			}
		}

		internal ExpressionInfo LabelIntervalOffset
		{
			get
			{
				return m_labelIntervalOffset;
			}
			set
			{
				m_labelIntervalOffset = value;
			}
		}

		internal ExpressionInfo LabelIntervalOffsetType
		{
			get
			{
				return m_labelIntervalOffsetType;
			}
			set
			{
				m_labelIntervalOffsetType = value;
			}
		}

		internal ChartAxisExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal ChartAxis()
		{
		}

		internal ChartAxis(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartAxisExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_title != null && m_exprHost.TitleHost != null)
			{
				m_title.SetExprHost(m_exprHost.TitleHost, reportObjectModel);
			}
			if (m_majorGridLines != null && m_exprHost.MajorGridLinesHost != null)
			{
				m_majorGridLines.SetExprHost(m_exprHost.MajorGridLinesHost, reportObjectModel);
			}
			if (m_minorGridLines != null && m_exprHost.MinorGridLinesHost != null)
			{
				m_minorGridLines.SetExprHost(m_exprHost.MinorGridLinesHost, reportObjectModel);
			}
			if (m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(m_customProperties != null, "(null != m_customProperties)");
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			IList<ChartStripLineExprHost> chartStripLinesHostsRemotable = m_exprHost.ChartStripLinesHostsRemotable;
			if (m_chartStripLines != null && chartStripLinesHostsRemotable != null)
			{
				for (int i = 0; i < m_chartStripLines.Count; i++)
				{
					ChartStripLine chartStripLine = m_chartStripLines[i];
					if (chartStripLine != null && chartStripLine.ExpressionHostID > -1)
					{
						chartStripLine.SetExprHost(chartStripLinesHostsRemotable[chartStripLine.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_majorTickMarks != null && m_exprHost.MajorTickMarksHost != null)
			{
				m_majorTickMarks.SetExprHost(m_exprHost.MajorTickMarksHost, reportObjectModel);
			}
			if (m_minorTickMarks != null && m_exprHost.MinorTickMarksHost != null)
			{
				m_minorTickMarks.SetExprHost(m_exprHost.MinorTickMarksHost, reportObjectModel);
			}
			if (m_axisScaleBreak != null && m_exprHost.AxisScaleBreakHost != null)
			{
				m_axisScaleBreak.SetExprHost(m_exprHost.AxisScaleBreakHost, reportObjectModel);
			}
		}

		internal virtual void Initialize(InitializationContext context, bool isValueAxis)
		{
			string propertyName = GetPropertyName(isValueAxis);
			context.ExprHostBuilder.ChartAxisStart(m_name, isValueAxis);
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			if (m_title != null)
			{
				m_title.Initialize(context);
			}
			if (m_minorGridLines != null)
			{
				m_minorGridLines.Initialize(context, isMajor: false);
			}
			if (m_majorGridLines != null)
			{
				m_majorGridLines.Initialize(context, isMajor: true);
			}
			if (m_min != null)
			{
				if (m_min.InitializeAxisExpression(propertyName + ".Minimum", context))
				{
					context.ExprHostBuilder.AxisMin(m_min);
				}
				else
				{
					m_min = null;
				}
			}
			if (m_max != null)
			{
				if (m_max.InitializeAxisExpression(propertyName + ".Maximum", context))
				{
					context.ExprHostBuilder.AxisMax(m_max);
				}
				else
				{
					m_max = null;
				}
			}
			if (m_crossAt != null)
			{
				if (m_crossAt.InitializeAxisExpression(propertyName + ".CrossAt", context))
				{
					context.ExprHostBuilder.AxisCrossAt(m_crossAt);
				}
				else
				{
					m_crossAt = null;
				}
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(propertyName + ".", context);
			}
			if (m_chartStripLines != null)
			{
				for (int i = 0; i < m_chartStripLines.Count; i++)
				{
					m_chartStripLines[i].Initialize(context, i);
				}
			}
			if (m_visible != null)
			{
				m_visible.Initialize("Visible", context);
				context.ExprHostBuilder.ChartAxisVisible(m_visible);
			}
			if (m_margin != null)
			{
				m_margin.Initialize("Margin", context);
				context.ExprHostBuilder.ChartAxisMargin(m_margin);
			}
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartAxisInterval(m_interval);
			}
			if (m_intervalType != null)
			{
				m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartAxisIntervalType(m_intervalType);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartAxisIntervalOffset(m_intervalOffset);
			}
			if (m_intervalOffsetType != null)
			{
				m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartAxisIntervalOffsetType(m_intervalOffsetType);
			}
			if (m_majorTickMarks != null)
			{
				m_majorTickMarks.Initialize(context, isMajor: true);
			}
			if (m_minorTickMarks != null)
			{
				m_minorTickMarks.Initialize(context, isMajor: false);
			}
			if (m_marksAlwaysAtPlotEdge != null)
			{
				m_marksAlwaysAtPlotEdge.Initialize("MarksAlwaysAtPlotEdge", context);
				context.ExprHostBuilder.ChartAxisMarksAlwaysAtPlotEdge(m_marksAlwaysAtPlotEdge);
			}
			if (m_reverse != null)
			{
				m_reverse.Initialize("Reverse", context);
				context.ExprHostBuilder.ChartAxisReverse(m_reverse);
			}
			if (m_location != null)
			{
				m_location.Initialize("Location", context);
				context.ExprHostBuilder.ChartAxisLocation(m_location);
			}
			if (m_interlaced != null)
			{
				m_interlaced.Initialize("Interlaced", context);
				context.ExprHostBuilder.ChartAxisInterlaced(m_interlaced);
			}
			if (m_interlacedColor != null)
			{
				m_interlacedColor.Initialize("InterlacedColor", context);
				context.ExprHostBuilder.ChartAxisInterlacedColor(m_interlacedColor);
			}
			if (m_logScale != null)
			{
				m_logScale.Initialize("LogScale", context);
				context.ExprHostBuilder.ChartAxisLogScale(m_logScale);
			}
			if (m_logBase != null)
			{
				m_logBase.Initialize("LogBase", context);
				context.ExprHostBuilder.ChartAxisLogBase(m_logBase);
			}
			if (m_hideLabels != null)
			{
				m_hideLabels.Initialize("HideLabels", context);
				context.ExprHostBuilder.ChartAxisHideLabels(m_hideLabels);
			}
			if (m_angle != null)
			{
				m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.ChartAxisAngle(m_angle);
			}
			if (m_arrows != null)
			{
				m_arrows.Initialize("Arrows", context);
				context.ExprHostBuilder.ChartAxisArrows(m_arrows);
			}
			if (m_preventFontShrink != null)
			{
				m_preventFontShrink.Initialize("PreventFontShrink", context);
				context.ExprHostBuilder.ChartAxisPreventFontShrink(m_preventFontShrink);
			}
			if (m_preventFontGrow != null)
			{
				m_preventFontGrow.Initialize("PreventFontGrow", context);
				context.ExprHostBuilder.ChartAxisPreventFontGrow(m_preventFontGrow);
			}
			if (m_preventLabelOffset != null)
			{
				m_preventLabelOffset.Initialize("PreventLabelOffset", context);
				context.ExprHostBuilder.ChartAxisPreventLabelOffset(m_preventLabelOffset);
			}
			if (m_preventWordWrap != null)
			{
				m_preventWordWrap.Initialize("PreventWordWrap", context);
				context.ExprHostBuilder.ChartAxisPreventWordWrap(m_preventWordWrap);
			}
			if (m_allowLabelRotation != null)
			{
				m_allowLabelRotation.Initialize("AllowLabelRotation", context);
				context.ExprHostBuilder.ChartAxisAllowLabelRotation(m_allowLabelRotation);
			}
			if (m_includeZero != null)
			{
				m_includeZero.Initialize("IncludeZero", context);
				context.ExprHostBuilder.ChartAxisIncludeZero(m_includeZero);
			}
			if (m_labelsAutoFitDisabled != null)
			{
				m_labelsAutoFitDisabled.Initialize("LabelsAutoFitDisabled", context);
				context.ExprHostBuilder.ChartAxisLabelsAutoFitDisabled(m_labelsAutoFitDisabled);
			}
			if (m_minFontSize != null)
			{
				m_minFontSize.Initialize("MinFontSize", context);
				context.ExprHostBuilder.ChartAxisMinFontSize(m_minFontSize);
			}
			if (m_maxFontSize != null)
			{
				m_maxFontSize.Initialize("MaxFontSize", context);
				context.ExprHostBuilder.ChartAxisMaxFontSize(m_maxFontSize);
			}
			if (m_offsetLabels != null)
			{
				m_offsetLabels.Initialize("OffsetLabels", context);
				context.ExprHostBuilder.ChartAxisOffsetLabels(m_offsetLabels);
			}
			if (m_hideEndLabels != null)
			{
				m_hideEndLabels.Initialize("HideEndLabels", context);
				context.ExprHostBuilder.ChartAxisHideEndLabels(m_hideEndLabels);
			}
			if (m_axisScaleBreak != null)
			{
				m_axisScaleBreak.Initialize(context);
			}
			if (m_variableAutoInterval != null)
			{
				m_variableAutoInterval.Initialize("VariableAutoInterval", context);
				context.ExprHostBuilder.ChartAxisVariableAutoInterval(m_variableAutoInterval);
			}
			if (m_labelInterval != null)
			{
				m_labelInterval.Initialize("LabelInterval", context);
				context.ExprHostBuilder.ChartAxisLabelInterval(m_labelInterval);
			}
			if (m_labelIntervalType != null)
			{
				m_labelIntervalType.Initialize("LabelIntervalType", context);
				context.ExprHostBuilder.ChartAxisLabelIntervalType(m_labelIntervalType);
			}
			if (m_labelIntervalOffset != null)
			{
				m_labelIntervalOffset.Initialize("LabelIntervalOffset", context);
				context.ExprHostBuilder.ChartAxisLabelIntervalOffset(m_labelIntervalOffset);
			}
			if (m_labelIntervalOffsetType != null)
			{
				m_labelIntervalOffsetType.Initialize("LabelIntervalOffsetType", context);
				context.ExprHostBuilder.ChartAxisLabelIntervalOffsetType(m_labelIntervalOffsetType);
			}
			m_exprHostID = context.ExprHostBuilder.ChartAxisEnd(isValueAxis);
		}

		private string GetPropertyName(bool isValueAxis)
		{
			if (!isValueAxis)
			{
				if (m_name == null)
				{
					m_name = "CategoryAxis";
					return m_name;
				}
				return "CategoryAxis_" + m_name;
			}
			if (m_name == null)
			{
				m_name = "ValueAxis";
				return m_name;
			}
			return "ValueAxis_" + m_name;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAxis chartAxis = (ChartAxis)base.PublishClone(context);
			if (m_title != null)
			{
				chartAxis.m_title = (ChartAxisTitle)m_title.PublishClone(context);
			}
			if (m_majorGridLines != null)
			{
				chartAxis.m_majorGridLines = (ChartGridLines)m_majorGridLines.PublishClone(context);
			}
			if (m_minorGridLines != null)
			{
				chartAxis.m_minorGridLines = (ChartGridLines)m_minorGridLines.PublishClone(context);
			}
			if (m_crossAt != null)
			{
				chartAxis.m_crossAt = (ExpressionInfo)m_crossAt.PublishClone(context);
			}
			if (m_min != null)
			{
				chartAxis.m_min = (ExpressionInfo)m_min.PublishClone(context);
			}
			if (m_max != null)
			{
				chartAxis.m_max = (ExpressionInfo)m_max.PublishClone(context);
			}
			if (m_customProperties != null)
			{
				chartAxis.m_customProperties = new DataValueList(m_customProperties.Count);
				foreach (DataValue customProperty in m_customProperties)
				{
					chartAxis.m_customProperties.Add(customProperty.PublishClone(context));
				}
			}
			if (m_chartStripLines != null)
			{
				chartAxis.m_chartStripLines = new List<ChartStripLine>(m_chartStripLines.Count);
				foreach (ChartStripLine chartStripLine in m_chartStripLines)
				{
					chartAxis.m_chartStripLines.Add((ChartStripLine)chartStripLine.PublishClone(context));
				}
			}
			if (m_visible != null)
			{
				chartAxis.m_visible = (ExpressionInfo)m_visible.PublishClone(context);
			}
			if (m_margin != null)
			{
				chartAxis.m_margin = (ExpressionInfo)m_margin.PublishClone(context);
			}
			if (m_interval != null)
			{
				chartAxis.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalType != null)
			{
				chartAxis.m_intervalType = (ExpressionInfo)m_intervalType.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				chartAxis.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			if (m_intervalOffsetType != null)
			{
				chartAxis.m_intervalOffsetType = (ExpressionInfo)m_intervalOffsetType.PublishClone(context);
			}
			if (m_majorTickMarks != null)
			{
				chartAxis.m_majorTickMarks = (ChartTickMarks)m_majorTickMarks.PublishClone(context);
			}
			if (m_minorTickMarks != null)
			{
				chartAxis.m_minorTickMarks = (ChartTickMarks)m_minorTickMarks.PublishClone(context);
			}
			if (m_marksAlwaysAtPlotEdge != null)
			{
				chartAxis.m_marksAlwaysAtPlotEdge = (ExpressionInfo)m_marksAlwaysAtPlotEdge.PublishClone(context);
			}
			if (m_reverse != null)
			{
				chartAxis.m_reverse = (ExpressionInfo)m_reverse.PublishClone(context);
			}
			if (m_location != null)
			{
				chartAxis.m_location = (ExpressionInfo)m_location.PublishClone(context);
			}
			if (m_interlaced != null)
			{
				chartAxis.m_interlaced = (ExpressionInfo)m_interlaced.PublishClone(context);
			}
			if (m_interlacedColor != null)
			{
				chartAxis.m_interlacedColor = (ExpressionInfo)m_interlacedColor.PublishClone(context);
			}
			if (m_logScale != null)
			{
				chartAxis.m_logScale = (ExpressionInfo)m_logScale.PublishClone(context);
			}
			if (m_logBase != null)
			{
				chartAxis.m_logBase = (ExpressionInfo)m_logBase.PublishClone(context);
			}
			if (m_hideLabels != null)
			{
				chartAxis.m_hideLabels = (ExpressionInfo)m_hideLabels.PublishClone(context);
			}
			if (m_angle != null)
			{
				chartAxis.m_angle = (ExpressionInfo)m_angle.PublishClone(context);
			}
			if (m_arrows != null)
			{
				chartAxis.m_arrows = (ExpressionInfo)m_arrows.PublishClone(context);
			}
			if (m_preventFontShrink != null)
			{
				chartAxis.m_preventFontShrink = (ExpressionInfo)m_preventFontShrink.PublishClone(context);
			}
			if (m_preventFontGrow != null)
			{
				chartAxis.m_preventFontGrow = (ExpressionInfo)m_preventFontGrow.PublishClone(context);
			}
			if (m_preventLabelOffset != null)
			{
				chartAxis.m_preventLabelOffset = (ExpressionInfo)m_preventLabelOffset.PublishClone(context);
			}
			if (m_preventWordWrap != null)
			{
				chartAxis.m_preventWordWrap = (ExpressionInfo)m_preventWordWrap.PublishClone(context);
			}
			if (m_allowLabelRotation != null)
			{
				chartAxis.m_allowLabelRotation = (ExpressionInfo)m_allowLabelRotation.PublishClone(context);
			}
			if (m_includeZero != null)
			{
				chartAxis.m_includeZero = (ExpressionInfo)m_includeZero.PublishClone(context);
			}
			if (m_labelsAutoFitDisabled != null)
			{
				chartAxis.m_labelsAutoFitDisabled = (ExpressionInfo)m_labelsAutoFitDisabled.PublishClone(context);
			}
			if (m_minFontSize != null)
			{
				chartAxis.m_minFontSize = (ExpressionInfo)m_minFontSize.PublishClone(context);
			}
			if (m_maxFontSize != null)
			{
				chartAxis.m_maxFontSize = (ExpressionInfo)m_maxFontSize.PublishClone(context);
			}
			if (m_offsetLabels != null)
			{
				chartAxis.m_offsetLabels = (ExpressionInfo)m_offsetLabels.PublishClone(context);
			}
			if (m_hideEndLabels != null)
			{
				chartAxis.m_hideEndLabels = (ExpressionInfo)m_hideEndLabels.PublishClone(context);
			}
			if (m_axisScaleBreak != null)
			{
				chartAxis.m_axisScaleBreak = (ChartAxisScaleBreak)m_axisScaleBreak.PublishClone(context);
			}
			if (m_variableAutoInterval != null)
			{
				chartAxis.m_variableAutoInterval = (ExpressionInfo)m_variableAutoInterval.PublishClone(context);
			}
			if (m_labelInterval != null)
			{
				chartAxis.m_labelInterval = (ExpressionInfo)m_hideEndLabels.PublishClone(context);
			}
			if (m_labelIntervalType != null)
			{
				chartAxis.m_labelIntervalType = (ExpressionInfo)m_hideEndLabels.PublishClone(context);
			}
			if (m_labelIntervalOffset != null)
			{
				chartAxis.m_labelIntervalOffset = (ExpressionInfo)m_labelIntervalOffset.PublishClone(context);
			}
			if (m_labelIntervalOffsetType != null)
			{
				chartAxis.m_labelIntervalOffsetType = (ExpressionInfo)m_labelIntervalOffsetType.PublishClone(context);
			}
			return chartAxis;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.AxisTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisTitle));
			list.Add(new MemberInfo(MemberName.MajorGridLines, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines));
			list.Add(new MemberInfo(MemberName.MinorGridLines, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines));
			list.Add(new MemberInfo(MemberName.CrossAt, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Scalar, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Minimum, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Maximum, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartStripLines, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStripLine));
			list.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.Visible, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Margin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MajorTickMarks, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks));
			list.Add(new MemberInfo(MemberName.MinorTickMarks, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks));
			list.Add(new MemberInfo(MemberName.MarksAlwaysAtPlotEdge, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reverse, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Location, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interlaced, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LogScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LogBase, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Arrows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventFontShrink, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventFontGrow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventLabelOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventWordWrap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowLabelRotation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IncludeZero, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelsAutoFitDisabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinFontSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxFontSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideEndLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AxisScaleBreak, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisScaleBreak));
			list.Add(new MemberInfo(MemberName.AutoCrossAt, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AutoScaleMin, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AutoScaleMax, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.VariableAutoInterval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelInterval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelIntervalType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelIntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelIntervalOffsetType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.AxisTitle:
					writer.Write(m_title);
					break;
				case MemberName.MajorGridLines:
					writer.Write(m_majorGridLines);
					break;
				case MemberName.MinorGridLines:
					writer.Write(m_minorGridLines);
					break;
				case MemberName.CrossAt:
					writer.Write(m_crossAt);
					break;
				case MemberName.AutoCrossAt:
					writer.Write(m_autoCrossAt);
					break;
				case MemberName.Scalar:
					writer.Write(m_scalar);
					break;
				case MemberName.Minimum:
					writer.Write(m_min);
					break;
				case MemberName.Maximum:
					writer.Write(m_max);
					break;
				case MemberName.AutoScaleMin:
					writer.Write(m_autoScaleMin);
					break;
				case MemberName.AutoScaleMax:
					writer.Write(m_autoScaleMax);
					break;
				case MemberName.CustomProperties:
					writer.Write(m_customProperties);
					break;
				case MemberName.ChartStripLines:
					writer.Write(m_chartStripLines);
					break;
				case MemberName.Visible:
					writer.Write(m_visible);
					break;
				case MemberName.Margin:
					writer.Write(m_margin);
					break;
				case MemberName.Interval:
					writer.Write(m_interval);
					break;
				case MemberName.IntervalType:
					writer.Write(m_intervalType);
					break;
				case MemberName.IntervalOffset:
					writer.Write(m_intervalOffset);
					break;
				case MemberName.IntervalOffsetType:
					writer.Write(m_intervalOffsetType);
					break;
				case MemberName.MajorTickMarks:
					writer.Write(m_majorTickMarks);
					break;
				case MemberName.MinorTickMarks:
					writer.Write(m_minorTickMarks);
					break;
				case MemberName.MarksAlwaysAtPlotEdge:
					writer.Write(m_marksAlwaysAtPlotEdge);
					break;
				case MemberName.Reverse:
					writer.Write(m_reverse);
					break;
				case MemberName.Location:
					writer.Write(m_location);
					break;
				case MemberName.Interlaced:
					writer.Write(m_interlaced);
					break;
				case MemberName.InterlacedColor:
					writer.Write(m_interlacedColor);
					break;
				case MemberName.LogScale:
					writer.Write(m_logScale);
					break;
				case MemberName.LogBase:
					writer.Write(m_logBase);
					break;
				case MemberName.HideLabels:
					writer.Write(m_hideLabels);
					break;
				case MemberName.Angle:
					writer.Write(m_angle);
					break;
				case MemberName.Arrows:
					writer.Write(m_arrows);
					break;
				case MemberName.PreventFontShrink:
					writer.Write(m_preventFontShrink);
					break;
				case MemberName.PreventFontGrow:
					writer.Write(m_preventFontGrow);
					break;
				case MemberName.PreventLabelOffset:
					writer.Write(m_preventLabelOffset);
					break;
				case MemberName.PreventWordWrap:
					writer.Write(m_preventWordWrap);
					break;
				case MemberName.AllowLabelRotation:
					writer.Write(m_allowLabelRotation);
					break;
				case MemberName.IncludeZero:
					writer.Write(m_includeZero);
					break;
				case MemberName.LabelsAutoFitDisabled:
					writer.Write(m_labelsAutoFitDisabled);
					break;
				case MemberName.MinFontSize:
					writer.Write(m_minFontSize);
					break;
				case MemberName.MaxFontSize:
					writer.Write(m_maxFontSize);
					break;
				case MemberName.OffsetLabels:
					writer.Write(m_offsetLabels);
					break;
				case MemberName.HideEndLabels:
					writer.Write(m_hideEndLabels);
					break;
				case MemberName.AxisScaleBreak:
					writer.Write(m_axisScaleBreak);
					break;
				case MemberName.VariableAutoInterval:
					writer.Write(m_variableAutoInterval);
					break;
				case MemberName.LabelInterval:
					writer.Write(m_labelInterval);
					break;
				case MemberName.LabelIntervalType:
					writer.Write(m_labelIntervalType);
					break;
				case MemberName.LabelIntervalOffset:
					writer.Write(m_labelIntervalOffset);
					break;
				case MemberName.LabelIntervalOffsetType:
					writer.Write(m_labelIntervalOffsetType);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.AxisTitle:
					m_title = (ChartAxisTitle)reader.ReadRIFObject();
					break;
				case MemberName.MajorGridLines:
					m_majorGridLines = (ChartGridLines)reader.ReadRIFObject();
					break;
				case MemberName.MinorGridLines:
					m_minorGridLines = (ChartGridLines)reader.ReadRIFObject();
					break;
				case MemberName.CrossAt:
					m_crossAt = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AutoCrossAt:
					m_autoCrossAt = reader.ReadBoolean();
					break;
				case MemberName.Scalar:
					m_scalar = reader.ReadBoolean();
					break;
				case MemberName.Minimum:
					m_min = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Maximum:
					m_max = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AutoScaleMin:
					m_autoScaleMin = reader.ReadBoolean();
					break;
				case MemberName.AutoScaleMax:
					m_autoScaleMax = reader.ReadBoolean();
					break;
				case MemberName.CustomProperties:
					m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.ChartStripLines:
					m_chartStripLines = reader.ReadGenericListOfRIFObjects<ChartStripLine>();
					break;
				case MemberName.Visible:
					m_visible = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Margin:
					m_margin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalType:
					m_intervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffsetType:
					m_intervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MajorTickMarks:
					m_majorTickMarks = (ChartTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.MinorTickMarks:
					m_minorTickMarks = (ChartTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.MarksAlwaysAtPlotEdge:
					m_marksAlwaysAtPlotEdge = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reverse:
					m_reverse = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Location:
					m_location = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interlaced:
					m_interlaced = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedColor:
					m_interlacedColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LogScale:
					m_logScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LogBase:
					m_logBase = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideLabels:
					m_hideLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Arrows:
					m_arrows = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventFontShrink:
					m_preventFontShrink = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventFontGrow:
					m_preventFontGrow = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventLabelOffset:
					m_preventLabelOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventWordWrap:
					m_preventWordWrap = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AllowLabelRotation:
					m_allowLabelRotation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IncludeZero:
					m_includeZero = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelsAutoFitDisabled:
					m_labelsAutoFitDisabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinFontSize:
					m_minFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxFontSize:
					m_maxFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetLabels:
					m_offsetLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideEndLabels:
					m_hideEndLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AxisScaleBreak:
					m_axisScaleBreak = (ChartAxisScaleBreak)reader.ReadRIFObject();
					break;
				case MemberName.VariableAutoInterval:
					m_variableAutoInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelInterval:
					m_labelInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelIntervalType:
					m_labelIntervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelIntervalOffset:
					m_labelIntervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelIntervalOffsetType:
					m_labelIntervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis;
		}

		internal object EvaluateCrossAt(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			if (m_crossAt == null)
			{
				return null;
			}
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAxisValueExpression(m_exprHost, m_crossAt, base.Name, "CrossAt", ExpressionType.CrossAt);
		}

		internal object EvaluateMin(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			if (m_min == null)
			{
				return null;
			}
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAxisValueExpression(m_exprHost, m_min, base.Name, "Minimum", ExpressionType.Min);
		}

		internal object EvaluateMax(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			if (m_max == null)
			{
				return null;
			}
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAxisValueExpression(m_exprHost, m_max, base.Name, "Maximum", ExpressionType.Max);
		}

		internal ChartAxisArrow EvaluateArrows(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAxisArrow(context.ReportRuntime.EvaluateChartAxisArrowsExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateVisible(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisVisibleExpression(this, m_chart.Name);
		}

		internal string EvaluateMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMarginExpression(this, m_chart.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisIntervalExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisIntervalTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisIntervalOffsetExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisIntervalOffsetTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateMarksAlwaysAtPlotEdge(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMarksAlwaysAtPlotEdgeExpression(this, m_chart.Name);
		}

		internal bool EvaluateReverse(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisReverseExpression(this, m_chart.Name);
		}

		internal ChartAxisLocation EvaluateLocation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAxisLocation(context.ReportRuntime.EvaluateChartAxisLocationExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateInterlaced(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisInterlacedExpression(this, m_chart.Name);
		}

		internal string EvaluateInterlacedColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisInterlacedColorExpression(this, m_chart.Name);
		}

		internal bool EvaluateLogScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLogScaleExpression(this, m_chart.Name);
		}

		internal double EvaluateLogBase(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLogBaseExpression(this, m_chart.Name);
		}

		internal bool EvaluateHideLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisHideLabelsExpression(this, m_chart.Name);
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisAngleExpression(this, m_chart.Name);
		}

		internal bool EvaluatePreventFontShrink(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventFontShrinkExpression(this, m_chart.Name);
		}

		internal bool EvaluatePreventFontGrow(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventFontGrowExpression(this, m_chart.Name);
		}

		internal bool EvaluatePreventLabelOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventLabelOffsetExpression(this, m_chart.Name);
		}

		internal bool EvaluatePreventWordWrap(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventWordWrapExpression(this, m_chart.Name);
		}

		internal ChartAxisLabelRotation EvaluateAllowLabelRotation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAxisLabelRotation(context.ReportRuntime.EvaluateChartAxisAllowLabelRotationExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateIncludeZero(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisIncludeZeroExpression(this, m_chart.Name);
		}

		internal bool EvaluateLabelsAutoFitDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLabelsAutoFitDisabledExpression(this, m_chart.Name);
		}

		internal string EvaluateMinFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMinFontSizeExpression(this, m_chart.Name);
		}

		internal string EvaluateMaxFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMaxFontSizeExpression(this, m_chart.Name);
		}

		internal bool EvaluateOffsetLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisOffsetLabelsExpression(this, m_chart.Name);
		}

		internal bool EvaluateHideEndLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisHideEndLabelsExpression(this, m_chart.Name);
		}

		internal bool EvaluateVariableAutoInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisVariableAutoIntervalExpression(this, m_chart.Name);
		}

		internal double EvaluateLabelInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLabelIntervalExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateLabelIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisLabelIntervalTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateLabelIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLabelIntervalOffsetsExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateLabelIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisLabelIntervalOffsetTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}
	}
}
