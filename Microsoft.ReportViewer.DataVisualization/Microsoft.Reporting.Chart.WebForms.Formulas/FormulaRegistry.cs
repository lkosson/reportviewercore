using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class FormulaRegistry : IServiceProvider
	{
		internal Hashtable registeredModules = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable createdModules = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private ArrayList modulesNames = new ArrayList();

		private IServiceContainer serviceContainer;

		public int Count => modulesNames.Count;

		private FormulaRegistry()
		{
		}

		public FormulaRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		public void Register(string name, Type moduleType)
		{
			if (registeredModules.Contains(name))
			{
				if (!(registeredModules[name].GetType() == moduleType))
				{
					throw new ArgumentException(SR.ExceptionFormulaModuleNameIsNotUnique(name));
				}
				return;
			}
			modulesNames.Add(name);
			bool flag = false;
			Type[] interfaces = moduleType.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (interfaces[i] == typeof(IFormula))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentException(SR.ExceptionFormulaModuleHasNoInterface);
			}
			registeredModules[name] = moduleType;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(FormulaRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionFormulaModuleRegistryUnsupportedType(serviceType.ToString()));
		}

		public IFormula GetFormulaModule(string name)
		{
			if (!registeredModules.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionFormulaModuleNameUnknown(name));
			}
			if (!createdModules.Contains(name))
			{
				createdModules[name] = ((Type)registeredModules[name]).Assembly.CreateInstance(((Type)registeredModules[name]).ToString());
			}
			return (IFormula)createdModules[name];
		}

		public string GetModuleName(int index)
		{
			return (string)modulesNames[index];
		}
	}
}
