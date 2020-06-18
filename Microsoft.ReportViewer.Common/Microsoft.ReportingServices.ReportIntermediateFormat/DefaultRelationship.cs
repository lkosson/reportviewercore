using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DefaultRelationship : Relationship
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private string m_relatedDataSetName;

		internal string RelatedDataSetName
		{
			get
			{
				return m_relatedDataSetName;
			}
			set
			{
				m_relatedDataSetName = value;
			}
		}

		internal void Initialize(DataSet thisDataSet, InitializationContext context)
		{
			if (m_relatedDataSet == null)
			{
				Global.Tracer.Assert(context.ErrorContext.HasError, "Expected error not found. BindAndValidate should have been called before Initialize");
			}
			else
			{
				JoinConditionInitialize(m_relatedDataSet, context);
			}
		}

		internal void BindAndValidate(DataSet thisDataSet, ErrorContext errorContext, Report report)
		{
			if (m_joinConditions == null && !m_naturalJoin)
			{
				errorContext.Register(ProcessingErrorCode.rsMissingDefaultRelationshipJoinCondition, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", m_relatedDataSetName.MarkAsPrivate());
				return;
			}
			if (!report.MappingNameToDataSet.TryGetValue(m_relatedDataSetName, out m_relatedDataSet))
			{
				errorContext.Register(ProcessingErrorCode.rsNonExistingRelationshipRelatedScope, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", "RelatedDataSet", m_relatedDataSetName.MarkAsPrivate());
				return;
			}
			if (thisDataSet.ID == m_relatedDataSet.ID)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidSelfJoinRelationship, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", "RelatedDataSet", m_relatedDataSetName.MarkAsPrivate());
			}
			if (!m_naturalJoin)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDefaultRelationshipNotNaturalJoin, Severity.Error, thisDataSet.ObjectType, thisDataSet.Name, "DefaultRelationship", "NaturalJoin");
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DefaultRelationship, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DefaultRelationship;
		}
	}
}
