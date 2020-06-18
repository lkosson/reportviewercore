using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLTablixRow
	{
		protected List<RPLTablixCell> m_cells;

		public RPLTablixCell this[int index]
		{
			get
			{
				return m_cells[index];
			}
			set
			{
				m_cells[index] = value;
			}
		}

		public virtual int HeaderStart => -1;

		public virtual int BodyStart => 0;

		public virtual List<RPLTablixMemberCell> OmittedHeaders => null;

		public int NumCells
		{
			get
			{
				if (m_cells == null)
				{
					return 0;
				}
				return m_cells.Count;
			}
		}

		internal List<RPLTablixCell> RowCells => m_cells;

		internal RPLTablixRow()
		{
			m_cells = new List<RPLTablixCell>();
		}

		internal RPLTablixRow(List<RPLTablixCell> cells)
		{
			m_cells = cells;
		}

		internal virtual void SetHeaderStart()
		{
		}

		internal virtual void SetBodyStart()
		{
		}

		internal virtual void AddOmittedHeader(RPLTablixMemberCell cell)
		{
		}

		internal void AddCells(List<RPLTablixCell> cells)
		{
			if (cells == null)
			{
				return;
			}
			if (m_cells == null || m_cells.Count == 0)
			{
				m_cells = cells;
				return;
			}
			if (m_cells.Count < cells.Count)
			{
				cells.InsertRange(0, m_cells);
				m_cells = cells;
				return;
			}
			for (int i = 0; i < cells.Count; i++)
			{
				m_cells.Add(cells[i]);
			}
		}
	}
}
