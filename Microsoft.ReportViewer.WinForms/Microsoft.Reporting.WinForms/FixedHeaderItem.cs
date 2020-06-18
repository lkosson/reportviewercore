using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class FixedHeaderItem
	{
		internal enum LayoutType
		{
			Empty,
			Horizontal,
			Vertical,
			Corner
		}

		internal List<Action> Actions;

		internal RectangleF Bounds;

		internal RenderingItemContainer Container;

		internal bool FoundActions;

		internal LayoutType Layout;

		internal List<RenderingItem> RenderingItems = new List<RenderingItem>();

		internal FixedHeaderItem(RenderingItemContainer container, RectangleF bounds, LayoutType layout)
		{
			Container = container;
			Bounds = bounds;
			Layout = layout;
		}

		internal void AddAction(Action action)
		{
			if (Actions == null)
			{
				Actions = new List<Action>();
			}
			Actions.Add(action);
		}
	}
}
