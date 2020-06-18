using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Report : ReportItem, IPersistable, IRIFReportScope, IInstancePath, IGloballyReferenceable, IGlobalIDOwner, IExpressionHostAssemblyHolder
	{
		internal enum ShowHideTypes
		{
			None,
			Static,
			Interactive
		}

		private bool m_consumeContainerWhitespace;

		private Guid m_reportVersion = Guid.Empty;

		private string m_author;

		private int m_autoRefresh = -1;

		private Dictionary<string, ImageInfo> m_embeddedImages;

		private List<DataSource> m_dataSources;

		private List<Variable> m_variables;

		private bool m_deferVariableEvaluation;

		private byte[] m_exprCompiledCode;

		private bool m_exprCompiledCodeGeneratedWithRefusedPermissions;

		private bool m_mergeOnePass;

		private bool m_subReportMergeTransactions;

		private bool m_needPostGroupProcessing;

		private bool m_hasPostSortAggregates;

		private bool m_hasAggregatesOfAggregates;

		private bool m_hasAggregatesOfAggregatesInUserSort;

		private bool m_hasReportItemReferences;

		private ShowHideTypes m_showHideType;

		private int m_lastID;

		[Reference]
		private List<SubReport> m_subReports;

		private bool m_hasImageStreams;

		private bool m_hasLabels;

		private bool m_hasBookmarks;

		private bool m_parametersNotUsedInQuery;

		private List<ParameterDef> m_parameters;

		private string m_oneDataSetName;

		private List<string> m_codeModules;

		private List<CodeClass> m_codeClasses;

		private bool m_hasSpecialRecursiveAggregates;

		private ExpressionInfo m_language;

		private string m_dataTransform;

		private string m_dataSchema;

		private bool m_dataElementStyleAttribute = true;

		private string m_code;

		private bool m_hasUserSortFilter;

		private bool m_hasHeadersOrFooters;

		private bool m_hasPreviousAggregates;

		private InScopeSortFilterHashtable m_nonDetailSortFiltersInScope;

		private InScopeSortFilterHashtable m_detailSortFiltersInScope;

		private List<DataRegion> m_topLevelDataRegions;

		[Reference]
		private DataSet m_firstDataSet;

		[Reference]
		private DataRegion m_topLeftDataRegion;

		private int m_dataSetsNotOnlyUsedInParameters;

		private List<IInScopeEventSource> m_inScopeEventSources;

		private List<IInScopeEventSource> m_eventSources;

		private List<ReportHierarchyNode> m_groupsWithVariables;

		private byte[] m_flattenedDatasetDependencyMatrix;

		private int m_firstDataSetIndexToProcess = -1;

		private byte[] m_variablesInScope;

		private bool m_hasLookups;

		private List<ReportSection> m_reportSections;

		private ExpressionInfo m_autoRefreshExpression;

		private ExpressionInfo m_initialPageName;

		private int m_sharedDSContainerCollectionIndex = -1;

		private int m_dataPipelineCount;

		private string m_defaultFontFamily;

		[NonSerialized]
		private DataSource m_sharedDSContainer;

		[NonSerialized]
		private int m_lastAggregateID = -1;

		[NonSerialized]
		private int m_lastLookupID = -1;

		[NonSerialized]
		private double m_topLeftDataRegionAbsTop;

		[NonSerialized]
		private double m_topLeftDataRegionAbsLeft;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ReportExprHost m_exprHost;

		[NonSerialized]
		private Dictionary<string, DataSet> m_mappingNameToDataSet;

		[NonSerialized]
		private List<int> m_mappingDataSetIndexToDataSourceIndex;

		[NonSerialized]
		private List<DataSet> m_mappingDataSetIndexToDataSet;

		[NonSerialized]
		private bool m_reportOrDescendentHasUserSortFilter;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Report;

		internal override string DataElementNameDefault => "Report";

		internal bool ConsumeContainerWhitespace
		{
			get
			{
				return m_consumeContainerWhitespace;
			}
			set
			{
				m_consumeContainerWhitespace = value;
			}
		}

		internal string Author
		{
			get
			{
				return m_author;
			}
			set
			{
				m_author = value;
			}
		}

		internal string DefaultFontFamily
		{
			get
			{
				return m_defaultFontFamily;
			}
			set
			{
				m_defaultFontFamily = value;
			}
		}

		internal ExpressionInfo AutoRefreshExpression
		{
			get
			{
				return m_autoRefreshExpression;
			}
			set
			{
				m_autoRefreshExpression = value;
			}
		}

		internal Dictionary<string, ImageInfo> EmbeddedImages
		{
			get
			{
				return m_embeddedImages;
			}
			set
			{
				m_embeddedImages = value;
			}
		}

		internal List<DataSource> DataSources
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

		internal int DataSourceCount
		{
			get
			{
				if (m_dataSources != null)
				{
					return m_dataSources.Count;
				}
				return 0;
			}
		}

		internal int DataSetCount
		{
			get
			{
				if (MappingNameToDataSet != null)
				{
					return MappingNameToDataSet.Count;
				}
				return 0;
			}
		}

		internal int DataPipelineCount
		{
			get
			{
				return m_dataPipelineCount;
			}
			set
			{
				m_dataPipelineCount = value;
			}
		}

		internal DataSource SharedDSContainer
		{
			get
			{
				if (m_sharedDSContainer == null && m_sharedDSContainerCollectionIndex >= 0 && m_dataSources != null)
				{
					m_sharedDSContainer = m_dataSources[m_sharedDSContainerCollectionIndex];
				}
				return m_sharedDSContainer;
			}
			set
			{
				m_sharedDSContainer = value;
			}
		}

		internal int SharedDSContainerCollectionIndex
		{
			get
			{
				return m_sharedDSContainerCollectionIndex;
			}
			set
			{
				m_sharedDSContainerCollectionIndex = value;
			}
		}

		internal bool HasSharedDataSetReferences => -1 != m_sharedDSContainerCollectionIndex;

		internal bool MergeOnePass
		{
			get
			{
				return m_mergeOnePass;
			}
			set
			{
				m_mergeOnePass = value;
			}
		}

		internal bool SubReportMergeTransactions
		{
			get
			{
				return m_subReportMergeTransactions;
			}
			set
			{
				m_subReportMergeTransactions = value;
			}
		}

		internal bool NeedPostGroupProcessing
		{
			get
			{
				if (!m_needPostGroupProcessing)
				{
					return HasVariables;
				}
				return true;
			}
			set
			{
				m_needPostGroupProcessing = value;
			}
		}

		internal bool HasPostSortAggregates
		{
			get
			{
				return m_hasPostSortAggregates;
			}
			set
			{
				m_hasPostSortAggregates = value;
			}
		}

		internal bool HasAggregatesOfAggregates
		{
			get
			{
				return m_hasAggregatesOfAggregates;
			}
			set
			{
				m_hasAggregatesOfAggregates = value;
			}
		}

		internal bool HasAggregatesOfAggregatesInUserSort
		{
			get
			{
				return m_hasAggregatesOfAggregatesInUserSort;
			}
			set
			{
				m_hasAggregatesOfAggregatesInUserSort = value;
			}
		}

		internal bool HasReportItemReferences
		{
			get
			{
				return m_hasReportItemReferences;
			}
			set
			{
				m_hasReportItemReferences = value;
			}
		}

		internal int DataSetsNotOnlyUsedInParameters
		{
			get
			{
				return m_dataSetsNotOnlyUsedInParameters;
			}
			set
			{
				m_dataSetsNotOnlyUsedInParameters = value;
			}
		}

		internal ShowHideTypes ShowHideType
		{
			get
			{
				return m_showHideType;
			}
			set
			{
				m_showHideType = value;
			}
		}

		internal bool ParametersNotUsedInQuery
		{
			get
			{
				return m_parametersNotUsedInQuery;
			}
			set
			{
				m_parametersNotUsedInQuery = value;
			}
		}

		internal int LastID
		{
			get
			{
				return m_lastID;
			}
			set
			{
				m_lastID = value;
			}
		}

		internal List<SubReport> SubReports
		{
			get
			{
				return m_subReports;
			}
			set
			{
				m_subReports = value;
			}
		}

		internal bool HasImageStreams
		{
			get
			{
				return m_hasImageStreams;
			}
			set
			{
				m_hasImageStreams = value;
			}
		}

		internal bool HasLabels
		{
			get
			{
				return m_hasLabels;
			}
			set
			{
				m_hasLabels = value;
			}
		}

		internal bool HasBookmarks
		{
			get
			{
				return m_hasBookmarks;
			}
			set
			{
				m_hasBookmarks = value;
			}
		}

		internal bool HasHeadersOrFooters
		{
			get
			{
				return m_hasHeadersOrFooters;
			}
			set
			{
				m_hasHeadersOrFooters = value;
			}
		}

		internal List<ParameterDef> Parameters
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

		internal string OneDataSetName
		{
			get
			{
				return m_oneDataSetName;
			}
			set
			{
				m_oneDataSetName = value;
			}
		}

		internal bool HasSpecialRecursiveAggregates
		{
			get
			{
				return m_hasSpecialRecursiveAggregates;
			}
			set
			{
				m_hasSpecialRecursiveAggregates = value;
			}
		}

		internal bool HasPreviousAggregates
		{
			get
			{
				return m_hasPreviousAggregates;
			}
			set
			{
				m_hasPreviousAggregates = value;
			}
		}

		internal bool HasVariables
		{
			get
			{
				if (m_variables == null)
				{
					return m_groupsWithVariables != null;
				}
				return true;
			}
		}

		internal bool HasLookups
		{
			get
			{
				return m_hasLookups;
			}
			set
			{
				m_hasLookups = value;
			}
		}

		internal ExpressionInfo Language
		{
			get
			{
				return m_language;
			}
			set
			{
				m_language = value;
			}
		}

		internal ReportExprHost ReportExprHost => m_exprHost;

		internal string DataTransform
		{
			get
			{
				return m_dataTransform;
			}
			set
			{
				m_dataTransform = value;
			}
		}

		internal string DataSchema
		{
			get
			{
				return m_dataSchema;
			}
			set
			{
				m_dataSchema = value;
			}
		}

		internal bool DataElementStyleAttribute
		{
			get
			{
				return m_dataElementStyleAttribute;
			}
			set
			{
				m_dataElementStyleAttribute = value;
			}
		}

		internal string Code
		{
			get
			{
				return m_code;
			}
			set
			{
				m_code = value;
			}
		}

		internal bool HasUserSortFilter
		{
			get
			{
				return m_hasUserSortFilter;
			}
			set
			{
				m_hasUserSortFilter = value;
			}
		}

		internal bool ReportOrDescendentHasUserSortFilter
		{
			get
			{
				return m_reportOrDescendentHasUserSortFilter;
			}
			set
			{
				m_reportOrDescendentHasUserSortFilter = value;
			}
		}

		internal InScopeSortFilterHashtable NonDetailSortFiltersInScope
		{
			get
			{
				return m_nonDetailSortFiltersInScope;
			}
			set
			{
				m_nonDetailSortFiltersInScope = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return m_detailSortFiltersInScope;
			}
			set
			{
				m_detailSortFiltersInScope = value;
			}
		}

		internal int LastAggregateID
		{
			get
			{
				return m_lastAggregateID;
			}
			set
			{
				m_lastAggregateID = value;
			}
		}

		internal int LastLookupID
		{
			get
			{
				return m_lastLookupID;
			}
			set
			{
				m_lastLookupID = value;
			}
		}

		internal List<Variable> Variables
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

		internal bool DeferVariableEvaluation
		{
			get
			{
				return m_deferVariableEvaluation;
			}
			set
			{
				m_deferVariableEvaluation = value;
			}
		}

		internal bool HasSubReports
		{
			get
			{
				if (m_subReports != null)
				{
					return m_subReports.Count > 0;
				}
				return false;
			}
		}

		internal Dictionary<string, DataSet> MappingNameToDataSet
		{
			get
			{
				if (m_mappingNameToDataSet == null)
				{
					GenerateDataSetMappings();
				}
				return m_mappingNameToDataSet;
			}
		}

		internal List<int> MappingDataSetIndexToDataSourceIndex
		{
			get
			{
				if (m_mappingDataSetIndexToDataSourceIndex == null)
				{
					GenerateDataSetMappings();
				}
				return m_mappingDataSetIndexToDataSourceIndex;
			}
		}

		internal List<DataSet> MappingDataSetIndexToDataSet
		{
			get
			{
				if (m_mappingDataSetIndexToDataSet == null)
				{
					GenerateDataSetMappings();
				}
				return m_mappingDataSetIndexToDataSet;
			}
		}

		internal List<DataRegion> TopLevelDataRegions
		{
			get
			{
				return m_topLevelDataRegions;
			}
			set
			{
				m_topLevelDataRegions = value;
			}
		}

		internal DataSet FirstDataSet
		{
			get
			{
				return m_firstDataSet;
			}
			set
			{
				m_firstDataSet = value;
			}
		}

		internal int FirstDataSetIndexToProcess => m_firstDataSetIndexToProcess;

		internal List<IInScopeEventSource> InScopeEventSources => m_inScopeEventSources;

		internal List<IInScopeEventSource> EventSources => m_eventSources;

		internal List<ReportHierarchyNode> GroupsWithVariables => m_groupsWithVariables;

		internal List<ReportSection> ReportSections
		{
			get
			{
				return m_reportSections;
			}
			set
			{
				m_reportSections = value;
			}
		}

		internal ExpressionInfo InitialPageName
		{
			get
			{
				return m_initialPageName;
			}
			set
			{
				m_initialPageName = value;
			}
		}

		bool IRIFReportScope.NeedToCacheDataRows
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IExpressionHostAssemblyHolder.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Report;

		string IExpressionHostAssemblyHolder.ExprHostAssemblyName => "expression_host_" + m_reportVersion.ToString().Replace("-", "");

		List<string> IExpressionHostAssemblyHolder.CodeModules
		{
			get
			{
				return m_codeModules;
			}
			set
			{
				m_codeModules = value;
			}
		}

		List<CodeClass> IExpressionHostAssemblyHolder.CodeClasses
		{
			get
			{
				return m_codeClasses;
			}
			set
			{
				m_codeClasses = value;
			}
		}

		byte[] IExpressionHostAssemblyHolder.CompiledCode
		{
			get
			{
				return m_exprCompiledCode;
			}
			set
			{
				m_exprCompiledCode = value;
			}
		}

		bool IExpressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions
		{
			get
			{
				return m_exprCompiledCodeGeneratedWithRefusedPermissions;
			}
			set
			{
				m_exprCompiledCodeGeneratedWithRefusedPermissions = value;
			}
		}

		internal Report()
			: base(null)
		{
		}

		internal Report(int id, int idForReportItems)
			: base(id, null)
		{
			m_reportVersion = Guid.NewGuid();
			m_height = "11in";
			m_width = "8.5in";
			m_dataSources = new List<DataSource>();
			m_exprCompiledCode = new byte[0];
		}

		internal Report(ReportItem parent)
			: base(parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location = Microsoft.ReportingServices.ReportPublishing.LocationFlags.None;
			context.ObjectType = ObjectType;
			context.ObjectName = null;
			if (m_variables != null && m_variables.Count != 0)
			{
				context.RegisterVariables(m_variables);
				context.ExprHostBuilder.VariableValuesStart();
				for (int i = 0; i < m_variables.Count; i++)
				{
					Variable variable = m_variables[i];
					variable.Initialize(context);
					context.ExprHostBuilder.VariableValueExpression(variable.Value);
				}
				context.ExprHostBuilder.VariableValuesEnd();
			}
			AllocateDatasetDependencyMatrix();
			base.Initialize(context);
			if (m_language != null)
			{
				m_language.Initialize("Language", context);
				context.ExprHostBuilder.ReportLanguage(m_language);
			}
			if (m_autoRefreshExpression != null)
			{
				m_autoRefreshExpression.Initialize("AutoRefresh", context);
				context.ExprHostBuilder.ReportAutoRefresh(m_autoRefreshExpression);
			}
			context.ReportDataElementStyleAttribute = m_dataElementStyleAttribute;
			if (m_dataSources != null)
			{
				for (int j = 0; j < m_dataSources.Count; j++)
				{
					Global.Tracer.Assert(m_dataSources[j] != null, "(null != m_dataSources[i])");
					m_dataSources[j].Initialize(context);
				}
			}
			m_variablesInScope = context.GetCurrentReferencableVariables();
			if (m_reportSections != null)
			{
				for (int k = 0; k < m_reportSections.Count; k++)
				{
					m_reportSections[k].Initialize(context);
				}
			}
			if (context.ExprHostBuilder.CustomCode)
			{
				context.ExprHostBuilder.CustomCodeProxyStart();
				if (m_codeClasses != null && m_codeClasses.Count > 0)
				{
					for (int num = m_codeClasses.Count - 1; num >= 0; num--)
					{
						CodeClass codeClass = m_codeClasses[num];
						context.EnforceRdlSandboxContentRestrictions(codeClass);
						context.ExprHostBuilder.CustomCodeClassInstance(codeClass.ClassName, codeClass.InstanceName, num);
					}
				}
				if (m_code != null && m_code.Length > 0)
				{
					context.ExprHostBuilder.ReportCode(m_code);
				}
				context.ExprHostBuilder.CustomCodeProxyEnd();
			}
			if (m_initialPageName != null)
			{
				m_initialPageName.Initialize("InitialPageName", context);
				context.ExprHostBuilder.ReportInitialPageName(m_initialPageName);
			}
			if (m_variables != null)
			{
				foreach (Variable variable2 in m_variables)
				{
					context.UnregisterVariable(variable2);
				}
			}
			if (m_dataSources != null)
			{
				foreach (DataSource dataSource in m_dataSources)
				{
					dataSource.DetermineDecomposability(context);
				}
			}
			return false;
		}

		internal void BindAndValidateDataSetDefaultRelationships(ErrorContext errorContext)
		{
			foreach (DataSet item in MappingDataSetIndexToDataSet)
			{
				item?.BindAndValidateDefaultRelationships(errorContext, this);
			}
		}

		internal ScopeTree BuildScopeTree()
		{
			return ScopeTreeBuilder.BuildScopeTree(this);
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (m_reportSections == null)
			{
				return;
			}
			foreach (ReportSection reportSection in m_reportSections)
			{
				reportSection.TraverseScopes(visitor);
			}
		}

		internal void UpdateTopLeftDataRegion(InitializationContext context, DataRegion dataRegion)
		{
			if (m_topLeftDataRegion == null || m_topLeftDataRegionAbsTop > context.CurrentAbsoluteTop || (0.0 == Math.Round(m_topLeftDataRegionAbsTop - context.CurrentAbsoluteTop, 10) && m_topLeftDataRegionAbsLeft > context.CurrentAbsoluteLeft))
			{
				m_topLeftDataRegion = dataRegion;
				m_topLeftDataRegionAbsTop = context.CurrentAbsoluteTop;
				m_topLeftDataRegionAbsLeft = context.CurrentAbsoluteLeft;
			}
		}

		bool IRIFReportScope.TextboxInScope(int sequenceIndex)
		{
			return false;
		}

		void IRIFReportScope.AddInScopeTextBox(TextBox textbox)
		{
			Global.Tracer.Assert(condition: false);
		}

		void IRIFReportScope.ResetTextBoxImpls(OnDemandProcessingContext context)
		{
		}

		bool IRIFReportScope.VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_variablesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		void IRIFReportScope.AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			if (m_inScopeEventSources == null)
			{
				m_inScopeEventSources = new List<IInScopeEventSource>();
			}
			m_inScopeEventSources.Add(eventSource);
		}

		internal void AddEventSource(IInScopeEventSource eventSource)
		{
			if (m_eventSources == null)
			{
				m_eventSources = new List<IInScopeEventSource>();
			}
			m_eventSources.Add(eventSource);
		}

		internal void AddGroupWithVariables(ReportHierarchyNode node)
		{
			if (m_groupsWithVariables == null)
			{
				m_groupsWithVariables = new List<ReportHierarchyNode>();
			}
			m_groupsWithVariables.Add(node);
		}

		private void AllocateDatasetDependencyMatrix()
		{
			if (MappingNameToDataSet != null)
			{
				int dataSetCount = DataSetCount;
				if (dataSetCount < 10000)
				{
					m_flattenedDatasetDependencyMatrix = new byte[(int)Math.Ceiling((double)(dataSetCount * dataSetCount) / 8.0)];
				}
			}
		}

		private void CalculateOffsetAndMask(int datasetIndex, int referencedDatasetIndex, out int byteOffset, out byte bitMask)
		{
			int dataSetCount = DataSetCount;
			byteOffset = dataSetCount * datasetIndex + referencedDatasetIndex;
			byte b = (byte)(byteOffset % 8);
			bitMask = (byte)(SequenceIndex.BitMask001 << (int)b);
			byteOffset >>= 3;
		}

		internal void SetDatasetDependency(int datasetIndex, int referencedDatasetIndex, bool clearDependency)
		{
			if (m_flattenedDatasetDependencyMatrix != null)
			{
				CalculateOffsetAndMask(datasetIndex, referencedDatasetIndex, out int byteOffset, out byte bitMask);
				if (clearDependency)
				{
					bitMask = (byte)(bitMask ^ SequenceIndex.BitMask255);
					m_flattenedDatasetDependencyMatrix[byteOffset] &= bitMask;
				}
				else
				{
					m_flattenedDatasetDependencyMatrix[byteOffset] |= bitMask;
				}
			}
		}

		internal bool HasDatasetDependency(int datasetIndex, int referencedDatasetIndex)
		{
			if (m_flattenedDatasetDependencyMatrix == null)
			{
				return false;
			}
			CalculateOffsetAndMask(datasetIndex, referencedDatasetIndex, out int byteOffset, out byte bitMask);
			return (m_flattenedDatasetDependencyMatrix[byteOffset] & bitMask) > 0;
		}

		internal void ClearDatasetParameterOnlyDependencies(int datasetIndex)
		{
			if (m_flattenedDatasetDependencyMatrix != null)
			{
				int dataSetCount = DataSetCount;
				for (int i = 0; i < dataSetCount; i++)
				{
					SetDatasetDependency(i, datasetIndex, clearDependency: true);
					SetDatasetDependency(datasetIndex, i, clearDependency: true);
				}
			}
		}

		internal int CalculateDatasetRootIndex(int suggestedRootIndex, bool[] exclusionList, int unprocessedDataSetCount)
		{
			if (m_flattenedDatasetDependencyMatrix == null)
			{
				return suggestedRootIndex;
			}
			int dataSetCount = DataSetCount;
			if (exclusionList == null)
			{
				exclusionList = new bool[dataSetCount];
				unprocessedDataSetCount = dataSetCount;
			}
			if (!exclusionList[suggestedRootIndex])
			{
				exclusionList[suggestedRootIndex] = true;
				unprocessedDataSetCount--;
			}
			int num = -1;
			while (++num < dataSetCount && unprocessedDataSetCount > 0)
			{
				if (!exclusionList[num] && HasDatasetDependency(suggestedRootIndex, num))
				{
					suggestedRootIndex = num;
					exclusionList[num] = true;
					unprocessedDataSetCount--;
					num = -1;
				}
			}
			return suggestedRootIndex;
		}

		internal void Phase4_DetermineFirstDatasetToProcess()
		{
			if (m_topLeftDataRegion == null)
			{
				m_firstDataSetIndexToProcess = 0;
				return;
			}
			int indexInCollection = m_topLeftDataRegion.GetDataSet(this).IndexInCollection;
			m_firstDataSetIndexToProcess = CalculateDatasetRootIndex(indexInCollection, null, -1);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportVersion, Token.Guid));
			list.Add(new MemberInfo(MemberName.Author, Token.String));
			list.Add(new MemberInfo(MemberName.AutoRefresh, Token.Int32));
			list.Add(new MemberInfo(MemberName.EmbeddedImages, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.Page, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page));
			list.Add(new ReadOnlyMemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new MemberInfo(MemberName.DataSources, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.CompiledCode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.MergeOnePass, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageMergeOnePass, Token.Boolean));
			list.Add(new MemberInfo(MemberName.SubReportMergeTransactions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NeedPostGroupProcessing, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasPostSortAggregates, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasReportItemReferences, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ShowHideType, Token.Enum));
			list.Add(new MemberInfo(MemberName.LastID, Token.Int32));
			list.Add(new MemberInfo(MemberName.SubReports, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport));
			list.Add(new MemberInfo(MemberName.HasImageStreams, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasLabels, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ParametersNotUsedInQuery, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef));
			list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			list.Add(new MemberInfo(MemberName.CodeModules, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.CodeClasses, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CodeClass));
			list.Add(new MemberInfo(MemberName.HasSpecialRecursiveAggregates, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Language, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataTransform, Token.String));
			list.Add(new MemberInfo(MemberName.DataSchema, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Code, Token.String));
			list.Add(new MemberInfo(MemberName.HasUserSortFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CompiledCodeGeneratedWithRefusedPermissions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NonDetailSortFiltersInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.Variables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable));
			list.Add(new MemberInfo(MemberName.DeferVariableEvaluation, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataRegions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion));
			list.Add(new MemberInfo(MemberName.FirstDataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.TopLeftDataRegion, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.Reference));
			list.Add(new MemberInfo(MemberName.DataSetsNotOnlyUsedInParameters, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasPreviousAggregates, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.InScopeTextBoxesInBody, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new ReadOnlyMemberInfo(MemberName.InScopeTextBoxesInPage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.EventSources, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.GroupsWithVariables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode));
			list.Add(new MemberInfo(MemberName.ConsumeContainerWhitespace, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FlattenedDatasetDependencyMatrix, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.FirstDataSetIndexToProcess, Token.Int32));
			list.Add(new ReadOnlyMemberInfo(MemberName.TextboxesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.HasLookups, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ReportSections, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection));
			list.Add(new MemberInfo(MemberName.HasHeadersOrFooters, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AutoRefreshExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InitialPageName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HasAggregatesOfAggregates, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasAggregatesOfAggregatesInUserSort, Token.Boolean));
			list.Add(new MemberInfo(MemberName.SharedDSContainerCollectionIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataPipelineCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.DefaultFontFamily, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Report, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportVersion:
					writer.Write(m_reportVersion);
					break;
				case MemberName.Author:
					writer.Write(m_author);
					break;
				case MemberName.AutoRefresh:
					writer.Write(m_autoRefresh);
					break;
				case MemberName.AutoRefreshExpression:
					writer.Write(m_autoRefreshExpression);
					break;
				case MemberName.EmbeddedImages:
					writer.WriteStringRIFObjectDictionary(m_embeddedImages);
					break;
				case MemberName.DataSources:
					writer.Write(m_dataSources);
					break;
				case MemberName.CompiledCode:
					writer.Write(m_exprCompiledCode);
					break;
				case MemberName.MergeOnePass:
					writer.Write(m_mergeOnePass);
					break;
				case MemberName.SubReportMergeTransactions:
					writer.Write(m_subReportMergeTransactions);
					break;
				case MemberName.NeedPostGroupProcessing:
					writer.Write(m_needPostGroupProcessing);
					break;
				case MemberName.HasPostSortAggregates:
					writer.Write(m_hasPostSortAggregates);
					break;
				case MemberName.HasReportItemReferences:
					writer.Write(m_hasReportItemReferences);
					break;
				case MemberName.ShowHideType:
					writer.WriteEnum((int)m_showHideType);
					break;
				case MemberName.LastID:
					writer.Write(m_lastID);
					break;
				case MemberName.SubReports:
					writer.WriteListOfReferences(m_subReports);
					break;
				case MemberName.HasImageStreams:
					writer.Write(m_hasImageStreams);
					break;
				case MemberName.HasLabels:
					writer.Write(m_hasLabels);
					break;
				case MemberName.HasBookmarks:
					writer.Write(m_hasBookmarks);
					break;
				case MemberName.ParametersNotUsedInQuery:
					writer.Write(m_parametersNotUsedInQuery);
					break;
				case MemberName.Parameters:
					writer.Write(m_parameters);
					break;
				case MemberName.DataSetName:
					writer.Write(m_oneDataSetName);
					break;
				case MemberName.CodeModules:
					writer.WriteListOfPrimitives(m_codeModules);
					break;
				case MemberName.CodeClasses:
					writer.Write(m_codeClasses);
					break;
				case MemberName.HasSpecialRecursiveAggregates:
					writer.Write(m_hasSpecialRecursiveAggregates);
					break;
				case MemberName.Language:
					writer.Write(m_language);
					break;
				case MemberName.DataTransform:
					writer.Write(m_dataTransform);
					break;
				case MemberName.DataSchema:
					writer.Write(m_dataSchema);
					break;
				case MemberName.DataElementStyleAttribute:
					writer.Write(m_dataElementStyleAttribute);
					break;
				case MemberName.Code:
					writer.Write(m_code);
					break;
				case MemberName.HasUserSortFilter:
					writer.Write(m_hasUserSortFilter);
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					writer.Write(m_exprCompiledCodeGeneratedWithRefusedPermissions);
					break;
				case MemberName.NonDetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(m_nonDetailSortFiltersInScope);
					break;
				case MemberName.DetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(m_detailSortFiltersInScope);
					break;
				case MemberName.Variables:
					writer.Write(m_variables);
					break;
				case MemberName.DeferVariableEvaluation:
					writer.Write(m_deferVariableEvaluation);
					break;
				case MemberName.DataRegions:
					writer.WriteListOfReferences(m_topLevelDataRegions);
					break;
				case MemberName.FirstDataSet:
					writer.WriteReference(m_firstDataSet);
					break;
				case MemberName.TopLeftDataRegion:
					writer.WriteReference(m_topLeftDataRegion);
					break;
				case MemberName.DataSetsNotOnlyUsedInParameters:
					writer.Write(m_dataSetsNotOnlyUsedInParameters);
					break;
				case MemberName.HasPreviousAggregates:
					writer.Write(m_hasPreviousAggregates);
					break;
				case MemberName.InScopeEventSources:
					writer.WriteListOfReferences(m_inScopeEventSources);
					break;
				case MemberName.EventSources:
					writer.WriteListOfReferences(m_eventSources);
					break;
				case MemberName.GroupsWithVariables:
					writer.WriteListOfReferences(m_groupsWithVariables);
					break;
				case MemberName.ConsumeContainerWhitespace:
					writer.Write(m_consumeContainerWhitespace);
					break;
				case MemberName.FlattenedDatasetDependencyMatrix:
					writer.Write(m_flattenedDatasetDependencyMatrix);
					break;
				case MemberName.FirstDataSetIndexToProcess:
					writer.Write(m_firstDataSetIndexToProcess);
					break;
				case MemberName.VariablesInScope:
					writer.Write(m_variablesInScope);
					break;
				case MemberName.HasLookups:
					writer.Write(m_hasLookups);
					break;
				case MemberName.ReportSections:
					writer.Write(m_reportSections);
					break;
				case MemberName.HasHeadersOrFooters:
					writer.Write(m_hasHeadersOrFooters);
					break;
				case MemberName.InitialPageName:
					writer.Write(m_initialPageName);
					break;
				case MemberName.HasAggregatesOfAggregates:
					writer.Write(m_hasAggregatesOfAggregates);
					break;
				case MemberName.HasAggregatesOfAggregatesInUserSort:
					writer.Write(m_hasAggregatesOfAggregatesInUserSort);
					break;
				case MemberName.SharedDSContainerCollectionIndex:
					writer.Write(m_sharedDSContainerCollectionIndex);
					break;
				case MemberName.DataPipelineCount:
					writer.Write(m_dataPipelineCount);
					break;
				case MemberName.DefaultFontFamily:
					writer.Write(m_defaultFontFamily);
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
			ReportSection reportSection = null;
			byte[] textboxesInScope = null;
			List<TextBox> inScopeTextBoxes = null;
			List<DataAggregateInfo> pageAggregates = null;
			if (reader.IntermediateFormatVersion.CompareTo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.BIRefresh) < 0)
			{
				reportSection = new ReportSection(0);
				reportSection.Name = "ReportSection0";
				reportSection.Width = base.Width;
				reportSection.WidthValue = base.WidthValue;
				reportSection.DataElementName = reportSection.DataElementNameDefault;
				reportSection.DataElementOutput = reportSection.DataElementOutputDefault;
				reportSection.ExprHostID = 0;
				reportSection.ParentInstancePath = this;
				reportSection.Height = base.Height;
				reportSection.HeightValue = base.HeightValue;
				reportSection.StyleClass = base.StyleClass;
				m_reportSections = new List<ReportSection>();
				m_reportSections.Add(reportSection);
			}
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReportVersion:
					m_reportVersion = reader.ReadGuid();
					break;
				case MemberName.Author:
					m_author = reader.ReadString();
					break;
				case MemberName.AutoRefresh:
					m_autoRefresh = reader.ReadInt32();
					break;
				case MemberName.AutoRefreshExpression:
					m_autoRefreshExpression = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EmbeddedImages:
					m_embeddedImages = reader.ReadStringRIFObjectDictionary<ImageInfo>();
					break;
				case MemberName.Page:
				{
					Page page2 = reportSection.Page = (Page)reader.ReadRIFObject();
					reportSection.Page.ExprHostID = 0;
					bool flag = page2.UpgradedSnapshotPageHeaderEvaluation || page2.UpgradedSnapshotPageFooterEvaluation;
					reportSection.NeedsOverallTotalPages |= flag;
					reportSection.NeedsReportItemsOnPage |= flag;
					if (page2.PageHeader != null || page2.PageFooter != null)
					{
						m_hasHeadersOrFooters = true;
					}
					break;
				}
				case MemberName.ReportItems:
					reportSection.ReportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.DataSources:
					m_dataSources = reader.ReadGenericListOfRIFObjects<DataSource>();
					break;
				case MemberName.PageAggregates:
					pageAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.CompiledCode:
					m_exprCompiledCode = reader.ReadByteArray();
					break;
				case MemberName.MergeOnePass:
					m_mergeOnePass = reader.ReadBoolean();
					break;
				case MemberName.PageMergeOnePass:
					reportSection.NeedsReportItemsOnPage |= !reader.ReadBoolean();
					break;
				case MemberName.SubReportMergeTransactions:
					m_subReportMergeTransactions = reader.ReadBoolean();
					break;
				case MemberName.NeedPostGroupProcessing:
					m_needPostGroupProcessing = reader.ReadBoolean();
					break;
				case MemberName.HasPostSortAggregates:
					m_hasPostSortAggregates = reader.ReadBoolean();
					break;
				case MemberName.HasReportItemReferences:
					m_hasReportItemReferences = reader.ReadBoolean();
					break;
				case MemberName.ShowHideType:
					m_showHideType = (ShowHideTypes)reader.ReadEnum();
					break;
				case MemberName.LastID:
					m_lastID = reader.ReadInt32();
					break;
				case MemberName.SubReports:
					m_subReports = reader.ReadGenericListOfReferences<SubReport>(this);
					break;
				case MemberName.HasImageStreams:
					m_hasImageStreams = reader.ReadBoolean();
					break;
				case MemberName.HasLabels:
					m_hasLabels = reader.ReadBoolean();
					break;
				case MemberName.HasBookmarks:
					m_hasBookmarks = reader.ReadBoolean();
					break;
				case MemberName.ParametersNotUsedInQuery:
					m_parametersNotUsedInQuery = reader.ReadBoolean();
					break;
				case MemberName.Parameters:
					m_parameters = reader.ReadGenericListOfRIFObjects<ParameterDef>();
					break;
				case MemberName.DataSetName:
					m_oneDataSetName = reader.ReadString();
					break;
				case MemberName.CodeModules:
					m_codeModules = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.CodeClasses:
					m_codeClasses = reader.ReadGenericListOfRIFObjects<CodeClass>();
					break;
				case MemberName.HasSpecialRecursiveAggregates:
					m_hasSpecialRecursiveAggregates = reader.ReadBoolean();
					break;
				case MemberName.Language:
					m_language = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataTransform:
					m_dataTransform = reader.ReadString();
					break;
				case MemberName.DataSchema:
					m_dataSchema = reader.ReadString();
					break;
				case MemberName.DataElementStyleAttribute:
					m_dataElementStyleAttribute = reader.ReadBoolean();
					break;
				case MemberName.Code:
					m_code = reader.ReadString();
					break;
				case MemberName.HasUserSortFilter:
					m_hasUserSortFilter = reader.ReadBoolean();
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					m_exprCompiledCodeGeneratedWithRefusedPermissions = reader.ReadBoolean();
					break;
				case MemberName.NonDetailSortFiltersInScope:
					m_nonDetailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.DetailSortFiltersInScope:
					m_detailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.Variables:
					m_variables = reader.ReadGenericListOfRIFObjects<Variable>();
					break;
				case MemberName.DeferVariableEvaluation:
					m_deferVariableEvaluation = reader.ReadBoolean();
					break;
				case MemberName.DataRegions:
					m_topLevelDataRegions = reader.ReadGenericListOfReferences<DataRegion>(this);
					break;
				case MemberName.FirstDataSet:
					m_firstDataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.TopLeftDataRegion:
					m_topLeftDataRegion = reader.ReadReference<DataRegion>(this);
					break;
				case MemberName.DataSetsNotOnlyUsedInParameters:
					m_dataSetsNotOnlyUsedInParameters = reader.ReadInt32();
					break;
				case MemberName.HasPreviousAggregates:
					m_hasPreviousAggregates = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxesInBody:
					reportSection.SetInScopeTextBoxes(reader.ReadGenericListOfReferences<TextBox>(this));
					break;
				case MemberName.InScopeTextBoxesInPage:
					inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.InScopeEventSources:
					m_inScopeEventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.EventSources:
					m_eventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.GroupsWithVariables:
					m_groupsWithVariables = reader.ReadGenericListOfReferences<ReportHierarchyNode>(this);
					break;
				case MemberName.ConsumeContainerWhitespace:
					m_consumeContainerWhitespace = reader.ReadBoolean();
					break;
				case MemberName.FlattenedDatasetDependencyMatrix:
					m_flattenedDatasetDependencyMatrix = reader.ReadByteArray();
					break;
				case MemberName.FirstDataSetIndexToProcess:
					m_firstDataSetIndexToProcess = reader.ReadInt32();
					break;
				case MemberName.TextboxesInScope:
				{
					byte[] textboxesInScope2 = reader.ReadByteArray();
					reportSection.SetTextboxesInScope(textboxesInScope2);
					textboxesInScope = null;
					break;
				}
				case MemberName.VariablesInScope:
					m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.HasLookups:
					m_hasLookups = reader.ReadBoolean();
					break;
				case MemberName.ReportSections:
					m_reportSections = reader.ReadGenericListOfRIFObjects<ReportSection>();
					break;
				case MemberName.HasHeadersOrFooters:
					m_hasHeadersOrFooters = reader.ReadBoolean();
					break;
				case MemberName.InitialPageName:
					m_initialPageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HasAggregatesOfAggregates:
					m_hasAggregatesOfAggregates = reader.ReadBoolean();
					break;
				case MemberName.HasAggregatesOfAggregatesInUserSort:
					m_hasAggregatesOfAggregatesInUserSort = reader.ReadBoolean();
					break;
				case MemberName.SharedDSContainerCollectionIndex:
					m_sharedDSContainerCollectionIndex = reader.ReadInt32();
					break;
				case MemberName.DataPipelineCount:
					m_dataPipelineCount = reader.ReadInt32();
					break;
				case MemberName.DefaultFontFamily:
					m_defaultFontFamily = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			if (reportSection != null)
			{
				reportSection.ID = ++m_lastID;
				reportSection.GlobalID = reportSection.ReportItems.GlobalID * -1;
				reportSection.Page.SetTextboxesInScope(textboxesInScope);
				reportSection.Page.SetInScopeTextBoxes(inScopeTextBoxes);
				reportSection.Page.PageAggregates = pageAggregates;
			}
			if (m_name == null)
			{
				m_name = "Report";
			}
			reader.ResolveReferences();
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item4 in value)
			{
				switch (item4.MemberName)
				{
				case MemberName.SubReports:
				{
					if (m_subReports == null)
					{
						m_subReports = new List<SubReport>();
					}
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value9);
					Global.Tracer.Assert(value9 != null && value9 is SubReport && !m_subReports.Contains((SubReport)value9));
					m_subReports.Add((SubReport)value9);
					break;
				}
				case MemberName.DataRegions:
				{
					if (m_topLevelDataRegions == null)
					{
						m_topLevelDataRegions = new List<DataRegion>();
					}
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value3);
					Global.Tracer.Assert(value3 != null && ((ReportItem)value3).IsDataRegion && !m_topLevelDataRegions.Contains((DataRegion)value3));
					m_topLevelDataRegions.Add((DataRegion)value3);
					break;
				}
				case MemberName.FirstDataSet:
				{
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value6);
					Global.Tracer.Assert(value6 != null && value6 is DataSet);
					m_firstDataSet = (DataSet)value6;
					break;
				}
				case MemberName.TopLeftDataRegion:
				{
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value7);
					Global.Tracer.Assert(value7 != null && ((ReportItem)value7).IsDataRegion);
					m_topLeftDataRegion = (DataRegion)value7;
					break;
				}
				case MemberName.InScopeTextBoxesInBody:
				{
					Global.Tracer.Assert(m_reportSections != null && m_reportSections.Count == 1, "Expected single section");
					ReportSection reportSection2 = m_reportSections[0];
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value10);
					TextBox textbox2 = (TextBox)value10;
					reportSection2.AddInScopeTextBox(textbox2);
					break;
				}
				case MemberName.InScopeTextBoxesInPage:
				{
					Global.Tracer.Assert(m_reportSections != null && m_reportSections.Count == 1, "Expected single section");
					ReportSection reportSection = m_reportSections[0];
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value4);
					TextBox textbox = (TextBox)value4;
					reportSection.Page.AddInScopeTextBox(textbox);
					break;
				}
				case MemberName.InScopeEventSources:
				{
					if (m_inScopeEventSources == null)
					{
						m_inScopeEventSources = new List<IInScopeEventSource>();
					}
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value8);
					IInScopeEventSource item3 = (IInScopeEventSource)value8;
					m_inScopeEventSources.Add(item3);
					break;
				}
				case MemberName.EventSources:
				{
					if (m_eventSources == null)
					{
						m_eventSources = new List<IInScopeEventSource>();
					}
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value5);
					IInScopeEventSource item2 = (IInScopeEventSource)value5;
					m_eventSources.Add(item2);
					break;
				}
				case MemberName.GroupsWithVariables:
				{
					if (m_groupsWithVariables == null)
					{
						m_groupsWithVariables = new List<ReportHierarchyNode>();
					}
					referenceableItems.TryGetValue(item4.RefID, out IReferenceable value2);
					ReportHierarchyNode item = (ReportHierarchyNode)value2;
					m_groupsWithVariables.Add(item);
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Report;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			m_exprHost = reportExprHost;
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (reportExprHost.VariableValueHosts != null)
			{
				reportExprHost.VariableValueHosts.SetReportObjectModel(reportObjectModel);
			}
			for (int i = 0; i < m_reportSections.Count; i++)
			{
				m_reportSections[i].SetExprHost(reportExprHost, reportObjectModel);
			}
			if ((reportExprHost.LookupExprHostsRemotable == null && reportExprHost.LookupDestExprHostsRemotable == null && reportExprHost.DataSetHostsRemotable == null) || m_dataSources == null)
			{
				return;
			}
			for (int j = 0; j < m_dataSources.Count; j++)
			{
				DataSource dataSource = m_dataSources[j];
				if (dataSource.DataSets != null)
				{
					for (int k = 0; k < dataSource.DataSets.Count; k++)
					{
						dataSource.DataSets[k].SetExprHost(reportExprHost, reportObjectModel);
					}
				}
			}
		}

		internal int EvaluateAutoRefresh(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			if (m_autoRefreshExpression == null)
			{
				return Math.Max(0, m_autoRefresh);
			}
			if (m_autoRefresh < 0)
			{
				if (!m_autoRefreshExpression.IsExpression)
				{
					m_autoRefresh = m_autoRefreshExpression.IntValue;
				}
				else
				{
					context.SetupContext(this, romInstance);
					m_autoRefresh = Math.Max(0, context.ReportRuntime.EvaluateReportAutoRefreshExpression(this));
				}
			}
			return m_autoRefresh;
		}

		internal string EvaluateInitialPageName(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, instance);
			return context.ReportRuntime.EvaluateInitialPageNameExpression(this);
		}

		internal void RegisterDataSetScopedAggregates(OnDemandProcessingContext odpContext)
		{
			int count = MappingDataSetIndexToDataSet.Count;
			for (int i = 0; i < count; i++)
			{
				odpContext.RuntimeInitializeAggregates(MappingDataSetIndexToDataSet[i].Aggregates);
				odpContext.RuntimeInitializeAggregates(MappingDataSetIndexToDataSet[i].PostSortAggregates);
			}
		}

		private void GenerateDataSetMappings()
		{
			if (m_mappingNameToDataSet != null)
			{
				return;
			}
			m_mappingNameToDataSet = new Dictionary<string, DataSet>();
			m_mappingDataSetIndexToDataSourceIndex = new List<int>();
			m_mappingDataSetIndexToDataSet = new List<DataSet>();
			int num = (m_dataSources != null) ? m_dataSources.Count : 0;
			for (int i = 0; i < num; i++)
			{
				DataSource dataSource = m_dataSources[i];
				int num2 = (dataSource.DataSets != null) ? dataSource.DataSets.Count : 0;
				for (int j = 0; j < num2; j++)
				{
					DataSet dataSet = dataSource.DataSets[j];
					AddDataSetMapping(i, dataSet);
				}
			}
		}

		private void AddDataSetMapping(int dataSourceIndex, DataSet dataSet)
		{
			if (!m_mappingNameToDataSet.ContainsKey(dataSet.Name))
			{
				m_mappingNameToDataSet.Add(dataSet.Name, dataSet);
				for (int i = m_mappingDataSetIndexToDataSourceIndex.Count; i <= dataSet.IndexInCollection; i++)
				{
					m_mappingDataSetIndexToDataSourceIndex.Add(-1);
					m_mappingDataSetIndexToDataSet.Add(null);
				}
				m_mappingDataSetIndexToDataSourceIndex[dataSet.IndexInCollection] = dataSourceIndex;
				m_mappingDataSetIndexToDataSet[dataSet.IndexInCollection] = dataSet;
			}
		}
	}
}
