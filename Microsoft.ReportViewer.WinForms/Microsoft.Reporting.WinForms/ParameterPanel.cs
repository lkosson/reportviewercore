using System;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class ParameterPanel : MirrorPanel
	{
		private ParameterLabel m_containedLabel;

		public int NonLabelWidth
		{
			get
			{
				int num = 0;
				foreach (Control control in base.Controls)
				{
					if (!(control is ParameterLabel))
					{
						num += control.Width;
					}
					num += 7;
				}
				return num;
			}
		}

		protected virtual int MinimumRequiredHeight
		{
			get
			{
				int num = 0;
				foreach (Control control in base.Controls)
				{
					num = Math.Max(num, control.Height);
				}
				return num;
			}
		}

		public ParameterLabel ContainedLabel => m_containedLabel;

		public void SetInternalLayout(int labelWidth)
		{
			if (!base.Created)
			{
				return;
			}
			ContainedLabel.Width = labelWidth;
			ContainedLabel.SetRequiredHeight();
			int left = 2;
			int num = 0;
			foreach (Control control in base.Controls)
			{
				control.Left = left;
				control.Top = 0;
				left = control.Right + 5;
				num += control.Width + 5;
			}
			base.Width = num;
			base.Height = Math.Max(MinimumRequiredHeight, ContainedLabel.Height);
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			ParameterLabel parameterLabel = e.Control as ParameterLabel;
			if (parameterLabel != null)
			{
				m_containedLabel = parameterLabel;
			}
		}
	}
}
