using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledParagraphInstance : ParagraphInstance, ICompiledParagraphInstance
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private CompiledTextRunInstanceCollection m_compiledTextRunInstances;

		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private ListStyle m_listStyle;

		private int m_listLevel;

		private ReportSize m_spaceBefore;

		private ReportSize m_spaceAfter;

		private InternalParagraphInstance NativeParagraphInstance => (InternalParagraphInstance)base.Definition.Instance;

		public override string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					m_uniqueName = m_reportElementDef.InstanceUniqueName + "x" + m_compiledRichTextInstance.GenerateID();
				}
				return m_uniqueName;
			}
		}

		public override StyleInstance Style => m_style;

		public override ReportSize LeftIndent
		{
			get
			{
				if (m_leftIndent == null)
				{
					m_leftIndent = NativeParagraphInstance.GetLeftIndent(constantUsable: false);
				}
				return m_leftIndent;
			}
		}

		public override ReportSize RightIndent
		{
			get
			{
				if (m_rightIndent == null)
				{
					m_rightIndent = NativeParagraphInstance.GetRightIndent(constantUsable: false);
				}
				return m_rightIndent;
			}
		}

		public override ReportSize HangingIndent
		{
			get
			{
				if (m_hangingIndent == null)
				{
					m_hangingIndent = NativeParagraphInstance.GetHangingIndent(constantUsable: false);
				}
				return m_hangingIndent;
			}
		}

		public override ListStyle ListStyle
		{
			get
			{
				if (m_listStyle == ListStyle.None)
				{
					return NativeParagraphInstance.ListStyle;
				}
				return m_listStyle;
			}
		}

		public override int ListLevel => m_listLevel + NativeParagraphInstance.ListLevel;

		public override ReportSize SpaceBefore
		{
			get
			{
				if (m_spaceBefore == null)
				{
					m_spaceBefore = NativeParagraphInstance.GetSpaceBefore(constantUsable: false);
				}
				return m_spaceBefore;
			}
		}

		public override ReportSize SpaceAfter
		{
			get
			{
				if (m_spaceAfter == null)
				{
					m_spaceAfter = NativeParagraphInstance.GetSpaceAfter(constantUsable: false);
				}
				return m_spaceAfter;
			}
		}

		public CompiledTextRunInstanceCollection CompiledTextRunInstances
		{
			get
			{
				return m_compiledTextRunInstances;
			}
			internal set
			{
				m_compiledTextRunInstances = value;
			}
		}

		internal TextRun TextRunDefinition => m_compiledRichTextInstance.TextRunDefinition;

		public override bool IsCompiled => true;

		IList<ICompiledTextRunInstance> ICompiledParagraphInstance.CompiledTextRunInstances
		{
			get
			{
				return m_compiledTextRunInstances;
			}
			set
			{
				m_compiledTextRunInstances = (CompiledTextRunInstanceCollection)value;
			}
		}

		ICompiledStyleInstance ICompiledParagraphInstance.Style
		{
			get
			{
				return (ICompiledStyleInstance)m_style;
			}
			set
			{
				m_style = (CompiledRichTextStyleInstance)value;
			}
		}

		ReportSize ICompiledParagraphInstance.LeftIndent
		{
			get
			{
				return m_leftIndent;
			}
			set
			{
				m_leftIndent = value;
			}
		}

		ReportSize ICompiledParagraphInstance.RightIndent
		{
			get
			{
				return m_rightIndent;
			}
			set
			{
				m_rightIndent = value;
			}
		}

		ReportSize ICompiledParagraphInstance.HangingIndent
		{
			get
			{
				return m_hangingIndent;
			}
			set
			{
				m_hangingIndent = value;
			}
		}

		ListStyle ICompiledParagraphInstance.ListStyle
		{
			get
			{
				return m_listStyle;
			}
			set
			{
				m_listStyle = value;
			}
		}

		int ICompiledParagraphInstance.ListLevel
		{
			get
			{
				return m_listLevel;
			}
			set
			{
				m_listLevel = value;
			}
		}

		ReportSize ICompiledParagraphInstance.SpaceBefore
		{
			get
			{
				return m_spaceBefore;
			}
			set
			{
				m_spaceBefore = value;
			}
		}

		ReportSize ICompiledParagraphInstance.SpaceAfter
		{
			get
			{
				return m_spaceAfter;
			}
			set
			{
				m_spaceAfter = value;
			}
		}

		internal CompiledParagraphInstance(CompiledRichTextInstance compiledRichTextInstance)
			: base(compiledRichTextInstance.ParagraphDefinition)
		{
			m_compiledRichTextInstance = compiledRichTextInstance;
		}
	}
}
