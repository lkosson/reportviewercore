using System;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ResizeEventArgs : EventArgs
	{
		private int m_deltaX;

		private int m_deltaY;

		internal int DeltaX => m_deltaX;

		internal int DeltaY => m_deltaY;

		internal ResizeEventArgs(int deltaX, int deltaY)
		{
			m_deltaX = deltaX;
			m_deltaY = deltaY;
		}
	}
}
