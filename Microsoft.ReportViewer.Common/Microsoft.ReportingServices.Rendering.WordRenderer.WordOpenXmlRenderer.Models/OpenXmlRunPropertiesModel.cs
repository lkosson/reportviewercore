using System;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlRunPropertiesModel : OpenXmlParagraphModel.IParagraphContent
	{
		private bool _rightToLeft;

		private bool _rightToLeftStyle;

		private bool _bold;

		private bool _italic;

		private bool _strikethrough;

		private bool _underline;

		private Color? _color;

		private string _font;

		private string _language;

		private double _size;

		public Color Color
		{
			set
			{
				_color = value;
			}
		}

		public bool RightToLeft
		{
			set
			{
				_rightToLeft = value;
			}
		}

		public string Language
		{
			set
			{
				_language = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				_strikethrough = value;
			}
		}

		public bool Underline
		{
			set
			{
				_underline = value;
			}
		}

		public bool NoProof
		{
			private get;
			set;
		}

		public void SetBold(bool bold, bool rightToLeft)
		{
			_bold = bold;
			_rightToLeftStyle |= rightToLeft;
		}

		public void SetFont(string name, bool rightToLeft)
		{
			_font = WordOpenXmlUtils.Escape(name);
			_rightToLeftStyle |= rightToLeft;
		}

		public void SetItalic(bool italic, bool rightToLeft)
		{
			_italic = italic;
			_rightToLeftStyle |= rightToLeft;
		}

		public void SetSize(double size, bool rightToLeft)
		{
			_size = ((size < 1.0) ? 1.0 : size);
			_rightToLeftStyle |= rightToLeft;
		}

		public void Write(TextWriter q)
		{
			q.Write("<w:rPr>");
			if (_font != null)
			{
				q.Write("<w:rFonts w:ascii=\"");
				q.Write(_font);
				q.Write("\" w:hAnsi=\"");
				q.Write(_font);
				q.Write("\" w:eastAsia=\"");
				q.Write(_font);
				q.Write("\"");
				if (_rightToLeftStyle)
				{
					q.Write(" w:cs=\"");
					q.Write(_font);
					q.Write("\"");
				}
				q.Write("/>");
			}
			if (_bold)
			{
				q.Write("<w:b/>");
				if (_rightToLeftStyle)
				{
					q.Write("<w:bCs/>");
				}
			}
			if (_italic)
			{
				q.Write("<w:i/>");
				if (_rightToLeftStyle)
				{
					q.Write("<w:iCs/>");
				}
			}
			if (_strikethrough)
			{
				q.Write("<w:strike/>");
			}
			if (NoProof)
			{
				q.Write("<w:noProof/>");
			}
			if (_color.HasValue)
			{
				q.Write("<w:color w:val=\"");
				q.Write(WordOpenXmlUtils.RgbColor(_color.Value));
				q.Write("\"/>");
			}
			if (_size > 0.0)
			{
				q.Write("<w:sz w:val=\"");
				string value = Math.Min(3276uL, (ulong)Math.Round(_size * 2.0)).ToString(CultureInfo.InvariantCulture);
				q.Write(value);
				q.Write("\"/>");
				if (_rightToLeftStyle)
				{
					q.Write("<w:szCs w:val=\"");
					q.Write(value);
					q.Write("\"/>");
				}
			}
			if (_underline)
			{
				q.Write("<w:u w:val=\"single\"/>");
			}
			if (_rightToLeft)
			{
				if (_language != null)
				{
					q.Write("<w:lang");
					q.Write(" w:bidi=\"");
					q.Write(_language);
					q.Write("\"");
					q.Write("/>");
				}
				q.Write("<w:rtl/>");
			}
			q.Write("</w:rPr>");
		}
	}
}
