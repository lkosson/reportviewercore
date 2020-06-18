using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
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
	internal sealed class ReportSection : ReportItem, IPersistable, IRIFReportScope, IInstancePath
	{
		private ReportItemCollection m_reportItems;

		private Page m_page;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private List<TextBox> m_inScopeTextBoxes;

		private bool m_needsOverallTotalPages;

		private bool m_needsPageBreakTotalPages;

		private bool m_needsReportItemsOnPage;

		private bool m_layoutDirection;

		[NonSerialized]
		private int m_publishingIndexInCollection = -1;

		[NonSerialized]
		internal const int UpgradedExprHostId = 0;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportSection;

		internal ReportItemCollection ReportItems
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

		internal Page Page
		{
			get
			{
				return m_page;
			}
			set
			{
				m_page = value;
			}
		}

		internal double PageSectionWidth => m_page.GetPageSectionWidth(m_widthValue);

		internal override string DataElementNameDefault => "ReportSection" + m_publishingIndexInCollection.ToString(CultureInfo.InvariantCulture);

		internal override DataElementOutputTypes DataElementOutputDefault => DataElementOutputTypes.ContentsOnly;

		internal bool NeedsOverallTotalPages
		{
			get
			{
				return m_needsOverallTotalPages;
			}
			set
			{
				m_needsOverallTotalPages = value;
			}
		}

		internal bool NeedsPageBreakTotalPages
		{
			get
			{
				return m_needsOverallTotalPages;
			}
			set
			{
				m_needsOverallTotalPages = value;
			}
		}

		internal bool NeedsReportItemsOnPage
		{
			get
			{
				return m_needsReportItemsOnPage;
			}
			set
			{
				m_needsReportItemsOnPage = value;
			}
		}

		internal bool LayoutDirection
		{
			get
			{
				return m_layoutDirection;
			}
			set
			{
				m_layoutDirection = value;
			}
		}

		public bool NeedToCacheDataRows
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		internal ReportSection()
			: base(null)
		{
		}

		internal ReportSection(int indexInCollection)
			: this()
		{
			m_publishingIndexInCollection = indexInCollection;
		}

		internal ReportSection(int indexInCollection, Report report, int id, int idForReportItems)
			: base(id, report)
		{
			m_publishingIndexInCollection = indexInCollection;
			m_height = "11in";
			m_width = "8.5in";
			m_reportItems = new ReportItemCollection(idForReportItems, normal: true);
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			ReportItemExprHost exprHost = null;
			if (base.ExprHostID >= 0)
			{
				if (reportExprHost.ReportSectionHostsRemotable != null)
				{
					exprHost = reportExprHost.ReportSectionHostsRemotable[base.ExprHostID];
				}
				else if (base.ExprHostID == 0)
				{
					exprHost = reportExprHost;
				}
				else
				{
					Global.Tracer.Assert(false, "Missing ExprHost for Body. ExprHostID: {0}", base.ExprHostID);
				}
				ReportItemSetExprHost(exprHost, reportObjectModel);
			}
			if (m_page != null)
			{
				m_page.SetExprHost(reportExprHost, reportObjectModel);
			}
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location = Microsoft.ReportingServices.ReportPublishing.LocationFlags.None;
			context.ObjectType = ObjectType;
			context.ObjectName = DataElementNameDefault;
			context.RegisterReportSection(this);
			context.ExprHostBuilder.ReportSectionStart();
			base.Initialize(context);
			base.ExprHostID = context.ExprHostBuilder.ReportSectionEnd();
			m_page.Initialize(context);
			BodyInitialize(context);
			context.ValidateToggleItems();
			m_page.PageHeaderFooterInitialize(context);
			context.UnRegisterReportSection();
			return false;
		}

		internal void BodyInitialize(InitializationContext context)
		{
			context.RegisterReportItems(m_reportItems);
			m_textboxesInScope = context.GetCurrentReferencableTextboxes();
			m_variablesInScope = context.GetCurrentReferencableVariables();
			m_reportItems.Initialize(context);
			Report report = context.Report;
			if (report.HasUserSortFilter || report.HasSubReports)
			{
				context.InitializingUserSorts = true;
				m_reportItems.InitializeRVDirectionDependentItems(context);
				context.EventSourcesWithDetailSortExpressionInitialize(null);
				List<DataSource> dataSources = report.DataSources;
				if (dataSources != null)
				{
					for (int i = 0; i < dataSources.Count; i++)
					{
						List<DataSet> dataSets = dataSources[i].DataSets;
						if (dataSets != null)
						{
							for (int j = 0; j < dataSets.Count; j++)
							{
								context.ProcessUserSortScopes(dataSets[j].Name);
							}
						}
					}
				}
				context.ProcessUserSortScopes("0_ReportScope");
			}
			if (report.HasPreviousAggregates)
			{
				m_reportItems.DetermineGroupingExprValueCount(context, 0);
			}
			context.UnRegisterReportItems(m_reportItems);
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			foreach (ReportItem reportItem in m_reportItems)
			{
				reportItem.TraverseScopes(visitor);
			}
		}

		public bool TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_textboxesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		public void AddInScopeTextBox(TextBox textbox)
		{
			if (m_inScopeTextBoxes == null)
			{
				m_inScopeTextBoxes = new List<TextBox>();
			}
			m_inScopeTextBoxes.Add(textbox);
		}

		public void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			if (m_inScopeTextBoxes != null)
			{
				for (int i = 0; i < m_inScopeTextBoxes.Count; i++)
				{
					m_inScopeTextBoxes[i].ResetTextBoxImpl(context);
				}
			}
		}

		public bool VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_variablesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		public void AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			Global.Tracer.Assert(condition: false, "Top level event sources should be registered on the Report, not ReportSection");
		}

		internal void SetTextboxesInScope(byte[] items)
		{
			m_textboxesInScope = items;
		}

		internal void SetInScopeTextBoxes(List<TextBox> items)
		{
			m_inScopeTextBoxes = items;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Page:
					writer.Write(m_page);
					break;
				case MemberName.ReportItems:
					writer.Write(m_reportItems);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(m_variablesInScope);
					break;
				case MemberName.NeedsOverallTotalPages:
					writer.Write(m_needsOverallTotalPages);
					break;
				case MemberName.NeedsPageBreakTotalPages:
					writer.Write(m_needsPageBreakTotalPages);
					break;
				case MemberName.NeedsReportItemsOnPage:
					writer.Write(m_needsReportItemsOnPage);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(m_inScopeTextBoxes);
					break;
				case MemberName.LayoutDirection:
					writer.Write(m_layoutDirection);
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
				case MemberName.Page:
					m_page = (Page)reader.ReadRIFObject();
					break;
				case MemberName.ReportItems:
					m_reportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.TextboxesInScope:
					m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.NeedsTotalPages:
				case MemberName.NeedsOverallTotalPages:
					m_needsOverallTotalPages = reader.ReadBoolean();
					break;
				case MemberName.NeedsPageBreakTotalPages:
					m_needsPageBreakTotalPages = reader.ReadBoolean();
					break;
				case MemberName.NeedsReportItemsOnPage:
					m_needsReportItemsOnPage = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxes:
					m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.LayoutDirection:
					m_layoutDirection = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
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
			foreach (MemberReference item2 in value)
			{
				MemberName memberName = item2.MemberName;
				if (memberName == MemberName.InScopeTextBoxes)
				{
					if (m_inScopeTextBoxes == null)
					{
						m_inScopeTextBoxes = new List<TextBox>();
					}
					referenceableItems.TryGetValue(item2.RefID, out IReferenceable value2);
					TextBox item = (TextBox)value2;
					m_inScopeTextBoxes.Add(item);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Page, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page));
				list.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
				list.Add(new MemberInfo(MemberName.TextboxesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
				list.Add(new MemberInfo(MemberName.VariablesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
				list.Add(new ReadOnlyMemberInfo(MemberName.NeedsTotalPages, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NeedsOverallTotalPages, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NeedsReportItemsOnPage, Token.Boolean));
				list.Add(new MemberInfo(MemberName.InScopeTextBoxes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
				list.Add(new MemberInfo(MemberName.NeedsPageBreakTotalPages, Token.Boolean));
				list.Add(new MemberInfo(MemberName.LayoutDirection, Token.Boolean, Lifetime.AddedIn(200)));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
			}
			return m_Declaration;
		}
	}
}
