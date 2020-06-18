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
	internal sealed class MapDataRegion : DataRegion, IPersistable
	{
		[NonSerialized]
		private MapMemberList m_columnMembers;

		[NonSerialized]
		private MapMemberList m_rowMembers;

		[NonSerialized]
		private MapMember m_innerMostMapMember;

		[NonSerialized]
		private MapRowList m_rows;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private MapDataRegionExprHost m_exprHost;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.MapDataRegion;

		internal override HierarchyNodeList ColumnMembers => m_columnMembers;

		internal override HierarchyNodeList RowMembers => m_rowMembers;

		internal override RowList Rows => m_rows;

		internal MapMember MapMember
		{
			get
			{
				if (m_columnMembers != null && m_columnMembers.Count == 1)
				{
					return m_columnMembers[0];
				}
				return null;
			}
			set
			{
				if (m_columnMembers == null)
				{
					m_columnMembers = new MapMemberList();
				}
				else
				{
					m_columnMembers.Clear();
				}
				m_innerMostMapMember = null;
				m_columnMembers.Add(value);
			}
		}

		internal MapMember InnerMostMapMember
		{
			get
			{
				if (m_innerMostMapMember == null)
				{
					m_innerMostMapMember = MapMember;
					while (m_innerMostMapMember.ChildMapMember != null)
					{
						m_innerMostMapMember = m_innerMostMapMember.ChildMapMember;
					}
				}
				return m_innerMostMapMember;
			}
		}

		internal MapMember MapRowMember
		{
			get
			{
				if (m_rowMembers != null && m_rowMembers.Count == 1)
				{
					return m_rowMembers[0];
				}
				return null;
			}
			set
			{
				if (m_rowMembers == null)
				{
					m_rowMembers = new MapMemberList();
				}
				else
				{
					m_rowMembers.Clear();
				}
				m_rowMembers.Add(value);
			}
		}

		internal MapRow MapRow
		{
			get
			{
				if (m_rows != null && m_rows.Count == 1)
				{
					return m_rows[0];
				}
				return null;
			}
			set
			{
				if (m_rows == null)
				{
					m_rows = new MapRowList();
				}
				else
				{
					m_rows.Clear();
				}
				m_rows.Add(value);
			}
		}

		internal MapDataRegionExprHost MapDataRegionExprHost => m_exprHost;

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (m_exprHost == null)
				{
					return null;
				}
				return m_exprHost.UserSortExpressionsHost;
			}
		}

		private Map Map => (Map)m_parent;

		internal MapDataRegion(ReportItem parent)
			: base(parent)
		{
		}

		internal MapDataRegion(int id, ReportItem parent)
			: base(id, parent)
		{
			base.RowCount = 1;
			base.ColumnCount = 1;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 && (context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ExprHostBuilder.DataRegionStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion, m_name);
				base.Initialize(context);
				base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if (MapRow != null)
			{
				MapRow.Initialize(context);
			}
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapDataRegion mapDataRegion = (MapDataRegion)(context.CurrentDataRegionClone = (MapDataRegion)base.PublishClone(context));
			mapDataRegion.m_parent = context.CurrentMapClone;
			mapDataRegion.m_rows = new MapRowList();
			mapDataRegion.m_rowMembers = new MapMemberList();
			mapDataRegion.m_columnMembers = new MapMemberList();
			if (MapMember != null)
			{
				mapDataRegion.MapMember = (MapMember)MapMember.PublishClone(context, mapDataRegion);
			}
			if (MapRowMember != null)
			{
				mapDataRegion.MapRowMember = (MapMember)MapRowMember.PublishClone(context);
			}
			if (MapRow != null)
			{
				mapDataRegion.MapRow = (MapRow)MapRow.PublishClone(context);
			}
			return mapDataRegion;
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return null;
		}

		[SkipMemberStaticValidation(MemberName.MapMember)]
		[SkipMemberStaticValidation(MemberName.MapRowMember)]
		[SkipMemberStaticValidation(MemberName.MapRow)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember));
			list.Add(new MemberInfo(MemberName.MapRowMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember));
			list.Add(new MemberInfo(MemberName.MapRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapRow));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		internal List<MapVectorLayer> GetChildVectorLayers()
		{
			List<MapVectorLayer> list = new List<MapVectorLayer>();
			if (Map.MapLayers != null)
			{
				foreach (MapLayer mapLayer in Map.MapLayers)
				{
					if (mapLayer is MapVectorLayer)
					{
						MapVectorLayer mapVectorLayer = (MapVectorLayer)mapLayer;
						if (string.Equals(mapVectorLayer.MapDataRegionName, base.Name, StringComparison.Ordinal))
						{
							list.Add(mapVectorLayer);
						}
					}
				}
				return list;
			}
			return list;
		}

		public override void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context)
		{
			Global.Tracer.Assert(condition: false, "CreateDomainScopeMember should not be called for MapDataRegion");
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapMember:
					writer.Write(MapMember);
					break;
				case MemberName.MapRowMember:
					writer.Write(MapRowMember);
					break;
				case MemberName.MapRow:
					writer.Write(MapRow);
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
				case MemberName.MapMember:
					MapMember = (MapMember)reader.ReadRIFObject();
					break;
				case MemberName.MapRowMember:
					MapRowMember = (MapMember)reader.ReadRIFObject();
					break;
				case MemberName.MapRow:
					MapRow = (MapRow)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.MapDataRegionHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_exprHost, m_exprHost.SortHost, m_exprHost.FilterHostsRemotable, m_exprHost.UserSortExpressionsHost, m_exprHost.PageBreakExprHost, m_exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
