using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class Converter
	{
		internal const double Inches455 = 11557.0;

		internal const double Pt1 = 0.35277777777777775;

		internal const double Pc1 = 4.2333333333333325;

		internal const double Pt200 = 70.555555555555543;

		internal const double PtPoint25 = 0.08814;

		internal const double Pt20 = 7.0555555555555554;

		internal const double Pt1000 = 352.77777777777777;

		internal const string FullDoubleFormatCode = "0.###############";

		private Converter()
		{
		}

		internal static string ConvertSize(double size)
		{
			return size.ToString("0.###############", CultureInfo.InvariantCulture) + "mm";
		}

		internal static string ConvertSizeFromMM(double sizeValue, RVUnitType unitType)
		{
			string str = "mm";
			switch (unitType)
			{
			case RVUnitType.Cm:
				sizeValue /= 10.0;
				str = "cm";
				break;
			case RVUnitType.Inch:
				sizeValue /= 25.4;
				str = "in";
				break;
			case RVUnitType.Pica:
				sizeValue /= 4.2333333333333325;
				str = "pc";
				break;
			case RVUnitType.Point:
				sizeValue /= 0.35277777777777775;
				str = "pt";
				break;
			default:
				unitType = RVUnitType.Mm;
				break;
			}
			return Math.Round(sizeValue, 5).ToString(CultureInfo.InvariantCulture) + str;
		}

		internal static double ConvertToMM(RVUnit unit)
		{
			if (!Validator.ValidateSizeUnitType(unit))
			{
				Global.Tracer.Assert(condition: false);
			}
			return unit.ToMillimeters();
		}
	}
}
