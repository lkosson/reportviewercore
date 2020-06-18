using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledRichTextInstance : BaseInstance, IRichTextInstanceCreator, IRichTextLogger
	{
		private bool m_multipleParagraphsAllowed;

		private TextRun m_textRunDef;

		private Paragraph m_paragraphDef;

		private CompiledParagraphInstanceCollection m_compiledParagraphCollection;

		private bool m_parseErrorOccured;

		private bool m_parsed;

		private string m_uniqueName;

		private int m_objectCount;

		private IErrorContext m_errorContext;

		public string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					m_uniqueName = m_textRunDef.InstanceUniqueName + "x" + GenerateID().ToString(CultureInfo.InvariantCulture);
				}
				return m_uniqueName;
			}
		}

		public CompiledParagraphInstanceCollection CompiledParagraphInstances
		{
			get
			{
				Parse();
				return m_compiledParagraphCollection;
			}
		}

		internal TextRun TextRunDefinition => m_textRunDef;

		internal Paragraph ParagraphDefinition => m_paragraphDef;

		public bool ParseErrorOccured
		{
			get
			{
				Parse();
				return m_parseErrorOccured;
			}
		}

		RSTrace IRichTextLogger.Tracer => Global.Tracer;

		internal CompiledRichTextInstance(IReportScope reportScope, TextRun textRunDef, Paragraph paragraphDef, bool multipleParagraphsAllowed)
			: base(reportScope)
		{
			m_paragraphDef = paragraphDef;
			m_textRunDef = textRunDef;
			m_multipleParagraphsAllowed = multipleParagraphsAllowed;
			m_errorContext = m_textRunDef.RenderingContext.ErrorContext;
		}

		private void Parse()
		{
			if (m_parsed)
			{
				return;
			}
			try
			{
				m_parsed = true;
				m_paragraphDef.CriGenerationPhase = ReportElement.CriGenerationPhases.Definition;
				m_textRunDef.CriGenerationPhase = ReportElement.CriGenerationPhases.Definition;
				ReportEnumProperty<MarkupType> markupType = m_textRunDef.MarkupType;
				MarkupType markupType2 = (!markupType.IsExpression) ? markupType.Value : m_textRunDef.Instance.MarkupType;
				RichTextParser richTextParser = null;
				if (markupType2 != MarkupType.HTML)
				{
					return;
				}
				richTextParser = new HtmlParser(m_multipleParagraphsAllowed, this, this);
				InternalTextRunInstance internalTextRunInstance = (InternalTextRunInstance)m_textRunDef.Instance;
				Microsoft.ReportingServices.RdlExpressions.VariantResult originalValue = internalTextRunInstance.GetOriginalValue();
				if (!originalValue.ErrorOccurred && originalValue.TypeCode != 0)
				{
					try
					{
						string richText = (originalValue.TypeCode != TypeCode.String) ? internalTextRunInstance.TextRunDef.FormatTextRunValue(originalValue.Value, originalValue.TypeCode, m_textRunDef.RenderingContext.OdpContext) : (originalValue.Value as string);
						m_compiledParagraphCollection = (CompiledParagraphInstanceCollection)richTextParser.Parse(richText);
					}
					catch (Exception ex)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidRichTextParseFailed, Severity.Warning, "TextRun", internalTextRunInstance.TextRunDef.Name, ex.Message);
						m_parseErrorOccured = true;
						CreateSingleTextRun().Value = RPRes.rsRichTextParseErrorValue;
					}
				}
				else
				{
					ICompiledTextRunInstance compiledTextRunInstance = CreateSingleTextRun();
					if (originalValue.ErrorOccurred)
					{
						compiledTextRunInstance.Value = RPRes.rsExpressionErrorValue;
					}
				}
			}
			finally
			{
				m_textRunDef.CriGenerationPhase = ReportElement.CriGenerationPhases.None;
				m_paragraphDef.CriGenerationPhase = ReportElement.CriGenerationPhases.None;
			}
		}

		private ICompiledTextRunInstance CreateSingleTextRun()
		{
			ICompiledParagraphInstance compiledParagraphInstance = new CompiledParagraphInstance(this);
			ICompiledTextRunInstance compiledTextRunInstance = new CompiledTextRunInstance(this);
			CompiledRichTextStyleInstance style = new CompiledRichTextStyleInstance(m_textRunDef, m_textRunDef.ReportScope, m_textRunDef.RenderingContext);
			m_compiledParagraphCollection = new CompiledParagraphInstanceCollection(this);
			compiledParagraphInstance.CompiledTextRunInstances = new CompiledTextRunInstanceCollection(this);
			compiledTextRunInstance.Style = style;
			compiledParagraphInstance.Style = style;
			((ICollection<ICompiledParagraphInstance>)m_compiledParagraphCollection).Add(compiledParagraphInstance);
			compiledParagraphInstance.CompiledTextRunInstances.Add(compiledTextRunInstance);
			return compiledTextRunInstance;
		}

		protected override void ResetInstanceCache()
		{
			m_compiledParagraphCollection = null;
			m_parseErrorOccured = false;
			m_parsed = false;
			m_uniqueName = null;
			m_objectCount = 0;
		}

		internal int GenerateID()
		{
			return m_objectCount++;
		}

		IList<ICompiledParagraphInstance> IRichTextInstanceCreator.CreateParagraphInstanceCollection()
		{
			return new CompiledParagraphInstanceCollection(this);
		}

		ICompiledParagraphInstance IRichTextInstanceCreator.CreateParagraphInstance()
		{
			return new CompiledParagraphInstance(this);
		}

		ICompiledTextRunInstance IRichTextInstanceCreator.CreateTextRunInstance()
		{
			return new CompiledTextRunInstance(this);
		}

		IList<ICompiledTextRunInstance> IRichTextInstanceCreator.CreateTextRunInstanceCollection()
		{
			return new CompiledTextRunInstanceCollection(this);
		}

		ICompiledStyleInstance IRichTextInstanceCreator.CreateStyleInstance(bool isParagraphStyle)
		{
			if (isParagraphStyle)
			{
				return new CompiledRichTextStyleInstance(m_paragraphDef, m_paragraphDef.ReportScope, m_paragraphDef.RenderingContext);
			}
			return new CompiledRichTextStyleInstance(m_textRunDef, m_textRunDef.ReportScope, m_textRunDef.RenderingContext);
		}

		IActionInstance IRichTextInstanceCreator.CreateActionInstance()
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem();
			Microsoft.ReportingServices.ReportIntermediateFormat.Action action = new Microsoft.ReportingServices.ReportIntermediateFormat.Action();
			action.ActionItems.Add(actionItem);
			return new Action(new ActionInfo(m_textRunDef.RenderingContext, m_textRunDef.ReportScope, action, ((InternalTextRun)m_textRunDef).TextRunDef, m_textRunDef, ObjectType.TextRun, ((InternalTextRun)m_textRunDef).TextRunDef.Name, m_textRunDef), actionItem, 0).Instance;
		}

		void IRichTextLogger.RegisterOutOfRangeSizeWarning(string propertyName, string value, string minVal, string maxVal)
		{
			m_errorContext.Register(ProcessingErrorCode.rsParseErrorOutOfRangeSize, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)m_textRunDef).TextRunDef.Name, propertyName, value, minVal, maxVal);
		}

		void IRichTextLogger.RegisterInvalidValueWarning(string propertyName, string value, int charPosition)
		{
			m_errorContext.Register(ProcessingErrorCode.rsParseErrorInvalidValue, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)m_textRunDef).TextRunDef.Name, propertyName, value, charPosition.ToString(CultureInfo.InvariantCulture));
		}

		void IRichTextLogger.RegisterInvalidColorWarning(string propertyName, string value, int charPosition)
		{
			m_errorContext.Register(ProcessingErrorCode.rsParseErrorInvalidColor, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)m_textRunDef).TextRunDef.Name, propertyName, value, charPosition.ToString(CultureInfo.InvariantCulture));
		}

		void IRichTextLogger.RegisterInvalidSizeWarning(string propertyName, string value, int charPosition)
		{
			m_errorContext.Register(ProcessingErrorCode.rsParseErrorInvalidSize, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)m_textRunDef).TextRunDef.Name, propertyName, value, charPosition.ToString(CultureInfo.InvariantCulture));
		}
	}
}
