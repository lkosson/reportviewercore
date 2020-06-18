using System;
using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ComparablePropertyDefinition<T> : PropertyDefinition<T>, IPropertyDefinition where T : struct, IComparable
	{
		private T? m_minimum;

		private T? m_maximum;

		public T? Minimum => m_minimum;

		public T? Maximum => m_maximum;

		object IPropertyDefinition.Default => base.Default;

		object IPropertyDefinition.Minimum => Minimum;

		object IPropertyDefinition.Maximum => Maximum;

		void IPropertyDefinition.Validate(object component, object value)
		{
			if (value is T)
			{
				Validate(component, (T)value);
				return;
			}
			if (value is ReportExpression<T>)
			{
				Validate(component, (ReportExpression<T>)value);
				return;
			}
			if (value is string)
			{
				Validate(component, (string)value);
				return;
			}
			throw new ArgumentException("Invalid type.");
		}

		public ComparablePropertyDefinition(string name, T? defaultValue)
			: base(name, defaultValue)
		{
		}

		public ComparablePropertyDefinition(string name, T? defaultValue, T? minimum, T? maximum)
			: this(name, defaultValue)
		{
			m_minimum = minimum;
			m_maximum = maximum;
		}

		public void Constrain(ref T value)
		{
			if (Minimum.HasValue && Minimum.Value.CompareTo(value) > 0)
			{
				value = Minimum.Value;
			}
			else if (Maximum.HasValue && Maximum.Value.CompareTo(value) < 0)
			{
				value = Maximum.Value;
			}
		}

		public void Validate(object component, T value)
		{
			if (Minimum.HasValue && Minimum.Value.CompareTo(value) > 0)
			{
				throw new ArgumentTooSmallException(component, base.Name, value, Minimum);
			}
			if (Maximum.HasValue && Maximum.Value.CompareTo(value) < 0)
			{
				throw new ArgumentTooLargeException(component, base.Name, value, Maximum);
			}
		}

		public void Validate(object component, ReportExpression<T> value)
		{
			if (!value.IsExpression)
			{
				Validate(component, value.Value);
			}
		}

		public void Validate(object component, string value)
		{
			Validate(component, new ReportExpression<T>(value, CultureInfo.InvariantCulture));
		}
	}
}
