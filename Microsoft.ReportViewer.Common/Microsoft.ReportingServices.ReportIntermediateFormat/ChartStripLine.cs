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
	internal sealed class ChartStripLine : ChartStyleContainer, IPersistable, IActionOwner
	{
		private int m_exprHostID;

		private Action m_action;

		private ExpressionInfo m_title;

		private ExpressionInfo m_titleAngle;

		private ExpressionInfo m_textOrientation;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		private ExpressionInfo m_stripWidth;

		private ExpressionInfo m_stripWidthType;

		private int m_id;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartStripLineExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal ChartStripLineExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal int ID => m_id;

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

		internal ExpressionInfo Title
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

		internal ExpressionInfo TitleAngle
		{
			get
			{
				return m_titleAngle;
			}
			set
			{
				m_titleAngle = value;
			}
		}

		internal ExpressionInfo TextOrientation
		{
			get
			{
				return m_textOrientation;
			}
			set
			{
				m_textOrientation = value;
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

		internal ExpressionInfo StripWidth
		{
			get
			{
				return m_stripWidth;
			}
			set
			{
				m_stripWidth = value;
			}
		}

		internal ExpressionInfo StripWidthType
		{
			get
			{
				return m_stripWidthType;
			}
			set
			{
				m_stripWidthType = value;
			}
		}

		internal ChartStripLine()
		{
		}

		internal ChartStripLine(Chart chart, int id)
			: base(chart)
		{
			m_id = id;
		}

		internal void SetExprHost(ChartStripLineExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartStripLineStart(index);
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_title != null)
			{
				m_title.Initialize("Title", context);
				context.ExprHostBuilder.ChartStripLineTitle(m_title);
			}
			if (m_titleAngle != null)
			{
				m_titleAngle.Initialize("TitleAngle", context);
				context.ExprHostBuilder.ChartStripLineTitleAngle(m_titleAngle);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartStripLineToolTip(m_toolTip);
			}
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartStripLineInterval(m_interval);
			}
			if (m_intervalType != null)
			{
				m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartStripLineIntervalType(m_intervalType);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartStripLineIntervalOffset(m_intervalOffset);
			}
			if (m_intervalOffsetType != null)
			{
				m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartStripLineIntervalOffsetType(m_intervalOffsetType);
			}
			if (m_stripWidth != null)
			{
				m_stripWidth.Initialize("StripWidth", context);
				context.ExprHostBuilder.ChartStripLineStripWidth(m_stripWidth);
			}
			if (m_stripWidthType != null)
			{
				m_stripWidthType.Initialize("StripWidthType", context);
				context.ExprHostBuilder.ChartStripLineStripWidthType(m_stripWidthType);
			}
			if (m_textOrientation != null)
			{
				m_textOrientation.Initialize("TextOrientation", context);
				context.ExprHostBuilder.ChartStripLineTextOrientation(m_textOrientation);
			}
			m_exprHostID = context.ExprHostBuilder.ChartStripLineEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartStripLine chartStripLine = (ChartStripLine)base.PublishClone(context);
			if (m_action != null)
			{
				chartStripLine.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_title != null)
			{
				chartStripLine.m_title = (ExpressionInfo)m_title.PublishClone(context);
			}
			if (m_titleAngle != null)
			{
				chartStripLine.m_titleAngle = (ExpressionInfo)m_titleAngle.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartStripLine.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_interval != null)
			{
				chartStripLine.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalType != null)
			{
				chartStripLine.m_intervalType = (ExpressionInfo)m_intervalType.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				chartStripLine.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			if (m_intervalOffsetType != null)
			{
				chartStripLine.m_intervalOffsetType = (ExpressionInfo)m_intervalOffsetType.PublishClone(context);
			}
			if (m_stripWidth != null)
			{
				chartStripLine.m_stripWidth = (ExpressionInfo)m_stripWidth.PublishClone(context);
			}
			if (m_stripWidthType != null)
			{
				chartStripLine.m_stripWidthType = (ExpressionInfo)m_stripWidthType.PublishClone(context);
			}
			if (m_textOrientation != null)
			{
				chartStripLine.m_textOrientation = (ExpressionInfo)m_textOrientation.PublishClone(context);
			}
			return chartStripLine;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Title, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TitleAngle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StripWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StripWidthType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextOrientation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStripLine, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal string EvaluateTitle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineTitleExpression(this, m_chart.Name);
		}

		internal int EvaluateTitleAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineTitleAngleExpression(this, m_chart.Name);
		}

		internal TextOrientations EvaluateTextOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateTextOrientations(context.ReportRuntime.EvaluateChartStripLineTextOrientationExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineToolTipExpression(this, m_chart.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineIntervalExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartStripLineIntervalTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineIntervalOffsetExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartStripLineIntervalOffsetTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateStripWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineStripWidthExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateStripWidthType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartStripLineStripWidthTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.Title:
					writer.Write(m_title);
					break;
				case MemberName.TitleAngle:
					writer.Write(m_titleAngle);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
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
				case MemberName.StripWidth:
					writer.Write(m_stripWidth);
					break;
				case MemberName.StripWidthType:
					writer.Write(m_stripWidthType);
					break;
				case MemberName.TextOrientation:
					writer.Write(m_textOrientation);
					break;
				case MemberName.ID:
					writer.Write(m_id);
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
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Title:
					m_title = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TitleAngle:
					m_titleAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
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
				case MemberName.StripWidth:
					m_stripWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StripWidthType:
					m_stripWidthType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextOrientation:
					m_textOrientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ID:
					m_id = reader.ReadInt32();
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
			if (m_id == 0)
			{
				m_id = m_chart.GenerateActionOwnerID();
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStripLine;
		}
	}
}
