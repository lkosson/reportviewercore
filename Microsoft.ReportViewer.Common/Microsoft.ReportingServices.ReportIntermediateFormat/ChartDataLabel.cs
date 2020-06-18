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
	internal sealed class ChartDataLabel : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_visible;

		private ExpressionInfo m_label;

		private ExpressionInfo m_position;

		private ExpressionInfo m_rotation;

		private ExpressionInfo m_useValueAsLabel;

		private Action m_action;

		private ExpressionInfo m_toolTip;

		[Reference]
		private ChartDataPoint m_chartDataPoint;

		[Reference]
		private ChartSeries m_chartSeries;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private ChartDataLabelExprHost m_exprHost;

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

		internal ExpressionInfo Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal ExpressionInfo UseValueAsLabel
		{
			get
			{
				return m_useValueAsLabel;
			}
			set
			{
				m_useValueAsLabel = value;
			}
		}

		internal ExpressionInfo Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal ExpressionInfo Rotation
		{
			get
			{
				return m_rotation;
			}
			set
			{
				m_rotation = value;
			}
		}

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

		internal ChartDataLabelExprHost ExprHost => m_exprHost;

		public override IInstancePath InstancePath
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
				return base.InstancePath;
			}
		}

		internal ChartDataLabel()
		{
		}

		internal ChartDataLabel(Chart chart, ChartDataPoint chartDataPoint)
			: base(chart)
		{
			m_chartDataPoint = chartDataPoint;
		}

		internal ChartDataLabel(Chart chart, ChartSeries chartSeries)
			: base(chart)
		{
			m_chartSeries = chartSeries;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDataLabel chartDataLabel = (ChartDataLabel)base.PublishClone(context);
			if (m_label != null)
			{
				chartDataLabel.m_label = (ExpressionInfo)m_label.PublishClone(context);
			}
			if (m_visible != null)
			{
				chartDataLabel.m_visible = (ExpressionInfo)m_visible.PublishClone(context);
			}
			if (m_position != null)
			{
				chartDataLabel.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			if (m_rotation != null)
			{
				chartDataLabel.m_rotation = (ExpressionInfo)m_rotation.PublishClone(context);
			}
			if (m_useValueAsLabel != null)
			{
				chartDataLabel.m_useValueAsLabel = (ExpressionInfo)m_useValueAsLabel.PublishClone(context);
			}
			if (m_action != null)
			{
				chartDataLabel.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartDataLabel.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			return chartDataLabel;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataLabelStart();
			base.Initialize(context);
			if (m_label != null)
			{
				m_label.Initialize("Label", context);
				context.ExprHostBuilder.DataLabelLabel(m_label);
			}
			if (m_visible != null)
			{
				m_visible.Initialize("Visible", context);
				context.ExprHostBuilder.DataLabelVisible(m_visible);
			}
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.DataLabelPosition(m_position);
			}
			if (m_rotation != null)
			{
				m_rotation.Initialize("Rotation", context);
				context.ExprHostBuilder.DataLabelRotation(m_rotation);
			}
			if (m_useValueAsLabel != null)
			{
				m_useValueAsLabel.Initialize("UseValueAsLabel", context);
				context.ExprHostBuilder.DataLabelUseValueAsLabel(m_useValueAsLabel);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartDataLabelToolTip(m_toolTip);
			}
			context.ExprHostBuilder.DataLabelEnd();
		}

		internal void SetExprHost(ChartDataLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(null != exprHost && null != reportObjectModel)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Visible, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseValueAsLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Rotation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Visible:
					writer.Write(m_visible);
					break;
				case MemberName.Label:
					writer.Write(m_label);
					break;
				case MemberName.Position:
					writer.Write(m_position);
					break;
				case MemberName.Rotation:
					writer.Write(m_rotation);
					break;
				case MemberName.UseValueAsLabel:
					writer.Write(m_useValueAsLabel);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(m_chartDataPoint);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(m_chartSeries);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
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
				case MemberName.Visible:
					m_visible = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					m_label = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Rotation:
					m_rotation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseValueAsLabel:
					m_useValueAsLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ChartDataPoint:
					m_chartDataPoint = reader.ReadReference<ChartDataPoint>(this);
					break;
				case MemberName.ChartSeries:
					m_chartSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
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
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.ChartDataPoint:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chartDataPoint = (ChartDataPoint)referenceableItems[item.RefID];
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

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel;
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelLabelExpression(this, base.Name);
		}

		internal ChartDataLabelPositions EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartDataLabelPosition(context.ReportRuntime.EvaluateChartDataLabePositionExpression(this, base.Name), context.ReportRuntime);
		}

		internal int EvaluateRotation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelRotationExpression(this, base.Name);
		}

		internal bool EvaluateUseValueAsLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelUseValueAsLabelExpression(this, base.Name);
		}

		internal bool EvaluateVisible(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelVisibleExpression(this, base.Name);
		}

		internal string GetFormattedValue(Microsoft.ReportingServices.RdlExpressions.VariantResult originalValue, IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			if (originalValue.ErrorOccurred)
			{
				return RPRes.rsExpressionErrorValue;
			}
			if (originalValue.Value != null)
			{
				return Formatter.Format(originalValue.Value, ref m_formatter, m_chart.StyleClass, m_styleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, m_chart.Name);
			}
			return null;
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartDataLabelToolTipExpression(this, m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_chart.StyleClass, m_styleClass, context, base.ObjectType, base.Name);
			}
			return result;
		}
	}
}
