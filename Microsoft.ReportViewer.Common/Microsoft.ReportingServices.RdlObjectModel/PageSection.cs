using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class PageSection : ReportElement
	{
		internal new class Definition : DefinitionStore<PageSection, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Height,
				PrintOnFirstPage,
				PrintOnLastPage,
				ReportItems,
				PrintBetweenSections
			}

			private Definition()
			{
			}
		}

		public ReportSize Height
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

		[DefaultValue(false)]
		public bool PrintOnFirstPage
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		[DefaultValue(false)]
		public bool PrintOnLastPage
		{
			get
			{
				return base.PropertyStore.GetBoolean(3);
			}
			set
			{
				base.PropertyStore.SetBoolean(3, value);
			}
		}

		[DefaultValue(false)]
		public bool PrintBetweenSections
		{
			get
			{
				return base.PropertyStore.GetBoolean(5);
			}
			set
			{
				base.PropertyStore.SetBoolean(5, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public PageSection()
		{
		}

		internal PageSection(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Height = Constants.DefaultZeroSize;
			ReportItems = new RdlCollection<ReportItem>();
		}
	}
}
