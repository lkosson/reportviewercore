using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixCellInstanceInfo : InstanceInfo
	{
		private NonComputedUniqueNames m_contentUniqueNames;

		private int m_rowIndex;

		private int m_columnIndex;

		internal NonComputedUniqueNames ContentUniqueNames
		{
			get
			{
				return m_contentUniqueNames;
			}
			set
			{
				m_contentUniqueNames = value;
			}
		}

		internal int RowIndex
		{
			get
			{
				return m_rowIndex;
			}
			set
			{
				m_rowIndex = value;
			}
		}

		internal int ColumnIndex
		{
			get
			{
				return m_columnIndex;
			}
			set
			{
				m_columnIndex = value;
			}
		}

		internal MatrixCellInstanceInfo(int rowIndex, int colIndex, Matrix matrixDef, int cellDefIndex, ReportProcessing.ProcessingContext pc, MatrixCellInstance owner, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
			if (0 < matrixDef.CellReportItems.Count && !matrixDef.CellReportItems.IsReportItemComputed(cellDefIndex))
			{
				m_contentUniqueNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, matrixDef.CellReportItems[cellDefIndex]);
			}
			nonComputedUniqueNames = m_contentUniqueNames;
			Global.Tracer.Assert(cellDefIndex < matrixDef.FirstCellInstances.Count);
			if (matrixDef.FirstCellInstances[cellDefIndex])
			{
				pc.ChunkManager.AddInstanceToFirstPage(this, owner, pc.InPageSection);
				matrixDef.FirstCellInstances[cellDefIndex] = false;
			}
			else
			{
				pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
			}
		}

		internal MatrixCellInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ContentUniqueNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.NonComputedUniqueNames));
			memberInfoList.Add(new MemberInfo(MemberName.RowIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnIndex, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
