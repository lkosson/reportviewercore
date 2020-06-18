using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetailInstance : InstanceInfoOwner, IShowHideContainer, ISearchByUniqueName
	{
		private int m_uniqueName;

		private TableRowInstance[] m_detailRowInstances;

		[NonSerialized]
		[Reference]
		private TableDetail m_tableDetailDef;

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

		internal TableDetail TableDetailDef
		{
			get
			{
				return m_tableDetailDef;
			}
			set
			{
				m_tableDetailDef = value;
			}
		}

		internal TableRowInstance[] DetailRowInstances
		{
			get
			{
				return m_detailRowInstances;
			}
			set
			{
				m_detailRowInstances = value;
			}
		}

		internal TableDetailInstance(ReportProcessing.ProcessingContext pc, TableDetail tableDetailDef, Table tableDef)
		{
			m_uniqueName = pc.CreateUniqueName();
			m_instanceInfo = new TableDetailInstanceInfo(pc, tableDetailDef, this, tableDef);
			pc.Pagination.EnterIgnoreHeight(tableDetailDef.StartHidden);
			m_tableDetailDef = tableDetailDef;
			if (tableDetailDef.DetailRows != null)
			{
				IndexedExprHost visibilityHiddenExprHost = (tableDetailDef.ExprHost != null) ? tableDetailDef.ExprHost.TableRowVisibilityHiddenExpressions : null;
				m_detailRowInstances = new TableRowInstance[tableDetailDef.DetailRows.Count];
				for (int i = 0; i < m_detailRowInstances.Length; i++)
				{
					m_detailRowInstances[i] = new TableRowInstance(pc, tableDetailDef.DetailRows[i], tableDef, visibilityHiddenExprHost);
				}
			}
		}

		internal TableDetailInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			if (m_detailRowInstances != null)
			{
				int num = m_detailRowInstances.Length;
				for (int i = 0; i < num; i++)
				{
					obj = ((ISearchByUniqueName)m_detailRowInstances[i]).Find(targetUniqueName, ref nonCompNames, chunkManager);
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
			context.BeginProcessContainer(m_uniqueName, m_tableDetailDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_tableDetailDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DetailRowInstances, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal TableDetailInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadTableDetailInstanceInfo();
			}
			return (TableDetailInstanceInfo)m_instanceInfo;
		}
	}
}
