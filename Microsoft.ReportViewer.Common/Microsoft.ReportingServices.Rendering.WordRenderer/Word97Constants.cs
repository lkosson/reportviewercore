namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class Word97Constants
	{
		internal const int TcSize = 20;

		internal const byte TcFirstMerge = 1;

		internal const byte TcMerge = 2;

		internal const byte TcVertMerge = 32;

		internal const byte TcVertRestart = 64;

		internal const byte TextPieceTableMarker = 2;

		internal const ushort TextPieceDescriptor = 64;

		internal const ushort DefaultStyleCount = 17;

		internal const short DefaultSepxSize = 24;

		internal const int WordPageSize = 512;

		internal const float InchInTwips = 1440f;

		internal const int NumHdrFtrOffsets = 14;

		internal const int NumHdrFtrOffsetsPerSection = 6;

		internal const byte PaddingTop = 1;

		internal const byte PaddingLeft = 2;

		internal const byte PaddingBottom = 4;

		internal const byte PaddingRight = 8;

		internal const byte PaddingTwips = 3;

		internal const int OddHeader = 8;

		internal const int EvenHeader = 7;

		internal const int FirstHeader = 11;

		internal const int OddFooter = 10;

		internal const int EvenFooter = 9;

		internal const int FirstFooter = 12;

		internal const string InlineImgCode = "\u0001";

		internal const int LeftJc = 0;

		internal const int CenterJc = 1;

		internal const int RightJc = 2;

		internal const float InchInMm = 25.4f;

		internal const int WordMaxColumns = 63;

		internal const float WordMaxWidth = 22f;

		internal const ushort WordMaxWidthTwips = 31680;

		internal const float WordMaxWidthMM = 558.8f;

		internal const int WorddmOrientPortrait = 1;

		internal const int WorddmOrientLandscape = 2;

		internal const string Word97ClsId = "00020906-0000-0000-c000-000000000046";
	}
}
