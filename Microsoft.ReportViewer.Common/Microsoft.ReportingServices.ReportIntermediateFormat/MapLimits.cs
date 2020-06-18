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
	internal sealed class MapLimits : IPersistable
	{
		[NonSerialized]
		private MapLimitsExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_minimumX;

		private ExpressionInfo m_minimumY;

		private ExpressionInfo m_maximumX;

		private ExpressionInfo m_maximumY;

		private ExpressionInfo m_limitToData;

		internal ExpressionInfo MinimumX
		{
			get
			{
				return m_minimumX;
			}
			set
			{
				m_minimumX = value;
			}
		}

		internal ExpressionInfo MinimumY
		{
			get
			{
				return m_minimumY;
			}
			set
			{
				m_minimumY = value;
			}
		}

		internal ExpressionInfo MaximumX
		{
			get
			{
				return m_maximumX;
			}
			set
			{
				m_maximumX = value;
			}
		}

		internal ExpressionInfo MaximumY
		{
			get
			{
				return m_maximumY;
			}
			set
			{
				m_maximumY = value;
			}
		}

		internal ExpressionInfo LimitToData
		{
			get
			{
				return m_limitToData;
			}
			set
			{
				m_limitToData = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapLimitsExprHost ExprHost => m_exprHost;

		internal MapLimits()
		{
		}

		internal MapLimits(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLimitsStart();
			if (m_minimumX != null)
			{
				m_minimumX.Initialize("MinimumX", context);
				context.ExprHostBuilder.MapLimitsMinimumX(m_minimumX);
			}
			if (m_minimumY != null)
			{
				m_minimumY.Initialize("MinimumY", context);
				context.ExprHostBuilder.MapLimitsMinimumY(m_minimumY);
			}
			if (m_maximumX != null)
			{
				m_maximumX.Initialize("MaximumX", context);
				context.ExprHostBuilder.MapLimitsMaximumX(m_maximumX);
			}
			if (m_maximumY != null)
			{
				m_maximumY.Initialize("MaximumY", context);
				context.ExprHostBuilder.MapLimitsMaximumY(m_maximumY);
			}
			if (m_limitToData != null)
			{
				m_limitToData.Initialize("LimitToData", context);
				context.ExprHostBuilder.MapLimitsLimitToData(m_limitToData);
			}
			context.ExprHostBuilder.MapLimitsEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapLimits mapLimits = (MapLimits)MemberwiseClone();
			mapLimits.m_map = context.CurrentMapClone;
			if (m_minimumX != null)
			{
				mapLimits.m_minimumX = (ExpressionInfo)m_minimumX.PublishClone(context);
			}
			if (m_minimumY != null)
			{
				mapLimits.m_minimumY = (ExpressionInfo)m_minimumY.PublishClone(context);
			}
			if (m_maximumX != null)
			{
				mapLimits.m_maximumX = (ExpressionInfo)m_maximumX.PublishClone(context);
			}
			if (m_maximumY != null)
			{
				mapLimits.m_maximumY = (ExpressionInfo)m_maximumY.PublishClone(context);
			}
			if (m_limitToData != null)
			{
				mapLimits.m_limitToData = (ExpressionInfo)m_limitToData.PublishClone(context);
			}
			return mapLimits;
		}

		internal void SetExprHost(MapLimitsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MinimumX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LimitToData, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLimits, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.MinimumX:
					writer.Write(m_minimumX);
					break;
				case MemberName.MinimumY:
					writer.Write(m_minimumY);
					break;
				case MemberName.MaximumX:
					writer.Write(m_maximumX);
					break;
				case MemberName.MaximumY:
					writer.Write(m_maximumY);
					break;
				case MemberName.LimitToData:
					writer.Write(m_limitToData);
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
				case MemberName.MinimumX:
					m_minimumX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumY:
					m_minimumY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumX:
					m_maximumX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumY:
					m_maximumY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LimitToData:
					m_limitToData = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLimits;
		}

		internal double EvaluateMinimumX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMinimumXExpression(this, m_map.Name);
		}

		internal double EvaluateMinimumY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMinimumYExpression(this, m_map.Name);
		}

		internal double EvaluateMaximumX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMaximumXExpression(this, m_map.Name);
		}

		internal double EvaluateMaximumY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsMaximumYExpression(this, m_map.Name);
		}

		internal bool EvaluateLimitToData(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLimitsLimitToDataExpression(this, m_map.Name);
		}
	}
}
