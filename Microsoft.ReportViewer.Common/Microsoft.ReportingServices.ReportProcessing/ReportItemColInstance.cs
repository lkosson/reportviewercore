using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemColInstance : InstanceInfoOwner, ISearchByUniqueName, IIndexInto
	{
		private ReportItemInstanceList m_reportItemInstances;

		private RenderingPagesRangesList m_childrenStartAndEndPages;

		[Reference]
		private ReportItemCollection m_reportItemColDef;

		[NonSerialized]
		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		internal ReportItemInstanceList ReportItemInstances
		{
			get
			{
				return m_reportItemInstances;
			}
			set
			{
				m_reportItemInstances = value;
			}
		}

		internal ReportItemCollection ReportItemColDef
		{
			get
			{
				return m_reportItemColDef;
			}
			set
			{
				m_reportItemColDef = value;
			}
		}

		internal ReportItemInstance this[int index] => m_reportItemInstances[index];

		internal NonComputedUniqueNames[] ChildrenNonComputedUniqueNames
		{
			get
			{
				return m_childrenNonComputedUniqueNames;
			}
			set
			{
				m_childrenNonComputedUniqueNames = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return m_childrenStartAndEndPages;
			}
			set
			{
				m_childrenStartAndEndPages = value;
			}
		}

		internal ReportItemColInstance(ReportProcessing.ProcessingContext pc, ReportItemCollection reportItemsDef)
		{
			m_reportItemColDef = reportItemsDef;
			if (reportItemsDef.ComputedReportItems != null)
			{
				m_reportItemInstances = new ReportItemInstanceList(reportItemsDef.ComputedReportItems.Count);
			}
			if (pc != null)
			{
				m_childrenNonComputedUniqueNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, reportItemsDef);
			}
			m_instanceInfo = new ReportItemColInstanceInfo(pc, reportItemsDef, this);
		}

		internal ReportItemColInstance()
		{
		}

		internal void Add(ReportItemInstance riInstance)
		{
			Global.Tracer.Assert(m_reportItemInstances != null);
			m_reportItemInstances.Add(riInstance);
		}

		internal int GetReportItemUniqueName(int index)
		{
			int num = -1;
			ReportItem reportItem = null;
			Global.Tracer.Assert(index >= 0 && index < m_reportItemColDef.Count);
			m_reportItemColDef.GetReportItem(index, out bool computed, out int internalIndex, out reportItem);
			if (!computed)
			{
				return m_childrenNonComputedUniqueNames[internalIndex].UniqueName;
			}
			return m_reportItemInstances[internalIndex].UniqueName;
		}

		internal void GetReportItemStartAndEndPages(int index, ref int startPage, ref int endPage)
		{
			Global.Tracer.Assert(index >= 0 && index < m_reportItemColDef.Count);
			if (m_childrenStartAndEndPages != null)
			{
				RenderingPagesRanges renderingPagesRanges = m_childrenStartAndEndPages[index];
				startPage = renderingPagesRanges.StartPage;
				endPage = renderingPagesRanges.EndPage;
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			int count = m_reportItemColDef.Count;
			for (int i = 0; i < count; i++)
			{
				m_reportItemColDef.GetReportItem(i, out bool computed, out int internalIndex, out ReportItem reportItem);
				if (computed)
				{
					obj = ((ISearchByUniqueName)m_reportItemInstances[internalIndex]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
					continue;
				}
				NonComputedUniqueNames nonCompNames2 = GetInstanceInfo(chunkManager, inPageSection: false).ChildrenNonComputedUniqueNames[internalIndex];
				obj = ((ISearchByUniqueName)reportItem).Find(targetUniqueName, ref nonCompNames2, chunkManager);
				if (obj != null)
				{
					nonCompNames = nonCompNames2;
					return obj;
				}
			}
			return null;
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			m_reportItemColDef.GetReportItem(index, out bool computed, out int internalIndex, out ReportItem reportItem);
			if (computed)
			{
				nonCompNames = null;
				return m_reportItemInstances[internalIndex];
			}
			nonCompNames = ((ReportItemColInstanceInfo)m_instanceInfo).ChildrenNonComputedUniqueNames[internalIndex];
			return reportItem;
		}

		internal ReportItemColInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, bool inPageSection)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(chunkManager != null);
				IntermediateFormatReader intermediateFormatReader = null;
				intermediateFormatReader = ((!inPageSection) ? chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset) : chunkManager.GetPageSectionInstanceReader(((OffsetInfo)m_instanceInfo).Offset));
				return intermediateFormatReader.ReadReportItemColInstanceInfo();
			}
			return (ReportItemColInstanceInfo)m_instanceInfo;
		}

		internal void SetPaginationForNonComputedChild(ReportProcessing.Pagination pagination, ReportItem reportItem, ReportItem parentDef)
		{
			ReportItemCollection reportItemColDef = m_reportItemColDef;
			int num = parentDef.StartPage;
			if (parentDef is Table)
			{
				num = ((Table)parentDef).CurrentPage;
			}
			else if (parentDef is Matrix)
			{
				num = ((Matrix)parentDef).CurrentPage;
			}
			reportItem.TopInStartPage = parentDef.TopInStartPage;
			if (reportItem.SiblingAboveMe != null)
			{
				for (int i = 0; i < reportItem.SiblingAboveMe.Count; i++)
				{
					ReportItem reportItem2 = reportItemColDef[reportItem.SiblingAboveMe[i]];
					int num2 = reportItem2.EndPage;
					if (reportItem2.TopValue + reportItem2.HeightValue > reportItem.TopValue)
					{
						num2 = reportItem2.StartPage;
						reportItem.TopInStartPage = reportItem2.TopInStartPage;
					}
					else
					{
						bool flag = reportItem2.ShareMyLastPage;
						if (!(reportItem2 is Table) && !(reportItem2 is Matrix))
						{
							flag = false;
						}
						if (!pagination.IgnorePageBreak && pagination.PageBreakAtEnd(reportItem2) && !flag)
						{
							num2++;
							if (i == reportItem.SiblingAboveMe.Count - 1)
							{
								pagination.SetCurrentPageHeight(reportItem, reportItem.HeightValue);
								reportItem.TopInStartPage = 0.0;
							}
						}
						else
						{
							reportItem.TopInStartPage = pagination.CurrentPageHeight;
						}
					}
					num = Math.Max(num, num2);
				}
			}
			if (pagination.CanMoveToNextPage(pagination.PageBreakAtStart(reportItem)))
			{
				num++;
				reportItem.TopInStartPage = 0.0;
				pagination.SetCurrentPageHeight(reportItem, reportItem.HeightValue);
			}
			reportItem.StartPage = num;
			reportItem.EndPage = num;
		}
	}
}
