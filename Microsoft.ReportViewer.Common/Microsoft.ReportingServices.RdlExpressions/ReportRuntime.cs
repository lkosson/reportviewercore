using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal sealed class ReportRuntime : IErrorContext, IStaticReferenceable
	{
		private delegate object EvalulateDataPoint(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint);

		internal sealed class TextRunExprHostWrapper : TextRunExprHost
		{
			private TextBoxExprHost m_textBoxExprHost;

			public override object ValueExpr => m_textBoxExprHost.ValueExpr;

			internal TextRunExprHostWrapper(TextBoxExprHost textBoxExprHost)
			{
				m_textBoxExprHost = textBoxExprHost;
			}
		}

		private enum NormalizationCode
		{
			Success,
			InvalidType,
			StringLengthViolation,
			ArrayLengthViolation
		}

		private sealed class ExpressionHostLoader : MarshalByRefObject
		{
			private static readonly Hashtable ExpressionHosts = new Hashtable();

			private const string ExprHostRootType = "ReportExprHostImpl";

			private static readonly byte[] ReportExpressionsDefaultEvidencePK = new byte[160]
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

			internal static ReportExprHost LoadExprHost(byte[] exprHostBytes, string exprHostAssemblyName, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				Type expressionHostLoaderType = typeof(ExpressionHostLoader);
				ExpressionHostLoader remoteEHL = new ExpressionHostLoader();
				return remoteEHL.LoadExprHostRemoteEntryPoint(exprHostBytes, exprHostAssemblyName, includeParameters, parametersOnly, objectModel, codeModules, assemblyLoadContext);
			}

			internal static ReportExprHost LoadExprHostIntoCurrentAppDomain(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AssemblyLoadContext assemblyLoadContext)
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
				return (ReportExprHost)LoadExprHostAssembly(exprHostBytes, exprHostAssemblyName, evidence, assemblyLoadContext).GetType("ReportExprHostImpl").GetConstructors()[0].Invoke(new object[3]
				{
					includeParameters,
					parametersOnly,
					objectModel
				});
			}

			private static Assembly LoadExprHostAssembly(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, AssemblyLoadContext assemblyLoadContext)
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
						if (assemblyLoadContext == null)
						{
							assembly = Assembly.Load(exprHostBytes);
							ExpressionHosts.Add(exprHostAssemblyName, assembly);
						}
						else
						{
							assembly = assemblyLoadContext.LoadFromStream(new MemoryStream(exprHostBytes));
						}
					}
					return assembly;
				}
			}

			private static Evidence CreateDefaultExpressionHostEvidence(string exprHostAssemblyName)
			{
				Evidence evidence = new Evidence();
				return evidence;
			}

			private ReportExprHost LoadExprHostRemoteEntryPoint(byte[] exprHostBytes, string exprHostAssemblyName, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AssemblyLoadContext assemblyLoadContext)
			{
				return LoadExprHostIntoCurrentAppDomain(exprHostBytes, exprHostAssemblyName, null, includeParameters, parametersOnly, objectModel, codeModules, assemblyLoadContext);
			}
		}

		private bool m_exprHostInSandboxAppDomain;

		private ReportExprHost m_reportExprHost;

		private Microsoft.ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl m_reportObjectModel;

		private ErrorContext m_errorContext;

		private IErrorContext m_delayedErrorContext;

		private bool m_contextUpdated;

		private IScope m_currentScope;

		private bool m_variableReferenceMode;

		private bool m_unfulfilledDependency;

		private ReportRuntime m_topLevelReportRuntime;

		private List<string> m_fieldsUsedInCurrentActionOwnerValue;

		private int m_id = int.MinValue;

		private int m_maxStringResultLength = -1;

		private int m_maxArrayResultLength = -1;

		private bool m_rdlSandboxingEnabled;

		private bool m_isSerializableValuesProhibited;

		private const int UnrestrictedStringResultLength = -1;

		private const int UnrestrictedArrayResultLength = -1;

		internal ReportExprHost ReportExprHost => m_reportExprHost;

		internal bool VariableReferenceMode
		{
			get
			{
				return m_variableReferenceMode;
			}
			set
			{
				m_variableReferenceMode = value;
			}
		}

		internal bool UnfulfilledDependency
		{
			get
			{
				return m_unfulfilledDependency;
			}
			set
			{
				m_unfulfilledDependency = value;
			}
		}

		internal bool ContextUpdated
		{
			get
			{
				return m_contextUpdated;
			}
			set
			{
				m_contextUpdated = value;
			}
		}

		internal IScope CurrentScope
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

		internal Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl ReportObjectModel => m_reportObjectModel;

		internal ErrorContext RuntimeErrorContext => m_errorContext;

		internal List<string> FieldsUsedInCurrentActionOwnerValue
		{
			get
			{
				return m_fieldsUsedInCurrentActionOwnerValue;
			}
			set
			{
				m_fieldsUsedInCurrentActionOwnerValue = value;
			}
		}

		public int ID => m_id;

		internal ReportRuntime(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ErrorContext errorContext)
		{
			m_objectType = objectType;
			m_reportObjectModel = reportObjectModel;
			m_errorContext = errorContext;
			if (reportObjectModel.OdpContext.IsRdlSandboxingEnabled())
			{
				m_rdlSandboxingEnabled = true;
				IRdlSandboxConfig rdlSandboxing = reportObjectModel.OdpContext.Configuration.RdlSandboxing;
				m_maxStringResultLength = rdlSandboxing.MaxStringResultLength;
				m_maxArrayResultLength = rdlSandboxing.MaxArrayResultLength;
			}
			if (reportObjectModel.OdpContext.Configuration != null)
			{
				m_isSerializableValuesProhibited = reportObjectModel.OdpContext.Configuration.ProhibitSerializableValues;
			}
		}

		internal ReportRuntime(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ErrorContext errorContext, ReportExprHost copyReportExprHost, ReportRuntime topLevelReportRuntime)
			: this(objectType, reportObjectModel, errorContext)
		{
			m_reportExprHost = copyReportExprHost;
			m_topLevelReportRuntime = topLevelReportRuntime;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (m_delayedErrorContext == null)
			{
				m_errorContext.Register(code, severity, m_objectType, m_objectName, m_propertyName, arguments);
			}
			else
			{
				m_delayedErrorContext.Register(code, severity, arguments);
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (m_delayedErrorContext == null)
			{
				m_errorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
			}
			else
			{
				m_delayedErrorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
			}
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

		internal string EvaluateReportLanguageExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, out CultureInfo language)
		{
			if (!EvaluateSimpleExpression(report.Language, report.ObjectType, report.Name, "Language", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.Language, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.ReportLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSpecificLanguage(ProcessStringResult(result).Value, this, out language);
		}

		internal int EvaluateReportAutoRefreshExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (!EvaluateSimpleExpression(report.AutoRefreshExpression, report.ObjectType, report.Name, "AutoRefresh", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.AutoRefreshExpression, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.AutoRefreshExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateInitialPageNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (!EvaluateSimpleExpression(report.InitialPageName, report.ObjectType, report.Name, "InitialPageName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(report.InitialPageName, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.InitialPageNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result, autocast: true).Value;
		}

		internal string EvaluateParamPrompt(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef)
		{
			if (!EvaluateSimpleExpression(paramDef.PromptExpression, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "Prompt", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramDef.PromptExpression, ref result, paramDef.ExprHost))
					{
						Global.Tracer.Assert(paramDef.PromptExpression != null, "(paramDef.PromptExpression != null)");
						result.Value = paramDef.ExprHost.PromptExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessAutocastStringResult(result).Value;
		}

		internal VariantResult EvaluateParamDefaultValue(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.DefaultExpressions != null, "(paramDef.DefaultExpressions != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.DefaultExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "DefaultValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && expressionInfo.ExprHostID >= 0, "(paramDef.ExprHost != null && defaultExpression.ExprHostID >= 0)");
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
				}
			}
			ProcessReportParameterVariantResult(expressionInfo, ref result);
			return result;
		}

		internal VariantResult EvaluateParamValidValue(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesValueExpressions != null, "(paramDef.ValidValuesValueExpressions != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.ValidValuesValueExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "ValidValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost.ValidValuesHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValuesHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValuesHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
				}
			}
			ProcessReportParameterVariantResult(expressionInfo, ref result);
			return result;
		}

		internal VariantResult EvaluateParamValidValueLabel(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesLabelExpressions != null, "(paramDef.ValidValuesLabelExpressions != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.ValidValuesLabelExpressions[index];
			if (!EvaluateSimpleExpression(expressionInfo, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost.ValidValueLabelsHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValueLabelsHost != null && expressionInfo.ExprHostID >= 0);
						m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValueLabelsHost[expressionInfo.ExprHostID];
						m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, this, isError: true);
				}
			}
			ProcessReportParameterVariantResult(expressionInfo, ref result);
			if (!result.ErrorOccurred && (result.Value is object[] || result.Value is IList))
			{
				object[] asObjectArray = GetAsObjectArray(ref result);
				try
				{
					VariantResult result2 = default(VariantResult);
					for (int i = 0; i < asObjectArray.Length; i++)
					{
						result2.Value = asObjectArray[i];
						ProcessLabelResult(ref result2);
						if (result2.ErrorOccurred)
						{
							result.ErrorOccurred = true;
							return result;
						}
						asObjectArray[i] = result2.Value;
					}
					result.Value = asObjectArray;
					return result;
				}
				catch (Exception)
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

		internal object EvaluateDataValueValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.DataValue value, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out TypeCode typeCode)
		{
			if (!EvaluateSimpleExpression(value.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value.Value, ref result, value.ExprHost))
					{
						Global.Tracer.Assert(value.ExprHost != null, "(value.ExprHost != null)");
						result.Value = value.ExprHost.DataValueValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value.Value, ref result);
			typeCode = result.TypeCode;
			return result.Value;
		}

		internal string EvaluateDataValueNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.DataValue value, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(value.Name, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value.Name, ref result, value.ExprHost))
					{
						Global.Tracer.Assert(value.ExprHost != null, "(value.ExprHost != null)");
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

		internal VariantResult EvaluateFilterVariantExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(filter.Expression, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null, "(filter.ExprHost != null)");
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

		internal StringResult EvaluateFilterStringExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(filter.Expression, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null, "(filter.ExprHost != null)");
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

		internal VariantResult EvaluateFilterVariantValue(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, int index, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateSimpleExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value, ref result, isArrayAllowed: true);
			return result;
		}

		internal FloatResult EvaluateFilterIntegerOrFloatValue(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, int index, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateIntegerOrFloatExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result);
		}

		internal IntegerResult EvaluateFilterIntegerValue(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, int index, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateIntegerExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result);
		}

		internal StringResult EvaluateFilterStringValue(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, int index, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			if (!EvaluateSimpleExpression(value, objectType, objectName, "FilterValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = filter.ExprHost[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal object EvaluateQueryParamValue(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo paramValue, IndexedExprHost queryParamsExprHost, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(paramValue, objectType, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramValue, ref result, queryParamsExprHost))
					{
						using (m_reportObjectModel.UserImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(queryParamsExprHost != null && paramValue.ExprHostID >= 0, "(queryParamsExprHost != null && paramValue.ExprHostID >= 0)");
							result.Value = queryParamsExprHost[paramValue.ExprHostID];
						}
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

		internal StringResult EvaluateConnectString(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource)
		{
			if (!EvaluateSimpleExpression(dataSource.ConnectStringExpression, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSource, dataSource.Name, "ConnectString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataSource.ConnectStringExpression, ref result, dataSource.ExprHost))
					{
						using (m_reportObjectModel.UserImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(dataSource.ExprHost != null, "(dataSource.ExprHost != null)");
							result.Value = dataSource.ExprHost.ConnectStringExpr;
						}
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

		internal StringResult EvaluateCommandText(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			if (!EvaluateSimpleExpression(dataSet.Query.CommandText, Microsoft.ReportingServices.ReportProcessing.ObjectType.Query, dataSet.Name, "CommandText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataSet.Query.CommandText, ref result, dataSet.ExprHost))
					{
						using (m_reportObjectModel.UserImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(dataSet.ExprHost != null, "(dataSet.ExprHost != null)");
							result.Value = dataSet.ExprHost.QueryCommandTextExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result);
		}

		internal VariantResult EvaluateFieldValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Field field)
		{
			if (!EvaluateSimpleExpression(field.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Field, field.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(field.Value, ref result, field.ExprHost))
					{
						Global.Tracer.Assert(field.ExprHost != null, "(field.ExprHost != null)");
						field.EnsureExprHostReportObjectModelBinding(m_reportObjectModel);
						result.Value = field.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(field.Value, ref result);
			return result;
		}

		internal VariantResult EvaluateAggregateVariantOrBinaryParamExpr(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateInfo, int index, IErrorContext errorContext)
		{
			IErrorContext delayedErrorContext = m_delayedErrorContext;
			try
			{
				m_delayedErrorContext = errorContext;
				Global.Tracer.Assert(aggregateInfo.Expressions != null, "(aggregateInfo.Expressions != null)");
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = aggregateInfo.Expressions[index];
				if (!EvaluateSimpleExpression(expressionInfo, out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expressionInfo, ref result, null))
						{
							Global.Tracer.Assert(aggregateInfo.ExpressionHosts != null && expressionInfo.ExprHostID >= 0, "(aggregateInfo.ExpressionHosts != null && aggParamExpression.ExprHostID >= 0)");
							if (m_exprHostInSandboxAppDomain)
							{
								aggregateInfo.ExpressionHosts[index].SetReportObjectModel(m_reportObjectModel);
							}
							result.Value = aggregateInfo.ExpressionHosts[index].ValueExpr;
						}
					}
					catch (ReportProcessingException_MissingAggregateDependency)
					{
						throw;
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e, errorContext, isError: false);
					}
				}
				ProcessVariantOrBinaryResult(expressionInfo, ref result, isAggregate: true, allowArray: false);
				return result;
			}
			finally
			{
				m_delayedErrorContext = delayedErrorContext;
			}
		}

		internal VariantResult EvaluateLookupDestExpression(LookupDestinationInfo lookupDestInfo, IErrorContext errorContext)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo destinationExpr = lookupDestInfo.DestinationExpr;
			if (!EvaluateSimpleExpression(destinationExpr, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(destinationExpr, ref result, lookupDestInfo.ExprHost))
					{
						Global.Tracer.Assert(lookupDestInfo.ExprHost != null, "(lookupDestInfo.ExprHost != null)");
						result.Value = lookupDestInfo.ExprHost.DestExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e, errorContext, isError: false);
				}
			}
			ProcessLookupVariantResult(destinationExpr, errorContext, isArrayRequired: false, normalizeDBNullToNull: false, ref result);
			return result;
		}

		internal VariantResult EvaluateLookupSourceExpression(LookupInfo lookupInfo)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo sourceExpr = lookupInfo.SourceExpr;
			if (!EvaluateSimpleExpression(sourceExpr, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(sourceExpr, ref result, lookupInfo.ExprHost))
					{
						LookupExprHost exprHost = lookupInfo.ExprHost;
						Global.Tracer.Assert(exprHost != null, "(lookupExprHost != null)");
						exprHost.SetReportObjectModel(m_reportObjectModel);
						result.Value = lookupInfo.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessLookupVariantResult(sourceExpr, this, lookupInfo.LookupType == LookupType.MultiLookup, normalizeDBNullToNull: false, ref result);
			return result;
		}

		internal VariantResult EvaluateLookupResultExpression(LookupInfo lookupInfo)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo resultExpr = lookupInfo.ResultExpr;
			if (!EvaluateSimpleExpression(resultExpr, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(resultExpr, ref result, lookupInfo.ExprHost))
					{
						LookupExprHost exprHost = lookupInfo.ExprHost;
						Global.Tracer.Assert(exprHost != null, "(lookupExprHost != null)");
						exprHost.SetReportObjectModel(m_reportObjectModel);
						result.Value = exprHost.ResultExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessLookupVariantResult(resultExpr, this, isArrayRequired: false, normalizeDBNullToNull: true, ref result);
			return result;
		}

		internal bool EvaluateParamValueOmitExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue paramVal, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateBooleanExpression(paramVal.Omit, canBeConstant: true, objectType, objectName, "ParameterOmit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Omit, ref result, paramVal.ExprHost))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null, "(paramVal.ExprHost != null)");
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

		internal ParameterValueResult EvaluateParameterValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue paramVal, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paramVal.Value, ref result, paramVal.ExprHost))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null, "(paramVal.ExprHost != null)");
						result.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessParameterValueResult(paramVal.Value, paramVal.Name, result);
		}

		internal bool EvaluateStartHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility, IVisibilityHiddenExprHost hiddenExprHostRI, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateBooleanExpression(visibility.Hidden, canBeConstant: true, objectType, objectName, "Hidden", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(visibility.Hidden, ref result, (ReportObjectModelProxy)hiddenExprHostRI))
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
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal bool EvaluateStartHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility, IndexedExprHost hiddenExprHostIdx, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateBooleanExpression(visibility.Hidden, canBeConstant: true, objectType, objectName, "Hidden", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(visibility.Hidden, ref result, hiddenExprHostIdx))
						{
							Global.Tracer.Assert(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0, "(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0)");
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
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemDocumentMapLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			UserProfileState newLocation = m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(reportItem.DocumentMapLabel, reportItem.ObjectType, reportItem.Name, "Label", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(reportItem.DocumentMapLabel, ref result, reportItem.ExprHost))
						{
							Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
							result.Value = reportItem.ExprHost.LabelExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessAutocastStringResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(newLocation);
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemBookmarkExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(reportItem.Bookmark, reportItem.ObjectType, reportItem.Name, "Bookmark", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(reportItem.Bookmark, ref result, reportItem.ExprHost))
						{
							Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
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
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			if (!EvaluateSimpleExpression(reportItem.ToolTip, reportItem.ObjectType, reportItem.Name, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(reportItem.ToolTip, ref result, reportItem.ExprHost))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
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

		internal string EvaluateActionLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
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

		internal string EvaluateReportItemHyperlinkURLExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Hyperlink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
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

		internal string EvaluateReportItemDrillthroughReportName(Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "DrillthroughReportName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
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

		internal string EvaluateReportItemBookmarkLinkExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BookmarkLink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
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

		internal string EvaluateImageStringValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Image image, out bool errorOccurred)
		{
			errorOccurred = false;
			if (!EvaluateSimpleExpression(image.Value, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.Value, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
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

		internal VariantResult EvaluateImageTagExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Image image, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo tag)
		{
			if (!EvaluateSimpleExpression(tag, image.ObjectType, image.Name, "Tag", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tag, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						Global.Tracer.Assert(image.Tags.Count == 1, "Only a single Tag expression host is allowed from old snapshots");
						result.Value = image.ImageExprHost.TagExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(tag, ref result);
			return result;
		}

		internal byte[] EvaluateImageBinaryValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Image image, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(image.Value, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.Value, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
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

		internal string EvaluateImageMIMETypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Image image)
		{
			if (!EvaluateSimpleExpression(image.MIMEType, image.ObjectType, image.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(image.MIMEType, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
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

		internal bool EvaluateTextBoxInitialToggleStateExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textBox)
		{
			if (!EvaluateBooleanExpression(textBox.InitialToggleState, canBeConstant: true, textBox.ObjectType, textBox.Name, "InitialState", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textBox.InitialToggleState, ref result, textBox.ExprHost))
					{
						Global.Tracer.Assert(textBox.ExprHost != null, "(textBox.ExprHost != null)");
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

		internal object EvaluateUserSortExpression(IInScopeEventSource eventSource)
		{
			int sortExpressionIndex = eventSource.UserSort.SortExpressionIndex;
			Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
			Global.Tracer.Assert(sortTarget.UserSortExpressions != null && 0 <= sortExpressionIndex && sortExpressionIndex < sortTarget.UserSortExpressions.Count);
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = sortTarget.UserSortExpressions[sortExpressionIndex];
			if (!EvaluateSimpleExpression(expressionInfo, eventSource.ObjectType, eventSource.Name, "SortExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, sortTarget.UserSortExpressionsHost))
					{
						Global.Tracer.Assert(sortTarget.UserSortExpressionsHost != null, "(sortTarget.UserSortExpressionsHost != null)");
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

		internal string EvaluateGroupingLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			UserProfileState newLocation = m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
			try
			{
				if (!EvaluateSimpleExpression(grouping.GroupLabel, objectType, objectName, "Label", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(grouping.GroupLabel, ref result, grouping.ExprHost))
						{
							Global.Tracer.Assert(grouping.ExprHost != null, "(grouping.ExprHost != null)");
							result.Value = grouping.ExprHost.LabelExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessAutocastStringResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(newLocation);
			}
		}

		internal string EvaluateGroupingPageNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, grouping.ExprHost))
						{
							Global.Tracer.Assert(grouping.ExprHost != null, "(grouping.ExprHost != null)");
							result.Value = grouping.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal object EvaluateRuntimeExpression(RuntimeExpressionInfo runtimeExpression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(runtimeExpression.Expression, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(runtimeExpression.Expression, ref result, runtimeExpression.ExpressionsHost))
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
			VerifyVariantResultAndStopOnError(ref result);
			return result.Value;
		}

		internal VariantResult EvaluateVariableValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Variable variable, IndexedExprHost variableValuesHost, Microsoft.ReportingServices.ReportProcessing.ObjectType parentObjectType, string parentObjectName, bool isReportScope)
		{
			if (!EvaluateSimpleExpression(variable.Value, parentObjectType, parentObjectName, variable.GetPropertyName(), out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(variable.Value, ref result, variableValuesHost))
					{
						Global.Tracer.Assert(variableValuesHost != null && variable.Value.ExprHostID >= 0);
						result.Value = variableValuesHost[variable.Value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			ProcessSerializableResult(variable.Value, isReportScope, ref result);
			return result;
		}

		internal string EvaluateSubReportNoRowsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(subReport.NoRowsMessage, Microsoft.ReportingServices.ReportProcessing.ObjectType.Subreport, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(subReport.NoRowsMessage, ref result, subReport.SubReportExprHost))
					{
						Global.Tracer.Assert(subReport.SubReportExprHost != null, "(subReport.SubReportExprHost != null)");
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

		internal string EvaluateDataRegionNoRowsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion region, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(region.NoRowsMessage, objectType, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(region.NoRowsMessage, ref result, region.ExprHost))
					{
						Global.Tracer.Assert(region.ExprHost != null, "(region.ExprHost != null)");
						result.Value = region.EvaluateNoRowsMessageExpression();
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateDataRegionPageNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, objectType, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, dataRegion.ExprHost))
						{
							Global.Tracer.Assert(dataRegion.ExprHost != null, "(dataRegion.ExprHost != null)");
							result.Value = dataRegion.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateTablixMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition marginPosition)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression = null;
			switch (marginPosition)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin:
				expression = tablix.TopMargin;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin:
				expression = tablix.BottomMargin;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin:
				expression = tablix.LeftMargin;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin:
				expression = tablix.RightMargin;
				break;
			}
			if (!EvaluateSimpleExpression(expression, tablix.ObjectType, tablix.Name, marginPosition.ToString(), out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, tablix.ExprHost))
					{
						Global.Tracer.Assert(tablix.TablixExprHost != null, "(tablix.TablixExprHost != null)");
						switch (marginPosition)
						{
						case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin:
							result.Value = tablix.TablixExprHost.TopMarginExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin:
							result.Value = tablix.TablixExprHost.BottomMarginExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin:
							result.Value = tablix.TablixExprHost.LeftMarginExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin:
							result.Value = tablix.TablixExprHost.RightMarginExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartDynamicSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expr, string propertyName, bool isDynamicWidth)
		{
			if (!EvaluateSimpleExpression(expr, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expr, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "(chart.ExprHost != null)");
						if (isDynamicWidth)
						{
							result.Value = chart.ChartExprHost.DynamicWidthExpr;
						}
						else
						{
							result.Value = chart.ChartExprHost.DynamicHeightExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal VariantResult EvaluateChartDynamicMemberLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember chartMember, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DocumentMapLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, chartMember.ExprHost))
					{
						Global.Tracer.Assert(chartMember.ExprHost != null, "(chartMember.ExprHost != null)");
						result.Value = chartMember.ExprHost.MemberLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(expression, ref result);
			return result;
		}

		internal string EvaluateChartPaletteExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, string objectName)
		{
			if (!EvaluateSimpleExpression(chart.Palette, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Palette", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chart.Palette, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "(chart.ExprHost != null)");
						result.Value = chart.ChartExprHost.PaletteExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartPaletteHatchBehaviorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, string objectName)
		{
			if (!EvaluateSimpleExpression(chart.PaletteHatchBehavior, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PaletteHatchBehavior", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chart.PaletteHatchBehavior, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "chart.ExprHost != null");
						result.Value = chart.ChartExprHost.PaletteHatchBehaviorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartTitleCaptionExpression(ChartTitleBase title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Caption, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = title.ExprHost.CaptionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(title.Caption, ref result);
			return result;
		}

		internal bool EvaluateEvaluateChartTitleHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Hidden, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartTitleDockingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Position, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Docking, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitlePositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitlePositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartTitleDockOutsideChartAreaExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.DockOutsideChartArea, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockOutsideChartAreaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal int EvaluateChartTitleDockOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.DockOffset, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockingOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartTitleToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.ToolTip, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitleTextOrientationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle chartTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTitle.TextOrientation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTitle.TextOrientation, ref result, chartTitle.ExprHost))
					{
						Global.Tracer.Assert(chartTitle.ExprHost != null, "(chartTitle.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)chartTitle.ExprHost).TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisTitlePositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle title, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(title.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartAxisTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisTitleTextOrientationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisTitle.TextOrientation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisTitle.TextOrientation, ref result, chartAxisTitle.ExprHost))
					{
						Global.Tracer.Assert(chartAxisTitle.ExprHost != null, "(chartAxisTitle.ExprHost != null)");
						result.Value = ((ChartAxisTitleExprHost)chartAxisTitle.ExprHost).TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendTitleTitleSeparatorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendTitle.TitleSeparator, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartTitleSeparator", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendTitle.TitleSeparator, ref result, chartLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(chartLegendTitle.ExprHost != null, "(chartLegendTitle.ExprHost != null)");
						result.Value = ((ChartLegendTitleExprHost)chartLegendTitle.ExprHost).TitleSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartDataLabelLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Label, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Label, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartDataLabel.Label, ref result);
			return result;
		}

		internal string EvaluateChartDataLabePositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Position, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Position, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.ChartDataLabelPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartDataLabelRotationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Rotation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Rotation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Rotation, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.RotationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartDataLabelUseValueAsLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.UseValueAsLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UseValueAsLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.UseValueAsLabel, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.UseValueAsLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartDataLabelVisibleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.Visible, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Visible", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataLabel.Visible, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.VisibleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateChartDataLabelToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataLabel.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartDataLabel.ToolTip, ref result, chartDataLabel.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
					result.Value = chartDataLabel.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.X, objectName, "X", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesXExpr);
		}

		internal string EvaluateChartTickMarksEnabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Enabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Enabled, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTickMarksTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Type, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Type, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartTickMarksLengthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Length, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Length", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Length, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.LengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateChartTickMarksIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.Interval, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartTickMarksIntervalTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.IntervalType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.IntervalType, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartTickMarksIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.IntervalOffset, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartTickMarksIntervalOffsetTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(chartTickMarks.IntervalOffsetType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartTickMarks.IntervalOffsetType, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartItemInLegendLegendTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartItemInLegend.LegendText, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartItemInLegend.LegendText, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						result.Value = chartItemInLegend.ExprHost.LegendTextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartItemInLegendToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartItemInLegend.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartItemInLegend.ToolTip, ref result, chartItemInLegend.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
					result.Value = chartItemInLegend.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal bool EvaluateChartItemInLegendHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartItemInLegend.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartItemInLegend.Hidden, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						if (chartItemInLegend.ExprHost != null)
						{
							result.Value = chartItemInLegend.ExprHost.HiddenExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateChartEmptyPointsAxisLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints, string objectName)
		{
			if (!EvaluateSimpleExpression(chartEmptyPoints.AxisLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AxisLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartEmptyPoints.AxisLabel, ref result, chartEmptyPoints.ExprHost))
					{
						Global.Tracer.Assert(chartEmptyPoints.ExprHost != null, "(chartEmptyPoints.ExprHost != null)");
						result.Value = chartEmptyPoints.ExprHost.AxisLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartEmptyPoints.AxisLabel, ref result);
			return result;
		}

		internal VariantResult EvaluateChartEmptyPointsToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints, string objectName)
		{
			if (!EvaluateSimpleExpression(chartEmptyPoints.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartEmptyPoints.ToolTip, ref result, chartEmptyPoints.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartEmptyPoints.ExprHost != null, "(chartEmptyPoints.ExprHost != null)");
					result.Value = chartEmptyPoints.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartFormulaParameterValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter, string objectName)
		{
			if (!EvaluateSimpleExpression(chartFormulaParameter.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartFormulaParameter.Value, ref result, chartFormulaParameter.ExprHost))
					{
						Global.Tracer.Assert(chartFormulaParameter.ExprHost != null, "(chartFormulaParameter.ExprHost != null)");
						result.Value = chartFormulaParameter.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartFormulaParameter.Value, ref result);
			return result;
		}

		internal double EvaluateChartElementPositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string propertyName, ChartElementPositionExprHost exprHost, Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position position, string objectName)
		{
			if (!EvaluateSimpleExpression(expressionInfo, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expressionInfo, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						switch (position)
						{
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Top:
							result.Value = exprHost.TopExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Left:
							result.Value = exprHost.LeftExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Height:
							result.Value = exprHost.HeightExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Width:
							result.Value = exprHost.WidthExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartSmartLabelAllowOutSidePlotAreaExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.AllowOutSidePlotArea, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AllowOutSidePlotArea", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.AllowOutSidePlotArea, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.AllowOutSidePlotAreaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutBackColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutBackColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutBackColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutBackColor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutBackColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartSmartLabelCalloutLineAnchorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineAnchor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineAnchor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineAnchor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineAnchorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutLineColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineColor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartSmartLabelCalloutLineStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineStyle, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutLineWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutLineWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutLineWidth, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartSmartLabelCalloutStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.CalloutStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.CalloutStyle, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelShowOverlappedExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.ShowOverlapped, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ShowOverlapped", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.ShowOverlapped, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.ShowOverlappedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelMarkerOverlappingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.MarkerOverlapping, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MarkerOverlapping", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.MarkerOverlapping, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MarkerOverlappingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelDisabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.Disabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Disabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.Disabled, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.DisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartSmartLabelMaxMovingDistanceExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.MaxMovingDistance, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxMovingDistance", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.MaxMovingDistance, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MaxMovingDistanceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartSmartLabelMinMovingDistanceExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSmartLabel.MinMovingDistance, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinMovingDistance", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSmartLabel.MinMovingDistance, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MinMovingDistanceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateChartLegendHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Hidden, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendPositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Position, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Position, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ChartLegendPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendLayoutExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Layout, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Layout, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.LayoutExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartLegendDockOutsideChartAreaExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.DockOutsideChartArea, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.DockOutsideChartArea, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.DockOutsideChartAreaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartLegendAutoFitTextDisabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.AutoFitTextDisabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.AutoFitTextDisabled, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.AutoFitTextDisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendMinFontSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.MinFontSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.MinFontSize, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartLegendHeaderSeparatorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.HeaderSeparator, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.HeaderSeparator, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HeaderSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendHeaderSeparatorColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.HeaderSeparatorColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.HeaderSeparatorColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HeaderSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartLegendColumnSeparatorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.ColumnSeparator, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.ColumnSeparator, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnSeparatorColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.ColumnSeparatorColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.ColumnSeparatorColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal int EvaluateChartLegendColumnSpacingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.ColumnSpacing, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.ColumnSpacing, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSpacingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartLegendInterlacedRowsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.InterlacedRows, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.InterlacedRows, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.InterlacedRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendInterlacedRowsColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.InterlacedRowsColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.InterlacedRowsColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.InterlacedRowsColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateChartLegendEquallySpacedItemsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.EquallySpacedItems, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.EquallySpacedItems, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.EquallySpacedItemsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendReversedExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegend.Reversed, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Reversed", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.Reversed, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ReversedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendMaxAutoSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.MaxAutoSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.MaxAutoSize, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.MaxAutoSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendTextWrapThresholdExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartLegend.TextWrapThreshold, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegend.TextWrapThreshold, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.TextWrapThresholdExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendColumnHeaderValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeader, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumnHeader.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumnHeader.Value, ref result, chartLegendColumnHeader.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumnHeader.ExprHost != null, "(chartLegendColumnHeader.ExprHost != null)");
						result.Value = chartLegendColumnHeader.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnColumnTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.ColumnType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ColumnType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.ColumnType, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ColumnTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.Value, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.ToolTip, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnMinimumWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.MinimumWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinimumWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.MinimumWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.MinimumWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartLegendColumnMaximumWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.MaximumWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaximumWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.MaximumWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.MaximumWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal int EvaluateChartLegendColumnSeriesSymbolWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.SeriesSymbolWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeriesSymbolWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.SeriesSymbolWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.SeriesSymbolWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendColumnSeriesSymbolHeightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendColumn.SeriesSymbolHeight, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeriesSymbolHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendColumn.SeriesSymbolHeight, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.SeriesSymbolHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellCellTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.CellType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CellType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.CellType, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.CellTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.Text, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.Text, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellCellSpanExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.CellSpan, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CellSpan", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.CellSpan, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.CellSpanExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.ToolTip, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellImageWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.ImageWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ImageWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.ImageWidth, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ImageWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellImageHeightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.ImageHeight, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ImageHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.ImageHeight, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ImageHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellSymbolHeightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.SymbolHeight, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SymbolHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.SymbolHeight, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.SymbolHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellSymbolWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.SymbolWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SymbolWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.SymbolWidth, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.SymbolWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellAlignmentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.Alignment, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Alignment", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.Alignment, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.AlignmentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellTopMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.TopMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TopMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.TopMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.TopMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellBottomMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.BottomMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "BottomMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.BottomMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.BottomMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellLeftMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.LeftMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LeftMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.LeftMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.LeftMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellRightMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItemCell.RightMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "RightMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItemCell.RightMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.RightMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Up, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Up", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Up, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Down, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Down", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Down, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsLeftExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Left, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Left", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Left, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsRightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.Right, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Right", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.Right, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.RightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpLeftExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.UpLeft, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UpLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.UpLeft, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpRightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.UpRight, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UpRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.UpRight, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownLeftExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.DownLeft, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DownLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.DownLeft, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownRightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			if (!EvaluateSimpleExpression(chartNoMoveDirections.DownRight, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DownRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartNoMoveDirections.DownRight, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartStripLineTitleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.Title, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Title", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.Title, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TitleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartStripLineTitleAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.TitleAngle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TitleAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.TitleAngle, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TitleAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartStripLineTextOrientationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.TextOrientation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.TextOrientation, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartStripLineToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.ToolTip, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.Interval, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineIntervalTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.IntervalType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.IntervalType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.IntervalOffset, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineIntervalOffsetTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.IntervalOffsetType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.IntervalOffsetType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineStripWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.StripWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "StripWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.StripWidth, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.StripWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineStripWidthTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			if (!EvaluateSimpleExpression(chartStripLine.StripWidthType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "StripWidthType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartStripLine.StripWidthType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.StripWidthTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemSeparatorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItem.Separator, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Separator", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItem.Separator, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.SeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemSeparatorColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItem.SeparatorColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeparatorColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItem.SeparatorColor, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.SeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateChartLegendCustomItemToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			if (!EvaluateSimpleExpression(chartLegendCustomItem.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartLegendCustomItem.ToolTip, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.Type, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.Type, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesSubtypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.Subtype, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Subtype", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.Subtype, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.SubtypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesLegendNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.LegendName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.LegendName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.LegendNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartSeriesLegendTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.LegendText, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendText", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartSeries.LegendText, ref result, chartSeries.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
					result.Value = chartSeries.ExprHost.LegendTextExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartSeriesChartAreaNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.ChartAreaName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartAreaName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.ChartAreaName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ChartAreaNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesValueAxisNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.ValueAxisName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ValueAxisName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.ValueAxisName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ValueAxisNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartSeriesToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartSeries.ToolTip, ref result, chartSeries.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
					result.Value = chartSeries.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartSeriesCategoryAxisNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.CategoryAxisName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CategoryAxisName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.CategoryAxisName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.CategoryAxisNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartSeriesHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.Hidden, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSeriesHideInLegendExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			if (!EvaluateSimpleExpression(chartSeries.HideInLegend, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideInLegend", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartSeries.HideInLegend, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.HideInLegendExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartBorderSkinBorderSkinTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkin, string objectName)
		{
			if (!EvaluateSimpleExpression(chartBorderSkin.BorderSkinType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartBorderSkinType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartBorderSkin.BorderSkinType, ref result, chartBorderSkin.ExprHost))
					{
						Global.Tracer.Assert(chartBorderSkin.ExprHost != null, "(chartBorderSkin.ExprHost != null)");
						result.Value = chartBorderSkin.ExprHost.BorderSkinTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisScaleBreakEnabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.Enabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.Enabled, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisScaleBreakBreakLineTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.BreakLineType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "BreakLineType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.BreakLineType, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.BreakLineTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result, autocast: true).Value;
		}

		internal int EvaluateChartAxisScaleBreakCollapsibleSpaceThresholdExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.CollapsibleSpaceThreshold, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CollapsibleSpaceThreshold", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.CollapsibleSpaceThreshold, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.CollapsibleSpaceThresholdExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartAxisScaleBreakMaxNumberOfBreaksExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.MaxNumberOfBreaks, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxNumberOfBreaks", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.MaxNumberOfBreaks, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.MaxNumberOfBreaksExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal double EvaluateChartAxisScaleBreakSpacingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.Spacing, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Spacing", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.Spacing, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.SpacingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisScaleBreakIncludeZeroExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxisScaleBreak.IncludeZero, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IncludeZero", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxisScaleBreak.IncludeZero, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.IncludeZeroExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartCustomPaletteColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor customPaletteColor, string objectName)
		{
			if (!EvaluateSimpleExpression(customPaletteColor.Color, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customPaletteColor.Color, ref result, customPaletteColor.ExprHost))
					{
						Global.Tracer.Assert(customPaletteColor.ExprHost != null, "(customPaletteColor.ExprHost != null)");
						result.Value = customPaletteColor.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal VariantResult EvaluateChartDataPointAxisLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataPoint.AxisLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AxisLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartDataPoint.AxisLabel, ref result, chartDataPoint.ExprHost))
					{
						Global.Tracer.Assert(chartDataPoint.ExprHost != null, "(chartDataPoint.ExprHost != null)");
						result.Value = chartDataPoint.ExprHost.AxisLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(chartDataPoint.AxisLabel, ref result);
			return result;
		}

		internal VariantResult EvaluateChartDataPointToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(chartDataPoint.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(chartDataPoint.ToolTip, ref result, chartDataPoint.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(chartDataPoint.ExprHost != null, "(chartDataPoint.ExprHost != null)");
					result.Value = chartDataPoint.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Y, objectName, "Y", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesYExpr);
		}

		internal VariantResult EvaluateChartDataPointValueSizesExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Size, objectName, "Size", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesSizeExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(dataPoint.DataPointValues.High, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "High", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(dataPoint.DataPointValues.High, ref result, dataPoint.ExprHost))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null, "(dataPoint.ExprHost != null)");
						result.Value = dataPoint.ExprHost.DataPointValuesHighExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(dataPoint.DataPointValues.High, ref result);
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesLowExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Low, objectName, "Low", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesLowExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesStartExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Start, objectName, "Start", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesStartExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesEndExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.End, objectName, "End", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesEndExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesMeanExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Mean, objectName, "Mean", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesMeanExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesMedianExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Median, objectName, "Median", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesMedianExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightX, objectName, "HighlightX", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightXExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightY, objectName, "HighlightY", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightYExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightSize, objectName, "HighlightSize", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightSizeExpr);
		}

		internal string EvaluateChartDataPointValuesFormatXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatX, objectName, "FormatX", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatXExpr);
		}

		internal string EvaluateChartDataPointValuesFormatYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatY, objectName, "FormatY", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatYExpr);
		}

		internal string EvaluateChartDataPointValuesFormatSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatSize, objectName, "FormatSize", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatSizeExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageX, objectName, "CurrencyLanguageX", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageXExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageY, objectName, "CurrencyLanguageY", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageYExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageSize, objectName, "CurrencyLanguageSize", (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageSizeExpr);
		}

		private VariantResult EvaluateChartDataPointValuesExpressionAsVariant(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = EvaluateChartDataPointValuesExpression(dataPoint, expressionInfo, objectName, propertyName, expressionFunction);
			ProcessVariantResult(expressionInfo, ref result);
			return result;
		}

		private string EvaluateChartDataPointValuesExpressionAsString(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = EvaluateChartDataPointValuesExpression(dataPoint, expressionInfo, objectName, propertyName, expressionFunction);
			return ProcessStringResult(result).Value;
		}

		private VariantResult EvaluateChartDataPointValuesExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			if (!EvaluateSimpleExpression(expressionInfo, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(expressionInfo, ref result, dataPoint.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(dataPoint.ExprHost != null, "(dataPoint.ExprHost != null)");
					result.Value = expressionFunction(dataPoint);
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartMarkerSize(Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker, string objectName)
		{
			if (!EvaluateSimpleExpression(chartMarker.Size, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Size", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartMarker.Size, ref result, chartMarker.ExprHost))
					{
						Global.Tracer.Assert(chartMarker.ExprHost != null, "(chartMarker.ExprHost != null)");
						result.Value = chartMarker.ExprHost.SizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartMarkerType(Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker, string objectName)
		{
			if (!EvaluateSimpleExpression(chartMarker.Type, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartMarker.Type, ref result, chartMarker.ExprHost))
					{
						Global.Tracer.Assert(chartMarker.ExprHost != null, "(chartMarker.ExprHost != null)");
						result.Value = chartMarker.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisVisibleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Visible, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Visible", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Visible, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.VisibleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Margin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Margin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Margin, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Interval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisIntervalTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IntervalType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IntervalType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IntervalOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisIntervalOffsetTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IntervalOffsetType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IntervalOffsetType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisMarksAlwaysAtPlotEdgeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.MarksAlwaysAtPlotEdge, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MarksAlwaysAtPlotEdge", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.MarksAlwaysAtPlotEdge, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MarksAlwaysAtPlotEdgeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisReverseExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Reverse, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Reverse", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Reverse, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.ReverseExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisLocationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Location, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Location", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Location, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LocationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisInterlacedExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Interlaced, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interlaced", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Interlaced, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.InterlacedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisInterlacedColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.InterlacedColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "InterlacedColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.InterlacedColor, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.InterlacedColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateChartAxisLogScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LogScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LogScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LogScale, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LogScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisLogBaseExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LogBase, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LogBase", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LogBase, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LogBaseExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateChartAxisHideLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.HideLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.HideLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.HideLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Angle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Angle, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisArrowsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.Arrows, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Arrows", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.Arrows, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.ArrowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventFontShrinkExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventFontShrink, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventFontShrink", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventFontShrink, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventFontShrinkExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventFontGrowExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventFontGrow, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventFontGrow", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventFontGrow, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventFontGrowExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventLabelOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventLabelOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventLabelOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventLabelOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventLabelOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventWordWrapExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.PreventWordWrap, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventWordWrap", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.PreventWordWrap, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventWordWrapExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisAllowLabelRotationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.AllowLabelRotation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AllowLabelRotation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.AllowLabelRotation, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.AllowLabelRotationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisIncludeZeroExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.IncludeZero, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IncludeZero", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.IncludeZero, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IncludeZeroExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisLabelsAutoFitDisabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelsAutoFitDisabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelsAutoFitDisabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelsAutoFitDisabled, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelsAutoFitDisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisMinFontSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.MinFontSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinFontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.MinFontSize, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartAxisMaxFontSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.MaxFontSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxFontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.MaxFontSize, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MaxFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateChartAxisOffsetLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.OffsetLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "OffsetLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.OffsetLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.OffsetLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisHideEndLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.HideEndLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideEndLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.HideEndLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.HideEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisVariableAutoIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.VariableAutoInterval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "VariableAutoInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.VariableAutoInterval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.VariableAutoIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisLabelIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelInterval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelInterval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisLabelIntervalTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelIntervalType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelIntervalType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisLabelIntervalOffsetsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelIntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelIntervalOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisLabelIntervalOffsetTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			if (!EvaluateSimpleExpression(chartAxis.LabelIntervalOffsetType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalOffsetType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAxis.LabelIntervalOffsetType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal object EvaluateChartAxisValueExpression(ChartAxisExprHost exprHost, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName, string propertyName, Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType type)
		{
			if (!EvaluateSimpleExpression(expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						switch (type)
						{
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.Min:
							result.Value = exprHost.AxisMinExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.Max:
							result.Value = exprHost.AxisMaxExpr;
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.CrossAt:
							result.Value = exprHost.AxisCrossAtExpr;
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

		internal bool EvaluateChartAreaEquallySizedAxesFontExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartArea.EquallySizedAxesFont, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartArea.EquallySizedAxesFont, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.EquallySizedAxesFontExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAreaAlignOrientationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartArea.AlignOrientation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartArea.AlignOrientation, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.AlignOrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAreaHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartArea.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartArea.Hidden, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeAxesViewExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.AxesView, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.AxesView, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.AxesViewExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeCursorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.Cursor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.Cursor, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.CursorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypePositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.Position, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.Position, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.ChartAlignTypePositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeInnerPlotPositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartAlignType.InnerPlotPosition, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartAlignType.InnerPlotPosition, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.InnerPlotPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartGridLinesEnabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.Enabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.Enabled, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartGridLinesIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.Interval, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartGridLinesIntervalTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.IntervalType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.IntervalType, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateChartGridLinesIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.IntervalOffset, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartGridLinesIntervalOffsetTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartGridLines.IntervalOffsetType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartGridLines.IntervalOffsetType, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartThreeDPropertiesEnabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Enabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Enabled, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartThreeDPropertiesProjectionModeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.ProjectionMode, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.ProjectionMode, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ProjectionModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesRotationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Rotation, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Rotation, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.RotationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesInclinationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Inclination, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Inclination, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.InclinationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesPerspectiveExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Perspective, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Perspective, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.PerspectiveExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesDepthRatioExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.DepthRatio, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.DepthRatio, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.DepthRatioExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartThreeDPropertiesShadingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Shading, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Shading, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ShadingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesGapDepthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.GapDepth, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.GapDepth, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.GapDepthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesWallThicknessExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.WallThickness, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.WallThickness, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.WallThicknessExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartThreeDPropertiesClusteredExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			if (!EvaluateSimpleExpression(chartThreeDProperties.Clustered, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(chartThreeDProperties.Clustered, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ClusteredExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateRectanglePageNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle rectangle, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, rectangle.ExprHost))
						{
							Global.Tracer.Assert(rectangle.ExprHost != null, "(rectangle.ExprHost != null)");
							result.Value = rectangle.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateStyleBorderColor(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorLeft(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorRight(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorTop(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorBottom(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal BorderStyles EvaluateStyleBorderStyle(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, (Microsoft.ReportingServices.ReportProcessing.ObjectType.Line != objectType) ? BorderStyles.None : BorderStyles.Solid, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleLeft(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleRight(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleTop(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleBottom(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidth(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthLeft(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthRight(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthTop(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthBottom(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundColor(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBackgroundGradientEndColor(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientEndColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundGradientEndColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal BackgroundGradients EvaluateStyleBackgroundGradientType(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBackgroundGradientType(ProcessStringResult(result).Value, this);
		}

		internal BackgroundRepeatTypes EvaluateStyleBackgroundRepeat(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundRepeat", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundRepeatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBackgroundRepeat(ProcessStringResult(result).Value, this, objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart);
		}

		internal FontStyles EvaluateStyleFontStyle(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateFontStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontFamily(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontFamily", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
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

		internal string EvaluateStyleFontSize(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateFontSize(ProcessStringResult(result).Value, this);
		}

		internal FontWeights EvaluateStyleFontWeight(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "FontWeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontWeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateFontWeight(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFormat(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Format", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
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

		internal TextDecorations EvaluateStyleTextDecoration(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextDecoration", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextDecorationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateTextDecoration(ProcessStringResult(result).Value, this);
		}

		internal TextAlignments EvaluateStyleTextAlign(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextAlign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextAlignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateTextAlign(ProcessStringResult(result).Value, this);
		}

		internal VerticalAlignments EvaluateStyleVerticalAlign(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "VerticalAlign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.VerticalAlignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateVerticalAlign(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleColor(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStylePaddingLeft(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingLeft", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingLeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingRight(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingRight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingRightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingTop(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingBottom(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "PaddingBottom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingBottomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLineHeight(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "LineHeight", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.LineHeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateLineHeight(ProcessStringResult(result).Value, this);
		}

		internal Directions EvaluateStyleDirection(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Direction", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.DirectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateDirection(ProcessStringResult(result).Value, this);
		}

		internal WritingModes EvaluateStyleWritingMode(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "WritingMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.WritingModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateWritingMode(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLanguage(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Language", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.LanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo culture;
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSpecificLanguage(ProcessStringResult(result).Value, this, out culture);
		}

		internal UnicodeBiDiTypes EvaluateStyleUnicodeBiDi(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "UnicodeBiDi", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.UnicodeBiDiExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateUnicodeBiDi(ProcessStringResult(result).Value, this);
		}

		internal Calendars EvaluateStyleCalendar(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "Calendar", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.CalendarExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateCalendar(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleCurrencyLanguage(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "CurrencyLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(condition: false, "(style.ExprHost should not be invoked for CurrencyLanguage.)");
						result.Value = null;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleNumeralLanguage(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "NumeralLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.NumeralLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo culture;
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateLanguage(ProcessStringResult(result).Value, this, out culture);
		}

		internal object EvaluateStyleNumeralVariant(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, objectType, objectName, "NumeralVariant", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.NumeralVariantExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			if (result.Value == null)
			{
				return null;
			}
			IntegerResult integerResult = ProcessIntegerResult(result);
			if (integerResult.ErrorOccurred)
			{
				return null;
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateNumeralVariant(integerResult.Value, this);
		}

		internal object EvaluateTransparentColor(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal object EvaluatePosition(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateIntegerExpression(expression, objectType, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslatePosition(ProcessStringResult(result).Value, this, objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart);
		}

		internal string EvaluateStyleBackgroundUrlImageValue(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
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

		internal string EvaluateStyleBackgroundEmbeddedImageValue(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateEmbeddedImageName(ProcessStringResult(result).Value, embeddedImages, this);
		}

		internal byte[] EvaluateStyleBackgroundDatabaseImageValue(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
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

		internal string EvaluateStyleBackgroundImageMIMEType(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageMIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageMIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateMimeType(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleTextEffect(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "TextEffect", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextEffectExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateTextEffect(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleShadowColor(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "ShadowColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ShadowColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleShadowOffset(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "ShadowOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundHatchType(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (!EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundHatchType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundHatchTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBackgroundHatchType(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphLeftIndentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.LeftIndent, paragraph.ObjectType, paragraph.Name, "LeftIndent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.LeftIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.LeftIndentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphRightIndentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.RightIndent, paragraph.ObjectType, paragraph.Name, "RightIndent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.RightIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.RightIndentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphHangingIndentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.HangingIndent, paragraph.ObjectType, paragraph.Name, "HangingIndent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.HangingIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.HangingIndentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, allowNegative: true, this);
		}

		internal string EvaluateParagraphSpaceBeforeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.SpaceBefore, paragraph.ObjectType, paragraph.Name, "SpaceBefore", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.SpaceBefore, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.SpaceBeforeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphSpaceAfterExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.SpaceAfter, paragraph.ObjectType, paragraph.Name, "SpaceAfter", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.SpaceAfter, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.SpaceAfterExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal int? EvaluateParagraphListLevelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.ListLevel, paragraph.ObjectType, paragraph.Name, "ListLevel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.ListLevel, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.ListLevelExpr;
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
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateParagraphListLevel(integerResult.Value, this);
		}

		internal string EvaluateParagraphListStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			if (!EvaluateSimpleExpression(paragraph.ListStyle, paragraph.ObjectType, paragraph.Name, "ListStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(paragraph.ListStyle, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.ListStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateParagraphListStyle(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateTextRunToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			if (!EvaluateSimpleExpression(textRun.ToolTip, textRun.ObjectType, textRun.Name, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textRun.ToolTip, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateTextRunMarkupTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			if (!EvaluateSimpleExpression(textRun.MarkupType, textRun.ObjectType, textRun.Name, "MarkupType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(textRun.MarkupType, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.MarkupTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateTextRunMarkupType(ProcessStringResult(result).Value, this);
		}

		internal VariantResult EvaluateTextRunValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = textRun.Value;
			if (!EvaluateSimpleExpression(value, textRun.ObjectType, textRun.Name, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(value, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(value, ref result);
			return result;
		}

		internal bool EvaluatePageBreakDisabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak pageBreak, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, objectType, objectName, "Disabled", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, pageBreak.ExprHost))
						{
							Global.Tracer.Assert(pageBreak.ExprHost != null, "(pageBreak.ExprHost != null)");
							result.Value = pageBreak.ExprHost.DisabledExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessBooleanResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal bool EvaluatePageBreakResetPageNumberExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak pageBreak, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, objectType, objectName, "ResetPageNumber", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, pageBreak.ExprHost))
						{
							Global.Tracer.Assert(pageBreak.ExprHost != null, "(pageBreak.ExprHost != null)");
							result.Value = pageBreak.ExprHost.ResetPageNumberExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessBooleanResult(result).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal VariantResult EvaluateJoinConditionForeignKeyExpression(Relationship.JoinCondition joinCondition)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo foreignKeyExpression = joinCondition.ForeignKeyExpression;
			JoinConditionExprHost exprHost = joinCondition.ExprHost;
			if (!EvaluateSimpleExpression(foreignKeyExpression, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(foreignKeyExpression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						result.Value = exprHost.ForeignKeyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(foreignKeyExpression, ref result);
			VerifyVariantResultAndStopOnError(ref result);
			return result;
		}

		internal VariantResult EvaluateJoinConditionPrimaryKeyExpression(Relationship.JoinCondition joinCondition)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo primaryKeyExpression = joinCondition.PrimaryKeyExpression;
			JoinConditionExprHost exprHost = joinCondition.ExprHost;
			if (!EvaluateSimpleExpression(primaryKeyExpression, out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(primaryKeyExpression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						result.Value = exprHost.PrimaryKeyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(primaryKeyExpression, ref result);
			VerifyVariantResultAndStopOnError(ref result);
			return result;
		}

		private bool EvaluateSimpleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
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

		private bool EvaluateSimpleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, out VariantResult result)
		{
			result = default(VariantResult);
			if (expression != null)
			{
				switch (expression.Type)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant:
					result.Value = expression.Value;
					result.TypeCode = expression.ConstantTypeCode;
					return true;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field:
					EvaluateSimpleFieldReference(expression.IntValue, ref result);
					return true;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token:
				{
					Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = m_reportObjectModel.DataSetsImpl[expression.StringValue];
					result.Value = dataSet.RewrittenCommandText;
					return true;
				}
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					return false;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression:
					return false;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
					return false;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.ScopedFieldReference:
					return false;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.RdlFunction:
					EvaluateRdlFunction(expression, ref result);
					return true;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Literal:
					result.Value = expression.LiteralInfo.Value;
					return true;
				default:
					Global.Tracer.Assert(condition: false);
					return true;
				}
			}
			return true;
		}

		internal void EvaluateSimpleFieldReference(int fieldIndex, ref VariantResult result)
		{
			try
			{
				Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.FieldImpl fieldImpl = m_reportObjectModel.FieldsImpl[fieldIndex];
				if (fieldImpl.IsMissing)
				{
					result.Value = null;
				}
				else if (fieldImpl.FieldStatus != 0)
				{
					result.ErrorOccurred = true;
					result.FieldStatus = fieldImpl.FieldStatus;
					result.ExceptionMessage = fieldImpl.ExceptionMessage;
					result.Value = null;
				}
				else
				{
					result.Value = fieldImpl.Value;
				}
			}
			catch (ReportProcessingException_NoRowsFieldAccess e)
			{
				RegisterRuntimeWarning(e, this);
				result.Value = null;
			}
			catch (ReportProcessingException_InvalidOperationException)
			{
				result.Value = null;
				result.ErrorOccurred = true;
			}
		}

		private bool EvaluateComplexExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, ReportObjectModelProxy exprHost)
		{
			if (expression != null)
			{
				switch (expression.Type)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					result.Value = m_reportObjectModel.AggregatesImpl[expression.StringValue];
					return true;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				{
					Lookup lookup2 = m_reportObjectModel.LookupsImpl[expression.StringValue];
					result.Value = lookup2.Value;
					return true;
				}
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
				{
					Lookup lookup = m_reportObjectModel.LookupsImpl[expression.StringValue];
					result.Value = lookup.Values;
					return true;
				}
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.ScopedFieldReference:
					try
					{
						m_reportObjectModel.OdpContext.StateManager.EvaluateScopedFieldReference(expression.StringValue, expression.ScopedFieldInfo.FieldIndex, ref result);
					}
					catch (ReportProcessingException_NonExistingScopeReference reportProcessingException_NonExistingScopeReference)
					{
						result.Value = null;
						result.ErrorOccurred = true;
						result.ExceptionMessage = reportProcessingException_NonExistingScopeReference.Message;
					}
					catch (ReportProcessingException_InvalidScopeReference reportProcessingException_InvalidScopeReference)
					{
						result.Value = null;
						result.ErrorOccurred = true;
						result.ExceptionMessage = reportProcessingException_InvalidScopeReference.Message;
					}
					return true;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression:
					if (m_exprHostInSandboxAppDomain)
					{
						exprHost?.SetReportObjectModel(m_reportObjectModel);
					}
					return false;
				default:
					Global.Tracer.Assert(condition: false);
					return true;
				}
			}
			return true;
		}

		private void EvaluateRdlFunction(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions = expression.RdlFunctionInfo.Expressions;
			object[] array = new object[expressions.Count];
			for (int i = 0; i < expressions.Count; i++)
			{
				if (!EvaluateSimpleExpression(expressions[i], out VariantResult result2) && !EvaluateComplexExpression(expressions[i], ref result2, null))
				{
					Global.Tracer.Assert(condition: false, "Rdl function argument is complex.");
				}
				array[i] = result2.Value;
				if (result2.ErrorOccurred)
				{
					result = result2;
					return;
				}
			}
			switch (expression.RdlFunctionInfo.FunctionType)
			{
			case RdlFunctionInfo.RdlFunctionType.MinValue:
				result.Value = MinValue(array);
				break;
			case RdlFunctionInfo.RdlFunctionType.MaxValue:
				result.Value = MaxValue(array);
				break;
			default:
				Global.Tracer.Assert(condition: false, "No case for: " + expression.RdlFunctionInfo.FunctionType);
				break;
			}
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
			if (result.ErrorOccurred)
			{
				ProcessingMessageList messages = m_errorContext.Messages;
				if (messages != null && messages.Count > 0)
				{
					throw new ReportProcessingException(messages[0].Message, ErrorCode.rsProcessingError);
				}
				throw new ReportProcessingException(messages);
			}
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e)
		{
			RegisterRuntimeErrorInExpression(ref result, e, this, isError: false);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e, IErrorContext iErrorContext, bool isError)
		{
			if (e is RSException || AsynchronousExceptionDetection.IsStoppingException(e))
			{
				throw new ReportProcessingException(e.GetType().FullName + ": " + e.Message, ErrorCode.rsProcessingError);
			}
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
			StringBuilder stringBuilder = new StringBuilder();
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				if (ex.Message != null)
				{
					stringBuilder.Append(ex.Message);
					stringBuilder.Append(";");
				}
				if (ex.Source != null)
				{
					stringBuilder.Append(" Source: ");
					stringBuilder.Append(ex.Source);
					stringBuilder.Append(";");
				}
			}
			result.ExceptionMessage = stringBuilder.ToString();
		}

		private bool EvaluateBooleanExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool canBeConstant, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.BoolValue;
					result.TypeCode = TypeCode.Boolean;
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
			return ProcessBooleanResult(result, stopOnError: false, m_objectType, null);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result, bool stopOnError, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			BooleanResult result2 = default(BooleanResult);
			bool processedValue;
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				if (stopOnError && result.FieldStatus != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, objectType, objectName, "Hidden", GetErrorName(result.FieldStatus, result.ExceptionMessage));
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
			else if (TryProcessObjectToBoolean(result.Value, out processedValue))
			{
				result2.Value = processedValue;
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

		internal static bool TryProcessObjectToBoolean(object value, out bool processedValue)
		{
			if (value is bool)
			{
				processedValue = (bool)value;
				return true;
			}
			if (value == null || DBNull.Value == value)
			{
				processedValue = false;
				return true;
			}
			processedValue = false;
			return false;
		}

		private bool EvaluateBinaryExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BinaryResult ProcessBinaryResult(VariantResult result)
		{
			BinaryResult result2 = default(BinaryResult);
			object value = result.Value;
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (value is byte[])
			{
				byte[] array = (byte[])value;
				if (ViolatesMaxArrayResultLength(array.Length))
				{
					result2.ErrorOccurred = true;
					result2.Value = null;
					RegisterSandboxMaxArrayLengthWarning();
				}
				else
				{
					result2.Value = array;
				}
			}
			else if (value == null || DBNull.Value == value)
			{
				result2.Value = null;
			}
			else
			{
				if (value is string)
				{
					try
					{
						string text = (string)value;
						if (ViolatesMaxStringResultLength(text))
						{
							result2.ErrorOccurred = true;
							result2.Value = null;
							RegisterSandboxMaxStringLengthWarning();
							return result2;
						}
						byte[] array2 = Convert.FromBase64String(text);
						if (array2 != null && ViolatesMaxArrayResultLength(array2.Length))
						{
							result2.ErrorOccurred = true;
							result2.Value = null;
							RegisterSandboxMaxArrayLengthWarning();
							return result2;
						}
						result2.Value = array2;
						return result2;
					}
					catch (FormatException)
					{
						result2.ErrorOccurred = true;
						RegisterInvalidExpressionDataTypeWarning();
						return result2;
					}
				}
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private StringResult ProcessAutocastStringResult(VariantResult result)
		{
			return ProcessStringResult(result, autocast: true);
		}

		private StringResult ProcessStringResult(VariantResult result)
		{
			return ProcessStringResult(result, autocast: false);
		}

		private StringResult ProcessStringResult(VariantResult result, bool autocast)
		{
			StringResult result2 = default(StringResult);
			result2.Value = ProcessVariantResultToString(result, autocast, Severity.Warning, out bool errorOccured);
			result2.ErrorOccurred = errorOccured;
			result2.FieldStatus = result.FieldStatus;
			return result2;
		}

		private void ProcessLabelResult(ref VariantResult result)
		{
			result.Value = ProcessVariantResultToString(result, autocast: true, Severity.Error, out bool errorOccured);
			result.ErrorOccurred = errorOccured;
			if (errorOccured)
			{
				throw new ReportProcessingException(m_errorContext.Messages);
			}
		}

		private string ProcessVariantResultToString(VariantResult result, bool autocast, Severity severity, out bool errorOccured)
		{
			string output = null;
			if (result.ErrorOccurred)
			{
				errorOccured = true;
			}
			else
			{
				errorOccured = !ProcessObjectToString(result.Value, autocast, out output);
				if (errorOccured)
				{
					RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, severity);
				}
				else if (ViolatesMaxStringResultLength(output))
				{
					RegisterSandboxMaxStringLengthWarning();
				}
			}
			return output;
		}

		internal static bool ProcessObjectToString(object value, bool autocast, out string output)
		{
			output = null;
			bool flag = false;
			if (value == null || DBNull.Value == value)
			{
				output = null;
			}
			else if (value is string)
			{
				output = (string)value;
			}
			else if (value is char)
			{
				output = Convert.ToString((char)value, CultureInfo.CurrentCulture);
			}
			else if (value is Guid)
			{
				output = ((Guid)value).ToString();
			}
			else if (autocast)
			{
				output = value.ToString();
			}
			else
			{
				flag = true;
			}
			return !flag;
		}

		private bool EvaluateIntegerExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (expression.ConstantType == DataType.Integer)
				{
					result.Value = expression.IntValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else
				{
					result.ErrorOccurred = true;
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
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
				return result2;
			}
			if (result.Value == null || result.Value == DBNull.Value)
			{
				return result2;
			}
			if (!SetVariantType(ref result))
			{
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
				return result2;
			}
			if (result.TypeCode == TypeCode.Object)
			{
				ConvertFromSqlTypes(ref result);
			}
			switch (result.TypeCode)
			{
			case TypeCode.Int32:
				result2.Value = (int)result.Value;
				break;
			case TypeCode.Byte:
				result2.Value = Convert.ToInt32((byte)result.Value);
				break;
			case TypeCode.SByte:
				result2.Value = Convert.ToInt32((sbyte)result.Value);
				break;
			case TypeCode.Int16:
				result2.Value = Convert.ToInt32((short)result.Value);
				break;
			case TypeCode.UInt16:
				result2.Value = Convert.ToInt32((ushort)result.Value);
				break;
			case TypeCode.UInt32:
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
			case TypeCode.Int64:
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
			case TypeCode.UInt64:
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
			case TypeCode.Double:
				try
				{
					result2.Value = Convert.ToInt32((double)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Single:
				try
				{
					result2.Value = Convert.ToInt32((float)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			case TypeCode.Decimal:
				try
				{
					result2.Value = Convert.ToInt32((decimal)result.Value);
					return result2;
				}
				catch (OverflowException)
				{
					result2.ErrorOccurred = true;
					return result2;
				}
			default:
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
				break;
			}
			return result2;
		}

		private bool EvaluateIntegerOrFloatExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (expression.ConstantType == DataType.Integer)
				{
					result.Value = expression.IntValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else if (expression.ConstantType == DataType.Float)
				{
					result.Value = expression.FloatValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else
				{
					result = default(VariantResult);
					result.ErrorOccurred = true;
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private FloatResult ProcessIntegerOrFloatResult(VariantResult result)
		{
			FloatResult result2 = default(FloatResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
				return result2;
			}
			if (result.Value == null || result.Value == DBNull.Value)
			{
				return result2;
			}
			if (!SetVariantType(ref result))
			{
				result2.ErrorOccurred = true;
				RegisterInvalidExpressionDataTypeWarning();
				return result2;
			}
			if (result.TypeCode == TypeCode.Object)
			{
				ConvertFromSqlTypes(ref result);
			}
			switch (result.TypeCode)
			{
			case TypeCode.Int32:
				result2.Value = (int)result.Value;
				break;
			case TypeCode.Byte:
				result2.Value = Convert.ToInt32((byte)result.Value);
				break;
			case TypeCode.SByte:
				result2.Value = Convert.ToInt32((sbyte)result.Value);
				break;
			case TypeCode.Int16:
				result2.Value = Convert.ToInt32((short)result.Value);
				break;
			case TypeCode.UInt16:
				result2.Value = Convert.ToInt32((ushort)result.Value);
				break;
			case TypeCode.Double:
				result2.Value = (double)result.Value;
				break;
			case TypeCode.Single:
				result2.Value = Convert.ToDouble((float)result.Value);
				break;
			case TypeCode.UInt32:
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
			case TypeCode.Int64:
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
			case TypeCode.UInt64:
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
			case TypeCode.Decimal:
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
			default:
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
				break;
			}
			return result2;
		}

		private void ProcessLookupVariantResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, IErrorContext errorContext, bool isArrayRequired, bool normalizeDBNullToNull, ref VariantResult result)
		{
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, isArrayRequired, isArrayRequired, isByteArrayAllowed: false, normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					errorContext.Register(ProcessingErrorCode.rsLookupOfInvalidExpressionDataType, Severity.Warning);
					break;
				case NormalizationCode.StringLengthViolation:
					errorContext.Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, Convert.ToString(m_maxStringResultLength, CultureInfo.InvariantCulture));
					break;
				case NormalizationCode.ArrayLengthViolation:
					errorContext.Register(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength, Severity.Warning, Convert.ToString(m_maxArrayResultLength, CultureInfo.InvariantCulture));
					break;
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private bool NormalizeVariantValue(VariantResult result, bool isArrayAllowed, bool isArrayRequired, bool isByteArrayAllowed, bool normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode)
		{
			if (!isArrayRequired && GetVariantTypeCode(result.Value, out typeCode))
			{
				if (typeCode == TypeCode.String && ViolatesMaxStringResultLength((string)result.Value))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.StringLengthViolation;
					return false;
				}
				normalizedValue = result.Value;
			}
			else if (!isArrayRequired && (result.Value == null || result.Value == DBNull.Value))
			{
				if (normalizeDBNullToNull)
				{
					normalizedValue = null;
				}
				else
				{
					normalizedValue = DBNull.Value;
				}
				typeCode = TypeCode.Empty;
			}
			else if (isArrayAllowed && result.Value is object[])
			{
				object[] array = (object[])result.Value;
				if (ViolatesMaxArrayResultLength(array.Length))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				if (!NormalizeVariantArray(array.GetEnumerator(), array, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
			}
			else if (!isArrayRequired && isByteArrayAllowed && result.Value is byte[])
			{
				byte[] array2 = (byte[])result.Value;
				if (ViolatesMaxArrayResultLength(array2.Length))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				normalizedValue = result.Value;
				typeCode = TypeCode.Object;
			}
			else if (isArrayAllowed && result.Value is IList)
			{
				IList list = (IList)result.Value;
				if (ViolatesMaxArrayResultLength(list.Count))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				object[] destArr = new object[list.Count];
				if (!NormalizeVariantArray(list.GetEnumerator(), destArr, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
			}
			else if (!isArrayRequired && result.Value is Guid)
			{
				normalizedValue = ((Guid)result.Value).ToString();
				typeCode = TypeCode.String;
			}
			else
			{
				if (result.Value == null || !ConvertFromSqlTypes(ref result))
				{
					typeCode = TypeCode.Empty;
					normalizedValue = null;
					normalCode = NormalizationCode.InvalidType;
					return false;
				}
				typeCode = TypeCode.Object;
				normalizedValue = result.Value;
			}
			normalCode = NormalizationCode.Success;
			return true;
		}

		private bool NormalizeVariantArray(IEnumerator source, object[] destArr, bool normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode)
		{
			int num = 0;
			while (source.MoveNext())
			{
				VariantResult result = default(VariantResult);
				result.Value = source.Current;
				if (!NormalizeVariantValue(result, isArrayAllowed: false, isArrayRequired: false, isByteArrayAllowed: false, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
				destArr[num] = normalizedValue;
				num++;
			}
			normalizedValue = destArr;
			typeCode = TypeCode.Object;
			normalCode = NormalizationCode.Success;
			return true;
		}

		private void ProcessReportParameterVariantResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			if (result.ErrorOccurred)
			{
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, isArrayAllowed: true, isArrayRequired: false, isByteArrayAllowed: false, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error);
					break;
				case NormalizationCode.StringLengthViolation:
					RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private bool ViolatesMaxStringResultLength(string value)
		{
			if (m_maxStringResultLength != -1 && value != null)
			{
				return value.Length > m_maxStringResultLength;
			}
			return false;
		}

		private bool ViolatesMaxArrayResultLength(int count)
		{
			if (m_maxArrayResultLength != -1)
			{
				return count > m_maxArrayResultLength;
			}
			return false;
		}

		private void ProcessSerializableResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool isReportScope, ref VariantResult result)
		{
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				ProcessSerializableResult(isReportScope, ref result);
			}
		}

		internal bool ProcessSerializableResult(bool isReportScope, ref VariantResult result)
		{
			bool result2 = false;
			if (!NormalizeVariantValue(result, isArrayAllowed: true, isArrayRequired: false, !m_rdlSandboxingEnabled, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					if (isReportScope)
					{
						try
						{
							if (result.Value is ISerializable || (result.Value.GetType().Attributes & TypeAttributes.Serializable) != 0)
							{
								result.TypeCode = TypeCode.Object;
								if (m_isSerializableValuesProhibited)
								{
									((IErrorContext)this).Register(ProcessingErrorCode.rsSerializableTypeNotSupported, Severity.Error, new string[2]
									{
										m_objectType.ToString(),
										m_objectName
									});
									result.Value = null;
									return false;
								}
								if (!m_rdlSandboxingEnabled)
								{
									result.ErrorOccurred = false;
									return false;
								}
							}
							else
							{
								result2 = true;
								result.Value = null;
							}
						}
						catch (Exception ex)
						{
							((IErrorContext)this).Register(ProcessingErrorCode.rsUnexpectedSerializationError, Severity.Warning, new string[1]
							{
								ex.Message
							});
							result.Value = null;
						}
					}
					else
					{
						result.Value = null;
					}
					RegisterInvalidExpressionDataTypeWarning();
					break;
				case NormalizationCode.StringLengthViolation:
					RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			else
			{
				result.Value = normalizedValue;
				result.TypeCode = typeCode;
			}
			return result2;
		}

		private void ProcessVariantResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			ProcessVariantResult(expression, ref result, isArrayAllowed: false);
		}

		private void ProcessVariantResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, bool isArrayAllowed)
		{
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, isArrayAllowed, isArrayRequired: false, isByteArrayAllowed: false, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				switch (normalCode)
				{
				case NormalizationCode.InvalidType:
					RegisterInvalidExpressionDataTypeWarning();
					break;
				case NormalizationCode.StringLengthViolation:
					RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private void VerifyVariantResultAndStopOnError(ref VariantResult result)
		{
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
		}

		private void ProcessVariantOrBinaryResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, bool isAggregate, bool allowArray)
		{
			if (expression == null || !expression.IsExpression || result.ErrorOccurred)
			{
				return;
			}
			if (!NormalizeVariantValue(result, allowArray, isArrayRequired: false, isByteArrayAllowed: true, normalizeDBNullToNull: true, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode))
			{
				result.ErrorOccurred = true;
				if (isAggregate)
				{
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
				}
				else
				{
					switch (normalCode)
					{
					case NormalizationCode.InvalidType:
						RegisterInvalidExpressionDataTypeWarning();
						break;
					case NormalizationCode.StringLengthViolation:
						RegisterSandboxMaxStringLengthWarning();
						break;
					case NormalizationCode.ArrayLengthViolation:
						RegisterSandboxMaxArrayLengthWarning();
						break;
					}
				}
			}
			result.Value = normalizedValue;
			result.TypeCode = typeCode;
		}

		private ParameterValueResult ProcessParameterValueResult(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string paramName, VariantResult result)
		{
			ParameterValueResult result2 = default(ParameterValueResult);
			if (expression != null)
			{
				if (expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
				{
					result2.Value = result.Value;
					result2.Type = expression.ConstantType;
				}
				else if (result.ErrorOccurred)
				{
					result2.ErrorOccurred = true;
				}
				else
				{
					if (!NormalizeParameterVariantValue(result.Value, paramName, out object normalizedValue, out DataType dataType) && Type.GetTypeCode(result.Value.GetType()) == TypeCode.Object && !ConvertFromSqlTypes(ref result))
					{
						result2.ErrorOccurred = true;
					}
					result2.Value = normalizedValue;
					result2.Type = dataType;
				}
			}
			return result2;
		}

		private bool NormalizeParameterVariantValue(object value, string paramName, out object normalizedValue, out DataType dataType)
		{
			if (value == null || value == DBNull.Value)
			{
				normalizedValue = null;
				dataType = DataType.String;
			}
			else if (value is bool)
			{
				normalizedValue = value;
				dataType = DataType.Boolean;
			}
			else if (value is DateTime)
			{
				normalizedValue = value;
				dataType = DataType.DateTime;
			}
			else if (value is double || value is float || value is decimal)
			{
				normalizedValue = Convert.ToDouble(value, CultureInfo.CurrentCulture);
				dataType = DataType.Float;
			}
			else if (value is string)
			{
				string text = (string)value;
				if (ViolatesMaxStringResultLength(text))
				{
					dataType = DataType.String;
					normalizedValue = null;
					RegisterSandboxMaxStringLengthWarning();
					return false;
				}
				normalizedValue = text;
				dataType = DataType.String;
			}
			else if (value is char)
			{
				normalizedValue = Convert.ToString(value, CultureInfo.CurrentCulture);
				dataType = DataType.String;
			}
			else if (value is int || value is short || value is byte || value is sbyte || value is ushort)
			{
				normalizedValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
				dataType = DataType.Integer;
			}
			else if (value is uint || value is long || value is ulong)
			{
				try
				{
					normalizedValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
					dataType = DataType.Integer;
				}
				catch (OverflowException)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsParameterValueCastFailure, Severity.Warning, m_objectType, m_objectName, m_propertyName, new string[1]
					{
						paramName
					});
					normalizedValue = value;
					dataType = DataType.Integer;
					return false;
				}
			}
			else if (value is TimeSpan)
			{
				try
				{
					normalizedValue = Convert.ToString(value, CultureInfo.CurrentCulture);
					dataType = DataType.String;
				}
				catch (FormatException)
				{
					RegisterInvalidExpressionDataTypeWarning();
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
			}
			else if (value is DateTimeOffset)
			{
				normalizedValue = value;
				dataType = DataType.DateTime;
			}
			else if (value is Guid)
			{
				normalizedValue = ((Guid)value).ToString();
				dataType = DataType.String;
			}
			else if (value is object[])
			{
				object[] array = (object[])value;
				if (ViolatesMaxArrayResultLength(array.Length))
				{
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				if (!NormalizeParameterVariantArray(array.GetEnumerator(), array, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
			}
			else
			{
				if (!(value is IList))
				{
					RegisterInvalidExpressionDataTypeWarning();
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				IList list = (IList)value;
				if (ViolatesMaxArrayResultLength(list.Count))
				{
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				object[] destArr = new object[list.Count];
				if (!NormalizeParameterVariantArray(list.GetEnumerator(), destArr, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
			}
			return true;
		}

		private bool NormalizeParameterVariantArray(IEnumerator source, object[] destArr, string paramName, out object normalizedValue, out DataType dataType)
		{
			dataType = DataType.String;
			int num = 0;
			while (source.MoveNext())
			{
				if (!NormalizeParameterVariantValue(source.Current, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
				destArr[num] = normalizedValue;
				num++;
			}
			normalizedValue = destArr;
			return true;
		}

		private static object[] GetAsObjectArray(ref VariantResult result)
		{
			object[] array = result.Value as object[];
			if (array == null)
			{
				IList list = result.Value as IList;
				if (list != null)
				{
					array = new object[list.Count];
					list.CopyTo(array, 0);
				}
			}
			return array;
		}

		private DataType GetDataType(object obj)
		{
			TypeCode typeCode = TypeCode.Empty;
			if (obj != null)
			{
				typeCode = Type.GetTypeCode(obj.GetType());
			}
			switch (typeCode)
			{
			case TypeCode.Boolean:
				return DataType.Boolean;
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return DataType.Float;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return DataType.Integer;
			case TypeCode.DateTime:
				return DataType.DateTime;
			case TypeCode.Empty:
			case TypeCode.DBNull:
			case TypeCode.Char:
			case TypeCode.String:
				return DataType.String;
			default:
				if (obj is TimeSpan)
				{
					return DataType.Integer;
				}
				if (obj is DateTimeOffset)
				{
					return DataType.DateTime;
				}
				return DataType.String;
			}
		}

		private void SetNullResult(ref VariantResult result)
		{
			result.Value = null;
			result.TypeCode = TypeCode.Empty;
		}

		private void SetGuidResult(ref VariantResult result)
		{
			result.Value = ((Guid)result.Value).ToString();
			result.TypeCode = TypeCode.String;
		}

		private bool ConvertFromSqlTypes(ref VariantResult result)
		{
			if (result.Value is SqlInt32)
			{
				if (((SqlInt32)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int32;
					result.Value = ((SqlInt32)result.Value).Value;
				}
			}
			else if (result.Value is SqlInt16)
			{
				if (((SqlInt16)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int16;
					result.Value = ((SqlInt16)result.Value).Value;
				}
			}
			else if (result.Value is SqlInt64)
			{
				if (((SqlInt64)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int64;
					result.Value = ((SqlInt64)result.Value).Value;
				}
			}
			else if (result.Value is SqlBoolean)
			{
				if (((SqlBoolean)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Boolean;
					result.Value = ((SqlBoolean)result.Value).Value;
				}
			}
			else if (result.Value is SqlDecimal)
			{
				if (((SqlDecimal)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Decimal;
					result.Value = ((SqlDecimal)result.Value).Value;
				}
			}
			else if (result.Value is SqlDouble)
			{
				if (((SqlDouble)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Double;
					result.Value = ((SqlDouble)result.Value).Value;
				}
			}
			else if (result.Value is SqlDateTime)
			{
				if (((SqlDateTime)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.DateTime;
					result.Value = ((SqlDateTime)result.Value).Value;
				}
			}
			else if (result.Value is SqlMoney)
			{
				if (((SqlMoney)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Decimal;
					result.Value = ((SqlMoney)result.Value).Value;
				}
			}
			else if (result.Value is SqlSingle)
			{
				if (((SqlSingle)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Single;
					result.Value = ((SqlSingle)result.Value).Value;
				}
			}
			else if (result.Value is SqlByte)
			{
				if (((SqlByte)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Byte;
					result.Value = ((SqlByte)result.Value).Value;
				}
			}
			else if (result.Value is SqlString)
			{
				if (((SqlString)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.String;
					string value = ((SqlString)result.Value).Value;
					if (ViolatesMaxStringResultLength(value))
					{
						value = null;
						RegisterSandboxMaxStringLengthWarning();
					}
					result.Value = value;
				}
			}
			else
			{
				if (!(result.Value is SqlGuid))
				{
					return false;
				}
				if (((SqlGuid)result.Value).IsNull)
				{
					SetNullResult(ref result);
				}
				else
				{
					SetGuidResult(ref result);
				}
			}
			return true;
		}

		private bool EvaluateNoConstantExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = new VariantResult(errorOccurred: true, null);
				RegisterInvalidExpressionDataTypeWarning();
				return true;
			}
			return EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		internal static bool GetVariantTypeCode(object o, out TypeCode typeCode)
		{
			if (o == null)
			{
				typeCode = TypeCode.Empty;
			}
			else
			{
				Type type = o.GetType();
				typeCode = Type.GetTypeCode(type);
				switch (typeCode)
				{
				case TypeCode.Empty:
				case TypeCode.DBNull:
					return false;
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					return true;
				}
				if (o is TimeSpan || o is DateTimeOffset)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool SetVariantType(ref VariantResult result)
		{
			if (GetVariantTypeCode(result.Value, out TypeCode typeCode))
			{
				result.TypeCode = typeCode;
				return true;
			}
			return false;
		}

		internal static string ConvertToStringFallBack(object value)
		{
			try
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}", value);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void RegisterInvalidExpressionDataTypeWarning()
		{
			RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning);
		}

		private void RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode errorCode, Severity severity)
		{
			((IErrorContext)this).Register(errorCode, severity, Array.Empty<string>());
		}

		private void RegisterSandboxMaxStringLengthWarning()
		{
			ReportObjectModel.OdpContext.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength);
			((IErrorContext)this).Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, new string[1]
			{
				Convert.ToString(m_maxStringResultLength, CultureInfo.InvariantCulture)
			});
		}

		private void RegisterSandboxMaxArrayLengthWarning()
		{
			ReportObjectModel.OdpContext.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength);
			((IErrorContext)this).Register(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength, Severity.Warning, new string[1]
			{
				Convert.ToString(m_maxArrayResultLength, CultureInfo.InvariantCulture)
			});
		}

		internal object MinValue(params object[] arguments)
		{
			if (arguments != null)
			{
				object obj = arguments[0];
				for (int i = 1; i < arguments.Length; i++)
				{
					if (CompareWithExtendedTypesAndStopOnError(obj, arguments[i]) > 0)
					{
						obj = arguments[i];
					}
				}
				return obj;
			}
			return null;
		}

		internal object MaxValue(params object[] arguments)
		{
			if (arguments != null)
			{
				object obj = arguments[0];
				for (int i = 1; i < arguments.Length; i++)
				{
					if (CompareWithExtendedTypesAndStopOnError(obj, arguments[i]) < 0)
					{
						obj = arguments[i];
					}
				}
				return obj;
			}
			return null;
		}

		private int CompareWithExtendedTypesAndStopOnError(object x, object y)
		{
			return m_reportObjectModel.OdpContext.CompareAndStopOnError(x, y, m_objectType, m_objectName, m_propertyName, extendedTypeComparisons: true);
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

		internal void LoadCompiledCode(IExpressionHostAssemblyHolder expressionHostAssemblyHolder, bool includeParameters, bool parametersOnly, Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ReportRuntimeSetup runtimeSetup)
		{
			if (expressionHostAssemblyHolder.CompiledCode == null || expressionHostAssemblyHolder.CompiledCode.Length == 0)
			{
				return;
			}
			try
			{
				if (runtimeSetup.RequireExpressionHostWithRefusedPermissions && !expressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace("Expression host generated with refused permissions is required.");
					}
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (runtimeSetup.AssemblyLoadContext == null)
				{
					m_exprHostInSandboxAppDomain = false;
					if (expressionHostAssemblyHolder.CodeModules != null)
					{
						for (int i = 0; i < expressionHostAssemblyHolder.CodeModules.Count; i++)
						{
							if (!runtimeSetup.CheckCodeModuleIsTrustedInCurrentAppDomain(expressionHostAssemblyHolder.CodeModules[i]))
							{
								m_errorContext.Register(ProcessingErrorCode.rsUntrustedCodeModule, Severity.Error, expressionHostAssemblyHolder.ObjectType, null, null, expressionHostAssemblyHolder.CodeModules[i]);
								throw new ReportProcessingException(m_errorContext.Messages);
							}
						}
					}
					m_reportExprHost = ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(expressionHostAssemblyHolder.CompiledCode, expressionHostAssemblyHolder.ExprHostAssemblyName, runtimeSetup.ExprHostEvidence, includeParameters, parametersOnly, reportObjectModel, expressionHostAssemblyHolder.CodeModules, runtimeSetup.AssemblyLoadContext);
				}
				else
				{
					m_exprHostInSandboxAppDomain = true;
					m_reportExprHost = ExpressionHostLoader.LoadExprHost(expressionHostAssemblyHolder.CompiledCode, expressionHostAssemblyHolder.ExprHostAssemblyName, includeParameters, parametersOnly, reportObjectModel, expressionHostAssemblyHolder.CodeModules, runtimeSetup.AssemblyLoadContext);
				}
			}
			catch (ReportProcessingException)
			{
				throw;
			}
			catch (Exception e)
			{
				ProcessLoadingExprHostException(expressionHostAssemblyHolder.ObjectType, e, ProcessingErrorCode.rsErrorLoadingExprHostAssembly);
			}
		}

		internal void CustomCodeOnInit(IExpressionHostAssemblyHolder expressionHostAssemblyHolder)
		{
			if (expressionHostAssemblyHolder.CompiledCode.Length != 0)
			{
				try
				{
					m_reportExprHost.CustomCodeOnInit();
				}
				catch (ReportProcessingException)
				{
					throw;
				}
				catch (Exception e)
				{
					ProcessLoadingExprHostException(expressionHostAssemblyHolder.ObjectType, e, ProcessingErrorCode.rsErrorInOnInit);
				}
			}
		}

		private void ProcessLoadingExprHostException(Microsoft.ReportingServices.ReportProcessing.ObjectType assemblyHolderObjectType, Exception e, ProcessingErrorCode errorCode)
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
			ProcessingMessage processingMessage = m_errorContext.Register(errorCode, Severity.Error, assemblyHolderObjectType, null, null, text);
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

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportRuntime;
		}

		public void SetID(int id)
		{
			m_id = id;
		}

		internal string EvaluateBaseGaugeImageSourceExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.Source, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.Source, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateBaseGaugeImageStringValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.Value, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.ValueExpr;
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

		internal byte[] EvaluateBaseGaugeImageBinaryValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(baseGaugeImage.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.Value, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.ValueExpr;
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

		internal string EvaluateBaseGaugeImageMIMETypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.MIMEType, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.MIMEType, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateBaseGaugeImageTransparentColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(baseGaugeImage.TransparentColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(baseGaugeImage.TransparentColor, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateGaugeImageSourceExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeImage.Source, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeImage.Source, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateGaugeImageValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeImage.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeImage.Value, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(gaugeImage.Value, ref result);
			return result;
		}

		internal string EvaluateGaugeImageTransparentColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeImage.TransparentColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeImage.TransparentColor, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateCapImageHueColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			if (!EvaluateSimpleExpression(capImage.HueColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(capImage.HueColor, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateCapImageOffsetXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			if (!EvaluateSimpleExpression(capImage.OffsetX, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(capImage.OffsetX, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateCapImageOffsetYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			if (!EvaluateSimpleExpression(capImage.OffsetY, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(capImage.OffsetY, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateFrameImageHueColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			if (!EvaluateSimpleExpression(frameImage.HueColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(frameImage.HueColor, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateFrameImageTransparencyExpression(Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			if (!EvaluateSimpleExpression(frameImage.Transparency, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(frameImage.Transparency, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateFrameImageClipImageExpression(Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			if (!EvaluateSimpleExpression(frameImage.ClipImage, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ClipImage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(frameImage.ClipImage, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).ClipImageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateTopImageHueColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TopImage topImage, string objectName)
		{
			if (!EvaluateSimpleExpression(topImage.HueColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(topImage.HueColor, ref result, topImage.ExprHost))
					{
						Global.Tracer.Assert(topImage.ExprHost != null, "(topImage.ExprHost != null)");
						result.Value = ((TopImageExprHost)topImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateBackFrameFrameStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.FrameStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.FrameStyle, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateBackFrameFrameShapeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.FrameShape, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameShape", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.FrameShape, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameShapeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateBackFrameFrameWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.FrameWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.FrameWidth, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateBackFrameGlassEffectExpression(Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			if (!EvaluateSimpleExpression(backFrame.GlassEffect, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "GlassEffect", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(backFrame.GlassEffect, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.GlassEffectExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePanelAntiAliasingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.AntiAliasing, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AntiAliasing", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.AntiAliasing, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.AntiAliasingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePanelAutoLayoutExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.AutoLayout, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AutoLayout", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.AutoLayout, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.AutoLayoutExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePanelShadowIntensityExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.ShadowIntensity, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShadowIntensity", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.ShadowIntensity, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.ShadowIntensityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePanelTextAntiAliasingQualityExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanel.TextAntiAliasingQuality, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TextAntiAliasingQuality", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanel.TextAntiAliasingQuality, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.TextAntiAliasingQualityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugePanelItemTopExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Top, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Top", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Top, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.TopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemLeftExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Left, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Left", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Left, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemHeightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Height, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Height", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Height, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.HeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Width, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal int EvaluateGaugePanelItemZIndexExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.ZIndex, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ZIndex", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.ZIndex, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.ZIndexExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateGaugePanelItemHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.Hidden, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateGaugePanelItemToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePanelItem.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePanelItem.ToolTip, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePointerBarStartExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.BarStart, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BarStart", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.BarStart, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.BarStartExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugePointerDistanceFromScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.DistanceFromScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.DistanceFromScale, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePointerMarkerLengthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.MarkerLength, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MarkerLength", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.MarkerLength, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.MarkerLengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePointerMarkerStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.MarkerStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MarkerStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.MarkerStyle, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.MarkerStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePointerPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.Placement, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.Placement, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePointerSnappingEnabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.SnappingEnabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingEnabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.SnappingEnabled, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.SnappingEnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePointerSnappingIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.SnappingInterval, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.SnappingInterval, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.SnappingIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePointerToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.ToolTip, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePointerHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.Hidden, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePointerWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugePointer.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugePointer.Width, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Interval, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.IntervalOffset, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateGaugeScaleLogarithmicExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Logarithmic, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Logarithmic", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Logarithmic, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.LogarithmicExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeScaleLogarithmicBaseExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.LogarithmicBase, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "LogarithmicBase", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.LogarithmicBase, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.LogarithmicBaseExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleMultiplierExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Multiplier, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Multiplier, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateGaugeScaleReversedExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Reversed, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Reversed", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Reversed, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.ReversedExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateGaugeScaleTickMarksOnTopExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.TickMarksOnTop, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TickMarksOnTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.TickMarksOnTop, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.TickMarksOnTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateGaugeScaleToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.ToolTip, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugeScaleHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Hidden, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeScaleWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeScale.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeScale.Width, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeTickMarksIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeTickMarks.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeTickMarks.Interval, ref result, gaugeTickMarks.ExprHost))
					{
						Global.Tracer.Assert(gaugeTickMarks.ExprHost != null, "(gaugeTickMarks.ExprHost != null)");
						result.Value = ((GaugeTickMarksExprHost)gaugeTickMarks.ExprHost).IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeTickMarksIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeTickMarks.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeTickMarks.IntervalOffset, ref result, gaugeTickMarks.ExprHost))
					{
						Global.Tracer.Assert(gaugeTickMarks.ExprHost != null, "(gaugeTickMarks.ExprHost != null)");
						result.Value = ((GaugeTickMarksExprHost)gaugeTickMarks.ExprHost).IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateLinearGaugeOrientationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge, string objectName)
		{
			if (!EvaluateSimpleExpression(linearGauge.Orientation, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Orientation", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearGauge.Orientation, ref result, linearGauge.ExprHost))
					{
						Global.Tracer.Assert(linearGauge.ExprHost != null, "(linearGauge.ExprHost != null)");
						result.Value = ((LinearGaugeExprHost)linearGauge.ExprHost).OrientationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateLinearPointerTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer, string objectName)
		{
			if (!EvaluateSimpleExpression(linearPointer.Type, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearPointer.Type, ref result, linearPointer.ExprHost))
					{
						Global.Tracer.Assert(linearPointer.ExprHost != null, "(linearPointer.ExprHost != null)");
						result.Value = ((LinearPointerExprHost)linearPointer.ExprHost).TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateLinearScaleStartMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			if (!EvaluateSimpleExpression(linearScale.StartMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearScale.StartMargin, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).StartMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateLinearScaleEndMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			if (!EvaluateSimpleExpression(linearScale.EndMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EndMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearScale.EndMargin, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).EndMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateLinearScalePositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			if (!EvaluateSimpleExpression(linearScale.Position, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(linearScale.Position, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).PositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePinLabelTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.Text, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.Text, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluatePinLabelAllowUpsideDownExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.AllowUpsideDown, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.AllowUpsideDown, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluatePinLabelDistanceFromScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.DistanceFromScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.DistanceFromScale, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluatePinLabelFontAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.FontAngle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.FontAngle, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePinLabelPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.Placement, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.Placement, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluatePinLabelRotateLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.RotateLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.RotateLabel, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.RotateLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePinLabelUseFontPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(pinLabel.UseFontPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pinLabel.UseFontPercent, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePointerCapOnTopExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.OnTop, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OnTop", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.OnTop, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.OnTopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePointerCapReflectionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.Reflection, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Reflection", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.Reflection, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.ReflectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluatePointerCapCapStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.CapStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "CapStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.CapStyle, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.CapStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluatePointerCapHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.Hidden, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluatePointerCapWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerCap.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerCap.Width, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePointerImageHueColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.HueColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.HueColor, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluatePointerImageTransparencyExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.Transparency, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.Transparency, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePointerImageOffsetXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.OffsetX, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.OffsetX, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluatePointerImageOffsetYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(pointerImage.OffsetY, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(pointerImage.OffsetY, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal double EvaluateRadialGaugePivotXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge, string objectName)
		{
			if (!EvaluateSimpleExpression(radialGauge.PivotX, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "PivotX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialGauge.PivotX, ref result, radialGauge.ExprHost))
					{
						Global.Tracer.Assert(radialGauge.ExprHost != null, "(radialGauge.ExprHost != null)");
						result.Value = ((RadialGaugeExprHost)radialGauge.ExprHost).PivotXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialGaugePivotYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge, string objectName)
		{
			if (!EvaluateSimpleExpression(radialGauge.PivotY, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "PivotY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialGauge.PivotY, ref result, radialGauge.ExprHost))
					{
						Global.Tracer.Assert(radialGauge.ExprHost != null, "(radialGauge.ExprHost != null)");
						result.Value = ((RadialGaugeExprHost)radialGauge.ExprHost).PivotYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateRadialPointerTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer, string objectName)
		{
			if (!EvaluateSimpleExpression(radialPointer.Type, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Type", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialPointer.Type, ref result, radialPointer.ExprHost))
					{
						Global.Tracer.Assert(radialPointer.ExprHost != null, "(radialPointer.ExprHost != null)");
						result.Value = ((RadialPointerExprHost)radialPointer.ExprHost).TypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateRadialPointerNeedleStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer, string objectName)
		{
			if (!EvaluateSimpleExpression(radialPointer.NeedleStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "NeedleStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialPointer.NeedleStyle, ref result, radialPointer.ExprHost))
					{
						Global.Tracer.Assert(radialPointer.ExprHost != null, "(radialPointer.ExprHost != null)");
						result.Value = ((RadialPointerExprHost)radialPointer.ExprHost).NeedleStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateRadialScaleRadiusExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			if (!EvaluateSimpleExpression(radialScale.Radius, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Radius", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialScale.Radius, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).RadiusExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialScaleStartAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			if (!EvaluateSimpleExpression(radialScale.StartAngle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialScale.StartAngle, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).StartAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialScaleSweepAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			if (!EvaluateSimpleExpression(radialScale.SweepAngle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SweepAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(radialScale.SweepAngle, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).SweepAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.Interval, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsIntervalOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.IntervalOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.IntervalOffset, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateScaleLabelsAllowUpsideDownExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.AllowUpsideDown, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.AllowUpsideDown, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScaleLabelsDistanceFromScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.DistanceFromScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.DistanceFromScale, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsFontAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.FontAngle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.FontAngle, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateScaleLabelsPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.Placement, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.Placement, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateScaleLabelsRotateLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.RotateLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.RotateLabels, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.RotateLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsShowEndLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.ShowEndLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowEndLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.ShowEndLabels, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.ShowEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.Hidden, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsUseFontPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleLabels.UseFontPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleLabels.UseFontPercent, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScalePinLocationExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin scalePin, string objectName)
		{
			if (!EvaluateSimpleExpression(scalePin.Location, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Location", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scalePin.Location, ref result, scalePin.ExprHost))
					{
						Global.Tracer.Assert(scalePin.ExprHost != null, "(scalePin.ExprHost != null)");
						result.Value = ((ScalePinExprHost)scalePin.ExprHost).LocationExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateScalePinEnableExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin scalePin, string objectName)
		{
			if (!EvaluateSimpleExpression(scalePin.Enable, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Enable", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scalePin.Enable, ref result, scalePin.ExprHost))
					{
						Global.Tracer.Assert(scalePin.ExprHost != null, "(scalePin.ExprHost != null)");
						result.Value = ((ScalePinExprHost)scalePin.ExprHost).EnableExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScaleRangeDistanceFromScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.DistanceFromScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.DistanceFromScale, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleRangeStartWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.StartWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.StartWidth, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.StartWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleRangeEndWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.EndWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EndWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.EndWidth, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.EndWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateScaleRangeInRangeBarPointerColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.InRangeBarPointerColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeBarPointerColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.InRangeBarPointerColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeBarPointerColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateScaleRangeInRangeLabelColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.InRangeLabelColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeLabelColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.InRangeLabelColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeLabelColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateScaleRangeInRangeTickMarksColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.InRangeTickMarksColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeTickMarksColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.InRangeTickMarksColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeTickMarksColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateScaleRangePlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.Placement, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.Placement, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateScaleRangeToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.ToolTip, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateScaleRangeHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.Hidden, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateScaleRangeBackgroundGradientTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			if (!EvaluateSimpleExpression(scaleRange.BackgroundGradientType, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BackgroundGradientType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(scaleRange.BackgroundGradientType, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateThermometerBulbOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			if (!EvaluateSimpleExpression(thermometer.BulbOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BulbOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(thermometer.BulbOffset, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.BulbOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateThermometerBulbSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			if (!EvaluateSimpleExpression(thermometer.BulbSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BulbSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(thermometer.BulbSize, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.BulbSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateThermometerThermometerStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			if (!EvaluateSimpleExpression(thermometer.ThermometerStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ThermometerStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(thermometer.ThermometerStyle, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.ThermometerStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateTickMarkStyleDistanceFromScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.DistanceFromScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.DistanceFromScale, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateTickMarkStylePlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Placement, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Placement, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateTickMarkStyleEnableGradientExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.EnableGradient, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EnableGradient", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.EnableGradient, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.EnableGradientExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateTickMarkStyleGradientDensityExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.GradientDensity, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "GradientDensity", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.GradientDensity, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.GradientDensityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateTickMarkStyleLengthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Length, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Length", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Length, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.LengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateTickMarkStyleWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Width, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateTickMarkStyleShapeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Shape, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Shape", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Shape, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.ShapeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateTickMarkStyleHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			if (!EvaluateSimpleExpression(tickMarkStyle.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(tickMarkStyle.Hidden, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateCustomLabelTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Text, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Text, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(customLabel.Text, ref result);
			return result;
		}

		internal bool EvaluateCustomLabelAllowUpsideDownExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.AllowUpsideDown, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.AllowUpsideDown, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateCustomLabelDistanceFromScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.DistanceFromScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.DistanceFromScale, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateCustomLabelFontAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.FontAngle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.FontAngle, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateCustomLabelPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Placement, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Placement, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateCustomLabelRotateLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.RotateLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.RotateLabel, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.RotateLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateCustomLabelValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Value, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateCustomLabelHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.Hidden, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateCustomLabelUseFontPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(customLabel.UseFontPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(customLabel.UseFontPercent, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateGaugeClipContentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Gauge gauge, string objectName)
		{
			if (!EvaluateSimpleExpression(gauge.ClipContent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ClipContent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gauge.ClipContent, ref result, gauge.ExprHost))
					{
						Global.Tracer.Assert(gauge.ExprHost != null, "(gauge.ExprHost != null)");
						result.Value = ((GaugeExprHost)gauge.ExprHost).ClipContentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeAspectRatioExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Gauge gauge, string objectName)
		{
			if (!EvaluateSimpleExpression(gauge.AspectRatio, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AspectRatio", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gauge.AspectRatio, ref result, gauge.ExprHost))
					{
						Global.Tracer.Assert(gauge.ExprHost != null, "(gauge.ExprHost != null)");
						result.Value = ((GaugeExprHost)gauge.ExprHost).AspectRatioExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateGaugeInputValueValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.Value, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(gaugeInputValue.Value, ref result);
			return result;
		}

		internal string EvaluateGaugeInputValueFormulaExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.Formula, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Formula", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.Formula, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.FormulaExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMinPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.MinPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MinPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.MinPercent, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MinPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMaxPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.MaxPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MaxPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.MaxPercent, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MaxPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMultiplierExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.Multiplier, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.Multiplier, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueAddConstantExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeInputValue.AddConstant, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AddConstant", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeInputValue.AddConstant, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.AddConstantExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateGaugeLabelTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.Text, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.Text, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).TextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(gaugeLabel.Text, ref result);
			return result;
		}

		internal double EvaluateGaugeLabelAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.Angle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.Angle, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugeLabelResizeModeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.ResizeMode, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.ResizeMode, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugeLabelTextShadowOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.TextShadowOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TextShadowOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.TextShadowOffset, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).TextShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateGaugeLabelUseFontPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			if (!EvaluateSimpleExpression(gaugeLabel.UseFontPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(gaugeLabel.UseFontPercent, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateNumericIndicatorDecimalDigitColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.DecimalDigitColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.DecimalDigitColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DecimalDigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateNumericIndicatorDigitColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.DigitColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.DigitColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateNumericIndicatorUseFontPercentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.UseFontPercent, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.UseFontPercent, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal int EvaluateNumericIndicatorDecimalDigitsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.DecimalDigits, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigits", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.DecimalDigits, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DecimalDigitsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal int EvaluateNumericIndicatorDigitsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.Digits, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Digits", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.Digits, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DigitsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal double EvaluateNumericIndicatorMultiplierExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.Multiplier, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.Multiplier, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorNonNumericStringExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.NonNumericString, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "NonNumericString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.NonNumericString, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.NonNumericStringExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorOutOfRangeStringExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.OutOfRangeString, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OutOfRangeString", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.OutOfRangeString, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.OutOfRangeStringExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorResizeModeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ResizeMode, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ResizeMode, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorShowDecimalPointExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ShowDecimalPoint, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowDecimalPoint", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ShowDecimalPoint, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowDecimalPointExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorShowLeadingZerosExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ShowLeadingZeros, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowLeadingZeros", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ShowLeadingZeros, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowLeadingZerosExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateNumericIndicatorIndicatorStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.IndicatorStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.IndicatorStyle, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorShowSignExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.ShowSign, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowSign", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.ShowSign, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowSignExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorSnappingEnabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SnappingEnabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingEnabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SnappingEnabled, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SnappingEnabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateNumericIndicatorSnappingIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SnappingInterval, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SnappingInterval, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SnappingIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorLedDimColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.LedDimColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "LedDimColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.LedDimColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.LedDimColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateNumericIndicatorSeparatorWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SeparatorWidth, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SeparatorWidth", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SeparatorWidth, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SeparatorWidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorSeparatorColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicator.SeparatorColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SeparatorColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicator.SeparatorColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateNumericIndicatorRangeDecimalDigitColorExpression(NumericIndicatorRange numericIndicatorRange, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicatorRange.DecimalDigitColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicatorRange.DecimalDigitColor, ref result, numericIndicatorRange.ExprHost))
					{
						Global.Tracer.Assert(numericIndicatorRange.ExprHost != null, "(numericIndicatorRange.ExprHost != null)");
						result.Value = numericIndicatorRange.ExprHost.DecimalDigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateNumericIndicatorRangeDigitColorExpression(NumericIndicatorRange numericIndicatorRange, string objectName)
		{
			if (!EvaluateSimpleExpression(numericIndicatorRange.DigitColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DigitColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(numericIndicatorRange.DigitColor, ref result, numericIndicatorRange.ExprHost))
					{
						Global.Tracer.Assert(numericIndicatorRange.ExprHost != null, "(numericIndicatorRange.ExprHost != null)");
						result.Value = numericIndicatorRange.ExprHost.DigitColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateIndicatorImageHueColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorImage.HueColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorImage.HueColor, ref result, indicatorImage.ExprHost))
					{
						Global.Tracer.Assert(indicatorImage.ExprHost != null, "(indicatorImage.ExprHost != null)");
						result.Value = indicatorImage.ExprHost.HueColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateIndicatorImageTransparencyExpression(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorImage.Transparency, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorImage.Transparency, ref result, indicatorImage.ExprHost))
					{
						Global.Tracer.Assert(indicatorImage.ExprHost != null, "(indicatorImage.ExprHost != null)");
						result.Value = indicatorImage.ExprHost.TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateStateIndicatorTransformationTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.TransformationType, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransformationType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.TransformationType, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.TransformationTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateStateIndicatorIndicatorStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.IndicatorStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.IndicatorStyle, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateStateIndicatorScaleFactorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.ScaleFactor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ScaleFactor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.ScaleFactor, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateStateIndicatorResizeModeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.ResizeMode, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.ResizeMode, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateStateIndicatorAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			if (!EvaluateSimpleExpression(stateIndicator.Angle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(stateIndicator.Angle, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateIndicatorStateColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorState.Color, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorState.Color, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal double EvaluateIndicatorStateScaleFactorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorState.ScaleFactor, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ScaleFactor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorState.ScaleFactor, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateIndicatorStateIndicatorStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			if (!EvaluateSimpleExpression(indicatorState.IndicatorStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(indicatorState.IndicatorStyle, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapLocationLeftExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLocation.Left, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Left", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLocation.Left, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLocationTopExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLocation.Top, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Top", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLocation.Top, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.TopExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapLocationUnitExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLocation.Unit, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Unit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLocation.Unit, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.UnitExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapSizeWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSize.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSize.Width, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapSizeHeightExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSize.Height, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Height", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSize.Height, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.HeightExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapSizeUnitExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSize.Unit, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Unit", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSize.Unit, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.UnitExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapGridLinesHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.Hidden, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapGridLinesIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.Interval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Interval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.Interval, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateMapGridLinesShowLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.ShowLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.ShowLabels, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.ShowLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapGridLinesLabelPositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			if (!EvaluateSimpleExpression(mapGridLines.LabelPosition, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPosition", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapGridLines.LabelPosition, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.LabelPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapDockableSubItemPositionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.Position, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Position", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDockableSubItem.Position, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.MapPositionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapDockableSubItemDockOutsideViewportExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.DockOutsideViewport, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DockOutsideViewport", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDockableSubItem.DockOutsideViewport, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.DockOutsideViewportExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapDockableSubItemHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDockableSubItem.Hidden, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateMapDockableSubItemToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDockableSubItem.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapDockableSubItem.ToolTip, ref result, mapDockableSubItem.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
					result.Value = mapDockableSubItem.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapSubItemLeftMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.LeftMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LeftMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.LeftMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.LeftMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemRightMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.RightMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "RightMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.RightMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.RightMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemTopMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.TopMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TopMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.TopMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.TopMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemBottomMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.BottomMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BottomMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.BottomMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.BottomMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal int EvaluateMapSubItemZIndexExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSubItem.ZIndex, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ZIndex", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSubItem.ZIndex, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.ZIndexExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateMapBindingFieldPairFieldNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBindingFieldPair.FieldName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "FieldName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBindingFieldPair.FieldName, ref result, mapBindingFieldPair.ExprHost))
					{
						Global.Tracer.Assert(mapBindingFieldPair.ExprHost != null, "(mapBindingFieldPair.ExprHost != null)");
						result.Value = mapBindingFieldPair.ExprHost.FieldNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapBindingFieldPairBindingExpressionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBindingFieldPair.BindingExpression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BindingExpression", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBindingFieldPair.BindingExpression, ref result, null))
					{
						if (mapBindingFieldPair.InElementView)
						{
							Global.Tracer.Assert(mapBindingFieldPair.ExprHost != null, "(mapBindingFieldPair.ExprHost != null)");
							if (m_exprHostInSandboxAppDomain)
							{
								mapBindingFieldPair.ExprHost.SetReportObjectModel(m_reportObjectModel);
							}
							result.Value = mapBindingFieldPair.ExprHost.BindingExpressionExpr;
						}
						else
						{
							Global.Tracer.Assert(mapBindingFieldPair.ExprHostMapMember != null, "(mapBindingFieldPair.ExprHostMapMember != null)");
							if (m_exprHostInSandboxAppDomain)
							{
								mapBindingFieldPair.ExprHostMapMember.SetReportObjectModel(m_reportObjectModel);
							}
							result.Value = mapBindingFieldPair.ExprHostMapMember.BindingExpressionExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapBindingFieldPair.BindingExpression, ref result);
			return result;
		}

		internal string EvaluateMapViewportMapCoordinateSystemExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MapCoordinateSystem, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapCoordinateSystem", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MapCoordinateSystem, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MapCoordinateSystemExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapViewportMapProjectionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MapProjection, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapProjection", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MapProjection, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MapProjectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapViewportProjectionCenterXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.ProjectionCenterX, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ProjectionCenterX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.ProjectionCenterX, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ProjectionCenterXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportProjectionCenterYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.ProjectionCenterY, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ProjectionCenterY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.ProjectionCenterY, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ProjectionCenterYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportMaximumZoomExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MaximumZoom, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MaximumZoom, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MaximumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportMinimumZoomExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.MinimumZoom, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.MinimumZoom, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MinimumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportSimplificationResolutionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.SimplificationResolution, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "SimplificationResolution", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.SimplificationResolution, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.SimplificationResolutionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapViewportContentMarginExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.ContentMargin, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ContentMargin", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.ContentMargin, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ContentMarginExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateMapViewportGridUnderContentExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			if (!EvaluateSimpleExpression(mapViewport.GridUnderContent, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "GridUnderContent", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapViewport.GridUnderContent, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.GridUnderContentExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapLimitsMinimumXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MinimumX, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MinimumX, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MinimumXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMinimumYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MinimumY, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MinimumY, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MinimumYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMaximumXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MaximumX, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MaximumX, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MaximumXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMaximumYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.MaximumY, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.MaximumY, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MaximumYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateMapLimitsLimitToDataExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLimits.LimitToData, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LimitToData", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLimits.LimitToData, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.LimitToDataExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapColorScaleTickMarkLengthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.TickMarkLength, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TickMarkLength", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.TickMarkLength, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.TickMarkLengthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapColorScaleColorBarBorderColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.ColorBarBorderColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ColorBarBorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.ColorBarBorderColor, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.ColorBarBorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal int EvaluateMapColorScaleLabelIntervalExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelInterval, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelInterval", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelInterval, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelIntervalExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelFormatExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelFormat, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelFormat", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelFormat, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelFormatExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelPlacement, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelPlacement, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelBehaviorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.LabelBehavior, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelBehavior", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.LabelBehavior, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelBehaviorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapColorScaleHideEndLabelsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.HideEndLabels, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "HideEndLabels", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.HideEndLabels, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.HideEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapColorScaleRangeGapColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.RangeGapColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "RangeGapColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.RangeGapColor, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.RangeGapColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapColorScaleNoDataTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScale.NoDataText, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "NoDataText", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorScale.NoDataText, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.NoDataTextExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapColorScaleTitleCaptionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorScaleTitle.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Caption", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapColorScaleTitle.Caption, ref result, mapColorScaleTitle.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapColorScaleTitle.ExprHost != null, "(mapColorScaleTitle.ExprHost != null)");
					result.Value = mapColorScaleTitle.ExprHost.CaptionExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapDistanceScaleScaleColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDistanceScale.ScaleColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDistanceScale.ScaleColor, ref result, mapDistanceScale.ExprHost))
					{
						Global.Tracer.Assert(mapDistanceScale.ExprHost != null, "(mapDistanceScale.ExprHost != null)");
						result.Value = mapDistanceScale.ExprHost.ScaleColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapDistanceScaleScaleBorderColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, string objectName)
		{
			if (!EvaluateSimpleExpression(mapDistanceScale.ScaleBorderColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleBorderColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapDistanceScale.ScaleBorderColor, ref result, mapDistanceScale.ExprHost))
					{
						Global.Tracer.Assert(mapDistanceScale.ExprHost != null, "(mapDistanceScale.ExprHost != null)");
						result.Value = mapDistanceScale.ExprHost.ScaleBorderColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal VariantResult EvaluateMapTitleTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTitle.Text, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Text", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapTitle.Text, ref result, mapTitle.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
					result.Value = mapTitle.ExprHost.TextExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal double EvaluateMapTitleAngleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTitle.Angle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Angle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTitle.Angle, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapTitleTextShadowOffsetExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTitle.TextShadowOffset, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextShadowOffset", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTitle.TextShadowOffset, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.TextShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapLegendLayoutExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.Layout, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Layout", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.Layout, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.LayoutExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapLegendAutoFitTextDisabledExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.AutoFitTextDisabled, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "AutoFitTextDisabled", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.AutoFitTextDisabled, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.AutoFitTextDisabledExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapLegendMinFontSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.MinFontSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinFontSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.MinFontSize, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateMapLegendInterlacedRowsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.InterlacedRows, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "InterlacedRows", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.InterlacedRows, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.InterlacedRowsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapLegendInterlacedRowsColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.InterlacedRowsColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "InterlacedRowsColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.InterlacedRowsColor, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.InterlacedRowsColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateMapLegendEquallySpacedItemsExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.EquallySpacedItems, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EquallySpacedItems", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.EquallySpacedItems, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.EquallySpacedItemsExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal int EvaluateMapLegendTextWrapThresholdExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegend.TextWrapThreshold, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextWrapThreshold", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegend.TextWrapThreshold, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.TextWrapThresholdExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal VariantResult EvaluateMapLegendTitleCaptionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegendTitle.Caption, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Caption", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapLegendTitle.Caption, ref result, mapLegendTitle.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
					result.Value = mapLegendTitle.ExprHost.CaptionExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapLegendTitleTitleSeparatorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegendTitle.TitleSeparator, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TitleSeparator", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegendTitle.TitleSeparator, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.TitleSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapLegendTitleTitleSeparatorColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLegendTitle.TitleSeparatorColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TitleSeparatorColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLegendTitle.TitleSeparatorColor, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.TitleSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal VariantResult EvaluateMapAppearanceRuleDataValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.DataValue, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.DataValue, ref result, mapAppearanceRule.ExprHostMapMember))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHostMapMember != null || mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHostMapMember != null) || (mapAppearanceRule.ExprHost != null)");
						if (mapAppearanceRule.ExprHostMapMember != null)
						{
							result.Value = mapAppearanceRule.ExprHostMapMember.DataValueExpr;
						}
						else
						{
							result.Value = mapAppearanceRule.ExprHost.DataValueExpr;
						}
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapAppearanceRule.DataValue, ref result);
			return result;
		}

		internal string EvaluateMapAppearanceRuleDistributionTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.DistributionType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DistributionType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.DistributionType, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.DistributionTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal int EvaluateMapAppearanceRuleBucketCountExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.BucketCount, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BucketCount", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.BucketCount, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.BucketCountExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerResult(result).Value;
		}

		internal VariantResult EvaluateMapAppearanceRuleStartValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.StartValue, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.StartValue, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.StartValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapAppearanceRule.StartValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapAppearanceRuleEndValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.EndValue, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapAppearanceRule.EndValue, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.EndValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapAppearanceRule.EndValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapAppearanceRuleLegendTextExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapAppearanceRule.LegendText, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LegendText", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapAppearanceRule.LegendText, ref result, mapAppearanceRule.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
					result.Value = mapAppearanceRule.ExprHost.LegendTextExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapBucketStartValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBucket.StartValue, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBucket.StartValue, ref result, mapBucket.ExprHost))
					{
						Global.Tracer.Assert(mapBucket.ExprHost != null, "(mapBucket.ExprHost != null)");
						result.Value = mapBucket.ExprHost.StartValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapBucket.StartValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapBucketEndValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBucket.EndValue, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndValue", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBucket.EndValue, ref result, mapBucket.ExprHost))
					{
						Global.Tracer.Assert(mapBucket.ExprHost != null, "(mapBucket.ExprHost != null)");
						result.Value = mapBucket.ExprHost.EndValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapBucket.EndValue, ref result);
			return result;
		}

		internal string EvaluateMapColorPaletteRulePaletteExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorPaletteRule.Palette, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Palette", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorPaletteRule.Palette, ref result, mapColorPaletteRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorPaletteRule.ExprHost != null, "(mapColorPaletteRule.ExprHost != null)");
						result.Value = mapColorPaletteRule.ExprHost.PaletteExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorRangeRuleStartColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRangeRule.StartColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRangeRule.StartColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.StartColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapColorRangeRuleMiddleColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRangeRule.MiddleColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MiddleColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRangeRule.MiddleColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.MiddleColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapColorRangeRuleEndColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRangeRule.EndColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRangeRule.EndColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.EndColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal bool EvaluateMapColorRuleShowInColorScaleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapColorRule.ShowInColorScale, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowInColorScale", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapColorRule.ShowInColorScale, ref result, mapColorRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRule.ExprHost != null, "(mapColorRule.ExprHost != null)");
						result.Value = mapColorRule.ExprHost.ShowInColorScaleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapSizeRuleStartSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSizeRule.StartSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSizeRule.StartSize, ref result, mapSizeRule.ExprHost))
					{
						Global.Tracer.Assert(mapSizeRule.ExprHost != null, "(mapSizeRule.ExprHost != null)");
						result.Value = mapSizeRule.ExprHost.StartSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSizeRuleEndSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSizeRule.EndSize, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndSize", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSizeRule.EndSize, ref result, mapSizeRule.ExprHost))
					{
						Global.Tracer.Assert(mapSizeRule.ExprHost != null, "(mapSizeRule.ExprHost != null)");
						result.Value = mapSizeRule.ExprHost.EndSizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapMarkerImageSourceExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.Source, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Source, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapMarkerImageValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapMarkerImage.Value, ref result);
			return result;
		}

		internal string EvaluateMapMarkerImageStringValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
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

		internal byte[] EvaluateMapMarkerImageBinaryValueExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName, out bool errorOccurred)
		{
			if (!EvaluateBinaryExpression(mapMarkerImage.Value, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
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

		internal string EvaluateMapMarkerImageMIMETypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.MIMEType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MIMEType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.MIMEType, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapMarkerImageTransparentColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.TransparentColor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TransparentColor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.TransparentColor, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapMarkerImageResizeModeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarkerImage.ResizeMode, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ResizeMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarkerImage.ResizeMode, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapMarkerMapMarkerStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker mapMarker, string objectName)
		{
			if (!EvaluateSimpleExpression(mapMarker.MapMarkerStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapMarkerStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapMarker.MapMarkerStyle, ref result, mapMarker.ExprHost))
					{
						Global.Tracer.Assert(mapMarker.ExprHost != null, "(mapMarker.ExprHost != null)");
						result.Value = mapMarker.ExprHost.MapMarkerStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapCustomColorColorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor, string objectName)
		{
			if (!EvaluateSimpleExpression(mapCustomColor.Color, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Color", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapCustomColor.Color, ref result, mapCustomColor.ExprHost))
					{
						Global.Tracer.Assert(mapCustomColor.ExprHost != null, "(mapCustomColor.ExprHost != null)");
						result.Value = mapCustomColor.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(ProcessStringResult(result).Value, this, allowTransparency: true);
		}

		internal string EvaluateMapLineTemplateWidthExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLineTemplate.Width, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Width", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLineTemplate.Width, ref result, mapLineTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapLineTemplate.ExprHost != null, "(mapLineTemplate.ExprHost != null)");
						result.Value = mapLineTemplate.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapLineTemplateLabelPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLineTemplate.LabelPlacement, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLineTemplate.LabelPlacement, ref result, mapLineTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapLineTemplate.ExprHost != null, "(mapLineTemplate.ExprHost != null)");
						result.Value = mapLineTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateScaleFactorExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.ScaleFactor, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleFactor", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.ScaleFactor, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateCenterPointOffsetXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.CenterPointOffsetX, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterPointOffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.CenterPointOffsetX, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.CenterPointOffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateCenterPointOffsetYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.CenterPointOffsetY, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterPointOffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.CenterPointOffsetY, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.CenterPointOffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapPolygonTemplateShowLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.ShowLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowLabel", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.ShowLabel, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.ShowLabelExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapPolygonTemplateLabelPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygonTemplate.LabelPlacement, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygonTemplate.LabelPlacement, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapLabelTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapSpatialElementTemplateHiddenExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.Hidden, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialElementTemplate.Hidden, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapSpatialElementTemplateOffsetXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.OffsetX, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "OffsetX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialElementTemplate.OffsetX, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapSpatialElementTemplateOffsetYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.OffsetY, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "OffsetY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialElementTemplate.OffsetY, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.Label, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Label", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapSpatialElementTemplate.Label, ref result, mapSpatialElementTemplate.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
					result.Value = mapSpatialElementTemplate.ExprHost.LabelExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateDataElementLabelExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.DataElementLabel, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataElementLabel", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapSpatialElementTemplate.DataElementLabel, ref result, mapSpatialElementTemplate.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
					result.Value = mapSpatialElementTemplate.ExprHost.DataElementLabelExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateToolTipExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialElementTemplate.ToolTip, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ToolTip", out VariantResult result))
			{
				try
				{
					if (EvaluateComplexExpression(mapSpatialElementTemplate.ToolTip, ref result, mapSpatialElementTemplate.ExprHost))
					{
						return result;
					}
					Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
					result.Value = mapSpatialElementTemplate.ExprHost.ToolTipExpr;
					return result;
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapPointTemplateSizeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPointTemplate.Size, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Size", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPointTemplate.Size, ref result, mapPointTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPointTemplate.ExprHost != null, "(mapPointTemplate.ExprHost != null)");
						result.Value = mapPointTemplate.ExprHost.SizeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapPointTemplateLabelPlacementExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPointTemplate.LabelPlacement, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPointTemplate.LabelPlacement, ref result, mapPointTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPointTemplate.ExprHost != null, "(mapPointTemplate.ExprHost != null)");
						result.Value = mapPointTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapLineUseCustomLineTemplateExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLine mapLine, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLine.UseCustomLineTemplate, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomLineTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLine.UseCustomLineTemplate, ref result, mapLine.ExprHost))
					{
						Global.Tracer.Assert(mapLine.ExprHost != null, "(mapLine.ExprHost != null)");
						result.Value = mapLine.ExprHost.UseCustomLineTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPolygonUseCustomPolygonTemplateExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygon.UseCustomPolygonTemplate, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomPolygonTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygon.UseCustomPolygonTemplate, ref result, mapPolygon.ExprHost))
					{
						Global.Tracer.Assert(mapPolygon.ExprHost != null, "(mapPolygon.ExprHost != null)");
						result.Value = mapPolygon.ExprHost.UseCustomPolygonTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPolygonUseCustomPointTemplateExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPolygon.UseCustomCenterPointTemplate, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomCenterPointTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPolygon.UseCustomCenterPointTemplate, ref result, mapPolygon.ExprHost))
					{
						Global.Tracer.Assert(mapPolygon.ExprHost != null, "(mapPolygon.ExprHost != null)");
						result.Value = mapPolygon.ExprHost.UseCustomPointTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPointUseCustomPointTemplateExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint mapPoint, string objectName)
		{
			if (!EvaluateSimpleExpression(mapPoint.UseCustomPointTemplate, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomPointTemplate", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapPoint.UseCustomPointTemplate, ref result, mapPoint.ExprHost))
					{
						Global.Tracer.Assert(mapPoint.ExprHost != null, "(mapPoint.ExprHost != null)");
						result.Value = mapPoint.ExprHost.UseCustomPointTemplateExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapFieldNameNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName mapFieldName, string objectName)
		{
			if (!EvaluateSimpleExpression(mapFieldName.Name, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Name", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapFieldName.Name, ref result, mapFieldName.ExprHost))
					{
						Global.Tracer.Assert(mapFieldName.ExprHost != null, "(mapFieldName.ExprHost != null)");
						result.Value = mapFieldName.ExprHost.NameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapLayerVisibilityModeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.VisibilityMode, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "VisibilityMode", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.VisibilityMode, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.VisibilityModeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapLayerMinimumZoomExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.MinimumZoom, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.MinimumZoom, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.MinimumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLayerMaximumZoomExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.MaximumZoom, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumZoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.MaximumZoom, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.MaximumZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLayerTransparencyExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapLayer.Transparency, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Transparency", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapLayer.Transparency, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapShapefileSourceExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile, string objectName)
		{
			if (!EvaluateSimpleExpression(mapShapefile.Source, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Source", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapShapefile.Source, ref result, mapShapefile.ExprHost))
					{
						Global.Tracer.Assert(mapShapefile.ExprHost != null, "(mapShapefile.ExprHost != null)");
						result.Value = mapShapefile.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapSpatialDataRegionVectorDataExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialDataRegion.VectorData, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "VectorData", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialDataRegion.VectorData, ref result, mapSpatialDataRegion.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataRegion.ExprHost != null, "(mapSpatialDataRegion.ExprHost != null)");
						result.Value = mapSpatialDataRegion.ExprHost.VectorDataExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			ProcessVariantResult(mapSpatialDataRegion.VectorData, ref result);
			return result;
		}

		internal string EvaluateMapSpatialDataSetDataSetNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialDataSet.DataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataSetName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialDataSet.DataSetName, ref result, mapSpatialDataSet.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataSet.ExprHost != null, "(mapSpatialDataSet.ExprHost != null)");
						result.Value = mapSpatialDataSet.ExprHost.DataSetNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapSpatialDataSetSpatialFieldExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, string objectName)
		{
			if (!EvaluateSimpleExpression(mapSpatialDataSet.SpatialField, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "SpatialField", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapSpatialDataSet.SpatialField, ref result, mapSpatialDataSet.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataSet.ExprHost != null, "(mapSpatialDataSet.ExprHost != null)");
						result.Value = mapSpatialDataSet.ExprHost.SpatialFieldExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTileLayerServiceUrlExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTileLayer.ServiceUrl, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ServiceUrl", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTileLayer.ServiceUrl, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.ServiceUrlExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTileLayerTileStyleExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTileLayer.TileStyle, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TileStyle", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTileLayer.TileStyle, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.TileStyleExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapTileLayerUseSecureConnectionExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			if (!EvaluateSimpleExpression(mapTileLayer.UseSecureConnection, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseSecureConnection", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapTileLayer.UseSecureConnection, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.UseSecureConnectionExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapAntiAliasingExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.AntiAliasing, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "AntiAliasing", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.AntiAliasing, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.AntiAliasingExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTextAntiAliasingQualityExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.TextAntiAliasingQuality, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextAntiAliasingQuality", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.TextAntiAliasingQuality, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.TextAntiAliasingQualityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapShadowIntensityExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.ShadowIntensity, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShadowIntensity", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.ShadowIntensity, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.ShadowIntensityExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapTileLanguageExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			if (!EvaluateSimpleExpression(map.TileLanguage, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TileLanguage", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(map.TileLanguage, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.TileLanguageExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal string EvaluateMapBorderSkinMapBorderSkinTypeExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin, string objectName)
		{
			if (!EvaluateSimpleExpression(mapBorderSkin.MapBorderSkinType, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapBorderSkinType", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapBorderSkin.MapBorderSkinType, ref result, mapBorderSkin.ExprHost))
					{
						Global.Tracer.Assert(mapBorderSkin.ExprHost != null, "(mapBorderSkin.ExprHost != null)");
						result.Value = mapBorderSkin.ExprHost.MapBorderSkinTypeExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapCustomViewCenterXExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapCustomView.CenterX, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterX", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapCustomView.CenterX, ref result, mapCustomView.ExprHost))
					{
						Global.Tracer.Assert(mapCustomView.ExprHost != null, "(mapCustomView.ExprHost != null)");
						result.Value = mapCustomView.ExprHost.CenterXExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapCustomViewCenterYExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapCustomView.CenterY, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterY", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapCustomView.CenterY, ref result, mapCustomView.ExprHost))
					{
						Global.Tracer.Assert(mapCustomView.ExprHost != null, "(mapCustomView.ExprHost != null)");
						result.Value = mapCustomView.ExprHost.CenterYExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapElementViewLayerNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView mapElementView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapElementView.LayerName, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LayerName", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapElementView.LayerName, ref result, mapElementView.ExprHost))
					{
						Global.Tracer.Assert(mapElementView.ExprHost != null, "(mapElementView.ExprHost != null)");
						result.Value = mapElementView.ExprHost.LayerNameExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessStringResult(result).Value;
		}

		internal double EvaluateMapViewZoomExpression(Microsoft.ReportingServices.ReportIntermediateFormat.MapView mapView, string objectName)
		{
			if (!EvaluateSimpleExpression(mapView.Zoom, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Zoom", out VariantResult result))
			{
				try
				{
					if (!EvaluateComplexExpression(mapView.Zoom, ref result, mapView.ExprHost))
					{
						Global.Tracer.Assert(mapView.ExprHost != null, "(mapView.ExprHost != null)");
						result.Value = mapView.ExprHost.ZoomExpr;
					}
				}
				catch (Exception e)
				{
					RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapPageNameExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				if (!EvaluateSimpleExpression(expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "PageName", out VariantResult result))
				{
					try
					{
						if (!EvaluateComplexExpression(expression, ref result, map.ExprHost))
						{
							Global.Tracer.Assert(map.ExprHost != null, "(map.ExprHost != null)");
							result.Value = map.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return ProcessStringResult(result, autocast: true).Value;
			}
			finally
			{
				m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}
	}
}
