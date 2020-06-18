using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class RuntimeExpressionInfo : IStorable, IPersistable
	{
		[StaticReference]
		private ExpressionInfo m_expression;

		private bool m_direction = true;

		[StaticReference]
		private IndexedExprHost m_expressionsHost;

		private int m_expressionIndex;

		private static Declaration m_declaration = GetDeclaration();

		internal ExpressionInfo Expression => m_expression;

		internal bool Direction => m_direction;

		internal IndexedExprHost ExpressionsHost => m_expressionsHost;

		internal int ExpressionIndex => m_expressionIndex;

		public int Size => ItemSizes.ReferenceSize + 1 + ItemSizes.ReferenceSize + 4;

		internal RuntimeExpressionInfo()
		{
		}

		internal RuntimeExpressionInfo(List<ExpressionInfo> expressions, IndexedExprHost expressionsHost, List<bool> directions, int expressionIndex)
		{
			m_expressionsHost = expressionsHost;
			m_expressionIndex = expressionIndex;
			m_expression = expressions[m_expressionIndex];
			if (directions != null)
			{
				m_direction = directions[m_expressionIndex];
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Expression:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_expression);
					writer.Write(value2);
					break;
				}
				case MemberName.Direction:
					writer.Write(m_direction);
					break;
				case MemberName.ExpressionsHost:
				{
					int value = scalabilityCache.StoreStaticReference(m_expressionsHost);
					writer.Write(value);
					break;
				}
				case MemberName.ExpressionIndex:
					writer.Write(m_expressionIndex);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Expression:
				{
					int id2 = reader.ReadInt32();
					m_expression = (ExpressionInfo)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.Direction:
					m_direction = reader.ReadBoolean();
					break;
				case MemberName.ExpressionsHost:
				{
					int id = reader.ReadInt32();
					m_expressionsHost = (IndexedExprHost)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ExpressionIndex:
					m_expressionIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Expression, Token.Int32));
				list.Add(new MemberInfo(MemberName.Direction, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ExpressionsHost, Token.Int32));
				list.Add(new MemberInfo(MemberName.ExpressionIndex, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
