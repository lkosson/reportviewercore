using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class MirrorPanel : Panel
	{
		protected override CreateParams CreateParams
		{
			get
			{
				return MirrorUtil.CreateMirrorParams(base.CreateParams, RightToLeft);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				if (value != base.RightToLeft)
				{
					base.RightToLeft = value;
					CreateHandle();
				}
			}
		}
	}
}
