using System;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ReportSection : ReportObject
	{
		internal class Definition : DefinitionStore<ReportSection, Definition.Properties>
		{
			internal enum Properties
			{
				Body,
				Width,
				Page,
				DataElementName,
				DataElementOutput
			}

			private Definition()
			{
			}
		}

		public Body Body
		{
			get
			{
				return (Body)base.PropertyStore.GetObject(0);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Body");
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportSize Width
		{
			get
			{
				return base.PropertyStore.GetSize(1);
			}
			set
			{
				base.PropertyStore.SetSize(1, value);
			}
		}

		public Page Page
		{
			get
			{
				return (Page)base.PropertyStore.GetObject(2);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Page");
				}
				base.PropertyStore.SetObject(2, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Auto)]
		[ValidEnumValues("ReportItemDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(4);
			}
			set
			{
				base.PropertyStore.SetInteger(4, (int)value);
			}
		}

		public ReportSection()
		{
		}

		internal ReportSection(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Width = Constants.DefaultZeroSize;
			Body = new Body();
			Page = new Page();
		}
	}
}
