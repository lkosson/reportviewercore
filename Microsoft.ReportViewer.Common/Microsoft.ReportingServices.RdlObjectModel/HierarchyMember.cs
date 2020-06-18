namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class HierarchyMember : ReportObject
	{
		public abstract Group Group
		{
			get;
			set;
		}

		public HierarchyMember()
		{
		}

		internal HierarchyMember(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
