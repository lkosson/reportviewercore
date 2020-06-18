using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CommonObjectCreator : IScalabilityObjectCreator
	{
		private static List<Declaration> m_declarations = BuildDeclarations();

		private static CommonObjectCreator m_instance = null;

		internal static CommonObjectCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new CommonObjectCreator();
				}
				return m_instance;
			}
		}

		private CommonObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.StorageItem:
				persistObj = new StorageItem();
				break;
			case ObjectType.ScalableDictionaryNode:
				persistObj = new ScalableDictionaryNode();
				break;
			case ObjectType.ScalableDictionaryValues:
				persistObj = new ScalableDictionaryValues();
				break;
			case ObjectType.StorableArray:
				persistObj = new StorableArray();
				break;
			case ObjectType.ScalableHybridListEntry:
				persistObj = new ScalableHybridListEntry();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			return new List<Declaration>(8)
			{
				BaseReference.GetDeclaration(),
				ScalableList<StorageItem>.GetDeclaration(),
				ScalableDictionary<int, StorageItem>.GetDeclaration(),
				ScalableDictionaryNode.GetDeclaration(),
				ScalableDictionaryValues.GetDeclaration(),
				StorageItem.GetDeclaration(),
				StorableArray.GetDeclaration(),
				ScalableHybridListEntry.GetDeclaration()
			};
		}
	}
}
