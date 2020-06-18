using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class FontCache
	{
		private Dictionary<string, Font> fontCache = new Dictionary<string, Font>();

		internal Font GetFont(string name, int size)
		{
			string key = string.Format(CultureInfo.InvariantCulture, "Name:{0}, Size:{1}", name, size);
			if (!fontCache.ContainsKey(key))
			{
				fontCache.Add(key, new Font(name, size));
			}
			return fontCache[key];
		}

		~FontCache()
		{
			foreach (Font value in fontCache.Values)
			{
				value.Dispose();
			}
		}
	}
}
