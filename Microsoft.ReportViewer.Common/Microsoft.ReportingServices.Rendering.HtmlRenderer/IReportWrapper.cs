namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IReportWrapper
	{
		bool HasBookmarks
		{
			get;
		}

		string SortItem
		{
			get;
		}

		string ShowHideToggle
		{
			get;
		}

		string GetStreamUrl(bool useSessionId, string streamName);

		string GetReportUrl(bool addParams);

		byte[] GetImageName(string imageID);
	}
}
