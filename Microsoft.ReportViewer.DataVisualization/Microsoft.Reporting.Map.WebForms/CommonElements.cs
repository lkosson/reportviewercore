using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CommonElements
	{
		private MapGraphics graph;

		internal IServiceContainer container;

		internal bool processModePaint = true;

		internal bool processModeRegions;

		private int width;

		private int height;

		private MapCore mapCore;

		internal Size Size => new Size(width, height);

		internal bool ProcessModePaint => processModePaint;

		internal bool ProcessModeRegions => processModeRegions;

		internal ImageLoader ImageLoader => (ImageLoader)container.GetService(typeof(ImageLoader));

		internal MapCore MapCore
		{
			get
			{
				if (mapCore == null)
				{
					mapCore = (MapCore)container.GetService(typeof(MapCore));
				}
				return mapCore;
			}
		}

		internal MapControl MapControl => MapCore.MapControl;

		internal bool IsGraphicsInitialized => graph != null;

		internal MapGraphics Graph
		{
			get
			{
				if (graph != null)
				{
					return graph;
				}
				throw new ApplicationException(SR.gdi_noninitialized);
			}
			set
			{
				graph = value;
			}
		}

		internal int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		internal int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		internal BorderTypeRegistry BorderTypeRegistry => (BorderTypeRegistry)container.GetService(typeof(BorderTypeRegistry));

		internal CommonElements(IServiceContainer container)
		{
			this.container = container;
		}

		internal void InvokePrePaint(NamedElement sender)
		{
			MapControl.OnPrePaint(MapControl, new MapPaintEventArgs(MapControl, sender, graph));
		}

		internal void InvokePostPaint(NamedElement sender)
		{
			MapControl.OnPostPaint(MapControl, new MapPaintEventArgs(MapControl, sender, graph));
		}

		internal void InvokeElementAdded(NamedElement sender)
		{
			MapControl.OnElementAdded(MapControl, new ElementEventArgs(MapControl, sender));
		}

		internal void InvokeElementRemoved(NamedElement sender)
		{
			MapControl.OnElementRemoved(MapControl, new ElementEventArgs(MapControl, sender));
		}

		internal static double ParseDouble(string stringToParse)
		{
			double num = 0.0;
			try
			{
				return double.Parse(stringToParse, CultureInfo.InvariantCulture);
			}
			catch
			{
				return double.Parse(stringToParse, CultureInfo.CurrentCulture);
			}
		}

		internal static float ParseFloat(string stringToParse)
		{
			float num = 0f;
			try
			{
				return float.Parse(stringToParse, CultureInfo.InvariantCulture);
			}
			catch
			{
				return float.Parse(stringToParse, CultureInfo.CurrentCulture);
			}
		}
	}
}
