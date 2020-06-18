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
	internal sealed class MapBorderSkin : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapBorderSkinExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_mapBorderSkinType;

		internal ExpressionInfo MapBorderSkinType
		{
			get
			{
				return m_mapBorderSkinType;
			}
			set
			{
				m_mapBorderSkinType = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapBorderSkinExprHost ExprHost => m_exprHost;

		internal MapBorderSkin()
		{
		}

		internal MapBorderSkin(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapBorderSkinStart();
			base.Initialize(context);
			if (m_mapBorderSkinType != null)
			{
				m_mapBorderSkinType.Initialize("MapBorderSkinType", context);
				context.ExprHostBuilder.MapBorderSkinMapBorderSkinType(m_mapBorderSkinType);
			}
			context.ExprHostBuilder.MapBorderSkinEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapBorderSkin mapBorderSkin = (MapBorderSkin)base.PublishClone(context);
			if (m_mapBorderSkinType != null)
			{
				mapBorderSkin.m_mapBorderSkinType = (ExpressionInfo)m_mapBorderSkinType.PublishClone(context);
			}
			return mapBorderSkin;
		}

		internal void SetExprHost(MapBorderSkinExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapBorderSkinType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBorderSkin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapBorderSkinType)
				{
					writer.Write(m_mapBorderSkinType);
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
				if (memberName == MemberName.MapBorderSkinType)
				{
					m_mapBorderSkinType = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBorderSkin;
		}

		internal MapBorderSkinType EvaluateMapBorderSkinType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapBorderSkinType(context.ReportRuntime.EvaluateMapBorderSkinMapBorderSkinTypeExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
