namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface IOutputRowHandler
{
	void OnRowBegin();

	void OnRowEnd();

	void OnRegionBegin();

	void OnRegionEnd();
}
