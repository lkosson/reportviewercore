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
	internal sealed class ChartArea : ChartStyleContainer, IPersistable
	{
		private string m_name;

		private List<ChartAxis> m_categoryAxes;

		private List<ChartAxis> m_valueAxes;

		private ChartThreeDProperties m_3dProperties;

		private ChartElementPosition m_chartElementPosition;

		private ChartElementPosition m_chartInnerPlotPosition;

		private int m_exprHostID;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_alignOrientation;

		private ChartAlignType m_chartAlignType;

		private string m_alignWithChartArea;

		private ExpressionInfo m_equallySizedAxesFont;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartAreaExprHost m_exprHost;

		internal string ChartAreaName
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

		internal List<ChartAxis> CategoryAxes
		{
			get
			{
				return m_categoryAxes;
			}
			set
			{
				m_categoryAxes = value;
			}
		}

		internal List<ChartAxis> ValueAxes
		{
			get
			{
				return m_valueAxes;
			}
			set
			{
				m_valueAxes = value;
			}
		}

		internal ChartThreeDProperties ThreeDProperties
		{
			get
			{
				return m_3dProperties;
			}
			set
			{
				m_3dProperties = value;
			}
		}

		internal ChartAreaExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

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

		internal ExpressionInfo AlignOrientation
		{
			get
			{
				return m_alignOrientation;
			}
			set
			{
				m_alignOrientation = value;
			}
		}

		internal ChartAlignType ChartAlignType
		{
			get
			{
				return m_chartAlignType;
			}
			set
			{
				m_chartAlignType = value;
			}
		}

		internal string AlignWithChartArea
		{
			get
			{
				return m_alignWithChartArea;
			}
			set
			{
				m_alignWithChartArea = value;
			}
		}

		internal ExpressionInfo EquallySizedAxesFont
		{
			get
			{
				return m_equallySizedAxesFont;
			}
			set
			{
				m_equallySizedAxesFont = value;
			}
		}

		internal ChartElementPosition ChartElementPosition
		{
			get
			{
				return m_chartElementPosition;
			}
			set
			{
				m_chartElementPosition = value;
			}
		}

		internal ChartElementPosition ChartInnerPlotPosition
		{
			get
			{
				return m_chartInnerPlotPosition;
			}
			set
			{
				m_chartInnerPlotPosition = value;
			}
		}

		internal Chart Chart
		{
			get
			{
				return m_chart;
			}
			set
			{
				m_chart = value;
			}
		}

		internal ChartArea()
		{
		}

		internal ChartArea(Chart chart)
			: base(chart)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartAreaStart(m_name);
			base.Initialize(context);
			if (m_categoryAxes != null)
			{
				for (int i = 0; i < m_categoryAxes.Count; i++)
				{
					m_categoryAxes[i].Initialize(context, isValueAxis: false);
				}
			}
			if (m_valueAxes != null)
			{
				for (int j = 0; j < m_valueAxes.Count; j++)
				{
					m_valueAxes[j].Initialize(context, isValueAxis: true);
				}
			}
			if (m_3dProperties != null)
			{
				m_3dProperties.Initialize(context);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartAreaHidden(m_hidden);
			}
			if (m_alignOrientation != null)
			{
				m_alignOrientation.Initialize("AlignOrientation", context);
				context.ExprHostBuilder.ChartAreaAlignOrientation(m_alignOrientation);
			}
			if (m_chartAlignType != null)
			{
				m_chartAlignType.Initialize(context);
			}
			if (m_equallySizedAxesFont != null)
			{
				m_equallySizedAxesFont.Initialize("EquallySizedAxesFont", context);
				context.ExprHostBuilder.ChartAreaEquallySizedAxesFont(m_equallySizedAxesFont);
			}
			if (m_chartElementPosition != null)
			{
				m_chartElementPosition.Initialize(context);
			}
			if (m_chartInnerPlotPosition != null)
			{
				m_chartInnerPlotPosition.Initialize(context, innerPlot: true);
			}
			m_exprHostID = context.ExprHostBuilder.ChartAreaEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartArea chartArea = (ChartArea)base.PublishClone(context);
			if (m_categoryAxes != null)
			{
				chartArea.m_categoryAxes = new List<ChartAxis>(m_categoryAxes.Count);
				foreach (ChartAxis categoryAxis in m_categoryAxes)
				{
					chartArea.m_categoryAxes.Add((ChartAxis)categoryAxis.PublishClone(context));
				}
			}
			if (m_valueAxes != null)
			{
				chartArea.m_valueAxes = new List<ChartAxis>(m_valueAxes.Count);
				foreach (ChartAxis valueAxis in m_valueAxes)
				{
					chartArea.m_valueAxes.Add((ChartAxis)valueAxis.PublishClone(context));
				}
			}
			if (m_3dProperties != null)
			{
				chartArea.m_3dProperties = (ChartThreeDProperties)m_3dProperties.PublishClone(context);
			}
			if (m_hidden != null)
			{
				chartArea.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_alignOrientation != null)
			{
				chartArea.m_alignOrientation = (ExpressionInfo)m_alignOrientation.PublishClone(context);
			}
			if (m_chartAlignType != null)
			{
				chartArea.m_chartAlignType = (ChartAlignType)m_chartAlignType.PublishClone(context);
			}
			if (m_equallySizedAxesFont != null)
			{
				chartArea.m_equallySizedAxesFont = (ExpressionInfo)m_equallySizedAxesFont.PublishClone(context);
			}
			if (m_chartElementPosition != null)
			{
				chartArea.m_chartElementPosition = (ChartElementPosition)m_chartElementPosition.PublishClone(context);
			}
			if (m_chartInnerPlotPosition != null)
			{
				chartArea.m_chartInnerPlotPosition = (ChartElementPosition)m_chartInnerPlotPosition.PublishClone(context);
			}
			return chartArea;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.CategoryAxes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis));
			list.Add(new MemberInfo(MemberName.ValueAxes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis));
			list.Add(new MemberInfo(MemberName.ThreeDProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ThreeDProperties));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AlignOrientation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartAlignType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAlignType));
			list.Add(new MemberInfo(MemberName.AlignWithChartArea, Token.String));
			list.Add(new MemberInfo(MemberName.EquallySizedAxesFont, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ChartElementPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			list.Add(new MemberInfo(MemberName.ChartInnerPlotPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartArea, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
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
				case MemberName.CategoryAxes:
					writer.Write(m_categoryAxes);
					break;
				case MemberName.ValueAxes:
					writer.Write(m_valueAxes);
					break;
				case MemberName.ThreeDProperties:
					writer.Write(m_3dProperties);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.AlignOrientation:
					writer.Write(m_alignOrientation);
					break;
				case MemberName.ChartAlignType:
					writer.Write(m_chartAlignType);
					break;
				case MemberName.AlignWithChartArea:
					writer.Write(m_alignWithChartArea);
					break;
				case MemberName.EquallySizedAxesFont:
					writer.Write(m_equallySizedAxesFont);
					break;
				case MemberName.ChartElementPosition:
					writer.Write(m_chartElementPosition);
					break;
				case MemberName.ChartInnerPlotPosition:
					writer.Write(m_chartInnerPlotPosition);
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
				case MemberName.CategoryAxes:
					m_categoryAxes = reader.ReadGenericListOfRIFObjects<ChartAxis>();
					break;
				case MemberName.ValueAxes:
					m_valueAxes = reader.ReadGenericListOfRIFObjects<ChartAxis>();
					break;
				case MemberName.ThreeDProperties:
					m_3dProperties = (ChartThreeDProperties)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AlignOrientation:
					m_alignOrientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartAlignType:
					m_chartAlignType = (ChartAlignType)reader.ReadRIFObject();
					break;
				case MemberName.AlignWithChartArea:
					m_alignWithChartArea = reader.ReadString();
					break;
				case MemberName.EquallySizedAxesFont:
					m_equallySizedAxesFont = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartElementPosition:
					m_chartElementPosition = (ChartElementPosition)reader.ReadRIFObject();
					break;
				case MemberName.ChartInnerPlotPosition:
					m_chartInnerPlotPosition = (ChartElementPosition)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartArea;
		}

		internal void SetExprHost(ChartAreaExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_3dProperties != null && m_exprHost.Chart3DPropertiesHost != null)
			{
				m_3dProperties.SetExprHost(m_exprHost.Chart3DPropertiesHost, reportObjectModel);
			}
			if (m_chartAlignType != null)
			{
				m_chartAlignType.SetExprHost(this);
			}
			IList<ChartAxisExprHost> categoryAxesHostsRemotable = exprHost.CategoryAxesHostsRemotable;
			if (m_categoryAxes != null && categoryAxesHostsRemotable != null)
			{
				for (int i = 0; i < m_categoryAxes.Count; i++)
				{
					ChartAxis chartAxis = m_categoryAxes[i];
					if (chartAxis != null && chartAxis.ExpressionHostID > -1)
					{
						chartAxis.SetExprHost(categoryAxesHostsRemotable[chartAxis.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartAxisExprHost> valueAxesHostsRemotable = exprHost.ValueAxesHostsRemotable;
			if (m_valueAxes != null && valueAxesHostsRemotable != null)
			{
				for (int j = 0; j < m_valueAxes.Count; j++)
				{
					ChartAxis chartAxis2 = m_valueAxes[j];
					if (chartAxis2 != null && chartAxis2.ExpressionHostID > -1)
					{
						chartAxis2.SetExprHost(valueAxesHostsRemotable[chartAxis2.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_chartElementPosition != null && m_exprHost.ChartElementPositionHost != null)
			{
				m_chartElementPosition.SetExprHost(m_exprHost.ChartElementPositionHost, reportObjectModel);
			}
			if (m_chartInnerPlotPosition != null && m_exprHost.ChartInnerPlotPositionHost != null)
			{
				m_chartInnerPlotPosition.SetExprHost(m_exprHost.ChartInnerPlotPositionHost, reportObjectModel);
			}
		}

		internal bool EvaluateHidden(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAreaHiddenExpression(this, base.Name, "Hidden");
		}

		internal ChartAreaAlignOrientations EvaluateAlignOrientation(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartAreaAlignOrientation(context.ReportRuntime.EvaluateChartAreaAlignOrientationExpression(this, base.Name, "AlignOrientation"), context.ReportRuntime);
		}

		internal bool EvaluateEquallySizedAxesFont(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAreaEquallySizedAxesFontExpression(this, base.Name, "EquallySizedAxesFont");
		}
	}
}
