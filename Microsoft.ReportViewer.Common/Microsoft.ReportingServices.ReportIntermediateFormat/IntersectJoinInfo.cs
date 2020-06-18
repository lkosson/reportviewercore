using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class IntersectJoinInfo : JoinInfo
	{
		[Reference]
		private DataSet m_rowParentDataSet;

		[Reference]
		private DataSet m_columnParentDataSet;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal DataSet RowParentDataSet => m_rowParentDataSet;

		internal DataSet ColumnParentDataSet => m_columnParentDataSet;

		public IntersectJoinInfo()
		{
		}

		public IntersectJoinInfo(List<IdcRelationship> relationships)
			: base(relationships)
		{
		}

		internal Relationship GetActiveRowRelationship(DataSet ourDataSet)
		{
			return GetActiveRelationship(ourDataSet, m_rowParentDataSet);
		}

		internal Relationship GetActiveColumnRelationship(DataSet ourDataSet)
		{
			return GetActiveRelationship(ourDataSet, m_columnParentDataSet);
		}

		internal override bool ValidateRelationships(ScopeTree scopeTree, ErrorContext errorContext, DataSet ourDataSet, ParentDataSetContainer parentDataSets, IRIFReportDataScope currentScope)
		{
			Global.Tracer.Assert(parentDataSets != null, "IntersectJoinInfo can only be used with one or two parent data sets");
			if (parentDataSets.Count == 1)
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
				errorContext.Register(ProcessingErrorCode.rsUnexpectedCellDataSetName, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "DataSetName", parentDataRegion.ObjectType.ToString());
				return false;
			}
			if (parentDataSets.AreAllSameDataSet() && DataSet.AreEqualById(parentDataSets.RowParentDataSet, ourDataSet))
			{
				return false;
			}
			m_rowParentDataSet = parentDataSets.RowParentDataSet;
			m_columnParentDataSet = parentDataSets.ColumnParentDataSet;
			if (m_rowParentDataSet == null || m_columnParentDataSet == null)
			{
				return false;
			}
			bool dataSetAlreadyHasRelationship = false;
			bool dataSetAlreadyHasRelationship2 = false;
			if (m_relationships != null)
			{
				foreach (IdcRelationship relationship in m_relationships)
				{
					if (relationship.ValidateIntersectRelationship(errorContext, currentScope, scopeTree))
					{
						CheckRelationshipDataSetBinding(scopeTree, errorContext, currentScope, relationship, m_rowParentDataSet, ref dataSetAlreadyHasRelationship);
						CheckRelationshipDataSetBinding(scopeTree, errorContext, currentScope, relationship, m_columnParentDataSet, ref dataSetAlreadyHasRelationship2);
						continue;
					}
					return false;
				}
			}
			dataSetAlreadyHasRelationship = HasRelationshipOrDefaultForDataSet(scopeTree, errorContext, currentScope, ourDataSet, m_rowParentDataSet, dataSetAlreadyHasRelationship);
			dataSetAlreadyHasRelationship2 = HasRelationshipOrDefaultForDataSet(scopeTree, errorContext, currentScope, ourDataSet, m_columnParentDataSet, dataSetAlreadyHasRelationship2);
			if (!dataSetAlreadyHasRelationship || !dataSetAlreadyHasRelationship2)
			{
				return false;
			}
			DataRegion parentDataRegion2 = scopeTree.GetParentDataRegion(currentScope);
			if (ValidateCellBoundTotheSameDataSetAsParentScpoe(m_columnParentDataSet, m_rowParentDataSet, ourDataSet, parentDataRegion2.IsColumnGroupingSwitched))
			{
				IRIFDataScope parentDataRegion3 = scopeTree.GetParentDataRegion(currentScope);
				IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(currentScope);
				IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(currentScope);
				if (parentDataRegion2.IsColumnGroupingSwitched)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidIntersectionNaturalJoin, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion3.Name, "ParentScope", parentDataRegion3.DataScopeObjectType.ToString(), parentColumnScopeForIntersection.Name, parentRowScopeForIntersection.Name);
					return false;
				}
				errorContext.Register(ProcessingErrorCode.rsInvalidIntersectionNaturalJoin, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion3.Name, "ParentScope", parentDataRegion3.DataScopeObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
				return false;
			}
			return true;
		}

		private bool ValidateCellBoundTotheSameDataSetAsParentScpoe(DataSet columnParentDataSet, DataSet rowParentDataSet, DataSet ourDataSet, bool isColumnGroupingSwitched)
		{
			if (isColumnGroupingSwitched)
			{
				if (DataSet.AreEqualById(m_rowParentDataSet, ourDataSet))
				{
					return GetActiveColumnRelationship(ourDataSet).NaturalJoin;
				}
				return false;
			}
			if (DataSet.AreEqualById(m_columnParentDataSet, ourDataSet))
			{
				return GetActiveRowRelationship(ourDataSet).NaturalJoin;
			}
			return false;
		}

		private bool HasRelationshipOrDefaultForDataSet(ScopeTree scopeTree, ErrorContext errorContext, IRIFReportDataScope currentScope, DataSet ourDataSet, DataSet parentDataSet, bool hasValidRelationship)
		{
			if (DataSet.AreEqualById(parentDataSet, ourDataSet))
			{
				return true;
			}
			if (hasValidRelationship || ourDataSet.HasDefaultRelationship(parentDataSet))
			{
				Relationship activeRelationship = GetActiveRelationship(ourDataSet, parentDataSet);
				if (activeRelationship == null)
				{
					RegisterInvalidCellDataSetNameError(scopeTree, errorContext, currentScope, ourDataSet, parentDataSet);
					return false;
				}
				if (activeRelationship.IsCrossJoin)
				{
					DataRegion parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
					errorContext.Register(ProcessingErrorCode.rsInvalidIntersectionNaturalCrossJoin, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "JoinConditions", parentDataRegion.ObjectType.ToString());
					return false;
				}
				return true;
			}
			RegisterInvalidCellDataSetNameError(scopeTree, errorContext, currentScope, ourDataSet, parentDataSet);
			return false;
		}

		private static void RegisterInvalidCellDataSetNameError(ScopeTree scopeTree, ErrorContext errorContext, IRIFReportDataScope currentScope, DataSet ourDataSet, DataSet parentDataSet)
		{
			DataRegion parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
			errorContext.Register(ProcessingErrorCode.rsInvalidCellDataSetName, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "DataSetName", parentDataSet.Name.MarkAsPrivate(), ourDataSet.Name.MarkAsPrivate(), parentDataRegion.ObjectType.ToString());
		}

		private void CheckRelationshipDataSetBinding(ScopeTree scopeTree, ErrorContext errorContext, IRIFReportDataScope currentScope, IdcRelationship relationship, DataSet parentDataSetCandidate, ref bool dataSetAlreadyHasRelationship)
		{
			if (DataSet.AreEqualById(relationship.RelatedDataSet, parentDataSetCandidate))
			{
				if (dataSetAlreadyHasRelationship)
				{
					IRIFDataScope parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
					IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(currentScope);
					IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(currentScope);
					errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipDuplicateParentScope, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "ParentScope", parentDataRegion.DataScopeObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
				}
				dataSetAlreadyHasRelationship = true;
			}
		}

		internal override void CheckContainerJoinForNaturalJoin(IRIFDataScope startScope, ErrorContext errorContext, IRIFDataScope scope)
		{
			DataSet dataSet = scope.DataScopeInfo.DataSet;
			CheckContainerRelationshipForNaturalJoin(startScope, errorContext, scope, GetActiveRowRelationship(dataSet));
			CheckContainerRelationshipForNaturalJoin(startScope, errorContext, scope, GetActiveColumnRelationship(dataSet));
		}

		internal override void ValidateScopeRulesForIdcNaturalJoin(InitializationContext context, IRIFDataScope scope)
		{
			DataSet dataSet = scope.DataScopeInfo.DataSet;
			ValidateScopeRulesForIdcNaturalJoin(context, context.ScopeTree.GetParentRowScopeForIntersection(scope), GetActiveRowRelationship(dataSet));
			ValidateScopeRulesForIdcNaturalJoin(context, context.ScopeTree.GetParentColumnScopeForIntersection(scope), GetActiveColumnRelationship(dataSet));
		}

		internal override void AddMappedFieldIndices(List<int> parentFieldIndices, DataSet parentDataSet, DataSet ourDataSet, List<int> ourFieldIndices)
		{
			Relationship relationship;
			if (DataSet.AreEqualById(m_rowParentDataSet, parentDataSet))
			{
				relationship = GetActiveRowRelationship(ourDataSet);
			}
			else if (DataSet.AreEqualById(m_columnParentDataSet, parentDataSet))
			{
				relationship = GetActiveColumnRelationship(ourDataSet);
			}
			else
			{
				Global.Tracer.Assert(condition: false, "Invalid parent data set");
				relationship = null;
			}
			JoinInfo.AddMappedFieldIndices(relationship, parentFieldIndices, ourFieldIndices);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RowParentDataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.ColumnParentDataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IntersectJoinInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowParentDataSet:
					writer.WriteReference(m_rowParentDataSet);
					break;
				case MemberName.ColumnParentDataSet:
					writer.WriteReference(m_columnParentDataSet);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowParentDataSet:
					m_rowParentDataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.ColumnParentDataSet:
					m_columnParentDataSet = reader.ReadReference<DataSet>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
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
				switch (item.MemberName)
				{
				case MemberName.RowParentDataSet:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
					m_rowParentDataSet = (DataSet)referenceableItems[item.RefID];
					break;
				case MemberName.ColumnParentDataSet:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
					m_columnParentDataSet = (DataSet)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IntersectJoinInfo;
		}
	}
}
