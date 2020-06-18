using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.WinForms
{
	public sealed class DataSourceCredentialsCollection : Collection<DataSourceCredentials>
	{
		public DataSourceCredentials this[string name]
		{
			get
			{
				using (IEnumerator<DataSourceCredentials> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataSourceCredentials current = enumerator.Current;
						if (string.Compare(current.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
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
