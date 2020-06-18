using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class KeywordConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add("#NAME");
			if (context != null && context.Instance != null)
			{
				object obj = null;
				obj = ((!(context.Instance is object[])) ? context.Instance : ((object[])context.Instance)[0]);
				if (obj is Group)
				{
					MapCore mapCore = ((Group)obj).GetMapCore();
					if (mapCore != null)
					{
						foreach (Field groupField in mapCore.GroupFields)
						{
							arrayList.Add(groupField.GetKeyword());
						}
					}
				}
				else if (obj is GroupRule)
				{
					MapCore mapCore2 = ((GroupRule)obj).GetMapCore();
					if (mapCore2 != null)
					{
						foreach (Field groupField2 in mapCore2.GroupFields)
						{
							arrayList.Add(groupField2.GetKeyword());
						}
					}
				}
				else if (obj is Shape)
				{
					MapCore mapCore3 = ((Shape)obj).GetMapCore();
					if (mapCore3 != null)
					{
						foreach (Field shapeField in mapCore3.ShapeFields)
						{
							arrayList.Add(shapeField.GetKeyword());
						}
					}
				}
				else if (obj is ShapeRule)
				{
					MapCore mapCore4 = ((ShapeRule)obj).GetMapCore();
					if (mapCore4 != null)
					{
						foreach (Field shapeField2 in mapCore4.ShapeFields)
						{
							arrayList.Add(shapeField2.GetKeyword());
						}
					}
				}
				else if (obj is Path)
				{
					MapCore mapCore5 = ((Path)obj).GetMapCore();
					if (mapCore5 != null)
					{
						foreach (Field pathField in mapCore5.PathFields)
						{
							arrayList.Add(pathField.GetKeyword());
						}
					}
				}
				else if (obj is PathRuleBase)
				{
					MapCore mapCore6 = ((PathRuleBase)obj).GetMapCore();
					if (mapCore6 != null)
					{
						foreach (Field pathField2 in mapCore6.PathFields)
						{
							arrayList.Add(pathField2.GetKeyword());
						}
					}
				}
				else if (obj is Symbol)
				{
					MapCore mapCore7 = ((Symbol)obj).GetMapCore();
					if (mapCore7 != null)
					{
						foreach (Field symbolField in mapCore7.SymbolFields)
						{
							arrayList.Add(symbolField.GetKeyword());
						}
					}
				}
				else if (obj is PredefinedSymbol)
				{
					MapCore mapCore8 = ((PredefinedSymbol)obj).GetMapCore();
					if (mapCore8 != null)
					{
						foreach (Field symbolField2 in mapCore8.SymbolFields)
						{
							arrayList.Add(symbolField2.GetKeyword());
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
