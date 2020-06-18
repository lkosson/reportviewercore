using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Variable : IPersistable
	{
		private DataType m_constantDataType;

		private string m_name;

		private ExpressionInfo m_value;

		private int m_sequenceID = -1;

		private bool m_writable;

		[NonSerialized]
		private bool m_isClone;

		[NonSerialized]
		private string m_propertyName;

		[NonSerialized]
		private VariableImpl m_cachedVariableObj;

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

		internal DataType DataType
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

		internal int SequenceID
		{
			get
			{
				return m_sequenceID;
			}
			set
			{
				m_sequenceID = value;
			}
		}

		internal bool Writable
		{
			get
			{
				return m_writable;
			}
			set
			{
				m_writable = value;
			}
		}

		internal Variable()
		{
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_value != null)
			{
				m_value.Initialize(GetPropertyName(), context);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			Variable variable = (Variable)MemberwiseClone();
			variable.SequenceID = context.GenerateVariableSequenceID();
			variable.m_isClone = true;
			if (m_name != null)
			{
				variable.m_name = context.CreateUniqueVariableName(m_name, m_isClone);
			}
			if (m_value != null)
			{
				variable.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			return variable;
		}

		internal string GetPropertyName()
		{
			if (m_propertyName == null)
			{
				StringBuilder stringBuilder = new StringBuilder("Variable");
				stringBuilder.Append("(");
				stringBuilder.Append(m_name);
				stringBuilder.Append(")");
				m_propertyName = stringBuilder.ToString();
			}
			return m_propertyName;
		}

		internal VariableImpl GetCachedVariableObj(OnDemandProcessingContext odpContext)
		{
			if (m_cachedVariableObj == null)
			{
				VariableImpl variableImpl = m_cachedVariableObj = (odpContext.ReportObjectModel.VariablesImpl[m_name] as VariableImpl);
			}
			return m_cachedVariableObj;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.SequenceID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Writable, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
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
				case MemberName.SequenceID:
					writer.Write(m_sequenceID);
					break;
				case MemberName.Writable:
					writer.Write(m_writable);
					break;
				default:
					Global.Tracer.Assert(condition: false);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataType:
					m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.SequenceID:
					m_sequenceID = reader.ReadInt32();
					break;
				case MemberName.Writable:
					m_writable = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable;
		}
	}
}
