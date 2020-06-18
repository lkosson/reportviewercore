using System;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FlagsEnumCheckedListBox : CheckedListBox
	{
		private object editValue;

		private Type editType;

		public FlagsEnumCheckedListBox(object editValue, Type editType)
		{
			this.editValue = editValue;
			this.editType = editType;
			base.BorderStyle = BorderStyle.None;
			FillList();
		}

		private void FillList()
		{
			if (!editType.IsEnum)
			{
				throw new ArgumentException("UI type editor may be set for the enumerations only.");
			}
			if (Enum.GetUnderlyingType(editType) != typeof(int))
			{
				throw new ArgumentException("UI type editor may be set for the enumerations with Int32 underlying type only.");
			}
			int num = 0;
			if (editValue != null)
			{
				num = (int)editValue;
			}
			foreach (object value in Enum.GetValues(editType))
			{
				int num2 = (int)value;
				if (num2 != 0)
				{
					bool isChecked = (num & num2) == num2;
					base.Items.Add(Enum.GetName(editType, value), isChecked);
				}
			}
		}

		public object GetNewValue()
		{
			int num = 0;
			foreach (object checkedItem in base.CheckedItems)
			{
				int num2 = (int)Enum.Parse(editType, (string)checkedItem);
				num |= num2;
			}
			return Enum.ToObject(editType, num);
		}
	}
}
