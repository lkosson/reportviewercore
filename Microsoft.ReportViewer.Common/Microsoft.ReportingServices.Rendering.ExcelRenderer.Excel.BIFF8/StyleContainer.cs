using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class StyleContainer : IStyle, IFont
	{
		private StyleState m_context;

		private Dictionary<string, StyleProperties> m_cache = new Dictionary<string, StyleProperties>();

		private List<BIFF8Font> m_fonts = new List<BIFF8Font>();

		private Dictionary<BIFF8Font, int> m_fontMap = new Dictionary<BIFF8Font, int>();

		private List<BIFF8Style> m_styles = new List<BIFF8Style>();

		private Dictionary<BIFF8Style, int> m_styleMap = new Dictionary<BIFF8Style, int>();

		private List<BIFF8Format> m_formats = new List<BIFF8Format>();

		private Dictionary<string, int> m_formatStringMap = new Dictionary<string, int>();

		private Dictionary<int, int> m_formatIntMap = new Dictionary<int, int>();

		private int m_currentCustomFormatIndex = 164;

		private ushort m_cellIxfe = 15;

		internal int CellIxfe
		{
			get
			{
				return m_cellIxfe;
			}
			set
			{
				m_cellIxfe = (ushort)value;
			}
		}

		public ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				CheckContext();
				m_context.BorderLeftStyle = value;
			}
		}

		public ExcelBorderStyle BorderRightStyle
		{
			set
			{
				CheckContext();
				m_context.BorderRightStyle = value;
			}
		}

		public ExcelBorderStyle BorderTopStyle
		{
			set
			{
				CheckContext();
				m_context.BorderTopStyle = value;
			}
		}

		public ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				CheckContext();
				m_context.BorderBottomStyle = value;
			}
		}

		public ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				CheckContext();
				m_context.BorderDiagStyle = value;
			}
		}

		public IColor BorderLeftColor
		{
			set
			{
				CheckContext();
				m_context.BorderLeftColor = value;
			}
		}

		public IColor BorderRightColor
		{
			set
			{
				CheckContext();
				m_context.BorderRightColor = value;
			}
		}

		public IColor BorderTopColor
		{
			set
			{
				CheckContext();
				m_context.BorderTopColor = value;
			}
		}

		public IColor BorderBottomColor
		{
			set
			{
				CheckContext();
				m_context.BorderBottomColor = value;
			}
		}

		public IColor BorderDiagColor
		{
			set
			{
				CheckContext();
				m_context.BorderDiagColor = value;
			}
		}

		public ExcelBorderPart BorderDiagPart
		{
			set
			{
				CheckContext();
				m_context.BorderDiagPart = value;
			}
		}

		public IColor BackgroundColor
		{
			set
			{
				if (value != null)
				{
					CheckContext();
					m_context.BackgroundColor = value;
				}
			}
		}

		public int IndentLevel
		{
			set
			{
				CheckContext();
				m_context.IndentLevel = value;
			}
		}

		public bool WrapText
		{
			set
			{
				CheckContext();
				m_context.WrapText = value;
			}
		}

		public int Orientation
		{
			set
			{
				CheckContext();
				m_context.Orientation = value;
			}
		}

		public string NumberFormat
		{
			get
			{
				if (m_context == null)
				{
					return null;
				}
				return m_context.NumberFormat;
			}
			set
			{
				CheckContext();
				m_context.NumberFormat = value;
			}
		}

		public HorizontalAlignment HorizontalAlignment
		{
			set
			{
				CheckContext();
				m_context.HorizontalAlignment = value;
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			set
			{
				CheckContext();
				m_context.VerticalAlignment = value;
			}
		}

		public TextDirection TextDirection
		{
			set
			{
				CheckContext();
				m_context.TextDirection = value;
			}
		}

		public int Bold
		{
			set
			{
				CheckContext();
				m_context.Bold = value;
			}
		}

		public bool Italic
		{
			set
			{
				CheckContext();
				m_context.Italic = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				CheckContext();
				m_context.Strikethrough = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
				CheckContext();
				m_context.ScriptStyle = value;
			}
		}

		public IColor Color
		{
			set
			{
				CheckContext();
				m_context.Color = value;
			}
		}

		public Underline Underline
		{
			set
			{
				CheckContext();
				m_context.Underline = value;
			}
		}

		public string Name
		{
			set
			{
				CheckContext();
				m_context.Name = value;
			}
		}

		public double Size
		{
			set
			{
				CheckContext();
				m_context.Size = value;
			}
		}

		internal StyleContainer()
		{
			AddBuiltInFormats();
		}

		internal void Finish()
		{
			if (m_context != null)
			{
				m_context.Finished();
			}
			else
			{
				m_cellIxfe = 15;
			}
			m_context = null;
		}

		internal void Reset()
		{
			m_context = null;
			m_cellIxfe = 15;
		}

		internal void DefineSharedStyle(string id)
		{
			m_context = new DefineSharedStyle(this, id);
		}

		internal bool UseSharedStyle(string id)
		{
			if (m_cache.ContainsKey(id))
			{
				m_context = new UseSharedStyle(this, GetSharedStyle(id));
				return true;
			}
			return false;
		}

		internal void SetContext(StyleState state)
		{
			m_context = state;
		}

		internal BIFF8Style GetStyle(int ixfe)
		{
			return m_styles[ixfe - 21];
		}

		internal string GetFormat(int ifmt)
		{
			if (m_formatIntMap.TryGetValue(ifmt, out int value))
			{
				return m_formats[value].String;
			}
			throw new ReportRenderingException(ExcelRenderRes.InvalidIndexException(ifmt.ToString(CultureInfo.InvariantCulture)));
		}

		internal BIFF8Font GetFont(int ifnt)
		{
			return m_fonts[ifnt - 5];
		}

		internal int AddStyle(BIFF8Style style)
		{
			if (!m_styleMap.TryGetValue(style, out int value))
			{
				value = m_styles.Count + 21;
				m_styleMap.Add(style, value);
				m_styles.Add(style);
			}
			return value;
		}

		internal int AddStyle(StyleProperties props)
		{
			BIFF8Style bIFF8Style = new BIFF8Style(props);
			BIFF8Font font = new BIFF8Font(props);
			bIFF8Style.Ifnt = AddFont(font);
			bIFF8Style.Ifmt = AddFormat(props.NumberFormat);
			return AddStyle(bIFF8Style);
		}

		internal int AddFormat(string format)
		{
			if (format == null || format.Length == 0)
			{
				return 0;
			}
			if (!m_formatStringMap.TryGetValue(format, out int value))
			{
				value = m_currentCustomFormatIndex;
				BIFF8Format item = new BIFF8Format(format, value);
				m_formatStringMap.Add(format, value);
				m_formatIntMap.Add(value, m_formats.Count);
				m_formats.Add(item);
				m_currentCustomFormatIndex++;
			}
			return value;
		}

		internal int AddFont(BIFF8Font font)
		{
			if (!m_fontMap.TryGetValue(font, out int value))
			{
				value = m_fonts.Count + 5;
				m_fontMap.Add(font, value);
				m_fonts.Add(font);
			}
			return value;
		}

		internal void AddSharedStyle(string id, StyleProperties style)
		{
			m_cache[id] = style;
		}

		internal StyleProperties GetSharedStyle(string id)
		{
			return m_cache[id];
		}

		internal void Write(BinaryWriter writer)
		{
			foreach (BIFF8Font font in m_fonts)
			{
				RecordFactory.FONT(writer, font);
			}
			m_fonts = null;
			m_fontMap = null;
			foreach (BIFF8Format format in m_formats)
			{
				RecordFactory.FORMAT(writer, format.String, format.Index);
			}
			m_formats = null;
			m_formatIntMap = null;
			m_formatStringMap = null;
			writer.BaseStream.Write(Constants.GLOBAL2, 0, Constants.GLOBAL2.Length);
			foreach (BIFF8Style style in m_styles)
			{
				RecordFactory.XF(writer, style.RecordData);
			}
			m_styles = null;
			m_styleMap = null;
		}

		private void AddBuiltInFormats()
		{
			BIFF8Format item = new BIFF8Format("General", 0);
			m_formats.Add(item);
			m_formatIntMap.Add(item.Index, 0);
			m_formatStringMap.Add(item.String, item.Index);
			m_formats.Add(new BIFF8Format("0", 1));
			m_formats.Add(new BIFF8Format("0.00", 2));
			m_formats.Add(new BIFF8Format("#,##0", 3));
			m_formats.Add(new BIFF8Format("#,##0.00", 4));
			m_formats.Add(new BIFF8Format("\"$\"#,##0_);\\(\"$\"#,##0\\)", 5));
			m_formats.Add(new BIFF8Format("\"$\"#,##0_);[Red]\\(\"$\"#,##0\\)", 6));
			m_formats.Add(new BIFF8Format("\"$\"#,##0.00_);\\(\"$\"#,##0.00\\)", 7));
			m_formats.Add(new BIFF8Format("\"$\"#,##0.00_);[Red]\\(\"$\"#,##0.00\\)", 8));
			m_formats.Add(new BIFF8Format("0%", 9));
			m_formats.Add(new BIFF8Format("0.00E+00", 11));
			m_formats.Add(new BIFF8Format("#?/?", 12));
			m_formats.Add(new BIFF8Format("#??/??", 13));
			m_formats.Add(new BIFF8Format("M/D/YY", 14));
			m_formats.Add(new BIFF8Format("D-MMM-YY", 15));
			m_formats.Add(new BIFF8Format("D-MMM", 16));
			m_formats.Add(new BIFF8Format("MMM-YY", 17));
			m_formats.Add(new BIFF8Format("h:mm AM/PM", 18));
			m_formats.Add(new BIFF8Format("h:mm:ss AM/PM", 19));
			m_formats.Add(new BIFF8Format("h:mm", 20));
			m_formats.Add(new BIFF8Format("h:mm:ss", 21));
			m_formats.Add(new BIFF8Format("M/D/YYYY h:mm", 22));
			m_formats.Add(new BIFF8Format("(#,##0_);(#,##0)", 37));
			m_formats.Add(new BIFF8Format("(#,##0_);[Red](#,##0)", 38));
			m_formats.Add(new BIFF8Format("(#,##0.00_);(#,##0.00)", 39));
			m_formats.Add(new BIFF8Format("(#,##0.00_);[Red](#,##0.00)", 40));
			m_formats.Add(new BIFF8Format("_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)", 41));
			m_formats.Add(new BIFF8Format("_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)", 42));
			m_formats.Add(new BIFF8Format("_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)", 43));
			m_formats.Add(new BIFF8Format("_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)", 44));
			m_formats.Add(new BIFF8Format("mm:ss", 45));
			m_formats.Add(new BIFF8Format("[h]:mm:ss", 46));
			m_formats.Add(new BIFF8Format("mm:ss.0", 47));
			m_formats.Add(new BIFF8Format("##0.0E+0", 48));
			m_formats.Add(new BIFF8Format("@", 49));
		}

		private void CheckContext()
		{
			if (m_context == null)
			{
				m_context = new InstanceStyle(this);
			}
		}
	}
}
