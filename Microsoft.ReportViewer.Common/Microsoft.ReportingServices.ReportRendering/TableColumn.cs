using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableColumn
	{
		private Table m_owner;

		private Microsoft.ReportingServices.ReportProcessing.TableColumn m_columnDef;

		private TableColumnInstance m_columnInstance;

		private int m_index;

		public string UniqueName
		{
			get
			{
				if (ColumnInstance == null)
				{
					return null;
				}
				return ColumnInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public ReportSize Width
		{
			get
			{
				if (m_columnDef.WidthForRendering == null)
				{
					m_columnDef.WidthForRendering = new ReportSize(m_columnDef.Width, m_columnDef.WidthValue);
				}
				return m_columnDef.WidthForRendering;
			}
		}

		public bool Hidden
		{
			get
			{
				if (ColumnInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(m_columnDef.Visibility);
				}
				if (m_columnDef.Visibility == null)
				{
					return false;
				}
				if (m_columnDef.Visibility.Toggle != null)
				{
					return m_owner.RenderingContext.IsItemHidden(ColumnInstance.UniqueName, potentialSender: false);
				}
				return ColumnInstance.StartHidden;
			}
		}

		public bool HasToggle => Visibility.HasToggle(m_columnDef.Visibility);

		public string ToggleItem
		{
			get
			{
				if (m_columnDef.Visibility == null)
				{
					return null;
				}
				return m_columnDef.Visibility.Toggle;
			}
		}

		public TextBox ToggleParent
		{
			get
			{
				if (!HasToggle)
				{
					return null;
				}
				if (ColumnInstance == null)
				{
					return null;
				}
				return m_owner.RenderingContext.GetToggleParent(ColumnInstance.UniqueName);
			}
		}

		public SharedHiddenState SharedHidden => Visibility.GetSharedHidden(m_columnDef.Visibility);

		public bool IsToggleChild
		{
			get
			{
				if (ColumnInstance == null)
				{
					return false;
				}
				return m_owner.RenderingContext.IsToggleChild(ColumnInstance.UniqueName);
			}
		}

		internal TableColumnInstance ColumnInstance
		{
			get
			{
				if (m_columnInstance == null)
				{
					TableInstanceInfo tableInstanceInfo = (TableInstanceInfo)m_owner.InstanceInfo;
					if (tableInstanceInfo != null)
					{
						TableColumnInstance[] columnInstances = tableInstanceInfo.ColumnInstances;
						if (columnInstances != null)
						{
							m_columnInstance = columnInstances[m_index];
						}
					}
				}
				return m_columnInstance;
			}
		}

		public bool FixedHeader => m_columnDef.FixedHeader;

		internal Microsoft.ReportingServices.ReportProcessing.TableColumn ColumnDefinition => m_columnDef;

		internal TableColumn(Table owner, Microsoft.ReportingServices.ReportProcessing.TableColumn columnDef, int index)
		{
			m_owner = owner;
			m_columnDef = columnDef;
			m_index = index;
		}
	}
}
