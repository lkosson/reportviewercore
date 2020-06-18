using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class VariablesImpl : Variables
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override Variable this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingVariableReference(key);
				}
				Variable variable = m_collection[key] as Variable;
				if (variable == null)
				{
					throw new ReportProcessingException_NonExistingVariableReference(key);
				}
				return variable;
			}
		}

		internal Hashtable Collection
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

		internal VariablesImpl(bool lockAdd)
		{
			m_lockAdd = lockAdd;
			m_collection = new Hashtable();
		}

		internal void Add(VariableImpl variable)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				m_collection.Add(variable.Name, variable);
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}

		internal void ResetAll()
		{
			foreach (VariableImpl value in m_collection.Values)
			{
				value.Reset();
			}
		}
	}
}
