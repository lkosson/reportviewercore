using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTextRunInstance : TextRunInstance
	{
		private string m_toolTip;

		private MarkupType? m_markupType;

		private bool m_formattedValueEvaluated;

		private string m_formattedValue;

		private VariantResult m_originalValue;

		private bool m_originalValueEvaluated;

		private bool m_originalValueNeedsReset;

		public override string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					m_uniqueName = InstancePathItem.GenerateUniqueNameString(TextRunDef.IDString, TextRunDef.InstancePath);
				}
				return m_uniqueName;
			}
		}

		public override string Value
		{
			get
			{
				if (!m_formattedValueEvaluated)
				{
					m_formattedValueEvaluated = true;
					EvaluateOriginalValue();
					if (m_originalValue.TypeCode == TypeCode.String)
					{
						m_formattedValue = (m_originalValue.Value as string);
					}
					else
					{
						m_formattedValue = TextRunDef.FormatTextRunValue(m_originalValue, base.ReportElementDef.RenderingContext.OdpContext);
					}
				}
				return m_formattedValue;
			}
		}

		public override object OriginalValue
		{
			get
			{
				EvaluateOriginalValue();
				if (IsDateTimeOffsetOrTimeSpan())
				{
					if (Value != null)
					{
						return Value;
					}
					return ReportRuntime.ConvertToStringFallBack(m_originalValue.Value);
				}
				return m_originalValue.Value;
			}
		}

		public override string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					ExpressionInfo toolTip = TextRunDef.ToolTip;
					if (toolTip != null)
					{
						if (toolTip.IsExpression)
						{
							m_toolTip = TextRunDef.EvaluateToolTip(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
						}
						else
						{
							m_toolTip = toolTip.StringValue;
						}
					}
				}
				return m_toolTip;
			}
		}

		public override MarkupType MarkupType
		{
			get
			{
				if (!m_markupType.HasValue)
				{
					ExpressionInfo markupType = TextRunDef.MarkupType;
					if (markupType != null)
					{
						if (markupType.IsExpression)
						{
							m_markupType = RichTextHelpers.TranslateMarkupType(TextRunDef.EvaluateMarkupType(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext));
						}
						else
						{
							m_markupType = RichTextHelpers.TranslateMarkupType(markupType.StringValue);
						}
					}
					else
					{
						m_markupType = MarkupType.None;
					}
				}
				return m_markupType.Value;
			}
		}

		public override TypeCode TypeCode
		{
			get
			{
				EvaluateOriginalValue();
				if (IsDateTimeOffsetOrTimeSpan())
				{
					return TypeCode.String;
				}
				return m_originalValue.TypeCode;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.TextRun TextRunDef => ((InternalTextRun)m_reportElementDef).TextRunDef;

		public override bool IsCompiled => false;

		public override bool ProcessedWithError
		{
			get
			{
				EvaluateOriginalValue();
				return m_originalValue.ErrorOccurred;
			}
		}

		internal InternalTextRunInstance(InternalTextRun textRunDef)
			: base(textRunDef)
		{
		}

		internal VariantResult GetOriginalValue()
		{
			EvaluateOriginalValue();
			return m_originalValue;
		}

		private void EvaluateOriginalValue()
		{
			if (m_originalValueEvaluated)
			{
				return;
			}
			m_originalValueEvaluated = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRunDef = TextRunDef;
			ExpressionInfo value = textRunDef.Value;
			if (value != null)
			{
				if (value.IsExpression)
				{
					m_originalValue = textRunDef.EvaluateValue(ReportScopeInstance, base.ReportElementDef.RenderingContext.OdpContext);
					m_originalValueNeedsReset = true;
				}
				else
				{
					m_originalValue = default(VariantResult);
					m_originalValue.Value = value.Value;
					ReportRuntime.SetVariantType(ref m_originalValue);
				}
			}
		}

		private bool IsDateTimeOffsetOrTimeSpan()
		{
			if (m_originalValue.TypeCode == TypeCode.Object && (m_originalValue.Value is DateTimeOffset || m_originalValue.Value is TimeSpan))
			{
				return true;
			}
			return false;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_formattedValueEvaluated = false;
			m_formattedValue = null;
			m_toolTip = null;
			m_markupType = null;
			if (m_originalValueNeedsReset)
			{
				m_originalValueNeedsReset = false;
				m_originalValueEvaluated = false;
			}
		}

		internal List<string> GetFieldsUsedInValueExpression()
		{
			List<string> result = null;
			ExpressionInfo value = TextRunDef.Value;
			if (value != null && value.IsExpression)
			{
				result = TextRunDef.GetFieldsUsedInValueExpression(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
			}
			return result;
		}
	}
}
