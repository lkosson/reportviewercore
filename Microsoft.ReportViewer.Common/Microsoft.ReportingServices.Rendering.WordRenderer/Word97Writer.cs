using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class Word97Writer : IWordWriter, IDisposable
	{
		public enum HeaderFooterLocation
		{
			First,
			Odd,
			Even
		}

		private class ListLevelInfo
		{
			internal RPLFormat.ListStyles? Style;

			internal int ListIndex = -1;

			public void Reset()
			{
				Style = null;
				ListIndex = -1;
			}
		}

		private const int EmptyStyleIndex = 16;

		private int m_fcStart = 1024;

		private Stream m_fontTable;

		private Stream m_tableStream;

		private Stream m_mainStream;

		private Stream m_dataStream;

		private Stream m_listStream;

		private Stream m_listLevelStream;

		private Stream m_outStream;

		private Dictionary<ImageHash, int> m_images;

		private Dictionary<string, int> m_fontNameSet;

		private int m_currentFontIndex;

		private TableData m_currentRow;

		private int[] m_headerFooterOffsets;

		private int m_ccpText;

		private int m_ccpHdd;

		private int m_nestingLevel;

		private Stack<TableData> m_tapStack;

		private FieldsTable m_fldsCurrent;

		private FieldsTable m_fldsMain;

		private FieldsTable m_fldsHdr;

		private Bookmarks m_bookmarks;

		private CharacterFormat m_charFormat;

		private ParagraphFormat m_parFormat;

		private SectionFormat m_secFormat;

		private WordText m_wordText;

		private int m_imgIndex;

		private int m_listIndex;

		private ListData m_currentList;

		private AutoFit m_autoFit = AutoFit.Default;

		private ListLevelInfo[] m_levelData = new ListLevelInfo[9];

		private int m_currentMaxListLevel;

		public bool CanBand => m_nestingLevel > 1;

		public AutoFit AutoFit
		{
			get
			{
				return m_autoFit;
			}
			set
			{
				m_autoFit = value;
			}
		}

		public int SectionCount => m_secFormat.SectionCount;

		public bool HasTitlePage
		{
			set
			{
			}
		}

		public Word97Writer()
		{
			InitListLevels();
		}

		public void Init(CreateAndRegisterStream createAndRegisterStream, AutoFit autoFit, string reportName)
		{
			m_outStream = createAndRegisterStream(reportName, "doc", null, "application/msword", willSeek: false, StreamOper.CreateAndRegister);
			m_tableStream = createAndRegisterStream("TableStream", null, null, null, willSeek: true, StreamOper.CreateOnly);
			m_mainStream = createAndRegisterStream("WordDocument", null, null, null, willSeek: true, StreamOper.CreateOnly);
			m_fontTable = createAndRegisterStream("FontTable", null, null, null, willSeek: true, StreamOper.CreateOnly);
			m_dataStream = createAndRegisterStream("Data", null, null, null, willSeek: true, StreamOper.CreateOnly);
			m_listStream = createAndRegisterStream("List", null, null, null, willSeek: true, StreamOper.CreateOnly);
			m_listLevelStream = createAndRegisterStream("ListLevel", null, null, null, willSeek: true, StreamOper.CreateOnly);
			Stream textPiece = createAndRegisterStream("TextPiece", null, null, null, willSeek: true, StreamOper.CreateOnly);
			Stream chpTable = createAndRegisterStream("ChpTable", null, null, null, willSeek: true, StreamOper.CreateOnly);
			Stream papTable = createAndRegisterStream("PapTable", null, null, null, willSeek: true, StreamOper.CreateOnly);
			m_charFormat = new CharacterFormat(chpTable, m_fcStart);
			m_parFormat = new ParagraphFormat(papTable, m_fcStart);
			m_wordText = new WordText(textPiece);
			m_secFormat = new SectionFormat();
			m_currentRow = new TableData(1, layoutTable: true);
			m_tapStack = new Stack<TableData>();
			m_fontNameSet = new Dictionary<string, int>();
			WriteFont("Times New Roman");
			WriteFont("Symbol");
			WriteFont("Arial");
			m_imgIndex = 0;
			m_fldsMain = new FieldsTable();
			m_fldsHdr = new FieldsTable();
			m_fldsCurrent = m_fldsMain;
			m_bookmarks = new Bookmarks();
			m_images = new Dictionary<ImageHash, int>();
			m_autoFit = autoFit;
		}

		public void SetPageDimensions(float pageHeight, float pageWidth, float leftMargin, float rightMargin, float topMargin, float bottomMargin)
		{
			if (pageWidth > 558.8f)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The maximum page width exceeded:{0}", pageWidth);
				pageWidth = 558.8f;
			}
			if (FixMargins(pageHeight, ref topMargin, ref bottomMargin))
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The top or bottom margin is either <0 or the sum exceeds the page height.");
			}
			if (pageHeight > 558.8f)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The maximum page height exceeded:{0}", pageHeight);
				pageHeight = 558.8f;
			}
			m_secFormat.AddSprm(45088, ToTwips(pageHeight), null);
			m_secFormat.AddSprm(45087, ToTwips(pageWidth), null);
			m_secFormat.AddSprm(45089, ToTwips(leftMargin), null);
			m_secFormat.AddSprm(45090, ToTwips(rightMargin), null);
			m_secFormat.AddSprm(36899, ToTwips(topMargin), null);
			m_secFormat.AddSprm(36900, ToTwips(bottomMargin), null);
			if (pageWidth > pageHeight)
			{
				m_secFormat.AddSprm(12317, 2, null);
			}
			m_secFormat.AddSprm(45079, 0, null);
			m_secFormat.AddSprm(45080, 0, null);
		}

		public void InitHeaderFooter()
		{
			m_ccpText = m_wordText.CurrentCp;
			m_headerFooterOffsets = new int[14 + 6 * m_secFormat.SectionCount];
			m_fldsCurrent = m_fldsHdr;
		}

		public void FinishHeader(int section)
		{
			FinishHeader(section, HeaderFooterLocation.Odd);
		}

		public void FinishFooter(int section)
		{
			FinishFooter(section, HeaderFooterLocation.Odd);
		}

		public void FinishHeader(int section, HeaderFooterLocation location)
		{
			int index = 0;
			switch (location)
			{
			case HeaderFooterLocation.Even:
				index = 7;
				break;
			case HeaderFooterLocation.Odd:
				index = 8;
				break;
			case HeaderFooterLocation.First:
				index = 11;
				break;
			}
			FinishHeaderFooterRegion(section, index);
		}

		public void FinishFooter(int section, HeaderFooterLocation location)
		{
			int index = 0;
			switch (location)
			{
			case HeaderFooterLocation.Even:
				index = 9;
				break;
			case HeaderFooterLocation.Odd:
				index = 10;
				break;
			case HeaderFooterLocation.First:
				index = 12;
				break;
			}
			FinishHeaderFooterRegion(section, index);
		}

		private void FinishHeaderFooterRegion(int section, int index)
		{
			WriteParagraphEnd();
			int num = m_wordText.CurrentCp - m_ccpText;
			index += section * 6;
			for (int i = index; i < m_headerFooterOffsets.Length - 1; i++)
			{
				m_headerFooterOffsets[i] = num;
			}
			m_headerFooterOffsets[m_headerFooterOffsets.Length - 1] = num + 3;
		}

		public void FinishHeadersFooters(bool hasTitlePage)
		{
			WriteParagraphEnd();
			m_ccpHdd = m_wordText.CurrentCp - m_ccpText;
			WriteParagraphEnd();
			if (hasTitlePage)
			{
				m_secFormat.UseTitlePage = true;
			}
		}

		public void AddImage(byte[] imgBuf, float height, float width, RPLFormat.Sizings sizing)
		{
			if (imgBuf == null || imgBuf.Length == 0)
			{
				sizing = RPLFormat.Sizings.Clip;
				imgBuf = PictureDescriptor.INVALIDIMAGEDATA;
			}
			byte[] hash = new OfficeImageHasher(imgBuf).Hash;
			int num = ToTwips(height);
			int num2 = ToTwips(width);
			int num3 = (int)m_dataStream.Position;
			ImageHash key = new ImageHash(hash, sizing, num2, num);
			if (m_images.ContainsKey(key))
			{
				num3 = m_images[key];
			}
			else
			{
				new PictureDescriptor(imgBuf, hash, num2, num, sizing, m_imgIndex).Serialize(m_dataStream);
				m_imgIndex++;
				m_images.Add(key, num3);
			}
			m_charFormat.SetIsInlineImage(num3);
			WriteSpecialText("\u0001");
		}

		private void WriteSpecialText(string text)
		{
			int currentCp = m_wordText.CurrentCp;
			m_wordText.WriteSpecialText(text);
			m_charFormat.CommitLastCharacterRun(currentCp, m_wordText.CurrentCp);
		}

		public void WriteText(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				int currentCp = m_wordText.CurrentCp;
				m_wordText.WriteText(text);
				m_charFormat.CommitLastCharacterRun(currentCp, m_wordText.CurrentCp);
			}
		}

		public void WriteHyperlinkBegin(string target, bool bookmarkLink)
		{
			HyperlinkWriter.LinkType type = HyperlinkWriter.LinkType.Bookmark;
			if (!bookmarkLink)
			{
				Uri uri = new Uri(target);
				if (uri.IsFile)
				{
					type = HyperlinkWriter.LinkType.File;
					target = uri.LocalPath;
				}
				else
				{
					type = HyperlinkWriter.LinkType.Hyperlink;
				}
			}
			m_charFormat.Push(15);
			m_fldsCurrent.Add(new HyperlinkFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.Start));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText('\u0013'.ToString());
			string text = " HYPERLINK " + (bookmarkLink ? "\\l" : "") + "\"" + target + "\" ";
			WriteSpecialText(text);
			string text2 = "\u0001";
			m_charFormat.AddSprm(2133, 1, null);
			m_charFormat.AddSprm(2050, 1, null);
			m_charFormat.AddSprm(2054, 1, null);
			m_charFormat.AddSprm(27139, (int)m_dataStream.Length, null);
			WriteSpecialText(text2);
			HyperlinkWriter.WriteHyperlink(m_dataStream, target, type);
			m_fldsCurrent.Add(new HyperlinkFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.Middle));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText('\u0014'.ToString());
			m_charFormat.Pop();
		}

		public void WriteHyperlinkEnd()
		{
			m_fldsCurrent.Add(new HyperlinkFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.End));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText('\u0015'.ToString());
		}

		public void AddTableStyleProp(byte code, object value)
		{
			if (value != null)
			{
				m_currentRow.AddTableStyleProp(code, value);
			}
		}

		public void SetTableContext(BorderContext borderContext)
		{
			m_currentRow.SetTableContext(borderContext);
		}

		public void AddBodyStyleProp(byte code, object value)
		{
			if ((uint)code > 18u)
			{
				_ = code - 33;
				_ = 4;
			}
		}

		public void AddCellStyleProp(int cellIndex, byte code, object value)
		{
			if (value != null)
			{
				m_currentRow.AddCellStyleProp(cellIndex, code, value);
			}
		}

		public void AddPadding(int cellIndex, byte code, object value, int defaultValue)
		{
			m_currentRow.AddPadding(cellIndex, code, value, defaultValue);
		}

		public void ApplyCellBorderContext(BorderContext borderContext)
		{
		}

		public void AddTextStyleProp(byte code, object value)
		{
			if (value == null)
			{
				return;
			}
			switch (code)
			{
			case 23:
			case 25:
			case 26:
			case 30:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
				break;
			case 24:
				RenderTextDecoration((RPLFormat.TextDecorations)value);
				break;
			case 27:
				if (value is string)
				{
					RenderTextColor((string)value);
				}
				else if (value is Color)
				{
					RenderTextColor((Color)value);
				}
				break;
			case 28:
				RenderLineHeight(value as string);
				break;
			case 29:
				RenderDirection((RPLFormat.Directions)value);
				break;
			case 31:
				RenderUnicodeBiDi((RPLFormat.UnicodeBiDiTypes)value);
				break;
			case 32:
				RenderLanguage(value as string);
				break;
			}
		}

		public void AddFirstLineIndent(float indent)
		{
			int param = PointsToTwips(indent);
			m_parFormat.AddSprm(ParagraphSprms.SPRM_DXALEFT12k3, param, null);
		}

		public void AddLeftIndent(float margin)
		{
			int param = PointsToTwips(margin);
			m_parFormat.AddSprm(ParagraphSprms.SPRM_DXALEFT2k3, param, null);
		}

		public void AddRightIndent(float margin)
		{
			int param = PointsToTwips(margin);
			m_parFormat.AddSprm(ParagraphSprms.SPRM_DXARIGHT2k3, param, null);
		}

		public void AddSpaceBefore(float space)
		{
			int param = PointsToTwips(space);
			m_parFormat.AddSprm(ParagraphSprms.SPRM_DYABEFORE, param, null);
		}

		public void AddSpaceAfter(float space)
		{
			int param = PointsToTwips(space);
			m_parFormat.AddSprm(ParagraphSprms.SPRM_DYAAFTER, param, null);
		}

		public static bool FixMargins(float totalSize, ref float left, ref float right)
		{
			if (left < 0f)
			{
				left = 0f;
			}
			if (right < 0f)
			{
				right = 0f;
			}
			if (left + right >= totalSize)
			{
				left = 0f;
				right = 0f;
				return true;
			}
			return false;
		}

		private int PointsToTwips(float indent)
		{
			return (int)(indent * 20f);
		}

		private void RenderTextDecoration(RPLFormat.TextDecorations textDecorations)
		{
			switch (textDecorations)
			{
			case RPLFormat.TextDecorations.Overline:
				break;
			case RPLFormat.TextDecorations.LineThrough:
				m_charFormat.AddSprm(2103, 1, null);
				break;
			case RPLFormat.TextDecorations.Underline:
				m_charFormat.AddSprm(10814, 1, null);
				break;
			}
		}

		private void RenderLanguage(string p)
		{
		}

		private void RenderUnicodeBiDi(RPLFormat.UnicodeBiDiTypes unicodeBiDiTypes)
		{
		}

		private void RenderDirection(RPLFormat.Directions directions)
		{
			int param = (directions == RPLFormat.Directions.RTL) ? 1 : 0;
			m_parFormat.AddSprm(9281, param, null);
		}

		public void RenderTextRunDirection(RPLFormat.Directions direction)
		{
			int param = (direction == RPLFormat.Directions.RTL) ? 1 : 0;
			m_charFormat.AddSprm(2138, param, null);
		}

		private void RenderLineHeight(string size)
		{
			int param = ToTwips((float)new RPLReportSize(size).ToMillimeters());
			m_parFormat.AddSprm(25618, param, null);
		}

		private void RenderTextColor(string strColor)
		{
			Color color = new RPLReportColor(strColor).ToColor();
			RenderTextColor(color);
		}

		private void RenderTextColor(Color color)
		{
			int param = (color.B << 16) | (color.G << 8) | color.R;
			m_charFormat.AddSprm(26736, param, null);
		}

		private bool GetTextAlignForType(TypeCode typeCode)
		{
			bool flag = false;
			if ((uint)(typeCode - 4) <= 12u)
			{
				return true;
			}
			return false;
		}

		public void RenderTextAlign(TypeCode type, RPLFormat.TextAlignments textAlignments, RPLFormat.Directions direction)
		{
			int param = 0;
			if (textAlignments == RPLFormat.TextAlignments.General)
			{
				textAlignments = ((!GetTextAlignForType(type)) ? RPLFormat.TextAlignments.Left : RPLFormat.TextAlignments.Right);
			}
			else if (direction == RPLFormat.Directions.RTL)
			{
				switch (textAlignments)
				{
				case RPLFormat.TextAlignments.Left:
					textAlignments = RPLFormat.TextAlignments.Right;
					break;
				case RPLFormat.TextAlignments.Right:
					textAlignments = RPLFormat.TextAlignments.Left;
					break;
				}
			}
			switch (textAlignments)
			{
			case RPLFormat.TextAlignments.Left:
				param = 0;
				break;
			case RPLFormat.TextAlignments.Center:
				param = 1;
				break;
			case RPLFormat.TextAlignments.Right:
				param = 2;
				break;
			}
			m_parFormat.AddSprm(9313, param, null);
		}

		public void RenderFontWeight(RPLFormat.FontWeights fontWeights, RPLFormat.Directions dir)
		{
			if ((int)fontWeights >= 5)
			{
				m_charFormat.AddSprm(2101, 1, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					m_charFormat.AddSprm(2140, 1, null);
				}
			}
			else
			{
				m_charFormat.AddSprm(2101, 0, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					m_charFormat.AddSprm(2140, 0, null);
				}
			}
		}

		public void RenderFontWeight(RPLFormat.FontWeights? fontWeights, RPLFormat.Directions dir)
		{
			if (fontWeights.HasValue)
			{
				RenderFontWeight(fontWeights.Value, dir);
			}
		}

		public void RenderFontSize(string size, RPLFormat.Directions dir)
		{
			int param = (int)Math.Round(new RPLReportSize(size).ToPoints() * 2.0);
			m_charFormat.AddSprm(19011, param, null);
			if (dir == RPLFormat.Directions.RTL)
			{
				m_charFormat.AddSprm(19041, param, null);
			}
		}

		public void RenderFontFamily(string font, RPLFormat.Directions dir)
		{
			int param = WriteFont(font);
			m_charFormat.AddSprm(19023, param, null);
			m_charFormat.AddSprm(19024, param, null);
			m_charFormat.AddSprm(19025, param, null);
			if (dir == RPLFormat.Directions.RTL)
			{
				m_charFormat.AddSprm(19038, param, null);
			}
		}

		public void RenderFontStyle(RPLFormat.FontStyles value, RPLFormat.Directions dir)
		{
			if (value == RPLFormat.FontStyles.Italic)
			{
				m_charFormat.AddSprm(2102, 1, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					m_charFormat.AddSprm(2141, 1, null);
				}
			}
			else
			{
				m_charFormat.AddSprm(2102, 0, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					m_charFormat.AddSprm(2141, 0, null);
				}
			}
		}

		public void RenderFontStyle(RPLFormat.FontStyles? value, RPLFormat.Directions dir)
		{
			if (value.HasValue)
			{
				RenderFontStyle(value.Value, dir);
			}
		}

		public void WriteParagraphEnd()
		{
			WriteCurrentListData();
			WriteParagraphEnd("\r", useTapx: false);
		}

		public void WriteListEnd(int level, RPLFormat.ListStyles listStyle, bool endParagraph)
		{
			int listIndex = m_listIndex;
			for (int i = level; i < m_currentMaxListLevel; i++)
			{
				m_levelData[i].Reset();
			}
			if (m_levelData[level - 1].Style == listStyle)
			{
				listIndex = m_levelData[level - 1].ListIndex;
			}
			else
			{
				if (m_currentList != null)
				{
					WriteCurrentListData();
				}
				m_currentList = new ListData(++m_listIndex);
				listIndex = m_listIndex;
				m_currentList.SetLevel(level - 1, new ListLevelOnFile(level - 1, listStyle, this));
				m_levelData[level - 1].Style = listStyle;
				m_levelData[level - 1].ListIndex = listIndex;
				m_currentMaxListLevel = level;
			}
			m_parFormat.AddSprm(17931, listIndex, null);
			m_parFormat.AddSprm(9738, level - 1, null);
			if (endParagraph)
			{
				WriteParagraphEnd("\r", useTapx: false);
			}
		}

		public void InitListLevels()
		{
			int num = m_levelData.Length;
			for (int i = 0; i < num; i++)
			{
				m_levelData[i] = new ListLevelInfo();
			}
		}

		public void ResetListlevels()
		{
			for (int i = 0; i < m_currentMaxListLevel; i++)
			{
				m_levelData[i].Reset();
			}
			m_currentMaxListLevel = 0;
		}

		public void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell)
		{
			m_currentRow.WriteTableCellEnd(cellIndex, borderContext);
			string parEnd = "\a";
			if (m_nestingLevel > 1)
			{
				m_parFormat.AddSprm(9291, 1, null);
				parEnd = "\r";
			}
			if (emptyLayoutCell)
			{
				m_parFormat.StyleIndex = 16;
			}
			WriteCurrentListData();
			WriteParagraphEnd(parEnd, useTapx: false);
		}

		public void WriteEmptyStyle()
		{
			m_parFormat.StyleIndex = 16;
		}

		public void WriteTableBegin(float left, bool layoutTable)
		{
			if (m_nestingLevel > 0)
			{
				m_tapStack.Push(m_currentRow);
			}
			m_currentRow = new TableData(m_nestingLevel + 1, layoutTable);
			m_nestingLevel++;
		}

		public void WriteTableRowBegin(float left, float height, float[] columnWidths)
		{
			m_currentRow.InitTableRow(left, height, columnWidths, m_autoFit);
		}

		public void IgnoreRowHeight(bool canGrow)
		{
			m_currentRow.WriteRowHeight = !canGrow;
		}

		public void SetWriteExactRowHeight(bool writeExactRowHeight)
		{
			m_currentRow.WriteExactRowHeight = writeExactRowHeight;
		}

		public void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			m_currentRow.WriteTableCellBegin(cellIndex, numColumns, firstVertMerge, firstHorzMerge, vertMerge, horzMerge);
		}

		public void WriteTableRowEnd()
		{
			if (m_nestingLevel > 1)
			{
				WriteParagraphEnd("\r", useTapx: true);
			}
			else
			{
				WriteParagraphEnd("\a", useTapx: true);
			}
		}

		public void WriteTableEnd()
		{
			if (m_nestingLevel > 1)
			{
				m_currentRow = m_tapStack.Pop();
			}
			m_nestingLevel--;
		}

		public void Finish(string title, string author, string comments)
		{
			m_charFormat.Finish(m_wordText.CurrentCp);
			m_parFormat.Finish(m_wordText.CurrentCp);
			byte[] array = new byte[FileInformationBlock.FieldsSize];
			BinaryWriter binaryWriter = new BinaryWriter(m_tableStream);
			BinaryWriter binaryWriter2 = new BinaryWriter(m_mainStream);
			binaryWriter2.Write(FileInformationBlock.StartBuffer, 0, FileInformationBlock.StartBuffer.Length);
			binaryWriter2.Write(new byte[2], 0, 2);
			binaryWriter2.Write(array);
			binaryWriter2.Write(FileInformationBlock.EndBuffer, 0, FileInformationBlock.EndBuffer.Length);
			binaryWriter.Flush();
			int num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(33), num);
			m_wordText.WriteClxTo(binaryWriter, m_fcStart);
			LittleEndian.PutInt(array, GetLcbField(33), (int)binaryWriter.BaseStream.Position - num);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(1), num);
			binaryWriter.Write(StyleSheet.Buffer, 0, StyleSheet.Buffer.Length);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, GetLcbField(1), (int)binaryWriter.BaseStream.Position - num);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(31), num);
			binaryWriter.Write(DocumentProperties.Buffer, 0, DocumentProperties.Buffer.Length);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, GetLcbField(31), (int)binaryWriter.BaseStream.Position - num);
			binaryWriter2.Flush();
			TransferData(m_wordText.Stream, binaryWriter2.BaseStream, 4096);
			int num2 = (int)binaryWriter2.BaseStream.Position;
			int num3 = 512 - num2 % 512;
			binaryWriter2.Write(new byte[num3], 0, num3);
			binaryWriter2.Flush();
			int pageStart = (int)binaryWriter2.BaseStream.Position / 512;
			TransferData(m_charFormat.Stream, binaryWriter2.BaseStream, 4096);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(12), num);
			m_charFormat.WriteBinTableTo(binaryWriter, ref pageStart);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, GetLcbField(12), (int)binaryWriter.BaseStream.Position - num);
			TransferData(m_parFormat.Stream, binaryWriter2.BaseStream, 4096);
			binaryWriter.Flush();
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(13), num);
			m_parFormat.WriteBinTableTo(binaryWriter, ref pageStart);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, GetLcbField(13), (int)binaryWriter.BaseStream.Position - num);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(6), num);
			m_secFormat.WriteTo(binaryWriter, binaryWriter2, m_wordText.CurrentCp);
			LittleEndian.PutInt(array, GetLcbField(6), (int)binaryWriter.BaseStream.Position - num);
			if (m_fldsMain.Size > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(16), num);
				LittleEndian.PutInt(array, GetLcbField(16), m_fldsMain.Size);
				m_fldsMain.WriteTo(binaryWriter, 0, m_wordText.CurrentCp);
				binaryWriter.Flush();
			}
			if (m_fldsHdr.Size > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(17), num);
				LittleEndian.PutInt(array, GetLcbField(17), m_fldsHdr.Size);
				m_fldsHdr.WriteTo(binaryWriter, m_ccpText, m_wordText.CurrentCp);
				binaryWriter.Flush();
			}
			if (m_headerFooterOffsets != null)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(11), num);
				LittleEndian.PutInt(array, GetLcbField(11), m_headerFooterOffsets.Length * 4);
				for (int i = 0; i < m_headerFooterOffsets.Length; i++)
				{
					binaryWriter.Write(m_headerFooterOffsets[i]);
				}
				binaryWriter.Flush();
			}
			if (m_bookmarks.Count > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(22), num);
				m_bookmarks.SerializeStarts(binaryWriter, m_wordText.CurrentCp);
				LittleEndian.PutInt(array, GetLcbField(22), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(23), num);
				m_bookmarks.SerializeEnds(binaryWriter, m_wordText.CurrentCp);
				LittleEndian.PutInt(array, GetLcbField(23), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(21), num);
				m_bookmarks.SerializeNames(binaryWriter);
				LittleEndian.PutInt(array, GetLcbField(21), (int)binaryWriter.BaseStream.Position - num);
			}
			if (m_listIndex > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(73), num);
				binaryWriter.Write((short)m_listIndex);
				binaryWriter.Flush();
				TransferData(m_listStream, binaryWriter.BaseStream, (int)m_listStream.Length);
				TransferData(m_listLevelStream, binaryWriter.BaseStream, (int)m_listLevelStream.Length);
				LittleEndian.PutInt(array, GetLcbField(73), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(74), num);
				binaryWriter.Write(m_listIndex);
				binaryWriter.Flush();
				WriteListFormatOverrides(binaryWriter);
				LittleEndian.PutInt(array, GetLcbField(74), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, GetFcField(91), num);
				WriteListNameTable(binaryWriter);
				LittleEndian.PutInt(array, GetLcbField(91), (int)binaryWriter.BaseStream.Position - num);
			}
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, GetFcField(15), num);
			binaryWriter.Write((short)m_currentFontIndex);
			binaryWriter.Write((short)0);
			binaryWriter.Flush();
			TransferData(m_fontTable, binaryWriter.BaseStream, (int)m_fontTable.Length);
			LittleEndian.PutInt(array, GetLcbField(15), (int)binaryWriter.BaseStream.Position - num);
			binaryWriter2.Flush();
			binaryWriter2.BaseStream.Seek(24L, SeekOrigin.Begin);
			binaryWriter2.Write(m_fcStart);
			binaryWriter2.Write(num2);
			binaryWriter2.Flush();
			binaryWriter2.BaseStream.Seek(64L, SeekOrigin.Begin);
			binaryWriter2.Write((int)binaryWriter2.BaseStream.Length);
			binaryWriter2.Flush();
			binaryWriter2.BaseStream.Seek(76L, SeekOrigin.Begin);
			binaryWriter2.Write((m_ccpText > 0) ? m_ccpText : m_wordText.CurrentCp);
			binaryWriter2.Flush();
			if (m_ccpText > 0)
			{
				binaryWriter2.BaseStream.Seek(84L, SeekOrigin.Begin);
				binaryWriter2.Write(m_ccpHdd);
				binaryWriter2.Flush();
			}
			int val = 0;
			for (int j = 0; j < array.Length; j += 8)
			{
				int @int = LittleEndian.getInt(array, j);
				int int2 = LittleEndian.getInt(array, j + 4);
				if (int2 == 0)
				{
					LittleEndian.PutInt(array, j, val);
				}
				else
				{
					val = @int + int2;
				}
			}
			binaryWriter2.BaseStream.Seek(FileInformationBlock.StartBuffer.Length, SeekOrigin.Begin);
			binaryWriter2.Write((short)(array.Length / 8));
			binaryWriter2.Write(array, 0, array.Length);
			binaryWriter2.Flush();
			StructuredStorage.CreateMultiStreamFile(new Stream[3]
			{
				m_mainStream,
				m_tableStream,
				m_dataStream
			}, new string[3]
			{
				"WordDocument",
				"1Table",
				"Data"
			}, "00020906-0000-0000-c000-000000000046", author, title, comments, m_outStream, forceInMemory: false);
			m_listStream.Close();
			m_mainStream.Close();
			m_tableStream.Close();
			m_dataStream.Close();
			m_fontTable.Close();
			m_listLevelStream.Close();
			m_charFormat.Stream.Close();
			m_parFormat.Stream.Close();
			m_wordText.Stream.Close();
		}

		public int WriteFont(string fontName)
		{
			if (string.IsNullOrEmpty(fontName))
			{
				fontName = "Times New Roman";
			}
			if (m_fontNameSet.ContainsKey(fontName))
			{
				return m_fontNameSet[fontName];
			}
			byte[] array = BuiltInFonts.GetFont(fontName).toByteArray();
			m_fontTable.Write(array, 0, array.Length);
			m_fontNameSet.Add(fontName, m_currentFontIndex);
			return m_currentFontIndex++;
		}

		private void WriteParagraphEnd(string parEnd, bool useTapx)
		{
			WriteSpecialText(parEnd);
			if (!useTapx && m_nestingLevel > 0)
			{
				m_parFormat.SetIsInTable(m_nestingLevel);
			}
			m_parFormat.CommitParagraph(m_wordText.CurrentCp, useTapx ? m_currentRow : null, m_dataStream);
		}

		public static int AddSprm(byte[] grpprl, int offset, ushort instruction, int param, byte[] varParam)
		{
			int num = (instruction & 0xE000) >> 13;
			byte[] array = null;
			switch (num)
			{
			case 0:
			case 1:
				array = new byte[3]
				{
					0,
					0,
					(byte)param
				};
				break;
			case 2:
				array = new byte[4];
				LittleEndian.PutUShort(array, 2, (ushort)param);
				break;
			case 3:
				array = new byte[6];
				LittleEndian.PutInt(array, 2, param);
				break;
			case 4:
			case 5:
				array = new byte[4];
				LittleEndian.PutUShort(array, 2, (ushort)param);
				break;
			case 6:
				array = new byte[3 + varParam.Length];
				array[2] = (byte)varParam.Length;
				Array.Copy(varParam, 0, array, 3, varParam.Length);
				break;
			case 7:
			{
				array = new byte[5];
				byte[] array2 = new byte[4];
				LittleEndian.PutInt(array2, 0, param);
				Array.Copy(array2, 0, array, 2, 3);
				break;
			}
			}
			LittleEndian.PutUShort(array, 0, instruction);
			Array.Copy(array, 0, grpprl, offset, array.Length);
			return array.Length;
		}

		public static double ToPoints(string size)
		{
			return new RPLReportSize(size).ToPoints();
		}

		public static int ToIco24(string color)
		{
			return WordColor.GetIco24(new RPLReportColor(color).ToColor());
		}

		public static ushort ToTwips(string size)
		{
			return (ushort)(new RPLReportSize(size).ToPoints() * 20.0);
		}

		public static ushort ToTwips(object size)
		{
			if (size == null)
			{
				return 0;
			}
			return ToTwips(size as string);
		}

		public static ushort ToTwips(float mm)
		{
			return (ushort)(mm / 25.4f * 1440f);
		}

		public static float TwipsToMM(int twips)
		{
			return (float)((double)twips / 1440.0 * 25.399999618530273);
		}

		private int GetFcField(int fieldNum)
		{
			return 4 * (fieldNum * 2);
		}

		private int GetLcbField(int fieldNum)
		{
			return 4 * (fieldNum * 2 + 1);
		}

		private void TransferData(Stream inStream, Stream outStream, int bufSize)
		{
			inStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[bufSize];
			for (int num = inStream.Read(array, 0, array.Length); num != 0; num = inStream.Read(array, 0, array.Length))
			{
				outStream.Write(array, 0, num);
			}
			inStream.Close();
			inStream.Dispose();
		}

		public void RenderBookmark(string name)
		{
			m_bookmarks.AddBookmark(name, m_wordText.CurrentCp);
		}

		public void RenderLabel(string label, int level)
		{
			m_charFormat.AddSprm(2133, 1, null);
			m_charFormat.AddSprm(2050, 1, null);
			WriteSpecialText("\u0013");
			m_charFormat.AddSprm(2050, 1, null);
			WriteSpecialText(" TC \"" + label + "\" \\f C \\l \"" + level + "\" ");
			m_charFormat.AddSprm(2133, 1, null);
			m_charFormat.AddSprm(2050, 1, null);
			WriteSpecialText("\u0015");
		}

		public void WritePageNumberField()
		{
			m_charFormat.CopyAndPush();
			m_fldsCurrent.Add(new PageNumberFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.Start));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText("\u0013");
			m_charFormat.Pop();
			m_charFormat.CopyAndPush();
			WriteSpecialText(" PAGE ");
			m_charFormat.Pop();
			m_charFormat.CopyAndPush();
			m_fldsCurrent.Add(new PageNumberFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.Middle));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText("\u0014");
			m_charFormat.Pop();
			m_charFormat.CopyAndPush();
			WriteSpecialText("1");
			m_charFormat.Pop();
			m_fldsCurrent.Add(new PageNumberFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.End));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText("\u0015");
		}

		public void WriteTotalPagesField()
		{
			m_charFormat.CopyAndPush();
			m_fldsCurrent.Add(new TotalPagesFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.Start));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText("\u0013");
			m_charFormat.Pop();
			m_charFormat.CopyAndPush();
			WriteSpecialText(" NUMPAGES ");
			m_charFormat.Pop();
			m_charFormat.CopyAndPush();
			m_fldsCurrent.Add(new TotalPagesFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.Middle));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText("\u0014");
			m_charFormat.Pop();
			m_charFormat.CopyAndPush();
			WriteSpecialText("1");
			m_charFormat.Pop();
			m_fldsCurrent.Add(new TotalPagesFieldInfo(m_wordText.CurrentCp, FieldInfo.Location.End));
			m_charFormat.AddSprm(2133, 1, null);
			WriteSpecialText("\u0015");
		}

		public void AddListStyle(int level, bool bulleted)
		{
		}

		private void WriteCurrentListData()
		{
			if (m_currentList != null)
			{
				BinaryWriter binaryWriter = new BinaryWriter(m_listStream);
				BinaryWriter binaryWriter2 = new BinaryWriter(m_listLevelStream);
				m_currentList.Write(binaryWriter, binaryWriter2, this);
				binaryWriter.Flush();
				binaryWriter2.Flush();
				m_currentList = null;
			}
		}

		private void WriteListFormatOverrides(BinaryWriter writer)
		{
			for (int i = 0; i < m_listIndex; i++)
			{
				int value = i + 1;
				writer.Write(value);
				writer.Write(0);
				writer.Write(0);
				writer.Write(0);
			}
			for (int j = 0; j < m_listIndex; j++)
			{
				writer.Write(-1);
			}
			writer.Flush();
		}

		private void WriteListNameTable(BinaryWriter writer)
		{
			writer.Write(ushort.MaxValue);
			writer.Write((short)m_listIndex);
			writer.Write((short)0);
			for (int i = 0; i < m_listIndex; i++)
			{
				writer.Write((short)0);
			}
			writer.Flush();
		}

		public void WriteCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp)
		{
			m_currentRow.AddCellDiagonal(cellIndex, style, width, color, slantUp);
		}

		public void WritePageBreak()
		{
			WriteParagraphEnd("\f", useTapx: false);
			m_parFormat.StyleIndex = 16;
			WriteParagraphEnd();
		}

		public void WriteEndSection()
		{
			m_parFormat.StyleIndex = 16;
			WriteParagraphEnd("\f", useTapx: false);
			m_secFormat.EndSection(m_wordText.CurrentCp);
		}

		public void ClearCellBorder(TableData.Positions position)
		{
			m_currentRow.ClearCellBorder(position);
		}

		public void Dispose()
		{
		}

		void IWordWriter.FinishHeaderFooterRegion(int section, int index)
		{
		}

		public void StartHeader()
		{
		}

		public void StartHeader(bool firstPage)
		{
		}

		public void FinishHeader()
		{
		}

		public void StartFooter()
		{
		}

		public void StartFooter(bool firstPage)
		{
		}

		public void FinishFooter()
		{
		}
	}
}
