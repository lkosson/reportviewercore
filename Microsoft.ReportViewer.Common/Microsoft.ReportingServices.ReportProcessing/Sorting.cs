using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Sorting
	{
		private ExpressionInfoList m_sortExpressions;

		private BoolList m_sortDirections;

		[NonSerialized]
		private SortingExprHost m_exprHost;

		internal ExpressionInfoList SortExpressions
		{
			get
			{
				return m_sortExpressions;
			}
			set
			{
				m_sortExpressions = value;
			}
		}

		internal BoolList SortDirections
		{
			get
			{
				return m_sortDirections;
			}
			set
			{
				m_sortDirections = value;
			}
		}

		internal SortingExprHost ExprHost => m_exprHost;

		internal Sorting(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				m_sortExpressions = new ExpressionInfoList();
				m_sortDirections = new BoolList();
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.SortingStart();
			if (m_sortExpressions != null)
			{
				for (int i = 0; i < m_sortExpressions.Count; i++)
				{
					ExpressionInfo expressionInfo = m_sortExpressions[i];
					expressionInfo.Initialize("SortExpression", context);
					context.ExprHostBuilder.SortingExpression(expressionInfo);
				}
			}
			context.ExprHostBuilder.SortingEnd();
		}

		internal void SetExprHost(SortingExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.SortExpressions, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.SortDirections, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.BoolList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
