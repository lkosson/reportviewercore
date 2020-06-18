using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class BorderTypeRegistry : IServiceProvider
	{
		internal Hashtable registeredBorderTypes = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);

		private Hashtable createdBorderTypes = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);

		private IServiceContainer serviceContainer;

		private BorderTypeRegistry()
		{
		}

		public BorderTypeRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("Valid Service Container object must be provided");
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
			throw new ArgumentException("3D border registry does not provide service of type: " + serviceType.ToString());
		}

		public void Register(string name, Type borderType)
		{
			if (registeredBorderTypes.Contains(name))
			{
				if (!(registeredBorderTypes[name].GetType() == borderType))
				{
					throw new ArgumentException("Border type with name \"" + name + "\" is already registered.");
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
				throw new ArgumentException("Border type must implement the IBorderType interface.");
			}
			registeredBorderTypes[name] = borderType;
		}

		public IBorderType GetBorderType(string name)
		{
			if (!registeredBorderTypes.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionUnknownBorderType(name));
			}
			if (!createdBorderTypes.Contains(name))
			{
				createdBorderTypes[name] = ((Type)registeredBorderTypes[name]).Assembly.CreateInstance(((Type)registeredBorderTypes[name]).ToString());
			}
			return (IBorderType)createdBorderTypes[name];
		}
	}
}
