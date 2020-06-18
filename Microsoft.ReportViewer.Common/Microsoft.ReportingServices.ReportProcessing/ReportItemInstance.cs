using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ReportItemInstance : InstanceInfoOwner, ISearchByUniqueName
	{
		protected int m_uniqueName;

		[Reference]
		protected ReportItem m_reportItemDef;

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

		internal ReportItem ReportItemDef
		{
			get
			{
				return m_reportItemDef;
			}
			set
			{
				m_reportItemDef = value;
			}
		}

		internal ReportItemInstance(int uniqueName, ReportItem reportItemDef)
		{
			m_uniqueName = uniqueName;
			m_reportItemDef = reportItemDef;
		}

		internal ReportItemInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_uniqueName == targetUniqueName)
			{
				nonCompNames = null;
				return this;
			}
			return SearchChildren(targetUniqueName, ref nonCompNames, chunkManager);
		}

		protected virtual object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return null;
		}

		internal virtual int GetDocumentMapUniqueName()
		{
			return m_uniqueName;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal ReportItemInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			return GetInstanceInfo(chunkManager, inPageSection: false);
		}

		internal ReportItemInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, bool inPageSection)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(chunkManager != null);
				IntermediateFormatReader intermediateFormatReader = null;
				intermediateFormatReader = ((!inPageSection) ? chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset) : chunkManager.GetPageSectionInstanceReader(((OffsetInfo)m_instanceInfo).Offset));
				return ReadInstanceInfo(intermediateFormatReader);
			}
			return (ReportItemInstanceInfo)m_instanceInfo;
		}

		internal abstract ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader);
	}
}
