using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RunningValueInfo : DataAggregateInfo, IPersistable
	{
		private string m_scope;

		private int m_totalGroupingExpressionCount;

		private bool m_isScopedInEvaluationScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override bool MustCopyAggregateResult => true;

		internal string Scope
		{
			get
			{
				return m_scope;
			}
			set
			{
				m_scope = value;
			}
		}

		internal int TotalGroupingExpressionCount
		{
			get
			{
				return m_totalGroupingExpressionCount;
			}
			set
			{
				m_totalGroupingExpressionCount = value;
			}
		}

		internal bool IsScopedInEvaluationScope
		{
			get
			{
				return m_isScopedInEvaluationScope;
			}
			set
			{
				m_isScopedInEvaluationScope = value;
			}
		}

		internal bool HasDirectFieldReferences
		{
			get
			{
				bool result = false;
				if (base.Expressions != null && base.Expressions.Length != 0)
				{
					for (int i = 0; i < base.Expressions.Length; i++)
					{
						if (base.Expressions[i].HasDirectFieldReferences)
						{
							return true;
						}
					}
				}
				return result;
			}
		}

		internal override bool IsRunningValue()
		{
			return true;
		}

		public override object PublishClone(AutomaticSubtotalContext context)
		{
			RunningValueInfo obj = (RunningValueInfo)base.PublishClone(context);
			obj.m_scope = context.GetNewScopeName(m_scope);
			return obj;
		}

		internal DataAggregateInfo GetAsAggregate()
		{
			DataAggregateInfo dataAggregateInfo = null;
			if (base.AggregateType != AggregateTypes.Previous)
			{
				dataAggregateInfo = new DataAggregateInfo();
				dataAggregateInfo.AggregateType = base.AggregateType;
				dataAggregateInfo.Expressions = base.Expressions;
				dataAggregateInfo.SetScope(m_scope);
			}
			return dataAggregateInfo;
		}

		internal override string GetAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (base.AggregateType)
			{
			case AggregateTypes.CountRows:
				stringBuilder.Append("RowNumber(");
				if (m_scope != null)
				{
					stringBuilder.Append("\"");
					stringBuilder.Append(m_scope);
					stringBuilder.Append("\"");
				}
				break;
			case AggregateTypes.Previous:
				stringBuilder.Append("Previous(");
				if (base.Expressions != null)
				{
					for (int j = 0; j < base.Expressions.Length; j++)
					{
						stringBuilder.Append(base.Expressions[j].OriginalText);
					}
					if (m_scope != null)
					{
						stringBuilder.Append(", \"");
						stringBuilder.Append(m_scope);
						stringBuilder.Append("\"");
					}
				}
				break;
			default:
				stringBuilder.Append("RunningValue(");
				if (base.Expressions != null)
				{
					for (int i = 0; i < base.Expressions.Length; i++)
					{
						stringBuilder.Append(base.Expressions[i].OriginalText);
					}
				}
				stringBuilder.Append(", ");
				stringBuilder.Append(base.AggregateType.ToString());
				if (m_scope != null)
				{
					stringBuilder.Append(", \"");
					stringBuilder.Append(m_scope);
					stringBuilder.Append("\"");
				}
				break;
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal void Initialize(InitializationContext context, string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (base.Expressions == null || base.Expressions.Length == 0)
			{
				return;
			}
			for (int i = 0; i < base.Expressions.Length; i++)
			{
				ExpressionInfo expressionInfo = base.Expressions[i];
				if (base.AggregateType == AggregateTypes.Previous && m_scope != null && expressionInfo.Aggregates != null)
				{
					foreach (DataAggregateInfo aggregate in expressionInfo.Aggregates)
					{
						if (aggregate.GetScope(out string scope) && !context.IsSameOrChildScope(m_scope, scope))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidScopeInInnerAggregateOfPreviousAggregate, Severity.Error, objectType, objectName, propertyName);
						}
					}
				}
				expressionInfo.AggregateInitialize(dataSetName, objectType, objectName, propertyName, context);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Scope, Token.String));
			list.Add(new MemberInfo(MemberName.TotalGroupingExpressionCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.IsScopedInEvaluationScope, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Scope:
					writer.Write(m_scope);
					break;
				case MemberName.TotalGroupingExpressionCount:
					writer.Write(m_totalGroupingExpressionCount);
					break;
				case MemberName.IsScopedInEvaluationScope:
					writer.Write(m_isScopedInEvaluationScope);
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
				case MemberName.Scope:
					m_scope = reader.ReadString();
					break;
				case MemberName.TotalGroupingExpressionCount:
					m_totalGroupingExpressionCount = reader.ReadInt32();
					break;
				case MemberName.IsScopedInEvaluationScope:
					m_isScopedInEvaluationScope = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo;
		}
	}
}
