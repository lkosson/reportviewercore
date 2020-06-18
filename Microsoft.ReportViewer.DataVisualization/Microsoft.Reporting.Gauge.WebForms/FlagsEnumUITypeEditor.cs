using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class FlagsEnumUITypeEditor : UITypeEditor
	{
		private Type enumType;

		private IWindowsFormsEditorService edSvc;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					if (value != null)
					{
						enumType = value.GetType();
					}
					else if (context != null && context.PropertyDescriptor != null)
					{
						enumType = context.PropertyDescriptor.PropertyType;
					}
					if (enumType != null)
					{
						FlagsEnumCheckedListBox flagsEnumCheckedListBox = new FlagsEnumCheckedListBox(value, enumType);
						edSvc.DropDownControl(flagsEnumCheckedListBox);
						value = flagsEnumCheckedListBox.GetNewValue();
					}
				}
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
}
