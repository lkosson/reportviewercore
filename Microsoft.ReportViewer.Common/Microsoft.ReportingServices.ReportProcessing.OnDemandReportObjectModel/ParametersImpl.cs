using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ParametersImpl : Parameters
	{
		private Hashtable m_nameMap;

		private ParameterImpl[] m_collection;

		private int m_count;

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
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException_NonExistingParameterReference(key);
				}
			}
		}

		internal ParameterImpl[] Collection
		{
			get
			{
				return m_collection;
			}
			set
			{
				m_collection = value;
			}
		}

		internal Hashtable NameMap
		{
			get
			{
				return m_nameMap;
			}
			set
			{
				m_nameMap = value;
			}
		}

		internal int Count
		{
			get
			{
				return m_count;
			}
			set
			{
				m_count = value;
			}
		}

		internal ParametersImpl()
		{
		}

		internal ParametersImpl(int size)
		{
			m_collection = new ParameterImpl[size];
			m_nameMap = new Hashtable(size);
			m_count = 0;
		}

		internal ParametersImpl(ParametersImpl copy)
		{
			m_count = copy.m_count;
			if (copy.m_collection != null)
			{
				m_collection = new ParameterImpl[m_count];
				Array.Copy(copy.m_collection, m_collection, m_count);
			}
			if (copy.m_nameMap != null)
			{
				m_nameMap = (Hashtable)copy.m_nameMap.Clone();
			}
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

		internal void Clear()
		{
			if (m_nameMap != null)
			{
				m_nameMap.Clear();
			}
			if (m_collection != null)
			{
				m_collection = new ParameterImpl[m_collection.Length];
			}
			m_count = 0;
		}
	}
}
