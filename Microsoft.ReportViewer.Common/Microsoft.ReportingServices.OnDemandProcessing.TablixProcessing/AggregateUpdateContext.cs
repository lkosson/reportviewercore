using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class AggregateUpdateContext : ITraversalContext
	{
		private AggregateMode m_mode;

		private OnDemandProcessingContext m_odpContext;

		private AggregateUpdateCollection m_activeAggregates;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_aggsForUpdateAtRowScope;

		private List<string> m_runningValuesForUpdateAtRow;

		public AggregateMode Mode => m_mode;

		public AggregateUpdateContext(OnDemandProcessingContext odpContext, AggregateMode mode)
		{
			m_mode = mode;
			m_odpContext = odpContext;
			m_activeAggregates = null;
		}

		public AggregateUpdateQueue ReplaceAggregatesToUpdate(BucketedDataAggregateObjs aggBuckets)
		{
			return HandleNewBuckets(aggBuckets, canMergeActiveAggs: false);
		}

		public AggregateUpdateQueue RegisterAggregatesToUpdate(BucketedDataAggregateObjs aggBuckets)
		{
			return HandleNewBuckets(aggBuckets, canMergeActiveAggs: true);
		}

		public AggregateUpdateQueue RegisterRunningValuesToUpdate(AggregateUpdateQueue workQueue, List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			if (runningValues == null || runningValues.Count == 0)
			{
				return workQueue;
			}
			if (workQueue == null)
			{
				workQueue = new AggregateUpdateQueue(m_activeAggregates);
				AggregateUpdateCollection aggregateUpdateCollection = new AggregateUpdateCollection(runningValues);
				aggregateUpdateCollection.LinkedCollection = m_activeAggregates;
				m_activeAggregates = aggregateUpdateCollection;
			}
			else
			{
				m_activeAggregates.MergeRunningValues(runningValues);
			}
			return workQueue;
		}

		private AggregateUpdateQueue HandleNewBuckets(BucketedDataAggregateObjs aggBuckets, bool canMergeActiveAggs)
		{
			bool flag = aggBuckets == null || aggBuckets.Buckets.Count == 0;
			if (canMergeActiveAggs && flag)
			{
				return null;
			}
			AggregateUpdateQueue aggregateUpdateQueue = new AggregateUpdateQueue(m_activeAggregates);
			AggregateUpdateCollection aggregateUpdateCollection = null;
			if (canMergeActiveAggs)
			{
				aggregateUpdateCollection = m_activeAggregates;
			}
			m_activeAggregates = null;
			if (flag)
			{
				return aggregateUpdateQueue;
			}
			for (int i = 0; i < aggBuckets.Buckets.Count; i++)
			{
				AggregateBucket<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregateBucket = aggBuckets.Buckets[i];
				AggregateUpdateCollection aggregateUpdateCollection2 = new AggregateUpdateCollection(aggregateBucket);
				if (aggregateUpdateCollection != null)
				{
					if (aggregateUpdateCollection.Level == aggregateBucket.Level)
					{
						aggregateUpdateCollection2.LinkedCollection = aggregateUpdateCollection;
						aggregateUpdateCollection = null;
					}
					else if (aggregateUpdateCollection.Level < aggregateBucket.Level)
					{
						aggregateUpdateCollection2 = aggregateUpdateCollection;
						i--;
						aggregateUpdateCollection = null;
					}
				}
				if (m_activeAggregates == null)
				{
					m_activeAggregates = aggregateUpdateCollection2;
				}
				else
				{
					aggregateUpdateQueue.Enqueue(aggregateUpdateCollection2);
				}
			}
			if (aggregateUpdateCollection != null)
			{
				aggregateUpdateQueue.Enqueue(aggregateUpdateCollection);
			}
			return aggregateUpdateQueue;
		}

		public bool AdvanceQueue(AggregateUpdateQueue queue)
		{
			if (queue == null)
			{
				return false;
			}
			if (queue.Count == 0)
			{
				RestoreOriginalState(queue);
				return false;
			}
			m_activeAggregates = queue.Dequeue();
			return true;
		}

		public void RestoreOriginalState(AggregateUpdateQueue queue)
		{
			if (queue != null)
			{
				m_activeAggregates = queue.OriginalState;
			}
		}

		public bool UpdateAggregates(DataScopeInfo scopeInfo, IDataRowHolder scopeInst, AggregateUpdateFlags updateFlags, bool needsSetupEnvironment)
		{
			m_aggsForUpdateAtRowScope = null;
			m_runningValuesForUpdateAtRow = null;
			if (m_activeAggregates == null)
			{
				return false;
			}
			for (AggregateUpdateCollection aggregateUpdateCollection = m_activeAggregates; aggregateUpdateCollection != null; aggregateUpdateCollection = aggregateUpdateCollection.LinkedCollection)
			{
				if (aggregateUpdateCollection.GetAggregatesForScope(scopeInfo.ScopeID, out List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggs))
				{
					if (needsSetupEnvironment)
					{
						scopeInst.SetupEnvironment();
						needsSetupEnvironment = false;
					}
					foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj item in aggs)
					{
						item.Update();
					}
				}
				if (aggregateUpdateCollection.GetAggregatesForRowScope(scopeInfo.ScopeID, out aggs))
				{
					if (m_aggsForUpdateAtRowScope == null)
					{
						m_aggsForUpdateAtRowScope = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>();
					}
					m_aggsForUpdateAtRowScope.AddRange(aggs);
				}
				if (aggregateUpdateCollection.GetRunningValuesForScope(scopeInfo.ScopeID, out List<string> aggs2))
				{
					if (needsSetupEnvironment)
					{
						scopeInst.SetupEnvironment();
						needsSetupEnvironment = false;
					}
					RuntimeDataTablixObj.UpdateRunningValues(m_odpContext, aggs2);
				}
				if (aggregateUpdateCollection.GetRunningValuesForRowScope(scopeInfo.ScopeID, out aggs2))
				{
					if (m_runningValuesForUpdateAtRow == null)
					{
						m_runningValuesForUpdateAtRow = new List<string>();
					}
					m_runningValuesForUpdateAtRow.AddRange(aggs2);
				}
			}
			if (m_aggsForUpdateAtRowScope != null || m_runningValuesForUpdateAtRow != null)
			{
				if (needsSetupEnvironment)
				{
					scopeInst.SetupEnvironment();
				}
				if (FlagUtils.HasFlag(updateFlags, AggregateUpdateFlags.RowAggregates))
				{
					scopeInst.ReadRows(DataActions.AggregatesOfAggregates, this);
				}
			}
			return scopeInfo.ScopeID != m_activeAggregates.InnermostUpdateScopeID;
		}

		public void UpdateAggregatesForRow()
		{
			Global.Tracer.Assert(m_aggsForUpdateAtRowScope != null || m_runningValuesForUpdateAtRow != null, "UpdateAggregatesForRow must be driven by a call to UpdateAggregates.");
			if (m_aggsForUpdateAtRowScope != null)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj item in m_aggsForUpdateAtRowScope)
				{
					item.Update();
				}
			}
			if (m_runningValuesForUpdateAtRow != null)
			{
				RuntimeDataTablixObj.UpdateRunningValues(m_odpContext, m_runningValuesForUpdateAtRow);
			}
		}

		public bool LastScopeNeedsRowAggregateProcessing()
		{
			if (m_aggsForUpdateAtRowScope == null)
			{
				return m_runningValuesForUpdateAtRow != null;
			}
			return true;
		}
	}
}
