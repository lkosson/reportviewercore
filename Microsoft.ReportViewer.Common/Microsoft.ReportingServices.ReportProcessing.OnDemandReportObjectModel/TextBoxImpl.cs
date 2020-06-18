using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class TextBoxImpl : ReportItemImpl
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.TextBox m_textBox;

		private Microsoft.ReportingServices.RdlExpressions.VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		private List<string> m_fieldsUsedInValueExpression;

		private ParagraphsImpl m_paragraphs;

		public override object Value
		{
			get
			{
				GetResult(null, calledFromValue: true);
				return m_result.Value;
			}
		}

		internal Paragraphs Paragraphs => m_paragraphs;

		internal TextBoxImpl(Microsoft.ReportingServices.ReportIntermediateFormat.TextBox itemDef, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext)
			: base(itemDef, reportRT, iErrorContext)
		{
			m_textBox = itemDef;
			m_paragraphs = new ParagraphsImpl(m_textBox, m_reportRT, m_iErrorContext, m_scope);
		}

		private bool IsTextboxInScope()
		{
			OnDemandProcessingContext odpContext = m_reportRT.ReportObjectModel.OdpContext;
			IRIFReportScope iRIFReportScope = null;
			if (odpContext.IsTablixProcessingMode)
			{
				iRIFReportScope = odpContext.LastTablixProcessingReportScope;
				if (iRIFReportScope == null)
				{
					iRIFReportScope = odpContext.ReportDefinition;
				}
			}
			else if (odpContext.IsTopLevelSubReportProcessing)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport = odpContext.LastRIFObject as Microsoft.ReportingServices.ReportIntermediateFormat.SubReport;
				Global.Tracer.Assert(subReport != null, "Missing reference to subreport object");
				iRIFReportScope = subReport.GetContainingSection(odpContext);
			}
			else
			{
				IReportScope currentReportScope = odpContext.CurrentReportScope;
				iRIFReportScope = ((currentReportScope == null) ? odpContext.ReportDefinition : currentReportScope.RIFReportScope);
			}
			if (iRIFReportScope == null || !iRIFReportScope.TextboxInScope(m_textBox.SequenceID))
			{
				return false;
			}
			return true;
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult GetResult(IReportScopeInstance romInstance, bool calledFromValue)
		{
			if (calledFromValue && !IsTextboxInScope())
			{
				m_result = default(Microsoft.ReportingServices.RdlExpressions.VariantResult);
			}
			else if (!m_isValueReady)
			{
				if (m_isVisited)
				{
					m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, m_textBox.ObjectType, m_textBox.Name, "Value");
					throw new ReportProcessingException_InvalidOperationException();
				}
				m_isVisited = true;
				_ = m_reportRT.ReportObjectModel;
				OnDemandProcessingContext odpContext = m_reportRT.ReportObjectModel.OdpContext;
				bool contextUpdated = m_reportRT.ContextUpdated;
				IInstancePath originalObject = null;
				m_reportRT.ContextUpdated = false;
				if (odpContext.IsTablixProcessingMode || calledFromValue)
				{
					originalObject = odpContext.LastRIFObject;
				}
				bool flag = m_textBox.Action != null && m_textBox.Action.TrackFieldsUsedInValueExpression;
				Dictionary<string, bool> dictionary = null;
				if (flag)
				{
					dictionary = new Dictionary<string, bool>();
				}
				try
				{
					bool flag2 = false;
					if (m_paragraphs.Count == 1)
					{
						TextRunsImpl textRunsImpl = (TextRunsImpl)m_paragraphs[0].TextRuns;
						if (textRunsImpl.Count == 1)
						{
							flag2 = true;
							TextRunImpl textRunImpl = (TextRunImpl)textRunsImpl[0];
							m_result = textRunImpl.GetResult(romInstance);
							if (flag)
							{
								textRunImpl.MergeFieldsUsedInValueExpression(dictionary);
							}
						}
					}
					if (!flag2)
					{
						bool flag3 = false;
						m_result = default(Microsoft.ReportingServices.RdlExpressions.VariantResult);
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < m_paragraphs.Count; i++)
						{
							if (i > 0)
							{
								flag3 = true;
								stringBuilder.Append(Environment.NewLine);
							}
							TextRunsImpl textRunsImpl2 = (TextRunsImpl)m_paragraphs[i].TextRuns;
							for (int j = 0; j < textRunsImpl2.Count; j++)
							{
								TextRunImpl textRunImpl2 = (TextRunImpl)textRunsImpl2[j];
								Microsoft.ReportingServices.RdlExpressions.VariantResult result = textRunImpl2.GetResult(romInstance);
								if (result.Value != null)
								{
									if (result.TypeCode == TypeCode.Object && (result.Value is TimeSpan || result.Value is DateTimeOffset))
									{
										string text = textRunImpl2.TextRunDef.FormatTextRunValue(result, odpContext);
										if (text != null)
										{
											result.Value = text;
										}
										else
										{
											result.Value = Microsoft.ReportingServices.RdlExpressions.ReportRuntime.ConvertToStringFallBack(result.Value);
										}
									}
									flag3 = true;
									stringBuilder.Append(result.Value);
								}
								if (flag)
								{
									textRunImpl2.MergeFieldsUsedInValueExpression(dictionary);
								}
							}
						}
						if (flag3)
						{
							m_result.Value = stringBuilder.ToString();
							m_result.TypeCode = TypeCode.String;
						}
					}
					if (flag)
					{
						m_fieldsUsedInValueExpression = new List<string>();
						foreach (string key in dictionary.Keys)
						{
							m_fieldsUsedInValueExpression.Add(key);
						}
					}
				}
				finally
				{
					odpContext.RestoreContext(originalObject);
					m_reportRT.ContextUpdated = contextUpdated;
					m_isVisited = false;
					m_isValueReady = true;
				}
			}
			return m_result;
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance)
		{
			if (!m_isValueReady)
			{
				GetResult(romInstance, calledFromValue: true);
			}
			return m_fieldsUsedInValueExpression;
		}

		internal override void Reset()
		{
			if (m_textBox.HasExpressionBasedValue)
			{
				m_isValueReady = false;
				m_paragraphs.Reset();
			}
		}

		internal override void Reset(Microsoft.ReportingServices.RdlExpressions.VariantResult value)
		{
			SetResult(value);
		}

		internal void SetResult(Microsoft.ReportingServices.RdlExpressions.VariantResult result)
		{
			m_result = result;
			m_isValueReady = true;
		}
	}
}
