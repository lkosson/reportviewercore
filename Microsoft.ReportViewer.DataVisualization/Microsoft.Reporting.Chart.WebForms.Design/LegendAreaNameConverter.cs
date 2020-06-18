using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class LegendAreaNameConverter : StringConverter
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
			arrayList.Add("NotSet");
			ChartAreaCollection chartAreaCollection = null;
			string b = "";
			if (context != null && context.Instance != null)
			{
				if (context.Instance is Legend)
				{
					Legend legend = (Legend)context.Instance;
					if (legend.Common != null && legend.Common.ChartPicture != null)
					{
						chartAreaCollection = legend.Common.ChartPicture.ChartAreas;
					}
				}
				else if (context.Instance is ChartArea)
				{
					ChartArea chartArea = (ChartArea)context.Instance;
					if (chartArea.Common != null && chartArea.Common.ChartPicture != null)
					{
						chartAreaCollection = chartArea.Common.ChartPicture.ChartAreas;
						b = chartArea.Name;
					}
				}
				else if (context.Instance is Title)
				{
					Title title = (Title)context.Instance;
					if (title.Chart != null && title.Chart.chartPicture != null)
					{
						chartAreaCollection = title.Chart.chartPicture.ChartAreas;
					}
				}
				else if (context.Instance is Annotation)
				{
					Annotation annotation = (Annotation)context.Instance;
					if (annotation.Chart != null && annotation.Chart.chartPicture != null)
					{
						chartAreaCollection = annotation.Chart.chartPicture.ChartAreas;
					}
				}
				else if (context.Instance is Array)
				{
					if (((Array)context.Instance).Length > 0 && ((Array)context.Instance).GetValue(0) is Legend)
					{
						Legend legend2 = (Legend)((Array)context.Instance).GetValue(0);
						if (legend2.Common != null && legend2.Common.ChartPicture != null)
						{
							chartAreaCollection = legend2.Common.ChartPicture.ChartAreas;
						}
					}
					else if (((Array)context.Instance).Length > 0 && ((Array)context.Instance).GetValue(0) is ChartArea)
					{
						ChartArea chartArea2 = (ChartArea)((Array)context.Instance).GetValue(0);
						if (chartArea2.Common != null && chartArea2.Common.ChartPicture != null)
						{
							chartAreaCollection = chartArea2.Common.ChartPicture.ChartAreas;
						}
					}
					else if (((Array)context.Instance).Length > 0 && ((Array)context.Instance).GetValue(0) is Title)
					{
						Title title2 = (Title)((Array)context.Instance).GetValue(0);
						if (title2.Chart != null && title2.Chart.chartPicture != null)
						{
							chartAreaCollection = title2.Chart.chartPicture.ChartAreas;
						}
					}
					else if (((Array)context.Instance).Length > 0 && ((Array)context.Instance).GetValue(0) is Annotation)
					{
						Annotation annotation2 = (Annotation)((Array)context.Instance).GetValue(0);
						if (annotation2.Chart != null && annotation2.Chart.chartPicture != null)
						{
							chartAreaCollection = annotation2.Chart.chartPicture.ChartAreas;
						}
					}
				}
			}
			if (chartAreaCollection != null)
			{
				foreach (ChartArea item in chartAreaCollection)
				{
					if (item.Name != b)
					{
						arrayList.Add(item.Name);
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
