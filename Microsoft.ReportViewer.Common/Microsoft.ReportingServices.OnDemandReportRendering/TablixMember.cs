using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixMember : DataRegionMember
	{
		protected TablixMemberCollection m_children;

		protected Visibility m_visibility;

		protected TablixMemberInstance m_instance;

		protected TablixHeader m_header;

		public TablixMember Parent => m_parent as TablixMember;

		public abstract string DataElementName
		{
			get;
		}

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract TablixHeader TablixHeader
		{
			get;
		}

		public abstract TablixMemberCollection Children
		{
			get;
		}

		public abstract bool FixedData
		{
			get;
		}

		public abstract KeepWithGroup KeepWithGroup
		{
			get;
		}

		public abstract bool RepeatOnNewPage
		{
			get;
		}

		public virtual bool KeepTogether => false;

		public abstract bool IsColumn
		{
			get;
		}

		internal abstract int RowSpan
		{
			get;
		}

		internal abstract int ColSpan
		{
			get;
		}

		public abstract bool IsTotal
		{
			get;
		}

		internal abstract PageBreakLocation PropagatedGroupBreak
		{
			get;
		}

		public abstract Visibility Visibility
		{
			get;
		}

		public abstract bool HideIfNoRows
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember MemberDefinition
		{
			get;
		}

		internal override ReportHierarchyNode DataRegionMemberDefinition => MemberDefinition;

		internal Tablix OwnerTablix => m_owner as Tablix;

		public abstract TablixMemberInstance Instance
		{
			get;
		}

		internal override IDataRegionMemberCollection SubMembers => m_children;

		internal TablixMember(IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, int parentCollectionIndex)
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
			if (m_header != null)
			{
				m_header.SetNewContext();
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			MemberDefinition?.ResetVisibilityComputationCache();
		}
	}
}
