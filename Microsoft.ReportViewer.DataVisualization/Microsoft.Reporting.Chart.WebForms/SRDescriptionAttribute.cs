using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class SRDescriptionAttribute : DescriptionAttribute
	{
		private bool replaced;

		public override string Description
		{
			get
			{
				if (!replaced)
				{
					replaced = true;
					base.DescriptionValue = SR.Keys.GetString(base.Description);
				}
				return base.Description;
			}
		}

		public SRDescriptionAttribute(string description)
			: base(description)
		{
		}
	}
}
