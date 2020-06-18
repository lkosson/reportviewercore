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
	internal sealed class MapBucket : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapBucketExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_startValue;

		private ExpressionInfo m_endValue;

		internal ExpressionInfo StartValue
		{
			get
			{
				return m_startValue;
			}
			set
			{
				m_startValue = value;
			}
		}

		internal ExpressionInfo EndValue
		{
			get
			{
				return m_endValue;
			}
			set
			{
				m_endValue = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapBucketExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapBucket()
		{
		}

		internal MapBucket(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapBucketStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (m_startValue != null)
			{
				m_startValue.Initialize("StartValue", context);
				context.ExprHostBuilder.MapBucketStartValue(m_startValue);
			}
			if (m_endValue != null)
			{
				m_endValue.Initialize("EndValue", context);
				context.ExprHostBuilder.MapBucketEndValue(m_endValue);
			}
			m_exprHostID = context.ExprHostBuilder.MapBucketEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapBucket mapBucket = (MapBucket)MemberwiseClone();
			mapBucket.m_map = context.CurrentMapClone;
			if (m_startValue != null)
			{
				mapBucket.m_startValue = (ExpressionInfo)m_startValue.PublishClone(context);
			}
			if (m_endValue != null)
			{
				mapBucket.m_endValue = (ExpressionInfo)m_endValue.PublishClone(context);
			}
			return mapBucket;
		}

		internal void SetExprHost(MapBucketExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBucket, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.StartValue:
					writer.Write(m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(m_endValue);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
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
				case MemberName.StartValue:
					m_startValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					m_endValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBucket;
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateStartValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBucketStartValueExpression(this, m_map.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateEndValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBucketEndValueExpression(this, m_map.Name);
		}
	}
}
