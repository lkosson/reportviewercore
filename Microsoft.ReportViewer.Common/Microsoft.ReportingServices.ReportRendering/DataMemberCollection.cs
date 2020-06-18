using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataMemberCollection
	{
		private CustomReportItem m_owner;

		private CustomReportItemHeading m_headingDef;

		private CustomReportItemHeadingInstanceList m_headingInstances;

		private DataMember[] m_members;

		private DataMember m_firstMember;

		private DataMember m_parent;

		private bool m_isSubtotal;

		public DataMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				DataMember dataMember = null;
				if (index == 0)
				{
					dataMember = m_firstMember;
				}
				else if (m_members != null)
				{
					dataMember = m_members[index - 1];
				}
				if (dataMember == null)
				{
					CustomReportItemHeadingInstance headingInstance = null;
					if (m_headingInstances != null && index < m_headingInstances.Count)
					{
						headingInstance = m_headingInstances[index];
					}
					dataMember = new DataMember(m_owner, m_parent, m_headingDef, headingInstance, m_isSubtotal, index);
					if (m_owner.UseCache)
					{
						if (index == 0)
						{
							m_firstMember = dataMember;
						}
						else
						{
							if (m_members == null)
							{
								m_members = new DataMember[Count - 1];
							}
							m_members[index - 1] = dataMember;
						}
					}
				}
				return dataMember;
			}
		}

		public int Count
		{
			get
			{
				if (m_headingInstances == null || m_headingInstances.Count == 0)
				{
					return 1;
				}
				return m_headingInstances.Count;
			}
		}

		internal DataMemberCollection(CustomReportItem owner, DataMember parent, CustomReportItemHeading headingDef, bool headingDefIsStaticSubtotal, CustomReportItemHeadingInstanceList headingInstances)
		{
			m_owner = owner;
			m_parent = parent;
			m_headingInstances = headingInstances;
			m_isSubtotal = headingDefIsStaticSubtotal;
			m_headingDef = headingDef;
		}
	}
}
