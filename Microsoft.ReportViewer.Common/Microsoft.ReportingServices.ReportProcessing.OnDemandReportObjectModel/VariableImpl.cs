using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class VariableImpl : Variable
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Variable m_variableDef;

		private IndexedExprHost m_exprHost;

		private ObjectType m_parentObjectType;

		private string m_parentObjectName;

		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IScope m_scope;

		private object m_value;

		private Microsoft.ReportingServices.RdlExpressions.VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		private int m_indexInCollection;

		public override object Value
		{
			get
			{
				if (!m_isValueReady)
				{
					if (m_reportRT.ReportObjectModel.OdpContext.IsTablixProcessingMode || m_reportRT.VariableReferenceMode)
					{
						return GetResult(fromValue: true);
					}
					return null;
				}
				if (!VariableInScope)
				{
					return null;
				}
				return m_value;
			}
			set
			{
				SetValue(value, internalSet: false);
			}
		}

		public override bool Writable => m_variableDef.Writable;

		internal IScope Scope
		{
			set
			{
				m_scope = value;
			}
		}

		internal string Name
		{
			get
			{
				return m_variableDef.Name;
			}
			set
			{
				m_variableDef.Name = value;
			}
		}

		private bool VariableInScope
		{
			get
			{
				if (!IsReportVariable)
				{
					IRIFReportScope iRIFReportScope = null;
					if (m_reportRT.ReportObjectModel.OdpContext.IsTablixProcessingMode || m_reportRT.VariableReferenceMode)
					{
						if (m_reportRT.CurrentScope != null)
						{
							iRIFReportScope = m_reportRT.CurrentScope.RIFReportScope;
						}
					}
					else
					{
						IReportScope currentReportScope = m_reportRT.ReportObjectModel.OdpContext.CurrentReportScope;
						if (currentReportScope != null)
						{
							iRIFReportScope = currentReportScope.RIFReportScope;
						}
					}
					if (iRIFReportScope == null || !iRIFReportScope.VariableInScope(m_variableDef.SequenceID))
					{
						return false;
					}
				}
				return true;
			}
		}

		private bool IsReportVariable => m_parentObjectType == ObjectType.Report;

		internal VariableImpl(Microsoft.ReportingServices.ReportIntermediateFormat.Variable variable, IndexedExprHost variableValuesHost, ObjectType parentObjectType, string parentObjectName, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT, int indexInCollection)
		{
			Global.Tracer.Assert(reportRT != null && variable != null, "(null != reportRT && null != variable)");
			m_variableDef = variable;
			m_exprHost = variableValuesHost;
			m_parentObjectType = parentObjectType;
			m_parentObjectName = parentObjectName;
			m_reportRT = reportRT;
			m_indexInCollection = indexInCollection;
		}

		internal void SetResult(Microsoft.ReportingServices.RdlExpressions.VariantResult result)
		{
			m_result = result;
			m_isValueReady = true;
		}

		public override bool SetValue(object value)
		{
			SetValue(value, internalSet: false, out bool succeeded);
			return succeeded;
		}

		internal void SetValue(object value, bool internalSet)
		{
			SetValue(value, internalSet, out bool _);
		}

		private void SetValue(object value, bool internalSet, out bool succeeded)
		{
			succeeded = false;
			if (!internalSet)
			{
				if (!IsReportVariable || !m_variableDef.Writable)
				{
					return;
				}
				m_result = new Microsoft.ReportingServices.RdlExpressions.VariantResult(errorOccurred: false, value);
				bool flag = m_reportRT.ProcessSerializableResult(isReportScope: true, ref m_result);
				if (m_result.ErrorOccurred)
				{
					if (flag)
					{
						((IErrorContext)m_reportRT).Register(ProcessingErrorCode.rsVariableTypeNotSerializable, Severity.Error, m_parentObjectType, m_parentObjectName, m_variableDef.GetPropertyName(), Array.Empty<string>());
					}
				}
				else
				{
					m_reportRT.ReportObjectModel.OdpContext.StoreUpdatedVariableValue(m_indexInCollection, value);
					succeeded = true;
					m_value = value;
					m_isValueReady = true;
				}
			}
			else
			{
				succeeded = true;
				m_value = value;
				m_isValueReady = true;
			}
		}

		internal void Reset()
		{
			m_isValueReady = false;
		}

		internal object GetResult()
		{
			return GetResult(fromValue: false);
		}

		private object GetResult(bool fromValue)
		{
			if (fromValue && !VariableInScope)
			{
				return null;
			}
			if (!m_isValueReady)
			{
				if (m_isVisited)
				{
					ProcessingErrorCode code = IsReportVariable ? ProcessingErrorCode.rsCyclicExpressionInReportVariable : ProcessingErrorCode.rsCyclicExpressionInGroupVariable;
					((IErrorContext)m_reportRT).Register(code, Severity.Error, m_parentObjectType, m_parentObjectName, m_variableDef.GetPropertyName(), Array.Empty<string>());
					throw new ReportProcessingException(m_reportRT.RuntimeErrorContext.Messages);
				}
				m_isVisited = true;
				bool variableReferenceMode = m_reportRT.VariableReferenceMode;
				ObjectType objectType = m_reportRT.ObjectType;
				string objectName = m_reportRT.ObjectName;
				string propertyName = m_reportRT.PropertyName;
				bool unfulfilledDependency = m_reportRT.UnfulfilledDependency;
				IScope currentScope = m_reportRT.CurrentScope;
				m_reportRT.VariableReferenceMode = true;
				m_reportRT.UnfulfilledDependency = false;
				m_result = m_reportRT.EvaluateVariableValueExpression(m_variableDef, m_exprHost, m_parentObjectType, m_parentObjectName, IsReportVariable);
				bool unfulfilledDependency2 = m_reportRT.UnfulfilledDependency;
				m_reportRT.UnfulfilledDependency |= unfulfilledDependency;
				m_reportRT.VariableReferenceMode = variableReferenceMode;
				m_reportRT.CurrentScope = currentScope;
				m_reportRT.ObjectType = objectType;
				m_reportRT.ObjectName = objectName;
				m_reportRT.PropertyName = propertyName;
				if (m_result.ErrorOccurred)
				{
					throw new ReportProcessingException(m_reportRT.RuntimeErrorContext.Messages);
				}
				if (unfulfilledDependency2 && fromValue)
				{
					m_value = null;
					m_isValueReady = false;
				}
				else
				{
					m_value = m_result.Value;
					m_isValueReady = true;
				}
				m_isVisited = false;
			}
			return m_value;
		}
	}
}
