namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MemberHierarchy<T> : IDefinitionPath
	{
		protected DataRegionMemberCollection<T> m_members;

		protected bool m_isColumn;

		protected ReportItem m_owner;

		protected string m_definitionPath;

		public string DefinitionPath
		{
			get
			{
				if (m_definitionPath == null)
				{
					m_definitionPath = DefinitionPathConstants.GetTablixHierarchyDefinitionPath(m_owner, m_isColumn);
				}
				return m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath => m_owner;

		internal MemberHierarchy(ReportItem owner, bool isColumn)
		{
			m_owner = owner;
			m_isColumn = isColumn;
		}

		internal void SetNewContext()
		{
			if (m_members != null)
			{
				((IDataRegionMemberCollection)m_members).SetNewContext();
			}
		}

		internal abstract void ResetContext();
	}
}
