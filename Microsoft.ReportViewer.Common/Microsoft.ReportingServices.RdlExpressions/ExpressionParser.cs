using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Xml;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal abstract class ExpressionParser
	{
		internal enum ExpressionType
		{
			General,
			ReportParameter,
			ReportLanguage,
			QueryParameter,
			GroupExpression,
			SortExpression,
			DataRegionSortExpression,
			DataSetFilters,
			DataRegionFilters,
			GroupingFilters,
			FieldValue,
			VariableValue,
			SubReportParameter,
			GroupVariableValue,
			UserSortExpression,
			JoinExpression,
			ScopeValue,
			Calculation
		}

		internal enum RecursiveFlags
		{
			Simple,
			Recursive
		}

		internal enum EvaluationMode
		{
			Auto,
			Constant
		}

		internal struct ExpressionContext
		{
			private ExpressionType m_expressionType;

			private DataType m_constantType;

			private Microsoft.ReportingServices.ReportPublishing.LocationFlags m_location;

			private ObjectType m_objectType;

			private string m_objectName;

			private string m_propertyName;

			private string m_dataSetName;

			private bool m_inPrevious;

			private bool m_inLookup;

			private int m_maxExpressionLength;

			private Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo m_outerAggregate;

			private readonly PublishingContextBase m_publishingContext;

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

			internal bool InLookup
			{
				get
				{
					return m_inLookup;
				}
				set
				{
					m_inLookup = value;
				}
			}

			internal ExpressionType ExpressionType => m_expressionType;

			internal DataType ConstantType => m_constantType;

			internal Microsoft.ReportingServices.ReportPublishing.LocationFlags Location => m_location;

			internal ObjectType ObjectType => m_objectType;

			internal string ObjectName => m_objectName;

			internal string PropertyName => m_propertyName;

			internal string DataSetName => m_dataSetName;

			internal int MaxExpressionLength => m_maxExpressionLength;

			internal Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo OuterAggregate
			{
				get
				{
					return m_outerAggregate;
				}
				set
				{
					m_outerAggregate = value;
				}
			}

			internal bool InAggregate => m_outerAggregate != null;

			internal PublishingVersioning PublishingVersioning => m_publishingContext.PublishingVersioning;

			internal ExpressionContext(ExpressionType expressionType, DataType constantType, Microsoft.ReportingServices.ReportPublishing.LocationFlags location, ObjectType objectType, string objectName, string propertyName, string dataSetName, int maxExpressionLength, PublishingContextBase publishingContext)
			{
				m_expressionType = expressionType;
				m_constantType = constantType;
				m_location = location;
				m_objectType = objectType;
				m_objectName = objectName;
				m_propertyName = propertyName;
				m_dataSetName = dataSetName;
				m_inPrevious = false;
				m_inLookup = false;
				m_maxExpressionLength = maxExpressionLength;
				m_outerAggregate = null;
				m_publishingContext = publishingContext;
			}
		}

		[Flags]
		protected enum GrammarFlags
		{
			DenyAggregates = 0x1,
			DenyRunningValue = 0x2,
			DenyRowNumber = 0x4,
			DenyFields = 0x8,
			DenyReportItems = 0x10,
			DenyOverallPageGlobals = 0x20,
			DenyPostSortAggregate = 0x40,
			DenyPrevious = 0x80,
			DenyDataSets = 0x100,
			DenyDataSources = 0x200,
			DenyVariables = 0x400,
			DenyMeDotValue = 0x800,
			DenyLookups = 0x1000,
			DenyAggregatesOfAggregates = 0x2000,
			DenyPageGlobals = 0x4000,
			DenyRenderFormatAll = 0x8000,
			DenyScopes = 0x10000
		}

		[Flags]
		protected enum Restrictions
		{
			None = 0x0,
			InPageSection = 0x10086,
			InBody = 0x4020,
			AggregateParameterInPageSection = 0x104C7,
			AggregateParameterInBody = 0x104D6,
			PreviousAggregateParameterInBody = 0x10494,
			ReportParameter = 0x1979F,
			ReportLanguage = 0x1979F,
			QueryParameter = 0x1979F,
			GroupExpression = 0x18493,
			SortExpression = 0x180D6,
			DataRowSortExpression = 0x184D7,
			DataSetFilters = 0x19497,
			DataRegionFilters = 0x18497,
			GroupingFilters = 0x1A0D6,
			FieldValue = 0x1D4B7,
			VariableValue = 0x1C0F6,
			GroupVariableValue = 0x1E0D6,
			LookupSourceExpression = 0x9400,
			SubReportParameter = 0x18000,
			JoinExpression = 0x18493,
			ScopeValue = 0x180D6
		}

		protected ErrorContext m_errorContext;

		private bool m_valueReferenced;

		private bool m_valueReferencedGlobal;

		internal const int UnrestrictedMaxExprLength = -1;

		internal abstract bool BodyRefersToReportItems
		{
			get;
		}

		internal abstract bool PageSectionRefersToReportItems
		{
			get;
		}

		internal abstract bool PageSectionRefersToOverallTotalPages
		{
			get;
		}

		internal abstract bool PageSectionRefersToTotalPages
		{
			get;
		}

		internal abstract int NumberOfAggregates
		{
			get;
		}

		internal abstract int LastID
		{
			get;
		}

		internal abstract int LastLookupID
		{
			get;
		}

		internal abstract bool PreviousAggregateUsed
		{
			get;
		}

		internal abstract bool AggregateOfAggregatesUsed
		{
			get;
		}

		internal abstract bool AggregateOfAggregatesUsedInUserSort
		{
			get;
		}

		internal bool ValueReferenced => m_valueReferenced;

		internal bool ValueReferencedGlobal => m_valueReferencedGlobal;

		internal ExpressionParser(ErrorContext errorContext)
		{
			m_errorContext = errorContext;
		}

		internal abstract CodeDomProvider GetCodeCompiler();

		internal abstract string GetCompilerArguments();

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionContext context, EvaluationMode evaluationMode);

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionContext context, EvaluationMode evaluationMode, out bool userCollectionReferenced);

		internal abstract void ConvertField2ComplexExpr(ref Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression);

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo CreateScopedFirstAggregate(string fieldName, string dataSetName);

		internal void ResetValueReferencedFlag()
		{
			m_valueReferenced = false;
		}

		internal abstract void ResetPageSectionRefersFlags();

		internal static void ParseRDLConstant(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, DataType constantType, ErrorContext errorContext, ObjectType objectType, string objectName, string propertyName)
		{
			expressionInfo.Type = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant;
			expressionInfo.ConstantType = constantType;
			switch (constantType)
			{
			case DataType.String:
				expressionInfo.StringValue = expression;
				break;
			case DataType.Boolean:
			{
				bool boolValue;
				try
				{
					if (string.Compare(expression, "0", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(expression, "1", StringComparison.OrdinalIgnoreCase) == 0)
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Warning, objectType, objectName, propertyName, expression);
					}
					boolValue = XmlConvert.ToBoolean(expression);
				}
				catch (Exception e3)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e3))
					{
						throw;
					}
					boolValue = false;
					errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, objectType, objectName, propertyName, expression);
				}
				expressionInfo.BoolValue = boolValue;
				break;
			}
			case DataType.Integer:
			{
				int intValue;
				try
				{
					intValue = XmlConvert.ToInt32(expression);
				}
				catch (Exception e2)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e2))
					{
						throw;
					}
					intValue = 0;
					errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, objectType, objectName, propertyName, expression.MarkAsPrivate());
				}
				expressionInfo.IntValue = intValue;
				break;
			}
			case DataType.Float:
			{
				double floatValue;
				try
				{
					floatValue = XmlConvert.ToDouble(expression);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					floatValue = 0.0;
					errorContext.Register(ProcessingErrorCode.rsInvalidFloatConstant, Severity.Error, objectType, objectName, propertyName, expression);
				}
				expressionInfo.FloatValue = floatValue;
				break;
			}
			case DataType.DateTime:
			{
				if (DateTimeUtil.TryParseDateTime(expression, CultureInfo.InvariantCulture, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset))
				{
					if (hasTimeOffset)
					{
						expressionInfo.SetDateTimeValue(dateTimeOffset);
					}
					else
					{
						expressionInfo.SetDateTimeValue(dateTimeOffset.DateTime);
					}
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidDateTimeConstant, Severity.Error, objectType, objectName, propertyName, expression);
				}
				break;
			}
			default:
				Global.Tracer.Assert(condition: false);
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		protected static Restrictions ExpressionType2Restrictions(ExpressionType expressionType)
		{
			switch (expressionType)
			{
			case ExpressionType.General:
				return Restrictions.None;
			case ExpressionType.ReportParameter:
				return Restrictions.ReportParameter;
			case ExpressionType.ReportLanguage:
				return Restrictions.ReportParameter;
			case ExpressionType.QueryParameter:
				return Restrictions.ReportParameter;
			case ExpressionType.GroupExpression:
				return Restrictions.GroupExpression;
			case ExpressionType.SortExpression:
			case ExpressionType.UserSortExpression:
				return Restrictions.SortExpression;
			case ExpressionType.DataRegionSortExpression:
				return Restrictions.DataRowSortExpression;
			case ExpressionType.DataSetFilters:
				return Restrictions.DataSetFilters;
			case ExpressionType.DataRegionFilters:
				return Restrictions.DataRegionFilters;
			case ExpressionType.GroupingFilters:
				return Restrictions.GroupingFilters;
			case ExpressionType.FieldValue:
				return Restrictions.FieldValue;
			case ExpressionType.VariableValue:
				return Restrictions.VariableValue;
			case ExpressionType.SubReportParameter:
				return Restrictions.SubReportParameter;
			case ExpressionType.GroupVariableValue:
				return Restrictions.GroupVariableValue;
			case ExpressionType.JoinExpression:
				return Restrictions.GroupExpression;
			case ExpressionType.ScopeValue:
				return Restrictions.SortExpression;
			case ExpressionType.Calculation:
				return Restrictions.None;
			default:
				Global.Tracer.Assert(condition: false);
				return Restrictions.None;
			}
		}

		protected void SetValueReferenced()
		{
			m_valueReferenced = true;
			m_valueReferencedGlobal = true;
		}
	}
}
