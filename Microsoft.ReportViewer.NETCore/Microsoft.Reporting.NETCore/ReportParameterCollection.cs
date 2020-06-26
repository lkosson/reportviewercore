using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.NETCore
{
	public sealed class ReportParameterCollection : Collection<ReportParameter>
	{
		public ReportParameter this[string name]
		{
			get
			{
				using (IEnumerator<ReportParameter> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReportParameter current = enumerator.Current;
						string name2 = current.Name;
						if (string.Compare(name, name2, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return current;
						}
					}
				}
				return null;
			}
		}
	}
}
