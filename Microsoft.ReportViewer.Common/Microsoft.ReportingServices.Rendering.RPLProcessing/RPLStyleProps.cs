using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLStyleProps : IRPLStyle, IEnumerable<byte>, IEnumerable
	{
		private Dictionary<byte, object> m_styleMap;

		public object this[byte styleName]
		{
			get
			{
				object value = null;
				if (m_styleMap.TryGetValue(styleName, out value))
				{
					return value;
				}
				return null;
			}
		}

		public int Count => m_styleMap.Count;

		internal RPLStyleProps()
		{
			m_styleMap = new Dictionary<byte, object>();
		}

		internal void Add(byte name, object value)
		{
			m_styleMap.Add(name, value);
		}

		internal void AddAll(RPLStyleProps styleProps)
		{
			if (styleProps == null)
			{
				return;
			}
			foreach (KeyValuePair<byte, object> item in styleProps.m_styleMap)
			{
				Add(item.Key, item.Value);
			}
		}

		IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
		{
			return m_styleMap.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_styleMap.Keys.GetEnumerator();
		}
	}
}
