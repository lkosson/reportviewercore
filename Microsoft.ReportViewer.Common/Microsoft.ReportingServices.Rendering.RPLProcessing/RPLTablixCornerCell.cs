namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLTablixCornerCell : RPLTablixCell
	{
		protected int m_rowSpan = 1;

		public override int RowSpan
		{
			get
			{
				return m_rowSpan;
			}
			set
			{
				m_rowSpan = value;
			}
		}

		internal RPLTablixCornerCell()
		{
		}

		internal RPLTablixCornerCell(RPLItem element, byte elementState, int rowSpan, int colSpan)
			: base(element, elementState)
		{
			m_rowSpan = rowSpan;
			m_colSpan = colSpan;
		}
	}
}
