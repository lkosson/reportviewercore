namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class AutoHeight : ISize
	{
		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_auto);
		}
	}
}
