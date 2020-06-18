using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class OWCChart : DataRegion, IRunningValueHolder
	{
		private ChartColumnList m_chartData;

		private string m_chartDefinition;

		private RunningValueInfoList m_detailRunningValues;

		private RunningValueInfoList m_runningValues;

		[NonSerialized]
		private OWCChartExprHost m_exprHost;

		internal override ObjectType ObjectType => ObjectType.OWCChart;

		internal ChartColumnList ChartData
		{
			get
			{
				return m_chartData;
			}
			set
			{
				m_chartData = value;
			}
		}

		internal string ChartDefinition
		{
			get
			{
				return m_chartDefinition;
			}
			set
			{
				m_chartDefinition = value;
			}
		}

		internal RunningValueInfoList DetailRunningValues
		{
			get
			{
				return m_detailRunningValues;
			}
			set
			{
				m_detailRunningValues = value;
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

		internal OWCChartExprHost OWCChartExprHost => m_exprHost;

		protected override DataRegionExprHost DataRegionExprHost => m_exprHost;

		internal OWCChart(ReportItem parent)
			: base(parent)
		{
		}

		internal OWCChart(int id, ReportItem parent)
			: base(id, parent)
		{
			m_chartData = new ChartColumnList();
			m_detailRunningValues = new RunningValueInfoList();
			m_runningValues = new RunningValueInfoList();
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.RegisterDataRegion(this);
			InternalInitialize(context);
			context.UnRegisterDataRegion(this);
			return false;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ExprHostBuilder.OWCChartStart(m_name);
			base.Initialize(context);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			context.RegisterRunningValues(m_runningValues);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			context.UnRegisterRunningValues(m_runningValues);
			context.RegisterRunningValues(m_detailRunningValues);
			if (m_chartData != null)
			{
				context.ExprHostBuilder.OWCChartColumnsStart();
				for (int i = 0; i < m_chartData.Count; i++)
				{
					m_chartData[i].Initialize(context);
				}
				context.ExprHostBuilder.OWCChartColumnsEnd();
			}
			context.UnRegisterRunningValues(m_detailRunningValues);
			base.ExprHostID = context.ExprHostBuilder.OWCChartEnd();
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.OWCChartHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_exprHost, reportObjectModel);
			}
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_detailRunningValues != null);
			if (m_detailRunningValues.Count == 0)
			{
				m_detailRunningValues = null;
			}
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ChartData, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartColumnList));
			memberInfoList.Add(new MemberInfo(MemberName.ChartDefinition, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DetailRunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion, memberInfoList);
		}
	}
}
