using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class RowHandler
	{
		internal enum TransformResult
		{
			NotHandled,
			Handled,
			Null
		}

		private BinaryWriter m_stream;

		private int m_row;

		private byte m_counter;

		private ushort[] m_xfs = new ushort[256];

		private uint[] m_rks = new uint[256];

		private short m_recType;

		private short m_valueCol1;

		private short m_valueCol2;

		private CellRenderingDetails m_details;

		private static readonly DateTime Epoch = new DateTime(1900, 1, 1, 0, 0, 0);

		private SSTHandler m_stringHandler;

		internal int Row
		{
			get
			{
				return m_row;
			}
			set
			{
				m_row = value;
			}
		}

		internal RowHandler(BinaryWriter output, int firstRow, SSTHandler sst)
		{
			m_stream = output;
			m_counter = 0;
			m_recType = -1;
			m_row = firstRow;
			m_valueCol1 = -1;
			m_valueCol2 = -1;
			m_stringHandler = sst;
		}

		internal bool Add(object value, RichTextInfo richTextInfo, TypeCode type, ExcelDataType excelType, ExcelErrorCode errorCode, short column, ushort ixfe)
		{
			m_details.Initialize(m_stream, m_row, column, ixfe);
			TransformResult transformResult;
			if (errorCode == ExcelErrorCode.None)
			{
				switch (excelType)
				{
				case ExcelDataType.Blank:
					transformResult = CreateBlankRecord(m_details);
					break;
				case ExcelDataType.Boolean:
					transformResult = CreateBoolRecord(value, m_details);
					break;
				case ExcelDataType.Number:
				{
					if (type == TypeCode.DateTime)
					{
						value = DateToDays((DateTime)value);
					}
					transformResult = CreateRKorNumberRecord((ValueType)value, m_details, out uint? rkValue);
					if (rkValue.HasValue)
					{
						transformResult = CreateRKRecord(rkValue.Value, m_details);
					}
					break;
				}
				case ExcelDataType.String:
				{
					string text = (string)value;
					transformResult = ((text.Length <= 0) ? CreateBlankRecord(m_details) : CreateStringRecord(text, m_details));
					break;
				}
				case ExcelDataType.RichString:
					transformResult = CreateRichStringRecord(richTextInfo, m_details);
					break;
				default:
					transformResult = CreateBlankRecord(m_details);
					break;
				}
			}
			else
			{
				transformResult = CreateErrorRecord(errorCode, m_details);
			}
			return TransformResult.Handled == transformResult;
		}

		private TransformResult CreateRichStringRecord(RichTextInfo richTextInfo, CellRenderingDetails details)
		{
			StringWrapperBIFF8 stringWrapperBIFF = richTextInfo.CompleteRun();
			if (stringWrapperBIFF.Cch > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(details.Row.ToString(CultureInfo.InvariantCulture), details.Column.ToString(CultureInfo.InvariantCulture)));
			}
			int isst = m_stringHandler.AddString(stringWrapperBIFF);
			OnCellBegin(253, details.Column);
			RecordFactory.LABELSST(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, (uint)isst);
			return TransformResult.Handled;
		}

		internal TransformResult CreateStringRecord(string input, CellRenderingDetails details)
		{
			if (input.Length > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(details.Row.ToString(CultureInfo.InvariantCulture), details.Column.ToString(CultureInfo.InvariantCulture)));
			}
			if (input.Length < 256)
			{
				OnCellBegin(516, details.Column);
				RecordFactory.LABEL(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, input.ToString());
			}
			else
			{
				int isst = m_stringHandler.AddString(input.ToString());
				OnCellBegin(253, details.Column);
				RecordFactory.LABELSST(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, (uint)isst);
			}
			return TransformResult.Handled;
		}

		internal TransformResult CreateBoolRecord(object val, CellRenderingDetails details)
		{
			if (val == null)
			{
				return TransformResult.Null;
			}
			bool value = Convert.ToBoolean(val, CultureInfo.InvariantCulture);
			return CreateBoolErrRecord(Convert.ToByte(value), isError: false, details);
		}

		internal TransformResult CreateErrorRecord(ExcelErrorCode errorCode, CellRenderingDetails details)
		{
			return CreateBoolErrRecord((byte)errorCode, isError: true, details);
		}

		private TransformResult CreateBoolErrRecord(byte val, bool isError, CellRenderingDetails details)
		{
			OnCellBegin(517, details.Column);
			RecordFactory.BOOLERR(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, val, isError);
			return TransformResult.Handled;
		}

		internal TransformResult CreateRKorNumberRecord(ValueType val, CellRenderingDetails details, out uint? rkValue)
		{
			if (val == null)
			{
				rkValue = null;
				return TransformResult.Null;
			}
			double num2;
			if (val is float)
			{
				float num = (float)(object)val;
				num2 = ((num == float.PositiveInfinity) ? double.PositiveInfinity : ((num == float.NegativeInfinity) ? double.NegativeInfinity : ((num != float.NaN) ? Convert.ToDouble(val, CultureInfo.InvariantCulture) : double.NaN)));
			}
			else
			{
				num2 = Convert.ToDouble(val, CultureInfo.InvariantCulture);
			}
			rkValue = RKEncoder.EncodeRK(num2);
			if (rkValue.HasValue)
			{
				OnCellBegin(638, details.Column);
			}
			else
			{
				OnCellBegin(515, details.Column);
				RecordFactory.NUMBER(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, num2);
			}
			return TransformResult.Handled;
		}

		private TransformResult CreateBlankRecord(CellRenderingDetails details)
		{
			OnCellBegin(513, details.Column);
			m_recType = 513;
			m_valueCol1 = ((m_counter == 0) ? details.Column : m_valueCol1);
			m_valueCol2 = details.Column;
			m_xfs[m_counter] = details.Ixfe;
			m_counter++;
			return TransformResult.Handled;
		}

		private TransformResult CreateRKRecord(uint value, CellRenderingDetails details)
		{
			OnCellBegin(638, details.Column);
			m_recType = 638;
			m_valueCol1 = ((m_counter == 0) ? details.Column : m_valueCol1);
			m_valueCol2 = details.Column;
			m_xfs[m_counter] = details.Ixfe;
			m_rks[m_counter] = value;
			m_counter++;
			return TransformResult.Handled;
		}

		internal void FlushRow()
		{
			FlushMultiRecord();
			m_counter = 0;
			m_recType = 0;
		}

		private void OnCellBegin(short recType, int col)
		{
			if (m_counter > 0 && (recType != m_recType || col != m_valueCol2 + 1))
			{
				FlushMultiRecord();
			}
		}

		private void FlushMultiRecord()
		{
			if (m_counter > 1)
			{
				if (m_recType == 638)
				{
					RecordFactory.MULRK(m_stream, (ushort)m_row, (ushort)m_valueCol1, (ushort)m_valueCol2, m_xfs, m_rks, m_counter);
				}
				else
				{
					RecordFactory.MULBLANK(m_stream, (ushort)m_row, (ushort)m_valueCol1, (ushort)m_valueCol2, m_xfs, m_counter);
				}
			}
			else if (m_counter == 1)
			{
				if (m_recType == 638)
				{
					RecordFactory.RK(m_stream, (ushort)m_row, (ushort)m_valueCol1, m_xfs[0], m_rks[0]);
				}
				else
				{
					RecordFactory.BLANK(m_stream, (ushort)m_row, (ushort)m_valueCol1, m_xfs[0]);
				}
			}
			m_counter = 0;
			m_valueCol1 = -1;
			m_valueCol2 = -1;
		}

		internal static double DateToDays(DateTime dateTime)
		{
			double num = dateTime.Subtract(Epoch).TotalDays + 1.0;
			if (num >= 60.0)
			{
				num += 1.0;
			}
			return num;
		}
	}
}
