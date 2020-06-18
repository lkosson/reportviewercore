namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class SequenceIndex
	{
		internal static byte BitMask001 = 1;

		internal static byte BitMask255 = byte.MaxValue;

		internal static void SetBit(ref byte[] sequence, int sequenceIndex)
		{
			byte b = (byte)(BitMask001 << sequenceIndex % 8);
			sequence[sequenceIndex >> 3] |= b;
		}

		internal static void ClearBit(ref byte[] sequence, int sequenceIndex)
		{
			byte b = (byte)(BitMask001 << sequenceIndex % 8);
			b = (byte)(b ^ BitMask255);
			sequence[sequenceIndex >> 3] &= b;
		}

		internal static bool GetBit(byte[] sequence, int sequenceIndex, bool returnValueIfSequenceNull)
		{
			if (sequence == null)
			{
				return returnValueIfSequenceNull;
			}
			byte b = (byte)(BitMask001 << sequenceIndex % 8);
			return (sequence[sequenceIndex >> 3] & b) > 0;
		}
	}
}
