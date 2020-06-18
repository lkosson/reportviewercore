using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	[EnumNames(typeof(Constants2005), "Calendar")]
	internal enum Calendar2005
	{
		Gregorian = 1,
		GregorianArabic,
		GregorianMiddleEastFrench,
		GregorianTransliteratedEnglish,
		GregorianTransliteratedFrench,
		GregorianUSEnglish,
		Hebrew,
		Hijri,
		Japanese,
		Korea,
		Taiwan,
		ThaiBuddhist
	}
}
