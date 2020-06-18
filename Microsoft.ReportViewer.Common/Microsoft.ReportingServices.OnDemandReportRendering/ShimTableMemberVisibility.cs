using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableMemberVisibility : ShimMemberVisibility
	{
		internal enum Mode
		{
			StaticColumn,
			StaticRow,
			TableGroup,
			TableDetails
		}

		private ShimTableMember m_owner;

		private Mode m_mode;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (m_startHidden == null)
				{
					m_startHidden = Visibility.GetStartHidden(GetVisibilityDefinition());
				}
				return m_startHidden;
			}
		}

		public override string ToggleItem => GetVisibilityDefinition()?.Toggle;

		public override bool RecursiveToggleReceiver => GetVisibilityDefinition()?.RecursiveReceiver ?? false;

		public override SharedHiddenState HiddenState => Visibility.GetHiddenState(GetVisibilityDefinition());

		public ShimTableMemberVisibility(ShimTableMember owner, Mode mode)
		{
			m_owner = owner;
			m_mode = mode;
		}

		private Microsoft.ReportingServices.ReportProcessing.Visibility GetVisibilityDefinition()
		{
			switch (m_mode)
			{
			case Mode.StaticColumn:
				return m_owner.RenderTableColumn.ColumnDefinition.Visibility;
			case Mode.StaticRow:
				return m_owner.RenderTableRow.m_rowDef.Visibility;
			case Mode.TableDetails:
				return m_owner.RenderTableDetails.DetailDefinition.Visibility;
			case Mode.TableGroup:
				return m_owner.RenderTableGroup.m_visibilityDef;
			default:
				return null;
			}
		}

		internal override bool GetInstanceHidden()
		{
			switch (m_mode)
			{
			case Mode.StaticColumn:
				return m_owner.RenderTableColumn.Hidden;
			case Mode.StaticRow:
				return m_owner.RenderTableRow.Hidden;
			case Mode.TableDetails:
				return m_owner.RenderTableDetails[m_owner.Group.CurrentRenderGroupIndex].Hidden;
			case Mode.TableGroup:
				return m_owner.RenderTableGroup.Hidden;
			default:
				return false;
			}
		}

		internal override bool GetInstanceStartHidden()
		{
			switch (m_mode)
			{
			case Mode.StaticColumn:
				if (m_owner.RenderTableColumn.ColumnInstance == null)
				{
					return GetInstanceHidden();
				}
				return m_owner.RenderTableColumn.ColumnInstance.StartHidden;
			case Mode.StaticRow:
				if (m_owner.RenderTableRow.InstanceInfo == null)
				{
					return GetInstanceHidden();
				}
				return m_owner.RenderTableRow.InstanceInfo.StartHidden;
			case Mode.TableDetails:
			{
				TableDetailRowCollection tableDetailRowCollection = m_owner.RenderTableDetails[m_owner.Group.CurrentRenderGroupIndex];
				if (tableDetailRowCollection.InstanceInfo != null)
				{
					return tableDetailRowCollection.InstanceInfo.StartHidden;
				}
				return GetInstanceHidden();
			}
			case Mode.TableGroup:
				if (m_owner.RenderTableGroup.InstanceInfo == null)
				{
					return GetInstanceHidden();
				}
				return m_owner.RenderTableGroup.InstanceInfo.StartHidden;
			default:
				return false;
			}
		}
	}
}
