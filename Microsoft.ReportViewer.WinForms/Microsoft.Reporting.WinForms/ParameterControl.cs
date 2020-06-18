using Microsoft.ReportingServices.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal abstract class ParameterControl : ParameterPanel
	{
		public bool IsDependency;

		protected ReportParameterInfo m_paramInfo;

		protected CheckBox m_nullCheckBox;

		protected ToolTip m_tooltip;

		private bool m_allowQueryExecution;

		private bool m_ignoreChanges;

		private GridLayoutCellDefinition m_cellDefinition;

		public ReportParameterInfo ParameterInfo => m_paramInfo;

		internal GridLayoutCellDefinition CellDefinition => m_cellDefinition;

		public bool DisableQueryParameter
		{
			get
			{
				if (!m_allowQueryExecution)
				{
					return ParameterInfo.IsQueryParameter;
				}
				return false;
			}
		}

		public abstract string[] CurrentValue
		{
			get;
		}

		private bool ShouldDisableParam
		{
			get
			{
				if (m_paramInfo.State != 0)
				{
					return m_paramInfo.State != ParameterState.MissingValidValue;
				}
				return false;
			}
		}

		public event EventHandler ValueChanged;

		protected event EventHandler NullValueChanged;

		public static ParameterControl Create(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
		{
			if (paramInfo.MultiValue)
			{
				if (paramInfo.ValidValues != null)
				{
					return new MultiValueValidValuesControl(paramInfo, tooltip, font, allowQueryExecution, cellDefinition);
				}
				return new MultiValueTextControl(paramInfo, tooltip, font, allowQueryExecution, cellDefinition);
			}
			if (paramInfo.ValidValues != null)
			{
				return new ValidValuesControl(paramInfo, tooltip, font, allowQueryExecution, cellDefinition);
			}
			if (paramInfo.DataType == ParameterDataType.Boolean)
			{
				return new BooleanControl(paramInfo, tooltip, font, allowQueryExecution, cellDefinition);
			}
			if (paramInfo.DataType == ParameterDataType.DateTime)
			{
				return new DateControl(paramInfo, tooltip, font, allowQueryExecution, cellDefinition);
			}
			return new TextControl(paramInfo, tooltip, font, allowQueryExecution, cellDefinition);
		}

		protected ParameterControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
		{
			m_paramInfo = paramInfo;
			m_tooltip = tooltip;
			Font = font;
			m_allowQueryExecution = allowQueryExecution;
			m_cellDefinition = cellDefinition;
			base.Enabled = (!ShouldDisableParam && !DisableQueryParameter);
			Label value = new ParameterLabel(m_paramInfo.Prompt);
			base.Controls.Add(value);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			CreateChildControls();
			ApplyCustomResources();
		}

		protected abstract void CreateChildControls();

		internal void ApplyCustomResources()
		{
			if (base.IsHandleCreated)
			{
				bool ignoreChanges = m_ignoreChanges;
				try
				{
					m_ignoreChanges = true;
					InternalApplyCustomResources();
				}
				finally
				{
					m_ignoreChanges = ignoreChanges;
				}
			}
		}

		protected virtual void InternalApplyCustomResources()
		{
			if (m_nullCheckBox != null)
			{
				m_nullCheckBox.Text = LocalizationHelper.Current.NullCheckBoxText;
				m_tooltip.SetToolTip(m_nullCheckBox, LocalizationHelper.Current.NullCheckBoxToolTip);
			}
		}

		protected void OnValueChanged(object sender, EventArgs e)
		{
			if (!m_ignoreChanges && this.ValueChanged != null)
			{
				this.ValueChanged(this, EventArgs.Empty);
			}
		}

		protected void OnNullValueChanged(object sender, EventArgs e)
		{
			if (this.NullValueChanged != null)
			{
				this.NullValueChanged(this, EventArgs.Empty);
			}
			if (CurrentValue != null)
			{
				OnValueChanged(sender, e);
			}
		}

		public bool Validate()
		{
			if (base.Enabled && CurrentValue == null)
			{
				string parameterPrompt = string.IsNullOrEmpty(m_paramInfo.Prompt) ? m_paramInfo.Name : m_paramInfo.Prompt;
				string text = (m_paramInfo.ValidValues == null) ? LocalizationHelper.Current.ParameterMissingValueError(parameterPrompt) : LocalizationHelper.Current.ParameterMissingSelectionError(parameterPrompt);
				MessageBoxWrappers.ShowMessageBox(this, text, LocalizationHelper.Current.PromptAreaErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			return true;
		}

		protected void RenderNull()
		{
			m_nullCheckBox = new CheckBox();
			m_nullCheckBox.AccessibleName = ReportPreviewStrings.NullCheckAccessibleName(ParameterInfo.Name);
			m_nullCheckBox.AutoSize = true;
			m_nullCheckBox.Checked = (m_paramInfo.Values.Count > 0 && m_paramInfo.Values[0] == null);
			m_nullCheckBox.Click += OnNullValueChanged;
			base.Controls.Add(m_nullCheckBox);
		}
	}
}
