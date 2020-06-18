using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Field
	{
		private string m_name;

		private string m_dataField;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		private bool m_dynamicPropertyReferences;

		private FieldPropertyHashtable m_referencedProperties;

		[NonSerialized]
		private CalcFieldExprHost m_exprHost;

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

		internal string DataField
		{
			get
			{
				return m_dataField;
			}
			set
			{
				m_dataField = value;
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

		internal bool IsCalculatedField => m_dataField == null;

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

		internal bool DynamicPropertyReferences
		{
			get
			{
				return m_dynamicPropertyReferences;
			}
			set
			{
				m_dynamicPropertyReferences = value;
			}
		}

		internal FieldPropertyHashtable ReferencedProperties
		{
			get
			{
				return m_referencedProperties;
			}
			set
			{
				m_referencedProperties = value;
			}
		}

		internal CalcFieldExprHost ExprHost => m_exprHost;

		internal void Initialize(InitializationContext context)
		{
			if (Value != null)
			{
				context.ExprHostBuilder.CalcFieldStart(m_name);
				m_value.Initialize("Field", context);
				context.ExprHostBuilder.GenericValue(m_value);
				m_exprHostID = context.ExprHostBuilder.CalcFieldEnd();
			}
		}

		internal void SetExprHost(DataSetExprHost dataSetExprHost, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(dataSetExprHost != null && reportObjectModel != null);
				m_exprHost = dataSetExprHost.FieldHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataField, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DynamicPropertyReferences, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReferencedProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.FieldPropertyHashtable));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
