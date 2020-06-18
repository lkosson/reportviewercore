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
				new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
				return MirrorUtil.CreateMirrorParams(base.CreateParams, RightToLeft);
			}
		}

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
