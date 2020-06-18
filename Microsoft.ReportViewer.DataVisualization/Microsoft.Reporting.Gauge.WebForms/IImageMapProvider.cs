namespace Microsoft.Reporting.Gauge.WebForms
{
	internal interface IImageMapProvider
	{
		string Href
		{
			set;
		}

		object Tag
		{
			get;
			set;
		}

		string GetToolTip();

		string GetHref();

		string GetMapAreaAttributes();
	}
}
