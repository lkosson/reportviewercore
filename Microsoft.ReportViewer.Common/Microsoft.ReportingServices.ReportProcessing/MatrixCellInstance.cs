using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class MatrixCellInstance : InstanceInfoOwner
	{
		private ReportItemInstance m_content;

		internal ReportItemInstance Content
		{
			get
			{
				return m_content;
			}
			set
			{
				m_content = value;
			}
		}

		internal MatrixCellInstance(int rowIndex, int colIndex, Matrix matrixDef, int cellDefIndex, ReportProcessing.ProcessingContext pc, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			m_instanceInfo = new MatrixCellInstanceInfo(rowIndex, colIndex, matrixDef, cellDefIndex, pc, this, out nonComputedUniqueNames);
		}

		internal MatrixCellInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemDef, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem));
			memberInfoList.Add(new MemberInfo(MemberName.Content, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal MatrixCellInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadMatrixCellInstanceInfo();
			}
			return (MatrixCellInstanceInfo)m_instanceInfo;
		}
	}
}
