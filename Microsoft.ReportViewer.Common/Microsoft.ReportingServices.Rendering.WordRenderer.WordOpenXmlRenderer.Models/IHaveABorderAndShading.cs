namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal interface IHaveABorderAndShading
	{
		string BackgroundColor
		{
			set;
		}

		OpenXmlBorderPropertiesModel BorderTop
		{
			get;
		}

		OpenXmlBorderPropertiesModel BorderBottom
		{
			get;
		}

		OpenXmlBorderPropertiesModel BorderLeft
		{
			get;
		}

		OpenXmlBorderPropertiesModel BorderRight
		{
			get;
		}
	}
}
