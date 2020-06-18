using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class AggregatorFactory
	{
		private readonly DataAggregate[] m_prototypes;

		private static AggregatorFactory m_instance;

		public static AggregatorFactory Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new AggregatorFactory();
				}
				return m_instance;
			}
		}

		private AggregatorFactory()
		{
			int length = Enum.GetValues(typeof(DataAggregateInfo.AggregateTypes)).Length;
			m_prototypes = new DataAggregate[length];
			Add(new First());
			Add(new Last());
			Add(new Sum());
			Add(new Avg());
			Add(new Max());
			Add(new Min());
			Add(new CountDistinct());
			Add(new CountRows());
			Add(new Count());
			Add(new StDev());
			Add(new Var());
			Add(new StDevP());
			Add(new VarP());
			Add(new Aggregate());
			Add(new Previous());
			Add(new Union());
		}

		private void Add(DataAggregate aggregator)
		{
			int aggregateType = (int)aggregator.AggregateType;
			m_prototypes[aggregateType] = aggregator;
		}

		[Conditional("DEBUG")]
		private void VerifyAllPrototypesCreated()
		{
			for (int i = 0; i < m_prototypes.Length; i++)
			{
				Global.Tracer.Assert(m_prototypes[i] != null, "Missing aggregate prototype for: {0}", (DataAggregateInfo.AggregateTypes)i);
			}
		}

		public DataAggregate CreateAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return GetPrototype(aggregateDef).ConstructAggregator(odpContext, aggregateDef);
		}

		public object GetNoRowsResult(DataAggregateInfo aggregateDef)
		{
			return GetPrototype(aggregateDef).Result();
		}

		private DataAggregate GetPrototype(DataAggregateInfo aggregateDef)
		{
			return m_prototypes[(int)aggregateDef.AggregateType];
		}
	}
}
