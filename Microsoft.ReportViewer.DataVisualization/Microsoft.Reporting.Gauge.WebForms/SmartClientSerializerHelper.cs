using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal static class SmartClientSerializerHelper
	{
		private static CaseInsensitiveHashCodeProvider hashCodeProvider = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture);
	}
}
