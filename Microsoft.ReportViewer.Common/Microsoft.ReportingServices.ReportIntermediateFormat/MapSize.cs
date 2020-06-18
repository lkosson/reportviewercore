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
	internal sealed class MapSize : IPersistable
	{
		[NonSerialized]
		private MapSizeExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_width;

		private ExpressionInfo m_height;

		private ExpressionInfo m_unit;

		internal ExpressionInfo Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal ExpressionInfo Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
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

		internal MapSizeExprHost ExprHost => m_exprHost;

		internal MapSize()
		{
		}

		internal MapSize(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapSizeStart();
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.MapSizeWidth(m_width);
			}
			if (m_height != null)
			{
				m_height.Initialize("Height", context);
				context.ExprHostBuilder.MapSizeHeight(m_height);
			}
			if (m_unit != null)
			{
				m_unit.Initialize("Unit", context);
				context.ExprHostBuilder.MapSizeUnit(m_unit);
			}
			context.ExprHostBuilder.MapSizeEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapSize mapSize = (MapSize)MemberwiseClone();
			mapSize.m_map = context.CurrentMapClone;
			if (m_width != null)
			{
				mapSize.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			if (m_height != null)
			{
				mapSize.m_height = (ExpressionInfo)m_height.PublishClone(context);
			}
			if (m_unit != null)
			{
				mapSize.m_unit = (ExpressionInfo)m_unit.PublishClone(context);
			}
			return mapSize;
		}

		internal void SetExprHost(MapSizeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Height, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Unit, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.Height:
					writer.Write(m_height);
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
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Height:
					m_height = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSize;
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSizeWidthExpression(this, m_map.Name);
		}

		internal double EvaluateHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSizeHeightExpression(this, m_map.Name);
		}

		internal Unit EvaluateUnit(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateUnit(context.ReportRuntime.EvaluateMapSizeUnitExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
