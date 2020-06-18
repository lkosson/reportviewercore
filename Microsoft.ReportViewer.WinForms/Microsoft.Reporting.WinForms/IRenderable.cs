using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal interface IRenderable
	{
		bool CanRender
		{
			get;
		}

		void RenderToGraphics(Graphics g);
	}
}
