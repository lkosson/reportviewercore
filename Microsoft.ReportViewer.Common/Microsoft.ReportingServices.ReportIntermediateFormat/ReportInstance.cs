using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class ReportInstance : ScopeInstance
	{
		private bool m_noRows;

		private string m_language;

		private object[] m_variables;

		[NonSerialized]
		private DataSetInstance[] m_dataSetInstances;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType => Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance;

		internal bool NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
			}
		}

		internal string Language
		{
			get
			{
				return m_language;
			}
			set
			{
				if (!base.IsReadOnly)
				{
					m_language = value;
				}
			}
		}

		internal object[] VariableValues => m_variables;

		internal ReportInstance(OnDemandProcessingContext odpContext, Report reportDef, ParameterInfoCollection parameters)
		{
			int count = reportDef.MappingNameToDataSet.Count;
			m_dataSetInstances = new DataSetInstance[count];
			List<DataRegion> topLevelDataRegions = reportDef.TopLevelDataRegions;
			if (topLevelDataRegions != null)
			{
				GroupTreeScalabilityCache groupTreeScalabilityCache = odpContext.OdpMetadata.GroupTreeScalabilityCache;
				int count2 = topLevelDataRegions.Count;
				m_dataRegionInstances = new List<IReference<DataRegionInstance>>(count2);
				for (int i = 0; i < count2; i++)
				{
					m_dataRegionInstances.Add(groupTreeScalabilityCache.AllocateEmptyTreePartition<DataRegionInstance>(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference));
				}
			}
		}

		internal ReportInstance()
		{
		}

		internal bool IsMissingExpectedDataChunk(OnDemandProcessingContext odpContext)
		{
			List<DataSet> mappingDataSetIndexToDataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet;
			for (int i = 0; i < mappingDataSetIndexToDataSet.Count; i++)
			{
				DataSet dataSet = mappingDataSetIndexToDataSet[i];
				if (!dataSet.UsedOnlyInParameters && GetDataSetInstance(dataSet, odpContext) == null)
				{
					return true;
				}
			}
			return false;
		}

		internal DataSetInstance GetDataSetInstance(DataSet dataSet, OnDemandProcessingContext odpContext)
		{
			if (m_dataSetInstances == null)
			{
				InitDataSetInstances(odpContext);
			}
			int indexInCollection = dataSet.IndexInCollection;
			if (m_dataSetInstances[indexInCollection] == null)
			{
				m_dataSetInstances[indexInCollection] = odpContext.GetDataSetInstance(dataSet);
			}
			return m_dataSetInstances[indexInCollection];
		}

		internal DataSetInstance GetDataSetInstance(int dataSetIndexInCollection, OnDemandProcessingContext odpContext)
		{
			if (m_dataSetInstances == null)
			{
				InitDataSetInstances(odpContext);
			}
			if (m_dataSetInstances[dataSetIndexInCollection] == null)
			{
				DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[dataSetIndexInCollection];
				m_dataSetInstances[dataSetIndexInCollection] = odpContext.GetDataSetInstance(dataSet);
			}
			return m_dataSetInstances[dataSetIndexInCollection];
		}

		internal void SetDataSetInstance(DataSetInstance dataSetInstance)
		{
			m_dataSetInstances[dataSetInstance.DataSetDef.IndexInCollection] = dataSetInstance;
		}

		private void InitDataSetInstances(OnDemandProcessingContext odpContext)
		{
			m_dataSetInstances = new DataSetInstance[odpContext.ReportDefinition.MappingDataSetIndexToDataSet.Count];
		}

		internal IEnumerator GetCachedDataSetInstances()
		{
			return m_dataSetInstances.GetEnumerator();
		}

		internal void InitializeFromSnapshot(OnDemandProcessingContext odpContext)
		{
			if (!odpContext.ReprocessSnapshot)
			{
				int num = 0;
				if (m_dataSetInstances == null && odpContext.ReportDefinition.MappingNameToDataSet != null)
				{
					num = odpContext.ReportDefinition.MappingNameToDataSet.Count;
				}
				m_dataSetInstances = new DataSetInstance[num];
			}
			Report reportDefinition = odpContext.ReportDefinition;
			m_noRows = (reportDefinition.DataSetsNotOnlyUsedInParameters > 0);
			List<DataSource> dataSources = reportDefinition.DataSources;
			for (int i = 0; i < dataSources.Count; i++)
			{
				List<DataSet> dataSets = dataSources[i].DataSets;
				if (dataSets == null)
				{
					continue;
				}
				for (int j = 0; j < dataSets.Count; j++)
				{
					DataSet dataSet = dataSets[j];
					if (dataSet != null)
					{
						DataSetInstance dataSetInstance = GetDataSetInstance(dataSet, odpContext);
						if (dataSetInstance != null && m_noRows && !dataSetInstance.NoRows)
						{
							m_noRows = false;
						}
					}
				}
			}
		}

		internal override void AddChildScope(IReference<ScopeInstance> child, int indexInCollection)
		{
			base.AddChildScope(child, indexInCollection);
		}

		internal IReference<DataRegionInstance> GetTopLevelDataRegionReference(int indexInCollection)
		{
			return m_dataRegionInstances[indexInCollection];
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext)
		{
			if (m_dataSetInstances == null)
			{
				InitDataSetInstances(odpContext);
			}
			for (int i = 0; i < m_dataSetInstances.Length; i++)
			{
				GetDataSetInstance(i, odpContext)?.SetupDataSetLevelAggregates(odpContext);
			}
			if (m_variables != null)
			{
				ScopeInstance.SetupVariables(odpContext, odpContext.ReportDefinition.Variables, m_variables);
			}
		}

		internal void CalculateAndStoreReportVariables(OnDemandProcessingContext odpContext)
		{
			if (odpContext.ReportDefinition.Variables != null && m_variables == null)
			{
				ScopeInstance.CalculateVariables(odpContext, odpContext.ReportDefinition.Variables, out m_variables);
			}
		}

		internal void ResetReportVariables(OnDemandProcessingContext odpContext)
		{
			if (odpContext.ReportDefinition.Variables != null)
			{
				ScopeInstance.ResetVariables(odpContext, odpContext.ReportDefinition.Variables);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.NoRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Language, Token.String));
			list.Add(new ReadOnlyMemberInfo(MemberName.Variables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.SerializableVariables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SerializableArray, Token.Serializable));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		internal static IReference<ReportInstance> CreateInstance(IReportInstanceContainer reportInstanceContainer, OnDemandProcessingContext odpContext, Report reportDef, ParameterInfoCollection parameters)
		{
			ReportInstance reportInstance = new ReportInstance(odpContext, reportDef, parameters);
			IReference<ReportInstance> reference = reportInstanceContainer.SetReportInstance(reportInstance, odpContext.OdpMetadata);
			reportInstance.m_cleanupRef = (IDisposable)reference;
			return reference;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NoRows:
					writer.Write(m_noRows);
					break;
				case MemberName.Language:
					writer.Write(m_language);
					break;
				case MemberName.SerializableVariables:
					writer.WriteSerializableArray(m_variables);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NoRows:
					m_noRows = reader.ReadBoolean();
					break;
				case MemberName.Language:
					m_language = reader.ReadString();
					break;
				case MemberName.Variables:
					m_variables = reader.ReadVariantArray();
					break;
				case MemberName.SerializableVariables:
					m_variables = reader.ReadSerializableArray();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance;
		}
	}
}
