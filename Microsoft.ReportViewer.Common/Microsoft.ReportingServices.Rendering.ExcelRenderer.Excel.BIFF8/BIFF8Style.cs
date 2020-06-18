using System;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Style : ICloneable
	{
		private const int BLOCK1_Offset = 4;

		private const short BLOCK1_fLocked = 1;

		private const short BLOCK1_fHidden = 2;

		private const short BLOCK1_fStyle = 4;

		private const short BLOCK1_f123Prefix = 8;

		private const short BLOCK1_ixfParent = -16;

		private const int BLOCK2_Offset = 6;

		private const short BLOCK2_alc = 7;

		private const short BLOCK2_fWrap = 8;

		private const short BLOCK2_alcV = 112;

		private const short BLOCK2_fJustLast = 128;

		private const short BLOCK2_trot = -256;

		private const int BLOCK3_Offset = 8;

		private const short BLOCK3_cIndent = 15;

		private const short BLOCK3_fShrinkToFit = 16;

		private const short BLOCK3_fMergeCell = 32;

		private const short BLOCK3_iReadingOrder = 192;

		private const short BLOCK3_fAtrNum = 1024;

		private const short BLOCK3_fAtrFnt = 2048;

		private const short BLOCK3_fAtrAlc = 4096;

		private const short BLOCK3_fAtrBdr = 8192;

		private const short BLOCK3_fAtrPat = 16384;

		private const short BLOCK3_fAtrProt = short.MinValue;

		private const int BLOCK4_Offset = 10;

		private const short BLOCK4_dgLeft = 15;

		private const short BLOCK4_dgRight = 240;

		private const short BLOCK4_dgTop = 3840;

		private const short BLOCK4_dgBottom = -4096;

		private const int BLOCK5_Offset = 12;

		private const short BLOCK5_icvLeft = 127;

		private const short BLOCK5_icvRight = 16256;

		private const short BLOCK5_grbitDiag = -16384;

		private const int BLOCK6_Offset = 14;

		private const int BLOCK6_icvTop = 127;

		private const int BLOCK6_icvBottom = 16256;

		private const int BLOCK6_icvDiag = 2080768;

		private const int BLOCK6_dgDiag = 31457280;

		private const int BLOCK6_fls = -67108864;

		private const int BLOCK7_Offset = 18;

		private const short BLOCK7_icvFore = 127;

		private const short BLOCK7_icvBack = 16256;

		private const short BLOCK7_fSxButton = 16384;

		private byte[] m_xfData = new byte[20];

		private int m_hash;

		internal byte[] RecordData => m_xfData;

		internal int Ifnt
		{
			get
			{
				return LittleEndianHelper.ReadShortU(m_xfData, 0);
			}
			set
			{
				LittleEndianHelper.WriteShortU(value, m_xfData, 0);
			}
		}

		internal int Ifmt
		{
			get
			{
				return LittleEndianHelper.ReadShortU(m_xfData, 2);
			}
			set
			{
				LittleEndianHelper.WriteShortU(value, m_xfData, 2);
			}
		}

		internal ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				SetValue16(10, 15, (int)value);
			}
		}

		internal ExcelBorderStyle BorderRightStyle
		{
			set
			{
				SetValue16(10, 240, (int)value);
			}
		}

		internal ExcelBorderStyle BorderTopStyle
		{
			set
			{
				SetValue16(10, 3840, (int)value);
			}
		}

		internal ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				SetValue16(10, -4096, (int)value);
			}
		}

		internal ExcelBorderStyle BorderOutlineStyle
		{
			set
			{
				BorderLeftStyle = value;
				BorderRightStyle = value;
				BorderTopStyle = value;
				BorderBottomStyle = value;
			}
		}

		internal ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				SetValue32(14, 31457280, (int)value);
			}
		}

		internal IColor BorderLeftColor
		{
			set
			{
				SetValue16(12, 127, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderRightColor
		{
			set
			{
				SetValue16(12, 16256, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderTopColor
		{
			set
			{
				SetValue32(14, 127, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderBottomColor
		{
			set
			{
				SetValue32(14, 16256, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderOutlineColor
		{
			set
			{
				BorderLeftColor = value;
				BorderRightColor = value;
				BorderBottomColor = value;
				BorderTopColor = value;
			}
		}

		internal IColor BorderDiagColor
		{
			set
			{
				SetValue32(14, 2080768, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal ExcelBorderPart BorderDiagPart
		{
			set
			{
				switch (value)
				{
				case ExcelBorderPart.DiagonalBoth:
					SetValue16(12, -16384, 0);
					break;
				case ExcelBorderPart.DiagonalDown:
					SetValue16(12, -16384, 1);
					break;
				case ExcelBorderPart.DiagonalUp:
					SetValue16(12, -16384, 2);
					break;
				}
			}
		}

		internal IColor BackgroundColor
		{
			set
			{
				int paletteIndex = ((BIFF8Color)value).PaletteIndex;
				SetValue32(14, -67108864, 1);
				SetValue16(18, 127, (short)paletteIndex);
			}
		}

		internal int IndentLevel
		{
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value > 15)
				{
					value = 15;
				}
				SetValue16(8, 15, (short)value);
			}
		}

		internal bool WrapText
		{
			set
			{
				SetValue16(6, 8, (short)(value ? 1 : 0));
			}
		}

		internal int Orientation
		{
			set
			{
				if (value > 90)
				{
					if (value != 255)
					{
						value = 90;
					}
				}
				else if (value < 0)
				{
					if (value < -90)
					{
						value = -90;
					}
					value = 90 + Math.Abs(value);
					if (value > 180)
					{
						value = 180;
					}
				}
				SetValue16(6, -256, (short)value);
			}
		}

		internal HorizontalAlignment HorizontalAlignment
		{
			set
			{
				SetValue16(6, 7, (short)value);
			}
		}

		internal VerticalAlignment VerticalAlignment
		{
			set
			{
				SetValue16(6, 112, (short)value);
			}
		}

		internal TextDirection TextDirection
		{
			set
			{
				short value2 = 0;
				if (value == TextDirection.LeftToRight)
				{
					value2 = 1;
				}
				if (value == TextDirection.RightToLeft)
				{
					value2 = 2;
				}
				SetValue16(8, 192, value2);
			}
		}

		internal BIFF8Style()
		{
			WrapText = true;
			VerticalAlignment = VerticalAlignment.Top;
		}

		internal BIFF8Style(StyleProperties props)
			: this()
		{
			if (props.BackgroundColor != null)
			{
				BackgroundColor = props.BackgroundColor;
			}
			if (props.BorderBottomColor != null)
			{
				BorderBottomColor = props.BorderBottomColor;
			}
			BorderBottomStyle = props.BorderBottomStyle;
			if (props.BorderDiagColor != null)
			{
				BorderDiagColor = props.BorderDiagColor;
			}
			BorderDiagStyle = props.BorderDiagStyle;
			BorderDiagPart = props.BorderDiagPart;
			if (props.BorderLeftColor != null)
			{
				BorderLeftColor = props.BorderLeftColor;
			}
			BorderLeftStyle = props.BorderLeftStyle;
			if (props.BorderRightColor != null)
			{
				BorderRightColor = props.BorderRightColor;
			}
			BorderRightStyle = props.BorderRightStyle;
			if (props.BorderTopColor != null)
			{
				BorderTopColor = props.BorderTopColor;
			}
			BorderTopStyle = props.BorderTopStyle;
			HorizontalAlignment = props.HorizontalAlignment;
			IndentLevel = props.IndentLevel;
			Orientation = props.Orientation;
			TextDirection = props.TextDirection;
			VerticalAlignment = props.VerticalAlignment;
			WrapText = props.WrapText;
		}

		private void SetValue16(int offset, short mask, int value)
		{
			LittleEndianHelper.WriteShort(BitField16.PutValue(LittleEndianHelper.ReadShort(m_xfData, offset), mask, (short)value), m_xfData, offset);
		}

		private void SetValue32(int offset, int mask, int value)
		{
			LittleEndianHelper.WriteInt(BitField32.PutValue(LittleEndianHelper.ReadInt(m_xfData, offset), mask, value), m_xfData, offset);
		}

		public override bool Equals(object target)
		{
			BIFF8Style bIFF8Style = (BIFF8Style)target;
			for (int i = 0; i < m_xfData.Length; i++)
			{
				byte num = m_xfData[i];
				byte b = bIFF8Style.m_xfData[i];
				if (num != b)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (m_hash == 0)
			{
				for (int i = 0; i < m_xfData.Length; i++)
				{
					m_hash ^= m_xfData[i] << i;
				}
			}
			return m_hash;
		}

		public object Clone()
		{
			BIFF8Style obj = (BIFF8Style)MemberwiseClone();
			obj.m_xfData = (byte[])m_xfData.Clone();
			obj.m_hash = 0;
			return obj;
		}
	}
}
