using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ParameterValue : IPersistable
	{
		private string m_name;

		private string m_uniqueName;

		private ExpressionInfo m_value;

		private DataType m_constantDataType = DataType.String;

		private int m_exprHostID = -1;

		private ExpressionInfo m_omit;

		[NonSerialized]
		private ParamExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal DataType ConstantDataType
		{
			get
			{
				return m_constantDataType;
			}
			set
			{
				m_constantDataType = value;
			}
		}

		internal ExpressionInfo Omit
		{
			get
			{
				return m_omit;
			}
			set
			{
				m_omit = value;
			}
		}

		internal string UniqueName
		{
			get
			{
				return m_uniqueName ?? m_name;
			}
			set
			{
				m_uniqueName = value;
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

		internal ParamExprHost ExprHost
		{
			get
			{
				return m_exprHost;
			}
			set
			{
				m_exprHost = value;
			}
		}

		internal void Initialize(string containerPropertyName, InitializationContext context, bool queryParam)
		{
			string str = (containerPropertyName == null) ? "" : (containerPropertyName + ".");
			if (m_value != null)
			{
				m_value.Initialize(str + "Value", context);
				if (!queryParam)
				{
					context.ExprHostBuilder.GenericValue(m_value);
				}
				else
				{
					context.ExprHostBuilder.QueryParameterValue(m_value);
				}
			}
			if (m_omit != null)
			{
				m_omit.Initialize(str + "Omit", context);
				context.ExprHostBuilder.ParameterOmit(m_omit);
			}
		}

		internal void SetExprHost(IList<ParamExprHost> paramExprHosts, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID >= 0)
			{
				Global.Tracer.Assert(paramExprHosts != null && reportObjectModel != null, "(paramExprHosts != null && reportObjectModel != null)");
				m_exprHost = paramExprHosts[m_exprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal object EvaluateQueryParameterValue(OnDemandProcessingContext odpContext, DataSetExprHost dataSetExprHost)
		{
			return odpContext.ReportRuntime.EvaluateQueryParamValue(m_value, dataSetExprHost?.QueryParametersHost, Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter, m_name);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ParameterValue parameterValue = (ParameterValue)MemberwiseClone();
			if (m_name != null)
			{
				parameterValue.m_name = (string)m_name.Clone();
			}
			if (m_value != null)
			{
				parameterValue.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			parameterValue.m_constantDataType = m_constantDataType;
			if (m_omit != null)
			{
				parameterValue.m_omit = (ExpressionInfo)m_omit.PublishClone(context);
			}
			return parameterValue;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Omit, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)m_constantDataType);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Omit:
					writer.Write(m_omit);
					break;
				case MemberName.UniqueName:
					writer.Write(m_uniqueName);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataType:
					m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Omit:
					m_omit = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UniqueName:
					m_uniqueName = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue;
		}
	}
}
