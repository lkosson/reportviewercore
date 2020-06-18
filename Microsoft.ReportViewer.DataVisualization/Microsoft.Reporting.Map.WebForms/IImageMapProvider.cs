namespace Microsoft.Reporting.Map.WebForms
{
	internal interface IImageMapProvider
	{
		object Tag
		{
			get;
			set;
		}

		string Href
		{
			get;
			set;
		}

		string GetToolTip();

		string GetHref();

		string GetMapAreaAttributes();
	}
}
