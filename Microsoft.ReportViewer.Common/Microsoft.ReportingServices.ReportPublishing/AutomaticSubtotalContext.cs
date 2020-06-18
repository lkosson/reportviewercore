using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal struct AutomaticSubtotalContext
	{
		private string m_objectName;

		private ObjectType m_objectType;

		private LocationFlags m_location;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion m_currentDataRegion;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion m_currentDataRegionClone;

		private Map m_currentMapClone;

		private MapVectorLayer m_currentMapVectorLayerClone;

		private string m_currentScope;

		private string m_currentScopeBeingCloned;

		private List<ICreateSubtotals> m_createSubtotals;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> m_domainScopeGroups;

		private Holder<int> m_startIndex;

		private Holder<int> m_currentIndex;

		private List<CellList> m_cellLists;

		private List<TablixColumn> m_tablixColumns;

		private RowList m_rows;

		private ScopeTree m_scopeTree;

		private Dictionary<string, string> m_scopeNameMap;

		private Dictionary<string, string> m_reportItemNameMap;

		private Dictionary<string, string> m_aggregateMap;

		private Dictionary<string, string> m_lookupMap;

		private Dictionary<string, string> m_variableNameMap;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> m_expressionsWithReportItemReferences;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Visibility> m_visibilitiesWithToggleToUpdate;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> m_reportItemsWithRepeatWithToUpdate;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort> m_endUserSortWithTarget;

		private Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope> m_reportScopes;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection> m_reportItemCollections;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.IAggregateHolder> m_aggregateHolders;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.IRunningValueHolder> m_runningValueHolders;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo m_outerAggregate;

		private Dictionary<string, IRIFDataScope> m_scopeNamesToClone;

		private IRIFDataScope m_currentDataScope;

		private NameValidator m_reportItemNameValidator;

		private NameValidator m_scopeNameValidator;

		private NameValidator m_variableNameValidator;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		private bool m_dynamicWithStaticPeerEncountered;

		private int m_headerLevel;

		private int m_originalColumnCount;

		private int m_originalRowCount;

		private bool[] m_headerLevelHasStaticArray;

		private Holder<int> m_variableSequenceIdCounter;

		private Holder<int> m_textboxSequenceIdCounter;

		internal LocationFlags Location
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		internal ObjectType ObjectType
		{
			get
			{
				return m_objectType;
			}
			set
			{
				m_objectType = value;
			}
		}

		internal string ObjectName
		{
			get
			{
				return m_objectName;
			}
			set
			{
				m_objectName = value;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion CurrentDataRegion
		{
			get
			{
				return m_currentDataRegion;
			}
			set
			{
				m_currentDataRegion = value;
			}
		}

		internal IRIFDataScope CurrentDataScope
		{
			get
			{
				return m_currentDataScope;
			}
			set
			{
				m_currentDataScope = value;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion CurrentDataRegionClone
		{
			get
			{
				return m_currentDataRegionClone;
			}
			set
			{
				m_currentDataRegionClone = value;
			}
		}

		internal Map CurrentMapClone
		{
			get
			{
				return m_currentMapClone;
			}
			set
			{
				m_currentMapClone = value;
			}
		}

		internal MapVectorLayer CurrentMapVectorLayerClone
		{
			get
			{
				return m_currentMapVectorLayerClone;
			}
			set
			{
				m_currentMapVectorLayerClone = value;
			}
		}

		internal string CurrentScope
		{
			get
			{
				return m_currentScope;
			}
			set
			{
				m_currentScope = value;
			}
		}

		internal string CurrentScopeBeingCloned
		{
			get
			{
				return m_currentScopeBeingCloned;
			}
			set
			{
				m_currentScopeBeingCloned = value;
			}
		}

		internal bool[] HeaderLevelHasStaticArray
		{
			get
			{
				return m_headerLevelHasStaticArray;
			}
			set
			{
				m_headerLevelHasStaticArray = value;
			}
		}

		internal List<ICreateSubtotals> CreateSubtotalsDefinitions => m_createSubtotals;

		internal List<Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> DomainScopeGroups => m_domainScopeGroups;

		internal int StartIndex
		{
			get
			{
				return m_startIndex.Value;
			}
			set
			{
				m_startIndex.Value = value;
			}
		}

		internal int CurrentIndex
		{
			get
			{
				return m_currentIndex.Value;
			}
			set
			{
				m_currentIndex.Value = value;
			}
		}

		internal List<CellList> CellLists
		{
			get
			{
				return m_cellLists;
			}
			set
			{
				m_cellLists = value;
			}
		}

		internal List<TablixColumn> TablixColumns
		{
			get
			{
				return m_tablixColumns;
			}
			set
			{
				m_tablixColumns = value;
			}
		}

		internal RowList Rows
		{
			get
			{
				return m_rows;
			}
			set
			{
				m_rows = value;
			}
		}

		internal bool DynamicWithStaticPeerEncountered
		{
			get
			{
				return m_dynamicWithStaticPeerEncountered;
			}
			set
			{
				m_dynamicWithStaticPeerEncountered = value;
			}
		}

		internal int HeaderLevel
		{
			get
			{
				return m_headerLevel;
			}
			set
			{
				m_headerLevel = value;
			}
		}

		internal int OriginalColumnCount
		{
			get
			{
				return m_originalColumnCount;
			}
			set
			{
				m_originalColumnCount = value;
			}
		}

		internal int OriginalRowCount
		{
			get
			{
				return m_originalRowCount;
			}
			set
			{
				m_originalRowCount = value;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo OuterAggregate
		{
			get
			{
				return m_outerAggregate;
			}
			set
			{
				m_outerAggregate = value;
			}
		}

		internal Dictionary<string, IRIFDataScope> ScopeNamesToClone => m_scopeNamesToClone;

		internal AutomaticSubtotalContext(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, List<ICreateSubtotals> createSubtotals, List<Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> domainScopeGroups, NameValidator reportItemNameValidator, NameValidator scopeNameValidator, NameValidator variableNameValidator, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope> reportScopes, List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection> reportItemCollections, List<Microsoft.ReportingServices.ReportIntermediateFormat.IAggregateHolder> aggregateHolders, List<Microsoft.ReportingServices.ReportIntermediateFormat.IRunningValueHolder> runningValueHolders, Holder<int> variableSequenceIdCounter, Holder<int> textboxSequenceIdCounter, ScopeTree scopeTree)
		{
			m_createSubtotals = createSubtotals;
			m_domainScopeGroups = domainScopeGroups;
			m_reportItemNameValidator = reportItemNameValidator;
			m_scopeNameValidator = scopeNameValidator;
			m_variableNameValidator = variableNameValidator;
			m_report = report;
			m_variableSequenceIdCounter = variableSequenceIdCounter;
			m_textboxSequenceIdCounter = textboxSequenceIdCounter;
			m_dynamicWithStaticPeerEncountered = false;
			m_location = LocationFlags.None;
			m_objectName = null;
			m_objectType = ObjectType.Tablix;
			m_currentDataRegion = null;
			m_cellLists = null;
			m_tablixColumns = null;
			m_rows = null;
			m_scopeNameMap = new Dictionary<string, string>(StringComparer.Ordinal);
			m_reportItemNameMap = new Dictionary<string, string>(StringComparer.Ordinal);
			m_aggregateMap = new Dictionary<string, string>(StringComparer.Ordinal);
			m_lookupMap = new Dictionary<string, string>(StringComparer.Ordinal);
			m_variableNameMap = new Dictionary<string, string>(StringComparer.Ordinal);
			m_currentScope = null;
			m_currentScopeBeingCloned = null;
			m_startIndex = new Holder<int>();
			m_currentIndex = new Holder<int>();
			m_headerLevel = 0;
			m_originalColumnCount = 0;
			m_originalRowCount = 0;
			m_reportScopes = reportScopes;
			m_reportItemCollections = reportItemCollections;
			m_aggregateHolders = aggregateHolders;
			m_runningValueHolders = runningValueHolders;
			m_expressionsWithReportItemReferences = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			m_visibilitiesWithToggleToUpdate = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Visibility>();
			m_reportItemsWithRepeatWithToUpdate = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem>();
			m_endUserSortWithTarget = new List<Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort>();
			m_scopeNamesToClone = new Dictionary<string, IRIFDataScope>(StringComparer.Ordinal);
			m_headerLevelHasStaticArray = null;
			m_currentDataRegionClone = null;
			m_currentMapClone = null;
			m_outerAggregate = null;
			m_scopeTree = scopeTree;
			m_currentDataScope = null;
			m_currentMapVectorLayerClone = null;
		}

		internal int GenerateVariableSequenceID()
		{
			return m_variableSequenceIdCounter.Value++;
		}

		internal int GenerateTextboxSequenceID()
		{
			return m_textboxSequenceIdCounter.Value++;
		}

		internal bool HasStaticPeerWithHeader(TablixMember member, out int spanDifference)
		{
			spanDifference = 0;
			if (member.HeaderLevel == -1)
			{
				return false;
			}
			int num = member.HeaderLevel + (member.IsColumn ? member.RowSpan : member.ColSpan);
			Global.Tracer.Assert(num <= m_headerLevelHasStaticArray.Length, "(count <= m_headerLevelHasStaticArray.Length)");
			for (int i = member.HeaderLevel; i < num; i++)
			{
				if (m_headerLevelHasStaticArray[i])
				{
					if (member.Grouping != null)
					{
						m_dynamicWithStaticPeerEncountered = true;
						spanDifference = i - member.HeaderLevel;
					}
					return true;
				}
			}
			return false;
		}

		internal void AddReportItemCollection(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection collection)
		{
			m_reportItemCollections.Add(collection);
		}

		internal void AddAggregateHolder(Microsoft.ReportingServices.ReportIntermediateFormat.IAggregateHolder aggregateHolder)
		{
			m_aggregateHolders.Add(aggregateHolder);
		}

		internal void AddRunningValueHolder(Microsoft.ReportingServices.ReportIntermediateFormat.IRunningValueHolder runningValueHolder)
		{
			m_runningValueHolders.Add(runningValueHolder);
		}

		internal string CreateUniqueReportItemName(string oldName, bool isClone)
		{
			return CreateUniqueReportItemName(oldName, emptyRectangle: false, isClone);
		}

		internal string CreateUniqueReportItemName(string oldName, bool emptyRectangle, bool isClone)
		{
			string value = null;
			if (!emptyRectangle && m_reportItemNameMap.TryGetValue(oldName, out value))
			{
				return value;
			}
			StringBuilder stringBuilder = null;
			int num = 1;
			if (isClone)
			{
				num = 2;
			}
			do
			{
				stringBuilder = new StringBuilder();
				stringBuilder.Append(oldName);
				if (emptyRectangle && !isClone)
				{
					stringBuilder.Append("_AsRectangle");
				}
				else if (isClone)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append("_InAutoSubtotal");
				}
				stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture.NumberFormat));
				value = stringBuilder.ToString();
				num++;
			}
			while (!m_reportItemNameValidator.Validate(value));
			if (!emptyRectangle)
			{
				if (m_reportItemNameMap.ContainsKey(oldName))
				{
					m_reportItemNameMap[oldName] = value;
				}
				else
				{
					m_reportItemNameMap.Add(oldName, value);
				}
			}
			return value;
		}

		internal string GetNewReportItemName(string oldName)
		{
			if (m_reportItemNameMap.TryGetValue(oldName, out string value))
			{
				return value;
			}
			return oldName;
		}

		internal string CreateAndRegisterUniqueGroupName(string oldName, bool isClone)
		{
			return CreateAndRegisterUniqueGroupName(oldName, isClone, isDomainScope: false);
		}

		internal string CreateAndRegisterUniqueGroupName(string oldName, bool isClone, bool isDomainScope)
		{
			string value = null;
			if (m_scopeNameMap.TryGetValue(oldName, out value))
			{
				return value;
			}
			StringBuilder stringBuilder = null;
			int num = 1;
			if (isClone)
			{
				num = 2;
			}
			do
			{
				stringBuilder = (isDomainScope ? new StringBuilder(oldName.Length + 16) : ((!isClone) ? new StringBuilder(oldName.Length + 19) : new StringBuilder(oldName.Length + 4)));
				stringBuilder.Append(oldName);
				if (isDomainScope)
				{
					stringBuilder.Append("_DomainScope");
				}
				else if (isClone)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append("_InAutoSubtotal");
				}
				stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture.NumberFormat));
				value = stringBuilder.ToString();
				num++;
			}
			while (!m_scopeNameValidator.Validate(value));
			RegisterClonedScopeName(oldName, value);
			return value;
		}

		internal string CreateUniqueVariableName(string oldName, bool isClone)
		{
			StringBuilder stringBuilder = null;
			string text = null;
			int num = 1;
			if (isClone)
			{
				num = 2;
			}
			do
			{
				stringBuilder = ((!isClone) ? new StringBuilder(oldName.Length + 19) : new StringBuilder(oldName.Length + 4));
				stringBuilder.Append(oldName);
				if (isClone)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append("_InAutoSubtotal");
				}
				stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture.NumberFormat));
				text = stringBuilder.ToString();
				num++;
			}
			while (!m_variableNameValidator.Validate(text));
			Global.Tracer.Assert(!m_variableNameMap.ContainsKey(oldName), "(!m_variableNameMap.ContainsKey(oldName))");
			m_variableNameMap.Add(oldName, text);
			return text;
		}

		internal string GetNewVariableName(string oldVariableName)
		{
			if (oldVariableName != null && oldVariableName.Length > 0 && m_variableNameMap.TryGetValue(oldVariableName, out string value))
			{
				return value;
			}
			return oldVariableName;
		}

		internal string GetNewScopeName(string oldScopeName)
		{
			if (oldScopeName != null && oldScopeName.Length > 0 && m_scopeNameMap.TryGetValue(oldScopeName, out string value))
			{
				return value;
			}
			return oldScopeName;
		}

		internal string GetNewScopeNameForInnerOrOuterAggregate(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo originalAggregate)
		{
			string scope = originalAggregate.PublishingInfo.Scope;
			if (m_scopeNamesToClone.TryGetValue(scope, out IRIFDataScope value))
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = value as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion;
				if (dataRegion != null)
				{
					return CreateUniqueReportItemName(scope, dataRegion.IsClone);
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = value as Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
				if (reportHierarchyNode != null)
				{
					return CreateAndRegisterUniqueGroupName(scope, reportHierarchyNode.IsClone);
				}
				Global.Tracer.Assert(false, "Unknown object type in GetNewScopeNameForNestedAggregate: {0}", value);
				return scope;
			}
			IRIFDataScope scopeByName = m_scopeTree.GetScopeByName(m_currentScope);
			int num = (scopeByName == null || !NeedsSubtotalScopeLift(originalAggregate, scopeByName)) ? (-1) : m_scopeTree.MeasureScopeDistance(m_currentScopeBeingCloned, m_currentScope);
			if (num <= 0)
			{
				return scope;
			}
			string text = m_scopeTree.FindAncestorScopeName(scope, num);
			if (text == null)
			{
				return scope;
			}
			if (m_outerAggregate != null && !string.IsNullOrEmpty(m_outerAggregate.PublishingInfo.Scope))
			{
				IRIFDataScope scopeByName2 = m_scopeTree.GetScopeByName(m_outerAggregate.PublishingInfo.Scope);
				IRIFDataScope scopeByName3 = m_scopeTree.GetScopeByName(text);
				if (scopeByName2 != null && scopeByName3 != null && m_scopeTree.IsParentScope(scopeByName3, scopeByName2))
				{
					text = m_outerAggregate.PublishingInfo.Scope;
				}
			}
			return text;
		}

		private bool NeedsSubtotalScopeLift(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate, IRIFDataScope displayScope)
		{
			if (aggregate.PublishingInfo.HasScope)
			{
				IRIFDataScope scopeByName = m_scopeTree.GetScopeByName(aggregate.PublishingInfo.Scope);
				if (scopeByName == null)
				{
					return false;
				}
				if (m_scopeTree.IsParentScope(displayScope, scopeByName))
				{
					return true;
				}
				return false;
			}
			return true;
		}

		internal void RegisterScopeName(string name)
		{
			Global.Tracer.Assert(!m_scopeNameMap.ContainsKey(name), "(!m_scopeNameMap.ContainsKey(name))");
			m_scopeNameMap.Add(name, m_currentScope);
			m_currentScopeBeingCloned = name;
		}

		internal void RegisterClonedScopeName(string oldName, string newName)
		{
			Global.Tracer.Assert(!m_scopeNameMap.ContainsKey(oldName), "(!m_scopeNameMap.ContainsKey(oldName))");
			m_scopeNameMap.Add(oldName, newName);
		}

		internal string CreateAggregateID(string oldID)
		{
			m_report.LastAggregateID++;
			string text = "Aggregate" + m_report.LastAggregateID;
			Global.Tracer.Assert(!m_aggregateMap.ContainsKey(oldID), "(!m_aggregateMap.ContainsKey(oldID))");
			m_aggregateMap.Add(oldID, text);
			return text;
		}

		internal string CreateLookupID(string oldID)
		{
			m_report.LastLookupID++;
			string text = "Lookup" + m_report.LastLookupID;
			Global.Tracer.Assert(!m_lookupMap.ContainsKey(oldID), "(!m_lookupMap.ContainsKey(oldID))");
			m_lookupMap.Add(oldID, text);
			return text;
		}

		internal string GetNewAggregateID(string oldID)
		{
			if (m_aggregateMap.TryGetValue(oldID, out string value))
			{
				return value;
			}
			return oldID;
		}

		internal string GetNewLookupID(string oldID)
		{
			if (m_lookupMap.TryGetValue(oldID, out string value))
			{
				return value;
			}
			return oldID;
		}

		internal int GenerateID()
		{
			m_report.LastID++;
			return m_report.LastID;
		}

		internal void AdjustReferences()
		{
			if (m_expressionsWithReportItemReferences.Count > 0)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionsWithReportItemReference in m_expressionsWithReportItemReferences)
				{
					expressionsWithReportItemReference.UpdateReportItemReferences(this);
				}
			}
			if (m_visibilitiesWithToggleToUpdate.Count > 0)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.Visibility item in m_visibilitiesWithToggleToUpdate)
				{
					item.UpdateToggleItemReference(this);
				}
			}
			if (m_reportItemsWithRepeatWithToUpdate.Count > 0)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem item2 in m_reportItemsWithRepeatWithToUpdate)
				{
					item2.UpdateRepeatWithReference(this);
				}
			}
			if (m_endUserSortWithTarget.Count > 0)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort item3 in m_endUserSortWithTarget)
				{
					item3.UpdateSortScopeAndTargetReference(this);
				}
			}
			m_lookupMap.Clear();
			m_aggregateMap.Clear();
			m_reportItemNameMap.Clear();
			m_variableNameMap.Clear();
			m_visibilitiesWithToggleToUpdate.Clear();
			m_reportItemsWithRepeatWithToUpdate.Clear();
			m_expressionsWithReportItemReferences.Clear();
			m_endUserSortWithTarget.Clear();
			m_scopeNameMap.Clear();
			m_scopeNamesToClone.Clear();
		}

		internal void AddSortTarget(string scopeName, Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope target)
		{
			Global.Tracer.Assert(!m_reportScopes.ContainsKey(scopeName), "(!m_reportScopes.ContainsKey(scopeName))");
			m_reportScopes.Add(scopeName, target);
		}

		internal bool TryGetNewSortTarget(string scopeName, out Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope target)
		{
			target = null;
			return m_reportScopes.TryGetValue(scopeName, out target);
		}

		internal void AddExpressionThatReferencesReportItems(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			m_expressionsWithReportItemReferences.Add(expression);
		}

		internal void AddVisibilityWithToggleToUpdate(Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility)
		{
			m_visibilitiesWithToggleToUpdate.Add(visibility);
		}

		internal void AddReportItemWithRepeatWithToUpdate(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			m_reportItemsWithRepeatWithToUpdate.Add(reportItem);
		}

		internal void AddEndUserSort(Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort endUserSort)
		{
			m_endUserSortWithTarget.Add(endUserSort);
		}

		internal void AddSubReport(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			m_report.SubReports.Add(subReport);
		}
	}
}
