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
	internal sealed class ChartMarker : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_markerType;

		private ExpressionInfo m_markerSize;

		[Reference]
		private ChartDataPoint m_chartDataPoint;

		[Reference]
		private ChartSeries m_chartSeries;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartMarkerExprHost m_exprHost;

		internal ExpressionInfo Type
		{
			get
			{
				return m_markerType;
			}
			set
			{
				m_markerType = value;
			}
		}

		internal ExpressionInfo Size
		{
			get
			{
				return m_markerSize;
			}
			set
			{
				m_markerSize = value;
			}
		}

		internal ChartMarkerExprHost ExprHost => m_exprHost;

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

		internal ChartMarker()
		{
		}

		internal ChartMarker(Chart chart, ChartDataPoint chartDataPoint)
			: base(chart)
		{
			m_chartDataPoint = chartDataPoint;
		}

		internal ChartMarker(Chart chart, ChartSeries chartSeries)
			: base(chart)
		{
			m_chartSeries = chartSeries;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Size, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.Write(m_markerType);
					break;
				case MemberName.Size:
					writer.Write(m_markerSize);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(m_chartDataPoint);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(m_chartSeries);
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
				case MemberName.Type:
					m_markerType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Size:
					m_markerSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartDataPoint:
					m_chartDataPoint = reader.ReadReference<ChartDataPoint>(this);
					break;
				case MemberName.ChartSeries:
					m_chartSeries = reader.ReadReference<ChartSeries>(this);
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartMarker chartMarker = (ChartMarker)base.PublishClone(context);
			if (m_markerSize != null)
			{
				chartMarker.m_markerSize = (ExpressionInfo)m_markerSize.PublishClone(context);
			}
			if (m_markerType != null)
			{
				chartMarker.m_markerType = (ExpressionInfo)m_markerType.PublishClone(context);
			}
			return chartMarker;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataPointMarkerStart();
			base.Initialize(context);
			if (m_markerSize != null)
			{
				m_markerSize.Initialize("Size", context);
				context.ExprHostBuilder.DataPointMarkerSize(m_markerSize);
			}
			if (m_markerType != null)
			{
				m_markerType.Initialize("Type", context);
				context.ExprHostBuilder.DataPointMarkerType(m_markerType);
			}
			context.ExprHostBuilder.DataPointMarkerEnd();
		}

		internal void SetExprHost(ChartMarkerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal string EvaluateChartMarkerSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartMarkerSize(this, m_chart.Name);
		}

		internal ChartMarkerTypes EvaluateChartMarkerType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartMarkerType(context.ReportRuntime.EvaluateChartMarkerType(this, m_chart.Name), context.ReportRuntime);
		}
	}
}
