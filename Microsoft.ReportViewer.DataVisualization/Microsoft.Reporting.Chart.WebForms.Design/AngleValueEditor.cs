using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms.Design;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AngleValueEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc;

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					AngleTrackForm angleTrackForm = new AngleTrackForm();
					angleTrackForm.ValueChanged += ValueChanged;
					bool flag = true;
					if (value is int)
					{
						angleTrackForm.Angle = (int)value;
					}
					else if (value is byte)
					{
						flag = false;
						angleTrackForm.Angle = (byte)value;
					}
					edSvc.DropDownControl(angleTrackForm);
					value = ((!flag) ? ((object)(byte)angleTrackForm.Angle) : ((object)angleTrackForm.Angle));
				}
			}
			return value;
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			if (edSvc != null)
			{
				edSvc.CloseDropDown();
			}
		}
	}
}
