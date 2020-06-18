using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class AggregateUpdateQueue : Queue<AggregateUpdateCollection>
	{
		private AggregateUpdateCollection m_originalState;

		public AggregateUpdateCollection OriginalState => m_originalState;

		internal AggregateUpdateQueue(AggregateUpdateCollection originalState)
		{
			m_originalState = originalState;
		}
	}
}
