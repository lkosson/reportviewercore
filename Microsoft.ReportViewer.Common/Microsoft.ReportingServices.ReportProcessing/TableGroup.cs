using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroup : ReportHierarchyNode, IRunningValueHolder, IPageBreakItem
	{
		private TableRowList m_headerRows;

		private bool m_headerRepeatOnNewPage;

		private TableRowList m_footerRows;

		private bool m_footerRepeatOnNewPage;

		private Visibility m_visibility;

		private bool m_propagatedPageBreakAtStart;

		private bool m_propagatedPageBreakAtEnd;

		private RunningValueInfoList m_runningValues;

		private bool m_hasExprHost;

		[NonSerialized]
		private TableGroupExprHost m_exprHost;

		[NonSerialized]
		private bool m_startHidden;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal TableGroup SubGroup
		{
			get
			{
				return (TableGroup)m_innerHierarchy;
			}
			set
			{
				m_innerHierarchy = value;
			}
		}

		internal TableRowList HeaderRows
		{
			get
			{
				return m_headerRows;
			}
			set
			{
				m_headerRows = value;
			}
		}

		internal bool HeaderRepeatOnNewPage
		{
			get
			{
				return m_headerRepeatOnNewPage;
			}
			set
			{
				m_headerRepeatOnNewPage = value;
			}
		}

		internal TableRowList FooterRows
		{
			get
			{
				return m_footerRows;
			}
			set
			{
				m_footerRows = value;
			}
		}

		internal bool FooterRepeatOnNewPage
		{
			get
			{
				return m_footerRepeatOnNewPage;
			}
			set
			{
				m_footerRepeatOnNewPage = value;
			}
		}

		internal Visibility Visibility
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

		internal bool PropagatedPageBreakAtStart
		{
			get
			{
				return m_propagatedPageBreakAtStart;
			}
			set
			{
				m_propagatedPageBreakAtStart = value;
			}
		}

		internal bool PropagatedPageBreakAtEnd
		{
			get
			{
				return m_propagatedPageBreakAtEnd;
			}
			set
			{
				m_propagatedPageBreakAtEnd = value;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return m_runningValues;
			}
			set
			{
				m_runningValues = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return m_renderingModelID;
			}
			set
			{
				m_renderingModelID = value;
			}
		}

		internal bool HasExprHost
		{
			get
			{
				return m_hasExprHost;
			}
			set
			{
				m_hasExprHost = value;
			}
		}

		internal TableGroupExprHost ExprHost => m_exprHost;

		internal int StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		internal int EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal double HeaderHeightValue
		{
			get
			{
				if (m_headerRows != null)
				{
					return m_headerRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal double FooterHeightValue
		{
			get
			{
				if (m_footerRows != null)
				{
					return m_footerRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal TableGroup()
		{
		}

		internal TableGroup(int id, Table tableDef)
			: base(id, tableDef)
		{
			m_runningValues = new RunningValueInfoList();
		}

		internal void Initialize(int numberOfColumns, TableDetail tableDetail, TableGroup detailGroup, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			Global.Tracer.Assert(m_grouping != null);
			context.Location |= LocationFlags.InGrouping;
			context.ExprHostBuilder.TableGroupStart(m_grouping.Name);
			context.RegisterGroupingScope(m_grouping.Name, m_grouping.SimpleGroupExpressions, m_grouping.Aggregates, m_grouping.PostSortAggregates, m_grouping.RecursiveAggregates, m_grouping);
			Initialize(context);
			context.RegisterRunningValues(m_runningValues);
			RegisterHeaderAndFooter(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			InitializeHeaderAndFooter(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			InitializeSubGroupsOrDetail(numberOfColumns, tableDetail, detailGroup, context, ref tableHeight, tableColumnVisibility);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			UnRegisterHeaderAndFooter(context);
			context.UnRegisterRunningValues(m_runningValues);
			context.UnRegisterGroupingScope(m_grouping.Name);
			m_hasExprHost = context.ExprHostBuilder.TableGroupEnd();
		}

		internal void RegisterReceiver(InitializationContext context, TableDetail tableDetail)
		{
			RegisterHeaderAndFooter(context);
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			RegisterHeaderAndFooterReceiver(context);
			RegisterSubGroupsOrDetailReceiver(context, tableDetail);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			UnRegisterHeaderAndFooter(context);
		}

		private void RegisterHeaderAndFooter(InitializationContext context)
		{
			if (m_headerRows != null)
			{
				m_headerRows.Register(context);
			}
			if (m_footerRows != null)
			{
				m_footerRows.Register(context);
			}
		}

		private void UnRegisterHeaderAndFooter(InitializationContext context)
		{
			if (m_footerRows != null)
			{
				m_footerRows.UnRegister(context);
			}
			if (m_headerRows != null)
			{
				m_headerRows.UnRegister(context);
			}
		}

		private void InitializeHeaderAndFooter(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsStart();
			if (m_headerRows != null)
			{
				for (int i = 0; i < m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(m_headerRows[i] != null);
					m_headerRows[i].Initialize(registerRunningValues: true, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			if (m_footerRows != null)
			{
				for (int j = 0; j < m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(m_footerRows[j] != null);
					m_footerRows[j].Initialize(registerRunningValues: true, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsEnd();
		}

		private void RegisterHeaderAndFooterReceiver(InitializationContext context)
		{
			if (m_headerRows != null)
			{
				for (int i = 0; i < m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(m_headerRows[i] != null);
					m_headerRows[i].RegisterReceiver(context);
				}
			}
			if (m_footerRows != null)
			{
				for (int j = 0; j < m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(m_footerRows[j] != null);
					m_footerRows[j].RegisterReceiver(context);
				}
			}
		}

		private void InitializeSubGroupsOrDetail(int numberOfColumns, TableDetail tableDetail, TableGroup detailGroup, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			if (detailGroup != null && SubGroup == null)
			{
				SubGroup = detailGroup;
				detailGroup = null;
			}
			if (SubGroup != null)
			{
				SubGroup.Initialize(numberOfColumns, tableDetail, detailGroup, context, ref tableHeight, tableColumnVisibility);
			}
			else
			{
				tableDetail?.Initialize(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			}
		}

		private void RegisterSubGroupsOrDetailReceiver(InitializationContext context, TableDetail tableDetail)
		{
			if (SubGroup != null)
			{
				SubGroup.RegisterReceiver(context, tableDetail);
			}
			else
			{
				tableDetail?.RegisterReceiver(context);
			}
		}

		internal void CalculatePropagatedFlags(out bool groupPageBreakAtStart, out bool groupPageBreakAtEnd)
		{
			if (SubGroup == null)
			{
				groupPageBreakAtStart = m_grouping.PageBreakAtStart;
				groupPageBreakAtEnd = m_grouping.PageBreakAtEnd;
				return;
			}
			SubGroup.CalculatePropagatedFlags(out groupPageBreakAtStart, out groupPageBreakAtEnd);
			groupPageBreakAtStart = (groupPageBreakAtStart || m_grouping.PageBreakAtStart);
			groupPageBreakAtEnd = (groupPageBreakAtEnd || m_grouping.PageBreakAtEnd);
			bool flag = true;
			if (SubGroup.HeaderRows != null)
			{
				flag = SubGroup.HeaderRepeatOnNewPage;
			}
			m_propagatedPageBreakAtStart = (SubGroup.Grouping.PageBreakAtStart || (SubGroup.PropagatedPageBreakAtStart && flag));
			flag = true;
			if (SubGroup.FooterRows != null)
			{
				flag = SubGroup.FooterRepeatOnNewPage;
			}
			m_propagatedPageBreakAtEnd = (SubGroup.Grouping.PageBreakAtEnd || (SubGroup.PropagatedPageBreakAtEnd && flag));
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			return IgnorePageBreaks(m_visibility);
		}

		internal void SetExprHost(TableGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && m_hasExprHost);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			ReportHierarchyNodeSetExprHost(m_exprHost, reportObjectModel);
			if (m_exprHost.TableRowVisibilityHiddenExpressions != null)
			{
				m_exprHost.TableRowVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.HasExprHost, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode, memberInfoList);
		}
	}
}
