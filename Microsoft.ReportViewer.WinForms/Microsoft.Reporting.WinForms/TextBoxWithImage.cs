using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class TextBoxWithImage : MirrorPanel
	{
		private int m_imageSpacing = 2;

		private TextBox m_textBox = new TextBox();

		private PictureBox m_pictureBox = new PictureBox();

		public override string Text
		{
			get
			{
				return m_textBox.Text;
			}
			set
			{
				m_textBox.Text = value;
			}
		}

		public event EventHandler ImageClick;

		public TextBoxWithImage(string accessibleName, string imageResource)
		{
			Image image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResource));
			m_pictureBox.Image = image;
			m_pictureBox.Width = image.Width;
			m_pictureBox.Height = image.Height;
			m_pictureBox.Click += m_pictureBox_Click;
			m_pictureBox.AccessibleName = ReportPreviewStrings.DropDownIconAccessibleName(accessibleName);
			m_textBox.TextChanged += m_textBox_TextChanged;
			m_textBox.AccessibleName = accessibleName;
			base.Controls.Add(m_textBox);
			base.Controls.Add(m_pictureBox);
			base.Height = Math.Max(m_textBox.Height + 2, m_pictureBox.Height);
			base.Width = 170;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			m_textBox.Width = base.Width - m_pictureBox.Width - m_imageSpacing;
			Point location = new Point(m_textBox.Width + m_imageSpacing);
			m_pictureBox.Location = location;
			base.OnLayout(levent);
		}

		private void OnImageClick()
		{
			if (this.ImageClick != null)
			{
				this.ImageClick(this, EventArgs.Empty);
			}
		}

		private void m_pictureBox_Click(object sender, EventArgs e)
		{
			OnImageClick();
		}

		private void m_textBox_TextChanged(object sender, EventArgs e)
		{
			OnTextChanged(e);
		}

		public void SetTooltip(ToolTip tooltip, string tooltipStr)
		{
			tooltip.SetToolTip(m_textBox, tooltipStr);
			tooltip.SetToolTip(m_pictureBox, tooltipStr);
		}
	}
}
