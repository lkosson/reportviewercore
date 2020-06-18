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
	internal sealed class ChartCustomPaletteColor : IPersistable
	{
		private int m_exprHostID;

		[Reference]
		private Chart m_chart;

		private ExpressionInfo m_color;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartCustomPaletteColorExprHost m_exprHost;

		internal ChartCustomPaletteColorExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal ExpressionInfo Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		internal ChartCustomPaletteColor()
		{
		}

		internal ChartCustomPaletteColor(Chart chart)
		{
			m_chart = chart;
		}

		internal void SetExprHost(ChartCustomPaletteColorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartCustomPaletteColorStart(index);
			if (m_color != null)
			{
				m_color.Initialize("Color", context);
				context.ExprHostBuilder.ChartCustomPaletteColor(m_color);
			}
			m_exprHostID = context.ExprHostBuilder.ChartCustomPaletteColorEnd();
		}

		internal string EvaluateColor(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartCustomPaletteColorExpression(this, m_chart.Name);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartCustomPaletteColor chartCustomPaletteColor = (ChartCustomPaletteColor)MemberwiseClone();
			chartCustomPaletteColor.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_color != null)
			{
				chartCustomPaletteColor.m_color = (ExpressionInfo)m_color.PublishClone(context);
			}
			return chartCustomPaletteColor;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Color, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartCustomPaletteColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Color:
					writer.Write(m_color);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
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
				case MemberName.Color:
					m_color = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartCustomPaletteColor;
		}
	}
}
