namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class Html5OutputStream : IOutputStream
	{
		private HTML5Renderer Renderer
		{
			get;
			set;
		}

		public Html5OutputStream(HTML5Renderer renderer)
		{
			Renderer = renderer;
		}

		public void Write(string text)
		{
			Renderer.WriteStream(text);
		}

		public void Write(byte[] text)
		{
			Renderer.WriteStream(text);
		}
	}
}
