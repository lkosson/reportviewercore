using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class AsyncWaitControl : AlphaPanel, IDisposable
	{
		private AsyncWaitMessage m_waitMessage;

		public override Font Font
		{
			get
			{
				return m_waitMessage.Font;
			}
			set
			{
				m_waitMessage.Font = value;
			}
		}

		public override Color BackColor
		{
			get
			{
				if (m_waitMessage != null)
				{
					return m_waitMessage.BackColor;
				}
				return Color.Empty;
			}
			set
			{
				if (m_waitMessage != null)
				{
					m_waitMessage.BackColor = value;
				}
			}
		}

		public AsyncWaitControl(IRenderable renderable)
			: base(renderable)
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AutoSize = true;
			m_waitMessage = new Microsoft.Reporting.WinForms.AsyncWaitMessage();
			m_waitMessage.AutoSize = true;
			m_waitMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			m_waitMessage.BackColor = System.Drawing.SystemColors.Control;
			base.Controls.Add(m_waitMessage);
			m_waitMessage.CenterToParent();
			Cursor = System.Windows.Forms.Cursors.WaitCursor;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			m_waitMessage.CenterToParent();
		}

		internal void ApplyCustomResources()
		{
			m_waitMessage.ApplyCustomResources();
		}
	}
}
