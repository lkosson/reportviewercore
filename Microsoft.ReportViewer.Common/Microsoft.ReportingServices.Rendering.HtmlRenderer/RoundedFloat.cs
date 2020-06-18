namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class RoundedFloat
	{
		internal float m_value;

		internal float Value
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

		public RoundedFloat(float x)
		{
			m_value = x;
		}

		public static bool operator ==(RoundedFloat x1, float x2)
		{
			if ((double)x2 - 0.0001 <= (double)x1.m_value)
			{
				return (double)x2 + 0.0001 >= (double)x1.m_value;
			}
			return false;
		}

		public static bool operator >(RoundedFloat x1, float x2)
		{
			if (!(x1 == x2))
			{
				return (double)x1.m_value - 0.0001 > (double)x2;
			}
			return false;
		}

		public static bool operator >=(RoundedFloat x1, float x2)
		{
			if (!(x1 == x2))
			{
				return (double)x1.m_value - 0.0001 >= (double)x2;
			}
			return true;
		}

		public static bool operator <(RoundedFloat x1, float x2)
		{
			if (!(x1 == x2))
			{
				return (double)x1.m_value + 0.0001 < (double)x2;
			}
			return false;
		}

		public static bool operator <=(RoundedFloat x1, float x2)
		{
			if (!(x1 == x2))
			{
				return (double)x1.m_value + 0.0001 <= (double)x2;
			}
			return true;
		}

		public static bool operator !=(RoundedFloat x1, float x2)
		{
			return !(x1 == x2);
		}

		public static RoundedFloat operator +(RoundedFloat x1, float x2)
		{
			x1.m_value += x2;
			return x1;
		}

		public static RoundedFloat operator -(RoundedFloat x1, float x2)
		{
			x1.m_value -= x2;
			return x1;
		}

		public static explicit operator RoundedFloat(float x)
		{
			return new RoundedFloat(x);
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
