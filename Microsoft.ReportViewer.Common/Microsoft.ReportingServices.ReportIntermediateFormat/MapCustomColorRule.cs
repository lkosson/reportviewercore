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
	internal sealed class MapCustomColorRule : MapColorRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private List<MapCustomColor> m_mapCustomColors;

		internal List<MapCustomColor> MapCustomColors
		{
			get
			{
				return m_mapCustomColors;
			}
			set
			{
				m_mapCustomColors = value;
			}
		}

		internal new MapCustomColorRuleExprHost ExprHost => (MapCustomColorRuleExprHost)m_exprHost;

		internal MapCustomColorRule()
		{
		}

		internal MapCustomColorRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapCustomColorRuleStart();
			base.Initialize(context);
			if (m_mapCustomColors != null)
			{
				for (int i = 0; i < m_mapCustomColors.Count; i++)
				{
					m_mapCustomColors[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapCustomColorRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapCustomColorRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapCustomColorRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapCustomColorRule mapCustomColorRule = (MapCustomColorRule)base.PublishClone(context);
			if (m_mapCustomColors != null)
			{
				mapCustomColorRule.m_mapCustomColors = new List<MapCustomColor>(m_mapCustomColors.Count);
				{
					foreach (MapCustomColor mapCustomColor in m_mapCustomColors)
					{
						mapCustomColorRule.m_mapCustomColors.Add((MapCustomColor)mapCustomColor.PublishClone(context));
					}
					return mapCustomColorRule;
				}
			}
			return mapCustomColorRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapCustomColorExprHost> mapCustomColorsHostsRemotable = ExprHost.MapCustomColorsHostsRemotable;
			if (m_mapCustomColors == null || mapCustomColorsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapCustomColors.Count; i++)
			{
				MapCustomColor mapCustomColor = m_mapCustomColors[i];
				if (mapCustomColor != null && mapCustomColor.ExpressionHostID > -1)
				{
					mapCustomColor.SetExprHost(mapCustomColorsHostsRemotable[mapCustomColor.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapCustomColors, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColor));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColorRule, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapCustomColors)
				{
					writer.Write(m_mapCustomColors);
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
				if (memberName == MemberName.MapCustomColors)
				{
					m_mapCustomColors = reader.ReadGenericListOfRIFObjects<MapCustomColor>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColorRule;
		}
	}
}
