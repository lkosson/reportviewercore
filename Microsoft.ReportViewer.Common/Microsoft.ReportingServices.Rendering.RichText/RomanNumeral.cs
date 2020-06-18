namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class RomanNumeral
	{
		internal int ArabicNumber;

		internal string RomanNumeralString;

		internal static RomanNumeral[] RomanNumerals = new RomanNumeral[16]
		{
			new RomanNumeral(1000, "m"),
			new RomanNumeral(500, "d"),
			new RomanNumeral(100, "c"),
			new RomanNumeral(50, "l"),
			new RomanNumeral(12, "xii"),
			new RomanNumeral(11, "xi"),
			new RomanNumeral(10, "x"),
			new RomanNumeral(9, "ix"),
			new RomanNumeral(8, "viii"),
			new RomanNumeral(7, "vii"),
			new RomanNumeral(6, "vi"),
			new RomanNumeral(5, "v"),
			new RomanNumeral(4, "iv"),
			new RomanNumeral(3, "iii"),
			new RomanNumeral(2, "ii"),
			new RomanNumeral(1, "i")
		};

		internal RomanNumeral(int arabicNumber, string romanNumeralString)
		{
			ArabicNumber = arabicNumber;
			RomanNumeralString = romanNumeralString;
		}
	}
}
