using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class IdcRelationship : Relationship
	{
		private string m_parentScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string ParentScope
		{
			get
			{
				return m_parentScope;
			}
			set
			{
				m_parentScope = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_joinConditions != null)
			{
				JoinConditionInitialize(m_relatedDataSet, context);
			}
		}

		internal bool ValidateLinearRelationship(ErrorContext errorContext, DataSet parentDataSet)
		{
			m_relatedDataSet = parentDataSet;
			m_parentScope = null;
			return true;
		}

		internal bool ValidateIntersectRelationship(ErrorContext errorContext, IRIFDataScope intersectScope, ScopeTree scopeTree)
		{
			IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(intersectScope);
			IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(intersectScope);
			if (ScopeTree.SameScope(m_parentScope, parentRowScopeForIntersection.Name))
			{
				m_relatedDataSet = parentRowScopeForIntersection.DataScopeInfo.DataSet;
				return true;
			}
			if (ScopeTree.SameScope(m_parentScope, parentColumnScopeForIntersection.Name))
			{
				m_relatedDataSet = parentColumnScopeForIntersection.DataScopeInfo.DataSet;
				return true;
			}
			IRIFDataScope parentDataRegion = scopeTree.GetParentDataRegion(intersectScope);
			errorContext.Register(ProcessingErrorCode.rsMissingIntersectionRelationshipParentScope, Severity.Error, intersectScope.DataScopeObjectType, parentDataRegion.Name, "ParentScope", parentDataRegion.DataScopeObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
			return false;
		}

		internal void InsertAggregateIndicatorJoinCondition(Field field, int fieldIndex, Field aggregateIndicatorField, int aggregateIndicatorFieldIndex, InitializationContext context)
		{
			int num = -1;
			if (m_joinConditions != null)
			{
				for (int i = 0; i < m_joinConditions.Count; i++)
				{
					JoinCondition joinCondition = m_joinConditions[i];
					if (joinCondition.ForeignKeyExpression.Type == ExpressionInfo.Types.Field)
					{
						if (joinCondition.ForeignKeyExpression.FieldIndex == fieldIndex)
						{
							num = i;
						}
						else if (joinCondition.ForeignKeyExpression.FieldIndex == aggregateIndicatorFieldIndex)
						{
							return;
						}
					}
				}
			}
			bool flag = num == -1;
			string text = FindRelatedAggregateIndicatorFieldName(field);
			if (text != null)
			{
				ExpressionInfo primaryKey = ExpressionInfo.CreateConstExpression(flag);
				ExpressionInfo expressionInfo = new ExpressionInfo();
				expressionInfo.SetAsSimpleFieldReference(text);
				JoinCondition joinCondition2 = new JoinCondition(expressionInfo, primaryKey, SortDirection.Ascending);
				joinCondition2.Initialize(m_relatedDataSet, m_naturalJoin, context);
				if (flag)
				{
					AddJoinCondition(joinCondition2);
				}
				else
				{
					m_joinConditions.Insert(num, joinCondition2);
				}
			}
		}

		private string FindRelatedAggregateIndicatorFieldName(Field field)
		{
			Field field2 = m_relatedDataSet.Fields.Where((Field f) => f.Name == field.Name).FirstOrDefault();
			if (field2 == null)
			{
				return null;
			}
			if (!field2.HasAggregateIndicatorField)
			{
				return null;
			}
			return m_relatedDataSet.Fields[field2.AggregateIndicatorFieldIndex].Name;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParentScope, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IdcRelationship, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ParentScope)
				{
					writer.Write(m_parentScope);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.ParentScope)
				{
					m_parentScope = reader.ReadString();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IdcRelationship;
		}
	}
}
