using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class ParameterLabel : Label
	{
		public int OneLineWidth
		{
			get
			{
				using (Graphics graphics = CreateGraphics())
				{
					return (int)Math.Ceiling(graphics.MeasureString(Text, Font).Width);
				}
			}
		}

		public ParameterLabel(string text)
		{
			Text = text;
			base.Width = 80;
			TextAlign = ContentAlignment.TopLeft;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle clientRectangle = base.ClientRectangle;
			StringFormat format = new StringFormat(StringFormat.GenericDefault)
			{
				LineAlignment = StringAlignment.Center
			};
			if (base.Parent != null)
			{
				clientRectangle = base.Parent.ClientRectangle;
			}
			e.Graphics.DrawString(Text, Font, SystemBrushes.WindowText, clientRectangle, format);
		}

		public void SetRequiredHeight()
		{
			using (Graphics graphics = CreateGraphics())
			{
				base.Height = (int)Math.Ceiling(graphics.MeasureString(layoutArea: new SizeF(base.Width, 20f * (float)Font.Height), text: Text, font: Font, stringFormat: StringFormat.GenericDefault).Height + 2f);
			}
		}
	}
}
