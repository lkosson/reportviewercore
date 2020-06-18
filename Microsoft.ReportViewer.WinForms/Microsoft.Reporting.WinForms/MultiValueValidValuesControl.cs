using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class MultiValueValidValuesControl : MultiValueControl
	{
		private CheckedListBox m_listBox;

		private bool m_updatingCheckState;

		public override string[] CurrentValue
		{
			get
			{
				List<string> list = new List<string>(m_listBox.CheckedItems.Count);
				foreach (object checkedItem in m_listBox.CheckedItems)
				{
					ComboBoxItem comboBoxItem = checkedItem as ComboBoxItem;
					if (comboBoxItem != null)
					{
						list.Add(comboBoxItem.Value);
					}
				}
				if (list.Count == 0)
				{
					return null;
				}
				return list.ToArray();
			}
		}

		protected override string SummaryString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object checkedItem in m_listBox.CheckedItems)
				{
					ComboBoxItem comboBoxItem = checkedItem as ComboBoxItem;
					if (comboBoxItem != null)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
							stringBuilder.Append(" ");
						}
						stringBuilder.Append(comboBoxItem.ToString().Trim());
					}
				}
				return stringBuilder.ToString();
			}
		}

		private bool HasSelectAllOption => base.ParameterInfo.ValidValues.Count > 1;

		public MultiValueValidValuesControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override void InternalApplyCustomResources()
		{
			base.InternalApplyCustomResources();
			PopulateItemList();
			UpdateSummaryStringDisplay();
		}

		protected override Control CreateSelectorControl()
		{
			m_listBox = new CheckedListBox();
			m_listBox.CheckOnClick = true;
			m_listBox.HorizontalScrollbar = false;
			m_listBox.BorderStyle = BorderStyle.None;
			m_listBox.IntegralHeight = false;
			m_listBox.ItemCheck += OnListBoxItemChecked;
			m_listBox.AutoSize = false;
			return m_listBox;
		}

		private void PopulateItemList()
		{
			int[] array = null;
			if (m_listBox.Items.Count > 0)
			{
				array = new int[m_listBox.CheckedIndices.Count];
				m_listBox.CheckedIndices.CopyTo(array, 0);
			}
			m_listBox.Items.Clear();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (HasSelectAllOption)
			{
				m_listBox.Items.Add(LocalizationHelper.Current.SelectAll);
			}
			foreach (ValidValue validValue in base.ParameterInfo.ValidValues)
			{
				m_listBox.Items.Add(new ComboBoxItem(validValue, null));
				if (!dictionary.ContainsKey(validValue.Value))
				{
					dictionary.Add(validValue.Value, m_listBox.Items.Count - 1);
				}
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					m_listBox.SetItemChecked(array[i], value: true);
				}
				return;
			}
			foreach (string value2 in base.ParameterInfo.Values)
			{
				if (dictionary.TryGetValue(value2, out int value))
				{
					m_listBox.SetItemChecked(value, value: true);
				}
			}
			if (HasSelectAllOption && m_listBox.CheckedItems.Count + 1 == m_listBox.Items.Count)
			{
				m_listBox.SetItemChecked(0, value: true);
			}
		}

		private void OnListBoxItemChecked(object sender, ItemCheckEventArgs e)
		{
			if (m_updatingCheckState)
			{
				return;
			}
			m_updatingCheckState = true;
			try
			{
				if (HasSelectAllOption)
				{
					if (e.Index == 0)
					{
						for (int i = 1; i < m_listBox.Items.Count; i++)
						{
							m_listBox.SetItemChecked(i, e.NewValue == CheckState.Checked);
						}
					}
					else if (e.NewValue == CheckState.Unchecked)
					{
						m_listBox.SetItemChecked(0, value: false);
					}
				}
			}
			finally
			{
				m_updatingCheckState = false;
			}
			m_selectorChanged = true;
		}
	}
}
