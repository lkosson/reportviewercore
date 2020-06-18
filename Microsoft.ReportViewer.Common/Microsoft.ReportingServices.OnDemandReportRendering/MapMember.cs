using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMember : DataRegionMember
	{
		private MapMemberCollection m_children;

		private MapMemberInstance m_instance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMember m_memberDef;

		private IReportScope m_reportScope;

		public MapMember Parent => m_parent as MapMember;

		internal override string UniqueName => m_memberDef.UniqueName;

		public override string ID => m_memberDef.RenderingModelID;

		public override bool IsStatic
		{
			get
			{
				if (m_memberDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public override int MemberCellIndex => m_memberDef.MemberCellIndex;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapMember MemberDefinition => m_memberDef;

		internal override ReportHierarchyNode DataRegionMemberDefinition => MemberDefinition;

		internal override IReportScope ReportScope
		{
			get
			{
				if (IsStatic)
				{
					return m_reportScope;
				}
				return this;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				if (IsStatic)
				{
					return m_reportScope.RIFReportScope;
				}
				return MemberDefinition;
			}
		}

		internal override IReportScopeInstance ReportScopeInstance
		{
			get
			{
				if (IsStatic)
				{
					return m_reportScope.ReportScopeInstance;
				}
				return (IReportScopeInstance)Instance;
			}
		}

		internal MapDataRegion OwnerMapDataRegion => m_owner as MapDataRegion;

		public MapMemberInstance Instance
		{
			get
			{
				if (OwnerMapDataRegion.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsStatic)
					{
						m_instance = new MapMemberInstance(OwnerMapDataRegion, this);
					}
					else
					{
						MapDynamicMemberInstance instance = new MapDynamicMemberInstance(OwnerMapDataRegion, this, BuildOdpMemberLogic(OwnerMapDataRegion.RenderingContext.OdpContext));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		public MapMember ChildMapMember
		{
			get
			{
				if (m_children != null && m_children.Count == 1)
				{
					return m_children[0];
				}
				return null;
			}
		}

		internal override IDataRegionMemberCollection SubMembers
		{
			get
			{
				if (m_children == null && m_memberDef.InnerHierarchy != null)
				{
					MapMemberList mapMemberList = (MapMemberList)m_memberDef.InnerHierarchy;
					if (mapMemberList == null)
					{
						return null;
					}
					m_children = new MapMemberCollection(this, OwnerMapDataRegion, this, mapMemberList);
				}
				return m_children;
			}
		}

		internal MapMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, MapDataRegion owner, MapMember parent, Microsoft.ReportingServices.ReportIntermediateFormat.MapMember memberDef)
			: base(parentDefinitionPath, owner, parent, 0)
		{
			m_memberDef = memberDef;
			if (m_memberDef.IsStatic)
			{
				m_reportScope = reportScope;
			}
			if (m_memberDef.Grouping != null)
			{
				m_group = new Group(owner, m_memberDef, this);
			}
		}

		internal MapMember(IDefinitionPath parentDefinitionPath, MapDataRegion owner, MapMember parent)
			: base(parentDefinitionPath, owner, parent, 0)
		{
		}

		internal override bool GetIsColumn()
		{
			return m_memberDef.IsColumn;
		}

		private List<MapVectorLayer> GetChildLayers()
		{
			return ((MapDataRegion)m_owner).GetChildLayers();
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && m_instance != null && !IsStatic)
			{
				((IDynamicInstance)m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			if (ChildMapMember == null)
			{
				foreach (MapVectorLayer childLayer in GetChildLayers())
				{
					childLayer.SetNewContext();
				}
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
