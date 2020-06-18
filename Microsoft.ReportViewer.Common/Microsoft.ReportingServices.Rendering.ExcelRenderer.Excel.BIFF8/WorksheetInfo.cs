using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class WorksheetInfo
	{
		private sealed class ImageCache
		{
			private uint m_shapeID;

			private string m_name;

			private Escher.ClientAnchor.SPRC m_clientAnchor;

			private uint m_refIndex;

			private string m_hyperlinkURL;

			private bool m_isBookmark;

			internal uint ShapeID
			{
				get
				{
					return m_shapeID;
				}
				set
				{
					m_shapeID = value;
				}
			}

			internal string Name
			{
				get
				{
					return m_name;
				}
				set
				{
					m_name = value;
				}
			}

			internal Escher.ClientAnchor.SPRC ClientAnchor
			{
				get
				{
					return m_clientAnchor;
				}
				set
				{
					m_clientAnchor = value;
				}
			}

			internal uint RefIndex
			{
				get
				{
					return m_refIndex;
				}
				set
				{
					m_refIndex = value;
				}
			}

			internal string HyperlinkURL
			{
				get
				{
					return m_hyperlinkURL;
				}
				set
				{
					m_hyperlinkURL = value;
				}
			}

			internal bool IsBookmark
			{
				get
				{
					return m_isBookmark;
				}
				set
				{
					m_isBookmark = value;
				}
			}

			internal ImageCache(uint shapeID, string name, Escher.ClientAnchor.SPRC clientAnchor, uint refIndex)
			{
				ShapeID = shapeID;
				Name = name;
				ClientAnchor = clientAnchor;
				RefIndex = refIndex;
				HyperlinkURL = null;
				IsBookmark = false;
			}

			internal ImageCache(uint shapeID, string name, Escher.ClientAnchor.SPRC clientAnchor, uint refIndex, string linkURL, bool isBookmark)
				: this(shapeID, name, clientAnchor, refIndex)
			{
				HyperlinkURL = linkURL;
				IsBookmark = isBookmark;
			}
		}

		internal sealed class ColumnInfo
		{
			private double m_width;

			private byte m_outline;

			private bool m_collapsed;

			public double Width
			{
				get
				{
					return m_width;
				}
				set
				{
					m_width = value;
				}
			}

			public bool Collapsed
			{
				get
				{
					return m_collapsed;
				}
				set
				{
					m_collapsed = value;
				}
			}

			public byte OutlineLevel
			{
				get
				{
					return m_outline;
				}
				set
				{
					m_outline = value;
				}
			}

			public ColumnInfo(double width)
			{
				m_width = width;
			}
		}

		private ushort m_rowFirst = ushort.MaxValue;

		private ushort m_rowLast;

		private ushort m_colFirst = ushort.MaxValue;

		private ushort m_colLast;

		private ColumnInfo[] m_columns = new ColumnInfo[256];

		private Stream m_cellData;

		private string m_sheetName;

		private long m_BOFStartOffset;

		private List<uint> m_DBCellOffsets;

		private List<ushort> m_sizeOfCellData;

		private List<AreaInfo> m_mergeCellAreas;

		private List<HyperlinkInfo> m_hyperlinks;

		private Escher.DrawingContainer m_drawingContainer;

		private List<ImageCache> m_images;

		private uint m_currentShapeID;

		private string m_headerString;

		private string m_footerString;

		private ushort m_rowSplit;

		private ushort m_columnSplit;

		private int m_paperSize;

		private bool m_isPortrait = true;

		private double m_headerMargin;

		private double m_footerMargin;

		private double m_topMargin;

		private double m_bottomMargin;

		private double m_leftMargin;

		private double m_rightMargin;

		private bool m_summaryRowBelow = true;

		private bool m_summaryColumnToRight = true;

		private byte m_maxRowOutline;

		private byte m_maxColOutline;

		private PrintTitleInfo m_printTitle;

		private int m_sheetIndex = -1;

		internal ushort RowFirst
		{
			get
			{
				return m_rowFirst;
			}
			set
			{
				m_rowFirst = value;
			}
		}

		internal ushort RowLast
		{
			get
			{
				return m_rowLast;
			}
			set
			{
				m_rowLast = value;
			}
		}

		internal ushort ColFirst
		{
			get
			{
				return m_colFirst;
			}
			set
			{
				m_colFirst = value;
			}
		}

		internal ushort ColLast
		{
			get
			{
				return m_colLast;
			}
			set
			{
				m_colLast = value;
			}
		}

		internal Stream CellData
		{
			get
			{
				return m_cellData;
			}
			set
			{
				m_cellData = value;
			}
		}

		internal string SheetName
		{
			get
			{
				return m_sheetName;
			}
			set
			{
				m_sheetName = value;
			}
		}

		internal long BOFStartOffset
		{
			get
			{
				return m_BOFStartOffset;
			}
			set
			{
				m_BOFStartOffset = value;
			}
		}

		internal List<uint> DBCellOffsets => m_DBCellOffsets;

		internal List<ushort> SizeOfCellData => m_sizeOfCellData;

		internal ColumnInfo[] Columns => m_columns;

		internal List<AreaInfo> MergeCellAreas => m_mergeCellAreas;

		internal string HeaderString
		{
			get
			{
				return m_headerString;
			}
			set
			{
				m_headerString = value;
			}
		}

		internal string FooterString
		{
			get
			{
				return m_footerString;
			}
			set
			{
				m_footerString = value;
			}
		}

		internal bool SummaryRowAfter
		{
			get
			{
				return m_summaryRowBelow;
			}
			set
			{
				m_summaryRowBelow = value;
			}
		}

		internal bool SummaryColumnToRight
		{
			get
			{
				return m_summaryColumnToRight;
			}
			set
			{
				m_summaryColumnToRight = value;
			}
		}

		internal byte MaxRowOutline
		{
			get
			{
				return m_maxRowOutline;
			}
			set
			{
				m_maxRowOutline = Math.Min((byte)7, value);
			}
		}

		internal byte MaxColumnOutline
		{
			get
			{
				return m_maxColOutline;
			}
			set
			{
				m_maxColOutline = Math.Min((byte)7, value);
			}
		}

		internal PrintTitleInfo PrintTitle
		{
			get
			{
				return m_printTitle;
			}
			set
			{
				m_printTitle = value;
			}
		}

		internal int SheetIndex
		{
			get
			{
				return m_sheetIndex;
			}
			set
			{
				m_sheetIndex = value;
			}
		}

		internal WorksheetInfo(Stream cellDataStream, string name)
		{
			m_cellData = cellDataStream;
			m_sheetName = name;
			m_mergeCellAreas = new List<AreaInfo>();
			m_sizeOfCellData = new List<ushort>();
			m_DBCellOffsets = new List<uint>();
			m_currentShapeID = 0u;
			m_hyperlinks = new List<HyperlinkInfo>();
			m_images = new List<ImageCache>();
		}

		internal void ResolveCellReferences(Dictionary<string, string> lookup)
		{
			foreach (HyperlinkInfo hyperlink in m_hyperlinks)
			{
				if (hyperlink.IsBookmark && lookup.ContainsKey(hyperlink.URL))
				{
					hyperlink.URL = lookup[hyperlink.URL];
				}
			}
			foreach (ImageCache image in m_images)
			{
				if (image.HyperlinkURL != null && image.IsBookmark && lookup.ContainsKey(image.HyperlinkURL))
				{
					image.HyperlinkURL = lookup[image.HyperlinkURL];
				}
			}
		}

		internal void Write(BinaryWriter writer, bool isFirstPage, ExcelGeneratorConstants.CreateTempStream createTempStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight)
		{
			BOFStartOffset = writer.BaseStream.Position;
			RecordFactory.BOF(writer, RecordFactory.BOFSubstreamType.Worksheet);
			RecordFactory.INDEX(writer, RowFirst, RowLast, DBCellOffsets);
			writer.BaseStream.Write(Constants.WORKSHEET1, 0, Constants.WORKSHEET1.Length);
			RecordFactory.GUTS(writer, m_maxRowOutline, m_maxColOutline);
			RecordFactory.WSBOOL(writer, m_summaryRowBelow, m_summaryColumnToRight);
			writer.BaseStream.Write(Constants.WORKSHEET2, 0, Constants.WORKSHEET2.Length);
			RecordFactory.SETUP(writer, (ushort)m_paperSize, m_isPortrait, m_headerMargin, m_footerMargin);
			RecordFactory.MARGINS(writer, m_topMargin, m_bottomMargin, m_leftMargin, m_rightMargin);
			if (backgroundImage != null)
			{
				RecordFactory.BACKGROUNDIMAGE(writer, backgroundImage, backgroundImageWidth, backgroundImageHeight);
			}
			if (HeaderString != null && HeaderString.Length > 0)
			{
				RecordFactory.HEADER(writer, HeaderString);
			}
			if (FooterString != null && FooterString.Length > 0)
			{
				RecordFactory.FOOTER(writer, FooterString);
			}
			for (int i = ColFirst; i <= ColLast; i++)
			{
				ColumnInfo columnInfo = Columns[i];
				if (columnInfo != null)
				{
					RecordFactory.COLINFO(writer, (ushort)i, columnInfo.Width, columnInfo.OutlineLevel, columnInfo.Collapsed);
				}
				else
				{
					RecordFactory.COLINFO(writer, (ushort)i, 0.0, 0, collapsed: false);
				}
			}
			RecordFactory.DIMENSIONS(writer, RowFirst, RowLast, ColFirst, ColLast);
			byte[] array = new byte[4096];
			CellData.Seek(0L, SeekOrigin.Begin);
			int count;
			while ((count = CellData.Read(array, 0, array.Length)) > 0)
			{
				writer.BaseStream.Write(array, 0, count);
			}
			CellData.Close();
			CellData = null;
			if (m_drawingContainer != null)
			{
				foreach (ImageCache image in m_images)
				{
					if (image.HyperlinkURL == null)
					{
						m_drawingContainer.AddShape(image.ShapeID, image.Name, image.ClientAnchor, image.RefIndex);
					}
					else
					{
						m_drawingContainer.AddShape(image.ShapeID, image.Name, image.ClientAnchor, image.RefIndex, image.HyperlinkURL, image.IsBookmark ? Escher.HyperlinkType.BOOKMARK : Escher.HyperlinkType.URL);
					}
				}
				m_drawingContainer.WriteToStream(writer);
			}
			RecordFactory.WINDOW2(writer, m_rowSplit > 0 || m_columnSplit > 0, isFirstPage);
			if (m_rowSplit > 0 || m_columnSplit > 0)
			{
				RecordFactory.PANE(writer, m_columnSplit, m_rowSplit, m_rowSplit, m_columnSplit, 2);
			}
			writer.BaseStream.Write(Constants.WORKSHEET3, 0, Constants.WORKSHEET3.Length);
			RecordFactory.MERGECELLS(writer, MergeCellAreas);
			foreach (HyperlinkInfo hyperlink in m_hyperlinks)
			{
				RecordFactory.HLINK(writer, hyperlink);
			}
			writer.BaseStream.Write(Constants.WORKSHEET4, 0, Constants.WORKSHEET4.Length);
		}

		internal void AddImage(ushort drawingID, uint starterShapeID, string name, Escher.ClientAnchor.SPRC clientAnchor, uint referenceIndex, string hyperlinkURL, bool isBookmark)
		{
			if (m_drawingContainer == null)
			{
				m_drawingContainer = new Escher.DrawingContainer(drawingID);
				m_currentShapeID = starterShapeID;
			}
			if (hyperlinkURL != null)
			{
				m_images.Add(new ImageCache(m_currentShapeID, name, clientAnchor, referenceIndex, hyperlinkURL, isBookmark));
			}
			else
			{
				m_images.Add(new ImageCache(m_currentShapeID, name, clientAnchor, referenceIndex));
			}
			m_currentShapeID++;
		}

		internal void AddFreezePane(int row, int column)
		{
			m_rowSplit = (ushort)row;
			m_columnSplit = (ushort)column;
		}

		internal void AddHyperlink(int row, int column, string url, string label)
		{
			m_hyperlinks.Add(new HyperlinkInfo(url, label, row, row, column, column));
		}

		internal void AddBookmark(int row, int column, string bookmark, string label)
		{
			m_hyperlinks.Add(new BookmarkInfo(bookmark, label, row, row, column, column));
		}

		internal void AddPrintTitle(int externSheetIndex, int rowStart, int rowEnd)
		{
			m_printTitle = new PrintTitleInfo((ushort)externSheetIndex, (ushort)(SheetIndex + 1), (ushort)rowStart, (ushort)rowEnd);
		}

		internal void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin)
		{
			m_paperSize = paperSize;
			m_isPortrait = isPortrait;
			m_headerMargin = headerMargin;
			m_footerMargin = footerMargin;
		}

		internal void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			m_topMargin = topMargin;
			m_bottomMargin = bottomMargin;
			m_leftMargin = leftMargin;
			m_rightMargin = rightMargin;
		}
	}
}
