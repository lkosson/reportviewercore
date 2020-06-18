using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class NamedElement : IDisposable, ICloneable
	{
		private string name = string.Empty;

		[NonSerialized]
		internal CommonElements common;

		[NonSerialized]
		internal NamedCollection collection;

		internal bool initialized = true;

		private object tag;

		internal bool disposed;

		[Browsable(false)]
		[SRDescription("DescriptionAttributeCommon")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal virtual CommonElements Common
		{
			get
			{
				return common;
			}
			set
			{
				if (common != value)
				{
					if (value == null)
					{
						OnRemove();
						common = value;
					}
					else
					{
						common = value;
						OnAdded();
					}
				}
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeCollection")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual NamedCollection Collection
		{
			get
			{
				return collection;
			}
			set
			{
				collection = value;
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeParentElement")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual NamedElement ParentElement
		{
			get
			{
				if (collection != null)
				{
					return collection.ParentElement;
				}
				return null;
			}
		}

		internal virtual string DefaultName => string.Empty;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeName10")]
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				_ = name;
				if (collection != null)
				{
					collection.IsValidNameCheck(value, this);
				}
				if (Common != null)
				{
					Common.GaugeCore.Notify(MessageType.NamedElementRename, this, value);
				}
				name = value;
				OnNameChanged();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		public NamedElement()
		{
		}

		protected NamedElement(string name)
		{
			this.name = name;
		}

		internal virtual void OnRemove()
		{
			if (Common != null)
			{
				Common.GaugeCore.Notify(MessageType.NamedElementRemove, this, null);
			}
		}

		internal virtual void OnAdded()
		{
			if (Common != null)
			{
				Common.GaugeCore.Notify(MessageType.NamedElementAdded, this, null);
			}
		}

		internal virtual void OnNameChanged()
		{
		}

		internal virtual void BeginInit()
		{
			initialized = false;
		}

		internal virtual void EndInit()
		{
			initialized = true;
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

		internal virtual void ReconnectData(bool exact)
		{
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
		}

		internal string GetNameAsParent()
		{
			return GetNameAsParent(Name);
		}

		internal string GetNameAsParent(string newName)
		{
			if (Collection != null)
			{
				return Collection.GetCollectionName() + "." + newName;
			}
			return newName;
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

		public virtual object Clone()
		{
			return CloneInternals(InitiateCopy());
		}

		internal virtual object InitiateCopy()
		{
			return MemberwiseClone();
		}

		internal virtual object CloneInternals(object copy)
		{
			return copy;
		}
	}
}
