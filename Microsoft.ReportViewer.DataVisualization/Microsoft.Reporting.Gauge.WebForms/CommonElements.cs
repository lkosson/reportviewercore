using System;
using System.ComponentModel.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CommonElements
	{
		private GaugeGraphics graph;

		internal IServiceContainer container;

		internal bool processModePaint = true;

		internal bool processModeRegions;

		internal ObjectLinker objectLinker;

		private int width;

		private int height;

		private GaugeCore gaugeCore;

		internal ImageLoader ImageLoader => (ImageLoader)container.GetService(typeof(ImageLoader));

		internal GaugeCore GaugeCore
		{
			get
			{
				if (gaugeCore == null)
				{
					gaugeCore = (GaugeCore)container.GetService(typeof(GaugeCore));
				}
				return gaugeCore;
			}
		}

		internal GaugeContainer GaugeContainer => GaugeCore.GaugeContainer;

		internal GaugeGraphics Graph
		{
			get
			{
				if (graph != null)
				{
					return graph;
				}
				throw new ApplicationException(Utils.SRGetStr("ExceptionGdiNonInitialized"));
			}
			set
			{
				graph = value;
			}
		}

		internal ObjectLinker ObjectLinker => objectLinker;

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

		internal CommonElements(IServiceContainer container)
		{
			this.container = container;
			objectLinker = new ObjectLinker(GaugeCore);
			objectLinker.Common = this;
		}

		internal void InvokePrePaint(object sender)
		{
			GaugeContainer.OnPrePaint(sender, new GaugePaintEventArgs(GaugeContainer, graph));
		}

		internal void InvokePostPaint(object sender)
		{
			GaugeContainer.OnPostPaint(sender, new GaugePaintEventArgs(GaugeContainer, graph));
		}
	}
}
