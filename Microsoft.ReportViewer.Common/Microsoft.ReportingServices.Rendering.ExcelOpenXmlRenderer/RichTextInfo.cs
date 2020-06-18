using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class RichTextInfo : IRichTextInfo
	{
		public delegate Style CreateStyle();

		private class CharacterRunFont
		{
			private Font _font;

			public int StartIndex
			{
				get;
				private set;
			}

			public int Length
			{
				get;
				private set;
			}

			public Font Font => _font;

			public CharacterRunFont(Font font, int startIndex, int length)
			{
				StartIndex = startIndex;
				Length = length;
				_font = font;
			}

			public void Expand(int additionalLength)
			{
				Length += additionalLength;
			}
		}

		private Cell _cell;

		private bool _checkForRotatedFarEastChars;

		private StringBuilder _text;

		private CreateStyle _createStyle;

		private readonly int _row;

		private readonly int _column;

		private List<CharacterRunFont> _fonts;

		private bool _foundRotatedEastAsianChar;

		public bool CheckForRotatedFarEastChars
		{
			set
			{
				_checkForRotatedFarEastChars = value;
			}
		}

		public bool FoundRotatedEastAsianChar => _foundRotatedEastAsianChar;

		public RichTextInfo(Cell cell, CreateStyle createStyle, int row, int column)
		{
			_cell = cell;
			_text = new StringBuilder();
			_createStyle = createStyle;
			_row = row;
			_column = column;
			_fonts = new List<CharacterRunFont>();
		}

		public void Commit(Style cellStyle)
		{
			if (_text.Length > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(_row.ToString(CultureInfo.InvariantCulture), _column.ToString(CultureInfo.InvariantCulture)));
			}
			_cell.Value = _text.ToString();
			bool flag = true;
			foreach (CharacterRunFont font in _fonts)
			{
				if (font.Length > 0)
				{
					if (flag)
					{
						cellStyle.Font.SetFont(font.Font);
						flag = false;
					}
					_cell.GetCharacters(font.StartIndex, font.Length).Font = font.Font;
				}
			}
		}

		private void AppendWithChecking(string value)
		{
			bool foundEastAsianChar = false;
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(value, _text, _checkForRotatedFarEastChars, out foundEastAsianChar);
			if (foundEastAsianChar)
			{
				_checkForRotatedFarEastChars = false;
				_foundRotatedEastAsianChar = true;
			}
		}

		private void ExpandLastRun(int length)
		{
			if (_fonts != null && _fonts.Count > 0)
			{
				_fonts[_fonts.Count - 1].Expand(length);
			}
		}

		public IFont AppendTextRun(string value)
		{
			return AppendTextRun(value, replaceInvalidWhitespace: true);
		}

		public IFont AppendTextRun(string value, bool replaceInvalidWhitespace)
		{
			int length = _text.Length;
			if (replaceInvalidWhitespace)
			{
				AppendWithChecking(value);
			}
			else
			{
				_text.Append(value);
			}
			CharacterRunFont characterRunFont = new CharacterRunFont(_createStyle().Font, length, _text.Length - length);
			_fonts.Add(characterRunFont);
			return characterRunFont.Font;
		}

		public void AppendText(string value)
		{
			AppendText(value, replaceInvalidWhiteSpace: true);
		}

		public void AppendText(string value, bool replaceInvalidWhiteSpace)
		{
			if (replaceInvalidWhiteSpace)
			{
				AppendWithChecking(value);
			}
			else
			{
				_text.Append(value);
			}
			ExpandLastRun(value.Length);
		}

		public void AppendText(char value)
		{
			_text.Append(value.ToString());
			ExpandLastRun(1);
		}
	}
}
