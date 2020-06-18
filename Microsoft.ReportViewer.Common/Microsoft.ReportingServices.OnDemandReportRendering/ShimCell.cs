namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ShimCell : TablixCell
	{
		protected bool m_inSubtotal;

		protected string m_shimID;

		public override string ID => base.DefinitionPath;

		public override string DataElementName => null;

		public override DataElementOutputTypes DataElementOutput => DataElementOutputTypes.ContentsOnly;

		internal ShimCell(Tablix owner, int rowIndex, int colIndex, bool inSubtotal)
			: base(null, owner, rowIndex, colIndex)
		{
			m_inSubtotal = inSubtotal;
		}
	}
}
