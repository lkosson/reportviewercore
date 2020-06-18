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
	internal sealed class ChartNoMoveDirections : IPersistable
	{
		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_chartSeries;

		private ExpressionInfo m_up;

		private ExpressionInfo m_down;

		private ExpressionInfo m_left;

		private ExpressionInfo m_right;

		private ExpressionInfo m_upLeft;

		private ExpressionInfo m_upRight;

		private ExpressionInfo m_downLeft;

		private ExpressionInfo m_downRight;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartNoMoveDirectionsExprHost m_exprHost;

		internal ChartNoMoveDirectionsExprHost ExprHost => m_exprHost;

		internal ExpressionInfo Up
		{
			get
			{
				return m_up;
			}
			set
			{
				m_up = value;
			}
		}

		internal ExpressionInfo Down
		{
			get
			{
				return m_down;
			}
			set
			{
				m_down = value;
			}
		}

		internal ExpressionInfo Left
		{
			get
			{
				return m_left;
			}
			set
			{
				m_left = value;
			}
		}

		internal ExpressionInfo Right
		{
			get
			{
				return m_right;
			}
			set
			{
				m_right = value;
			}
		}

		internal ExpressionInfo UpLeft
		{
			get
			{
				return m_upLeft;
			}
			set
			{
				m_upLeft = value;
			}
		}

		internal ExpressionInfo UpRight
		{
			get
			{
				return m_upRight;
			}
			set
			{
				m_upRight = value;
			}
		}

		internal ExpressionInfo DownLeft
		{
			get
			{
				return m_downLeft;
			}
			set
			{
				m_downLeft = value;
			}
		}

		internal ExpressionInfo DownRight
		{
			get
			{
				return m_downRight;
			}
			set
			{
				m_downRight = value;
			}
		}

		internal ChartNoMoveDirections()
		{
		}

		internal ChartNoMoveDirections(Chart chart, ChartSeries chartSeries)
		{
			m_chart = chart;
			m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartNoMoveDirectionsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartNoMoveDirectionsStart();
			if (m_up != null)
			{
				m_up.Initialize("Up", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsUp(m_up);
			}
			if (m_down != null)
			{
				m_down.Initialize("Down", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsDown(m_down);
			}
			if (m_left != null)
			{
				m_left.Initialize("Left", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsLeft(m_left);
			}
			if (m_right != null)
			{
				m_right.Initialize("Right", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsRight(m_right);
			}
			if (m_upLeft != null)
			{
				m_upLeft.Initialize("UpLeft", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsUpLeft(m_upLeft);
			}
			if (m_upRight != null)
			{
				m_upRight.Initialize("UpRight", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsUpRight(m_upRight);
			}
			if (m_downLeft != null)
			{
				m_downLeft.Initialize("DownLeft", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsDownLeft(m_downLeft);
			}
			if (m_downRight != null)
			{
				m_downRight.Initialize("DownRight", context);
				context.ExprHostBuilder.ChartNoMoveDirectionsDownRight(m_downRight);
			}
			context.ExprHostBuilder.ChartNoMoveDirectionsEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartNoMoveDirections chartNoMoveDirections = (ChartNoMoveDirections)MemberwiseClone();
			chartNoMoveDirections.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_up != null)
			{
				chartNoMoveDirections.m_up = (ExpressionInfo)m_up.PublishClone(context);
			}
			if (m_down != null)
			{
				chartNoMoveDirections.m_down = (ExpressionInfo)m_down.PublishClone(context);
			}
			if (m_left != null)
			{
				chartNoMoveDirections.m_left = (ExpressionInfo)m_left.PublishClone(context);
			}
			if (m_right != null)
			{
				chartNoMoveDirections.m_right = (ExpressionInfo)m_right.PublishClone(context);
			}
			if (m_upLeft != null)
			{
				chartNoMoveDirections.m_upLeft = (ExpressionInfo)m_upLeft.PublishClone(context);
			}
			if (m_upRight != null)
			{
				chartNoMoveDirections.m_upRight = (ExpressionInfo)m_upRight.PublishClone(context);
			}
			if (m_downLeft != null)
			{
				chartNoMoveDirections.m_downLeft = (ExpressionInfo)m_downLeft.PublishClone(context);
			}
			if (m_downRight != null)
			{
				chartNoMoveDirections.m_downRight = (ExpressionInfo)m_downRight.PublishClone(context);
			}
			return chartNoMoveDirections;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Up, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Down, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Left, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Right, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UpLeft, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UpRight, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DownLeft, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DownRight, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoMoveDirections, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal bool EvaluateUp(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsUpExpression(this, m_chart.Name);
		}

		internal bool EvaluateDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsDownExpression(this, m_chart.Name);
		}

		internal bool EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsLeftExpression(this, m_chart.Name);
		}

		internal bool EvaluateRight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsRightExpression(this, m_chart.Name);
		}

		internal bool EvaluateUpLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsUpLeftExpression(this, m_chart.Name);
		}

		internal bool EvaluateUpRight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsUpRightExpression(this, m_chart.Name);
		}

		internal bool EvaluateDownLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsDownLeftExpression(this, m_chart.Name);
		}

		internal bool EvaluateDownRight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chartSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartNoMoveDirectionsDownRightExpression(this, m_chart.Name);
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
				case MemberName.Up:
					writer.Write(m_up);
					break;
				case MemberName.Down:
					writer.Write(m_down);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.Right:
					writer.Write(m_right);
					break;
				case MemberName.UpLeft:
					writer.Write(m_upLeft);
					break;
				case MemberName.UpRight:
					writer.Write(m_upRight);
					break;
				case MemberName.DownLeft:
					writer.Write(m_downLeft);
					break;
				case MemberName.DownRight:
					writer.Write(m_downRight);
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
				case MemberName.Up:
					m_up = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Down:
					m_down = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Left:
					m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Right:
					m_right = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UpLeft:
					m_upLeft = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UpRight:
					m_upRight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DownLeft:
					m_downLeft = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DownRight:
					m_downRight = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoMoveDirections;
		}
	}
}
