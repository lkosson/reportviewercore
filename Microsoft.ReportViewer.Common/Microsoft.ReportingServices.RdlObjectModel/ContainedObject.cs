using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class ContainedObject : IContainedObject
	{
		private IContainedObject m_parent;

		[XmlIgnore]
		public IContainedObject Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}
	}
}
