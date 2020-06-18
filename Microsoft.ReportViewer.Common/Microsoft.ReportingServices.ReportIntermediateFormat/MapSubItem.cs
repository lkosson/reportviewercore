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
	internal class MapSubItem : MapStyleContainer, IPersistable
	{
		protected int m_exprHostID = -1;

		[NonSerialized]
		protected MapSubItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private MapLocation m_mapLocation;

		private MapSize m_mapSize;

		private ExpressionInfo m_leftMargin;

		private ExpressionInfo m_rightMargin;

		private ExpressionInfo m_topMargin;

		private ExpressionInfo m_bottomMargin;

		private ExpressionInfo m_zIndex;

		internal MapLocation MapLocation
		{
			get
			{
				return m_mapLocation;
			}
			set
			{
				m_mapLocation = value;
			}
		}

		internal MapSize MapSize
		{
			get
			{
				return m_mapSize;
			}
			set
			{
				m_mapSize = value;
			}
		}

		internal ExpressionInfo LeftMargin
		{
			get
			{
				return m_leftMargin;
			}
			set
			{
				m_leftMargin = value;
			}
		}

		internal ExpressionInfo RightMargin
		{
			get
			{
				return m_rightMargin;
			}
			set
			{
				m_rightMargin = value;
			}
		}

		internal ExpressionInfo TopMargin
		{
			get
			{
				return m_topMargin;
			}
			set
			{
				m_topMargin = value;
			}
		}

		internal ExpressionInfo BottomMargin
		{
			get
			{
				return m_bottomMargin;
			}
			set
			{
				m_bottomMargin = value;
			}
		}

		internal ExpressionInfo ZIndex
		{
			get
			{
				return m_zIndex;
			}
			set
			{
				m_zIndex = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapSubItemExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapSubItem()
		{
		}

		internal MapSubItem(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_mapLocation != null)
			{
				m_mapLocation.Initialize(context);
			}
			if (m_mapSize != null)
			{
				m_mapSize.Initialize(context);
			}
			if (m_leftMargin != null)
			{
				m_leftMargin.Initialize("LeftMargin", context);
				context.ExprHostBuilder.MapSubItemLeftMargin(m_leftMargin);
			}
			if (m_rightMargin != null)
			{
				m_rightMargin.Initialize("RightMargin", context);
				context.ExprHostBuilder.MapSubItemRightMargin(m_rightMargin);
			}
			if (m_topMargin != null)
			{
				m_topMargin.Initialize("TopMargin", context);
				context.ExprHostBuilder.MapSubItemTopMargin(m_topMargin);
			}
			if (m_bottomMargin != null)
			{
				m_bottomMargin.Initialize("BottomMargin", context);
				context.ExprHostBuilder.MapSubItemBottomMargin(m_bottomMargin);
			}
			if (m_zIndex != null)
			{
				m_zIndex.Initialize("ZIndex", context);
				context.ExprHostBuilder.MapSubItemZIndex(m_zIndex);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSubItem mapSubItem = (MapSubItem)base.PublishClone(context);
			if (m_mapLocation != null)
			{
				mapSubItem.m_mapLocation = (MapLocation)m_mapLocation.PublishClone(context);
			}
			if (m_mapSize != null)
			{
				mapSubItem.m_mapSize = (MapSize)m_mapSize.PublishClone(context);
			}
			if (m_leftMargin != null)
			{
				mapSubItem.m_leftMargin = (ExpressionInfo)m_leftMargin.PublishClone(context);
			}
			if (m_rightMargin != null)
			{
				mapSubItem.m_rightMargin = (ExpressionInfo)m_rightMargin.PublishClone(context);
			}
			if (m_topMargin != null)
			{
				mapSubItem.m_topMargin = (ExpressionInfo)m_topMargin.PublishClone(context);
			}
			if (m_bottomMargin != null)
			{
				mapSubItem.m_bottomMargin = (ExpressionInfo)m_bottomMargin.PublishClone(context);
			}
			if (m_zIndex != null)
			{
				mapSubItem.m_zIndex = (ExpressionInfo)m_zIndex.PublishClone(context);
			}
			return mapSubItem;
		}

		internal void SetExprHost(MapSubItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_mapLocation != null && ExprHost.MapLocationHost != null)
			{
				m_mapLocation.SetExprHost(ExprHost.MapLocationHost, reportObjectModel);
			}
			if (m_mapSize != null && ExprHost.MapSizeHost != null)
			{
				m_mapSize.SetExprHost(ExprHost.MapSizeHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapLocation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLocation));
			list.Add(new MemberInfo(MemberName.MapSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSize));
			list.Add(new MemberInfo(MemberName.LeftMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BottomMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ZIndex, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapLocation:
					writer.Write(m_mapLocation);
					break;
				case MemberName.MapSize:
					writer.Write(m_mapSize);
					break;
				case MemberName.LeftMargin:
					writer.Write(m_leftMargin);
					break;
				case MemberName.RightMargin:
					writer.Write(m_rightMargin);
					break;
				case MemberName.TopMargin:
					writer.Write(m_topMargin);
					break;
				case MemberName.BottomMargin:
					writer.Write(m_bottomMargin);
					break;
				case MemberName.ZIndex:
					writer.Write(m_zIndex);
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

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapLocation:
					m_mapLocation = (MapLocation)reader.ReadRIFObject();
					break;
				case MemberName.MapSize:
					m_mapSize = (MapSize)reader.ReadRIFObject();
					break;
				case MemberName.LeftMargin:
					m_leftMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightMargin:
					m_rightMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopMargin:
					m_topMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BottomMargin:
					m_bottomMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ZIndex:
					m_zIndex = (ExpressionInfo)reader.ReadRIFObject();
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

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem;
		}

		internal string EvaluateLeftMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemLeftMarginExpression(this, m_map.Name);
		}

		internal string EvaluateRightMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemRightMarginExpression(this, m_map.Name);
		}

		internal string EvaluateTopMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemTopMarginExpression(this, m_map.Name);
		}

		internal string EvaluateBottomMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemBottomMarginExpression(this, m_map.Name);
		}

		internal int EvaluateZIndex(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemZIndexExpression(this, m_map.Name);
		}
	}
}
