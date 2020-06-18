using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeObject : IDisposable
	{
		internal bool initialized = true;

		private object parent;

		private CommonElements common;

		private bool disposed;

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
					if (obj is GaugeObject)
					{
						common = ((GaugeObject)obj).Common;
					}
					else if (obj is NamedElement)
					{
						common = ((NamedElement)obj).Common;
					}
					else if (obj is NamedCollection)
					{
						common = ((NamedCollection)obj).Common;
					}
					else if (obj is GaugeCore)
					{
						common = ((GaugeCore)obj).Common;
					}
				}
				return common;
			}
			set
			{
				common = value;
			}
		}

		internal GaugeObject(object parent)
		{
			this.parent = parent;
		}

		internal virtual void Invalidate()
		{
			if (Common != null)
			{
				Common.GaugeCore.Invalidate();
			}
		}

		internal virtual void Refresh()
		{
			if (Common != null)
			{
				Common.GaugeCore.Refresh();
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
