using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapMember : HierarchyMember, IHierarchy, IHierarchyMember
	{
		internal class Definition : DefinitionStore<MapMember, Definition.Properties>
		{
			internal enum Properties
			{
				Group,
				ChildMapMember,
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
				if (value == null)
				{
					throw new ArgumentNullException("Group");
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement("MapMember")]
		public MapMember ChildMapMember
		{
			get
			{
				return (MapMember)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> SortExpressions
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		IEnumerable<IHierarchyMember> IHierarchyMember.Members
		{
			get
			{
				yield return ChildMapMember;
			}
		}

		IEnumerable<IHierarchyMember> IHierarchy.Members
		{
			get
			{
				yield return ChildMapMember;
			}
		}

		public MapMember()
		{
		}

		internal MapMember(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Group = new Group();
		}
	}
}
