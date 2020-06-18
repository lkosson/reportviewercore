using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartMember : DataRegionMember
	{
		protected ChartMemberCollection m_children;

		protected ChartMemberInstance m_instance;

		protected ReportStringProperty m_label;

		protected ChartSeries m_chartSeries;

		public ChartMember Parent => m_parent as ChartMember;

		public abstract ReportStringProperty Label
		{
			get;
		}

		public abstract string DataElementName
		{
			get;
		}

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract ChartMemberCollection Children
		{
			get;
		}

		public abstract bool IsCategory
		{
			get;
		}

		public abstract int SeriesSpan
		{
			get;
		}

		public abstract int CategorySpan
		{
			get;
		}

		public abstract bool IsTotal
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember MemberDefinition
		{
			get;
		}

		internal override ReportHierarchyNode DataRegionMemberDefinition => MemberDefinition;

		internal Chart OwnerChart => m_owner as Chart;

		public abstract ChartMemberInstance Instance
		{
			get;
		}

		internal override IDataRegionMemberCollection SubMembers => m_children;

		private ChartSeries ChartSeries
		{
			get
			{
				if (IsCategory || Children != null)
				{
					return null;
				}
				if (m_chartSeries == null)
				{
					m_chartSeries = ((Chart)m_owner).ChartData.SeriesCollection[MemberCellIndex];
				}
				return m_chartSeries;
			}
		}

		internal ChartMember(IDefinitionPath parentDefinitionPath, Chart owner, ChartMember parent, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
		}

		internal override bool GetIsColumn()
		{
			return IsCategory;
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			base.SetNewContext(fromMoveNext);
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			ChartSeries?.SetNewContext();
		}
	}
}
