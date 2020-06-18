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
	internal sealed class ChartElementPosition : IPersistable
	{
		internal enum Position
		{
			Top,
			Left,
			Height,
			Width
		}

		[NonSerialized]
		private ChartElementPositionExprHost m_exprHost;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_top;

		private ExpressionInfo m_left;

		private ExpressionInfo m_height;

		private ExpressionInfo m_width;

		internal ExpressionInfo Top
		{
			get
			{
				return m_top;
			}
			set
			{
				m_top = value;
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

		internal ExpressionInfo Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal ExpressionInfo Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal string OwnerName => m_chart.Name;

		internal ChartElementPositionExprHost ExprHost => m_exprHost;

		internal ChartElementPosition()
		{
		}

		internal ChartElementPosition(Chart chart)
		{
			m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			Initialize(context, innerPlot: false);
		}

		internal void Initialize(InitializationContext context, bool innerPlot)
		{
			context.ExprHostBuilder.ChartElementPositionStart(innerPlot);
			if (m_top != null)
			{
				m_top.Initialize("Top", context);
				context.ExprHostBuilder.ChartElementPositionTop(m_top);
			}
			if (m_left != null)
			{
				m_left.Initialize("Left", context);
				context.ExprHostBuilder.ChartElementPositionLeft(m_left);
			}
			if (m_height != null)
			{
				m_height.Initialize("Height", context);
				context.ExprHostBuilder.ChartElementPositionHeight(m_height);
			}
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.ChartElementPositionWidth(m_width);
			}
			context.ExprHostBuilder.ChartElementPositionEnd(innerPlot);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartElementPosition chartElementPosition = (ChartElementPosition)MemberwiseClone();
			chartElementPosition.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_top != null)
			{
				chartElementPosition.m_top = (ExpressionInfo)m_top.PublishClone(context);
			}
			if (m_left != null)
			{
				chartElementPosition.m_left = (ExpressionInfo)m_left.PublishClone(context);
			}
			if (m_height != null)
			{
				chartElementPosition.m_height = (ExpressionInfo)m_height.PublishClone(context);
			}
			if (m_width != null)
			{
				chartElementPosition.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			return chartElementPosition;
		}

		internal void SetExprHost(ChartElementPositionExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Top, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Left, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Height, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Top:
					writer.Write(m_top);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.Height:
					writer.Write(m_height);
					break;
				case MemberName.Width:
					writer.Write(m_width);
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
				case MemberName.Top:
					m_top = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Left:
					m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Height:
					m_height = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
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

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition;
		}

		internal double EvaluateTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(Top, "Top", ExprHost, Position.Top, m_chart.Name);
		}

		internal double EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(Left, "Left", ExprHost, Position.Left, m_chart.Name);
		}

		internal double EvaluateHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(Height, "Height", ExprHost, Position.Height, m_chart.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartElementPositionExpression(Width, "Width", ExprHost, Position.Width, m_chart.Name);
		}
	}
}
