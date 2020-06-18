using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataMember : DataRegionMember
	{
		protected DataMemberCollection m_children;

		protected DataMemberInstance m_instance;

		public DataMember Parent => m_parent as DataMember;

		public virtual DataMemberCollection Children => m_children;

		public abstract bool IsColumn
		{
			get;
		}

		public abstract int RowSpan
		{
			get;
		}

		public abstract int ColSpan
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.DataMember MemberDefinition
		{
			get;
		}

		internal override ReportHierarchyNode DataRegionMemberDefinition => MemberDefinition;

		internal CustomReportItem OwnerCri => m_owner as CustomReportItem;

		public abstract DataMemberInstance Instance
		{
			get;
		}

		internal override IDataRegionMemberCollection SubMembers => m_children;

		internal DataMember(IDefinitionPath parentDefinitionPath, CustomReportItem owner, DataMember parent, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
		}

		internal override bool GetIsColumn()
		{
			return IsColumn;
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			base.SetNewContext(fromMoveNext);
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
