namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal interface HtmlElement
	{
		void Render(IOutputStream outputStream);
	}
}
