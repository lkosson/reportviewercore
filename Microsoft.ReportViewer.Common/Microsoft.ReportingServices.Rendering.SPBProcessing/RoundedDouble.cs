namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class RoundedDouble
	{
		internal double m_value;

		internal bool m_forOverlapDetection;

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
			: this(x, forOverlapDetection: false)
		{
		}

		public RoundedDouble(double x, bool forOverlapDetection)
		{
			m_value = x;
			m_forOverlapDetection = forOverlapDetection;
		}

		internal static double GetRoundingDelta(RoundedDouble x)
		{
			if (!x.m_forOverlapDetection)
			{
				return 0.01;
			}
			return 0.0001;
		}

		public static bool operator ==(RoundedDouble x1, double x2)
		{
			if (x2 - GetRoundingDelta(x1) <= x1.m_value)
			{
				return x2 + GetRoundingDelta(x1) >= x1.m_value;
			}
			return false;
		}

		public static bool operator >(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value - GetRoundingDelta(x1) > x2;
			}
			return false;
		}

		public static bool operator >=(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value - GetRoundingDelta(x1) >= x2;
			}
			return true;
		}

		public static bool operator <(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value + GetRoundingDelta(x1) < x2;
			}
			return false;
		}

		public static bool operator <=(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value + GetRoundingDelta(x1) <= x2;
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
			return new RoundedDouble(x, forOverlapDetection: false);
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
