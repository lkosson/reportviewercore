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
	internal sealed class MapLocation : IPersistable
	{
		[NonSerialized]
		private MapLocationExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_left;

		private ExpressionInfo m_top;

		private ExpressionInfo m_unit;

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

		internal ExpressionInfo Unit
		{
			get
			{
				return m_unit;
			}
			set
			{
				m_unit = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapLocationExprHost ExprHost => m_exprHost;

		internal MapLocation()
		{
		}

		internal MapLocation(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLocationStart();
			if (m_left != null)
			{
				m_left.Initialize("Left", context);
				context.ExprHostBuilder.MapLocationLeft(m_left);
			}
			if (m_top != null)
			{
				m_top.Initialize("Top", context);
				context.ExprHostBuilder.MapLocationTop(m_top);
			}
			if (m_unit != null)
			{
				m_unit.Initialize("Unit", context);
				context.ExprHostBuilder.MapLocationUnit(m_unit);
			}
			context.ExprHostBuilder.MapLocationEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapLocation mapLocation = (MapLocation)MemberwiseClone();
			mapLocation.m_map = context.CurrentMapClone;
			if (m_left != null)
			{
				mapLocation.m_left = (ExpressionInfo)m_left.PublishClone(context);
			}
			if (m_top != null)
			{
				mapLocation.m_top = (ExpressionInfo)m_top.PublishClone(context);
			}
			if (m_unit != null)
			{
				mapLocation.m_unit = (ExpressionInfo)m_unit.PublishClone(context);
			}
			return mapLocation;
		}

		internal void SetExprHost(MapLocationExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Left, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Top, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Unit, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLocation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.Top:
					writer.Write(m_top);
					break;
				case MemberName.Unit:
					writer.Write(m_unit);
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
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Left:
					m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Top:
					m_top = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Unit:
					m_unit = (ExpressionInfo)reader.ReadRIFObject();
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
				if (memberName == MemberName.Map)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLocation;
		}

		internal double EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLocationLeftExpression(this, m_map.Name);
		}

		internal double EvaluateTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLocationTopExpression(this, m_map.Name);
		}

		internal Unit EvaluateUnit(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateUnit(context.ReportRuntime.EvaluateMapLocationUnitExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
