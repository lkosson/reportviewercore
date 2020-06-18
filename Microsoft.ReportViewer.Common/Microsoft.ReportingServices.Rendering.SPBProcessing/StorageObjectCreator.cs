using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static StorageObjectCreator m_instance = null;

		private static List<Declaration> m_declarations = BuildDeclarations();

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new StorageObjectCreator();
				}
				return m_instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.RowMemberInfo:
				persistObj = new Tablix.RowMemberInfo();
				break;
			case ObjectType.SizeInfo:
				persistObj = new Tablix.SizeInfo();
				break;
			case ObjectType.DetailCell:
				persistObj = new Tablix.DetailCell();
				break;
			case ObjectType.CornerCell:
				persistObj = new Tablix.CornerCell();
				break;
			case ObjectType.MemberCell:
				persistObj = new Tablix.MemberCell();
				break;
			case ObjectType.StreamMemberCell:
				persistObj = new Tablix.StreamMemberCell();
				break;
			case ObjectType.RPLMemberCell:
				persistObj = new Tablix.RPLMemberCell();
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
				Tablix.RowMemberInfo.GetDeclaration(),
				Tablix.SizeInfo.GetDeclaration(),
				Tablix.DetailCell.GetDeclaration(),
				Tablix.CornerCell.GetDeclaration(),
				Tablix.MemberCell.GetDeclaration(),
				Tablix.PageMemberCell.GetDeclaration(),
				Tablix.StreamMemberCell.GetDeclaration(),
				Tablix.RPLMemberCell.GetDeclaration()
			};
		}
	}
}
