using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class FontCache : IDisposable
	{
		private const int DEFAULT_EM_HEIGHT = 2048;

		internal static List<string> FontFallback = PopulateFontFallBack();

		internal static Dictionary<int, int> ScriptFontMapping = PopulateScriptFontMapping();

		private Dictionary<string, CachedFont> m_fontDict;

		private readonly float m_dpi;

		private Win32ObjectSafeHandle m_selectedFont = Win32ObjectSafeHandle.Zero;

		private Win32DCSafeHandle m_selectedHdc = Win32DCSafeHandle.Zero;

		private RPLFormat.WritingModes m_writingMode;

		private bool m_useEmSquare;

		private uint m_fontQuality;

		internal float Dpi => m_dpi;

		internal RPLFormat.WritingModes WritingMode
		{
			set
			{
				m_writingMode = value;
			}
		}

		internal bool VerticalMode
		{
			get
			{
				if (m_writingMode != RPLFormat.WritingModes.Vertical)
				{
					return m_writingMode == RPLFormat.WritingModes.Rotate270;
				}
				return true;
			}
		}

		internal bool UseEmSquare => m_useEmSquare;

		internal bool AllowVerticalFont
		{
			get
			{
				if (m_writingMode == RPLFormat.WritingModes.Vertical)
				{
					return true;
				}
				return false;
			}
		}

		public uint FontQuality
		{
			get
			{
				return m_fontQuality;
			}
			set
			{
				m_fontQuality = value;
			}
		}

		internal FontCache(float dpi)
		{
			m_fontDict = new Dictionary<string, CachedFont>();
			m_dpi = dpi;
		}

		internal FontCache(float dpi, bool useEmSquare)
			: this(dpi)
		{
			m_useEmSquare = useEmSquare;
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (m_fontDict != null)
			{
				foreach (CachedFont value in m_fontDict.Values)
				{
					value.Dispose();
				}
				m_fontDict = null;
			}
			ResetGraphics();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~FontCache()
		{
			Dispose(disposing: false);
		}

		internal void ResetGraphics()
		{
			m_selectedHdc = Win32DCSafeHandle.Zero;
			m_selectedFont = Win32ObjectSafeHandle.Zero;
		}

		internal CachedFont GetFont(ITextRunProps textRunProps, byte charset, bool verticalFont)
		{
			string fontFamily = null;
			float fontSize = 0f;
			CachedFont value = null;
			string runKey = GetRunKey(textRunProps, out fontFamily, out fontSize);
			string key = GetKey(runKey, charset, verticalFont, null, null);
			if (!m_fontDict.TryGetValue(key, out value))
			{
				return CreateFont(textRunProps, key, charset, verticalFont, fontFamily, fontSize);
			}
			return value;
		}

		internal CachedFont GetFallbackFont(ITextRunProps textRunProps, byte charset, int script, bool verticalFont)
		{
			string fontFamily = null;
			float fontSize = 0f;
			string runKey = GetRunKey(textRunProps, out fontFamily, out fontSize);
			int value = 0;
			if (!ScriptFontMapping.TryGetValue(script, out value))
			{
				value = 0;
			}
			fontFamily = FontFallback[value];
			string key = GetKey(runKey, charset, verticalFont, fontFamily, null);
			CachedFont value2 = null;
			if (!m_fontDict.TryGetValue(key, out value2))
			{
				return CreateFont(textRunProps, key, charset, verticalFont, fontFamily, fontSize);
			}
			return value2;
		}

		internal CachedFont GetFont(ITextRunProps textRunProps, byte charset, float fontSize, bool verticalFont)
		{
			string fontFamily = null;
			float fontSize2 = 0f;
			string runKey = GetRunKey(textRunProps, out fontFamily, out fontSize2);
			string key = GetKey(runKey, charset, verticalFont, null, fontSize);
			CachedFont value = null;
			if (!m_fontDict.TryGetValue(key, out value))
			{
				return CreateFont(textRunProps, key, charset, verticalFont, fontFamily, fontSize);
			}
			return value;
		}

		private CachedFont CreateFont(ITextRunProps textRun, string key, byte charset, bool verticalFont, string fontFamily, float fontSize)
		{
			CachedFont cachedFont = new CachedFont();
			m_fontDict.Add(key, cachedFont);
			bool bold = textRun.Bold;
			bool italic = textRun.Italic;
			bool lineThrough = textRun.TextDecoration == RPLFormat.TextDecorations.LineThrough;
			cachedFont.Font = CreateGdiPlusFont(fontFamily, textRun.FontSize, ref bold, ref italic, lineThrough, underLine: false);
			int num = 0;
			if (UseEmSquare)
			{
				int emHeight = cachedFont.Font.FontFamily.GetEmHeight(cachedFont.Font.Style);
				cachedFont.ScaleFactor = (float)emHeight / fontSize;
				num = emHeight;
			}
			else
			{
				num = (int)(fontSize + 0.5f);
			}
			cachedFont.Hfont = CreateGdiFont(m_writingMode, num, bold, italic, lineThrough, charset, verticalFont, cachedFont.Font.FontFamily.Name);
			return cachedFont;
		}

		private Win32ObjectSafeHandle CreateGdiFont(RPLFormat.WritingModes writingMode, int fontSize, bool bold, bool italic, bool lineThrough, byte charset, bool verticalFont, string fontFamily)
		{
			int nEscapement = 0;
			int nOrientation = 0;
			if (VerticalMode)
			{
				if (!UseEmSquare)
				{
					if (writingMode == RPLFormat.WritingModes.Vertical)
					{
						nEscapement = 2700;
						nOrientation = 2700;
					}
					else
					{
						nEscapement = 900;
						nOrientation = 900;
					}
				}
				if (verticalFont)
				{
					fontFamily = "@" + fontFamily;
				}
			}
			return Win32.CreateFont(-fontSize, 0, nEscapement, nOrientation, bold ? 700 : 400, italic ? 1u : 0u, 0u, lineThrough ? 1u : 0u, charset, 4u, 0u, m_fontQuality, 0u, fontFamily);
		}

		private string GetRunKey(ITextRunProps textRunProps, out string fontFamily, out float fontSize)
		{
			string text = textRunProps.FontKey;
			if (text == null)
			{
				text = (textRunProps.FontKey = GetKey(textRunProps, out fontFamily, out fontSize));
			}
			else
			{
				fontFamily = textRunProps.FontFamily;
				fontSize = textRunProps.FontSize * m_dpi / 72f;
			}
			return text;
		}

		private string GetKey(string runKey, int charset, bool verticalFont, string fontFamily, float? fontSize)
		{
			StringBuilder stringBuilder = new StringBuilder(runKey);
			stringBuilder.Append(fontFamily);
			if (fontSize.HasValue)
			{
				stringBuilder.Append(fontSize.Value.ToString(CultureInfo.InvariantCulture));
			}
			switch (m_writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				stringBuilder.Append('h');
				break;
			case RPLFormat.WritingModes.Vertical:
				stringBuilder.Append('v');
				break;
			case RPLFormat.WritingModes.Rotate270:
				stringBuilder.Append('r');
				break;
			}
			stringBuilder.Append(charset);
			stringBuilder.Append(verticalFont ? 't' : 'f');
			return stringBuilder.ToString();
		}

		private string GetKey(ITextRunProps textRunProps, out string fontFamily, out float fontSize)
		{
			fontSize = textRunProps.FontSize * m_dpi / 72f;
			fontFamily = textRunProps.FontFamily;
			StringBuilder stringBuilder = new StringBuilder(fontFamily);
			stringBuilder.Append(fontSize.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append(textRunProps.Bold ? 'b' : 'n');
			stringBuilder.Append(textRunProps.Italic ? 'i' : 'n');
			RPLFormat.TextDecorations textDecoration = textRunProps.TextDecoration;
			if (UseEmSquare && textDecoration == RPLFormat.TextDecorations.Underline)
			{
				stringBuilder.Append('u');
			}
			else
			{
				switch (textDecoration)
				{
				case RPLFormat.TextDecorations.Overline:
					stringBuilder.Append('o');
					break;
				case RPLFormat.TextDecorations.LineThrough:
					stringBuilder.Append('s');
					break;
				default:
					stringBuilder.Append('n');
					break;
				}
			}
			return stringBuilder.ToString();
		}

		internal void SelectFontObject(Win32DCSafeHandle hdc, Win32ObjectSafeHandle hFont)
		{
			if (hdc != m_selectedHdc)
			{
				m_selectedHdc = hdc;
				m_selectedFont = Win32ObjectSafeHandle.Zero;
			}
			if (hFont != m_selectedFont)
			{
				Win32.SelectObject(hdc, hFont).SetHandleAsInvalid();
				m_selectedFont = hFont;
			}
		}

		internal static Font CreateGdiPlusFont(string fontFamilyName, float fontSize, ref bool bold, ref bool italic, bool lineThrough, bool underLine)
		{
			FontStyle fontStyle = FontStyle.Regular;
			if (bold)
			{
				fontStyle |= FontStyle.Bold;
			}
			if (italic)
			{
				fontStyle |= FontStyle.Italic;
			}
			if (underLine)
			{
				fontStyle |= FontStyle.Underline;
			}
			else if (lineThrough)
			{
				fontStyle |= FontStyle.Strikeout;
			}
			Font font = null;
			try
			{
				try
				{
					font = new Font(fontFamilyName, fontSize, fontStyle);
				}
				catch (ArgumentException)
				{
					FontStyle fontStyle2 = FontStyle.Bold;
					if (italic)
					{
						fontStyle2 |= FontStyle.Italic;
					}
					if (underLine)
					{
						fontStyle2 |= FontStyle.Underline;
					}
					else if (lineThrough)
					{
						fontStyle2 |= FontStyle.Strikeout;
					}
					try
					{
						font = new Font(fontFamilyName, fontSize, fontStyle2);
						bold = true;
					}
					catch (ArgumentException)
					{
						fontStyle2 = FontStyle.Italic;
						if (bold)
						{
							fontStyle2 |= FontStyle.Bold;
						}
						if (underLine)
						{
							fontStyle2 |= FontStyle.Underline;
						}
						else if (lineThrough)
						{
							fontStyle2 |= FontStyle.Strikeout;
						}
						try
						{
							font = new Font(fontFamilyName, fontSize, fontStyle2);
						}
						catch (ArgumentException)
						{
							fontStyle2 = (FontStyle.Bold | FontStyle.Italic);
							if (underLine)
							{
								fontStyle2 |= FontStyle.Underline;
							}
							else if (lineThrough)
							{
								fontStyle2 |= FontStyle.Strikeout;
							}
							try
							{
								font = new Font(fontFamilyName, fontSize, fontStyle2);
							}
							catch (ArgumentException)
							{
								font = new Font(FontFamily.GenericSansSerif, fontSize, fontStyle);
							}
						}
					}
				}
				bold = font.Bold;
				italic = font.Italic;
				return font;
			}
			catch
			{
				if (font != null)
				{
					font.Dispose();
					font = null;
				}
				throw;
			}
		}

		private static List<string> PopulateFontFallBack()
		{
			return new List<string>(28)
			{
				"Microsoft Sans Serif",
				"Mangal",
				"Latha",
				"Estrangelo Edessa",
				"Sylfaen",
				"Raavi",
				"Shruti",
				"Tunga",
				"Gautami",
				"Mv Boli",
				"Vrinda",
				"Kartika",
				"Kalinga",
				"Microsoft Himalaya",
				"DokChampa",
				"DaunPenh",
				"Mongolian Baiti",
				"Nyala",
				"Plantagenet Cherokee",
				"Euphemia",
				"Iskoola Pota",
				"Microsoft Yi Baiti",
				"MS PGothic",
				"PMingLiu",
				"Gulim",
				"Microsoft Tai Le",
				"Microsoft New Tai Lue",
				"MingLiU-ExtB"
			};
		}

		private static Dictionary<int, int> PopulateScriptFontMapping()
		{
			return new Dictionary<int, int>(49)
			{
				{
					34,
					1
				},
				{
					35,
					1
				},
				{
					36,
					2
				},
				{
					37,
					2
				},
				{
					30,
					3
				},
				{
					9,
					4
				},
				{
					10,
					4
				},
				{
					41,
					5
				},
				{
					42,
					5
				},
				{
					43,
					6
				},
				{
					44,
					6
				},
				{
					45,
					6
				},
				{
					50,
					7
				},
				{
					51,
					7
				},
				{
					48,
					8
				},
				{
					49,
					8
				},
				{
					66,
					9
				},
				{
					38,
					10
				},
				{
					39,
					10
				},
				{
					40,
					10
				},
				{
					52,
					11
				},
				{
					53,
					11
				},
				{
					46,
					12
				},
				{
					47,
					12
				},
				{
					54,
					13
				},
				{
					55,
					13
				},
				{
					56,
					14
				},
				{
					57,
					14
				},
				{
					58,
					15
				},
				{
					59,
					15
				},
				{
					62,
					16
				},
				{
					63,
					16
				},
				{
					64,
					17
				},
				{
					65,
					17
				},
				{
					68,
					18
				},
				{
					67,
					19
				},
				{
					72,
					20
				},
				{
					21,
					21
				},
				{
					14,
					22
				},
				{
					15,
					22
				},
				{
					16,
					22
				},
				{
					17,
					23
				},
				{
					18,
					24
				},
				{
					19,
					24
				},
				{
					20,
					23
				},
				{
					22,
					23
				},
				{
					75,
					25
				},
				{
					76,
					26
				},
				{
					12,
					27
				}
			};
		}
	}
}
