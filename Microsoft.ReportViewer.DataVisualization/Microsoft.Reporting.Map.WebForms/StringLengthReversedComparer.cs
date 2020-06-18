using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class StringLengthReversedComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			if (x is string && y is string)
			{
				string text = (string)x;
				string text2 = (string)y;
				if (text.Length > text2.Length)
				{
					return -1;
				}
				if (text.Length < text2.Length)
				{
					return 1;
				}
				return new Comparer(CultureInfo.InvariantCulture).Compare(x, y);
			}
			return 0;
		}
	}
}
