using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableDetailRow : TableRow
	{
		private TableDetailRowCollection m_detail;

		public override bool Hidden
		{
			get
			{
				if (m_detail.Hidden)
				{
					return true;
				}
				return base.Hidden;
			}
		}

		public override TextBox ToggleParent
		{
			get
			{
				TextBox textBox = null;
				if (m_detail.DetailInstance != null)
				{
					textBox = m_owner.RenderingContext.GetToggleParent(m_detail.DetailInstance.UniqueName);
				}
				if (textBox == null)
				{
					return base.ToggleParent;
				}
				return textBox;
			}
		}

		public override bool HasToggle
		{
			get
			{
				if (Visibility.HasToggle(((Microsoft.ReportingServices.ReportProcessing.Table)m_owner.ReportItemDef).TableDetail.Visibility))
				{
					return true;
				}
				return base.HasToggle;
			}
		}

		public override string ToggleItem
		{
			get
			{
				TableDetail tableDetail = ((Microsoft.ReportingServices.ReportProcessing.Table)m_owner.ReportItemDef).TableDetail;
				string text = null;
				if (tableDetail.Visibility != null)
				{
					text = tableDetail.Visibility.Toggle;
				}
				if (text == null)
				{
					text = base.ToggleItem;
				}
				return text;
			}
		}

		public override SharedHiddenState SharedHidden
		{
			get
			{
				SharedHiddenState sharedHidden = Visibility.GetSharedHidden(((Microsoft.ReportingServices.ReportProcessing.Table)m_owner.ReportItemDef).TableDetail.Visibility);
				if (sharedHidden == SharedHiddenState.Always)
				{
					return SharedHiddenState.Always;
				}
				SharedHiddenState sharedHidden2 = base.SharedHidden;
				if (SharedHiddenState.Never == sharedHidden)
				{
					return sharedHidden2;
				}
				if (sharedHidden2 == SharedHiddenState.Always)
				{
					return SharedHiddenState.Always;
				}
				return SharedHiddenState.Sometimes;
			}
		}

		public override bool IsToggleChild
		{
			get
			{
				bool flag = false;
				if (m_detail.DetailInstance != null)
				{
					flag = m_owner.RenderingContext.IsToggleChild(m_detail.DetailInstance.UniqueName);
				}
				if (flag)
				{
					return true;
				}
				return base.IsToggleChild;
			}
		}

		internal TableDetailRow(Table owner, Microsoft.ReportingServices.ReportProcessing.TableRow rowDef, TableRowInstance rowInstance, TableDetailRowCollection detail)
			: base(owner, rowDef, rowInstance)
		{
			m_detail = detail;
		}
	}
}
