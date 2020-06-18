using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class ExpressionInfoTypeValuePair : IPersistable
	{
		private DataType m_constantDataType;

		private ExpressionInfo m_value;

		[NonSerialized]
		private bool m_hadExplicitDataType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal DataType DataType => m_constantDataType;

		internal ExpressionInfo Value => m_value;

		internal bool HadExplicitDataType => m_hadExplicitDataType;

		internal ExpressionInfoTypeValuePair(DataType constantType, bool hadExplicitDataType, ExpressionInfo value)
		{
			m_constantDataType = constantType;
			m_hadExplicitDataType = hadExplicitDataType;
			m_value = value;
		}

		internal ExpressionInfoTypeValuePair()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfoTypeValuePair, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataType:
					writer.WriteEnum((int)m_constantDataType);
					break;
				case MemberName.Value:
					writer.Write(m_value);
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
				case MemberName.DataType:
					m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfoTypeValuePair;
		}
	}
}
