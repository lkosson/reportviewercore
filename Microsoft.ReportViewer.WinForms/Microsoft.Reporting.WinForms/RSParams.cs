using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class RSParams : MirrorPanel
	{
		internal class TestAccessor : RSParams
		{
			public ReportParameterInfoCollection ParamInfos
			{
				get
				{
					return m_paramInfos;
				}
				set
				{
					m_paramInfos = value;
				}
			}

			public ParameterControlCollection ParamControls
			{
				get
				{
					return m_paramControls;
				}
				set
				{
					m_paramControls = value;
				}
			}

			public ParametersPaneLayout ParamLayout
			{
				get
				{
					return m_paramLayout;
				}
				set
				{
					m_paramLayout = value;
				}
			}

			public new void GetRowsAndColumnsToRender(out bool[] rowsToRender, out bool[] columnsToRender)
			{
				base.GetRowsAndColumnsToRender(out rowsToRender, out columnsToRender);
			}
		}

		private IContainer components;

		internal const int SEPARATOR = 5;

		internal const int MIN_LABEL_WIDTH = 80;

		internal const int MAX_LABEL_HEIGHT = 20;

		internal const int EDIT_CONTROL_WIDTH = 170;

		internal const int DROPDOWN_HEIGHT = 150;

		internal const int PADDING = 2;

		internal const int TOP_PADDING = 6;

		internal const int LEFT_PADDING = 6;

		internal const int EMPTY_COLUMN_WIDTH = 60;

		internal const int EMPTY_ROW_HEIGHT = 20;

		private int m_preferredHeight;

		private ToolTip toolTipForParams;

		private MirrorPanel buttonPanel;

		private Button viewReport;

		private Splitter splitterViewer;

		private MirrorPanel promptPanel;

		private ReportViewer m_currentViewerControl;

		private bool m_credentialsHaveChanged;

		private bool m_allCredentialsSatisfied;

		private List<DataSourcePromptControl> m_dataSourcePrompts = new List<DataSourcePromptControl>();

		private ReportParameterInfoCollection m_paramInfos;

		private ParametersPaneLayout m_paramLayout;

		private ParameterControlCollection m_paramControls = new ParameterControlCollection();

		private ColumnControlCollection m_leftColumnControls = new ColumnControlCollection();

		private LinkLabel linkChangeCredentials;

		private bool m_linkCredentialsVisible;

		private ColumnControlCollection m_rightColumnControls = new ColumnControlCollection();

		internal virtual ReportViewer ViewerControl
		{
			get
			{
				return m_currentViewerControl;
			}
			set
			{
				m_currentViewerControl = value;
			}
		}

		internal virtual Report Report => m_currentViewerControl.Report;

		public bool HaveContent
		{
			get
			{
				if (promptPanel.Controls.Count <= 0)
				{
					return m_linkCredentialsVisible;
				}
				return true;
			}
		}

		public int PreferredHeight => m_preferredHeight;

		public event EventHandler ViewButtonClick;

		public event ReportCredentialsEventHandler SubmitDataSourceCredentials;

		public event ReportParametersEventHandler SubmitParameters;

		public event EventHandler PreferredHeightChanged;

		public RSParams()
		{
			InitializeComponent();
			SetViewReportPosition();
			viewReport.NotifyDefault(value: true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		internal void ApplyCustomResources()
		{
			string changeCredentialsText = LocalizationHelper.Current.ChangeCredentialsText;
			if (changeCredentialsText != null)
			{
				linkChangeCredentials.Text = changeCredentialsText;
			}
			string viewReportButtonText = LocalizationHelper.Current.ViewReportButtonText;
			if (viewReportButtonText != null)
			{
				viewReport.Text = viewReportButtonText;
			}
			toolTipForParams.SetToolTip(viewReport, LocalizationHelper.Current.ViewReportButtonToolTip);
			SetViewReportPosition();
			foreach (DataSourcePromptControl dataSourcePrompt in m_dataSourcePrompts)
			{
				dataSourcePrompt.ApplyCustomResources();
			}
			foreach (ParameterControl value in m_paramControls.Values)
			{
				value.ApplyCustomResources();
			}
			promptPanel.PerformLayout();
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.WinForms.RSParams));
			buttonPanel = new Microsoft.Reporting.WinForms.MirrorPanel();
			splitterViewer = new System.Windows.Forms.Splitter();
			viewReport = new System.Windows.Forms.Button();
			linkChangeCredentials = new System.Windows.Forms.LinkLabel();
			promptPanel = new Microsoft.Reporting.WinForms.MirrorPanel();
			toolTipForParams = new System.Windows.Forms.ToolTip(components);
			buttonPanel.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(buttonPanel, "buttonPanel");
			buttonPanel.BackColor = System.Drawing.SystemColors.Control;
			buttonPanel.ForeColor = System.Drawing.SystemColors.ControlText;
			buttonPanel.Controls.Add(splitterViewer);
			buttonPanel.Controls.Add(viewReport);
			buttonPanel.Name = "buttonPanel";
			buttonPanel.TabStop = true;
			resources.ApplyResources(splitterViewer, "splitterViewer");
			splitterViewer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			splitterViewer.Name = "splitterViewer";
			splitterViewer.TabStop = false;
			resources.ApplyResources(viewReport, "viewReport");
			viewReport.Name = "viewReport";
			toolTipForParams.SetToolTip(viewReport, resources.GetString("viewReport.ToolTip"));
			viewReport.Click += new System.EventHandler(viewReport_Click);
			resources.ApplyResources(linkChangeCredentials, "linkChangeCredentials");
			linkChangeCredentials.Name = "linkChangeCredentials";
			linkChangeCredentials.TabStop = true;
			linkChangeCredentials.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(OnChangeCredentialsClicked);
			resources.ApplyResources(promptPanel, "promptPanel");
			promptPanel.Name = "promptPanel";
			promptPanel.TabStop = true;
			promptPanel.Layout += new System.Windows.Forms.LayoutEventHandler(PromptPanel_Layout);
			resources.ApplyResources(this, "$this");
			BackColor = System.Drawing.SystemColors.Control;
			base.Controls.Add(promptPanel);
			base.Controls.Add(linkChangeCredentials);
			base.Controls.Add(buttonPanel);
			base.Name = "RSParams";
			buttonPanel.ResumeLayout(false);
			buttonPanel.PerformLayout();
			ResumeLayout(false);
		}

		public void Clear()
		{
			promptPanel.Controls.Clear();
			m_dataSourcePrompts.Clear();
			m_allCredentialsSatisfied = false;
			m_credentialsHaveChanged = false;
			m_paramControls.Clear();
			m_leftColumnControls.Clear();
			m_rightColumnControls.Clear();
			toolTipForParams.RemoveAll();
			SetCredentialLinkVisibility(isVisible: false);
		}

		internal void EnsureParamsLoaded()
		{
			EnsureParamsLoaded(forceCredentialsShown: false, null);
		}

		private void EnsureParamsLoaded(bool forceCredentialsShown, ReportParameterInfoCollection parameterInfos)
		{
			try
			{
				promptPanel.SuspendLayout();
				Clear();
				ReportDataSourceInfoCollection dataSources = GetDataSources(out m_allCredentialsSatisfied);
				if (!m_allCredentialsSatisfied || forceCredentialsShown)
				{
					if (ViewerControl.ShowCredentialPrompts)
					{
						AddDataSourcePrompts(dataSources);
						return;
					}
					throw new MissingDataSourceCredentialsException();
				}
				CreateParameterPrompts(parameterInfos);
				if (ViewerControl.ShowParameterPrompts)
				{
					AddParameterPrompts();
					if (!m_paramControls.VisibleParameterNeedsValue && m_paramControls.HiddenParameterThatNeedsValue != null)
					{
						throw new MissingParameterException(m_paramControls.HiddenParameterThatNeedsValue);
					}
				}
				else
				{
					ValidateReportInputsSatisfied();
				}
				SetCredentialLinkVisibility(dataSources.Count > 0 && ViewerControl.ShowCredentialPrompts);
			}
			finally
			{
				SetViewReportPosition();
				try
				{
					m_paramInfos = Report.GetParameters();
					m_paramLayout = Report.GetParametersPaneLayout();
				}
				catch (Exception)
				{
				}
				promptPanel.ResumeLayout();
				promptPanel.AutoScroll = true;
			}
		}

		public void ValidateReportInputsSatisfied()
		{
			if (!m_allCredentialsSatisfied)
			{
				throw new MissingDataSourceCredentialsException();
			}
			if (m_paramControls != null && m_paramControls.AnyParameterThatNeedsValue != null)
			{
				throw new MissingParameterException(m_paramControls.AnyParameterThatNeedsValue);
			}
		}

		private void SetViewReportPosition()
		{
			buttonPanel.Width = splitterViewer.Width + viewReport.Width + 8;
			viewReport.Left = splitterViewer.Width + 4;
			viewReport.Top = 4;
			linkChangeCredentials.Left = viewReport.Left;
			linkChangeCredentials.Top = viewReport.Bottom + 7;
		}

		private ReportDataSourceInfoCollection GetDataSources(out bool allSatisfied)
		{
			if (ViewerControl.ProcessingMode == ProcessingMode.Remote)
			{
				return ViewerControl.ServerReport.GetDataSources(out allSatisfied);
			}
			allSatisfied = true;
			return new ReportDataSourceInfoCollection();
		}

		private void AddDataSourcePrompts(ReportDataSourceInfoCollection dsInfos)
		{
			foreach (ReportDataSourceInfo dsInfo in dsInfos)
			{
				DataSourcePromptControl dataSourcePromptControl = new DataSourcePromptControl(dsInfo, toolTipForParams);
				if (dataSourcePromptControl.PromptPanel != null)
				{
					promptPanel.Controls.Add(dataSourcePromptControl.PromptPanel);
				}
				promptPanel.Controls.Add(dataSourcePromptControl.UserNamePanel);
				m_leftColumnControls.Add(dataSourcePromptControl.UserNamePanel);
				promptPanel.Controls.Add(dataSourcePromptControl.PasswordPanel);
				m_rightColumnControls.Add(dataSourcePromptControl.PasswordPanel);
				dataSourcePromptControl.ValueChanged += OnCredentialsChanged;
				m_dataSourcePrompts.Add(dataSourcePromptControl);
			}
		}

		private void CreateParameterPrompts(ReportParameterInfoCollection parameterInfos)
		{
			ReportParameterInfoCollection reportParameterInfoCollection = parameterInfos;
			if (reportParameterInfoCollection == null)
			{
				reportParameterInfoCollection = Report.GetParameters();
			}
			ParametersPaneLayout parametersPaneLayout = Report.GetParametersPaneLayout();
			bool isQueryExecutionAllowed = true;
			if (ViewerControl.ProcessingMode == ProcessingMode.Remote)
			{
				isQueryExecutionAllowed = ViewerControl.ServerReport.IsQueryExecutionAllowed();
			}
			m_paramControls.Populate(reportParameterInfoCollection, isQueryExecutionAllowed, toolTipForParams, Font, parametersPaneLayout);
		}

		private void AddParameterPrompts()
		{
			foreach (ParameterControl value in m_paramControls.Values)
			{
				SetAutoPostbackOnDependencies(value.ParameterInfo);
				value.Width = 0;
				promptPanel.Controls.Add(value);
				if (m_leftColumnControls.Count == m_rightColumnControls.Count)
				{
					m_leftColumnControls.Add(value);
				}
				else
				{
					m_rightColumnControls.Add(value);
				}
			}
		}

		private void SetAutoPostbackOnDependencies(ReportParameterInfo p)
		{
			if (p.Dependencies == null)
			{
				return;
			}
			foreach (ReportParameterInfo dependency in p.Dependencies)
			{
				if (m_paramControls.TryGetValue(dependency.Name, out ParameterControl value) && !value.IsDependency)
				{
					value.IsDependency = true;
					value.ValueChanged += OnDependencyChanged;
				}
			}
		}

		private void viewReport_Click(object sender, EventArgs e)
		{
			if (!ValidateVisiblePrompts())
			{
				return;
			}
			bool flag = false;
			try
			{
				SaveControlValuesToReport(autoSubmit: false);
				flag = true;
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
			if (flag)
			{
				if (this.ViewButtonClick != null)
				{
					this.ViewButtonClick(this, EventArgs.Empty);
				}
				return;
			}
			try
			{
				EnsureParamsLoaded();
			}
			catch (Exception e3)
			{
				ViewerControl.UpdateUIState(e3);
			}
		}

		private bool ValidateVisiblePrompts()
		{
			foreach (DataSourcePromptControl dataSourcePrompt in m_dataSourcePrompts)
			{
				if (!dataSourcePrompt.Validate(this))
				{
					return false;
				}
			}
			foreach (ParameterControl value in m_paramControls.Values)
			{
				if (!value.Validate())
				{
					return false;
				}
			}
			return true;
		}

		private void OnChangeCredentialsClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				EnsureParamsLoaded(forceCredentialsShown: true, null);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		internal void SaveControlValuesToReport(bool autoSubmit)
		{
			if (m_credentialsHaveChanged)
			{
				DataSourceCredentialsCollection dataSourceCredentialsCollection = new DataSourceCredentialsCollection();
				foreach (DataSourcePromptControl dataSourcePrompt in m_dataSourcePrompts)
				{
					DataSourceCredentials dataSourceCredentials = new DataSourceCredentials();
					dataSourceCredentials.Name = dataSourcePrompt.DataSourceInfo.Name;
					dataSourceCredentials.UserId = dataSourcePrompt.UserName;
					dataSourceCredentials.Password = dataSourcePrompt.Password;
					dataSourceCredentialsCollection.Add(dataSourceCredentials);
				}
				ReportCredentialsEventArgs reportCredentialsEventArgs = new ReportCredentialsEventArgs(dataSourceCredentialsCollection);
				if (this.SubmitDataSourceCredentials != null)
				{
					this.SubmitDataSourceCredentials(this, reportCredentialsEventArgs);
				}
				if (!reportCredentialsEventArgs.Cancel && dataSourceCredentialsCollection.Count > 0 && ViewerControl.ProcessingMode == ProcessingMode.Remote)
				{
					ViewerControl.ServerReport.SetDataSourceCredentials(dataSourceCredentialsCollection);
				}
				return;
			}
			ReportParameterCollection reportParameterCollection = new ReportParameterCollection();
			foreach (ParameterControl value in m_paramControls.Values)
			{
				if (value.Enabled)
				{
					string[] currentValue = value.CurrentValue;
					if (currentValue != null)
					{
						ReportParameter reportParameter = new ReportParameter();
						reportParameter.Name = value.ParameterInfo.Name;
						reportParameter.Values.AddRange(currentValue);
						reportParameterCollection.Add(reportParameter);
					}
				}
			}
			if (this.SubmitParameters != null && reportParameterCollection.Count > 0)
			{
				ReportParametersEventArgs reportParametersEventArgs = new ReportParametersEventArgs(reportParameterCollection, autoSubmit);
				this.SubmitParameters(this, reportParametersEventArgs);
				if (!reportParametersEventArgs.Cancel && reportParameterCollection.Count > 0)
				{
					ViewerControl.Report.SetParameters(reportParameterCollection);
				}
			}
		}

		private bool HasDownstreamParametersWithDefaults(ReportParameterInfo rootParam)
		{
			foreach (ReportParameterInfo dependent in rootParam.Dependents)
			{
				if (dependent.AreDefaultValuesQueryBased && dependent.State != 0)
				{
					return true;
				}
			}
			return false;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if ((keyData & (Keys.KeyCode | Keys.Control | Keys.Alt)) == Keys.Return)
			{
				viewReport.Focus();
				viewReport.PerformClick();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		private void PromptPanel_Layout(object sender, LayoutEventArgs e)
		{
			foreach (Control control in promptPanel.Controls)
			{
				control.PerformLayout();
			}
			if (m_paramControls.HasLayout && m_paramControls.Count > 0)
			{
				PromptPanel_LayoutGrid(sender, e);
			}
			else
			{
				PromptPanel_LayoutTwoColumns(sender, e);
			}
		}

		private void PromptPanel_LayoutTwoColumns(object sender, LayoutEventArgs e)
		{
			int maxColumnWidth = 0;
			int maxColumnWidth2 = 0;
			int maxLabelWidth = 0;
			int maxLabelWidth2 = 0;
			m_leftColumnControls.GetMaxWidths(out maxLabelWidth, out maxColumnWidth);
			m_rightColumnControls.GetMaxWidths(out maxLabelWidth2, out maxColumnWidth2);
			int num = 0;
			int num2 = 0;
			int num3 = promptPanel.Width - (maxColumnWidth + maxColumnWidth2 + 30 + 160 + 6);
			if (num3 < 0)
			{
				num = 80;
				num2 = 80;
			}
			else
			{
				num = Math.Min(80 + num3 / 2 - 5, maxLabelWidth);
				num2 = Math.Min(80 + num3 / 2 - 5, maxLabelWidth2);
			}
			int num4 = maxColumnWidth + maxColumnWidth2 + 30 + num + num2;
			foreach (ParameterPanel control in promptPanel.Controls)
			{
				int internalLayout = (!m_leftColumnControls.Contains(control)) ? ((!m_rightColumnControls.Contains(control)) ? num4 : num2) : num;
				control.SetInternalLayout(internalLayout);
			}
			int num5 = 6 + maxColumnWidth + 25 + num;
			int num6 = promptPanel.AutoScrollPosition.Y + 6;
			ParameterPanel parameterPanel2 = null;
			foreach (ParameterPanel control2 in promptPanel.Controls)
			{
				control2.Top = num6;
				bool flag = m_rightColumnControls.Contains(control2);
				if (flag)
				{
					control2.Left = promptPanel.AutoScrollPosition.X + num5;
				}
				else
				{
					control2.Left = promptPanel.AutoScrollPosition.X + 6;
					parameterPanel2 = control2;
				}
				if (control2.Name == "_prompt" || flag)
				{
					num6 = Math.Max(control2.Bottom, parameterPanel2.Bottom) + 4;
				}
			}
			int num7 = 0;
			if (m_linkCredentialsVisible)
			{
				num7 = linkChangeCredentials.Height;
			}
			int val = 0;
			if (parameterPanel2 != null)
			{
				val = Math.Max(parameterPanel2.Bottom + 2, num6) + num7 + SystemInformation.HorizontalScrollBarHeight;
			}
			val = Math.Min(val, 200);
			val = (m_preferredHeight = Math.Max(val, viewReport.Bottom + 2));
			if (this.PreferredHeightChanged != null)
			{
				this.PreferredHeightChanged(this, EventArgs.Empty);
			}
		}

		private void GetRowsAndColumnsToRender(out bool[] rowsToRender, out bool[] columnsToRender)
		{
			ReportParameterInfoCollection paramInfos = m_paramInfos;
			GridLayoutDefinition gridLayoutDefinition = m_paramLayout.GridLayoutDefinition;
			ParameterControlCollection paramControls = m_paramControls;
			int num = -1;
			int num2 = -1;
			Dictionary<string, GridLayoutCellDefinition> dictionary = new Dictionary<string, GridLayoutCellDefinition>();
			bool?[] array = new bool?[gridLayoutDefinition.NumberOfRows];
			bool?[] array2 = new bool?[gridLayoutDefinition.NumberOfColumns];
			rowsToRender = new bool[gridLayoutDefinition.NumberOfRows];
			columnsToRender = new bool[gridLayoutDefinition.NumberOfColumns];
			foreach (GridLayoutCellDefinition cellDefinition in gridLayoutDefinition.CellDefinitions)
			{
				dictionary.Add(cellDefinition.ParameterName, cellDefinition);
			}
			foreach (ReportParameterInfo item in paramInfos)
			{
				GridLayoutCellDefinition gridLayoutCellDefinition = dictionary[item.Name];
				if (paramControls.ContainsKey(item.Name))
				{
					array[gridLayoutCellDefinition.Row] = false;
					array2[gridLayoutCellDefinition.Column] = false;
					num = Math.Max(gridLayoutCellDefinition.Row, num);
					num2 = Math.Max(gridLayoutCellDefinition.Column, num2);
					continue;
				}
				if (!array[gridLayoutCellDefinition.Row].HasValue)
				{
					array[gridLayoutCellDefinition.Row] = true;
				}
				if (!array2[gridLayoutCellDefinition.Column].HasValue)
				{
					array2[gridLayoutCellDefinition.Column] = true;
				}
			}
			for (int i = 0; i < gridLayoutDefinition.NumberOfRows; i++)
			{
				if (i > num)
				{
					rowsToRender[i] = false;
				}
				else
				{
					rowsToRender[i] = (array[i] != true);
				}
			}
			for (int j = 0; j < gridLayoutDefinition.NumberOfColumns; j++)
			{
				if (j > num2)
				{
					columnsToRender[j] = false;
				}
				else
				{
					columnsToRender[j] = (array2[j] != true);
				}
			}
		}

		private void PromptPanel_LayoutGrid(object sender, LayoutEventArgs e)
		{
			List<ParameterControl> list = m_paramControls.Values.ToList();
			List<ColumnControlCollection> list2 = new List<ColumnControlCollection>();
			GetRowsAndColumnsToRender(out bool[] rowsToRender, out bool[] columnsToRender);
			int col2;
			for (col2 = 0; col2 < m_paramControls.NumberOfCols; col2++)
			{
				list2.Add(new ColumnControlCollection(list.Where((ParameterControl p) => p.CellDefinition.Column == col2).OfType<ParameterPanel>()));
			}
			var columnSpaces = list2.Select(delegate(ColumnControlCollection c)
			{
				int maxLabelWidth = 0;
				int maxColumnWidth = 0;
				c.GetMaxWidths(out maxLabelWidth, out maxColumnWidth);
				return new
				{
					LabelWidth = maxLabelWidth,
					ColumnWidth = maxColumnWidth
				};
			}).ToList();
			int column = 0;
			list2.ForEach(delegate(ColumnControlCollection c)
			{
				c.ForEach(delegate(ParameterPanel p)
				{
					p.SetInternalLayout(columnSpaces[column].LabelWidth);
				});
				column++;
			});
			List<int> rowHeights = new List<int>(m_paramControls.NumberOfRows);
			int row;
			for (row = 0; row < m_paramControls.NumberOfRows; row++)
			{
				if (rowsToRender[row])
				{
					IEnumerable<ParameterControl> source = list.Where((ParameterControl p) => p.CellDefinition.Row == row);
					int num = (source.Count() > 0) ? source.Max((ParameterControl p) => p.Height) : 20;
					rowHeights.Add(num + 2);
				}
				else
				{
					rowHeights.Add(0);
				}
			}
			List<int> colWidths = new List<int>(m_paramControls.NumberOfCols);
			int col;
			for (col = 0; col < m_paramControls.NumberOfCols; col++)
			{
				if (columnsToRender[col])
				{
					IEnumerable<ParameterControl> source2 = list.Where((ParameterControl p) => p.CellDefinition.Column == col);
					int num2 = (source2.Count() > 0) ? source2.Max((ParameterControl p) => p.Width) : 60;
					colWidths.Add(num2 + 10);
				}
				else
				{
					colWidths.Add(0);
				}
			}
			list.ForEach(delegate(ParameterControl p)
			{
				p.Top = rowHeights.Take(p.CellDefinition.Row).Sum() + 6;
				p.Left = colWidths.Take(p.CellDefinition.Column).Sum() + 6;
			});
			int num3 = 0;
			if (m_linkCredentialsVisible)
			{
				num3 = linkChangeCredentials.Height;
			}
			int num4 = 0;
			num4 = (new int?(rowHeights.Sum()) ?? 0) + 6 + num3 + SystemInformation.HorizontalScrollBarHeight;
			num4 = Math.Min(num4, 200);
			num4 = (m_preferredHeight = Math.Max(num4, viewReport.Bottom + 2));
			if (this.PreferredHeightChanged != null)
			{
				this.PreferredHeightChanged(this, EventArgs.Empty);
			}
		}

		private void OnDependencyChanged(object sender, EventArgs e)
		{
			try
			{
				SaveControlValuesToReport(autoSubmit: true);
				ParameterControl parameterControl = (ParameterControl)sender;
				ReportParameterInfoCollection reportParameterInfoCollection = ViewerControl.Report.GetParameters();
				if (reportParameterInfoCollection[parameterControl.ParameterInfo.Name].HasUnsatisfiedDownstreamParametersWithDefaults)
				{
					ViewerControl.Report.SetParameters(new ReportParameter[0]);
					reportParameterInfoCollection = null;
				}
				EnsureParamsLoaded(forceCredentialsShown: false, reportParameterInfoCollection);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnCredentialsChanged(object sender, EventArgs e)
		{
			m_credentialsHaveChanged = true;
		}

		private void SetCredentialLinkVisibility(bool isVisible)
		{
			m_linkCredentialsVisible = isVisible;
			linkChangeCredentials.Visible = isVisible;
		}
	}
}
