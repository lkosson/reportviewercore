using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class LegacyProcessReportParameters : ProcessReportParameters
	{
		private Report m_report;

		private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.RuntimeDataSourceNode m_runtimeDataSourceNode;

		internal LegacyProcessReportParameters(Report aReport, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext aContext)
			: base(aContext)
		{
			m_report = aReport;
		}

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext GetLegacyContext()
		{
			return (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext)base.ProcessingContext;
		}

		internal override IParameterDef GetParameterDef(int aParamIndex)
		{
			return m_report.Parameters[aParamIndex];
		}

		internal override void InitParametersContext(ParameterInfoCollection parameters)
		{
			int dataSourceCount = m_report.DataSourceCount;
			Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = GetLegacyContext();
			Global.Tracer.Assert(legacyContext.ReportObjectModel == null, "(null == processingContext.ReportObjectModel)");
			legacyContext.ReportObjectModel = new ObjectModelImpl(legacyContext);
			legacyContext.ReportObjectModel.ParametersImpl = new ParametersImpl(parameters.Count);
			legacyContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
			legacyContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
			legacyContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(null);
			legacyContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(legacyContext.ReportContext.ItemName, legacyContext.ExecutionTime, legacyContext.ReportContext.HostRootUri, legacyContext.ReportContext.ParentPath);
			legacyContext.ReportObjectModel.UserImpl = new UserImpl(legacyContext.RequestUserName, legacyContext.UserLanguage.Name, legacyContext.AllowUserProfileState);
			legacyContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
			legacyContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(dataSourceCount);
			if (legacyContext.ReportRuntime == null)
			{
				legacyContext.ReportRuntime = new ReportRuntime(legacyContext.ReportObjectModel, legacyContext.ErrorContext);
				legacyContext.ReportRuntime.LoadCompiledCode(m_report, parametersOnly: true, legacyContext.ReportObjectModel, legacyContext.ReportRuntimeSetup);
			}
		}

		internal override void Cleanup()
		{
			Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = GetLegacyContext();
			if (legacyContext.ReportRuntime != null)
			{
				legacyContext.ReportRuntime.Close();
			}
		}

		internal override void AddToRuntime(ParameterInfo aParamInfo)
		{
			ParameterImpl parameter = new ParameterImpl(aParamInfo.Values, aParamInfo.Labels, aParamInfo.MultiValue);
			GetLegacyContext().ReportObjectModel.ParametersImpl.Add(aParamInfo.Name, parameter);
		}

		internal override void SetupExprHost(IParameterDef aParamDef)
		{
			Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = GetLegacyContext();
			if (legacyContext.ReportRuntime.ReportExprHost != null)
			{
				((ParameterDef)aParamDef).SetExprHost(legacyContext.ReportRuntime.ReportExprHost, legacyContext.ReportObjectModel);
			}
		}

		internal override string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef)
		{
			Global.Tracer.Assert(condition: false);
			return null;
		}

		internal override object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex)
		{
			VariantResult variantResult = GetLegacyContext().ReportRuntime.EvaluateParamDefaultValue((ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex)
		{
			VariantResult variantResult = GetLegacyContext().ReportRuntime.EvaluateParamValidValue((ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex)
		{
			VariantResult variantResult = GetLegacyContext().ReportRuntime.EvaluateParamValidValueLabel((ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override bool NeedPrompt(IParameterDataSource paramDS)
		{
			bool result = false;
			Microsoft.ReportingServices.ReportProcessing.DataSource dataSource = m_report.DataSources[paramDS.DataSourceIndex];
			if (GetLegacyContext().DataSourceInfos != null)
			{
				DataSourceInfo byID = GetLegacyContext().DataSourceInfos.GetByID(dataSource.ID);
				if (byID != null)
				{
					result = byID.NeedPrompt;
				}
			}
			return result;
		}

		internal override void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName)
		{
			Microsoft.ReportingServices.ReportProcessing.DataSet dataSet = m_report.DataSources[aDataSourceIndex].DataSets[aDataSetIndex];
			throw new ReportProcessingException(ErrorCode.rsReportParameterQueryProcessingError, aParamName.MarkAsPrivate(), propertyName, dataSet.Fields[aFieldIndex].Name.MarkAsModelInfo(), dataSet.Name.MarkAsPrivate(), ReportRuntime.GetErrorName(aError.Status, aError.Message));
		}

		internal override ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues)
		{
			EventHandler eventHandler = null;
			LegacyReportParameterDataSetCache legacyReportParameterDataSetCache = new LegacyReportParameterDataSetCache(this, aParam, (ParameterDef)aParamDef, aRetrieveValidValues, aRetrievalDefaultValues);
			Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = GetLegacyContext();
			try
			{
				m_runtimeDataSourceNode = new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportRuntimeDataSourceNode(m_report, m_report.DataSources[paramDS.DataSourceIndex], paramDS.DataSetIndex, legacyContext, legacyReportParameterDataSetCache);
				eventHandler = AbortHandler;
				legacyContext.AbortInfo.ProcessingAbortEvent += eventHandler;
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Abort handler registered.");
				}
				m_runtimeDataSourceNode.InitProcessingParams(mergeTran: false, prefetchDataOnly: true);
				m_runtimeDataSourceNode.ProcessConcurrent(null);
				legacyContext.CheckAndThrowIfAborted();
				_ = m_runtimeDataSourceNode.RuntimeDataSetNodes[0];
				return legacyReportParameterDataSetCache;
			}
			finally
			{
				if (eventHandler != null)
				{
					legacyContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
				}
				if (m_runtimeDataSourceNode != null)
				{
					m_runtimeDataSourceNode.Cleanup();
				}
			}
		}

		private void AbortHandler(object sender, EventArgs e)
		{
			if (Global.Tracer.TraceInfo)
			{
				Global.Tracer.Trace(TraceLevel.Info, "Merge abort handler called. Aborting data sources ...");
			}
			m_runtimeDataSourceNode.Abort();
		}

		protected override string ApplySandboxStringRestriction(string value, string paramName, string propertyName)
		{
			return value;
		}
	}
}
