using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListContentInstance : InstanceInfoOwner, ISearchByUniqueName, IShowHideContainer
	{
		private int m_uniqueName;

		private ReportItemColInstance m_reportItemColInstance;

		[NonSerialized]
		[Reference]
		private List m_listDef;

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

		internal List ListDef
		{
			get
			{
				return m_listDef;
			}
			set
			{
				m_listDef = value;
			}
		}

		internal ReportItemColInstance ReportItemColInstance
		{
			get
			{
				return m_reportItemColInstance;
			}
			set
			{
				m_reportItemColInstance = value;
			}
		}

		internal ListContentInstance(ReportProcessing.ProcessingContext pc, List listDef)
		{
			m_uniqueName = pc.CreateUniqueName();
			m_listDef = listDef;
			m_reportItemColInstance = new ReportItemColInstance(pc, listDef.ReportItems);
			m_instanceInfo = new ListContentInstanceInfo(pc, this, listDef);
			pc.Pagination.EnterIgnoreHeight(listDef.StartHidden);
		}

		internal ListContentInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)m_reportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(m_uniqueName, m_listDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_listDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal ListContentInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadListContentInstanceInfo();
			}
			return (ListContentInstanceInfo)m_instanceInfo;
		}
	}
}
