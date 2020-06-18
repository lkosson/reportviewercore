using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.CommonControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class DateControl : ParameterControl
	{
		private bool m_changed;

		private TextBoxWithImage m_textbox;

		private GenericDropDown m_dropDown;

		private MonthCalendar m_monthCalendar;

		private TimeSpan m_savedTime = TimeSpan.MinValue;

		private TimeSpan? m_savedTimeOffset;

		private readonly TimeSpan _Midnight = new TimeSpan(0L);

		protected override int MinimumRequiredHeight => m_textbox.Height;

		public override string[] CurrentValue
		{
			get
			{
				if (m_nullCheckBox != null && m_nullCheckBox.Checked)
				{
					return new string[1];
				}
				if (m_textbox.Text.Trim().Length > 0)
				{
					return new string[1]
					{
						m_textbox.Text
					};
				}
				return null;
			}
		}

		public DateControl(ReportParameterInfo paramInfo, ToolTip tooltip, Font font, bool allowQueryExecution, GridLayoutCellDefinition cellDefinition)
			: base(paramInfo, tooltip, font, allowQueryExecution, cellDefinition)
		{
		}

		protected override void CreateChildControls()
		{
			m_textbox = new TextBoxWithImage(base.ParameterInfo.Name, "Microsoft.Reporting.WinForms.Resources.16cal.gif");
			m_textbox.Width = 170;
			m_textbox.ImageClick += OnDropDown;
			m_textbox.AutoSize = true;
			m_monthCalendar = new MonthCalendar();
			m_monthCalendar.MaxSelectionCount = 1;
			m_monthCalendar.MouseWheel += m_monthCalendar_MouseWheel;
			m_monthCalendar.DateSelected += m_monthCalendar_DateSelected;
			m_monthCalendar.KeyPress += m_monthCalendar_KeyPress;
			m_monthCalendar.RightToLeftLayout = true;
			m_dropDown = new GenericDropDown();
			m_dropDown.TopControl = m_textbox;
			m_dropDown.DropDownControl = m_monthCalendar;
			m_dropDown.DropDownControl.AutoSize = false;
			m_dropDown.DropDownControl.MinimumSize = m_monthCalendar.MinimumSize;
			m_dropDown.DropDownControl.MaximumSize = m_monthCalendar.MaximumSize;
			m_dropDown.DropDownClosed += m_monthCalendar_DateSelected;
			int height = m_textbox.Height;
			m_textbox.AutoSize = false;
			m_textbox.Height = height;
			if (m_paramInfo.Values.Count > 0 && base.Enabled && DateTimeUtil.TryParseDateTime(m_paramInfo.Values[0], null, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset))
			{
				if (hasTimeOffset)
				{
					SetTextBoxValue(dateTimeOffset);
				}
				else
				{
					SetTextBoxValue(dateTimeOffset.DateTime);
				}
			}
			m_textbox.TextChanged += m_textbox_TextChanged;
			base.Validated += OnDateControlValidated;
			base.Controls.Add(m_dropDown);
			if (m_paramInfo.Nullable)
			{
				RenderNull();
				base.NullValueChanged += SetEnabledState;
			}
			SetEnabledState(null, null);
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			if (m_monthCalendar != null)
			{
				m_monthCalendar.RightToLeft = RightToLeft;
			}
		}

		protected override void InternalApplyCustomResources()
		{
			base.InternalApplyCustomResources();
			m_textbox.SetTooltip(m_tooltip, LocalizationHelper.Current.DateToolTip);
		}

		private void m_monthCalendar_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\n' || e.KeyChar == '\r')
			{
				OnDateSelected();
			}
		}

		private void m_monthCalendar_DateSelected(object sender, EventArgs e)
		{
			OnDateSelected();
		}

		private void OnDateSelected()
		{
			UpdateSummaryStringDisplay();
			m_dropDown.CloseDropDown();
			NotifyIfValueChanged();
		}

		private void SetEnabledState(object sender, EventArgs e)
		{
			m_textbox.Enabled = (m_nullCheckBox == null || !m_nullCheckBox.Checked);
		}

		private void UpdateSummaryStringDisplay()
		{
			DateTime dateTime = m_monthCalendar.SelectionStart.Date;
			if (m_savedTime != TimeSpan.MinValue)
			{
				dateTime = dateTime.Add(m_savedTime);
			}
			if (m_savedTimeOffset.HasValue)
			{
				SetTextBoxValue(new DateTimeOffset(dateTime, m_savedTimeOffset.Value));
			}
			else
			{
				SetTextBoxValue(dateTime);
			}
		}

		private void SetTextBoxValue(DateTime date)
		{
			if (date.TimeOfDay == _Midnight)
			{
				m_textbox.Text = date.ToShortDateString();
			}
			else
			{
				m_textbox.Text = date.ToString();
			}
		}

		private void SetTextBoxValue(DateTimeOffset date)
		{
			m_textbox.Text = date.ToString();
		}

		private void OnDropDown(object sender, EventArgs e)
		{
			m_savedTime = TimeSpan.MinValue;
			m_savedTimeOffset = null;
			if (!string.IsNullOrEmpty(m_textbox.Text) && DateTimeUtil.TryParseDateTime(m_textbox.Text, null, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset))
			{
				m_savedTime = dateTimeOffset.TimeOfDay;
				if (hasTimeOffset)
				{
					m_savedTimeOffset = dateTimeOffset.Offset;
				}
				if (IsSelectableDate(dateTimeOffset.Date))
				{
					m_monthCalendar.SelectionStart = dateTimeOffset.Date;
				}
			}
			m_dropDown.OpenDropDown();
		}

		private bool IsSelectableDate(DateTime testDate)
		{
			if (testDate >= m_monthCalendar.MinDate)
			{
				return testDate < m_monthCalendar.MaxDate;
			}
			return false;
		}

		private void m_monthCalendar_MouseWheel(object sender, MouseEventArgs e)
		{
			DateTime dateTime = m_monthCalendar.SelectionStart.AddMonths((e.Delta <= 0) ? 1 : (-1));
			if (IsSelectableDate(dateTime))
			{
				m_monthCalendar.SelectionStart = dateTime;
			}
		}

		private void m_textbox_TextChanged(object sender, EventArgs e)
		{
			m_changed = true;
		}

		private void OnDateControlValidated(object sender, EventArgs e)
		{
			NotifyIfValueChanged();
		}

		private void NotifyIfValueChanged()
		{
			if (m_changed)
			{
				OnValueChanged(this, EventArgs.Empty);
			}
			m_changed = false;
		}
	}
}
