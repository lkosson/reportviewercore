using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	[SkipStaticValidation]
	internal abstract class SyntheticReferenceBase<T> : IReference<T>, IReference, IStorable, IPersistable
	{
		public ReferenceID Id
		{
			get
			{
				Global.Tracer.Assert(condition: false, "Id may not be used on a synthetic reference.");
				throw new InvalidOperationException();
			}
		}

		public int Size
		{
			get
			{
				Global.Tracer.Assert(condition: false, "Size may not be used on a synthetic reference.");
				throw new InvalidOperationException();
			}
		}

		public abstract T Value();

		public IDisposable PinValue()
		{
			Global.Tracer.Assert(condition: false, "PinValue() may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void UnPinValue()
		{
			Global.Tracer.Assert(condition: false, "UnPinValue() may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void Free()
		{
			Global.Tracer.Assert(condition: false, "Free() may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void UpdateSize(int sizeDeltaBytes)
		{
			Global.Tracer.Assert(condition: false, "UpdateSize(int) may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public IReference TransferTo(IScalabilityCache scaleCache)
		{
			Global.Tracer.Assert(condition: false, "TransferTo(IScalabilityCache) may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			Global.Tracer.Assert(condition: false, "Serialize may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(condition: false, "Deserialize may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "ResolveReferences may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public abstract Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
