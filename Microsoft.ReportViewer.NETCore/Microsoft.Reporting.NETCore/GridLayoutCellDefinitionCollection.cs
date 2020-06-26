using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class GridLayoutCellDefinitionCollection : ReadOnlyCollection<GridLayoutCellDefinition>
	{
		private struct Index
		{
			public int Row;

			public int Column;

			public Index(int row, int column)
			{
				Row = row;
				Column = column;
			}
		}

		private readonly Dictionary<Index, GridLayoutCellDefinition> m_cellDefintionsByIndex = new Dictionary<Index, GridLayoutCellDefinition>();

		private readonly Dictionary<string, GridLayoutCellDefinition> m_cellDefintionsByName = new Dictionary<string, GridLayoutCellDefinition>(StringComparer.OrdinalIgnoreCase);

		public string this[int rowIndex, int colIndex]
		{
			get
			{
				if (m_cellDefintionsByIndex.TryGetValue(new Index(rowIndex, colIndex), out GridLayoutCellDefinition value))
				{
					return value.ParameterName;
				}
				return null;
			}
		}

		internal GridLayoutCellDefinitionCollection(IList<GridLayoutCellDefinition> cellDefinitions)
			: base(cellDefinitions)
		{
			foreach (GridLayoutCellDefinition cellDefinition in cellDefinitions)
			{
				m_cellDefintionsByIndex[new Index(cellDefinition.Row, cellDefinition.Column)] = cellDefinition;
				m_cellDefintionsByName[cellDefinition.ParameterName] = cellDefinition;
			}
		}

		internal GridLayoutCellDefinitionCollection()
			: base((IList<GridLayoutCellDefinition>)new GridLayoutCellDefinition[0])
		{
		}

		public GridLayoutCellDefinition GetByName(string parameterName)
		{
			if (m_cellDefintionsByName.TryGetValue(parameterName, out GridLayoutCellDefinition value))
			{
				return value;
			}
			return null;
		}
	}
}
