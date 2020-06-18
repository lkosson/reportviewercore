using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartAlignType : IPersistable
	{
		private ExpressionInfo m_position;

		private ExpressionInfo m_axesView;

		private ExpressionInfo m_cursor;

		private ExpressionInfo m_innerPlotPosition;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartArea m_chartArea;

		internal ExpressionInfo Cursor
		{
			get
			{
				return m_cursor;
			}
			set
			{
				m_cursor = value;
			}
		}

		internal ExpressionInfo AxesView
		{
			get
			{
				return m_axesView;
			}
			set
			{
				m_axesView = value;
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

		internal ExpressionInfo InnerPlotPosition
		{
			get
			{
				return m_innerPlotPosition;
			}
			set
			{
				m_innerPlotPosition = value;
			}
		}

		internal ChartAreaExprHost ExprHost => m_chartArea.ExprHost;

		internal ChartAlignType()
		{
		}

		internal ChartAlignType(Chart chart)
		{
			m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartAlignTypePosition(m_position);
			}
			if (m_innerPlotPosition != null)
			{
				m_innerPlotPosition.Initialize("InnerPlotPosition", context);
				context.ExprHostBuilder.ChartAlignTypeInnerPlotPosition(m_innerPlotPosition);
			}
			if (m_cursor != null)
			{
				m_cursor.Initialize("Cursor", context);
				context.ExprHostBuilder.ChartAlignTypCursor(m_cursor);
			}
			if (m_axesView != null)
			{
				m_axesView.Initialize("AxesView", context);
				context.ExprHostBuilder.ChartAlignTypeAxesView(m_axesView);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAlignType chartAlignType = (ChartAlignType)MemberwiseClone();
			chartAlignType.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_position != null)
			{
				chartAlignType.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			if (m_innerPlotPosition != null)
			{
				chartAlignType.m_innerPlotPosition = (ExpressionInfo)m_innerPlotPosition.PublishClone(context);
			}
			if (m_cursor != null)
			{
				chartAlignType.m_cursor = (ExpressionInfo)m_cursor.PublishClone(context);
			}
			if (m_axesView != null)
			{
				chartAlignType.m_axesView = (ExpressionInfo)m_axesView.PublishClone(context);
			}
			return chartAlignType;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Cursor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AxesView, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InnerPlotPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAlignType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Position:
					writer.Write(m_position);
					break;
				case MemberName.InnerPlotPosition:
					writer.Write(m_innerPlotPosition);
					break;
				case MemberName.AxesView:
					writer.Write(m_axesView);
					break;
				case MemberName.Cursor:
					writer.Write(m_cursor);
					break;
				case MemberName.Chart:
					writer.WriteReference(m_chart);
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
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InnerPlotPosition:
					m_innerPlotPosition = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AxesView:
					m_axesView = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Cursor:
					m_cursor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
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
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Chart)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAlignType;
		}

		internal void SetExprHost(ChartArea chartArea)
		{
			m_chartArea = chartArea;
		}

		internal bool EvaluateAxesView(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypeAxesViewExpression(this, m_chart.Name, "AxesView");
		}

		internal bool EvaluateCursor(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypeCursorExpression(this, m_chart.Name, "Cursor");
		}

		internal bool EvaluatePosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypePositionExpression(this, m_chart.Name, "Position");
		}

		internal bool EvaluateInnerPlotPosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartAlignTypeInnerPlotPositionExpression(this, m_chart.Name, "InnerPlotPosition");
		}
	}
}
