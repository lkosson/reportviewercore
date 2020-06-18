using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct LookupRIFObjectCreator : IRIFObjectCreator, IScalabilityObjectCreator
	{
		private static List<Declaration> m_declarations = BuildDeclarations();

		public IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistObj = null;
			if (objectType == ObjectType.Null)
			{
				return null;
			}
			Global.Tracer.Assert(TryCreateObject(objectType, out persistObj));
			persistObj.Deserialize(context);
			return persistObj;
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.LookupMatches:
				persistObj = new LookupMatches();
				break;
			case ObjectType.LookupMatchesWithRows:
				persistObj = new LookupMatchesWithRows();
				break;
			case ObjectType.LookupTable:
				persistObj = new LookupTable();
				break;
			case ObjectType.IntermediateFormatVersion:
				persistObj = new IntermediateFormatVersion();
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
			return new List<Declaration>(3)
			{
				LookupMatches.GetDeclaration(),
				LookupMatchesWithRows.GetDeclaration(),
				LookupTable.GetDeclaration()
			};
		}
	}
}
