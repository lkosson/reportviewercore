using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomProperty
	{
		private ReportStringProperty m_name;

		private ReportVariantProperty m_value;

		private CustomPropertyInstance m_instance;

		private RenderingContext m_renderingContext;

		private ReportElement m_reportElementOwner;

		public ReportStringProperty Name => m_name;

		public ReportVariantProperty Value => m_value;

		internal ReportElement ReportElementOwner => m_reportElementOwner;

		public CustomPropertyInstance Instance
		{
			get
			{
				if (m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return m_instance;
			}
		}

		internal CustomProperty(ReportElement reportElementOwner, RenderingContext renderingContext, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo nameExpr, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo valueExpr, string name, object value, TypeCode typeCode)
		{
			m_reportElementOwner = reportElementOwner;
			Init(nameExpr, valueExpr, name, value, typeCode);
			m_renderingContext = renderingContext;
		}

		internal CustomProperty(RenderingContext renderingContext, Microsoft.ReportingServices.ReportProcessing.ExpressionInfo nameExpr, Microsoft.ReportingServices.ReportProcessing.ExpressionInfo valueExpr, string name, object value, TypeCode typeCode)
		{
			m_name = new ReportStringProperty(nameExpr);
			m_value = new ReportVariantProperty(valueExpr);
			if (nameExpr.IsExpression || valueExpr.IsExpression)
			{
				m_instance = new CustomPropertyInstance(this, name, value, typeCode);
			}
			m_renderingContext = renderingContext;
		}

		private void Init(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo nameExpr, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo valueExpr, string name, object value, TypeCode typeCode)
		{
			m_name = new ReportStringProperty(nameExpr);
			m_value = new ReportVariantProperty(valueExpr);
			if (nameExpr.IsExpression || valueExpr.IsExpression)
			{
				m_instance = new CustomPropertyInstance(this, name, value, typeCode);
			}
			else
			{
				m_instance = null;
			}
		}

		internal void Update(string name, object value, TypeCode typeCode)
		{
			if (m_instance != null)
			{
				m_instance.Update(name, value, typeCode);
			}
		}

		internal void ConstructCustomPropertyDefinition(Microsoft.ReportingServices.ReportIntermediateFormat.DataValue dataValueDef)
		{
			Global.Tracer.Assert(m_reportElementOwner != null && m_instance != null, "m_reportElementOwner != null && m_instance != null");
			if (m_instance.Name != null)
			{
				dataValueDef.Name = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(m_instance.Name);
			}
			else
			{
				dataValueDef.Name = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			if (m_instance.Value != null)
			{
				dataValueDef.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((string)m_instance.Value);
			}
			else
			{
				dataValueDef.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			Init(dataValueDef.Name, dataValueDef.Value, m_instance.Name, m_instance.Value, m_instance.TypeCode);
		}
	}
}
