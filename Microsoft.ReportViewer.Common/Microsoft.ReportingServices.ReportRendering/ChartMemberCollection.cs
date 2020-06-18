using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ChartMemberCollection
	{
		private Chart m_owner;

		private ChartHeading m_headingDef;

		private ChartHeadingInstanceList m_headingInstances;

		private ChartMember[] m_members;

		private ChartMember m_firstMember;

		private ChartMember m_parent;

		public ChartMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				ChartMember chartMember = null;
				if (index == 0)
				{
					chartMember = m_firstMember;
				}
				else if (m_members != null)
				{
					chartMember = m_members[index - 1];
				}
				if (chartMember == null)
				{
					ChartHeadingInstance headingInstance = null;
					if (m_headingInstances != null && index < m_headingInstances.Count)
					{
						headingInstance = m_headingInstances[index];
					}
					chartMember = new ChartMember(m_owner, m_parent, m_headingDef, headingInstance, index);
					if (m_owner.RenderingContext.CacheState)
					{
						if (index == 0)
						{
							m_firstMember = chartMember;
						}
						else
						{
							if (m_members == null)
							{
								m_members = new ChartMember[Count - 1];
							}
							m_members[index - 1] = chartMember;
						}
					}
				}
				return chartMember;
			}
		}

		public int Count
		{
			get
			{
				if (m_headingInstances == null || m_headingInstances.Count == 0)
				{
					if (m_headingDef.Grouping == null && m_headingDef.Labels != null)
					{
						return m_headingDef.Labels.Count;
					}
					return 1;
				}
				return m_headingInstances.Count;
			}
		}

		internal ChartMemberCollection(Chart owner, ChartMember parent, ChartHeading headingDef, ChartHeadingInstanceList headingInstances)
		{
			m_owner = owner;
			m_parent = parent;
			m_headingInstances = headingInstances;
			m_headingDef = headingDef;
		}
	}
}
