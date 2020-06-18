using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class StripLineTitleAngleConverter : Int32Converter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new ArrayList
			{
				0,
				90,
				180,
				270
			});
		}
	}
}
