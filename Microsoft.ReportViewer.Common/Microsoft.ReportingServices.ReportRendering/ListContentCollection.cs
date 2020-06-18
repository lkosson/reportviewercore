using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ListContentCollection
	{
		private List m_owner;

		private ListContent[] m_listContents;

		private ListContent m_firstListContent;

		public ListContent this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				ListContent listContent = null;
				if (index == 0)
				{
					listContent = m_firstListContent;
				}
				else if (m_listContents != null)
				{
					listContent = m_listContents[index - 1];
				}
				if (listContent == null)
				{
					listContent = new ListContent(m_owner, index);
					if (m_owner.RenderingContext.CacheState)
					{
						if (index == 0)
						{
							m_firstListContent = listContent;
						}
						else
						{
							if (m_listContents == null)
							{
								m_listContents = new ListContent[Count - 1];
							}
							m_listContents[index - 1] = listContent;
						}
					}
				}
				return listContent;
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				ListInstance listInstance = (ListInstance)m_owner.ReportItemInstance;
				if (listInstance != null)
				{
					num = listInstance.ListContents.Count;
				}
				if (num == 0)
				{
					return 1;
				}
				return num;
			}
		}

		internal ListContentCollection(List owner)
		{
			m_owner = owner;
		}
	}
}
