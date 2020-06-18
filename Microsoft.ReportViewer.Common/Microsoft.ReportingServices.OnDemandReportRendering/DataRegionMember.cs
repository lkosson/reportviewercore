using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegionMember : IDefinitionPath, IReportScope
	{
		protected IDefinitionPath m_parentDefinitionPath;

		protected int m_parentCollectionIndex;

		protected string m_definitionPath;

		protected ReportItem m_owner;

		protected Group m_group;

		protected DataRegionMember m_parent;

		protected CustomPropertyCollection m_customPropertyCollection;

		internal abstract string UniqueName
		{
			get;
		}

		public abstract string ID
		{
			get;
		}

		public string DefinitionPath
		{
			get
			{
				if (m_definitionPath == null)
				{
					m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(m_parentDefinitionPath, m_parentCollectionIndex);
				}
				return m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath => m_parentDefinitionPath;

		public Group Group => m_group;

		public abstract bool IsStatic
		{
			get;
		}

		public virtual CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customPropertyCollection == null)
				{
					m_customPropertyCollection = new CustomPropertyCollection();
				}
				return m_customPropertyCollection;
			}
		}

		public abstract int MemberCellIndex
		{
			get;
		}

		internal abstract IReportScope ReportScope
		{
			get;
		}

		IReportScopeInstance IReportScope.ReportScopeInstance => ReportScopeInstance;

		internal abstract IReportScopeInstance ReportScopeInstance
		{
			get;
		}

		IRIFReportScope IReportScope.RIFReportScope => RIFReportScope;

		internal abstract IRIFReportScope RIFReportScope
		{
			get;
		}

		internal IDataRegion OwnerDataRegion => (IDataRegion)m_owner;

		internal abstract IDataRegionMemberCollection SubMembers
		{
			get;
		}

		internal abstract ReportHierarchyNode DataRegionMemberDefinition
		{
			get;
		}

		internal DataRegionMember(IDefinitionPath parentDefinitionPath, ReportItem owner, DataRegionMember parent, int parentCollectionIndex)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_owner = owner;
			m_parent = parent;
			m_parentCollectionIndex = parentCollectionIndex;
		}

		internal abstract bool GetIsColumn();

		internal virtual void ResetContext()
		{
			if (m_group != null)
			{
				m_group.SetNewContext();
			}
		}

		internal virtual void SetNewContext(bool fromMoveNext)
		{
			if (m_group != null)
			{
				m_group.SetNewContext();
			}
			if (IsStatic || SubMembers == null || fromMoveNext)
			{
				if (SubMembers != null)
				{
					SubMembers.SetNewContext();
				}
				else
				{
					SetCellsNewContext();
				}
			}
			if (!fromMoveNext && DataRegionMemberDefinition != null)
			{
				DataRegionMemberDefinition.ClearStreamingScopeInstanceBinding();
			}
		}

		private void SetCellsNewContext()
		{
			if (!OwnerDataRegion.HasDataCells)
			{
				return;
			}
			IDataRegionRowCollection rowCollection = OwnerDataRegion.RowCollection;
			if (GetIsColumn())
			{
				for (int i = 0; i < rowCollection.Count; i++)
				{
					rowCollection.GetIfExists(i)?.GetIfExists(MemberCellIndex)?.SetNewContext();
				}
				return;
			}
			IDataRegionRow ifExists = rowCollection.GetIfExists(MemberCellIndex);
			if (ifExists != null)
			{
				for (int j = 0; j < ifExists.Count; j++)
				{
					ifExists.GetIfExists(j)?.SetNewContext();
				}
			}
		}

		internal virtual InternalDynamicMemberLogic BuildOdpMemberLogic(OnDemandProcessingContext odpContext)
		{
			if (odpContext.StreamingMode)
			{
				return new InternalStreamingOdpDynamicMemberLogic(this, odpContext);
			}
			return new InternalFullOdpDynamicMemberLogic(this, odpContext);
		}
	}
}
