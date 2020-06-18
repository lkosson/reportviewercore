using System;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Font : ICloneable, IFont
	{
		private const short GRBIT_fItalic = 2;

		private const short GRBIT_fStrikeout = 8;

		private const short GRBIT_fOutline = 16;

		private const short GRBIT_fShadow = 32;

		private static readonly byte[] DEFAULT_DATA = new byte[14]
		{
			200,
			0,
			0,
			0,
			255,
			127,
			144,
			1,
			0,
			0,
			0,
			0,
			1,
			0
		};

		private int m_hash;

		private byte[] m_data = new byte[14];

		private string m_fontName = "Arial";

		internal byte[] RecordData => m_data;

		public int Bold
		{
			get
			{
				return LittleEndianHelper.ReadShortU(m_data, 6);
			}
			set
			{
				LittleEndianHelper.WriteShortU(value, m_data, 6);
			}
		}

		public bool Italic
		{
			get
			{
				return BitField16.GetValue(LittleEndianHelper.ReadShort(m_data, 2), 2) == 1;
			}
			set
			{
				WriteMaskedValue(2, 2, (short)(value ? 1 : 0));
			}
		}

		public bool Strikethrough
		{
			get
			{
				return BitField16.GetValue(LittleEndianHelper.ReadShort(m_data, 2), 8) == 1;
			}
			set
			{
				WriteMaskedValue(2, 8, (short)(value ? 1 : 0));
			}
		}

		public ScriptStyle ScriptStyle
		{
			get
			{
				return (ScriptStyle)LittleEndianHelper.ReadShort(m_data, 8);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, m_data, 8);
			}
		}

		public int Color
		{
			get
			{
				return LittleEndianHelper.ReadShort(m_data, 4);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, m_data, 4);
			}
		}

		IColor IFont.Color
		{
			set
			{
				Color = ((BIFF8Color)value).PaletteIndex;
			}
		}

		public Underline Underline
		{
			get
			{
				return (Underline)LittleEndianHelper.ReadShort(m_data, 10);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, m_data, 10);
			}
		}

		public CharSet CharSet
		{
			get
			{
				return (CharSet)LittleEndianHelper.ReadShort(m_data, 12);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, m_data, 12);
			}
		}

		public string Name
		{
			get
			{
				return m_fontName;
			}
			set
			{
				m_fontName = value;
			}
		}

		public double Size
		{
			get
			{
				return LittleEndianHelper.ReadShortU(m_data, 0) / 20;
			}
			set
			{
				LittleEndianHelper.WriteShortU((ushort)(value * 20.0), m_data, 0);
			}
		}

		internal BIFF8Font()
		{
			Array.Copy(DEFAULT_DATA, m_data, 14);
		}

		internal BIFF8Font(StyleProperties props)
			: this()
		{
			Bold = props.Bold;
			if (props.Color != null)
			{
				Color = ((BIFF8Color)props.Color).PaletteIndex;
			}
			Size = props.Size;
			Italic = props.Italic;
			if (props.Name != null)
			{
				Name = props.Name;
			}
			CharSet = props.CharSet;
			ScriptStyle = props.ScriptStyle;
			Strikethrough = props.Strikethrough;
			Underline = props.Underline;
		}

		private void WriteMaskedValue(int offset, short mask, short value)
		{
			LittleEndianHelper.WriteShort(BitField16.PutValue(LittleEndianHelper.ReadShort(m_data, offset), mask, value), m_data, offset);
		}

		public override bool Equals(object target)
		{
			BIFF8Font bIFF8Font = (BIFF8Font)target;
			if (bIFF8Font.m_fontName.Equals(m_fontName))
			{
				for (int i = 0; i < m_data.Length; i++)
				{
					if (m_data[i] != bIFF8Font.m_data[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (m_hash == 0)
			{
				m_hash = m_fontName.GetHashCode();
				for (int i = 0; i < m_data.Length; i++)
				{
					m_hash ^= m_data[i] << i;
				}
			}
			return m_hash;
		}

		public object Clone()
		{
			BIFF8Font obj = (BIFF8Font)MemberwiseClone();
			obj.m_data = (byte[])m_data.Clone();
			obj.m_hash = 0;
			return obj;
		}
	}
}
