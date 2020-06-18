using System;
using System.ComponentModel.Design;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class EventsManager : IServiceProvider
	{
		internal IServiceContainer serviceContainer;

		private Chart control;

		private EventsManager()
		{
		}

		public EventsManager(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(EventsManager))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionEventManagerUnsupportedType(serviceType.ToString()));
		}

		internal void OnBackPaint(object caller, ChartPaintEventArgs e)
		{
			if (control == null)
			{
				control = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			if (control != null)
			{
				control.CallBackPaint(caller, e);
			}
		}

		internal void OnPaint(object caller, ChartPaintEventArgs e)
		{
			if (control == null)
			{
				control = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			if (control != null)
			{
				control.CallPaint(caller, e);
			}
		}

		internal void OnCustomize()
		{
			if (control == null)
			{
				control = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			if (control != null)
			{
				control.CallCustomize();
			}
		}

		internal void OnCustomizeLegend(LegendItemsCollection legendItems, string legendName)
		{
			if (control == null)
			{
				control = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			if (control != null)
			{
				control.CallCustomizeLegend(legendItems, legendName);
			}
		}

		internal void OnCustomizeMapAreas(MapAreasCollection areaItems)
		{
			if (control == null)
			{
				control = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			if (control != null)
			{
				control.CallCustomizeMapAreas(areaItems);
			}
		}
	}
}
