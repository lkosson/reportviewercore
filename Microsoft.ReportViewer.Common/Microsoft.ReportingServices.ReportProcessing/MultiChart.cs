using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MultiChart : ReportHierarchyNode
	{
		internal enum Layouts
		{
			Automatic,
			Horizontal,
			Vertical
		}

		private Layouts m_layout;

		private int m_maxCount;

		private bool m_syncScale;

		internal Layouts Layout
		{
			get
			{
				return m_layout;
			}
			set
			{
				m_layout = value;
			}
		}

		internal int MaxCount
		{
			get
			{
				return m_maxCount;
			}
			set
			{
				m_maxCount = value;
			}
		}

		internal bool SyncScale
		{
			get
			{
				return m_syncScale;
			}
			set
			{
				m_syncScale = value;
			}
		}

		internal MultiChart()
		{
		}

		internal MultiChart(int id, Chart chartDef)
			: base(id, chartDef)
		{
		}

		internal void SetExprHost(MultiChartExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			ReportHierarchyNodeSetExprHost(exprHost.GroupingHost, null, reportObjectModel);
		}

		internal new void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MultiChartStart();
			base.Initialize(context);
			context.ExprHostBuilder.MultiChartEnd();
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Layout, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MaxCount, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SyncScale, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode, memberInfoList);
		}
	}
}
