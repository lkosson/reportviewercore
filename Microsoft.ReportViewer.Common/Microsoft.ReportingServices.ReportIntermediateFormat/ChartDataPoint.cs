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
	internal sealed class ChartDataPoint : Cell, IPersistable, IActionOwner, IStyleContainer, ICustomPropertiesHolder
	{
		private ChartDataPointValues m_dataPointValues;

		private ChartDataLabel m_dataLabel;

		private Action m_action;

		private Style m_styleClass;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.ContentsOnly;

		private DataValueList m_customProperties;

		private ChartMarker m_marker;

		private ExpressionInfo m_axisLabel;

		private ChartItemInLegend m_itemInLegend;

		private ExpressionInfo m_toolTip;

		[NonSerialized]
		private ChartDataPointExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		protected override bool IsDataRegionBodyCell => true;

		internal ChartDataPointValues DataPointValues
		{
			get
			{
				return m_dataPointValues;
			}
			set
			{
				m_dataPointValues = value;
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

		public Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath => this;

		public Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart;

		public string Name => m_dataRegionDef.Name;

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties => CustomProperties;

		IInstancePath ICustomPropertiesHolder.InstancePath => this;

		internal DataValueList CustomProperties
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

		internal ChartDataPointExprHost ExprHost => m_exprHost;

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

		internal ChartItemInLegend ItemInLegend
		{
			get
			{
				return m_itemInLegend;
			}
			set
			{
				m_itemInLegend = value;
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

		public override Microsoft.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.ChartDataPoint;

		protected override Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode => Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart;

		internal ChartDataPoint()
		{
		}

		internal ChartDataPoint(int id, Chart chart)
			: base(id, chart)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDataPoint chartDataPoint = (ChartDataPoint)base.PublishClone(context);
			if (m_action != null)
			{
				chartDataPoint.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_styleClass != null)
			{
				chartDataPoint.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			if (m_customProperties != null)
			{
				chartDataPoint.m_customProperties = new DataValueList(m_customProperties.Count);
				foreach (DataValue customProperty in m_customProperties)
				{
					chartDataPoint.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
				}
			}
			if (m_marker != null)
			{
				chartDataPoint.m_marker = (ChartMarker)m_marker.PublishClone(context);
			}
			if (m_dataPointValues != null)
			{
				chartDataPoint.m_dataPointValues = (ChartDataPointValues)m_dataPointValues.PublishClone(context);
				chartDataPoint.m_dataPointValues.DataPoint = chartDataPoint;
			}
			if (m_dataLabel != null)
			{
				chartDataPoint.m_dataLabel = (ChartDataLabel)m_dataLabel.PublishClone(context);
			}
			if (m_axisLabel != null)
			{
				chartDataPoint.m_axisLabel = (ExpressionInfo)m_axisLabel.PublishClone(context);
			}
			if (m_itemInLegend != null)
			{
				chartDataPoint.m_itemInLegend = (ChartItemInLegend)m_itemInLegend.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartDataPoint.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			return chartDataPoint;
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			if (m_dataPointValues != null)
			{
				m_dataPointValues.Initialize(context);
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.Initialize(context);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_styleClass != null)
			{
				exprHostBuilder.DataPointStyleStart();
				m_styleClass.Initialize(context);
				exprHostBuilder.DataPointStyleEnd();
			}
			if (m_marker != null)
			{
				m_marker.Initialize(context);
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(null, context);
			}
			if (m_axisLabel != null)
			{
				m_axisLabel.Initialize("AxisLabel", context);
				context.ExprHostBuilder.ChartDataPointAxisLabel(m_axisLabel);
			}
			if (m_itemInLegend != null)
			{
				m_itemInLegend.Initialize(context);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartDataPointToolTip(m_toolTip);
			}
			DataRendererInitialize(context);
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			if (m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				m_dataElementOutput = DataElementOutputTypes.Output;
			}
			Microsoft.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref m_dataElementName, "Value", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.DataPointValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPointValues));
			list.Add(new MemberInfo(MemberName.Marker, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.AxisLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartItemInLegend, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, list);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateAxisLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointAxisLabelExpression(this, m_dataRegionDef.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartDataPointToolTipExpression(this, m_dataRegionDef.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_dataRegionDef.StyleClass, m_styleClass, context, ObjectType, Name);
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
				case MemberName.DataPointValues:
					writer.Write(m_dataPointValues);
					break;
				case MemberName.DataLabel:
					writer.Write(m_dataLabel);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				case MemberName.CustomProperties:
					writer.Write(m_customProperties);
					break;
				case MemberName.Marker:
					writer.Write(m_marker);
					break;
				case MemberName.AxisLabel:
					writer.Write(m_axisLabel);
					break;
				case MemberName.ChartItemInLegend:
					writer.Write(m_itemInLegend);
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
				case MemberName.DataPointValues:
					m_dataPointValues = (ChartDataPointValues)reader.ReadRIFObject();
					break;
				case MemberName.DataLabel:
					m_dataLabel = (ChartDataLabel)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.CustomProperties:
					m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.Marker:
					m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.AxisLabel:
					m_axisLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartItemInLegend:
					m_itemInLegend = (ChartItemInLegend)reader.ReadRIFObject();
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
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint;
		}

		internal void SetExprHost(ChartDataPointExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (m_styleClass != null && m_exprHost.StyleHost != null)
			{
				m_exprHost.StyleHost.SetReportObjectModel(reportObjectModel);
				m_styleClass.SetStyleExprHost(m_exprHost.StyleHost);
			}
			if (m_marker != null && m_exprHost.ChartMarkerHost != null)
			{
				m_marker.SetExprHost(m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			if (m_dataLabel != null && m_exprHost.DataLabelHost != null)
			{
				m_dataLabel.SetExprHost(m_exprHost.DataLabelHost, reportObjectModel);
			}
			if (m_itemInLegend != null && m_exprHost.DataPointInLegendHost != null)
			{
				m_itemInLegend.SetExprHost(m_exprHost.DataPointInLegendHost, reportObjectModel);
			}
			if (m_customProperties != null && m_exprHost.CustomPropertyHostsRemotable != null)
			{
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			BaseSetExprHost(exprHost, reportObjectModel);
		}
	}
}
