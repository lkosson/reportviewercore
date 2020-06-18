using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataAggregateObj : IErrorContext, IStorable, IPersistable
	{
		private bool m_nonAggregateMode;

		private DataAggregate m_aggregator;

		[StaticReference]
		private DataAggregateInfo m_aggregateDef;

		[StaticReference]
		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private bool m_usedInExpression;

		private DataAggregateObjResult m_aggregateResult;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal string Name => m_aggregateDef.Name;

		internal List<string> DuplicateNames => m_aggregateDef.DuplicateNames;

		internal bool NonAggregateMode => m_nonAggregateMode;

		internal DataAggregateInfo AggregateDef => m_aggregateDef;

		internal bool UsedInExpression
		{
			get
			{
				return m_usedInExpression;
			}
			set
			{
				m_usedInExpression = value;
			}
		}

		public int Size => 1 + ItemSizes.SizeOf(m_aggregator) + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + 1 + ItemSizes.SizeOf(m_aggregateResult);

		internal DataAggregateObj()
		{
		}

		internal DataAggregateObj(DataAggregateInfo aggInfo, OnDemandProcessingContext odpContext)
		{
			m_nonAggregateMode = false;
			m_aggregator = AggregatorFactory.Instance.CreateAggregator(odpContext, aggInfo);
			m_aggregateDef = aggInfo;
			m_reportRT = odpContext.ReportRuntime;
			if (m_reportRT.ReportExprHost != null)
			{
				m_aggregateDef.SetExprHosts(m_reportRT.ReportExprHost, odpContext.ReportObjectModel);
			}
			Init();
		}

		internal DataAggregateObj(DataAggregateInfo aggrDef, DataAggregateObjResult aggrResult)
		{
			m_nonAggregateMode = true;
			m_aggregateDef = aggrDef;
			m_aggregateResult = aggrResult;
		}

		internal void Init()
		{
			if (!m_nonAggregateMode)
			{
				m_aggregator.Init();
				m_aggregateResult = new DataAggregateObjResult();
			}
		}

		internal void ResetForNoRows()
		{
			if (m_nonAggregateMode)
			{
				m_aggregateResult = new DataAggregateObjResult();
				m_aggregateResult.Value = AggregatorFactory.Instance.GetNoRowsResult(m_aggregateDef);
			}
			else
			{
				Init();
			}
		}

		internal void Update()
		{
			if (m_aggregateResult.ErrorOccurred || m_nonAggregateMode)
			{
				return;
			}
			if (m_aggregateDef.ShouldRecordFieldReferences())
			{
				m_reportRT.ReportObjectModel.FieldsImpl.ResetFieldsUsedInExpression();
			}
			m_aggregateResult.ErrorOccurred = EvaluateParameters(out object[] values, out DataFieldStatus fieldStatus);
			if (fieldStatus != 0)
			{
				m_aggregateResult.HasCode = true;
				m_aggregateResult.FieldStatus = fieldStatus;
			}
			if (m_aggregateDef.ShouldRecordFieldReferences())
			{
				List<string> list = new List<string>();
				m_reportRT.ReportObjectModel.FieldsImpl.AddFieldsUsedInExpression(list);
				m_aggregateDef.StoreFieldReferences(m_reportRT.ReportObjectModel.OdpContext, list);
			}
			if (!m_aggregateResult.ErrorOccurred)
			{
				try
				{
					m_aggregator.Update(values, this);
				}
				catch (ReportProcessingException)
				{
					m_aggregateResult.ErrorOccurred = true;
				}
			}
		}

		internal DataAggregateObjResult AggregateResult()
		{
			if (!m_nonAggregateMode && !m_aggregateResult.ErrorOccurred)
			{
				try
				{
					m_aggregateResult.Value = m_aggregator.Result();
				}
				catch (ReportProcessingException)
				{
					m_aggregateResult.ErrorOccurred = true;
					m_aggregateResult.Value = null;
				}
			}
			if (m_aggregateDef.MustCopyAggregateResult)
			{
				return new DataAggregateObjResult(m_aggregateResult);
			}
			return m_aggregateResult;
		}

		internal bool EvaluateParameters(out object[] values, out DataFieldStatus fieldStatus)
		{
			bool flag = false;
			fieldStatus = DataFieldStatus.None;
			values = new object[m_aggregateDef.Expressions.Length];
			for (int i = 0; i < m_aggregateDef.Expressions.Length; i++)
			{
				try
				{
					Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = m_reportRT.EvaluateAggregateVariantOrBinaryParamExpr(m_aggregateDef, i, this);
					values[i] = variantResult.Value;
					flag |= variantResult.ErrorOccurred;
					if (variantResult.FieldStatus != 0)
					{
						fieldStatus = variantResult.FieldStatus;
					}
				}
				catch (ReportProcessingException_MissingAggregateDependency)
				{
					if (m_aggregateDef.AggregateType == DataAggregateInfo.AggregateTypes.Previous)
					{
						values[i] = null;
						fieldStatus = DataFieldStatus.None;
						return false;
					}
					Global.Tracer.Assert(condition: false, "Unfulfilled aggregate dependency outside of a previous");
					throw;
				}
			}
			return flag;
		}

		internal void Set(DataAggregateObjResult aggregateResult)
		{
			m_nonAggregateMode = true;
			m_aggregateResult = aggregateResult;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (!m_aggregateResult.HasCode)
			{
				m_aggregateResult.HasCode = true;
				m_aggregateResult.Code = code;
				m_aggregateResult.Severity = severity;
				m_aggregateResult.Arguments = arguments;
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (!m_aggregateResult.HasCode)
			{
				m_aggregateResult.HasCode = true;
				m_aggregateResult.Code = code;
				m_aggregateResult.Severity = severity;
				m_aggregateResult.Arguments = arguments;
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
				case MemberName.NonAggregateMode:
					writer.Write(m_nonAggregateMode);
					break;
				case MemberName.Aggregator:
					writer.Write(m_aggregator);
					break;
				case MemberName.AggregateDef:
					writer.Write(scalabilityCache.StoreStaticReference(m_aggregateDef));
					break;
				case MemberName.ReportRuntime:
				{
					int value = scalabilityCache.StoreStaticReference(m_reportRT);
					writer.Write(value);
					break;
				}
				case MemberName.UsedInExpression:
					writer.Write(m_usedInExpression);
					break;
				case MemberName.AggregateResult:
					writer.Write(m_aggregateResult);
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
				case MemberName.NonAggregateMode:
					m_nonAggregateMode = reader.ReadBoolean();
					break;
				case MemberName.Aggregator:
					m_aggregator = (DataAggregate)reader.ReadRIFObject();
					break;
				case MemberName.AggregateDef:
				{
					int id2 = reader.ReadInt32();
					m_aggregateDef = (DataAggregateInfo)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.ReportRuntime:
				{
					int id = reader.ReadInt32();
					m_reportRT = (Microsoft.ReportingServices.RdlExpressions.ReportRuntime)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.UsedInExpression:
					m_usedInExpression = reader.ReadBoolean();
					break;
				case MemberName.AggregateResult:
					m_aggregateResult = (DataAggregateObjResult)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.NonAggregateMode, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Aggregator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate));
				list.Add(new MemberInfo(MemberName.AggregateDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.ReportRuntime, Token.Int32));
				list.Add(new MemberInfo(MemberName.UsedInExpression, Token.Boolean));
				list.Add(new MemberInfo(MemberName.AggregateResult, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
