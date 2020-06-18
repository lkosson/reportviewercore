using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static StorageObjectCreator _instance = null;

		private static List<Declaration> _declarations = BuildDeclarations();

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StorageObjectCreator();
				}
				return _instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.WordOpenXmlTableGrid:
				persistObj = new OpenXmlTableGridModel();
				break;
			case ObjectType.WordOpenXmlTableRowProperties:
				persistObj = new OpenXmlTableRowPropertiesModel();
				break;
			case ObjectType.WordOpenXmlBorderProperties:
				persistObj = new OpenXmlBorderPropertiesModel();
				break;
			case ObjectType.WordOpenXmlHeaderFooterReferences:
				persistObj = new HeaderFooterReferences();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return _declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			return new List<Declaration>(6)
			{
				BaseInterleaver.GetDeclaration(),
				OpenXmlTableGridModel.GetDeclaration(),
				OpenXmlTableRowPropertiesModel.GetDeclaration(),
				OpenXmlBorderPropertiesModel.GetDeclaration(),
				HeaderFooterReferences.GetDeclaration()
			};
		}
	}
}
