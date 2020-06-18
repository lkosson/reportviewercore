using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignTimeLayerConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Instance != null)
			{
				object obj = null;
				obj = ((!(context.Instance is object[])) ? context.Instance : ((object[])context.Instance)[0]);
				if (obj is ILayerElement)
				{
					MapCore mapCore = (MapCore)((NamedElement)obj).ParentElement;
					if (mapCore != null)
					{
						foreach (Layer layer in mapCore.Layers)
						{
							arrayList.Add(layer.Name);
						}
						arrayList.Sort();
						arrayList.Insert(0, "(all)");
						arrayList.Insert(0, "(none)");
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
