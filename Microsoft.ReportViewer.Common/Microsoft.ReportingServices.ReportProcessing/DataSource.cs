using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSource : IProcessingDataSource
	{
		private string m_name;

		private bool m_transaction;

		private string m_type;

		private ExpressionInfo m_connectString;

		private bool m_integratedSecurity;

		private string m_prompt;

		private string m_dataSourceReference;

		private DataSetList m_dataSets;

		private Guid m_ID = Guid.Empty;

		private int m_exprHostID = -1;

		private string m_sharedDataSourceReferencePath;

		[NonSerialized]
		private DataSourceExprHost m_exprHost;

		[NonSerialized]
		private bool m_isComplex;

		[NonSerialized]
		private StringList m_parameterNames;

		public string Name
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

		public bool Transaction
		{
			get
			{
				return m_transaction;
			}
			set
			{
				m_transaction = value;
			}
		}

		public string Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal ExpressionInfo ConnectStringExpression
		{
			get
			{
				return m_connectString;
			}
			set
			{
				m_connectString = value;
			}
		}

		public bool IntegratedSecurity
		{
			get
			{
				return m_integratedSecurity;
			}
			set
			{
				m_integratedSecurity = value;
			}
		}

		public string Prompt
		{
			get
			{
				return m_prompt;
			}
			set
			{
				m_prompt = value;
			}
		}

		public string DataSourceReference
		{
			get
			{
				return m_dataSourceReference;
			}
			set
			{
				m_dataSourceReference = value;
			}
		}

		internal DataSetList DataSets
		{
			get
			{
				return m_dataSets;
			}
			set
			{
				m_dataSets = value;
			}
		}

		public Guid ID
		{
			get
			{
				return m_ID;
			}
			set
			{
				m_ID = value;
			}
		}

		internal DataSourceExprHost ExprHost => m_exprHost;

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

		internal bool IsComplex
		{
			get
			{
				return m_isComplex;
			}
			set
			{
				m_isComplex = value;
			}
		}

		internal StringList ParameterNames
		{
			get
			{
				return m_parameterNames;
			}
			set
			{
				m_parameterNames = value;
			}
		}

		public string SharedDataSourceReferencePath
		{
			get
			{
				return m_sharedDataSourceReferencePath;
			}
			set
			{
				m_sharedDataSourceReferencePath = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType.DataSource;
			context.ObjectName = m_name;
			InternalInitialize(context);
			if (m_dataSets != null)
			{
				for (int i = 0; i < m_dataSets.Count; i++)
				{
					Global.Tracer.Assert(m_dataSets[i] != null);
					m_dataSets[i].Initialize(context);
				}
			}
		}

		internal string ResolveConnectionString(ReportProcessing.ReportProcessingContext pc, out DataSourceInfo dataSourceInfo)
		{
			dataSourceInfo = null;
			string text = null;
			if (pc.DataSourceInfos != null)
			{
				if (Guid.Empty != ID)
				{
					dataSourceInfo = pc.DataSourceInfos.GetByID(ID);
				}
				if (dataSourceInfo == null)
				{
					dataSourceInfo = pc.DataSourceInfos.GetByName(Name, pc.ReportContext);
				}
				if (dataSourceInfo == null)
				{
					throw new DataSourceNotFoundException(Name);
				}
				text = dataSourceInfo.GetConnectionString(pc.DataProtection);
				if (!dataSourceInfo.IsReference && text == null)
				{
					text = EvaluateConnectStringExpression(pc);
				}
			}
			else
			{
				if (DataSourceReference != null)
				{
					throw new DataSourceNotFoundException(Name);
				}
				text = EvaluateConnectStringExpression(pc);
			}
			return text;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataSourceStart(m_name);
			if (m_connectString != null)
			{
				m_connectString.Initialize("ConnectString", context);
				context.ExprHostBuilder.DataSourceConnectString(m_connectString);
			}
			m_exprHostID = context.ExprHostBuilder.DataSourceEnd();
		}

		private void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.DataSourceHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		private string EvaluateConnectStringExpression(ReportProcessing.ProcessingContext processingContext)
		{
			if (m_connectString == null)
			{
				return null;
			}
			if (ExpressionInfo.Types.Constant == m_connectString.Type)
			{
				return m_connectString.Value;
			}
			Global.Tracer.Assert(processingContext.ReportRuntime != null);
			if (processingContext.ReportRuntime.ReportExprHost != null)
			{
				SetExprHost(processingContext.ReportRuntime.ReportExprHost, processingContext.ReportObjectModel);
			}
			StringResult stringResult = processingContext.ReportRuntime.EvaluateConnectString(this);
			if (stringResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsDataSourceConnectStringProcessingError, m_name);
			}
			return stringResult.Value;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Transaction, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ConnectString, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.IntegratedSecurity, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Prompt, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataSourceReference, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataSets, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataSetList));
			memberInfoList.Add(new MemberInfo(MemberName.ID, Token.Guid));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SharedDataSourceReferencePath, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
