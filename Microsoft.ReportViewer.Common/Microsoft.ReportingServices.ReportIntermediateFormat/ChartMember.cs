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
	internal sealed class ChartMember : ReportHierarchyNode, IPersistable
	{
		private ChartMemberList m_chartMembers;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		private ExpressionInfo m_labelExpression;

		[NonSerialized]
		private bool m_chartGroupExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private ChartMemberExprHost m_exprHost;

		internal override string RdlElementName => "ChartMember";

		internal override HierarchyNodeList InnerHierarchy => m_chartMembers;

		internal ChartMemberList ChartMembers
		{
			get
			{
				return m_chartMembers;
			}
			set
			{
				m_chartMembers = value;
			}
		}

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

		internal ExpressionInfo Label
		{
			get
			{
				return m_labelExpression;
			}
			set
			{
				m_labelExpression = value;
			}
		}

		internal bool ChartGroupExpression
		{
			get
			{
				return m_chartGroupExpression;
			}
			set
			{
				m_chartGroupExpression = value;
			}
		}

		internal ChartMemberExprHost ExprHost => m_exprHost;

		internal ChartMember()
		{
		}

		internal ChartMember(int id, Chart crItem)
			: base(id, crItem)
		{
		}

		internal void SetIsCategoryMember(bool value)
		{
			m_isColumn = value;
			if (m_chartMembers == null)
			{
				return;
			}
			foreach (ChartMember chartMember in m_chartMembers)
			{
				chartMember.SetIsCategoryMember(value);
			}
		}

		protected override void DataGroupStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart, m_isColumn);
		}

		protected override int DataGroupEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart, m_isColumn);
		}

		internal override bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			if (m_labelExpression != null)
			{
				m_labelExpression.Initialize("Label", context);
				context.ExprHostBuilder.ChartMemberLabel(m_labelExpression);
			}
			ChartSeries chartSeries = GetChartSeries();
			chartSeries?.Initialize(context, chartSeries.Name);
			return base.InnerInitialize(context, restrictive);
		}

		internal override bool Initialize(InitializationContext context, bool restrictive)
		{
			DataRendererInitialize(context);
			return base.Initialize(context, restrictive);
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			if (m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				if (m_grouping != null)
				{
					m_dataElementOutput = DataElementOutputTypes.Output;
				}
				else
				{
					m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
				}
			}
			string defaultName = string.Empty;
			if (m_grouping != null)
			{
				defaultName = m_grouping.Name + "_Collection";
			}
			Microsoft.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref m_dataElementName, defaultName, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			ChartMember chartMember = (ChartMember)base.PublishClone(context, newContainingRegion);
			if (m_chartMembers != null)
			{
				chartMember.m_chartMembers = new ChartMemberList(m_chartMembers.Count);
				foreach (ChartMember chartMember2 in m_chartMembers)
				{
					chartMember.m_chartMembers.Add(chartMember2.PublishClone(context, newContainingRegion));
				}
			}
			if (m_labelExpression != null)
			{
				chartMember.m_labelExpression = (ExpressionInfo)m_labelExpression.PublishClone(context);
			}
			return chartMember;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ChartMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		private ChartSeries GetChartSeries()
		{
			if (base.IsColumn || ChartMembers != null)
			{
				return null;
			}
			ChartSeriesList chartSeriesCollection = ((Chart)m_dataRegionDef).ChartSeriesCollection;
			if (chartSeriesCollection.Count <= base.MemberCellIndex)
			{
				return null;
			}
			return chartSeriesCollection[base.MemberCellIndex];
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ChartMembers:
					writer.Write(m_chartMembers);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				case MemberName.Label:
					writer.Write(m_labelExpression);
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
				case MemberName.ChartMembers:
					m_chartMembers = reader.ReadListOfRIFObjects<ChartMemberList>();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.Label:
					m_labelExpression = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
				m_exprHost = (ChartMemberExprHost)memberExprHost;
				m_exprHost.SetReportObjectModel(reportObjectModel);
				MemberNodeSetExprHost(m_exprHost, reportObjectModel);
			}
			if (m_exprHost != null && m_exprHost.ChartSeriesHost != null)
			{
				GetChartSeries()?.SetExprHost(m_exprHost.ChartSeriesHost, reportObjectModel);
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateLabel(ChartMemberInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, instance.ReportScopeInstance);
			return context.ReportRuntime.EvaluateChartDynamicMemberLabelExpression(this, m_labelExpression, m_dataRegionDef.Name);
		}

		internal string GetFormattedLabelValue(Microsoft.ReportingServices.RdlExpressions.VariantResult labelObject, OnDemandProcessingContext context)
		{
			string result = null;
			if (labelObject.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (labelObject.Value != null)
			{
				TypeCode typeCode = Type.GetTypeCode(labelObject.Value.GetType());
				if (m_formatter == null)
				{
					m_formatter = new Formatter(base.DataRegionDef.StyleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, base.DataRegionDef.Name);
				}
				result = m_formatter.FormatValue(labelObject.Value, typeCode);
			}
			return result;
		}
	}
}
