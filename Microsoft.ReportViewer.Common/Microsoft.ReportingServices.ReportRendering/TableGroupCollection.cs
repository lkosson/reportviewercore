using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableGroupCollection
	{
		private Table m_owner;

		private Microsoft.ReportingServices.ReportProcessing.TableGroup m_groupDef;

		private TableGroupInstanceList m_groupInstances;

		private TableGroup[] m_groups;

		private TableGroup m_firstGroup;

		private TableGroup m_parent;

		public TableGroup this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				TableGroup tableGroup = null;
				if (index == 0)
				{
					tableGroup = m_firstGroup;
				}
				else if (m_groups != null)
				{
					tableGroup = m_groups[index - 1];
				}
				if (tableGroup == null)
				{
					TableGroupInstance groupInstance = null;
					if (m_groupInstances != null && index < m_groupInstances.Count)
					{
						groupInstance = m_groupInstances[index];
					}
					else
					{
						Global.Tracer.Assert(m_groupInstances == null || m_groupInstances.Count == 0);
					}
					tableGroup = new TableGroup(m_owner, m_parent, m_groupDef, groupInstance);
					if (m_owner.RenderingContext.CacheState)
					{
						if (index == 0)
						{
							m_firstGroup = tableGroup;
						}
						else
						{
							if (m_groups == null)
							{
								m_groups = new TableGroup[m_groupInstances.Count - 1];
							}
							m_groups[index - 1] = tableGroup;
						}
					}
				}
				return tableGroup;
			}
		}

		public int Count
		{
			get
			{
				if (m_groupInstances == null || m_groupInstances.Count == 0)
				{
					return 1;
				}
				return m_groupInstances.Count;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.TableGroup GroupDefinition => m_groupDef;

		internal TableGroupCollection(Table owner, TableGroup parent, Microsoft.ReportingServices.ReportProcessing.TableGroup groupDef, TableGroupInstanceList groupInstances)
		{
			m_owner = owner;
			m_parent = parent;
			m_groupInstances = groupInstances;
			m_groupDef = groupDef;
		}
	}
}
