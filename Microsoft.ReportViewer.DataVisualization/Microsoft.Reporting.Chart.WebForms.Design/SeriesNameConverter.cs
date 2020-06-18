using Microsoft.Reporting.Chart.WebForms.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class SeriesNameConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			DataManager dataManager = null;
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Instance != null)
			{
				MethodInfo method = context.Instance.GetType().GetMethod("GetService");
				if (method != null)
				{
					dataManager = (DataManager)method.Invoke(parameters: new object[1]
					{
						typeof(DataManager)
					}, obj: context.Instance);
				}
				if (dataManager == null)
				{
					throw new InvalidOperationException(SR.ExceptionEditorChartTypeRegistryServiceInObjectInaccessible(context.Instance.GetType().ToString()));
				}
				foreach (Series item in dataManager.Series)
				{
					arrayList.Add(item.Name);
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
