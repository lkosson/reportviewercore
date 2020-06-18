using Microsoft.ReportingServices.RdlObjectModel;
using System;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class CategoryAxis2005 : ReportObject
	{
		public class Definition : DefinitionStore<CategoryAxis2005, Definition.Properties>
		{
			public enum Properties
			{
				Axis
			}

			private Definition()
			{
			}
		}

		public Axis2005 Axis
		{
			get
			{
				return (Axis2005)base.PropertyStore.GetObject(0);
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

		public CategoryAxis2005()
		{
		}

		public CategoryAxis2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			Axis = new Axis2005();
		}
	}
}
