using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixHierarchy : ReportObject, IHierarchy
	{
		internal class Definition : DefinitionStore<TablixHierarchy, Definition.Properties>
		{
			internal enum Properties
			{
				TablixMembers
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<TablixMember>))]
		public IList<TablixMember> TablixMembers
		{
			get
			{
				return (IList<TablixMember>)base.PropertyStore.GetObject(0);
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
				foreach (TablixMember tablixMember in TablixMembers)
				{
					yield return tablixMember;
				}
			}
		}

		public TablixHierarchy()
		{
		}

		internal TablixHierarchy(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TablixMembers = new RdlCollection<TablixMember>();
		}
	}
}
