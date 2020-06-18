using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetail : IDOwner, IRunningValueHolder
	{
		private TableRowList m_detailRows;

		private Sorting m_sorting;

		private Visibility m_visibility;

		private RunningValueInfoList m_runningValues;

		private bool m_hasExprHost;

		private bool m_simpleDetailRows;

		[NonSerialized]
		private TableGroupExprHost m_exprHost;

		[NonSerialized]
		private bool m_startHidden;

		internal TableRowList DetailRows
		{
			get
			{
				return m_detailRows;
			}
			set
			{
				m_detailRows = value;
			}
		}

		internal Sorting Sorting
		{
			get
			{
				return m_sorting;
			}
			set
			{
				m_sorting = value;
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

		internal bool SimpleDetailRows
		{
			get
			{
				return m_simpleDetailRows;
			}
			set
			{
				m_simpleDetailRows = value;
			}
		}

		internal TableGroupExprHost ExprHost => m_exprHost;

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

		internal TableDetail()
		{
		}

		internal TableDetail(int id)
			: base(id)
		{
			m_runningValues = new RunningValueInfoList();
		}

		internal void Initialize(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			context.Location |= LocationFlags.InDetail;
			context.DetailObjectType = ObjectType.Table;
			context.ExprHostBuilder.TableGroupStart("TableDetails");
			context.RegisterRunningValues(m_runningValues);
			if (m_sorting != null)
			{
				m_sorting.Initialize(context);
			}
			if (m_detailRows != null)
			{
				m_detailRows.Register(context);
			}
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			InitializeDetailRows(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			if (m_detailRows != null)
			{
				m_detailRows.UnRegister(context);
			}
			context.UnRegisterRunningValues(m_runningValues);
			m_hasExprHost = context.ExprHostBuilder.TableGroupEnd();
		}

		private void InitializeDetailRows(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			if (m_detailRows == null)
			{
				return;
			}
			if (!context.MergeOnePass || 1 >= context.DataRegionCount)
			{
				m_simpleDetailRows = true;
			}
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsStart();
			for (int i = 0; i < m_detailRows.Count; i++)
			{
				Global.Tracer.Assert(m_detailRows[i] != null);
				if (!m_detailRows[i].Initialize(registerRunningValues: true, numberOfColumns, context, ref tableHeight, tableColumnVisibility))
				{
					m_simpleDetailRows = false;
				}
			}
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsEnd();
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			if (m_detailRows != null)
			{
				m_detailRows.Register(context);
				if (m_visibility != null)
				{
					m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
				}
				for (int i = 0; i < m_detailRows.Count; i++)
				{
					Global.Tracer.Assert(m_detailRows[i] != null);
					m_detailRows[i].RegisterReceiver(context);
				}
				if (m_visibility != null)
				{
					m_visibility.UnRegisterReceiver(context);
				}
				m_detailRows.UnRegister(context);
			}
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

		internal void SetExprHost(TableGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && m_hasExprHost);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.TableRowVisibilityHiddenExpressions != null)
			{
				m_exprHost.TableRowVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
			}
			if (m_exprHost.SortingHost != null)
			{
				Global.Tracer.Assert(m_sorting != null);
				m_sorting.SetExprHost(exprHost.SortingHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DetailRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.Sorting, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Sorting));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.HasExprHost, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SimpleDetailRows, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
