using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
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
	internal class SubReport : ReportItem, IPersistable, IIndexedInCollection, IGloballyReferenceable, IGlobalIDOwner
	{
		internal enum Status
		{
			NotRetrieved,
			DataRetrieveFailed,
			DefinitionRetrieveFailed,
			PreFetched,
			DefinitionRetrieved,
			DataRetrieved,
			DataNotRetrieved,
			ParametersNotSpecified
		}

		internal const uint MaxSubReportLevel = 20u;

		private string m_reportName;

		private List<ParameterValue> m_parameters;

		private ExpressionInfo m_noRowsMessage;

		private bool m_mergeTransactions;

		[Reference]
		private GroupingList m_containingScopes;

		private bool m_omitBorderOnPageBreak;

		private bool m_keepTogether;

		private bool m_isTablixCellScope;

		private Microsoft.ReportingServices.ReportPublishing.LocationFlags m_location = Microsoft.ReportingServices.ReportPublishing.LocationFlags.None;

		private int m_indexInCollection = -1;

		[Reference]
		private ReportSection m_containingSection;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ParameterInfoCollection m_parametersFromCatalog;

		[NonSerialized]
		private Status m_status;

		[NonSerialized]
		private Report m_report;

		[NonSerialized]
		private string m_description;

		[NonSerialized]
		private string m_reportPath;

		[NonSerialized]
		private SubreportExprHost m_exprHost;

		[NonSerialized]
		private List<SubReport> m_detailScopeSubReports;

		[NonSerialized]
		private SubReportInfo m_odpSubReportInfo;

		[NonSerialized]
		private ICatalogItemContext m_reportContext;

		[NonSerialized]
		private OnDemandProcessingContext m_odpContext;

		[NonSerialized]
		private bool m_exceededMaxLevel;

		[NonSerialized]
		private IReference<SubReportInstance> m_currentSubReportInstance;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Subreport;

		internal string OriginalCatalogPath
		{
			get
			{
				return m_reportPath;
			}
			set
			{
				m_reportPath = value;
			}
		}

		internal List<ParameterValue> Parameters
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

		internal ExpressionInfo NoRowsMessage
		{
			get
			{
				return m_noRowsMessage;
			}
			set
			{
				m_noRowsMessage = value;
			}
		}

		internal bool MergeTransactions
		{
			get
			{
				return m_mergeTransactions;
			}
			set
			{
				m_mergeTransactions = value;
			}
		}

		internal GroupingList ContainingScopes
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

		internal Status RetrievalStatus
		{
			get
			{
				return m_status;
			}
			set
			{
				m_status = value;
			}
		}

		internal string ReportName
		{
			get
			{
				return m_reportName;
			}
			set
			{
				m_reportName = value;
			}
		}

		internal string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		internal Report Report
		{
			get
			{
				return m_report;
			}
			set
			{
				m_report = value;
			}
		}

		internal ICatalogItemContext ReportContext
		{
			get
			{
				return m_reportContext;
			}
			set
			{
				m_reportContext = value;
			}
		}

		internal ParameterInfoCollection ParametersFromCatalog
		{
			get
			{
				return m_parametersFromCatalog;
			}
			set
			{
				m_parametersFromCatalog = value;
			}
		}

		internal SubreportExprHost SubReportExprHost => m_exprHost;

		internal bool IsTablixCellScope => m_isTablixCellScope;

		internal bool IsDetailScope
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

		internal List<SubReport> DetailScopeSubReports
		{
			get
			{
				return m_detailScopeSubReports;
			}
			set
			{
				m_detailScopeSubReports = value;
			}
		}

		internal SubReportInfo OdpSubReportInfo
		{
			get
			{
				return m_odpSubReportInfo;
			}
			set
			{
				m_odpSubReportInfo = value;
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

		internal bool OmitBorderOnPageBreak
		{
			get
			{
				return m_omitBorderOnPageBreak;
			}
			set
			{
				m_omitBorderOnPageBreak = value;
			}
		}

		internal OnDemandProcessingContext OdpContext
		{
			get
			{
				return m_odpContext;
			}
			set
			{
				m_odpContext = value;
			}
		}

		internal bool ExceededMaxLevel
		{
			get
			{
				return m_exceededMaxLevel;
			}
			set
			{
				m_exceededMaxLevel = value;
			}
		}

		internal bool InDataRegion => (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0;

		public int IndexInCollection
		{
			get
			{
				return m_indexInCollection;
			}
			set
			{
				m_indexInCollection = value;
			}
		}

		public IndexedInCollectionType IndexedInCollectionType => IndexedInCollectionType.SubReport;

		internal IReference<SubReportInstance> CurrentSubReportInstance
		{
			get
			{
				return m_currentSubReportInstance;
			}
			set
			{
				m_currentSubReportInstance = value;
			}
		}

		internal SubReport(ReportItem parent)
			: base(parent)
		{
		}

		internal SubReport(int id, ReportItem parent)
			: base(id, parent)
		{
			m_parameters = new List<ParameterValue>();
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			return new InstancePathItem(InstancePathItemType.SubReport, IndexInCollection);
		}

		internal ReportSection GetContainingSection(OnDemandProcessingContext parentReportOdpContext)
		{
			if (m_containingSection == null)
			{
				m_containingSection = parentReportOdpContext.ReportDefinition.ReportSections[0];
			}
			return m_containingSection;
		}

		internal void SetContainingSection(ReportSection section)
		{
			m_containingSection = section;
		}

		internal override bool Initialize(InitializationContext context)
		{
			m_location = context.Location;
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			if (InDataRegion)
			{
				context.SetDataSetHasSubReports();
				if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0)
				{
					m_isTablixCellScope = context.IsDataRegionScopedCell;
				}
				if ((Microsoft.ReportingServices.ReportPublishing.LocationFlags)0 < (context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail))
				{
					m_isDetailScope = true;
					context.SetDataSetDetailUserSortFilter();
				}
			}
			context.SetIndexInCollection(this);
			context.ExprHostBuilder.SubreportStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			if (m_parameters != null)
			{
				for (int i = 0; i < m_parameters.Count; i++)
				{
					ParameterValue parameterValue = m_parameters[i];
					context.ExprHostBuilder.SubreportParameterStart();
					parameterValue.Initialize("SubreportParameter(" + parameterValue.Name + ")", context, queryParam: false);
					parameterValue.ExprHostID = context.ExprHostBuilder.SubreportParameterEnd();
				}
			}
			if (m_noRowsMessage != null)
			{
				m_noRowsMessage.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(m_noRowsMessage);
			}
			base.ExprHostID = context.ExprHostBuilder.SubreportEnd();
			return false;
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			m_containingScopes = context.GetContainingScopes();
			for (int i = 0; i < m_containingScopes.Count; i++)
			{
				m_containingScopes[i].SaveGroupExprValues = true;
			}
			if (m_isDetailScope)
			{
				m_containingScopes.Add(null);
			}
		}

		internal void UpdateSubReportScopes(Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext context)
		{
			if (m_containingScopes != null && 0 < m_containingScopes.Count && m_containingScopes.LastEntry == null)
			{
				if (context.DetailScopeSubReports != null)
				{
					m_detailScopeSubReports = context.CloneDetailScopeSubReports();
				}
				else
				{
					m_detailScopeSubReports = new List<SubReport>();
				}
				m_detailScopeSubReports.Add(this);
			}
			else
			{
				m_detailScopeSubReports = context.DetailScopeSubReports;
			}
			if (context.ContainingScopes != null)
			{
				if (m_containingScopes != null && 0 < m_containingScopes.Count)
				{
					m_containingScopes.InsertRange(0, context.ContainingScopes);
				}
				else
				{
					m_containingScopes = context.ContainingScopes;
				}
			}
			if (m_report == null || m_report.EventSources == null)
			{
				return;
			}
			int count = m_report.EventSources.Count;
			for (int i = 0; i < count; i++)
			{
				IInScopeEventSource inScopeEventSource = m_report.EventSources[i];
				if (inScopeEventSource.UserSort != null)
				{
					inScopeEventSource.UserSort.DetailScopeSubReports = m_detailScopeSubReports;
				}
				if (m_containingScopes != null && 0 < m_containingScopes.Count)
				{
					if (inScopeEventSource.ContainingScopes != null && 0 < inScopeEventSource.ContainingScopes.Count)
					{
						inScopeEventSource.ContainingScopes.InsertRange(0, m_containingScopes);
						continue;
					}
					inScopeEventSource.IsSubReportTopLevelScope = true;
					inScopeEventSource.ContainingScopes = m_containingScopes;
				}
			}
		}

		internal void UpdateSubReportEventSourceGlobalDataSetIds(SubReportInfo subReportInfo)
		{
			m_odpSubReportInfo = subReportInfo;
			if (m_report == null || m_report.EventSources == null)
			{
				return;
			}
			int count = m_report.EventSources.Count;
			for (int i = 0; i < count; i++)
			{
				IInScopeEventSource inScopeEventSource = m_report.EventSources[i];
				if (inScopeEventSource.UserSort != null && -1 != subReportInfo.UserSortDataSetGlobalId)
				{
					inScopeEventSource.UserSort.SubReportDataSetGlobalId = subReportInfo.UserSortDataSetGlobalId;
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.NoRowsMessage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MergeTransactions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ContainingScopes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.IsTablixCellScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ReportName, Token.String));
			list.Add(new MemberInfo(MemberName.OmitBorderOnPageBreak, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Location, Token.Enum));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.ContainingSection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Parameters:
					writer.Write(m_parameters);
					break;
				case MemberName.NoRowsMessage:
					writer.Write(m_noRowsMessage);
					break;
				case MemberName.MergeTransactions:
					writer.Write(m_mergeTransactions);
					break;
				case MemberName.ContainingScopes:
					writer.WriteListOfReferences(m_containingScopes);
					break;
				case MemberName.IsTablixCellScope:
					writer.Write(m_isTablixCellScope);
					break;
				case MemberName.ReportName:
					writer.Write(m_reportName);
					break;
				case MemberName.OmitBorderOnPageBreak:
					writer.Write(m_omitBorderOnPageBreak);
					break;
				case MemberName.KeepTogether:
					writer.Write(m_keepTogether);
					break;
				case MemberName.Location:
					writer.WriteEnum((int)m_location);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.ContainingSection:
					writer.WriteReference(m_containingSection);
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
				case MemberName.Parameters:
					m_parameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.NoRowsMessage:
					m_noRowsMessage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MergeTransactions:
					m_mergeTransactions = reader.ReadBoolean();
					break;
				case MemberName.ContainingScopes:
					if (reader.ReadListOfReferencesNoResolution(this) == 0)
					{
						m_containingScopes = new GroupingList();
					}
					break;
				case MemberName.IsTablixCellScope:
					m_isTablixCellScope = reader.ReadBoolean();
					break;
				case MemberName.ReportName:
					m_reportName = reader.ReadString();
					break;
				case MemberName.OmitBorderOnPageBreak:
					m_omitBorderOnPageBreak = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.Location:
					m_location = (Microsoft.ReportingServices.ReportPublishing.LocationFlags)reader.ReadEnum();
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.ContainingSection:
					m_containingSection = reader.ReadReference<ReportSection>(this);
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
				case MemberName.ContainingSection:
				{
					referenceableItems.TryGetValue(item.RefID, out IReferenceable value2);
					m_containingSection = (ReportSection)value2;
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			SubReport subReport = (SubReport)base.PublishClone(context);
			context.AddSubReport(subReport);
			if (m_reportPath != null)
			{
				subReport.m_reportPath = (string)m_reportPath.Clone();
			}
			if (m_parameters != null)
			{
				subReport.m_parameters = new List<ParameterValue>(m_parameters.Count);
				foreach (ParameterValue parameter in m_parameters)
				{
					subReport.m_parameters.Add((ParameterValue)parameter.PublishClone(context));
				}
			}
			if (m_noRowsMessage != null)
			{
				subReport.m_noRowsMessage = (ExpressionInfo)m_noRowsMessage.PublishClone(context);
			}
			return subReport;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			m_exprHost = reportExprHost.SubreportHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_exprHost.ParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_parameters != null, "(m_parameters != null)");
				for (int num = m_parameters.Count - 1; num >= 0; num--)
				{
					m_parameters[num].SetExprHost(m_exprHost.ParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal string EvaulateNoRowMessage(IReportScopeInstance subReportInstance, OnDemandProcessingContext odpContext)
		{
			odpContext.SetupContext(this, subReportInstance);
			return odpContext.ReportRuntime.EvaluateSubReportNoRowsExpression(this, "Subreport", "NoRowsMessage");
		}
	}
}
