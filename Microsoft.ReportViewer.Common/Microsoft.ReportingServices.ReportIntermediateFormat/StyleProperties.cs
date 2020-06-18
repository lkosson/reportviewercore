using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportPublishing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class StyleProperties
	{
		private Hashtable m_nameMap;

		private ArrayList m_valueCollection;

		public object this[int index] => m_valueCollection[index];

		public object this[string styleName]
		{
			get
			{
				object obj = m_nameMap[styleName];
				if (obj != null)
				{
					return m_valueCollection[(int)obj];
				}
				return null;
			}
		}

		public int Count => m_valueCollection.Count;

		public ICollection Keys => m_nameMap.Keys;

		internal StyleProperties()
		{
			m_nameMap = new Hashtable();
			m_valueCollection = new ArrayList();
		}

		internal StyleProperties(int capacity)
		{
			m_nameMap = new Hashtable(capacity);
			m_valueCollection = new ArrayList(capacity);
		}

		public bool ContainStyleProperty(string styleName)
		{
			if (styleName == null || m_nameMap.Count == 0)
			{
				return false;
			}
			return m_nameMap.ContainsKey(styleName);
		}

		internal void Add(string name, object value)
		{
			if (!m_nameMap.Contains(name))
			{
				m_nameMap.Add(name, m_valueCollection.Count);
				m_valueCollection.Add(value);
			}
		}

		internal void Set(string name, object value)
		{
			object obj = m_nameMap[name];
			if (obj != null)
			{
				m_valueCollection[(int)obj] = value;
				return;
			}
			m_nameMap.Add(name, m_valueCollection.Count);
			m_valueCollection.Add(value);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			StyleProperties styleProperties = (StyleProperties)MemberwiseClone();
			if (m_nameMap != null)
			{
				styleProperties.m_nameMap = new Hashtable(m_nameMap.Count);
				styleProperties.m_valueCollection = new ArrayList(m_valueCollection);
				{
					foreach (DictionaryEntry item in m_nameMap)
					{
						styleProperties.m_nameMap.Add(((string)item.Key).Clone(), item.Value);
						object obj = m_valueCollection[(int)item.Value];
						object value = null;
						if (obj is string)
						{
							value = string.Copy(obj as string);
						}
						else if (obj is int)
						{
							value = (int)obj;
						}
						else if (obj is ReportSize)
						{
							value = ((ReportSize)obj).DeepClone();
						}
						styleProperties.m_valueCollection[(int)item.Value] = value;
					}
					return styleProperties;
				}
			}
			return styleProperties;
		}
	}
}
