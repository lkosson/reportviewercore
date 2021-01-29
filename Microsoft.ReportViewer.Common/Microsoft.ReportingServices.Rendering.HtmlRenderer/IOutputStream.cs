namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IOutputStream
	{
		void Write(string text);

		void Write(byte[] text);
	}
}
