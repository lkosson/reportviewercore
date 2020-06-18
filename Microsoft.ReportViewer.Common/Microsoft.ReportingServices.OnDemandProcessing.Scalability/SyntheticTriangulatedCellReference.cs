using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	[SkipStaticValidation]
	internal sealed class SyntheticTriangulatedCellReference : SyntheticReferenceBase<IOnDemandScopeInstance>, IReference<RuntimeCell>, IReference, IStorable, IPersistable
	{
		private IReference<IOnDemandMemberInstance> m_outerGroupLeafRef;

		private IReference<IOnDemandMemberInstance> m_innerGroupLeafRef;

		public SyntheticTriangulatedCellReference(IReference<IOnDemandMemberInstance> outerGroupLeafRef, IReference<IOnDemandMemberInstance> innerGroupLeafRef)
		{
			UpdateGroupLeafReferences(outerGroupLeafRef, innerGroupLeafRef);
		}

		public void UpdateGroupLeafReferences(IReference<IOnDemandMemberInstance> outerGroupLeafRef, IReference<IOnDemandMemberInstance> innerGroupLeafRef)
		{
			m_outerGroupLeafRef = outerGroupLeafRef;
			m_innerGroupLeafRef = innerGroupLeafRef;
		}

		RuntimeCell IReference<RuntimeCell>.Value()
		{
			return (RuntimeCell)Value();
		}

		public override IOnDemandScopeInstance Value()
		{
			IReference<IOnDemandScopeInstance> cellRef;
			return GetCellInstance(m_outerGroupLeafRef, m_innerGroupLeafRef, out cellRef);
		}

		internal static IOnDemandScopeInstance GetCellInstance(IReference<IOnDemandMemberInstance> outerGroupLeafRef, IReference<IOnDemandMemberInstance> innerGroupLeafRef, out IReference<IOnDemandScopeInstance> cellRef)
		{
			using (innerGroupLeafRef.PinValue())
			{
				return innerGroupLeafRef.Value().GetCellInstance((IOnDemandMemberInstanceReference)outerGroupLeafRef, out cellRef);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SyntheticTriangulatedCellReference;
		}

		public override bool Equals(object obj)
		{
			SyntheticTriangulatedCellReference syntheticTriangulatedCellReference = obj as SyntheticTriangulatedCellReference;
			if (syntheticTriangulatedCellReference == null)
			{
				return false;
			}
			if (object.Equals(m_outerGroupLeafRef, syntheticTriangulatedCellReference.m_outerGroupLeafRef))
			{
				return object.Equals(m_innerGroupLeafRef, syntheticTriangulatedCellReference.m_innerGroupLeafRef);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_outerGroupLeafRef.GetHashCode() ^ m_innerGroupLeafRef.GetHashCode();
		}
	}
}
