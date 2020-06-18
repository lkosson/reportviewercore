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
	internal sealed class ChartEmptyPoints : ChartStyleContainer, IPersistable, IActionOwner, ICustomPropertiesHolder
	{
		private Action m_action;

		private ChartMarker m_marker;

		private ChartDataLabel m_dataLabel;

		private ExpressionInfo m_axisLabel;

		private DataValueList m_customProperties;

		private ExpressionInfo m_toolTip;

		[Reference]
		private ChartSeries m_chartSeries;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private ChartEmptyPointsExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		internal ChartEmptyPointsExprHost ExprHost => m_exprHost;

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

		internal ChartMarker Marker
		{
			get
			{
				return m_marker;
			}
			set
			{
				m_marker = value;
			}
		}

		internal ChartDataLabel DataLabel
		{
			get
			{
				return m_dataLabel;
			}
			set
			{
				m_dataLabel = value;
			}
		}

		internal ExpressionInfo AxisLabel
		{
			get
			{
				return m_axisLabel;
			}
			set
			{
				m_axisLabel = value;
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

		public override IInstancePath InstancePath
		{
			get
			{
				if (m_chartSeries != null)
				{
					return m_chartSeries;
				}
				return base.InstancePath;
			}
		}

		internal ChartEmptyPoints()
		{
		}

		internal ChartEmptyPoints(Chart chart, ChartSeries chartSeries)
			: base(chart)
		{
			m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartEmptyPointsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_marker != null && m_exprHost.ChartMarkerHost != null)
			{
				m_marker.SetExprHost(m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			if (m_dataLabel != null && m_exprHost.DataLabelHost != null)
			{
				m_dataLabel.SetExprHost(m_exprHost.DataLabelHost, reportObjectModel);
			}
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (m_customProperties != null && m_exprHost.CustomPropertyHostsRemotable != null)
			{
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartEmptyPointsStart();
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_marker != null)
			{
				m_marker.Initialize(context);
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.Initialize(context);
			}
			if (m_axisLabel != null)
			{
				m_axisLabel.Initialize("AxisLabel", context);
				context.ExprHostBuilder.ChartEmptyPointsAxisLabel(m_axisLabel);
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(null, context);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartEmptyPointsToolTip(m_toolTip);
			}
			context.ExprHostBuilder.ChartEmptyPointsEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartEmptyPoints chartEmptyPoints = (ChartEmptyPoints)base.PublishClone(context);
			if (m_action != null)
			{
				chartEmptyPoints.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_marker != null)
			{
				chartEmptyPoints.m_marker = (ChartMarker)m_marker.PublishClone(context);
			}
			if (m_dataLabel != null)
			{
				chartEmptyPoints.m_dataLabel = (ChartDataLabel)m_dataLabel.PublishClone(context);
			}
			if (m_axisLabel != null)
			{
				chartEmptyPoints.m_axisLabel = (ExpressionInfo)m_axisLabel.PublishClone(context);
			}
			if (m_customProperties != null)
			{
				chartEmptyPoints.m_customProperties = new DataValueList(m_customProperties.Count);
				foreach (DataValue customProperty in m_customProperties)
				{
					chartEmptyPoints.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
				}
			}
			if (m_toolTip != null)
			{
				chartEmptyPoints.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			return chartEmptyPoints;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Marker, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.DataLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel));
			list.Add(new MemberInfo(MemberName.AxisLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartEmptyPoints, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateAxisLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartEmptyPointsAxisLabelExpression(this, m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartEmptyPointsToolTipExpression(this, m_chart.Name);
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

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.Marker:
					writer.Write(m_marker);
					break;
				case MemberName.DataLabel:
					writer.Write(m_dataLabel);
					break;
				case MemberName.AxisLabel:
					writer.Write(m_axisLabel);
					break;
				case MemberName.CustomProperties:
					writer.Write(m_customProperties);
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
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Marker:
					m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.DataLabel:
					m_dataLabel = (ChartDataLabel)reader.ReadRIFObject();
					break;
				case MemberName.AxisLabel:
					m_axisLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CustomProperties:
					m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
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
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.ChartSeries)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartEmptyPoints;
		}
	}
}
