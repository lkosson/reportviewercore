using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting.Map.WebForms
{
	[ListBindable(false)]
	internal class NamedCollection : CollectionBase, IDisposable, ICloneable
	{
		internal CommonElements common;

		internal Type elementType = typeof(NamedElement);

		internal NamedElement parent;

		internal bool editModeActive;

		private Hashtable nameToObject;

		private Hashtable nameToIndex;

		private int suspendUpdatesCount;

		private bool suppressAddedAndRemovedEvents;

		protected bool disposed;

		public NamedElement ParentElement => parent;

		internal CommonElements Common
		{
			get
			{
				return common;
			}
			set
			{
				common = value;
				IEnumerator enumerator = GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						((NamedElement)enumerator.Current).Common = common;
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal bool SuppressAddedAndRemovedEvents
		{
			get
			{
				return suppressAddedAndRemovedEvents;
			}
			set
			{
				suppressAddedAndRemovedEvents = true;
			}
		}

		internal bool IsSuspended => suspendUpdatesCount > 0;

		private NamedCollection()
		{
		}

		internal NamedCollection(NamedElement parent, CommonElements common)
		{
			Common = common;
			this.parent = parent;
			nameToObject = new Hashtable();
			nameToIndex = new Hashtable();
		}

		public int GetIndex(string name)
		{
			if (nameToIndex.ContainsKey(name))
			{
				return (int)nameToIndex[name];
			}
			return -1;
		}

		public NamedElement GetByName(string name)
		{
			if (nameToObject.ContainsKey(name))
			{
				return (NamedElement)nameToObject[name];
			}
			return null;
		}

		public NamedElement GetByIndex(int index)
		{
			return (NamedElement)base.List[index];
		}

		public virtual int IndexOf(object o)
		{
			return base.List.IndexOf(o);
		}

		internal void SuspendUpdates()
		{
			suspendUpdatesCount++;
		}

		internal void ResumeUpdates()
		{
			if (suspendUpdatesCount > 0)
			{
				suspendUpdatesCount--;
			}
		}

		protected override void OnClear()
		{
			if (!IsSuspended)
			{
				while (base.Count > 0)
				{
					RemoveAt(0);
				}
			}
			nameToObject.Clear();
			nameToIndex.Clear();
			base.OnClear();
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
			if (!IsSuspended)
			{
				CheckForTypeDublicatesAndName(value);
			}
			nameToObject.Add(((NamedElement)value).Name, value);
			nameToIndex.Add(((NamedElement)value).Name, index);
		}

		protected override void OnRemove(int index, object value)
		{
			base.OnRemove(index, value);
			nameToObject.Remove(((NamedElement)value).Name);
			nameToIndex.Remove(((NamedElement)value).Name);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((NamedElement)value).Collection = this;
			((NamedElement)value).Common = common;
			Invalidate();
			base.OnInsertComplete(index, value);
			if (Common != null && !SuppressAddedAndRemovedEvents)
			{
				Common.InvokeElementAdded((NamedElement)value);
			}
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			if (Common != null)
			{
				Common.MapCore.HotRegionList.RemoveHotRegionOfObject(value);
				Common.InvokeElementRemoved((NamedElement)value);
			}
			if (!editModeActive)
			{
				((NamedElement)value).Common = null;
				((NamedElement)value).Collection = null;
			}
			Invalidate();
			base.OnRemoveComplete(index, value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			base.OnSet(index, oldValue, newValue);
			if (oldValue != newValue && !IsSuspended)
			{
				IsValidNameCheck(((NamedElement)newValue).Name, (NamedElement)oldValue);
			}
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((NamedElement)oldValue).Common = null;
			((NamedElement)oldValue).Collection = null;
			((NamedElement)newValue).Collection = this;
			((NamedElement)newValue).Common = common;
			Invalidate();
			base.OnSetComplete(index, oldValue, newValue);
		}

		private void CheckForTypeDublicatesAndName(object value)
		{
			if (!IsCorrectType(value))
			{
				throw new NotSupportedException(SR.invalid_object_type(GetCollectionName(), elementType.Name));
			}
			if (value is int && base.List.IndexOf(value) != -1)
			{
				throw new NotSupportedException(SR.duplicate_object_failed(GetCollectionName()));
			}
			NamedElement namedElement = (NamedElement)value;
			if (namedElement.Name == null || namedElement.Name == string.Empty)
			{
				if (base.Count == 0)
				{
					namedElement.Name = GetDefaultElementName(namedElement);
				}
				else
				{
					namedElement.Name = GenerateUniqueName(namedElement);
				}
			}
			else if (!IsUniqueName(namedElement.Name))
			{
				throw new ArgumentException(SR.duplicate_name_failed);
			}
		}

		internal virtual void IsValidNameCheck(string name, NamedElement element)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentException(SR.empty_name_failed(elementType.Name));
			}
			NamedElement byName = GetByName(name);
			if (byName != null && byName != element)
			{
				throw new NotSupportedException(SR.duplicate_name_failed);
			}
		}

		internal virtual bool IsUniqueName(string name)
		{
			return !nameToObject.ContainsKey(name);
		}

		internal string GenerateUniqueName(NamedElement element)
		{
			string elementNameFormat = GetElementNameFormat(element);
			for (int i = base.Count + 1; i < int.MaxValue; i++)
			{
				string text = string.Format(CultureInfo.InvariantCulture, elementNameFormat, i);
				if (IsUniqueName(text))
				{
					return text;
				}
			}
			throw new ApplicationException(SR.generate_name_failed);
		}

		internal NamedElement GetByNameCheck(string name)
		{
			return GetByName(name) ?? throw new ArgumentException(SR.element_not_found(elementType.Name, name, GetType().Name));
		}

		internal bool SetByName(string name, NamedElement element)
		{
			int index = GetIndex(name);
			if (index != -1)
			{
				base.List[index] = element;
				return true;
			}
			return false;
		}

		internal void SetByNameCheck(string name, NamedElement element)
		{
			if (!SetByName(name, element))
			{
				throw new ArgumentException(SR.element_not_found(elementType.Name, name, GetType().Name));
			}
		}

		internal virtual bool IsCorrectType(object value)
		{
			return elementType.IsInstanceOfType(value);
		}

		internal virtual void BeginInit()
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((NamedElement)enumerator.Current).BeginInit();
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal virtual void EndInit()
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((NamedElement)enumerator.Current).EndInit();
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal virtual void ReconnectData(bool exact)
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((NamedElement)enumerator.Current).ReconnectData(exact);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal virtual string GetElementNameFormat(NamedElement el)
		{
			string text = el.DefaultName;
			if (text == string.Empty)
			{
				text = el.GetType().Name;
			}
			return text + "{0}";
		}

		internal virtual string GetDefaultElementName(NamedElement el)
		{
			return "Default";
		}

		internal virtual void Invalidate()
		{
			if (Common != null && !IsSuspended)
			{
				Common.MapCore.Invalidate();
			}
		}

		internal virtual string GetCollectionName()
		{
			return GetType().Name.Replace("Collection", "s");
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
			if (IsSuspended)
			{
				return;
			}
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((NamedElement)enumerator.Current).Notify(msg, element, param);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
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
				OnDispose();
			}
			disposed = true;
		}

		protected virtual void OnDispose()
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((NamedElement)enumerator.Current).Dispose();
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			Clear();
		}

		public virtual object Clone()
		{
			NamedCollection namedCollection = (NamedCollection)GetType().GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new object[2]
			{
				parent,
				common
			});
			namedCollection.parent = parent;
			namedCollection.common = common;
			namedCollection.elementType = elementType;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NamedElement namedElement = (NamedElement)((ICloneable)enumerator.Current).Clone();
					namedCollection.InnerList.Add(namedElement);
					namedElement.collection = namedCollection;
				}
				return namedCollection;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
