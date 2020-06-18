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
	internal sealed class ChartBorderSkin : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_borderSkinType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartBorderSkinExprHost m_exprHost;

		internal ChartBorderSkinExprHost ExprHost => m_exprHost;

		internal ExpressionInfo BorderSkinType
		{
			get
			{
				return m_borderSkinType;
			}
			set
			{
				m_borderSkinType = value;
			}
		}

		internal ChartBorderSkin()
		{
		}

		internal ChartBorderSkin(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartBorderSkinExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartBorderSkinStart();
			base.Initialize(context);
			if (m_borderSkinType != null)
			{
				m_borderSkinType.Initialize("ChartBorderSkinType", context);
				context.ExprHostBuilder.ChartBorderSkinBorderSkinType(m_borderSkinType);
			}
			context.ExprHostBuilder.ChartBorderSkinEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartBorderSkin chartBorderSkin = (ChartBorderSkin)base.PublishClone(context);
			if (m_borderSkinType != null)
			{
				chartBorderSkin.m_borderSkinType = (ExpressionInfo)m_borderSkinType.PublishClone(context);
			}
			return chartBorderSkin;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.BorderSkinType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartBorderSkin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartBorderSkinType EvaluateBorderSkinType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartBorderSkinType(context.ReportRuntime.EvaluateChartBorderSkinBorderSkinTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.BorderSkinType)
				{
					writer.Write(m_borderSkinType);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.BorderSkinType)
				{
					m_borderSkinType = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartBorderSkin;
		}
	}
}
