namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class RoundedDouble
	{
		internal double m_value;

		internal double Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		public RoundedDouble(double x)
		{
			m_value = x;
		}

		public static bool operator ==(RoundedDouble x1, double x2)
		{
			if (x2 - 0.01 <= x1.m_value)
			{
				return x2 + 0.01 >= x1.m_value;
			}
			return false;
		}

		public static bool operator >(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value - 0.01 > x2;
			}
			return false;
		}

		public static bool operator >=(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value - 0.01 >= x2;
			}
			return true;
		}

		public static bool operator <(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value + 0.01 < x2;
			}
			return false;
		}

		public static bool operator <=(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value + 0.01 <= x2;
			}
			return true;
		}

		public static bool operator !=(RoundedDouble x1, double x2)
		{
			return !(x1 == x2);
		}

		public static RoundedDouble operator +(RoundedDouble x1, double x2)
		{
			x1.m_value += x2;
			return x1;
		}

		public static RoundedDouble operator -(RoundedDouble x1, double x2)
		{
			x1.m_value -= x2;
			return x1;
		}

		public static explicit operator RoundedDouble(double x)
		{
			return new RoundedDouble(x);
		}

		public override bool Equals(object x1)
		{
			return (object)m_value == x1;
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}
	}
}
