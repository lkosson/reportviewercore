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
	internal sealed class TextBox : ReportItem, IActionOwner, IPersistable, IInScopeEventSource, IReferenceable, IGloballyReferenceable, IGlobalIDOwner
	{
		private List<Paragraph> m_paragraphs;

		private Action m_action;

		private bool m_canGrow;

		private bool m_canShrink;

		private string m_hideDuplicates;

		private bool m_isToggle;

		private ExpressionInfo m_initialToggleState;

		private bool m_valueReferenced;

		private bool m_textRunValueReferenced;

		private bool m_recursiveSender;

		private bool m_hasNonRecursiveSender;

		[Reference]
		private TablixMember m_recursiveMember;

		private bool m_dataElementStyleAttribute = true;

		private bool m_hasValue;

		private bool m_hasExpressionBasedValue;

		private bool m_keepTogether;

		private bool m_canScrollVertically;

		[Reference]
		private GroupingList m_containingScopes;

		private EndUserSort m_userSort;

		private bool m_isTablixCellScope;

		private int m_sequenceID = -1;

		private bool m_isSimple;

		[NonSerialized]
		private InitializationContext.ScopeChainInfo m_scopeChainInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private bool m_isSubReportTopLevelScope;

		[NonSerialized]
		private bool m_overrideReportDataElementStyle;

		[NonSerialized]
		private string m_textboxScope;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private Microsoft.ReportingServices.RdlExpressions.VariantResult m_oldResult;

		[NonSerialized]
		private bool m_hasOldResult;

		[NonSerialized]
		private TextBoxExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private TextBoxImpl m_textBoxImpl;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Textbox;

		internal List<Paragraph> Paragraphs
		{
			get
			{
				return m_paragraphs;
			}
			set
			{
				m_paragraphs = value;
			}
		}

		internal bool CanScrollVertically
		{
			get
			{
				return m_canScrollVertically;
			}
			set
			{
				m_canScrollVertically = value;
			}
		}

		internal bool CanGrow
		{
			get
			{
				return m_canGrow;
			}
			set
			{
				m_canGrow = value;
			}
		}

		internal bool CanShrink
		{
			get
			{
				return m_canShrink;
			}
			set
			{
				m_canShrink = value;
			}
		}

		internal string HideDuplicates
		{
			get
			{
				return m_hideDuplicates;
			}
			set
			{
				m_hideDuplicates = value;
			}
		}

		internal bool IsToggle
		{
			get
			{
				return m_isToggle;
			}
			set
			{
				m_isToggle = value;
			}
		}

		internal ExpressionInfo InitialToggleState
		{
			get
			{
				return m_initialToggleState;
			}
			set
			{
				m_initialToggleState = value;
			}
		}

		internal bool RecursiveSender
		{
			get
			{
				return m_recursiveSender;
			}
			set
			{
				m_recursiveSender = value;
			}
		}

		internal bool HasNonRecursiveSender
		{
			get
			{
				return m_hasNonRecursiveSender;
			}
			set
			{
				m_hasNonRecursiveSender = value;
			}
		}

		internal TablixMember RecursiveMember
		{
			get
			{
				return m_recursiveMember;
			}
			set
			{
				m_recursiveMember = value;
			}
		}

		internal bool ValueReferenced
		{
			get
			{
				return m_valueReferenced;
			}
			set
			{
				m_valueReferenced = value;
			}
		}

		internal TextBoxExprHost TextBoxExprHost => m_exprHost;

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

		internal EndUserSort UserSort
		{
			get
			{
				return m_userSort;
			}
			set
			{
				m_userSort = value;
			}
		}

		internal bool OverrideReportDataElementStyle
		{
			get
			{
				return m_overrideReportDataElementStyle;
			}
			set
			{
				m_overrideReportDataElementStyle = value;
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

		internal bool HasExpressionBasedValue => m_hasExpressionBasedValue;

		internal bool HasValue => m_hasValue;

		internal bool IsSimple => m_isSimple;

		internal override DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				if (DataElementOutputTypes.Auto == m_dataElementOutput && HasExpressionBasedValue)
				{
					return DataElementOutputTypes.Output;
				}
				return DataElementOutputTypes.NoOutput;
			}
		}

		internal Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal bool TextRunValueReferenced
		{
			get
			{
				return m_textRunValueReferenced;
			}
			set
			{
				m_textRunValueReferenced = value;
			}
		}

		internal int SequenceID
		{
			get
			{
				return m_sequenceID;
			}
			set
			{
				m_sequenceID = value;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IInScopeEventSource.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Textbox;

		string IInScopeEventSource.Name => m_name;

		ReportItem IInScopeEventSource.Parent => m_parent;

		EndUserSort IInScopeEventSource.UserSort => m_userSort;

		GroupingList IInScopeEventSource.ContainingScopes
		{
			get
			{
				return m_containingScopes;
			}
			set
			{
				m_containingScopes = value;
			}
		}

		internal GroupingList ContainingScopes => m_containingScopes;

		string IInScopeEventSource.Scope
		{
			get
			{
				return m_textboxScope;
			}
			set
			{
				m_textboxScope = value;
			}
		}

		bool IInScopeEventSource.IsTablixCellScope
		{
			get
			{
				return m_isTablixCellScope;
			}
			set
			{
				m_isTablixCellScope = value;
			}
		}

		bool IInScopeEventSource.IsDetailScope
		{
			get
			{
				return m_isDetailScope;
			}
			set
			{
				m_isDetailScope = value;
			}
		}

		bool IInScopeEventSource.IsSubReportTopLevelScope
		{
			get
			{
				return m_isSubReportTopLevelScope;
			}
			set
			{
				m_isSubReportTopLevelScope = value;
			}
		}

		InitializationContext.ScopeChainInfo IInScopeEventSource.ScopeChainInfo
		{
			get
			{
				return m_scopeChainInfo;
			}
			set
			{
				m_scopeChainInfo = value;
			}
		}

		internal TextBox(ReportItem parent)
			: base(parent)
		{
		}

		internal TextBox(int id, ReportItem parent)
			: base(id, parent)
		{
			m_paragraphs = new List<Paragraph>();
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.TextBoxStart(m_name);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			bool flag = context.RegisterVisibility(m_visibility, this);
			context.RegisterTextBoxInScope(this);
			if (m_paragraphs != null)
			{
				foreach (Paragraph paragraph in m_paragraphs)
				{
					m_hasValue |= paragraph.Initialize(context, out bool aHasExpressionBasedValue);
					m_hasExpressionBasedValue |= aHasExpressionBasedValue;
				}
			}
			if (m_paragraphs.Count == 1)
			{
				m_isSimple = m_paragraphs[0].DetermineSimplicity();
			}
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_initialToggleState != null)
			{
				m_initialToggleState.Initialize("InitialState", context);
				context.ExprHostBuilder.TextBoxToggleImageInitialState(m_initialToggleState);
			}
			if (m_userSort != null)
			{
				context.RegisterSortEventSource(this);
			}
			if (m_hideDuplicates != null)
			{
				context.ValidateHideDuplicateScope(m_hideDuplicates, this);
			}
			context.RegisterToggleItem(this);
			if (flag)
			{
				context.UnRegisterVisibility(m_visibility, this);
			}
			base.ExprHostID = context.ExprHostBuilder.TextBoxEnd();
			return true;
		}

		InScopeSortFilterHashtable IInScopeEventSource.GetSortFiltersInScope(bool create, bool inDetail)
		{
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			ReportItem parent = ((IInScopeEventSource)this).Parent;
			if (inDetail)
			{
				while (parent != null && !parent.IsDataRegion)
				{
					parent = parent.Parent;
				}
			}
			else
			{
				while (parent != null && !(parent is Report))
				{
					parent = parent.Parent;
				}
			}
			Global.Tracer.Assert(parent.IsDataRegion || parent is Report, "(parent.IsDataRegion || parent is Report)");
			if (parent is Report)
			{
				Report report = (Report)parent;
				if (((IInScopeEventSource)this).UserSort.SortExpressionScope == null)
				{
					if (report.DetailSortFiltersInScope == null && create)
					{
						report.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					return report.DetailSortFiltersInScope;
				}
				if (report.NonDetailSortFiltersInScope == null && create)
				{
					report.NonDetailSortFiltersInScope = new InScopeSortFilterHashtable();
				}
				return report.NonDetailSortFiltersInScope;
			}
			Global.Tracer.Assert(((IInScopeEventSource)this).UserSort.SortExpressionScope == null, "(null == eventSource.UserSort.SortExpressionScope)");
			DataRegion dataRegion = (DataRegion)parent;
			if (dataRegion.DetailSortFiltersInScope == null && create)
			{
				dataRegion.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
			}
			return dataRegion.DetailSortFiltersInScope;
		}

		List<int> IInScopeEventSource.GetPeerSortFilters(bool create)
		{
			EndUserSort userSort = ((IInScopeEventSource)this).UserSort;
			if (userSort == null)
			{
				return null;
			}
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			List<int> list = null;
			if (((IInScopeEventSource)this).ContainingScopes == null || ((IInScopeEventSource)this).ContainingScopes.Count == 0 || ((IInScopeEventSource)this).IsSubReportTopLevelScope)
			{
				inScopeSortFilterHashtable = ((IInScopeEventSource)this).GetSortFiltersInScope(create, inDetail: false);
			}
			else
			{
				Grouping lastEntry = ((IInScopeEventSource)this).ContainingScopes.LastEntry;
				if (lastEntry == null)
				{
					inScopeSortFilterHashtable = ((IInScopeEventSource)this).GetSortFiltersInScope(create, inDetail: true);
				}
				else if (userSort.SortExpressionScope == null)
				{
					if (lastEntry.DetailSortFiltersInScope == null && create)
					{
						lastEntry.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					inScopeSortFilterHashtable = lastEntry.DetailSortFiltersInScope;
				}
				else
				{
					if (lastEntry.NonDetailSortFiltersInScope == null && create)
					{
						lastEntry.NonDetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					inScopeSortFilterHashtable = lastEntry.NonDetailSortFiltersInScope;
				}
			}
			if (inScopeSortFilterHashtable != null)
			{
				int num = (userSort.SortExpressionScope == null) ? userSort.SortTarget.ID : userSort.SortExpressionScope.ID;
				list = inScopeSortFilterHashtable[num];
				if (list == null && create)
				{
					list = new List<int>();
					inScopeSortFilterHashtable.Add(num, list);
				}
			}
			return list;
		}

		internal string GetRecursiveUniqueName(int parentInstanceIndex)
		{
			return InstancePathItem.GenerateUniqueNameString(base.ID, InstancePath, parentInstanceIndex);
		}

		internal bool EvaluateIsToggle(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			bool flag = IsToggle;
			if (flag && RecursiveSender && !HasNonRecursiveSender)
			{
				if (m_recursiveMember != null)
				{
					context.SetupContext(this, romInstance);
					return m_recursiveMember.InstanceHasRecursiveChildren;
				}
				flag = true;
			}
			return flag;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			if (!m_overrideReportDataElementStyle)
			{
				m_dataElementStyleAttribute = context.ReportDataElementStyleAttribute;
			}
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (m_userSort != null)
			{
				context.ProcessSortEventSource(this);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TextBox textBox = (TextBox)base.PublishClone(context);
			textBox.m_sequenceID = context.GenerateTextboxSequenceID();
			if (m_paragraphs != null)
			{
				textBox.m_paragraphs = new List<Paragraph>(m_paragraphs.Count);
				foreach (Paragraph paragraph2 in m_paragraphs)
				{
					Paragraph paragraph = (Paragraph)paragraph2.PublishClone(context);
					paragraph.TextBox = textBox;
					textBox.m_paragraphs.Add(paragraph);
				}
			}
			if (m_hideDuplicates != null)
			{
				textBox.m_hideDuplicates = context.GetNewScopeName(m_hideDuplicates);
			}
			if (m_action != null)
			{
				textBox.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_initialToggleState != null)
			{
				textBox.m_initialToggleState = (ExpressionInfo)m_initialToggleState.PublishClone(context);
			}
			if (m_userSort != null)
			{
				textBox.m_userSort = (EndUserSort)m_userSort.PublishClone(context);
			}
			return textBox;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Paragraphs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph));
			list.Add(new MemberInfo(MemberName.CanGrow, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CanShrink, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HideDuplicates, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.IsToggle, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InitialToggleState, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ValueReferenced, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RecursiveSender, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ContainingScopes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.UserSort, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.EndUserSort));
			list.Add(new MemberInfo(MemberName.IsTablixCellScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsSubReportTopLevelScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.SequenceID, Token.Int32));
			list.Add(new MemberInfo(MemberName.RecursiveMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember, Token.Reference));
			list.Add(new MemberInfo(MemberName.HasExpressionBasedValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsSimple, Token.Boolean));
			list.Add(new MemberInfo(MemberName.TextRunValueReferenced, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasNonRecursiveSender, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CanScrollVertically, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Paragraphs:
					writer.Write(m_paragraphs);
					break;
				case MemberName.CanScrollVertically:
					writer.Write(m_canScrollVertically);
					break;
				case MemberName.CanGrow:
					writer.Write(m_canGrow);
					break;
				case MemberName.CanShrink:
					writer.Write(m_canShrink);
					break;
				case MemberName.HideDuplicates:
					writer.Write(m_hideDuplicates);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.IsToggle:
					writer.Write(m_isToggle);
					break;
				case MemberName.InitialToggleState:
					writer.Write(m_initialToggleState);
					break;
				case MemberName.ValueReferenced:
					writer.Write(m_valueReferenced);
					break;
				case MemberName.TextRunValueReferenced:
					writer.Write(m_textRunValueReferenced);
					break;
				case MemberName.RecursiveSender:
					writer.Write(m_recursiveSender);
					break;
				case MemberName.DataElementStyleAttribute:
					writer.Write(m_dataElementStyleAttribute);
					break;
				case MemberName.ContainingScopes:
					writer.WriteListOfReferences(m_containingScopes);
					break;
				case MemberName.UserSort:
					writer.Write(m_userSort);
					break;
				case MemberName.IsTablixCellScope:
					writer.Write(m_isTablixCellScope);
					break;
				case MemberName.IsSubReportTopLevelScope:
					writer.Write(m_isSubReportTopLevelScope);
					break;
				case MemberName.RecursiveMember:
					writer.WriteReference(m_recursiveMember);
					break;
				case MemberName.KeepTogether:
					writer.Write(m_keepTogether);
					break;
				case MemberName.SequenceID:
					writer.Write(m_sequenceID);
					break;
				case MemberName.HasExpressionBasedValue:
					writer.Write(m_hasExpressionBasedValue);
					break;
				case MemberName.HasValue:
					writer.Write(m_hasValue);
					break;
				case MemberName.IsSimple:
					writer.Write(m_isSimple);
					break;
				case MemberName.HasNonRecursiveSender:
					writer.Write(m_hasNonRecursiveSender);
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
				case MemberName.Paragraphs:
					m_paragraphs = reader.ReadGenericListOfRIFObjects<Paragraph>();
					break;
				case MemberName.Value:
				{
					TextRun orCreateSingleTextRun = GetOrCreateSingleTextRun(reader);
					ExpressionInfo expressionInfo = (ExpressionInfo)reader.ReadRIFObject();
					m_hasValue = true;
					m_hasExpressionBasedValue = expressionInfo.IsExpression;
					orCreateSingleTextRun.Value = expressionInfo;
					if (m_styleClass != null)
					{
						orCreateSingleTextRun.Paragraph.StyleClass = new ParagraphFilteredStyle(m_styleClass);
						orCreateSingleTextRun.StyleClass = new TextRunFilteredStyle(m_styleClass);
						m_styleClass = new TextBoxFilteredStyle(m_styleClass);
					}
					break;
				}
				case MemberName.CanScrollVertically:
					m_canScrollVertically = reader.ReadBoolean();
					break;
				case MemberName.CanGrow:
					m_canGrow = reader.ReadBoolean();
					break;
				case MemberName.CanShrink:
					m_canShrink = reader.ReadBoolean();
					break;
				case MemberName.HideDuplicates:
					m_hideDuplicates = reader.ReadString();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.IsToggle:
					m_isToggle = reader.ReadBoolean();
					break;
				case MemberName.InitialToggleState:
					m_initialToggleState = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ValueReferenced:
					m_valueReferenced = reader.ReadBoolean();
					break;
				case MemberName.TextRunValueReferenced:
					m_textRunValueReferenced = reader.ReadBoolean();
					break;
				case MemberName.RecursiveSender:
					m_recursiveSender = reader.ReadBoolean();
					break;
				case MemberName.DataElementStyleAttribute:
					m_dataElementStyleAttribute = reader.ReadBoolean();
					break;
				case MemberName.ContainingScopes:
					if (reader.ReadListOfReferencesNoResolution(this) == 0)
					{
						m_containingScopes = new GroupingList();
					}
					break;
				case MemberName.UserSort:
					m_userSort = (EndUserSort)reader.ReadRIFObject();
					break;
				case MemberName.IsTablixCellScope:
					m_isTablixCellScope = reader.ReadBoolean();
					break;
				case MemberName.IsSubReportTopLevelScope:
					m_isSubReportTopLevelScope = reader.ReadBoolean();
					break;
				case MemberName.DataType:
					GetOrCreateSingleTextRun(reader).DataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.KeepTogether:
					m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.SequenceID:
					m_sequenceID = reader.ReadInt32();
					break;
				case MemberName.RecursiveMember:
					m_recursiveMember = reader.ReadReference<TablixMember>(this);
					break;
				case MemberName.HasExpressionBasedValue:
					m_hasExpressionBasedValue = reader.ReadBoolean();
					break;
				case MemberName.HasValue:
					m_hasValue = reader.ReadBoolean();
					break;
				case MemberName.IsSimple:
					m_isSimple = reader.ReadBoolean();
					break;
				case MemberName.HasNonRecursiveSender:
					m_hasNonRecursiveSender = reader.ReadBoolean();
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
				case MemberName.ContainingScopes:
					if (m_containingScopes == null)
					{
						m_containingScopes = new GroupingList();
					}
					if (item.RefID != -2)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is Grouping);
						Global.Tracer.Assert(!m_containingScopes.Contains((Grouping)referenceableItems[item.RefID]));
						m_containingScopes.Add((Grouping)referenceableItems[item.RefID]);
					}
					else
					{
						m_containingScopes.Add(null);
					}
					break;
				case MemberName.RecursiveMember:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value2))
					{
						m_recursiveMember = (value2 as TablixMember);
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox;
		}

		private TextRun GetOrCreateSingleTextRun(IntermediateFormatReader reader)
		{
			if (m_paragraphs == null)
			{
				m_isSimple = true;
				m_paragraphs = new List<Paragraph>(1);
				Paragraph paragraph = new Paragraph(this, 0, -1);
				paragraph.GlobalID = -1;
				m_paragraphs.Add(paragraph);
				List<TextRun> list = new List<TextRun>(1);
				TextRun textRun = new TextRun(paragraph, 0, -1);
				textRun.GlobalID = -1;
				list.Add(textRun);
				paragraph.TextRuns = list;
				return textRun;
			}
			return m_paragraphs[0].TextRuns[0];
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			m_exprHost = reportExprHost.TextBoxHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (m_paragraphs == null)
			{
				return;
			}
			foreach (Paragraph paragraph in m_paragraphs)
			{
				paragraph.SetExprHost(m_exprHost, reportObjectModel);
			}
		}

		internal bool EvaluateInitialToggleState(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateTextBoxInitialToggleStateExpression(this);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			return GetTextBoxImpl(context).GetResult(romInstance, calledFromValue: false);
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			return GetTextBoxImpl(context).GetFieldsUsedInValueExpression(romInstance);
		}

		internal TextBoxImpl GetTextBoxImpl(OnDemandProcessingContext context)
		{
			if (m_textBoxImpl == null)
			{
				_ = context.ReportRuntime;
				ReportItemsImpl reportItemsImpl = context.ReportObjectModel.ReportItemsImpl;
				m_textBoxImpl = (TextBoxImpl)reportItemsImpl.GetReportItem(m_name);
				Global.Tracer.Assert(m_textBoxImpl != null, "(m_textBoxImpl != null)");
			}
			return m_textBoxImpl;
		}

		internal void ResetTextBoxImpl(OnDemandProcessingContext context)
		{
			GetTextBoxImpl(context).Reset();
		}

		internal void ResetDuplicates()
		{
			m_hasOldResult = false;
		}

		internal bool CalculateDuplicates(Microsoft.ReportingServices.RdlExpressions.VariantResult currentResult, OnDemandProcessingContext context)
		{
			bool flag = false;
			if (m_hideDuplicates != null)
			{
				if (m_hasOldResult)
				{
					if (currentResult.ErrorOccurred && m_oldResult.ErrorOccurred)
					{
						flag = true;
					}
					else if (currentResult.ErrorOccurred)
					{
						flag = false;
					}
					else if (m_oldResult.ErrorOccurred)
					{
						flag = false;
					}
					else if (currentResult.Value == null && m_oldResult.Value == null)
					{
						flag = true;
					}
					else if (currentResult.Value == null)
					{
						flag = false;
					}
					else
					{
						flag = (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareTo(currentResult.Value, m_oldResult.Value, (context.CurrentOdpDataSetInstance != null) ? context.CurrentOdpDataSetInstance.DataSetDef.NullsAsBlanks : context.NullsAsBlanks, (context.CurrentOdpDataSetInstance != null) ? context.CurrentOdpDataSetInstance.CompareInfo : context.CompareInfo, (context.CurrentOdpDataSetInstance != null) ? context.CurrentOdpDataSetInstance.ClrCompareOptions : context.ClrCompareOptions, throwExceptionOnComparisonFailure: false, extendedTypeComparisons: false, out bool validComparisonResult) == 0);
						if (!validComparisonResult)
						{
							flag = false;
						}
					}
				}
				if (!flag)
				{
					m_hasOldResult = true;
					m_oldResult = currentResult;
				}
			}
			return flag;
		}
	}
}
