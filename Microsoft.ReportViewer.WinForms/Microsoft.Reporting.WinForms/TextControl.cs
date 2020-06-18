using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class TextControl : ParameterControl
	{
		private bool m_textChanged;

		private TextBox m_textBox;

		public override string[] CurrentValue
		{
			get
			{
				if (m_nullCheckBox != null && m_nullCheckBox.Checked)
				{
					return new string[1];
				}
				if (m_paramInfo.AllowBlank)
				{
					return new string[1]
					{
						m_textBox.Text
					};
				}
				if (m_paramInfo.DataType == ParameterDataType.String)
				{
					if (m_textBox.Text.Length > 0)
					{
						return new string[1]
						{
							m_textBox.Text
						};
					}
				}
				else if (m_textBox.Text.Trim().Length > 0)
				{
					return new string[1]
					{
						m_textBox.Text
					};
				}
				return null;
			}
		}

		public TextControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override void CreateChildControls()
		{
			m_textBox = new TextBox();
			m_textBox.Width = 170;
			if (m_paramInfo.Values.Count > 0 && base.Enabled)
			{
				m_textBox.Text = m_paramInfo.Values[0];
			}
			m_textBox.BorderStyle = BorderStyle.Fixed3D;
			m_textBox.TextChanged += OnTextChanged;
			base.Validated += OnTextValidated;
			base.Controls.Add(m_textBox);
			if (m_paramInfo.Nullable)
			{
				RenderNull();
				base.NullValueChanged += SetEnabledState;
			}
			SetEnabledState(null, null);
		}

		protected override void InternalApplyCustomResources()
		{
			base.InternalApplyCustomResources();
			string caption = string.Empty;
			if (m_paramInfo.DataType == ParameterDataType.String)
			{
				caption = LocalizationHelper.Current.StringToolTip;
			}
			else if (m_paramInfo.DataType == ParameterDataType.Float)
			{
				caption = LocalizationHelper.Current.FloatToolTip;
			}
			else if (m_paramInfo.DataType == ParameterDataType.Integer)
			{
				caption = LocalizationHelper.Current.IntToolTip;
			}
			m_tooltip.SetToolTip(m_textBox, caption);
		}

		private void OnTextValidated(object sender, EventArgs e)
		{
			if (m_textChanged)
			{
				OnValueChanged(this, e);
			}
			m_textChanged = false;
		}

		private void SetEnabledState(object sender, EventArgs e)
		{
			m_textBox.Enabled = (m_nullCheckBox == null || !m_nullCheckBox.Checked);
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			m_textChanged = true;
		}
	}
}
