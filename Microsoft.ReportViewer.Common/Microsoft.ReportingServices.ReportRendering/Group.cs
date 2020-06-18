using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal abstract class Group
	{
		protected ReportItem m_ownerItem;

		internal Grouping m_groupingDef;

		internal Visibility m_visibilityDef;

		protected int m_uniqueName;

		protected CustomPropertyCollection m_customProperties;

		public string Name
		{
			get
			{
				if (m_groupingDef == null)
				{
					return null;
				}
				return m_groupingDef.Name;
			}
		}

		public abstract string ID
		{
			get;
		}

		public string UniqueName
		{
			get
			{
				if (m_uniqueName == 0)
				{
					return null;
				}
				return m_uniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public abstract string Label
		{
			get;
		}

		public virtual bool PageBreakAtEnd
		{
			get
			{
				if (m_groupingDef == null)
				{
					return false;
				}
				return m_groupingDef.PageBreakAtEnd;
			}
		}

		public virtual bool PageBreakAtStart
		{
			get
			{
				if (m_groupingDef == null)
				{
					return false;
				}
				return m_groupingDef.PageBreakAtStart;
			}
		}

		public string Custom
		{
			get
			{
				if (m_groupingDef != null)
				{
					string text = m_groupingDef.Custom;
					if (text == null && CustomProperties != null)
					{
						CustomProperty customProperty = CustomProperties["Custom"];
						if (customProperty != null && customProperty.Value != null)
						{
							text = DataTypeUtility.ConvertToInvariantString(customProperty.Value);
						}
					}
					return text;
				}
				return null;
			}
		}

		public abstract CustomPropertyCollection CustomProperties
		{
			get;
		}

		public abstract bool Hidden
		{
			get;
		}

		public virtual bool HasToggle => Visibility.HasToggle(m_visibilityDef);

		public virtual string ToggleItem
		{
			get
			{
				if (m_visibilityDef == null)
				{
					return null;
				}
				return m_visibilityDef.Toggle;
			}
		}

		internal virtual TextBox ToggleParent
		{
			get
			{
				if (!HasToggle)
				{
					return null;
				}
				Global.Tracer.Assert(OwnerDataRegion != null);
				return OwnerDataRegion.RenderingContext.GetToggleParent(m_uniqueName);
			}
		}

		public virtual SharedHiddenState SharedHidden => Visibility.GetSharedHidden(m_visibilityDef);

		public virtual bool IsToggleChild
		{
			get
			{
				Global.Tracer.Assert(OwnerDataRegion != null);
				return OwnerDataRegion.RenderingContext.IsToggleChild(m_uniqueName);
			}
		}

		public virtual string DataElementName
		{
			get
			{
				if (m_groupingDef == null)
				{
					return null;
				}
				return m_groupingDef.DataElementName;
			}
		}

		public virtual string DataCollectionName
		{
			get
			{
				if (m_groupingDef == null)
				{
					return null;
				}
				return m_groupingDef.DataCollectionName;
			}
		}

		public virtual DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_groupingDef == null)
				{
					return DataElementOutputTypes.Output;
				}
				return m_groupingDef.DataElementOutput;
			}
		}

		internal DataRegion OwnerDataRegion => m_ownerItem as DataRegion;

		internal Group(CustomReportItem owner, Grouping groupingDef)
		{
			m_ownerItem = owner;
			m_groupingDef = groupingDef;
		}

		internal Group(DataRegion owner, Grouping groupingDef, Visibility visibilityDef)
		{
			m_ownerItem = owner;
			m_groupingDef = groupingDef;
			m_visibilityDef = visibilityDef;
		}
	}
}
