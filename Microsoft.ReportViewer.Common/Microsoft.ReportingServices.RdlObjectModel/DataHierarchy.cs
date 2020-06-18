using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class DataHierarchy : ReportObject, IHierarchy
	{
		internal class Definition : DefinitionStore<DataHierarchy, Definition.Properties>
		{
			internal enum Properties
			{
				DataMembers
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<DataMember>))]
		public IList<DataMember> DataMembers
		{
			get
			{
				return (IList<DataMember>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		IEnumerable<IHierarchyMember> IHierarchy.Members
		{
			get
			{
				foreach (DataMember dataMember in DataMembers)
				{
					yield return dataMember;
				}
			}
		}

		public DataHierarchy()
		{
		}

		internal DataHierarchy(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataMembers = new RdlCollection<DataMember>();
		}
	}
}
