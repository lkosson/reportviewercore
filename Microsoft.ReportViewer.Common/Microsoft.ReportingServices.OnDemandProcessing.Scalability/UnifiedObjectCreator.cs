using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class UnifiedObjectCreator : IRIFObjectCreator
	{
		private IScalabilityObjectCreator[] m_objectCreators;

		private IReferenceCreator[] m_referenceCreators;

		private IScalabilityCache m_scalabilityCache;

		internal IScalabilityCache ScalabilityCache
		{
			get
			{
				return m_scalabilityCache;
			}
			set
			{
				m_scalabilityCache = value;
			}
		}

		internal UnifiedObjectCreator(IScalabilityObjectCreator appObjectCreator, IReferenceCreator appReferenceCreator)
		{
			m_objectCreators = new IScalabilityObjectCreator[2];
			m_objectCreators[0] = CommonObjectCreator.Instance;
			m_objectCreators[1] = appObjectCreator;
			m_referenceCreators = new IReferenceCreator[2];
			m_referenceCreators[0] = CommonReferenceCreator.Instance;
			m_referenceCreators[1] = appReferenceCreator;
		}

		public IPersistable CreateRIFObject(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable newObject = null;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < m_objectCreators.Length; i++)
			{
				if (flag)
				{
					break;
				}
				flag = m_objectCreators[i].TryCreateObject(objectType, out newObject);
			}
			if (!flag)
			{
				flag2 = true;
				BaseReference newReference = null;
				for (int j = 0; j < m_referenceCreators.Length; j++)
				{
					if (flag)
					{
						break;
					}
					flag = m_referenceCreators[j].TryCreateReference(objectType, out newReference);
				}
				newObject = newReference;
			}
			if (flag)
			{
				newObject.Deserialize(context);
				if (flag2)
				{
					BaseReference baseReference = (BaseReference)newObject;
					newObject = baseReference.ScalabilityCache.PoolReference(baseReference);
				}
			}
			else
			{
				Global.Tracer.Assert(false, "Cannot create object of type: {0}", objectType);
			}
			return newObject;
		}

		internal List<Declaration> GetDeclarations()
		{
			List<Declaration> list = new List<Declaration>();
			for (int i = 0; i < m_objectCreators.Length; i++)
			{
				list.AddRange(m_objectCreators[i].GetDeclarations());
			}
			return list;
		}
	}
}
