using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class SegmentsCache
	{
		private Hashtable cacheTable = new Hashtable();

		private Matrix matrix = new Matrix();

		private float size = -1f;

		internal GraphicsPath GetSegment(Enum segments, PointF p, float size)
		{
			CheckCache(size);
			if (cacheTable.Contains(segments))
			{
				GraphicsPath obj = (GraphicsPath)((GraphicsPath)cacheTable[segments]).Clone();
				matrix.Reset();
				matrix.Translate(p.X, p.Y);
				obj.Transform(matrix);
				return obj;
			}
			return null;
		}

		internal void Reset()
		{
			foreach (object value in cacheTable.Values)
			{
				if (value is IDisposable)
				{
					((IDisposable)value).Dispose();
				}
			}
			cacheTable.Clear();
		}

		private void CheckCache(float size)
		{
			if (Math.Abs(this.size - size) > float.Epsilon)
			{
				Reset();
				this.size = size;
			}
		}

		internal void SetSegment(Enum segment, GraphicsPath path, PointF p, float size)
		{
			CheckCache(size);
			GraphicsPath graphicsPath = (GraphicsPath)path.Clone();
			matrix.Reset();
			matrix.Translate(0f - p.X, 0f - p.Y);
			graphicsPath.Transform(matrix);
			cacheTable[segment] = graphicsPath;
		}
	}
}
