namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartMemberCollection : DataRegionMemberCollection<ChartMember>
	{
		public override string DefinitionPath
		{
			get
			{
				if (m_parentDefinitionPath is ChartMember)
				{
					return m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal Chart OwnerChart => m_owner as Chart;

		internal ChartMemberCollection(IDefinitionPath parentDefinitionPath, Chart owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}
	}
}
