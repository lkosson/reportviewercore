using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal struct ReferenceID
	{
		private long m_value;

		private const long IsTemporaryMask = 4611686018427387904L;

		private const ulong HasMultiPartMask = 9223372036854775808uL;

		private const long PartitionIDMask = 4294967295L;

		public const long MinimumValidTempID = long.MinValue;

		public const long MaximumValidOffset = 72057594037927935L;

		public const int SizeInBytes = 16;

		internal long Value
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

		internal bool HasMultiPart
		{
			get
			{
				return m_value < 0;
			}
			set
			{
				ulong value2 = (ulong)m_value;
				value2 = (ulong)(m_value = ((!value) ? ((long)(value2 & long.MaxValue)) : ((long)value2 | long.MinValue)));
			}
		}

		internal bool IsTemporary
		{
			get
			{
				return (m_value & 0x4000000000000000L) != 0;
			}
			set
			{
				if (value)
				{
					m_value |= 4611686018427387904L;
				}
				else
				{
					m_value &= -4611686018427387905L;
				}
			}
		}

		internal int PartitionID
		{
			get
			{
				return (int)(m_value & uint.MaxValue);
			}
			set
			{
				long num = value;
				num &= uint.MaxValue;
				m_value &= -4294967296L;
				m_value |= num;
			}
		}

		internal ReferenceID(long value)
		{
			m_value = value;
		}

		internal ReferenceID(bool hasMultiPart, bool isTemporary, int partitionId)
		{
			m_value = 0L;
			HasMultiPart = hasMultiPart;
			IsTemporary = isTemporary;
			PartitionID = partitionId;
		}

		public static bool operator ==(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value == id2.m_value;
		}

		public static bool operator !=(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value != id2.m_value;
		}

		public static bool operator <(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value < id2.m_value;
		}

		public static bool operator >(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value > id2.m_value;
		}

		public static bool operator <=(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value <= id2.m_value;
		}

		public static bool operator >=(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value >= id2.m_value;
		}

		public override bool Equals(object obj)
		{
			return m_value == ((ReferenceID)obj).Value;
		}

		public override int GetHashCode()
		{
			return (int)m_value;
		}

		public override string ToString()
		{
			return m_value.ToString("x", CultureInfo.InvariantCulture);
		}
	}
}
