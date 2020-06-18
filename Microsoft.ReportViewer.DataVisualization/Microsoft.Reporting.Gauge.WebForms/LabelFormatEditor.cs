using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LabelFormatEditor : UITypeEditor
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
					LabelFormatEditorForm labelFormatEditorForm = new LabelFormatEditorForm();
					labelFormatEditorForm.resultFormat = (string)value;
					edSvc.ShowDialog(labelFormatEditorForm);
					value = labelFormatEditorForm.resultFormat;
				}
			}
			return value;
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
