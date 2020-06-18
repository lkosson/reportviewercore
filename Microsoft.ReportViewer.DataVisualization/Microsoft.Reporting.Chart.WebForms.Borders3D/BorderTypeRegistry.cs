using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Resources;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class BorderTypeRegistry : IServiceProvider
	{
		private ResourceManager resourceManager;

		internal Hashtable registeredBorderTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable createdBorderTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

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

		private BorderTypeRegistry()
		{
		}

		public BorderTypeRegistry(IServiceContainer container)
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
			if (serviceType == typeof(BorderTypeRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionBorderTypeRegistryUnsupportedType(serviceType.ToString()));
		}

		public void Register(string name, Type borderType)
		{
			if (registeredBorderTypes.Contains(name))
			{
				if (!(registeredBorderTypes[name].GetType() == borderType))
				{
					throw new ArgumentException(SR.ExceptionBorderTypeNameIsNotUnique(name));
				}
				return;
			}
			bool flag = false;
			Type[] interfaces = borderType.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (interfaces[i] == typeof(IBorderType))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentException(SR.ExceptionBorderTypeHasNoInterface);
			}
			registeredBorderTypes[name] = borderType;
		}

		public IBorderType GetBorderType(string name)
		{
			if (!registeredBorderTypes.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionBorderTypeUnknown(name));
			}
			if (!createdBorderTypes.Contains(name))
			{
				createdBorderTypes[name] = ((Type)registeredBorderTypes[name]).Assembly.CreateInstance(((Type)registeredBorderTypes[name]).ToString());
			}
			return (IBorderType)createdBorderTypes[name];
		}
	}
}
