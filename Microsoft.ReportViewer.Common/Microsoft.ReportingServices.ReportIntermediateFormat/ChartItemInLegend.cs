using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
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
	internal sealed class ChartItemInLegend : IPersistable, IActionOwner
	{
		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_chartSeries;

		[Reference]
		private ChartDataPoint m_chartDataPoint;

		private Action m_action;

		private ExpressionInfo m_legendText;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartDataPointInLegendExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		internal ChartDataPointInLegendExprHost ExprHost => m_exprHost;

		internal Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal ExpressionInfo LegendText
		{
			get
			{
				return m_legendText;
			}
			set
			{
				m_legendText = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		private IInstancePath InstancePath
		{
			get
			{
				if (m_chartDataPoint != null)
				{
					return m_chartDataPoint;
				}
				if (m_chartSeries != null)
				{
					return m_chartSeries;
				}
				return m_chart;
			}
		}

		internal ChartItemInLegend()
		{
		}

		internal ChartItemInLegend(Chart chart, ChartDataPoint chartDataPoint)
		{
			m_chart = chart;
			m_chartDataPoint = chartDataPoint;
		}

		internal ChartItemInLegend(Chart chart, ChartSeries chartSeries)
		{
			m_chart = chart;
			m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartDataPointInLegendExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartItemInLegendStart();
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_legendText != null)
			{
				m_legendText.Initialize("LegendText", context);
				context.ExprHostBuilder.ChartItemInLegendLegendText(m_legendText);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartItemInLegendToolTip(m_toolTip);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartItemInLegendHidden(m_hidden);
			}
			context.ExprHostBuilder.ChartItemInLegendEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartItemInLegend chartItemInLegend = (ChartItemInLegend)MemberwiseClone();
			chartItemInLegend.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_action != null)
			{
				chartItemInLegend.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_legendText != null)
			{
				chartItemInLegend.m_legendText = (ExpressionInfo)m_legendText.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartItemInLegend.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_hidden != null)
			{
				chartItemInLegend.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			return chartItemInLegend;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.LegendText, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal string EvaluateLegendText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartItemInLegendLegendTextExpression(this, m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartItemInLegendToolTipExpression(this, m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_chart.StyleClass, null, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, m_chart.Name);
			}
			return result;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartItemInLegendHiddenExpression(this, m_chart.Name);
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
				case MemberName.ChartDataPoint:
					writer.WriteReference(m_chartDataPoint);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.LegendText:
					writer.Write(m_legendText);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
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
				case MemberName.ChartDataPoint:
					m_chartDataPoint = reader.ReadReference<ChartDataPoint>(this);
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.LegendText:
					m_legendText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
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
				case MemberName.ChartDataPoint:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chartDataPoint = (ChartDataPoint)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend;
		}
	}
}
