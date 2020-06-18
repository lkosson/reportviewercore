using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ExpressionInfo : IPersistable, IStaticReferenceable
	{
		internal enum Types
		{
			Expression,
			Field,
			Aggregate,
			Constant,
			Token,
			Lookup_OneValue,
			Lookup_MultiValue,
			RdlFunction,
			ScopedFieldReference,
			Literal
		}

		private class TransformedExprSpecialFunctionInfo
		{
			internal enum SpecialFunctionType
			{
				Aggregate,
				RunningValue,
				Lookup
			}

			internal SpecialFunctionType FunctionType;

			internal int Position;

			internal string FunctionID;

			internal int IndexIntoCollection;

			internal TransformedExprSpecialFunctionInfo(int position, string functionID, SpecialFunctionType functionType, int indexIntoCollection)
			{
				FunctionType = functionType;
				Position = position;
				FunctionID = functionID;
				IndexIntoCollection = indexIntoCollection;
			}

			internal object PublishClone(AutomaticSubtotalContext context)
			{
				TransformedExprSpecialFunctionInfo obj = (TransformedExprSpecialFunctionInfo)MemberwiseClone();
				obj.FunctionID = (string)FunctionID.Clone();
				return obj;
			}
		}

		private Types m_type;

		private DataType m_constantType = DataType.String;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

		private DateTime m_dateTimeValue;

		private DateTimeOffset? m_dateTimeOffsetValue;

		private double m_floatValue;

		private int m_exprHostID = -1;

		private string m_originalText;

		private bool m_inPrevious;

		private RdlFunctionInfo m_rdlFunctionInfo;

		private ScopedFieldInfo m_scopedFieldInfo;

		[NonSerialized]
		private LiteralInfo m_literalInfo;

		[NonSerialized]
		private string m_transformedExpression;

		[NonSerialized]
		private List<TransformedExprSpecialFunctionInfo> m_transformedExprAggregateInfos;

		[NonSerialized]
		private List<string> m_referencedFields;

		[NonSerialized]
		private List<string> m_referencedReportItems;

		[NonSerialized]
		private List<int> m_referencedReportItemPositionsInTransformedExpression;

		[NonSerialized]
		private List<int> m_referencedReportItemPositionsInOriginalText;

		[NonSerialized]
		private List<string> m_referencedVariables;

		[NonSerialized]
		private List<int> m_referencedVariablePositions;

		[NonSerialized]
		private List<string> m_referencedParameters;

		[NonSerialized]
		private string m_simpleParameterName;

		[NonSerialized]
		private bool m_hasDynamicParameterReference;

		[NonSerialized]
		private List<string> m_referencedDataSets;

		[NonSerialized]
		private List<string> m_referencedDataSources;

		[NonSerialized]
		private List<ScopeReference> m_referencedScopes;

		[NonSerialized]
		private List<DataAggregateInfo> m_aggregates;

		[NonSerialized]
		private List<RunningValueInfo> m_runningValues;

		[NonSerialized]
		private List<LookupInfo> m_lookups;

		[NonSerialized]
		private int m_compileTimeID = -1;

		[NonSerialized]
		private Hashtable m_referencedFieldProperties;

		[NonSerialized]
		private bool m_dynamicFieldReferences;

		[NonSerialized]
		private bool m_referencedOverallPageGlobals;

		[NonSerialized]
		private bool m_hasAnyFieldReferences;

		[NonSerialized]
		private bool m_referencedPageGlobals;

		[NonSerialized]
		private List<int> m_meDotValuePositionsInOriginalText;

		[NonSerialized]
		private List<int> m_meDotValuePositionsInTranformedExpr;

		[NonSerialized]
		private bool m_nullLevelInExpr;

		[NonSerialized]
		private int m_id = int.MinValue;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool IsExpression => m_type != Types.Constant;

		internal DataType ConstantType
		{
			get
			{
				return m_constantType;
			}
			set
			{
				m_constantType = value;
			}
		}

		internal TypeCode ConstantTypeCode
		{
			get
			{
				if (!IsExpression)
				{
					switch (m_constantType)
					{
					case DataType.Boolean:
						return TypeCode.Boolean;
					case DataType.DateTime:
						return TypeCode.DateTime;
					case DataType.Float:
						return TypeCode.Double;
					case DataType.Integer:
						return TypeCode.Int32;
					default:
						return TypeCode.String;
					}
				}
				return TypeCode.Object;
			}
		}

		internal Types Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal string StringValue
		{
			get
			{
				return m_stringValue;
			}
			set
			{
				m_stringValue = value;
			}
		}

		internal object Value
		{
			get
			{
				if (!IsExpression)
				{
					switch (m_constantType)
					{
					case DataType.Boolean:
						return m_boolValue;
					case DataType.DateTime:
						return GetDateTimeValue();
					case DataType.Float:
						return m_floatValue;
					case DataType.Integer:
						return m_intValue;
					case DataType.String:
						return m_stringValue;
					}
				}
				return null;
			}
		}

		internal bool BoolValue
		{
			get
			{
				return m_boolValue;
			}
			set
			{
				m_boolValue = value;
			}
		}

		internal int IntValue
		{
			get
			{
				return m_intValue;
			}
			set
			{
				m_intValue = value;
			}
		}

		internal double FloatValue
		{
			get
			{
				return m_floatValue;
			}
			set
			{
				m_floatValue = value;
			}
		}

		internal string OriginalText
		{
			get
			{
				return m_originalText;
			}
			set
			{
				m_originalText = value;
			}
		}

		internal string TransformedExpression
		{
			get
			{
				return m_transformedExpression;
			}
			set
			{
				m_transformedExpression = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal int CompileTimeID
		{
			get
			{
				return m_compileTimeID;
			}
			set
			{
				m_compileTimeID = value;
			}
		}

		internal List<DataAggregateInfo> Aggregates => m_aggregates;

		internal List<RunningValueInfo> RunningValues => m_runningValues;

		internal List<LookupInfo> Lookups => m_lookups;

		internal bool InPrevious
		{
			get
			{
				return m_inPrevious;
			}
			set
			{
				m_inPrevious = value;
			}
		}

		internal Hashtable ReferencedFieldProperties => m_referencedFieldProperties;

		internal List<string> ReferencedReportItems => m_referencedReportItems;

		internal bool DynamicFieldReferences
		{
			get
			{
				return m_dynamicFieldReferences;
			}
			set
			{
				m_dynamicFieldReferences = value;
			}
		}

		internal bool ReferencedOverallPageGlobals
		{
			get
			{
				return m_referencedOverallPageGlobals;
			}
			set
			{
				m_referencedOverallPageGlobals = value;
			}
		}

		internal bool ReferencedPageGlobals
		{
			get
			{
				return m_referencedPageGlobals;
			}
			set
			{
				m_referencedPageGlobals = value;
			}
		}

		internal bool MeDotValueDetected
		{
			get
			{
				if (m_meDotValuePositionsInOriginalText == null || m_meDotValuePositionsInOriginalText.Count <= 0)
				{
					if (m_meDotValuePositionsInTranformedExpr != null)
					{
						return m_meDotValuePositionsInTranformedExpr.Count > 0;
					}
					return false;
				}
				return true;
			}
		}

		internal bool NullLevelDetected
		{
			get
			{
				return m_nullLevelInExpr;
			}
			set
			{
				m_nullLevelInExpr = value;
			}
		}

		internal bool HasDirectFieldReferences
		{
			get
			{
				if (m_referencedFields != null)
				{
					return m_referencedFields.Count > 0;
				}
				return false;
			}
		}

		internal bool HasAnyFieldReferences
		{
			get
			{
				if (m_hasAnyFieldReferences)
				{
					return true;
				}
				if (m_rdlFunctionInfo != null)
				{
					foreach (ExpressionInfo expression in m_rdlFunctionInfo.Expressions)
					{
						if (expression.HasAnyFieldReferences)
						{
							return true;
						}
					}
				}
				return false;
			}
			set
			{
				m_hasAnyFieldReferences = value;
			}
		}

		internal string SimpleParameterName
		{
			get
			{
				return m_simpleParameterName;
			}
			set
			{
				m_simpleParameterName = value;
			}
		}

		internal bool HasDynamicParameterReference
		{
			get
			{
				return m_hasDynamicParameterReference;
			}
			set
			{
				m_hasDynamicParameterReference = value;
			}
		}

		internal List<string> ReferencedParameters => m_referencedParameters;

		internal int FieldIndex => IntValue;

		internal RdlFunctionInfo RdlFunctionInfo
		{
			get
			{
				return m_rdlFunctionInfo;
			}
			set
			{
				m_rdlFunctionInfo = value;
			}
		}

		internal ScopedFieldInfo ScopedFieldInfo
		{
			get
			{
				return m_scopedFieldInfo;
			}
			set
			{
				m_scopedFieldInfo = value;
			}
		}

		internal LiteralInfo LiteralInfo
		{
			get
			{
				return m_literalInfo;
			}
			set
			{
				m_literalInfo = value;
			}
		}

		public int ID => m_id;

		internal ExpressionInfo()
		{
		}

		internal static ExpressionInfo CreateConstExpression(string value)
		{
			return new ExpressionInfo
			{
				Type = Types.Constant,
				ConstantType = DataType.String,
				OriginalText = value,
				StringValue = value
			};
		}

		internal static ExpressionInfo CreateConstExpression(bool value)
		{
			return new ExpressionInfo
			{
				Type = Types.Constant,
				ConstantType = DataType.Boolean,
				OriginalText = value.ToString(CultureInfo.InvariantCulture),
				BoolValue = value
			};
		}

		internal static ExpressionInfo CreateConstExpression(int value)
		{
			return new ExpressionInfo
			{
				Type = Types.Constant,
				ConstantType = DataType.Integer,
				OriginalText = value.ToString(CultureInfo.InvariantCulture),
				IntValue = value
			};
		}

		internal static ExpressionInfo CreateEmptyExpression()
		{
			return new ExpressionInfo
			{
				Type = Types.Expression,
				OriginalText = null,
				StringValue = null
			};
		}

		internal object GetDateTimeValue()
		{
			if (m_dateTimeOffsetValue.HasValue)
			{
				return m_dateTimeOffsetValue;
			}
			return m_dateTimeValue;
		}

		internal void SetDateTimeValue(DateTime dateTime)
		{
			m_dateTimeValue = dateTime;
		}

		internal void SetDateTimeValue(DateTimeOffset dateTimeOffset)
		{
			m_dateTimeOffsetValue = dateTimeOffset;
		}

		internal void Initialize(string propertyName, InitializationContext context)
		{
			Initialize(propertyName, context, initializeDataOnError: true);
		}

		internal void Initialize(string propertyName, InitializationContext context, bool initializeDataOnError)
		{
			context.EnforceRdlSandboxContentRestrictions(this, propertyName);
			context.CheckVariableReferences(m_referencedVariables, propertyName);
			context.CheckFieldReferences(m_referencedFields, propertyName);
			context.CheckReportItemReferences(m_referencedReportItems, propertyName);
			context.CheckReportParameterReferences(m_referencedParameters, propertyName);
			context.CheckDataSetReference(m_referencedDataSets, propertyName);
			context.CheckDataSourceReference(m_referencedDataSources, propertyName);
			context.CheckScopeReferences(m_referencedScopes, propertyName);
			if (Type == Types.ScopedFieldReference)
			{
				ScopedFieldInfo.FieldIndex = context.ResolveScopedFieldReferenceToIndex(StringValue, ScopedFieldInfo.FieldName);
			}
			if (!context.ErrorContext.HasError || initializeDataOnError)
			{
				context.FillInFieldIndex(this);
				context.TransferAggregates(m_aggregates, propertyName);
				context.TransferRunningValues(m_runningValues, propertyName);
				context.TransferLookups(m_lookups, propertyName);
				context.FillInTokenIndex(this);
			}
			m_referencedFieldProperties = null;
			if (m_nullLevelInExpr && context.InRecursiveHierarchyRows && context.InRecursiveHierarchyColumns)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsLevelCallRecursiveHierarchyBothDimensions, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			if (m_rdlFunctionInfo != null)
			{
				m_rdlFunctionInfo.Initialize(propertyName, context, initializeDataOnError);
			}
		}

		internal bool InitializeAxisExpression(string propertyName, InitializationContext context)
		{
			bool hasError = context.ErrorContext.HasError;
			context.ErrorContext.HasError = false;
			Initialize(propertyName, context, initializeDataOnError: false);
			bool hasError2 = context.ErrorContext.HasError;
			context.ErrorContext.HasError = hasError;
			return !hasError2;
		}

		internal void AggregateInitialize(string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, InitializationContext context)
		{
			SpecialFunctionArgInitialize(dataSetName, objectType, objectName, propertyName, context, isLookup: false);
		}

		private void SpecialFunctionArgInitialize(string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, InitializationContext context, bool isLookup)
		{
			context.EnforceRdlSandboxContentRestrictions(this, objectType, objectName, propertyName);
			context.AggregateCheckVariableReferences(m_referencedVariables, objectType, objectName, propertyName);
			context.AggregateCheckFieldReferences(m_referencedFields, dataSetName, objectType, objectName, propertyName);
			context.AggregateCheckReportItemReferences(m_referencedReportItems, objectType, objectName, propertyName);
			context.AggregateCheckDataSetReference(m_referencedDataSets, objectType, objectName, propertyName);
			context.AggregateCheckDataSourceReference(m_referencedDataSources, objectType, objectName, propertyName);
			context.FillInFieldIndex(this, dataSetName);
			if (!isLookup)
			{
				context.ExprHostBuilder.AggregateParamExprAdd(this);
			}
			if (m_inPrevious || isLookup)
			{
				context.TransferAggregates(m_aggregates, propertyName);
				context.TransferRunningValues(m_runningValues, propertyName);
			}
			else
			{
				context.TransferNestedAggregates(m_aggregates, propertyName);
			}
			if (!isLookup)
			{
				context.TransferLookups(m_lookups, objectType, objectName, propertyName, dataSetName);
			}
			if (m_rdlFunctionInfo != null)
			{
				m_rdlFunctionInfo.Initialize(propertyName, context, initializeDataOnError: false);
			}
		}

		internal void LookupInitialize(string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, InitializationContext context)
		{
			SpecialFunctionArgInitialize(dataSetName, objectType, objectName, propertyName, context, isLookup: true);
		}

		internal bool HasRecursiveAggregates()
		{
			if (m_aggregates != null)
			{
				for (int i = 0; i < m_aggregates.Count; i++)
				{
					if (m_aggregates[i].Recursive)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void GroupExpressionInitialize(InitializationContext context)
		{
			context.CheckVariableReferences(m_referencedVariables, "Group");
			context.CheckFieldReferences(m_referencedFields, "Group");
			context.CheckReportItemReferences(m_referencedReportItems, "Group");
			context.CheckReportParameterReferences(m_referencedParameters, "Group");
			context.CheckDataSetReference(m_referencedDataSets, "Group");
			context.CheckDataSourceReference(m_referencedDataSources, "Group");
			context.FillInFieldIndex(this);
			context.TransferGroupExpressionRowNumbers(m_runningValues);
			context.TransferLookups(m_lookups, "Group");
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, Token.Enum));
			list.Add(new MemberInfo(MemberName.StringValue, Token.String));
			list.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.OriginalValue, Token.String));
			list.Add(new MemberInfo(MemberName.InPrevious, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DateTimeValue, Token.DateTime));
			list.Add(new MemberInfo(MemberName.FloatValue, Token.Double));
			list.Add(new MemberInfo(MemberName.ConstantType, Token.Enum));
			list.Add(new MemberInfo(MemberName.DateTimeOffsetValue, Token.Object));
			list.Add(new MemberInfo(MemberName.RdlFunctionInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RdlFunctionInfo));
			list.Add(new MemberInfo(MemberName.ScopedFieldInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopedFieldInfo, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.WriteEnum((int)m_type);
					break;
				case MemberName.ConstantType:
					writer.WriteEnum((int)m_constantType);
					break;
				case MemberName.StringValue:
					writer.Write(m_stringValue);
					break;
				case MemberName.BoolValue:
					writer.Write(m_boolValue);
					break;
				case MemberName.IntValue:
					writer.Write(m_intValue);
					break;
				case MemberName.FloatValue:
					writer.Write(m_floatValue);
					break;
				case MemberName.DateTimeValue:
					writer.Write(m_dateTimeValue);
					break;
				case MemberName.DateTimeOffsetValue:
					writer.Write(m_dateTimeOffsetValue.HasValue ? ((object)m_dateTimeOffsetValue) : null);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.OriginalValue:
					writer.Write(m_originalText);
					break;
				case MemberName.InPrevious:
					writer.Write(m_inPrevious);
					break;
				case MemberName.RdlFunctionInfo:
					writer.Write(m_rdlFunctionInfo);
					break;
				case MemberName.ScopedFieldInfo:
					writer.Write(m_scopedFieldInfo);
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
				case MemberName.Type:
					m_type = (Types)reader.ReadEnum();
					break;
				case MemberName.ConstantType:
					m_constantType = (DataType)reader.ReadEnum();
					break;
				case MemberName.StringValue:
					m_stringValue = reader.ReadString();
					break;
				case MemberName.BoolValue:
					m_boolValue = reader.ReadBoolean();
					break;
				case MemberName.IntValue:
					m_intValue = reader.ReadInt32();
					break;
				case MemberName.FloatValue:
					m_floatValue = reader.ReadDouble();
					break;
				case MemberName.DateTimeValue:
					m_dateTimeValue = reader.ReadDateTime();
					break;
				case MemberName.DateTimeOffsetValue:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						m_dateTimeOffsetValue = (DateTimeOffset)obj;
					}
					else
					{
						m_dateTimeOffsetValue = null;
					}
					break;
				}
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.OriginalValue:
					m_originalText = reader.ReadString();
					break;
				case MemberName.InPrevious:
					m_inPrevious = reader.ReadBoolean();
					break;
				case MemberName.RdlFunctionInfo:
					m_rdlFunctionInfo = reader.ReadRIFObject<RdlFunctionInfo>();
					break;
				case MemberName.ScopedFieldInfo:
					m_scopedFieldInfo = (ScopedFieldInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo;
		}

		internal void SetAsSimpleFieldReference(string fieldName)
		{
			AddReferencedField(fieldName);
			HasAnyFieldReferences = true;
			Type = Types.Field;
			StringValue = fieldName;
		}

		internal void SetAsScopedFieldReference(string scopeName, string fieldName)
		{
			AddReferencedScope(new ScopeReference(scopeName, fieldName));
			ScopedFieldInfo scopedFieldInfo = new ScopedFieldInfo();
			scopedFieldInfo.FieldName = fieldName;
			Type = Types.ScopedFieldReference;
			StringValue = scopeName;
			ScopedFieldInfo = scopedFieldInfo;
		}

		internal void SetAsLiteral(LiteralInfo literal)
		{
			Type = Types.Literal;
			LiteralInfo = literal;
		}

		internal void SetAsRdlFunction(RdlFunctionInfo function)
		{
			Type = Types.RdlFunction;
			RdlFunctionInfo = function;
		}

		internal void AddReferencedField(string fieldName)
		{
			if (m_referencedFields == null)
			{
				m_referencedFields = new List<string>();
			}
			m_referencedFields.Add(fieldName);
		}

		internal void AddReferencedReportItemInOriginalText(string reportItemName, int index)
		{
			if (m_referencedReportItemPositionsInOriginalText == null)
			{
				m_referencedReportItemPositionsInOriginalText = new List<int>();
			}
			m_referencedReportItemPositionsInOriginalText.Add(index);
		}

		internal void AddReferencedReportItemInTransformedExpression(string reportItemName, int index)
		{
			if (m_referencedReportItems == null)
			{
				m_referencedReportItems = new List<string>();
			}
			if (m_referencedReportItemPositionsInTransformedExpression == null)
			{
				m_referencedReportItemPositionsInTransformedExpression = new List<int>();
			}
			m_referencedReportItemPositionsInTransformedExpression.Add(index);
			m_referencedReportItems.Add(reportItemName);
		}

		internal void AddMeDotValueInOriginalText(int index)
		{
			if (m_meDotValuePositionsInOriginalText == null)
			{
				m_meDotValuePositionsInOriginalText = new List<int>();
			}
			m_meDotValuePositionsInOriginalText.Add(index);
		}

		internal void AddMeDotValueInTransformedExpression(int index)
		{
			if (m_meDotValuePositionsInTranformedExpr == null)
			{
				m_meDotValuePositionsInTranformedExpr = new List<int>();
			}
			m_meDotValuePositionsInTranformedExpr.Add(index);
		}

		internal void AddReferencedVariable(string variableName, int index)
		{
			if (m_referencedVariables == null)
			{
				m_referencedVariables = new List<string>();
			}
			if (m_referencedVariablePositions == null)
			{
				m_referencedVariablePositions = new List<int>();
			}
			m_referencedVariablePositions.Add(index);
			m_referencedVariables.Add(variableName);
		}

		internal void AddReferencedParameter(string parameterName)
		{
			if (m_referencedParameters == null)
			{
				m_referencedParameters = new List<string>();
			}
			m_referencedParameters.Add(parameterName);
		}

		internal void AddReferencedDataSet(string dataSetName)
		{
			if (m_referencedDataSets == null)
			{
				m_referencedDataSets = new List<string>();
			}
			m_referencedDataSets.Add(dataSetName);
		}

		internal void AddReferencedDataSource(string dataSourceName)
		{
			if (m_referencedDataSources == null)
			{
				m_referencedDataSources = new List<string>();
			}
			m_referencedDataSources.Add(dataSourceName);
		}

		internal void AddReferencedScope(ScopeReference scopeReference)
		{
			if (m_referencedScopes == null)
			{
				m_referencedScopes = new List<ScopeReference>();
			}
			m_referencedScopes.Add(scopeReference);
		}

		internal void AddAggregate(DataAggregateInfo aggregate)
		{
			if (m_aggregates == null)
			{
				m_aggregates = new List<DataAggregateInfo>();
			}
			m_aggregates.Add(aggregate);
		}

		internal void AddRunningValue(RunningValueInfo runningValue)
		{
			if (m_runningValues == null)
			{
				m_runningValues = new List<RunningValueInfo>();
			}
			m_runningValues.Add(runningValue);
		}

		internal void AddLookup(LookupInfo lookup)
		{
			if (m_lookups == null)
			{
				m_lookups = new List<LookupInfo>();
			}
			m_lookups.Add(lookup);
		}

		internal DataAggregateInfo GetSumAggregateWithoutScope()
		{
			if (Types.Aggregate == m_type && m_aggregates != null)
			{
				Global.Tracer.Assert(1 == m_aggregates.Count);
				DataAggregateInfo dataAggregateInfo = m_aggregates[0];
				if (DataAggregateInfo.AggregateTypes.Sum == dataAggregateInfo.AggregateType && !dataAggregateInfo.GetScope(out string _))
				{
					return dataAggregateInfo;
				}
			}
			return null;
		}

		internal void AddDynamicPropertyReference(string fieldName)
		{
			Global.Tracer.Assert(fieldName != null, "(null != fieldName)");
			if (m_referencedFieldProperties == null)
			{
				m_referencedFieldProperties = new Hashtable();
			}
			else if (m_referencedFieldProperties.ContainsKey(fieldName))
			{
				m_referencedFieldProperties.Remove(fieldName);
			}
			m_referencedFieldProperties.Add(fieldName, null);
		}

		internal void AddStaticPropertyReference(string fieldName, string propertyName)
		{
			Global.Tracer.Assert(fieldName != null && propertyName != null, "(null != fieldName && null != propertyName)");
			if (m_referencedFieldProperties == null)
			{
				m_referencedFieldProperties = new Hashtable();
			}
			if (m_referencedFieldProperties.ContainsKey(fieldName))
			{
				Hashtable hashtable = m_referencedFieldProperties[fieldName] as Hashtable;
				if (hashtable != null)
				{
					hashtable[propertyName] = null;
				}
			}
			else
			{
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add(propertyName, null);
				m_referencedFieldProperties.Add(fieldName, hashtable2);
			}
		}

		internal void AddTransformedExpressionAggregateInfo(int position, string aggregateID, bool isRunningValue)
		{
			int num = 0;
			TransformedExprSpecialFunctionInfo.SpecialFunctionType funcType;
			if (isRunningValue)
			{
				num = m_runningValues.Count - 1;
				funcType = TransformedExprSpecialFunctionInfo.SpecialFunctionType.RunningValue;
			}
			else
			{
				num = m_aggregates.Count - 1;
				funcType = TransformedExprSpecialFunctionInfo.SpecialFunctionType.Aggregate;
			}
			AddTransformedSpecialFunctionInfo(position, aggregateID, funcType, num);
		}

		private void AddTransformedSpecialFunctionInfo(int position, string specialFunctionID, TransformedExprSpecialFunctionInfo.SpecialFunctionType funcType, int index)
		{
			if (m_transformedExprAggregateInfos == null)
			{
				m_transformedExprAggregateInfos = new List<TransformedExprSpecialFunctionInfo>();
			}
			m_transformedExprAggregateInfos.Add(new TransformedExprSpecialFunctionInfo(position, specialFunctionID, funcType, index));
		}

		internal void AddTransformedExpressionLookupInfo(int position, string lookupID)
		{
			AddTransformedSpecialFunctionInfo(position, lookupID, TransformedExprSpecialFunctionInfo.SpecialFunctionType.Lookup, m_lookups.Count - 1);
		}

		private int UpdateReferencedItemsCollection(ExpressionInfo meDotValueExpression, int referencedIndex, int meDotValuePositionInOriginalText, int meDotValuePositionInTransformedExpression, List<int> positionsInTransformedExpression, List<int> positionsInOriginalText, List<string> referencedValues, List<int> positionsInMeDotValueTransformedExpression, List<int> positionsInMeDotValueOriginalText, List<string> referencedMeDotValueValues)
		{
			int num = 8;
			for (int i = referencedIndex; i < positionsInTransformedExpression.Count; i++)
			{
				if (positionsInOriginalText != null && meDotValuePositionInOriginalText < positionsInOriginalText[i])
				{
					positionsInOriginalText[i] += meDotValueExpression.OriginalText.Length - num;
				}
				if (meDotValuePositionInTransformedExpression < positionsInTransformedExpression[i])
				{
					positionsInTransformedExpression[i] += meDotValueExpression.TransformedExpression.Length - num;
				}
				else
				{
					referencedIndex++;
				}
			}
			if (referencedMeDotValueValues != null)
			{
				for (int j = 0; j < referencedMeDotValueValues.Count; j++)
				{
					if (positionsInMeDotValueOriginalText != null)
					{
						int num2 = positionsInMeDotValueOriginalText[j];
						positionsInOriginalText.Insert(referencedIndex, meDotValuePositionInOriginalText + num2);
					}
					string item = referencedMeDotValueValues[j];
					int num3 = positionsInMeDotValueTransformedExpression[j];
					positionsInTransformedExpression.Insert(referencedIndex, meDotValuePositionInTransformedExpression + num3);
					referencedValues.Insert(referencedIndex, item);
					referencedIndex++;
				}
			}
			return referencedIndex;
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ExpressionInfo expressionInfo = (ExpressionInfo)MemberwiseClone();
			if (RdlFunctionInfo != null)
			{
				expressionInfo.m_rdlFunctionInfo = (RdlFunctionInfo)m_rdlFunctionInfo.PublishClone(context);
				Global.Tracer.Assert(m_aggregates == null, "this.m_aggregates == null");
				Global.Tracer.Assert(m_runningValues == null, "this.m_aggregates == null");
				Global.Tracer.Assert(m_lookups == null, "this.m_aggregates == null");
				Global.Tracer.Assert(m_referencedReportItems == null, "this.m_aggregates == null");
				Global.Tracer.Assert(m_referencedReportItemPositionsInOriginalText == null, "this.m_aggregates == null");
				Global.Tracer.Assert(m_meDotValuePositionsInOriginalText == null, "this.m_aggregates == null");
				Global.Tracer.Assert(m_transformedExpression == null, "this.m_aggregates == null");
				return expressionInfo;
			}
			if (m_aggregates != null)
			{
				expressionInfo.m_aggregates = new List<DataAggregateInfo>(m_aggregates.Count);
				foreach (DataAggregateInfo aggregate in m_aggregates)
				{
					expressionInfo.m_aggregates.Add((DataAggregateInfo)aggregate.PublishClone(context));
				}
			}
			if (m_runningValues != null)
			{
				expressionInfo.m_runningValues = new List<RunningValueInfo>(m_runningValues.Count);
				foreach (RunningValueInfo runningValue in m_runningValues)
				{
					expressionInfo.m_runningValues.Add((RunningValueInfo)runningValue.PublishClone(context));
				}
			}
			if (m_lookups != null)
			{
				expressionInfo.m_lookups = new List<LookupInfo>(m_lookups.Count);
				foreach (LookupInfo lookup in m_lookups)
				{
					expressionInfo.m_lookups.Add((LookupInfo)lookup.PublishClone(context));
				}
			}
			if (m_referencedReportItems != null)
			{
				expressionInfo.m_referencedReportItems = new List<string>(m_referencedReportItems.Count);
				foreach (string referencedReportItem in m_referencedReportItems)
				{
					expressionInfo.m_referencedReportItems.Add((string)referencedReportItem.Clone());
				}
				context.AddExpressionThatReferencesReportItems(expressionInfo);
			}
			if (m_referencedReportItemPositionsInOriginalText != null)
			{
				expressionInfo.m_referencedReportItemPositionsInOriginalText = new List<int>(m_referencedReportItemPositionsInOriginalText.Count);
				foreach (int item in m_referencedReportItemPositionsInOriginalText)
				{
					expressionInfo.m_referencedReportItemPositionsInOriginalText.Add(item);
				}
			}
			if (m_meDotValuePositionsInOriginalText != null)
			{
				expressionInfo.m_meDotValuePositionsInOriginalText = new List<int>(m_meDotValuePositionsInOriginalText.Count);
				foreach (int item2 in m_meDotValuePositionsInOriginalText)
				{
					expressionInfo.m_meDotValuePositionsInOriginalText.Add(item2);
				}
			}
			if (m_transformedExpression != null)
			{
				StringBuilder stringBuilder = new StringBuilder(m_transformedExpression);
				StringBuilder stringBuilder2 = null;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				bool flag = false;
				bool flag2 = false;
				if (m_transformedExprAggregateInfos != null)
				{
					expressionInfo.m_transformedExprAggregateInfos = new List<TransformedExprSpecialFunctionInfo>(m_transformedExprAggregateInfos.Count);
					foreach (TransformedExprSpecialFunctionInfo transformedExprAggregateInfo in m_transformedExprAggregateInfos)
					{
						expressionInfo.m_transformedExprAggregateInfos.Add((TransformedExprSpecialFunctionInfo)transformedExprAggregateInfo.PublishClone(context));
					}
					flag = true;
					num3 += m_transformedExprAggregateInfos.Count;
				}
				if (m_referencedVariablePositions != null)
				{
					expressionInfo.m_referencedVariablePositions = new List<int>(m_referencedVariablePositions.Count);
					foreach (int referencedVariablePosition in m_referencedVariablePositions)
					{
						expressionInfo.m_referencedVariablePositions.Add(referencedVariablePosition);
					}
					flag2 = true;
					num3 += m_referencedVariablePositions.Count;
				}
				if (m_meDotValuePositionsInTranformedExpr != null)
				{
					expressionInfo.m_meDotValuePositionsInTranformedExpr = new List<int>(m_meDotValuePositionsInTranformedExpr.Count);
					foreach (int item3 in m_meDotValuePositionsInTranformedExpr)
					{
						expressionInfo.m_meDotValuePositionsInTranformedExpr.Add(item3);
					}
				}
				if (m_referencedReportItemPositionsInTransformedExpression != null)
				{
					expressionInfo.m_referencedReportItemPositionsInTransformedExpression = new List<int>(m_referencedReportItemPositionsInTransformedExpression.Count);
					foreach (int item4 in m_referencedReportItemPositionsInTransformedExpression)
					{
						expressionInfo.m_referencedReportItemPositionsInTransformedExpression.Add(item4);
					}
				}
				int num4 = 11;
				int num5 = 8;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				int num15 = 0;
				if (flag || flag2)
				{
					stringBuilder2 = new StringBuilder();
					stringBuilder2.Append("=");
					for (int i = 0; i < num3; i++)
					{
						Global.Tracer.Assert(!(flag && flag2) || num14 >= m_transformedExprAggregateInfos.Count || num15 >= m_referencedVariablePositions.Count || m_transformedExprAggregateInfos[num14].Position != m_referencedVariablePositions[num15]);
						int num16 = int.MaxValue;
						int num17 = int.MaxValue;
						if (flag && num14 < m_transformedExprAggregateInfos.Count)
						{
							num16 = m_transformedExprAggregateInfos[num14].Position;
						}
						if (flag2 && num15 < m_referencedVariablePositions.Count)
						{
							num17 = m_referencedVariablePositions[num15];
						}
						if (num16 < num17)
						{
							num10 = m_transformedExprAggregateInfos[num14].Position;
							TransformedExprSpecialFunctionInfo transformedExprSpecialFunctionInfo = m_transformedExprAggregateInfos[num14];
							string functionID = transformedExprSpecialFunctionInfo.FunctionID;
							string text;
							int num18;
							if (transformedExprSpecialFunctionInfo.FunctionType == TransformedExprSpecialFunctionInfo.SpecialFunctionType.Lookup)
							{
								text = context.GetNewLookupID(functionID);
								num18 = num5;
							}
							else
							{
								text = context.GetNewAggregateID(functionID);
								num18 = num4;
							}
							TransformedExprSpecialFunctionInfo transformedExprSpecialFunctionInfo2 = expressionInfo.m_transformedExprAggregateInfos[num14];
							transformedExprSpecialFunctionInfo2.FunctionID = text;
							transformedExprSpecialFunctionInfo2.Position = num10 + num7;
							stringBuilder.Replace(functionID, text, num10 + num18 + num7, functionID.Length);
							stringBuilder2.Append(m_transformedExpression.Substring(num9, num10 - num9));
							num11 = stringBuilder2.Length;
							int indexIntoCollection = transformedExprSpecialFunctionInfo.IndexIntoCollection;
							string text2 = null;
							string text3 = null;
							switch (transformedExprSpecialFunctionInfo.FunctionType)
							{
							case TransformedExprSpecialFunctionInfo.SpecialFunctionType.Aggregate:
								text2 = m_aggregates[indexIntoCollection].GetAsString();
								text3 = expressionInfo.m_aggregates[indexIntoCollection].GetAsString();
								break;
							case TransformedExprSpecialFunctionInfo.SpecialFunctionType.RunningValue:
								text2 = m_runningValues[indexIntoCollection].GetAsString();
								text3 = expressionInfo.m_runningValues[indexIntoCollection].GetAsString();
								break;
							case TransformedExprSpecialFunctionInfo.SpecialFunctionType.Lookup:
								text2 = m_lookups[indexIntoCollection].GetAsString();
								text3 = expressionInfo.m_lookups[indexIntoCollection].GetAsString();
								break;
							default:
								Global.Tracer.Assert(false, "Unknown transformed item function type: {0}", transformedExprSpecialFunctionInfo.FunctionType);
								break;
							}
							stringBuilder2.Append(text3);
							num9 = num10 + num18 + functionID.Length;
							Global.Tracer.Assert(text.Length >= functionID.Length, "(newName.Length >= oldName.Length)");
							num6 = text.Length - functionID.Length;
							num8 = text3.Length - text2.Length;
							num7 += num6;
							num14++;
						}
						else if (num17 != int.MaxValue)
						{
							num10 = m_referencedVariablePositions[num15];
							string text4 = m_referencedVariables[num15];
							string newVariableName = context.GetNewVariableName(text4);
							expressionInfo.m_referencedVariablePositions[num15] = num10 + num7;
							stringBuilder.Replace(text4, newVariableName, num10 + num7, text4.Length);
							stringBuilder2.Append(m_transformedExpression.Substring(num9, num10 - num9));
							num11 = stringBuilder2.Length;
							stringBuilder2.Append(newVariableName);
							num9 = num10 + text4.Length;
							Global.Tracer.Assert(newVariableName.Length >= text4.Length, "(newName.Length >= oldName.Length)");
							num6 = newVariableName.Length - text4.Length;
							num8 = num6;
							num7 += num6;
							num15++;
						}
						if (num6 != 0)
						{
							if (m_meDotValuePositionsInTranformedExpr != null)
							{
								for (int j = num13; j < m_meDotValuePositionsInTranformedExpr.Count; j++)
								{
									if (num11 > m_meDotValuePositionsInTranformedExpr[j])
									{
										num13++;
										continue;
									}
									int num19 = m_meDotValuePositionsInTranformedExpr[j];
									expressionInfo.m_meDotValuePositionsInTranformedExpr[j] = num19 + num6;
								}
							}
							if (m_referencedReportItemPositionsInTransformedExpression != null)
							{
								for (int k = num2; k < m_referencedReportItemPositionsInTransformedExpression.Count; k++)
								{
									if (num10 > m_referencedReportItemPositionsInTransformedExpression[k])
									{
										num2++;
										continue;
									}
									int num20 = m_referencedReportItemPositionsInTransformedExpression[k];
									expressionInfo.m_referencedReportItemPositionsInTransformedExpression[k] = num20 + num6;
								}
							}
						}
						if (num8 == 0)
						{
							continue;
						}
						if (m_meDotValuePositionsInOriginalText != null)
						{
							for (int l = num12; l < m_meDotValuePositionsInOriginalText.Count; l++)
							{
								if (num11 > m_meDotValuePositionsInOriginalText[l])
								{
									num12++;
									continue;
								}
								int num21 = m_meDotValuePositionsInOriginalText[l];
								expressionInfo.m_meDotValuePositionsInOriginalText[l] = num21 + num8;
							}
						}
						if (m_referencedReportItemPositionsInOriginalText == null)
						{
							continue;
						}
						for (int m = num; m < m_referencedReportItemPositionsInOriginalText.Count; m++)
						{
							if (num11 > m_referencedReportItemPositionsInOriginalText[m])
							{
								num++;
								continue;
							}
							int num22 = m_referencedReportItemPositionsInOriginalText[m];
							expressionInfo.m_referencedReportItemPositionsInOriginalText[m] = num22 + num8;
						}
					}
					stringBuilder2.Append(m_transformedExpression.Substring(num9));
					Global.Tracer.Assert(num14 + num15 == num3, "((currentAggIDIndex + currentVariableIndex) == potentialChangeCount)");
				}
				else
				{
					stringBuilder2 = new StringBuilder(m_originalText);
				}
				expressionInfo.m_originalText = stringBuilder2.ToString();
				expressionInfo.m_transformedExpression = stringBuilder.ToString();
			}
			else if (expressionInfo.m_aggregates != null && expressionInfo.m_aggregates.Count > 0)
			{
				expressionInfo.m_stringValue = expressionInfo.m_aggregates[0].Name;
				expressionInfo.m_originalText = expressionInfo.m_aggregates[0].GetAsString();
			}
			else if (expressionInfo.m_runningValues != null && expressionInfo.m_runningValues.Count > 0)
			{
				expressionInfo.m_stringValue = expressionInfo.m_runningValues[0].Name;
				expressionInfo.m_originalText = expressionInfo.m_runningValues[0].GetAsString();
			}
			else if (expressionInfo.m_lookups != null && expressionInfo.m_lookups.Count > 0)
			{
				expressionInfo.m_stringValue = expressionInfo.m_lookups[0].Name;
				expressionInfo.m_originalText = expressionInfo.m_lookups[0].GetAsString();
			}
			return expressionInfo;
		}

		internal void UpdateReportItemReferences(AutomaticSubtotalContext context)
		{
			StringBuilder stringBuilder = new StringBuilder(m_transformedExpression);
			StringBuilder stringBuilder2 = new StringBuilder(m_originalText);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (int i = 0; i < m_referencedReportItemPositionsInTransformedExpression.Count; i++)
			{
				string text = m_referencedReportItems[i];
				string newReportItemName = context.GetNewReportItemName(text);
				num2 = 0;
				if (!(newReportItemName != text))
				{
					continue;
				}
				m_referencedReportItems[i] = newReportItemName;
				if (m_transformedExpression != null)
				{
					num4 = m_referencedReportItemPositionsInTransformedExpression[i];
					stringBuilder.Replace(text, newReportItemName, num4 + num, text.Length);
					m_referencedReportItemPositionsInTransformedExpression[i] = num4 + num;
				}
				num3 = m_referencedReportItemPositionsInOriginalText[i];
				stringBuilder2.Replace(text, newReportItemName, num3 + num, text.Length);
				m_referencedReportItemPositionsInOriginalText[i] = num3 + num;
				num2 = newReportItemName.Length - text.Length;
				num += num2;
				if (num2 == 0)
				{
					continue;
				}
				if (m_transformedExpression != null && m_transformedExprAggregateInfos != null)
				{
					for (int j = num5; j < m_transformedExprAggregateInfos.Count; j++)
					{
						if (num4 > m_transformedExprAggregateInfos[j].Position)
						{
							num5++;
							continue;
						}
						int position = m_transformedExprAggregateInfos[j].Position;
						m_transformedExprAggregateInfos[j].Position = position + num2;
					}
				}
				if (m_referencedVariablePositions != null)
				{
					for (int k = num6; k < m_referencedVariablePositions.Count; k++)
					{
						if (num4 > m_referencedVariablePositions[k])
						{
							num6++;
							continue;
						}
						int num9 = m_referencedVariablePositions[k];
						m_referencedVariablePositions[k] = num9 + num2;
					}
				}
				if (m_meDotValuePositionsInOriginalText == null)
				{
					continue;
				}
				for (int l = num8; l < m_meDotValuePositionsInOriginalText.Count; l++)
				{
					if (num3 > m_meDotValuePositionsInOriginalText[l])
					{
						num8++;
						continue;
					}
					int num10 = m_meDotValuePositionsInOriginalText[l];
					m_meDotValuePositionsInOriginalText[l] = num10 + num2;
				}
				for (int m = num7; m < m_meDotValuePositionsInTranformedExpr.Count; m++)
				{
					if (num4 > m_meDotValuePositionsInTranformedExpr[m])
					{
						num7++;
						continue;
					}
					int num11 = m_meDotValuePositionsInTranformedExpr[m];
					m_meDotValuePositionsInTranformedExpr[m] = num11 + num2;
				}
			}
			m_transformedExpression = stringBuilder.ToString();
			m_originalText = stringBuilder2.ToString();
		}

		public void SetID(int id)
		{
			m_id = id;
		}
	}
}
