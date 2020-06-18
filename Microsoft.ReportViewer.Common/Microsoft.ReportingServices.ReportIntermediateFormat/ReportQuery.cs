using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportQuery : IPersistable
	{
		private CommandType m_commandType = CommandType.Text;

		private ExpressionInfo m_commandText;

		private List<ParameterValue> m_queryParameters;

		private int m_timeOut;

		[NonSerialized]
		private string m_dataSourceName;

		[NonSerialized]
		private IndexedExprHost m_queryParamsExprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal CommandType CommandType
		{
			get
			{
				return m_commandType;
			}
			set
			{
				m_commandType = value;
			}
		}

		internal ExpressionInfo CommandText
		{
			get
			{
				return m_commandText;
			}
			set
			{
				m_commandText = value;
			}
		}

		internal List<ParameterValue> Parameters
		{
			get
			{
				return m_queryParameters;
			}
			set
			{
				m_queryParameters = value;
			}
		}

		internal int TimeOut
		{
			get
			{
				return m_timeOut;
			}
			set
			{
				m_timeOut = value;
			}
		}

		internal string DataSourceName
		{
			get
			{
				return m_dataSourceName;
			}
			set
			{
				m_dataSourceName = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_commandText != null)
			{
				m_commandText.Initialize("CommandText", context);
				context.ExprHostBuilder.DataSetQueryCommandText(m_commandText);
			}
			if (m_queryParameters != null)
			{
				Microsoft.ReportingServices.ReportProcessing.ObjectType objectType = context.ObjectType;
				string objectName = context.ObjectName;
				context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
				context.ExprHostBuilder.QueryParametersStart();
				for (int i = 0; i < m_queryParameters.Count; i++)
				{
					ParameterValue parameterValue = m_queryParameters[i];
					context.ObjectName = parameterValue.Name;
					parameterValue.Initialize(null, context, queryParam: true);
				}
				context.ExprHostBuilder.QueryParametersEnd();
				context.ObjectType = objectType;
				context.ObjectName = objectName;
			}
		}

		internal void SetExprHost(IndexedExprHost queryParamsExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(queryParamsExprHost != null && reportObjectModel != null, "(queryParamsExprHost != null && reportObjectModel != null)");
			m_queryParamsExprHost = queryParamsExprHost;
			m_queryParamsExprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CommandType, Token.Enum));
			list.Add(new MemberInfo(MemberName.CommandText, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.QueryParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.Timeout, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CommandType:
					writer.WriteEnum((int)m_commandType);
					break;
				case MemberName.CommandText:
					writer.Write(m_commandText);
					break;
				case MemberName.QueryParameters:
					writer.Write(m_queryParameters);
					break;
				case MemberName.Timeout:
					writer.Write(m_timeOut);
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CommandType:
					m_commandType = (CommandType)reader.ReadEnum();
					break;
				case MemberName.CommandText:
					m_commandText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.QueryParameters:
					m_queryParameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.Timeout:
					m_timeOut = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, string.Empty);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery;
		}
	}
}
