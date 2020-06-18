using System;

namespace Microsoft.ReportingServices.Interfaces
{
	[Serializable]
	public class AceStruct
	{
		public string PrincipalName;

		public CatalogOperationsCollection CatalogOperations;

		public ReportOperationsCollection ReportOperations;

		public FolderOperationsCollection FolderOperations;

		public ResourceOperationsCollection ResourceOperations;

		public DatasourceOperationsCollection DatasourceOperations;

		public ModelOperationsCollection ModelOperations;

		public ModelItemOperationsCollection ModelItemOperations;

		public AceStruct(string name)
		{
			PrincipalName = name;
			CatalogOperations = new CatalogOperationsCollection();
			ReportOperations = new ReportOperationsCollection();
			FolderOperations = new FolderOperationsCollection();
			ResourceOperations = new ResourceOperationsCollection();
			DatasourceOperations = new DatasourceOperationsCollection();
			ModelOperations = new ModelOperationsCollection();
			ModelItemOperations = new ModelItemOperationsCollection();
		}

		public AceStruct(AceStruct other)
		{
			PrincipalName = other.PrincipalName;
			CatalogOperations = other.CatalogOperations;
			ReportOperations = other.ReportOperations;
			FolderOperations = other.FolderOperations;
			ResourceOperations = other.ResourceOperations;
			DatasourceOperations = other.DatasourceOperations;
			ModelOperations = other.ModelOperations;
			ModelItemOperations = other.ModelItemOperations;
		}
	}
}
