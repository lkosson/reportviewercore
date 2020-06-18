using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class ParametersImpl : Parameters
	{
		private Hashtable m_nameMap;

		private ParameterImpl[] m_collection;

		private int m_count;

		internal const string Name = "Parameters";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.Parameters";

		public override Parameter this[string key]
		{
			get
			{
				if (key == null || m_nameMap == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingParameterReference(key);
				}
				try
				{
					return m_collection[(int)m_nameMap[key]];
				}
				catch
				{
					throw new ReportProcessingException_NonExistingParameterReference(key);
				}
			}
		}

		internal ParametersImpl(int size)
		{
			m_collection = new ParameterImpl[size];
			m_nameMap = new Hashtable(size);
			m_count = 0;
		}

		internal void Add(string name, ParameterImpl parameter)
		{
			Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
			Global.Tracer.Assert(m_nameMap != null, "(null != m_nameMap)");
			Global.Tracer.Assert(m_count < m_collection.Length, "(m_count < m_collection.Length)");
			m_nameMap.Add(name, m_count);
			m_collection[m_count] = parameter;
			m_count++;
		}
	}
}
