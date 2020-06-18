using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class GridLayoutDefinition
	{
		private readonly int m_numberOfColumns;

		private readonly int m_numberOfRows;

		private readonly GridLayoutCellDefinitionCollection m_cellDefinitions;

		private readonly bool[] m_rowsVisibility;

		private readonly bool[] m_columnsVisibility;

		public int NumberOfColumns => m_numberOfColumns;

		public int NumberOfRows => m_numberOfRows;

		public GridLayoutCellDefinitionCollection CellDefinitions => m_cellDefinitions;

		public bool[] RowsVisibility => m_rowsVisibility;

		public bool[] ColumnsVisibility => m_columnsVisibility;

		internal GridLayoutDefinition(GridLayoutCellDefinitionCollection cellDefs, int numRows, int numColumns, ReportParameterInfoCollection paramInfoCollection)
		{
			m_cellDefinitions = cellDefs;
			m_numberOfRows = numRows;
			m_numberOfColumns = numColumns;
			m_columnsVisibility = new bool[numColumns];
			m_rowsVisibility = new bool[numRows];
			if (paramInfoCollection == null || !paramInfoCollection.Any())
			{
				return;
			}
			int j;
			for (j = 0; j < m_numberOfColumns; j++)
			{
				IEnumerable<GridLayoutCellDefinition> enumerable = m_cellDefinitions.Where((GridLayoutCellDefinition p) => p.Column == j);
				if (!enumerable.Any() && j < m_cellDefinitions.Max((GridLayoutCellDefinition p) => p.Column))
				{
					m_columnsVisibility[j] = true;
				}
				foreach (GridLayoutCellDefinition param in enumerable)
				{
					if (paramInfoCollection.Any((ReportParameterInfo p) => p.Name == param.ParameterName && IsParamVisible(p)))
					{
						m_columnsVisibility[j] = true;
						m_rowsVisibility[param.Row] = true;
					}
				}
			}
			int i;
			for (i = 0; i < m_numberOfRows; i++)
			{
				if (!m_cellDefinitions.Where((GridLayoutCellDefinition p) => p.Row == i).Any() && i < m_cellDefinitions.Max((GridLayoutCellDefinition p) => p.Row))
				{
					m_rowsVisibility[i] = true;
				}
			}
		}

		private bool IsParamVisible(ReportParameterInfo param)
		{
			if (!param.PromptUser)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(param.Prompt))
			{
				return param.Visible;
			}
			return false;
		}
	}
}
