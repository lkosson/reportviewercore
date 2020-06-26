using System.Drawing;

namespace Microsoft.Reporting.NETCore
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
