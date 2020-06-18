using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class GaugeMember : HierarchyMember, IHierarchy, IHierarchyMember
	{
		internal class Definition : DefinitionStore<GaugeMember, Definition.Properties>
		{
			internal enum Properties
			{
				Group,
				SortExpressions,
				ChildGaugeMember,
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
					throw new ArgumentNullException("value");
				}
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

		[XmlElement("GaugeMember")]
		public GaugeMember ChildGaugeMember
		{
			get
			{
				return (GaugeMember)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		IEnumerable<IHierarchyMember> IHierarchyMember.Members
		{
			get
			{
				yield return ChildGaugeMember;
			}
		}

		IEnumerable<IHierarchyMember> IHierarchy.Members
		{
			get
			{
				yield return ChildGaugeMember;
			}
		}

		public GaugeMember()
		{
		}

		internal GaugeMember(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Group = new Group();
			SortExpressions = new RdlCollection<SortExpression>();
		}
	}
}
