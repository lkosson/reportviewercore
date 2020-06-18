using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class SharedDataSetProcessReportParameters : ProcessReportParameters
	{
		private DataSetCore m_dataSetCore;

		internal override bool IsReportParameterProcessing => false;

		internal SharedDataSetProcessReportParameters(DataSetCore dataSetCore, OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
			m_dataSetCore = dataSetCore;
			if (odpContext.IsRdlSandboxingEnabled())
			{
				IRdlSandboxConfig rdlSandboxing = odpContext.Configuration.RdlSandboxing;
				m_maxStringResultLength = rdlSandboxing.MaxStringResultLength;
			}
		}

		internal OnDemandProcessingContext GetOnDemandContext()
		{
			return (OnDemandProcessingContext)base.ProcessingContext;
		}

		internal override IParameterDef GetParameterDef(int aParamIndex)
		{
			return m_dataSetCore.Query.Parameters[aParamIndex] as DataSetParameterValue;
		}

		protected override void AssertAreSameParameterByName(ParameterInfo paramInfo, IParameterDef paramDef)
		{
			DataSetParameterValue dataSetParameterValue = (DataSetParameterValue)paramDef;
			Global.Tracer.Assert(string.Compare(paramInfo.Name, dataSetParameterValue.UniqueName, StringComparison.OrdinalIgnoreCase) == 0, "paramInfo.Name == dataSetParamDef.UniqueName, parameter {0}", paramInfo.Name.MarkAsPrivate());
		}

		protected override string ApplySandboxStringRestriction(string value, string paramName, string propertyName)
		{
			return ProcessReportParameters.ApplySandboxRestriction(ref value, paramName, propertyName, GetOnDemandContext(), m_maxStringResultLength);
		}

		internal override void InitParametersContext(ParameterInfoCollection parameters)
		{
			OnDemandProcessingContext onDemandContext = GetOnDemandContext();
			Global.Tracer.Assert(onDemandContext.ReportObjectModel != null && onDemandContext.ReportRuntime != null);
			if (onDemandContext.ReportRuntime.ReportExprHost != null)
			{
				m_dataSetCore.SetExprHost(onDemandContext.ReportRuntime.ReportExprHost, onDemandContext.ReportObjectModel);
			}
		}

		internal override void Cleanup()
		{
		}

		internal override void AddToRuntime(ParameterInfo aParamInfo)
		{
			ParameterImpl parameter = new ParameterImpl(aParamInfo);
			GetOnDemandContext().ReportObjectModel.ParametersImpl.Add(aParamInfo.Name, parameter);
		}

		internal override void SetupExprHost(IParameterDef aParamDef)
		{
		}

		internal override object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex)
		{
			return (aParamDef as DataSetParameterValue).EvaluateQueryParameterValue(GetOnDemandContext(), m_dataSetCore.ExprHost);
		}

		internal override object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex)
		{
			throw new NotSupportedException();
		}

		internal override object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex)
		{
			throw new NotSupportedException();
		}

		internal override string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef)
		{
			throw new NotSupportedException();
		}

		internal override bool NeedPrompt(IParameterDataSource paramDS)
		{
			return false;
		}

		internal override void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName)
		{
			throw new NotSupportedException();
		}

		internal override ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues)
		{
			throw new NotSupportedException();
		}
	}
}
