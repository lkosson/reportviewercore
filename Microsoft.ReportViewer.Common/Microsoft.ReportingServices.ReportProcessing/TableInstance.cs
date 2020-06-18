using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableInstance : ReportItemInstance, IShowHideContainer, IPageItem
	{
		private TableRowInstance[] m_headerRowInstances;

		private TableGroupInstanceList m_tableGroupInstances;

		private TableDetailInstanceList m_tableDetailInstances;

		private TableRowInstance[] m_footerRowInstances;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		private int m_currentPage = -1;

		[NonSerialized]
		private int m_numberOfChildrenOnThisPage;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

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

		internal TableGroupInstanceList TableGroupInstances
		{
			get
			{
				return m_tableGroupInstances;
			}
			set
			{
				m_tableGroupInstances = value;
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

		internal int CurrentPage
		{
			get
			{
				return m_currentPage;
			}
			set
			{
				m_currentPage = value;
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

		int IPageItem.StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal TableInstance(ReportProcessing.ProcessingContext pc, Table reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			ConstructorHelper(pc, reportItemDef);
			if (reportItemDef.TableGroups == null && reportItemDef.TableDetail != null)
			{
				m_tableDetailInstances = new TableDetailInstanceList();
			}
			m_renderingPages = new RenderingPagesRangesList();
			m_currentPage = reportItemDef.StartPage;
			reportItemDef.CurrentPage = reportItemDef.StartPage;
		}

		internal TableInstance(ReportProcessing.ProcessingContext pc, Table reportItemDef, TableDetailInstanceList tableDetailInstances, RenderingPagesRangesList renderingPages)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			ConstructorHelper(pc, reportItemDef);
			if (reportItemDef.TableGroups == null && reportItemDef.TableDetail != null)
			{
				m_tableDetailInstances = tableDetailInstances;
				m_renderingPages = renderingPages;
			}
			m_currentPage = reportItemDef.StartPage;
			reportItemDef.CurrentPage = reportItemDef.StartPage;
			reportItemDef.BottomInEndPage = pc.Pagination.CurrentPageHeight;
		}

		internal TableInstance()
		{
		}

		private void ConstructorHelper(ReportProcessing.ProcessingContext pc, Table reportItemDef)
		{
			m_instanceInfo = new TableInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			IndexedExprHost visibilityHiddenExprHost = (reportItemDef.TableExprHost != null) ? reportItemDef.TableExprHost.TableRowVisibilityHiddenExpressions : null;
			if (reportItemDef.HeaderRows != null)
			{
				m_headerRowInstances = new TableRowInstance[reportItemDef.HeaderRows.Count];
				for (int i = 0; i < m_headerRowInstances.Length; i++)
				{
					m_headerRowInstances[i] = new TableRowInstance(pc, reportItemDef.HeaderRows[i], reportItemDef, visibilityHiddenExprHost);
				}
			}
			if (reportItemDef.FooterRows != null)
			{
				m_footerRowInstances = new TableRowInstance[reportItemDef.FooterRows.Count];
				for (int j = 0; j < m_footerRowInstances.Length; j++)
				{
					m_footerRowInstances[j] = new TableRowInstance(pc, reportItemDef.FooterRows[j], reportItemDef, visibilityHiddenExprHost);
				}
			}
			if (reportItemDef.TableGroups != null)
			{
				m_tableGroupInstances = new TableGroupInstanceList();
			}
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
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
			if (m_tableGroupInstances != null)
			{
				int count = m_tableGroupInstances.Count;
				for (int j = 0; j < count; j++)
				{
					obj = ((ISearchByUniqueName)m_tableGroupInstances[j]).Find(targetUniqueName, ref nonCompNames, chunkManager);
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
			context.BeginProcessContainer(m_uniqueName, m_reportItemDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_reportItemDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRowInstances, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.TableGroupInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroupInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.SimpleDetailStartUniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.TableDetailInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableDetailInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRowInstances, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadTableInstanceInfo((Table)m_reportItemDef);
		}
	}
}
