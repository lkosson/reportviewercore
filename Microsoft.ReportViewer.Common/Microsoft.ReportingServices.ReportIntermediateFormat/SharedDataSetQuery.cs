using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class SharedDataSetQuery : IPersistable
	{
		private List<ParameterValue> m_queryParameters;

		private string m_originalSharedDataSetReference;

		[NonSerialized]
		private IndexedExprHost m_queryParamsExprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal string SharedDataSetReference
		{
			get
			{
				return m_originalSharedDataSetReference;
			}
			set
			{
				m_originalSharedDataSetReference = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
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
			list.Add(new MemberInfo(MemberName.QueryParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.OriginalSharedDataSetReference, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SharedDataSetQuery, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.QueryParameters:
					writer.Write(m_queryParameters);
					break;
				case MemberName.OriginalSharedDataSetReference:
					writer.Write(m_originalSharedDataSetReference);
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
				case MemberName.QueryParameters:
					m_queryParameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.OriginalSharedDataSetReference:
					m_originalSharedDataSetReference = reader.ReadString();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SharedDataSetQuery;
		}
	}
}
