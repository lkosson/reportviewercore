using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListRowCollection : TablixRowCollection
	{
		private ShimListRow m_row;

		public override TablixRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_row;
			}
		}

		public override int Count => 1;

		internal ShimListRowCollection(Tablix owner)
			: base(owner)
		{
			m_row = new ShimListRow(owner);
		}
	}
}
