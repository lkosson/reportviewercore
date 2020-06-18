using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class UserSort : ReportObject
	{
		internal class Definition : DefinitionStore<UserSort, Definition.Properties>
		{
			internal enum Properties
			{
				SortExpression,
				SortExpressionScope,
				SortTarget
			}

			private Definition()
			{
			}
		}

		public ReportExpression SortExpression
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue("")]
		public string SortExpressionScope
		{
			get
			{
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValue("")]
		public string SortTarget
		{
			get
			{
				return (string)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public UserSort()
		{
		}

		internal UserSort(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
