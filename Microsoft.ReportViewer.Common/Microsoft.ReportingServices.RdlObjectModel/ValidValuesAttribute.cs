using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class ValidValuesAttribute : Attribute
	{
		private object m_minimum;

		private object m_maximum;

		public object Minimum
		{
			get
			{
				return m_minimum;
			}
			set
			{
				m_minimum = value;
			}
		}

		public object Maximum
		{
			get
			{
				return m_maximum;
			}
			set
			{
				m_maximum = value;
			}
		}

		public ValidValuesAttribute(int minimum, int maximum)
		{
			m_minimum = minimum;
			m_maximum = maximum;
		}

		public ValidValuesAttribute(double minimum, double maximum)
		{
			m_minimum = minimum;
			m_maximum = maximum;
		}

		public ValidValuesAttribute(string minimumField, string maximumField)
		{
			m_minimum = typeof(Constants).InvokeMember(minimumField, BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
			m_maximum = typeof(Constants).InvokeMember(maximumField, BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
		}
	}
}
