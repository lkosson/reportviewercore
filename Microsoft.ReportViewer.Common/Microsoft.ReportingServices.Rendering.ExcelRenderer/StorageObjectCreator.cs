using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer
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
			case ObjectType.ExcelRowInfo:
				persistObj = new LayoutEngine.RowInfo();
				break;
			case ObjectType.RowItemStruct:
				persistObj = new LayoutEngine.RowItemStruct();
				break;
			case ObjectType.TablixStruct:
				persistObj = new LayoutEngine.TablixStruct();
				break;
			case ObjectType.TablixMemberStruct:
				persistObj = new LayoutEngine.TablixMemberStruct();
				break;
			case ObjectType.ToggleParent:
				persistObj = new ToggleParent();
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
			return new List<Declaration>(6)
			{
				LayoutEngine.RowInfo.GetDeclaration(),
				LayoutEngine.RowItemStruct.GetDeclaration(),
				LayoutEngine.TablixItemStruct.GetDeclaration(),
				LayoutEngine.TablixStruct.GetDeclaration(),
				LayoutEngine.TablixMemberStruct.GetDeclaration(),
				ToggleParent.GetDeclaration()
			};
		}
	}
}
