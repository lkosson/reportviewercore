namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixMemberCollection : DataRegionMemberCollection<TablixMember>
	{
		public override string DefinitionPath
		{
			get
			{
				if (m_parentDefinitionPath is TablixMember)
				{
					return m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal Tablix OwnerTablix => m_owner as Tablix;

		internal virtual double SizeDelta => 0.0;

		internal TablixMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}
	}
}
