using Microsoft.ReportingServices.CommonControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal abstract class MultiValueControl : ParameterControl
	{
		protected bool m_selectorChanged;

		private MirrorComboBox m_summaryCombo;

		private GenericDropDown m_dropDown;

		protected abstract string SummaryString
		{
			get;
		}

		protected override int MinimumRequiredHeight => m_summaryCombo.Height;

		public MultiValueControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override void CreateChildControls()
		{
			m_summaryCombo = new MirrorComboBox();
			m_summaryCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			m_summaryCombo.DropDownHeight = 1;
			m_summaryCombo.Width = 170;
			m_summaryCombo.DropDown += OnDropDown;
			m_summaryCombo.AccessibleName = base.ParameterInfo.Name;
			m_dropDown = new GenericDropDown();
			m_dropDown.TopControl = m_summaryCombo;
			Control control = CreateSelectorControl();
			control.AutoSize = false;
			control.Size = new Size(m_summaryCombo.Width - 2, 150);
			m_dropDown.DropDownControl = new ResizableToolStripPanel(control);
			m_dropDown.DropDownClosed += OnDropDownClosed;
			UpdateSummaryStringDisplay();
			base.Controls.Add(m_dropDown);
		}

		protected abstract Control CreateSelectorControl();

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			if (m_summaryCombo != null)
			{
				m_summaryCombo.RightToLeft = RightToLeft;
			}
		}

		private void OnDropDown(object sender, EventArgs e)
		{
			if (base.Parent != null)
			{
				BeginInvoke((MethodInvoker)delegate
				{
					m_dropDown.OpenDropDown();
				});
			}
		}

		private void OnDropDownClosed(object sender, EventArgs e)
		{
			if (m_selectorChanged)
			{
				OnValueChanged(this, EventArgs.Empty);
			}
			m_selectorChanged = false;
			UpdateSummaryStringDisplay();
		}

		protected void UpdateSummaryStringDisplay()
		{
			m_summaryCombo.Items.Clear();
			m_summaryCombo.Items.Add(SummaryString.ToString());
			m_summaryCombo.SelectedIndex = 0;
		}
	}
}
