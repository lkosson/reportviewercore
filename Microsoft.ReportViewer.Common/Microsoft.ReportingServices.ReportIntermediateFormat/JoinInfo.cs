using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class JoinInfo : IPersistable
	{
		protected List<IdcRelationship> m_relationships;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal List<IdcRelationship> Relationships => m_relationships;

		public JoinInfo()
		{
		}

		public JoinInfo(IdcRelationship relationship)
		{
			m_relationships = new List<IdcRelationship>(1);
			m_relationships.Add(relationship);
		}

		public JoinInfo(List<IdcRelationship> relationships)
		{
			m_relationships = relationships;
		}

		internal void SetJoinConditionExprHost(IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
		{
			if (m_relationships == null)
			{
				return;
			}
			foreach (IdcRelationship relationship in m_relationships)
			{
				relationship.SetExprHost(joinConditionExprHost, reportObjectModel);
			}
		}

		protected Relationship GetActiveRelationship(DataSet ourDataSet, DataSet parentDataSet)
		{
			if (m_relationships == null && (ourDataSet == null || ourDataSet.DefaultRelationships == null))
			{
				return null;
			}
			Relationship relationship = FindActiveRelationship(m_relationships, parentDataSet);
			if (relationship == null && ourDataSet != null)
			{
				relationship = ourDataSet.GetDefaultRelationship(parentDataSet);
			}
			return relationship;
		}

		internal static T FindActiveRelationship<T>(List<T> relationships, DataSet parentDataSet) where T : Relationship
		{
			if (relationships != null)
			{
				foreach (T relationship in relationships)
				{
					if (DataSet.AreEqualById(relationship.RelatedDataSet, parentDataSet))
					{
						return relationship;
					}
				}
			}
			return null;
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_relationships == null || 0 >= m_relationships.Count)
			{
				return;
			}
			foreach (IdcRelationship relationship in m_relationships)
			{
				relationship.Initialize(context);
			}
		}

		internal JoinInfo PublishClone(AutomaticSubtotalContext context)
		{
			Global.Tracer.Assert(condition: false, "IDC does not support automatic subtotals");
			throw new InvalidOperationException();
		}

		internal abstract bool ValidateRelationships(ScopeTree scopeTree, ErrorContext errorContext, DataSet ourDataSet, ParentDataSetContainer parentDataSets, IRIFReportDataScope currentScope);

		internal abstract void CheckContainerJoinForNaturalJoin(IRIFDataScope startScope, ErrorContext errorContext, IRIFDataScope scope);

		protected void CheckContainerRelationshipForNaturalJoin(IRIFDataScope startScope, ErrorContext errorContext, IRIFDataScope scope, Relationship outerRelationship)
		{
			if (outerRelationship != null && !outerRelationship.NaturalJoin)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipContainerNotNaturalJoin, Severity.Error, startScope.DataScopeObjectType, startScope.Name, "Relationship", scope.DataScopeObjectType.ToString(), scope.Name);
			}
		}

		internal abstract void ValidateScopeRulesForIdcNaturalJoin(InitializationContext context, IRIFDataScope scope);

		protected void ValidateScopeRulesForIdcNaturalJoin(InitializationContext context, IRIFDataScope startScopeForValidation, Relationship relationship)
		{
			if (relationship != null && relationship.NaturalJoin)
			{
				context.ValidateScopeRulesForIdcNaturalJoin(startScopeForValidation);
			}
		}

		internal abstract void AddMappedFieldIndices(List<int> parentFieldIndices, DataSet parentDataSet, DataSet ourDataSet, List<int> ourFieldIndices);

		protected static void AddMappedFieldIndices(Relationship relationship, List<int> parentFieldIndices, List<int> ourFieldIndices)
		{
			foreach (int parentFieldIndex in parentFieldIndices)
			{
				if (relationship.TryMapFieldIndex(parentFieldIndex, out int foreignKeyFieldIndex))
				{
					ourFieldIndices.Add(foreignKeyFieldIndex);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Relationships, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IdcRelationship));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Relationships)
				{
					writer.Write(m_relationships);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Relationships)
				{
					m_relationships = reader.ReadGenericListOfRIFObjects<IdcRelationship>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo;
		}
	}
}
