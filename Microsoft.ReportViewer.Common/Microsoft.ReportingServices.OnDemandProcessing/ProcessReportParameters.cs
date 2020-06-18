using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class ProcessReportParameters
	{
		private IInternalProcessingContext m_processingContext;

		private ReportParameterDataSetCache m_paramDataSetCache;

		private Dictionary<string, bool> m_dependenciesSubmitted;

		private int m_lastDynamicParam;

		private ParameterInfoCollection m_parameters;

		protected int m_maxStringResultLength = -1;

		protected const int UnrestrictedStringResultLength = -1;

		internal virtual bool IsReportParameterProcessing => true;

		internal IInternalProcessingContext ProcessingContext => m_processingContext;

		public ProcessReportParameters(IInternalProcessingContext aContext)
		{
			m_processingContext = aContext;
		}

		private void ProcessParameter(ParameterInfoCollection aParameters, int aParamIndex)
		{
			ParameterInfo parameterInfo = aParameters[aParamIndex];
			parameterInfo.MissingUpstreamDataSourcePrompt = false;
			IParameterDef parameterDef = null;
			bool flag = aParameters.UserProfileState != UserProfileState.None;
			if (m_processingContext.SnapshotProcessing && parameterInfo.UsedInQuery)
			{
				parameterInfo.State = ReportParameterState.HasValidValue;
				parameterInfo.StoreLabels();
				return;
			}
			if (parameterInfo.DynamicDefaultValue || parameterInfo.DynamicValidValues || parameterInfo.DynamicPrompt)
			{
				UpdateParametersContext(aParameters, m_lastDynamicParam, aParamIndex);
				m_lastDynamicParam = aParamIndex;
				parameterDef = GetParameterDef(aParamIndex);
				Global.Tracer.Assert(parameterDef != null, "null != paramDef, parameter {0}", parameterInfo.Name.MarkAsPrivate());
				Global.Tracer.Assert(parameterInfo.DataType == parameterDef.DataType, "paramInfo.DataType == paramDef.DataType, parameter {0}", parameterInfo.Name.MarkAsPrivate());
				AssertAreSameParameterByName(parameterInfo, parameterDef);
			}
			bool flag2 = m_dependenciesSubmitted.ContainsKey(parameterInfo.Name);
			if (parameterInfo.DynamicPrompt && (flag2 || !parameterInfo.IsUserSupplied || flag))
			{
				SetupExprHost(parameterDef);
				string text = EvaluatePromptExpr(parameterInfo, parameterDef);
				if (text == null || text.Equals(string.Empty))
				{
					text = parameterInfo.Name;
				}
				parameterInfo.Prompt = text;
			}
			switch (parameterInfo.CalculateDependencyStatus())
			{
			case ReportParameterDependencyState.HasOutstandingDependencies:
				parameterInfo.State = ReportParameterState.HasOutstandingDependencies;
				parameterInfo.Values = null;
				if (parameterInfo.DynamicDefaultValue)
				{
					parameterInfo.DefaultValues = null;
				}
				if (parameterInfo.DynamicValidValues)
				{
					parameterInfo.ValidValues = null;
				}
				return;
			case ReportParameterDependencyState.MissingUpstreamDataSourcePrompt:
				parameterInfo.MissingUpstreamDataSourcePrompt = true;
				parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
				return;
			default:
				Global.Tracer.Assert(condition: false, "Unexpected dependency state.");
				break;
			case ReportParameterDependencyState.AllDependenciesSpecified:
				break;
			}
			bool flag3 = parameterInfo.DynamicDefaultValue && (parameterInfo.Values == null || (parameterInfo.Values != null && !parameterInfo.IsUserSupplied)) && ((m_processingContext.SnapshotProcessing && parameterDef.HasDefaultValuesExpressions() && (flag || (parameterInfo.DependencyList != null && (parameterInfo.Values == null || (!parameterInfo.IsUserSupplied && flag2))))) || (!m_processingContext.SnapshotProcessing && (flag2 || parameterInfo.Values == null)));
			if (parameterInfo.DynamicValidValues && ((m_processingContext.SnapshotProcessing && parameterDef.HasValidValuesValueExpressions() && (parameterInfo.DependencyList != null || (flag && flag3))) || (!m_processingContext.SnapshotProcessing && ((parameterInfo.ValidValues != null && flag2) || parameterInfo.ValidValues == null))) && !ProcessValidValues(parameterInfo, parameterDef, flag3))
			{
				parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
				return;
			}
			if (!flag3 && parameterInfo.Values != null)
			{
				if (parameterInfo.ValueIsValid())
				{
					parameterInfo.State = ReportParameterState.HasValidValue;
					parameterInfo.StoreLabels();
				}
				else
				{
					parameterInfo.State = ReportParameterState.InvalidValueProvided;
					parameterInfo.Values = null;
					parameterInfo.EnsureLabelsAreGenerated();
				}
				return;
			}
			parameterInfo.Values = null;
			parameterInfo.State = ReportParameterState.MissingValidValue;
			if (flag3 && !ProcessDefaultValue(parameterInfo, parameterDef))
			{
				parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
				return;
			}
			if (parameterInfo.DefaultValues != null)
			{
				parameterInfo.Values = parameterInfo.DefaultValues;
				if (!parameterInfo.ValueIsValid())
				{
					parameterInfo.Values = null;
					parameterInfo.State = ReportParameterState.DefaultValueInvalid;
					parameterInfo.EnsureLabelsAreGenerated();
				}
				else
				{
					parameterInfo.State = ReportParameterState.HasValidValue;
					parameterInfo.StoreLabels();
				}
			}
			m_paramDataSetCache = null;
		}

		protected virtual void AssertAreSameParameterByName(ParameterInfo paramInfo, IParameterDef paramDef)
		{
			Global.Tracer.Assert(string.Compare(paramInfo.Name, paramDef.Name, StringComparison.OrdinalIgnoreCase) == 0, "paramInfo.Name == paramDef.Name, parameter {0}", paramInfo.Name.MarkAsPrivate());
		}

		public ProcessingMessageList Process(ParameterInfoCollection aParameters)
		{
			m_parameters = aParameters;
			try
			{
				if (m_parameters.IsAnyParameterDynamic)
				{
					InitParametersContext(m_parameters);
				}
				m_dependenciesSubmitted = BuildSubmittedDependencyList(m_parameters);
				for (int i = 0; i < aParameters.Count; i++)
				{
					ProcessParameter(m_parameters, i);
				}
				m_parameters.Validated = true;
				return m_processingContext.ErrorContext.Messages;
			}
			finally
			{
				Cleanup();
			}
		}

		internal static Dictionary<string, bool> BuildSubmittedDependencyList(ParameterInfoCollection parameters)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.DependencyList == null)
				{
					continue;
				}
				for (int j = 0; j < parameterInfo.DependencyList.Count; j++)
				{
					ParameterInfo parameterInfo2 = parameterInfo.DependencyList[j];
					if ((parameterInfo2.IsUserSupplied && parameterInfo2.ValuesChanged) || dictionary.ContainsKey(parameterInfo2.Name))
					{
						dictionary.Add(parameterInfo.Name, value: true);
						break;
					}
				}
			}
			return dictionary;
		}

		internal abstract IParameterDef GetParameterDef(int aParamIndex);

		internal abstract void InitParametersContext(ParameterInfoCollection parameters);

		internal abstract void Cleanup();

		internal abstract void AddToRuntime(ParameterInfo aParamInfo);

		internal abstract void SetupExprHost(IParameterDef aParamDef);

		internal abstract object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex);

		internal abstract object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex);

		internal abstract object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex);

		internal abstract bool NeedPrompt(IParameterDataSource paramDS);

		internal abstract void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName);

		internal abstract string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef);

		internal abstract ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues);

		protected abstract string ApplySandboxStringRestriction(string value, string paramName, string propertyName);

		internal bool ValidateValue(object newValue, IParameterDef paramDef, string parameterValueProperty)
		{
			if (paramDef.ValidateValueForNull(newValue, m_processingContext.ErrorContext, parameterValueProperty) && paramDef.ValidateValueForBlank(newValue, m_processingContext.ErrorContext, parameterValueProperty))
			{
				return true;
			}
			return false;
		}

		internal object ConvertValue(object o, IParameterDef paramDef, bool isDefaultValue)
		{
			if (o == null || DBNull.Value == o)
			{
				return null;
			}
			bool flag = false;
			object obj = null;
			try
			{
				DataType dataType = paramDef.DataType;
				if (dataType <= DataType.Integer)
				{
					switch (dataType)
					{
					case DataType.Object:
						obj = o;
						return obj;
					case DataType.Boolean:
						obj = (bool)o;
						return obj;
					case DataType.Integer:
						obj = Convert.ToInt32(o, Thread.CurrentThread.CurrentCulture);
						return obj;
					default:
						return obj;
					}
				}
				switch (dataType)
				{
				case DataType.DateTime:
					if (o is DateTimeOffset)
					{
						obj = (DateTimeOffset)o;
						return obj;
					}
					obj = (DateTime)o;
					return obj;
				case DataType.Float:
					obj = Convert.ToDouble(o, Thread.CurrentThread.CurrentCulture);
					return obj;
				case DataType.String:
					obj = Convert.ToString(o, Thread.CurrentThread.CurrentCulture);
					obj = ApplySandboxStringRestriction((string)obj, paramDef.Name, isDefaultValue ? "DefaultValue" : "ValidValue");
					return obj;
				default:
					return obj;
				}
			}
			catch (InvalidCastException)
			{
				flag = true;
				return obj;
			}
			catch (OverflowException)
			{
				flag = true;
				return obj;
			}
			catch (FormatException)
			{
				flag = true;
				return obj;
			}
			finally
			{
				if (flag)
				{
					string propertyName = (!isDefaultValue) ? "ValidValues" : "DefaultValue";
					m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, paramDef.ParameterObjectType, paramDef.Name, propertyName);
					throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
				}
			}
		}

		internal void UpdateParametersContext(ParameterInfoCollection parameters, int lastIndex, int currentIndex)
		{
			for (int i = lastIndex; i < currentIndex; i++)
			{
				ParameterInfo aParamInfo = parameters[i];
				AddToRuntime(aParamInfo);
			}
		}

		internal bool ProcessDefaultValue(ParameterInfo parameter, IParameterDef paramDef)
		{
			if (parameter == null || paramDef == null)
			{
				return true;
			}
			object obj = null;
			if (paramDef.HasDefaultValuesExpressions())
			{
				int num = paramDef.DefaultValuesExpressionCount;
				Global.Tracer.Assert(num != 0, "(0 != count)");
				if (!paramDef.MultiValue)
				{
					num = 1;
				}
				SetupExprHost(paramDef);
				ArrayList arrayList = new ArrayList(num);
				for (int i = 0; i < num; i++)
				{
					obj = EvaluateDefaultValueExpr(paramDef, i);
					if (obj is object[])
					{
						object[] array = obj as object[];
						foreach (object o in array)
						{
							object obj2 = ConvertValue(o, paramDef, isDefaultValue: true);
							if (ValidateValue(obj2, paramDef, "DefaultValue"))
							{
								arrayList.Add(obj2);
								continue;
							}
							return true;
						}
					}
					else
					{
						obj = ConvertValue(obj, paramDef, isDefaultValue: true);
						if (!ValidateValue(obj, paramDef, "DefaultValue"))
						{
							return true;
						}
						arrayList.Add(obj);
					}
				}
				Global.Tracer.Assert(arrayList != null, "(null != defaultValues)");
				if (paramDef.MultiValue)
				{
					parameter.DefaultValues = new object[arrayList.Count];
					arrayList.CopyTo(parameter.DefaultValues);
				}
				else if (arrayList.Count > 0)
				{
					parameter.DefaultValues = new object[1];
					parameter.DefaultValues[0] = arrayList[0];
				}
				else
				{
					parameter.DefaultValues = new object[0];
				}
			}
			else if (paramDef.HasDefaultValuesDataSource() && m_processingContext.EnableDataBackedParameters)
			{
				IParameterDataSource defaultDataSource = paramDef.DefaultDataSource;
				IParameterDataSource validValuesDataSource = paramDef.ValidValuesDataSource;
				List<object> list = null;
				if (m_paramDataSetCache != null && validValuesDataSource != null && defaultDataSource.DataSourceIndex == validValuesDataSource.DataSourceIndex && defaultDataSource.DataSetIndex == validValuesDataSource.DataSetIndex)
				{
					list = m_paramDataSetCache.DefaultValues;
				}
				else
				{
					if (NeedPrompt(defaultDataSource))
					{
						parameter.MissingUpstreamDataSourcePrompt = true;
						return false;
					}
					list = ProcessReportParameterDataSet(parameter, paramDef, defaultDataSource, aRetrieveValidValues: false, aRetrievalDefaultValues: true).DefaultValues;
					if (Global.Tracer.TraceVerbose && (list == null || list.Count == 0))
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Parameter '{0}' default value list does not contain any values.", parameter.Name.MarkAsPrivate());
					}
				}
				if (list != null)
				{
					int count = list.Count;
					parameter.DefaultValues = new object[count];
					for (int k = 0; k < count; k++)
					{
						obj = list[k];
						if (ValidateValue(obj, paramDef, "DefaultValue"))
						{
							parameter.DefaultValues[k] = obj;
							continue;
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Parameter '{0}' has a default value '{1}' which is not a valid value.", parameter.Name.MarkAsPrivate(), obj.ToString().MarkAsPrivate());
						}
						parameter.DefaultValues = null;
						return true;
					}
				}
			}
			return true;
		}

		internal bool ProcessValidValues(ParameterInfo parameter, IParameterDef paramDef, bool aEvaluateDefaultValues)
		{
			if (parameter == null || paramDef == null)
			{
				return true;
			}
			IParameterDataSource validValuesDataSource = paramDef.ValidValuesDataSource;
			if (paramDef.HasValidValuesDataSource())
			{
				if (m_processingContext.EnableDataBackedParameters)
				{
					if (NeedPrompt(validValuesDataSource))
					{
						parameter.MissingUpstreamDataSourcePrompt = true;
						return false;
					}
					IParameterDataSource defaultDataSource = paramDef.DefaultDataSource;
					bool aRetrievalDefaultValues = aEvaluateDefaultValues && defaultDataSource != null && defaultDataSource.DataSourceIndex == validValuesDataSource.DataSourceIndex && defaultDataSource.DataSetIndex == validValuesDataSource.DataSetIndex;
					m_paramDataSetCache = ProcessReportParameterDataSet(parameter, paramDef, validValuesDataSource, aRetrieveValidValues: true, aRetrievalDefaultValues);
					if (Global.Tracer.TraceVerbose && parameter.ValidValues != null && parameter.ValidValues.Count == 0)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Parameter '{0}' dynamic valid value list does not contain any values.", parameter.Name.MarkAsPrivate());
					}
				}
			}
			else if (paramDef.HasValidValuesValueExpressions())
			{
				int validValuesValueExpressionCount = paramDef.ValidValuesValueExpressionCount;
				Global.Tracer.Assert(validValuesValueExpressionCount != 0, "(0 != count)");
				Global.Tracer.Assert(paramDef.HasValidValuesLabelExpressions() && validValuesValueExpressionCount == paramDef.ValidValuesLabelExpressionCount);
				SetupExprHost(paramDef);
				parameter.ValidValues = new ValidValueList(validValuesValueExpressionCount);
				for (int i = 0; i < validValuesValueExpressionCount; i++)
				{
					object obj = EvaluateValidValueExpr(paramDef, i);
					object obj2 = EvaluateValidValueLabelExpr(paramDef, i);
					bool flag = obj is object[];
					bool flag2 = obj2 is object[];
					if (flag && (flag2 || obj2 == null))
					{
						object[] array = obj as object[];
						object[] array2 = obj2 as object[];
						if (array2 != null && array.Length != array2.Length)
						{
							m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidValidValueList, Severity.Error, ObjectType.ReportParameter, paramDef.Name, "ValidValues");
							throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
						}
						int num = array.Length;
						for (int j = 0; j < num; j++)
						{
							obj2 = ((array2 == null) ? null : array2[j]);
							ConvertAndAddValidValue(parameter, paramDef, array[j], obj2);
						}
					}
					else
					{
						if (flag || (flag2 && obj2 != null))
						{
							m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidValidValueList, Severity.Error, ObjectType.ReportParameter, paramDef.Name, "ValidValues");
							throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
						}
						ConvertAndAddValidValue(parameter, paramDef, obj, obj2);
					}
				}
			}
			return true;
		}

		internal void ConvertAndAddValidValue(ParameterInfo parameter, IParameterDef paramDef, object value, object label)
		{
			value = ConvertValue(value, paramDef, isDefaultValue: false);
			string value2 = label as string;
			value2 = ApplySandboxStringRestriction(value2, paramDef.Name, "Label");
			if (ValidateValue(value, paramDef, "ValidValues"))
			{
				parameter.AddValidValue(value, value2);
			}
		}

		protected static string ApplySandboxRestriction(ref string value, string paramName, string propertyName, OnDemandProcessingContext odpContext, int maxStringResultLength)
		{
			if (maxStringResultLength != -1 && value != null && value.Length > maxStringResultLength)
			{
				value = null;
				odpContext.ErrorContext.Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, ObjectType.ReportParameter, paramName, propertyName);
			}
			return value;
		}
	}
}
