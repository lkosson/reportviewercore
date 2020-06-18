using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal class TableRow
	{
		internal Table m_owner;

		internal Microsoft.ReportingServices.ReportProcessing.TableRow m_rowDef;

		internal TableRowInstance m_rowInstance;

		internal TableCellCollection m_rowCells;

		internal TableRowInstanceInfo m_tableRowInstanceInfo;

		public string ID
		{
			get
			{
				if (m_rowDef.RenderingModelID == null)
				{
					m_rowDef.RenderingModelID = m_rowDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return m_rowDef.RenderingModelID;
			}
		}

		public string UniqueName
		{
			get
			{
				if (m_rowInstance == null)
				{
					return null;
				}
				return m_rowInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				return m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[m_rowDef.ID];
			}
			set
			{
				m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[m_rowDef.ID] = value;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (m_rowDef.HeightForRendering == null)
				{
					m_rowDef.HeightForRendering = new ReportSize(m_rowDef.Height, m_rowDef.HeightValue);
				}
				return m_rowDef.HeightForRendering;
			}
		}

		public TableCellCollection TableCellCollection
		{
			get
			{
				TableCellCollection tableCellCollection = m_rowCells;
				if (m_rowCells == null)
				{
					tableCellCollection = new TableCellCollection(m_owner, m_rowDef, m_rowInstance);
					if (m_owner.RenderingContext.CacheState)
					{
						m_rowCells = tableCellCollection;
					}
				}
				return tableCellCollection;
			}
		}

		public virtual bool Hidden
		{
			get
			{
				if (m_rowInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(m_rowDef.Visibility);
				}
				if (m_rowDef.Visibility == null)
				{
					return false;
				}
				if (m_rowDef.Visibility.Toggle != null)
				{
					return m_owner.RenderingContext.IsItemHidden(m_rowInstance.UniqueName, potentialSender: false);
				}
				return InstanceInfo.StartHidden;
			}
		}

		public virtual bool HasToggle => Visibility.HasToggle(m_rowDef.Visibility);

		public virtual string ToggleItem
		{
			get
			{
				if (m_rowDef.Visibility == null)
				{
					return null;
				}
				return m_rowDef.Visibility.Toggle;
			}
		}

		public virtual TextBox ToggleParent
		{
			get
			{
				if (!HasToggle)
				{
					return null;
				}
				if (m_rowInstance == null)
				{
					return null;
				}
				return m_owner.RenderingContext.GetToggleParent(m_rowInstance.UniqueName);
			}
		}

		public virtual SharedHiddenState SharedHidden => Visibility.GetSharedHidden(m_rowDef.Visibility);

		public virtual bool IsToggleChild
		{
			get
			{
				if (m_rowInstance == null)
				{
					return false;
				}
				return m_owner.RenderingContext.IsToggleChild(m_rowInstance.UniqueName);
			}
		}

		internal TableRowInstanceInfo InstanceInfo
		{
			get
			{
				if (m_rowInstance == null)
				{
					return null;
				}
				if (m_tableRowInstanceInfo == null)
				{
					m_tableRowInstanceInfo = m_rowInstance.GetInstanceInfo(m_owner.RenderingContext.ChunkManager);
				}
				return m_tableRowInstanceInfo;
			}
		}

		internal TableRow(Table owner, Microsoft.ReportingServices.ReportProcessing.TableRow rowDef, TableRowInstance rowInstance)
		{
			m_owner = owner;
			m_rowDef = rowDef;
			m_rowInstance = rowInstance;
		}
	}
}
