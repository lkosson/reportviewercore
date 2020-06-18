using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
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
	internal class TablixMember : ReportHierarchyNode, IPersistable, IVisibilityOwner, IReferenceable
	{
		private class VisibilityState
		{
			internal DataRegionMemberInstance MemberInstance;

			internal bool CachedHiddenValue;

			internal bool HasCachedHidden;

			internal bool CachedDeepHiddenValue;

			internal bool HasCachedDeepHidden;

			internal bool CachedStartHiddenValue;

			internal bool HasCachedStartHidden;

			internal void Reset()
			{
				CachedHiddenValue = false;
				HasCachedHidden = false;
				CachedDeepHiddenValue = false;
				HasCachedDeepHidden = false;
				CachedStartHiddenValue = false;
				HasCachedStartHidden = false;
				MemberInstance = null;
			}
		}

		private TablixHeader m_tablixHeader;

		private TablixMemberList m_tablixMembers;

		private Visibility m_visibility;

		private PageBreakLocation m_propagatedPageBreakLocation;

		private bool m_keepTogether;

		private bool m_fixedData;

		private KeepWithGroup m_keepWithGroup;

		private bool m_repeatOnNewPage;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		private bool m_hideIfNoRows;

		[Reference]
		private List<TextBox> m_inScopeTextBoxes;

		[Reference]
		private IVisibilityOwner m_containingDynamicVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicRowVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicColumnVisibility;

		[NonSerialized]
		private bool m_keepTogetherSpecified;

		[NonSerialized]
		private TablixMember m_parentMember;

		[NonSerialized]
		private bool[] m_headerLevelHasStaticArray;

		[NonSerialized]
		private int m_headerLevel = -1;

		[NonSerialized]
		private bool m_isInnerMostMemberWithHeader;

		[NonSerialized]
		private bool m_hasStaticPeerWithHeader;

		[NonSerialized]
		private int m_resizedForLevel;

		[NonSerialized]
		private bool m_canHaveSpanDecreased;

		[NonSerialized]
		private int m_consecutiveZeroHeightDescendentCount;

		[NonSerialized]
		private int m_consecutiveZeroHeightAncestorCount;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private string m_senderUniqueName;

		[NonSerialized]
		private int? m_parentInstanceIndex;

		[NonSerialized]
		private bool? m_instanceHasRecursiveChildren;

		[NonSerialized]
		private IList<DataRegionMemberInstance> m_memberInstances;

		[NonSerialized]
		private TablixMemberExprHost m_exprHost;

		[NonSerialized]
		private IReportScopeInstance m_romScopeInstance;

		[NonSerialized]
		private List<VisibilityState> m_recursiveVisibilityCache;

		[NonSerialized]
		private VisibilityState m_nonRecursiveVisibilityCache;

		internal override string RdlElementName => "TablixMember";

		internal override HierarchyNodeList InnerHierarchy => m_tablixMembers;

		internal TablixMemberList SubMembers
		{
			get
			{
				return m_tablixMembers;
			}
			set
			{
				m_tablixMembers = value;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return m_visibility;
			}
			set
			{
				m_visibility = value;
			}
		}

		internal TablixMember ParentMember
		{
			get
			{
				return m_parentMember;
			}
			set
			{
				m_parentMember = value;
			}
		}

		internal bool HasStaticPeerWithHeader
		{
			get
			{
				return m_hasStaticPeerWithHeader;
			}
			set
			{
				m_hasStaticPeerWithHeader = value;
			}
		}

		internal PageBreakLocation PropagatedPageBreakLocation
		{
			get
			{
				return m_propagatedPageBreakLocation;
			}
			set
			{
				m_propagatedPageBreakLocation = value;
			}
		}

		internal TablixHeader TablixHeader
		{
			get
			{
				return m_tablixHeader;
			}
			set
			{
				m_tablixHeader = value;
			}
		}

		internal bool FixedData
		{
			get
			{
				return m_fixedData;
			}
			set
			{
				m_fixedData = value;
			}
		}

		internal bool RepeatOnNewPage
		{
			get
			{
				return m_repeatOnNewPage;
			}
			set
			{
				m_repeatOnNewPage = value;
			}
		}

		internal KeepWithGroup KeepWithGroup
		{
			get
			{
				return m_keepWithGroup;
			}
			set
			{
				m_keepWithGroup = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return m_keepTogether;
			}
			set
			{
				m_keepTogether = value;
			}
		}

		internal bool KeepTogetherSpecified
		{
			get
			{
				return m_keepTogetherSpecified;
			}
			set
			{
				m_keepTogetherSpecified = value;
			}
		}

		internal bool HideIfNoRows
		{
			get
			{
				return m_hideIfNoRows;
			}
			set
			{
				m_hideIfNoRows = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal override bool IsNonToggleableHiddenMember
		{
			get
			{
				if (m_visibility != null && m_visibility.Toggle == null && m_visibility.Hidden != null && m_visibility.Hidden.Type == ExpressionInfo.Types.Constant)
				{
					return m_visibility.Hidden.BoolValue;
				}
				return false;
			}
		}

		private bool WasResized => m_resizedForLevel > 0;

		internal bool CanHaveSpanDecreased
		{
			get
			{
				return m_canHaveSpanDecreased;
			}
			set
			{
				m_canHaveSpanDecreased = value;
			}
		}

		internal bool HasToggleableVisibility
		{
			get
			{
				if (m_visibility != null)
				{
					return m_visibility.Toggle != null;
				}
				return false;
			}
		}

		internal bool HasConditionalOrToggleableVisibility
		{
			get
			{
				if (m_visibility != null)
				{
					if (m_visibility.Toggle == null)
					{
						if (m_visibility.Hidden != null)
						{
							return m_visibility.Hidden.Type != ExpressionInfo.Types.Constant;
						}
						return false;
					}
					return true;
				}
				return false;
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

		internal bool IsInnerMostMemberWithHeader
		{
			get
			{
				return m_isInnerMostMemberWithHeader;
			}
			set
			{
				m_isInnerMostMemberWithHeader = value;
			}
		}

		internal override bool IsTablixMember => true;

		internal List<TextBox> InScopeTextBoxes => m_inScopeTextBoxes;

		public IReportScopeInstance ROMScopeInstance
		{
			get
			{
				return m_romScopeInstance;
			}
			set
			{
				m_romScopeInstance = value;
			}
		}

		public IVisibilityOwner ContainingDynamicVisibility
		{
			get
			{
				return m_containingDynamicVisibility;
			}
			set
			{
				m_containingDynamicVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicColumnVisibility
		{
			get
			{
				return m_containingDynamicColumnVisibility;
			}
			set
			{
				m_containingDynamicColumnVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicRowVisibility
		{
			get
			{
				return m_containingDynamicRowVisibility;
			}
			set
			{
				m_containingDynamicRowVisibility = value;
			}
		}

		public string SenderUniqueName
		{
			get
			{
				if (m_senderUniqueName == null && m_visibility != null)
				{
					TextBox toggleSender = m_visibility.ToggleSender;
					if (toggleSender != null)
					{
						if (toggleSender.RecursiveSender && m_visibility.RecursiveReceiver)
						{
							int value = GetRecursiveParentIndex().Value;
							if (value >= 0)
							{
								m_senderUniqueName = toggleSender.GetRecursiveUniqueName(value);
							}
						}
						else
						{
							m_senderUniqueName = toggleSender.UniqueName;
						}
					}
				}
				return m_senderUniqueName;
			}
		}

		internal int ConsecutiveZeroHeightDescendentCount
		{
			get
			{
				return m_consecutiveZeroHeightDescendentCount;
			}
			set
			{
				m_consecutiveZeroHeightDescendentCount = value;
			}
		}

		internal int ConsecutiveZeroHeightAncestorCount
		{
			get
			{
				return m_consecutiveZeroHeightAncestorCount;
			}
			set
			{
				m_consecutiveZeroHeightAncestorCount = value;
			}
		}

		internal bool InstanceHasRecursiveChildren
		{
			get
			{
				if (m_instanceHasRecursiveChildren.HasValue)
				{
					return m_instanceHasRecursiveChildren.Value;
				}
				return true;
			}
		}

		internal override List<ReportItem> MemberContentCollection
		{
			get
			{
				List<ReportItem> list = null;
				if (m_tablixHeader != null && m_tablixHeader.CellContents != null)
				{
					list = new List<ReportItem>((m_tablixHeader.AltCellContents == null) ? 1 : 2);
					list.Add(m_tablixHeader.CellContents);
					if (m_tablixHeader.AltCellContents != null)
					{
						list.Add(m_tablixHeader.AltCellContents);
					}
				}
				return list;
			}
		}

		private ToggleCascadeDirection ToggleCascadeDirection
		{
			get
			{
				if (base.IsColumn)
				{
					return ToggleCascadeDirection.Column;
				}
				return ToggleCascadeDirection.Row;
			}
		}

		internal TablixMember()
		{
		}

		internal TablixMember(int id, Tablix tablixDef)
			: base(id, tablixDef)
		{
		}

		internal override void TraverseMemberScopes(IRIFScopeVisitor visitor)
		{
			if (m_tablixHeader != null)
			{
				if (m_tablixHeader.CellContents != null)
				{
					m_tablixHeader.CellContents.TraverseScopes(visitor);
				}
				if (m_tablixHeader.AltCellContents != null)
				{
					m_tablixHeader.AltCellContents.TraverseScopes(visitor);
				}
			}
		}

		public bool ComputeHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			VisibilityState cachedVisibilityState = GetCachedVisibilityState(renderingContext.OdpContext);
			if (!cachedVisibilityState.HasCachedHidden)
			{
				cachedVisibilityState.HasCachedHidden = true;
				bool valueIsDeep = false;
				int? num = SetupParentRecursiveIndex(renderingContext.OdpContext);
				if (num.HasValue && num < 0 && IsRecursiveToggleReceiver())
				{
					valueIsDeep = true;
					cachedVisibilityState.CachedHiddenValue = false;
				}
				else
				{
					cachedVisibilityState.CachedHiddenValue = Visibility.ComputeHidden(this, renderingContext, direction, out valueIsDeep);
				}
				if (valueIsDeep)
				{
					cachedVisibilityState.HasCachedDeepHidden = true;
					cachedVisibilityState.CachedDeepHiddenValue = cachedVisibilityState.CachedHiddenValue;
				}
			}
			return cachedVisibilityState.CachedHiddenValue;
		}

		public bool ComputeDeepHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			VisibilityState cachedVisibilityState = GetCachedVisibilityState(renderingContext.OdpContext);
			if (!cachedVisibilityState.HasCachedDeepHidden)
			{
				direction = ToggleCascadeDirection;
				bool flag = false;
				flag = ((!cachedVisibilityState.HasCachedHidden) ? ComputeHidden(renderingContext, direction) : cachedVisibilityState.CachedHiddenValue);
				if (!cachedVisibilityState.HasCachedDeepHidden)
				{
					cachedVisibilityState.HasCachedDeepHidden = true;
					cachedVisibilityState.CachedDeepHiddenValue = Visibility.ComputeDeepHidden(flag, this, direction, renderingContext);
				}
			}
			return cachedVisibilityState.CachedDeepHiddenValue;
		}

		public bool ComputeToggleSenderDeepHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			bool flag = false;
			ToggleCascadeDirection toggleCascadeDirection = ToggleCascadeDirection;
			TablixMember recursiveMember = GetRecursiveMember();
			if (recursiveMember != null)
			{
				int? num = SetupParentRecursiveIndex(renderingContext.OdpContext);
				if (num.Value >= 0)
				{
					_ = recursiveMember.m_memberInstances[num.Value];
					int? parentInstanceIndex = recursiveMember.m_parentInstanceIndex;
					IList<DataRegionMemberInstance> memberInstances = recursiveMember.m_memberInstances;
					bool? instanceHasRecursiveChildren = recursiveMember.m_instanceHasRecursiveChildren;
					int instanceIndex = recursiveMember.InstancePathItem.InstanceIndex;
					recursiveMember.InstancePathItem.SetContext(num.Value);
					m_romScopeInstance.IsNewContext = true;
					recursiveMember.ResetVisibilityComputationCache();
					if (this != recursiveMember)
					{
						ResetVisibilityComputationCache();
					}
					flag |= m_visibility.ToggleSender.ComputeDeepHidden(renderingContext, toggleCascadeDirection);
					recursiveMember.InstancePathItem.SetContext(instanceIndex);
					m_romScopeInstance.IsNewContext = true;
					recursiveMember.m_parentInstanceIndex = parentInstanceIndex;
					recursiveMember.m_memberInstances = memberInstances;
					recursiveMember.m_instanceHasRecursiveChildren = instanceHasRecursiveChildren;
				}
			}
			return flag;
		}

		public bool ComputeStartHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			VisibilityState cachedVisibilityState = GetCachedVisibilityState(renderingContext.OdpContext);
			if (!cachedVisibilityState.HasCachedStartHidden)
			{
				cachedVisibilityState.HasCachedStartHidden = true;
				if (m_visibility == null || m_visibility.Hidden == null)
				{
					cachedVisibilityState.CachedStartHiddenValue = false;
				}
				else if (!m_visibility.Hidden.IsExpression)
				{
					cachedVisibilityState.CachedStartHiddenValue = m_visibility.Hidden.BoolValue;
				}
				else
				{
					cachedVisibilityState.CachedStartHiddenValue = EvaluateStartHidden(m_romScopeInstance, renderingContext.OdpContext);
				}
			}
			return cachedVisibilityState.CachedStartHiddenValue;
		}

		internal void ResetVisibilityComputationCache()
		{
			if (m_nonRecursiveVisibilityCache != null)
			{
				m_nonRecursiveVisibilityCache.Reset();
			}
			m_parentInstanceIndex = null;
			m_senderUniqueName = null;
		}

		protected override void DataGroupStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix, m_isColumn);
		}

		protected override int DataGroupEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix, m_isColumn);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			TablixMember tablixMember = (TablixMember)base.PublishClone(context, newContainingRegion);
			if (m_tablixHeader != null)
			{
				tablixMember.m_tablixHeader = (TablixHeader)m_tablixHeader.PublishClone(context, isClonedDynamic: false);
			}
			if (m_tablixMembers != null)
			{
				tablixMember.m_tablixMembers = new TablixMemberList(m_tablixMembers.Count);
				foreach (TablixMember tablixMember3 in m_tablixMembers)
				{
					TablixMember tablixMember2 = (TablixMember)tablixMember3.PublishClone(context, newContainingRegion);
					tablixMember2.ParentMember = tablixMember;
					tablixMember.m_tablixMembers.Add(tablixMember2);
				}
			}
			if (m_visibility != null)
			{
				tablixMember.m_visibility = (Visibility)m_visibility.PublishClone(context, isSubtotalMember: false);
			}
			return tablixMember;
		}

		internal TablixMember CreateAutomaticSubtotalClone(AutomaticSubtotalContext context, TablixMember parent, bool isDynamicTarget, out int aDynamicsRemoved, ref bool aAllWereDynamic)
		{
			TablixMember tablixMember = null;
			TablixMember tablixMember2 = null;
			aDynamicsRemoved = 0;
			int spanDifference = -1;
			if (m_grouping != null)
			{
				context.RegisterScopeName(m_grouping.Name);
			}
			if (isDynamicTarget || m_grouping == null || context.DynamicWithStaticPeerEncountered || (((m_isColumn && context.OriginalColumnCount > 1) || (!m_isColumn && context.OriginalRowCount > 1)) && (context.HasStaticPeerWithHeader(this, out spanDifference) || (m_parentMember.m_tablixMembers.Count > 1 && m_isInnerMostMemberWithHeader && !HasStaticAncestorWithOneMemberGenerations(this)) || (!m_isInnerMostMemberWithHeader && m_tablixMembers == null && !HasInnermostHeaderAncestorWithOneMemberGenerations(this)))))
			{
				tablixMember = (TablixMember)base.PublishClone(context, null, isSubtotal: true);
				tablixMember2 = tablixMember;
				tablixMember2.DataElementOutput = DataElementOutputTypes.NoOutput;
				tablixMember2.ParentMember = parent;
				if (parent != null)
				{
					tablixMember2.m_level = parent.m_level + 1;
				}
				else
				{
					Global.Tracer.Assert(tablixMember2.m_level == 0, "(currentClone.m_level == 0)");
				}
				if (m_tablixHeader != null)
				{
					tablixMember2.m_headerLevel = context.HeaderLevel;
					context.HeaderLevel++;
					tablixMember2.m_tablixHeader = (TablixHeader)m_tablixHeader.PublishClone(context, m_grouping != null);
				}
				if (m_grouping != null)
				{
					if (spanDifference > 0 || (context.HasStaticPeerWithHeader(this, out spanDifference) && spanDifference > 0))
					{
						if (m_isColumn)
						{
							tablixMember2.RowSpan -= spanDifference;
						}
						else
						{
							tablixMember2.ColSpan -= spanDifference;
						}
						tablixMember2.m_resizedForLevel = 1;
					}
					if (!context.DynamicWithStaticPeerEncountered)
					{
						tablixMember2.m_canHaveSpanDecreased = true;
					}
				}
				if (m_visibility != null)
				{
					tablixMember2.m_visibility = (Visibility)m_visibility.PublishClone(context, isDynamicTarget);
				}
				if (m_tablixMembers != null)
				{
					tablixMember2.m_tablixMembers = new TablixMemberList(m_tablixMembers.Count);
				}
			}
			else
			{
				if (m_tablixMembers != null)
				{
					tablixMember2 = parent;
					if (m_isColumn)
					{
						aDynamicsRemoved = base.RowSpan;
					}
					else
					{
						aDynamicsRemoved = base.ColSpan;
					}
				}
				if (m_tablixHeader != null)
				{
					context.HeaderLevel++;
				}
				if (m_tablixHeader != null)
				{
					TablixMember tablixMember3 = parent;
					while (tablixMember3.m_tablixHeader == null)
					{
						if (tablixMember3.m_parentMember != null)
						{
							tablixMember3 = tablixMember3.m_parentMember;
						}
					}
					if (tablixMember3.m_tablixHeader != null && tablixMember3.m_resizedForLevel < m_headerLevel)
					{
						if (m_isColumn)
						{
							tablixMember3.RowSpan += base.RowSpan;
							Global.Tracer.Assert(m_headerLevel > 0, "(this.m_headerLevel > 0)");
							tablixMember3.m_resizedForLevel = m_headerLevel + base.RowSpan - 1;
						}
						else
						{
							tablixMember3.ColSpan += base.ColSpan;
							Global.Tracer.Assert(m_headerLevel > 0, "(this.m_headerLevel > 0)");
							tablixMember3.m_resizedForLevel = m_headerLevel + base.ColSpan - 1;
						}
					}
				}
			}
			if (m_tablixMembers != null)
			{
				int num = int.MaxValue;
				bool aAllWereDynamic2 = true;
				int count = tablixMember2.m_tablixMembers.Count;
				foreach (TablixMember tablixMember7 in m_tablixMembers)
				{
					int aDynamicsRemoved2 = 0;
					TablixMember tablixMember5 = tablixMember7.CreateAutomaticSubtotalClone(context, tablixMember2, isDynamicTarget: false, out aDynamicsRemoved2, ref aAllWereDynamic2);
					if (tablixMember5 != null)
					{
						if (tablixMember7.m_grouping == null)
						{
							aAllWereDynamic2 = false;
							num = 0;
						}
						tablixMember2.m_tablixMembers.Add(tablixMember5);
					}
					num = Math.Min(num, aDynamicsRemoved2);
				}
				aDynamicsRemoved += num;
				aAllWereDynamic &= aAllWereDynamic2;
				if (tablixMember != null && (m_grouping == null || isDynamicTarget))
				{
					for (int i = count; i < tablixMember2.m_tablixMembers.Count; i++)
					{
						TablixMember tablixMember6 = tablixMember2.m_tablixMembers[i];
						if (!tablixMember6.m_canHaveSpanDecreased)
						{
							continue;
						}
						if (aAllWereDynamic2)
						{
							if (m_isColumn)
							{
								tablixMember6.RowSpan = 1;
							}
							else
							{
								tablixMember6.ColSpan = 1;
							}
						}
						else if (m_isColumn)
						{
							if (tablixMember6.RowSpan > 1)
							{
								tablixMember6.RowSpan -= num;
							}
						}
						else if (tablixMember6.ColSpan > 1)
						{
							tablixMember6.ColSpan -= num;
						}
						tablixMember6.m_canHaveSpanDecreased = false;
						tablixMember6.m_resizedForLevel = 1;
					}
				}
			}
			else
			{
				RowList rows = context.CurrentDataRegion.Rows;
				if (m_isColumn)
				{
					for (int j = 0; j < rows.Count; j++)
					{
						Cell value = (Cell)rows[j].Cells[context.CurrentIndex].PublishClone(context);
						context.CellLists[j].Add(value);
					}
					TablixColumn tablixColumn = (TablixColumn)((Tablix)context.CurrentDataRegion).TablixColumns[context.CurrentIndex].PublishClone(context);
					tablixColumn.ForAutoSubtotal = true;
					context.TablixColumns.Add(tablixColumn);
				}
				else
				{
					TablixRow tablixRow = (TablixRow)rows[context.CurrentIndex].PublishClone(context);
					tablixRow.ForAutoSubtotal = true;
					context.Rows.Add(tablixRow);
				}
				context.CurrentIndex++;
			}
			if (tablixMember != null && tablixMember.m_tablixMembers != null && tablixMember.m_tablixMembers.Count == 0)
			{
				tablixMember.m_tablixMembers = null;
			}
			return tablixMember;
		}

		private bool HasStaticAncestorWithOneMemberGenerations(TablixMember member)
		{
			if (member.ParentMember != null)
			{
				if (member.ParentMember.Grouping == null)
				{
					if (member.ParentMember.SubMembers.Count == 1)
					{
						return true;
					}
				}
				else if (member.ParentMember.SubMembers.Count == 1)
				{
					return HasStaticAncestorWithOneMemberGenerations(member.ParentMember);
				}
			}
			return false;
		}

		private bool HasInnermostHeaderAncestorWithOneMemberGenerations(TablixMember member)
		{
			if (member.ParentMember != null)
			{
				if (member.ParentMember.m_isInnerMostMemberWithHeader)
				{
					if (member.ParentMember.SubMembers.Count == 1)
					{
						return true;
					}
				}
				else if (member.ParentMember.SubMembers.Count == 1)
				{
					return HasInnermostHeaderAncestorWithOneMemberGenerations(member.ParentMember);
				}
			}
			return false;
		}

		internal override bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			context.RegisterMemberReportItems(this, firstPass: true, restrictive);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, registerVisibilityToggle: false);
			}
			bool result = base.InnerInitialize(context, restrictive);
			if (m_tablixHeader != null)
			{
				m_tablixHeader.Initialize(context, m_isColumn, WasResized);
			}
			context.UnRegisterMemberReportItems(this, firstPass: true, restrictive);
			return result;
		}

		internal override bool Initialize(InitializationContext context, bool restrictive)
		{
			if (m_visibility != null)
			{
				string objectName = null;
				Microsoft.ReportingServices.ReportProcessing.ObjectType objectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix;
				if (m_grouping != null)
				{
					objectName = context.ObjectName;
					objectType = context.ObjectType;
					context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping;
					context.ObjectName = m_grouping.Name;
				}
				VisibilityToggleInfo visibilityToggleInfo = m_visibility.RegisterVisibilityToggle(context);
				if (visibilityToggleInfo != null)
				{
					visibilityToggleInfo.IsTablixMember = true;
				}
				if (m_grouping != null)
				{
					if (visibilityToggleInfo != null)
					{
						visibilityToggleInfo.GroupName = m_grouping.Name;
					}
					context.ObjectName = objectName;
					context.ObjectType = objectType;
				}
			}
			bool flag = context.RegisterVisibility(m_visibility, this);
			if (!m_hideIfNoRows && m_grouping == null)
			{
				((Tablix)context.GetCurrentDataRegion()).HideStaticsIfNoRows = false;
			}
			if (!m_isColumn)
			{
				Tablix.ValidateKeepWithGroup(m_tablixMembers, context);
			}
			bool flag2 = base.Initialize(context, restrictive);
			if (m_keepWithGroup != 0 && flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroupOnDynamicTablixMember, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", KeepWithGroup.None.ToString());
				m_keepWithGroup = KeepWithGroup.None;
			}
			DataRendererInitialize(context);
			if (flag)
			{
				context.UnRegisterVisibility(m_visibility, this);
			}
			return flag2;
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			if (m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				if (m_grouping != null || (m_tablixHeader != null && m_tablixHeader.CellContents != null))
				{
					m_dataElementOutput = DataElementOutputTypes.Output;
				}
				else
				{
					m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
				}
			}
			string defaultName = string.Empty;
			if (m_grouping != null)
			{
				defaultName = m_grouping.Name + "_Collection";
			}
			else if (m_tablixHeader != null && m_tablixHeader.CellContents != null)
			{
				defaultName = m_tablixHeader.CellContents.DataElementName;
			}
			Microsoft.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref m_dataElementName, defaultName, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal override bool PreInitializeDataMember(InitializationContext context)
		{
			if (m_tablixHeader != null)
			{
				if (WasResized)
				{
					double headerSize = context.GetHeaderSize(m_isColumn, m_headerLevel, m_isColumn ? m_rowSpan : m_colSpan);
					m_tablixHeader.SizeValue = Math.Round(headerSize, 10);
					m_tablixHeader.Size = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertSize(headerSize);
				}
				else if (m_headerLevel > -1)
				{
					context.ValidateHeaderSize(m_tablixHeader.SizeValue, m_headerLevel, m_isColumn ? m_rowSpan : m_colSpan, m_isColumn, m_memberCellIndex);
				}
			}
			bool result = context.RegisterVisibility(m_visibility, this);
			context.RegisterMemberReportItems(this, firstPass: false);
			return result;
		}

		internal override void PostInitializeDataMember(InitializationContext context, bool registeredVisibility)
		{
			context.UnRegisterMemberReportItems(this, firstPass: false);
			if (registeredVisibility)
			{
				context.UnRegisterVisibility(m_visibility, this);
			}
			base.PostInitializeDataMember(context, registeredVisibility);
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (m_tablixHeader != null)
			{
				if (m_tablixHeader.CellContents != null)
				{
					m_tablixHeader.CellContents.InitializeRVDirectionDependentItems(context);
				}
				if (m_tablixHeader.AltCellContents != null)
				{
					m_tablixHeader.AltCellContents.InitializeRVDirectionDependentItems(context);
				}
			}
		}

		internal override void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			if (m_tablixHeader != null)
			{
				if (m_tablixHeader.CellContents != null)
				{
					m_tablixHeader.CellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
				}
				if (m_tablixHeader.AltCellContents != null)
				{
					m_tablixHeader.AltCellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
				}
			}
		}

		internal void ValidateTablixMemberForBanding(PublishingContextStruct context, out bool isdynamic)
		{
			isdynamic = false;
			SetIgnoredPropertiesForBandingToDefault(context);
			if (!base.IsStatic)
			{
				isdynamic = true;
				if (Visibility != null && Visibility.IsToggleReceiver)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandShouldNotBeTogglable, Severity.Error, context.ObjectType, context.ObjectName, base.Grouping.Name.MarkAsModelInfo());
				}
				if (base.Grouping.PageBreak != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandPageBreakIsSet, Severity.Error, context.ObjectType, context.ObjectName, base.Grouping.Name.MarkAsModelInfo());
				}
			}
		}

		internal void SetIgnoredPropertiesForBandingToDefault(PublishingContextStruct context)
		{
			if (base.CustomProperties != null)
			{
				base.CustomProperties = null;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "CustomProperties");
			}
			if (FixedData)
			{
				FixedData = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedData");
			}
			if (HideIfNoRows)
			{
				HideIfNoRows = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "HideIfNoRows");
			}
			if (KeepWithGroup != 0)
			{
				KeepWithGroup = KeepWithGroup.None;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "KeepWithGroup");
			}
			if (RepeatOnNewPage)
			{
				RepeatOnNewPage = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "RepeatOnNewPage");
			}
			if (!KeepTogether)
			{
				KeepTogether = true;
				if (KeepTogetherSpecified)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsBandKeepTogetherIgnored, Severity.Warning, context.ObjectType, context.ObjectName, "KeepTogether");
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TablixHeader, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixHeader));
			list.Add(new MemberInfo(MemberName.TablixMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember));
			list.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility));
			list.Add(new MemberInfo(MemberName.PropagatedPageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.FixedData, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepWithGroup, Token.Enum));
			list.Add(new MemberInfo(MemberName.RepeatOnNewPage, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.HideIfNoRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeTextBoxes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.ContainingDynamicVisibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicRowVisibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicColumnVisibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TablixHeader:
					writer.Write(m_tablixHeader);
					break;
				case MemberName.TablixMembers:
					writer.Write(m_tablixMembers);
					break;
				case MemberName.Visibility:
					writer.Write(m_visibility);
					break;
				case MemberName.PropagatedPageBreakLocation:
					writer.WriteEnum((int)m_propagatedPageBreakLocation);
					break;
				case MemberName.FixedData:
					writer.Write(m_fixedData);
					break;
				case MemberName.KeepWithGroup:
					writer.WriteEnum((int)m_keepWithGroup);
					break;
				case MemberName.RepeatOnNewPage:
					writer.Write(m_repeatOnNewPage);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				case MemberName.HideIfNoRows:
					writer.Write(m_hideIfNoRows);
					break;
				case MemberName.KeepTogether:
					writer.Write(m_keepTogether);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(m_inScopeTextBoxes);
					break;
				case MemberName.ContainingDynamicVisibility:
					writer.WriteReference(m_containingDynamicVisibility);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					writer.WriteReference(m_containingDynamicRowVisibility);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					writer.WriteReference(m_containingDynamicColumnVisibility);
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
				case MemberName.TablixHeader:
					m_tablixHeader = (TablixHeader)reader.ReadRIFObject();
					break;
				case MemberName.TablixMembers:
					m_tablixMembers = reader.ReadListOfRIFObjects<TablixMemberList>();
					break;
				case MemberName.Visibility:
					m_visibility = (Visibility)reader.ReadRIFObject();
					break;
				case MemberName.PropagatedPageBreakLocation:
					m_propagatedPageBreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.FixedData:
					m_fixedData = reader.ReadBoolean();
					break;
				case MemberName.KeepWithGroup:
					m_keepWithGroup = (KeepWithGroup)reader.ReadEnum();
					break;
				case MemberName.RepeatOnNewPage:
					m_repeatOnNewPage = reader.ReadBoolean();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.HideIfNoRows:
					m_hideIfNoRows = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxes:
					m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.ContainingDynamicVisibility:
					m_containingDynamicVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					m_containingDynamicRowVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					m_containingDynamicColumnVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.InScopeTextBoxes:
					if (m_inScopeTextBoxes == null)
					{
						m_inScopeTextBoxes = new List<TextBox>();
					}
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is TextBox);
					m_inScopeTextBoxes.Add((TextBox)referenceableItems[item.RefID]);
					break;
				case MemberName.ContainingDynamicVisibility:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value3))
					{
						m_containingDynamicVisibility = (value3 as IVisibilityOwner);
					}
					break;
				}
				case MemberName.ContainingDynamicRowVisibility:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value4))
					{
						m_containingDynamicRowVisibility = (value4 as IVisibilityOwner);
					}
					break;
				}
				case MemberName.ContainingDynamicColumnVisibility:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value2))
					{
						m_containingDynamicColumnVisibility = (value2 as IVisibilityOwner);
					}
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
				m_exprHost = (TablixMemberExprHost)memberExprHost;
				m_exprHost.SetReportObjectModel(reportObjectModel);
				MemberNodeSetExprHost(m_exprHost, reportObjectModel);
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (m_tablixHeader != null && m_tablixHeader.CellContents != null)
			{
				reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(m_tablixHeader.CellContents, traverseDataRegions);
				if (m_tablixHeader.AltCellContents != null)
				{
					reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(m_tablixHeader.AltCellContents, traverseDataRegions);
				}
			}
		}

		internal override void MoveNextForUserSort(OnDemandProcessingContext odpContext)
		{
			base.MoveNextForUserSort(odpContext);
			ResetTextBoxImpls(odpContext);
		}

		protected override void AddInScopeTextBox(TextBox textbox)
		{
			if (m_inScopeTextBoxes == null)
			{
				m_inScopeTextBoxes = new List<TextBox>();
			}
			m_inScopeTextBoxes.Add(textbox);
		}

		internal override void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			if (m_inScopeTextBoxes != null)
			{
				for (int i = 0; i < m_inScopeTextBoxes.Count; i++)
				{
					m_inScopeTextBoxes[i].ResetTextBoxImpl(context);
				}
			}
		}

		internal bool EvaluateStartHidden(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			string objectName = base.IsStatic ? m_dataRegionDef.Name : m_grouping.Name;
			return context.ReportRuntime.EvaluateStartHiddenExpression(m_visibility, m_exprHost, Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix, objectName);
		}

		internal override void SetMemberInstances(IList<DataRegionMemberInstance> memberInstances)
		{
			m_memberInstances = memberInstances;
		}

		internal override void SetRecursiveParentIndex(int parentInstanceIndex)
		{
			if (!m_parentInstanceIndex.HasValue || parentInstanceIndex != m_parentInstanceIndex)
			{
				m_senderUniqueName = null;
				m_parentInstanceIndex = parentInstanceIndex;
			}
		}

		internal override void SetInstanceHasRecursiveChildren(bool? hasRecursiveChildren)
		{
			m_instanceHasRecursiveChildren = hasRecursiveChildren;
		}

		private int? SetupParentRecursiveIndex(OnDemandProcessingContext odpContext)
		{
			if (IsRecursive())
			{
				if (!m_parentInstanceIndex.HasValue)
				{
					odpContext.SetupContext(this, m_romScopeInstance);
				}
			}
			else if (IsToggleableChildOfRecursive())
			{
				m_parentInstanceIndex = m_visibility.RecursiveMember.SetupParentRecursiveIndex(odpContext);
			}
			return m_parentInstanceIndex;
		}

		private VisibilityState GetCachedVisibilityState(OnDemandProcessingContext odpContext)
		{
			return GetCachedVisibilityState(odpContext, int.MinValue);
		}

		private VisibilityState GetCachedVisibilityState(OnDemandProcessingContext odpContext, int memberIndex)
		{
			TablixMember recursiveMember = GetRecursiveMember();
			if (recursiveMember != null && m_visibility != null && m_visibility.RecursiveReceiver)
			{
				if (memberIndex == int.MinValue)
				{
					if (!recursiveMember.m_parentInstanceIndex.HasValue)
					{
						recursiveMember.SetupParentRecursiveIndex(odpContext);
					}
					memberIndex = recursiveMember.CurrentMemberIndex;
				}
				DataRegionMemberInstance dataRegionMemberInstance = recursiveMember.m_memberInstances[memberIndex];
				int recursiveLevel = dataRegionMemberInstance.RecursiveLevel;
				if (m_recursiveVisibilityCache == null)
				{
					m_recursiveVisibilityCache = new List<VisibilityState>();
				}
				VisibilityState visibilityState = null;
				if (recursiveLevel >= m_recursiveVisibilityCache.Count)
				{
					for (int i = m_recursiveVisibilityCache.Count; i <= recursiveLevel; i++)
					{
						visibilityState = new VisibilityState();
						m_recursiveVisibilityCache.Add(visibilityState);
					}
				}
				else
				{
					visibilityState = m_recursiveVisibilityCache[recursiveLevel];
				}
				if (visibilityState.MemberInstance != dataRegionMemberInstance)
				{
					visibilityState.Reset();
					visibilityState.MemberInstance = dataRegionMemberInstance;
				}
				return visibilityState;
			}
			if (m_nonRecursiveVisibilityCache == null)
			{
				m_nonRecursiveVisibilityCache = new VisibilityState();
			}
			else if (m_romScopeInstance != null && m_romScopeInstance.IsNewContext)
			{
				m_nonRecursiveVisibilityCache.Reset();
			}
			return m_nonRecursiveVisibilityCache;
		}

		private TablixMember GetRecursiveMember()
		{
			TablixMember result = null;
			if (IsRecursive())
			{
				result = this;
			}
			else if (IsToggleableChildOfRecursive())
			{
				result = m_visibility.RecursiveMember;
			}
			return result;
		}

		private int? GetRecursiveParentIndex()
		{
			return m_parentInstanceIndex;
		}

		private bool IsRecursive()
		{
			if (m_grouping != null)
			{
				return m_grouping.Parent != null;
			}
			return false;
		}

		private bool IsToggleableChildOfRecursive()
		{
			if (m_visibility != null)
			{
				return m_visibility.RecursiveMember != null;
			}
			return false;
		}

		private bool IsRecursiveToggleReceiver()
		{
			if (m_visibility != null && m_visibility.Toggle != null)
			{
				return m_visibility.RecursiveReceiver;
			}
			return false;
		}
	}
}
