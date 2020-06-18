using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignTimeShapeConverter : StringConverter
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
			arrayList.Add("(none)");
			if (context != null && context.Instance != null)
			{
				object obj = null;
				obj = ((!(context.Instance is object[])) ? context.Instance : ((object[])context.Instance)[0]);
				if (obj is Symbol)
				{
					MapCore mapCore = ((Symbol)obj).GetMapCore();
					if (mapCore != null)
					{
						foreach (Shape shape in mapCore.Shapes)
						{
							arrayList.Add(shape.Name);
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
