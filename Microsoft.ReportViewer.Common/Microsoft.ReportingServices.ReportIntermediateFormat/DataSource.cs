using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataSource : IPersistable, IReferenceable, IProcessingDataSource
	{
		private int m_referenceID = -1;

		private string m_name;

		private bool m_transaction;

		private string m_type;

		private ExpressionInfo m_connectString;

		private bool m_integratedSecurity;

		private string m_prompt;

		private string m_dataSourceReference;

		private List<DataSet> m_dataSets;

		private Guid m_ID = Guid.Empty;

		private int m_exprHostID = -1;

		private string m_sharedDataSourceReferencePath;

		private bool m_isArtificialDataSource;

		[NonSerialized]
		private DataSourceExprHost m_exprHost;

		[NonSerialized]
		private bool m_isComplex;

		[NonSerialized]
		private Dictionary<string, bool> m_parameterNames;

		[NonSerialized]
		private string m_connectionCategory;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool IsArtificialForSharedDataSets => m_isArtificialDataSource;

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

		internal List<DataSet> DataSets
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

		internal Dictionary<string, bool> ParameterNames
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

		internal string ConnectionCategory
		{
			get
			{
				return m_connectionCategory;
			}
			set
			{
				m_connectionCategory = value;
			}
		}

		int IReferenceable.ID => m_referenceID;

		internal DataSource()
		{
		}

		internal DataSource(int id)
		{
			m_referenceID = id;
		}

		internal DataSource(int id, Guid sharedDataSourceReferenceId)
		{
			m_referenceID = id;
			m_ID = sharedDataSourceReferenceId;
			m_isArtificialDataSource = true;
			m_name = " Data source for shared dataset";
		}

		internal DataSource(int id, Guid sharedDataSourceReferenceId, DataSetCore dataSetCore)
			: this(id, sharedDataSourceReferenceId)
		{
			DataSet item = new DataSet(dataSetCore);
			m_dataSets = new List<DataSet>(1);
			m_dataSets.Add(item);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSource;
			context.ObjectName = m_name;
			InternalInitialize(context);
			if (m_dataSets != null)
			{
				for (int i = 0; i < m_dataSets.Count; i++)
				{
					Global.Tracer.Assert(m_dataSets[i] != null, "(null != m_dataSets[i])");
					m_dataSets[i].Initialize(context);
				}
				for (int j = 0; j < m_dataSets.Count; j++)
				{
					m_dataSets[j].CheckCircularDefaultRelationshipReference(context);
				}
			}
		}

		internal void DetermineDecomposability(InitializationContext context)
		{
			if (m_dataSets == null)
			{
				return;
			}
			foreach (DataSet dataSet in m_dataSets)
			{
				dataSet.DetermineDecomposability(context);
			}
		}

		internal string ResolveConnectionString(OnDemandProcessingContext pc, out DataSourceInfo dataSourceInfo)
		{
			dataSourceInfo = GetDataSourceInfo(pc);
			string text = null;
			if (dataSourceInfo != null)
			{
				text = dataSourceInfo.GetConnectionString(pc.DataProtection);
				if (!dataSourceInfo.IsReference && text == null)
				{
					text = EvaluateConnectStringExpression(pc);
				}
			}
			else
			{
				text = EvaluateConnectStringExpression(pc);
			}
			if (DataSourceInfo.HasUseridReference(text))
			{
				pc.ReportObjectModel.UserImpl.SetConnectionStringUserProfileDependencyOrThrow();
			}
			return text;
		}

		internal DataSourceInfo GetDataSourceInfo(OnDemandProcessingContext pc)
		{
			DataSourceInfo dataSourceInfo = null;
			if (pc.DataSourceInfos != null)
			{
				if (pc.IsSharedDataSetExecutionOnly)
				{
					dataSourceInfo = pc.DataSourceInfos.GetForSharedDataSetExecution();
				}
				else
				{
					if (Guid.Empty != ID)
					{
						dataSourceInfo = pc.DataSourceInfos.GetByID(ID);
					}
					if (dataSourceInfo == null)
					{
						dataSourceInfo = pc.DataSourceInfos.GetByName(Name, pc.ReportContext);
					}
				}
				if (dataSourceInfo == null)
				{
					throw new DataSourceNotFoundException(Name);
				}
			}
			else if (DataSourceReference != null)
			{
				throw new DataSourceNotFoundException(Name);
			}
			return dataSourceInfo;
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
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.DataSourceHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		private string EvaluateConnectStringExpression(OnDemandProcessingContext processingContext)
		{
			if (m_connectString == null)
			{
				return null;
			}
			if (ExpressionInfo.Types.Constant == m_connectString.Type)
			{
				return m_connectString.StringValue;
			}
			Global.Tracer.Assert(processingContext.ReportRuntime != null, "(null != processingContext.ReportRuntime)");
			if (processingContext.ReportRuntime.ReportExprHost != null)
			{
				SetExprHost(processingContext.ReportRuntime.ReportExprHost, processingContext.ReportObjectModel);
			}
			Microsoft.ReportingServices.RdlExpressions.StringResult stringResult = processingContext.ReportRuntime.EvaluateConnectString(this);
			if (stringResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsDataSourceConnectStringProcessingError, m_name);
			}
			return stringResult.Value;
		}

		internal bool AnyActiveDataSetNeedsAutoDetectCollation()
		{
			foreach (DataSet dataSet in m_dataSets)
			{
				if (!dataSet.UsedOnlyInParameters && dataSet.NeedAutoDetectCollation())
				{
					return true;
				}
			}
			return false;
		}

		internal void MergeCollationSettingsForAllDataSets(ErrorContext errorContext, string cultureName, bool caseSensitive, bool accentSensitive, bool kanatypeSensitive, bool widthSensitive)
		{
			for (int i = 0; i < m_dataSets.Count; i++)
			{
				m_dataSets[i].MergeCollationSettings(errorContext, m_type, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Transaction, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Type, Token.String));
			list.Add(new MemberInfo(MemberName.ConnectString, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntegratedSecurity, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Prompt, Token.String));
			list.Add(new MemberInfo(MemberName.DataSourceReference, Token.String));
			list.Add(new MemberInfo(MemberName.DataSets, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet));
			list.Add(new MemberInfo(MemberName.ID, Token.Guid));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.SharedDataSourceReferencePath, Token.String));
			list.Add(new MemberInfo(MemberName.ReferenceID, Token.Int32));
			list.Add(new MemberInfo(MemberName.IsArtificialDataSource, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Transaction:
					writer.Write(m_transaction);
					break;
				case MemberName.Type:
					writer.Write(m_type);
					break;
				case MemberName.ConnectString:
					writer.Write(m_connectString);
					break;
				case MemberName.IntegratedSecurity:
					writer.Write(m_integratedSecurity);
					break;
				case MemberName.Prompt:
					writer.Write(m_prompt);
					break;
				case MemberName.DataSourceReference:
					writer.Write(m_dataSourceReference);
					break;
				case MemberName.DataSets:
					writer.Write(m_dataSets);
					break;
				case MemberName.ID:
					writer.Write(m_ID);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.SharedDataSourceReferencePath:
					writer.Write(m_sharedDataSourceReferencePath);
					break;
				case MemberName.ReferenceID:
					writer.Write(m_referenceID);
					break;
				case MemberName.IsArtificialDataSource:
					writer.Write(m_isArtificialDataSource);
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Transaction:
					m_transaction = reader.ReadBoolean();
					break;
				case MemberName.Type:
					m_type = reader.ReadString();
					break;
				case MemberName.ConnectString:
					m_connectString = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntegratedSecurity:
					m_integratedSecurity = reader.ReadBoolean();
					break;
				case MemberName.Prompt:
					m_prompt = reader.ReadString();
					break;
				case MemberName.DataSourceReference:
					m_dataSourceReference = reader.ReadString();
					break;
				case MemberName.DataSets:
					m_dataSets = reader.ReadGenericListOfRIFObjects<DataSet>();
					break;
				case MemberName.ID:
					m_ID = reader.ReadGuid();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.SharedDataSourceReferencePath:
					m_sharedDataSourceReferencePath = reader.ReadString();
					break;
				case MemberName.ReferenceID:
					m_referenceID = reader.ReadInt32();
					break;
				case MemberName.IsArtificialDataSource:
					m_isArtificialDataSource = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, string.Empty);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IReferenceable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource;
		}
	}
}
