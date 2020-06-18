using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ActionInfo : ReportObject
	{
		internal class Definition : DefinitionStore<ActionInfo, Definition.Properties>
		{
			internal enum Properties
			{
				Actions,
				LayoutDirection,
				Style
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<Action>))]
		public IList<Action> Actions
		{
			get
			{
				return (IList<Action>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ActionInfo()
		{
		}

		internal ActionInfo(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Actions = new RdlCollection<Action>();
		}
	}
}
