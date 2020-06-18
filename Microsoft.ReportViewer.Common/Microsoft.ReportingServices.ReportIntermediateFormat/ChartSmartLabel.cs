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
	internal sealed class ChartSmartLabel : IPersistable
	{
		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_chartSeries;

		private ExpressionInfo m_allowOutSidePlotArea;

		private ExpressionInfo m_calloutBackColor;

		private ExpressionInfo m_calloutLineAnchor;

		private ExpressionInfo m_calloutLineColor;

		private ExpressionInfo m_calloutLineStyle;

		private ExpressionInfo m_calloutLineWidth;

		private ExpressionInfo m_calloutStyle;

		private ExpressionInfo m_showOverlapped;

		private ExpressionInfo m_markerOverlapping;

		private ExpressionInfo m_maxMovingDistance;

		private ExpressionInfo m_minMovingDistance;

		private ChartNoMoveDirections m_noMoveDirections;

		private ExpressionInfo m_disabled;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartSmartLabelExprHost m_exprHost;

		internal ChartSmartLabelExprHost ExprHost => m_exprHost;

		internal ExpressionInfo AllowOutSidePlotArea
		{
			get
			{
				return m_allowOutSidePlotArea;
			}
			set
			{
				m_allowOutSidePlotArea = value;
			}
		}

		internal ExpressionInfo CalloutBackColor
		{
			get
			{
				return m_calloutBackColor;
			}
			set
			{
				m_calloutBackColor = value;
			}
		}

		internal ExpressionInfo CalloutLineAnchor
		{
			get
			{
				return m_calloutLineAnchor;
			}
			set
			{
				m_calloutLineAnchor = value;
			}
		}

		internal ExpressionInfo CalloutLineColor
		{
			get
			{
				return m_calloutLineColor;
			}
			set
			{
				m_calloutLineColor = value;
			}
		}

		internal ExpressionInfo CalloutLineStyle
		{
			get
			{
				return m_calloutLineStyle;
			}
			set
			{
				m_calloutLineStyle = value;
			}
		}

		internal ExpressionInfo CalloutLineWidth
		{
			get
			{
				return m_calloutLineWidth;
			}
			set
			{
				m_calloutLineWidth = value;
			}
		}

		internal ExpressionInfo CalloutStyle
		{
			get
			{
				return m_calloutStyle;
			}
			set
			{
				m_calloutStyle = value;
			}
		}

		internal ExpressionInfo ShowOverlapped
		{
			get
			{
				return m_showOverlapped;
			}
			set
			{
				m_showOverlapped = value;
			}
		}

		internal ExpressionInfo MarkerOverlapping
		{
			get
			{
				return m_markerOverlapping;
			}
			set
			{
				m_markerOverlapping = value;
			}
		}

		internal ExpressionInfo MaxMovingDistance
		{
			get
			{
				return m_maxMovingDistance;
			}
			set
			{
				m_maxMovingDistance = value;
			}
		}

		internal ExpressionInfo MinMovingDistance
		{
			get
			{
				return m_minMovingDistance;
			}
			set
			{
				m_minMovingDistance = value;
			}
		}

		internal ChartNoMoveDirections NoMoveDirections
		{
			get
			{
				return m_noMoveDirections;
			}
			set
			{
				m_noMoveDirections = value;
			}
		}

		internal ExpressionInfo Disabled
		{
			get
			{
				return m_disabled;
			}
			set
			{
				m_disabled = value;
			}
		}

		private IInstancePath InstancePath
		{
			get
			{
				if (m_chartSeries != null)
				{
					return m_chartSeries;
				}
				return m_chart;
			}
		}

		internal ChartSmartLabel()
		{
		}

		internal ChartSmartLabel(Chart chart, ChartSeries chartSeries)
		{
			m_chart = chart;
			m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartSmartLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_noMoveDirections != null && m_exprHost.NoMoveDirectionsHost != null)
			{
				m_noMoveDirections.SetExprHost(m_exprHost.NoMoveDirectionsHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartSmartLabelStart();
			if (m_allowOutSidePlotArea != null)
			{
				m_allowOutSidePlotArea.Initialize("AllowOutSidePlotArea", context);
				context.ExprHostBuilder.ChartSmartLabelAllowOutSidePlotArea(m_allowOutSidePlotArea);
			}
			if (m_calloutBackColor != null)
			{
				m_calloutBackColor.Initialize("CalloutBackColor", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutBackColor(m_calloutBackColor);
			}
			if (m_calloutLineAnchor != null)
			{
				m_calloutLineAnchor.Initialize("CalloutLineAnchor", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineAnchor(m_calloutLineAnchor);
			}
			if (m_calloutLineColor != null)
			{
				m_calloutLineColor.Initialize("CalloutLineColor", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineColor(m_calloutLineColor);
			}
			if (m_calloutLineStyle != null)
			{
				m_calloutLineStyle.Initialize("CalloutLineStyle", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineStyle(m_calloutLineStyle);
			}
			if (m_calloutLineWidth != null)
			{
				m_calloutLineWidth.Initialize("CalloutLineWidth", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineWidth(m_calloutLineWidth);
			}
			if (m_calloutStyle != null)
			{
				m_calloutStyle.Initialize("CalloutStyle", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutStyle(m_calloutStyle);
			}
			if (m_showOverlapped != null)
			{
				m_showOverlapped.Initialize("ShowOverlapped", context);
				context.ExprHostBuilder.ChartSmartLabelShowOverlapped(m_showOverlapped);
			}
			if (m_markerOverlapping != null)
			{
				m_markerOverlapping.Initialize("MarkerOverlapping", context);
				context.ExprHostBuilder.ChartSmartLabelMarkerOverlapping(m_markerOverlapping);
			}
			if (m_maxMovingDistance != null)
			{
				m_maxMovingDistance.Initialize("MaxMovingDistance", context);
				context.ExprHostBuilder.ChartSmartLabelMaxMovingDistance(m_maxMovingDistance);
			}
			if (m_minMovingDistance != null)
			{
				m_minMovingDistance.Initialize("MinMovingDistance", context);
				context.ExprHostBuilder.ChartSmartLabelMinMovingDistance(m_minMovingDistance);
			}
			if (m_noMoveDirections != null)
			{
				m_noMoveDirections.Initialize(context);
			}
			if (m_disabled != null)
			{
				m_disabled.Initialize("Disabled", context);
				context.ExprHostBuilder.ChartSmartLabelDisabled(m_disabled);
			}
			context.ExprHostBuilder.ChartSmartLabelEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartSmartLabel chartSmartLabel = (ChartSmartLabel)MemberwiseClone();
			chartSmartLabel.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_allowOutSidePlotArea != null)
			{
				chartSmartLabel.m_allowOutSidePlotArea = (ExpressionInfo)m_allowOutSidePlotArea.PublishClone(context);
			}
			if (m_calloutBackColor != null)
			{
				chartSmartLabel.m_calloutBackColor = (ExpressionInfo)m_calloutBackColor.PublishClone(context);
			}
			if (m_calloutLineAnchor != null)
			{
				chartSmartLabel.m_calloutLineAnchor = (ExpressionInfo)m_calloutLineAnchor.PublishClone(context);
			}
			if (m_calloutLineColor != null)
			{
				chartSmartLabel.m_calloutLineColor = (ExpressionInfo)m_calloutLineColor.PublishClone(context);
			}
			if (m_calloutLineStyle != null)
			{
				chartSmartLabel.m_calloutLineStyle = (ExpressionInfo)m_calloutLineStyle.PublishClone(context);
			}
			if (m_calloutLineWidth != null)
			{
				chartSmartLabel.m_calloutLineWidth = (ExpressionInfo)m_calloutLineWidth.PublishClone(context);
			}
			if (m_calloutStyle != null)
			{
				chartSmartLabel.m_calloutStyle = (ExpressionInfo)m_calloutStyle.PublishClone(context);
			}
			if (m_showOverlapped != null)
			{
				chartSmartLabel.m_showOverlapped = (ExpressionInfo)m_showOverlapped.PublishClone(context);
			}
			if (m_markerOverlapping != null)
			{
				chartSmartLabel.m_markerOverlapping = (ExpressionInfo)m_markerOverlapping.PublishClone(context);
			}
			if (m_maxMovingDistance != null)
			{
				chartSmartLabel.m_maxMovingDistance = (ExpressionInfo)m_maxMovingDistance.PublishClone(context);
			}
			if (m_minMovingDistance != null)
			{
				chartSmartLabel.m_minMovingDistance = (ExpressionInfo)m_minMovingDistance.PublishClone(context);
			}
			if (m_noMoveDirections != null)
			{
				chartSmartLabel.m_noMoveDirections = (ChartNoMoveDirections)m_noMoveDirections.PublishClone(context);
			}
			if (m_disabled != null)
			{
				chartSmartLabel.m_disabled = (ExpressionInfo)m_disabled.PublishClone(context);
			}
			return chartSmartLabel;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.AllowOutSidePlotArea, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutBackColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineAnchor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowOverlapped, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MarkerOverlapping, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxMovingDistance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinMovingDistance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.NoMoveDirections, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoMoveDirections));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.Disabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSmartLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal ChartAllowOutsideChartArea EvaluateAllowOutSidePlotArea(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartAllowOutsideChartArea(context.ReportRuntime.EvaluateChartSmartLabelAllowOutSidePlotAreaExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateCalloutBackColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelCalloutBackColorExpression(this, m_chart.Name);
		}

		internal ChartCalloutLineAnchor EvaluateCalloutLineAnchor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartCalloutLineAnchor(context.ReportRuntime.EvaluateChartSmartLabelCalloutLineAnchorExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateCalloutLineColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelCalloutLineColorExpression(this, m_chart.Name);
		}

		internal ChartCalloutLineStyle EvaluateCalloutLineStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartCalloutLineStyle(context.ReportRuntime.EvaluateChartSmartLabelCalloutLineStyleExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateCalloutLineWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelCalloutLineWidthExpression(this, m_chart.Name);
		}

		internal ChartCalloutStyle EvaluateCalloutStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartCalloutStyle(context.ReportRuntime.EvaluateChartSmartLabelCalloutStyleExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateShowOverlapped(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelShowOverlappedExpression(this, m_chart.Name);
		}

		internal bool EvaluateMarkerOverlapping(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelMarkerOverlappingExpression(this, m_chart.Name);
		}

		internal string EvaluateMaxMovingDistance(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelMaxMovingDistanceExpression(this, m_chart.Name);
		}

		internal string EvaluateMinMovingDistance(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelMinMovingDistanceExpression(this, m_chart.Name);
		}

		internal bool EvaluateDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelDisabledExpression(this, m_chart.Name);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(m_chartSeries);
					break;
				case MemberName.AllowOutSidePlotArea:
					writer.Write(m_allowOutSidePlotArea);
					break;
				case MemberName.CalloutBackColor:
					writer.Write(m_calloutBackColor);
					break;
				case MemberName.CalloutLineAnchor:
					writer.Write(m_calloutLineAnchor);
					break;
				case MemberName.CalloutLineColor:
					writer.Write(m_calloutLineColor);
					break;
				case MemberName.CalloutLineStyle:
					writer.Write(m_calloutLineStyle);
					break;
				case MemberName.CalloutLineWidth:
					writer.Write(m_calloutLineWidth);
					break;
				case MemberName.CalloutStyle:
					writer.Write(m_calloutStyle);
					break;
				case MemberName.ShowOverlapped:
					writer.Write(m_showOverlapped);
					break;
				case MemberName.MarkerOverlapping:
					writer.Write(m_markerOverlapping);
					break;
				case MemberName.MaxMovingDistance:
					writer.Write(m_maxMovingDistance);
					break;
				case MemberName.MinMovingDistance:
					writer.Write(m_minMovingDistance);
					break;
				case MemberName.NoMoveDirections:
					writer.Write(m_noMoveDirections);
					break;
				case MemberName.Disabled:
					writer.Write(m_disabled);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.ChartSeries:
					m_chartSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.AllowOutSidePlotArea:
					m_allowOutSidePlotArea = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutBackColor:
					m_calloutBackColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineAnchor:
					m_calloutLineAnchor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineColor:
					m_calloutLineColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineStyle:
					m_calloutLineStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineWidth:
					m_calloutLineWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutStyle:
					m_calloutStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowOverlapped:
					m_showOverlapped = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MarkerOverlapping:
					m_markerOverlapping = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxMovingDistance:
					m_maxMovingDistance = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinMovingDistance:
					m_minMovingDistance = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.NoMoveDirections:
					m_noMoveDirections = (ChartNoMoveDirections)reader.ReadRIFObject();
					break;
				case MemberName.Disabled:
					m_disabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.Chart:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
					break;
				case MemberName.ChartSeries:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSmartLabel;
		}
	}
}
