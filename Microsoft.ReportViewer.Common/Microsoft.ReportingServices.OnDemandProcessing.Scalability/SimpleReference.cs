using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SimpleReference<T> : Reference<T> where T : IStorable
	{
		[NonSerialized]
		private ObjectType m_objectType;

		internal SimpleReference(ObjectType referenceType)
		{
			m_objectType = referenceType;
		}

		public override ObjectType GetObjectType()
		{
			return m_objectType;
		}
	}
}
