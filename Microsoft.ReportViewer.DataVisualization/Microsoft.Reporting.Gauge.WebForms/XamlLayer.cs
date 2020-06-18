using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class XamlLayer : IDisposable
	{
		private GraphicsPath[] paths;

		private Brush[] brushes;

		private Pen[] pens;

		private XamlLayer[] innerLayers;

		private bool disposed;

		public GraphicsPath[] Paths
		{
			get
			{
				return paths;
			}
			set
			{
				if (paths != value && paths != null)
				{
					GraphicsPath[] array = paths;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				paths = value;
			}
		}

		public Brush[] Brushes
		{
			get
			{
				return brushes;
			}
			set
			{
				if (brushes != value && brushes != null)
				{
					Brush[] array = brushes;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				brushes = value;
			}
		}

		public Pen[] Pens
		{
			get
			{
				return pens;
			}
			set
			{
				if (pens != value && pens != null)
				{
					Pen[] array = pens;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				pens = value;
			}
		}

		public XamlLayer[] InnerLayers
		{
			get
			{
				return innerLayers;
			}
			set
			{
				if (innerLayers != value && innerLayers != null)
				{
					XamlLayer[] array = innerLayers;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				innerLayers = value;
			}
		}

		public void Render(GaugeGraphics g)
		{
			if (InnerLayers != null)
			{
				for (int i = 0; i < InnerLayers.Length; i++)
				{
					InnerLayers[i].Render(g);
				}
			}
			if (Paths == null)
			{
				return;
			}
			for (int j = 0; j < Paths.Length; j++)
			{
				if (Brushes[j] != null)
				{
					g.FillPath(Brushes[j], Paths[j]);
				}
				if (Pens[j] != null)
				{
					g.DrawPath(Pens[j], Paths[j]);
				}
			}
		}

		public void SetSingleBrush(Brush brush)
		{
			Brushes = new Brush[Paths.Length];
			for (int i = 0; i < Brushes.Length; i++)
			{
				Brushes[i] = (Brush)brush.Clone();
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed && disposing)
			{
				Paths = null;
				Brushes = null;
				Pens = null;
				InnerLayers = null;
			}
			disposed = true;
		}

		~XamlLayer()
		{
			Dispose(disposing: false);
		}
	}
}
