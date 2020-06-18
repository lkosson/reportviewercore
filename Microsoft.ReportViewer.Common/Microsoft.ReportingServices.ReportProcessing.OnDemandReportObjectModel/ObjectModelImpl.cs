using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal class ObjectModelImpl : OnDemandObjectModel, IConvertible, IStaticReferenceable
	{
		internal sealed class SecondaryFieldsCollectionWithAutomaticRestore : IDisposable
		{
			private ObjectModelImpl m_reportOM;

			private FieldsContext m_fieldsContext;

			internal SecondaryFieldsCollectionWithAutomaticRestore(ObjectModelImpl reportOM, FieldsContext fieldsContext)
			{
				m_reportOM = reportOM;
				m_fieldsContext = fieldsContext;
			}

			public void Dispose()
			{
				m_reportOM.RestoreFields(m_fieldsContext);
			}
		}

		private FieldsContext m_currentFields;

		private ParametersImpl m_parameters;

		private GlobalsImpl m_globals;

		private UserImpl m_user;

		private ReportItemsImpl m_reportItems;

		private AggregatesImpl m_aggregates;

		private LookupsImpl m_lookups;

		private DataSetsImpl m_dataSets;

		private DataSourcesImpl m_dataSources;

		private VariablesImpl m_variables;

		private OnDemandProcessingContext m_odpContext;

		private int m_id = int.MinValue;

		internal const string NamespacePrefix = "Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.";

		internal virtual bool UseDataSetFieldsCache
		{
			get
			{
				if (!m_odpContext.InSubreport)
				{
					return !m_odpContext.IsPageHeaderFooter;
				}
				return false;
			}
		}

		public override Fields Fields => FieldsImpl;

		public override Parameters Parameters => ParametersImpl;

		public override Globals Globals => GlobalsImpl;

		public override User User => UserImpl;

		public override ReportItems ReportItems => ReportItemsImpl;

		public override Aggregates Aggregates => AggregatesImpl;

		public override Lookups Lookups => LookupsImpl;

		public override DataSets DataSets => DataSetsImpl;

		public override DataSources DataSources => DataSourcesImpl;

		public override Variables Variables => VariablesImpl;

		internal FieldsContext CurrentFields => m_currentFields;

		internal FieldsImpl FieldsImpl => m_currentFields.Fields;

		internal ParametersImpl ParametersImpl
		{
			get
			{
				return m_parameters;
			}
			set
			{
				m_parameters = value;
			}
		}

		internal GlobalsImpl GlobalsImpl
		{
			get
			{
				return m_globals;
			}
			set
			{
				m_globals = value;
			}
		}

		internal UserImpl UserImpl
		{
			get
			{
				return m_user;
			}
			set
			{
				m_user = value;
			}
		}

		internal ReportItemsImpl ReportItemsImpl
		{
			get
			{
				return m_reportItems;
			}
			set
			{
				m_reportItems = value;
			}
		}

		internal AggregatesImpl AggregatesImpl
		{
			get
			{
				return m_aggregates;
			}
			set
			{
				m_aggregates = value;
			}
		}

		internal LookupsImpl LookupsImpl
		{
			get
			{
				return m_lookups;
			}
			set
			{
				m_lookups = value;
			}
		}

		internal DataSetsImpl DataSetsImpl
		{
			get
			{
				return m_dataSets;
			}
			set
			{
				m_dataSets = value;
			}
		}

		internal DataSourcesImpl DataSourcesImpl
		{
			get
			{
				return m_dataSources;
			}
			set
			{
				m_dataSources = value;
			}
		}

		internal VariablesImpl VariablesImpl
		{
			get
			{
				return m_variables;
			}
			set
			{
				m_variables = value;
			}
		}

		internal OnDemandProcessingContext OdpContext => m_odpContext;

		internal bool AllFieldsCleared => m_currentFields.AllFieldsCleared;

		int IStaticReferenceable.ID => m_id;

		internal ObjectModelImpl(OnDemandProcessingContext odpContext)
		{
			m_currentFields = null;
			m_parameters = null;
			m_globals = null;
			m_user = null;
			m_reportItems = null;
			m_aggregates = null;
			m_lookups = null;
			m_dataSets = null;
			m_dataSources = null;
			m_odpContext = odpContext;
		}

		internal ObjectModelImpl(ObjectModelImpl copy, OnDemandProcessingContext odpContext)
		{
			m_odpContext = odpContext;
			m_currentFields = new FieldsContext(this);
			m_parameters = copy.m_parameters;
			m_globals = new GlobalsImpl(odpContext);
			m_user = new UserImpl(copy.m_user, odpContext);
			m_dataSets = copy.m_dataSets;
			m_dataSources = copy.m_dataSources;
			m_reportItems = null;
			m_aggregates = null;
			m_lookups = null;
		}

		internal void Initialize(DataSetDefinition dataSetDefinition)
		{
			int size = 0;
			if (dataSetDefinition.DataSetCore != null && dataSetDefinition.DataSetCore.Query != null && dataSetDefinition.DataSetCore.Query.Parameters != null)
			{
				size = dataSetDefinition.DataSetCore.Query.Parameters.Count;
			}
			m_parameters = new ParametersImpl(size);
			InitializeGlobalAndUserCollections();
			m_currentFields = new FieldsContext(this, dataSetDefinition.DataSetCore);
			m_dataSources = new DataSourcesImpl(0);
			m_dataSets = new DataSetsImpl(0);
			m_variables = new VariablesImpl(lockAdd: false);
			m_aggregates = new AggregatesImpl(lockAdd: false, m_odpContext);
			m_reportItems = new ReportItemsImpl(lockAdd: false);
			m_lookups = new LookupsImpl();
		}

		internal void Initialize(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			int size = 0;
			if (report.Parameters != null)
			{
				size = report.Parameters.Count;
			}
			m_parameters = new ParametersImpl(size);
			InitializeGlobalAndUserCollections();
			m_currentFields = new FieldsContext(this);
			m_dataSources = new DataSourcesImpl(report.DataSourceCount);
			m_dataSets = new DataSetsImpl(report.DataSetCount);
			InitOrUpdateDataSetCollection(report, reportInstance, initialize: true);
			m_variables = new VariablesImpl(lockAdd: false);
			m_aggregates = new AggregatesImpl(lockAdd: false, m_odpContext);
			m_reportItems = new ReportItemsImpl(lockAdd: false);
			m_lookups = new LookupsImpl();
		}

		private void InitializeGlobalAndUserCollections()
		{
			m_globals = new GlobalsImpl(m_odpContext);
			m_user = new UserImpl(m_odpContext.RequestUserName, m_odpContext.UserLanguage.Name, m_odpContext.AllowUserProfileState, m_odpContext);
		}

		private int InitOrUpdateDataSetCollection(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, bool initialize)
		{
			int dataSetCount = 0;
			for (int i = 0; i < report.DataSourceCount; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource = report.DataSources[i];
				if (initialize && !dataSource.IsArtificialForSharedDataSets)
				{
					m_dataSources.Add(dataSource);
				}
				if (dataSource.DataSets != null)
				{
					for (int j = 0; j < dataSource.DataSets.Count; j++)
					{
						InitDataSet(reportInstance, dataSource.DataSets[j], ref dataSetCount);
					}
				}
			}
			return dataSetCount;
		}

		private void InitDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, ref int dataSetCount)
		{
			DataSetInstance dataSetInstance = null;
			if (reportInstance != null)
			{
				dataSetInstance = reportInstance.GetDataSetInstance(dataSet, m_odpContext);
			}
			m_dataSets.AddOrUpdate(dataSet, dataSetInstance, m_odpContext.ExecutionTime);
			if (!dataSet.UsedOnlyInParameters)
			{
				dataSetCount++;
			}
		}

		internal void Initialize(ParameterInfoCollection parameters)
		{
			m_parameters = new ParametersImpl(parameters.Count);
			if (parameters != null && parameters.Count > 0)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					m_parameters.Add(parameterInfo.Name, new ParameterImpl(parameterInfo));
				}
			}
		}

		internal void SetForNewSubReportContext(ParametersImpl parameters)
		{
			m_parameters = parameters;
			if (m_variables != null)
			{
				m_variables.ResetAll();
			}
			if (m_reportItems != null)
			{
				m_reportItems.ResetAll();
			}
			if (m_aggregates != null)
			{
				m_aggregates.ClearAll();
			}
			if (m_currentFields != null && m_currentFields.Fields != null)
			{
				ResetFieldValues();
			}
			InitOrUpdateDataSetCollection(m_odpContext.ReportDefinition, m_odpContext.CurrentReportInstance, initialize: false);
		}

		internal void SetupEmptyTopLevelFields()
		{
			m_currentFields = new FieldsContext(this);
		}

		internal void SetupPageSectionDataSetFields(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataset)
		{
			m_currentFields = new FieldsContext(this, dataset.DataSetCore, addRowIndex: false, noRows: true);
			m_currentFields.Fields.NeedsInlineSetup = true;
		}

		internal void SetupFieldsForNewDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataset, bool addRowIndex, bool noRows, bool forceNewFieldsContext)
		{
			m_currentFields.ResetFieldFlags();
			SetupFieldsForNewDataSetWithoutResettingOldFieldFlags(dataset, addRowIndex, noRows, forceNewFieldsContext);
		}

		private void SetupFieldsForNewDataSetWithoutResettingOldFieldFlags(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataset, bool addRowIndex, bool noRows, bool forceNewFieldsContext)
		{
			m_currentFields = (UseDataSetFieldsCache ? dataset.DataSetCore.FieldsContext : null);
			if (m_currentFields == null || !m_currentFields.Fields.IsCollectionInitialized || m_currentFields.Fields.NeedsInlineSetup || forceNewFieldsContext)
			{
				m_currentFields = new FieldsContext(this, dataset.DataSetCore, addRowIndex, noRows);
			}
		}

		internal void CreateNoRows()
		{
			m_currentFields.CreateNoRows();
		}

		internal void ResetFieldValues()
		{
			if (m_odpContext.IsTablixProcessingMode || m_odpContext.StreamingMode)
			{
				m_currentFields.CreateNoRows();
			}
			else
			{
				m_currentFields.CreateNullFieldValues();
			}
		}

		internal void PerformPendingFieldValueUpdate()
		{
			m_currentFields.PerformPendingFieldValueUpdate(this, UseDataSetFieldsCache);
		}

		internal void RegisterOnDemandFieldValueUpdate(long firstRowOffsetInScope, DataSetInstance dataSetInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader)
		{
			m_currentFields.RegisterOnDemandFieldValueUpdate(firstRowOffsetInScope, dataSetInstance, dataReader);
		}

		internal void UpdateFieldValues(long firstRowOffsetInScope)
		{
			m_currentFields.UpdateFieldValues(this, UseDataSetFieldsCache, firstRowOffsetInScope);
		}

		internal void UpdateFieldValues(bool reuseFieldObjects, Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row, DataSetInstance dataSetInstance, bool readerExtensionsSupported)
		{
			m_currentFields.UpdateFieldValues(this, UseDataSetFieldsCache, reuseFieldObjects, row, dataSetInstance, readerExtensionsSupported);
		}

		internal void ResetFieldsUsedInExpression()
		{
			FieldsImpl.ResetFieldsUsedInExpression();
			AggregatesImpl.ResetFieldsUsedInExpression();
		}

		internal void AddFieldsUsedInExpression(List<string> fieldsUsedInExpression)
		{
			FieldsImpl.AddFieldsUsedInExpression(fieldsUsedInExpression);
			AggregatesImpl.AddFieldsUsedInExpression(m_odpContext, fieldsUsedInExpression);
		}

		internal SecondaryFieldsCollectionWithAutomaticRestore SetupNewFieldsWithBackup(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataset, DataSetInstance dataSetInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader)
		{
			SecondaryFieldsCollectionWithAutomaticRestore result = new SecondaryFieldsCollectionWithAutomaticRestore(this, m_currentFields);
			bool addRowIndex = m_currentFields.Fields.Count != m_currentFields.Fields.CountWithRowIndex;
			SetupFieldsForNewDataSetWithoutResettingOldFieldFlags(dataset, addRowIndex, dataSetInstance.NoRows, forceNewFieldsContext: true);
			m_currentFields.UpdateDataSetInfo(dataSetInstance, dataChunkReader);
			return result;
		}

		internal void RestoreFields(FieldsContext fieldsContext)
		{
			m_currentFields = fieldsContext;
			m_currentFields.AttachToDataSetCache(this);
		}

		internal FieldsImpl GetFieldsImplForUpdate(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet currentDataSet)
		{
			if (currentDataSet.DataSetCore != m_currentFields.DataSet)
			{
				if (currentDataSet.DataSetCore.FieldsContext != null && UseDataSetFieldsCache)
				{
					m_currentFields = currentDataSet.DataSetCore.FieldsContext;
				}
				else
				{
					Global.Tracer.Assert(condition: false, "Fields collection is not setup correctly. Actual: " + m_currentFields.DataSet.Name.MarkAsPrivate() + " Expected: " + currentDataSet.DataSetCore.Name.MarkAsPrivate());
				}
			}
			return m_currentFields.Fields;
		}

		public override bool InScope(string scope)
		{
			return m_odpContext.InScope(scope);
		}

		public override int RecursiveLevel(string scope)
		{
			return m_odpContext.RecursiveLevel(scope);
		}

		public override object MinValue(params object[] arguments)
		{
			return m_odpContext.ReportRuntime.MinValue(arguments);
		}

		public override object MaxValue(params object[] arguments)
		{
			return m_odpContext.ReportRuntime.MaxValue(arguments);
		}

		void IStaticReferenceable.SetID(int id)
		{
			m_id = id;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ObjectModelImpl;
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(ObjectModel))
			{
				return this;
			}
			throw new NotSupportedException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}
	}
}
