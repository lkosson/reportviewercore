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
	internal sealed class MapGridLines : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapGridLinesExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_showLabels;

		private ExpressionInfo m_labelPosition;

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

		internal ExpressionInfo ShowLabels
		{
			get
			{
				return m_showLabels;
			}
			set
			{
				m_showLabels = value;
			}
		}

		internal ExpressionInfo LabelPosition
		{
			get
			{
				return m_labelPosition;
			}
			set
			{
				m_labelPosition = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapGridLinesExprHost ExprHost => m_exprHost;

		internal MapGridLines()
		{
		}

		internal MapGridLines(Map map)
			: base(map)
		{
		}

		internal void Initialize(InitializationContext context, bool isMeridian)
		{
			context.ExprHostBuilder.MapGridLinesStart(isMeridian);
			base.Initialize(context);
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.MapGridLinesHidden(m_hidden);
			}
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.MapGridLinesInterval(m_interval);
			}
			if (m_showLabels != null)
			{
				m_showLabels.Initialize("ShowLabels", context);
				context.ExprHostBuilder.MapGridLinesShowLabels(m_showLabels);
			}
			if (m_labelPosition != null)
			{
				m_labelPosition.Initialize("LabelPosition", context);
				context.ExprHostBuilder.MapGridLinesLabelPosition(m_labelPosition);
			}
			context.ExprHostBuilder.MapGridLinesEnd(isMeridian);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapGridLines mapGridLines = (MapGridLines)base.PublishClone(context);
			if (m_hidden != null)
			{
				mapGridLines.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_interval != null)
			{
				mapGridLines.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_showLabels != null)
			{
				mapGridLines.m_showLabels = (ExpressionInfo)m_showLabels.PublishClone(context);
			}
			if (m_labelPosition != null)
			{
				mapGridLines.m_labelPosition = (ExpressionInfo)m_labelPosition.PublishClone(context);
			}
			return mapGridLines;
		}

		internal void SetExprHost(MapGridLinesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapGridLines, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Interval:
					writer.Write(m_interval);
					break;
				case MemberName.ShowLabels:
					writer.Write(m_showLabels);
					break;
				case MemberName.LabelPosition:
					writer.Write(m_labelPosition);
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
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowLabels:
					m_showLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelPosition:
					m_labelPosition = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapGridLines;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapGridLinesHiddenExpression(this, m_map.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapGridLinesIntervalExpression(this, m_map.Name);
		}

		internal bool EvaluateShowLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapGridLinesShowLabelsExpression(this, m_map.Name);
		}

		internal MapLabelPosition EvaluateLabelPosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateLabelPosition(context.ReportRuntime.EvaluateMapGridLinesLabelPositionExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
