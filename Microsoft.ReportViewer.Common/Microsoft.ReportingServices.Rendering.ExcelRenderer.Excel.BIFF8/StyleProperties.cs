using Microsoft.ReportingServices.Rendering.RichText;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class StyleProperties
	{
		private static Dictionary<string, CharSet> m_charSetLookup = new Dictionary<string, CharSet>();

		private static object m_charSetLookupLock = new object();

		private const float DEFAULT_FONT_SIZE = 400f;

		private IColor m_backgroundColor;

		private int m_indentLevel;

		private bool m_wrapText = true;

		private int m_orientation;

		private string m_format;

		private HorizontalAlignment m_horizAlign;

		private VerticalAlignment m_vertAlign;

		private TextDirection m_textDir = TextDirection.LeftToRight;

		private ExcelBorderStyle m_borderLeftStyle;

		private ExcelBorderStyle m_borderRightStyle;

		private ExcelBorderStyle m_borderTopStyle;

		private ExcelBorderStyle m_borderBottomStyle;

		private ExcelBorderStyle m_borderDiagStyle;

		private IColor m_borderLeftColor;

		private IColor m_borderRightColor;

		private IColor m_borderTopColor;

		private IColor m_borderBottomColor;

		private IColor m_borderDiagColor;

		private ExcelBorderPart m_borderDiagPart = ExcelBorderPart.DiagonalBoth;

		private int m_bold = 400;

		private bool m_italic;

		private bool m_strikethrough;

		private Underline m_underline;

		private ScriptStyle m_scriptStyle;

		private IColor m_fontColor;

		private string m_fontName;

		private double m_fontSize = 10.0;

		private int m_ixfe;

		internal IColor BackgroundColor
		{
			get
			{
				return m_backgroundColor;
			}
			set
			{
				m_backgroundColor = value;
			}
		}

		internal int IndentLevel
		{
			get
			{
				return m_indentLevel;
			}
			set
			{
				m_indentLevel = value;
			}
		}

		internal bool WrapText
		{
			get
			{
				return m_wrapText;
			}
			set
			{
				m_wrapText = value;
			}
		}

		internal int Orientation
		{
			get
			{
				return m_orientation;
			}
			set
			{
				m_orientation = value;
			}
		}

		internal int Bold
		{
			get
			{
				return m_bold;
			}
			set
			{
				m_bold = value;
			}
		}

		internal bool Italic
		{
			get
			{
				return m_italic;
			}
			set
			{
				m_italic = value;
			}
		}

		internal bool Strikethrough
		{
			get
			{
				return m_strikethrough;
			}
			set
			{
				m_strikethrough = value;
			}
		}

		internal Underline Underline
		{
			get
			{
				return m_underline;
			}
			set
			{
				m_underline = value;
			}
		}

		internal ScriptStyle ScriptStyle
		{
			get
			{
				return m_scriptStyle;
			}
			set
			{
				m_scriptStyle = value;
			}
		}

		internal IColor Color
		{
			get
			{
				return m_fontColor;
			}
			set
			{
				m_fontColor = value;
			}
		}

		internal string Name
		{
			get
			{
				return m_fontName;
			}
			set
			{
				m_fontName = value;
				AddCharSet(m_fontName);
			}
		}

		internal double Size
		{
			get
			{
				return m_fontSize;
			}
			set
			{
				m_fontSize = value;
			}
		}

		internal CharSet CharSet => GetCharSet(m_fontName);

		internal string NumberFormat
		{
			get
			{
				return m_format;
			}
			set
			{
				m_format = value;
			}
		}

		internal HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return m_horizAlign;
			}
			set
			{
				m_horizAlign = value;
			}
		}

		internal VerticalAlignment VerticalAlignment
		{
			get
			{
				return m_vertAlign;
			}
			set
			{
				m_vertAlign = value;
			}
		}

		internal TextDirection TextDirection
		{
			get
			{
				return m_textDir;
			}
			set
			{
				m_textDir = value;
			}
		}

		internal ExcelBorderStyle BorderLeftStyle
		{
			get
			{
				return m_borderLeftStyle;
			}
			set
			{
				m_borderLeftStyle = value;
			}
		}

		internal ExcelBorderStyle BorderRightStyle
		{
			get
			{
				return m_borderRightStyle;
			}
			set
			{
				m_borderRightStyle = value;
			}
		}

		internal ExcelBorderStyle BorderTopStyle
		{
			get
			{
				return m_borderTopStyle;
			}
			set
			{
				m_borderTopStyle = value;
			}
		}

		internal ExcelBorderStyle BorderBottomStyle
		{
			get
			{
				return m_borderBottomStyle;
			}
			set
			{
				m_borderBottomStyle = value;
			}
		}

		internal ExcelBorderStyle BorderDiagStyle
		{
			get
			{
				return m_borderDiagStyle;
			}
			set
			{
				m_borderDiagStyle = value;
			}
		}

		internal IColor BorderLeftColor
		{
			get
			{
				return m_borderLeftColor;
			}
			set
			{
				m_borderLeftColor = value;
			}
		}

		internal IColor BorderRightColor
		{
			get
			{
				return m_borderRightColor;
			}
			set
			{
				m_borderRightColor = value;
			}
		}

		internal IColor BorderTopColor
		{
			get
			{
				return m_borderTopColor;
			}
			set
			{
				m_borderTopColor = value;
			}
		}

		internal IColor BorderBottomColor
		{
			get
			{
				return m_borderBottomColor;
			}
			set
			{
				m_borderBottomColor = value;
			}
		}

		internal IColor BorderDiagColor
		{
			get
			{
				return m_borderDiagColor;
			}
			set
			{
				m_borderDiagColor = value;
			}
		}

		internal ExcelBorderPart BorderDiagPart
		{
			get
			{
				return m_borderDiagPart;
			}
			set
			{
				m_borderDiagPart = value;
			}
		}

		internal int Ixfe
		{
			get
			{
				return m_ixfe;
			}
			set
			{
				m_ixfe = value;
			}
		}

		internal StyleProperties()
		{
		}

		private static void AddCharSet(string fontName)
		{
			if (fontName == null)
			{
				return;
			}
			lock (m_charSetLookupLock)
			{
				if (m_charSetLookup.ContainsKey(fontName))
				{
					return;
				}
				Font font = null;
				try
				{
					bool bold = false;
					bool italic = false;
					font = FontCache.CreateGdiPlusFont(fontName, 400f, ref bold, ref italic, lineThrough: false, underLine: false);
					LOGFONT lOGFONT = new LOGFONT();
					font.ToLogFont(lOGFONT);
					m_charSetLookup.Add(fontName, (CharSet)lOGFONT.lfCharSet);
				}
				finally
				{
					if (font != null)
					{
						font.Dispose();
						font = null;
					}
				}
			}
		}

		private static CharSet GetCharSet(string fontName)
		{
			if (fontName != null)
			{
				lock (m_charSetLookupLock)
				{
					if (m_charSetLookup.TryGetValue(fontName, out CharSet value))
					{
						return value;
					}
				}
			}
			return CharSet.DEFAULT_CHARSET;
		}
	}
}
