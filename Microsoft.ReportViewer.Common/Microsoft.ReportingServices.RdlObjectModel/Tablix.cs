using System;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Tablix : DataRegion
	{
		internal new class Definition : DefinitionStore<Tablix, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				KeepTogether,
				NoRowsMessage,
				DataSetName,
				PageBreak,
				PageName,
				Filters,
				SortExpressions,
				TablixCorner,
				TablixBody,
				TablixColumnHierarchy,
				TablixRowHierarchy,
				LayoutDirection,
				GroupsBeforeRowHeaders,
				RepeatColumnHeaders,
				RepeatRowHeaders,
				FixedColumnHeaders,
				FixedRowHeaders,
				OmitBorderOnPageBreak,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public TablixCorner TablixCorner
		{
			get
			{
				return (TablixCorner)base.PropertyStore.GetObject(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		public TablixBody TablixBody
		{
			get
			{
				return (TablixBody)base.PropertyStore.GetObject(26);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(26, value);
			}
		}

		public TablixHierarchy TablixColumnHierarchy
		{
			get
			{
				return (TablixHierarchy)base.PropertyStore.GetObject(27);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(27, value);
			}
		}

		public TablixHierarchy TablixRowHierarchy
		{
			get
			{
				return (TablixHierarchy)base.PropertyStore.GetObject(28);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(28, value);
			}
		}

		[DefaultValue(LayoutDirections.LTR)]
		public LayoutDirections LayoutDirection
		{
			get
			{
				return (LayoutDirections)base.PropertyStore.GetInteger(29);
			}
			set
			{
				base.PropertyStore.SetInteger(29, (int)value);
			}
		}

		[DefaultValue(0)]
		[ValidValues(0, int.MaxValue)]
		public int GroupsBeforeRowHeaders
		{
			get
			{
				return base.PropertyStore.GetInteger(30);
			}
			set
			{
				((IntProperty)DefinitionStore<Tablix, Definition.Properties>.GetProperty(30)).Validate(this, value);
				base.PropertyStore.SetInteger(30, value);
			}
		}

		[DefaultValue(false)]
		public bool RepeatColumnHeaders
		{
			get
			{
				return base.PropertyStore.GetBoolean(31);
			}
			set
			{
				base.PropertyStore.SetBoolean(31, value);
			}
		}

		[DefaultValue(false)]
		public bool RepeatRowHeaders
		{
			get
			{
				return base.PropertyStore.GetBoolean(32);
			}
			set
			{
				base.PropertyStore.SetBoolean(32, value);
			}
		}

		[DefaultValue(false)]
		public bool FixedColumnHeaders
		{
			get
			{
				return base.PropertyStore.GetBoolean(33);
			}
			set
			{
				base.PropertyStore.SetBoolean(33, value);
			}
		}

		[DefaultValue(false)]
		public bool FixedRowHeaders
		{
			get
			{
				return base.PropertyStore.GetBoolean(34);
			}
			set
			{
				base.PropertyStore.SetBoolean(34, value);
			}
		}

		[DefaultValue(false)]
		public bool OmitBorderOnPageBreak
		{
			get
			{
				return base.PropertyStore.GetBoolean(35);
			}
			set
			{
				base.PropertyStore.SetBoolean(35, value);
			}
		}

		public Tablix()
		{
		}

		internal Tablix(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TablixBody = new TablixBody();
			TablixColumnHierarchy = new TablixHierarchy();
			TablixRowHierarchy = new TablixHierarchy();
		}
	}
}
