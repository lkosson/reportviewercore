using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextBoxInstance : ReportItemInstance
	{
		private ParagraphInstanceCollection m_paragraphInstances;

		private bool m_formattedValueEvaluated;

		private string m_formattedValue;

		private bool m_originalValueEvaluated;

		private VariantResult m_originalValue;

		private bool m_toggleState;

		private bool m_toggleStateEvaluated;

		private bool? m_duplicate;

		private TypeCode? m_typeCode;

		private TextBox m_textBoxDef;

		private bool? m_isToggleParent;

		public ParagraphInstanceCollection ParagraphInstances
		{
			get
			{
				if (m_paragraphInstances == null)
				{
					m_paragraphInstances = new ParagraphInstanceCollection(m_textBoxDef);
				}
				return m_paragraphInstances;
			}
		}

		public string Value
		{
			get
			{
				if (!m_formattedValueEvaluated)
				{
					m_formattedValueEvaluated = true;
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_formattedValue = ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).Value;
					}
					else if (m_textBoxDef.IsSimple)
					{
						m_formattedValue = m_textBoxDef.Paragraphs[0].TextRuns[0].Instance.Value;
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						bool flag = false;
						bool flag2 = true;
						foreach (ParagraphInstance paragraphInstance in ParagraphInstances)
						{
							if (!flag2)
							{
								flag = true;
								stringBuilder.Append(Environment.NewLine);
							}
							else
							{
								flag2 = false;
							}
							foreach (TextRunInstance textRunInstance in paragraphInstance.TextRunInstances)
							{
								string value = textRunInstance.Value;
								if (value != null)
								{
									flag = true;
									stringBuilder.Append(value);
								}
							}
						}
						if (flag)
						{
							m_formattedValue = stringBuilder.ToString();
						}
					}
				}
				return m_formattedValue;
			}
		}

		public object OriginalValue
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).OriginalValue;
				}
				EvaluateOriginalValue();
				return m_originalValue.Value;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				if (!m_isToggleParent.HasValue)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_isToggleParent = ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).IsToggleParent;
					}
					else
					{
						m_isToggleParent = m_textBoxDef.TexBoxDef.EvaluateIsToggle(ReportScopeInstance, base.RenderingContext.OdpContext);
					}
				}
				return m_isToggleParent.Value;
			}
		}

		public bool ToggleState
		{
			get
			{
				if (!m_toggleStateEvaluated)
				{
					m_toggleStateEvaluated = true;
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_toggleState = ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).ToggleState;
					}
					else if (IsToggleParent)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = m_textBoxDef.TexBoxDef;
						m_toggleState = texBoxDef.EvaluateInitialToggleState(ReportScopeInstance, base.RenderingContext.OdpContext);
						if (base.RenderingContext.IsSenderToggled(UniqueName))
						{
							m_toggleState = !m_toggleState;
						}
					}
					else
					{
						m_toggleState = false;
					}
				}
				return m_toggleState;
			}
		}

		public SortOptions SortState
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).SortState;
				}
				return base.RenderingContext.GetSortState(UniqueName);
			}
		}

		public bool Duplicate
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).Duplicate;
				}
				if (!m_duplicate.HasValue)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = m_textBoxDef.TexBoxDef;
					if (texBoxDef.HideDuplicates == null)
					{
						return false;
					}
					EvaluateOriginalValue();
					m_duplicate = texBoxDef.CalculateDuplicates(m_originalValue, base.RenderingContext.OdpContext);
				}
				return m_duplicate.Value;
			}
		}

		public TypeCode TypeCode
		{
			get
			{
				if (!m_typeCode.HasValue)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						object originalValue = ((Microsoft.ReportingServices.ReportRendering.TextBox)m_reportElementDef.RenderReportItem).OriginalValue;
						if (originalValue != null)
						{
							Type type = originalValue.GetType();
							m_typeCode = Type.GetTypeCode(type);
						}
						else
						{
							m_typeCode = TypeCode.Empty;
						}
					}
					else
					{
						EvaluateOriginalValue();
					}
				}
				return m_typeCode.Value;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot || m_textBoxDef.TexBoxDef.IsSimple)
				{
					return m_textBoxDef.Paragraphs[0].TextRuns[0].Instance.ProcessedWithError;
				}
				return false;
			}
		}

		internal TextBoxInstance(TextBox reportItemDef)
			: base(reportItemDef)
		{
			m_textBoxDef = reportItemDef;
		}

		public void AddToCurrentPage()
		{
			m_reportElementDef.RenderingContext.AddToCurrentPage(m_textBoxDef.Name, OriginalValue);
		}

		public void RegisterToggleSender()
		{
			if (!m_reportElementDef.IsOldSnapshot && IsToggleParent)
			{
				m_reportElementDef.RenderingContext.AddValidToggleSender(UniqueName);
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			if (m_reportElementDef.IsOldSnapshot)
			{
				m_typeCode = null;
			}
			else
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = m_textBoxDef.TexBoxDef;
				if (texBoxDef.HasExpressionBasedValue)
				{
					texBoxDef.ResetTextBoxImpl(base.RenderingContext.OdpContext);
					m_originalValueEvaluated = false;
					if (texBoxDef.IsSimple)
					{
						m_typeCode = null;
					}
					else
					{
						m_typeCode = TypeCode.String;
					}
				}
			}
			m_formattedValueEvaluated = false;
			m_formattedValue = null;
			m_toggleStateEvaluated = false;
			m_duplicate = null;
			m_isToggleParent = null;
		}

		private void EvaluateOriginalValue()
		{
			if (m_originalValueEvaluated)
			{
				return;
			}
			m_originalValueEvaluated = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = m_textBoxDef.TexBoxDef;
			if (texBoxDef.HasValue)
			{
				_ = base.RenderingContext.OdpContext;
				m_originalValue = default(VariantResult);
				if (texBoxDef.IsSimple)
				{
					InternalTextRunInstance internalTextRunInstance = (InternalTextRunInstance)m_textBoxDef.Paragraphs[0].TextRuns[0].Instance;
					m_originalValue.Value = internalTextRunInstance.OriginalValue;
					m_originalValue.ErrorOccurred = internalTextRunInstance.ProcessedWithError;
					m_typeCode = internalTextRunInstance.TypeCode;
					m_originalValue.TypeCode = m_typeCode.Value;
					return;
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				bool flag2 = true;
				foreach (ParagraphInstance paragraphInstance in ParagraphInstances)
				{
					if (!flag2)
					{
						flag = true;
						stringBuilder.Append(Environment.NewLine);
					}
					else
					{
						flag2 = false;
					}
					foreach (TextRunInstance textRunInstance in paragraphInstance.TextRunInstances)
					{
						object originalValue = textRunInstance.OriginalValue;
						if (originalValue != null)
						{
							flag = true;
							stringBuilder.Append(originalValue);
						}
					}
				}
				if (flag)
				{
					m_originalValue.Value = stringBuilder.ToString();
					m_originalValue.TypeCode = TypeCode.String;
					m_typeCode = TypeCode.String;
				}
			}
			else
			{
				m_typeCode = TypeCode.Empty;
			}
		}

		internal List<string> GetFieldsUsedInValueExpression()
		{
			List<string> result = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = m_textBoxDef.TexBoxDef;
			if (texBoxDef.HasExpressionBasedValue)
			{
				result = texBoxDef.GetFieldsUsedInValueExpression(ReportScopeInstance, base.RenderingContext.OdpContext);
			}
			return result;
		}
	}
}
