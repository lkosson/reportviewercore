using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TagInstance
	{
		private readonly Tag m_tagDef;

		private VariantResult? m_tag;

		public TypeCode DataType
		{
			get
			{
				EvaluateTagValue();
				return m_tag.Value.TypeCode;
			}
		}

		public object Value
		{
			get
			{
				EvaluateTagValue();
				return m_tag.Value.Value;
			}
		}

		internal TagInstance(Tag tagDef)
		{
			m_tagDef = tagDef;
		}

		private void EvaluateTagValue()
		{
			if (m_tag.HasValue)
			{
				return;
			}
			ExpressionInfo expression = m_tagDef.Expression;
			if (expression != null)
			{
				if (expression.IsExpression)
				{
					Image image = m_tagDef.Image;
					m_tag = image.ImageDef.EvaluateTagExpression(expression, image.Instance.ReportScopeInstance, image.RenderingContext.OdpContext);
				}
				else
				{
					VariantResult result = new VariantResult(errorOccurred: false, expression.Value);
					ReportRuntime.SetVariantType(ref result);
					m_tag = result;
				}
			}
			else
			{
				m_tag = new VariantResult(errorOccurred: false, null);
			}
		}

		internal void ResetInstanceCache()
		{
			m_tag = null;
		}
	}
}
