using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class DataSetBase : ReportObject, IGlobalNamedObject, INamedObject
	{
		internal class Definition : DefinitionStore<DataSetBase, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				CaseSensitivity,
				Collation,
				AccentSensitivity,
				KanatypeSensitivity,
				WidthSensitivity,
				InterpretSubtotalsAsDetails
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue(CaseSensitivities.Auto)]
		public CaseSensitivities CaseSensitivity
		{
			get
			{
				return (CaseSensitivities)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		[DefaultValue("")]
		public string Collation
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

		[DefaultValue(AccentSensitivities.Auto)]
		public AccentSensitivities AccentSensitivity
		{
			get
			{
				return (AccentSensitivities)base.PropertyStore.GetInteger(3);
			}
			set
			{
				base.PropertyStore.SetInteger(3, (int)value);
			}
		}

		[DefaultValue(KanatypeSensitivities.Auto)]
		public KanatypeSensitivities KanatypeSensitivity
		{
			get
			{
				return (KanatypeSensitivities)base.PropertyStore.GetInteger(4);
			}
			set
			{
				base.PropertyStore.SetInteger(4, (int)value);
			}
		}

		[DefaultValue(WidthSensitivities.Auto)]
		public WidthSensitivities WidthSensitivity
		{
			get
			{
				return (WidthSensitivities)base.PropertyStore.GetInteger(5);
			}
			set
			{
				base.PropertyStore.SetInteger(5, (int)value);
			}
		}

		[DefaultValue(InterpretSubtotalsAsDetailsTypes.Auto)]
		public InterpretSubtotalsAsDetailsTypes InterpretSubtotalsAsDetails
		{
			get
			{
				return (InterpretSubtotalsAsDetailsTypes)base.PropertyStore.GetInteger(6);
			}
			set
			{
				base.PropertyStore.SetInteger(6, (int)value);
			}
		}

		public DataSetBase()
		{
		}

		internal DataSetBase(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public abstract QueryBase GetQuery();

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
