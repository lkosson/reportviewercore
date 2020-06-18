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
	internal class MapLayer : IPersistable
	{
		protected int m_exprHostID = -1;

		[NonSerialized]
		protected MapLayerExprHost m_exprHost;

		[Reference]
		protected Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected string m_name;

		private ExpressionInfo m_visibilityMode;

		private ExpressionInfo m_minimumZoom;

		private ExpressionInfo m_maximumZoom;

		private ExpressionInfo m_transparency;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ExpressionInfo VisibilityMode
		{
			get
			{
				return m_visibilityMode;
			}
			set
			{
				m_visibilityMode = value;
			}
		}

		internal ExpressionInfo MinimumZoom
		{
			get
			{
				return m_minimumZoom;
			}
			set
			{
				m_minimumZoom = value;
			}
		}

		internal ExpressionInfo MaximumZoom
		{
			get
			{
				return m_maximumZoom;
			}
			set
			{
				m_maximumZoom = value;
			}
		}

		internal ExpressionInfo Transparency
		{
			get
			{
				return m_transparency;
			}
			set
			{
				m_transparency = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapLayerExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapLayer()
		{
		}

		internal MapLayer(Map map)
		{
			m_map = map;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_visibilityMode != null)
			{
				m_visibilityMode.Initialize("VisibilityMode", context);
				context.ExprHostBuilder.MapLayerVisibilityMode(m_visibilityMode);
			}
			if (m_minimumZoom != null)
			{
				m_minimumZoom.Initialize("MinimumZoom", context);
				context.ExprHostBuilder.MapLayerMinimumZoom(m_minimumZoom);
			}
			if (m_maximumZoom != null)
			{
				m_maximumZoom.Initialize("MaximumZoom", context);
				context.ExprHostBuilder.MapLayerMaximumZoom(m_maximumZoom);
			}
			if (m_transparency != null)
			{
				m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.MapLayerTransparency(m_transparency);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapLayer mapLayer = (MapLayer)MemberwiseClone();
			mapLayer.m_map = context.CurrentMapClone;
			if (m_visibilityMode != null)
			{
				mapLayer.m_visibilityMode = (ExpressionInfo)m_visibilityMode.PublishClone(context);
			}
			if (m_minimumZoom != null)
			{
				mapLayer.m_minimumZoom = (ExpressionInfo)m_minimumZoom.PublishClone(context);
			}
			if (m_maximumZoom != null)
			{
				mapLayer.m_maximumZoom = (ExpressionInfo)m_maximumZoom.PublishClone(context);
			}
			if (m_transparency != null)
			{
				mapLayer.m_transparency = (ExpressionInfo)m_transparency.PublishClone(context);
			}
			return mapLayer;
		}

		internal virtual void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.VisibilityMode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumZoom, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumZoom, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Transparency, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.VisibilityMode:
					writer.Write(m_visibilityMode);
					break;
				case MemberName.MinimumZoom:
					writer.Write(m_minimumZoom);
					break;
				case MemberName.MaximumZoom:
					writer.Write(m_maximumZoom);
					break;
				case MemberName.Transparency:
					writer.Write(m_transparency);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.VisibilityMode:
					m_visibilityMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumZoom:
					m_minimumZoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumZoom:
					m_maximumZoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Transparency:
					m_transparency = (ExpressionInfo)reader.ReadRIFObject();
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

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
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

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer;
		}

		internal MapVisibilityMode EvaluateVisibilityMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapVisibilityMode(context.ReportRuntime.EvaluateMapLayerVisibilityModeExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal double EvaluateMinimumZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLayerMinimumZoomExpression(this, m_map.Name);
		}

		internal double EvaluateMaximumZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLayerMaximumZoomExpression(this, m_map.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLayerTransparencyExpression(this, m_map.Name);
		}
	}
}
