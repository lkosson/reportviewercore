using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ValueCollection
	{
		private ArrayList m_values;

		public object this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_values[index];
			}
		}

		public int Count => m_values.Count;

		internal ValueCollection()
		{
			m_values = new ArrayList();
		}

		internal ValueCollection(int capacity)
		{
			m_values = new ArrayList(capacity);
		}

		internal ValueCollection(ArrayList values)
		{
			m_values = values;
		}

		internal void Add(object value)
		{
			m_values.Add(value);
		}
	}
}
