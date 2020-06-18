using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class DataMember : HierarchyMember, IHierarchyMember
	{
		internal class Definition : DefinitionStore<DataMember, Definition.Properties>
		{
			internal enum Properties
			{
				Group,
				SortExpressions,
				CustomProperties,
				DataMembers,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public override Group Group
		{
			get
			{
				return (Group)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> SortExpressions
		{
			get
			{
				return (IList<SortExpression>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[XmlElement(typeof(RdlCollection<DataMember>))]
		public IList<DataMember> DataMembers
		{
			get
			{
				return (IList<DataMember>)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		IEnumerable<IHierarchyMember> IHierarchyMember.Members
		{
			get
			{
				foreach (DataMember dataMember in DataMembers)
				{
					yield return dataMember;
				}
			}
		}

		public DataMember()
		{
		}

		internal DataMember(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			SortExpressions = new RdlCollection<SortExpression>();
			CustomProperties = new RdlCollection<CustomProperty>();
			DataMembers = new RdlCollection<DataMember>();
		}
	}
}
