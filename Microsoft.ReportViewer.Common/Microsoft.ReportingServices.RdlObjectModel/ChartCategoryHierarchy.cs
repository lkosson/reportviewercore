using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartCategoryHierarchy : ReportObject, IHierarchy
	{
		internal class Definition : DefinitionStore<ChartCategoryHierarchy, Definition.Properties>
		{
			internal enum Properties
			{
				ChartMembers,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ChartMember>))]
		public IList<ChartMember> ChartMembers
		{
			get
			{
				return (IList<ChartMember>)base.PropertyStore.GetObject(0);
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
				foreach (ChartMember chartMember in ChartMembers)
				{
					yield return chartMember;
				}
			}
		}

		public ChartCategoryHierarchy()
		{
		}

		internal ChartCategoryHierarchy(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartMembers = new RdlCollection<ChartMember>();
		}
	}
}
