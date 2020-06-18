using Microsoft.ReportingServices.Common;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Converter
	{
		internal static double Inches160 = 4064.0;

		internal static double Pt1 = 0.3528;

		internal static double Pt200 = 70.56;

		internal static double PtPoint25 = 0.08814;

		internal static double Pt20 = 7.056;

		internal static double Pt1000 = 352.8;

		private Converter()
		{
		}

		internal static string ConvertSize(double size)
		{
			return size.ToString(CultureInfo.InvariantCulture) + "mm";
		}

		internal static double ConvertToMM(RVUnit unit)
		{
			double num = unit.Value;
			switch (unit.Type)
			{
			case RVUnitType.Cm:
				num *= 10.0;
				break;
			case RVUnitType.Inch:
				num *= 25.4;
				break;
			case RVUnitType.Pica:
				num *= 4.2333;
				break;
			case RVUnitType.Point:
				num *= 0.3528;
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			case RVUnitType.Mm:
				break;
			}
			return num;
		}
	}
}
