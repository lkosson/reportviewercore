using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapBindingFieldPair : IPersistable
	{
		private int m_exprHostID = -1;

		private int m_exprHostMapMemberID = -1;

		[NonSerialized]
		private MapBindingFieldPairExprHost m_exprHost;

		[NonSerialized]
		private MapBindingFieldPairExprHost m_exprHostMapMember;

		[Reference]
		private Map m_map;

		[Reference]
		private MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_fieldName;

		private ExpressionInfo m_bindingExpression;

		internal ExpressionInfo FieldName
		{
			get
			{
				return m_fieldName;
			}
			set
			{
				m_fieldName = value;
			}
		}

		internal ExpressionInfo BindingExpression
		{
			get
			{
				return m_bindingExpression;
			}
			set
			{
				m_bindingExpression = value;
			}
		}

		internal bool InElementView => m_mapVectorLayer == null;

		internal string OwnerName => m_map.Name;

		internal MapBindingFieldPairExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapBindingFieldPairExprHost ExprHostMapMember => m_exprHostMapMember;

		internal int ExpressionHostMapMemberID => m_exprHostMapMemberID;

		internal IInstancePath InstancePath
		{
			get
			{
				if (m_mapVectorLayer != null)
				{
					return m_mapVectorLayer.InstancePath;
				}
				return m_map;
			}
		}

		internal MapBindingFieldPair()
		{
		}

		internal MapBindingFieldPair(Map map, MapVectorLayer mapVectorLayer)
		{
			m_map = map;
			m_mapVectorLayer = mapVectorLayer;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapBindingFieldPairStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (m_fieldName != null)
			{
				m_fieldName.Initialize("FieldName", context);
				context.ExprHostBuilder.MapBindingFieldPairFieldName(m_fieldName);
			}
			if (InElementView && m_bindingExpression != null)
			{
				m_bindingExpression.Initialize("BindingExpression", context);
				context.ExprHostBuilder.MapBindingFieldPairBindingExpression(m_bindingExpression);
			}
			m_exprHostID = context.ExprHostBuilder.MapBindingFieldPairEnd();
		}

		internal void InitializeMapMember(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapBindingFieldPairStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (!InElementView && m_bindingExpression != null)
			{
				m_bindingExpression.Initialize("BindingExpression", context);
				context.ExprHostBuilder.MapBindingFieldPairBindingExpression(m_bindingExpression);
			}
			m_exprHostMapMemberID = context.ExprHostBuilder.MapBindingFieldPairEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapBindingFieldPair mapBindingFieldPair = (MapBindingFieldPair)MemberwiseClone();
			mapBindingFieldPair.m_map = context.CurrentMapClone;
			if (m_mapVectorLayer != null)
			{
				mapBindingFieldPair.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			}
			if (m_fieldName != null)
			{
				mapBindingFieldPair.m_fieldName = (ExpressionInfo)m_fieldName.PublishClone(context);
			}
			if (m_bindingExpression != null)
			{
				mapBindingFieldPair.m_bindingExpression = (ExpressionInfo)m_bindingExpression.PublishClone(context);
			}
			return mapBindingFieldPair;
		}

		internal void SetExprHost(MapBindingFieldPairExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void SetExprHostMapMember(MapBindingFieldPairExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHostMapMember = exprHost;
			m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FieldName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BindingExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostMapMemberID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.MapVectorLayer:
					writer.WriteReference(m_mapVectorLayer);
					break;
				case MemberName.FieldName:
					writer.Write(m_fieldName);
					break;
				case MemberName.BindingExpression:
					writer.Write(m_bindingExpression);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ExprHostMapMemberID:
					writer.Write(m_exprHostMapMemberID);
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
				case MemberName.MapVectorLayer:
					m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.FieldName:
					m_fieldName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BindingExpression:
					m_bindingExpression = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ExprHostMapMemberID:
					m_exprHostMapMemberID = reader.ReadInt32();
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
				case MemberName.Map:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
					break;
				case MemberName.MapVectorLayer:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair;
		}

		internal string EvaluateFieldName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBindingFieldPairFieldNameExpression(this, m_map.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateBindingExpression(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBindingFieldPairBindingExpressionExpression(this, m_map.Name);
		}
	}
}
