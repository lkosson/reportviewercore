using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class LinearJoinInfo : JoinInfo
	{
		[Reference]
		private DataSet m_parentDataSet;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal DataSet ParentDataSet => m_parentDataSet;

		public LinearJoinInfo()
		{
		}

		public LinearJoinInfo(IdcRelationship relationship)
			: base(relationship)
		{
		}

		internal override bool ValidateRelationships(ScopeTree scopeTree, ErrorContext errorContext, DataSet ourDataSet, ParentDataSetContainer parentDataSets, IRIFReportDataScope currentScope)
		{
			Global.Tracer.Assert(parentDataSets != null && parentDataSets.Count == 1, "LinearJoinInfo can only be used with exactly one parent data set");
			m_parentDataSet = parentDataSets.ParentDataSet;
			if (DataSet.AreEqualById(ourDataSet, m_parentDataSet))
			{
				return false;
			}
			bool flag = false;
			if (m_relationships != null)
			{
				foreach (IdcRelationship relationship in m_relationships)
				{
					flag |= relationship.ValidateLinearRelationship(errorContext, m_parentDataSet);
				}
			}
			if (flag || ourDataSet.HasDefaultRelationship(m_parentDataSet))
			{
				Relationship activeRelationship = GetActiveRelationship(ourDataSet);
				if (activeRelationship == null)
				{
					RegisterInvalidInnerDataSetNameError(errorContext, ourDataSet, currentScope);
					return false;
				}
				if (activeRelationship.IsCrossJoin && (!activeRelationship.NaturalJoin || ScopeHasParentGroups(currentScope, scopeTree)))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidNaturalCrossJoin, Severity.Error, currentScope.DataScopeObjectType, currentScope.Name, "JoinConditions");
					return false;
				}
				return true;
			}
			RegisterInvalidInnerDataSetNameError(errorContext, ourDataSet, currentScope);
			return false;
		}

		private static bool ScopeHasParentGroups(IRIFReportDataScope currentScope, ScopeTree scopeTree)
		{
			ScopeTree.DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope candidate)
			{
				if (candidate == currentScope)
				{
					return true;
				}
				return (candidate.DataScopeObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.DataShapeMember && candidate.DataScopeObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping) ? true : false;
			};
			return !scopeTree.Traverse(visitor, currentScope);
		}

		private void RegisterInvalidInnerDataSetNameError(ErrorContext errorContext, DataSet ourDataSet, IRIFReportDataScope currentScope)
		{
			Severity severity = Severity.Error;
			if (currentScope is DataRegion)
			{
				severity = Severity.Warning;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidInnerDataSetName, severity, currentScope.DataScopeObjectType, currentScope.Name, "DataSetName", m_parentDataSet.Name.MarkAsPrivate(), ourDataSet.Name.MarkAsPrivate());
		}

		internal Relationship GetActiveRelationship(DataSet ourDataSet)
		{
			return GetActiveRelationship(ourDataSet, m_parentDataSet);
		}

		internal override void CheckContainerJoinForNaturalJoin(IRIFDataScope startScope, ErrorContext errorContext, IRIFDataScope scope)
		{
			CheckContainerRelationshipForNaturalJoin(startScope, errorContext, scope, GetActiveRelationship(scope.DataScopeInfo.DataSet));
		}

		internal override void ValidateScopeRulesForIdcNaturalJoin(InitializationContext context, IRIFDataScope scope)
		{
			Relationship activeRelationship = GetActiveRelationship(scope.DataScopeInfo.DataSet);
			ValidateScopeRulesForIdcNaturalJoin(context, scope, activeRelationship);
		}

		internal override void AddMappedFieldIndices(List<int> parentFieldIndices, DataSet parentDataSet, DataSet ourDataSet, List<int> ourFieldIndices)
		{
			Global.Tracer.Assert(DataSet.AreEqualById(m_parentDataSet, parentDataSet), "Invalid parent data set");
			JoinInfo.AddMappedFieldIndices(GetActiveRelationship(ourDataSet), parentFieldIndices, ourFieldIndices);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParentDataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearJoinInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ParentDataSet)
				{
					writer.WriteReference(m_parentDataSet);
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
				if (memberName == MemberName.ParentDataSet)
				{
					m_parentDataSet = reader.ReadReference<DataSet>(this);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.ParentDataSet)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
					m_parentDataSet = (DataSet)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearJoinInfo;
		}
	}
}
