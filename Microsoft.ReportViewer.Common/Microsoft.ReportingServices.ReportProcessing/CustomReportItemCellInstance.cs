using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemCellInstance
	{
		private int m_rowIndex;

		private int m_columnIndex;

		private DataValueInstanceList m_dataValueInstances;

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

		internal DataValueInstanceList DataValueInstances
		{
			get
			{
				return m_dataValueInstances;
			}
			set
			{
				m_dataValueInstances = value;
			}
		}

		internal CustomReportItemCellInstance(int rowIndex, int colIndex, CustomReportItem definition, ReportProcessing.ProcessingContext pc)
		{
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
			Global.Tracer.Assert(definition != null && definition.DataRowCells != null && rowIndex < definition.DataRowCells.Count && colIndex < definition.DataRowCells[rowIndex].Count && 0 < definition.DataRowCells[rowIndex][colIndex].Count);
			DataValueCRIList dataValueCRIList = definition.DataRowCells[rowIndex][colIndex];
			Global.Tracer.Assert(dataValueCRIList != null);
			m_dataValueInstances = dataValueCRIList.EvaluateExpressions(definition.ObjectType, definition.Name, null, dataValueCRIList.RDLRowIndex, dataValueCRIList.RDLColumnIndex, pc);
			Global.Tracer.Assert(m_dataValueInstances != null);
		}

		internal CustomReportItemCellInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.RowIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataValueInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
