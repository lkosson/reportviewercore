using System;
using System.Threading;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class DefinitionStore<T, E>
	{
		private static PropertyStore m_propertyDefinitions = new PropertyStore(null);

		private static ReaderWriterLock m_lock = new ReaderWriterLock();

		public static IPropertyDefinition GetProperty(int index)
		{
			m_lock.AcquireReaderLock(-1);
			IPropertyDefinition propertyDefinition;
			try
			{
				propertyDefinition = (IPropertyDefinition)m_propertyDefinitions.GetObject(index);
				if (propertyDefinition != null)
				{
					return propertyDefinition;
				}
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			propertyDefinition = PropertyDefinition.Create(typeof(T), Enum.GetName(typeof(E), index));
			m_lock.AcquireWriterLock(-1);
			try
			{
				m_propertyDefinitions.SetObject(index, propertyDefinition);
				return propertyDefinition;
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
		}

		protected DefinitionStore()
		{
		}
	}
}
