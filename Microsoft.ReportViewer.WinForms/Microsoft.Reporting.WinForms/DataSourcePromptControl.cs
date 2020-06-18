using Microsoft.ReportingServices.Common;
using System;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class DataSourcePromptControl
	{
		private ParameterPanel m_promptPanel;

		private ParameterPanel m_userNamePanel;

		private ParameterPanel m_passwordPanel;

		private TextBox m_userName;

		private TextBox m_password;

		private ReportDataSourceInfo m_dsInfo;

		public ReportDataSourceInfo DataSourceInfo => m_dsInfo;

		public string UserName => m_userName.Text;

		public string Password => m_password.Text;

		public ParameterPanel PromptPanel => m_promptPanel;

		public ParameterPanel UserNamePanel => m_userNamePanel;

		public ParameterPanel PasswordPanel => m_passwordPanel;

		public event EventHandler ValueChanged;

		public DataSourcePromptControl(ReportDataSourceInfo dsInfo, ToolTip toolTip)
		{
			m_dsInfo = dsInfo;
			if (!string.IsNullOrEmpty(m_dsInfo.Prompt))
			{
				m_promptPanel = new ParameterPanel();
				m_promptPanel.Name = "_prompt";
				m_promptPanel.Width = 0;
				m_promptPanel.Controls.Add(new ParameterLabel(m_dsInfo.Prompt));
			}
			m_userName = CreateTextBox();
			m_userNamePanel = CreatePanelForTextBox(m_userName);
			m_password = CreateTextBox();
			m_password.PasswordChar = '*';
			m_passwordPanel = CreatePanelForTextBox(m_password);
			ApplyCustomResources();
		}

		internal void ApplyCustomResources()
		{
			m_userNamePanel.ContainedLabel.Text = LocalizationHelper.Current.UserNamePrompt;
			m_passwordPanel.ContainedLabel.Text = LocalizationHelper.Current.PasswordPrompt;
		}

		private void OnChanged(object sender, EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, EventArgs.Empty);
			}
		}

		public bool Validate(Control owner)
		{
			if (UserName.Length == 0)
			{
				string dataSourcePrompt = (m_promptPanel == null) ? m_dsInfo.Name : m_promptPanel.ContainedLabel.Text;
				MessageBoxWrappers.ShowMessageBox(owner, LocalizationHelper.Current.CredentialMissingUserNameError(dataSourcePrompt), LocalizationHelper.Current.PromptAreaErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			return true;
		}

		private TextBox CreateTextBox()
		{
			TextBox textBox = new TextBox();
			textBox.Width = 170;
			textBox.BorderStyle = BorderStyle.Fixed3D;
			textBox.TextChanged += OnChanged;
			return textBox;
		}

		private ParameterPanel CreatePanelForTextBox(TextBox textBox)
		{
			return new ParameterPanel
			{
				Width = 0,
				Controls = 
				{
					(Control)new ParameterLabel(null),
					(Control)textBox
				}
			};
		}
	}
}
