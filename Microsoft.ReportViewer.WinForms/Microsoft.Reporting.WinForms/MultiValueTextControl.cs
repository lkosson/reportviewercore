using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class MultiValueTextControl : MultiValueControl
	{
		private TextBox m_textBox;

		public override string[] CurrentValue
		{
			get
			{
				string[] lines = m_textBox.Lines;
				if (base.ParameterInfo.AllowBlank && base.ParameterInfo.DataType == ParameterDataType.String)
				{
					if (lines.Length == 0)
					{
						return new string[1]
						{
							string.Empty
						};
					}
					return lines;
				}
				List<string> list = new List<string>(lines.Length);
				string[] array = lines;
				foreach (string text in array)
				{
					if (text.Length > 0)
					{
						list.Add(text);
					}
				}
				if (list.Count > 0)
				{
					return list.ToArray();
				}
				return null;
			}
		}

		protected override string SummaryString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				if (CurrentValue != null)
				{
					string[] currentValue = CurrentValue;
					foreach (string value in currentValue)
					{
						if (!flag)
						{
							stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
							stringBuilder.Append(" ");
						}
						stringBuilder.Append(value);
						flag = false;
					}
				}
				return stringBuilder.ToString();
			}
		}

		public MultiValueTextControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override Control CreateSelectorControl()
		{
			m_textBox = new TextBox();
			m_textBox.Multiline = true;
			m_textBox.WordWrap = false;
			m_textBox.ScrollBars = ScrollBars.Vertical;
			m_textBox.BorderStyle = BorderStyle.None;
			m_textBox.AcceptsReturn = true;
			m_textBox.TextChanged += OnSelectorChanged;
			IList<string> values = base.ParameterInfo.Values;
			string[] array = new string[values.Count];
			for (int i = 0; i < values.Count; i++)
			{
				array[i] = values[i];
			}
			m_textBox.Lines = array;
			return m_textBox;
		}

		private void OnSelectorChanged(object sender, EventArgs e)
		{
			m_selectorChanged = true;
		}
	}
}
