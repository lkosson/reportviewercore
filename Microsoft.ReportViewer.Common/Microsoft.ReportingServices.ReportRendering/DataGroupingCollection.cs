using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataGroupingCollection
	{
		private CustomReportItem m_owner;

		private CustomReportItemHeadingList m_headingDef;

		private CustomReportItemHeadingInstanceList m_headingInstances;

		private DataMemberCollection[] m_collections;

		private DataMemberCollection m_firstCollection;

		private DataMember m_parent;

		public DataMemberCollection this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				DataMemberCollection dataMemberCollection = null;
				if (index == 0)
				{
					dataMemberCollection = m_firstCollection;
				}
				else if (m_collections != null)
				{
					dataMemberCollection = m_collections[index - 1];
				}
				if (dataMemberCollection == null)
				{
					bool headingDefIsStaticSubtotal = index > 0 && m_headingDef[index].Static && !m_headingDef[index].Subtotal && m_headingDef[index - 1].Subtotal;
					CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = null;
					if (m_headingDef[index].Static && m_headingInstances != null && m_headingInstances.Count > index)
					{
						customReportItemHeadingInstanceList = new CustomReportItemHeadingInstanceList(1);
						customReportItemHeadingInstanceList.Add(m_headingInstances[index]);
					}
					else
					{
						customReportItemHeadingInstanceList = m_headingInstances;
					}
					dataMemberCollection = new DataMemberCollection(m_owner, m_parent, m_headingDef[index], headingDefIsStaticSubtotal, customReportItemHeadingInstanceList);
					if (m_owner.UseCache)
					{
						if (index == 0)
						{
							m_firstCollection = dataMemberCollection;
						}
						else
						{
							if (m_collections == null)
							{
								m_collections = new DataMemberCollection[Count - 1];
							}
							m_collections[index - 1] = dataMemberCollection;
						}
					}
				}
				return dataMemberCollection;
			}
		}

		public int Count => m_headingDef.Count;

		internal DataGroupingCollection(CustomReportItem owner, DataMember parent, CustomReportItemHeadingList headingDef, CustomReportItemHeadingInstanceList headingInstances)
		{
			m_owner = owner;
			m_parent = parent;
			m_headingInstances = headingInstances;
			m_headingDef = headingDef;
		}
	}
}
