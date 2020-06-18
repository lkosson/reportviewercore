using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class DataAggregateInfo : IPersistable, IStaticReferenceable
	{
		internal enum AggregateTypes
		{
			First,
			Last,
			Sum,
			Avg,
			Max,
			Min,
			CountDistinct,
			CountRows,
			Count,
			StDev,
			Var,
			StDevP,
			VarP,
			Aggregate,
			Previous,
			Union
		}

		internal class PublishingValidationInfo
		{
			private Microsoft.ReportingServices.ReportProcessing.ObjectType m_objectType;

			private string m_objectName;

			private string m_propertyName;

			private IRIFDataScope m_evaluationScope;

			private List<DataAggregateInfo> m_nestedAggregates;

			private string m_scope;

			private bool m_hasScope;

			private bool m_recursive;

			private int m_aggregateOfAggregatesLevel = -1;

			private bool m_hasAnyFieldReferences;

			internal Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType
			{
				get
				{
					return m_objectType;
				}
				set
				{
					m_objectType = value;
				}
			}

			internal string ObjectName
			{
				get
				{
					return m_objectName;
				}
				set
				{
					m_objectName = value;
				}
			}

			internal string PropertyName
			{
				get
				{
					return m_propertyName;
				}
				set
				{
					m_propertyName = value;
				}
			}

			internal IRIFDataScope EvaluationScope
			{
				get
				{
					return m_evaluationScope;
				}
				set
				{
					m_evaluationScope = value;
				}
			}

			internal List<DataAggregateInfo> NestedAggregates
			{
				get
				{
					return m_nestedAggregates;
				}
				set
				{
					m_nestedAggregates = value;
				}
			}

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

			internal bool HasScope
			{
				get
				{
					return m_hasScope;
				}
				set
				{
					m_hasScope = value;
				}
			}

			internal bool Recursive
			{
				get
				{
					return m_recursive;
				}
				set
				{
					m_recursive = value;
				}
			}

			internal int AggregateOfAggregatesLevel
			{
				get
				{
					return m_aggregateOfAggregatesLevel;
				}
				set
				{
					m_aggregateOfAggregatesLevel = value;
				}
			}

			internal bool HasAnyFieldReferences
			{
				get
				{
					return m_hasAnyFieldReferences;
				}
				set
				{
					m_hasAnyFieldReferences = value;
				}
			}

			internal PublishingValidationInfo PublishClone()
			{
				return (PublishingValidationInfo)MemberwiseClone();
			}
		}

		private string m_name;

		private AggregateTypes m_aggregateType;

		private ExpressionInfo[] m_expressions;

		private List<string> m_duplicateNames;

		private int m_dataSetIndexInCollection = -1;

		private int m_updateScopeID = -1;

		private int m_updateScopeDepth = -1;

		private bool m_updatesAtRowScope;

		[NonSerialized]
		private PublishingValidationInfo m_publishingInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private AggregateParamExprHost[] m_expressionHosts;

		[NonSerialized]
		private bool m_exprHostInitialized;

		[NonSerialized]
		private ObjectModelImpl m_exprHostReportObjectModel;

		[NonSerialized]
		private bool m_hasCachedFieldReferences;

		[NonSerialized]
		private int m_staticId = int.MinValue;

		internal virtual bool MustCopyAggregateResult => false;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal string EvaluationScopeName
		{
			get
			{
				return PublishingInfo.Scope;
			}
			set
			{
				PublishingInfo.Scope = value;
			}
		}

		internal IRIFDataScope EvaluationScope
		{
			get
			{
				return PublishingInfo.EvaluationScope;
			}
			set
			{
				PublishingInfo.EvaluationScope = value;
			}
		}

		internal AggregateTypes AggregateType
		{
			get
			{
				return m_aggregateType;
			}
			set
			{
				m_aggregateType = value;
			}
		}

		internal ExpressionInfo[] Expressions
		{
			get
			{
				return m_expressions;
			}
			set
			{
				m_expressions = value;
			}
		}

		internal List<string> DuplicateNames
		{
			get
			{
				return m_duplicateNames;
			}
			set
			{
				m_duplicateNames = value;
			}
		}

		internal int DataSetIndexInCollection
		{
			get
			{
				return m_dataSetIndexInCollection;
			}
			set
			{
				m_dataSetIndexInCollection = value;
			}
		}

		internal string ExpressionText
		{
			get
			{
				if (m_expressions != null && 1 == m_expressions.Length)
				{
					return m_expressions[0].OriginalText;
				}
				return string.Empty;
			}
		}

		internal string ExpressionTextForCompaction
		{
			get
			{
				if (PublishingInfo.Recursive)
				{
					return ExpressionText + "$Recursive";
				}
				return ExpressionText;
			}
		}

		internal AggregateParamExprHost[] ExpressionHosts => m_expressionHosts;

		internal bool ExprHostInitialized
		{
			get
			{
				return m_exprHostInitialized;
			}
			set
			{
				m_exprHostInitialized = value;
			}
		}

		internal bool Recursive
		{
			get
			{
				return PublishingInfo.Recursive;
			}
			set
			{
				PublishingInfo.Recursive = value;
			}
		}

		internal bool IsAggregateOfAggregate
		{
			get
			{
				if (PublishingInfo.NestedAggregates != null)
				{
					return PublishingInfo.NestedAggregates.Count > 0;
				}
				return false;
			}
		}

		internal int UpdateScopeID
		{
			get
			{
				return m_updateScopeID;
			}
			set
			{
				m_updateScopeID = value;
			}
		}

		internal int UpdateScopeDepth
		{
			get
			{
				return m_updateScopeDepth;
			}
			set
			{
				m_updateScopeDepth = value;
			}
		}

		internal bool UpdatesAtRowScope
		{
			get
			{
				return m_updatesAtRowScope;
			}
			set
			{
				m_updatesAtRowScope = value;
			}
		}

		internal PublishingValidationInfo PublishingInfo
		{
			get
			{
				if (m_publishingInfo == null)
				{
					m_publishingInfo = new PublishingValidationInfo();
				}
				return m_publishingInfo;
			}
		}

		public int ID => m_staticId;

		internal void AddNestedAggregate(DataAggregateInfo agg)
		{
			if (AggregateTypes.Previous != m_aggregateType)
			{
				int num = agg.IsAggregateOfAggregate ? (agg.PublishingInfo.AggregateOfAggregatesLevel + 1) : 0;
				if (num > PublishingInfo.AggregateOfAggregatesLevel)
				{
					PublishingInfo.AggregateOfAggregatesLevel = num;
				}
				if (PublishingInfo.NestedAggregates == null)
				{
					PublishingInfo.NestedAggregates = new List<DataAggregateInfo>();
				}
				PublishingInfo.NestedAggregates.Add(agg);
			}
		}

		internal bool ShouldRecordFieldReferences()
		{
			return !m_hasCachedFieldReferences;
		}

		internal void StoreFieldReferences(OnDemandProcessingContext odpContext, List<string> dataFieldNames)
		{
			m_hasCachedFieldReferences = true;
			odpContext.OdpMetadata.ReportSnapshot.AggregateFieldReferences[m_name] = dataFieldNames;
		}

		public virtual object PublishClone(AutomaticSubtotalContext context)
		{
			DataAggregateInfo dataAggregateInfo = (DataAggregateInfo)MemberwiseClone();
			if (dataAggregateInfo.m_publishingInfo != null)
			{
				dataAggregateInfo.m_publishingInfo = m_publishingInfo.PublishClone();
				dataAggregateInfo.m_publishingInfo.NestedAggregates = null;
			}
			dataAggregateInfo.m_name = context.CreateAggregateID(m_name);
			bool flag = false;
			if (context.OuterAggregate != null)
			{
				flag = true;
				context.OuterAggregate.AddNestedAggregate(dataAggregateInfo);
			}
			if (IsAggregateOfAggregate)
			{
				context.OuterAggregate = dataAggregateInfo;
			}
			if (PublishingInfo.HasScope)
			{
				if (flag)
				{
					dataAggregateInfo.SetScope(context.GetNewScopeNameForInnerOrOuterAggregate(this));
				}
				else
				{
					dataAggregateInfo.SetScope(context.GetNewScopeName(PublishingInfo.Scope));
				}
			}
			if (m_expressions != null)
			{
				dataAggregateInfo.m_expressions = new ExpressionInfo[m_expressions.Length];
				for (int i = 0; i < m_expressions.Length; i++)
				{
					dataAggregateInfo.m_expressions[i] = (ExpressionInfo)m_expressions[i].PublishClone(context);
				}
			}
			return dataAggregateInfo;
		}

		internal virtual string GetAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(m_aggregateType.ToString());
			stringBuilder.Append("(");
			if (m_expressions != null)
			{
				for (int i = 0; i < m_expressions.Length; i++)
				{
					stringBuilder.Append(m_expressions[i].OriginalText);
				}
			}
			if (PublishingInfo.HasScope)
			{
				if (m_expressions != null)
				{
					stringBuilder.Append(", \"");
				}
				stringBuilder.Append(PublishingInfo.Scope);
				stringBuilder.Append("\"");
			}
			if (PublishingInfo.Recursive)
			{
				if (m_expressions != null || PublishingInfo.HasScope)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("Recursive");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal void SetScope(string scope)
		{
			PublishingInfo.HasScope = true;
			PublishingInfo.Scope = scope;
		}

		internal bool GetScope(out string scope)
		{
			scope = PublishingInfo.Scope;
			return PublishingInfo.HasScope;
		}

		internal void SetExprHosts(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			if (!m_exprHostInitialized)
			{
				for (int i = 0; i < m_expressions.Length; i++)
				{
					ExpressionInfo expressionInfo = m_expressions[i];
					if (expressionInfo.ExprHostID >= 0)
					{
						if (m_expressionHosts == null)
						{
							m_expressionHosts = new AggregateParamExprHost[m_expressions.Length];
						}
						AggregateParamExprHost aggregateParamExprHost = reportExprHost.AggregateParamHostsRemotable[expressionInfo.ExprHostID];
						aggregateParamExprHost.SetReportObjectModel(reportObjectModel);
						m_expressionHosts[i] = aggregateParamExprHost;
					}
				}
				m_exprHostInitialized = true;
				m_exprHostReportObjectModel = reportObjectModel;
			}
			else
			{
				if (m_exprHostReportObjectModel == reportObjectModel || m_expressionHosts == null)
				{
					return;
				}
				for (int j = 0; j < m_expressionHosts.Length; j++)
				{
					if (m_expressionHosts[j] != null)
					{
						m_expressionHosts[j].SetReportObjectModel(reportObjectModel);
					}
				}
				m_exprHostReportObjectModel = reportObjectModel;
			}
		}

		internal bool IsPostSortAggregate()
		{
			if (m_aggregateType == AggregateTypes.First || AggregateTypes.Last == m_aggregateType || AggregateTypes.Previous == m_aggregateType)
			{
				return true;
			}
			return false;
		}

		internal virtual bool IsRunningValue()
		{
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.AggregateType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Expressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DuplicateNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.DataSetIndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.UpdateScopeID, Token.Int32));
			list.Add(new MemberInfo(MemberName.UpdateScopeDepth, Token.Int32));
			list.Add(new MemberInfo(MemberName.UpdatesAtRowScope, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.AggregateType:
					writer.WriteEnum((int)m_aggregateType);
					break;
				case MemberName.Expressions:
					writer.Write(m_expressions);
					break;
				case MemberName.DuplicateNames:
					writer.WriteListOfPrimitives(m_duplicateNames);
					break;
				case MemberName.DataSetIndexInCollection:
					writer.Write(m_dataSetIndexInCollection);
					break;
				case MemberName.UpdateScopeID:
					writer.Write(m_updateScopeID);
					break;
				case MemberName.UpdateScopeDepth:
					writer.Write(m_updateScopeDepth);
					break;
				case MemberName.UpdatesAtRowScope:
					writer.Write(m_updatesAtRowScope);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.AggregateType:
					m_aggregateType = (AggregateTypes)reader.ReadEnum();
					break;
				case MemberName.Expressions:
					m_expressions = reader.ReadArrayOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DuplicateNames:
					m_duplicateNames = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.DataSetIndexInCollection:
					m_dataSetIndexInCollection = reader.ReadInt32();
					break;
				case MemberName.UpdateScopeID:
					m_updateScopeID = reader.ReadInt32();
					break;
				case MemberName.UpdateScopeDepth:
					m_updateScopeDepth = reader.ReadInt32();
					break;
				case MemberName.UpdatesAtRowScope:
					m_updatesAtRowScope = reader.ReadBoolean();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo;
		}

		public void SetID(int id)
		{
			m_staticId = id;
		}
	}
}
