namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class NoBackgroundRepeat : IBackgroundRepeatAttribute
	{
		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_noRepeat);
		}
	}
}
