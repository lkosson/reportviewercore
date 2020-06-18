using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ObjectLinker : GaugeObject
	{
		private bool invalidated = true;

		private Hashtable elements = new Hashtable();

		private Hashtable Elements
		{
			get
			{
				if (invalidated)
				{
					elements = CollectValues();
					invalidated = false;
				}
				return elements;
			}
		}

		internal ObjectLinker(object parent)
			: base(parent)
		{
		}

		internal override void Invalidate()
		{
			invalidated = true;
		}

		private Hashtable CollectValues()
		{
			Hashtable hashtable = new Hashtable();
			if (Common != null)
			{
				NamedCollection[] renderCollections = Common.GaugeCore.GetRenderCollections();
				for (int i = 0; i < renderCollections.Length; i++)
				{
					foreach (NamedElement item in renderCollections[i])
					{
						if (!hashtable.ContainsKey(item.Name))
						{
							hashtable.Add(item.Name, item);
						}
						else
						{
							hashtable.Remove(item.Name);
						}
						hashtable.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", item.Collection.GetCollectionName(), item.Name), item);
					}
				}
			}
			return hashtable;
		}

		internal ArrayList GetObjectNames(object thisObject)
		{
			ArrayList arrayList = new ArrayList();
			if (Common != null)
			{
				NamedCollection[] renderCollections = Common.GaugeCore.GetRenderCollections();
				for (int i = 0; i < renderCollections.Length; i++)
				{
					foreach (NamedElement item in renderCollections[i])
					{
						if (item != thisObject)
						{
							arrayList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", item.Collection.GetCollectionName(), item.Name));
						}
					}
				}
			}
			return arrayList;
		}

		internal NamedElement GetElement(string name)
		{
			if (name != string.Empty && Elements.ContainsKey(name))
			{
				return (NamedElement)Elements[name];
			}
			return null;
		}

		internal bool IsParentElementValid(IRenderable r, object startObject, bool raiseException)
		{
			string parentRenderableName = r.GetParentRenderableName();
			if (parentRenderableName != string.Empty && parentRenderableName != "(none)")
			{
				NamedElement element = GetElement(parentRenderableName);
				if (element != null)
				{
					if (element == startObject)
					{
						if (raiseException)
						{
							throw new ApplicationException(Utils.SRGetStr("ExceptionCircularReference"));
						}
						return false;
					}
					if (!(element is IRenderable))
					{
						if (raiseException)
						{
							throw new ApplicationException(Utils.SRGetStr("ExceptionParentNotRenderable", parentRenderableName));
						}
						return false;
					}
					return IsParentElementValid((IRenderable)element, startObject, raiseException);
				}
				if (raiseException)
				{
					throw new ApplicationException(Utils.SRGetStr("ExceptionInvalidParent", parentRenderableName));
				}
				return false;
			}
			return true;
		}
	}
}
