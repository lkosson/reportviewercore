using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal static class WordOpenXmlConstants
	{
		internal static class Spacing
		{
			internal const int MaxTwipsBefore = 31680;

			internal const int MinTwipsBefore = 0;

			internal const int MaxTwipsAfter = 31680;

			internal const int MinTwipsAfter = 0;

			internal const int MaxLineSpacing = 31680;

			internal const int MinLineSpacing = 0;
		}

		internal static class Images
		{
			internal const int MinHeight = 0;

			internal const int MaxHeight = 20116800;

			internal const int MinWidth = 0;

			internal const int MaxWidth = 20116800;
		}

		internal static class PageSize
		{
			internal const int MinHeight = 144;

			internal const int MaxHeight = 31680;

			internal const int MinWidth = 144;

			internal const int MaxWidth = 31680;
		}

		internal static class PageMargins
		{
			internal const int Min = 0;

			internal const int Max = 31680;
		}

		internal static class Indentation
		{
			internal static class Left
			{
				internal const int MinTwips = -31680;

				internal const int MaxTwips = 31680;
			}

			internal static class Right
			{
				internal const int MinTwips = -31680;

				internal const int MaxTwips = 31680;
			}

			internal static class Hanging
			{
				internal const int MinTwips = 0;

				internal const int MaxTwips = 31680;
			}

			internal static class FirstLine
			{
				internal const int MinTwips = 0;

				internal const int MaxTwips = 31680;
			}

			internal static class ListIndentations
			{
				internal const int HangingPoints = 18;

				internal const int LeftPointsPerLevel = 36;
			}
		}

		internal static class Lists
		{
			internal static class Bulleted
			{
				internal const int StyleId = 1;

				internal static readonly ST_NumberFormat LevelStyle = ST_NumberFormat.bullet;

				internal static readonly string[] BulletTexts = new string[3]
				{
					"·",
					"o",
					"§"
				};

				internal static readonly string[] BulletFonts = new string[3]
				{
					"Symbol",
					"Courier New",
					"Wingdings"
				};

				internal const string BulletSize = "20";

				internal static readonly ST_Jc LevelJc = ST_Jc.left;
			}

			internal static class Numbered
			{
				internal const int FirstStyleId = 2;

				internal static readonly ST_NumberFormat[] LevelStyles = new ST_NumberFormat[3]
				{
					ST_NumberFormat._decimal,
					ST_NumberFormat.lowerRoman,
					ST_NumberFormat.lowerLetter
				};

				internal const string LevelTextFormat = "%{0}.";

				internal const string BulletSize = "20";

				internal const string BulletFont = "Arial";

				internal static readonly ST_Jc LevelJc = ST_Jc.left;
			}

			internal const int MaxLevelDefinition = 9;
		}

		internal static class ContentTypes
		{
			internal const string Document = "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml";

			internal const string Stylesheet = "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml";

			internal const string Numbering = "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml";

			internal const string Settings = "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml";

			internal const string Header = "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml";

			internal const string Footer = "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml";

			internal const string DocumentProperties = "application/vnd.openxmlformats-package.core-properties+xml";
		}

		internal static class RelationshipTypes
		{
			internal const string Document = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

			internal const string Stylesheet = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";

			internal const string Numbering = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering";

			internal const string Settings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings";

			internal const string Header = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header";

			internal const string Footer = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer";

			internal const string Image = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";

			internal const string DocumentProperties = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";
		}

		internal static class DefaultPaths
		{
			internal const string Document = "word/document.xml";

			internal const string Stylesheet = "word/styles.xml";

			internal const string Numbering = "word/numbering.xml";

			internal const string Settings = "word/settings.xml";

			internal const string Header = "word/header{0}.xml";

			internal const string Footer = "word/footer{0}.xml";

			internal const string Image = "word/media/img{0}.{{0}}";

			internal const string DocumentProperties = "docProps/core.xml";
		}

		internal static class FileExtensions
		{
			internal const string JPEG = "jpg";

			internal const string PNG = "png";

			internal const string GIF = "gif";

			internal const string BMP = "bmp";
		}

		internal static class NamedStyles
		{
			internal const string EmptyCellStyle = "EmptyCellLayoutStyle";

			internal const string BaseStyle = "Normal";
		}

		internal static class Fonts
		{
			internal const int EmptyCellFontSize = 2;
		}

		internal static class TableXml
		{
			public const string TableOpenTag = "<w:tbl>";

			public const string TableCloseTag = "</w:tbl>";

			public const string RowOpenTag = "<w:tr>";

			public const string RowCloseTag = "</w:tr>";

			public const string CellOpenTag = "<w:tc>";

			public const string CellCloseTag = "</w:tc>";
		}

		internal static class TableGridXml
		{
			public const string OpenTag = "<w:tblGrid>";

			public const string CloseTag = "</w:tblGrid>";

			public const string ColumnBegin = "<w:gridCol w:w=\"";

			public const string ColumnEnd = "\"/>";
		}

		internal static class TablePropertiesXml
		{
			public static class TableCellMargins
			{
				public const string Default = "<w:tblCellMar><w:top w:w=\"0\" w:type=\"dxa\"/><w:left w:w=\"0\" w:type=\"dxa\"/><w:bottom w:w=\"0\" w:type=\"dxa\"/><w:right w:w=\"0\" w:type=\"dxa\"/></w:tblCellMar>";

				private const string OpenTag = "<w:tblCellMar>";

				private const string CloseTag = "</w:tblCellMar>";

				private const string Begin = "<w:";

				private const string End = " w:w=\"0\" w:type=\"dxa\"/>";
			}

			public const string OpenTag = "<w:tblPr>";

			public const string CloseTag = "</w:tblPr>";

			public const string ShadingBegin = "<w:shd w:val=\"clear\" w:fill=\"";

			public const string ShadingEnd = "\"/>";

			public const string BordersOpenTag = "<w:tblBorders>";

			public const string BordersCloseTag = "</w:tblBorders>";

			public const string FixedWidth = "<w:tblLayout w:type=\"fixed\"/>";
		}

		internal static class RowPropertiesXml
		{
			internal static class TableEx
			{
				public const string OpenTag = "<w:tblPrEx>";

				public const string CloseTag = "</w:tblPrEx>";

				public const string BordersOpenTag = "<w:tblBorders>";

				public const string BordersCloseTag = "</w:tblBorders>";

				public const string ShadingBegin = "<w:shd w:val=\"clear\" w:fill=\"";

				public const string ShadingEnd = "\"/>";
			}

			public const string OpenTag = "<w:trPr>";

			public const string CloseTag = "</w:trPr>";

			public const string HeightBegin = "<w:trHeight w:val=\"";

			public const string HeightExactEnd = "\" w:hRule=\"exact\"/>";

			public const string HeightAtLeastEnd = "\" w:hRule=\"atLeast\"/>";

			public const string IndentBegin = "<w:wBefore w:w=\"";

			public const string IndentEnd = "\" w:type=\"dxa\"/>";
		}

		internal static class CellPropertiesXml
		{
			public const string BordersOpenTag = "<w:tcBorders>";

			public const string BordersCloseTag = "</w:tcBorders>";
		}

		internal static class BordersXml
		{
			internal static class BorderPropertiesXml
			{
				public const string Prefix = "<w:";

				public const string DashSmallGap = " w:val=\"dashSmallGap\"";

				public const string Dotted = " w:val=\"dotted\"";

				public const string Double = " w:val=\"double\"";

				public const string Nil = " w:val=\"nil\"";

				public const string Single = " w:val=\"single\"";

				public const string None = " w:val=\"none\"";

				private const string StyleBegin = " w:val=\"";

				private const string StyleEnd = "\"";

				public const string ColorBegin = " w:color=\"";

				public const string AttributeEnd = "\"";

				public const string SizeBegin = " w:sz=\"";

				public const string EndEmptyTag = "/>";
			}

			public const string TopName = "top";

			public const string LeftName = "left";

			public const string BottomName = "bottom";

			public const string RightName = "right";

			public const string DiagonalDownName = "tl2br";

			public const string DiagonalUpName = "tr2bl";
		}

		internal static class ParagraphXml
		{
			public const string CloseTag = "</w:p>";

			public const string OpenTag = "<w:p>";

			public const string InvisibleParagraph = "<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"0\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr></w:p>";

			public const string EmptyParagraph = "<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>";

			public const string EmptyLayoutCellParagraph = "<w:p><w:pPr><w:pStyle w:val=\"EmptyCellLayoutStyle\"/><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>";

			public const string PageBreakParagraph = "<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr><w:r><w:br w:type=\"page\"/></w:r></w:p>";
		}

		internal static class ParagraphPropertiesXml
		{
			public static class ListProperties
			{
				public const string OpenTag = "<w:numPr>";

				public const string LevelStart = "<w:ilvl w:val=\"";

				public const string LevelEnd = "\"/>";

				public const string NumberIdStart = "<w:numId w:val=\"";

				public const string NumberIdEnd = "\"/>";

				public const string CloseTag = "</w:numPr>";
			}

			public static class Spacing
			{
				public const string StartOfTag = "<w:spacing ";

				public const string BeforeBegin = "w:before=\"";

				public const string BeforeEnd = "\" ";

				public const string AfterBegin = "w:after=\"";

				public const string LineBegin = "\" w:line=\"";

				public const string LineRuleAtLeast = "\" w:lineRule=\"atLeast\"/>";

				public const string LineRuleAuto = "\" w:lineRule=\"auto\"/>";
			}

			public static class Indentation
			{
				public const string StartOfTag = "<w:ind";

				public const string AttributeEnd = "\"";

				public const string LeftBegin = " w:left=\"";

				public const string RightBegin = " w:right=\"";

				public const string HangingBegin = " w:hanging=\"";

				public const string FirstLineBegin = " w:firstLine=\"";

				public const string EndOfTag = "/>";
			}

			public const string OpenTag = "<w:pPr>";

			public const string CloseTag = "</w:pPr>";

			public const string InvisibleProperties = "<w:pPr><w:spacing w:after=\"0\" w:line=\"0\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr>";

			public const string DefaultProperties = "<w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr>";

			public const string EmptyLayoutCellProperties = "<w:pPr><w:pStyle w:val=\"EmptyCellLayoutStyle\"/><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr>";

			public const string PageBreakProperties = "<w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr>";

			public const string BidiTag = "<w:bidi/>";

			public const string JcCenter = "<w:jc w:val=\"center\"/>";

			public const string JcLeft = "<w:jc w:val=\"left\"/>";

			public const string JcRight = "<w:jc w:val=\"right\"/>";
		}

		public static class RunXml
		{
			public const string OpenTag = "<w:r>";

			public const string TextOpenTag = "<w:t xml:space=\"preserve\">";

			public const string TextCloseTag = "</w:t>";

			public const string SoftBreak = "<w:br/>";

			public const string CloseTag = "</w:r>";
		}

		public static class RunPropertiesXml
		{
			public const string OpenTag = "<w:rPr>";

			public const string CloseTag = "</w:rPr>";

			public const string BoldTag = "<w:b/>";

			public const string RightToLeftBoldTag = "<w:bCs/>";

			public const string ItalicTag = "<w:i/>";

			public const string RightToLeftItalicTag = "<w:iCs/>";

			public const string ColorBegin = "<w:color w:val=\"";

			public const string ColorEnd = "\"/>";

			public const string FontBegin = "<w:rFonts";

			public const string AsciiBegin = " w:ascii=\"";

			public const string HAnsiBegin = " w:hAnsi=\"";

			public const string EastAsiaBegin = " w:eastAsia=\"";

			public const string FontCsBegin = " w:cs=\"";

			public const string NoProofTag = "<w:noProof/>";

			public const string RightToLeftTag = "<w:rtl/>";

			public const string LanguageBegin = "<w:lang";

			public const string LanguageEnd = "/>";

			public const string BidiBegin = " w:bidi=\"";

			public const string StrikeTag = "<w:strike/>";

			public const string SizeBegin = "<w:sz w:val=\"";

			public const string SizeEnd = "\"/>";

			public const string RightToLeftSizeBegin = "<w:szCs w:val=\"";

			public const string RightToLeftSizeEnd = "\"/>";

			public const string UnderlineTag = "<w:u w:val=\"single\"/>";

			public const string AttributeEnd = "\"";

			public const string FontEnd = "/>";
		}

		public static class FieldXml
		{
			public const string FieldCharBegin = "<w:fldChar w:fldCharType=\"begin\" w:fldLock=\"0\" w:dirty=\"0\"/>";

			public const string FieldCharSeparate = "<w:fldChar w:fldCharType=\"separate\" w:fldLock=\"0\" w:dirty=\"0\"/>";

			public const string FieldCharEnd = "<w:fldChar w:fldCharType=\"end\" w:fldLock=\"0\" w:dirty=\"0\"/>";

			private const string FieldCharXmlBegin = "<w:fldChar w:fldCharType=\"";

			private const string FieldCharXmlEnd = "\" w:fldLock=\"0\" w:dirty=\"0\"/>";

			public const string PageNumberFieldInstructions = "<w:instrText xml:space=\"preserve\"> PAGE </w:instrText>";

			public const string PageCountFieldInstructions = "<w:instrText xml:space=\"preserve\"> NUMPAGES </w:instrText>";

			public const string TableOfContentsEntryBegin = "<w:instrText xml:space=\"preserve\"> TC &quot;";

			public const string TableOfContentsEntryMiddle = "&quot; \\f C \\l &quot;";

			public const string TableOfContentsEntryEnd = "&quot; </w:instrText>";

			public const string BookmarkLinkBegin = "<w:instrText xml:space=\"preserve\"> HYPERLINK \\l &quot;";

			public const string HyperlinkBegin = "<w:instrText xml:space=\"preserve\"> HYPERLINK &quot;";

			public const string HyperlinkEnd = "&quot; </w:instrText>";

			private const string InstructionsOpenTag = "<w:instrText xml:space=\"preserve\">";

			private const string InstructionsCloseTag = "</w:instrText>";

			public const string NoProofProperties = "<w:rPr><w:noProof/></w:rPr>";

			public const string OneText = "<w:t xml:space=\"preserve\">1</w:t>";
		}

		public static class BookmarkXml
		{
			public const string BookmarkStartBegin = "<w:bookmarkStart w:id=\"";

			public const string BookmarkStartMiddle = "\" w:name=\"";

			public const string BookmarkStartEnd = "\"/>";

			public const string BookmarkEndBegin = "<w:bookmarkEnd w:id=\"";

			public const string BookmarkEndEnd = "\"/>";
		}

		public const int STREAM_COPY_BUFFER_SIZE = 1024;

		internal const CompressionOption CompressionLevel = CompressionOption.Normal;

		internal const int TableRowMaxHeight = 31680;

		internal const int TableRowMinHeight = 0;

		internal const int TableCellMaxWidth = 31680;

		internal const int TableCellMinWidth = 0;

		internal const float TableMaxWidth = 31680f;

		internal const float TableMinWidth = 0f;

		internal const int TableCellMaxPadding = 31680;

		internal const int TableCellMinPadding = 0;

		internal const int BorderMinWidth = 0;

		internal const int BorderMaxWidth = 255;

		public const string DefaultFont = "Times New Roman";
	}
}
