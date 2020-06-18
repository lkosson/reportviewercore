using System;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapObject : IDisposable
	{
		internal bool initialized = true;

		private object parent;

		private CommonElements common;

		protected bool disposed;

		internal virtual object Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		internal virtual CommonElements Common
		{
			get
			{
				if (common == null)
				{
					object obj = Parent;
					if (obj is MapObject)
					{
						common = ((MapObject)obj).Common;
					}
					else if (obj is NamedElement)
					{
						common = ((NamedElement)obj).Common;
					}
					else if (obj is NamedCollection)
					{
						common = ((NamedCollection)obj).Common;
					}
					else if (obj is MapCore)
					{
						common = ((MapCore)obj).Common;
					}
				}
				return common;
			}
			set
			{
				common = value;
			}
		}

		internal MapObject(object parent)
		{
			this.parent = parent;
		}

		internal virtual void Invalidate()
		{
			if (Common != null)
			{
				Common.MapCore.Invalidate();
			}
		}

		internal virtual void Invalidate(RectangleF rect)
		{
			if (Common != null)
			{
				Common.MapCore.Invalidate(rect);
			}
		}

		internal virtual void InvalidateViewport(bool invalidateGridSections)
		{
			if (Common != null)
			{
				Common.MapCore.InvalidateViewport(invalidateGridSections);
			}
		}

		internal virtual void InvalidateDistanceScalePanel()
		{
			if (Common != null)
			{
				Common.MapCore.InvalidateDistanceScalePanel();
			}
		}

		internal virtual void InvalidateViewport()
		{
			if (Common != null)
			{
				Common.MapCore.InvalidateViewport(invalidateGridSections: true);
			}
		}

		internal virtual void BeginInit()
		{
			initialized = false;
		}

		internal virtual void EndInit()
		{
			initialized = true;
		}

		internal virtual void ReconnectData(bool exact)
		{
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
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
				OnDispose();
			}
			disposed = true;
		}

		protected virtual void OnDispose()
		{
		}
	}
}
