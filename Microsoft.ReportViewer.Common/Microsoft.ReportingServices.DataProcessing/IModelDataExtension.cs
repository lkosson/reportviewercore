namespace Microsoft.ReportingServices.DataProcessing
{
	internal interface IModelDataExtension
	{
		string GetModelMetadata(string perspectiveName, string supportedVersion);

		void CancelModelMetadataRetrieval();
	}
}
