using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledTextRunInstance : TextRunInstance, ICompiledTextRunInstance
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private MarkupType m_markupType;

		private string m_toolTip;

		private string m_label;

		private string m_value;

		private ActionInstance m_actionInstance;

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

		public override string Value => m_value ?? "";

		public override object OriginalValue => m_value ?? "";

		public override string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = base.Definition.Instance.ToolTip;
				}
				return m_toolTip;
			}
		}

		public override MarkupType MarkupType => m_markupType;

		public ActionInstance ActionInstance
		{
			get
			{
				if (m_actionInstance == null && base.Definition.ActionInfo != null)
				{
					ActionCollection actions = base.Definition.ActionInfo.Actions;
					if (actions != null && actions.Count > 0)
					{
						m_actionInstance = actions[0].Instance;
					}
				}
				return m_actionInstance;
			}
		}

		public override TypeCode TypeCode => TypeCode.String;

		public override bool IsCompiled => true;

		public override bool ProcessedWithError => false;

		ICompiledStyleInstance ICompiledTextRunInstance.Style
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

		string ICompiledTextRunInstance.Label
		{
			get
			{
				return m_label;
			}
			set
			{
				if (value == null)
				{
					m_label = string.Empty;
				}
				else
				{
					m_label = value;
				}
			}
		}

		string ICompiledTextRunInstance.Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		string ICompiledTextRunInstance.ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				if (value == null)
				{
					m_toolTip = string.Empty;
				}
				else
				{
					m_toolTip = value;
				}
			}
		}

		MarkupType ICompiledTextRunInstance.MarkupType
		{
			get
			{
				return m_markupType;
			}
			set
			{
				m_markupType = value;
			}
		}

		IActionInstance ICompiledTextRunInstance.ActionInstance
		{
			get
			{
				return m_actionInstance;
			}
			set
			{
				m_actionInstance = (ActionInstance)value;
			}
		}

		internal CompiledTextRunInstance(CompiledRichTextInstance compiledRichTextInstance)
			: base(compiledRichTextInstance.TextRunDefinition)
		{
			m_compiledRichTextInstance = compiledRichTextInstance;
		}
	}
}
