using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class DataAggregateInfo
	{
		internal enum AggregateTypes
		{
			First,
			Last,
			Sum,
			Avg,
			Max,
			Min,
			CountDistinct,
			CountRows,
			Count,
			StDev,
			Var,
			StDevP,
			VarP,
			Aggregate,
			Previous
		}

		private string m_name;

		private AggregateTypes m_aggregateType;

		private ExpressionInfo[] m_expressions;

		private StringList m_duplicateNames;

		[NonSerialized]
		private string m_scope;

		[NonSerialized]
		private bool m_hasScope;

		[NonSerialized]
		private bool m_recursive;

		[NonSerialized]
		private bool m_isCopied;

		[NonSerialized]
		private AggregateParamExprHost[] m_expressionHosts;

		[NonSerialized]
		private bool m_exprHostInitialized;

		[NonSerialized]
		private ObjectModelImpl m_exprHostReportObjectModel;

		[NonSerialized]
		private bool m_suppressExceptions;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal AggregateTypes AggregateType
		{
			get
			{
				return m_aggregateType;
			}
			set
			{
				m_aggregateType = value;
			}
		}

		internal ExpressionInfo[] Expressions
		{
			get
			{
				return m_expressions;
			}
			set
			{
				m_expressions = value;
			}
		}

		internal StringList DuplicateNames
		{
			get
			{
				return m_duplicateNames;
			}
			set
			{
				m_duplicateNames = value;
			}
		}

		internal string ExpressionText
		{
			get
			{
				if (m_expressions != null && 1 == m_expressions.Length)
				{
					return m_expressions[0].OriginalText;
				}
				return string.Empty;
			}
		}

		internal AggregateParamExprHost[] ExpressionHosts => m_expressionHosts;

		internal bool ExprHostInitialized
		{
			get
			{
				return m_exprHostInitialized;
			}
			set
			{
				m_exprHostInitialized = value;
			}
		}

		internal bool Recursive
		{
			get
			{
				return m_recursive;
			}
			set
			{
				m_recursive = value;
			}
		}

		internal bool IsCopied
		{
			get
			{
				return m_isCopied;
			}
			set
			{
				m_isCopied = value;
			}
		}

		internal bool SuppressExceptions => m_suppressExceptions;

		internal List<string> FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal DataAggregateInfo DeepClone(InitializationContext context)
		{
			DataAggregateInfo dataAggregateInfo = new DataAggregateInfo();
			DeepCloneInternal(dataAggregateInfo, context);
			return dataAggregateInfo;
		}

		protected void DeepCloneInternal(DataAggregateInfo clone, InitializationContext context)
		{
			clone.m_name = context.GenerateAggregateID(m_name);
			clone.m_aggregateType = m_aggregateType;
			if (m_expressions != null)
			{
				int num = m_expressions.Length;
				clone.m_expressions = new ExpressionInfo[num];
				for (int i = 0; i < num; i++)
				{
					clone.m_expressions[i] = m_expressions[i].DeepClone(context);
				}
			}
			Global.Tracer.Assert(m_duplicateNames == null);
			clone.m_recursive = m_recursive;
			clone.m_isCopied = false;
			clone.m_suppressExceptions = true;
			if (m_hasScope)
			{
				clone.SetScope(context.EscalateScope(m_scope));
			}
		}

		internal void SetScope(string scope)
		{
			m_hasScope = true;
			m_scope = scope;
		}

		internal bool GetScope(out string scope)
		{
			scope = m_scope;
			return m_hasScope;
		}

		internal void SetExprHosts(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			if (!m_exprHostInitialized)
			{
				for (int i = 0; i < m_expressions.Length; i++)
				{
					ExpressionInfo expressionInfo = m_expressions[i];
					if (expressionInfo.ExprHostID >= 0)
					{
						if (m_expressionHosts == null)
						{
							m_expressionHosts = new AggregateParamExprHost[m_expressions.Length];
						}
						AggregateParamExprHost aggregateParamExprHost = reportExprHost.AggregateParamHostsRemotable[expressionInfo.ExprHostID];
						aggregateParamExprHost.SetReportObjectModel(reportObjectModel);
						m_expressionHosts[i] = aggregateParamExprHost;
					}
				}
				m_exprHostInitialized = true;
				m_exprHostReportObjectModel = reportObjectModel;
			}
			else
			{
				if (m_exprHostReportObjectModel == reportObjectModel || m_expressionHosts == null)
				{
					return;
				}
				for (int j = 0; j < m_expressionHosts.Length; j++)
				{
					if (m_expressionHosts[j] != null)
					{
						m_expressionHosts[j].SetReportObjectModel(reportObjectModel);
					}
				}
				m_exprHostReportObjectModel = reportObjectModel;
			}
		}

		internal bool IsPostSortAggregate()
		{
			if (m_aggregateType == AggregateTypes.First || AggregateTypes.Last == m_aggregateType || AggregateTypes.Previous == m_aggregateType)
			{
				return true;
			}
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.AggregateType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Expressions, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DuplicateNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
