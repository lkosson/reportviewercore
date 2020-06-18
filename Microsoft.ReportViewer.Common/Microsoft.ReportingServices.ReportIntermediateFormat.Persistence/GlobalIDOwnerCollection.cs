using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class GlobalIDOwnerCollection
	{
		private int m_currentID = -1;

		private Dictionary<int, IGloballyReferenceable> m_globallyReferenceableItems;

		internal int LastAssignedID => m_currentID;

		internal GlobalIDOwnerCollection()
		{
			m_globallyReferenceableItems = new Dictionary<int, IGloballyReferenceable>(EqualityComparers.Int32ComparerInstance);
		}

		internal int GetGlobalID()
		{
			return ++m_currentID;
		}

		internal void Add(IGloballyReferenceable globallyReferenceableItem)
		{
			m_globallyReferenceableItems.Add(m_currentID, globallyReferenceableItem);
		}

		internal bool TryGetValue(int refID, out IGloballyReferenceable referenceableItem)
		{
			return m_globallyReferenceableItems.TryGetValue(refID, out referenceableItem);
		}
	}
}
