using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataSet : IDOwner, IAggregateHolder, ISortFilterScope, IPersistable, IReferenceable, IGloballyReferenceable, IGlobalIDOwner, IRIFDataScope
	{
		internal enum TriState
		{
			Auto,
			True,
			False
		}

		internal const uint CompareFlag_Default = 0u;

		internal const uint CompareFlag_IgnoreCase = 1u;

		internal const uint CompareFlag_IgnoreNonSpace = 2u;

		internal const uint CompareFlag_IgnoreKanatype = 65536u;

		internal const uint CompareFlag_IgnoreWidth = 131072u;

		private DataSetCore m_dataSetCore;

		[Reference]
		private List<DataRegion> m_dataRegions;

		private List<DataAggregateInfo> m_aggregates;

		private List<LookupInfo> m_lookups;

		private List<LookupDestinationInfo> m_lookupDestinationInfos;

		private bool m_usedOnlyInParameters;

		private List<DataAggregateInfo> m_postSortAggregates;

		private bool m_hasDetailUserSortFilter;

		private List<ExpressionInfo> m_userSortExpressions;

		private bool m_hasSubReports;

		private int m_indexInCollection = -1;

		private bool m_hasScopeWithCustomAggregates;

		[Reference]
		private DataSource m_dataSource;

		private bool m_allowIncrementalProcessing = true;

		private List<DefaultRelationship> m_defaultRelationships;

		[NonSerialized]
		private bool m_usedOnlyInParametersSet;

		[NonSerialized]
		private bool[] m_isSortFilterTarget;

		[NonSerialized]
		private bool m_usedInAggregates;

		[NonSerialized]
		private bool? m_hasSameDataSetLookups;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet;

		internal DataSetCore DataSetCore
		{
			get
			{
				return m_dataSetCore;
			}
			set
			{
				m_dataSetCore = value;
			}
		}

		public string Name
		{
			get
			{
				return m_dataSetCore.Name;
			}
			set
			{
				m_dataSetCore.Name = value;
			}
		}

		internal List<Field> Fields
		{
			get
			{
				return m_dataSetCore.Fields;
			}
			set
			{
				m_dataSetCore.Fields = value;
			}
		}

		internal bool HasAggregateIndicatorFields => m_dataSetCore.HasAggregateIndicatorFields;

		internal ReportQuery Query
		{
			get
			{
				return m_dataSetCore.Query;
			}
			set
			{
				m_dataSetCore.Query = value;
			}
		}

		internal SharedDataSetQuery SharedDataSetQuery
		{
			get
			{
				return m_dataSetCore.SharedDataSetQuery;
			}
			set
			{
				m_dataSetCore.SharedDataSetQuery = value;
			}
		}

		internal bool IsReferenceToSharedDataSet => m_dataSetCore.SharedDataSetQuery != null;

		internal TriState CaseSensitivity
		{
			get
			{
				return m_dataSetCore.CaseSensitivity;
			}
			set
			{
				m_dataSetCore.CaseSensitivity = value;
			}
		}

		internal string Collation
		{
			get
			{
				return m_dataSetCore.Collation;
			}
			set
			{
				m_dataSetCore.Collation = value;
			}
		}

		internal string CollationCulture
		{
			get
			{
				return m_dataSetCore.CollationCulture;
			}
			set
			{
				m_dataSetCore.CollationCulture = value;
			}
		}

		internal TriState AccentSensitivity
		{
			get
			{
				return m_dataSetCore.AccentSensitivity;
			}
			set
			{
				m_dataSetCore.AccentSensitivity = value;
			}
		}

		internal TriState KanatypeSensitivity
		{
			get
			{
				return m_dataSetCore.KanatypeSensitivity;
			}
			set
			{
				m_dataSetCore.KanatypeSensitivity = value;
			}
		}

		internal TriState WidthSensitivity
		{
			get
			{
				return m_dataSetCore.WidthSensitivity;
			}
			set
			{
				m_dataSetCore.WidthSensitivity = value;
			}
		}

		internal bool NullsAsBlanks
		{
			get
			{
				return m_dataSetCore.NullsAsBlanks;
			}
			set
			{
				m_dataSetCore.NullsAsBlanks = value;
			}
		}

		internal bool UseOrdinalStringKeyGeneration
		{
			get
			{
				return m_dataSetCore.UseOrdinalStringKeyGeneration;
			}
			set
			{
				m_dataSetCore.UseOrdinalStringKeyGeneration = value;
			}
		}

		internal List<Filter> Filters
		{
			get
			{
				return m_dataSetCore.Filters;
			}
			set
			{
				m_dataSetCore.Filters = value;
			}
		}

		internal List<DataRegion> DataRegions
		{
			get
			{
				return m_dataRegions;
			}
			set
			{
				m_dataRegions = value;
			}
		}

		internal List<DataAggregateInfo> Aggregates
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

		internal List<LookupInfo> Lookups
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

		internal List<LookupDestinationInfo> LookupDestinationInfos
		{
			get
			{
				return m_lookupDestinationInfos;
			}
			set
			{
				m_lookupDestinationInfos = value;
			}
		}

		internal bool HasLookups => m_lookupDestinationInfos != null;

		internal bool HasSameDataSetLookups
		{
			get
			{
				if (!m_hasSameDataSetLookups.HasValue)
				{
					m_hasSameDataSetLookups = false;
					if (m_lookupDestinationInfos != null)
					{
						for (int i = 0; i < m_lookupDestinationInfos.Count; i++)
						{
							if (m_lookupDestinationInfos[i].UsedInSameDataSetTablixProcessing)
							{
								m_hasSameDataSetLookups = true;
								break;
							}
						}
					}
				}
				return m_hasSameDataSetLookups.Value;
			}
		}

		internal bool UsedOnlyInParametersSet => m_usedOnlyInParametersSet;

		internal bool UsedOnlyInParameters
		{
			get
			{
				return m_usedOnlyInParameters;
			}
			set
			{
				if (!m_usedOnlyInParametersSet)
				{
					m_usedOnlyInParameters = value;
					m_usedOnlyInParametersSet = true;
				}
			}
		}

		internal int NonCalculatedFieldCount
		{
			get
			{
				return m_dataSetCore.NonCalculatedFieldCount;
			}
			set
			{
				m_dataSetCore.NonCalculatedFieldCount = value;
			}
		}

		internal List<DataAggregateInfo> PostSortAggregates
		{
			get
			{
				return m_postSortAggregates;
			}
			set
			{
				m_postSortAggregates = value;
			}
		}

		internal uint LCID
		{
			get
			{
				return m_dataSetCore.LCID;
			}
			set
			{
				m_dataSetCore.LCID = value;
			}
		}

		internal bool HasDetailUserSortFilter
		{
			get
			{
				return m_hasDetailUserSortFilter;
			}
			set
			{
				m_hasDetailUserSortFilter = value;
			}
		}

		internal List<ExpressionInfo> UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		internal DataSetExprHost ExprHost => m_dataSetCore.ExprHost;

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		int ISortFilterScope.ID => m_ID;

		string ISortFilterScope.ScopeName => m_dataSetCore.Name;

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return null;
			}
			set
			{
				Global.Tracer.Assert(condition: false, string.Empty);
			}
		}

		List<ExpressionInfo> ISortFilterScope.UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost
		{
			get
			{
				if (m_dataSetCore.ExprHost == null)
				{
					return null;
				}
				return m_dataSetCore.ExprHost.UserSortExpressionsHost;
			}
		}

		internal bool UsedInAggregates
		{
			get
			{
				return m_usedInAggregates;
			}
			set
			{
				m_usedInAggregates = value;
			}
		}

		internal bool HasScopeWithCustomAggregates
		{
			get
			{
				return m_hasScopeWithCustomAggregates;
			}
			set
			{
				m_hasScopeWithCustomAggregates = value;
			}
		}

		internal TriState InterpretSubtotalsAsDetails
		{
			get
			{
				return m_dataSetCore.InterpretSubtotalsAsDetails;
			}
			set
			{
				m_dataSetCore.InterpretSubtotalsAsDetails = value;
			}
		}

		internal bool HasSubReports
		{
			get
			{
				return m_hasSubReports;
			}
			set
			{
				m_hasSubReports = value;
			}
		}

		internal int IndexInCollection => m_indexInCollection;

		internal DataSource DataSource
		{
			get
			{
				return m_dataSource;
			}
			set
			{
				m_dataSource = value;
			}
		}

		internal List<DefaultRelationship> DefaultRelationships
		{
			get
			{
				return m_defaultRelationships;
			}
			set
			{
				m_defaultRelationships = value;
			}
		}

		internal bool AllowIncrementalProcessing
		{
			get
			{
				return m_allowIncrementalProcessing;
			}
			set
			{
				m_allowIncrementalProcessing = value;
			}
		}

		public DataScopeInfo DataScopeInfo => null;

		Microsoft.ReportingServices.ReportProcessing.ObjectType IRIFDataScope.DataScopeObjectType => ObjectType;

		internal DataSet(int id, int indexCounter)
			: base(id)
		{
			m_indexInCollection = indexCounter;
			m_dataRegions = new List<DataRegion>();
			m_aggregates = new List<DataAggregateInfo>();
			m_postSortAggregates = new List<DataAggregateInfo>();
			m_dataSetCore = new DataSetCore();
			m_dataSetCore.Fields = new List<Field>();
		}

		internal DataSet()
		{
			m_dataSetCore = new DataSetCore();
		}

		internal DataSet(DataSetCore dataSetCore)
		{
			m_dataSetCore = dataSetCore;
		}

		internal CultureInfo CreateCultureInfoFromLcid()
		{
			return m_dataSetCore.CreateCultureInfoFromLcid();
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_dataSetCore.Name;
			context.RegisterDataSet(this);
			InternalInitialize(context);
			context.UnRegisterDataSet(this);
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataSetStart(m_dataSetCore.Name);
			context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet;
			m_dataSetCore.Initialize(context);
			if (m_defaultRelationships != null)
			{
				foreach (DefaultRelationship defaultRelationship in m_defaultRelationships)
				{
					defaultRelationship.Initialize(this, context);
				}
			}
			if (m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				foreach (ExpressionInfo userSortExpression in m_userSortExpressions)
				{
					context.ExprHostBuilder.UserSortExpression(userSortExpression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			m_dataSetCore.ExprHostID = context.ExprHostBuilder.DataSetEnd();
		}

		internal void BindAndValidateDefaultRelationships(ErrorContext errorContext, Report report)
		{
			if (m_defaultRelationships == null)
			{
				return;
			}
			List<string> list = new List<string>(m_defaultRelationships.Count);
			foreach (DefaultRelationship defaultRelationship in m_defaultRelationships)
			{
				defaultRelationship.BindAndValidate(this, errorContext, report);
				if (list.Contains(defaultRelationship.RelatedDataSetName))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidDefaultRelationshipDuplicateRelatedDataset, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, Name, "DefaultRelationship", "RelatedDataSet", defaultRelationship.RelatedDataSetName.MarkAsPrivate());
				}
				else
				{
					list.Add(defaultRelationship.RelatedDataSetName);
				}
			}
		}

		internal void CheckCircularDefaultRelationshipReference(InitializationContext context)
		{
			HashSet<int> visitedDataSetIds = new HashSet<int>();
			CheckCircularDefaultRelationshipReference(context, this, visitedDataSetIds);
		}

		private void CheckCircularDefaultRelationshipReference(InitializationContext context, DataSet dataSet, HashSet<int> visitedDataSetIds)
		{
			visitedDataSetIds.Add(base.ID);
			if (m_defaultRelationships != null)
			{
				foreach (DefaultRelationship defaultRelationship in m_defaultRelationships)
				{
					if (defaultRelationship.RelatedDataSet != null && !defaultRelationship.IsCrossJoin)
					{
						if (visitedDataSetIds.Contains(defaultRelationship.RelatedDataSet.ID))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDefaultRelationshipCircularReference, Severity.Error, dataSet.ObjectType, dataSet.Name, "DefaultRelationship", Name.MarkAsPrivate());
						}
						else
						{
							defaultRelationship.RelatedDataSet.CheckCircularDefaultRelationshipReference(context, dataSet, visitedDataSetIds);
						}
					}
				}
			}
			visitedDataSetIds.Remove(base.ID);
		}

		internal bool HasDefaultRelationship(DataSet parentDataSet)
		{
			return GetDefaultRelationship(parentDataSet) != null;
		}

		internal DefaultRelationship GetDefaultRelationship(DataSet parentDataSet)
		{
			return JoinInfo.FindActiveRelationship(m_defaultRelationships, parentDataSet);
		}

		internal void DetermineDecomposability(InitializationContext context)
		{
			if (context.EvaluateAtomicityCondition(m_dataSetCore.Filters != null, this, AtomicityReason.Filters) || context.EvaluateAtomicityCondition(HasAggregatesForAtomicityCheck(), this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(HasLookups, this, AtomicityReason.Lookups) || context.EvaluateAtomicityCondition(m_dataRegions.Count > 1, this, AtomicityReason.PeerChildScopes))
			{
				m_allowIncrementalProcessing = false;
			}
		}

		public static bool AreEqualById(DataSet dataSet1, DataSet dataSet2)
		{
			if (dataSet1 == null && dataSet2 == null)
			{
				return true;
			}
			if (dataSet1 == null || dataSet2 == null)
			{
				return false;
			}
			return dataSet1.ID == dataSet2.ID;
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(m_aggregates))
			{
				return DataScopeInfo.HasAggregates(m_postSortAggregates);
			}
			return true;
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return m_postSortAggregates;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_aggregates != null, "(null != m_aggregates)");
			if (m_aggregates.Count == 0)
			{
				m_aggregates = null;
			}
			Global.Tracer.Assert(m_postSortAggregates != null, "(null != m_postSortAggregates)");
			if (m_postSortAggregates.Count == 0)
			{
				m_postSortAggregates = null;
			}
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			m_dataSetCore.SetExprHost(reportExprHost, reportObjectModel);
			if (m_lookups != null && reportExprHost.LookupExprHostsRemotable != null)
			{
				for (int i = 0; i < m_lookups.Count; i++)
				{
					m_lookups[i].SetExprHost(reportExprHost, reportObjectModel);
				}
			}
			if (m_lookupDestinationInfos != null && reportExprHost.LookupDestExprHostsRemotable != null)
			{
				for (int j = 0; j < m_lookupDestinationInfos.Count; j++)
				{
					m_lookupDestinationInfos[j].SetExprHost(reportExprHost, reportObjectModel);
				}
			}
			if (m_defaultRelationships == null || ExprHost == null)
			{
				return;
			}
			foreach (DefaultRelationship defaultRelationship in m_defaultRelationships)
			{
				defaultRelationship.SetExprHost(ExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal void SetFilterExprHost(ObjectModelImpl reportObjectModel)
		{
			m_dataSetCore.SetFilterExprHost(reportObjectModel);
		}

		internal void SetupRuntimeEnvironment(OnDemandProcessingContext odpContext)
		{
			odpContext.SetComparisonInformation(m_dataSetCore);
		}

		internal bool NeedAutoDetectCollation()
		{
			return m_dataSetCore.NeedAutoDetectCollation();
		}

		internal void MergeCollationSettings(ErrorContext errorContext, string dataSourceType, string cultureName, bool caseSensitive, bool accentSensitive, bool kanatypeSensitive, bool widthSensitive)
		{
			m_dataSetCore.MergeCollationSettings(errorContext, dataSourceType, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
		}

		internal void MarkDataRegionsAsNoRows()
		{
			if (m_dataRegions == null)
			{
				return;
			}
			foreach (DataRegion dataRegion in m_dataRegions)
			{
				dataRegion.NoRows = true;
			}
		}

		internal CompareOptions GetCLRCompareOptions()
		{
			return m_dataSetCore.GetCLRCompareOptions();
		}

		internal void ClearDataRegionStreamingScopeInstances()
		{
			if (m_dataRegions == null)
			{
				return;
			}
			foreach (DataRegion dataRegion in m_dataRegions)
			{
				dataRegion.ClearStreamingScopeInstanceBinding();
			}
		}

		internal void RestrictDataSetAggregates(PublishingErrorContext m_errorContext)
		{
			if (m_usedOnlyInParameters || !m_usedInAggregates || (m_dataRegions != null && m_dataRegions.Count != 0))
			{
				return;
			}
			if (m_defaultRelationships != null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDefaultRelationshipIgnored, Severity.Warning, ObjectType, Name, "DefaultRelationship");
			}
			if (m_aggregates == null)
			{
				return;
			}
			foreach (DataAggregateInfo aggregate in m_aggregates)
			{
				if (aggregate.AggregateType != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSetScopedAggregate, Severity.Error, ObjectType, Name, aggregate.AggregateType.ToString());
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.Name, Token.String));
			list.Add(new ReadOnlyMemberInfo(MemberName.Fields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field));
			list.Add(new ReadOnlyMemberInfo(MemberName.Query, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery));
			list.Add(new ReadOnlyMemberInfo(MemberName.CaseSensitivity, Token.Enum));
			list.Add(new ReadOnlyMemberInfo(MemberName.Collation, Token.String));
			list.Add(new ReadOnlyMemberInfo(MemberName.AccentSensitivity, Token.Enum));
			list.Add(new ReadOnlyMemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			list.Add(new ReadOnlyMemberInfo(MemberName.WidthSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.DataRegions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion));
			list.Add(new MemberInfo(MemberName.Aggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.UsedOnlyInParameters, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.NonCalculatedFieldCount, Token.Int32));
			list.Add(new ReadOnlyMemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.LCID, Token.UInt32));
			list.Add(new MemberInfo(MemberName.HasDetailUserSortFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.UserSortExpressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.InterpretSubtotalsAsDetails, Token.Enum));
			list.Add(new MemberInfo(MemberName.HasSubReports, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataSource, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource, Token.Reference));
			list.Add(new MemberInfo(MemberName.Lookups, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo));
			list.Add(new MemberInfo(MemberName.LookupDestinations, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupDestinationInfo));
			list.Add(new MemberInfo(MemberName.DataSetCore, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetCore, Token.Reference));
			list.Add(new MemberInfo(MemberName.AllowIncrementalProcessing, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DefaultRelationships, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DefaultRelationship));
			list.Add(new MemberInfo(MemberName.HasScopeWithCustomAggregates, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetCore:
					writer.Write(m_dataSetCore);
					break;
				case MemberName.DataRegions:
					writer.WriteListOfReferences(m_dataRegions);
					break;
				case MemberName.Aggregates:
					writer.Write(m_aggregates);
					break;
				case MemberName.UsedOnlyInParameters:
					writer.Write(m_usedOnlyInParameters);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(m_postSortAggregates);
					break;
				case MemberName.HasDetailUserSortFilter:
					writer.Write(m_hasDetailUserSortFilter);
					break;
				case MemberName.UserSortExpressions:
					writer.Write(m_userSortExpressions);
					break;
				case MemberName.HasSubReports:
					writer.Write(m_hasSubReports);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.DataSource:
					writer.WriteReference(m_dataSource);
					break;
				case MemberName.Lookups:
					writer.Write(m_lookups);
					break;
				case MemberName.LookupDestinations:
					writer.Write(m_lookupDestinationInfos);
					break;
				case MemberName.AllowIncrementalProcessing:
					writer.Write(m_allowIncrementalProcessing);
					break;
				case MemberName.DefaultRelationships:
					writer.Write(m_defaultRelationships);
					break;
				case MemberName.HasScopeWithCustomAggregates:
					writer.Write(m_hasScopeWithCustomAggregates);
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
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
				case MemberName.Name:
					m_dataSetCore.Name = reader.ReadString();
					break;
				case MemberName.Fields:
					m_dataSetCore.Fields = reader.ReadGenericListOfRIFObjects<Field>();
					break;
				case MemberName.Query:
					m_dataSetCore.Query = (ReportQuery)reader.ReadRIFObject();
					break;
				case MemberName.CaseSensitivity:
					m_dataSetCore.CaseSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.Collation:
					m_dataSetCore.Collation = reader.ReadString();
					break;
				case MemberName.AccentSensitivity:
					m_dataSetCore.AccentSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.KanatypeSensitivity:
					m_dataSetCore.KanatypeSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.WidthSensitivity:
					m_dataSetCore.WidthSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.DataRegions:
					m_dataRegions = reader.ReadGenericListOfReferences<DataRegion>(this);
					break;
				case MemberName.Aggregates:
					m_aggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.Filters:
					m_dataSetCore.Filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.UsedOnlyInParameters:
					m_usedOnlyInParameters = reader.ReadBoolean();
					break;
				case MemberName.NonCalculatedFieldCount:
					m_dataSetCore.NonCalculatedFieldCount = reader.ReadInt32();
					break;
				case MemberName.ExprHostID:
					m_dataSetCore.ExprHostID = reader.ReadInt32();
					break;
				case MemberName.PostSortAggregates:
					m_postSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.LCID:
					m_dataSetCore.LCID = reader.ReadUInt32();
					break;
				case MemberName.HasDetailUserSortFilter:
					m_hasDetailUserSortFilter = reader.ReadBoolean();
					break;
				case MemberName.UserSortExpressions:
					m_userSortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.InterpretSubtotalsAsDetails:
					m_dataSetCore.InterpretSubtotalsAsDetails = (TriState)reader.ReadEnum();
					break;
				case MemberName.HasSubReports:
					m_hasSubReports = reader.ReadBoolean();
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.DataSource:
					m_dataSource = reader.ReadReference<DataSource>(this);
					break;
				case MemberName.Lookups:
					m_lookups = reader.ReadGenericListOfRIFObjects<LookupInfo>();
					break;
				case MemberName.LookupDestinations:
					m_lookupDestinationInfos = reader.ReadGenericListOfRIFObjects<LookupDestinationInfo>();
					break;
				case MemberName.DataSetCore:
					m_dataSetCore = (DataSetCore)reader.ReadRIFObject();
					break;
				case MemberName.AllowIncrementalProcessing:
					m_allowIncrementalProcessing = reader.ReadBoolean();
					break;
				case MemberName.DefaultRelationships:
					m_defaultRelationships = reader.ReadGenericListOfRIFObjects<DefaultRelationship>();
					break;
				case MemberName.HasScopeWithCustomAggregates:
					m_hasScopeWithCustomAggregates = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.DataRegions:
					if (m_dataRegions == null)
					{
						m_dataRegions = new List<DataRegion>();
					}
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(((ReportItem)referenceableItems[item.RefID]).IsDataRegion);
					Global.Tracer.Assert(!m_dataRegions.Contains((DataRegion)referenceableItems[item.RefID]));
					m_dataRegions.Add((DataRegion)referenceableItems[item.RefID]);
					break;
				case MemberName.DataSource:
					Global.Tracer.Assert(m_dataSource == null);
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSource);
					m_dataSource = (DataSource)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet;
		}
	}
}
