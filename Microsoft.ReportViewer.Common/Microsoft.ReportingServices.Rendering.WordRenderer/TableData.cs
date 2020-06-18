using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class TableData
	{
		internal enum Positions
		{
			Top,
			Left,
			Bottom,
			Right
		}

		internal static byte TC_ROTATEFONT = 16;

		internal static byte TC_BACKWARD = 8;

		internal static byte TC_VERTICAL = 4;

		private const int DefaultOffset = 14;

		private const int BrcSize = 8;

		private const int Brc97Size = 4;

		private const int MaxTapxSprmSize = 2048;

		private CellBorderColor m_borderColors;

		private CellShading m_cellShading;

		private CellPadding m_cellPadding;

		private SprmBuffer m_tapx;

		private int m_numColumns;

		private float[] m_columnWidths;

		private BorderCode[] m_tableBorders;

		private BorderCode[] m_cellBorders;

		private byte[] m_tableShd;

		private int m_startOffset;

		private int m_rowHeight;

		private bool m_writeRowHeight = true;

		private bool m_writeExactRowHeight;

		private bool m_layoutTable;

		internal bool WriteRowHeight
		{
			get
			{
				return m_writeRowHeight;
			}
			set
			{
				m_writeRowHeight = value;
			}
		}

		internal bool WriteExactRowHeight
		{
			get
			{
				return m_writeExactRowHeight;
			}
			set
			{
				m_writeExactRowHeight = value;
			}
		}

		internal byte[] Tapx
		{
			get
			{
				m_cellPadding.Finish();
				m_rowHeight -= m_cellPadding.HeightAdjustment;
				m_rowHeight = Math.Min(m_rowHeight, 31680);
				if (WriteRowHeight && m_rowHeight > 0)
				{
					short num = (short)m_rowHeight;
					if (WriteExactRowHeight && num > 0)
					{
						num = (short)(num * -1);
					}
					m_tapx.AddSprm(37895, num, null);
				}
				byte[] array = new byte[m_tapx.Offset + m_borderColors.SprmSize + m_cellShading.SprmSize + m_cellPadding.SprmSize];
				Array.Copy(m_tapx.Buf, array, m_tapx.Offset);
				int offset = m_tapx.Offset;
				byte[] array2 = m_borderColors.ToByteArray();
				Array.Copy(array2, 0, array, offset, array2.Length);
				offset += array2.Length;
				byte[] array3 = m_cellShading.ToByteArray();
				Array.Copy(array3, 0, array, offset, array3.Length);
				offset += array3.Length;
				byte[] array4 = m_cellPadding.ToByteArray();
				Array.Copy(array4, 0, array, offset, array4.Length);
				return array;
			}
		}

		internal TableData(int tableLevel, bool layoutTable)
		{
			m_tapx = new SprmBuffer(2048, 2);
			m_tapx.AddSprm(9238, 1, null);
			m_tapx.AddSprm((ushort)((tableLevel > 1) ? 9292 : 9239), 1, null);
			m_tapx.AddSprm(26185, tableLevel, null);
			m_tableBorders = new BorderCode[4];
			m_tableBorders[0] = new BorderCode();
			m_tableBorders[1] = new BorderCode();
			m_tableBorders[2] = new BorderCode();
			m_tableBorders[3] = new BorderCode();
			m_cellBorders = new BorderCode[4];
			m_cellBorders[0] = new BorderCode();
			m_cellBorders[1] = new BorderCode();
			m_cellBorders[2] = new BorderCode();
			m_cellBorders[3] = new BorderCode();
			m_layoutTable = layoutTable;
		}

		internal void InitTableRow(float leftStart, float rowHeight, float[] columnWidths, AutoFit autoFit)
		{
			if (m_tapx.Offset == 14)
			{
				float num = 0f;
				foreach (float num2 in columnWidths)
				{
					num += num2;
				}
				if (num / 25.4f > 22f)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The maximum page width of the report exceeds 22 inches (55.88 centimeters).");
				}
				else if (columnWidths.Length > 63)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The rendered report contains a table that has more than 63 columns.");
				}
				m_columnWidths = columnWidths;
				m_numColumns = columnWidths.Length;
				m_borderColors = new CellBorderColor(m_numColumns);
				m_cellShading = new CellShading(m_numColumns, m_tableShd);
				m_cellPadding = new CellPadding(m_numColumns);
				CreateTableDefSprm(leftStart);
				if (autoFit != AutoFit.Never)
				{
					m_tapx.AddSprm(13845, (int)autoFit, null);
				}
				if (m_tableShd != null)
				{
					m_tapx.AddSprm(54880, 0, m_tableShd);
				}
				m_startOffset = m_tapx.Offset;
			}
			else
			{
				m_tapx.Clear(m_startOffset, m_tapx.Buf.Length - m_startOffset);
				int tcLocation = GetTcLocation(m_numColumns, 0);
				m_tapx.Clear(tcLocation, m_numColumns * 20);
				m_borderColors.Reset();
				m_cellShading.Reset();
				m_cellPadding.Reset();
				m_tapx.Reset(m_startOffset);
			}
			m_rowHeight = Word97Writer.ToTwips(rowHeight);
			WriteExactRowHeight = false;
			WriteRowHeight = true;
		}

		private void CreateTableDefSprm(float leftStart)
		{
			int num = 1 + 2 * (m_numColumns + 1) + 20 * m_numColumns;
			byte[] array = new byte[num + 4];
			int num2 = 0;
			LittleEndian.PutUShort(array, num2, 54792);
			num2 += 2;
			LittleEndian.PutUShort(array, num2, (ushort)(num + 1));
			num2 += 2;
			array[num2++] = (byte)m_numColumns;
			ushort num3 = Word97Writer.ToTwips(leftStart);
			if (num3 > 31680)
			{
				num3 = 31680;
			}
			LittleEndian.PutUShort(array, num2, num3);
			num2 += 2;
			for (int i = 0; i < m_numColumns; i++)
			{
				ushort num4 = Word97Writer.ToTwips(m_columnWidths[i]);
				if (num4 == 0)
				{
					num4 = 1;
				}
				num3 = (ushort)(num3 + num4);
				if (num3 > 31680)
				{
					num3 = 31680;
				}
				LittleEndian.PutUShort(array, num2, num3);
				num2 += 2;
			}
			m_tapx.AddRawSprmData(array);
		}

		internal void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			int tcLocation = GetTcLocation(numColumns, cellIndex);
			m_tapx.Buf[tcLocation] |= (byte)(firstVertMerge ? 96 : 0);
			m_tapx.Buf[tcLocation] |= (byte)(vertMerge ? 32 : 0);
			m_tapx.Buf[tcLocation] |= (byte)(firstHorzMerge ? 1 : 0);
			m_tapx.Buf[tcLocation] |= (byte)(horzMerge ? 2 : 0);
		}

		internal void AddCellStyleProp(int cellIndex, byte code, object value)
		{
			switch (code)
			{
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 27:
			case 28:
			case 29:
			case 31:
			case 32:
			case 33:
			case 35:
				break;
			case 0:
				SetCellBorderColor(cellIndex, (string)value);
				break;
			case 1:
				SetCellBorderColor(cellIndex, (string)value, Positions.Left);
				break;
			case 2:
				SetCellBorderColor(cellIndex, (string)value, Positions.Right);
				break;
			case 3:
				SetCellBorderColor(cellIndex, (string)value, Positions.Top);
				break;
			case 4:
				SetCellBorderColor(cellIndex, (string)value, Positions.Bottom);
				break;
			case 5:
				SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value);
				break;
			case 6:
				SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Left);
				break;
			case 7:
				SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Right);
				break;
			case 8:
				SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Top);
				break;
			case 9:
				SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Bottom);
				break;
			case 10:
				SetCellBorderWidth(cellIndex, (string)value);
				break;
			case 11:
				SetCellBorderWidth(cellIndex, (string)value, Positions.Left);
				break;
			case 12:
				SetCellBorderWidth(cellIndex, (string)value, Positions.Right);
				break;
			case 13:
				SetCellBorderWidth(cellIndex, (string)value, Positions.Top);
				break;
			case 14:
				SetCellBorderWidth(cellIndex, (string)value, Positions.Bottom);
				break;
			case 26:
				RenderVerticalAlign(cellIndex, (RPLFormat.VerticalAlignments)value);
				break;
			case 30:
				RenderWritingMode(cellIndex, (RPLFormat.WritingModes)value);
				break;
			case 34:
				SetTableCellShading(cellIndex, (string)value);
				break;
			}
		}

		public void AddPadding(int cellIndex, byte code, object value, int defaultValue)
		{
			int twips = defaultValue;
			if (value != null)
			{
				twips = Word97Writer.ToTwips((string)value);
			}
			switch (code)
			{
			case 15:
				m_cellPadding.SetPaddingLeft(cellIndex, twips);
				break;
			case 16:
				m_cellPadding.SetPaddingRight(cellIndex, twips);
				break;
			case 17:
				m_cellPadding.SetPaddingTop(cellIndex, twips);
				break;
			case 18:
				m_cellPadding.SetPaddingBottom(cellIndex, twips);
				break;
			}
		}

		internal void AddCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp)
		{
			BorderCode borderCode = new BorderCode();
			byte b = (byte)(slantUp ? 32 : 16);
			borderCode.Ico24 = Word97Writer.ToIco24(color);
			double num = Word97Writer.ToPoints(width);
			borderCode.LineWidth = (byte)(num * 8.0);
			borderCode.Style = ConvertBorderStyle(style);
			byte[] array = new byte[11]
			{
				(byte)cellIndex,
				(byte)(cellIndex + 1),
				b,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			borderCode.Serialize2K3(array, 3);
			m_tapx.AddSprm(54831, 0, array);
		}

		private void SetCellBorderWidth(int cellIndex, string width, Positions position)
		{
			double num = Word97Writer.ToPoints(width);
			m_cellBorders[(int)position].LineWidth = (int)(num * 8.0);
		}

		private void SetCellBorderWidth(int cellIndex, string width)
		{
			byte lineWidth = (byte)(Word97Writer.ToPoints(width) * 8.0);
			for (int i = 0; i < m_cellBorders.Length; i++)
			{
				m_cellBorders[i].LineWidth = lineWidth;
			}
		}

		private void SetCellBorderStyle(int cellIndex, RPLFormat.BorderStyles borderStyle, Positions position)
		{
			m_cellBorders[(int)position].Style = ConvertBorderStyle(borderStyle);
		}

		private void SetCellBorderStyle(int cellIndex, RPLFormat.BorderStyles borderStyle)
		{
			LineStyle style = ConvertBorderStyle(borderStyle);
			for (int i = 0; i < m_cellBorders.Length; i++)
			{
				m_cellBorders[i].Style = style;
			}
		}

		private void SetCellBorderColor(int cellIndex, string color)
		{
			int ico = Word97Writer.ToIco24(color);
			for (int i = 0; i < m_cellBorders.Length; i++)
			{
				m_cellBorders[i].Ico24 = ico;
			}
		}

		private void SetCellBorderColor(int cellIndex, string color, Positions position)
		{
			int ico = Word97Writer.ToIco24(color);
			m_cellBorders[(int)position].Ico24 = ico;
		}

		private void SetTableCellShading(int index, string color)
		{
			if (!color.Equals("Transparent"))
			{
				int ico = Word97Writer.ToIco24(color);
				m_cellShading.SetCellShading(index, ico);
			}
		}

		internal void AddTableStyleProp(byte code, object value)
		{
			switch (code)
			{
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 35:
			case 36:
			case 37:
				break;
			case 0:
				SetDefaultBorderColor((string)value);
				break;
			case 1:
				SetBorderColor((string)value, Positions.Left);
				break;
			case 2:
				SetBorderColor((string)value, Positions.Right);
				break;
			case 3:
				SetBorderColor((string)value, Positions.Top);
				break;
			case 4:
				SetBorderColor((string)value, Positions.Bottom);
				break;
			case 5:
				SetDefaultBorderStyle((RPLFormat.BorderStyles)value);
				break;
			case 6:
				SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Left);
				break;
			case 7:
				SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Right);
				break;
			case 8:
				SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Top);
				break;
			case 9:
				SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Bottom);
				break;
			case 10:
				SetDefaultBorderWidth((string)value);
				break;
			case 11:
				SetBorderWidth((string)value, Positions.Left);
				break;
			case 12:
				SetBorderWidth((string)value, Positions.Right);
				break;
			case 13:
				SetBorderWidth((string)value, Positions.Top);
				break;
			case 14:
				SetBorderWidth((string)value, Positions.Bottom);
				break;
			case 34:
				SetTableShading((string)value);
				break;
			}
		}

		internal void SetTableContext(BorderContext borderContext)
		{
			if (borderContext.Top)
			{
				m_tableBorders[0] = new BorderCode();
			}
			if (borderContext.Left)
			{
				m_tableBorders[1] = new BorderCode();
			}
			if (borderContext.Bottom)
			{
				m_tableBorders[2] = new BorderCode();
			}
			if (borderContext.Right)
			{
				m_tableBorders[3] = new BorderCode();
			}
		}

		private void SetDefaultBorderColor(string color)
		{
			int color2 = Word97Writer.ToIco24(color);
			for (int i = 0; i < m_tableBorders.Length; i++)
			{
				m_tableBorders[i].SetColor(color2);
			}
		}

		private void SetTableShading(string color)
		{
			if (!color.Equals("Transparent"))
			{
				int val = Word97Writer.ToIco24(color);
				m_tableShd = new byte[10];
				LittleEndian.PutInt(m_tableShd, 4, val);
			}
		}

		private void SetBorderWidth(string width, Positions position)
		{
			m_tableBorders[(int)position].LineWidth = (int)(Word97Writer.ToPoints(width) * 8.0);
		}

		private void SetDefaultBorderWidth(string width)
		{
			for (int i = 0; i < m_tableBorders.Length; i++)
			{
				m_tableBorders[i].LineWidth = (int)(Word97Writer.ToPoints(width) * 8.0);
			}
		}

		private void SetBorderStyle(RPLFormat.BorderStyles style, Positions position)
		{
			m_tableBorders[(int)position].Style = ConvertBorderStyle(style);
		}

		private void SetDefaultBorderStyle(RPLFormat.BorderStyles style)
		{
			for (int i = 0; i < m_tableBorders.Length; i++)
			{
				m_tableBorders[i].Style = ConvertBorderStyle(style);
			}
		}

		private void SetBorderColor(string color, Positions position)
		{
			int color2 = Word97Writer.ToIco24(color);
			m_tableBorders[(int)position].SetColor(color2);
		}

		private void RenderVerticalAlign(int cellIndex, RPLFormat.VerticalAlignments vertAlign)
		{
			int num = 0;
			switch (vertAlign)
			{
			case RPLFormat.VerticalAlignments.Bottom:
				num = 2;
				break;
			case RPLFormat.VerticalAlignments.Middle:
				num = 1;
				break;
			}
			int tcLocation = GetTcLocation(m_numColumns, cellIndex);
			ushort uShort = LittleEndian.getUShort(m_tapx.Buf, tcLocation);
			uShort = (ushort)(uShort | (ushort)(num << 7));
			LittleEndian.PutUShort(m_tapx.Buf, tcLocation, uShort);
		}

		private void RenderWritingMode(int cellIndex, RPLFormat.WritingModes writingModes)
		{
			if (writingModes == RPLFormat.WritingModes.Vertical || writingModes == RPLFormat.WritingModes.Rotate270)
			{
				int tcLocation = GetTcLocation(m_numColumns, cellIndex);
				ushort uShort = LittleEndian.getUShort(m_tapx.Buf, tcLocation);
				uShort = (ushort)(uShort | (byte)((writingModes == RPLFormat.WritingModes.Vertical) ? (TC_ROTATEFONT | TC_VERTICAL) : (TC_BACKWARD | TC_VERTICAL)));
				LittleEndian.PutUShort(m_tapx.Buf, tcLocation, uShort);
			}
		}

		internal void WriteBrc97(byte[] grpprl, int offset, BorderCode brc)
		{
			brc.Serialize97(grpprl, offset);
		}

		private int GetTcLocation(int numColumns, int cellIndex)
		{
			return 19 + 2 * (numColumns + 1) + 20 * cellIndex;
		}

		private LineStyle ConvertBorderStyle(RPLFormat.BorderStyles style)
		{
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				return LineStyle.DashSmallGap;
			case RPLFormat.BorderStyles.Dotted:
				return LineStyle.Dot;
			case RPLFormat.BorderStyles.Double:
				return LineStyle.Double;
			case RPLFormat.BorderStyles.None:
				return LineStyle.None;
			case RPLFormat.BorderStyles.Solid:
				return LineStyle.Single;
			default:
				return LineStyle.None;
			}
		}

		internal void WriteTableCellEnd(int cellIndex, BorderContext borderContext)
		{
			int offset = GetTcLocation(m_numColumns, cellIndex) + 4;
			UpdateBorderColor(Positions.Top, offset, cellIndex, borderContext.Top);
			UpdateBorderColor(Positions.Left, offset, cellIndex, borderContext.Left);
			UpdateBorderColor(Positions.Bottom, offset, cellIndex, borderContext.Bottom);
			UpdateBorderColor(Positions.Right, offset, cellIndex, borderContext.Right);
		}

		private void UpdateBorderColor(Positions position, int offset, int cellIndex, bool borderContext)
		{
			if (!borderContext)
			{
				if (!m_cellBorders[(int)position].Empty)
				{
					m_cellBorders[(int)position].Serialize97(m_tapx.Buf, offset + (int)position * 4);
					m_borderColors.SetColor(position, cellIndex, m_cellBorders[(int)position].Ico24);
				}
			}
			else if (!m_tableBorders[(int)position].Empty)
			{
				m_tableBorders[(int)position].Serialize97(m_tapx.Buf, offset + (int)position * 4);
				m_borderColors.SetColor(position, cellIndex, m_tableBorders[(int)position].Ico24);
			}
			m_cellBorders[(int)position] = new BorderCode();
		}

		internal void ClearCellBorder(Positions position)
		{
			m_cellBorders[(int)position] = new BorderCode();
		}
	}
}
