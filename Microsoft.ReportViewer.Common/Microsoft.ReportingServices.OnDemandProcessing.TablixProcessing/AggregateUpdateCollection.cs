using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class AggregateUpdateCollection
	{
		private class AggregatesByScopeId : Dictionary<int, List<DataAggregateObj>>
		{
			public AggregatesByScopeId()
				: base(5)
			{
			}
		}

		private class RunningValuesByScopeId : Dictionary<int, List<string>>
		{
			public RunningValuesByScopeId()
				: base(5)
			{
			}
		}

		private int m_level;

		private int m_innermostUpdateScopeID;

		private int m_innermostUpdateScopeDepth;

		private AggregateUpdateCollection m_linkedCol;

		private AggregatesByScopeId m_aggsByUpdateScope;

		private AggregatesByScopeId m_rowAggsByUpdateScope;

		private RunningValuesByScopeId m_runningValuesByUpdateScope;

		private RunningValuesByScopeId m_rowRunningValuesByUpdateScope;

		public int Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		public int InnermostUpdateScopeID => m_innermostUpdateScopeID;

		public int InnermostUpdateScopeDepth => m_innermostUpdateScopeDepth;

		public AggregateUpdateCollection LinkedCollection
		{
			get
			{
				return m_linkedCol;
			}
			set
			{
				m_linkedCol = value;
				if (m_linkedCol != null && m_linkedCol.m_innermostUpdateScopeDepth > m_innermostUpdateScopeDepth)
				{
					m_innermostUpdateScopeDepth = m_linkedCol.m_innermostUpdateScopeDepth;
					m_innermostUpdateScopeID = m_linkedCol.m_innermostUpdateScopeID;
				}
			}
		}

		public AggregateUpdateCollection(AggregateBucket<DataAggregateObj> bucket)
		{
			m_level = bucket.Level;
			m_innermostUpdateScopeID = -1;
			m_innermostUpdateScopeDepth = -1;
			foreach (DataAggregateObj aggregate in bucket.Aggregates)
			{
				DataAggregateInfo aggregateDef = aggregate.AggregateDef;
				if (aggregateDef.UpdatesAtRowScope)
				{
					Add(ref m_rowAggsByUpdateScope, aggregateDef, aggregate);
				}
				else
				{
					Add(ref m_aggsByUpdateScope, aggregateDef, aggregate);
				}
			}
		}

		public AggregateUpdateCollection(List<RunningValueInfo> runningValues)
		{
			m_level = 0;
			MergeRunningValues(runningValues);
		}

		public void MergeRunningValues(List<RunningValueInfo> runningValues)
		{
			foreach (RunningValueInfo runningValue in runningValues)
			{
				if (runningValue.UpdatesAtRowScope)
				{
					Add(ref m_rowRunningValuesByUpdateScope, runningValue, runningValue.Name);
				}
				else
				{
					Add(ref m_runningValuesByUpdateScope, runningValue, runningValue.Name);
				}
			}
		}

		private void Add<T, U>(ref T colByScope, DataAggregateInfo agg, U item) where T : Dictionary<int, List<U>>, new()
		{
			if (colByScope == null)
			{
				colByScope = new T();
			}
			int updateScopeID = agg.UpdateScopeID;
			if (!colByScope.TryGetValue(updateScopeID, out List<U> value))
			{
				value = new List<U>();
				colByScope.Add(updateScopeID, value);
			}
			value.Add(item);
			if (agg.UpdateScopeDepth > m_innermostUpdateScopeDepth)
			{
				m_innermostUpdateScopeID = agg.UpdateScopeID;
				m_innermostUpdateScopeDepth = agg.UpdateScopeDepth;
			}
		}

		public bool GetAggregatesForScope(int scopeId, out List<DataAggregateObj> aggs)
		{
			return NullCheckTryGetValue(m_aggsByUpdateScope, scopeId, out aggs);
		}

		public bool GetAggregatesForRowScope(int scopeId, out List<DataAggregateObj> aggs)
		{
			return NullCheckTryGetValue(m_rowAggsByUpdateScope, scopeId, out aggs);
		}

		public bool GetRunningValuesForScope(int scopeId, out List<string> aggs)
		{
			return NullCheckTryGetValue(m_runningValuesByUpdateScope, scopeId, out aggs);
		}

		public bool GetRunningValuesForRowScope(int scopeId, out List<string> aggs)
		{
			return NullCheckTryGetValue(m_rowRunningValuesByUpdateScope, scopeId, out aggs);
		}

		private bool NullCheckTryGetValue<T>(Dictionary<int, List<T>> aggsById, int scopeId, out List<T> aggs)
		{
			if (aggsById == null)
			{
				aggs = null;
				return false;
			}
			return aggsById.TryGetValue(scopeId, out aggs);
		}
	}
}
