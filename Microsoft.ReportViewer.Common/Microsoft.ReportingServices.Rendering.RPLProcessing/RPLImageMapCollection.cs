using System;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageMapCollection
	{
		private RPLImageMap[] m_list;

		public RPLImageMap this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new InvalidOperationException();
				}
				return m_list[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new InvalidOperationException();
				}
				m_list[index] = value;
			}
		}

		public int Count
		{
			get
			{
				if (m_list == null)
				{
					return 0;
				}
				return m_list.Length;
			}
		}

		internal RPLImageMapCollection(int count)
		{
			m_list = new RPLImageMap[count];
		}

		internal RPLImageMapCollection(RPLImageMap[] list)
		{
			m_list = list;
		}
	}
}
