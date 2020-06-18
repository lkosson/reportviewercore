using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class BooleanControl : ParameterControl
	{
		private RadioButton m_trueButton;

		private RadioButton m_falseButton;

		public override string[] CurrentValue
		{
			get
			{
				if (m_nullCheckBox != null && m_nullCheckBox.Checked)
				{
					return new string[1];
				}
				if (m_trueButton.Checked)
				{
					return new string[1]
					{
						"true"
					};
				}
				if (m_falseButton.Checked)
				{
					return new string[1]
					{
						"false"
					};
				}
				return null;
			}
		}

		public BooleanControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override void InternalApplyCustomResources()
		{
			base.InternalApplyCustomResources();
			m_trueButton.Text = LocalizationHelper.Current.TrueValueText;
			m_tooltip.SetToolTip(m_trueButton, LocalizationHelper.Current.TrueBooleanToolTip);
			m_falseButton.Text = LocalizationHelper.Current.FalseValueText;
			m_tooltip.SetToolTip(m_falseButton, LocalizationHelper.Current.FalseBooleanToolTip);
			RadioButton[] array = new RadioButton[2]
			{
				m_trueButton,
				m_falseButton
			};
			foreach (RadioButton obj in array)
			{
				obj.PerformLayout();
				int width = obj.Width;
				obj.AutoSize = false;
				obj.Height = 22;
				obj.Width = width;
			}
		}

		protected override void CreateChildControls()
		{
			bool isChecked = false;
			bool isChecked2 = false;
			if (m_paramInfo.Values.Count > 0 && base.Enabled)
			{
				isChecked = (string.Compare(m_paramInfo.Values[0], "true", StringComparison.OrdinalIgnoreCase) == 0);
				isChecked2 = (string.Compare(m_paramInfo.Values[0], "false", StringComparison.OrdinalIgnoreCase) == 0);
			}
			m_trueButton = RenderBoolRadioButton(isChecked);
			m_falseButton = RenderBoolRadioButton(isChecked2);
			if (m_paramInfo.Nullable)
			{
				RenderNull();
				base.NullValueChanged += SetEnabledState;
			}
			SetEnabledState(null, null);
		}

		private RadioButton RenderBoolRadioButton(bool isChecked)
		{
			RadioButton radioButton = new RadioButton();
			radioButton.AutoSize = true;
			radioButton.Checked = isChecked;
			radioButton.CheckedChanged += base.OnValueChanged;
			base.Controls.Add(radioButton);
			return radioButton;
		}

		private void SetEnabledState(object sender, EventArgs e)
		{
			m_trueButton.Enabled = (m_nullCheckBox == null || !m_nullCheckBox.Checked);
			m_falseButton.Enabled = (m_nullCheckBox == null || !m_nullCheckBox.Checked);
		}
	}
}
