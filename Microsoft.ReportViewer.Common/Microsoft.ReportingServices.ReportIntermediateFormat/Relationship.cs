using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class Relationship : IPersistable
	{
		internal sealed class JoinCondition : IPersistable
		{
			private ExpressionInfo m_foreignKeyExpression;

			private ExpressionInfo m_primaryKeyExpression;

			private int m_exprHostID;

			private SortDirection m_sortDirection;

			[NonSerialized]
			private static readonly Declaration m_Declaration = GetDeclaration();

			[NonSerialized]
			private JoinConditionExprHost m_exprHost;

			internal ExpressionInfo ForeignKeyExpression => m_foreignKeyExpression;

			internal ExpressionInfo PrimaryKeyExpression => m_primaryKeyExpression;

			internal SortDirection SortDirection => m_sortDirection;

			internal JoinConditionExprHost ExprHost => m_exprHost;

			internal JoinCondition()
			{
			}

			internal JoinCondition(ExpressionInfo foreignKey, ExpressionInfo primaryKey, SortDirection direction)
			{
				m_foreignKeyExpression = foreignKey;
				m_primaryKeyExpression = primaryKey;
				m_sortDirection = direction;
			}

			internal void Initialize(DataSet relatedDataSet, bool naturalJoin, InitializationContext context)
			{
				context.ExprHostBuilder.JoinConditionStart();
				if (m_foreignKeyExpression != null)
				{
					m_foreignKeyExpression.Initialize("ForeignKey", context);
					context.ExprHostBuilder.JoinConditionForeignKeyExpr(m_foreignKeyExpression);
				}
				if (m_primaryKeyExpression != null)
				{
					context.RegisterDataSet(relatedDataSet);
					m_primaryKeyExpression.Initialize("PrimaryKey", context);
					context.ExprHostBuilder.JoinConditionPrimaryKeyExpr(m_primaryKeyExpression);
					context.UnRegisterDataSet(relatedDataSet);
				}
				m_exprHostID = context.ExprHostBuilder.JoinConditionEnd();
			}

			internal void SetExprHost(IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
			{
				if (m_exprHostID >= 0)
				{
					Global.Tracer.Assert(joinConditionExprHost != null && reportObjectModel != null, "(joinConditionExprHost != null && reportObjectModel != null)");
					m_exprHost = joinConditionExprHost[m_exprHostID];
					m_exprHost.SetReportObjectModel(reportObjectModel);
				}
			}

			internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateForeignKeyExpr(Microsoft.ReportingServices.RdlExpressions.ReportRuntime runtime)
			{
				return runtime.EvaluateJoinConditionForeignKeyExpression(this);
			}

			internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluatePrimaryKeyExpr(Microsoft.ReportingServices.RdlExpressions.ReportRuntime runtime)
			{
				return runtime.EvaluateJoinConditionPrimaryKeyExpression(this);
			}

			internal static Declaration GetDeclaration()
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ForeignKeyExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.PrimaryKeyExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortDirection, Token.Enum));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinCondition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_Declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ForeignKeyExpression:
						writer.Write(m_foreignKeyExpression);
						break;
					case MemberName.PrimaryKeyExpression:
						writer.Write(m_primaryKeyExpression);
						break;
					case MemberName.ExprHostID:
						writer.Write(m_exprHostID);
						break;
					case MemberName.SortDirection:
						writer.WriteEnum((int)m_sortDirection);
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
					case MemberName.ForeignKeyExpression:
						m_foreignKeyExpression = (ExpressionInfo)reader.ReadRIFObject();
						break;
					case MemberName.PrimaryKeyExpression:
						m_primaryKeyExpression = (ExpressionInfo)reader.ReadRIFObject();
						break;
					case MemberName.ExprHostID:
						m_exprHostID = reader.ReadInt32();
						break;
					case MemberName.SortDirection:
						m_sortDirection = (SortDirection)reader.ReadEnum();
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
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinCondition;
			}
		}

		protected List<JoinCondition> m_joinConditions;

		protected bool m_naturalJoin;

		protected DataSet m_relatedDataSet;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool NaturalJoin
		{
			get
			{
				return m_naturalJoin;
			}
			set
			{
				m_naturalJoin = value;
			}
		}

		internal DataSet RelatedDataSet => m_relatedDataSet;

		internal bool IsCrossJoin => JoinConditionCount == 0;

		internal int JoinConditionCount
		{
			get
			{
				if (m_joinConditions != null)
				{
					return m_joinConditions.Count;
				}
				return 0;
			}
		}

		internal void AddJoinCondition(ExpressionInfo foreignKey, ExpressionInfo primaryKey, SortDirection direction)
		{
			AddJoinCondition(new JoinCondition(foreignKey, primaryKey, direction));
		}

		internal void AddJoinCondition(JoinCondition joinCondition)
		{
			if (m_joinConditions == null)
			{
				m_joinConditions = new List<JoinCondition>();
			}
			m_joinConditions.Add(joinCondition);
		}

		internal void JoinConditionInitialize(DataSet relatedDataSet, InitializationContext context)
		{
			for (int i = 0; i < JoinConditionCount; i++)
			{
				m_joinConditions[i].Initialize(relatedDataSet, m_naturalJoin, context);
			}
		}

		internal void SetExprHost(IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
		{
			for (int i = 0; i < JoinConditionCount; i++)
			{
				m_joinConditions[i].SetExprHost(joinConditionExprHost, reportObjectModel);
			}
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult[] EvaluateJoinConditionKeys(bool evaluatePrimaryKeys, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRuntime)
		{
			int joinConditionCount = JoinConditionCount;
			if (joinConditionCount == 0)
			{
				return null;
			}
			Microsoft.ReportingServices.RdlExpressions.VariantResult[] array = new Microsoft.ReportingServices.RdlExpressions.VariantResult[joinConditionCount];
			for (int i = 0; i < joinConditionCount; i++)
			{
				if (evaluatePrimaryKeys)
				{
					array[i] = m_joinConditions[i].EvaluatePrimaryKeyExpr(reportRuntime);
				}
				else
				{
					array[i] = m_joinConditions[i].EvaluateForeignKeyExpr(reportRuntime);
				}
			}
			return array;
		}

		internal ExpressionInfo[] GetForeignKeyExpressions()
		{
			int joinConditionCount = JoinConditionCount;
			if (joinConditionCount == 0)
			{
				return null;
			}
			ExpressionInfo[] array = new ExpressionInfo[joinConditionCount];
			for (int i = 0; i < joinConditionCount; i++)
			{
				array[i] = m_joinConditions[i].ForeignKeyExpression;
			}
			return array;
		}

		internal SortDirection[] GetSortDirections()
		{
			if (JoinConditionCount == 0)
			{
				return null;
			}
			SortDirection[] array = new SortDirection[JoinConditionCount];
			for (int i = 0; i < JoinConditionCount; i++)
			{
				array[i] = m_joinConditions[i].SortDirection;
			}
			return array;
		}

		internal bool TryMapFieldIndex(int primaryKeyFieldIndex, out int foreignKeyFieldIndex)
		{
			if (JoinConditionCount > 0)
			{
				foreach (JoinCondition joinCondition in m_joinConditions)
				{
					if (joinCondition.PrimaryKeyExpression.Type == ExpressionInfo.Types.Field && joinCondition.ForeignKeyExpression.Type == ExpressionInfo.Types.Field && joinCondition.PrimaryKeyExpression.FieldIndex == primaryKeyFieldIndex)
					{
						foreignKeyFieldIndex = joinCondition.ForeignKeyExpression.FieldIndex;
						return true;
					}
				}
			}
			foreignKeyFieldIndex = -1;
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.JoinConditions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinCondition));
			list.Add(new MemberInfo(MemberName.NaturalJoin, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RelatedDataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.JoinConditions:
					writer.Write(m_joinConditions);
					break;
				case MemberName.NaturalJoin:
					writer.Write(m_naturalJoin);
					break;
				case MemberName.RelatedDataSet:
					writer.WriteReference(m_relatedDataSet);
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
				case MemberName.JoinConditions:
					m_joinConditions = reader.ReadGenericListOfRIFObjects<JoinCondition>();
					break;
				case MemberName.NaturalJoin:
					m_naturalJoin = reader.ReadBoolean();
					break;
				case MemberName.RelatedDataSet:
					m_relatedDataSet = reader.ReadReference<DataSet>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.RelatedDataSet)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
					Global.Tracer.Assert(m_relatedDataSet != (DataSet)referenceableItems[item.RefID]);
					m_relatedDataSet = (DataSet)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship;
		}
	}
}
