namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegionMemberCollection<T> : ReportElementCollectionBase<T>, IDefinitionPath, IDataRegionMemberCollection
	{
		protected DataRegionMember[] m_children;

		protected IDefinitionPath m_parentDefinitionPath;

		protected ReportItem m_owner;

		public abstract string DefinitionPath
		{
			get;
		}

		public IDefinitionPath ParentDefinitionPath => m_parentDefinitionPath;

		internal DataRegionMemberCollection(IDefinitionPath parentDefinitionPath, ReportItem owner)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_owner = owner;
		}

		void IDataRegionMemberCollection.SetNewContext()
		{
			if (m_children == null)
			{
				return;
			}
			for (int i = 0; i < Count; i++)
			{
				if (m_children[i] != null)
				{
					m_children[i].SetNewContext(fromMoveNext: false);
				}
			}
		}
	}
}
