using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartHeading : PivotHeading, IRunningValueHolder
	{
		private ExpressionInfoList m_labels;

		private RunningValueInfoList m_runningValues;

		private bool m_chartGroupExpression;

		private BoolList m_plotTypesLine;

		[NonSerialized]
		private ChartDynamicGroupExprHost m_exprHost;

		internal new ChartHeading SubHeading
		{
			get
			{
				return (ChartHeading)m_innerHierarchy;
			}
			set
			{
				m_innerHierarchy = value;
			}
		}

		internal ExpressionInfoList Labels
		{
			get
			{
				return m_labels;
			}
			set
			{
				m_labels = value;
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

		internal bool ChartGroupExpression
		{
			get
			{
				return m_chartGroupExpression;
			}
			set
			{
				m_chartGroupExpression = value;
			}
		}

		internal BoolList PlotTypesLine
		{
			get
			{
				return m_plotTypesLine;
			}
			set
			{
				m_plotTypesLine = value;
			}
		}

		internal ChartDynamicGroupExprHost ExprHost => m_exprHost;

		internal ChartHeading()
		{
		}

		internal ChartHeading(int id, Chart chartDef)
			: base(id, chartDef)
		{
			m_runningValues = new RunningValueInfoList();
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

		internal void SetExprHost(ChartDynamicGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && base.HasExprHost);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			ReportHierarchyNodeSetExprHost(m_exprHost, reportObjectModel);
		}

		internal void LabelCollectionInitialize(InitializationContext context, bool registerRunningValues, bool isStatic)
		{
			if (isStatic)
			{
				if (m_isColumn)
				{
					context.ExprHostBuilder.ChartStaticColumnLabelsStart();
				}
				else
				{
					context.ExprHostBuilder.ChartStaticRowLabelsStart();
				}
			}
			if (registerRunningValues)
			{
				context.RegisterRunningValues(m_runningValues);
			}
			Global.Tracer.Assert(m_labels != null);
			for (int i = 0; i < m_labels.Count; i++)
			{
				Global.Tracer.Assert(m_labels[i] != null);
				m_labels[i].Initialize("Label", context);
				if (isStatic)
				{
					context.ExprHostBuilder.ChartStaticColumnRowLabel(m_labels[i]);
				}
				else
				{
					context.ExprHostBuilder.ChartHeadingLabel(m_labels[i]);
				}
			}
			if (registerRunningValues)
			{
				context.UnRegisterRunningValues(m_runningValues);
			}
			if (isStatic)
			{
				if (m_isColumn)
				{
					context.ExprHostBuilder.ChartStaticColumnLabelsEnd();
				}
				else
				{
					context.ExprHostBuilder.ChartStaticRowLabelsEnd();
				}
			}
		}

		internal int DynamicInitialize(bool column, int level, InitializationContext context)
		{
			m_level = level;
			m_isColumn = column;
			if (m_grouping == null)
			{
				if (SubHeading != null)
				{
					SubHeading.DynamicInitialize(column, ++level, context);
				}
				return 1;
			}
			context.ExprHostBuilder.ChartDynamicGroupStart(m_grouping.Name);
			if (m_subtotal != null)
			{
				m_subtotal.RegisterReportItems(context);
				m_subtotal.Initialize(context);
			}
			context.Location |= LocationFlags.InGrouping;
			context.RegisterGroupingScope(m_grouping.Name, m_grouping.SimpleGroupExpressions, m_grouping.Aggregates, m_grouping.PostSortAggregates, m_grouping.RecursiveAggregates, m_grouping);
			ObjectType objectType = context.ObjectType;
			string objectName = context.ObjectName;
			context.ObjectType = ObjectType.Grouping;
			context.ObjectName = m_grouping.Name;
			Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			if (SubHeading != null)
			{
				m_subtotalSpan = SubHeading.DynamicInitialize(column, ++level, context);
			}
			else
			{
				m_subtotalSpan = 1;
			}
			if (m_labels != null)
			{
				LabelCollectionInitialize(context, registerRunningValues: true, isStatic: false);
			}
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.ObjectType = objectType;
			context.ObjectName = objectName;
			context.UnRegisterGroupingScope(m_grouping.Name);
			if (m_subtotal != null)
			{
				m_subtotal.UnregisterReportItems(context);
			}
			m_hasExprHost = context.ExprHostBuilder.ChartDynamicGroupEnd(column);
			return m_subtotalSpan + 1;
		}

		internal int StaticInitialize(InitializationContext context)
		{
			if (m_grouping != null)
			{
				int num = 1;
				if (SubHeading != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScope(m_grouping.Name, m_grouping.SimpleGroupExpressions, m_aggregates, m_postSortAggregates, m_recursiveAggregates, m_grouping);
					num = SubHeading.StaticInitialize(context);
					context.UnRegisterGroupingScope(m_grouping.Name);
				}
				return num + 1;
			}
			if (SubHeading != null)
			{
				m_subtotalSpan = SubHeading.StaticInitialize(context);
			}
			else
			{
				m_subtotalSpan = 1;
			}
			if (m_labels != null)
			{
				LabelCollectionInitialize(context, registerRunningValues: true, isStatic: true);
			}
			return 0;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Labels, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.ChartGroupExpression, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PlotTypesLine, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.BoolList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.PivotHeading, memberInfoList);
		}
	}
}
