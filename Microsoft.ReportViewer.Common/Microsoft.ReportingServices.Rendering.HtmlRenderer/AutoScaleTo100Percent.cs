namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class AutoScaleTo100Percent : ISize
	{
		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_defaultPixelSize);
		}
	}
}
