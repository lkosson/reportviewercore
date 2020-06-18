using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class ReportParameter
	{
		private string m_name = "";

		private StringCollection m_value = new StringCollection();

		private bool m_visible = true;

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public StringCollection Values => m_value;

		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				m_visible = value;
			}
		}

		public ReportParameter()
		{
		}

		public ReportParameter(string name)
			: this(name, new string[0])
		{
		}

		public ReportParameter(string name, string value)
			: this(name, new string[1]
			{
				value
			})
		{
		}

		public ReportParameter(string name, string[] values)
			: this(name, values, visible: true)
		{
		}

		public ReportParameter(string name, string value, bool visible)
			: this(name, new string[1]
			{
				value
			}, visible)
		{
		}

		public ReportParameter(string name, string[] values, bool visible)
		{
			Name = name;
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			Values.AddRange(values);
			Visible = visible;
		}

		internal static NameValueCollection ToNameValueCollection(IEnumerable<ReportParameter> reportParameters)
		{
			if (reportParameters == null)
			{
				throw new ArgumentNullException("reportParameters");
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (ReportParameter reportParameter in reportParameters)
			{
				if (reportParameter == null || reportParameter.Name == null)
				{
					throw new ArgumentNullException("reportParameters");
				}
				StringEnumerator enumerator2 = reportParameter.Values.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						nameValueCollection.Add(reportParameter.Name, current2);
					}
				}
				finally
				{
					(enumerator2 as IDisposable)?.Dispose();
				}
			}
			return nameValueCollection;
		}

		internal static ReportParameter[] FromNameValueCollection(NameValueCollection parameterColl)
		{
			if (parameterColl != null)
			{
				ReportParameter[] array = new ReportParameter[parameterColl.Keys.Count];
				for (int i = 0; i < parameterColl.Keys.Count; i++)
				{
					ReportParameter reportParameter = new ReportParameter();
					reportParameter.Name = parameterColl.GetKey(i);
					string[] array2 = parameterColl.GetValues(i);
					if (array2 == null)
					{
						array2 = new string[1];
					}
					reportParameter.Values.AddRange(array2);
					array[i] = reportParameter;
				}
				return array;
			}
			return new ReportParameter[0];
		}
	}
}
