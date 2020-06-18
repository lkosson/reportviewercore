using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;
using System.Text;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ExpressionInfo
	{
		internal enum Types
		{
			Expression,
			Field,
			Aggregate,
			Constant,
			Token
		}

		private Types m_type;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

		private int m_exprHostID = -1;

		private string m_originalText;

		[NonSerialized]
		private string m_transformedExpression;

		[NonSerialized]
		private IntList m_transformedExpressionAggregatePositions;

		[NonSerialized]
		private StringList m_transformedExpressionAggregateIDs;

		[NonSerialized]
		private StringList m_referencedFields;

		[NonSerialized]
		private StringList m_referencedReportItems;

		[NonSerialized]
		private StringList m_referencedParameters;

		[NonSerialized]
		private StringList m_referencedDataSets;

		[NonSerialized]
		private StringList m_referencedDataSources;

		[NonSerialized]
		private DataAggregateInfoList m_aggregates;

		[NonSerialized]
		private RunningValueInfoList m_runningValues;

		[NonSerialized]
		private int m_compileTimeID = -1;

		[NonSerialized]
		private Hashtable m_referencedFieldProperties;

		[NonSerialized]
		private bool m_dynamicFieldReferences;

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

		internal bool IsExpression => m_type != Types.Constant;

		internal string Value
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

		internal IntList TransformedExpressionAggregatePositions
		{
			get
			{
				return m_transformedExpressionAggregatePositions;
			}
			set
			{
				m_transformedExpressionAggregatePositions = value;
			}
		}

		internal StringList TransformedExpressionAggregateIDs
		{
			get
			{
				return m_transformedExpressionAggregateIDs;
			}
			set
			{
				m_transformedExpressionAggregateIDs = value;
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

		internal DataAggregateInfoList Aggregates => m_aggregates;

		internal RunningValueInfoList RunningValues => m_runningValues;

		internal Hashtable ReferencedFieldProperties => m_referencedFieldProperties;

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

		internal ExpressionInfo()
		{
		}

		internal ExpressionInfo(Types type)
		{
			m_type = Types.Expression;
		}

		internal void Initialize(string propertyName, InitializationContext context)
		{
			context.CheckFieldReferences(m_referencedFields, propertyName);
			context.CheckReportItemReferences(m_referencedReportItems, propertyName);
			context.CheckReportParameterReferences(m_referencedParameters, propertyName);
			context.CheckDataSetReference(m_referencedDataSets, propertyName);
			context.CheckDataSourceReference(m_referencedDataSources, propertyName);
			if ((LocationFlags.InMatrixCellTopLevelItem & context.Location) != 0 && m_referencedFields != null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsNonAggregateInMatrixCell, Severity.Warning, context.ObjectType, context.ObjectName, propertyName);
			}
			context.FillInFieldIndex(this);
			context.TransferAggregates(m_aggregates, propertyName);
			context.TransferRunningValues(m_runningValues, propertyName);
			context.MergeFieldPropertiesIntoDataset(this);
			context.FillInTokenIndex(this);
			m_referencedFieldProperties = null;
		}

		internal void AggregateInitialize(string dataSetName, ObjectType objectType, string objectName, string propertyName, InitializationContext context)
		{
			context.AggregateCheckFieldReferences(m_referencedFields, dataSetName, objectType, objectName, propertyName);
			context.AggregateCheckReportItemReferences(m_referencedReportItems, objectType, objectName, propertyName);
			context.AggregateCheckDataSetReference(m_referencedDataSets, objectType, objectName, propertyName);
			context.AggregateCheckDataSourceReference(m_referencedDataSources, objectType, objectName, propertyName);
			context.MergeFieldPropertiesIntoDataset(this);
			context.FillInFieldIndex(this, dataSetName);
			context.ExprHostBuilder.AggregateParamExprAdd(this);
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
			context.CheckFieldReferences(m_referencedFields, "Group");
			context.CheckReportItemReferences(m_referencedReportItems, "Group");
			context.CheckReportParameterReferences(m_referencedParameters, "Group");
			context.CheckDataSetReference(m_referencedDataSets, "Group");
			context.CheckDataSourceReference(m_referencedDataSources, "Group");
			context.MergeFieldPropertiesIntoDataset(this);
			context.FillInFieldIndex(this);
			context.TransferGroupExpressionRowNumbers(m_runningValues);
		}

		internal ExpressionInfo DeepClone(InitializationContext context)
		{
			Global.Tracer.Assert(-1 == m_exprHostID);
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.m_type = m_type;
			expressionInfo.m_compileTimeID = m_compileTimeID;
			expressionInfo.m_stringValue = m_stringValue;
			expressionInfo.m_boolValue = m_boolValue;
			expressionInfo.m_intValue = m_intValue;
			expressionInfo.m_originalText = m_originalText;
			expressionInfo.m_referencedFields = m_referencedFields;
			expressionInfo.m_referencedParameters = m_referencedParameters;
			Global.Tracer.Assert(m_referencedReportItems == null);
			if (m_aggregates != null)
			{
				int count = m_aggregates.Count;
				expressionInfo.m_aggregates = new DataAggregateInfoList(count);
				for (int i = 0; i < count; i++)
				{
					expressionInfo.m_aggregates.Add(m_aggregates[i].DeepClone(context));
				}
			}
			if (m_runningValues != null)
			{
				int count2 = m_runningValues.Count;
				expressionInfo.m_runningValues = new RunningValueInfoList(count2);
				for (int j = 0; j < count2; j++)
				{
					expressionInfo.m_runningValues.Add(m_runningValues[j].DeepClone(context));
				}
			}
			if (m_transformedExpression != null)
			{
				StringBuilder stringBuilder = new StringBuilder(m_transformedExpression);
				if (context.AggregateRewriteMap != null && m_transformedExpressionAggregateIDs != null)
				{
					Global.Tracer.Assert(m_transformedExpressionAggregatePositions != null && m_transformedExpressionAggregateIDs.Count == m_transformedExpressionAggregatePositions.Count && m_transformedExpression != null);
					int num = 11;
					for (int k = 0; k < m_transformedExpressionAggregateIDs.Count; k++)
					{
						string text = m_transformedExpressionAggregateIDs[k];
						string text2 = context.AggregateRewriteMap[text] as string;
						int num2 = m_transformedExpressionAggregatePositions[k];
						if (text2 != null)
						{
							Global.Tracer.Assert(text != null && text2 != null && text2.Length >= text.Length);
							Global.Tracer.Assert(m_transformedExpression.Length > num2 + num);
							stringBuilder.Replace(text, text2, num2 + num, text.Length);
							num += text2.Length - text.Length;
						}
					}
				}
				expressionInfo.m_transformedExpression = stringBuilder.ToString();
			}
			return expressionInfo;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.StringValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.OriginalValue, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void AddReferencedField(string fieldName)
		{
			if (m_referencedFields == null)
			{
				m_referencedFields = new StringList();
			}
			m_referencedFields.Add(fieldName);
		}

		internal void AddReferencedReportItem(string reportItemName)
		{
			if (m_referencedReportItems == null)
			{
				m_referencedReportItems = new StringList();
			}
			m_referencedReportItems.Add(reportItemName);
		}

		internal void AddReferencedParameter(string parameterName)
		{
			if (m_referencedParameters == null)
			{
				m_referencedParameters = new StringList();
			}
			m_referencedParameters.Add(parameterName);
		}

		internal void AddReferencedDataSet(string dataSetName)
		{
			if (m_referencedDataSets == null)
			{
				m_referencedDataSets = new StringList();
			}
			m_referencedDataSets.Add(dataSetName);
		}

		internal void AddReferencedDataSource(string dataSourceName)
		{
			if (m_referencedDataSources == null)
			{
				m_referencedDataSources = new StringList();
			}
			m_referencedDataSources.Add(dataSourceName);
		}

		internal void AddAggregate(DataAggregateInfo aggregate)
		{
			if (m_aggregates == null)
			{
				m_aggregates = new DataAggregateInfoList();
			}
			m_aggregates.Add(aggregate);
		}

		internal void AddRunningValue(RunningValueInfo runningValue)
		{
			if (m_runningValues == null)
			{
				m_runningValues = new RunningValueInfoList();
			}
			m_runningValues.Add(runningValue);
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
			Global.Tracer.Assert(fieldName != null);
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
			Global.Tracer.Assert(fieldName != null && propertyName != null);
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
	}
}
