using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	public sealed class ReportDataSourceInfoCollection : ReadOnlyCollection<ReportDataSourceInfo>
	{
		public ReportDataSourceInfo this[string name]
		{
			get
			{
				using (IEnumerator<ReportDataSourceInfo> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReportDataSourceInfo current = enumerator.Current;
						if (string.Compare(current.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return current;
						}
					}
				}
				return null;
			}
		}

		internal ReportDataSourceInfoCollection(IList<ReportDataSourceInfo> dsInfos)
			: base(dsInfos)
		{
		}

		internal ReportDataSourceInfoCollection()
			: base((IList<ReportDataSourceInfo>)new ReportDataSourceInfo[0])
		{
		}
	}
}
