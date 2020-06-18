using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class RdlFunctionInfo : IPersistable
	{
		internal enum RdlFunctionType
		{
			MinValue,
			MaxValue,
			ScopeKeys,
			Comparable,
			Array
		}

		private RdlFunctionType m_functionType;

		private List<ExpressionInfo> m_simpleExpressions;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal RdlFunctionType FunctionType
		{
			get
			{
				return m_functionType;
			}
			set
			{
				m_functionType = value;
			}
		}

		internal List<ExpressionInfo> Expressions
		{
			get
			{
				return m_simpleExpressions;
			}
			set
			{
				m_simpleExpressions = value;
			}
		}

		internal void SetFunctionType(string functionName)
		{
			FunctionType = (RdlFunctionType)Enum.Parse(typeof(RdlFunctionType), functionName, ignoreCase: true);
		}

		internal void Initialize(string propertyName, InitializationContext context, bool initializeDataOnError)
		{
			foreach (ExpressionInfo simpleExpression in m_simpleExpressions)
			{
				simpleExpression.Initialize(propertyName, context, initializeDataOnError);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			RdlFunctionInfo rdlFunctionInfo = (RdlFunctionInfo)MemberwiseClone();
			rdlFunctionInfo.m_simpleExpressions = new List<ExpressionInfo>(m_simpleExpressions.Count);
			foreach (ExpressionInfo simpleExpression in m_simpleExpressions)
			{
				rdlFunctionInfo.m_simpleExpressions.Add((ExpressionInfo)simpleExpression.PublishClone(context));
			}
			return rdlFunctionInfo;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RdlFunctionType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Expressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RdlFunctionInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RdlFunctionType:
					writer.WriteEnum((int)m_functionType);
					break;
				case MemberName.Expressions:
					writer.Write(m_simpleExpressions);
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
				case MemberName.RdlFunctionType:
					m_functionType = (RdlFunctionType)reader.ReadEnum();
					break;
				case MemberName.Expressions:
					m_simpleExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RdlFunctionInfo;
		}
	}
}
