using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ReportRuntime : IErrorContext
	{
		private sealed class ExpressionHostLoader : MarshalByRefObject
		{
			private static readonly Hashtable ExpressionHosts;

			private const string ExprHostRootType = "ReportExprHostImpl";

			private static readonly byte[] ReportExpressionsDefaultEvidencePK;

			private static readonly Assembly ProcessingObjectModelAssembly;

			internal static ReportExprHost LoadExprHost(byte[] exprHostBytes, string exprHostAssemblyName, bool parametersOnly, ObjectModel objectModel, StringList codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				Type typeFromHandle = typeof(ExpressionHostLoader);
				return ((ExpressionHostLoader)Activator.CreateInstance(typeFromHandle)).LoadExprHostRemoteEntryPoint(exprHostBytes, exprHostAssemblyName, parametersOnly, objectModel, codeModules, assemblyLoadContext);
			}

			internal static ReportExprHost LoadExprHostIntoCurrentAppDomain(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, bool parametersOnly, ObjectModel objectModel, StringList codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				if (codeModules != null && 0 < codeModules.Count)
				{
					RevertImpersonationContext.RunFromRestrictedCasContext(delegate
					{
						for (int num = codeModules.Count - 1; num >= 0; num--)
						{
							Assembly.Load(codeModules[num]);
						}
					});
				}
				Assembly assembly;
				if (assemblyLoadContext == null)
					assembly = LoadExprHostAssembly(exprHostBytes, exprHostAssemblyName, evidence);
				else
					assembly = assemblyLoadContext.LoadFromStream(new MemoryStream(exprHostBytes));
				Type type = assembly.GetType("ReportExprHostImpl");
				try
				{
					return (ReportExprHost)type.GetConstructors()[0].Invoke(new object[2]
					{
						parametersOnly,
						objectModel
					});
				}
				catch (Exception ex)
				{
					if (assembly.GetName().Version >= new Version(8, 0, 700))
					{
						throw ex;
					}
					return (ReportExprHost)type.GetConstructors()[0].Invoke(new object[1]
					{
						parametersOnly
					});
				}
			}

			private static Assembly LoadExprHostAssembly(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence)
			{
				lock (ExpressionHosts.SyncRoot)
				{
					Assembly assembly = (Assembly)ExpressionHosts[exprHostAssemblyName];
					if (assembly == null)
					{
						if (evidence == null)
						{
							evidence = CreateDefaultExpressionHostEvidence(exprHostAssemblyName);
						}
						assembly = Assembly.Load(exprHostBytes);
						ExpressionHosts.Add(exprHostAssemblyName, assembly);
					}
					return assembly;
				}
			}

			private static Evidence CreateDefaultExpressionHostEvidence(string exprHostAssemblyName)
			{
				Evidence evidence = new Evidence();
				return evidence;
			}

			private ReportExprHost LoadExprHostRemoteEntryPoint(byte[] exprHostBytes, string exprHostAssemblyName, bool parametersOnly, ObjectModel objectModel, StringList codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				return LoadExprHostIntoCurrentAppDomain(exprHostBytes, exprHostAssemblyName, null, parametersOnly, objectModel, codeModules, assemblyLoadContext);
			}

			static ExpressionHostLoader()
			{
				ExpressionHosts = new Hashtable();
				ReportExpressionsDefaultEvidencePK = new byte[160]
				{
					0,
					36,
					0,
					0,
					4,
					128,
					0,
					0,
					148,
					0,
					0,
					0,
					6,
					2,
					0,
					0,
					0,
					36,
					0,
					0,
					82,
					83,
					65,
					49,
					0,
					4,
					0,
					0,
					1,
					0,
					1,
					0,
					81,
					44,
					142,
					135,
					46,
					40,
					86,
					158,
					115,
					59,
					203,
					18,
					55,
					148,
					218,
					181,
					81,
					17,
					160,
					87,
					11,
					59,
					61,
					77,
					227,
					121,
					65,
					83,
					222,
					165,
					239,
					183,
					195,
					254,
					169,
					242,
					216,
					35,
					108,
					255,
					50,
					12,
					79,
					208,
					234,
					213,
					246,
					119,
					136,
					11,
					246,
					193,
					129,
					242,
					150,
					199,
					81,
					197,
					246,
					230,
					91,
					4,
					211,
					131,
					76,
					2,
					247,
					146,
					254,
					224,
					254,
					69,
					41,
					21,
					212,
					74,
					254,
					116,
					160,
					194,
					126,
					13,
					142,
					75,
					141,
					4,
					236,
					82,
					168,
					226,
					129,
					224,
					31,
					244,
					126,
					125,
					105,
					78,
					108,
					114,
					117,
					160,
					154,
					252,
					191,
					216,
					204,
					130,
					112,
					90,
					6,
					178,
					15,
					214,
					239,
					97,
					235,
					186,
					104,
					115,
					226,
					156,
					140,
					15,
					44,
					174,
					221,
					162
				};
				ProcessingObjectModelAssembly = typeof(ObjectModel).Assembly;
				AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyHandler;
			}

			private static Assembly ResolveAssemblyHandler(object sender, ResolveEventArgs args)
			{
				if (args.Name != null && args.Name.StartsWith("Microsoft.ReportingServices.Processing", StringComparison.Ordinal))
				{
					return ProcessingObjectModelAssembly;
				}
				return null;
			}
		}

		private Assembly m_exprHostAssembly;

		private ReportExprHost m_reportExprHost;

		private ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private ObjectModelImpl m_reportObjectModel;

		private ErrorContext m_errorContext;

		private ReportProcessing.IScope m_currentScope;

		private ReportRuntime m_topLevelReportRuntime;

		private IActionOwner m_currentActionOwner;

		internal ReportExprHost ReportExprHost => m_reportExprHost;

		internal ReportProcessing.IScope CurrentScope
		{
			get
			{
				return m_currentScope;
			}
			set
			{
				m_currentScope = value;
			}
		}

		internal ObjectType ObjectType
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

		internal ObjectModelImpl ReportObjectModel => m_reportObjectModel;

		internal IActionOwner CurrentActionOwner
		{
			get
			{
				return m_currentActionOwner;
			}
			set
			{
				m_currentActionOwner = value;
			}
		}

		internal ReportRuntime(ObjectModelImpl reportObjectModel, ErrorContext errorContext)
		{
			m_objectType = ObjectType.Report;
			m_reportObjectModel = reportObjectModel;
			m_errorContext = errorContext;
		}

		internal ReportRuntime(ObjectModelImpl reportObjectModel, ErrorContext errorContext, ReportExprHost copyReportExprHost, ReportRuntime topLevelReportRuntime)
			: this(reportObjectModel, errorContext)
		{
			m_reportExprHost = copyReportExprHost;
			m_topLevelReportRuntime = topLevelReportRuntime;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			m_errorContext.Register(code, severity, m_objectType, m_objectName, m_propertyName, arguments);
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			m_errorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
		}

		internal static string GetErrorName(DataFieldStatus status, string exceptionMessage)
		{
			if (exceptionMessage != null)
			{
				return exceptionMessage;
			}
			switch (status)
			{
			case DataFieldStatus.Overflow:
				return "OverflowException.";
			case DataFieldStatus.UnSupportedDataType:
				return "UnsupportedDatatypeException.";
			case DataFieldStatus.IsError:
				return "FieldValueException.";
			default:
				return null;
			}
		}

		internal string EvaluateReportLanguageExpression(Report report, out CultureInfo language)
		{
			if (!EvaluateSimpleExpression(report.Language, report.ObjectType, report.Name, "Language", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.Language, ref result))
					{
						Global.Tracer.Assert(report.ReportExprHost != null);
						result.Value = report.ReportExprHost.ReportLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateSpecificLanguage(ProcessStringResult(result).Value, this, out language);
		}

		internal VariantResult EvaluateParamDefaultValue(ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.DefaultExpressions != null);
			ExpressionInfo expressionInfo = paramDef.DefaultExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, ObjectType.ReportParameter, paramDef.Name, "DefaultValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expressionInfo, ref result, isArrayAllowed: true);
			return result;
		}

		internal VariantResult EvaluateParamValidValue(ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesValueExpressions != null);
			ExpressionInfo expressionInfo = paramDef.ValidValuesValueExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, ObjectType.ReportParameter, paramDef.Name, "ValidValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValuesHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValuesHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expressionInfo, ref result, isArrayAllowed: true);
			return result;
		}

		internal VariantResult EvaluateParamValidValueLabel(ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesLabelExpressions != null);
			ExpressionInfo expressionInfo = paramDef.ValidValuesLabelExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, ObjectType.ReportParameter, paramDef.Name, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValueLabelsHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValueLabelsHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expressionInfo, ref result, isArrayAllowed: true);
			if (!result.ErrorOccurred && result.Value is object[])
			{
				try
				{
					VariantResult result2 = default(VariantResult);
					object[] array = result.Value as object[];
					for (int i = 0; i < array.Length; i++)
					{
						result2.Value = array[i];
						ProcessLabelResult(ref result2);
						if (result2.ErrorOccurred)
						{
							result.ErrorOccurred = true;
							return result;
						}
						array[i] = result2.Value;
					}
					return result;
				}
				catch
				{
					RegisterInvalidExpressionDataTypeWarning();
					result.ErrorOccurred = true;
					return result;
				}
			}
			if (!result.ErrorOccurred)
			{
				ProcessLabelResult(ref result);
			}
			return result;
		}

		internal object EvaluateDataValueValueExpression(DataValue value, ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(value.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value.Value, ref result))
					{
						Global.Tracer.Assert(value.ExprHost != null);
						result.Value = value.ExprHost.DataValueValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value.Value, ref result);
			return result.Value;
		}

		internal string EvaluateDataValueNameExpression(DataValue value, ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(value.Name, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value.Name, ref result))
					{
						Global.Tracer.Assert(value.ExprHost != null);
						result.Value = value.ExprHost.DataValueNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateFilterVariantExpression(Filter filter, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(filter.Expression, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null);
						result.Value = filter.ExprHost.FilterExpressionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(filter.Expression, ref result, isArrayAllowed: false);
			return result;
		}

		internal StringResult EvaluateFilterStringExpression(Filter filter, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(filter.Expression, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null);
						result.Value = filter.ExprHost.FilterExpressionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal VariantResult EvaluateFilterVariantValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			if (!EvaluateSimpleExpression(expressionInfo, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = filter.ExprHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expressionInfo, ref result, isArrayAllowed: true);
			return result;
		}

		internal FloatResult EvaluateFilterIntegerOrFloatValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			if (!EvaluateIntegerOrFloatExpression(expressionInfo, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = filter.ExprHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result);
		}

		internal IntegerResult EvaluateFilterIntegerValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			if (!EvaluateIntegerExpression(expressionInfo, canBeConstant: false, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = filter.ExprHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result);
		}

		internal StringResult EvaluateFilterStringValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			if (!EvaluateSimpleExpression(expressionInfo, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = filter.ExprHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal object EvaluateQueryParamValue(ExpressionInfo paramValue, IndexedExprHost queryParamsExprHost, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(paramValue, objectType, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramValue, ref result))
					{
						m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InQuery;
						Global.Tracer.Assert(queryParamsExprHost != null && paramValue.ExprHostID >= 0);
						result.Value = queryParamsExprHost[paramValue.ExprHostID];
						m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InReport;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessVariantResult(paramValue, ref result, isArrayAllowed: true);
			return result.Value;
		}

		internal StringResult EvaluateConnectString(DataSource dataSource)
		{
			if (!EvaluateSimpleExpression(dataSource.ConnectStringExpression, ObjectType.DataSource, dataSource.Name, "ConnectString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataSource.ConnectStringExpression, ref result))
					{
						m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InQuery;
						Global.Tracer.Assert(dataSource.ExprHost != null);
						result.Value = dataSource.ExprHost.ConnectStringExpr;
						m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InReport;
					}
				}
				catch (Exception ex)
				{
					if (!(ex is ReportProcessingException_NonExistingParameterReference))
					{
						result = new VariantResult(errorOccurred: true, null);
					}
					else
					{
						RegisterRuntimeErrorInExpression(ref result, ex);
					}
				}
			}
			return ProcessStringResult(result);
		}

		internal StringResult EvaluateCommandText(DataSet dataSet)
		{
			if (!EvaluateSimpleExpression(dataSet.Query.CommandText, ObjectType.Query, dataSet.Name, "CommandText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataSet.Query.CommandText, ref result))
					{
						m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InQuery;
						Global.Tracer.Assert(dataSet.ExprHost != null);
						result.Value = dataSet.ExprHost.QueryCommandTextExpr;
						m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InReport;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal object EvaluateFieldValueExpression(Field field)
		{
			if (!EvaluateSimpleExpression(field.Value, ObjectType.Field, field.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(field.Value, ref result))
					{
						Global.Tracer.Assert(field.ExprHost != null);
						result.Value = field.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(field.Value, ref result);
			return result.Value;
		}

		internal VariantResult EvaluateAggregateVariantOrBinaryParamExpr(DataAggregateInfo aggregateInfo, int index, IErrorContext errorContext)
		{
			Global.Tracer.Assert(aggregateInfo.Expressions != null);
			ExpressionInfo expressionInfo = aggregateInfo.Expressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(aggregateInfo.ExpressionHosts != null && expressionInfo.ExprHostID >= 0);
						result.Value = aggregateInfo.ExpressionHosts[index].ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, errorContext, isError: false);
				}
			}
			ProcessVariantOrBinaryResult(expressionInfo, ref result, errorContext, isAggregate: true);
			return result;
		}

		internal bool EvaluateParamValueOmitExpression(ParameterValue paramVal, ObjectType objectType, string objectName)
		{
			if (!EvaluateBooleanExpression(paramVal.Omit, canBeConstant: true, objectType, objectName, "ParameterOmit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Omit, ref result))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null);
						result.Value = paramVal.ExprHost.OmitExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal object EvaluateParamVariantValueExpression(ParameterValue paramVal, ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Value, ref result))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null);
						result.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(paramVal.Value, ref result, isArrayAllowed: true);
			return result.Value;
		}

		internal ParameterValueResult EvaluateParameterValueExpression(ParameterValue paramVal, ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Value, ref result))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null);
						result.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessParameterValueResult(paramVal.Value, result);
		}

		internal bool EvaluateStartHiddenExpression(Visibility visibility, IVisibilityHiddenExprHost hiddenExprHostRI, ObjectType objectType, string objectName)
		{
			if (!EvaluateBooleanExpression(visibility.Hidden, canBeConstant: true, objectType, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(visibility.Hidden, ref result))
					{
						result.Value = hiddenExprHostRI.VisibilityHiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			return ProcessBooleanResult(result, stopOnError: true, objectType, objectName).Value;
		}

		internal bool EvaluateStartHiddenExpression(Visibility visibility, IndexedExprHost hiddenExprHostIdx, ObjectType objectType, string objectName)
		{
			if (!EvaluateBooleanExpression(visibility.Hidden, canBeConstant: true, objectType, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(visibility.Hidden, ref result))
					{
						Global.Tracer.Assert(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0);
						result.Value = hiddenExprHostIdx[visibility.Hidden.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			return ProcessBooleanResult(result, stopOnError: true, objectType, objectName).Value;
		}

		internal VariantResult EvaluateReportItemLabelExpression(ReportItem reportItem)
		{
			if (!EvaluateSimpleExpression(reportItem.Label, reportItem.ObjectType, reportItem.Name, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(reportItem.Label, ref result))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null);
						result.Value = reportItem.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(reportItem.Label, ref result);
			return result;
		}

		internal string EvaluateReportItemBookmarkExpression(ReportItem reportItem)
		{
			if (!EvaluateSimpleExpression(reportItem.Bookmark, reportItem.ObjectType, reportItem.Name, "Bookmark", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(reportItem.Bookmark, ref result))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null);
						result.Value = reportItem.ExprHost.BookmarkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemToolTipExpression(ReportItem reportItem)
		{
			if (!EvaluateSimpleExpression(reportItem.ToolTip, reportItem.ObjectType, reportItem.Name, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(reportItem.ToolTip, ref result))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null);
						result.Value = reportItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateActionLabelExpression(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
						result.Value = actionItem.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemHyperlinkURLExpression(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Hyperlink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
						result.Value = actionItem.ExprHost.HyperlinkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemDrillthroughReportName(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "DrillthroughReportName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
						result.Value = actionItem.ExprHost.DrillThroughReportNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemBookmarkLinkExpression(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BookmarkLink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
						result.Value = actionItem.ExprHost.BookmarkLinkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateImageStringValueExpression(Image image, out bool errorOccurred)
		{
			errorOccurred = false;
			if (!EvaluateSimpleExpression(image.Value, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.Value, ref result))
					{
						Global.Tracer.Assert(image.ImageExprHost != null);
						result.Value = image.ImageExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal byte[] EvaluateImageBinaryValueExpression(Image image, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(image.Value, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.Value, ref result))
					{
						Global.Tracer.Assert(image.ImageExprHost != null);
						result.Value = image.ImageExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateImageMIMETypeExpression(Image image)
		{
			if (!EvaluateSimpleExpression(image.MIMEType, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.MIMEType, ref result))
					{
						Global.Tracer.Assert(image.ImageExprHost != null);
						result.Value = image.ImageExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateTextBoxValueExpression(TextBox textBox)
		{
			if (!EvaluateSimpleExpression(textBox.Value, textBox.ObjectType, textBox.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textBox.Value, ref result))
					{
						Global.Tracer.Assert(textBox.TextBoxExprHost != null);
						result.Value = textBox.TextBoxExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(textBox.Value, ref result);
			return result;
		}

		internal bool EvaluateTextBoxInitialToggleStateExpression(TextBox textBox)
		{
			if (!EvaluateBooleanExpression(textBox.InitialToggleState, canBeConstant: true, textBox.ObjectType, textBox.Name, "InitialState", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textBox.InitialToggleState, ref result))
					{
						Global.Tracer.Assert(textBox.ExprHost != null);
						result.Value = textBox.TextBoxExprHost.ToggleImageInitialStateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal object EvaluateUserSortExpression(TextBox textBox)
		{
			int sortExpressionIndex = textBox.UserSort.SortExpressionIndex;
			ISortFilterScope sortTarget = textBox.UserSort.SortTarget;
			Global.Tracer.Assert(sortTarget.UserSortExpressions != null && 0 <= sortExpressionIndex && sortExpressionIndex < sortTarget.UserSortExpressions.Count);
			ExpressionInfo expressionInfo = sortTarget.UserSortExpressions[sortExpressionIndex];
			if (!EvaluateSimpleExpression(expressionInfo, textBox.ObjectType, textBox.Name, "SortExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(sortTarget.UserSortExpressionsHost != null);
						result.Value = sortTarget.UserSortExpressionsHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessVariantResult(expressionInfo, ref result);
			if (result.Value == null)
			{
				result.Value = DBNull.Value;
			}
			return result.Value;
		}

		internal VariantResult EvaluateGroupingLabelExpression(Grouping grouping, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(grouping.GroupLabel, objectType, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(grouping.GroupLabel, ref result))
					{
						Global.Tracer.Assert(grouping.ExprHost != null);
						result.Value = grouping.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(grouping.GroupLabel, ref result);
			return result;
		}

		internal object EvaluateRuntimeExpression(ReportProcessing.RuntimeExpressionInfo runtimeExpression, ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(runtimeExpression.Expression, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(runtimeExpression.Expression, ref result))
					{
						Global.Tracer.Assert(runtimeExpression.ExpressionsHost != null && runtimeExpression.Expression.ExprHostID >= 0);
						result.Value = runtimeExpression.ExpressionsHost[runtimeExpression.Expression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessVariantResult(runtimeExpression.Expression, ref result);
			if (result.ErrorOccurred)
			{
				if (result.FieldStatus != 0)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, new string[1]
					{
						GetErrorName(result.FieldStatus, result.ExceptionMessage)
					});
				}
				else
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, Array.Empty<string>());
				}
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			if (result.Value == null)
			{
				result.Value = DBNull.Value;
			}
			return result.Value;
		}

		internal object EvaluateOWCChartData(OWCChart chart, ExpressionInfo chartDataExpression)
		{
			if (!EvaluateSimpleExpression(chartDataExpression, chart.ObjectType, chart.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataExpression, ref result))
					{
						Global.Tracer.Assert(chart.OWCChartExprHost != null && chart.OWCChartExprHost.OWCChartColumnHosts != null && chartDataExpression.ExprHostID >= 0);
						result.Value = chart.OWCChartExprHost.OWCChartColumnHosts[chartDataExpression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartDataExpression, ref result);
			return result.Value;
		}

		internal string EvaluateSubReportNoRowsExpression(SubReport subReport, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(subReport.NoRows, ObjectType.Subreport, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(subReport.NoRows, ref result))
					{
						Global.Tracer.Assert(subReport.SubReportExprHost != null);
						result.Value = subReport.SubReportExprHost.NoRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateDataRegionNoRowsExpression(DataRegion region, ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(region.NoRows, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(region.NoRows, ref result))
					{
						Global.Tracer.Assert(region.ExprHost != null);
						result.Value = ((DataRegionExprHost)region.ExprHost).NoRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal object EvaluateChartDataPointDataValueExpression(ChartDataPoint dataPoint, ExpressionInfo dataPointDataValueExpression, string objectName)
		{
			if (!EvaluateSimpleExpression(dataPointDataValueExpression, ObjectType.Chart, objectName, "DataPoint", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataPointDataValueExpression, ref result))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null && dataPointDataValueExpression.ExprHostID >= 0);
						result.Value = dataPoint.ExprHost[dataPointDataValueExpression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(dataPointDataValueExpression, ref result);
			return result.Value;
		}

		internal object EvaluateChartStaticHeadingLabelExpression(ChartHeading chartHeading, ExpressionInfo expression, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, ObjectType.Chart, objectName, "HeadingLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Chart chart = (Chart)chartHeading.DataRegionDef;
						IndexedExprHost indexedExprHost = null;
						if (chart.ChartExprHost != null)
						{
							indexedExprHost = ((!chartHeading.IsColumn) ? chart.ChartExprHost.StaticRowLabelsHost : chart.ChartExprHost.StaticColumnLabelsHost);
						}
						Global.Tracer.Assert(indexedExprHost != null && expression.ExprHostID >= 0);
						result.Value = indexedExprHost[expression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expression, ref result);
			return result.Value;
		}

		internal object EvaluateChartDynamicHeadingLabelExpression(ChartHeading chartHeading, ExpressionInfo expression, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, ObjectType.Chart, objectName, "HeadingLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(chartHeading.ExprHost != null);
						result.Value = chartHeading.ExprHost.HeadingLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expression, ref result);
			return result.Value;
		}

		internal string EvaluateChartTitleCaptionExpression(ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Caption, ref result))
					{
						Global.Tracer.Assert(title.ExprHost != null);
						result.Value = title.ExprHost.CaptionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartDataLabelValueExpression(ChartDataPoint dataPoint, string objectName, object[] dataLabelStyleAttributeValues)
		{
			Global.Tracer.Assert(dataPoint.DataLabel != null);
			if (!EvaluateSimpleExpression(dataPoint.DataLabel.Value, ObjectType.Chart, objectName, "DataLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataPoint.DataLabel.Value, ref result))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null);
						result.Value = dataPoint.ExprHost.DataLabelValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(dataPoint.DataLabel.Value, ref result);
			if (result.Value != null)
			{
				string format = null;
				if (dataPoint.DataLabel.StyleClass != null)
				{
					AttributeInfo attributeInfo = dataPoint.DataLabel.StyleClass.StyleAttributes["Format"];
					if (attributeInfo != null)
					{
						format = ((!attributeInfo.IsExpression) ? attributeInfo.Value : ((string)dataLabelStyleAttributeValues[attributeInfo.IntValue]));
					}
				}
				string text = (string)(result.Value = ((result.Value is IFormattable) ? ((IFormattable)result.Value).ToString(format, Thread.CurrentThread.CurrentCulture) : result.Value.ToString()));
			}
			return (string)result.Value;
		}

		internal object EvaluateChartAxisValueExpression(AxisExprHost exprHost, ExpressionInfo expression, string objectName, string propertyName, Axis.ExpressionType type)
		{
			if (!EvaluateSimpleExpression(expression, ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(exprHost != null);
						switch (type)
						{
						case Axis.ExpressionType.Min:
							result.Value = exprHost.AxisMinExpr;
							break;
						case Axis.ExpressionType.Max:
							result.Value = exprHost.AxisMaxExpr;
							break;
						case Axis.ExpressionType.CrossAt:
							result.Value = exprHost.AxisCrossAtExpr;
							break;
						case Axis.ExpressionType.MajorInterval:
							result.Value = exprHost.AxisMajorIntervalExpr;
							break;
						case Axis.ExpressionType.MinorInterval:
							result.Value = exprHost.AxisMinorIntervalExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expression, ref result);
			return result.Value;
		}

		internal string EvaluateStyleBorderColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderStyle(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderWidth(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundGradientEndColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientEndColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundGradientEndColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundGradientType(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBackgroundGradientType(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundRepeat(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundRepeat", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundRepeatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBackgroundRepeat(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontStyle(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateFontStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontFamily(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontFamily", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontFamilyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleFontSize(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateFontSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontWeight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontWeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontWeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateFontWeight(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFormat(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Format", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FormatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleTextDecoration(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextDecoration", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.TextDecorationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateTextDecoration(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleTextAlign(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextAlign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.TextAlignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateTextAlign(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleVerticalAlign(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "VerticalAlign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.VerticalAlignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateVerticalAlign(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingBottom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLineHeight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "LineHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.LineHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateLineHeight(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleDirection(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Direction", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.DirectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateDirection(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleWritingMode(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "WritingMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.WritingModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateWritingMode(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLanguage(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Language", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.LanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo culture;
			return ProcessingValidator.ValidateSpecificLanguage(ProcessStringResult(result).Value, this, out culture);
		}

		internal string EvaluateStyleUnicodeBiDi(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "UnicodeBiDi", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.UnicodeBiDiExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateUnicodeBiDi(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleCalendar(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Calendar", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.CalendarExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateCalendar(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleNumeralLanguage(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "NumeralLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.NumeralLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo culture;
			return ProcessingValidator.ValidateLanguage(ProcessStringResult(result).Value, this, out culture);
		}

		internal object EvaluateStyleNumeralVariant(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, canBeConstant: true, objectType, objectName, "NumeralVariant", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.NumeralVariantExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			IntegerResult integerResult = ProcessIntegerResult(result);
			if (integerResult.ErrorOccurred)
			{
				return null;
			}
			return ProcessingValidator.ValidateNumeralVariant(integerResult.Value, this);
		}

		internal string EvaluateStyleBackgroundUrlImageValue(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, canBeConstant: true, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleBackgroundEmbeddedImageValue(Style style, ExpressionInfo expression, EmbeddedImageHashtable embeddedImages, ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, canBeConstant: true, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateEmbeddedImageName(ProcessStringResult(result).Value, embeddedImages, this);
		}

		internal byte[] EvaluateStyleBackgroundDatabaseImageValue(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, canBeConstant: true, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBinaryResult(result).Value;
		}

		internal string EvaluateStyleBackgroundImageMIMEType(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, canBeConstant: true, objectType, objectName, "BackgroundImageMIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundImageMIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateMimeType(ProcessStringResult(result).Value, this);
		}

		private bool EvaluateSimpleExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			m_objectType = objectType;
			m_objectName = objectName;
			m_propertyName = propertyName;
			if (m_topLevelReportRuntime != null)
			{
				m_topLevelReportRuntime.ObjectType = objectType;
				m_topLevelReportRuntime.ObjectName = objectName;
				m_topLevelReportRuntime.PropertyName = propertyName;
			}
			return EvaluateSimpleExpression(expression, out result);
		}

		private bool EvaluateSimpleExpression(ExpressionInfo expression, out VariantResult result)
		{
			result = default(VariantResult);
			if (expression != null)
			{
				switch (expression.Type)
				{
				case ExpressionInfo.Types.Constant:
					result.Value = expression.Value;
					return true;
				case ExpressionInfo.Types.Field:
					try
					{
						FieldImpl fieldImpl = m_reportObjectModel.FieldsImpl[expression.IntValue];
						if (fieldImpl.IsMissing)
						{
							result.Value = null;
							return true;
						}
						if (fieldImpl.FieldStatus != 0)
						{
							result.ErrorOccurred = true;
							result.FieldStatus = fieldImpl.FieldStatus;
							result.ExceptionMessage = fieldImpl.ExceptionMessage;
							result.Value = null;
							return true;
						}
						result.Value = m_reportObjectModel.FieldsImpl[expression.IntValue].Value;
					}
					catch (ReportProcessingException_NoRowsFieldAccess e)
					{
						RegisterRuntimeWarning(e, this);
						result.Value = null;
						return true;
					}
					return true;
				case ExpressionInfo.Types.Token:
				{
					Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = m_reportObjectModel.DataSetsImpl[expression.Value];
					result.Value = dataSet.RewrittenCommandText;
					return true;
				}
				case ExpressionInfo.Types.Aggregate:
					return false;
				case ExpressionInfo.Types.Expression:
					return false;
				default:
					Global.Tracer.Assert(condition: false);
					return true;
				}
			}
			return true;
		}

		private bool EvaluateComplexExpression(ExpressionInfo expression, ref VariantResult result)
		{
			if (expression != null)
			{
				switch (expression.Type)
				{
				case ExpressionInfo.Types.Aggregate:
					result.Value = m_reportObjectModel.AggregatesImpl[expression.Value];
					return true;
				case ExpressionInfo.Types.Expression:
					return false;
				default:
					Global.Tracer.Assert(condition: false);
					return true;
				}
			}
			return true;
		}

		private void RegisterRuntimeWarning(Exception e, IErrorContext iErrorContext)
		{
			if (e is ReportProcessingException_NoRowsFieldAccess)
			{
				iErrorContext.Register(ProcessingErrorCode.rsRuntimeErrorInExpression, Severity.Warning, e.Message);
				return;
			}
			if (Global.Tracer.TraceError)
			{
				Global.Tracer.Trace("Caught unexpected exception inside of RegisterRuntimeWarning.");
			}
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		private void RegisterRuntimeErrorInExpressionAndStop(ref VariantResult result, Exception e)
		{
			RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e)
		{
			RegisterRuntimeErrorInExpression(ref result, e, this, isError: false);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e, IErrorContext iErrorContext, bool isError)
		{
			if (e is ReportProcessingException_FieldError)
			{
				result.FieldStatus = ((ReportProcessingException_FieldError)e).Status;
				if (DataFieldStatus.IsMissing == result.FieldStatus)
				{
					result = new VariantResult(errorOccurred: false, null);
				}
				else
				{
					result = new VariantResult(errorOccurred: true, null);
				}
				return;
			}
			if (e is ReportProcessingException_InvalidOperationException)
			{
				result = new VariantResult(errorOccurred: true, null);
				return;
			}
			if (e is ReportProcessingException_UserProfilesDependencies)
			{
				iErrorContext.Register(ProcessingErrorCode.rsRuntimeUserProfileDependency, Severity.Error, (string[])null);
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			string text;
			if (e != null)
			{
				try
				{
					text = ((e.InnerException == null) ? e.Message : e.InnerException.Message);
				}
				catch
				{
					text = RPRes.NonClsCompliantException;
				}
			}
			else
			{
				text = RPRes.NonClsCompliantException;
			}
			iErrorContext.Register(ProcessingErrorCode.rsRuntimeErrorInExpression, (!isError) ? Severity.Warning : Severity.Error, text);
			if (e is ReportProcessingException_NoRowsFieldAccess)
			{
				result = new VariantResult(errorOccurred: false, null);
				return;
			}
			if (isError)
			{
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			result = new VariantResult(errorOccurred: true, null);
		}

		private bool EvaluateBooleanExpression(ExpressionInfo expression, bool canBeConstant, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.BoolValue;
				}
				else
				{
					result.ErrorOccurred = true;
					RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result)
		{
			return ProcessBooleanResult(result, stopOnError: false, ObjectType.Report, null);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result, bool stopOnError, ObjectType objectType, string objectName)
		{
			BooleanResult result2 = default(BooleanResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				if (stopOnError && result.FieldStatus != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, objectType, objectName, "Hidden", GetErrorName(result.FieldStatus, result.ExceptionMessage));
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
			else if (result.Value is bool)
			{
				result2.Value = (bool)result.Value;
			}
			else if (result.Value == null || DBNull.Value == result.Value)
			{
				result2.Value = false;
			}
			else
			{
				result2.ErrorOccurred = true;
				if (stopOnError)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, objectType, objectName, "Hidden");
					throw new ReportProcessingException(m_errorContext.Messages);
				}
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private bool EvaluateBinaryExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BinaryResult ProcessBinaryResult(VariantResult result)
		{
			BinaryResult result2 = default(BinaryResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is byte[])
			{
				result2.Value = (byte[])result.Value;
			}
			else if (result.Value == null || DBNull.Value == result.Value)
			{
				result2.Value = null;
			}
			else
			{
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private StringResult ProcessStringResult(VariantResult result)
		{
			StringResult result2 = default(StringResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is string)
			{
				result2.Value = (string)result.Value;
			}
			else if (result.Value is char)
			{
				result2.Value = new string((char)result.Value, 1);
			}
			else if (result.Value == null || DBNull.Value == result.Value)
			{
				result2.Value = null;
			}
			else if (result.Value is Guid)
			{
				result.Value = ((Guid)result.Value).ToString();
			}
			else
			{
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private void ProcessLabelResult(ref VariantResult result)
		{
			if (!result.ErrorOccurred && !(result.Value is string))
			{
				if (result.Value is char)
				{
					result.Value = new string((char)result.Value, 1);
					return;
				}
				if (result.Value == null || DBNull.Value == result.Value)
				{
					result.Value = null;
					return;
				}
				if (result.Value is Guid)
				{
					result.Value = ((Guid)result.Value).ToString();
					return;
				}
				result.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
			}
		}

		private bool EvaluateIntegerExpression(ExpressionInfo expression, bool canBeConstant, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.IntValue;
				}
				else
				{
					result.ErrorOccurred = true;
					RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private IntegerResult ProcessIntegerResult(VariantResult result)
		{
			IntegerResult result2 = default(IntegerResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is int)
			{
				result2.Value = (int)result.Value;
			}
			else if (result.Value is byte)
			{
				result2.Value = Convert.ToInt32((byte)result.Value);
			}
			else if (result.Value is sbyte)
			{
				result2.Value = Convert.ToInt32((sbyte)result.Value);
			}
			else if (result.Value is short)
			{
				result2.Value = Convert.ToInt32((short)result.Value);
			}
			else if (result.Value is ushort)
			{
				result2.Value = Convert.ToInt32((ushort)result.Value);
			}
			else
			{
				if (result.Value is uint)
				{
					try
					{
						result2.Value = Convert.ToInt32((uint)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is long)
				{
					try
					{
						result2.Value = Convert.ToInt32((long)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is ulong)
				{
					try
					{
						result2.Value = Convert.ToInt32((ulong)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is TimeSpan)
				{
					try
					{
						result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private bool EvaluateIntegerOrFloatExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private FloatResult ProcessIntegerOrFloatResult(VariantResult result)
		{
			FloatResult result2 = default(FloatResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is int)
			{
				result2.Value = (int)result.Value;
			}
			else if (result.Value is byte)
			{
				result2.Value = Convert.ToInt32((byte)result.Value);
			}
			else if (result.Value is sbyte)
			{
				result2.Value = Convert.ToInt32((sbyte)result.Value);
			}
			else if (result.Value is short)
			{
				result2.Value = Convert.ToInt32((short)result.Value);
			}
			else if (result.Value is ushort)
			{
				result2.Value = Convert.ToInt32((ushort)result.Value);
			}
			else
			{
				if (result.Value is uint)
				{
					try
					{
						result2.Value = Convert.ToInt32((uint)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is long)
				{
					try
					{
						result2.Value = Convert.ToInt32((long)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is ulong)
				{
					try
					{
						result2.Value = Convert.ToInt32((ulong)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is TimeSpan)
				{
					try
					{
						result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				}
				if (result.Value is double)
				{
					result2.Value = (double)result.Value;
				}
				else if (result.Value is float)
				{
					result2.Value = Convert.ToDouble((float)result.Value);
				}
				else
				{
					if (result.Value is decimal)
					{
						try
						{
							result2.Value = Convert.ToDouble((decimal)result.Value);
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					}
					result2.ErrorOccurred = true;
					RegisterInvalidExpressionDataTypeWarning();
				}
			}
			return result2;
		}

		private void ProcessVariantResult(ExpressionInfo expression, ref VariantResult result)
		{
			ProcessVariantResult(expression, ref result, isArrayAllowed: false);
		}

		private void ProcessVariantResult(ExpressionInfo expression, ref VariantResult result, bool isArrayAllowed)
		{
			if (expression == null || expression.Type == ExpressionInfo.Types.Constant || result.ErrorOccurred || IsVariant(result.Value))
			{
				return;
			}
			if (result.Value == null || result.Value == DBNull.Value)
			{
				result.Value = null;
			}
			else if (!isArrayAllowed || !(result.Value is ICollection))
			{
				if (result.Value is Guid)
				{
					result.Value = ((Guid)result.Value).ToString();
					return;
				}
				result.ErrorOccurred = true;
				result.Value = null;
				RegisterInvalidExpressionDataTypeWarning();
			}
		}

		private void ProcessVariantOrBinaryResult(ExpressionInfo expression, ref VariantResult result, IErrorContext iErrorContext, bool isAggregate)
		{
			if (expression == null || expression.Type == ExpressionInfo.Types.Constant || result.ErrorOccurred || IsVariant(result.Value) || result.Value is byte[])
			{
				return;
			}
			if (result.Value == null || result.Value == DBNull.Value)
			{
				result.Value = null;
				return;
			}
			if (result.Value is Guid)
			{
				result.Value = ((Guid)result.Value).ToString();
				return;
			}
			result.ErrorOccurred = true;
			result.Value = null;
			if (!isAggregate)
			{
				RegisterInvalidExpressionDataTypeWarning();
			}
		}

		private ParameterValueResult ProcessParameterValueResult(ExpressionInfo expression, VariantResult result)
		{
			ParameterValueResult result2 = default(ParameterValueResult);
			DataAggregate.DataTypeCode dataTypeCode = DataAggregate.DataTypeCode.Null;
			if (result.Value is Guid)
			{
				result.Value = ((Guid)result.Value).ToString();
			}
			if (!(result.Value is object[]))
			{
				dataTypeCode = DataAggregate.GetTypeCode(result.Value);
			}
			if (expression != null)
			{
				if (expression.Type == ExpressionInfo.Types.Constant)
				{
					result2.Value = expression.Value;
					result2.Type = DataType.String;
				}
				else if (result.ErrorOccurred)
				{
					result2.ErrorOccurred = true;
				}
				else
				{
					switch (dataTypeCode)
					{
					case DataAggregate.DataTypeCode.Boolean:
						result2.Value = result.Value;
						result2.Type = DataType.Boolean;
						break;
					case DataAggregate.DataTypeCode.DateTime:
						result2.Value = result.Value;
						result2.Type = DataType.DateTime;
						break;
					case DataAggregate.DataTypeCode.Single:
					case DataAggregate.DataTypeCode.Double:
					case DataAggregate.DataTypeCode.Decimal:
						result2.Value = Convert.ToDouble(result.Value, CultureInfo.CurrentCulture);
						result2.Type = DataType.Float;
						break;
					case DataAggregate.DataTypeCode.String:
					case DataAggregate.DataTypeCode.Char:
						result2.Value = Convert.ToString(result.Value, CultureInfo.CurrentCulture);
						result2.Type = DataType.String;
						break;
					case DataAggregate.DataTypeCode.Int16:
					case DataAggregate.DataTypeCode.Int32:
					case DataAggregate.DataTypeCode.UInt16:
					case DataAggregate.DataTypeCode.Byte:
					case DataAggregate.DataTypeCode.SByte:
						result2.Value = Convert.ToInt32(result.Value, CultureInfo.CurrentCulture);
						result2.Type = DataType.Integer;
						break;
					case DataAggregate.DataTypeCode.TimeSpan:
						try
						{
							result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
							result2.Type = DataType.Integer;
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					case DataAggregate.DataTypeCode.Int64:
					case DataAggregate.DataTypeCode.UInt32:
					case DataAggregate.DataTypeCode.UInt64:
						try
						{
							result2.Value = Convert.ToInt32(result.Value, CultureInfo.CurrentCulture);
							result2.Type = DataType.Integer;
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					default:
						if (result.Value == null || DBNull.Value == result.Value)
						{
							result2.Value = null;
							result2.Type = DataType.String;
						}
						else if (result.Value is object[])
						{
							result2.Value = result.Value;
							object[] array = result.Value as object[];
							Global.Tracer.Assert(array != null);
							result2.Type = GetDataType(DataAggregate.GetTypeCode(array[0]));
						}
						else
						{
							result2.ErrorOccurred = true;
							RegisterInvalidExpressionDataTypeWarning();
						}
						break;
					}
				}
			}
			return result2;
		}

		private DataType GetDataType(DataAggregate.DataTypeCode typecode)
		{
			switch (typecode)
			{
			case DataAggregate.DataTypeCode.Boolean:
				return DataType.Boolean;
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.Int64:
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.UInt64:
			case DataAggregate.DataTypeCode.Byte:
			case DataAggregate.DataTypeCode.SByte:
			case DataAggregate.DataTypeCode.TimeSpan:
				return DataType.Integer;
			case DataAggregate.DataTypeCode.Single:
			case DataAggregate.DataTypeCode.Double:
			case DataAggregate.DataTypeCode.Decimal:
				return DataType.Float;
			case DataAggregate.DataTypeCode.Null:
			case DataAggregate.DataTypeCode.String:
			case DataAggregate.DataTypeCode.Char:
				return DataType.String;
			case DataAggregate.DataTypeCode.DateTime:
				return DataType.DateTime;
			default:
				return DataType.String;
			}
		}

		private bool EvaluateNoConstantExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Constant)
			{
				result = new VariantResult(errorOccurred: true, null);
				RegisterInvalidExpressionDataTypeWarning();
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		internal static bool IsVariant(object o)
		{
			if (!(o is string) && !(o is int) && !(o is decimal) && !(o is DateTime) && !(o is double) && !(o is float) && !(o is short) && !(o is bool) && !(o is byte) && !(o is TimeSpan) && !(o is sbyte) && !(o is long) && !(o is ushort) && !(o is uint) && !(o is ulong))
			{
				return o is char;
			}
			return true;
		}

		private void RegisterInvalidExpressionDataTypeWarning()
		{
			((IErrorContext)this).Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning, Array.Empty<string>());
		}

		internal bool InScope(string scope)
		{
			if (m_currentScope == null)
			{
				return false;
			}
			return m_currentScope.InScope(scope);
		}

		internal int RecursiveLevel(string scope)
		{
			if (m_currentScope == null)
			{
				return 0;
			}
			int num = m_currentScope.RecursiveLevel(scope);
			if (-1 == num)
			{
				return 0;
			}
			return num;
		}

		internal void LoadCompiledCode(Report report, bool parametersOnly, ObjectModelImpl reportObjectModel, ReportRuntimeSetup runtimeSetup)
		{
			Global.Tracer.Assert(report.CompiledCode != null && m_exprHostAssembly == null && m_reportExprHost == null);
			if (report.CompiledCode.Length == 0)
			{
				return;
			}
			ProcessingErrorCode errorCode = ProcessingErrorCode.rsErrorLoadingExprHostAssembly;
			try
			{
				if (runtimeSetup.RequireExpressionHostWithRefusedPermissions && !report.CompiledCodeGeneratedWithRefusedPermissions)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace("Expression host generated with refused permissions is required.");
					}
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (runtimeSetup.AssemblyLoadContext == null)
				{
					if (report.CodeModules != null)
					{
						for (int i = 0; i < report.CodeModules.Count; i++)
						{
							if (!runtimeSetup.CheckCodeModuleIsTrustedInCurrentAppDomain(report.CodeModules[i]))
							{
								m_errorContext.Register(ProcessingErrorCode.rsUntrustedCodeModule, Severity.Error, ObjectType.Report, null, null, report.CodeModules[i]);
								throw new ReportProcessingException(m_errorContext.Messages);
							}
						}
					}
					m_reportExprHost = ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(report.CompiledCode, report.ExprHostAssemblyName, runtimeSetup.ExprHostEvidence, parametersOnly, reportObjectModel, report.CodeModules, runtimeSetup.AssemblyLoadContext);
				}
				else
				{
					m_reportExprHost = ExpressionHostLoader.LoadExprHost(report.CompiledCode, report.ExprHostAssemblyName, parametersOnly, reportObjectModel, report.CodeModules, runtimeSetup.AssemblyLoadContext);
				}
				errorCode = ProcessingErrorCode.rsErrorInOnInit;
				m_reportExprHost.CustomCodeOnInit();
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception e)
			{
				ProcessLoadingExprHostException(e, errorCode);
			}
		}

		private void ProcessLoadingExprHostException(Exception e, ProcessingErrorCode errorCode)
		{
			if (e != null && e is TargetInvocationException && e.InnerException != null)
			{
				e = e.InnerException;
			}
			string str = null;
			string text;
			if (e != null)
			{
				try
				{
					text = e.Message;
					str = e.ToString();
				}
				catch
				{
					text = RPRes.NonClsCompliantException;
				}
			}
			else
			{
				text = RPRes.NonClsCompliantException;
			}
			ProcessingMessage processingMessage = m_errorContext.Register(errorCode, Severity.Error, ObjectType.Report, null, null, text);
			if (Global.Tracer.TraceError && processingMessage != null)
			{
				Global.Tracer.Trace(TraceLevel.Error, processingMessage.Message + Environment.NewLine + str);
			}
			throw new ReportProcessingException(m_errorContext.Messages);
		}

		internal void Close()
		{
			m_reportExprHost = null;
		}
	}
}
