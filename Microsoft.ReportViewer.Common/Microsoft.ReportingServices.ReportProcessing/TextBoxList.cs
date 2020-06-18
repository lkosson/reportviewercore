using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class TextBoxList : ArrayList
	{
		internal new TextBox this[int index] => (TextBox)base[index];

		internal TextBoxList()
		{
		}

		internal TextBoxList(int capacity)
			: base(capacity)
		{
		}
	}
}
