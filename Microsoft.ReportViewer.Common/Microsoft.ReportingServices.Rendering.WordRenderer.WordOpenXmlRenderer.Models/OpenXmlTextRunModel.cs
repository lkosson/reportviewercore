using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal class OpenXmlTextRunModel : OpenXmlParagraphModel.IParagraphContent
	{
		private string _text;

		private bool _startsWithBreak;

		private OpenXmlRunPropertiesModel _properties;

		public OpenXmlTextRunModel(string text, bool startsWithBreak, OpenXmlRunPropertiesModel properties)
		{
			_text = text;
			_properties = properties;
			_startsWithBreak = startsWithBreak;
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:r>");
			_properties.Write(writer);
			if (_startsWithBreak)
			{
				writer.Write("<w:br/>");
			}
			if (!string.IsNullOrEmpty(_text))
			{
				writer.Write("<w:t xml:space=\"preserve\">");
				writer.Write(_text);
				writer.Write("</w:t>");
			}
			writer.Write("</w:r>");
		}
	}
}
