namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataMemberCollection : DataRegionMemberCollection<DataMember>
	{
		public override string DefinitionPath
		{
			get
			{
				if (m_parentDefinitionPath is DataMember)
				{
					return m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal CustomReportItem OwnerCri => m_owner as CustomReportItem;

		internal DataMemberCollection(IDefinitionPath parentDefinitionPath, CustomReportItem owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}
	}
}
