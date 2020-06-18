using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class ReportObjectBase : IContainedObject
	{
		private IPropertyStore m_propertyStore;

		[XmlIgnore]
		internal IPropertyStore PropertyStore => m_propertyStore;

		[XmlIgnore]
		public IContainedObject Parent
		{
			get
			{
				return m_propertyStore.Parent;
			}
			set
			{
				m_propertyStore.Parent = value;
			}
		}

		protected ReportObjectBase()
		{
			m_propertyStore = WrapPropertyStore(new PropertyStore((ReportObject)this));
			Initialize();
		}

		internal ReportObjectBase(IPropertyStore propertyStore)
		{
			m_propertyStore = WrapPropertyStore(propertyStore);
		}

		public virtual void Initialize()
		{
		}

		internal virtual IPropertyStore WrapPropertyStore(IPropertyStore propertyStore)
		{
			return propertyStore;
		}
	}
}
