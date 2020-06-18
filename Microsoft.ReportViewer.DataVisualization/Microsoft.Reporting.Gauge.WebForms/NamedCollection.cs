using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[ListBindable(false)]
	internal abstract class NamedCollection : CollectionBase, IDisposable, ICloneable
	{
		internal CommonElements common;

		internal Type elementType = typeof(NamedElement);

		internal NamedElement parent;

		internal bool editModeActive;

		private bool disposed;

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

		private NamedCollection()
		{
		}

		internal NamedCollection(NamedElement parent, CommonElements common)
		{
			Common = common;
			this.parent = parent;
		}

		public int GetIndex(string name)
		{
			int result = -1;
			for (int i = 0; i < base.List.Count; i++)
			{
				if (string.Compare(((NamedElement)base.List[i]).Name, name, ignoreCase: false, CultureInfo.CurrentCulture) == 0)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public NamedElement GetByName(string name)
		{
			int index = GetIndex(name);
			if (index != -1)
			{
				return (NamedElement)base.List[index];
			}
			return null;
		}

		public NamedElement GetByIndex(int index)
		{
			return (NamedElement)base.List[index];
		}

		public virtual int IndexOf(object value)
		{
			return base.List.IndexOf(value);
		}

		protected override void OnClear()
		{
			while (base.Count > 0)
			{
				RemoveAt(0);
			}
			base.OnClear();
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
			CheckForTypeDublicatesAndName(value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((NamedElement)value).Collection = this;
			((NamedElement)value).Common = common;
			Invalidate();
			base.OnInsertComplete(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);
			if (!editModeActive)
			{
				((NamedElement)value).Common = null;
				((NamedElement)value).Collection = null;
			}
			Invalidate();
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			base.OnSet(index, oldValue, newValue);
			if (oldValue != newValue)
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
				throw new NotSupportedException(Utils.SRGetStr("ExceptionInvalidObjectType", GetCollectionName(), elementType.Name));
			}
			if (value is int && base.List.IndexOf(value) != -1)
			{
				throw new NotSupportedException(Utils.SRGetStr("ExceptionDuplicateObjectFailed", GetCollectionName()));
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionDuplicateNameFailed"));
			}
		}

		internal virtual void IsValidNameCheck(string name, NamedElement element)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionEmptyNameFailed", elementType.Name));
			}
			NamedElement byName = GetByName(name);
			if (byName != null && byName != element)
			{
				throw new NotSupportedException(Utils.SRGetStr("ExceptionDuplicateNameFailed"));
			}
		}

		internal virtual bool IsUniqueName(string name)
		{
			return GetIndex(name) == -1;
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
			throw new ApplicationException(Utils.SRGetStr("ExceptionGenerateNameFailed"));
		}

		internal NamedElement GetByNameCheck(string name)
		{
			NamedElement byName = GetByName(name);
			if (byName == null)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionElementNotFound", elementType.Name, name, GetType().Name));
			}
			return byName;
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionElementNotFound", elementType.Name, name, GetType().Name));
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
			if (Common != null)
			{
				Common.GaugeCore.Invalidate();
			}
		}

		internal virtual string GetCollectionName()
		{
			return GetType().Name.Replace("Collection", "s");
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
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
