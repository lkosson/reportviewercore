namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLFormat2008
	{
		public enum SortOptions : byte
		{
			None,
			Ascending,
			Descending
		}

		public enum Sizings : byte
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		public enum ShapeType : byte
		{
			Rectangle,
			Polygon,
			Circle
		}

		public enum LayoutDirection : byte
		{
			Columns,
			Rows
		}

		public enum ImageRawFormat : byte
		{
			BMP,
			JPEG,
			GIF,
			PNG
		}

		public enum BackgroundGradients : byte
		{
			None,
			LeftRight,
			TopBottom,
			Center,
			DiagonalLeft,
			DiagonalRight,
			HorizontalCenter,
			VerticalCenter
		}

		public enum FontStyles : byte
		{
			Normal,
			Italic
		}

		public enum FontWeights : byte
		{
			Normal,
			Thin,
			ExtraLight,
			Light,
			Medium,
			SemiBold,
			Bold,
			ExtraBold,
			Heavy
		}

		public enum TextDecorations : byte
		{
			None,
			Underline,
			Overline,
			LineThrough
		}

		public enum TextAlignments : byte
		{
			General,
			Left,
			Center,
			Right
		}

		public enum VerticalAlignments : byte
		{
			Top,
			Middle,
			Bottom
		}

		public enum Directions : byte
		{
			LTR,
			RTL
		}

		public enum WritingModes : byte
		{
			Horizontal,
			Vertical
		}

		public enum UnicodeBiDiTypes : byte
		{
			Normal,
			Embed,
			BiDiOverride
		}

		public enum Calendars : byte
		{
			Gregorian,
			GregorianArabic,
			GregorianMiddleEastFrench,
			GregorianTransliteratedEnglish,
			GregorianTransliteratedFrench,
			GregorianUSEnglish,
			Hebrew,
			Hijri,
			Japanese,
			Korean,
			Julian,
			Taiwan,
			ThaiBuddhist
		}

		public enum BorderStyles : byte
		{
			None,
			Dotted,
			Dashed,
			Solid,
			Double
		}

		public enum BackgroundRepeatTypes : byte
		{
			Repeat,
			Clip,
			RepeatX,
			RepeatY
		}

		public enum ListStyles : byte
		{
			None,
			Numbered,
			Bulleted
		}

		public enum MarkupStyles : byte
		{
			None,
			HTML,
			RTF
		}

		public class StateValues
		{
			public const byte OmitTop = 1;

			public const byte OmitBottom = 2;

			public const byte OmitLeft = 4;

			public const byte OmitRight = 8;

			public const byte HasToggle = 16;

			public const byte ToggleCollapse = 32;
		}

		public class ItemProps
		{
			public const byte UniqueName = 0;

			public const byte ID = 1;

			public const byte Name = 2;

			public const byte Label = 3;

			public const byte Bookmark = 4;

			public const byte ToolTip = 5;

			public const byte StyleStart = 6;

			public const byte ActionInfoStart = 7;

			public const byte ToggleItem = 8;

			public const byte Description = 9;

			public const byte Location = 10;

			public const byte Language = 11;

			public const byte ExecutionTime = 12;

			public const byte Author = 13;

			public const byte AutoRefresh = 14;

			public const byte ReportName = 15;

			public const byte PageHeight = 16;

			public const byte PageWidth = 17;

			public const byte MarginTop = 18;

			public const byte MarginLeft = 19;

			public const byte MarginBottom = 20;

			public const byte MarginRight = 21;

			public const byte ColumnSpacing = 22;

			public const byte Columns = 23;

			public const byte Slant = 24;

			public const byte CanGrow = 25;

			public const byte CanShrink = 26;

			public const byte Value = 27;

			public const byte ToggleState = 28;

			public const byte CanSort = 29;

			public const byte SortState = 30;

			public const byte Formula = 31;

			public const byte IsToggleParent = 32;

			public const byte TypeCode = 33;

			public const byte OriginalValue = 34;

			public const byte IsSimple = 35;

			public const byte ContentHeight = 36;

			public const byte ContentOffset = 37;

			public const byte ActionImageMapAreasStart = 38;

			public const byte DynamicImageDataStart = 39;

			public const byte StreamName = 40;

			public const byte Sizing = 41;

			public const byte ImageStart = 42;

			public const byte LinkToChild = 43;

			public const byte PrintOnFirstPage = 44;

			public const byte FormattedValueExpressionBased = 45;

			public const byte ProcessedWithError = 46;

			public const byte ImageConsolidationOffsets = 47;
		}

		public class ImageProps
		{
			public const byte ImageMimeType = 0;

			public const byte ImageName = 1;

			public const byte ImageDataStart = 2;

			public const byte Width = 3;

			public const byte Height = 4;

			public const byte HorizontalResolution = 5;

			public const byte VerticalResolution = 6;

			public const byte RawFormat = 7;
		}

		public class ActionInfoProps
		{
			public const byte ActionInfoStyleStart = 0;

			public const byte LayoutDirection = 1;

			public const byte ActionsStart = 2;

			public const byte ActionStart = 3;

			public const byte Label = 4;

			public const byte ActionStyleStart = 5;

			public const byte HyperLink = 6;

			public const byte BookmarkLink = 7;

			public const byte DrillthroughId = 8;

			public const byte DrillthroughUrl = 9;

			public const byte ImageMapAreasStart = 10;
		}

		public class StyleProps
		{
			public const byte BorderColor = 0;

			public const byte BorderColorLeft = 1;

			public const byte BorderColorRight = 2;

			public const byte BorderColorTop = 3;

			public const byte BorderColorBottom = 4;

			public const byte BorderStyle = 5;

			public const byte BorderStyleLeft = 6;

			public const byte BorderStyleRight = 7;

			public const byte BorderStyleTop = 8;

			public const byte BorderStyleBottom = 9;

			public const byte BorderWidth = 10;

			public const byte BorderWidthLeft = 11;

			public const byte BorderWidthRight = 12;

			public const byte BorderWidthTop = 13;

			public const byte BorderWidthBottom = 14;

			public const byte PaddingLeft = 15;

			public const byte PaddingRight = 16;

			public const byte PaddingTop = 17;

			public const byte PaddingBottom = 18;

			public const byte FontStyle = 19;

			public const byte FontFamily = 20;

			public const byte FontSize = 21;

			public const byte FontWeight = 22;

			public const byte Format = 23;

			public const byte TextDecoration = 24;

			public const byte TextAlign = 25;

			public const byte VerticalAlign = 26;

			public const byte Color = 27;

			public const byte LineHeight = 28;

			public const byte Direction = 29;

			public const byte WritingMode = 30;

			public const byte UnicodeBiDi = 31;

			public const byte Language = 32;

			public const byte BackgroundImage = 33;

			public const byte BackgroundColor = 34;

			public const byte BackgroundRepeat = 35;

			public const byte NumeralLanguage = 36;

			public const byte NumeralVariant = 37;

			public const byte Calendar = 38;
		}

		public class TablixMeasurements
		{
			public const byte FixedRows = 1;

			public const byte SharedLayoutRow = 2;

			public const byte UseSharedLayoutRow = 4;

			public const byte ColumnHeaderRows = 0;

			public const byte RowHeaderColumns = 1;

			public const byte ColsBeforeRowHeaders = 2;

			public const byte LayoutDirection = 3;

			public const byte ColumnsWidthsStart = 4;

			public const byte RowHeightsStart = 5;

			public const byte ContentTop = 6;

			public const byte ContentLeft = 7;

			public const byte TablixRowStart = 8;

			public const byte TablixBodyRowCellsStart = 9;

			public const byte TablixCornerStart = 10;

			public const byte TablixColumnHeaderStart = 11;

			public const byte TablixRowHeaderStart = 12;

			public const byte TablixBodyCellStart = 13;

			public const byte TablixRowMembersDefStart = 14;

			public const byte TablixColMembersDefStart = 15;

			public const byte TablixMemberDefStart = 16;
		}

		public class TablixMemberDefStateValues
		{
			public const byte Column = 1;

			public const byte Static = 2;

			public const byte StaticHeadersTree = 4;
		}

		public class TablixMemberDefProps
		{
			public const byte DefinitionPath = 0;

			public const byte Level = 1;

			public const byte MemberCellIndex = 2;

			public const byte State = 3;
		}

		public class TablixMemberStateValues
		{
			public const byte HasToggle = 1;

			public const byte ToggleCollapse = 2;

			public const byte InnerMost = 4;
		}

		public class ContentSizeProps
		{
			public const byte ContentTop = 0;

			public const byte ContentLeft = 1;

			public const byte ContentWidth = 2;

			public const byte ContentHeight = 3;
		}

		public class TablixCellProps : ContentSizeProps
		{
			public const byte CellItemOffset = 4;

			public const byte ColSpan = 5;

			public const byte RowSpan = 6;

			public const byte DefIndex = 7;

			public const byte ColumnIndex = 8;

			public const byte RowIndex = 9;

			public const byte GroupLabel = 10;

			public const byte UniqueName = 11;

			public const byte State = 12;

			public const byte CellItemState = 13;

			public const byte RecursiveToggleLevel = 14;
		}

		public class ParagraphProps : ContentSizeProps
		{
			public const byte UniqueName = 4;

			public const byte ID = 5;

			public const byte StyleStart = 6;

			public const byte ListStyle = 7;

			public const byte ListLevel = 8;

			public const byte LeftIndent = 9;

			public const byte RightIndent = 10;

			public const byte HangingIndent = 11;

			public const byte SpaceBefore = 12;

			public const byte SpaceAfter = 13;

			public const byte ParagraphNumber = 14;

			public const byte FirstLine = 15;
		}

		public class TextRunProps : ContentSizeProps
		{
			public const byte UniqueName = 4;

			public const byte ID = 5;

			public const byte StyleStart = 6;

			public const byte Markup = 7;

			public const byte Label = 8;

			public const byte ToolTip = 9;

			public const byte Value = 10;

			public const byte ActionInfoStart = 11;

			public const byte Formula = 12;

			public const byte ProcessedWithError = 13;
		}

		public const byte Shared = 0;

		public const byte NonShared = 1;

		public const byte UseShared = 2;

		public const byte OffsetsArrayElementStart = 18;

		public const byte PageContentStart = 19;

		public const byte Columns = 20;

		public const byte ReportStart = 0;

		public const byte PageStart = 1;

		public const byte ReportPropertiesStart = 2;

		public const byte PagePropertiesStart = 3;

		public const byte PageHeader = 4;

		public const byte PageFooter = 5;

		public const byte Body = 6;

		public const byte TextBox = 7;

		public const byte Line = 8;

		public const byte Image = 9;

		public const byte Rectangle = 10;

		public const byte Chart = 11;

		public const byte SubReport = 12;

		public const byte Tablix = 13;

		public const byte GaugePanel = 14;

		public const byte ElementPropsStart = 15;

		public const byte MeasurementsStart = 16;

		public const byte TablixStructureStart = 17;

		public const byte TablixBodyRowStart = 18;

		public const byte RichTextBoxStructureStart = 18;

		public const byte ParagraphStart = 19;

		public const byte TextRunStart = 20;

		public const byte ElementEnd = 254;

		public const byte Delimiter = byte.MaxValue;
	}
}
