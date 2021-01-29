namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal abstract class ComboElement : HtmlElement
	{
		private HtmlElement Element
		{
			get;
			set;
		}

		protected ComboElement(HtmlElement element)
		{
			Element = element;
		}

		public void Render(IOutputStream outputStream)
		{
			Element.Render(outputStream);
		}
	}
}
