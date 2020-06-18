using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class SpatialElementKey
	{
		private List<object> m_keyValues;

		internal List<object> KeyValues => m_keyValues;

		internal SpatialElementKey(List<object> values)
		{
			m_keyValues = values;
		}

		public override bool Equals(object obj)
		{
			SpatialElementKey spatialElementKey = (SpatialElementKey)obj;
			if (m_keyValues == null || spatialElementKey.m_keyValues == null)
			{
				return false;
			}
			if (m_keyValues.Count != spatialElementKey.m_keyValues.Count)
			{
				return false;
			}
			for (int i = 0; i < m_keyValues.Count; i++)
			{
				object obj2 = m_keyValues[i];
				if (obj2 == null)
				{
					return false;
				}
				if (!obj2.Equals(spatialElementKey.m_keyValues[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (m_keyValues != null)
			{
				for (int i = 0; i < m_keyValues.Count; i++)
				{
					if (m_keyValues[i] != null)
					{
						num ^= m_keyValues[i].GetHashCode();
					}
				}
			}
			return num;
		}
	}
}
