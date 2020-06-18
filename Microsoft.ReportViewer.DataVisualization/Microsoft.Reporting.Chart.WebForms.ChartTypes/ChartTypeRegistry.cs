using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Resources;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class ChartTypeRegistry : IServiceProvider
	{
		private ResourceManager resourceManager;

		internal Hashtable registeredChartTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable createdChartTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private IServiceContainer serviceContainer;

		public ResourceManager ResourceManager
		{
			get
			{
				if (resourceManager == null)
				{
					resourceManager = new ResourceManager("Microsoft.Reporting.Chart.WebForms.Design", Assembly.GetExecutingAssembly());
				}
				return resourceManager;
			}
		}

		private ChartTypeRegistry()
		{
		}

		public ChartTypeRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ChartTypeRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionChartTypeRegistryUnsupportedType(serviceType.ToString()));
		}

		public void Register(string name, Type chartType)
		{
			if (registeredChartTypes.Contains(name))
			{
				if (!(registeredChartTypes[name].GetType() == chartType))
				{
					throw new ArgumentException(SR.ExceptionChartTypeNameIsNotUnique(name));
				}
				return;
			}
			bool flag = false;
			Type[] interfaces = chartType.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (interfaces[i] == typeof(IChartType))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentException(SR.ExceptionChartTypeHasNoInterface);
			}
			registeredChartTypes[name] = chartType;
		}

		public IChartType GetChartType(SeriesChartType chartType)
		{
			return GetChartType(Series.GetChartTypeName(chartType));
		}

		public IChartType GetChartType(string name)
		{
			if (!registeredChartTypes.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionChartTypeUnknown(name));
			}
			if (!createdChartTypes.Contains(name))
			{
				createdChartTypes[name] = ((Type)registeredChartTypes[name]).Assembly.CreateInstance(((Type)registeredChartTypes[name]).ToString());
			}
			return (IChartType)createdChartTypes[name];
		}
	}
}
