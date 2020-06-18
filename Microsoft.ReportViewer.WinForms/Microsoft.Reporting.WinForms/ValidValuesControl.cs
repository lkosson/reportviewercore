using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ValidValuesControl : ParameterControl
	{
		private int m_lastSelectedIndex;

		private AutoWidthComboBox m_comboBox;

		public override string[] CurrentValue
		{
			get
			{
				ComboBoxItem comboBoxItem = m_comboBox.SelectedItem as ComboBoxItem;
				if (comboBoxItem == null)
				{
					return null;
				}
				return new string[1]
				{
					comboBoxItem.Value
				};
			}
		}

		public ValidValuesControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override void InternalApplyCustomResources()
		{
			base.InternalApplyCustomResources();
			PopulateItemList();
		}

		protected override void CreateChildControls()
		{
			m_comboBox = new AutoWidthComboBox();
			m_comboBox.AutoSize = true;
			m_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			m_comboBox.DropDownClosed += OnDropDownExit;
			m_comboBox.LostFocus += OnDropDownExit;
			base.Controls.Add(m_comboBox);
		}

		private void PopulateItemList()
		{
			bool flag = m_comboBox.Items.Count > 0 && !(m_comboBox.Items[0] is ComboBoxItem);
			int selectedIndex = m_comboBox.SelectedIndex;
			bool flag2 = selectedIndex == -1;
			m_comboBox.Items.Clear();
			if (m_paramInfo.ValidValues == null)
			{
				return;
			}
			string nullValueText = LocalizationHelper.Current.NullValueText;
			foreach (ValidValue validValue in m_paramInfo.ValidValues)
			{
				bool flag3 = false;
				if (m_paramInfo.Values.Count > 0)
				{
					flag3 = ((validValue.Value != null) ? (string.Compare(validValue.Value, m_paramInfo.Values[0], StringComparison.Ordinal) == 0) : (m_paramInfo.Values[0] == null));
				}
				ComboBoxItem item = new ComboBoxItem(validValue, nullValueText);
				m_comboBox.Items.Add(item);
				if (flag3 && base.Enabled && flag2)
				{
					m_comboBox.SelectedIndex = m_comboBox.Items.Count - 1;
				}
			}
			if (flag || (flag2 && m_comboBox.SelectedIndex == -1))
			{
				m_comboBox.Items.Insert(0, LocalizationHelper.Current.SelectAValue);
				m_comboBox.SelectedIndex = 0;
			}
			else if (!flag2)
			{
				m_comboBox.SelectedIndex = selectedIndex;
			}
			m_lastSelectedIndex = m_comboBox.SelectedIndex;
		}

		private void OnDropDownExit(object sender, EventArgs e)
		{
			if (m_comboBox.SelectedIndex != m_lastSelectedIndex)
			{
				OnSelectedValueChanged(sender, EventArgs.Empty);
				OnValueChanged(sender, EventArgs.Empty);
			}
		}

		private void OnSelectedValueChanged(object sender, EventArgs e)
		{
			if (m_comboBox.SelectedItem is ComboBoxItem && !(m_comboBox.Items[0] is ComboBoxItem))
			{
				m_comboBox.Items.RemoveAt(0);
			}
			m_lastSelectedIndex = m_comboBox.SelectedIndex;
		}
	}
}
