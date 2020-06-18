using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class SeriesAreaNameConverter : StringConverter
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
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Instance != null)
			{
				IServiceContainer serviceContainer = null;
				if (context.Instance is Series && ((Series)context.Instance).serviceContainer != null)
				{
					serviceContainer = ((Series)context.Instance).serviceContainer;
				}
				if (serviceContainer == null && context.Instance is Array)
				{
					Array array = (Array)context.Instance;
					if (array.Length > 0 && array.GetValue(0) is Series)
					{
						serviceContainer = ((Series)array.GetValue(0)).serviceContainer;
					}
				}
				if (serviceContainer != null)
				{
					ChartImage chartImage = (ChartImage)serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						foreach (ChartArea chartArea in chartImage.ChartAreas)
						{
							arrayList.Add(chartArea.Name);
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
