using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroupInstance : InstanceInfoOwner, IShowHideContainer, ISearchByUniqueName
	{
		private int m_uniqueName;

		private TableRowInstance[] m_headerRowInstances;

		private TableRowInstance[] m_footerRowInstances;

		private TableGroupInstanceList m_subGroupInstances;

		private TableDetailInstanceList m_tableDetailInstances;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		[Reference]
		private TableGroup m_tableGroupDef;

		[NonSerialized]
		private int m_numberOfChildrenOnThisPage;

		internal int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		internal TableGroup TableGroupDef
		{
			get
			{
				return m_tableGroupDef;
			}
			set
			{
				m_tableGroupDef = value;
			}
		}

		internal TableRowInstance[] HeaderRowInstances
		{
			get
			{
				return m_headerRowInstances;
			}
			set
			{
				m_headerRowInstances = value;
			}
		}

		internal TableRowInstance[] FooterRowInstances
		{
			get
			{
				return m_footerRowInstances;
			}
			set
			{
				m_footerRowInstances = value;
			}
		}

		internal TableGroupInstanceList SubGroupInstances
		{
			get
			{
				return m_subGroupInstances;
			}
			set
			{
				m_subGroupInstances = value;
			}
		}

		internal TableDetailInstanceList TableDetailInstances
		{
			get
			{
				return m_tableDetailInstances;
			}
			set
			{
				m_tableDetailInstances = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return m_renderingPages;
			}
			set
			{
				m_renderingPages = value;
			}
		}

		internal int NumberOfChildrenOnThisPage
		{
			get
			{
				return m_numberOfChildrenOnThisPage;
			}
			set
			{
				m_numberOfChildrenOnThisPage = value;
			}
		}

		internal TableGroupInstance(ReportProcessing.ProcessingContext pc, TableGroup tableGroupDef)
		{
			Table table = (Table)tableGroupDef.DataRegionDef;
			m_uniqueName = pc.CreateUniqueName();
			m_instanceInfo = new TableGroupInstanceInfo(pc, tableGroupDef, this);
			pc.Pagination.EnterIgnoreHeight(tableGroupDef.StartHidden);
			m_tableGroupDef = tableGroupDef;
			IndexedExprHost visibilityHiddenExprHost = (tableGroupDef.ExprHost != null) ? tableGroupDef.ExprHost.TableRowVisibilityHiddenExpressions : null;
			m_renderingPages = new RenderingPagesRangesList();
			if (tableGroupDef.HeaderRows != null)
			{
				m_headerRowInstances = new TableRowInstance[tableGroupDef.HeaderRows.Count];
				for (int i = 0; i < m_headerRowInstances.Length; i++)
				{
					m_headerRowInstances[i] = new TableRowInstance(pc, tableGroupDef.HeaderRows[i], table, visibilityHiddenExprHost);
				}
			}
			if (tableGroupDef.FooterRows != null)
			{
				m_footerRowInstances = new TableRowInstance[tableGroupDef.FooterRows.Count];
				for (int j = 0; j < m_footerRowInstances.Length; j++)
				{
					m_footerRowInstances[j] = new TableRowInstance(pc, tableGroupDef.FooterRows[j], table, visibilityHiddenExprHost);
				}
			}
			if (tableGroupDef.SubGroup != null)
			{
				m_subGroupInstances = new TableGroupInstanceList();
			}
			else if (table.TableDetail != null)
			{
				m_tableDetailInstances = new TableDetailInstanceList();
			}
		}

		internal TableGroupInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			if (m_headerRowInstances != null)
			{
				int num = m_headerRowInstances.Length;
				for (int i = 0; i < num; i++)
				{
					obj = ((ISearchByUniqueName)m_headerRowInstances[i]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			if (m_subGroupInstances != null)
			{
				int count = m_subGroupInstances.Count;
				for (int j = 0; j < count; j++)
				{
					obj = ((ISearchByUniqueName)m_subGroupInstances[j]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			else if (m_tableDetailInstances != null)
			{
				int count2 = m_tableDetailInstances.Count;
				for (int k = 0; k < count2; k++)
				{
					obj = ((ISearchByUniqueName)m_tableDetailInstances[k]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			if (m_footerRowInstances != null)
			{
				int num2 = m_footerRowInstances.Length;
				for (int l = 0; l < num2; l++)
				{
					obj = ((ISearchByUniqueName)m_footerRowInstances[l]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			return null;
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(m_uniqueName, m_tableGroupDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_tableGroupDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRowInstances, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRowInstances, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.SubGroupInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroupInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.SimpleDetailStartUniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.TableDetailInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableDetailInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal TableGroupInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadTableGroupInstanceInfo();
			}
			return (TableGroupInstanceInfo)m_instanceInfo;
		}
	}
}
