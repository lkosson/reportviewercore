using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SubReport : ReportItem, IPageBreakItem
	{
		internal enum Status
		{
			NotRetrieved,
			Retrieved,
			RetrieveFailed,
			PreFetched
		}

		internal const uint MaxSubReportLevel = 20u;

		private string m_reportPath;

		private ParameterValueList m_parameters;

		private ExpressionInfo m_noRows;

		private bool m_mergeTransactions;

		[Reference]
		private GroupingList m_containingScopes;

		private bool m_isMatrixCellScope;

		private Status m_status;

		private string m_reportName;

		private string m_description;

		private Report m_report;

		private string m_stringUri;

		private ParameterInfoCollection m_parametersFromCatalog;

		private ScopeLookupTable m_dataSetUniqueNameMap;

		[NonSerialized]
		private string m_subReportScope;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private PageBreakStates m_pagebreakState;

		[NonSerialized]
		private SubreportExprHost m_exprHost;

		[NonSerialized]
		private SubReportList m_detailScopeSubReports;

		[NonSerialized]
		private bool m_saveDataSetUniqueName;

		[NonSerialized]
		private Uri m_uri;

		[NonSerialized]
		private ICatalogItemContext m_reportContext;

		internal override ObjectType ObjectType => ObjectType.Subreport;

		internal string ReportPath
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

		internal ParameterValueList Parameters
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

		internal ExpressionInfo NoRows
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

		internal string StringUri
		{
			get
			{
				return m_stringUri;
			}
			set
			{
				m_stringUri = value;
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

		internal Uri Uri
		{
			get
			{
				if (null == m_uri)
				{
					m_uri = new Uri(m_stringUri);
				}
				return m_uri;
			}
		}

		internal SubreportExprHost SubReportExprHost => m_exprHost;

		internal string SubReportScope
		{
			get
			{
				return m_subReportScope;
			}
			set
			{
				m_subReportScope = value;
			}
		}

		internal bool IsMatrixCellScope
		{
			get
			{
				return m_isMatrixCellScope;
			}
			set
			{
				m_isMatrixCellScope = value;
			}
		}

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

		internal SubReportList DetailScopeSubReports
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

		internal ScopeLookupTable DataSetUniqueNameMap
		{
			get
			{
				return m_dataSetUniqueNameMap;
			}
			set
			{
				m_dataSetUniqueNameMap = value;
			}
		}

		internal bool SaveDataSetUniqueName => m_saveDataSetUniqueName;

		internal SubReport(ReportItem parent)
			: base(parent)
		{
		}

		internal SubReport(int id, ReportItem parent)
			: base(id, parent)
		{
			m_parameters = new ParameterValueList();
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			m_subReportScope = context.GetCurrentScope();
			if ((LocationFlags)0 < (context.Location & LocationFlags.InMatrixCellTopLevelItem))
			{
				m_isMatrixCellScope = true;
			}
			if ((LocationFlags)0 < (context.Location & LocationFlags.InDetail))
			{
				m_isDetailScope = true;
				context.SetDataSetDetailUserSortFilter();
			}
			context.ExprHostBuilder.SubreportStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			if (m_parameters != null)
			{
				for (int i = 0; i < m_parameters.Count; i++)
				{
					ParameterValue parameterValue = m_parameters[i];
					context.ExprHostBuilder.SubreportParameterStart();
					parameterValue.Initialize(context, queryParam: false);
					parameterValue.ExprHostID = context.ExprHostBuilder.SubreportParameterEnd();
				}
			}
			if (m_noRows != null)
			{
				m_noRows.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(m_noRows);
			}
			base.ExprHostID = context.ExprHostBuilder.SubreportEnd();
			return false;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.SubreportHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_exprHost.ParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_parameters != null);
				for (int num = m_parameters.Count - 1; num >= 0; num--)
				{
					m_parameters[num].SetExprHost(m_exprHost.ParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_visibility))
				{
					m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		bool IPageBreakItem.HasPageBreaks(bool atStart)
		{
			return false;
		}

		internal void UpdateSubReportScopes(UserSortFilterContext context)
		{
			if (m_containingScopes != null && 0 < m_containingScopes.Count && m_containingScopes.LastEntry == null)
			{
				if (context.DetailScopeSubReports != null)
				{
					m_detailScopeSubReports = context.DetailScopeSubReports.Clone();
				}
				else
				{
					m_detailScopeSubReports = new SubReportList();
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
		}

		internal void AddDataSetUniqueName(VariantList[] scopeValues, int subReportUniqueName)
		{
			if (m_dataSetUniqueNameMap == null)
			{
				m_dataSetUniqueNameMap = new ScopeLookupTable();
				m_saveDataSetUniqueName = true;
			}
			m_dataSetUniqueNameMap.Add(m_containingScopes, scopeValues, subReportUniqueName);
		}

		internal int GetDataSetUniqueName(VariantList[] scopeValues)
		{
			Global.Tracer.Assert(m_dataSetUniqueNameMap != null);
			return m_dataSetUniqueNameMap.Lookup(m_containingScopes, scopeValues);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportPath, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.MergeTransactions, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ContainingScopes, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.GroupingList));
			memberInfoList.Add(new MemberInfo(MemberName.IsMatrixCellScope, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.DataSetUniqueNameMap, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ScopeLookupTable));
			memberInfoList.Add(new MemberInfo(MemberName.Status, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.ReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Description, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Report, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Report));
			memberInfoList.Add(new MemberInfo(MemberName.StringUri, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ParametersFromCatalog, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
