using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class NativeGDIWrapper
	{
		public struct ABCFloat
		{
			public float abcfA;

			public float abcfB;

			public float abcfC;

			public ABCFloat(float a, float b, float c)
			{
				abcfA = a;
				abcfB = b;
				abcfC = c;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct OutlineTextMetric
		{
			public uint otmSize;

			public int tmHeight;

			public int tmAscent;

			public int tmDescent;

			public int tmInternalLeading;

			public int tmExternalLeading;

			public int tmAveCharWidth;

			public int tmMaxCharWidth;

			public int tmWeight;

			public int tmOverhang;

			public int tmDigitizedAspectX;

			public int tmDigitizedAspectY;

			public char tmFirstChar;

			public char tmLastChar;

			public char tmDefaultChar;

			public char tmBreakChar;

			public byte tmItalic;

			public byte tmUnderlined;

			public byte tmStruckOut;

			public byte tmPitchAndFamily;

			public byte tmCharSet;

			public byte otmFiller;

			public byte bFamilyType;

			public byte bSerifStyle;

			public byte bWeight;

			public byte bProportion;

			public byte bContrast;

			public byte bStrokeVariation;

			public byte bArmStyle;

			public byte bLetterform;

			public byte bMidline;

			public byte bXHeight;

			public uint otmfsSelection;

			public uint otmfsType;

			public int otmsCharSlopeRise;

			public int otmsCharSlopeRun;

			public int otmItalicAngle;

			public uint otmEMSquare;

			public int otmAscent;

			public int otmDescent;

			public uint otmLineGap;

			public uint otmsCapEmHeight;

			public uint otmsXHeight;

			public int left;

			public int top;

			public int right;

			public int bottom;

			public int otmMacAscent;

			public int otmMacDescent;

			public uint otmMacLineGap;

			public uint otmusMinimumPPEM;

			public int otmptSubscriptSizeX;

			public int otmptSubscriptSizeY;

			public int otmptSubscriptOffsetX;

			public int otmptSubscriptOffsetY;

			public int otmptSuperscriptSizeX;

			public int otmptSuperscriptSizeY;

			public int otmptSuperscriptOffsetX;

			public int otmptSuperscriptOffsetY;

			public uint otmsStrikeoutSize;

			public int otmsStrikeoutPosition;

			public int otmsUnderscoreSize;

			public int otmsUnderscorePosition;

			public string otmpFamilyName;

			public string otmpFaceName;

			public string otmpStyleName;

			public string otmpFullName;

			public OutlineTextMetric(string intialValue)
			{
				otmSize = 0u;
				tmHeight = 0;
				tmAscent = 0;
				tmDescent = 0;
				tmInternalLeading = 0;
				tmExternalLeading = 0;
				tmAveCharWidth = 0;
				tmMaxCharWidth = 0;
				tmWeight = 0;
				tmOverhang = 0;
				tmDigitizedAspectX = 0;
				tmDigitizedAspectY = 0;
				tmFirstChar = ' ';
				tmLastChar = ' ';
				tmDefaultChar = ' ';
				tmBreakChar = ' ';
				tmItalic = 0;
				tmUnderlined = 0;
				tmStruckOut = 0;
				tmPitchAndFamily = 0;
				tmCharSet = 0;
				otmFiller = 0;
				bFamilyType = 0;
				bSerifStyle = 0;
				bWeight = 0;
				bProportion = 0;
				bContrast = 0;
				bStrokeVariation = 0;
				bArmStyle = 0;
				bLetterform = 0;
				bMidline = 0;
				bXHeight = 0;
				otmfsSelection = 0u;
				otmfsType = 0u;
				otmsCharSlopeRise = 0;
				otmsCharSlopeRun = 0;
				otmItalicAngle = 0;
				otmEMSquare = 0u;
				otmAscent = 0;
				otmDescent = 0;
				otmLineGap = 0u;
				otmsCapEmHeight = 0u;
				otmsXHeight = 0u;
				left = 0;
				top = 0;
				right = 0;
				bottom = 0;
				otmMacAscent = 0;
				otmMacDescent = 0;
				otmMacLineGap = 0u;
				otmusMinimumPPEM = 0u;
				otmptSubscriptSizeX = 0;
				otmptSubscriptSizeY = 0;
				otmptSubscriptOffsetX = 0;
				otmptSubscriptOffsetY = 0;
				otmptSuperscriptSizeX = 0;
				otmptSuperscriptSizeY = 0;
				otmptSuperscriptOffsetX = 0;
				otmptSuperscriptOffsetY = 0;
				otmsStrikeoutSize = 0u;
				otmsStrikeoutPosition = 0;
				otmsUnderscoreSize = 0;
				otmsUnderscorePosition = 0;
				otmpFamilyName = intialValue;
				otmpFaceName = "";
				otmpStyleName = "";
				otmpFullName = "";
			}
		}

		private NativeGDIWrapper()
		{
		}

		[DllImport("GDI32")]
		public static extern IntPtr SelectObject(IntPtr hdc, [In] [Out] IntPtr hgdiobj);

		[DllImport("GDI32")]
		public static extern bool DeleteObject([In] [Out] IntPtr hgdiobj);

		[DllImport("GDI32")]
		public static extern int GetCharABCWidthsFloat(IntPtr hdc, uint iFirstChar, uint iLastChar, [In] [Out] ABCFloat[] lpABCF);

		[DllImport("GDI32")]
		public static extern uint GetGlyphIndicesW(IntPtr hdc, ushort[] lpstr, int c, ushort[] g, uint fl);

		[DllImport("GDI32")]
		public static extern bool GetTextExtentExPointI(IntPtr hdc, ushort[] pgiIn, int cgi, int nMaxExtent, ref int lpnFit, [In] [Out] int[] alpDx, ref Size lpSize);

		[DllImport("GDI32")]
		public static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, ref OutlineTextMetric lpOTM);
	}
}
