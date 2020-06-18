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

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class MapAppearanceRule : IPersistable
	{
		[NonSerialized]
		protected MapAppearanceRuleExprHost m_exprHost;

		[NonSerialized]
		protected MapAppearanceRuleExprHost m_exprHostMapMember;

		[Reference]
		protected Map m_map;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_dataValue;

		private ExpressionInfo m_distributionType;

		private ExpressionInfo m_bucketCount;

		private ExpressionInfo m_startValue;

		private ExpressionInfo m_endValue;

		private List<MapBucket> m_mapBuckets;

		private string m_legendName;

		private ExpressionInfo m_legendText;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal ExpressionInfo DataValue
		{
			get
			{
				return m_dataValue;
			}
			set
			{
				m_dataValue = value;
			}
		}

		internal ExpressionInfo DistributionType
		{
			get
			{
				return m_distributionType;
			}
			set
			{
				m_distributionType = value;
			}
		}

		internal ExpressionInfo BucketCount
		{
			get
			{
				return m_bucketCount;
			}
			set
			{
				m_bucketCount = value;
			}
		}

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

		internal List<MapBucket> MapBuckets
		{
			get
			{
				return m_mapBuckets;
			}
			set
			{
				m_mapBuckets = value;
			}
		}

		internal string LegendName
		{
			get
			{
				return m_legendName;
			}
			set
			{
				m_legendName = value;
			}
		}

		internal ExpressionInfo LegendText
		{
			get
			{
				return m_legendText;
			}
			set
			{
				m_legendText = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapAppearanceRuleExprHost ExprHost => m_exprHost;

		internal MapAppearanceRuleExprHost ExprHostMapMember => m_exprHostMapMember;

		internal MapAppearanceRule()
		{
		}

		internal MapAppearanceRule(MapVectorLayer mapVectorLayer, Map map)
		{
			m_map = map;
			m_mapVectorLayer = mapVectorLayer;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_distributionType != null)
			{
				m_distributionType.Initialize("DistributionType", context);
				context.ExprHostBuilder.MapAppearanceRuleDistributionType(m_distributionType);
			}
			if (m_bucketCount != null)
			{
				m_bucketCount.Initialize("BucketCount", context);
				context.ExprHostBuilder.MapAppearanceRuleBucketCount(m_bucketCount);
			}
			if (m_startValue != null)
			{
				m_startValue.Initialize("StartValue", context);
				context.ExprHostBuilder.MapAppearanceRuleStartValue(m_startValue);
			}
			if (m_endValue != null)
			{
				m_endValue.Initialize("EndValue", context);
				context.ExprHostBuilder.MapAppearanceRuleEndValue(m_endValue);
			}
			if (m_mapBuckets != null)
			{
				for (int i = 0; i < m_mapBuckets.Count; i++)
				{
					m_mapBuckets[i].Initialize(context, i);
				}
			}
			if (m_legendText != null)
			{
				m_legendText.Initialize("LegendText", context);
				context.ExprHostBuilder.MapAppearanceRuleLegendText(m_legendText);
			}
			if (m_mapVectorLayer.MapDataRegionName == null && m_dataValue != null)
			{
				m_dataValue.Initialize("DataValue", context);
				context.ExprHostBuilder.MapAppearanceRuleDataValue(m_dataValue);
			}
		}

		internal virtual void InitializeMapMember(InitializationContext context)
		{
			if (m_mapVectorLayer.MapDataRegionName != null && m_dataValue != null)
			{
				m_dataValue.Initialize("DataValue", context);
				context.ExprHostBuilder.MapAppearanceRuleDataValue(m_dataValue);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapAppearanceRule mapAppearanceRule = (MapAppearanceRule)MemberwiseClone();
			mapAppearanceRule.m_map = context.CurrentMapClone;
			mapAppearanceRule.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			if (m_dataValue != null)
			{
				mapAppearanceRule.m_dataValue = (ExpressionInfo)m_dataValue.PublishClone(context);
			}
			if (m_distributionType != null)
			{
				mapAppearanceRule.m_distributionType = (ExpressionInfo)m_distributionType.PublishClone(context);
			}
			if (m_bucketCount != null)
			{
				mapAppearanceRule.m_bucketCount = (ExpressionInfo)m_bucketCount.PublishClone(context);
			}
			if (m_startValue != null)
			{
				mapAppearanceRule.m_startValue = (ExpressionInfo)m_startValue.PublishClone(context);
			}
			if (m_endValue != null)
			{
				mapAppearanceRule.m_endValue = (ExpressionInfo)m_endValue.PublishClone(context);
			}
			if (m_mapBuckets != null)
			{
				mapAppearanceRule.m_mapBuckets = new List<MapBucket>(m_mapBuckets.Count);
				foreach (MapBucket mapBucket in m_mapBuckets)
				{
					mapAppearanceRule.m_mapBuckets.Add((MapBucket)mapBucket.PublishClone(context));
				}
			}
			if (m_legendText != null)
			{
				mapAppearanceRule.m_legendText = (ExpressionInfo)m_legendText.PublishClone(context);
			}
			return mapAppearanceRule;
		}

		internal virtual void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			IList<MapBucketExprHost> mapBucketsHostsRemotable = ExprHost.MapBucketsHostsRemotable;
			if (m_mapBuckets == null || mapBucketsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapBuckets.Count; i++)
			{
				MapBucket mapBucket = m_mapBuckets[i];
				if (mapBucket != null && mapBucket.ExpressionHostID > -1)
				{
					mapBucket.SetExprHost(mapBucketsHostsRemotable[mapBucket.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal virtual void SetExprHostMapMember(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHostMapMember = exprHost;
			m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistributionType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BucketCount, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StartValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapBuckets, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBucket));
			list.Add(new MemberInfo(MemberName.LegendName, Token.String));
			list.Add(new MemberInfo(MemberName.LegendText, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
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
				case MemberName.DataValue:
					writer.Write(m_dataValue);
					break;
				case MemberName.DistributionType:
					writer.Write(m_distributionType);
					break;
				case MemberName.BucketCount:
					writer.Write(m_bucketCount);
					break;
				case MemberName.StartValue:
					writer.Write(m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(m_endValue);
					break;
				case MemberName.MapBuckets:
					writer.Write(m_mapBuckets);
					break;
				case MemberName.LegendName:
					writer.Write(m_legendName);
					break;
				case MemberName.LegendText:
					writer.Write(m_legendText);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
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
				case MemberName.DataValue:
					m_dataValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistributionType:
					m_distributionType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BucketCount:
					m_bucketCount = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StartValue:
					m_startValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					m_endValue = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapBuckets:
					m_mapBuckets = reader.ReadGenericListOfRIFObjects<MapBucket>();
					break;
				case MemberName.LegendName:
					m_legendName = reader.ReadString();
					break;
				case MemberName.LegendText:
					m_legendText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
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

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule;
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateDataValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_mapVectorLayer.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleDataValueExpression(this, m_map.Name);
		}

		internal MapRuleDistributionType EvaluateDistributionType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapRuleDistributionType(context.ReportRuntime.EvaluateMapAppearanceRuleDistributionTypeExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal int EvaluateBucketCount(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleBucketCountExpression(this, m_map.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateStartValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleStartValueExpression(this, m_map.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateEndValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapAppearanceRuleEndValueExpression(this, m_map.Name);
		}

		internal string EvaluateLegendText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapAppearanceRuleLegendTextExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}
	}
}
