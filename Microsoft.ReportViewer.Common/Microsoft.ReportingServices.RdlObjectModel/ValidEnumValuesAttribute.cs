using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class ValidEnumValuesAttribute : Attribute
	{
		private IList<int> m_validValues;

		public IList<int> ValidValues
		{
			get
			{
				return m_validValues;
			}
			set
			{
				m_validValues = value;
			}
		}

		public ValidEnumValuesAttribute(string field)
			: this(typeof(InternalConstants), field)
		{
		}

		public ValidEnumValuesAttribute(Type type, string field)
		{
			int[] list = (int[])type.InvokeMember(field, BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
			m_validValues = new ReadOnlyCollection<int>(list);
		}
	}
}
