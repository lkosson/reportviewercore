using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Filter
	{
		internal enum Operators
		{
			Equal,
			Like,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			TopN,
			BottomN,
			TopPercent,
			BottomPercent,
			In,
			Between,
			NotEqual
		}

		private ExpressionInfo m_expression;

		private Operators m_operator;

		private ExpressionInfoList m_values;

		private int m_exprHostID = -1;

		[NonSerialized]
		private FilterExprHost m_exprHost;

		internal ExpressionInfo Expression
		{
			get
			{
				return m_expression;
			}
			set
			{
				m_expression = value;
			}
		}

		internal Operators Operator
		{
			get
			{
				return m_operator;
			}
			set
			{
				m_operator = value;
			}
		}

		internal ExpressionInfoList Values
		{
			get
			{
				return m_values;
			}
			set
			{
				m_values = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal FilterExprHost ExprHost => m_exprHost;

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FilterStart();
			if (m_expression != null)
			{
				m_expression.Initialize("FilterExpression", context);
				context.ExprHostBuilder.FilterExpression(m_expression);
			}
			if (m_values != null)
			{
				for (int i = 0; i < m_values.Count; i++)
				{
					ExpressionInfo expressionInfo = m_values[i];
					Global.Tracer.Assert(expressionInfo != null);
					expressionInfo.Initialize("FilterValue", context);
					context.ExprHostBuilder.FilterValue(expressionInfo);
				}
			}
			m_exprHostID = context.ExprHostBuilder.FilterEnd();
		}

		internal void SetExprHost(IList<FilterExprHost> filterHosts, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(filterHosts != null && reportObjectModel != null);
				m_exprHost = filterHosts[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Expression, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Operator, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Values, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
